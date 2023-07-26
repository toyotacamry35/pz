using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using JetBrains.Annotations;
using NLog;
using ReactivePropsNs.Touchables;
using UnityAsyncAwaitUtil;

namespace ReactivePropsNs
{
    /// <summary>
    /// Посредник Предоставляет интерфейс ICollection, превращает в вызовы и перекладывает эти вызовы в UnityThread Не нарушая порядок, так чтобы в UnityThread возникла копия.
    /// Доступен для записи только а альтернативных тредах, а для чтения только в UnityThread
    /// </summary>
    public class UnityThreadHashsetStream<TValue> : IHashSetStream<TValue>, IIsDisposed
    {
        [NotNull] private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        ThreadSafeDisposeWrapper _disposeWrapper;
        private HashSetStream<TValue> _unityThreadOutput = new HashSetStream<TValue>();
        private object _recivedValuesQueueLock = new object();
        private Queue<HashSetChange> _recivedValues = new Queue<HashSetChange>();
        private readonly struct HashSetChange
        {
            public HashSetChange(changeType type, TValue value)
            {
                this.type = type;
                this.value = value;
            }
            public enum changeType { add, remove, clear }
            public readonly changeType type;
            public readonly TValue value;
        }
        /// <summary> Используем флаг запрошен или идёт ли в данный момент процессинг в UnityThread </summary>
        private volatile bool _processing = false;

        public UnityThreadHashsetStream()
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
                HashSetChange nextValue;
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
                    case HashSetChange.changeType.add:
                        _unityThreadOutput.Add(nextValue.value);
                        break;
                    case HashSetChange.changeType.remove:
                        _unityThreadOutput.Remove(nextValue.value);
                        break;
                    case HashSetChange.changeType.clear:
                        _unityThreadOutput.Clear();
                        break;
                }
            } while (true);
            _disposeWrapper.FinishWorker();
        }

        #region IDictionaryStream<TKey, TValue>
        public IStream<TValue> AddStream => _unityThreadOutput.AddStream;
        public IStream<TValue> RemoveStream => _unityThreadOutput.RemoveStream;
        public IStream<int> CountStream => _unityThreadOutput.CountStream;
        #endregion

        #region IDictionary<TKey, TValue>
        /// <summary> В данном методе не проверяем тред использования, потому что это обращение может осуществляться шибко часто. </summary>
        public int Count => _unityThreadOutput.Count;
        public bool IsReadOnly => _unityThreadOutput.IsReadOnly;

        public void Add(TValue item)
        {
            UnityThreadUtils.AssertRunOutOfUnityThread(Logger);
            if (!_disposeWrapper.StartWorker())
                return;
            lock (_recivedValuesQueueLock)
                _recivedValues.Enqueue(new HashSetChange(HashSetChange.changeType.add, item));
            _disposeWrapper.FinishWorker();
            RequestProcessing();

        }

        public void Clear()
        {
            UnityThreadUtils.AssertRunOutOfUnityThread(Logger);
            if (!_disposeWrapper.StartWorker())
                return;
            lock (_recivedValuesQueueLock)
                _recivedValues.Enqueue(new HashSetChange(HashSetChange.changeType.clear, default));
            _disposeWrapper.FinishWorker();
            RequestProcessing();
        }

        public bool Contains(TValue item)
        {
            UnityThreadUtils.AssertRunIntoUnityThread(Logger);
            if (_disposeWrapper.IsDisposed) // Упрощённая схема потому что _unityThreadCopy может статиь IsDisposed только внутри того же UnityThread в котором мы сейчас находимся
                return false;
            return _unityThreadOutput.Contains(item);
        }

        public void CopyTo(TValue[] array, int arrayIndex)
        {
            UnityThreadUtils.AssertRunIntoUnityThread(Logger);
            if (_disposeWrapper.IsDisposed) // Упрощённая схема потому что _unityThreadCopy может статиь IsDisposed только внутри того же UnityThread в котором мы сейчас находимся
                return;
            _unityThreadOutput.CopyTo(array, arrayIndex);
        }

        /// <summary> ВНИМАНИЕ! В связи с особенностями мультитредового использования всегда синхронно возвращает true и в душе не знает как оно на самом деле. Не используейте возвращаемое значение никак. </summary>
        /// <returns>НЕ ИСПОЛЬЗОВАТЬ!!!</returns>
        public bool Remove(TValue value)
        {
            UnityThreadUtils.AssertRunOutOfUnityThread(Logger);
            if (!_disposeWrapper.StartWorker())
                return false;
            lock (_recivedValuesQueueLock)
                _recivedValues.Enqueue(new HashSetChange(HashSetChange.changeType.remove, default));
            _disposeWrapper.FinishWorker();
            RequestProcessing();
            return true;
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            UnityThreadUtils.AssertRunIntoUnityThread(Logger);
            if (_disposeWrapper.IsDisposed) // Упрощённая схема потому что _unityThreadCopy может статиь IsDisposed только внутри того же UnityThread в котором мы сейчас находимся
                return ((IEnumerable<TValue>)Array.Empty<TValue>()).GetEnumerator();
            return _unityThreadOutput.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            UnityThreadUtils.AssertRunIntoUnityThread(Logger);
            if (_disposeWrapper.IsDisposed) // Упрощённая схема потому что _unityThreadCopy может статиь IsDisposed только внутри того же UnityThread в котором мы сейчас находимся
                return Array.Empty<TValue>().GetEnumerator();
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

        IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
