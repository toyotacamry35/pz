using System;
using System.Collections.Generic;

namespace ReactivePropsNs
{
    public static class TransformStreamExtensions
    {
        public static ReactiveProperty<T2> Transform<T1, T2>(
            this IStream<T1> source,
            ICollection<IDisposable> externalD,
            Func<T1, ICollection<IDisposable>, T2> factory,
            bool reactOnChangesOnly = true) where T2 : IDisposable
        {
            var localD = externalD.CreateInnerD();
            var output = new ReactiveProperty<T2>(reactOnChangesOnly);
            localD.Add(output);

            var currentD = localD.CreateInnerD();

            source.Subscribe(
                localD,
                element =>
                {
                    localD.DisposeInnerD(currentD);
                    currentD = localD.CreateInnerD();

                    var innerD = currentD.CreateInnerD();
                    var item = factory(element, innerD);
                    if (item != null)
                        currentD.Add(item);

                    output.Value = item;
                },
                () => { externalD.DisposeInnerD(localD); });

            return output;
        }

        public static ReactiveProperty<T2> Transform<T1, T2>(
            this IStream<T1> source,
            ICollection<IDisposable> externalD,
            Func<T1, T2> factory,
            bool reactOnChangesOnly = true) where T2 : IDisposable
        {
            return Transform(source, externalD, (t1, localD) => factory(t1), reactOnChangesOnly);
            // var localD = externalD.CreateInnerD();
            // var output = new ReactiveProperty<T2>(reactOnChangesOnly);
            // localD.Add(output);
            //
            // var currentD = localD.CreateInnerD();
            //
            // source.Subscribe(
            //     localD,
            //     element =>
            //     {
            //         localD.DisposeInnerD(currentD);
            //         currentD = localD.CreateInnerD();
            //
            //         var item = factory(element);
            //         if (item != null)
            //             currentD.Add(item);
            //
            //         output.Value = item;
            //     },
            //     () => { externalD.DisposeInnerD(localD); });
            //
            // return output;
        }
        
        public static DisposableComposite CreateInnerD(this ICollection<IDisposable> externalD)
        {
            var innerD = new DisposableComposite();
            externalD.Add(innerD);
            return innerD;
        }

        public static void DisposeInnerD(this ICollection<IDisposable> externalD, IDisposable innerD)
        {
            externalD.Remove(innerD);
            innerD.Dispose();
        }
    }
}