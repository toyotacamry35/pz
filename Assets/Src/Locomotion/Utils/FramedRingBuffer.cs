using System;

namespace Src.Locomotion
{
    public class FramedRingBuffer<T> 
    {
        private readonly T[] _buffer;
        private readonly int _frameSize;
        private readonly int _capacity;
        private int _end;
        private int _start;
        private int _count;

        public FramedRingBuffer(int frameSize, int capacity)
        {
            if (frameSize < 1)
                throw new ArgumentException("Ring buffer cannot have negative or zero frame size.", nameof(frameSize));
            if (capacity < 1)
                throw new ArgumentException("Ring buffer cannot have negative or zero capacity.", nameof(capacity));

            _frameSize = frameSize;
            _capacity = capacity;
            _buffer = new T[_capacity * _frameSize];
            _count = 0;
            _start = 0;
            _end = (_start + _count) % _capacity;
        }

        public int Count => _count;

        public int Capacity => _capacity;

        public bool IsFull => _count == _capacity;

        public bool IsEmpty => _count == 0;

        public ArraySegment<T> this[int index]
        {
            get
            {
                if (index < 0 || index >= _count) throw new IndexOutOfRangeException($"Cannot access index {index}. Buffer size is {_count}");
                return new ArraySegment<T>(_buffer, (_start + index) % _capacity * _frameSize, _frameSize);
            }
            set
            {
                if (value.Array == null) throw new ArgumentNullException(nameof(value));
                if (index < 0 || index >= _count) throw new IndexOutOfRangeException($"Cannot access index {index}. Buffer size is {_count}");
                if (value.Count != _frameSize) throw new ArgumentException($"Value size ({value.Count}) is not equal frame size ({_frameSize}).");
                Array.Copy(value.Array, value.Offset, _buffer, (_start + index) % _capacity * _frameSize, _frameSize);
            }
        }
        
        public T this[int index, int pos]
        {
            get
            {
                if (index < 0 || index >= _count) throw new IndexOutOfRangeException($"Cannot access index {index}. Buffer size is {_count}");
                if (pos < 0 || pos >= _frameSize) throw new IndexOutOfRangeException($"Cannot access pos {index}. Frame size is {_frameSize}");
                return _buffer[(_start + index) % _capacity * _frameSize + pos];
            }
            set
            {
                if (index < 0 || index >= _count) throw new IndexOutOfRangeException($"Cannot access index {index}. Buffer size is {_count}");
                if (pos < 0 || pos >= _frameSize) throw new IndexOutOfRangeException($"Cannot access pos {index}. Frame size is {_frameSize}");
                _buffer[(_start + index) % _capacity * _frameSize + pos] = value;
            }
        }

        public ArraySegment<T> Front()
        {
            if (_count == 0) throw new InvalidOperationException("Cannot access an empty buffer.");
            return new ArraySegment<T>(_buffer, _start * _frameSize, _frameSize);
        }

        public ArraySegment<T> Back()
        {
            if (_count == 0) throw new InvalidOperationException("Cannot access an empty buffer.");
            return new ArraySegment<T>(_buffer, (_start + _count - 1) % _capacity * _frameSize, _frameSize);
        }

        public void PushBack(T[] item)
        {
            PushBack(new ArraySegment<T>(item, 0, item.Length));
        }

        public void PushBack(ArraySegment<T> item)
        {
            if (item.Array == null) throw new ArgumentNullException(nameof(item));
            if (item.Count != _frameSize) throw new ArgumentException($"Item size ({item.Count}) is not equal frame size ({_frameSize}).");
            
            Array.Copy(item.Array, item.Offset, _buffer, _end * _frameSize, _frameSize);
            _end = (_end + 1) % _capacity;
            if (IsFull)
                _start = _end;
            else
                ++_count;
        }
        
        public ArraySegment<T> PushBack()
        {
            var rv = new ArraySegment<T>(_buffer, _end * _frameSize, _frameSize);
            rv.Clear();
            _end = (_end + 1) % _capacity;
            if (IsFull)
                _start = _end;
            else
                ++_count;
            return rv;
        }
 
        public void PushFront(T[] item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            PushFront(new ArraySegment<T>(item, 0, item.Length));
        }
        
        public void PushFront(ArraySegment<T> item)
        {
            if (item.Array == null) throw new ArgumentNullException(nameof(item));
            if (item.Count != _frameSize) throw new ArgumentException($"Item size ({item.Count}) is not equal frame size ({_frameSize}).");
            
            _start = (_start + _capacity - 1) % _capacity;
            if (IsFull)
                _end = _start;
            else
                ++_count;
            Array.Copy(item.Array, 0, _buffer, _start * _frameSize, _frameSize);
        }

        public ArraySegment<T> PushFront()
        {
            _start = (_start + _capacity - 1) % _capacity;
            if (IsFull)
                _end = _start;
            else
                ++_count;
            var rv = new ArraySegment<T>(_buffer, _start * _frameSize, _frameSize);
            rv.Clear();
            return rv;
        }
 
        public void PopBack()
        {
            if (_count == 0) throw new InvalidOperationException("Cannot access an empty buffer.");
            _end = (_end + _capacity - 1) % _capacity;
            Array.Clear(_buffer, _end * _frameSize, _frameSize);
            --_count;
        }

        public void PopFront()
        {
            if (_count == 0) throw new InvalidOperationException("Cannot access an empty buffer.");
            Array.Clear(_buffer, _start * _frameSize, _frameSize);
            _start = (_start + 1) % _capacity;
            --_count;
        }

        public void Clear()
        {
            _start = _end = _count = 0;
            Array.Clear(_buffer, 0, _capacity);
        }
    }
}