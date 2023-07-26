using System;

namespace SharedCode.Utils
{
    public interface IScopeAction
    {
        void Enter();
        void Leave();
    }

    public class AsyncScopeSystem<Action> where Action : IScopeAction
    {
        private static readonly Pool<LockScope> _pool = new Pool<LockScope>(100, 5, () => new LockScope(), (_) => { });

        private class LockScope : IDisposable
        {
            private Action _action;

            internal bool Disposed = false;

            public void Enter(Action action)
            {
                _action = action;
                _action.Enter();
            }

            public void Dispose()
            {
                if (Disposed)
                    return;

                Disposed = true;

                _action.Leave();

                _pool.Return(this);
            }
        }

        public IDisposable Lock(Action action)
        {
            var existing = _pool.Take();
            existing.Disposed = false;
            existing.Enter(action);

            return existing;
        }
    }
}
