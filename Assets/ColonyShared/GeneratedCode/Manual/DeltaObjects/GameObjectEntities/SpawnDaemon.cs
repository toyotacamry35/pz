using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Aspects.Damage;
using Assets.ColonyShared.SharedCode.Utils;
using Assets.Src.SpawnSystem;
using Assets.Src.Tools;
using GeneratedCode.DeltaObjects.Chain;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedCode.Repositories;
using NLog;
using SharedCode.AI;
using SharedCode.Entities;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.Entities.Mineable;
using SharedCode.Entities.Service;
using SharedCode.EntitySystem;
using SharedCode.MapSystem;
using SharedCode.Serializers;
using SharedCode.Utils;
using SharedCode.Aspects.Item.Templates;
using SharedCode.EntitySystem.EntityPropertyResolvers;
using GeneratedCode.Transactions;
using SharedCode.Repositories;
using System.IO;
using GeneratedCode.MapSystem;
using Assets.Src.ResourcesSystem.Base;
using Core.Environment.Logging.Extension;
using SharedCode.MovementSync;

namespace GeneratedCode.DeltaObjects
{
    public partial class SpawnDaemon : IHookOnInit, IHookOnUnload, IHookOnDestroy
    {
        ConcurrentDictionary<OuterRef<IEntity>, IEntityObjectDef> SpawnedObjects { get; set; } = new ConcurrentDictionary<OuterRef<IEntity>, IEntityObjectDef>();

        private static NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        Dictionary<SpawnPointTypeDef, List<SpawnTemplateDef>> _cachedTemplateDefsBySpawnPointType = new Dictionary<SpawnPointTypeDef, List<SpawnTemplateDef>>();

        private readonly Random _random = new Random();
        IList<SpawnObjectDescription> _goalsMobs;
        IList<SpawnObjectDescription> _goalsNonMobs;
        private string _goalsName;
        bool hasPoints;
        SpawnDaemonGoalsDef Goals;
        SpawnDaemonDef DaemonDef;

        public async Task InitDaemon()
        {
            if (SceneDef == null)
            {
                Logger.IfError()?.Message($"Spawn daemon missing scene data {Def.____GetDebugShortName()}").Write();
                return;
            }
            if (SceneDef.Goals == null)
            {
                Logger.IfError()?.Message($"Spawn daemon missing goals {SceneDef.____GetDebugShortName()}").Write();
                return;
            }
            Goals = SceneDef.Goals;
            DaemonDef = (SpawnDaemonDef)Def;
            if (Goals?.TurnOff ?? true)
                return;
            _goalsMobs = Constants.WorldConstants.SpawnMobs ?
                Goals.SpawnedObjects.Values.Where(x => x.ContentKey.IsEnabled()).Where(x => DefToType.GetEntityType(x.Object.Target.GetType()) == typeof(ILegionaryEntity)).ToArray() :
                Array.Empty<SpawnObjectDescription>();
            _goalsNonMobs = Goals.SpawnedObjects.Values.Where(x => x.ContentKey.IsEnabled()).Where(x => DefToType.GetEntityType(x.Object.Target.GetType()) != typeof(ILegionaryEntity)).ToList();
            if (Name == null)
                Name = Path.GetRandomFileName();
            SpawnDaemonCatalogue.DaemonNameToGuidDic.TryAdd(Name, Id);
            _goalsName = Goals.____GetDebugShortName();
            var selfDef = SceneDef;
            if (!(string.IsNullOrWhiteSpace(selfDef.Filter) && selfDef.Filters.Length == 0))
            {
                var batch = new EntityBatch();
                batch.Add(SceneEntity.StaticTypeId, MapOwner.OwnerSceneId, nameof(SpawnDaemon));
                batch.Add(MapEntity.StaticTypeId, MapOwner.OwnerMapId, nameof(SpawnDaemon));
                using (var sceneW = await EntitiesRepository.Get(batch))
                {
                    var scene = sceneW.Get<ISceneEntityServer>(MapOwner.OwnerSceneId);
                    var map = sceneW.Get<IMapEntityServer>(MapOwner.OwnerMapId);
                    foreach (var sceneChunk in scene.SceneChunks)
                    {
                        foreach (var template in sceneChunk.SpawnTemplates)
                        {
                            var name = template.Target.SceneName;
                            if (name.Contains(selfDef.Filter) || selfDef.Filters.Any(x => name.Contains(x)))
                                if (await map.TryAquireSpawnRightsForPointsSet(new OuterRef<IEntity>(this), sceneChunk))
                                    Maps.Add(template);
                        }
                    }
                }
            }
            foreach (var map in Maps)
                foreach (var template in map.Templates)
                {
                    foreach (var spawnPoint in template.Points)
                    {
                        _cachedTemplateDefsBySpawnPointType.GetOrCreate(spawnPoint.PointType.Target).Add(template);
                        hasPoints = true;
                    }
                }
            if (_goalsMobs.Count > 0)
                _maxSpawnedObjectsPerUpdate = 5;
            await InitPointsMap();
            if (Logger.IsDebugEnabled)
                Logger.Debug($"SpawnDaemon {Name} {string.Join(", ", Goals.SpawnedObjects.Select(x => $"{x.Key} - {x.Value.Amount} {x.Value.SpawnOnPoint.Target.____GetDebugShortName()}"))} " +
                    $"{string.Join(", ", Pools.Select(x => $"{x.Key.____GetDebugShortName()} - {x.Value.Count}"))}");
        }
        public async Task OnInit()
        {
            await InitDaemon();
            DelayUpdate((float)_random.NextDouble() * 10, false);
        }

