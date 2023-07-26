using Assets.ColonyShared.SharedCode.Player;
using Assets.Src.NetworkedMovement;
using Assets.Src.RubiconAI.BehaviourTree;
using Assets.Src.RubiconAI.KnowledgeSystem;
using Assets.Src.SpatialSystem;
using Assets.Src.Tools;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedCode.Repositories;
using NLog;
using NLog.Fluent;
using SharedCode.AI;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;
using SharedCode.Extensions;
using SharedCode.MovementSync;
using SharedCode.Repositories;
using SharedCode.Serializers;
using SharedCode.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using SharedCode.Entities.Engine;
using GeneratedCode.DeltaObjects;

namespace Assets.Src.RubiconAI
{
    public class AIWorld
    {
        private static readonly NLog.Logger _overallStatisticsLog = LogManager.GetLogger("Telemetry-AI");
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly int LegionaryEntityTypeId = ReplicaTypeRegistry.GetIdByType(typeof(ILegionaryEntity));

        private static ConcurrentDictionary<Guid, AIWorld> _worlds = new ConcurrentDictionary<Guid, AIWorld>();
        public static AIWorld GetWorld(Guid guid)
        {
            _worlds.TryGetValue(guid, out var aiworld);
            return aiworld;
        }

        public static readonly SpatialHash<OuterRef<IEntity>> RelevancyGrid = new SpatialHash<OuterRef<IEntity>>(15, 40);

        private IEntitiesRepository _repo;
        private CancellationTokenSource _source;
        CancellationToken _token;
        private MockLocomotionWorld _mockLocomotionWorld;
        private DateTime _lastStatisticsLogTime;
        private TimeSpan _logPeriod = TimeSpan.FromSeconds(1);
        RelevancyQueuesArray.ProcessingStatus _processingStatus = default;
        int AllMobs => _mobs.Count;
        int AllDummies => _hosts.Count;
        void LogStatistics()
        {
            var delta = (DateTime.Now - _lastStatisticsLogTime);
            if (delta < _logPeriod)
                return;
            _lastStatisticsLogTime = DateTime.Now;
            _overallStatisticsLog.Log(LogLevel.Info).Message("AIUpdate {repo_id}: " +
                 "delta {delta_ms}, " +
                 "mobs count {mobs_count}, " +
                 "all objects tracked by AI: {ai_tracked_objects}, ",
                 _repo.Id,
                 delta.TotalMilliseconds,
                 AllMobs,
                 AllDummies
                 ).Property("update_status", _processingStatus).Write();
            _processingStatus = default;

        }
        public AIWorld(IEntitiesRepository repo, bool forceLoadExistingEntities, AIWorldMode mode)
        {
            if (Constants.WorldConstants.UseMockLocomotion == MockLocomotionUsage.ClusterLocalToMobs)
            {
                _mockLocomotionWorld = new MockLocomotionWorld(repo);
            }
            Mode = mode;
            //Logger.IfError()?.Message($"============== AIWorld({repo})").Write();

            _source = new CancellationTokenSource();
            _token = _source.Token;
            _repo = repo;
            if (mode == AIWorldMode.Bot)
            {
                repo.EntityUnloaded += async (t, g, b, e) => { EntityDestroyOrUnloaded(t, g); };
                repo.NewEntityUploaded += async (t, g) => { EntityCreatedOrLoaded(t, g); };
            }
        }

        public void Attach(OuterRef<IEntity> ent)
        {
            EntityCreatedOrLoaded(ent.TypeId, ent.Guid);
        }

        public void Detach(OuterRef<IEntity> ent)
        {
            EntityDestroyOrUnloaded(ent.TypeId, ent.Guid);
        }
        public ConcurrentDictionary<OuterRef<IEntity>, LegionaryEntityDef> ActiveBots = new ConcurrentDictionary<OuterRef<IEntity>, LegionaryEntityDef>();
        public void SetBots(Dictionary<OuterRef<IEntity>, LegionaryEntityDef> botsRefs)
        {
            foreach (var botRef in botsRefs)
                ActiveBots.TryAdd(botRef.Key, botRef.Value);
        }

        public AIWorldMode Mode { get; set; } = AIWorldMode.Mob;

