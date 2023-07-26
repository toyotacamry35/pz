using System;
using SharedCode.EntitySystem;

namespace ReactivePropsNs.Touchables
{
    public static class Toucher
    {
        /// <summary> Служебный класс чтобы легко делать обвязку на основе дельтафункций. </summary>
        public class Proxy<T> : IToucher<T>, IIsDisposed where T : IDeltaObject
        {
            private ThreadSafeDisposeWrapper _disposeWrapper;

            private object sendingLock = new object();
            private Action<T> _onAdd;
            private Action<T> _onRemove;
            private Action _onDestroyEntity;
            private Action _onComlete;

            public Proxy(Action<T> onAdd = null, Action<T> onRemove = null, Action onDestroyEntity = null, Action onCompleted = null)
            {
                _disposeWrapper = new ThreadSafeDisposeWrapper(DisposeInternal, this);
                _onAdd = onAdd ?? DoNothing;
                _onRemove = onRemove ?? DoNothing;
                _onDestroyEntity = onDestroyEntity ?? DoNothing;
                _onComlete = onCompleted ?? DoNothing;
            }

            public void OnAdd(T deltaObject)
            {
                if (!_disposeWrapper.StartWorker())
                    return;

                lock (sendingLock)
                    _onAdd(deltaObject);

                _disposeWrapper.FinishWorker();
            }

            public void OnRemove(T deltaObject)
            {
                if (!_disposeWrapper.StartWorker())
                    return;

                lock (sendingLock)
                    if (deltaObject != null)
                    {
                        _onRemove(deltaObject);
                    }
                    else
                    {
                        _onDestroyEntity();
                    }

                _disposeWrapper.FinishWorker();
            }

            public void OnCompleted()
            {
                if (!_disposeWrapper.StartWorker())
                    return;

                lock (sendingLock)
                    _onComlete();

                _disposeWrapper.FinishWorker();
            }

            /// <summary> Можно сразу добавлять к строке лога, переход на новую строку включён. </summary>
            public string LogTouchable(string prefix)
            {
                if (_requestLogHandler == null)
                    return " DISCONNECTED";
                return "\n" + _requestLogHandler(prefix);
            }

            private Func<string, string> _requestLogHandler;
            public void SetRequestLogHandler(Func<string, string> handler) => _requestLogHandler = handler;

            private static void DoNothing() { }
            private static void DoNothing(T value) { }

            public bool IsDisposed => _disposeWrapper.IsDisposed;

            public void Dispose()
            {
                _disposeWrapper.RequestDispose();
            }

            private void DisposeInternal()
            {
                _onAdd = DoNothing;
                _onRemove = DoNothing;
                _onDestroyEntity = DoNothing;
                _onComlete = DoNothing;
            }
        }
    }
}
