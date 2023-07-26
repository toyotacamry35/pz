using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;

namespace Infrastructure.Cloud
{
    public delegate Task InnerProcess(CancellationToken ct);
    public delegate Task InnerProcess<T>(T parameter, CancellationToken ct);

    public delegate Task AsyncProcess(InnerProcess inner, CancellationToken ct);
    public delegate Task AsyncProcessIn<In>(In parameter, InnerProcess inner, CancellationToken ct);
    public delegate Task AsyncProcessOut<Out>(InnerProcess<Out> inner, CancellationToken ct);
    public delegate Task AsyncProcessInOut<In, Out>(In parameter, InnerProcess<Out> inner, CancellationToken ct);

    public static class AsyncProcessExtensions
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetLogger("AsyncProcess");

        public static Task EmptyProcess(CancellationToken ct) => Task.Delay(TimeSpan.FromMilliseconds(-1), ct);

        private static Task EmptyProcess<T>(T t, CancellationToken ct) => Task.Delay(TimeSpan.FromMilliseconds(-1), ct);

        private static async Task Wrap(AsyncProcess proc, InnerProcess placeholder, CancellationToken outer)
        {
            using (var cts = new CancellationTokenSource())
            {
                var nextTask = proc(placeholder, cts.Token);
                var cancellation = EmptyProcess(outer);
                
                var task = await Task.WhenAny(nextTask, cancellation);

                cts.Cancel();
                await nextTask;
            }
        }

        private static async Task Wrap<T>(AsyncProcessOut<T> proc, InnerProcess<T> placeholder, CancellationToken prev)
        {
            using (var cts = new CancellationTokenSource())
            {
                var nextTask = proc(placeholder, cts.Token);
                try
                {
                    var cancellation = EmptyProcess(prev);
                    await await Task.WhenAny(nextTask, cancellation);
                }
                finally
                {
                    await ShutdownProcess(cts, nextTask);
                }
            }
        }

        private static async Task Wrap<T>(T t, AsyncProcessIn<T> proc, InnerProcess placeholder, CancellationToken prev)
        {
            using (var cts = new CancellationTokenSource())
            {
                var nextTask = proc(t, placeholder, cts.Token);
                try
                {
                    var cancellation = EmptyProcess(prev);
                    await await Task.WhenAny(nextTask, cancellation);
                }
                finally
                {
                    await ShutdownProcess(cts, nextTask);
                }
            }
        }

        private static async Task Wrap<T, T2>(T t, AsyncProcessInOut<T, T2> proc, InnerProcess<T2> placeholder, CancellationToken prev)
        {
            using (var cts = new CancellationTokenSource())
            {
                var nextTask = proc(t, placeholder, cts.Token);
                try
                {
                    var cancellation = EmptyProcess(prev);
                    await await Task.WhenAny(nextTask, cancellation);
                }
                finally
                {
                    await ShutdownProcess(cts, nextTask);
                }
            }
        }

        private static async Task Wrap<T, T2>(T t, AsyncProcessInOut<T, T2> proc, InnerProcess placeholder, CancellationToken prev)
        {
            using (var cts = new CancellationTokenSource())
            {
                var nextTask = proc(t, Convert<T2>(placeholder), cts.Token);
                try
                {
                    var cancellation = EmptyProcess(prev);
                    await await Task.WhenAny(nextTask, cancellation);
                }
                finally
                {
                    await ShutdownProcess(cts, nextTask);
                }
            }
        }

        private static InnerProcess<T> Convert<T>(InnerProcess raw)
        {
            return (t, ct) => raw(ct);
        }

        private static AsyncProcess Convert<T>(AsyncProcessOut<T> raw)
        {
            return (inner, ct) => raw(Convert<T>(inner), ct);
        }

        //----

        private static Task ThenInner(AsyncProcess outer, AsyncProcess inner, InnerProcess placeholder, CancellationToken prev)
        {
            Task nextProcess(CancellationToken ct) => Wrap(inner, placeholder, ct);
            return outer(nextProcess, prev);
        }

        private static Task ThenInner<T>(AsyncProcessOut<T> outer, AsyncProcessIn<T> inner, InnerProcess placeholder, CancellationToken prev)
        {
            Task nextProcess(T t, CancellationToken ct) => Wrap(t, inner, placeholder, ct);
            return outer(nextProcess, prev);
        }

        private static Task ThenInner<T>(AsyncProcessOut<T> outer, AsyncProcess inner, InnerProcess placeholder, CancellationToken prev)
        {
            Task nextProcess(T t, CancellationToken ct) => Wrap(inner, placeholder, ct);
            return outer(nextProcess, prev);
        }

        public static AsyncProcess Then(this AsyncProcess first, AsyncProcess second) => (inner, ct) => ThenInner(first, second, inner, ct);
        public static AsyncProcess Then<T>(this AsyncProcessOut<T> first, AsyncProcessIn<T> second) => (inner, ct) => ThenInner(first, second, inner, ct);
        public static AsyncProcess Then<T>(this AsyncProcessOut<T> first, AsyncProcess second) => (inner, ct) => ThenInner(first, second, inner, ct);

        //----

        private static Task ThenInner<T, T2>(T t, AsyncProcessInOut<T, T2> outer, AsyncProcessIn<T2> inner, InnerProcess placeholder, CancellationToken prev)
        {
            Task nextProcess(T2 t2, CancellationToken ct) => Wrap(t2, inner, placeholder, ct);
            return outer(t, nextProcess, prev);
        }

        private static Task ThenInner<T>(T t, AsyncProcessIn<T> outer, AsyncProcess inner, InnerProcess placeholder, CancellationToken prev)
        {
            Task nextProcess(CancellationToken ct) => Wrap(inner, placeholder, ct);
            return outer(t, nextProcess, prev);
        }

