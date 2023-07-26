using System;
using System.Threading;
using System.Threading.Tasks;

namespace SharedCode.Utils.Threads
{
    public static class TaskEx
    {
        public static Task Run(Action action) => Task.Run(action);
        public static Task Run(Action action, CancellationToken ct) => Task.Run(action, ct);
        public static Task<TResult> Run<TResult>(Func<TResult> function) => Task.Run(function);
        public static Task<TResult> Run<TResult>(Func<TResult> function, CancellationToken ct) => Task.Run(function, ct);
        public static Task Run(Func<Task> function) => Task.Run(function);
        public static Task Run(Func<Task> function, CancellationToken ct) => Task.Run(function, ct);
        public static Task<TResult> Run<TResult>(Func<Task<TResult>> function) => Task.Run(function);
        public static Task<TResult> Run<TResult>(Func<Task<TResult>> function, CancellationToken ct) => Task.Run(function, ct);
    }
}
