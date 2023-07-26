using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReactivePropsNs.Touchables
{
    public static class AsyncCalcerStreamExtention
    {
        //Пример использования
        //var v1 = new StreamProxy<int>();
        //var r1 = ReactiveProperty<decimal>.Create().DefaultValue(2m);
        //FuncAsync(r1.Zip(D, v1), D, SumAsync).Action(D, v => Debug.Log(v));
        //v1.OnNext(1);

        /// <summary>
        /// TODO Протестировать!
        /// Принимает стрим пар значений и асинхронный вычислитель, складирующий результаты в Task(R).
        /// Возвращает в Unity-потоке вычисленные значения в той же последовательности что и исходные данные, по мере готовности вычислений
        /// </summary>
        public static IStream<R> FuncAsync<T1, T2, R>(this IStream<ValueTuple<T1, T2>> source, ICollection<IDisposable> disposables,
            Func<T1, T2, Task<R>> functor)
        {
            var output = new UnityThreadStream<R>((string prefix) => $"{prefix} IStream<({typeof(T1).NiceName()}, {typeof(T2).NiceName()})>.FuncAsync<{typeof(R).NiceName()}>({disposables}, {functor})\n{source.DeepLog(prefix + '\t')}");
            disposables.Add(output);
            object queueLock = new object();
            Queue<Task<R>> queue = new Queue<Task<R>>();
            IDisposable subscribtion = null;
            subscribtion = source.Subscribe(disposables,
                value =>
                {
                    var t = functor(value.Item1, value.Item2);
                    queue.Enqueue(t);
                    t.ContinueWith((t1) =>
                    {
                        while (queue.Count > 0 && queue.Peek().IsCompleted)
                        {
                            var task = queue.Dequeue();
                            if (task.IsFaulted)
                                throw new System.Exception("", task.Exception);
                            output.OnNext(task.Result);
                        }
                    });
                },
                () =>
                {
                    subscribtion?.Dispose();
                    output.Dispose();
                }
            );
            disposables.Add(subscribtion);
            return output;
        }
    }
}