        private void EntityCreatedOrLoaded(int typeId, Guid entityId)
        {
            switch (Mode)
            {
                case AIWorldMode.Mob:
                    {
                        if (typeId == LegionaryEntityTypeId)
                        {
                            _entitiesToAdd.Enqueue(new OuterRef<IEntity>(entityId, typeId));
                        }
                        else if (typeof(IIsDummyLegionary).IsAssignableFrom(ReplicaTypeRegistry.GetTypeById(typeId)))
                        {
                            _dummiesToAdd.Enqueue(new OuterRef<IEntity>(entityId, typeId));
                        }
                        break;
                    }
                case AIWorldMode.Bot:
                    {
                        var @ref = new OuterRef<IEntity>(entityId, typeId);
                        if (ActiveBots.TryGetValue(@ref, out _))
                        {
                            _entitiesToAdd.Enqueue(@ref);
                        }
                        else if (typeId == LegionaryEntityTypeId || typeof(IIsDummyLegionary).IsAssignableFrom(ReplicaTypeRegistry.GetTypeById(typeId)))
                        {
                            _dummiesToAdd.Enqueue(@ref);
                        }
                        break;
                    }
                case AIWorldMode.Off:
                    break;
                default:
                    break;
            }
        }

        private void EntityDestroyOrUnloaded(int typeId, Guid entityId)
        {
            var @ref = new OuterRef<IEntity>(entityId, typeId);
            var type = @ref.Type;
            if (type == typeof(ILegionaryEntity) || typeof(IIsDummyLegionary).IsAssignableFrom(type) || ActiveBots.TryRemove(@ref, out var _))
            {
                _entitiesToRemove.Enqueue(@ref);
            }
        }
        Guid _myId;
        public void Register(Guid wsGuid)
        {
            _myId = wsGuid;
            _worlds.TryAdd(wsGuid, this);
            _mockLocomotionWorld?.Register(wsGuid);
        }

        public void Unregister()
        {
            _worlds.TryRemove(_repo.Id, out var removed);
            removed.Stop();
            _mockLocomotionWorld?.Unregister();
        }

        private void Stop()
        {
            _source.Cancel();
        }

        Dictionary<Guid, Dictionary<Guid, Dictionary<KnowledgeCategoryDef, List<(Guid, ScenicEntityDef)>>>> _pois = new Dictionary<Guid, Dictionary<Guid, Dictionary<KnowledgeCategoryDef, List<(Guid, ScenicEntityDef)>>>>();
        public void InitForScene(ISceneEntityServer scene)
        {
            // Fill `_pois` dic.:
            var pois = new Dictionary<Guid, Dictionary<KnowledgeCategoryDef, List<(Guid, ScenicEntityDef)>>>();
            var scenes = scene.SceneChunks;
            var objects = scenes.SelectMany(x => x?.Entities).Where(x => x != null).Select(x => x.Target).Where(x => x.Object != null);
            var spawnDaemons = objects.Where(x => x.SpawnDaemonSceneDef != null).Select(x => x.SpawnDaemonSceneDef);
            var allObjects = new Dictionary<OuterRef<IEntity>, (Guid, ScenicEntityDef)>();
            foreach (var obj in objects)
            {
                var staticId = new OuterRef<IEntity>(obj.RefId, ReplicaTypeRegistry.GetIdByType(DefToType.GetEntityType(obj.Object.Target.GetType())));
                if (!scene.StaticToDynamicData.TryGetValue(obj.RefId, out var resId))
                    continue;
                if (allObjects.ContainsKey(staticId))
                {
                    Logger.IfError()?.Message($"Duplicate id {staticId} {obj.____GetDebugRootName()}").Write();
                    continue;
                }
                allObjects.Add(staticId, (resId.Guid, obj));
            }
            foreach (var spawnDaemon in spawnDaemons.Select(x => x.Target))
            {
                if (spawnDaemon.POIs.Count == 0)
                    continue;
                scene.StaticToDynamicData.TryGetValue(spawnDaemon.SpawnDaemonId, out var spawnerId);
                if (pois.ContainsKey(spawnerId.Guid))
                    Logger.IfError()?.Message($"DUPLICATE SPAWN DAEMON IDS {spawnerId} {spawnDaemon.Name} {spawnDaemon.Goals.Target.____GetDebugShortName()}").Write();
                pois.Add(spawnerId.Guid, spawnDaemon.POIs.ToDictionary(x => x.Key.Target,
                    x => x.Value.Where(y => allObjects.ContainsKey(new OuterRef<IEntity>(y))).Select(y => allObjects[new OuterRef<IEntity>(y)]).ToList()));
            }
            _pois.Add(scene.Id, pois);
        }

