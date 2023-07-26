using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ColonyShared.SharedCode.Utils;
using ReactivePropsNs.ThreadSafe;

namespace ColonyShared.SharedCode.InputActions
{
    public class InputActionStatesInjector : ThreadSafeDisposable, IInputActionStatesSource, IInputActionStatesRecipient, IDisposable
    {
        private readonly IInputActionStatesSource _parentSource;
        
        private readonly ConcurrentDictionary<InputActionDef, Lazy<TriggerProxy>> _triggers = new ConcurrentDictionary<InputActionDef, Lazy<TriggerProxy>>();
        private readonly ConcurrentDictionary<InputActionDef, Lazy<ValueProxy>> _values = new ConcurrentDictionary<InputActionDef, Lazy<ValueProxy>>();
        private readonly DisposableComposite _disposables = new DisposableComposite();
        
        public InputActionStatesInjector(IInputActionStatesSource parentSource)
        {
            _parentSource = parentSource ?? throw new ArgumentNullException(nameof(parentSource));
        }

        protected override void DisposeImpl()
        {
            _triggers.Clear();
            _values.Clear();
            _disposables.Dispose();
        }

        public IStream<InputActionTriggerState> Stream(InputActionTriggerDef action) => GetOrAddProxy(action).Stream;

        public IStream<InputActionValueState> Stream(InputActionValueDef action) => GetOrAddProxy(action).Stream;

        public IListener<InputActionTriggerState> Listener(InputActionTriggerDef action) => GetOrAddProxy(action);

        public IListener<InputActionValueState> Listener(InputActionValueDef action) => GetOrAddProxy(action);

        private TriggerProxy GetOrAddProxy(InputActionTriggerDef action)
        {
            if (EnterIfNotDisposed()) try {
                return _triggers.GetOrAdd(action, a => new Lazy<TriggerProxy>(() => new TriggerProxy(_parentSource.Stream(action), _disposables))).Value;
            } finally { ExitAndDisposeIfRequested(); }
            return TriggerProxy.Empty;
        }

        private ValueProxy GetOrAddProxy(InputActionValueDef action)
        {
            if (EnterIfNotDisposed()) try {
                return _values.GetOrAdd(action, a => new Lazy<ValueProxy>(() => new ValueProxy(_parentSource.Stream(action), _disposables))).Value;
            } finally { ExitAndDisposeIfRequested(); }
            return ValueProxy.Empty;
        }

        private class TriggerProxy : IListener<InputActionTriggerState>
        {
            private readonly StreamProxy<InputActionTriggerState> _stream = new StreamProxy<InputActionTriggerState>();
            private readonly object _lock = new object();
            private InputActionTriggerState _state;
            
            public TriggerProxy(IStream<InputActionTriggerState> inputStream, ICollection<IDisposable> disposables)
            {
                disposables.Add(_stream);
                inputStream.Subscribe(disposables, this);
            }

            public void OnNext(InputActionTriggerState @event)
            {
                InputActionTriggerState oldState;
                lock (_lock) (oldState, _state) = (_state, @event);
                _stream.OnNext(new InputActionTriggerState(@event.Active, @event.Active && !oldState.Active, !@event.Active && oldState.Active));
            }

            public IStream<InputActionTriggerState> Stream => _stream;
            
            public void OnCompleted() {}

            public void Dispose() {}

            public bool IsDisposed => false;
            
            public static readonly TriggerProxy Empty = new TriggerProxy(Stream<InputActionTriggerState>.Empty, new DisposableComposite());
        }
        
        private class ValueProxy : IListener<InputActionValueState>
        {
            private readonly ReactiveProperty<InputActionValueState> _stream = new ReactiveProperty<InputActionValueState>();
            
            public ValueProxy(IStream<InputActionValueState> inputStream, ICollection<IDisposable> disposables)
            {
                disposables.Add(_stream);
                inputStream.Subscribe(disposables, this);
            }

            public void OnNext(InputActionValueState @event)
            {
                _stream.Value = @event;
            }

            public IStream<InputActionValueState> Stream => _stream;

            public void OnCompleted() {}
            
            public void Dispose() {}

            public bool IsDisposed => false;
            
            public static readonly ValueProxy Empty = new ValueProxy(Stream<InputActionValueState>.Empty, new DisposableComposite());
        }
    }
}