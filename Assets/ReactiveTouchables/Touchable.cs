using System;
using Core.Environment.Logging.Extension;
using SharedCode.EntitySystem;

namespace ReactivePropsNs.Touchables
{
    public static class Touchable
    {
        public class Proxy<T> : ITouchable<T> where T : IDeltaObject
        {
            private const bool CREATE_LOG_WARNING = false;

            private Action<ToucherSubscriptionAgent<T>> _onSubscribeHandler;
            private RootSubscriptionAgent<T> _root;

            private object _chainLock = new object();

            private string _stackTrace = null;
            private Func<string, string> _createLog;

            // Всё что нужно для потокобезопасного диспоуза
            private ThreadSafeDisposeWrapper _disposeWrapper;

            public Proxy(Func<string, string> createLog, Action<ToucherSubscriptionAgent<T>> onSubscribeHandler = null)
            {
                _disposeWrapper = new ThreadSafeDisposeWrapper(DisposeInternal, this);
                _onSubscribeHandler = onSubscribeHandler ?? DoNothingOnSubscribe;
                _root = new RootSubscriptionAgent<T>(this);
                if (createLog == null)
                {
                    if (CREATE_LOG_WARNING)
                    {
                        #pragma warning disable CS0162 // Unreachable code detected
                        _stackTrace = Tools.StackTraceLastString();
                        ReactiveLogs.Logger.IfWarn()?.Message($"Не создан формирователь лога для {GetType().Name} по адресу: \n {_stackTrace}").Write();
                        #pragma warning restore CS0162 // Unreachable code detected
                    }
                    _createLog = prefix => $"{prefix}{GetType().FullName}<{typeof(T).Name}>{(_stackTrace != null ? $" ({_stackTrace})" : "")}";
                }
                else
                    _createLog = createLog;
            }

            ~Proxy()
            {
                if (IsDisposed)
                    return;
                //ReactiveLogs.Logger.IfWarn()?.Message($"Non disposed {GetType().NiceName()} was finalized -- {this}").Write();
            }


            public void ReceiveAddContainerChain(ITouchContainer<T> container)
            {
                if (!_disposeWrapper.StartWorker())
                    return;

                lock (_chainLock)
                    _root.Next.ReceiveAddContainerChain(container);

                _disposeWrapper.FinishWorker();
            }

            public void ReceiveRemoveContainerChain(ITouchContainer<T> container)
            {
                if (!_disposeWrapper.StartWorker())
                    return;

                lock (_chainLock)
                    _root.Next.ReceiveRemoveContainerChain(container);

                _disposeWrapper.FinishWorker();
            }

            IDisposable ITouchable<T>.Subscribe(IToucher<T> toucher)
            {
                return Subscribe(toucher);
            }

            public ToucherSubscriptionAgent<T> Subscribe(IToucher<T> toucher)
            {
                if (!_disposeWrapper.StartWorker())
                {
                    toucher.OnCompleted();
                    return EmptySubscriptionAgent<T>.Instance;
                }

                var agent = new ToucherSubscriptionAgent<T>(_chainLock, toucher.OnAdd, toucher.OnRemove, toucher.OnCompleted);
                agent.InsertAfter(_root);
                toucher.SetRequestLogHandler(DeepLog);
                _onSubscribeHandler(agent);

                _disposeWrapper.FinishWorker();

                return agent;
            }

            private static void DoNothingOnSubscribe(ToucherSubscriptionAgent<T> newAgent) { }

            public bool IsDisposed => _disposeWrapper.IsDisposed;

            public void Dispose()
            {
                _disposeWrapper.RequestDispose();
            }

            private void DisposeInternal()
            {
                _createLog = prefix => $"{prefix}{GetType().Name}<{typeof(T).Name}> DISPOSED";
                _root.Next.DisposeChain();
                GC.SuppressFinalize(this);
            }

            public string DeepLog(string prefix)
            {
                return _createLog(prefix);
            }
        }
    }
}