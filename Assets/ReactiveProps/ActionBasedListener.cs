//#define ENABLE_REACTIVE_STACKTRACE
//#define ENABLE_REACTIVE_FINALIZER_CHECK
using System;
using System.Collections.Generic;
using ReactiveProps;

namespace ReactivePropsNs
{
    public class ActionBasedListener<T> : IListener<T>
    {
        private Action<T> _onNextAction;
        private Action _onCompleteAction;
        private Func<string, string> _reciveStreamLog;
        private CreateLog _createLog;
        private bool _isDisposed;
#if ENABLE_REACTIVE_STACKTRACE
        private readonly string _stackTrace;
#endif

        public bool IsDisposed => _isDisposed;

        bool IIsDisposed.IsDisposed => _isDisposed;

        //=== Ctor ============================================================
        public delegate string CreateLog(string prefix, Func<string, string> previous);

        public ActionBasedListener(Action<T> onNextAction, Action onCompleteAction, CreateLog createLog = null)
        {
            Setup(onNextAction, onCompleteAction, createLog);
        }
        
        protected void Setup(Action<T> onNextAction, Action onCompleteAction, CreateLog createLog = null)
        {
            _onNextAction = onNextAction;
            _onCompleteAction = onCompleteAction;
            _reciveStreamLog = default;
            _createLog = default;
            _isDisposed = default;
            #if ENABLE_REACTIVE_STACKTRACE
            _stackTrace = default;
            if (createLog == null)
            {
                _stackTrace = Tools.StackTraceLastString();
                ReactiveLogs.Logger.IfWarn()?.Message($"Не создан формирователь лога для {GetType().Name} по адресу: \n {_stackTrace}").Write();
            }
            else
            #endif
                _createLog = createLog;
        }

        public void SetRequestLogHandler(Func<string, string> handler) {
            _reciveStreamLog = handler;
        }

#if ENABLE_REACTIVE_FINALIZER_CHECK        
        ~ActionBasedListener()
        {
            if (IsDisposed)
                return;
        
            ReactiveLogs.Logger.IfWarn()?.Message($"Non disposed {GetType().NiceName()} was finalized -- {this}").Write();
        }
#endif

        //=== Public ==========================================================

        public void OnNext(T value)
        {
            if (IsDisposed)
                return;

            _onNextAction?.Invoke(value);
        }

        public void OnCompleted()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            _isDisposed = true;
            var onComplete = _onCompleteAction;
            _onNextAction = null;
            _onCompleteAction = null;
            _reciveStreamLog = null;
            onComplete?.Invoke();
            DisposeInternal();
#if ENABLE_REACTIVE_FINALIZER_CHECK
            GC.SuppressFinalize(this);
#endif
        }
        
        public string Log(string prefix)
        {
            if (IsDisposed)
                return LogOfDisposedObject(prefix, _reciveStreamLog);
            if (_createLog == null)
                return LogOfObject(prefix, _reciveStreamLog);
            return _createLog(prefix, _reciveStreamLog);
        }

        protected virtual void DisposeInternal()
        {}
        
        private string LogOfObject(string prefix, Func<string, string> previous) =>
            prefix + nameof(ActionBasedListener<T>)
                   + "\n" + prefix + ">\t" + CreationStackTrace
                   + (previous != null ? "\n" + previous(prefix + "\t") : ""); // Вызывается только при ручной отладке, так что пофиг на эффективность
        
        private string LogOfDisposedObject(string prefix, Func<string, string> previous) => $"{prefix}{{{GetType().Name}<{typeof(T).Name}> disposed}}";
        
