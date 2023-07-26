using System;
using System.Collections.Generic;
using ReactivePropsNs;

namespace Assets.ReactiveProps
{
    public static class ListStreamTransformExtension
    {
        /// <summary>
        /// Вход - IListStream<T1> и функтор - как сделать из него IListStream<T2>
        /// Возвращает полученный IListStream<T2>
        /// </summary>
        public static IListStream<T2> Transform<T1, T2>(this IListStream<T1> source, ICollection<IDisposable> D, Func<T1, T2> factory)
            where T2 : IDisposable
        {
            ListStream<T2> output = new ListStream<T2>();
            source.InsertStream.Action(D, e => output.Insert(e.Index, factory(e.Item)));
            source.RemoveStream.Action(
                D,
                e =>
                {
                    output[e.Index]?.Dispose();
                    output.RemoveAt(e.Index);
                });
            source.ChangeStream.Action(
                D,
                e =>
                {
                    output[e.Index]?.Dispose();
                    output[e.Index] = factory(e.NewItem);
                });
            return output;
        }

        /// <summary>
        /// Вход - IListStream<T1> и функтор - как сделать из него IListStream<T2>
        /// Возвращает полученный IListStream<T2>
        /// </summary>
        public static ListStream<T2> Transform<T1, T2>(
            this IListStream<T1> source,
            ICollection<IDisposable> d,
            Func<T1, ICollection<IDisposable>, T2> factory) where T2 : IDisposable
        {
            var localD = new DisposableComposite();
            d.Add(localD);
            var disposableCompositeForEachItem = new System.Collections.Generic.List<DisposableComposite>();
            var output = new ListStream<T2>();
            localD.Add(output);
            source.InsertStream.Action(
                d,
                e =>
                {
                    var itemDc = new DisposableComposite();
                    localD.Add(itemDc);
                    disposableCompositeForEachItem.Insert(e.Index, itemDc);
                    var item = factory(e.Item, itemDc);
                    localD.Add(item);
                    output.Insert(e.Index, item);
                });
            source.RemoveStream.Action(
                d,
                e =>
                {
                    localD.Remove(disposableCompositeForEachItem[e.Index]);
                    disposableCompositeForEachItem[e.Index].Dispose();
                    output[e.Index]?.Dispose();
                    disposableCompositeForEachItem.RemoveAt(e.Index);
                    output.RemoveAt(e.Index);
                });
            source.ChangeStream.Subscribe(
                d,
                e =>
                {
                    localD.Remove(disposableCompositeForEachItem[e.Index]);
                    disposableCompositeForEachItem[e.Index].Dispose();
                    output[e.Index]?.Dispose();
                    var itemDc = new DisposableComposite();
                    localD.Add(itemDc);
                    var item = factory(e.NewItem, itemDc);
                    localD.Add(item);
                    output[e.Index] = factory(e.NewItem, itemDc);
                },
                () =>
                {
                    localD.Dispose();
                    d.Remove(localD);
                    disposableCompositeForEachItem.Clear();
                });

            return output;
        }

    }
}