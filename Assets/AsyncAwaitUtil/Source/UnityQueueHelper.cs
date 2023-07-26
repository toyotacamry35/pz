using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using UnityAsyncAwaitUtil;
using UnityUpdate;
using GeneratedCode.Manual.Repositories;
using GeneratedCode.Manual.AsyncStack;
using SharedCode.Serializers;
using System.Diagnostics;
using SharedCode.Utils;
using NLog;
using UnityThreading;
using GeneratedCode.Network.Statistic;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using NLog.Fluent;

public static class UnityQueueHelper
{
    private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

    public static void RunInUnityThreadNoWait(
        Action action,
        [CallerMemberName] string methodName = null,
        [CallerFilePath] string filePath = null,
        [CallerLineNumber] int line = 0)
        => RunInUnityThread(action, methodName, filePath, line);

    public static void RunInUnityThreadNoWait(
        Func<Task> action,
        [CallerMemberName] string methodName = null,
        [CallerFilePath] string filePath = null,
        [CallerLineNumber] int line = 0)
        => RunInUnityThread(action, methodName, filePath, line);

    public static SuspendingAwaitable RunInUnityThread(
        Action action,
        [CallerMemberName] string methodName = null,
        [CallerFilePath] string filePath = null,
        [CallerLineNumber] int line = 0)
    {
        if (action == null)
        {
             Logger.IfError()?.Message("action == null").Write();;
            throw new ArgumentNullException(nameof(action));
        }

        var stackTrace = StackTraceUtils.GetStackTrace();
        var task = IsolatingDelegate(action, stackTrace, methodName, filePath, line);
        return new SuspendingAwaitable(task);
    }

    public static SuspendingAwaitable<T> RunInUnityThread<T>(
        Func<T> action,
        [CallerMemberName] string methodName = null,
        [CallerFilePath] string filePath = null,
        [CallerLineNumber] int line = 0)
    {
        if (action == null)
        {
             Logger.IfError()?.Message("action == null").Write();;
            throw new ArgumentNullException(nameof(action));
        }

        var stackTrace = StackTraceUtils.GetStackTrace();
        var task = IsolatingDelegate(action, stackTrace, methodName, filePath, line);
        return new SuspendingAwaitable<T>(task);
    }

    public static SuspendingAwaitable RunInUnityThread(
        Func<Task> action,
        [CallerMemberName] string methodName = null,
        [CallerFilePath] string filePath = null,
        [CallerLineNumber] int line = 0)
    {
        if (action == null)
        {
             Logger.IfError()?.Message("action == null").Write();;
            throw new ArgumentNullException(nameof(action));
        }

        var stackTrace = StackTraceUtils.GetStackTrace();
        var task = IsolatingDelegate(action, stackTrace, methodName, filePath, line);
        return new SuspendingAwaitable(task);
    }

    public static SuspendingAwaitable<T> RunInUnityThread<T>(
        Func<Task<T>> action,
        [CallerMemberName] string methodName = null,
        [CallerFilePath] string filePath = null,
        [CallerLineNumber] int line = 0)
    {
        if (action == null)
        {
             Logger.IfError()?.Message("action == null").Write();;
            throw new ArgumentNullException(nameof(action));
        }

        var stackTrace = StackTraceUtils.GetStackTrace();
        var task = IsolatingDelegate(action, stackTrace, methodName, filePath, line);
        return new SuspendingAwaitable<T>(task);
    }

    private static async Task IsolatingDelegate(Action action, StackTrace stackTrace,
        [CallerMemberName] string methodName = null,
        [CallerFilePath] string filePath = null,
        [CallerLineNumber] int line = 0)
    {
        if (SyncContextUtil.IsInUnity)
            Statistics<UnityQueueHelperStatistics>.Instance.Used(methodName);
        else
            Statistics<UnityQueueHelperStatistics>.Instance.UsedAsync(methodName);

        await Awaiters.UnityThread;

        using (AsyncStackIsolator.IsolateContext())
        {
            var sampler = CustomSamplerCache.Samplers.Get(methodName, filePath, line);
            sampler?.Begin();
            try
            {
                action();
            }
            catch (OperationCanceledException e)
            {
                Logger.IfInfo()?.Message(e, "Cancelled, called from: {0}", stackTrace?.ToString() ?? string.Empty).Write();
                throw;
            }
            catch (Exception e)
            {
                Logger.IfError()?.Message(e, "Exception, called from: {0}", stackTrace?.ToString() ?? string.Empty).Write();
                throw;
            }
            finally
            {
                sampler?.End();
            }
        }
    }

    private static async Task<T> IsolatingDelegate<T>(Func<T> action, StackTrace stackTrace,
        [CallerMemberName] string methodName = null,
        [CallerFilePath] string filePath = null,
        [CallerLineNumber] int line = 0)
    {
        if (SyncContextUtil.IsInUnity)
            Statistics<UnityQueueHelperStatistics>.Instance.Used(methodName);
        else
            Statistics<UnityQueueHelperStatistics>.Instance.UsedAsync(methodName);

        await Awaiters.UnityThread;

        using (AsyncStackIsolator.IsolateContext())
        {
            var sampler = CustomSamplerCache.Samplers.Get(methodName, filePath, line);
            sampler?.Begin();
            try
            {
                return action();
            }
            catch (OperationCanceledException e)
            {
                Logger.IfInfo()?.Message(e, "Cancelled, called from: {0}", stackTrace?.ToString() ?? string.Empty).Write();
                throw;
            }
            catch (Exception e)
            {
                Logger.IfError()?.Message(e, "Exception, called from: {0}", stackTrace?.ToString() ?? string.Empty).Write();
                throw;
            }
            finally
            {
                sampler?.End();
            }
        }
    }

