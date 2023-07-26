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
    public class InternalDeltaObjectListPropertyToViewModelsStream<TDelta, TValue, TOutputValue> : IToucher<TDelta>
        where TDelta : IDeltaObject where TValue : class, IDeltaObject, IBaseDeltaObjectWrapper where TOutputValue : IDisposable
    {
        private readonly Action<Func<Task>, IEntitiesRepository> _asyncTaskRunner;
        private readonly string _childName;

        private readonly object _deltaObjectLock = new object();
        private readonly ThreadSafeDisposeWrapper _disposeWrapper;
        private readonly Expression<Func<TDelta, IDeltaList<TValue>>> _getListExpression;

        private readonly DisposableComposite _localD = new DisposableComposite();

        // Локальный опущенный в UnityThread откэшированный клон внешней недоступной конструкции.
        private readonly ICollection<TOutputValue> _output;

        // Прокси для родительского объекта, чтобы к нему могли присасываться KeyExtention-ы
        private readonly DeltaObjectChildProxy<TDelta> _parentProxy;

        private readonly Func<TValue, Func<ITouchable<TValue>>, TOutputValue> _factoryInThreadPool;
        private IDeltaList<TValue> _cachedDeltaList;
        private TDelta _cachedDeltaObject;
        private Func<TDelta, IDeltaList<TValue>> _getValue;
        private IListener<bool> _hasValue;

        private Func<string, string> _requestLogHandler;
        private Dictionary<TValue, TOutputValue> _transformedValues;



        public InternalDeltaObjectListPropertyToViewModelsStream(
            ICollection<TOutputValue> output,
            IListener<bool> hasValueOutput,
            Expression<Func<TDelta, IDeltaList<TValue>>> getListExpression,
            Func<TValue, Func<ITouchable<TValue>>, TOutputValue> factoryInThreadPool,
            Action<Func<Task>, IEntitiesRepository> asyncTaskRunner,
            IDisposable toDisposeOnComplete)
        {
            _getListExpression = getListExpression;
            _factoryInThreadPool = factoryInThreadPool;
            _asyncTaskRunner = asyncTaskRunner;
            // Холостая прокладка, которую, по идее, в логе можно бы и н епоказывать или вообще исключить?
            // Но пока мы ничего такого сделать не можем, да и вообще сигнатуру метода менять придётся, скорее всего,
            // поэтому просто оставляем обработчик по умолчанию..
            _parentProxy = new DeltaObjectChildProxy<TDelta>(asyncTaskRunner, SubLevelLog);
            _disposeWrapper = new ThreadSafeDisposeWrapper(InternalDispose, this);
            _childName = getListExpression.GetMemberName();
            _getValue = getListExpression.Compile();
            _hasValue = hasValueOutput;
            _output = output;
            if (toDisposeOnComplete != null)
                _localD.Add(toDisposeOnComplete);
        }

        public void OnCompleted()
        {
            lock (_deltaObjectLock)
                _disposeWrapper.RequestDispose();
        }

        public void SetRequestLogHandler(Func<string, string> handler) => _requestLogHandler = handler;

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
                _cachedDeltaList = _getValue(_cachedDeltaObject);
                ConnectDeltaList();
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
                        DisconnectDeltaList();
                        _cachedDeltaObject.UnsubscribePropertyChanged(_childName, OnChangeProperty);
                        _cachedDeltaObject = default;
                    }
                    // А это если Entity бездарно сдохла в процессе
                    else
                    {
                        _hasValue?.OnNext(false);
                        // От _cachedDeltaList Честно отпишемся, потому что мало ли для чего его ещё воспользуют
                        DisconnectDeltaList();
                        // Отписываться от SubscribePropertyChanged больше не нужно, потому как она померла ваще.
                        _cachedDeltaObject = default;
                    }
                }

                _disposeWrapper.FinishWorker();
            }

            // Расылка получателям должно произойти синхронно, потому что мы внутри using-а Entity
            _parentProxy.OnRemove(deltaObject);
        }

        private void ConnectDeltaList()
        {
            if (_cachedDeltaList != null)
            {
                if (_transformedValues == null)
                    _transformedValues = new Dictionary<TValue, TOutputValue>();
                else
                    _transformedValues.Clear();
                foreach (var item in _cachedDeltaList)
                {
                    var outputValue = TransformValue(item);
                    _transformedValues.Add(item, outputValue);
                    _output.Add(outputValue);
                }

                _cachedDeltaList.OnItemAdded += OnItemAdded;
                _cachedDeltaList.OnItemRemoved += OnItemRemoved;
                _hasValue?.OnNext(true);
            }
        }

        private void DisconnectDeltaList()
        {
            if (_cachedDeltaList != null)
            {
                _hasValue?.OnNext(false);
                _cachedDeltaList.OnItemAdded -= OnItemAdded;
                _cachedDeltaList.OnItemRemoved -= OnItemRemoved;
                foreach (var item in _transformedValues)
                    item.Value.Dispose();
                _output.Clear();

                _transformedValues.Clear();
            }
        }

        private Task OnItemAdded(DeltaListChangedEventArgs<TValue> args)
        {
            //Console.WriteLine($"DeltaList.OnItemAdded(key:{args.Key}, value:{args.Value}, OldValue:{args.OldValue})");
            var value = args.Value;

            lock (_deltaObjectLock)
            {
                var outputValue = TransformValue(value);
                _transformedValues[value] = outputValue;
                _output.Add(outputValue);
            }

            return Task.CompletedTask;
        }

        private Task OnItemRemoved(DeltaListChangedEventArgs<TValue> args)
        {
            //Console.WriteLine($"DeltaList.OnItemRemoved(key:{args.Key}, value:{args.Value}, OldValue:{args.OldValue})");
            var value = args.Value;

            lock (_deltaObjectLock)
            {
                if (_transformedValues.TryGetValue(value, out var transformed))
                {
                    _transformedValues.Remove(value);
                    transformed.Dispose();
                    _output.Remove(transformed);
                }
            }

            return Task.CompletedTask;
        }

        private Task OnChangeProperty(EntityEventArgs args)
        {
            if (!_disposeWrapper.StartWorker())
                return Task.CompletedTask;
            lock (_deltaObjectLock)
            {
                DisconnectDeltaList();
                _cachedDeltaList = (IDeltaList<TValue>) Activator
                    .CreateInstance(
                        typeof(DeltaListWrapper<,>)
                            .MakeGenericType(
                                ReplicaTypeRegistry.GetMasterTypeByReplicationLevelType(typeof(TValue)),
                                typeof(TValue)
                            ),
                        args.NewValue
                    );
                ConnectDeltaList();
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

        private TOutputValue TransformValue(TValue value)
        {
            return _factoryInThreadPool(value, () => CreateTouchable(value));
        }

        private ITouchable<TValue> CreateTouchable(TValue child)
        {
            var processor = new DeltaObjectListPropertyItemExtention.Processor<TDelta, TValue>(
                _getListExpression,
                child,
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