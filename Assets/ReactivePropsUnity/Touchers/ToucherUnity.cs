using System;
using SharedCode.EntitySystem;

namespace ReactivePropsNs.Touchables
{
    public class ToucherUnity<T> : IToucher<T> where T : IEntity
    {
        public bool IsDisposed { get; protected set; }

        public virtual void OnAdd(T entity)
        {
        }

        public virtual void OnRemove(T entity)
        {
        }

        public virtual void OnCompleted()
        {
        }

        public virtual void Dispose()
        {
            IsDisposed = true;
        }

        public virtual void SetRequestLogHandler(Func<string, string> handler)
        {
        }
    }
}