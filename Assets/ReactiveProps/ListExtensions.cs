using System;
using System.Collections.Generic;

namespace ReactivePropsNs
{
    public static class ListExtensions
    {
        /// <summary>
        /// Преобразование типа элементов: из ListStream(T) в ListStream(U) через func
        /// </summary>
        public static ListStream<U> Func<T, U>(this ListStream<T> list, ICollection<IDisposable> disposables, Func<T, U> func)
        {
            var innerD = new DisposableComposite();
            if (list == null)
            {
                var completedStream = new ListStream<U>();
                completedStream.Dispose();
                return completedStream;
            }

            var outStrm = new ListStream<U>();
            innerD.Add(outStrm);

            innerD.Add(list.InsertStream.Subscribe(innerD, insertEvent => outStrm.Insert(insertEvent.Index, func(insertEvent.Item)), () => innerD.Clear()));
            innerD.Add(list.RemoveStream.Subscribe(innerD, removeEvent => outStrm.RemoveAt(removeEvent.Index), () => innerD.Clear()));
            innerD.Add(list.ChangeStream.Subscribe(innerD, changeEvent => outStrm[changeEvent.Index] = func(changeEvent.NewItem), () => innerD.Clear()));
            foreach (var disposable in innerD)
                disposables.Add(disposable);
            return outStrm;
        }
        
        public static ListStream<T> ConcatListStreams<T>(this ListStream<T> list1, ICollection<IDisposable> disposables, ListStream<T> list2)
        {
            var innerD = new DisposableComposite();
            if (list1 == null && list2 == null)
            {
                var completedStream = new ListStream<T>();
                completedStream.Dispose();
                return completedStream;
            }

            if (list1 == null)
                return list2;

            if (list2 == null)
                return list1;

            var outStream = new ListStream<T>();
            innerD.Add(outStream);

            innerD.Add(list1.InsertStream.Subscribe(innerD, insertEvent => outStream.Insert(insertEvent.Index, insertEvent.Item), () => innerD.Clear()));
            innerD.Add(list1.RemoveStream.Subscribe(innerD, removeEvent => outStream.RemoveAt(removeEvent.Index), () => innerD.Clear()));
            innerD.Add(list1.ChangeStream.Subscribe(innerD, changeEvent => outStream[changeEvent.Index] = changeEvent.NewItem, () => innerD.Clear()));

            innerD.Add(list2.InsertStream.Subscribe(innerD, insertEvent => outStream.Insert(list1.Count + insertEvent.Index, insertEvent.Item), () => innerD.Clear()));
            innerD.Add(list2.RemoveStream.Subscribe(innerD, removeEvent => outStream.RemoveAt(list1.Count + removeEvent.Index), () => innerD.Clear()));
            innerD.Add(list2.ChangeStream.Subscribe(innerD, changeEvent => outStream[list1.Count + changeEvent.Index] = changeEvent.NewItem, () => innerD.Clear()));
            
            foreach (var disposable in innerD)
                disposables.Add(disposable);
            
            return outStream;
        }

        public static void CopyTo<T>(this IListStream<T> source, ICollection<IDisposable> externalD, IList<T> target)
        {
            source.InsertStream.Action(externalD, e => target.Insert(e.Index, e.Item));
            source.RemoveStream.Action(externalD, e => target.RemoveAt(e.Index));
            source.ChangeStream.Subscribe(externalD, e => target[e.Index] = e.NewItem, target.Clear);
        }

        /// <summary>
        /// Стрим изменений list, где в качестве параметра рассылается собственно list
        /// </summary>
        public static IStream<ListStream<T>> ListChanges<T>(this ListStream<T> list, ICollection<IDisposable> disposables)
        {
            var innerD = new DisposableComposite();
            if (list == null)
            {
                return CompletedStream<ListStream<T>>.Empty;
            }

            var outStream = PooledStreamProxy<ListStream<T>>.Create(subscription => subscription.OnNext(list)); //при подписке передаем текущее состояние листа
            innerD.Add(outStream);

            innerD.Add(list.InsertStream.Subscribe(innerD, insertEvent => outStream.OnNext(list), () => innerD.Clear()));
            innerD.Add(list.RemoveStream.Subscribe(innerD, removeEvent => outStream.OnNext(list), () => innerD.Clear()));
            innerD.Add(list.ChangeStream.Subscribe(innerD, changeEvent => outStream.OnNext(list), () => innerD.Clear()));
            //Непосредственно при подписке outStream на list.InsertStream, из него придет череда ивентов, строящих лист. Мы их пропускаем
            foreach (var disposable in innerD)
                disposables.Add(disposable);
            return outStream;
        }