        private static Task ThenInner<T1, T2>(T1 t, AsyncProcessInOut<T1, T2> outer, AsyncProcess inner, InnerProcess placeholder, CancellationToken prev)
        {
            Task nextProcess(T2 t2, CancellationToken ct) => Wrap(inner, placeholder, ct);
            return outer(t, nextProcess, prev);
        }

        public static AsyncProcessIn<T> Then<T, T2>(this AsyncProcessInOut<T, T2> first, AsyncProcessIn<T2> second) => (t, inner, ct) => ThenInner(t, first, second, inner, ct);
        public static AsyncProcessIn<T> Then<T>(this AsyncProcessIn<T> first, AsyncProcess second) => (t, inner, ct) => ThenInner(t, first, second, inner, ct);
        public static AsyncProcessIn<T> Then<T, T2>(this AsyncProcessInOut<T, T2> first, AsyncProcess second) => (t, inner, ct) => ThenInner(t, first, second, inner, ct);

        //----

        private static Task ThenInner<T>(AsyncProcess outer, AsyncProcessOut<T> inner, InnerProcess<T> placeholder, CancellationToken prev)
        {
            Task nextProcess(CancellationToken ct) => Wrap(inner, placeholder, ct);
            return outer(nextProcess, prev);
        }

        private static Task ThenInner<T, T2>(AsyncProcessOut<T> outer, AsyncProcessInOut<T, T2> inner, InnerProcess<T2> placeholder, CancellationToken prev)
        {
            Task nextProcess(T t, CancellationToken ct) => Wrap(t, inner, placeholder, ct);
            return outer(nextProcess, prev);
        }

        private static Task ThenInner<T1, T2>(AsyncProcessOut<T1> outer, AsyncProcessOut<T2> inner, InnerProcess<T2> placeholder, CancellationToken prev)
        {
            Task nextProcess(T1 t2, CancellationToken ct) => Wrap(inner, placeholder, ct);
            return outer(nextProcess, prev);
        }

        public static AsyncProcessOut<T2> Then<T2>(this AsyncProcess first, AsyncProcessOut<T2> second) => (inner, ct) => ThenInner(first, second, inner, ct);
        public static AsyncProcessOut<T2> Then<T, T2>(this AsyncProcessOut<T> first, AsyncProcessInOut<T, T2> second) => (inner, ct) => ThenInner(first, second, inner, ct);
        public static AsyncProcessOut<T2> Then<T, T2>(this AsyncProcessOut<T> first, AsyncProcessOut<T2> second) => (inner, ct) => ThenInner(first, second, inner, ct);

        //----

        private static Task ThenInner<T1, T2>(T1 t, AsyncProcessIn<T1> outer, AsyncProcessOut<T2> inner, InnerProcess<T2> placeholder, CancellationToken prev)
        {
            Task nextProcess(CancellationToken ct) => Wrap(inner, placeholder, ct);
            return outer(t, nextProcess, prev);
        }

        private static Task ThenInner<T, T2, T3>(T t, AsyncProcessInOut<T, T2> outer, AsyncProcessInOut<T2, T3> inner, InnerProcess<T3> placeholder, CancellationToken prev)
        {
            Task nextProcess(T2 t2, CancellationToken ct) => Wrap(t2, inner, placeholder, ct);
            return outer(t, nextProcess, prev);
        }


        private static Task ThenInner<T1, T2, T3>(T1 t, AsyncProcessInOut<T1, T2> outer, AsyncProcessOut<T3> inner, InnerProcess<T3> placeholder, CancellationToken prev)
        {
            Task nextProcess(T2 t2, CancellationToken ct) => Wrap(inner, placeholder, ct);
            return outer(t, nextProcess, prev);
        }

        public static AsyncProcessInOut<T1, T2> Then<T1, T2>(this AsyncProcessIn<T1> first, AsyncProcessOut<T2> second) => (t, inner, ct) => ThenInner(t, first, second, inner, ct);
        public static AsyncProcessInOut<T1, T2> Then<T1, T2, TMiddle>(this AsyncProcessInOut<T1, TMiddle> first, AsyncProcessInOut<TMiddle, T2> second) => (t, inner, ct) => ThenInner(t, first, second, inner, ct);
        public static AsyncProcessInOut<T1, T2> Then<T1, T2, TMiddle>(this AsyncProcessInOut<T1, TMiddle> first, AsyncProcessOut<T2> second) => (t, inner, ct) => ThenInner(t, first, second, inner, ct);

        public static Task Exec(this AsyncProcess process, InnerProcess inner, CancellationToken ct) => Wrap(process, inner, ct);
        public static Task Exec(this AsyncProcess process, CancellationToken ct) => Wrap(process, EmptyProcess, ct);

        public static Task Exec<T>(this AsyncProcessOut<T> process, InnerProcess<T> inner, CancellationToken ct) => Wrap(process, inner, ct);
        public static Task Exec<T>(this AsyncProcessOut<T> process, InnerProcess inner, CancellationToken ct) => Wrap(process, Convert<T>(inner), ct);
        public static Task Exec<T>(this AsyncProcessOut<T> process, CancellationToken ct) => Wrap(process, EmptyProcess, ct);

        public static async Task ShutdownProcess(CancellationTokenSource cts, Task activity)
        {
            try
            {
                try
                {
                    cts.Cancel();
                    cts.Dispose();
                }
                catch(Exception)
                {
                }

                await activity;
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception e)
            {
                Logger.IfError()?.Exception(e).Write();
            }
        }
    }
}
