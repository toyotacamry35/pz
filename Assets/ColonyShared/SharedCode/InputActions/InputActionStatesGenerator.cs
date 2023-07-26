using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using NLog;
using ReactivePropsNs.ThreadSafe;
using SharedCode.Wizardry;

namespace ColonyShared.SharedCode.InputActions
{
    public class InputActionStatesGenerator : ThreadSafeDisposable, IInputActionStatesGenerator, IInputActionStatesSource
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly NLog.Logger _triggersLogger;
        private readonly NLog.Logger _valuesLogger;
        private readonly ConcurrentDictionary<InputActionTriggerDef, Lazy<TriggerProxy>> _triggers = new ConcurrentDictionary<InputActionTriggerDef, Lazy<TriggerProxy>>();
        private readonly ConcurrentDictionary<InputActionValueDef, Lazy<ValueProxy>> _values = new ConcurrentDictionary<InputActionValueDef, Lazy<ValueProxy>>();
        private readonly DisposableComposite _disposables = new DisposableComposite();

        public InputActionStatesGenerator(NLog.Logger triggersLogger = null, NLog.Logger valuesLogger = null)
        {
            _triggersLogger = triggersLogger;
            _valuesLogger = valuesLogger;
        }

        public IStream<InputActionTriggerState> Stream(InputActionTriggerDef action) => GetOrAddProxy(action).Stream;

        public IStream<InputActionValueState> Stream(InputActionValueDef action) => GetOrAddProxy(action).Stream;
        
        public void PushTrigger(object causer, InputActionTriggerDef action, AwaitableSpellDoerCast awaitable = default)
        {
            if (causer == null) throw new ArgumentNullException(nameof(causer));
            if (action == null) throw new ArgumentNullException(nameof(action));
            if(Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Push Action | Action:{action.ActionToString()} Causer:{causer}").Write();
            GetOrAddProxy(action).AddCauser(causer, awaitable);
        }
        
        public void PopTrigger(object causer, InputActionTriggerDef action)
        {
            if (!TryPopTrigger(causer, action, false))
                if(Logger.IsWarnEnabled) Logger.IfError()?.Message($"Pop action | Pair (Action,Causer) not found | Action:{action} Causer:{causer}").Write();
        }
        
        public bool TryPopTrigger(object causer, InputActionTriggerDef action, bool all)
        {
            if (causer == null) throw new ArgumentNullException(nameof(causer));
            if (action == null) throw new ArgumentNullException(nameof(action));
            return GetOrAddProxy(action).RemoveCauser(causer, all);
        }
        
        public void SetValue(InputActionValueDef action, float value)
        {
            GetOrAddProxy(action).Send(new InputActionValueState(value));
        }

        protected override void DisposeImpl()
        {
            _triggers.Clear();
            _values.Clear();
            _disposables.Dispose();
        }

        private TriggerProxy GetOrAddProxy(InputActionTriggerDef action)
        {
            if(EnterIfNotDisposed()) try
            {
                return _triggers.GetOrAdd(action, a => new Lazy<TriggerProxy>(() => new TriggerProxy(action, _disposables, _triggersLogger))).Value;
            } finally { ExitAndDisposeIfRequested(); }
            return TriggerProxy.Empty;
        }

        private ValueProxy GetOrAddProxy(InputActionValueDef action)
        {
            if(EnterIfNotDisposed()) try
            {
                return _values.GetOrAdd(action, a => new Lazy<ValueProxy>(() => new ValueProxy(action, _disposables, _valuesLogger))).Value;
            } finally { ExitAndDisposeIfRequested(); }
            return ValueProxy.Empty;
        }

        private class TriggerProxy
        {
            private static readonly NLog.Logger DefaultLogger = LogManager.GetLogger($"{InputActionStatesGenerator.Logger.Name}.Triggers");
            public static readonly TriggerProxy Empty = new TriggerProxy();

            // ReSharper disable once MemberHidesStaticFromOuterClass
            private readonly NLog.Logger Logger;
            private readonly List<object> _causers = new List<object>();
            private readonly StreamProxy<InputActionTriggerState> _stream = new StreamProxy<InputActionTriggerState>();
            private readonly InputActionTriggerDef _action;
            private readonly bool _isEmpty = false;
            
            public TriggerProxy(InputActionTriggerDef action, IDisposableCollection disposables, NLog.Logger logger)
            {
                _action = action;
                Logger = logger ?? DefaultLogger;
                disposables.Add(_stream);
            }
            
            public IStream<InputActionTriggerState> Stream => _stream;

            public void AddCauser(object causer, AwaitableSpellDoerCast awaitable = default)
            {
                if (_isEmpty) return;
                
                int count;
                lock (_causers)
                {
                    count = _causers.Count;
                    _causers.Add(causer);
                }
                if (count == 0)
                {
                    SendEvent(new InputActionTriggerState(true, true, false, awaitable));
                    SendEvent(new InputActionTriggerState(true, false, false, awaitable)); // чтобы это значение оставалось как last value в пропертях
                }
            }

            public bool RemoveCauser(object causer, bool all)
            {
                if (_isEmpty) return true;
                
                int count;
                lock (_causers)
                {
                    var removed = all ? _causers.RemoveAll(x => x.Equals(causer)) != 0 : _causers.Remove(causer);
                    if (!removed)
                        return false;
                    count = _causers.Count;
                }
                if (count == 0)
                {
                    SendEvent(new InputActionTriggerState(false, false, true));
                    SendEvent(new InputActionTriggerState(false, false, false));
                }
                return true;
            }

            private TriggerProxy()
            {
                _isEmpty = true;
            }
            
            private void SendEvent(InputActionTriggerState @event)
            {
                if (Logger.IsTraceEnabled) Logger.IfTrace()?.Message($"Generate event | Action:{_action.ActionToString()} Event:{@event}").Write();
                _stream.OnNext(@event);
            }
        }
        
        private class ValueProxy
        {
            private static readonly NLog.Logger DefaultLogger = LogManager.GetLogger($"{InputActionStatesGenerator.Logger.Name}.Values");
            public static readonly ValueProxy Empty = new ValueProxy();

            // ReSharper disable once MemberHidesStaticFromOuterClass
            private readonly NLog.Logger Logger;
            private readonly InputActionValueDef _action;
            private readonly StreamProxy<InputActionValueState> _stream = new StreamProxy<InputActionValueState>();
            private readonly bool _isNull = false;
            
            public ValueProxy(InputActionValueDef action, IDisposableCollection disposables, NLog.Logger logger)
            {
                _action = action;
                Logger = logger ?? DefaultLogger;
                disposables.Add(_stream);
            }
            
            public IStream<InputActionValueState> Stream => _stream;
            
            public void Send(InputActionValueState @event)
            {
                if (_isNull) return;
                if (Logger.IsTraceEnabled) Logger.IfTrace()?.Message($"Generate event | Action:{_action.ActionToString()} Event:{@event}").Write();
                _stream.OnNext(@event);
            }

            private ValueProxy()
            {
                _isNull = true;
            }
        }
    }
}
