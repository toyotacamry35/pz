using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SharedCode.Logging;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using GeneratedCode.EntitySystem;
using GeneratedCode.Repositories;
using NLog;
using SharedCode.EntitySystem;
using SharedCode.Utils;
using SharedCode.Repositories;

namespace GeneratedCode.Manual.Repositories
{
    public class EntityQueue : IDisposable
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private static Pool<QueueWithIndex<EntityQueueElement>> _queuePool =
            new Pool<QueueWithIndex<EntityQueueElement>>(5000,
                0,
                () => new QueueWithIndex<EntityQueueElement>(),
                (x) => { x.Clear(); });

        private int _typeId { get; set; }

        private Guid _entityId { get; set; }

        private DateTime _lastRemovedTime { get; set; }

        private DateTime _lastOperationLogTime = DateTime.UtcNow;

        public EntityQueue(int typeId, Guid entityId)
        {
            _typeId = typeId;
            _entityId = entityId;
            _lastRemovedTime = DateTime.UtcNow;
            _queue = _queuePool.Take();
            //_queue.dumpAction = (val) => { return $"{(val.Item1 == ReadWriteEntityOperationType.Read ? "R" : "W")}{val.Item2.Id}-{val.Item2.EntitiesContainer.Batch.Id}-{val.Item2.EntitiesContainer.Tag ?? "none"}"; };
        }

        private QueueWithIndex<EntityQueueElement> _queue;

        private long _writeCount;

        public int TypeId => _typeId;

        public Guid EntityId => _entityId;

        public bool IsFree()
        {
            return _queue.Count == 0;
        }

        private DateTime _lastWarnSizeTime = DateTime.UtcNow;

        public void Enqueue(ReadWriteEntityOperationType operationType, EntitiesBatchWaitWrapper batch, bool readyToUse, ReplicationLevel requestedLevel)
        {
            if (operationType == ReadWriteEntityOperationType.Write)
                _writeCount++;
            _queue.Enqueue(new EntityQueueElement(operationType, batch, readyToUse, requestedLevel));

            if (_queue.Count > ServerCoreRuntimeParameters.RepositoryWaitQueueSizeWarningCount)
            {
                if ((DateTime.UtcNow - _lastWarnSizeTime).TotalSeconds > 2)
                {
                    _lastWarnSizeTime = DateTime.UtcNow;
                    Log.Logger.Error(
                        "Entity wait queue size == {0}, typeId {1} entityId {2}. Last removed {3:0.00} seconds ago",
                        _queue.Count, ReplicaTypeRegistry.GetTypeById(_typeId).Name, _entityId, (DateTime.UtcNow - _lastRemovedTime).TotalSeconds);
                }
            }
        }

        public static ValueTuple<ReadWriteEntityOperationType, EntitiesBatchWaitWrapper> DefaultWrapperTuple = default;

        public int Count => _queue.Count;

        //public string QueueDump => _queue.ToString() + $" wr:{_writeCount.ToString()} h:{this.GetHashCode().ToString()}";

        public ref EntityQueueElement this[int index] => ref _queue[index];

        public bool TryWrite(EntitiesBatchWaitWrapper batchWrapper)
        {
            ref var firstElement = ref _queue[0];
            if (firstElement.Batch != batchWrapper)
                return false;

            firstElement.ReadyToUse = true;
            batchWrapper.DecrementWaitQueuesCount();
            return true;
        }

        public bool HasWrite()
        {
            return _writeCount > 0;
        }

        public bool TryRead(EntitiesBatchWaitWrapper batchWrapper)
        {
            for (int i = 0; i < _queue.Count; i++)
            {
                ref var item = ref _queue[i];
                if (batchWrapper == item.Batch)
                {
                    item.ReadyToUse = true;
                    batchWrapper.DecrementWaitQueuesCount();
                    return true;
                }

                if (item.OperationType == ReadWriteEntityOperationType.Write)
                    return false;
            }

            Logger.IfError()?.Message("CantUse Batch not in queue. Entity {0} {1}", ReplicaTypeRegistry.GetTypeById(TypeId).Name, EntityId).Write();
            return false;
        }

        public void RemoveBatch(long id)
        {
            if (id == 0)
                return;

            for (int i = 0; i < _queue.Count; i++)
            {
                ref var item = ref _queue[i];
                if (item.Batch.Id == id)
                {
                    if (item.OperationType == ReadWriteEntityOperationType.Write)
                        _writeCount--;
                    _queue.SwapAndRemoveAt(i);

                    _lastRemovedTime = DateTime.UtcNow;
                    return;
                }
            }
            Logger.IfError()?.Message("RemoveBatch Batch {0} not in queue. Entity {1} {2}", id, ReplicaTypeRegistry.GetTypeById(TypeId).Name, EntityId).Write();
        }

        public void RemoveTimeoutedBatches(long id, HashSet<EntityQueue> queuesToRun)
        {
            var firstWrite = true;
            for (int i = 0; i < _queue.Count; i++)
            {
                ref var item = ref _queue[i];
                if (item.Batch.Id == id)
                {
                    if (item.OperationType == ReadWriteEntityOperationType.Write)
                        _writeCount--;

                    if (item.OperationType == ReadWriteEntityOperationType.Write)//батч первый из батчей на запись, 
                    {
                        if (firstWrite)
                            queuesToRun.Add(this);
                    }
                    else if (i == 0 && _queue.Count > 1 && _queue[1].OperationType == ReadWriteEntityOperationType.Write) //батч на чтение первый в очереди и за ним сразу батч на запись
                        queuesToRun.Add(this);

                    _queue.RemoveAt(i);
                    return;
                }

                if (item.OperationType == ReadWriteEntityOperationType.Write)
                    firstWrite = false;
            }
        }

