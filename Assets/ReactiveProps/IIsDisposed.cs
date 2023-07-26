using System;

namespace ReactivePropsNs
{
    public interface IIsDisposed : IDisposable
    {
        bool IsDisposed { get; }
    }
}