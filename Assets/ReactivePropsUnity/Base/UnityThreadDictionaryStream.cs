using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using NLog;
using ReactivePropsNs.Touchables;

namespace ReactivePropsNs
{
    /// <summary>
    /// Посредник Предоставляет интерфейс IDictionary, превращает в вызовы и перекладывает эти вызовы в UnityThread Не нарушая порядок, так чтобы в UnityThread возникла копия.
    /// Доступен для записи только а альтернативных тредах, а для чтения только в UnityThread
    /// </summary>
    public class UnityThreadDictionaryStream<TKey, TValue> : IDictionaryStream<TKey, TValue>, IDictionary<TKey, TValue>, IIsDisposed
    {
        [NotNull] private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        ThreadSafeDisposeWrapper _disposeWrapper;
        private DictionaryStream<TKey, TValue> _unityThreadOutput = new DictionaryStream<TKey, TValue>();
        private object _recivedValuesQueueLock = new object();
        private Queue<DictionaryChange> _recivedValues = new Queue<DictionaryChange>();
        private readonly struct DictionaryChange
        {
            public DictionaryChange(changeType type, TKey key, TValue value)
            {
                this.type = type;
                this.key = key;
                this.value = value;
            }
            public enum changeType { addOrUpdate, remove, removePair, clear }
            public readonly changeType type;
            public readonly TKey key;
            public readonly TValue value;
        }
        /// <summary> Используем флаг запрошен или идёт ли в данный момент процессинг в UnityThread </summary>
        private volatile bool _processing;

        public UnityThreadDictionaryStream()
        {
            _disposeWrapper = new ThreadSafeDisposeWrapper(DisposeInternal, this);
        }

        /// <summary> В очереди на изменения появилось что-то новенькое, и надо отпроцессить его. </summary>
        private void RequestProcessing()
        {
            if (!_disposeWrapper.StartWorker())
                return;
            lock (_recivedValuesQueueLock)
                if (!_processing)
                {
                    _processing = true;
                    UnityQueueHelper.RunInUnityThreadNoWait(ProcessingIntoUnityThread);
                }
            _disposeWrapper.FinishWorker();
        }
        private void ProcessingIntoUnityThread()
        {
            if (!_disposeWrapper.StartWorker())
                return;
            do
            {
                DictionaryChange nextValue;
                lock (_recivedValuesQueueLock)
                {
                    if (_recivedValues.Count == 0)
                    {
                        _processing = false;
                        break;
                    }
                    // TODO Сделать аварийный выход и перевызов Processing() для перевызова необходимо извращение чтобы прервать трэд
                    nextValue = _recivedValues.Dequeue();
                }
                switch (nextValue.type)
                {
                    case DictionaryChange.changeType.addOrUpdate:
                        _unityThreadOutput[nextValue.key] = nextValue.value;
                        break;
                    case DictionaryChange.changeType.remove:
                        _unityThreadOutput.Remove(nextValue.key);
                        break;
                    case DictionaryChange.changeType.removePair:
                        _unityThreadOutput.Remove(new KeyValuePair<TKey, TValue>(nextValue.key, nextValue.value));
                        break;
                    case DictionaryChange.changeType.clear:
                        _unityThreadOutput.Clear();
                        break;
                }
            } while (true);
            _disposeWrapper.FinishWorker();
        }

        #region IDictionaryStream<TKey, TValue>
        public IStream<DctAddEvent<TKey, TValue>> AddStream => _unityThreadOutput.AddStream;
        public IStream<DctRemoveEvent<TKey, TValue>> RemoveStream => _unityThreadOutput.RemoveStream;
        public IStream<DctChangeEvent<TKey, TValue>> ChangeStream => _unityThreadOutput.ChangeStream;
        public IStream<int> CountStream => _unityThreadOutput.CountStream;
        #endregion

        #region IDictionary<TKey, TValue>
        public TValue this[TKey key]
        {
            get
            {
                UnityThreadUtils.AssertRunIntoUnityThread(Logger);
                if (_disposeWrapper.IsDisposed) // Упрощённая схема потому что _unityThreadCopy может статиь IsDisposed только внутри того же UnityThread в котором мы сейчас находимся
                    return default;
                var value = _unityThreadOutput[key];
                return value;
            }
            set
            {
                UnityThreadUtils.AssertRunOutOfUnityThread(Logger);
                if (!_disposeWrapper.StartWorker())
                    return;
                lock (_recivedValuesQueueLock)
                    _recivedValues.Enqueue(new DictionaryChange(DictionaryChange.changeType.addOrUpdate, key, value));
                _disposeWrapper.FinishWorker();
                RequestProcessing();
            }
        }
        public ICollection<TKey> Keys
        {
            get
            {
                UnityThreadUtils.AssertRunIntoUnityThread(Logger);
                if (_disposeWrapper.IsDisposed) // Упрощённая схема потому что _unityThreadCopy может статиь IsDisposed только внутри того же UnityThread в котором мы сейчас находимся
                    return Array.Empty<TKey>();
                return _unityThreadOutput.Keys;
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                UnityThreadUtils.AssertRunIntoUnityThread(Logger);
                if (_disposeWrapper.IsDisposed) // Упрощённая схема потому что _unityThreadCopy может статиь IsDisposed только внутри того же UnityThread в котором мы сейчас находимся
                    return Array.Empty<TValue>();
                return _unityThreadOutput.Values;
            }
        }

