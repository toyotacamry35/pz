#if UNITY_EDITOR
#define ENABLE_SPIN_LOCK_DEBUG
#endif
using System;
using Core.Environment.Logging.Extension;
using NLog;

namespace ReactivePropsNs.ThreadSafe
{
    internal static class SpinLockExt
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        
        private const int Timeout = 1000;
        
#if ENABLE_SPIN_LOCK_DEBUG        
        public static System.Threading.SpinLock Create() => new System.Threading.SpinLock(true);
#else
        public static System.Threading.SpinLock Create() => new System.Threading.SpinLock();
#endif
        
        public static void EnterWithTimeout(ref this System.Threading.SpinLock @lock, ref bool lockTaken)
        {
            @lock.TryEnter(Timeout, ref lockTaken);
            if (!lockTaken)
            {
                Logger.IfError()?.Message("Can't take lock!").Write();
                throw new TimeoutException("Can't take lock!");
            }
        }
    }
}