using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;

namespace ReactivePropsNs.Touchables
{
    public static class DeltaObjectDictionaryPropertyToStream
    {
        /// <summary>
        /// Инструмент по умолчанию. Отсутствие просоединённой коллекции трактуется как отсутствие в коллекции значений. Если нужно различать эти случаи нужно сделать другой экстеншен.
        /// Перекладывает DeltaObject.DeltaDictionary Property в Stream Не Выводя его в UnityThread
        /// </summary>
        public static IDictionaryStream<TKey, TValue> ToDictionaryStreamThreadPool<TDelta, TKey, TValue>(
            this ITouchable<TDelta> source,
            DisposableComposite disposables,
            Expression<Func<TDelta, IDeltaDictionary<TKey, TValue>>> getValueExpression
        ) where TDelta : class, IDeltaObject
        {
            if (typeof(IDeltaObject).IsAssignableFrom(typeof(TValue)))
                throw new Exception($"{nameof(ToDictionaryStreamThreadPool)} значения являются {typeof(TValue).GetType().Name} а IDeltaObject нельзя перекладывать напрямую в стрим. Это расширение только для простых значений. Используйте вместо этого ToDictionaryStream с конструктором моделей в параметрах");
            var output = new DictionaryStream<TKey, TValue>();
            disposables.Add(output);
            var processor = new InternalDeltaObjectDictionaryPropertyToStream<TDelta, TKey, TValue, TValue>(output, null, getValueExpression, (key, value) => value, output);
            disposables.Add(processor);
            disposables.Add(source.Subscribe(processor));
            return output;
        }

        public class InternalDeltaObjectDictionaryPropertyToStream<TDelta, TKey, TValue, TOutputValue> : IToucher<TDelta>, IIsDisposed where TDelta : class, IDeltaObject
        {
            protected string _childName;
            private Func<TDelta, IDeltaDictionary<TKey, TValue>> _getValue;
            private IListener<bool> _hasValue;
            protected IDeltaDictionary<TKey, TValue> _cachedDeltaDictionary = null;
            protected Dictionary<TKey, TOutputValue> _transformedValues = null;
            private Func<TKey, TValue, TOutputValue> _transformValue;

            protected object _deltaObjectLock = new object();
            protected TDelta cachedDeltaObject = default;
            protected ThreadSafeDisposeWrapper _disposeWrapper = null;

            private IDictionary<TKey, TOutputValue> _output; // Локальный опущенный в UnityThread откэшированный клон внешней недоступной конструкции.

            protected DisposableComposite internalDisposedCollection = new DisposableComposite();

            public InternalDeltaObjectDictionaryPropertyToStream(IDictionary<TKey, TOutputValue> output, IListener<bool> hasValueOutput, Expression<Func<TDelta, IDeltaDictionary<TKey, TValue>>> getPropertyExpression, Func<TKey, TValue, TOutputValue> transformValue, IDisposable toDisposeOnComplete)
                : this(output, hasValueOutput, getPropertyExpression.GetMemberName(), getPropertyExpression.Compile(), transformValue, toDisposeOnComplete) { }

            public InternalDeltaObjectDictionaryPropertyToStream(IDictionary<TKey, TOutputValue> output, IListener<bool> hasValueOutput, string childName, Func<TDelta, IDeltaDictionary<TKey, TValue>> getPropertyExpression, Func<TKey, TValue, TOutputValue> transformValue, IDisposable toDisposeOnComplete)
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
            protected virtual TOutputValue TransformValue(TKey key, TValue value) {
                return _transformValue(key, value);
            }
            protected virtual void GetRidOfValue(TKey key, TOutputValue outputValue)
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
                    _cachedDeltaDictionary = _getValue(cachedDeltaObject);
                    ConnectDeltaDictionary();
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
                        DisconnectDeltaDictionary();
                        cachedDeltaObject.UnsubscribePropertyChanged(_childName, OnChangeProperty);
                        cachedDeltaObject = default;
                    }
                    else // А это если Entity бездарно сдохла в процессе
                    {
                        if (_hasValue != null)
                            _hasValue.OnNext(false);
                        DisconnectDeltaDictionary(); // От _cachedDeltaDictionary Честно отпишемся, потому что мало ли для чего его ещё воспользуют
                        cachedDeltaObject = default; // Отписываться от SubscribePropertyChanged больше не нужно, потому как она померла ваще.
                    }
                }