        private float MobUpdateStep = 5f;

        private int _maxSpawnedObjectsPerUpdate = 150;

        private const float FastUpdateStep = 0.1f;
        private const float SlowUpdateStep = 20f;

        private bool wasActive = false;

        private bool _isUpdating;

        private readonly Random random = new Random();

        private OuterRef<IEntity> TryPlaceObject(IEntityObjectDef objDef, SpawnObjectDescription desc, SpawnPointTypeDef spawnPointTypeDef, Vector3 point, Quaternion rotation)
        {
            if (isDead)
                return default;
            var spawnedType = DefToType.GetEntityType(objDef.GetType());
            var outerRef = new OuterRef<IEntity>(Guid.NewGuid(), ReplicaTypeRegistry.GetIdByType(spawnedType));
            var mapRef = MapOwner.OwnerMap;
            var owner = MapOwner;
            var myId = Id;
            if (point == default)
                return default;
            var le = LinksEngine;
            AsyncUtils.RunAsyncTask(async () =>
            {
                Guid wsId;

                var map = EntitiesRepository.TryGetLockfree<IMapEntityServer>(mapRef, ReplicationLevel.Server);
                wsId = (await map.GetWorldSpaceForPoint(point)).Guid;
                var ws = EntitiesRepository.TryGetLockfree<IWorldSpaceServiceEntityServer>(wsId, ReplicationLevel.Server);
                var result = await ws.SpawnEntity(
                    default,
                    outerRef, point, rotation, owner, myId, objDef, spawnPointTypeDef, null, null);
                if (result)
                {
                    if (desc.RememberAsLink != null)
                        using (var w = await this.GetThisWrite())
                            await le.AddLinkRef(desc.RememberAsLink.Target, outerRef, true, false);
                    EntitiesRepository.SubscribeOnDestroyOrUnload(outerRef.TypeId, outerRef.Guid, OnEntityVanished);
                }
            });
            return outerRef;
        }
        private OuterRef<IEntity> TryPlaceObject(IEntityObjectDef objDef, SpawnObjectDescription desc, SpawnPointTypeDef spawnPointTypeDef)
        {
            var point = SceneDef.MobsSpawnPoints.Length == 0 ? default(Vector3) :
                SceneDef.MobsSpawnPoints[random.Next(SceneDef.MobsSpawnPoints.Length)] + MovementSync.Position;
            if (point == default)
                return default;
            return TryPlaceObject(objDef, desc, spawnPointTypeDef, point, default);
        }
        //я имею право так кешировать лист, потому что одновременно может быть только один апдейт у каждого демона
        List<OuterRef<IEntity>> _objectsRegisterCache = new List<OuterRef<IEntity>>();
        public async Task<bool> UpdateSpawnDaemonImpl(bool outOfOrder)
        {
            if (_isUpdating)
                return true;

            _isUpdating = true;
            _objectsRegisterCache.Clear();
            int spawnedObjects = 0;
            try
            {
                if (_goalsNonMobs.Count > 0)
                {
                    bool spawnedInThisCycle = true;

                    int ultraWatchdog = 0;
                    while (spawnedInThisCycle && spawnedObjects < _maxSpawnedObjectsPerUpdate)
                    {
                        if (ultraWatchdog++ > _maxSpawnedObjectsPerUpdate * 2)
                            throw new InvalidOperationException("Infinite loop in non-mobs routine for spawn daemon " + _goalsName);

                        spawnedInThisCycle = false;
                        _goalsNonMobs.Shuffle(random);
                        foreach (var sDesc in _goalsNonMobs)
                        {
                            int amount;
                            SpawnedObjectsAmounts.TryGetValue(sDesc.Object.Target, out amount);
                            if (amount >= sDesc.Amount)
                                continue;

                            var res = await TryPlaceObjectImpl(new OuterRef<ISpawnDaemon>(this), sDesc, sDesc.SpawnOnPoint.Target, sDesc.Object.Target, true);

                            if (res != default)
                            {
                                spawnedInThisCycle = true;
                                spawnedObjects++;
                                SpawnedObjectsAmounts.TryGetValue(sDesc.Object.Target, out amount);
                                SpawnedObjectsAmounts[sDesc.Object.Target] = amount + 1;

                                SpawnedObjects.TryAdd(new OuterRef<IEntity>(res.Guid, res.TypeId), sDesc.Object.Target);
                                SpawnDaemonsInfo.Add(Id, _goalsName, sDesc.Object.Target);
                                _objectsRegisterCache.Add(new OuterRef<IEntity>(res.Guid, res.TypeId));
                            }
                        }
                    }

                }

                if (_goalsMobs.Count > 0)
                {
                    bool spawnedInThisCycle = true;

                    int ultraWatchdog = 0;
                    while (spawnedInThisCycle && spawnedObjects < _maxSpawnedObjectsPerUpdate)
                    {
                        if (ultraWatchdog++ > _maxSpawnedObjectsPerUpdate * 2)
                            throw new InvalidOperationException("Infinite loop in non-mobs routine for spawn daemon " + _goalsName);

                        spawnedInThisCycle = false;
                        _goalsMobs.Shuffle(random);
                        foreach (var sDesc in _goalsMobs)
                        {
                            int amount;
                            SpawnedObjectsAmounts.TryGetValue(sDesc.Object.Target, out amount);
                            if (amount >= sDesc.Amount)
                                continue;

                            var res = TryPlaceObject(sDesc.Object.Target, sDesc, sDesc.SpawnOnPoint.Target);

                            if (res != default)
                            {
                                spawnedInThisCycle = true;
                                spawnedObjects++;
                                SpawnedObjectsAmounts.TryGetValue(sDesc.Object.Target, out amount);
                                SpawnedObjectsAmounts[sDesc.Object.Target] = amount + 1;

                                SpawnedObjects.TryAdd(new OuterRef<IEntity>(res.Guid, res.TypeId), sDesc.Object.Target);
                                SpawnDaemonsInfo.Add(Id, _goalsName, sDesc.Object.Target);
                                _objectsRegisterCache.Add(new OuterRef<IEntity>(res.Guid, res.TypeId));
                            }
                        }
                    }
                }
            }
            finally
            {
                double delay;

                if (!outOfOrder)
                {
                    if (_goalsMobs.Count > 0)
                    {
                        delay = MobUpdateStep;
                        if (spawnedObjects > 0)
                            DelayUpdate(delay * (random.NextDouble() * 0.5 + 0.75), false);
                    }
                    else
                    {
                        if (spawnedObjects > 0)
                        {
                            if (!wasActive)
                                Logger.IfInfo()?.Message("Spawn Daemon {0} is waking up", this.Id).Write();
                            delay = FastUpdateStep;
                        }
                        else
                        {
                            if (wasActive)
                                Logger.IfInfo()?.Message("Spawn Daemon {0} is falling asleep", this.Id).Write();
                            delay = SlowUpdateStep;
                        }
                        wasActive = spawnedObjects > 0;
                        DelayUpdate(delay * (random.NextDouble() * 0.5 + 0.75), false);
                    }

                }
                /*if(_objectsRegisterCache.Count > 0)
                    using (var cMap = await EntitiesRepository.Get<IMapEntity>(MapGuid))
                    {
                        var map = cMap.Get<IMapEntityServer>(MapGuid);
                        //await map.RegisterMapObjectList(_objectsRegisterCache);
                    }*/
                _isUpdating = false;
            }

            return true;
        }

