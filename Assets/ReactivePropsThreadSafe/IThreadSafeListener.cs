using System;

namespace ReactivePropsNs.ThreadSafe
{
    public interface IListener<in T> : IDisposable// : IObserver<T>
    {
        void OnNext(T value);
        void OnCompleted();
    }
}