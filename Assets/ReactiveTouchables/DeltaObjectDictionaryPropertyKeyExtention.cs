using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using SharedCode.Serializers;

namespace ReactivePropsNs.Touchables
{
    public static class DeltaObjectDictionaryPropertyKeyExtention
    {
        public static ITouchable<TValue> KeyThreadPool<TDelta, TKey, TValue>(this ITouchable<TDelta> source, ICollection<IDisposable> disposables, Expression<Func<TDelta, IDeltaDictionary<TKey, TValue>>> getDictionaryExpression, TKey key) where TDelta : class, IDeltaObject where TValue : class, IDeltaObject
        {
            var processor = new DeltaObjectDictionaryPropertyKeyProcessor<TDelta, TKey, TValue>(getDictionaryExpression, key, (taskFactory, repository) => AsyncUtils.RunAsyncTask(taskFactory, repository));
            disposables.Add(processor);
            disposables.Add(source.Subscribe(processor));
            return processor;
        }
        public class DeltaObjectDictionaryPropertyKeyProcessor<TDelta, TKey, TValue> : IToucher<TDelta>, ITouchable<TValue>, IIsDisposed where TDelta : class, IDeltaObject where TValue : class, IDeltaObject
        {
            private EqualityComparer<TKey> _keyComparer = EqualityComparer<TKey>.Default;

            private TKey _key;
            private string _propertyName;
            private Func<TDelta, IDeltaDictionary<TKey, TValue>> _getValue;

            private object _deltaObjectLock = new object();
            private TDelta _cachedParent = default;

            private IDeltaDictionary<TKey, TValue> _cachedDeltaDictionary = null;
            private DeltaObjectChildProxy<TValue> _childProxy;
            private TValue _cachedChild = default;

            private ThreadSafeDisposeWrapper _disposeWrapper = null;
            private IDisposable _toDisposeOnComplete = null;

            private Func<string, string> _reciveStreamLog = null;

            public DeltaObjectDictionaryPropertyKeyProcessor(Expression<Func<TDelta, IDeltaDictionary<TKey, TValue>>> getPropertyExpression, TKey key, Action<Func<Task>, IEntitiesRepository> asyncTaskRunner)
            {
                _key = key;
                _childProxy = new DeltaObjectChildProxy<TValue>(asyncTaskRunner, DeepLog);
                _disposeWrapper = new ThreadSafeDisposeWrapper(InternalDispose, this);
                _propertyName = getPropertyExpression.GetMemberName();
                _getValue = getPropertyExpression.Compile();
            }

            public void OnAdd(TDelta deltaObject)
            {
                if (!_disposeWrapper.StartWorker())
                    return;
                lock (_deltaObjectLock)
                {
                    if (_cachedParent != null)
                        throw new Exception($"{this}.{nameof(OnAdd)}({deltaObject}) // Но мы уже подключены к {_cachedParent}");
                    _cachedParent = deltaObject;
                    deltaObject.SubscribePropertyChanged(_propertyName, OnChangeProperty);
                    _cachedDeltaDictionary = _getValue(_cachedParent);
                    ConnectDeltaDictionary();
                }

                _disposeWrapper.FinishWorker();
            }

            public void OnRemove(TDelta deltaObject)
            {
                if (!_disposeWrapper.StartWorker())
                    return;
                lock (_deltaObjectLock)
                {
                    if (_cachedParent == null)
                        throw new Exception($"{this}.{nameof(OnRemove)}({deltaObject}) // Но мы ни к чему сейчас не подключены");
                    if (deltaObject != null) // Это если мы штатно отписываемся
                    {
                        DisconnectDeltaDictionary(false);
                        _cachedDeltaDictionary = default;
                        _cachedParent.UnsubscribePropertyChanged(_propertyName, OnChangeProperty);
                        _cachedParent = default;
                    }
                    else // А это если Entity бездарно сдохла в процессе
                    {
                        DisconnectDeltaDictionary(true); // От _cachedDeltaDictionary Честно отпишемся, потому что мало ли для чего его ещё воспользуют
                        _cachedDeltaDictionary = default;
                        _cachedParent = default; // Отписываться от SubscribePropertyChanged больше не нужно, потому как она померла ваще.
                    }
                }

                _disposeWrapper.FinishWorker();
            }
            private void ConnectDeltaDictionary()
            {
                if (_cachedDeltaDictionary != null)
                {
                    //Console.WriteLine($"{GetType().NiceName()}[{_key}].{nameof(ConnectDeltaDictionary)}() {_cachedDeltaDictionary.ToKeysStringUnderLock(4)}");
                    if (_cachedDeltaDictionary.TryGetValue(_key, out var value)) // Если нет ничего, в словаре, то ничего отдавать и не нужно.
                        _childProxy.OnAdd(_cachedChild = value);
                    _cachedDeltaDictionary.OnItemAddedOrUpdated += OnItemAddedOrUpdated;
                    _cachedDeltaDictionary.OnItemRemoved += OnItemRemoved;
                }
            }
            private void DisconnectDeltaDictionary(bool entityDestroyed)
            {
                if (_cachedDeltaDictionary != null)
                {
                   //Console.WriteLine($"{GetType().NiceName()}[{_key}].{nameof(DisconnectDeltaDictionary)}({entityDestroyed}) {(entityDestroyed ? "entity destroyed" : _cachedDeltaDictionary.ToKeysStringUnderLock(4))}");
                    _cachedDeltaDictionary.OnItemAddedOrUpdated -= OnItemAddedOrUpdated;
                    _cachedDeltaDictionary.OnItemRemoved -= OnItemRemoved;
                    if (_cachedChild != default)
                    {
                        var previouse = _cachedChild;
                        _cachedChild = default;
                        _childProxy.OnRemove(entityDestroyed ? default : previouse);
                    }
                }
            }
            private Task OnItemAddedOrUpdated(DeltaDictionaryChangedEventArgs<TKey, TValue> args)
            {
                lock (_deltaObjectLock)
                {
                    if (_keyComparer.Equals(_key, args.Key))
                    {
                        //Console.WriteLine($"{GetType().NiceName()}[{_key}].{nameof(OnItemAddedOrUpdated)}({args.Key}, {args.Value}, {args.OldValue})");
                        if (_cachedChild != default)
                            _childProxy.OnRemove(_cachedChild);
                        if (args.Value != default)
                            _childProxy.OnAdd(_cachedChild = args.Value);
                    }
                }
                return Task.CompletedTask;
            }
            private Task OnItemRemoved(DeltaDictionaryChangedEventArgs<TKey, TValue> args)
            {
                lock (_deltaObjectLock)
                {
                    if (_keyComparer.Equals(_key, args.Key))
                        //Console.WriteLine($"{GetType().NiceName()}[{_key}].{nameof(OnItemRemoved)}({args.Key}, {args.Value}, {args.OldValue})");
                    if (_cachedChild != null)
                        {
                            var previouse = _cachedChild;
                            _cachedChild = default;
                            _childProxy.OnRemove(previouse);
                        }
                }
                return Task.CompletedTask;
            }