        public void DelayUpdate(double delay, bool outOfOrder)
        {
            AsyncUtils.RunAsyncTask(async () =>
            {
                if (EntitytObjectsUnitySpawnService.SpawnService != null)
                    await EntitytObjectsUnitySpawnService.SpawnService.RunInUnityThread(() => { });
                else if (!outOfOrder)//heuristics that assumes that main loop spawns a lot of objects
                    await Task.Delay(4000);
                await Task.Delay(TimeSpan.FromSeconds(delay));
                if (!((IEntityExt)this).IsMaster())
                    return;
                if (((IEntitiesRepositoryExtension)EntitiesRepository).GetRepositoryEntityContainsStatus(TypeId, Id) == RepositoryEntityContainsStatus.Master)
                    using (var wrapper = await ((IEntitiesRepositoryExtension)EntitiesRepository).GetExclusive(TypeId, Id))
                    {
                        await ((ISpawnDaemon)this).UpdateSpawnDaemon(outOfOrder);
                    }
            }, EntitiesRepository);

        }

        public async Task<bool> NotifyOfObjectDestructionImpl(Guid id, int typeId)
        {
            /*using (var cMap = await EntitiesRepository.Get<IMapEntity>(MapGuid))
            {
                var map = cMap.Get<IMapEntityServer>(MapGuid);
                //await map.UnregisterMapObject(new OuterRef<IEntity>(id, typeId));
            }*/
            return true;
        }

