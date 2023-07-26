using SharedCode.Utils;
using System;
using System.Runtime.Serialization;
using System.Threading;


namespace SharedCode.EntitySystem
{
    public class RemoteCallsLockedException : Exception
    {
        public RemoteCallsLockedException()
        {
        }

        public RemoteCallsLockedException(string message) : base(message)
        {
        }

        public RemoteCallsLockedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RemoteCallsLockedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    public static class EntitySystemBlock
    {
        private static readonly AsyncLocal<int> _locks = new AsyncLocal<int>();

        private static readonly AsyncScopeSystem<ScopeActionBlockEntitySystem> _scopeSystem = new AsyncScopeSystem<ScopeActionBlockEntitySystem>();

        private struct ScopeActionBlockEntitySystem : IScopeAction
        {
            public void Enter()
            {
                _locks.Value += 1;
            }

            public void Leave()
            {
                _locks.Value -= 1;
            }
        }

        public static IDisposable Lock
        {
            get => _scopeSystem.Lock(new ScopeActionBlockEntitySystem());
        }

        public static void ThrowIfLocked()
        {
            if (_locks.Value != 0)
                Throw();
        }

        public static bool Locked => _locks.Value != 0;

        public static void Throw() => throw new RemoteCallsLockedException("Entity system is locked");
    }
}
