using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using NLog;
using ReactivePropsNs.ThreadSafe;
using static ColonyShared.SharedCode.Utils.EqualityComparerFactory;

namespace ColonyShared.SharedCode.InputActions
{
    public class InputActionStatesCompositeSource : ThreadSafeDisposable, IInputActionStatesSource
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        
        private readonly IInputActionStatesSource[] _sources;
        private readonly NLog.Logger _triggersLogger;
        private readonly NLog.Logger _valuesLogger;

        private readonly ConcurrentDictionary<InputActionDef, Lazy<TriggerProxy>> _triggers = new ConcurrentDictionary<InputActionDef, Lazy<TriggerProxy>>();
        private readonly ConcurrentDictionary<InputActionDef, Lazy<ValueProxy>> _values = new ConcurrentDictionary<InputActionDef, Lazy<ValueProxy>>();
        private readonly DisposableComposite _disposables = new DisposableComposite();

        public InputActionStatesCompositeSource(params IInputActionStatesSource[] sources) : this(sources, null, null)
        {}
        
        public InputActionStatesCompositeSource(IInputActionStatesSource[] sources, NLog.Logger triggersLogger, NLog.Logger valuesLogger)
        {
            _sources = sources ?? throw new ArgumentNullException(nameof(sources));
            _triggersLogger = triggersLogger;
            _valuesLogger = valuesLogger;
        }

        public IStream<InputActionTriggerState> Stream(InputActionTriggerDef action) => GetOrAddProxy(action).Stream;

        public IStream<InputActionValueState> Stream(InputActionValueDef action) => GetOrAddProxy(action).Stream;

        private TriggerProxy GetOrAddProxy(InputActionTriggerDef action)
        {
            if (EnterIfNotDisposed()) try {
                return _triggers.GetOrAdd(action, a => new Lazy<TriggerProxy>(() => new TriggerProxy(action, _sources.Select(x => x.Stream(action)), _disposables, _triggersLogger))).Value;
            } finally { ExitAndDisposeIfRequested(); }
            return TriggerProxy.Empty;
        }

        private ValueProxy GetOrAddProxy(InputActionValueDef action)
        {
            if (EnterIfNotDisposed()) try {
                return _values.GetOrAdd(action, a => new Lazy<ValueProxy>(() => new ValueProxy(_sources.Select(x => x.Stream(action)), _disposables, _valuesLogger))).Value;
            } finally { ExitAndDisposeIfRequested(); }
            return ValueProxy.Empty;
        }

        protected override void DisposeImpl()
        {
            _triggers.Clear();
            _values.Clear();
            _disposables.Dispose();
        }

        private class TriggerProxy
        {
            private static readonly NLog.Logger DefaultLogger = LogManager.GetLogger($"{InputActionStatesCompositeSource.Logger.Name}.Triggers");
            public static readonly TriggerProxy Empty = new TriggerProxy(InputActionDef.Empty, new []{Stream<InputActionTriggerState>.Empty}, new DisposableComposite(), null);

            // ReSharper disable once MemberHidesStaticFromOuterClass
            private readonly NLog.Logger Logger;
            private readonly InputActionDef _action;
            private readonly IStream<InputActionTriggerState> _stream;

            public TriggerProxy(InputActionDef action, IEnumerable<IStream<InputActionTriggerState>> inputStreams, IDisposableCollection disposables, NLog.Logger logger)
            {
                _action = action;
                Logger = logger ?? DefaultLogger;
                for (var itr = inputStreams.GetEnumerator(); itr.MoveNext();)
                    if (itr.Current != null)
                    {
                        _stream = _stream == null ? itr.Current : _stream.ZipFunc(disposables, itr.Current, Merge, true, FalseComparer<InputActionTriggerState>());
                      //  _stream.Action(disposables, s => Logger.IfDebug()?.Message($"Combined:{s}")).Write();
                    }

                if (_stream == null) throw new InvalidOperationException("Null trigger streams");
                disposables.Add(_stream);
            }

            public IStream<InputActionTriggerState> Stream => _stream;
            
            private InputActionTriggerState Merge(InputActionTriggerState l, InputActionTriggerState r)
            {
                var result = new InputActionTriggerState(
                    active: l.Active || r.Active,
                    activated: l.Activated && (!r.Active || r.Activated) || r.Activated && !l.Active,
                    deactivated: l.Deactivated && !r.Active || r.Deactivated && !l.Active,
                    l.Awaiter != null ? l.Awaiter : r.Awaiter
                );
                if (Logger.IsTraceEnabled) Logger.IfTrace()?.Message($"Merge events | Action:{_action.ActionToString()} Event1:{l} Event2:{r} Result:{result}").Write();
                return result;
            }
        }
        
        private class ValueProxy
        {
            private static readonly NLog.Logger DefaultLogger = LogManager.GetLogger($"{InputActionStatesCompositeSource.Logger.Name}.Values");
            public static readonly ValueProxy Empty = new ValueProxy(new []{Stream<InputActionValueState>.Empty}, new DisposableComposite(), null);

            // ReSharper disable once MemberHidesStaticFromOuterClass
            private readonly NLog.Logger Logger;
            private readonly InputActionDef _action;
            private readonly IStream<InputActionValueState> _stream;
            
            public ValueProxy(IEnumerable<IStream<InputActionValueState>> inputStreams, IDisposableCollection disposables, NLog.Logger logger)
            {
                Logger = logger ?? DefaultLogger;
                for (var itr = inputStreams.GetEnumerator(); itr.MoveNext();)
                    if (itr.Current != null)
                        _stream = _stream == null ? itr.Current : _stream.Merge(disposables, itr.Current);
                if (_stream == null) throw new InvalidOperationException("Null value streams");
                disposables.Add(_stream);
            }

            public IStream<InputActionValueState> Stream => _stream;
        }
    }
}
