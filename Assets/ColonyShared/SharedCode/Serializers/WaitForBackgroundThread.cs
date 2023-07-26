using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace ShareCode.Threading
{
    public static class Awaiters
    {
        public static readonly ThreadPoolRedirector ThreadPool = default;
        public static readonly ThreadPoolRedirectorForceYield ThreadPoolYield = default;
    }

    public readonly struct ThreadPoolRedirector : ICriticalNotifyCompletion
    {
        public ThreadPoolRedirector GetAwaiter() => default;

        // true означает выполнять продолжение немедленно 
        public bool IsCompleted => Thread.CurrentThread.IsThreadPoolThread;

        public void OnCompleted(Action continuation) => ThreadPool.QueueUserWorkItem(_callback, continuation);
        public void UnsafeOnCompleted(Action continuation) => ThreadPool.UnsafeQueueUserWorkItem(_callback, continuation);

        private static readonly WaitCallback _callback = (obj) => ((Action)obj)();

        public void GetResult() { }
    }

    public readonly struct ThreadPoolRedirectorForceYield : ICriticalNotifyCompletion
    {
        public ThreadPoolRedirectorForceYield GetAwaiter() => default;

        // true означает выполнять продолжение немедленно 
        public bool IsCompleted => false;

        public void OnCompleted(Action continuation) => ThreadPool.QueueUserWorkItem(_callback, continuation);
        public void UnsafeOnCompleted(Action continuation) => ThreadPool.UnsafeQueueUserWorkItem(_callback, continuation);

        private static readonly WaitCallback _callback = (obj) => ((Action)obj)();

        public void GetResult() { }
    }
}