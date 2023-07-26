using System;
using SharedCode.EntitySystem;

namespace ReactivePropsNs.Touchables
{
    /// <summary>
    /// Трогатель получает Entity два раза, один раз на входе когда ему даётся возможность на что-нибудь в ней подписаться, другой раз на выходе,
    /// когда он может где нужно отписаться.  Вызовы на подписку и отписку могут прийти в самых неожиданных thread-ах, обычно из ThreadPool
    /// и на это нельзя закладываться. В промежутке между этими вызовами линк на Entity не должен сохраняться и трогать его нельзя.
    /// </summary>
    public interface IToucher<T> : IIsDisposed where T : IDeltaObject
    {
        /// <summary>
        /// Вполне можем получить вместо Entity null, вернее default(T), если пока мы пытались к ней подконнектится она сдохла.
        /// Это не исключает получения OnRemove(null)
        /// </summary>
        void OnAdd(T deltaObject);
        /// <summary>
        /// Если Entity померло в процессе то вызов будет произведён, но с пустой ссылкой на Entity, default(T) если быть точным.  В этом случае можно
        /// не отписываться вообще или отписываться нетипичным способом. Но главное - почистить ссылки, чтобы не мешать GarbageCollector-у
        /// </summary>
        void OnRemove(T deltaObject);
        void OnCompleted();
        /// <summary> Сбрасывать не надо, OnCompleted или на Dispose сами почистим </summary>
        void SetRequestLogHandler(Func<string, string> handler);
    }
}