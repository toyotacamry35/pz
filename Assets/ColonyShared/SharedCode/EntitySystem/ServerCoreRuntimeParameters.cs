using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using HWT;
using Infrastructure.Config;
using NLog;
using SharedCode.Utils.Threads;

namespace SharedCode.EntitySystem
{
    public static class ServerCoreRuntimeParameters
    {
        public static ContainerConfig Config { get; set; } = new ContainerConfig();

        public const float EntityEventTimeoutSeconds = 5f;

        public const float EntitiesContainerLockTimeoutSeconds = 10f;

        public const float RepositoryEntityUnlockTimeoutSeconds = 20f;

        public const float WaitFinishExecutingRpcMethodsTimeoutSeconds = 5f;

        public const float RpcTimeoutSeconds = 10.0f;

        public const float ReleaseInvalidContainersDelaySeconds = 2f;

        public const int ReleaseInvalidContainersIsContextCountWarn = 10;

        public const int EntityEventsProcessingListsInQueueWarnCount = 10;

        public const float EntitiesContainerLockOverallTimeoutSeconds = 10f;

        public const int EventSubscribersWarnCount = 3;

        public const int RepositoryEventSubscribersWarnCount = 30;

        public const int MaxProcessEventsCount = 50;

        public const int RepositoryWaitQueueSizeWarningCount = 500;

        public const float ToLongWaitEntityErrorLoggingSeconds = 2.0f;

        public static bool CanDisableRepositoryGetEntitiesTimeout => Config.CanDisableRepositoryGetEntitiesTimeout;

        public const int ReplicateWorldObjectBatchCount = 100;

        public const float EntityOperationLogMinDelaySeconds = 3f;
        
        public const float CloudNodeReconnectDelaySecondsAfterDisconnecting = 10f;

        public static TimeSpan WaitUpdateTime = TimeSpan.FromSeconds(10);

        public static TimeSpan HangDetectTimeout = TimeSpan.FromSeconds(5);

        #region debug mode
        public static bool CollectStackTraces => Config.CollectStackTraces;

        public static bool EnableEntityUsagesAndEventsTimeouts => Config.EnableEntityUsagesAndEventsTimeouts;

        public static bool CollectOperationsLog => Config.CollectOperationsLog;

        public static bool CollectChainCallHistory => Config.CollectChainCallHistory;

        public static bool CollectReplicationHistory => Config.CollectReplicationHistory;

        public static bool CollectSubscriptionsHistory => Config.CollectSubscriptionsHistory;

        public static bool CollectEntityLifecycleHistory => Config.CollectEntityLifecycleHistory;

        public static bool WriteToLogFreeze => true;
        #endregion

        public static void SetDebugMode(bool enabled)
        {
            Config.CollectStackTraces = enabled;
            Config.EnableEntityUsagesAndEventsTimeouts = enabled;
            Config.CollectOperationsLog = enabled;
        }
    }

    public interface ITimeoutPayload
    {
        bool Run();
    }

    public interface ICancellable
    {
        void Cancel();
    }

    public static class TimeoutSystem
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private static ITimeoutSystem _instance = HWTTimeouts.It;
        public static ITimeoutSystem Instance 
        { 
            get => _instance;
            set
            {
                _instance = value;
                Logger.IfInfo()?.Message("Timeout system set to {0}", value?.GetType().FullName ?? "<null>").Write();
            }
        }
        public static ICancellable Install<T>(in T payload, TimeSpan length) where T : struct, ITimeoutPayload => Instance.Install(in payload, length);
    }

    public interface ITimeoutSystem
    {
        ICancellable Install<T>(in T payload, TimeSpan length) where T : struct, ITimeoutPayload;
    }

    public sealed class HWTTimeouts : ITimeoutSystem
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly HWT.Timer Timer = new HashedWheelTimer(TimeSpan.FromSeconds(1), 100, 0);

        public static HWTTimeouts It { get; } = new HWTTimeouts();

        private HWTTimeouts()
        {
            var field = Timer.GetType().GetField("_workerThread", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var thread = (Thread)field.GetValue(Timer);
            thread.IsBackground = true;
        }

        public ICancellable Install<T>(in T payload, TimeSpan length) where T : struct, ITimeoutPayload
        {
            var to = new Impl<T>(in payload, length);
            to._timeout = Timer.NewTimeout(to, length);
            return to;
        }

        private sealed class Impl<T> : TimerTask, ICancellable where T : struct, ITimeoutPayload
        {
            private readonly T _payload;
            private readonly TimeSpan _length;

            private const int Status_Active = 0;
            private const int Status_Cancelled = 1;

            private volatile int Status = Status_Active;

            internal HWT.Timeout _timeout;

            public Impl(in T payload, TimeSpan length)
            {
                _payload = payload;
                _length = length;
            }

            public void Cancel()
            {
                Status = Status_Cancelled;
                _timeout?.Cancel();
            }

            public void Run(HWT.Timeout timeout)
            {
                try
                {
                    if (Status == Status_Cancelled)
                        return;

                    if (!_payload.Run())
                    {
                        _timeout = timeout.Timer.NewTimeout(this, _length);
                    }
                }
                catch (Exception e)
                {
                    Logger.IfError()?.Exception(e).Write();
                }
            }
        }
    }

    public sealed class TPLTimeouts : ITimeoutSystem
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public static TPLTimeouts It { get; } = new TPLTimeouts();

        private TPLTimeouts()
        {
        }

        private readonly struct Cancellation : ICancellable
        {
            private readonly CancellationTokenSource _cts;

            public Cancellation(CancellationTokenSource cts)
            {
                _cts = cts;
            }

            public void Cancel()
            {
                try
                {
                    _cts.Cancel();
                    _cts.Dispose();
                }
                catch(Exception e)
                {
                    Logger.IfError()?.Exception(e).Write();
                }
            }
        }
        public ICancellable Install<T>(in T payload, TimeSpan length) where T : struct, ITimeoutPayload
        {
            var cts = new CancellationTokenSource();
            var token = cts.Token;

            T payloadLocal = payload;
            TaskEx.Run(() => Impl(payloadLocal, length, token));

            return new Cancellation(cts);
        }

        private static readonly TimeSpan NoCtDelay = TimeSpan.FromSeconds(1);

        private static async void Impl<T>(T payload, TimeSpan delta, CancellationToken ct) where T : struct, ITimeoutPayload
        {
            try
            {
                do
                {
                    try
                    {
                        var firstDelay = NoCtDelay < delta ? NoCtDelay : delta;
                        await Task.Delay(firstDelay);
                        if (ct.IsCancellationRequested)
                            return;
                        
                        var remaining = delta - firstDelay;
                        if(remaining > TimeSpan.Zero)
                            await Task.Delay(remaining, ct);
                    }
                    catch(OperationCanceledException)
                    {
                        return;
                    }
                }
                while (!payload.Run());
            }
            catch(Exception e)
            {
                Logger.IfError()?.Exception(e).Write();
            }
        }
    }

}
