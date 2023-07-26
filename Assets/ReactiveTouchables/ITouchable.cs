using System;
using SharedCode.EntitySystem;

namespace ReactivePropsNs.Touchables
{
    public interface ITouchable<T> : IIsDisposed where T : IDeltaObject
    {
        IDisposable Subscribe(IToucher<T> toucher);
        /// <summary> Метод медленный и нагружающий систему лишней информацией. Не в отладке не трогать. </summary>
        string DeepLog(string prefix);
    }
}