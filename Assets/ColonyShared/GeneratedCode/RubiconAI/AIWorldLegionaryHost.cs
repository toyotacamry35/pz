using Assets.ColonyShared.GeneratedCode.Shared.Utils;
using Assets.ColonyShared.SharedCode.Utils;
using Assets.Src.Lib.Extensions;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using NLog;
using SharedCode.Entities;
using SharedCode.EntitySystem;
using SharedCode.MovementSync;
using SharedCode.Serializers;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using SharedCode.Aspects.Item.Templates;
using static Assets.Src.RubiconAI.AIWorld;

namespace Assets.Src.RubiconAI
{
    // Is historical heir of `SpatialLegionary`
    public class AIWorldLegionaryHost : IDebugInfoProvider
    {
        private new static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly AIWorld _world;

        public AIWorldLegionaryHost(AIWorld world, IEntitiesRepository repo, OuterRef<IEntity> ent, Guid worldSpaceGuid, Legionary leg)
        {
            this._world = world;
            Legionary = leg;
            if (MobLegionary != null)
                MobLegionary.DemandsUpdate += Legionary_DemandsUpdate;
        }

        public bool Destroying = false;
        public IEntitiesRepository Repo => Legionary.Repository;
        public OuterRef<IEntity> Ent => Legionary.Ref;
        public Guid WorldSpaceGuid => Legionary.WorldSpaceGuid;
        public Legionary Legionary { get; }
        public MobLegionary MobLegionary => Legionary as MobLegionary;
        bool _updating = false;
        public bool ReallyClose;

        public event Action OnFrameStart;
        public event Action OnFrameEnd;
        public ActionType ActionType;
        public async Task UpdateOnce()
        {
            _updating = true;
            if (!Legionary.IsValid)
                return;
            try
            {
                OnFrameStart?.Invoke();
                SharedCode.Utils.Vector3 pos = default;
                SharedCode.Utils.Vector3 fwd = default;
                //Logger.IfError()?.Message($"============== UpdateOnce({Repo}) stack {(new StackTrace()).ToString()}").Write();

                using (var ew = await Repo.Get(Ent.TypeId, Ent.Guid))
                {
                    var entity = PositionedObjectHelper.GetPositioned(ew, Ent.TypeId, Ent.Guid);
                    if (entity == null)
                    {
                        return;
                    }
                    pos = entity.Transform.Position;
                    fwd = entity.Transform.Forward;
                }

                MobLegionary.FromUnitySelfSample = new VisibilityDataSample() { Pos = pos, Ref = Legionary.Ref, Def = Legionary.EntityDef };
                MobLegionary.Forward = fwd;
                //no_need: Legionary.WorldSpaceGuid = WorldSpaceGuid;
                //MobStatisticsManager.Current.MobUpdated(Legionary.Def);
                await MobLegionary.Update(); // !<--

                if (GlobalConstsDef.DebugFlagsGetter.IsDebugMobs(GlobalConstsHolder.GlobalConstsDef) && IsGatherDebugDataEnabled && IsTimeToGatherDebugData)
                {
                    var now = DateTime.UtcNow;
                    _lastBetweanUpdatesSec = (float)((now - _lastUpdatedDateTime).TotalSeconds);
                    _lastBetweanUpdatesSec = (float)((now - _lastUpdatedDateTime).TotalSeconds);
                    _lastUpdatedDateTime = now;
                    ++_updatesCount;

                    if ((now > _lastBetweanUpdatesRecentMaxForgetAt)
                      || (_lastBetweanUpdatesSec > _lastBetweanUpdatesRecentMax))
                    {
                        _lastBetweanUpdatesRecentMax = _lastBetweanUpdatesSec;
                        _lastBetweanUpdatesRecentMaxForgetAt = now + RememberRecentMaxDuring;
                    }

                    StoreGatheredDebugDataFrame(now);
                }
            }
            catch (Exception e)
            {
                if (_world.Mode == AIWorldMode.Mob)
                {
                    Logger.IfError()?.Message($"Exception during mob update {e.ToString()}").Write();
                    Destroying = true;
                    _= AsyncUtils.RunAsyncTask(async () =>
                    {
                        await Repo.Destroy(Ent.TypeId, Ent.Guid);
                    }, Repo);
                }

            }
            finally
            {
                try
                {
                    if (!Destroying)
                    {
                        _= AsyncUtils.RunAsyncTask(async () => {
                            await Task.Delay(TimeSpan.FromSeconds(Constants.WorldConstants.SingleMobsUpdatesNoOftenThan));
                            _world.HostsToUpdate.Enqueue(this);
                        },Repo);
                    }
                    _updating = false;
                    OnFrameEnd?.Invoke();
                }
                catch (Exception e)
                {
                    Logger.IfError()?.Message(e.ToString()).Write();
                }
            }
        }

        private void Legionary_DemandsUpdate(float delay)
        {
            if (delay == 0f)
                _world.HostsToUpdate.Enqueue(this);
            else
                AsyncUtils.RunAsyncTask(async () =>
                    {
                        await Task.Delay((int)(delay * 1000));
                        _world.HostsToUpdate.Enqueue(this);
                    }, Repo);
        }

