using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Assets.Src.Arithmetic;
using Assets.Src.Aspects.Impl.Stats;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using ResourceSystem.Utils;
using SharedCode.EntitySystem;
using Src.Aspects.Impl.Stats;
using Src.Aspects.Impl.Stats.Proxy;

namespace Src.Locomotion
{
    public partial class CalcersCache
    {
        /// <summary>
        /// CalcerHolder
        /// </summary>
        private class CalcerHolder : IDisposable, ILocomotionUpdateable
        {
            private readonly ICalcerWrapper _calcer;
            private readonly float _period;
            private readonly AsyncTaskRunner _taskRunner;
            private readonly IEntitiesRepository _repository;
            private readonly OuterRef _entityRef;
            private readonly PropertyChangedDelegate _onStatChangedFn;
            private readonly PropertyChangedDelegate _onTimeStatChangedFn;
            private readonly Action _onArgsChangedFn;
            private readonly StatResource[] _notifiers;
            private readonly bool _periodic;
            private readonly object _jobsLock = new object();
            private readonly object _periodicTillTimeLock = new object();
            private TaskCompletionSource<bool> _prevJob;
            private CancellationTokenSource _prevJobCancellation;
            private CalcerContext.Arg[] _args; 
            private long _periodicTillTime;
            private float _timeLeft;
            private bool _disposed;

            public float Value { get; private set; }

            public float Period => _period;
            
            public bool IsReady { get; private set; }

            public CalcerHolder(ICalcerWrapper calcer, AsyncTaskRunner taskRunner, IEntitiesRepository repository, OuterRef entityRef, float period)
            {
                _calcer = calcer ?? throw new ArgumentException(nameof(calcer));
                _taskRunner = taskRunner ?? throw new ArgumentException(nameof(taskRunner));
                _repository = repository ?? throw new ArgumentException(nameof(repository));
                _entityRef = entityRef.IsValid ? entityRef : throw new ArgumentException(nameof(entityRef));
                _period = period;
                var notifiers = _calcer.CollectNotifiers()?.Distinct().ToArray();
                _notifiers = notifiers?.Where(x => x != null).ToArray();
                _periodic = notifiers?.Any(x => x == null) == true;
                Func<ValueTask> calc = Calc;
                _onStatChangedFn = _ => { EnqueueJob(calc, true); return Task.CompletedTask; };
                _onTimeStatChangedFn = a => { EnqueueJob(calc, true); return OnTimeStatChanged(a); };
                _onArgsChangedFn = () => EnqueueJob(calc, true);
            }

            public void Activate()
            {
                UpdateArgs();
                EnqueueJob(async () =>
                {
                     if (_notifiers != null && _notifiers.Length != 0)
                        await ManageSubscription(_notifiers, true);
                     await Calc();
                     IsReady = true;
                }, false);
            }

            public void Dispose()
            {
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message("Dispose | {@}", new { Calcer = _calcer, Entity = _entityRef.Guid }).Write();
                if (_notifiers != null && _notifiers.Length != 0)
                    EnqueueJob(() => ManageSubscription(_notifiers, false), false);
                _disposed = true;
            }

            public void Update(float dt)
            {
                if (UpdateArgs() || (_periodic || _periodicTillTime != -1) && (_timeLeft -= dt) <= 0)
                {
                    _onArgsChangedFn();
                    _timeLeft = _period;
                    if (_periodicTillTime != -1)
                        lock (_periodicTillTimeLock)
                            if (_periodicTillTime <= SyncTime.Now)
                                _periodicTillTime = -1;
                }
            }

            private bool UpdateArgs()
            {
                (bool changed, var args) = _calcer.CalcArgs(_args);
                _args = args;
                if (changed)
                    if (Logger.IsTraceEnabled)  Logger.IfTrace()?.Message("Arguments changed | {@}",  new {Calcer = _calcer, Entity = _entityRef.Guid}).Write();
                return changed;
            }
            
            private async ValueTask Calc()
            {
                using (var cnt = await _repository.Get(_entityRef.TypeId, _entityRef.Guid))
                {
                    Value = await _calcer.Calc(new CalcerContext(cnt, _entityRef, _repository, args: _args));
                    if (Logger.IsTraceEnabled)  Logger.IfTrace()?.Message("Calc | {@}",  new { Calcer = _calcer, Value = Value, Stats = _notifiers.Select(x => x.____GetDebugRootName()), Entity = _entityRef.Guid }).Write();
                }
            }

            private async ValueTask ManageSubscription(StatResource[] notifiers, bool subscribe)
            {
                _repository.StopToken.ThrowIfCancellationRequested();
                using (var cnt = await _repository.Get(_entityRef.TypeId, _entityRef.Guid))
                {
                    var hasStats = cnt?.Get<IHasStatsEngineClientFull>(_entityRef, ReplicationLevel.ClientFull);
                    if (hasStats == null)
                    {
                        if (subscribe)
                            throw new Exception($"Entity {_entityRef} is not a {nameof(IHasStatsEngineClientFull)}");
                        return;
                    }

                    var stats = hasStats.Stats;
                    foreach (var notifier in notifiers)
                    {
                        if (notifier != null)
                        {
                            if (subscribe)
                                SubscribeToStat(notifier, stats);
                            else
                                UnsubscribeFromStat(notifier, stats);
                        }
                    }
                }
            }

