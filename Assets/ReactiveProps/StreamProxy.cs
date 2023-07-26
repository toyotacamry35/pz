//#define ENABLE_REACTIVE_STACKTRACE
//#define ENABLE_REACTIVE_FINALIZER_CHECK
using System;
using ReactiveProps;

namespace ReactivePropsNs
{
    public class StreamProxy<T> : StreamProxyBase<T>, IStream<T>
    {
        private Action<ISubscription<T>> _newListenerCallback;

        public StreamProxy(Action<ISubscription<T>> newListenerCallback = null, Func<string, string> createLog = null) => Setup(newListenerCallback, createLog);
        
#region protected

        protected void Setup(Action<ISubscription<T>> newListenerCallback = null, Func<string, string> createLog = null)
        {
            base.Setup(createLog);
            _newListenerCallback = newListenerCallback;
        }

        protected override void OnNewSubscription(ISubscription<T> listener) => _newListenerCallback?.Invoke(listener);
        
#endregion
    }


    public class PooledStreamProxy<T> : StreamProxy<T>, IPooledObject
    {
        private static readonly Pool<PooledStreamProxy<T>> _Pool = Pool.Create(() => new PooledStreamProxy<T>(), PoolSizes.GetPoolMaxCapacityForType<PooledStreamProxy<T>>(2000));

        public static PooledStreamProxy<T> Create(Action<ISubscription<T>> newListenerCallback = null, Func<string, string> createLog = null)
        {
            var proxy = _Pool.Acquire();
            proxy.Setup(newListenerCallback, createLog);
            return proxy;
        }

        private static void Release(PooledStreamProxy<T> proxy)
        {
            _Pool.Release(proxy);
        }

        protected override void DisposeInternal()
        {
            base.DisposeInternal();
            Release(this); // FIXME: Ugly! Но политика работы со временем жизни в реактивностях не позволяет сделать иначе 
        }
        
        private PooledStreamProxy() {}
        
        bool IPooledObject.Released { get; set; }
    }


    //=== Class =======================================================================================================

    public static class StreamHelperExtensions
    {
        public static ISubscription<T> CreateSubscribtion<T>(this StreamProxy<T> target, IListener<T> listener)
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
