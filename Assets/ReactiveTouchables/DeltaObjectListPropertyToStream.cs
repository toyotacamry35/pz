using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;

namespace ReactivePropsNs.Touchables
{
    public static class DeltaObjectListPropertyToStream
    {
        /// <summary>
        /// Инструмент по умолчанию. Отсутствие просоединённой коллекции трактуется как отсутствие в коллекции значений. Если нужно различать эти случаи нужно сделать другой экстеншен.
        /// Перекладывает DeltaObject.DeltaList Property в Stream Не Выводя его в UnityThread
        /// </summary>
        public static IHashSetStream<TValue> ToHashSetStreamThreadPool<TDelta, TValue>(
            this ITouchable<TDelta> source,
            DisposableComposite disposables,
            Expression<Func<TDelta, IDeltaList<TValue>>> getValueExpression
        ) where TDelta : IDeltaObject
        {
            if (typeof(IDeltaObject).IsAssignableFrom(typeof(TValue)))
                throw new Exception($"{nameof(ToHashSetStreamThreadPool)} значения являются {typeof(TValue).GetType().Name} а IDeltaObject нельзя перекладывать напрямую в стрим. Это расширение только для простых значений. Используйте вместо этого ToHashSetStream с конструктором моделей в параметрах");
            var output = new HashSetStream<TValue>();
            disposables.Add(output);
            var processor = new Processor<TDelta, TValue, TValue>(output, null, getValueExpression, value => value, output);
            disposables.Add(processor);
            disposables.Add(source.Subscribe(processor));
            return output;
        }

        public class Processor<TDelta, TValue, TOutputValue> : IToucher<TDelta>, IIsDisposed where TDelta : IDeltaObject
        {
            protected string _childName;
            private Func<TDelta, IDeltaList<TValue>> _getValue;
            private IListener<bool> _hasValue;
            protected IDeltaList<TValue> _cachedDeltaList = null;
            protected Dictionary<TValue, TOutputValue> _transformedValues;
            private Func<TValue, TOutputValue> _transformValue;

            protected object _deltaObjectLock = new object();
            protected TDelta cachedDeltaObject = default;
            protected ThreadSafeDisposeWrapper _disposeWrapper = null;

            private ICollection<TOutputValue> _output; // Локальный опущенный в UnityThread откэшированный клон внешней недоступной конструкции.

            protected DisposableComposite internalDisposedCollection = new DisposableComposite();

            public Processor(ICollection<TOutputValue> output, IListener<bool> hasValueOutput, Expression<Func<TDelta, IDeltaList<TValue>>> getPropertyExpression, Func<TValue, TOutputValue> transformValue, IDisposable toDisposeOnComplete)
                : this(output, hasValueOutput, getPropertyExpression.GetMemberName(), getPropertyExpression.Compile(), transformValue, toDisposeOnComplete) { }

            public Processor(ICollection<TOutputValue> output, IListener<bool> hasValueOutput, string childName, Func<TDelta, IDeltaList<TValue>> getPropertyExpression, Func<TValue, TOutputValue> transformValue, IDisposable toDisposeOnComplete)
            {
                _disposeWrapper = new ThreadSafeDisposeWrapper(InternalDispose, this);
                _childName = childName;
                _getValue = getPropertyExpression;
                _hasValue = hasValueOutput;
                _output = output;
                _transformValue = transformValue;
                if (toDisposeOnComplete != null)
                    internalDisposedCollection.Add(toDisposeOnComplete);
            }
            protected virtual TOutputValue TransformValue(TValue value)
            {
                return _transformValue(value);
            }
            protected virtual void GetRidOfValue(TOutputValue outputValue)
            {
            }

            public virtual void OnAdd(TDelta deltaObject)
            {
                if (!_disposeWrapper.StartWorker())
                    return;
                lock (_deltaObjectLock)
                {
                    cachedDeltaObject = deltaObject;
                    deltaObject.SubscribePropertyChanged(_childName, OnChangeProperty);
                    _cachedDeltaList = _getValue(cachedDeltaObject);
                    ConnectDeltaList();
                }

                _disposeWrapper.FinishWorker();
            }

