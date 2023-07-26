using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SharedCode.EntitySystem;

namespace ReactivePropsNs.Touchables
{
    public static class DeltaObjectPropertyToStreamExtention
    {
        private static int _nextFreeId = 0;
        public class DeltaObjectPropertyToStream<TDelta, TValue> : IToucher<TDelta> where TDelta : class, IDeltaObject
        {
            public int Id { get; private set; }

            private static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

            private static bool _isDeltaObjectValue;
            
            public bool useDefaultWhenHasNoValue = false;

            private string _detaliedLogPrefix = null;

            private string _childName;
            private Func<TDelta, TValue> _getValue;
            private IListener<TValue> _output;
            private IListener<bool> _hasValue;

            private object _deltaObjectLock = new object();
            private TDelta _cachedDeltaObject = default;
            private TValue _lastOutput;
            private ThreadSafeDisposeWrapper _disposeWrapper = null;

            static DeltaObjectPropertyToStream() {
                _isDeltaObjectValue = typeof(IDeltaObject).IsAssignableFrom(typeof(TValue));
            }

            public DeltaObjectPropertyToStream(IListener<TValue> output, IListener<bool> hasValue, Expression<Func<TDelta, TValue>> getPropertyExpression, string detaliedLogPrefix = null)
            {
                Id = System.Threading.Interlocked.Increment(ref _nextFreeId);
                _childName = getPropertyExpression.GetMemberName();
                _getValue = getPropertyExpression.Compile();
                _disposeWrapper = new ThreadSafeDisposeWrapper(InternalDispose, this);
                _output = output;
                _hasValue = hasValue;
                _detaliedLogPrefix = detaliedLogPrefix;
                if (_detaliedLogPrefix != null)
                    Logger.Error($"$$$$$$$$$$$$$ {_detaliedLogPrefix} new DOPTS[{Id}]({typeof(TDelta).NiceName()} => {_childName})");
            }

            public void OnAdd(TDelta deltaObject)
            {
                if (!_disposeWrapper.StartWorker())
                    return;
                lock (_deltaObjectLock)
                {
                    try
                    {
                        _cachedDeltaObject = deltaObject;
                        if (_isDeltaObjectValue)
                        {
                            if (_detaliedLogPrefix != null)
                                Logger.Error($"$$$$$$$$$$$$$ {_detaliedLogPrefix}.DOPTS[{Id}].OnAdd({deltaObject.ToLogString()}).SubscribePropertyChanged({_childName}, OnChangeDeltaObjectProperty)");
                            deltaObject.SubscribePropertyChanged(_childName, OnChangeDeltaObjectProperty);
                        }
                        else
                        {
                            if (_detaliedLogPrefix != null)
                                Logger.Error($"$$$$$$$$$$$$$ {_detaliedLogPrefix}.DOPTS[{Id}].OnAdd({deltaObject.ToLogString()}).SubscribePropertyChanged({_childName}, OnChangeProperty)");
                            deltaObject.SubscribePropertyChanged(_childName, OnChangeProperty);
                        }
                        _output.OnNext(_lastOutput = _getValue(_cachedDeltaObject));
                        if (_hasValue != null)
                            _hasValue.OnNext(true);
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e);
                        //Logger.Error(e, $"{GetType().NiceName()}.OnAdd({deltaObject.ToLogString()})\n{Log("DeepLog:")}");
                    }
                }

                _disposeWrapper.FinishWorker();
            }

            public void OnRemove(TDelta deltaObject)
            {
                if (!_disposeWrapper.StartWorker())
                    return;
                lock (_deltaObjectLock)
                {
                    try
                    {
                        if (deltaObject != null) // Это если мы штатно отписываемся
                        {
                            if (_hasValue != null)
                                _hasValue.OnNext(false);
                            if (useDefaultWhenHasNoValue)
                                _output.OnNext(_lastOutput = default);
                            if (_isDeltaObjectValue)
                                {
                                if (_detaliedLogPrefix != null)
                                    Logger.Error($"$$$$$$$$$$$$$ {_detaliedLogPrefix}.DOPTS[{Id}].OnRemove({deltaObject.ToLogString()}).UnsubscribePropertyChanged({_childName}, OnChangeDeltaObjectProperty)");
                                deltaObject.UnsubscribePropertyChanged(_childName, OnChangeDeltaObjectProperty);
                            }
                            else
                            {
                                if (_detaliedLogPrefix != null)
                                    Logger.Error($"$$$$$$$$$$$$$ {_detaliedLogPrefix}.DOPTS[{Id}].OnRemove({deltaObject.ToLogString()}).UnsubscribePropertyChanged({_childName}, OnChangeProperty)");
                                deltaObject.UnsubscribePropertyChanged(_childName, OnChangeProperty);
                            }
                            _cachedDeltaObject = default;
                        }
                        else // А это если Entity бездарно сдохла в процессе
                        {
                            if (_hasValue != null)
                                _hasValue.OnNext(false);
                            if (useDefaultWhenHasNoValue)
                                _output.OnNext(_lastOutput = default);
                            _cachedDeltaObject = default; // Отписываться от дельты больше не нужно, потому как она померла ваще.
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e, $"{GetType().NiceName()}.OnRemove({deltaObject.ToLogString()})\n{Log("DeepLog:")}");
                    }
                }

                _disposeWrapper.FinishWorker();
            }

