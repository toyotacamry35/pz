using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using NLog;
using ReactivePropsNs.ThreadSafe;

namespace ColonyShared.SharedCode.InputActions
{
    public class InputActionStatesCapacitor : ThreadSafeDisposable, IInputActionStatesSource
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private const long Timeout = 2000; 
        
        private readonly IInputActionStatesSource _source;
        private readonly NLog.Logger _triggersLogger;
        private readonly NLog.Logger _valuesLogger;
        private readonly ConcurrentDictionary<InputActionDef, Lazy<Proxy<InputActionTriggerState>>> _triggers = new ConcurrentDictionary<InputActionDef, Lazy<Proxy<InputActionTriggerState>>>();
        private readonly ConcurrentDictionary<InputActionDef, Lazy<Proxy<InputActionValueState>>> _values = new ConcurrentDictionary<InputActionDef, Lazy<Proxy<InputActionValueState>>>();
        private readonly DisposableComposite _disposables = new DisposableComposite();
        private readonly EventsBox _eventsBox = new EventsBox();

        public InputActionStatesCapacitor(IInputActionStatesSource source, NLog.Logger triggersLogger = null, NLog.Logger valuesLogger = null)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
            _triggersLogger = triggersLogger;
            _valuesLogger = valuesLogger;
        }

        public void Accumulate()
        {
            _eventsBox.InvokeAccumulateEvent();
        }

        public void Flush()
        {
            _eventsBox.InvokeFlushEvent();
        }  

        public IStream<InputActionTriggerState> Stream(InputActionTriggerDef action) => GetOrAddProxy(action).OutStream;

        public IStream<InputActionValueState> Stream(InputActionValueDef action) => GetOrAddProxy(action).OutStream;
        
        protected override void DisposeImpl()
        {
            _triggers.Clear();
            _values.Clear();
            _disposables.Dispose();
        }

        private Proxy<InputActionTriggerState> GetOrAddProxy(InputActionTriggerDef action)
        {
            if(EnterIfNotDisposed()) try {
                return _triggers.GetOrAdd(action, a => new Lazy<Proxy<InputActionTriggerState>>(() => new Proxy<InputActionTriggerState>(action, _source.Stream(action), _eventsBox, _disposables, _triggersLogger))).Value;
            } finally { ExitAndDisposeIfRequested(); }
            return Proxy<InputActionTriggerState>.Empty;
        }

        private Proxy<InputActionValueState> GetOrAddProxy(InputActionValueDef action)
        {
            if(EnterIfNotDisposed()) try {
                return _values.GetOrAdd(action, a => new Lazy<Proxy<InputActionValueState>>(() => new Proxy<InputActionValueState>(action, _source.Stream(action), _eventsBox, _disposables, _valuesLogger))).Value;
            } finally { ExitAndDisposeIfRequested(); }
            return Proxy<InputActionValueState>.Empty;
        }

        private class Proxy<TEvent> 
        {
            private static readonly NLog.Logger DefaultLogger = LogManager.GetLogger($"{InputActionStatesCapacitor.Logger.Name}.{typeof(TEvent).Name}");
            public static readonly Proxy<TEvent> Empty = new Proxy<TEvent>(InputActionDef.Empty, Stream<TEvent>.Empty, new EventsBox(), new DisposableComposite(), null);
            
            // ReSharper disable once MemberHidesStaticFromOuterClass
            private readonly NLog.Logger Logger;
            private readonly InputActionDef _action;
            private readonly StreamProxy<TEvent> _outStream = new StreamProxy<TEvent>();
            private readonly ConcurrentQueue<TEvent> _buffer = new ConcurrentQueue<TEvent>();
            private readonly SpinWait _spinWait = new SpinWait();
            private int _accumulate;
            private int _accumulated;
            private long _timeoutAt;
            
            public Proxy(InputActionDef action, IStream<TEvent> inStream, EventsBox eventsBox, ICollection<IDisposable> disposables, NLog.Logger logger)
            {
                _action = action;
                Logger = logger ?? DefaultLogger;
                inStream.Action(disposables, OnNext);
                disposables.Add(_outStream);
                eventsBox.AccumulateEvent += Accumulate;
                eventsBox.FlushEvent += Flush;
            }

            private void Accumulate()
            {
                if(Logger.IsDebugEnabled && !Logger.IsTraceEnabled) Logger.IfDebug()?.Message($"Accumulate | Action:{_action.ActionToString()}").Write();
                Thread.VolatileWrite(ref _accumulate, 1);
                Thread.VolatileWrite(ref _timeoutAt, SyncTime.NowUnsynced + Timeout);
            }

            private void Flush()
            {
                if (Interlocked.Exchange(ref _accumulate, 0) == 1)
                {
                    if(Logger.IsDebugEnabled && !Logger.IsTraceEnabled) Logger.IfDebug()?.Message($"Flush | Action:{_action.ActionToString()}").Write();

                    while(true)
                    {
                        while (_buffer.TryDequeue(out var s))
                        {
                            if(Logger.IsTraceEnabled) Logger.IfTrace()?.Message($"Flush | Action:{_action.ActionToString()} Event:{s}").Write();
                            _outStream.OnNext(s);
                            Interlocked.Decrement(ref _accumulated);
                        }
                        var accumulated = Thread.VolatileRead(ref _accumulated);
                        if (accumulated == 0)
                            return;
                        if (accumulated < 0)
                            throw new Exception($"_accumulated:{accumulated}");
                        _spinWait.SpinOnce();
                    } 
                }
            }
            
            private void OnNext(TEvent @event)
            {
                Interlocked.Increment(ref _accumulated);
                if (Thread.VolatileRead(ref _accumulate) != 0)
                {
                    if(Logger.IsTraceEnabled) Logger.IfTrace()?.Message($"Accumulate | Action:{_action.ActionToString()} Event:{@event}").Write();
                    _buffer.Enqueue(@event);
                    if (_timeoutAt < SyncTime.NowUnsynced)
                    {
                        Logger.IfError()?.Message("Flush by timeout").Write();
                        Flush();
                    }
                }
                else
                {
                    Interlocked.Decrement(ref _accumulated);
                    _outStream.OnNext(@event);
                }
            }

            public IStream<TEvent> OutStream => _outStream;
        }

        private class EventsBox
        {
            public event Action AccumulateEvent = delegate {};
            public event Action FlushEvent = delegate {};
            public void InvokeAccumulateEvent() => AccumulateEvent.Invoke();
            public void InvokeFlushEvent() => FlushEvent.Invoke();
        }
    }
}