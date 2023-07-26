using System;
using System.Threading;

namespace ReactivePropsNs.ThreadSafe
{
    public class StreamProxy<T>: StreamProxyBase<T>
    {
        private Action<ISubscription<T>> _newListenerCallback;

        //=== Ctors, Finalizer ================================================

        public StreamProxy(Action<ISubscription<T>> newListenerCallback = null)
        {
            _newListenerCallback = newListenerCallback;
        }

        protected override void OnNewListener(ISubscription<T> listener)
        {
            var callback = _newListenerCallback;
            callback?.Invoke(listener);
        }

        protected override void OnDispose()
        {
            _newListenerCallback = null;
        }
    }
    
    //=== Class =======================================================================================================

    public static class StreamHelperExtensions
    {
        public static ISubscription<T> CreateSubscription<T>(this StreamProxy<T> target, IListener<T> listener)
        {
            if (target == null)
            {
                listener.OnCompleted();
                return Subscription<T>.Empty;
            }

            var disposable = target.CreateSubscriptionInternal(listener);
            return disposable;
        }
    }
}