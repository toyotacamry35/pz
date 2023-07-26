using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Text;
using NLog;

namespace SharedCode.Utils
{
    public static class StringBuildersPool
    {
        private const int DefaultSize = 50 * 1024;
        private const int MaxPoolSize = 30;
        private static readonly ConcurrentStack<StringBuilder> _localSB = new ConcurrentStack<StringBuilder>(Enumerable.Range(0, 10).Select(x => new StringBuilder(DefaultSize)));

        public static StringBuilder Get
        {
            get
            {
                StringBuilder result;
                if (_localSB.TryPop(out result))
                    return result;
                return new StringBuilder(DefaultSize);
            }
        }

        public static string ToStringAndReturn(this StringBuilder stringBuilder)
        {
            var str = stringBuilder.ToString();
            stringBuilder.Clear();
            if (_localSB.Count < MaxPoolSize)
                _localSB.Push(stringBuilder);
            return str;
        }
    }

    public class Pool<T>
    {
        private readonly int _maxPoolSize;
        private readonly ConcurrentQueue<T> _poolQueue;
        private readonly Action<T> _resetAction;
        private readonly Func<T> _createAction;

        public Pool(int maxPoolSize, int defaultPoolsize, Func<T> createAction, Action<T> resetAction)
        {
            _maxPoolSize = maxPoolSize;
            _createAction = createAction;
            _resetAction = resetAction;
            // ConcurrentQueue doesn't allocates
            if (defaultPoolsize == 0)
                _poolQueue = new ConcurrentQueue<T>();
            else
                _poolQueue = new ConcurrentQueue<T>(Enumerable.Range(0, defaultPoolsize - 1).Select(x => _createAction()));
        }

        public T Take()
        {
            if (_poolQueue.TryDequeue(out var result))
                return result;
            return _createAction();
        }

        public void Return(T obj)
        {
            if (_resetAction != null)
                _resetAction(obj);
            if (_poolQueue.Count < _maxPoolSize)
                _poolQueue.Enqueue(obj);
        }
    }

    public static class StopWatchPool
    {
        private static Pool<Stopwatch> _pool = new Pool<Stopwatch>(5000, 1000, 
            () => new Stopwatch(), stopwatch => stopwatch.Reset());

        public static Pool<Stopwatch> Pool => _pool;
    }
}