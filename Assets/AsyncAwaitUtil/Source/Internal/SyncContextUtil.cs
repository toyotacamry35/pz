using System.Threading;
using UnityEngine;

namespace UnityAsyncAwaitUtil
{
    public static class SyncContextUtil
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Install()
        {
            UnityThreadId = Thread.CurrentThread.ManagedThreadId;
        }

        internal static int UnityThreadId { get; set; }

        internal static UnitySynchronizationContext2 UnitySynchronizationContextReplaced
        {
            get; set;
        }

        public static bool IsInUnity => UnityThreadId == Thread.CurrentThread.ManagedThreadId;
    }
}

