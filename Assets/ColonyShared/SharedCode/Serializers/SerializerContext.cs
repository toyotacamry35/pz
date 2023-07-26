using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SharedCode.EntitySystem;
using SharedCode.Refs.Operations;
using SharedCode.Serializers.Protobuf;
using SharedCode.Utils;

namespace SharedCode.Serializers
{
    public class _EntityContext
    {
        public static readonly DisposableThreadLocalPool<_EntityContext> Pool =
            new DisposableThreadLocalPool<_EntityContext>(100, 5, () => new _EntityContext(), (c) => c.Reset(),
                () => new _EntityContext());

        public bool FullCreating { get; set; } = false;

        public void Reset()
        {
            FullCreating = false;
        }
    }
    
    public class _SerializerContext
    {
        public static readonly DisposableThreadLocalPool<_SerializerContext> Pool =
            new DisposableThreadLocalPool<_SerializerContext>(100, 5, () => new _SerializerContext(), (c) => c.Reset(),
                () => new _SerializerContext());

            /// <summary>
            /// Сериализуем свойства независимо от их измененности
            /// </summary>
        public bool FullSerialize { get; set; } = false;
        public long ReplicationMask { get; set; } = long.MaxValue;
        public bool IsDump { get; set; } = false;
        public bool Deserialization { get; set; } = false;

        public Dictionary<ulong, DeserializedObjectInfo> ChangedObjects { get; private set; } =
            new Dictionary<ulong, DeserializedObjectInfo>();
        
        public IEntitiesRepository EntitiesRepository { get; set; }

        public void Reset()
        {
            FullSerialize = false;
            ReplicationMask = long.MaxValue;
            IsDump = false;
            EntitiesRepository = null;
            Deserialization = false;
            ChangedObjects.Clear();
        }
    }

    public class DisposableThreadLocalPool<T>
    {
        private static readonly Pool<LockScope> _lockPool = new Pool<LockScope>(100, 5, () => new LockScope(), (_) => {});
        private readonly Pool<T> _pool;

        public readonly ThreadLocal<T> _current;

        public T Current => _current.Value;

        private class LockScope : IDisposable
        {
            private DisposableThreadLocalPool<T> _parent;
            private T _prev;

            internal bool Disposed = false;

            public void Enter(DisposableThreadLocalPool<T> parent, T action)
            {
                Disposed = false;
                _parent = parent;
                _prev = _parent._current.Value;
                _parent._current.Value = action;
            }

            public void Dispose()
            {
                if (Disposed)
                    return;

                Disposed = true;

                _parent._pool.Return(_parent._current.Value);
                _parent._current.Value = _prev;

                _prev = default;
                _parent = null;
                _lockPool.Return(this);
            }
        }


        public DisposableThreadLocalPool(int maxPoolSize, int defaultPoolsize, Func<T> createAction, Action<T> resetAction, Func<T> defValCreator)
        {
            _pool = new Pool<T>(maxPoolSize, defaultPoolsize, createAction, resetAction);
            _current = new ThreadLocal<T>(defValCreator);
        }

        public IDisposable Set()
        {
            var @lock = _lockPool.Take();
            @lock.Enter(this, _pool.Take());
            return @lock;
        }
    }
}
