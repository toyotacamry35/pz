using System;
using System.Threading;

namespace ReactivePropsNs.ThreadSafe
{
    public abstract class StreamProxyBase<T>: IStream<T>
    {
        private SubscriptionAgent<T> _first;
        private SpinLock _lock = SpinLockExt.Create();
        private readonly Action<SubscriptionAgent<T>> _unsubscribe;
        private bool _disposed;


        //=== Props ===========================================================

        public bool IsDisposed => _disposed;


        //=== Ctors, Finalizer ================================================

        protected StreamProxyBase()
        {
            _unsubscribe = Unsubscribe;
        }

        ~StreamProxyBase()
        {
            /* // Disposing process is not correct anyway. When I screened it there were 67 warnings.
            if (!_disposed)
            {
                ReactiveLogs.Logger.IfWarn()?.Message($"Non disposed {GetType().NiceName()} was finalized").Write();
            }
            */
        }


        //=== Public ==========================================================

        public void Dispose()
        {
            SubscriptionAgent<T> agent = null;
            bool locked = false;
            try
            {
                _lock.EnterWithTimeout(ref locked);
                if (_disposed)
                    return;
                _disposed = true;
                agent = _first;
                _first = null;
                GC.SuppressFinalize(this);
            } finally { if (locked) _lock.Exit(); }

            while (agent != null)
            {
                var next = agent.Next;
                agent.Dispose();
                agent = next;
            }

            OnDispose();
        }

        public void OnNext(T value)
        {
            if (_disposed)
                return;

            var agent = _first;
            while (agent != null)
            {
                var next = agent.Next;
                agent.OnNext(value);
                agent = next;
            }
        }

        public IDisposable Subscribe(IListener<T> listener)
        {
            return CreateSubscriptionInternal(listener);
        }

        public ISubscription<T> CreateSubscriptionInternal(IListener<T> listener)
        {
            if (listener == null) throw new ArgumentNullException(nameof(listener));
            ISubscription<T> subscription = Subscription<T>.Empty;
            bool disposed = false;
            bool locked = false;
            try
            {
                _lock.EnterWithTimeout(ref locked);
                if (!_disposed)
                {
                    var agent = new SubscriptionAgent<T>(_unsubscribe, listener);
                    if (_first != null)
                        _first.Previous = agent;
                    agent.Next = _first;
                    _first = agent;
                    subscription = agent;
                }
                else
                {
                    disposed = true;
                }
            } finally { if (locked) _lock.Exit(); }

            if (disposed)
                listener.OnCompleted();
            else
                OnNewListener(subscription);
            
            return subscription;
        }

        protected abstract void OnNewListener(ISubscription<T> listener);

        protected abstract void OnDispose();
        
        private void Unsubscribe(SubscriptionAgent<T> agent)
        {
            bool locked = false;
            try
            {
                _lock.EnterWithTimeout(ref locked);
                if (agent == _first)
                    _first = agent.Next;
                if (agent.Previous != null)
                    agent.Previous.Next = agent.Next;
                if (agent.Next != null)
                    agent.Next.Previous = agent.Previous;
            } finally { if (locked) _lock.Exit(); }
        }
    }
}