            public virtual void OnRemove(TDelta deltaObject)
            {
                if (!_disposeWrapper.StartWorker())
                    return;
                lock (_deltaObjectLock)
                {
                    if (deltaObject != null) // Это если мы штатно отписываемся
                    {
                        if (_hasValue != null)
                            _hasValue.OnNext(false);
                        DisconnectDeltaList();
                        cachedDeltaObject.UnsubscribePropertyChanged(_childName, OnChangeProperty);
                        cachedDeltaObject = default;
                    }
                    else // А это если Entity бездарно сдохла в процессе
                    {
                        if (_hasValue != null)
                            _hasValue.OnNext(false);
                        DisconnectDeltaList(); // От _cachedDeltaList Честно отпишемся, потому что мало ли для чего его ещё воспользуют
                        cachedDeltaObject = default; // Отписываться от SubscribePropertyChanged больше не нужно, потому как она померла ваще.
                    }
                }

                _disposeWrapper.FinishWorker();
            }
            private void ConnectDeltaList()
            {
                if (_cachedDeltaList != null)
                {
                    if (_transformedValues == null)
                        _transformedValues = new Dictionary<TValue, TOutputValue>();
                    for (int i = 0; i < _cachedDeltaList.Count; i++)
                    {
                        var outputValue = TransformValue(_cachedDeltaList[i]);
                        _transformedValues.Add(_cachedDeltaList[i], outputValue);
                        _output.Add(outputValue);
                    }
                    _cachedDeltaList.OnItemAdded += OnItemAdded;
                    _cachedDeltaList.OnItemRemoved += OnItemRemoved;
                    if (_hasValue != null)
                        _hasValue.OnNext(true);
                }
            }
            private void DisconnectDeltaList()
            {
                if (_cachedDeltaList != null)
                {
                    if (_hasValue != null)
                        _hasValue.OnNext(false);
                    _cachedDeltaList.OnItemAdded -= OnItemAdded;
                    _cachedDeltaList.OnItemRemoved -= OnItemRemoved;
                    foreach (var item in _transformedValues)
                        GetRidOfValue(item.Value);
                    _transformedValues.Clear();
                    _output.Clear();
                }
            }
            private Task OnItemAdded(DeltaListChangedEventArgs<TValue> args)
            {
                //Console.WriteLine($"DeltaList.OnItemAdded(key:{args.Key}, value:{args.Value}, OldValue:{args.OldValue})");
                lock (_deltaObjectLock)
                {
                    var outputValue = TransformValue(args.Value);
                    _transformedValues.Add(args.Value, outputValue);
                    _output.Add(outputValue);
                }
                return Task.CompletedTask;
            }
            private Task OnItemRemoved(DeltaListChangedEventArgs<TValue> args)
            {
                //Console.WriteLine($"DeltaList.OnItemRemoved(key:{args.Key}, value:{args.Value}, OldValue:{args.OldValue})");
                lock (_deltaObjectLock)
                {
                    if (_transformedValues.TryGetValue(args.Value, out var transformed))
                    {
                        _transformedValues.Remove(args.Value);
                        GetRidOfValue(transformed);
                        _output.Remove(transformed);
                    }
                }
                return Task.CompletedTask;
            }

            /// <summary>
            /// Боря утверждает, что это событие к нам вообще никогда не придёт в связи со структурой репозитория, типа сериализатор не умеет понимать, что List сменился.
            /// Но мы на всякий случай эту обработку оставим. Проблема в том, что никто не гарантирует очерёдность прихода этого эвента и внутренних эвентов DeltaList.
            /// От всего этого назщищает только протобаф, который будет вот это всё превращать просто в накатывание новых данных на существующий элемент и всякое разное,
            /// типа сначала поменять многое в List а потом его удалить протобаф вот это всё просто не сумеет отследить.
            /// </summary>
            private Task OnChangeProperty(EntityEventArgs args)
            {
                if (!_disposeWrapper.StartWorker())
                    return Task.CompletedTask;
                lock (_deltaObjectLock)
                {
                    DisconnectDeltaList();
                    _cachedDeltaList = (IDeltaList<TValue>)args.NewValue;
                    ConnectDeltaList();
                }
                _disposeWrapper.FinishWorker();
                return Task.CompletedTask;
            }

            public void OnCompleted()
            {
                lock (_deltaObjectLock)
                    _disposeWrapper.RequestDispose();
            }

            protected Func<string, string> requestLogHandler = null;
            public void SetRequestLogHandler(Func<string, string> handler) => requestLogHandler = handler;
            public virtual string DeepLog(string prefix)
            {
                string msg = $"{prefix}ITouchable<{typeof(TDelta).NiceName()}.ToListStream(_childName):";
                if (_disposeWrapper.IsDisposed)
                    return msg + " DISPOSED";
                string sources = requestLogHandler != null ? "\n" + requestLogHandler(prefix + '\t') : "";
                if (EqualityComparer<TDelta>.Default.Equals(cachedDeltaObject, default))

                    return msg + " DISCONNECTED" + sources;
                return msg + " CONNECTED " + _cachedDeltaList.ToShortLog() + sources;
            }

            public bool IsDisposed => _disposeWrapper.IsDisposed;

            public void Dispose()
            {
                _disposeWrapper.RequestDispose();
            }

            private void InternalDispose()
            {
                cachedDeltaObject = default;
                internalDisposedCollection.Dispose();
                _getValue = null;
                _hasValue?.OnCompleted();
                _hasValue = null;
            }
        }
    }
}
