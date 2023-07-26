using Assets.Src.Aspects.Impl.Factions.Template;
using ColonyShared.SharedCode.Aspects.Counters.Template;
using ColonyShared.SharedCode.Utils;
using GeneratorAnnotations;
using NLog;
using SharedCode.Entities.Engine;
using SharedCode.EntitySystem;
using SharedCode.Serializers;
using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;

namespace GeneratedCode.DeltaObjects
{
    [GenerateDeltaObjectCode]
    public interface ITimerCounter : IQuestCounter
    {
        long StartTime { get; set; }
        
    }

    public partial class TimerCounter
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private CancellationTokenSource timerCancellationTokenSource = null;
        private TimerCounterDef Def => (TimerCounterDef)CounterDef;

        public Task OnInitImpl(QuestDef questDef, QuestCounterDef counterDef, IEntitiesRepository repository)
        {
            if (Logger.IsDebugEnabled)  Logger.IfDebug()?.Message("========== TimerCounter.OnInitImpl()").Write();;

            StartTime = SyncTime.Now;
            QuestDef = questDef;
            CounterDef = counterDef;
            return Task.CompletedTask;
        }

        public async Task OnDatabaseLoadImpl(IEntitiesRepository repository)
        {
            if (Logger.IsDebugEnabled)  Logger.IfDebug()?.Message("========== TimerCounter.OnDatabaseLoadImpl()").Write();;

            await Subscribe(repository, true);
        }

        public async Task OnDestroyImpl(IEntitiesRepository repository)
        {
            if (Logger.IsDebugEnabled)  Logger.IfDebug()?.Message("========== TimerCounter.OnDestroyImpl()").Write();;

            await Subscribe(repository, false);
        }

        private async Task Complete(bool timerCompleted)
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"========== TimerCounter.Complete({timerCompleted})").Write();

            if (parentEntity == null)
                return;

            if (timerCompleted)
            {
                //we can do it because this subscription is guaranteed to happen on master copy
                using (var wrapper = await ((IEntitiesRepositoryExtension)EntitiesRepository).GetExclusive(parentEntity.TypeId, parentEntity.Id))
                {
                    if (Count < 1)
                    {
                        Count = CountForClient = 1;
                        await OnOnCounterCompletedInvoke(QuestDef, this);
                    }
                }
            }
            return;
        }

        private async Task Subscribe(IEntitiesRepository repository, bool subscribe)
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"========== TimerCounter.Subscribe({subscribe})").Write();

            if (subscribe)
            {
                var finishTime = StartTime + SyncTime.FromSeconds(Def.Time);
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"========== finishTime : {finishTime}, time: {SyncTime.Now}, diff: {SyncTime.ToSeconds(finishTime - SyncTime.Now)}").Write();
                if (finishTime < SyncTime.Now)
                {
                    await Complete(true);
                }
                else
                {
                    if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"========== TimerCounter.RunAsyncTask : {SyncTime.ToSeconds(finishTime - SyncTime.Now)}").Write();
                    timerCancellationTokenSource = new CancellationTokenSource();
                    var token = timerCancellationTokenSource.Token;
                    AsyncUtils.RunAsyncTask(async () =>
                    {
                        if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"========== TimerCounter.ASYNC BEGIN").Write();
                        await Task.Delay(TimeSpan.FromSeconds(SyncTime.ToSeconds(finishTime - SyncTime.Now)), token);
                        if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"========== TimerCounter.ASYNC END {token.IsCancellationRequested}").Write();
                        await Complete(!token.IsCancellationRequested);
                    }, repository);
                    if (Logger.IsDebugEnabled)  Logger.IfDebug()?.Message("========== TimerCounter.RunAsyncTask : AFTER").Write();;
                }
            }
            else
            {
                if (timerCancellationTokenSource != null)
                {
                    timerCancellationTokenSource.Cancel();
                    timerCancellationTokenSource.Dispose();
                    timerCancellationTokenSource = null;
                }
            }
        }
        public Task PreventOnCompleteEventImpl()
        {
            return Task.CompletedTask;
        }
        public override string ToString()
        {
            return $"def: timer: {Def.Time}, started: {StartTime}, current time: {SyncTime.Now}, active: {((timerCancellationTokenSource != null) ? "true" : "false")}";
        }
    }
}
