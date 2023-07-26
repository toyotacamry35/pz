using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Core.Environment.Logging.Extension;
using ReactiveProps;
using SharedCode.Utils.BsonSerialization;

namespace ReactivePropsNs
{
    public abstract class Pool
    {
        public static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private const int _MinPoolCapacity = 10; 
        private const int _MinPoolOverflow = 10; 

        private static readonly List<WeakReference<Pool>> _Pools = new List<WeakReference<Pool>>();
        public static readonly HashSet<Type> _PoolsWithDefaultCapacity = new HashSet<Type>();

        public static Pool<T> Create<T>(int maxCapacity, int initialCapacity) where T : IPooledObject, new()
        {
            return new Pool<T>(() => new T(), maxCapacity, initialCapacity);
        }

        public static Pool<T> Create<T>(Func<T> ctor, int maxCapacity, int initialCapacity) where T : IPooledObject
        {
            return new Pool<T>(ctor, maxCapacity, initialCapacity);
        }

        public static Pool<T> Create<T>(Func<T> ctor, PoolSizes.CapacityInfo prms) where T : IPooledObject
        {
            return new Pool<T>(ctor, prms.MaxCapacity, prms.InitialCapacity);
        }

        public static void Dump()
        {
            if (Logger.IsInfoEnabled)
            {
                var sb = new StringBuilder();
                sb.Append("Pools:").AppendLine();
                Dump(sb);
                Logger.IfInfo()?.Write(sb.ToString());
            }
        }

        public static void Dump(StringBuilder sb)
        {
            int totalCount = 0, totalCreated = 0, totalInPool = 0, totalCapacity = 0, totalMemory = 0;
            lock(_Pools)
                foreach (var tuple in _Pools
                    .Select(r => (HasTarget: r.TryGetTarget(out var pool), Pool: pool))
                    .Where(x => x.HasTarget)
                    .OrderBy(x => x.Pool.Created)
                    .Reverse())
                {
                    totalCount++;
                    totalCreated += tuple.Pool.Created;
                    totalInPool += tuple.Pool.InPool;
                    totalCapacity += tuple.Pool.CapacityReached;
                    sb
                    .Append("Type:").Append(tuple.Pool.PooledType.NiceName())
                    .Append(" Created:").Append(tuple.Pool.Created)
                    .Append(" InPool:").Append(tuple.Pool.InPool)
                    .Append(" InUse:").Append(tuple.Pool.Created - (tuple.Pool.InPool + tuple.Pool.Overflow))
                    .Append(" CapacityReached:").Append(tuple.Pool.CapacityReached)
                    .Append(" CapacityLimit:").Append(tuple.Pool.CapacityLimit)
                    .Append("\n");
                }
            sb.Append("--------------------------------------------------------\n")
                .Append(" Pools:").Append(totalCount)
                .Append(" TotalCreated:").Append(totalCreated)
                .Append(" TotalInPool:").Append(totalInPool)
                .Append(" TotalInUse:").Append(totalCreated - totalInPool)
                .Append(" TotalCapacity:").Append(totalCapacity)
                .Append("\n");
        }
        
        public static void DumpWithOverflow()
        {
            if (Logger.IsInfoEnabled)
            {
                var sb = new StringBuilder();
                sb.Append("Pools with overflow:").AppendLine();
                Pool.DumpWithOverflow(sb);
                Logger.IfInfo()?.Write(sb.ToString());
            }
        }

        public static void DumpWithOverflow(StringBuilder sb)
        {
            lock(_Pools)
                foreach (var tuple in _Pools
                    .Select(r => (HasTarget: r.TryGetTarget(out var pool), Pool: pool))
                    .Where(x => x.HasTarget && x.Pool.Overflow >= _MinPoolOverflow && x.Pool.CapacityReached >= _MinPoolCapacity)
                    .Select(x => (TypeName: x.Pool.PooledType.FullNiceName(), Capacity: x.Pool.CapacityReached))
                )
                {
                    sb.Append("            {\"").Append(tuple.TypeName).Append("\",").Append(RoundCapacity(tuple.Capacity)).AppendLine("},");
                }
        }

        public static void DumpWithDefaultCapacity()
        {
            if (Logger.IsInfoEnabled)
            {
                var sb = new StringBuilder();
                sb.Append("Pools with default capacity:").AppendLine();
                Pool.DumpWithDefaultCapacity(sb);
                Logger.IfInfo()?.Write(sb.ToString());
            }
        }
        
        public static void DumpWithDefaultCapacity(StringBuilder sb)
        {
            lock(_Pools)
            lock (_PoolsWithDefaultCapacity)
                foreach (var tuple in _Pools
                    .Select(r => (HasTarget: r.TryGetTarget(out var pool), Pool: pool))
                    .Where(x => x.HasTarget && _PoolsWithDefaultCapacity.Contains(x.Pool.PooledType))
                    .Select(x => (TypeName: x.Pool.PooledType.FullNiceName(), Capacity: x.Pool.CapacityReached)))
                    sb.Append("            {\"").Append(tuple.TypeName).Append("\",").Append(RoundCapacity(tuple.Capacity)).AppendLine("},");
        }

        public static void GeneratePoolSizes(bool onlyIncrease)
        {
            var sb = new StringBuilder();
            sb.Append("// Generated by cheat Pools");
            Pool.GeneratePoolSizes(sb, onlyIncrease);
            Directory.CreateDirectory(Path.GetDirectoryName(PoolSizes.GeneratedFile));
            File.WriteAllText(PoolSizes.GeneratedFile, sb.ToString());
        }

