using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using SharedCode.Serializers;

namespace ReactivePropsNs.Touchables
{
    public static class DeltaObjectListPropertyItemExtention {
        public static ITouchable<TChild> ListItemTouchable<TParent, TChild>(this ITouchable<TParent> source, ICollection<IDisposable> disposables, Expression<Func<TParent, IDeltaList<TChild>>> getPropertyExpression, TChild directRef) where TParent : IDeltaObject where TChild : IDeltaObject  {
            var processor = new Processor<TParent, TChild>(getPropertyExpression, directRef, (taskFactory, repository) => AsyncUtils.RunAsyncTask(taskFactory, repository));
            disposables.Add(processor);
            disposables.Add(source.Subscribe(processor));
            return processor;
        }

        /// <summary> Очень нестандартный компонент для очень редкого использования. Он в качестве параметра получае кэшированную ссылку на DeltaObject </summary>
        public class Processor<TDelta, TValue> : IToucher<TDelta>, ITouchable<TValue>, IIsDisposed where TDelta : IDeltaObject where TValue : IDeltaObject
        {
            public bool logEverithing = false;

            private EqualityComparer<TValue> _itemComparer = EqualityComparer<TValue>.Default;

            private ThreadSafeDisposeWrapper _disposeWrapper = null;

            private string _propertyName;
            private Func<TDelta, IDeltaList<TValue>> _getValue;

            private object _deltaObjectLock = new object();
            private TDelta _cachedParent = default;
            private IDeltaList<TValue> _cachedDeltaList = null;

            private TValue _requiredChild;
            private bool _childSended = false;

            private DeltaObjectChildProxy<TValue> _childProxy;
            //private TValue _cachedChild = default;

            private IDisposable _toDisposeOnComplete = null;

            private Func<string, string> _reciveStreamLog = null;

            public Processor(Expression<Func<TDelta, IDeltaList<TValue>>> getPropertyExpression, TValue requiredChild, Action<Func<Task>, IEntitiesRepository> asyncTaskRunner)
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
                    _cachedDeltaList = _getValue(_cachedParent);
                    ConnectDeltaList();
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
                        DisconnectDeltaList(false);
                        _cachedDeltaList = default;
                        _cachedParent.UnsubscribePropertyChanged(_propertyName, OnChangeProperty);
                        _cachedParent = default;
                    }
                    else // А это если Entity бездарно сдохла в процессе
                    {
                        DisconnectDeltaList(true); // От _cachedDeltaDictionary Честно отпишемся, потому что мало ли для чего его ещё воспользуют
                        _cachedDeltaList = default;
                        _cachedParent = default; // Отписываться от SubscribePropertyChanged больше не нужно, потому как она померла ваще.
                    }
                }