        /// <summary>
        /// Подписка на IListStream, в OnNext() рассылается ListStream<T>-клон
        /// </summary>
        public static IStream<ListStream<T>> ListChanges<T>(this IListStream<T> list, ICollection<IDisposable> disposables)
        {
            DisposableComposite innerD = new DisposableComposite();
            if (list == null)
            {
                return CompletedStream<ListStream<T>>.Empty;
            }

            var innerList = new ListStream<T>();
            innerD.Add(innerList);
            var outStream = PooledStreamProxy<ListStream<T>>.Create(subscription => subscription.OnNext(innerList)); //при подписке передаем текущее состояние листа
            innerD.Add(outStream);

            innerD.Add(list.InsertStream.Subscribe(innerD, insertEvent =>
                {
                    innerList.Insert(insertEvent.Index, insertEvent.Item);
                    outStream.OnNext(innerList);
                },
                () => innerD.Clear()));
            innerD.Add(list.RemoveStream.Subscribe(innerD, removeEvent =>
                {
                    innerList.RemoveAt(removeEvent.Index);
                    outStream.OnNext(innerList);
                },
                () => innerD.Clear()));
            innerD.Add(list.ChangeStream.Subscribe(innerD, changeEvent =>
            {
                innerList[changeEvent.Index] = changeEvent.NewItem;
                outStream.OnNext(innerList);
            }, () => innerD.Clear()));
            //Собран innerList - клон list
            //При подписке на outStream получаем OnNext(innerList) и далее события при любых его изменениях
            foreach (var disposable in innerD)
                disposables.Add(disposable);
            return outStream;
        }

        /// <summary>
        /// Получение из интерфейса экземпляра ListStream. Внутренний метод
        /// </summary>
        public static ListStream<T> ToListStream<T>(this IListStream<T> list, ICollection<IDisposable> disposables, bool reactOnChangesOnly = true)
        {
            if (list == null)
            {
                var completedStream = new ListStream<T>();
                completedStream.Dispose();
                return completedStream;
            }

            var innerD = new DisposableComposite();
            var outListStream = new ListStream<T>(null, reactOnChangesOnly);
            innerD.Add(outListStream);

            innerD.Add(list.InsertStream.Subscribe(innerD, insertEvent => { outListStream.Insert(insertEvent.Index, insertEvent.Item); },
                () => innerD.Clear()));
            innerD.Add(list.RemoveStream.Subscribe(innerD, removeEvent => { outListStream.RemoveAt(removeEvent.Index); }, () => innerD.Clear()));
            innerD.Add(list.ChangeStream.Subscribe(innerD, changeEvent => { outListStream[changeEvent.Index] = changeEvent.NewItem; }, () => innerD.Clear()));

            foreach (var disposable in innerD)
                disposables.Add(disposable);

            return outListStream;
        }
        
        public static ListStream<TOut> FuncMutable<TIn, TOut>(
            this IListStream<TIn> source,
            ICollection<IDisposable> externalD,
            IStream<Func<TIn, ICollection<IDisposable>, TOut>> factoryStream)
        {
            var localD = externalD.CreateInnerD();
            var output = new ListStream<TOut>();
            localD.Add(output);
            
            var produced = new List<IDisposable>();
            localD.Add(new DisposeAgent(produced.Clear));

            var currentD = localD.CreateInnerD();

            factoryStream.Subscribe(
                localD,
                factory =>
                {
                    localD.DisposeInnerD(currentD);
                    currentD = localD.CreateInnerD();
                    produced.Clear();

                    source.InsertStream.Action(currentD,
                        e =>
                        {
                            Create(e.Index, e.Item);
                        });
                    source.ChangeStream.Action(
                        currentD,
                        e =>
                        {
                            Destroy(e.Index);
                            Create(e.Index, e.NewItem);
                        });
                    source.RemoveStream.Subscribe(currentD, e=>
                    {
                        Destroy(e.Index);
                    }, output.Clear);

                    void Create(int index, TIn item)
                    {
                        var innerD = currentD.CreateInnerD();
                        var outItem = factory.Invoke(item, innerD);
                        produced[index] = innerD;
                        output[index] = outItem;
                    }

                    void Destroy(int index)
                    {
                        var innerD = produced[index];
                        output.RemoveAt(index);
                        currentD.DisposeInnerD(innerD);
                    }
                },
                () => externalD.DisposeInnerD(localD));

            return output;
        }
    }
}