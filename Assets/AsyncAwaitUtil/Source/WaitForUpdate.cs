using System;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityAsyncAwaitUtil;

namespace UnityThreading
{
    public static class Awaiters
    {
        public static readonly WaitForUpdate UnityThread = default;
    }

    // This can be used as a way to return to the main unity thread when using multiple threads
    // with async methods
    public readonly struct WaitForUpdate : ICriticalNotifyCompletion
    {
        public WaitForUpdate GetAwaiter() => default;

        // true означает выполнять продолжение немедленно 
        public bool IsCompleted => SyncContextUtil.UnitySynchronizationContextReplaced == SynchronizationContext.Current;

        public void OnCompleted(Action continuation) => SyncContextUtil.UnitySynchronizationContextReplaced.Post(_callback, continuation);
        public void UnsafeOnCompleted(Action continuation) => SyncContextUtil.UnitySynchronizationContextReplaced.PostUnsafe(_callback, continuation);

        private static readonly SendOrPostCallback _callback = (obj) => ((Action)obj)();

        public void GetResult() { }
    }
}