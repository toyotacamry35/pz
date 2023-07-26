using System;
using System.Threading;
using Core.Environment.Logging.Extension;

namespace ReactivePropsNs.ThreadSafe
{
    public class SubscriptionAgent<T> : ISubscription<T>
    {
        private readonly Action<SubscriptionAgent<T>> _unsubscribeAction;
        private IListener<T> _listener;

        public SubscriptionAgent<T> Previous;

        /// <summary>
        /// Конвенция: Ссылка на Next не должна чиститься при уничтожении этого элемента,
        /// чтобы избежать обрыва вызова onNext по цепочке если элемент при обработке этого вызова застрелился
        /// </summary>
        public SubscriptionAgent<T> Next;


        //=== Props ===========================================================

        public bool IsDisposed => _listener == null;


        //=== Ctor ============================================================

        public SubscriptionAgent(Action<SubscriptionAgent<T>> unsubscribeAction, IListener<T> listener)
        {
            _listener = listener ?? throw new ArgumentNullException(nameof(listener));
            _unsubscribeAction = unsubscribeAction ?? throw new ArgumentNullException(nameof(unsubscribeAction));
        }


        //=== Public ==========================================================

        public void OnNext(T value)
        {
            var listener = _listener;
            try
            {
                listener?.OnNext(value);
            }
            catch (Exception e)
            {
                ReactiveLogs.Logger.IfError()?.Exception(e).Write();
            }
        }

        public void OnCompleted()
        {
            Dispose();
        }

        public void Dispose()
        {
            var listener = Interlocked.Exchange(ref _listener, null);
            if (listener == null)
                return;
            _unsubscribeAction(this);
            listener.OnCompleted();
        }
    }
}