                _disposeWrapper.FinishWorker();
            }
            private void ConnectDeltaDictionary()
            {
                if (_cachedDeltaDictionary != null)
                {
                    _transformedValues = new Dictionary<TKey, TOutputValue>();
                    foreach (var item in _cachedDeltaDictionary)
                    {
                        var transformedValue = TransformValue(item.Key, item.Value);
                        _transformedValues.Add(item.Key, transformedValue);
                        _output.Add(item.Key, transformedValue);
                    }

                    _cachedDeltaDictionary.OnItemAddedOrUpdated += OnItemAddedOrUpdated;
                    _cachedDeltaDictionary.OnItemRemoved += OnItemRemoved;
                    if (_hasValue != null)
                        _hasValue.OnNext(true);
                }
            }
            private void DisconnectDeltaDictionary()
            {
                if (_cachedDeltaDictionary != null)
                {
                    if (_hasValue != null)
                        _hasValue.OnNext(false);
                    _cachedDeltaDictionary.OnItemAddedOrUpdated -= OnItemAddedOrUpdated;
                    _cachedDeltaDictionary.OnItemRemoved -= OnItemRemoved;
                    foreach (var item in _transformedValues)
                        GetRidOfValue(item.Key, item.Value);
                    _output.Clear();
                    
                    _transformedValues = null;
                }
            }
            private Task OnItemAddedOrUpdated(DeltaDictionaryChangedEventArgs<TKey, TValue> args)
            {
                //Console.WriteLine($"DeltaDictionary.OnItemAddedOrUpdated(key:{args.Key}, value:{args.Value}, OldValue:{args.OldValue})");
                if (_transformedValues.TryGetValue(args.Key, out var oldTransformedValue)) // For update
                {
                    GetRidOfValue(args.Key, oldTransformedValue);
                    _transformedValues.Remove(args.Key);
                    _output.Remove(args.Key);
                }

                var transformedValue = TransformValue(args.Key, args.Value);
                _transformedValues[args.Key] = transformedValue;
                _output[args.Key] = transformedValue;
                return Task.CompletedTask;
            }
            private Task OnItemRemoved(DeltaDictionaryChangedEventArgs<TKey, TValue> args)
            {
                //Console.WriteLine($"DeltaDictionary.OnItemRemoved(key:{args.Key}, value:{args.Value}, OldValue:{args.OldValue})");
                if (_transformedValues.TryGetValue(args.Key, out var item)) 
                    GetRidOfValue(args.Key, item);

                _transformedValues.Remove(args.Key);
                _output.Remove(args.Key);
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
                if (!_disposeWrapper.StartWorker())
                    return Task.CompletedTask;
                lock (_deltaObjectLock)
                {
                    DisconnectDeltaDictionary();
                    _cachedDeltaDictionary = (IDeltaDictionary<TKey, TValue>)args.NewValue;
                    ConnectDeltaDictionary();
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
                string msg = $"{prefix}ITouchable<{typeof(TDelta).NiceName()}.ToDictionaryStream(_childName):";
                if (_disposeWrapper.IsDisposed)
                    return msg + " DISPOSED";
                string sources = requestLogHandler != null ? "\n" + requestLogHandler(prefix + '\t') : "";
                if (EqualityComparer<TDelta>.Default.Equals(cachedDeltaObject, default))
                    return msg + " DISCONNECTED" + sources;
                return msg + " CONNECTED " + _cachedDeltaDictionary.ToShortLog() + sources;
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