        public EntityQueueElement[] GetBatchesToDump()
        {
            _lastOperationLogTime = DateTime.UtcNow;
            var result = new EntityQueueElement[5];
            for (int i = 0; i < Math.Min(5, _queue.Count); i++)
                result[i] = _queue[i];

            return result;
        }

        public void Dispose()
        {
            _queuePool.Return(_queue);
            _queue = null;
        }

        public bool CanGetLog()
        {
            return (DateTime.UtcNow - _lastOperationLogTime).TotalSeconds >= ServerCoreRuntimeParameters.EntityOperationLogMinDelaySeconds;
        }
        
        public struct EntityQueueElement : IEquatable<EntityQueueElement>
        {
            public EntityQueueElement(ReadWriteEntityOperationType operationType, EntitiesBatchWaitWrapper batch, bool readyToUse, ReplicationLevel requestedLevel)
            {
                OperationType = operationType;
                Batch = batch;
                ReadyToUse = readyToUse;
                RequestedLevel = requestedLevel;
            }

            public ReadWriteEntityOperationType OperationType { get; }
            public EntitiesBatchWaitWrapper Batch { get; }
            public bool ReadyToUse { get; set; }
            public ReplicationLevel RequestedLevel { get; }

            public bool Equals(EntityQueueElement other)
            {
                return OperationType == other.OperationType && Equals(Batch, other.Batch) && ReadyToUse == other.ReadyToUse;
            }

            public override bool Equals(object obj)
            {
                return obj is EntityQueueElement other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = (int) OperationType;
                    hashCode = (hashCode * 397) ^ (Batch != null ? Batch.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ ReadyToUse.GetHashCode();
                    return hashCode;
                }
            }
        }
    }


    public class QueueWithIndex<T>
    {
        private const int DefaultSize = 1;

        private const int MaxSizeIncrease = 512;

        private T[] _items;

        private int _startIndex;

        private int _endIndex;
        public int Count { get; private set; }

        //public Func<T, string> dumpAction;

        public QueueWithIndex():this(DefaultSize)
        {
            
        }

        public QueueWithIndex(int size)
        {
            _items = new T[size];
        }

        public ref T this[int index]
        {
            get
            {
                if (index >= Count || index < 0)
                    throw new IndexOutOfRangeException($"{index} out of range. Count {Count}");

                return ref _items[(index + _startIndex) % _items.Length];
            }
        }

        public void Enqueue(T item)
        {
            if (_items.Length == Count)
                increaseSize();

            _items[_endIndex] = item;
            _endIndex = (_endIndex + 1) % _items.Length;
            Count++;
        }

        public void SwapAndRemoveAt(int index)
        {
            if (index >= Count || index < 0)
                throw new IndexOutOfRangeException($"{index} out of range. Count {Count}");

            var currentIndex = (index + _startIndex) % _items.Length;

            if (currentIndex != _startIndex)
                _items[currentIndex] = _items[_startIndex];
            _items[_startIndex] = default;
            _startIndex = (_startIndex + 1) % _items.Length;
            Count--;
        }

        public void RemoveAt(int index)
        {
            if (index >= Count || index < 0)
                throw new IndexOutOfRangeException($"{index} out of range. Count {Count}");

            var currentIndex = (index + _startIndex) % _items.Length;
            var lastElementIndex = _endIndex == 0 ? _items.Length - 1 : _endIndex - 1;
            if (currentIndex != lastElementIndex)
            {
                if (_startIndex < lastElementIndex) // ---S*C*L----
                {
                    Array.Copy(_items, currentIndex + 1, _items, currentIndex, lastElementIndex - currentIndex);
                }
                else if (currentIndex < lastElementIndex)  // *CL---S***
                {
                    Array.Copy(_items, currentIndex + 1, _items, currentIndex, lastElementIndex - currentIndex);
                }
                else // **L---S**С*
                {
                    if (currentIndex < _items.Length - 1)
                        Array.Copy(_items, currentIndex + 1, _items, currentIndex, _items.Length - 1 - currentIndex);
                    _items[_items.Length - 1] = _items[0];
                    Array.Copy(_items, 1, _items, 0, lastElementIndex);
                }
            }

            _items[lastElementIndex] = default;
            _endIndex = lastElementIndex;
            Count--;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void increaseSize()
        {
            var newCapacity = _items.Length + Math.Min(_items.Length * 2, MaxSizeIncrease);
            var newItems = new T[newCapacity];
            Array.Copy(_items, _startIndex, newItems, 0, _items.Length - _startIndex);
            if (_endIndex > 0)
                Array.Copy(_items, 0, newItems, _items.Length - _startIndex, _endIndex);
            _endIndex = _items.Length;
            _startIndex = 0;
            _items = newItems;
        }

        public void Clear()
        {
            for (int i = 0; i < Count; i++)
            {
                var currentIndex = (i + _startIndex) % _items.Length;
                _items[currentIndex] = default;
            }

            _startIndex = 0;
            _endIndex = 0;
            Count = 0;
            //dumpAction = null;
        }

        //public override string ToString()
        //{
        //    var sb = new StringBuilder();
        //    for (int i = 0; i < _items.Length; i++)
        //    {
        //        ref var item = ref _items[i];
        //        sb.AppendFormat("{0}:{1},", i, (item?.Equals(default(T)) ?? true) ? "D" : dumpAction(item));
        //    }

        //    return string.Format("_startIndex:{0} _endIndex:{1} count:{2} items:{3}", _startIndex, _endIndex, Count, sb.ToString());
        //}
    }
}
