//#define ENABLE_REACTIVE_STACKTRACE
using System;
using Core.Environment.Logging.Extension;

#if ENABLE_REACTIVE_STACKTRACE
using System.Diagnostics;
#endif

namespace ReactivePropsNs
{
    public abstract class StreamProxyBase<T> : SubscriptionAgent<T>.ISubscriptionAgentOwner
    {
        private SubscriptionAgent<T> _first;
        private string _name;
        private Func<string, string> _createLog;
        private bool _isDisposed;
        #if ENABLE_REACTIVE_STACKTRACE
        private StackTrace _creationStackTrace;
        protected string CreationStackTrace => (_creationStackTrace != null ? $" ({Tools.StackTraceLastString(_creationStackTrace)})" : string.Empty); 
        #else
        protected static readonly string CreationStackTrace = string.Empty;
        #endif

        public bool IsDisposed => _isDisposed;
        
        public bool HasListeners() => _first != null && _first.Next != _first;

        public void SetCreateLog(Func<string, string> createLog) => _createLog = createLog;

        public IDisposable Subscribe(IListener<T> listener) => CreateSubscriptionInternal(listener);

        public void Dispose()
        {
            if (IsDisposed)
                return;

            DisposeInternal();
            #if ENABLE_REACTIVE_FINALIZER_CHECK
            GC.SuppressFinalize(this);
            #endif            
        }

        public void OnNext(T value)
        {
            if (IsDisposed)
                return;

            var agent = _first;
            while (agent != null)
            {
                agent.OnNext(value);
                agent = agent.Next;
            }
        }

        public ISubscription<T> CreateSubscriptionInternal(IListener<T> listener) // Internal и public, весело )
        {
            if (IsDisposed)
            {
                listener.OnCompleted();
                return Subscription<T>.Empty;
            }

            var agent = SubscriptionAgent<T>.Create(this, listener);
            if (_first != null)
            {
                agent.Next = _first;
                _first.Previous = agent;
            }

            if (agent.IsDisposed) throw new Exception("Created SubscriptionAgent already DISPOSED");
            _first = agent;
            OnNewSubscription(agent);
            return agent;
        }

        public string DeepLog(string prefix) => _createLog != null ? _createLog.Invoke(prefix) : Log(prefix);

        public override string ToString() => IsDisposed ? $"{Name} DISPOSED" : Name;
        
#region protected

        protected abstract void OnNewSubscription(ISubscription<T> listener);
        
        protected string ClassName => GetType().NiceName();
        
        protected string Name => _name ?? (_name = $"{ClassName}[{UniqueId.Id++}]{CreationStackTrace}");

        protected void Setup(Func<string, string> createLog = null)
        {
            _first = default;
            _name = default;
            _createLog = default;
            _isDisposed = default;
            #if ENABLE_REACTIVE_STACKTRACE
            if (createLog == null)
            {
                _creationStackTrace = new StackTrace(1, true); // Создаётся только в режиме глубокой отладки, поэтому пофигу сколько оно жрёт
                ReactiveLogs.Logger.IfWarn()?.Message($"Не создан формирователь лога для {ClassName} по адресу: \n {_creationStackTrace}").Write();
            }
            else
            #endif
                _createLog = createLog;
        }

        protected virtual void DisposeInternal()
        {
            _isDisposed = true;
            while (_first != null)
            {
                var agent = _first;
                if (!agent.IsDisposed)
                    agent.Dispose();
                else
                {
                    ReactiveLogs.Logger.IfError()?.Write("Trying to dispose already disposed SubscriptionAgent");
                    Unsubscribe(agent);
                }
                SubscriptionAgent<T>.Release(agent);
            }
        }

        protected virtual string Log (string prefix)  => $"{prefix}{ClassName}{CreationStackTrace}";
    
#endregion

#region private
        
        #if ENABLE_REACTIVE_FINALIZER_CHECK
        ~StreamProxy()
        {
            if (IsDisposed)
                return;

            ReactiveLogs.Logger.IfWarn()?.Message($"Non disposed {ClassName} was finalized {this}").Write();
        }
        #endif
        
        public void Unsubscribe(SubscriptionAgent<T> agent)
        {
            if (agent == _first)
                _first = agent.Next;
            if (agent.Previous != null)
                agent.Previous.Next = agent.Next;
            if (agent.Next != null)
                agent.Next.Previous = agent.Previous;
        }
#endregion
    }
}