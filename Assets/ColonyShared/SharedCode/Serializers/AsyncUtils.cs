using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using GeneratedCode.Manual.AsyncStack;
using GeneratedCode.Manual.Repositories;
using GeneratedCode.Repositories;
using NLog;
using ShareCode.Threading;
using SharedCode.EntitySystem;
using SharedCode.Utils;

namespace SharedCode.Serializers
{
    public readonly struct SuspendingAwaitable : IEquatable<SuspendingAwaitable>
    {
        private readonly Task _task;
        public TaskAwaiter GetAwaiter() => SuspendResume(_task).GetAwaiter();
        public bool IsCompleted => _task.IsCompleted;
        public bool IsFaulted => _task.IsFaulted;
        public void Wait()
        {
            if (_task.IsCompleted)
                _task.Wait();
            else
                throw new InvalidOperationException("You shall not block");
        }

        public SuspendingAwaitable(Task task)
        {
            _task = task;
        }

        private static async Task SuspendResume(Task inner)
        {
            if (inner.IsCompleted)
            {
                await inner;
                return;
            }

            var head = AsyncEntitiesRepositoryRequestContext.Head;
            if (head != null)
                head.Context.Release();

            try
            {
                await inner;
            }
            finally
            {
                if (AsyncEntitiesRepositoryRequestContext.Head != head)
                    throw new AsyncContextException("Async stack head mismatch");

                AsyncEntitiesRepositoryRequestContext.AssertNoChildren();
                if (head != null)
                    await head.Context.Relock();
            }
        }

        private static async Task<SuspendingAwaitable> AnyInternal(Task<Task> task)
        {
            return new SuspendingAwaitable(await task);
        }

        public static SuspendingAwaitable<SuspendingAwaitable> WhenAny(IEnumerable<SuspendingAwaitable> awaitables)
        {
            var allTask = Task.WhenAny(awaitables.Select(v => v._task));
            var inner = AnyInternal(allTask);
            return new SuspendingAwaitable<SuspendingAwaitable>(inner);
        }

        public static SuspendingAwaitable WhenAll(IEnumerable<SuspendingAwaitable> awaitables)
        {
            var allTask = Task.WhenAll(awaitables.Select(v => v._task));
            return new SuspendingAwaitable(allTask);
        }

        public override bool Equals(object obj) => obj is SuspendingAwaitable awaitable && Equals(awaitable);

        public bool Equals(SuspendingAwaitable other) => EqualityComparer<Task>.Default.Equals(_task, other._task);

        public override int GetHashCode() => _task.GetHashCode();

        public static bool operator ==(SuspendingAwaitable left, SuspendingAwaitable right) => left.Equals(right);

        public static bool operator !=(SuspendingAwaitable left, SuspendingAwaitable right) => !(left == right);
    }

    public readonly struct SuspendingAwaitable<T> : IEquatable<SuspendingAwaitable<T>>
    {
        private readonly Task<T> _task;
        public TaskAwaiter<T> GetAwaiter() => SuspendResume(_task).GetAwaiter();
        public bool IsCompleted => _task.IsCompleted;
        public bool IsFaulted => _task.IsFaulted;
        public T Result => _task.IsCompleted ? _task.Result : throw new InvalidOperationException("You shall not block");
        public void Wait() 
        {
            if (_task.IsCompleted)
                _task.Wait();
            else 
                throw new InvalidOperationException("You shall not block"); 
        }

        public SuspendingAwaitable(Task<T> task)
        {
            _task = task;
        }

        private static async Task<T> SuspendResume(Task<T> inner)
        {
            if (inner.IsCompleted)
            {
                return await inner;
            }

            var head = AsyncEntitiesRepositoryRequestContext.Head;
            if (head != null)
                head.Context.Release();

            try
            {
                return await inner;
            }
            finally
            {
                if (AsyncEntitiesRepositoryRequestContext.Head != head)
                    throw new AsyncContextException("Async stack head mismatch");

                AsyncEntitiesRepositoryRequestContext.AssertNoChildren();
                if (head != null)
                    await head.Context.Relock();
            }
        }

        private static async Task<SuspendingAwaitable<T>> AnyInternal(Task<Task<T>> task)
        {
            return new SuspendingAwaitable<T>(await task);
        }

        public static SuspendingAwaitable<SuspendingAwaitable<T>> WhenAny(IEnumerable<SuspendingAwaitable<T>> awaitables)
        {
            var allTask = Task.WhenAny(awaitables.Select(v => v._task));
            var inner = AnyInternal(allTask);
            return new SuspendingAwaitable<SuspendingAwaitable<T>>(inner);
        }

        public static SuspendingAwaitable<T[]> WhenAll(IEnumerable<SuspendingAwaitable<T>> awaitables)
        {
            var allTask = Task.WhenAll(awaitables.Select(v => v._task));
            return new SuspendingAwaitable<T[]>(allTask);
        }

        public static explicit operator SuspendingAwaitable(in SuspendingAwaitable<T> awaitable) => new SuspendingAwaitable(awaitable._task);

        public static bool operator ==(SuspendingAwaitable<T> left, SuspendingAwaitable<T> right) => left.Equals(right);

        public static bool operator !=(SuspendingAwaitable<T> left, SuspendingAwaitable<T> right) => !(left == right);

        public override bool Equals(object obj) => obj is SuspendingAwaitable<T> awaitable && Equals(awaitable);

        public bool Equals(SuspendingAwaitable<T> other) => EqualityComparer<Task<T>>.Default.Equals(_task, other._task);

        public override int GetHashCode() => _task.GetHashCode();
    }