        private ConcurrentDictionary<OuterRef<IEntity>, AIWorldLegionaryHost> _hosts = new ConcurrentDictionary<OuterRef<IEntity>, AIWorldLegionaryHost>();
        private ConcurrentDictionary<OuterRef<IEntity>, AIWorldLegionaryHost> _mobs = new ConcurrentDictionary<OuterRef<IEntity>, AIWorldLegionaryHost>();

        public ConcurrentQueue<AIWorldLegionaryHost> HostsToUpdate = new ConcurrentQueue<AIWorldLegionaryHost>();
        private ConcurrentQueue<OuterRef<IEntity>> _entitiesToRemove = new ConcurrentQueue<OuterRef<IEntity>>();
        private ConcurrentQueue<OuterRef<IEntity>> _entitiesToAdd = new ConcurrentQueue<OuterRef<IEntity>>();
        private ConcurrentQueue<OuterRef<IEntity>> _dummiesToAdd = new ConcurrentQueue<OuterRef<IEntity>>();
        private Dictionary<(Guid, LegionDef), Legion> _legions = new Dictionary<(Guid, LegionDef), Legion>();

        public AIWorldLegionaryHost GetHost(OuterRef<IEntity> obj)
        {
            _hosts.TryGetValue(obj, out var host);
            return host;
        }