            public void OnCompleted()
            {
                lock (_deltaObjectLock)
                    _disposeWrapper.RequestDispose();
            }

            private Task OnChangeProperty(EntityEventArgs args)
            {
                if (!_disposeWrapper.StartWorker())
                    return Task.CompletedTask;
                lock (_deltaObjectLock) { // Возможно он лишний но перестраховываемся
                    if (_detaliedLogPrefix != null)
                        Logger.Error($"$$$$$$$$$$$$$ {_detaliedLogPrefix}.DOPTS[{Id}].OnChangeProperty(sender:{args.Sender.ToLogString()}, name:{args.PropertyName}, value:{args.NewValue}) // Cached: {_cachedDeltaObject.ToLogString()}");
                    _output.OnNext(_lastOutput = (TValue)args.NewValue);
                }
                _disposeWrapper.FinishWorker();
                return Task.CompletedTask;
            }
            private Task OnChangeDeltaObjectProperty(EntityEventArgs args) {
                if (!_disposeWrapper.StartWorker())
                    return Task.CompletedTask;
                lock (_deltaObjectLock) { // Возможно он лишний но перестраховываемся
                    if (_detaliedLogPrefix != null)
                        Logger.Error($"$$$$$$$$$$$$$ {_detaliedLogPrefix}.DOPTS[{Id}].OnChangeDeltaObjectProperty(sender:{args.Sender.ToLogString()}, name:{args.PropertyName}, value:{(args.NewValue as IDeltaObject).ToLogString()}) // Cached: {_cachedDeltaObject.ToLogString()}");
                    _output.OnNext(_lastOutput = ((IDeltaObject)args.NewValue).To<TValue>());
                }
                _disposeWrapper.FinishWorker();
                return Task.CompletedTask;
            }

            public string Log(string prefix)
            {
                if (_disposeWrapper.IsDisposed)
                    return $"{prefix}ITouchable<{typeof(TDelta).NiceName()}>.ToStream({_childName}) DISPOSED";
                lock (_deltaObjectLock)
                    return $"{prefix}ITouchable<{typeof(TDelta).NiceName()}>.ToStream({_childName}) {(EqualityComparer<TDelta>.Default.Equals(_cachedDeltaObject, default) ? "DISCONNECTED" : "Value:" + _lastOutput)}{(_touchableLogHandler != null ? "\n" + _touchableLogHandler(prefix + '\t') : "")}";
            }

            Func<string, string> _touchableLogHandler = null;
            public void SetRequestLogHandler(Func<string, string> handler) => _touchableLogHandler = handler;

            public bool IsDisposed => _disposeWrapper.IsDisposed;

            public void Dispose()
            {
                _disposeWrapper.RequestDispose();
            }

            private void InternalDispose()
            {
                _cachedDeltaObject = default;
                _touchableLogHandler = null; // Развязывание, возможно, избыточное, но хуже от этого не будет.
                _getValue = null;
                _hasValue?.OnCompleted();
                _hasValue = null;
                _output?.OnCompleted();
                _output = null;
            }
        }

        /// <summary>
        /// Перекладывает DeltaObject.Property в Stream Не выводя его в UnityThread
        /// </summary>
        public static IStream<TValue> ToStreamThreadPool<TDelta, TValue>(this ITouchable<TDelta> source, DisposableComposite disposables,
            Expression<Func<TDelta, TValue>> getValueExpression) where TDelta : class, IDeltaObject
        {
            Func<string, string> _createLog = null;
            string CreateLog(string prefix) =>_createLog(prefix);
            ReactiveProperty<TValue> output = PooledReactiveProperty<TValue>.Create(CreateLog);
            disposables.Add(output);
            var listener = PooledActionBasedListener<TValue>.Create(value => output.Value = value, () => output.Dispose());
            disposables.Add(listener);
            var dopts = new DeltaObjectPropertyToStream<TDelta, TValue>(listener, null, getValueExpression);
            _createLog = dopts.Log;
            disposables.Add(dopts);
            disposables.Add(source.Subscribe(dopts));
            return output;
        }

        /// <summary>
        /// Перекладывает DeltaObject.Property в Stream Не выводя его в UnityThread
        /// </summary>
        public static (IStream<TValue>, IStream<bool>) ToStreamHasValueThreadPool<TDelta, TValue>(this ITouchable<TDelta> source,
            DisposableComposite disposables, Expression<Func<TDelta, TValue>> getValueExpression, bool useDefaultWhenHasNoValue = false)
            where TDelta : class, IDeltaObject
        {
            Func<string, string> _createLog = null;
            string CreateLog(string prefix) => _createLog(prefix);
            ReactiveProperty<TValue> output = PooledReactiveProperty<TValue>.Create(CreateLog);
            disposables.Add(output);
            ReactiveProperty<bool> hasValue = PooledReactiveProperty<bool>.Create(CreateLog);
            disposables.Add(hasValue);
            var dopts = new DeltaObjectPropertyToStream<TDelta, TValue>(
                new ActionBasedListener<TValue>(value => output.Value = value, () => output.Dispose()),
                new ActionBasedListener<bool>(value => hasValue.Value = value, () => hasValue.Dispose()),
                getValueExpression)
            { useDefaultWhenHasNoValue = useDefaultWhenHasNoValue };
            _createLog = dopts.Log;
            disposables.Add(dopts);
            disposables.Add(source.Subscribe(dopts));
            return (output, hasValue);
        }
    }
}