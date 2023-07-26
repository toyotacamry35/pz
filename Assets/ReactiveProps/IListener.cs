using System;

namespace ReactivePropsNs
{
    public interface IListener<in T> : IIsDisposed
    {
        void OnNext(T value);
        void OnCompleted();
        /// <summary> Сбрасывать не надо, OnCompleted или на Dispose сами почистим </summary>
        void SetRequestLogHandler(Func<string, string> handler);
    }
}