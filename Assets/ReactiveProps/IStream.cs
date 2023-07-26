using System;

namespace ReactivePropsNs
{
    public interface IStream<out T> : IIsDisposed
    {
        IDisposable Subscribe(IListener<T> listener);
        /// <summary> Метод медленный и нагружающий систему лишней информацией. Не в отладке не трогать. </summary>
        string DeepLog(string prefix);
    }
}