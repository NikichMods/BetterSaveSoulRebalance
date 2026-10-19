using System;
using System.Collections.Generic;
using SoulDLCRebalance;

class PaymentTests
{
    private struct Flow { public int Value; }
    private delegate void Handler(Flow flow);
    private static int checks;
    private static void Check(bool value, string why)
    { checks++; if (!value) throw new Exception(why); }

    private static void Main()
    {
        int money = EuricPaymentPlan.FirstPayment, paid = 1;
        for (int stage = 1; stage <= 3; stage++)
        {
            Check(EuricPaymentPlan.IsDue(paid, 1 << stage, stage), "new milestone owed");
            money += EuricPaymentPlan.Amount(stage); paid |= 1 << stage;
            Check(!EuricPaymentPlan.IsDue(paid, 1 << stage, stage), "repeat milestone paid twice");
            Check(EuricPaymentPlan.ReadMask((float)paid) == paid, "reload mask roundtrip");
        }
        Check(money == 110 && paid == 15, "approved total");
        for (int mask = 0; mask < 16; mask++)
        {
            if (mask != 0 && (mask & 1) == 0) continue;
            for (int earned = 0; earned < 16; earned++)
            {
                int replay = 0, caughtUp = mask;
                for (int stage = 1; stage <= 3; stage++)
                    if (EuricPaymentPlan.IsDue(caughtUp, earned, stage))
                    { replay += EuricPaymentPlan.Amount(stage); caughtUp |= 1 << stage; }
                for (int stage = 1; stage <= 3; stage++)
                    Check(!EuricPaymentPlan.IsDue(caughtUp, earned, stage), "catch-up replay duplicate");
                if (mask == 0) Check(replay == 0 && caughtUp == 0, "legacy save received new money");
            }
        }
        foreach (float invalid in new[] { -1f, 2f, 16f, 1.5f, float.NaN, float.PositiveInfinity })
        {
            bool rejected = false;
            try { EuricPaymentPlan.ReadMask(invalid); } catch (InvalidOperationException) { rejected = true; }
            Check(rejected, "invalid saved mask accepted");
        }
        var order = new List<string>();
        Handler original = flow => { Check(flow.Value == 42, "native flow argument lost"); order.Add("native"); };
        var callback = NativeContinuation.CreateCallback(original, "milestone", flow => order.Add("reward"));
        var wrapped = (Handler)Delegate.Combine(original, callback);
        wrapped(new Flow { Value = 42 });
        Check(string.Join(",", order) == "native,reward", "native continuation order changed");
        Check(NativeContinuation.IsOwned(wrapped, "milestone"), "multicast callback not recognized");
        Check(!NativeContinuation.IsOwned(original, "milestone"), "foreign delegate claimed");
        Exception expected = new InvalidOperationException("native failure");
        Handler failing = flow => { throw expected; };
        try { NativeContinuation.Call(failing, new Flow()); throw new Exception("native error lost"); }
        catch (InvalidOperationException actual) { Check(object.ReferenceEquals(actual, expected), "native exception replaced"); }
        Console.WriteLine("PASS payment policy and native continuations: " + checks + " assertions");
    }
}
