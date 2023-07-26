using System;

namespace ReactivePropsNs.ThreadSafe
{
    public interface IStream<out T> : IDisposable // : IObservable<T>
    {
        IDisposable Subscribe(IListener<T> listener);
    }
}