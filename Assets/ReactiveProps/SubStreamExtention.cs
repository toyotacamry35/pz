using System;
using System.Collections.Generic;

namespace ReactivePropsNs
{
    public static class SubStreamExtention
    {
        /// <summary>
        ///  Способ скрыть от пользователя удаление VM, присвоение ссылке на VM нового значения. Для пользователя всегда видно лишь адресованное поле и просто
        /// меняются его значения. О замене самой VM он не знает.
        /// </summary>
        public static IStream<U> SubStream<T, U>(this IStream<T> source, ICollection<IDisposable> disposables, Func<T, IStream<U>> subStreamFunc,
            U defaultValue = default, bool reactOnChangesOnly = false)
        {
            var innerD = new DisposableComposite();
            var proxyRp = PooledReactiveProperty<U>.Create(reactOnChangesOnly).InitialValue(defaultValue);
            innerD.Add(proxyRp);
            IDisposable currentSubscibtion = null;

            innerD.Add(source.Subscribe(innerD,
                tValue =>
                {
                    if (currentSubscibtion != null)
                        currentSubscibtion.Dispose();

                    if (tValue != null)
                    {
                        var uStream = subStreamFunc(tValue);
                        if (uStream != null)
                        {
                            currentSubscibtion = uStream.Subscribe(innerD,
                                uValue => { proxyRp.Value = uValue; },
                                () =>
                                {
                                    proxyRp.Value = defaultValue;
                                    if (currentSubscibtion != null)
                                    {
                                        currentSubscibtion.Dispose();
                                        currentSubscibtion = null;
                                    }
                                }
                            );
                        }
                        else
                        {
                            proxyRp.Value = defaultValue;
                        }
                    }
                    else
                    {
                        proxyRp.Value = defaultValue;
                    }
                },
                () =>
                {
                    innerD.Clear();
                    currentSubscibtion?.Dispose();
                    currentSubscibtion = null;
                }
            ));

            foreach (var disposable in innerD)
                disposables.Add(disposable);

            return proxyRp;
        }
    }
}