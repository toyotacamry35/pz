using System;
using System.Collections;
using System.Collections.Generic;

namespace Src.Locomotion
{
    public class RingBuffer<T> : IEnumerable<T>, ICollection<T>
    {
        private static readonly T[] _empty = new T[0];
        private readonly T[] _buffer;
        private int _end;
        private int _start;
        private int _count;
        private bool _isReadOnly;

        public RingBuffer(int capacity)
            : this(capacity, _empty)
        {}

        public RingBuffer(int capacity, T[] items)
        {
            if (capacity < 1)
                throw new ArgumentException("Ring buffer cannot have negative or zero capacity.", nameof(capacity));
            if (items == null) 
                throw new ArgumentNullException(nameof(items));
            if (items.Length > capacity)
                throw new ArgumentException("Too many items to fit circular buffer", nameof(items));

            _buffer = new T[capacity];
            Array.Copy(items, _buffer, items.Length);
            _count = items.Length;
            _start = 0;
            _end = (_start + _count) % capacity;
        }

        public int Count => _count;

        public bool IsReadOnly => _isReadOnly;

        public int Capacity => _buffer.Length;

        public bool IsFull => _count == _buffer.Length;

        public bool IsEmpty => _count == 0;

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= _count) throw new IndexOutOfRangeException($"Cannot access index {index}. Buffer size is {_count}");
                return _buffer[(_start + index) % _buffer.Length];
            }
            set
            {
                if (index < 0 || index >= _count) throw new IndexOutOfRangeException($"Cannot access index {index}. Buffer size is {_count}");
                _buffer[(_start + index) % _buffer.Length] = value;
            }
        }

        public ref T DirectGet(int directIndex)
        {
            if(!(directIndex >= _start && directIndex < Math.Min(_buffer.Length, _start + _count) || _start >= _end && directIndex >= 0 && directIndex < _end))
                throw new IndexOutOfRangeException($"Cannot access direct index {directIndex}. Count:{_count} Start:{_start} End:{_end}");
            return ref _buffer[directIndex];
        }

        public void DirectSet(int directIndex, in T value)
        {
            if(!(directIndex >= _start && directIndex < Math.Min(_buffer.Length, _start + _count) || _start >= _end && directIndex >= 0 && directIndex < _end))
                throw new IndexOutOfRangeException($"Cannot access direct index {directIndex}. Count:{_count} Start:{_start} End:{_end}");
            _buffer[directIndex] = value;
        }

        public IEnumerator<T> GetEnumerator()
        {
            int i = 0;
            for (int pos = _start, end = _buffer.Length; i < _count && pos < end; ++i, ++pos)
                yield return _buffer[pos];
            for (int pos = 0; i < _count; ++i, ++pos)
                yield return _buffer[pos];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public T Front()
        {
            if (_count == 0) throw new InvalidOperationException("Cannot access an empty buffer.");
            return _buffer[_start];
        }

        public T Back()
        {
            if (_count == 0) throw new InvalidOperationException("Cannot access an empty buffer.");
            return _buffer[(_start + _count - 1) % _buffer.Length];
        }

        public void PushBack(T item)
        {
            _buffer[_end] = item;
            _end = (_end + 1) % _buffer.Length;
            if (IsFull)
                _start = _end;
            else
                ++_count;
        }

        /// <summary>
        /// Returns pair with direct index in buffer and true, if buffer is full and element in front was displaced.    
        /// </summary>
        public (int,bool) PushBackEx(in T item)
        {
            _buffer[_end] = item;
            int directIndex = _end;
            _end = (_end + 1) % _buffer.Length;
            if (IsFull)
            {
                _start = _end;
                return (directIndex, true);
            }
            ++_count;
            return (directIndex, false);
        }
        
        public void PushFront(T item)
        {
            _start = (_start + _buffer.Length - 1) % _buffer.Length;
            if (IsFull)
                _end = _start;
            else
                ++_count;
            _buffer[_start] = item;
        }

        public void PopBack()
        {
            if (_count == 0) throw new InvalidOperationException("Cannot take elements from an empty buffer.");
            _end = (_end + _buffer.Length - 1) % _buffer.Length;
            _buffer[_end] = default(T);
            --_count;
        }

        public void PopFront()
        {
            if (_count == 0) throw new InvalidOperationException("Cannot take elements from an empty buffer.");
            _buffer[_start] = default(T);
            _start = (_start + 1) % _buffer.Length;
            --_count;
        }

        /// Вставляет элемент в указанную позицию, вытесняя, если необходимо, элемент в конце 
        public void InsertFront(int index, T item)
        {
            if (index < 0 || index > _count) throw new IndexOutOfRangeException($"Cannot insert to index {index}. Buffer size is {_count}");

            if (index == 0)
            {
                PushFront(item);
            }
            else
            if (index == _count)
            {
                if(!IsFull)
                    PushBack(item);
            }
            else
            {
                var pos = (_start + index) % _buffer.Length;
                if (_start == 0)
                {
                    var count = !IsFull ? _end  - pos: _buffer.Length - pos - 1;
                    Array.Copy(_buffer, pos, _buffer, pos + 1, count);
                    _buffer[pos] = item;
                    if (!IsFull)
                    {
                        _count++;
                        _end = (_start + _count) % _buffer.Length;
                    }
                }
                else
                if (pos < _start)
                {
                    if (_end > _start) throw new Exception(string.Format("_end:{0} > _start:{1}. Buffer size is {2}. Pos is {3}.", _end,
                        _start, _count, pos));
                    if (pos > _end) throw new Exception(string.Format("pos:{0} > _start:{1}. Buffer size is {2}", pos, _end, _count));
                    var count = !IsFull ? _end - pos : _end - pos - 1;
                    Array.Copy(_buffer, pos, _buffer, pos + 1, count);
                    _buffer[pos] = item;
                    if (!IsFull)
                    {
                        _count++;
                        _end = (_start + _count) % _buffer.Length;
                    }
                }
                else
                {
                    Array.Copy(_buffer, _start, _buffer, _start - 1, pos - _start);
                    _buffer[pos - 1] = item;
                    --_start;
                    --pos;
                    if (!IsFull)
                        _count++;
                    else
                        _end = (_start + _count) % _buffer.Length;
                }
            }
        }
        
        public T[] ToArray()
        {
            var newArray = new T[_count];
            CopyTo(newArray, 0);
            return newArray;
        }
        
        public void Add(T item)
        {
            PushBack(item);
        }

        public void Clear()
        {
            _start = _end = _count = 0;
            for (int i = 0; i < _buffer.Length; i++)
                _buffer[i] = default(T);
        }

        public bool Contains(T item)
        {
            return Array.IndexOf(_buffer, item) != -1;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            var head = Head();
            var tail = Tail();
            Array.Copy(head.Array, head.Offset, array, arrayIndex, head.Count);
            Array.Copy(tail.Array, tail.Offset, array, arrayIndex + head.Count, tail.Count);
        }

        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        private ArraySegment<T> Head()
        {
            return new ArraySegment<T>(_buffer, _start, Math.Min(_buffer.Length - _start, _count));
        }

        private ArraySegment<T> Tail()
        {
            if (_start < _end)
                return new ArraySegment<T>(_buffer, _end, 0);
            return new ArraySegment<T>(_buffer, 0, _end);
        }
    }
}
