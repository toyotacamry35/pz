using System;
using System.Collections.Generic;
using System.Text;
using NLog;
using NLog.Fluent;

namespace Assets.Src.Effects.FX
{
    public class UpdatePriorityQueueOperation<T>
    {
        public PriorityQueueOperationVariation OperationVariation { get; set; }
        public T Object { get; set; }
        public int NewPriority { get; set; }
    }

    public enum PriorityQueueOperationVariation
    {
        Nothing,
        GiveMaximumPriority,
        Delete
    }

    public class PriorityQueue<T>
    {
        private static Logger Logger = LogManager.GetCurrentClassLogger();
        
        private class Node
        {
            public int Priority { get; set; }
            public T Object { get; set; }
        }

        public int Count => _queue.Count;

        //object tree
        private readonly List<Node> _queue = new List<Node>();
        private Queue<UpdatePriorityQueueOperation<T>> _operations = new Queue<UpdatePriorityQueueOperation<T>>();
        private readonly bool _isMinPriorityQueue;
        private int _heapSize = -1;

        /// <summary>
        /// If min queue or max queue
        /// </summary>
        /// <param name="isMinPriorityQueue"></param>
        public PriorityQueue(bool isMinPriorityQueue = false)
        {
            _isMinPriorityQueue = isMinPriorityQueue;
        }

        /// <summary>
        /// Enqueue the object with priority
        /// </summary>
        /// <param name="priority"></param>
        /// <param name="obj"></param>
        public void Enqueue(int priority, T obj)
        {
            var node = new Node {Priority = priority, Object = obj};
            _queue.Add(node);
            _heapSize++;
            //Maintaining heap
            if (_isMinPriorityQueue)
                BuildHeapMin(_heapSize);
            else
                BuildHeapMax(_heapSize);
        }

        /// <summary>
        /// Dequeue the object
        /// </summary>
        /// <returns></returns>
        public T Dequeue()
        {
            if (_heapSize > -1)
            {
                var returnVal = _queue[0].Object;
                _queue[0] = _queue[_heapSize];
                _queue.RemoveAt(_heapSize);
                _heapSize--;
                //Maintaining lowest or highest at root based on min or max queue
                if (_isMinPriorityQueue)
                    MinHeapify(0);
                else
                    MaxHeapify(0);
                return returnVal;
            }
            else
                throw new Exception("Queue is empty");
        }

        /// <summary>
        /// Remove specific object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool Remove(T obj)
        {
            int index = -1;
            for (int i = 0; i < _queue.Count; i++)
            {
                if (ReferenceEquals(obj, _queue[i].Object))
                    index = i;
            }

            if (index == -1)
                return false;

            _heapSize--;
            _queue.RemoveAt(index);
            if (_isMinPriorityQueue)
                BuildHeapMin(_heapSize);
            else
                BuildHeapMax(_heapSize);
            return true;
        }

        /// <summary>
        /// Get maximum priority
        /// </summary>
        /// <returns></returns>
        public int GetMaximumPriority()
        {
            return _isMinPriorityQueue ? 0 : int.MaxValue;
        }

        /// <summary>
        /// Check if next priority has maximum priority
        /// </summary>
        /// <returns></returns>
        public bool IsMaximumPriority()
        {
            return _queue.Count != 0 && _queue[0].Priority == GetMaximumPriority();
        }

        /// <summary>
        /// Updating the priority of specific object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="priority"></param>
        public void UpdatePriority(T obj, int priority)
        {
            for (int i = 0; i <= _heapSize; i++)
            {
                var node = _queue[i];
                if (!ReferenceEquals(node.Object, obj))
                    continue;

                node.Priority = priority;
                if (_isMinPriorityQueue)
                {
                    BuildHeapMin(i);
                    MinHeapify(i);
                }
                else
                {
                    BuildHeapMax(i);
                    MaxHeapify(i);
                }
            }
        }