    public static class AsyncUtils
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public static SuspendingAwaitable RunAsyncTask(Action action, IEntitiesRepository _ = null)
        {
            if (action == null)
            {
                Logger.IfError()?.Message("action == null").Write();
                throw new ArgumentNullException(nameof(action));
            }

            var stackTrace = StackTraceUtils.GetStackTrace();
            var task = IsolatingDelegate(action, stackTrace);
            return new SuspendingAwaitable(task);
        }

        public static SuspendingAwaitable<T> RunAsyncTask<T>(Func<T> action, IEntitiesRepository _ = null)
        {
            if (action == null)
            {
                Logger.IfError()?.Message("action == null").Write();
                throw new ArgumentNullException(nameof(action));
            }

            var stackTrace = StackTraceUtils.GetStackTrace();
            var task = IsolatingDelegate(action, stackTrace);
            return new SuspendingAwaitable<T>(task);
        }

        public static SuspendingAwaitable RunAsyncTask(Func<Task> action, IEntitiesRepository _ = null)
        {
            if(action == null)
			{
				Logger.IfError()?.Message("action == null").Write();
                throw new ArgumentNullException(nameof(action));
			}

            var stackTrace = StackTraceUtils.GetStackTrace();
            var task = IsolatingDelegate(action, stackTrace);
            return new SuspendingAwaitable(task);
        }

        public static SuspendingAwaitable<T> RunAsyncTask<T>(Func<Task<T>> action, IEntitiesRepository _ = null)
        {
            if (action == null)
            {
                Logger.IfError()?.Message("action == null").Write();
                throw new ArgumentNullException(nameof(action));
            }

            var stackTrace = StackTraceUtils.GetStackTrace();
            var task = IsolatingDelegate(action, stackTrace);
            return new SuspendingAwaitable<T>(task);
        }

        private static async Task IsolatingDelegate(Action action, StackTrace stackTrace)
        {
            await Awaiters.ThreadPoolYield;

            using (AsyncStackIsolator.IsolateContext(stackTrace))
            {
                try
                {
                    action();
                }
                catch (OperationCanceledException e)
                {
                    Logger.IfInfo()?.Message(e, "runAsyncTask cancelled, called from: {0}", stackTrace?.ToString() ?? string.Empty).Write();
                    throw;
                }
                catch (Exception e)
                {
                    Logger.IfError()?.Message(e, "runAsyncTask exception, called from: {0}", stackTrace?.ToString() ?? string.Empty).Write();
                    throw;
                }
            }
        }

        private static async Task<T> IsolatingDelegate<T>(Func<T> action, StackTrace stackTrace)
        {
            await Awaiters.ThreadPoolYield;

            using (AsyncStackIsolator.IsolateContext(stackTrace))
            {
                try
                {
                    return action();
                }
                catch (OperationCanceledException e)
                {
                    Logger.IfInfo()?.Message(e, "runAsyncTask cancelled, called from: {0}", stackTrace?.ToString() ?? string.Empty).Write();
                    throw;
                }
                catch (Exception e)
                {
                    Logger.IfError()?.Message(e, "runAsyncTask exception, called from: {0}", stackTrace?.ToString() ?? string.Empty).Write();
                    throw;
                }
            }
        }

        private static async Task IsolatingDelegate(Func<Task> action, StackTrace stackTrace)
        {
            await Awaiters.ThreadPoolYield;

            using (AsyncStackIsolator.IsolateContext(stackTrace))
            {
                try
                {
                    await action();
                }
                catch (OperationCanceledException e)
                {
                    Logger.IfInfo()?.Message(e, "runAsyncTask cancelled, called from: {0}", stackTrace?.ToString() ?? string.Empty).Write();
                    throw;
                }
                catch (Exception e)
                {
                    Logger.IfError()?.Message(e, "runAsyncTask exception, called from: {0}", stackTrace?.ToString() ?? string.Empty).Write();
                    throw;
                }
            }
        }

        private static async Task<T> IsolatingDelegate<T>(Func<Task<T>> action, StackTrace stackTrace)
        {
            await Awaiters.ThreadPoolYield;

            using (AsyncStackIsolator.IsolateContext(stackTrace))
            {
                try
                {
                    return await action();
                }
                catch (OperationCanceledException e)
                {
                    Logger.IfInfo()?.Message(e, "runAsyncTask cancelled, called from: {0}", stackTrace?.ToString() ?? string.Empty).Write();
                    throw;
                }
                catch (Exception e)
                {
                    Logger.IfError()?.Message(e, "runAsyncTask exception, called from: {0}", stackTrace?.ToString() ?? string.Empty).Write();
                    throw;
                }
            }
        }

