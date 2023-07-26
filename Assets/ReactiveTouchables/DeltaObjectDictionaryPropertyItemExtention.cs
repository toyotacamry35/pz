using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;

namespace ReactivePropsNs.Touchables
{
    public static class DeltaObjectDictionaryPropertyItemExtention
    {
        /// <summary> Очень нестандартный компонент для очень редкого использования. Он в качестве параметра получае кэшированную ссылку на DeltaObject </summary>
        public class Processor<TDelta, TKey, TValue> : IToucher<TDelta>, ITouchable<TValue>, IIsDisposed where TDelta : IDeltaObject where TValue : IDeltaObject
        {
            public bool logEverithing = false;

            private EqualityComparer<TValue> _itemComparer = EqualityComparer<TValue>.Default;

            private ThreadSafeDisposeWrapper _disposeWrapper = null;

            private string _propertyName;
            private Func<TDelta, IDeltaDictionary<TKey, TValue>> _getValue;

            private object _deltaObjectLock = new object();
            private TDelta _cachedParent = default;
            private IDeltaDictionary<TKey, TValue> _cachedDeltaDictionary = null;

            private TValue _requiredChild;
            private bool _childSended = false;

            private DeltaObjectChildProxy<TValue> _childProxy;
            //private TValue _cachedChild = default;

            private IDisposable _toDisposeOnComplete = null;

            private Func<string, string> _reciveStreamLog = null;

            public Processor(Expression<Func<TDelta, IDeltaDictionary<TKey,TValue>>> getPropertyExpression, TValue requiredChild, Action<Func<Task>, IEntitiesRepository> asyncTaskRunner)
            {
                _disposeWrapper = new ThreadSafeDisposeWrapper(InternalDispose, this);
                _requiredChild = requiredChild;
                _childProxy = new DeltaObjectChildProxy<TValue>(asyncTaskRunner, DeepLog);
                _propertyName = getPropertyExpression.GetMemberName();
                _getValue = getPropertyExpression.Compile();
            }