        public async Task<bool> ResetDaemonImpl()
        {
            // Logger.IfDebug()?.Message("ResetDaemonImpl").Write();;
            using (var numenWrapperOuter = await EntitiesRepository.GetFirstService<INumenServiceEntityServer>())
            {
                if (numenWrapperOuter?.GetFirstService<INumenServiceEntityServer>() == null)
                {
                    Logger.IfError()?.Message($"Can't get {nameof(INumenServiceEntityServer)}.").Write();
                    return false;
                }

                var repo = EntitiesRepository;
                //foreach (var outerRef in SpawnedObjects.Keys.ToArray())
                var array = SpawnedObjects.Keys.ToArray();
                for (int i = 0; i < array.Length; ++i)
                {
                    var outerRef = array[i];
                    AsyncUtils.RunAsyncTask(async () =>
                    {
                        using (var numenWrapperInner = await repo.GetFirstService<INumenServiceEntityServer>())
                        {
                            var numenServiceEntityInner = numenWrapperInner?.GetFirstService<INumenServiceEntityServer>();
                            if (numenServiceEntityInner == null)
                            {
                                Logger.IfError()?.Message($"Can't get {nameof(INumenServiceEntityServer)}.").Write();
                                return;
                            }

                            using (var wrapper = await repo.Get(outerRef))
                            {
                                var healthOwner = wrapper?.Get<IHasHealthServer>(outerRef, ReplicationLevel.Server);
                                if (healthOwner == null)
                                {
                                    Logger.IfWarn()?.Message($"Entity ({outerRef.TypeId}, {outerRef.Guid}) is not {nameof(IHasHealthServer)}. So can't damage it.").Write();
                                    return;
                                }

                                //#Note: Vitaly said: it's some wierd - just comment it.
                                // await healthOwner.Health.ReceiveDamage(
                                //     new Damage(
                                //         battleDamage: GlobalConstsHolder.DeadlyDamage,
                                //         miningDamage: GlobalConstsHolder.DeadlyDamage,
                                //         damageType: GlobalConstsHolder.GlobalConstsDef.DefaultDamageType
                                //         )
                                //     , new OuterRef<IEntity>(numenServiceEntityInner.Id, numenServiceEntityInner.TypeId));

                                //Logger.IfDebug()?.Message($"called `healthOwner.ReceiveDamage` on {outerRef.TypeId}, {outerRef.Guid}.").Write();
                            }
                        }
                    }, repo);

                }// foreach
            }

            // Logger.IfDebug()?.Message("ResetDaemonImpl. DONE").Write();;
            return true;
        }
        Dictionary<SpawnPointTypeDef, List<PointData>> Pools { get; set; } = new Dictionary<SpawnPointTypeDef, List<PointData>>();

