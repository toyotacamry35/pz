using System;
using System.Buffers;

namespace SharedCode.Utils
{
    public readonly struct PooledArray<T> : IDisposable // Главное не задиспозить дважды... 
    {
        private static readonly ArrayPool<T> _Pool = ArrayPool<T>.Create();

        private readonly T[] _array;
        private readonly bool _clear;
        private readonly bool _needDispose;

        public static PooledArray<T> Create(int capacity, bool clear = true)
        {
            return capacity > 0
                ? new PooledArray<T>(_Pool.Rent(capacity), true, clear)
                : new PooledArray<T>(System.Array.Empty<T>(), false, false);
        }

        private PooledArray(T[] array, bool needDispose, bool clear)
        {
            _array = array;
            _clear = clear;
            _needDispose = needDispose;
        }

        public T[] Array => _array;
        
        public ArraySegment<T> GetSegment(int offset, int count) => new ArraySegment<T>(_array, offset, count);
        
        public void Dispose()
        {
            if (_needDispose)
                _Pool.Return(_array, _clear);
        }
    }
}