        private string CreationStackTrace => 
#if ENABLE_REACTIVE_STACKTRACE
            _stackTrace ?? string.Empty;
#else
            string.Empty;
#endif
    }

    //=================================================================================================================
    
    public class PooledActionBasedListener<T> : ActionBasedListener<T>, IPooledObject
    {
        private static readonly Pool<PooledActionBasedListener<T>> _Pool = Pool.Create(() => new PooledActionBasedListener<T>(), PoolSizes.GetPoolMaxCapacityForType<PooledActionBasedListener<T>>(2000));

        public static PooledActionBasedListener<T> Create(Action<T> onNextAction, Action onCompleteAction, CreateLog createLog = null)
        {
            var prop = _Pool.Acquire();
            prop.Setup(onNextAction, onCompleteAction, createLog);
            return prop;
        }
        
        private void Release(PooledActionBasedListener<T> prop)
        {
            _Pool.Release(prop);
        }

        protected override void DisposeInternal()
        {
            base.DisposeInternal();
            Release(this); // FIXME: Ugly! Но политика работы со временем жизни в реактивностях не позволяет сделать иначе 
        }

        private PooledActionBasedListener() : base(null, null) {}
        
        bool IPooledObject.Released { get; set; }
    }
    
    //=================================================================================================================

    public static class ActionBasedListenerExtentions
    {
        public static IDisposable Subscribe<T>(this IStream<T> stream, ICollection<IDisposable> disposables, Action<T> onNextAction, Action onCompletedAction)
        {
            var listener = PooledActionBasedListener<T>.Create(onNextAction, onCompletedAction);
            disposables.Add(listener);
            var disposable = stream.Subscribe(listener);
            disposables.Add(disposable);
            return listener;
        }
        public static IDisposable Subscribe<T>(this IStream<T> stream, ICollection<IDisposable> disposables,
            IListener<T> listener)
        {
            var disposable = stream.Subscribe(listener);
            disposables.Add(disposable);
            return listener;
        }
        public static IDisposable Subscribe<T1, T2>(this IStream<ValueTuple<T1, T2>> stream, ICollection<IDisposable> disposables,
            Action<T1, T2> onNextAction, Action onCompletedAction)
        {
            return stream.Subscribe(disposables, val => onNextAction(val.Item1, val.Item2), onCompletedAction);
        }

        public static IDisposable Action<T1, T2, T3>(this IStream<ValueTuple<ValueTuple<T1, T2>, T3>> stream, ICollection<IDisposable> disposables,
            Action<T1, T2, T3> onNextAction, Action onCompletedAction)
        {
            return stream.Subscribe(disposables, val => onNextAction(val.Item1.Item1, val.Item1.Item2, val.Item2), onCompletedAction);
        }

        public static IDisposable Action<T1, T2, T3, T4>(this IStream<ValueTuple<ValueTuple<ValueTuple<T1, T2>, T3>, T4>> stream,
            ICollection<IDisposable> disposables, Action<T1, T2, T3, T4> onNextAction, Action onCompletedAction)
        {
            return stream.Subscribe(
                disposables, val => onNextAction(val.Item1.Item1.Item1, val.Item1.Item1.Item2, val.Item1.Item2, val.Item2), onCompletedAction);
        }
        public static IDisposable Action<T>(this IStream<T> stream, ICollection<IDisposable> disposables, Action<T> onNextAction)
        {
            return Subscribe(stream, disposables, onNextAction, null);
        }

        public static IDisposable Action<T1, T2>(this IStream<ValueTuple<T1, T2>> stream, ICollection<IDisposable> disposables, Action<T1, T2> action)
        {
            return stream.Action(disposables, val => action(val.Item1, val.Item2));
        }

        public static IDisposable Action<T1, T2, T3>(this IStream<ValueTuple<ValueTuple<T1, T2>, T3>> stream, ICollection<IDisposable> disposables,
            Action<T1, T2, T3> action)
        {
            return stream.Action(disposables, val => action(val.Item1.Item1, val.Item1.Item2, val.Item2));
        }

        public static IDisposable Action<T1, T2, T3, T4>(this IStream<ValueTuple<ValueTuple<ValueTuple<T1, T2>, T3>, T4>> stream,
            ICollection<IDisposable> disposables, Action<T1, T2, T3, T4> action)
        {
            return stream.Action(disposables, val => action(val.Item1.Item1.Item1, val.Item1.Item1.Item2, val.Item1.Item2, val.Item2));
        }

        public static IDisposable Action<T1, T2, T3, T4, T5>(this IStream<ValueTuple<ValueTuple<ValueTuple<ValueTuple<T1, T2>, T3>, T4>, T5>> stream,
            ICollection<IDisposable> disposables, Action<T1, T2, T3, T4, T5> action)
        {
            return stream.Action(disposables, val => action(
                val.Item1.Item1.Item1.Item1, val.Item1.Item1.Item1.Item2, val.Item1.Item1.Item2, val.Item1.Item2, val.Item2));
        }

        public static IDisposable Bind<T>(this IStream<T> stream, ICollection<IDisposable> disposables, ReactiveProperty<T> rp)
        {
            return stream.Action(disposables, t => rp.Value = t);
        }
        
        public static IDisposable Bind<T>(this IStream<T> stream, ICollection<IDisposable> disposables, StreamProxy<T> sp)
        {
            return stream.Action(disposables, sp.OnNext);
        }
    }
}