        private static long LastTimeoutId = 0;

        private readonly struct Timeout : ITimeoutPayload
        {
            private readonly string caller;
            private readonly Func<string> tag;
            private readonly long contextId;
            private readonly long timeoutId;

            private readonly DateTime startTime;

            public Timeout(in DateTime startTime, string caller, Func<string> tag, long contextId, long timeoutId)
            {
                this.caller = caller;
                this.tag = tag;
                this.contextId = contextId;
                this.timeoutId = timeoutId;
                this.startTime = startTime;
            }

            public bool Run()
            {
                try
                {
                    var duration = DateTime.UtcNow - startTime;
                    Logger.Error("{0} run async with check timeout takes too long {1} elapsed: {2} asyncContextid:{3} timeoutId:{4}", 
                        caller ?? "null",
                        GetTagStr(tag, caller) ?? "null",
                        duration.TotalSeconds,
                        contextId, 
                        timeoutId);;
                }
                catch (Exception e)
                {
                    Logger.IfError()?.Exception(e).Write();
                }
                return false;
            }
        }

        public static async Task RunAsyncWithCheckTimeout(Func<Task> func, float timeout, Func<string> tag, [CallerMemberName] string caller = null, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
        {
            var contextId = AsyncEntitiesRepositoryRequestContext.Head?.Context.Id ?? 0;
            var timeoutId = Interlocked.Increment(ref LastTimeoutId);
            try
            {
                if (ServerCoreRuntimeParameters.EnableEntityUsagesAndEventsTimeouts)
                {
                    var startTime = DateTime.UtcNow;
                    var token = TimeoutSystem.Install(new Timeout(in startTime, caller, tag, contextId, timeoutId), TimeSpan.FromSeconds(timeout));
                    try
                    {
                        await func();
                    }
                    finally
                    {
                        token.Cancel();
                    }
                    var duration = DateTime.UtcNow - startTime;
                    if (duration >= TimeSpan.FromSeconds(ServerCoreRuntimeParameters.EntityEventTimeoutSeconds))
                        Logger.Error("{0} run async with check timeout took too long {1} elapsed: {2} asyncContextid:{3} timeoutId:{4}", 
                            caller ?? "null",
                            GetTagStr(tag, caller) ?? "null", 
                            duration.TotalSeconds,
                            contextId, 
                            timeoutId);
                }
                else
                    await func();
            }
            catch (Exception e)
            {
                Logger.IfError()?.Message(e, "{0} exception run async with check timeout {1} asyncContextid:{2} timeoutId:{3}", caller ?? "null", GetTagStr(tag, caller) ?? "null", contextId, timeoutId).Write();
                throw;
            }
        }

        public static async Task<T> RunAsyncWithCheckTimeout<T>(Func<Task<T>> func, float timeout, Func<string> tag, [CallerMemberName] string caller = null, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
        {
            var contextId = AsyncEntitiesRepositoryRequestContext.Head?.Context.Id ?? 0;
            var timeoutId = Interlocked.Increment(ref LastTimeoutId);
            try
            {
                if (ServerCoreRuntimeParameters.EnableEntityUsagesAndEventsTimeouts)
                {
                    var startTime = DateTime.UtcNow;
                    var token = TimeoutSystem.Install(new Timeout(in startTime, caller, tag, contextId, timeoutId), TimeSpan.FromSeconds(timeout));
                    try
                    {
                        return await func();
                    }
                    finally
                    {
                        token.Cancel();
                        var duration = DateTime.UtcNow - startTime;
                        if (duration >= TimeSpan.FromSeconds(ServerCoreRuntimeParameters.EntityEventTimeoutSeconds))
                            Logger.Error("{0} run async with check timeout took too long {1} elapsed: {2} asyncContextid:{3} timeoutId:{4}",
                                caller ?? "null",
                                GetTagStr(tag, caller) ?? "null",
                                duration.TotalSeconds,
                                contextId,
                                timeoutId);
                    }
                }
                else
                    return await func();
            }
            catch (Exception e)
            {
                Logger.IfError()?.Message(e, "{0} exception run async with check timeout {1} asyncContextid:{2} timeoutId:{3}", caller ?? "null", GetTagStr(tag, caller) ?? "null", contextId, timeoutId).Write();
                throw;
            }
        }

        private static string GetTagStr(Func<string> tag, string caller)
        {
            string tagStr = null;
            try
            {
                tagStr = tag?.Invoke();
            }
            catch (Exception e2)
            {
                Logger.IfError()?.Message(e2, "{0} run async with check timeout get tag exception", caller ?? "null").Write();
            }
            return tagStr;
        }
    }
}