    private static async Task IsolatingDelegate(Func<Task> action, StackTrace stackTrace,
        [CallerMemberName] string methodName = null,
        [CallerFilePath] string filePath = null,
        [CallerLineNumber] int line = 0)
    {
        if (SyncContextUtil.IsInUnity)
            Statistics<UnityQueueHelperStatistics>.Instance.Used(methodName);
        else
            Statistics<UnityQueueHelperStatistics>.Instance.UsedAsync(methodName);

        await Awaiters.UnityThread;

        using (AsyncStackIsolator.IsolateContext())
        {
            var sampler = CustomSamplerCache.Samplers.Get(methodName, filePath, line);
            sampler?.Begin();
            try
            {
                Task task;
                try
                {
                    task = action();
                }
                finally
                {
                    sampler?.End();
                }

                await task;
            }
            catch (OperationCanceledException e)
            {
                Logger.IfInfo()?.Message(e, "Cancelled, called from: {0}", stackTrace?.ToString() ?? string.Empty).Write();
                throw;
            }
            catch (Exception e)
            {
                Logger.IfError()?.Message(e, "Exception, called from: {0}", stackTrace?.ToString() ?? string.Empty).Write();
                throw;
            }
        }
    }

    private static async Task<T> IsolatingDelegate<T>(Func<Task<T>> action, StackTrace stackTrace,
    [CallerMemberName] string methodName = null,
    [CallerFilePath] string filePath = null,
    [CallerLineNumber] int line = 0)
    {
        if (SyncContextUtil.IsInUnity)
            Statistics<UnityQueueHelperStatistics>.Instance.Used(methodName);
        else
            Statistics<UnityQueueHelperStatistics>.Instance.UsedAsync(methodName);

        await Awaiters.UnityThread;

        using (AsyncStackIsolator.IsolateContext())
        {
            var sampler = CustomSamplerCache.Samplers.Get(methodName, filePath, line);
            sampler?.Begin();
            try
            {
                Task<T> task;
                try
                {
                    task = action();
                }
                finally
                {
                    sampler?.End();
                }

                return await task;
            }
            catch (OperationCanceledException e)
            {
                Logger.IfInfo()?.Message(e, "Cancelled, called from: {0}", stackTrace?.ToString() ?? string.Empty).Write();
                throw;
            }
            catch (Exception e)
            {
                Logger.IfError()?.Message(e, "Exception, called from: {0}", stackTrace?.ToString() ?? string.Empty).Write();
                throw;
            }
        }
    }

    public static void AssertInUnityThread() => AsyncStackHolder.ThrowIfNotInUnityContext();
}

public class UnityQueueHelperStatistics : CoreBaseStatistic
{
    ConcurrentDictionary<string, UnityQueueHelperStat> _currentStats = new ConcurrentDictionary<string, UnityQueueHelperStat>();
    ConcurrentDictionary<string, UnityQueueHelperStat> _currentAsyncStats = new ConcurrentDictionary<string, UnityQueueHelperStat>();
    
    private const int TopCount = 40;

    private static readonly NLog.Logger Logger = NLog.LogManager.GetLogger("Telemetry-UnityQueueHelper");

    protected override void LogStatistics()
    {
        var stats = _currentStats;
        var asyncStats = _currentAsyncStats;
        _currentStats = new ConcurrentDictionary<string, UnityQueueHelperStat>();
        _currentAsyncStats = new ConcurrentDictionary<string, UnityQueueHelperStat>();
        Thread.Sleep(100); //ждем, вдруг из другого потока прямо сейчас хотят дописать в старый Stat

        var count = stats.Values.Count;
        var asyncCount = asyncStats.Values.Count;
        if (count + asyncCount == 0)
            return;

        var takeCount = Math.Min(TopCount, count);
        var topUsagesStats = stats.Values.OrderByDescending(x => x.Count).Take(takeCount).ToList();

        var takeAsyncCount = Math.Min(TopCount, asyncCount);
        var topUsagesAsyncStats = asyncStats.Values.OrderByDescending(x => x.Count).Take(takeAsyncCount).ToList();

        Logger.Info()
            .Message("Total sync: {unity_queue_helper_sync_count}, Total async: {unity_queue_helper_async_count}", count, asyncCount)
            .Property("unity_queue_helper_stats",
            new StatInfo
            {
                OverallAsync = asyncStats.Values.Sum(x => x.CountField),
                Overall = stats.Values.Sum(x => x.CountField),
                TopUsagesAsync = topUsagesAsyncStats,
                TopUsages = topUsagesStats
            })
            .Write();
    }

    UnityQueueHelperStat getStat(string caller) => _currentStats.GetOrAdd(caller, (key) => new UnityQueueHelperStat() { CallerField = key });
    UnityQueueHelperStat getAsyncStat(string caller) => _currentAsyncStats.GetOrAdd(caller, (key) => new UnityQueueHelperStat() { CallerField = key });

    public void Used(string caller)
    {
        var stat = getStat(caller);
        Interlocked.Increment(ref stat.CountField);
    }

    public void UsedAsync(string caller)
    {
        var stat = getAsyncStat(caller);
        Interlocked.Increment(ref stat.CountField);
    }

    struct StatInfo
    {
        public long OverallAsync { get; set; }
        public long Overall { get; set; }
        public List<UnityQueueHelperStat> TopUsagesAsync { get; set; }
        public List<UnityQueueHelperStat> TopUsages { get; set; }
    }

    public class UnityQueueHelperStat
    {
        public string CallerField;
        public long CountField; //outdoing

        public string Caller => CallerField;
        public long Count => CountField;
    }

}
