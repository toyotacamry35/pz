using System;
using System.Collections.Generic;
using System.Linq;

namespace ReactivePropsNs
{
    public static class SubListStreamExtension
    {
        /// <summary>
        /// ??вроде как так??:
        /// Вход: стрим моделей, которая содержит и функтор - как наковырять из неё ListStream.
        /// Возвращает: инстанс ListStream, в который будут повтряться все изменения, происходящие с оригинальным.
        /// </summary>
        public static ListStream<U> SubListStream<T, U>(
            this IStream<T> source,
            ICollection<IDisposable> disposables,
            Func<T, IListStream<U>> listStreamFunc,
            bool reactOnChangesOnly = false)
        {
            var innerD = new DisposableComposite();
            disposables.Add(innerD);
            var proxyListStream = new ListStream<U>(null, reactOnChangesOnly);
            innerD.Add(proxyListStream);
            var tempoD = new DisposableComposite();

            IListStream<U> connected = default;

            void Connect(IListStream<U> list)
            {
                if (connected == list)
                    return;

                if (connected != null)
                    Disconnect();

                if (list == null)
                    return;

                connected = list;
                tempoD.Add(
                    list.InsertStream.Subscribe(
                        tempoD,
                        insertEvent => proxyListStream.Insert(insertEvent.Index, insertEvent.Item),
                        Disconnect));
                tempoD.Add(list.RemoveStream.Subscribe(tempoD, removeEvent => proxyListStream.RemoveAt(removeEvent.Index), Disconnect));
                tempoD.Add(
                    list.ChangeStream.Subscribe(
                        tempoD,
                        changeEvent => proxyListStream[changeEvent.Index] = changeEvent.NewItem,
                        Disconnect));
            }

            void Disconnect()
            {
                if (connected != null)
                {
                    tempoD.Clear();
                    proxyListStream.Clear();
                    connected = null;
                }
            }

            innerD.Add(
                source.Subscribe(
                    innerD,
                    tValue =>
                    {
                        if (tValue != null)
                            Connect(listStreamFunc(tValue));
                        else
                            Disconnect();
                    },
                    () =>
                    {
                        Disconnect();
                        innerD.Dispose();
                    }
                ));

            return proxyListStream;
        }

        /// <summary>
        /// Прилетающий null нужно обрабатывать самостоятельно внутри enumerableGetter.
        /// Сделано, чтобы доставать IEnumerable (напр. простой лист) из source.
        /// Чтобы достать IListStream используй одноимённое расширение (с 4-м bool-параметром)
        /// </summary>
        public static ListStream<U> NonMutableEnumerableAsSubListStream<T, U>(
            this IStream<T> source,
            ICollection<IDisposable> externalD,
            Func<T, IEnumerable<U>> enumerableGetter)
        {
            var localD = externalD.CreateInnerD();
            var listStream = new ListStream<U>();
            localD.Add(listStream);
            source.Subscribe(
                localD,
                element =>
                {
                    listStream.Clear();
                    foreach (var item in enumerableGetter(element) ?? Enumerable.Empty<U>()) listStream.Add(item);
                },
                () => { externalD.DisposeInnerD(localD); });
            return listStream;
        }
    }
}