                _disposeWrapper.FinishWorker();
            }
            private void ConnectDeltaList()
            {
                Console.WriteLine($"{GetType().NiceName()}.ConnectDeltaList() // _cachedDeltaList(Count={_cachedDeltaList.Count}): {_cachedDeltaList}");
                if (_cachedDeltaList != null)
                {
                    if (logEverithing)
                        Console.WriteLine($"{GetType().NiceName()}.{nameof(ConnectDeltaList)}() {_cachedDeltaList.Select(i => $"{i}").Aggregate((a, i) => a + "," + i)}");
                    if (_cachedDeltaList.Contains(_requiredChild))
                    { // Если нет ничего, в словаре, то ничего отдавать и не нужно.
                        _childSended = true;
                        _childProxy.OnAdd(_requiredChild);
                    }
                    _cachedDeltaList.OnItemAdded += OnItemAdded;
                    _cachedDeltaList.OnItemRemoved += OnItemRemoved;
                }
            }
            private void DisconnectDeltaList(bool entityDestroyed)
            {
                if (logEverithing)
                    Console.WriteLine($"{GetType().NiceName()}.DisconnectDeltaList({entityDestroyed}) //" + (entityDestroyed ? "" : $" _cachedDeltaList(Count={_cachedDeltaList.Count}): {_cachedDeltaList}") + "\n" + DeepLog("Disconnect"));
                if (_cachedDeltaList != null)
                {
                    if (logEverithing)
                        Console.WriteLine($"{GetType().NiceName()}.{nameof(DisconnectDeltaList)}({entityDestroyed}) {_cachedDeltaList.Select(i => $"{i}").Aggregate((a, i) => a + "," + i)}");
                    _cachedDeltaList.OnItemAdded -= OnItemAdded;
                    _cachedDeltaList.OnItemRemoved -= OnItemRemoved;
                    if (_childSended)
                    {
                        _childSended = false;
                        _childProxy.OnRemove(entityDestroyed ? default : _requiredChild);
                    }
                }
            }
            private Task OnItemAdded(DeltaListChangedEventArgs<TValue> args)
            {
                lock (_deltaObjectLock)
                {
                    if (_itemComparer.Equals(_requiredChild, args.Value))
                    {
                        if (logEverithing)
                            Console.WriteLine($"FILTERED EVENT:{GetType().NiceName()}.{nameof(OnItemAdded)}({args.Value}");
                        _childSended = true;
                        _childProxy.OnAdd(args.Value);
                    }
                    else
                        if (logEverithing)
                            Console.WriteLine($"REJECTED EVENT:{GetType().NiceName()}.OnItemAdded({args.Value})");
                }
                return Task.CompletedTask;
            }
            private Task OnItemRemoved(DeltaListChangedEventArgs<TValue> args)
            {
                lock (_deltaObjectLock)
                {
                    if (_itemComparer.Equals(_requiredChild, args.Value))
                    {
                        if (logEverithing)
                            Console.WriteLine($"FILTERED EVENT:{GetType().NiceName()}.{nameof(OnItemRemoved)}({args.Value})");
                        _childSended = false;
                        _childProxy.OnRemove(_requiredChild);
                    }
                    else
                        if (logEverithing)
                        Console.WriteLine($"REJECTED EVENT:{GetType().NiceName()}.OnItemRemoved({args.Value})");
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
                Console.WriteLine($"!!!ALERT!!!ALERT!!!ALERT!!! {GetType().NiceName()}[{_requiredChild}].{nameof(OnChangeProperty)}({args.PropertyName}, {args.PropertyAddress}, {args.NewValue}, {args.Sender})");
                ReactiveLogs.Logger.IfError()?.Message($"!!!ALERT!!!ALERT!!!ALERT!!! {GetType().NiceName()}[{_requiredChild}].{nameof(OnChangeProperty)}({args.PropertyName}, {args.PropertyAddress}, {args.NewValue}, {args.Sender})").Write();
                if (!_disposeWrapper.StartWorker())
                    return Task.CompletedTask;
                lock (_deltaObjectLock)
                {
                    DisconnectDeltaList(false);
                    _cachedDeltaList = (IDeltaList<TValue>)args.NewValue;
                    ConnectDeltaList();
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
                string header = $"{prefix}ITouchable<{typeof(TDelta).Name}>.ListItem({_propertyName}, {_requiredChild})";
                if (_disposeWrapper.IsDisposed)
                    return header + " DISPOSED";
                lock (_deltaObjectLock)
                {
                    string sources = _reciveStreamLog == null ? "" : $"\n{_reciveStreamLog(prefix + '\t')}";
                    if (_cachedParent == null)
                        return $"{header} DELTA OBJECT DISCONNECTED{sources}";
                    if (_cachedDeltaList == null)
                        return $"{header} LIST DISCONNECTED{sources}";
                    var keys = _cachedDeltaList.ToShortLog();
                    if (!_childSended)
                        return $"{header} CHILD DISCONNECTED LIST {_cachedDeltaList.ToShortLog()}{sources}";
                    else
                        return $"{header} CHILD CONNECTED LIST {_cachedDeltaList.ToShortLog()}{sources}";
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
                _cachedDeltaList = null;
                _childProxy.Dispose();
                _toDisposeOnComplete?.Dispose();
                _getValue = null;
            }
        }
    }
}
