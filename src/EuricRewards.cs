using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace SoulDLCRebalance
{
    internal static class EuricRewards
    {
        private static readonly string[] Graphs = { "npc_euric", "on_enter_ev_s_s25_gd_zone", "on_enter_ev_s_s34_gd_zone" };
        // GetGraph prepends FlowCanvas/; the NPC asset is in its WGO scripts subfolder.
        private static readonly string[] ResourcePaths = { "WGO scripts/npc_euric", "on_enter_ev_s_s25_gd_zone", "on_enter_ev_s_s34_gd_zone" };
        private static readonly string[] Texts = { "souls_s_s16_1_10", "souls_s_s25_14", "souls_s_s34_9" };
        private static readonly string[] Tasks = { "dlc_souls_s16_1", "dlc_souls_s25", "dlc_souls_s34_w8_day" };
        private static readonly List<WeakReference> InitialValuePorts = new List<WeakReference>();
        private static MethodInfo GetInput, GetOutput, GatherPorts, GetGraph, GetItemParam, SetItemParam, Append;
        private static MethodInfo GetNpc, GetQuestState;
        private static Type MoneyNode;
        private static readonly Delegate[] Pay = new Delegate[3];
        private static Action<string> Info, Failure;
        private static bool Enabled;

        internal static bool Install(Action<string> info, Action<string> failure)
        {
            Info = info; Failure = failure;
            try
            {
                Type flowNode = RequireType("FlowCanvas.FlowNode");
                GetInput = RequireMethod(flowNode, "GetInputPort", false, typeof(string));
                GetOutput = RequireMethod(flowNode, "GetOutputPort", false, typeof(string));
                GatherPorts = RequireMethod(flowNode, "GatherPorts", false);
                GetGraph = RequireMethod(R.GameType("CustomFlowScript"), "GetGraph", true, typeof(string));
                MoneyNode = RequireType("FlowCanvas.Nodes.Flow_AddMoney");
                Append = RequireMethod(RequireType("FlowCanvas.FlowOutput"), "Append", false, RequireType("FlowCanvas.FlowHandler"));
                GetItemParam = RequireMethod(R.GameType("Item"), "GetParam", false, typeof(string), typeof(float));
                SetItemParam = RequireMethod(R.GameType("Item"), "SetParam", false, typeof(string), typeof(float));
                GetNpc = RequireMethod(R.GameType("KnownNPCList"), "GetNPC", false, typeof(string));
                GetQuestState = RequireMethod(R.GameType("KnownNPC"), "GetQuestState", false, typeof(string));
                for (int i = 0; i < Graphs.Length; i++)
                {
                    object graph = GetGraph.Invoke(null, new object[] { ResourcePaths[i] });
                    ValidateGraph(graph, i, false);
                    object money = Activator.CreateInstance(MoneyNode);
                    GatherPorts.Invoke(money, null);
                    SetValue(Input(money, "Value"), (float)EuricPaymentPlan.Amount(i + 1));
                    Pay[i] = Pointer(Input(money, "In"));
                    if (Pay[i] == null) throw new InvalidOperationException("Native payout action unavailable");
                }
                R.Patch(SoulDLCRebalancePlugin.PluginGuid + ".euric", typeof(EuricRewards),
                    RequireMethod(RequireType("FlowCanvas.FlowGraph"), "OnGraphStarted", false), null, "GraphStarted", null);
                Enabled = true;
                Info("BSSR_EURIC_READY schedule=10/20/30/50 total=110");
                return true;
            }
            catch (Exception ex) { Disable("initialization", ex); return false; }
        }

        private static void GraphStarted(object __instance)
        {
            if (!Enabled) return;
            string name = R.Get(__instance, "name") as string;
            if (name != null && name.EndsWith("(Clone)", StringComparison.Ordinal)) name = name.Substring(0, name.Length - 7);
            int index = Array.IndexOf(Graphs, name);
            if (index < 0) return;
            var undo = new List<Action>();
            try
            {
                object talk = ValidateGraph(__instance, index, true);
                object output = Output(talk, "On Finished");
                int stage = index + 1;
                Attach(output, "treatment-" + stage, flow => TryPayment(() => Settle(flow, 1 << stage)), undo);
                if (index == 0)
                {
                    object money = FindMoney(__instance);
                    object value = Input(money, "Value"), moneyOut = Output(money, "Out");
                    float oldValue = Convert.ToSingle(R.Get(value, "serializedValue"), CultureInfo.InvariantCulture);
                    if (oldValue == 100f)
                    {
                        SetValue(value, 10f); undo.Add(() => SetValue(value, oldValue));
                        InitialValuePorts.RemoveAll(weak => !weak.IsAlive);
                        InitialValuePorts.Add(new WeakReference(value));
                    }
                    Attach(moneyOut, "enroll", flow =>
                    {
                        TryPayment(() =>
                        {
                            object item = PlayerItem();
                            if (ReadPaid(item) == 0)
                            {
                                WritePaid(item, 1);
                                Info("BSSR_EURIC_PAYMENT stage=room silver=10");
                            }
                            Settle(flow, 0);
                        });
                    }, undo);
                }
                // Replay only earned unpaid stages on a normal resumed graph; legacy saves have mask zero.
                if (R.Player() != null) TryPayment(() => Settle(Activator.CreateInstance(Pay[0].GetType().GetMethod("Invoke").GetParameters()[0].ParameterType), 0));
            }
            catch (Exception ex)
            {
                for (int i = undo.Count - 1; i >= 0; i--) try { undo[i](); } catch { }
                Disable("graph=" + name, ex);
            }
        }

        private static object ValidateGraph(object graph, int index, bool bound)
        {
            if (graph == null) throw new InvalidOperationException("Missing native graph: " + Graphs[index]);
            object[] talks = Nodes(graph).Where(n => n.GetType().FullName == "FlowCanvas.Nodes.Flow_Talk" &&
                (R.Get(Input(n, "Text"), "serializedValue") as string) == Texts[index]).ToArray();
            if (talks.Length != 1) throw new InvalidOperationException("Treatment anchor mismatch: " + Graphs[index]);
            object talk = talks[0];
            if (Convert.ToInt32(R.Get(Input(talk, "Text"), "connections")) != 0 ||
                Convert.ToInt32(R.Get(Output(talk, "On Finished"), "connections")) != 1)
                throw new InvalidOperationException("Treatment port shape mismatch");
            if (bound && Pointer(Output(talk, "On Finished")) == null) throw new InvalidOperationException("Unbound treatment continuation");
            if (index == 0)
            {
                object money = FindMoney(graph), value = Input(money, "Value");
                float amount = Convert.ToSingle(R.Get(value, "serializedValue"), CultureInfo.InvariantCulture);
                bool owned = NativeContinuation.IsOwned(Pointer(Output(money, "Out")), "enroll");
                if ((amount != 100f && !(amount == 10f && owned)) || Convert.ToInt32(R.Get(value, "connections")) != 0 ||
                    Convert.ToInt32(R.Get(Input(money, "In"), "connections")) != 1 ||
                    Convert.ToInt32(R.Get(Output(money, "Out"), "connections")) != 1)
                    throw new InvalidOperationException("Room reward baseline mismatch");
                if (bound && Pointer(Output(money, "Out")) == null) throw new InvalidOperationException("Unbound room continuation");
            }
            return talk;
        }

        private static object FindMoney(object graph)
        {
            object[] matches = Nodes(graph).Where(n => n.GetType() == MoneyNode).ToArray();
            if (matches.Length != 1) throw new InvalidOperationException("Room money node mismatch");
            return matches[0];
        }

        private static void Settle(object flow, int currentStage)
        {
            if (!Enabled) return;
            object item = PlayerItem(); int paid = ReadPaid(item);
            if (paid == 0) return;
            int earned = currentStage | EarnedTasks();
            for (int stage = 1; stage <= 3; stage++)
            {
                if (!EuricPaymentPlan.IsDue(paid, earned, stage)) continue;
                float before = ReadParam(item, "money");
                Exception notificationError = null;
                try { NativeContinuation.Call(Pay[stage - 1], flow); }
                catch (Exception ex) { notificationError = ex; }
                float after = ReadParam(item, "money");
                if (Math.Abs(after - before - EuricPaymentPlan.Amount(stage)) > .001f)
                    throw new InvalidOperationException("Native payout did not credit the expected amount", notificationError);
                paid |= 1 << stage; WritePaid(item, paid);
                Info("BSSR_EURIC_PAYMENT stage=" + stage + " silver=" + EuricPaymentPlan.Amount(stage));
                if (notificationError != null) throw new InvalidOperationException("Payment credited and recorded; native notification failed", notificationError);
            }
        }

        private static int EarnedTasks()
        {
            object save = R.Get(R.GetStatic(R.GameType("MainGame"), "me"), "save");
            object known = R.Get(save, "known_npcs");
            if (known == null) throw new InvalidOperationException("Quest state unavailable");
            object npc = GetNpc.Invoke(known, new object[] { "npc_euric" });
            int earned = 0;
            if (npc == null) return earned;
            for (int i = 0; i < Tasks.Length; i++)
            {
                string state = GetQuestState.Invoke(npc, new object[] { Tasks[i] }).ToString();
                if (state == "Visible" || state == "Complete") earned |= 1 << (i + 1);
            }
            return earned;
        }

        private static void TryPayment(Action action)
        { if (Enabled) try { action(); } catch (Exception ex) { Disable("payment", ex); } }

        private static void Disable(string context, Exception ex)
        {
            Enabled = false;
            foreach (WeakReference weak in InitialValuePorts)
                try { if (weak.IsAlive) SetValue(weak.Target, 100f); } catch { }
            InitialValuePorts.Clear();
            if (Failure != null) Failure("BSSR_EURIC_DISABLED " + context + " action=stock-unclaimed-reward installments=stopped " + ex);
        }

        private static object PlayerItem()
        { object item = R.Get(R.Player(), "data"); if (item == null) throw new InvalidOperationException("Player data unavailable"); return item; }
        private static float ReadParam(object item, string key)
        { return Convert.ToSingle(GetItemParam.Invoke(item, new object[] { key, 0f }), CultureInfo.InvariantCulture); }
        private static int ReadPaid(object item) { return EuricPaymentPlan.ReadMask(ReadParam(item, EuricPaymentPlan.PaidKey)); }
        private static void WritePaid(object item, int mask) { SetItemParam.Invoke(item, new object[] { EuricPaymentPlan.PaidKey, (float)mask }); }
        private static IEnumerable<object> Nodes(object graph) { return ((IEnumerable)R.Get(graph, "allNodes")).Cast<object>(); }
        private static object Input(object node, string id) { return GetInput.Invoke(node, new object[] { id }); }
        private static object Output(object node, string id) { return GetOutput.Invoke(node, new object[] { id }); }
        private static Delegate Pointer(object port) { return R.Get(port, "pointer") as Delegate; }
        private static void SetValue(object port, float value) { R.Set(port, "serializedValue", value, true); }
        private static void Attach(object port, string tag, Action<object> handler, List<Action> undo)
        {
            Delegate original = Pointer(port);
            if (NativeContinuation.IsOwned(original, tag)) return;
            Delegate callback = NativeContinuation.CreateCallback(original, tag, handler);
            Append.Invoke(port, new object[] { callback });
            undo.Add(() => R.Set(port, "pointer", original, true));
        }
        private static Type RequireType(string name)
        { return R.AnyType(name) ?? throw new TypeLoadException(name); }
        private static MethodInfo RequireMethod(Type type, string name, bool isStatic, params Type[] args)
        { return R.Method(type, name, isStatic, args) ?? throw new MissingMethodException(name); }
    }
}