        Vector3Int GetCellFromPos(Vector3 vec)
        {
            return new Vector3Int((int)Math.Round(vec.x), (int)Math.Round(vec.y), (int)Math.Round(vec.z));
        }
        float TemplatesLongevity { get; set; }

        System.Random _rand = new Random();
        HashSet<Vector3Int> _simplestHashset = new HashSet<Vector3Int>();
        private Task<bool> ActivateTemplatePointsInternal(SpawnTemplateDef def)
        {
            foreach (var points in def.Points)
            {
                List<PointData> pool = null;
                if (!Pools.TryGetValue(points.PointType, out pool))
                    Pools.Add(points.PointType, pool = new List<PointData>());

                int count = 0;
                foreach (var point in points.Points)
                {
                    if (_simplestHashset.Add(GetCellFromPos(point.SpatialPoint * 5)))
                    {
                        pool.Add(new PointData() { Pos = point.SpatialPoint + (Goals.OffsetSpawnedStatics ? MovementSync.Position : default), Rot = point.Rotation });
                        count++;
                    }
                }
            }
            return Task.FromResult(true);
        }

        public Task<bool> ActivateTemplatePointsImpl(SpawnTemplateDef def)
        {
            return ActivateTemplatePointsInternal(def);
        }

        public async Task<bool> ActivateTemplatePointsBatchImpl(List<SpawnTemplateDef> defs)
        {
            bool result = true;
            foreach (var def in defs)
                result &= await ActivateTemplatePointsInternal(def);
            //Logger.IfInfo()?.Message($"Activate spawn points {string.Concat(Pools.Select(x => $"{x.Key.____GetDebugShortName()}=>{x.Value.Count}"))}").Write();
            return result;
        }
        List<SpawnTemplateDef> _templatesCachedList = new List<SpawnTemplateDef>();
        async Task InitPointsMap()
        {
            _templatesCachedList.Clear();
            foreach (var desc in Goals.SpawnedObjects.Values)
            {
                int amount;
                SpawnedObjectsAmounts.TryGetValue(desc.Object.Target, out amount);
                List<SpawnTemplateDef> neededTemplates;
                if (!_cachedTemplateDefsBySpawnPointType.TryGetValue(desc.SpawnOnPoint, out neededTemplates))
                {
                    if (DefToType.GetEntityType(desc.Object.Target.GetType()) != typeof(ILegionaryEntity))
                        Logger.IfError()?.Message($"Can't add cached templates {DaemonDef.____GetDebugShortName()} {desc.SpawnOnPoint.Target.____GetDebugShortName()}").Write();
                    continue;
                }

                var templates = neededTemplates;
                _templatesCachedList.AddRange(templates);
            }
            await ActivateTemplatePointsBatchImpl(_templatesCachedList);

        }

        public Task<OuterRef<IEntity>> TryPlaceObjectImpl(OuterRef<ISpawnDaemon> daemon, SpawnObjectDescription desc, SpawnPointTypeDef pointType, IEntityObjectDef objDef, bool ignoreGeometry)
        {
            List<PointData> pool = null;
            var spawnedType = DefToType.GetEntityType(objDef.GetType());
            if (!Pools.TryGetValue(pointType, out pool))
                return Task.FromResult<OuterRef<IEntity>>(default);
            if (pool.Count == 0)
                return Task.FromResult<OuterRef<IEntity>>(default);
            var index = _rand.Next(pool.Count);
            var point = pool[index];
            if (pool.Count == 1)
                pool.RemoveAt(index);//index == 0;
            else
            {
                //unordered fast remove
                pool[index] = pool[pool.Count - 1];
                pool.RemoveAt(pool.Count - 1);
            }

            return Task.FromResult(TryPlaceObject(objDef, desc, pointType, point.Pos, point.Rot));
        }

