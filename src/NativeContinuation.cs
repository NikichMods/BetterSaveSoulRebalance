using System;
using System.Reflection;
using System.Runtime.ExceptionServices;

namespace SoulDLCRebalance
{
    internal interface INativeContinuation { string Tag { get; } }

    // Preserve the real FlowHandler's value-type argument without referencing host binaries.
    internal sealed class NativeContinuation<T> : INativeContinuation
    {
        public string Tag { get; private set; }
        private readonly Action<object> handler;

        public NativeContinuation(string tag, Action<object> handler)
        { Tag = tag; this.handler = handler; }

        public void Invoke(T flow) { handler(flow); }
    }

    internal static class NativeContinuation
    {
        internal static bool IsOwned(Delegate value, string tag)
        {
            if (value == null) return false;
            foreach (Delegate callback in value.GetInvocationList())
            { var owned = callback.Target as INativeContinuation; if (owned != null && owned.Tag == tag) return true; }
            return false;
        }

        internal static Delegate CreateCallback(Delegate original, string tag, Action<object> handler)
        {
            if (original == null) throw new InvalidOperationException("Missing native continuation: " + tag);
            MethodInfo invoke = original.GetType().GetMethod("Invoke");
            ParameterInfo[] args = invoke.GetParameters();
            if (invoke.ReturnType != typeof(void) || args.Length != 1 || args[0].ParameterType.IsByRef)
                throw new InvalidOperationException("Unsupported native continuation: " + tag);
            Type wrapperType = typeof(NativeContinuation<>).MakeGenericType(args[0].ParameterType);
            object wrapper = Activator.CreateInstance(wrapperType, tag, handler);
            return Delegate.CreateDelegate(original.GetType(), wrapper, wrapperType.GetMethod("Invoke"));
        }

        internal static void Call(Delegate original, object flow)
        {
            try { original.DynamicInvoke(flow); }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException == null) throw;
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw;
            }
        }
    }
}