        /// <summary> В данном методе не проверяем тред использования, потому что это обращение может осуществляться шибко часто. </summary>
        public int Count => _unityThreadOutput.Count;
        public bool IsReadOnly => _unityThreadOutput.IsReadOnly;

        public void Add(TKey key, TValue value)
        {
            UnityThreadUtils.AssertRunOutOfUnityThread(Logger);
            if (!_disposeWrapper.StartWorker())
                return;
            lock (_recivedValuesQueueLock)
                _recivedValues.Enqueue(new DictionaryChange(DictionaryChange.changeType.addOrUpdate, key, value));
            _disposeWrapper.FinishWorker();
            RequestProcessing();

        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            UnityThreadUtils.AssertRunOutOfUnityThread(Logger);
            if (!_disposeWrapper.StartWorker())
                return;
            lock (_recivedValuesQueueLock)
                _recivedValues.Enqueue(new DictionaryChange(DictionaryChange.changeType.addOrUpdate, item.Key, item.Value));
            _disposeWrapper.FinishWorker();
            RequestProcessing();
        }

        public void Clear()
        {
            UnityThreadUtils.AssertRunOutOfUnityThread(Logger);
            if (!_disposeWrapper.StartWorker())
                return;
            lock (_recivedValuesQueueLock)
                _recivedValues.Enqueue(new DictionaryChange(DictionaryChange.changeType.clear, default, default));
            _disposeWrapper.FinishWorker();
            RequestProcessing();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            UnityThreadUtils.AssertRunIntoUnityThread(Logger);
            if (_disposeWrapper.IsDisposed) // Упрощённая схема потому что _unityThreadCopy может статиь IsDisposed только внутри того же UnityThread в котором мы сейчас находимся
                return false;
            return _unityThreadOutput.Contains(item);
        }

        public bool ContainsKey(TKey key)
        {
            UnityThreadUtils.AssertRunIntoUnityThread(Logger);
            if (_disposeWrapper.IsDisposed) // Упрощённая схема потому что _unityThreadCopy может статиь IsDisposed только внутри того же UnityThread в котором мы сейчас находимся
                return false;
            return _unityThreadOutput.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            UnityThreadUtils.AssertRunIntoUnityThread(Logger);
            if (_disposeWrapper.IsDisposed) // Упрощённая схема потому что _unityThreadCopy может статиь IsDisposed только внутри того же UnityThread в котором мы сейчас находимся
                return;
            _unityThreadOutput.CopyTo(array, arrayIndex);
        }

        /// <summary> ВНИМАНИЕ! В связи с особенностями мультитредового использования всегда синхронно возвращает true и в душе не знает как оно на самом деле. Не используейте возвращаемое значение никак. </summary>
        /// <returns>НЕ ИСПОЛЬЗОВАТЬ!!!</returns>
        public bool Remove(TKey key)
        {
            UnityThreadUtils.AssertRunOutOfUnityThread(Logger);
            if (!_disposeWrapper.StartWorker())
                return false;
            lock (_recivedValuesQueueLock)
                _recivedValues.Enqueue(new DictionaryChange(DictionaryChange.changeType.remove, key, default));
            _disposeWrapper.FinishWorker();
            RequestProcessing();
            return true;
        }
        /// <summary> ВНИМАНИЕ! В связи с особенностями мультитредового использования всегда синхронно возвращает true и в душе не знает как оно на самом деле. Не используейте возвращаемое значение никак. </summary>
        /// <returns>НЕ ИСПОЛЬЗОВАТЬ!!!</returns>
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            UnityThreadUtils.AssertRunOutOfUnityThread(Logger);
            if (!_disposeWrapper.StartWorker())
                return false;
            lock (_recivedValuesQueueLock)
                _recivedValues.Enqueue(new DictionaryChange(DictionaryChange.changeType.removePair, item.Key, item.Value));
            _disposeWrapper.FinishWorker();
            RequestProcessing();
            return true;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            UnityThreadUtils.AssertRunIntoUnityThread(Logger);
            if (_disposeWrapper.IsDisposed)
            { // Упрощённая схема потому что _unityThreadCopy может статиь IsDisposed только внутри того же UnityThread в котором мы сейчас находимся
                value = default;
                return false;
            }
            return _unityThreadOutput.TryGetValue(key, out value);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            UnityThreadUtils.AssertRunIntoUnityThread(Logger);
            if (_disposeWrapper.IsDisposed) // Упрощённая схема потому что _unityThreadCopy может статиь IsDisposed только внутри того же UnityThread в котором мы сейчас находимся
                return ((IEnumerable<KeyValuePair<TKey, TValue>>)Array.Empty<KeyValuePair<TKey, TValue>>()).GetEnumerator();
            return _unityThreadOutput.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            UnityThreadUtils.AssertRunIntoUnityThread(Logger);
            if (_disposeWrapper.IsDisposed) // Упрощённая схема потому что _unityThreadCopy может статиь IsDisposed только внутри того же UnityThread в котором мы сейчас находимся
                return Array.Empty<KeyValuePair<TKey, TValue>>().GetEnumerator();
            return _unityThreadOutput.GetEnumerator();
        }
        #endregion

        #region IIsDisposed
        public bool IsDisposed => _disposeWrapper.IsDisposed;
        public void Dispose()
        {
            _disposeWrapper.RequestDispose();
        }
        private void DisposeInternal()
        {
            UnityQueueHelper.RunInUnityThreadNoWait(() => _unityThreadOutput.Dispose());
            _recivedValues.Clear();

        }
        #endregion
    }
}