            /// <summary>
            /// Боря утверждает, что это событие к нам вообще никогда не придёт в связи со структурой репозитория, типа сериализатор не умеет понимать, что Dictionary сменился.
            /// Но мы на всякий случай эту обработку оставим. Проблема в том, что никто не гарантирует очерёдность прихода этого эвента и внутренних эвентов DeltaDictionary.
            /// От всего этого назщищает только протобаф, который будет вот это всё превращать просто в накатывание новых данных на существующий элемент и всякое разное,
            /// типа сначала поменять многое в Dictionary а потом его удалить протобаф вот это всё просто не сумеет отследить.
            /// </summary>
            private Task OnChangeProperty(EntityEventArgs args)
            {
                Console.WriteLine($"!!!ALERT!!!ALERT!!!ALERT!!! {GetType().NiceName()}[{_key}].{nameof(OnChangeProperty)}({args.PropertyName}, {args.PropertyAddress}, {args.NewValue}, {args.Sender})");
                ReactiveLogs.Logger.IfError()?.Message($"!!!ALERT!!!ALERT!!!ALERT!!! {GetType().NiceName()}[{_key}].{nameof(OnChangeProperty)}({args.PropertyName}, {args.PropertyAddress}, {args.NewValue}, {args.Sender})").Write();
                if (!_disposeWrapper.StartWorker())
                    return Task.CompletedTask;
                lock (_deltaObjectLock)
                {
                    DisconnectDeltaDictionary(false);
                    _cachedDeltaDictionary = (IDeltaDictionary<TKey, TValue>)args.NewValue;
                    ConnectDeltaDictionary();
                }
                _disposeWrapper.FinishWorker();
                return Task.CompletedTask;
            }

            public IDisposable Subscribe(IToucher<TValue> toucher)
            {
                //Console.WriteLine($"{GetType().Name}.{nameof(Subscribe)}({toucher})".AddTime());
                return _childProxy.Subscribe(toucher);
            }

            public void OnCompleted()
            {
                //Console.WriteLine($"{GetType().Name}.{nameof(OnCompleted)}()".AddTime());
                lock (_deltaObjectLock)
                    _disposeWrapper.RequestDispose();
            }

            public void SetRequestLogHandler(Func<string, string> handler)
            {
                _reciveStreamLog = handler;
            }

            public string DeepLog(string prefix)
            {
                string header = $"{prefix}ITouchable<{typeof(TDelta).Name}>.Key({_propertyName}, {_key})";
                if (_disposeWrapper.IsDisposed)
                    return header + " DISPOSED";
                lock (_deltaObjectLock)
                {
                    string sources = _reciveStreamLog == null ? "" : $"\n{_reciveStreamLog(prefix + '\t')}";
                    if (_cachedParent == null)
                        return $"{header} DELTA OBJECT DISCONNECTED{sources}";
                    if (_cachedDeltaDictionary == null)
                        return $"{header} DICTIONARY DISCONNECTED{sources}";
                    var keys = _cachedDeltaDictionary.ToShortLog();
                    if (_cachedChild == null)
                        return $"{header} CHILD DISCONNECTED DICTIONARY {sources}";
                    else
                        return $"{header} CHILD:{_cachedChild} DICTIONARY {_cachedDeltaDictionary.ToShortLog()}{sources}";
                }
            }

            public bool IsDisposed => _disposeWrapper.IsDisposed;

            public void Dispose()
            {
                _disposeWrapper.RequestDispose();
            }

            private void InternalDispose()
            {
                _cachedParent = default;
                _cachedChild = default;
                _cachedDeltaDictionary = null;
                _childProxy.Dispose();
                _toDisposeOnComplete?.Dispose();
                _getValue = null;
            }
        }
    }
}
