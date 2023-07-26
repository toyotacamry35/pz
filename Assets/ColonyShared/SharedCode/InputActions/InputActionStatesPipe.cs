using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using NLog;
using ReactivePropsNs.ThreadSafe;

namespace ColonyShared.SharedCode.InputActions
{
    public class InputActionStatesPipe : ThreadSafeDisposable, IInputActionStatesSource, IInputActionStatesRecipient
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        
        private readonly NLog.Logger _triggersLogger;
        private readonly NLog.Logger _valuesLogger;
        private readonly ConcurrentDictionary<InputActionDef, Lazy<Proxy<InputActionTriggerState>>> _triggers = new ConcurrentDictionary<InputActionDef, Lazy<Proxy<InputActionTriggerState>>>();
        private readonly ConcurrentDictionary<InputActionDef, Lazy<Proxy<InputActionValueState>>> _values = new ConcurrentDictionary<InputActionDef, Lazy<Proxy<InputActionValueState>>>();
        private readonly DisposableComposite _disposables = new DisposableComposite();
        
        public InputActionStatesPipe() : this (null, null) {}
        
        public InputActionStatesPipe(NLog.Logger triggersLogger, NLog.Logger valuesLogger)
        {
            _triggersLogger = triggersLogger;
            _valuesLogger = valuesLogger;
        }

        public IStream<InputActionTriggerState> Stream(InputActionTriggerDef action) => GetOrAddProxy(action).Stream;

        public IStream<InputActionValueState> Stream(InputActionValueDef action) => GetOrAddProxy(action).Stream;

        public IListener<InputActionTriggerState> Listener(InputActionTriggerDef action) => GetOrAddProxy(action);

        public IListener<InputActionValueState> Listener(InputActionValueDef action) => GetOrAddProxy(action);
        
        protected override void DisposeImpl()
        {
            _triggers.Clear();
            _values.Clear();
            _disposables.Dispose();
        }

        private Proxy<InputActionTriggerState> GetOrAddProxy(InputActionTriggerDef action)
        {
            if(EnterIfNotDisposed()) try {
                return _triggers.GetOrAdd(action, a => new Lazy<Proxy<InputActionTriggerState>>(() => new Proxy<InputActionTriggerState>(action, _disposables, _triggersLogger))).Value;
            } finally { ExitAndDisposeIfRequested(); }
            return Proxy<InputActionTriggerState>.Empty;
        }

        private Proxy<InputActionValueState> GetOrAddProxy(InputActionValueDef action)
        {
            if(EnterIfNotDisposed()) try {
                return _values.GetOrAdd(action, a => new Lazy<Proxy<InputActionValueState>>(() => new Proxy<InputActionValueState>(action, _disposables, _valuesLogger))).Value;
            } finally { ExitAndDisposeIfRequested(); }
            return Proxy<InputActionValueState>.Empty;
        }

        private class Proxy<TEvent> : IListener<TEvent>
        {
            private static readonly NLog.Logger DefaultLogger = LogManager.GetLogger($"{InputActionStatesPipe.Logger.Name}.{typeof(TEvent).Name}");
            public static readonly Proxy<TEvent> Empty = new Proxy<TEvent>(InputActionDef.Empty, new DisposableComposite(), DefaultLogger);

            // ReSharper disable once MemberHidesStaticFromOuterClass
            private readonly NLog.Logger Logger;
            private readonly InputActionDef _action;
            private readonly StreamProxy<TEvent> _stream = new StreamProxy<TEvent>();
            
            public Proxy(InputActionDef action, ICollection<IDisposable> disposables, NLog.Logger logger)
            {
                _action = action;
                Logger = logger ?? DefaultLogger;
                disposables.Add(_stream);
            }

            public void OnNext(TEvent @event)
            {
                if (Logger.IsTraceEnabled) Logger.IfTrace()?.Message($"Pipe event | Action:{_action} Event:{@event}").Write();
                _stream.OnNext(@event);
            }

            public IStream<TEvent> Stream => _stream;
            
            public void OnCompleted() {}

            public void Dispose() {}

            public bool IsDisposed => false;
        }
    }
}