        public void Run()
        {
            //Logger.IfError()?.Message($"1. ============== RunAsyncTask({_repo})").Write();
            AsyncUtils.RunAsyncTask(async () =>
            {
                while (!_source.IsCancellationRequested)
                {
                    foreach (var mob in _mobs)
                    {
                        if (mob.Value.Destroying)
                            continue;
                        Guid? pathGuid;
                        if (Mode == AIWorldMode.Mob)
                        {
                            pathGuid = _repo.TryGetLockfree<ILegionaryEntity>(mob.Key, ReplicationLevel.Master)?.MovementSync.PathFindingOwnerRepositoryIdRuntime;
                            if (!pathGuid.HasValue || pathGuid.Value == Guid.Empty)
                            {
                                if (mob.Value.ActionType != ActionType.ThinkUnseen)
                                {
                                    HostsToUpdate.Enqueue(mob.Value);
                                    mob.Value.ActionType = ActionType.ThinkUnseen;
                                }
                                continue;
                            }
                        }
                        mob.Value.ReallyClose = VisibilityGrid.Get(mob.Value.WorldSpaceGuid, _repo).SampleDataForAnyAround<WorldCharacterDef>(mob.Value.Ent, 30, true);
                        var newType = mob.Value.ReallyClose
                            ? ((mob.Value.MobLegionary.IsExecutingHighFreqUpdatedBehNodes)
                                ? ActionType.ThinkCloseHighFreq
                                : ActionType.ThinkClose)
                            : ((mob.Value.MobLegionary.IsExecutingHighFreqUpdatedBehNodes)
                                ? ActionType.ThinkFarHighFreq
                                : ActionType.ThinkFar);
                        if (mob.Value.ActionType != newType)
                        {
                            mob.Value.ActionType = newType;
                            HostsToUpdate.Enqueue(mob.Value);
                        }
                    }

                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            }, _repo);

            //Logger.IfError()?.Message($"2. ============== RunAsyncTask({_repo})").Write();
            AsyncUtils.RunAsyncTask(async () =>
            {
                try
                {
                    while (!_source.IsCancellationRequested)
                    {
                        while (_entitiesToAdd.TryDequeue(out var oRef))
                        {
                            using (var ew = await _repo.Get(oRef.TypeId, oRef.Guid))
                            {
                                var entity = ew.Get<IEntityObjectClientBroadcast>(oRef.TypeId, oRef.Guid, ReplicationLevel.ClientBroadcast);
                                if (entity == null) // could be removed from repository at this moment
                                    continue;
                                ew.TryGet<IHasSpawnedObjectServer>(oRef.TypeId, oRef.Guid, ReplicationLevel.Server, out IHasSpawnedObjectServer spawned);
                                ew.TryGet<IHasWizardEntityServer>(oRef.TypeId, oRef.Guid, ReplicationLevel.Server, out var hwe);
                                LegionaryEntityDef def;
                                if (entity.Def is LegionaryEntityDef ldef)
                                    def = ldef;
                                else
                                    ActiveBots.TryGetValue(oRef, out def);
                                ew.TryGet<IHasInputActionHandlers>(oRef.TypeId, oRef.Guid, ReplicationLevel.Master, out var handlers);
                                if (def.InputActionHandlers != null && handlers != null)
                                    if (handlers.InputActionHandlers.BindingsSource == null)
                                    {
                                        _ = AsyncUtils.RunAsyncTask(async () =>
                                        {
                                            await Task.Delay(TimeSpan.FromSeconds(0.5f));
                                            _entitiesToAdd.Enqueue(oRef);
                                        });
                                        continue;
                                    }

                                
                                try
                                {
                                    LegionaryDef legionaryDef = def.LegionaryDef.Target.LegionaryDef.Target;
                                    MobBrain brain = new MobBrain(new Strategy(legionaryDef.MainStrategy));


                                    MobLegionary legionary = new MobLegionary(brain, legionaryDef, _repo);
                                    await legionary.Init();
                                    legionary.SceneId = entity.MapOwner.OwnerSceneId;
                                    legionary.Ref = oRef;
                                    legionary.WorldSpaceGuid = entity.WorldSpaced.OwnWorldSpace.Guid;
                                    legionary.EntityDef = entity.Def;
                                    legionary.SDef = def;
                                    legionary.SpellDoer = hwe?.SlaveWizardHolder.SpellDoer;
                                    AIWorldLegionaryHost host = new AIWorldLegionaryHost(this, _repo, oRef, entity.WorldSpaced.OwnWorldSpace.Guid, legionary);

                                    await AssignLegionaryToLegion(legionary, spawned?.SpawnedObject?.Spawner ?? default, legionaryDef.DefaultLegion.Target);
                                    legionary.InitAllKnowledge();
                                    if (def.InputActionHandlers != null && handlers != null && entity.Def is LegionaryEntityDef d)
                                        await legionary.InitInputActions(handlers, d.BodyType);
                                    await brain.Init(legionary);
                                    _mobs.TryAdd(oRef, host); 
                                    _hosts.TryAdd(oRef, host); 
                                    GetQueues(_myId).AddToQueue(legionary.Index, host.UpdateOnce, ActionType.ThinkCloseHighFreq);
                                   
                                }
                                catch(Exception e)
                                {
                                    Logger.Error($"Exception during final mobLegionary creation {entity.Def.____GetDebugShortName()} {e.ToString()}");
                                }
                            }
                        }

                        while (_dummiesToAdd.TryDequeue(out var oRef))
                        {
                            using (var ew = await _repo.Get(oRef.TypeId, oRef.Guid))
                            {
                                var entity = ew.Get<IEntityObjectClientBroadcast>(oRef.TypeId, oRef.Guid, ReplicationLevel.ClientBroadcast);
                                if (entity == null) // could be removed from repository at this moment
                                    continue;

                                if (entity.WorldSpaced.OwnWorldSpace.Guid == Guid.Empty)
                                {
                                    _dummiesToAdd.Enqueue(oRef);
                                    continue;
                                }
                                ew.TryGet(oRef.TypeId, oRef.Guid, ReplicationLevel.Server, out IHasSpawnedObjectServer spawned);

                                var def = entity.Def;
                                var legionary = new DummyLegionary(_repo);
                                legionary.Ref = oRef;

                                legionary.WorldSpaceGuid = entity.WorldSpaced.OwnWorldSpace.Guid;
                                legionary.EntityDef = def;
                                var host = new AIWorldLegionaryHost(this, _repo, oRef, entity.WorldSpaced.OwnWorldSpace.Guid, legionary);
                                AssignLegionaryToLegion(legionary, spawned?.SpawnedObject?.Spawner ?? default, (def as IIsDummyLegionaryDef)?.LegionType.Target);
                                _hosts.TryAdd(oRef, host);
                            }
                        }

                        while (_entitiesToRemove.TryDequeue(out var eToR))
                        {
                            _hosts.TryRemove(eToR, out var removedLegionary);
                            if (removedLegionary != null)
                            {
                                if (removedLegionary.Legionary is MobLegionary ml)
                                    await ml.TerminateEverything();

                                removedLegionary.Legionary.IsValid = false;
                                Legionary.LegionariesByRef.TryRemove(removedLegionary.Legionary.Ref, out var _);
                            }
                            _mobs.TryRemove(eToR, out var _);
                        }

                        while (HostsToUpdate.TryDequeue(out var htu))
                        {
                            if (!htu.Destroying)
                                GetQueues(_myId).AddToQueue(htu.Legionary.Index, htu.UpdateOnce, htu.ActionType);
                        }

                        // Refresh `_actionsPerFrame` dic:
                        foreach (var actionType in TotalActionsPerFrame.Keys)
                            _actionsPerFrame[actionType] = (QueuesArrs.Count > 0)
                                ? Math.Max(TotalActionsPerFrame[actionType] / QueuesArrs.Count, 1)
                                : TotalActionsPerFrame[actionType];

                        // Process queues:

                        var kvp = GetQueues(_myId);
                        //Logger.IfError()?.Message($"Pre tick {string.Join(",", _mobs.Select(x => x.Value.Legionary.Index).OrderBy(x=>x))}").Write();
                        var status = await kvp.AsyncProcessQueues(_actionsPerFrame);
                        if (this.Mode == AIWorldMode.Mob)
                        {
                            _processingStatus.Add(status);
                            LogStatistics();
                        }
                        await Task.Delay(TimeSpan.FromSeconds(Constants.WorldConstants.MobUpdateTick));
                    }
                }
                catch (Exception e)
                {
                    Logger.IfError()?.Message($"{e.ToString()}").Write();
                    _source.Cancel();
                    _source.Dispose();
                    if (!_source.IsCancellationRequested)
                    {
                        _source = new CancellationTokenSource();
                        _token = _source.Token;
                        Run();
                    }
                    throw;
                }
            }, _repo);
        }

        async ValueTask AssignLegionaryToLegion(Legionary legionary, OuterRef<ISpawnDaemon> spawnerId, LegionDef legionType)
        {
            _legions.TryGetValue((spawnerId.Guid, legionType), out var legion);
            if (legion == null)
            {
                _legions.Add((spawnerId.Guid, legionType), legion = new Legion(null, legionType, _repo));
                await legion.Legionary.Init();
                if (_pois.TryGetValue(legionary.SceneId, out var scenePois))
                    if (scenePois.TryGetValue(spawnerId.Guid, out var ktpois))
                    {
                        foreach (var ktopoi in ktpois)
                        {
                            var listOfPois = ktopoi.Value.Select(x => x).ToList();
                            legion.Legionary.Knowledge.Register(new StaticsKnowledgeSource(ktopoi.Key, listOfPois));
                        }
                    }
            }
            legionary.AssignToLegion(legion);
        }
        public RelevancyQueuesArray GetQueues(Guid shard) => (shard != Guid.Empty) ? QueuesArrs.GetOrCreate(shard) : null;
        // ConcurrentDictionary, 'cos AI 'll be multi-thread (note: but every single "brain" single-thread)
        private readonly ConcurrentDictionary<Guid, RelevancyQueuesArray> QueuesArrs = new ConcurrentDictionary<Guid, RelevancyQueuesArray>();

        static readonly Dictionary<ActionType, int> TotalActionsPerFrame = new Dictionary<ActionType, int>()
        {
            {ActionType.ThinkUnseen,            1},
            {ActionType.ThinkFar,            4},//old 1
            {ActionType.ThinkFarHighFreq,   8},//old 2
            {ActionType.ThinkClose,         10},//old 3
            {ActionType.ThinkCloseHighFreq, 30},
        };
        readonly Dictionary<ActionType, int> _actionsPerFrame = new Dictionary<ActionType, int>();

        [VerifyAfterCompilation]
        static bool VerifyAfterCompilation()
        {
            for (int i = 0; i <= (int)ActionType.Max; ++i)
                if (!TotalActionsPerFrame.ContainsKey((ActionType)i))
                {
                    Logger.IfError()?.Message($"{nameof(TotalActionsPerFrame)} should contain entry for every {nameof(ActionType)} enum value, but it doesn't for {(ActionType)i}!").Write();
                    return false;
                }

            return true;
        }

        public enum ActionType
        {
            ThinkUnseen = 0,
            ThinkFar = 1,
            ThinkFarHighFreq = 2,
            ThinkClose = 3,
            ThinkCloseHighFreq = 4,

            Max = ThinkCloseHighFreq,
        }
    }

    public enum AIWorldMode
    {
        Off,
        Mob,
        Bot
    }
}