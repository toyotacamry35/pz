using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using SharedCode.Repositories;

// ReSharper disable CommentTypo
namespace ReactivePropsNs.Touchables
{
    public class InternalDeltaObjectDictionaryPropertyToViewModelsStream<TDelta, TKey, TValue, TOutputValue> : IToucher<TDelta>
        where TDelta : class, IDeltaObject where TValue : class, IDeltaObject, IBaseDeltaObjectWrapper where TOutputValue : IDisposable
    {
        private readonly Action<Func<Task>, IEntitiesRepository> _asyncTaskRunner;
        private readonly string _childName;

        private readonly object _deltaObjectLock = new object();
        private readonly ThreadSafeDisposeWrapper _disposeWrapper;
        private readonly Expression<Func<TDelta, IDeltaDictionary<TKey, TValue>>> _getDictionaryExpression;

        private readonly DisposableComposite _localD = new DisposableComposite();

        // Локальный опущенный в UnityThread откэшированный клон внешней недоступной конструкции.
        private readonly IDictionary<TKey, TOutputValue> _output;

        // Прокси для родительского объекта, чтобы к нему могли присасываться KeyExtention-ы
        private readonly DeltaObjectChildProxy<TDelta> _parentProxy;

        private readonly Func<TKey, TValue, Func<ITouchable<TValue>>, TOutputValue> _factoryInThreadPool;
        private IDeltaDictionary<TKey, TValue> _cachedDeltaDictionary;
        private TDelta _cachedDeltaObject;
        private Func<TDelta, IDeltaDictionary<TKey, TValue>> _getValue;
        private IListener<bool> _hasValue;

        private Func<string, string> _requestLogHandler;
        private Dictionary<TKey, TOutputValue> _transformedValues;

        public InternalDeltaObjectDictionaryPropertyToViewModelsStream(
            IDictionary<TKey, TOutputValue> output,
            IListener<bool> hasValueOutput,
            Expression<Func<TDelta, IDeltaDictionary<TKey, TValue>>> getDictionaryExpression,
            Func<TKey, TValue, Func<ITouchable<TValue>>, TOutputValue> factoryInThreadPool,
            Action<Func<Task>, IEntitiesRepository> asyncTaskRunner,
            IDisposable toDisposeOnComplete)
        {
            _getDictionaryExpression = getDictionaryExpression;
            _factoryInThreadPool = factoryInThreadPool;
            _asyncTaskRunner = asyncTaskRunner;
            // Холостая прокладка, которую, по идее, в логе можно бы и н епоказывать или вообще исключить?
            // Но пока мы ничего такого сделать не можем, да и вообще сигнатуру метода менять придётся, скорее всего,
            // поэтому просто оставляем обработчик по умолчанию..
            _parentProxy = new DeltaObjectChildProxy<TDelta>(asyncTaskRunner, SubLevelLog);
            _disposeWrapper = new ThreadSafeDisposeWrapper(InternalDispose, this);
            _childName = getDictionaryExpression.GetMemberName();
            _getValue = getDictionaryExpression.Compile();
            _hasValue = hasValueOutput;
            _output = output;
            if (toDisposeOnComplete != null)
                _localD.Add(toDisposeOnComplete);
        }

        public void OnCompleted()
        {
            lock (_deltaObjectLock)
            {
                _disposeWrapper.RequestDispose();
            }
        }

        public void SetRequestLogHandler(Func<string, string> handler)
        {
            _requestLogHandler = handler;
        }

        public bool IsDisposed => _disposeWrapper.IsDisposed;

        public void Dispose()
        {
            _disposeWrapper.RequestDispose();
        }

        public void OnAdd(TDelta deltaObject)
        {
            // Расылка получателям должно произойти синхронно, потому что мы внутри using-а Entity
            _parentProxy.OnAdd(deltaObject);
            if (!_disposeWrapper.StartWorker())
                return;
            lock (_deltaObjectLock)
            {
                _cachedDeltaObject = deltaObject;
                deltaObject.SubscribePropertyChanged(_childName, OnChangeProperty);
                _cachedDeltaDictionary = _getValue(_cachedDeltaObject);
                ConnectDeltaDictionary();
            }

            _disposeWrapper.FinishWorker();
        }

        public void OnRemove(TDelta deltaObject)
        {
            if (_disposeWrapper.StartWorker())
            {
                lock (_deltaObjectLock)
                {
                    // Это если мы штатно отписываемся
                    if (deltaObject != null)
                    {
                        _hasValue?.OnNext(false);
                        DisconnectDeltaDictionary();
                        _cachedDeltaObject.UnsubscribePropertyChanged(_childName, OnChangeProperty);
                        _cachedDeltaObject = default;
                    }
                    // А это если Entity бездарно сдохла в процессе
                    else
                    {
                        _hasValue?.OnNext(false);
                        // От _cachedDeltaDictionary Честно отпишемся, потому что мало ли для чего его ещё воспользуют
                        DisconnectDeltaDictionary();
                        // Отписываться от SubscribePropertyChanged больше не нужно, потому как она померла ваще.
                        _cachedDeltaObject = default;
                    }
                }

                _disposeWrapper.FinishWorker();
            }

            // Расылка получателям должно произойти синхронно, потому что мы внутри using-а Entity
            _parentProxy.OnRemove(deltaObject);
        }

        private void ConnectDeltaDictionary()
        {
            if (_cachedDeltaDictionary != null)
            {
                if (_transformedValues == null)
                    _transformedValues = new Dictionary<TKey, TOutputValue>();
                else
                    _transformedValues.Clear();
                foreach (var item in _cachedDeltaDictionary)
                {
                    var transformedValue = TransformValue(item.Key, item.Value);
                    _transformedValues.Add(item.Key, transformedValue);
                    _output.Add(item.Key, transformedValue);
                }

                _cachedDeltaDictionary.OnItemAddedOrUpdated += OnItemAddedOrUpdated;
                _cachedDeltaDictionary.OnItemRemoved += OnItemRemoved;
                _hasValue?.OnNext(true);
            }
        }

        private void DisconnectDeltaDictionary()
        {
            if (_cachedDeltaDictionary != null)
            {
                _hasValue?.OnNext(false);
                _cachedDeltaDictionary.OnItemAddedOrUpdated -= OnItemAddedOrUpdated;
                _cachedDeltaDictionary.OnItemRemoved -= OnItemRemoved;
                foreach (var item in _transformedValues)
                    item.Value.Dispose();
                _output.Clear();

                _transformedValues.Clear();
            }
        }

        private Task OnItemAddedOrUpdated(DeltaDictionaryChangedEventArgs<TKey, TValue> args)
        {
            //Console.WriteLine($"DeltaDictionary.OnItemAddedOrUpdated(key:{args.Key}, value:{args.Value}, OldValue:{args.OldValue})");
            var key = args.Key;
            var value = args.Value;

            if (_transformedValues.TryGetValue(key, out var oldTransformedValue))
            {
                oldTransformedValue?.Dispose();
                _transformedValues.Remove(key);
                _output.Remove(key);
            }

            var transformedValue = TransformValue(key, value);
            _transformedValues[key] = transformedValue;
            _output[key] = transformedValue;
            return Task.CompletedTask;
        }

        private Task OnItemRemoved(DeltaDictionaryChangedEventArgs<TKey, TValue> args)
        {
            //Console.WriteLine($"DeltaDictionary.OnItemRemoved(key:{args.Key}, value:{args.Value}, OldValue:{args.OldValue})");
            var key = args.Key;

            if (_transformedValues.TryGetValue(key, out var outputValue))
                outputValue?.Dispose();

            _transformedValues.Remove(key);
            _output.Remove(key);
            return Task.CompletedTask;
        }

        /// <summary>
        ///     Боря утверждает, что это событие к нам вообще никогда не придёт в связи со структурой репозитория, типа
        ///     сериализатор не умеет понимать, что Dictionary сменился.
        ///     Но мы на всякий случай эту обработку оставим. Проблема в том, что никто не гарантирует очерёдность прихода этого
        ///     эвента и внутренних эвентов DeltaDictionary.
        ///     От всего этого назщищает только протобаф, который будет вот это всё превращать просто в накатывание новых данных на
        ///     существующий элемент и всякое разное,
        ///     типа сначала поменять многое в Dictionary а потом его удалить протобаф вот это всё просто не сумеет отследить.
        /// </summary>
        private Task OnChangeProperty(EntityEventArgs args)
        {
            if (!_disposeWrapper.StartWorker())
                return Task.CompletedTask;
            lock (_deltaObjectLock)
            {
                DisconnectDeltaDictionary();
                _cachedDeltaDictionary = (IDeltaDictionary<TKey, TValue>) Activator
                    .CreateInstance(
                        typeof(DeltaDictionaryWrapper<,,>)
                            .MakeGenericType(
                                typeof(TKey),
                                ReplicaTypeRegistry.GetMasterTypeByReplicationLevelType(typeof(TValue)),
                                typeof(TValue)
                            ),
                        args.NewValue
                    );
                // _cachedDeltaDictionary = ((IDeltaDictionaryExt<TKey>) args.NewValue).ToDeltaDictionary<TValue>();
                ConnectDeltaDictionary();
            }

            _disposeWrapper.FinishWorker();
            return Task.CompletedTask;
        }

        private void InternalDispose()
        {
            _cachedDeltaObject = default;
            _localD.Dispose();
            _getValue = null;
            _hasValue?.OnCompleted();
            _hasValue = null;
        }

        private TOutputValue TransformValue(TKey key, TValue valueTyped)
        {
            return _factoryInThreadPool(key, valueTyped, () => CreateTouchable(valueTyped));
        }

        private ITouchable<TValue> CreateTouchable(TValue value)
        {
            var processor = new DeltaObjectDictionaryPropertyItemExtention.Processor<TDelta, TKey, TValue>(
                _getDictionaryExpression,
                value,
                _asyncTaskRunner
            );
            _localD.Add(processor);
            _localD.Add(_parentProxy.Subscribe(processor));
            return processor;
        }

        private string SubLevelLog(string prefix)
        {
            return _requestLogHandler(prefix);
        }
    }
}