        public static void GeneratePoolSizes(StringBuilder sb, bool onlyIncrease)
        {
            int totalCount = 0, totalCreated = 0, totalInPool = 0, totalCapacity = 0, totalMemory = 0;
            sb.Append(@"
using System;
using System.Collections.Generic;
namespace ReactiveProps
{
    public static partial class PoolSizes
    {
        private static readonly IReadOnlyDictionary<string, int> _Sizes = new Dictionary<string, int>
        {
");
            lock(_Pools)
                foreach (var tuple in _Pools
                    .Select(r => (HasTarget: r.TryGetTarget(out var pool), Pool: pool))
                    .Where(x => x.HasTarget)
                    .Select(x => (TypeName: x.Pool.PooledType.FullNiceName(), Capacity: onlyIncrease ? Math.Max(x.Pool.CapacityReached, x.Pool.CapacityLimit) : x.Pool.CapacityReached))
                    .OrderBy(x => x.TypeName))
                    sb.Append("            {\"").Append(tuple.TypeName).Append("\",").Append(RoundCapacity(tuple.Capacity)).AppendLine("},");
            sb.Append(@"
        };
    }
}
");
        }

        private static int RoundCapacity(int capacity) => capacity <= _MinPoolCapacity ? 0 : (capacity / 10 + 1) * 10;

        public static void AddPoolWithDefaultCapacity(Type poolType)
        {
            lock(_PoolsWithDefaultCapacity)
                _PoolsWithDefaultCapacity.Add(poolType);
        }
        
        
        protected static void RegisterPool(Pool pool)
        {
            lock(_Pools)
                _Pools.Add(new WeakReference<Pool>(pool));
        }
        
        protected abstract int Created { get; }
        protected abstract int InPool { get; }
        protected abstract int CapacityLimit { get; }
        protected abstract int CapacityReached { get; }
        protected abstract int Overflow { get; }
        protected abstract Type PooledType { get; }
    }
    
    public class Pool<T> : Pool where T : IPooledObject
    {
        //private readonly ConcurrentStack<T> _pool = new ConcurrentStack<T>(); // не имеет смысла (как и ConcurrentBag), так как внутри себя оно аллоцирует на каждом добавлении 
        private const int _LockTimeout = 100;
        private readonly Stack<T> _pool;
        private SpinLock _poolLock = new SpinLock();
        private readonly int _capacityLimit;
        private readonly bool _isValueType = typeof(T).IsValueType;
        private readonly Func<T> _ctor;
        private int _capacityReached;
        private int _objectsCreated;
        private int _overflow;

        internal Pool(Func<T> ctor, int capacityLimit, int initialCapacity)
        {
            _ctor = ctor ?? throw new ArgumentNullException(nameof(ctor));
            _capacityLimit = capacityLimit >= 0 ? capacityLimit : throw new ArgumentException(nameof(capacityLimit));;
            _pool = new Stack<T>(Math.Max(initialCapacity, 0));
            RegisterPool(this);
        }
        
        public T Acquire()
        {
            if (_pool.Count > 0 || _overflow > 0)
            {
                bool lockTaken = false;
                try
                {
                    _poolLock.TryEnter(_LockTimeout, ref lockTaken);
                    if (lockTaken)
                    {
                        if (_pool.Count > 0)
                        {
                            var obj = _pool.Pop();
                            obj.Released = false;
                            return obj;
                        }
                        else if (_overflow > 0)
                        {
                            --_overflow;
                        }
                    }
                    else 
                        Logger.IfError()?.Write($"Can't take lock in {GetType().GetFriendlyName()}.{nameof(Acquire)}");
                }
                finally
                {
                    if (lockTaken) _poolLock.Exit();
                }
            }

            Interlocked.Increment(ref _objectsCreated);
            return _ctor();
        }

        public void Release(T item)
        {
            if (!_isValueType && item == null) throw new ArgumentNullException(nameof(item));

            if (item.Released) throw new Exception($"Item {item} already released. Type:{item.GetType().NiceName()}");
            item.Released = true;
            (bool detected, int created, int released) overRelease = default;

            bool lockTaken = false;
            try
            {
                _poolLock.TryEnter(_LockTimeout, ref lockTaken);
                if (lockTaken)
                {
                    if (_pool.Count < _capacityLimit)
                        _pool.Push(item);
                    else
                        ++_overflow;

                    var totalCapacity = _pool.Count + _overflow;
                    if (_capacityReached < totalCapacity)
                    {
                        _capacityReached = totalCapacity;

                        if (_objectsCreated < totalCapacity)
                        {
                            overRelease.detected = true;
                            overRelease.created = _objectsCreated;
                            overRelease.released = totalCapacity;
                            _objectsCreated = totalCapacity;
                        }
                    }
                }
                else
                    Logger.IfError()?.Write($"Can't take lock in {GetType().GetFriendlyName()}.{nameof(Release)}");
            }
            finally
            {
                if (lockTaken) _poolLock.Exit();
            }

            if (overRelease.detected)
                Logger.IfError()?.Write($"The count of created items ({overRelease.created}) is less than the count of released items ({overRelease.released}) for {typeof(T).FullNiceName()}.");
        }

        protected override int Created => _objectsCreated;
        
        protected override int InPool => _pool.Count;

        protected override int CapacityLimit => _capacityLimit;
        
        protected override int CapacityReached => _capacityReached;

        protected override int Overflow => _overflow;

        protected override Type PooledType => typeof(T);
    }

    public interface IPooledObject
    {
        bool Released { get; set; }
    }
}