            public void OnAdd(TDelta deltaObject)
            {
                if (!_disposeWrapper.StartWorker())
                    return;
                if (logEverithing)
                    Console.WriteLine($"{GetType().NiceName()}.OnAdd({deltaObject})");
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
                if (logEverithing)
                    Console.WriteLine($"{GetType().NiceName()}.OnRemove({deltaObject})");
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
                //Console.WriteLine($"{GetType().NiceName()}.{nameof(ConnectDeltaDictionary)}() // _cachedDeltaDictionary(Count={_cachedDeltaDictionary.Count}): {_cachedDeltaDictionary}");
                if (_cachedDeltaDictionary != null)
                {
                    if (logEverithing)
                        Console.WriteLine($"{GetType().NiceName()}.{nameof(ConnectDeltaDictionary)}() {_cachedDeltaDictionary.Select(i => $"{{{i.Key},{i.Value}}}").Aggregate((a, i) => a + "," + i)}");
                    foreach(var item in _cachedDeltaDictionary)
                        if (_itemComparer.Equals(_requiredChild, item.Value))
                        {
                            _childSended = true;
                            _childProxy.OnAdd(_requiredChild);
                            break;
                        }
                    _cachedDeltaDictionary.OnItemAddedOrUpdated += OnItemAddedOrUpdated;
                    _cachedDeltaDictionary.OnItemRemoved += OnItemRemoved;
                }
            }
            private void DisconnectDeltaDictionary(bool entityDestroyed)
            {
                if (logEverithing)
                    Console.WriteLine($"{GetType().NiceName()}.{nameof(DisconnectDeltaDictionary)}({entityDestroyed}) //" + (entityDestroyed ? "" : $" _cachedDeltaDictionary(Count={_cachedDeltaDictionary.Count}): {_cachedDeltaDictionary}") + "\n" + DeepLog("Disconnect"));
                if (_cachedDeltaDictionary != null)
                {
                    //Console.WriteLine($"{GetType().NiceName()}.{nameof(DisconnectDeltaDictionary)}({entityDestroyed}) {_cachedDeltaDictionary.Select(i => $"{{{i.Key},{i.Value}}}").Aggregate((a, i) => a + "," + i)}");
                    _cachedDeltaDictionary.OnItemAddedOrUpdated -= OnItemAddedOrUpdated;
                    _cachedDeltaDictionary.OnItemRemoved -= OnItemRemoved;
                    if (_childSended)
                    {
                        _childSended = false;
                        _childProxy.OnRemove(entityDestroyed ? default : _requiredChild);
                    }
                }
            }
            private Task OnItemAddedOrUpdated(DeltaDictionaryChangedEventArgs<TKey, TValue> args)
            {
                if (!_disposeWrapper.StartWorker())
                    return Task.CompletedTask;
                lock (_deltaObjectLock)
                {
                    bool processed = false;
                    if (_childSended && _itemComparer.Equals(_requiredChild, args.OldValue))
                    {
                        if (logEverithing)
                            Console.WriteLine($"FILTERED EVENT:{GetType().NiceName()}.{nameof(OnItemAddedOrUpdated)}({args.Value}) // REMOVED");
                        _childSended = false;
                        _childProxy.OnRemove(_requiredChild);
                        processed = true;
                    }
                    if (_itemComparer.Equals(_requiredChild, args.Value))
                    {
                        if (logEverithing)
                            Console.WriteLine($"FILTERED EVENT:{GetType().NiceName()}.{nameof(OnItemAddedOrUpdated)}({args.Value}) // ADDED");
                        _childSended = true;
                        _childProxy.OnAdd(args.Value);
                        processed = true;
                    }
                    if (!processed && logEverithing)
                        Console.WriteLine($"REJECTED EVENT:{GetType().NiceName()}.{nameof(OnItemRemoved)}({args.Value})");
                }
                _disposeWrapper.FinishWorker();
                return Task.CompletedTask;
            }
            private Task OnItemRemoved(DeltaDictionaryChangedEventArgs<TKey, TValue> args)
            {
                if (!_disposeWrapper.StartWorker())
                    return Task.CompletedTask;
                lock (_deltaObjectLock)
                {
                    if (_itemComparer.Equals(_requiredChild, args.Value))
                    {
                        if (logEverithing)
                            Console.WriteLine($"FILTERED EVENT:{GetType().NiceName()}.{nameof(OnItemRemoved)}({args.Value}) // REMOVED");
                        _childSended = true;
                        _childProxy.OnRemove(_requiredChild);
                    }
                    else
                        if (logEverithing)
                            Console.WriteLine($"REJECTED EVENT:{GetType().NiceName()}.{nameof(OnItemRemoved)}({args.Value})");
                }
                _disposeWrapper.FinishWorker();
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
                Console.WriteLine($"!!!ALERT!!!ALERT!!!ALERT!!! {GetType().NiceName()}[{_requiredChild}].{nameof(OnChangeProperty)}({args.PropertyName}, {args.PropertyAddress}, {args.NewValue}, {args.Sender})");
                ReactiveLogs.Logger.IfError()?.Message($"!!!ALERT!!!ALERT!!!ALERT!!! {GetType().NiceName()}[{_requiredChild}].{nameof(OnChangeProperty)}({args.PropertyName}, {args.PropertyAddress}, {args.NewValue}, {args.Sender})").Write();
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
                string header = $"{prefix}ITouchable<{typeof(TDelta).Name}>.DictionaryItem({_propertyName}, {_requiredChild})";
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
                    if (!_childSended)
                        return $"{header} CHILD DISCONNECTED DICTIONARY {_cachedDeltaDictionary.ToShortLog()}{sources}";
                    else
                        return $"{header} CHILD CONNECTED DICTIONARY {_cachedDeltaDictionary.ToShortLog()}{sources}";
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
                _requiredChild = default;
                _cachedDeltaDictionary = null;
                _childProxy.Dispose();
                _toDisposeOnComplete?.Dispose();
                _getValue = null;
            }
        }
    }
}