            private void SubscribeToStat(StatResource statDef, IStatsEngineClientFull stats)
            {
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message("Subscribe to stat | {@}", new {Calcer = _calcer, Stat = statDef.____GetDebugRootName(), Entity = _entityRef.Guid}).Write();
                if (stats.TimeStats.TryGetValue(statDef, out var timeStat))
                    timeStat.SubscribePropertyChanged(nameof(ITimeStatClientFull.State), _onTimeStatChangedFn);
                else if (stats.ValueStats.TryGetValue(statDef, out var valueStat))
                    valueStat.SubscribePropertyChanged(nameof(IValueStatClientFull.Value), _onStatChangedFn);
                else if (stats.AccumulatedStats.TryGetValue(statDef, out var accumulatedStat))
                    accumulatedStat.SubscribePropertyChanged(nameof(IAccumulatedStatClientFull.ValueCache), _onStatChangedFn);
                else if (stats.ProxyStats.TryGetValue(statDef, out var proxyStat))
                    proxyStat.SubscribePropertyChanged(nameof(IProxyStatClientFull.ValueCache), _onStatChangedFn);
                else if (stats.ProceduralStats.TryGetValue(statDef, out var proceduralStat))
                    proceduralStat.SubscribePropertyChanged(nameof(IProceduralStatClientFull.ValueCache), _onStatChangedFn);
            }

            private void UnsubscribeFromStat(StatResource statDef, IStatsEngineClientFull stats)
            {
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message("Unsubscribe from stat | {@}", new {Calcer = _calcer, Stat = statDef.____GetDebugRootName(), Entity = _entityRef.Guid}).Write();
                if (stats.TimeStats.TryGetValue(statDef, out var timeStat))
                    timeStat.UnsubscribePropertyChanged(nameof(ITimeStatClientFull.State), _onTimeStatChangedFn);
                else if (stats.ValueStats.TryGetValue(statDef, out var valueStat))
                    valueStat.UnsubscribePropertyChanged(nameof(IValueStatClientFull.Value), _onStatChangedFn);
                else if (stats.AccumulatedStats.TryGetValue(statDef, out var accumulatedStat))
                    accumulatedStat.UnsubscribePropertyChanged(nameof(IAccumulatedStatClientFull.ValueCache), _onStatChangedFn);
                else if (stats.ProxyStats.TryGetValue(statDef, out var proxyStat))
                    proxyStat.UnsubscribePropertyChanged(nameof(IProxyStatClientFull.ValueCache), _onStatChangedFn);
                else if (stats.ProceduralStats.TryGetValue(statDef, out var proceduralStat))
                    proceduralStat.UnsubscribePropertyChanged(nameof(IProceduralStatClientFull.ValueCache), _onStatChangedFn);
            }

            private void EnqueueJob(Func<ValueTask> job, bool cancellable)
            {
                if (_disposed)
                    return;
                
                TaskCompletionSource<bool> prevJob, currentJob = new TaskCompletionSource<bool>();
                CancellationToken cancellationToken = CancellationToken.None;
                
                lock (_jobsLock)
                {
                    (prevJob, _prevJob) = (_prevJob, currentJob);
                    if (_prevJobCancellation != null)
                    {
                        _prevJobCancellation.Cancel();
                        _prevJobCancellation.Dispose();
                        _prevJobCancellation = null;
                    }
                    if (cancellable)
                    {
                        _prevJobCancellation = new CancellationTokenSource();
                        cancellationToken = _prevJobCancellation.Token;
                    }
                }
                
                _taskRunner(async () =>
                {
                    try { if (prevJob != null) await prevJob.Task; } catch (OperationCanceledException)  { }

                    try
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        await job();
                        currentJob.SetResult(true);
                    }
                    catch (OperationCanceledException)
                    {
                        if (Logger.IsTraceEnabled)  Logger.IfTrace()?.Message("Job cancelled | {@}",  new {Calcer = _calcer, Entity = _entityRef.Guid}).Write();
                        currentJob.SetCanceled();
                    }
                    catch (Exception e)
                    {
                        Logger.IfError()?.Exception(e).Write();
                        currentJob.SetCanceled();
                    }
                });
            }
            
            private async Task OnTimeStatChanged(EntityEventArgs args)
            {
                var stat = (ITimeStat)args.Sender;
                using(await stat.EntitiesRepository.Get(stat.ParentTypeId, stat.ParentEntityId))
                {
                    var stopAt = TimeStatHelpers.CalculateChangingStopTime(stat);
                    if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"TimeStat:{stat} StopTime:{stopAt.TimeToString()} Now:{SyncTime.Now}").Write();
                    lock(_periodicTillTimeLock)
                        _periodicTillTime = stopAt;
                }
            }
        }
    }
}