        private const float ReleaseSpatialCellDelaySec = 15f;
        async Task OnEntityVanished(int typeId, Guid guid, IEntity entity)
        {
            var pos = Vector3.Default;
            var rot = Quaternion.Default;
            SpawnPointTypeDef point = null;
            var positioned = PositionedObjectHelper.GetPositioned(entity);
            if (positioned != null)
            {
                pos = positioned.Transform.Position;
                rot = positioned.Transform.Rotation;
            }

            if (entity is IHasSpawnedObject spawnedObj)
            {
                point = spawnedObj.SpawnedObject.PointType;
            }

            DelayNotify(point?.CooldownSec ?? 0f, pos, rot, point, guid, typeId);
        }
        public void DelayNotify(float delay, Vector3 pos, Quaternion rot, SpawnPointTypeDef point, Guid guid, int typeId)
        {
            AsyncUtils.RunAsyncTask(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(delay));
                if (!((IEntityExt)this).IsMaster())
                    return;
                try
                {
                    if (((IEntitiesRepositoryExtension)EntitiesRepository).GetRepositoryEntityContainsStatus(TypeId, Id) == RepositoryEntityContainsStatus.Master)
                        using (var wrapper = await ((IEntitiesRepositoryExtension)EntitiesRepository).GetExclusive(TypeId, Id))
                        {
                            await ((ISpawnDaemon)this).NotifyOfEntityDissipation(pos, rot, point, guid, typeId);
                        }
                }
                catch (Exception e)
                {
                    Logger.IfError()?.Exception(e).Write();
                }
            }, EntitiesRepository);
        }
        public Task<bool> TryPlaceObjectNearImpl(SpawnPointTypeDef pointType, IEntityObjectDef objDef, Vector3 pos, bool ignoreGeometry)
        {
            throw new NotImplementedException();
        }

        public Task NotifyOfEntityDissipationImpl(Vector3 pos, Quaternion rot, SpawnPointTypeDef point, Guid guid, int typeId)
        {
            if (point == null)
                return Task.CompletedTask;
            var entRef = new OuterRef<IEntity>(guid, typeId);
            IEntityObjectDef entObjDef;
            if (SpawnedObjects.TryGetValue(entRef, out entObjDef))
            {
                SpawnedObjectsAmounts[entObjDef] = SpawnedObjectsAmounts[entObjDef] - 1;
                SpawnDaemonsInfo.Remove(Id, entObjDef);
                IEntityObjectDef def;
                SpawnedObjects.TryRemove(entRef, out def);
            }
            if (!_isUpdating)
                DelayUpdate(10, true);
            List<PointData> pool = null;
            if (!Pools.TryGetValue(point, out pool))
                return Task.FromResult(default(OuterRef<IEntityObject>));
            pool.Add(new PointData() { Pos = pos, Rot = rot });
            return Task.CompletedTask;
        }

        private const float AwaitUnityThreadTimeout = 3f;

        public async Task<bool> AwaitUnityThreadImpl()
        {
            var unityTask = EntitytObjectsUnitySpawnService.SpawnService.RunInUnityThread(() => { });
            var delayTask = new SuspendingAwaitable(Task.Delay(TimeSpan.FromSeconds(AwaitUnityThreadTimeout)));
            await SuspendingAwaitable.WhenAny(new [] { unityTask, delayTask });
            if (!unityTask.IsCompleted)
            {
                Logger.IfError()?.Message("Ultra-long unity tick more {0} seconds", AwaitUnityThreadTimeout).Write();
                return false;
            }
            return true;
        }
        bool isDead = false;
        public async Task OnUnload()
        {
            isDead = true;
            IEnumerable<OuterRef<IEntity>> _objectsToUnload;
            OuterRef<IEntity> mapRef;
            MapOwner owner;
            var myId = Id;
            owner = MapOwner;
            mapRef = MapOwner.OwnerMap;
            _objectsToUnload = SpawnedObjects.Select(x => x.Key).ToArray();
            var repo = EntitiesRepository;
            await AsyncUtils.RunAsyncTask(async () =>
            {
                //hack
                Guid wsId = default;
                using (var mapW = await repo.Get(mapRef))
                {
                    var map = mapW.Get<IMapEntityServer>(mapRef, ReplicationLevel.Server);
                    wsId = map.WorldSpaces.First().WorldSpaceGuid;
                }

                foreach (var obj in _objectsToUnload)
                {
                    var ws = repo.TryGetLockfree<IWorldSpaceServiceEntityServer>(wsId, ReplicationLevel.Server);
                    if (ws != null)
                        await ws.DespawnEntity(obj);
                }

            });
        }

        public Task OnDestroy()
        {
            return OnUnload();
        }
    }
}