        /// <summary>
        /// Check all object by given expression
        /// </summary>
        /// <param name="checkMethod"></param>
        /// <returns></returns>
        public void ClearOrUpdatePriority(Func<T, PriorityQueueOperationVariation> checkMethod)
        {
            // Собираем все операции по очистке и обновлению приоритета если такие есть
            for (int i = 0; i < _queue.Count; i++)
            {
                switch (checkMethod.Invoke(_queue[i].Object))
                {
                    case PriorityQueueOperationVariation.GiveMaximumPriority:
                        _operations.Enqueue(
                            new UpdatePriorityQueueOperation<T>
                                {OperationVariation = PriorityQueueOperationVariation.GiveMaximumPriority, NewPriority = GetMaximumPriority(), Object = _queue[i].Object});
                        break;
                    case PriorityQueueOperationVariation.Delete:
                        _operations.Enqueue(new UpdatePriorityQueueOperation<T> {OperationVariation = PriorityQueueOperationVariation.GiveMaximumPriority, Object = _queue[i].Object});
                        break;
                }
            }

            //Применяем все операции
            var operationApplied = _operations.Count > 0;
            while (_operations.Count > 0)
            {
                var operation = _operations.Dequeue();
                switch (operation.OperationVariation)
                {
                    case PriorityQueueOperationVariation.GiveMaximumPriority:
                        for (int i = 0; i < _queue.Count; i++)
                        {
                            if (!ReferenceEquals(operation.Object, _queue[i].Object)) continue;
                            
                            _queue[i].Priority = operation.NewPriority;
                            break;
                        }
                        break;
                    case PriorityQueueOperationVariation.Delete:
                        var index = -1;
                        for (int i = 0; i < _queue.Count; i++)
                        {
                            if (!ReferenceEquals(operation.Object, _queue[i].Object)) continue;
                            
                            index = i;
                            break;
                        }

                        if (index == -1)
                            continue;

                        _heapSize--;
                        _queue.RemoveAt(index);
                        break;
                }
            }

            //Обновляем всё древо от самого высокого индекса
            if (operationApplied && _queue.Count > 0)
            {
                if (_isMinPriorityQueue)
                {
                    BuildHeapMin(0);
                    MinHeapify(0);
                }
                else
                {
                    BuildHeapMax(0);
                    MaxHeapify(0);
                }
            }
        }

        /// <summary>
        /// Searching an object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool IsInQueue(T obj)
        {
            foreach (var node in _queue)
                if (ReferenceEquals(node.Object, obj))
                    return true;
            return false;
        }
        
        /// <summary>
        /// Log current queue
        /// </summary>
        /// <returns></returns>
        public void ShowCurrentQueue()
        {
            var builder = new StringBuilder(10);
            builder.Append($"Elements in queue: ");
            for (var index = 0; index < _queue.Count; index++)
            {
                var node = _queue[index];
                builder.Append($"{index} - {node.Object},");
            }

            Logger.Info(builder.ToString);
        }

        /// <summary>
        /// Maintain max heap
        /// </summary>
        /// <param name="i"></param>
        private void BuildHeapMax(int i)
        {
            while (i >= 0 && _queue[(i - 1) / 2].Priority < _queue[i].Priority)
            {
                Swap(i, (i - 1) / 2);
                i = (i - 1) / 2;
            }
        }

        /// <summary>
        /// Maintain min heap
        /// </summary>
        /// <param name="i"></param>
        private void BuildHeapMin(int i)
        {
            while (i >= 0 && _queue[(i - 1) / 2].Priority > _queue[i].Priority)
            {
                Swap(i, (i - 1) / 2);
                i = (i - 1) / 2;
            }
        }

        private void MaxHeapify(int i)
        {
            var left = ChildL(i);
            var right = ChildR(i);

            var highest = i;

            if (left <= _heapSize && _queue[highest].Priority < _queue[left].Priority)
                highest = left;
            if (right <= _heapSize && _queue[highest].Priority < _queue[right].Priority)
                highest = right;

            if (highest == i)
                return;

            Swap(highest, i);
            MaxHeapify(highest);
        }

        private void MinHeapify(int i)
        {
            var left = ChildL(i);
            var right = ChildR(i);

            var lowest = i;

            if (left <= _heapSize && _queue[lowest].Priority > _queue[left].Priority)
                lowest = left;
            if (right <= _heapSize && _queue[lowest].Priority > _queue[right].Priority)
                lowest = right;

            if (lowest == i)
                return;

            Swap(lowest, i);
            MinHeapify(lowest);
        }

        private void Swap(int i, int j)
        {
            var temp = _queue[i];
            _queue[i] = _queue[j];
            _queue[j] = temp;
        }

        private int ChildL(int i)
        {
            return i * 2 + 1;
        }

        private int ChildR(int i)
        {
            return i * 2 + 2;
        }
    }
}