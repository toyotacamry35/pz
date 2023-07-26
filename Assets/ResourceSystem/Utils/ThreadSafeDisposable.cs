using System;
using System.Threading;

namespace ColonyShared.SharedCode.Utils
{
    public abstract class ThreadSafeDisposable : IDisposable
    {
        private SpinLock _lock = new SpinLock();
        private int _usageCounter;
        private bool _disposeRequired;
        private bool _disposed;

        protected abstract void DisposeImpl();

        public void Dispose()
        {
            bool locked = false;
            try
            {
                _lock.Enter(ref locked);
               
                if (_disposed)
                    return;

                if (_usageCounter > 0)
                {
                    _disposeRequired = true;
                    return;
                }
                
                _disposed = true;
            }
            finally
            {
                if(locked) _lock.Exit();
            }
            DisposeImpl();
            GC.SuppressFinalize(this);
        }

        protected bool EnterIfNotDisposed(bool quiet = true)
        {
            bool locked = false;
            try
            {
                _lock.Enter(ref locked);
                if (_disposed)
                {
                    if(!quiet) throw new ObjectDisposedException($"{this}");
                    return false;
                }
                ++_usageCounter;
            }
            finally
            {
                if(locked) _lock.Exit();
            }
            return true;
        }

        protected void ExitAndDisposeIfRequested()
        {
            bool locked = false;
            try
            {
                _lock.Enter(ref locked);
                
                if (_disposed)
                    return;
                
                if (--_usageCounter > 0 || !_disposeRequired)
                    return;
                    
                _disposed = true;
            }
            finally
            {
                if(locked) _lock.Exit();
            }
            DisposeImpl();
            GC.SuppressFinalize(this);
        }
        
        ~ThreadSafeDisposable()
        {
            if (_disposed)
                return;
            DisposeImpl();
        }
    }
}