        // --- IDebugInfoProvider: ---------------------------------
        private void StoreGatheredDebugDataFrame(DateTime utcNow)
        {
            var newData = GetContainerForWrite;
            newData.UpdateData(
                ActionType,
                ReallyClose,
                Legionary != null,
                _updatesCount,
                _lastUpdatedDateTime,
                _lastBetweanUpdatesSec,
                _lastBetweanUpdatesRecentMax,
                RelevancyGrid.GetCellVector(MobLegionary?.FromUnitySelfSample.Pos ?? default),
                RelevancyGrid.Dbg_DANGER_GetAllObjects());

            _debugDataQ.Enqueue(newData);
            _nextTimeGatherDebugData = utcNow + ((_debugDataQ.Count < QueueCountWhenSwitchToRarelyGatheringDbgData)
                                                        ? DtBetweanGatherDebugData_Common
                                                        : DtBetweanGatherDebugData_Rarely);
        }

        // Делаю ConcurrentQ.
        // Писатель пушит,
        // Читатель DeQ прочитывает (причём последний кадр нужно сохранять, чтобы было что читать, если писатель не успел новое написать.
        // Т.е.: Читатель берёт Count (сохраняет его в var), после этого deq всё кадры, кроме последнего (наи> свежего) и выводит его.
        // Д/ борьбы с аллокациями читатель использованные контейнеры перекладывает в аналогичную ConcurrentQ
        // Вероятно на случай, если писатель гораздо бысрее читателя, стоит писать гораздо реже, когде Count превышает некий порог (но не совсем переставать)

        private ConcurrentQueue<AIWorldLegionaryHostDebugData> _debugDataQ = new ConcurrentQueue<AIWorldLegionaryHostDebugData>();
        //! To avoid allocations - pool of used debug-data-containers:
        private ConcurrentBag<AIWorldLegionaryHostDebugData> _debugDataContainersPool = new ConcurrentBag<AIWorldLegionaryHostDebugData>();
        private AIWorldLegionaryHostDebugData GetContainerForWrite
        {
            get
            {
                if (_debugDataContainersPool.TryTake(out AIWorldLegionaryHostDebugData takenFromPool))
                    return takenFromPool;
                return new AIWorldLegionaryHostDebugData();
            }
        }

        public bool IsGatherDebugDataEnabled;
        private bool IsTimeToGatherDebugData => DateTime.UtcNow > _nextTimeGatherDebugData;
        private DateTime _nextTimeGatherDebugData;
        private static readonly TimeSpan DtBetweanGatherDebugData_Common = TimeSpan.FromSeconds(0.1);
        private static readonly TimeSpan DtBetweanGatherDebugData_Rarely = TimeSpan.FromSeconds(1);
        private const int QueueCountWhenSwitchToRarelyGatheringDbgData = 10;

        private DateTime _lastUpdatedDateTime;
        private float _lastBetweanUpdatesSec;
        private float _lastBetweanUpdatesRecentMax;
        private DateTime _lastBetweanUpdatesRecentMaxForgetAt;
        private readonly TimeSpan RememberRecentMaxDuring = TimeSpan.FromSeconds(60);
        private long _updatesCount;

        private readonly string _noDataMsg = $"No DebugData at `{nameof(_debugDataQ)}`";
        private DateTime _spamPreventTime;
        private readonly TimeSpan _spamPreventTimeStep = TimeSpan.FromSeconds(10);
        //#todo: pass data to UI
        public string GetDebugInfo()
        {
            //1. DeQ all but last (most fresh):
            var n = _debugDataQ.Count;
            if (n == 0)
            {
                if (DateTime.UtcNow > _spamPreventTime)
                {
                    _spamPreventTime = DateTime.UtcNow + _spamPreventTimeStep;
                    Logger.IfWarn()?.Message($"#Dbg: `{nameof(_debugDataQ)}` is empty. Make sure `{nameof(GlobalConstsHolder.GlobalConstsDef.DebugMobs)}` is on (use cheat \"SetDebugMobs true false\" to on it)").Write();
                }

                return null;
            }

            for (int i = 0; i < n - 1; ++i)
            {
                if (!_debugDataQ.TryDequeue(out var returnToPool))
                    Logger.IfError()?.Message("Unexpected _debugDataQ.TryDequeue(..) failed").Write();
                _debugDataContainersPool.Add(returnToPool);
            }

            //2. Return the most fresh item:
            if (!_debugDataQ.TryPeek(out var result))
                Logger.IfError()?.Message("Unexpected _debugDataQ.TryPeek(..) failed").Write();

            return result?.ToString() ?? _noDataMsg;

            //#Old:
            return // $"SpatialLegionary::1 _farAway:  {_farAway}\n" +
                   $"SpatialLegionary::0 ActionType:  {ActionType}\n" +
                   $"SpatialLegionary::2 _reallyClose:  {ReallyClose}\n" ///[V]
                 + $"SpatialLegionary::4 Legionary!=null ?:  {(Legionary != null)}\n"
                 + $"SpatialLegionary::5 _updatesCount:  {_updatesCount}\n"
                 + $"SpatialLegionary::6 _lastUpdatedDateTime:  {SharedHelpers.TimeStamp(_lastUpdatedDateTime)}\n"
                 + $"SpatialLegionary::7 _lastBetweanUpdates:  {_lastBetweanUpdatesSec:##.00}\n"
                 //+ $"SpatialLegionary::7 _lastEnqueuedDateTime:  {SharedHelpers.TimeStamp(_lastEnqueuedDateTime)}\n"
                 //+ $"SpatialLegionary::9 is: {(_dbg_isWokenUp ? "WokenUp" : "FalledAsleep")}\n"
                 + $"SpatialLegionary::8.1 Cell my:  {RelevancyGrid.GetCellVector(MobLegionary?.FromUnitySelfSample.Pos ?? default)}\n"
                 + $"SpatialLegionary::8.2 Cells char:  {RelevancyGrid.Dbg_DANGER_GetAllObjects()?.ToStringCustom(":\n:") ?? "null"}";
        }
    }
}
