using Assets.Src.GameObjectAssembler;
using NLog;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using System.Collections.Concurrent;
using System.Diagnostics;
using GeneratedCode.Custom.Config;
using Assets.Src.Server.Impl;
using Assets.Src.Aspects;
using System.Linq;
using Assets.ColonyShared.SharedCode.Player;
using Assets.Src.WorldSpace;
using Assets.Src.Scenes;
using SharedCode.Cloud;
using GeneratedCode.Repositories;
using Assets.Src.RubiconAI;
using SharedCode.Aspects.Item.Templates;
using Assets.Src.Utils;
using UnityEngine.Scripting;
using SharedCode.Entities;
using Assets.ResourceSystem.Aspects.ManualDefsForSpells;
using Assets.Src.App;
using SharedCode.Utils;
using Assets.Src.ResourcesSystem.Base;
using ShrdTransform = SharedCode.Entities.Transform;
using ShrdVec3 = SharedCode.Utils.Vector3;
using SharedCode.Repositories;
using Assets.Src.Client;
using Assets.Src.Cartographer;
using Assets.ColonyShared.GeneratedCode.Regions;
using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using ReactivePropsNs;
using SharedCode.Serializers;
using SharedCode.MovementSync;
using Uins;

namespace Assets.Src.SpawnSystem
{
    //#Note: `UnitySpawnService` not good name - it's actually Unity service (the only for now)
    public class UnitySpawnService : IUnityService
    {
        // ReSharper disable once UnusedMember.Local
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ReactiveProperty<MapDef> _currentMap = new ReactiveProperty<MapDef>();
        public IReactiveProperty<MapDef> CurrentMap => _currentMap;


        readonly ConcurrentDictionary<OuterRef<IEntityObject>, UnityObjectHandle> _handles =
            new ConcurrentDictionary<OuterRef<IEntityObject>, UnityObjectHandle>();

        readonly ConcurrentDictionary<OuterRef<IEntityObject>, TaskCompletionSource<GameObject>> _subscriptions =
            new ConcurrentDictionary<OuterRef<IEntityObject>, TaskCompletionSource<GameObject>>();

        public void RegisterRepo(IEntitiesRepository EntitiesRepository)
        {
            if (!Constants.WorldConstants.SpawnUnityObjects)
                return;
            EntitiesRepository.EntityCreated += async (t, i) => await GotEntity(EntitiesRepository, t, i);
            EntitiesRepository.NewEntityUploaded += async (t, i) => await GotEntity(EntitiesRepository, t, i);
            EntitiesRepository.EntityLoaded += async (t, i) => await GotEntity(EntitiesRepository, t, i);
            EntitiesRepository.EntityDestroy += async (t, i, e) => await LostEntity(EntitiesRepository, t, i, e);
            EntitiesRepository.EntityUnloaded += async (t, i, d, e) => await LostEntity(EntitiesRepository, t, i, d, e);
        }

        internal void Clear()
        {
            _handles.Clear();
            _subscriptions.Clear();
            _currentMap.Value = null;
        }

        private Task LostEntity(IEntitiesRepository repo, int typeId, Guid objId, IEntity entity)
        {
            return LostEntity(repo, typeId, objId, true, entity);
        }

        private Task LostEntity(IEntitiesRepository repo, int typeId, Guid objId, bool destroy, IEntity entity)
        {
            OnLostEntity(repo, objId, typeId, entity);
            return Task.CompletedTask;
        }

        private Task GotEntity(IEntitiesRepository repo, int typeId, Guid objId) => OnGotEntity(repo, objId, typeId);
        bool _doNotCreateUnnecessaryObjects = true;

        //static int _count;
        private async Task OnGotEntity(IEntitiesRepository entitiesRepository, Guid id, int typeId)
        {
            //here I should instantiate prefab  
            var tId = EntitiesRepository.GetReplicationTypeId(typeId, ReplicationLevel.Always);
            if (typeof(IEntityObject).IsAssignableFrom(ReplicaTypeRegistry.GetTypeById(typeId)))
            {
                var batch = EntityBatch.Create().Add(tId, id);
                using (var ent = await entitiesRepository.Get(batch))
                {
                    var eObj = ent.Get<IEntityObjectAlways>(tId, id);
                    if (EntitytObjectsUnitySpawnService.SpawnService != null)
                    {
                        var eObjCustom = eObj as IEntityObjectWithCustomUnityInstantiationAlways;
                        if (eObj == null)
                        {
                        }
                        else if (eObj.Def == null)
                        {
                            Logger.IfWarn()?.Write(id, $"Possible error in entityObjects, eObj.Def is null while entity is created {ReplicaTypeRegistry.GetTypeById(typeId).Name}");
                        }
                        else if (eObjCustom != null && !await eObjCustom.MustBeInstantiatedHere())
                        {   
                            Logger.IfDebug()?.Message("Entity must not be instantiated here. {entity} {repo}", new OuterRef<IEntity>(id, typeId), entitiesRepository).Entity(id).Write();
                        }
                        else // || (eObj.Def.SpawnOnServer && IsServer))
                        {
                            //System.Threading.Interlocked.Increment(ref _count);
                            //Logger.IfWarn()?.Message("OnGotEntity {0}", _count).Write();
                            var transform = PositionedObjectHelper.GetPositioned(ent, tId, id).Transform;
                            var entityRef = new OuterRef<IEntityObject>(id, typeId);
                            var prefab = eObjCustom != null ? await eObjCustom.GetPrefab() : eObj.Def?.Prefab;
                            await InstantiateObject(entitiesRepository, entityRef, eObj.Def, prefab, transform.Position, transform.Rotation);

                            if (eObj is IHasMobMovementSyncAlways hasMobMovementEntity)
                            {
                                hasMobMovementEntity.MovementSync.SubscribePropertyChanged(
                                    nameof(hasMobMovementEntity.MovementSync.PathFindingOwnerRepositoryId),
                                    (args) =>
                                    {
                                        OnPathFindingOwnerRepositoryChanged(entitiesRepository, entityRef, (Guid) args.NewValue);
                                        return Task.CompletedTask;
                                    });

                                OnPathFindingOwnerRepositoryChanged(
                                    entitiesRepository,
                                    entityRef,
                                    hasMobMovementEntity.MovementSync.PathFindingOwnerRepositoryId);
                            }
                        }
                    }
                    else
                        Logger.Warn(
                            $"Possible error in entityObjects, UnityService is null while entity is created {ReplicaTypeRegistry.GetTypeById(typeId).Name}");
                }
            }
        }

        private void OnPathFindingOwnerRepositoryChanged(
            IEntitiesRepository entitiesRepository,
            OuterRef<IEntityObject> entity,
            Guid newOwnerRepositoryId)
        {
            if (!_handles.TryGetValue(entity, out var unityHandle))
            {
                 Logger.IfError()?.Message("Couldn't find unity handle when got pathfinding ownership").Write();;
                return;
            }

            if (entitiesRepository.Id == newOwnerRepositoryId)
            {
                UnityQueueHelper.RunInUnityThreadNoWait(() =>
                    {
                        unityHandle.Ego.GotPathfindingOwnership();
                    });
            }
            else
            {
                UnityQueueHelper.RunInUnityThreadNoWait(() =>
                    {
                        unityHandle.Ego.TryCleanPathfindingOwnership();
                    });
            }
        }

        void OnLostEntity(IEntitiesRepository EntitiesRepository, Guid id, int typeId, IEntity entity)
        {
            //here I should destroy prefab I've created
            if (EntitytObjectsUnitySpawnService.SpawnService == null)
            {
                 Logger.IfError()?.Message("OnLostEntity UnityService == null").Write();;
                return;
            }

            DestroyObject(EntitiesRepository, new OuterRef<IEntityObject>(id, typeId), entity);
        }

        public void DestroyObject(IEntitiesRepository repo, OuterRef<IEntityObject> creator, IEntity entity)
        {
            var doLog = creator.Type.Name.Contains("IWorldBox");
            if (doLog) Logger.IfInfo()?.Message($"UnitySpawnService. DestroyObject({repo})").Write();

            UnityObjectHandle handle = null;
            if (!_handles.TryGetValue(creator, out handle))
                return;

            UnityQueueHelper.RunInUnityThreadNoWait(
                () =>
                {
                    if (doLog) Logger.IfInfo()?.Message($"UnitySpawnService: DestroyObject({repo}). handle = {handle.ToString()}").Write();

                    handle.Repos &= ~repo?.CloudNodeType ?? 0;
                    if (handle.Ego != null && repo != null)
                        handle.Ego.RemoveRepo(repo);
                    if (handle.Repos == CloudNodeType.None)
                    {
                        UnityObjectHandle _ = null;
                        var result = _handles.TryRemove(creator, out _);
                        if (doLog) Logger.IfInfo()?.Message($"UnitySpawnService: DestroyObject. result = {result}").Write();
                        if (handle.Obj != null)
                        {
                            if (doLog) Logger.IfInfo()?.Message($"UnitySpawnService: DestroyObject. GameObject.Destroy({handle.Obj})").Write();
                            GameObject.Destroy(handle.Obj);
                        }
                    }
                });
        }

        public GameObject GetImmediateObjectFor(OuterRef<IEntityObject> e)
        {
            return GetHandle(e)?.Obj;
        }

        public GameObject GetImmediateObjectFor(OuterRef<IEntity> e)
        {
            return GetHandle(e.To<IEntityObject>())?.Obj;
        }

        public ISpatialLegionary GetImmediateMobObjectFor(OuterRef<IEntity> e)
        {
            return GetHandle(e.To<IEntityObject>())?.Leg;
        }

        public Task<GameObject> GetObjectForWhenRegister(OuterRef<IEntityObject> e)
        {
            var handle = GetHandle(e);
            if (handle != null && handle.Instantiated)
                return Task.FromResult(handle.Obj);
            TaskCompletionSource<GameObject> task = _subscriptions.GetOrAdd(e, x => new TaskCompletionSource<GameObject>());
            return task.Task;
        }

        public Task<GameObject> GetObjectForWhenRegister(OuterRef<IEntity> e)
        {
            return GetObjectForWhenRegister(e.To<IEntityObject>());
        }

        private UnityObjectHandle GetHandle(OuterRef<IEntityObject> e)
        {
            UnityObjectHandle c;
            _handles.TryGetValue(e, out c);
            return c;
        }

        // public void RegisterObject(EntityGameObject ego, IEntitiesRepository fromRepo)
        // {
        //     var eRef = new OuterRef<IEntity>(ego.EntityId, ego.TypeId);
        //     var oRef = new OuterRef<IEntityObject>(ego.EntityId, ego.TypeId);
        //     var handle = new UnityObjectHandle() { Ego = ego, Obj = ego.gameObject, Repos = fromRepo.CloudNodeType };
        //     _handles[oRef] = handle;
        //     lock (_clientAuthorityEntities)
        //         if (_clientAuthorityEntities.Contains(eRef))
        //             handle.Ego.NotifyOfClientAuthority();
        //     lock (_serverAuthorityEntities)
        //         if (_serverAuthorityEntities.Contains(eRef))
        //             handle.Ego.NotifyOfServerAuthority();
        //     ego.AddRepo(fromRepo);
        //     NotifySubscriptors(oRef, ego.gameObject);
        // }

        /// <summary>
        /// Возвращает true, если handle для указанного creator ещ не было, и false, если он уже присутствовал   
        /// </summary>
        private bool AddRepoIfAlreadyContainsCreator(
            IEntitiesRepository repo,
            OuterRef<IEntityObject> creator,
            out UnityObjectHandle outHandle)
        {
            var newHandle = new UnityObjectHandle();
            var handle = _handles.GetOrAdd(creator, newHandle);
            handle.Repos = handle.Repos | repo.CloudNodeType;
            outHandle = handle;
            return handle == newHandle;
        }

        public ValueTask<IUnityObjectHandle> InstantiateObject(
            IEntitiesRepository repo,
            OuterRef<IEntityObject> creator,
            IEntityObjectDef def,
            UnityRef<GameObject> prefabRef,
            SharedCode.Utils.Vector3 pos,
            SharedCode.Utils.Quaternion rot)
        {
            if (def == null)
                return default;
            //old: Logger.IfDebug()?.Message("InstantiateObject {1}.{0} Def:{2} At:{3})", creator.Guid, EntitiesRepositoryBase.GetTypeById(creator.TypeId)?.GetFriendlyName(), def, pos).Write();
            if (pos.sqrMagnitude < 1 && !(def is WorldCharacterDef))
                 Logger.IfError()?.Message("InstantiateObject at ZERO position | Def:{0} Creator:{1} Repo:{2}",  def, creator, repo.Id).Write();
            else
                Logger.IfDebug()?.Message("InstantiateObject | Def:{0} Creator:{1} Repo:{2} Pos:{3}", def, creator, repo.Id, pos).Write();

            UnityObjectHandle handle;

            // Double checks to not wait next frame if already contains creator. 2nd is needed, 'cos situation could changed while waiting next frame.
            if (!AddRepoIfAlreadyContainsCreator(repo, creator, out handle))
            {
                UnityQueueHelper.RunInUnityThreadNoWait(
                    async () =>
                    {
                        if (Logger.IsDebugEnabled)
                            Logger.IfDebug()?.Message($"Instantce already exists | Obj:{handle.Obj} Def:{def} Creator:{creator} Repo:{repo.Id}").Write();
                        var task = GetObjectForWhenRegister(creator);
                        await task;

                        if (handle.Ego != null) // на случай преждевременной кончины
                            handle.Ego.AddRepo(repo, false);
                        
                            if (repo.TryGetLockfree<IEntity>(new OuterRef<IEntity>(creator.Guid, creator.TypeId), ReplicationLevel.ClientFull) != null &&
                                repo.CloudNodeType == CloudNodeType.Client)
                                handle.Ego.NotifyOfClientAuthority();
                            if (repo.TryGetLockfree<IEntity>(new OuterRef<IEntity>(creator.Guid, creator.TypeId), ReplicationLevel.Server) != null &&
                                repo.CloudNodeType == CloudNodeType.Server)
                                handle.Ego.NotifyOfServerAuthority();
                    });
                return new ValueTask<IUnityObjectHandle>(handle);
            }
            else
            {
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Instantce not exists | Def:{def} Creator:{creator} Repo:{repo.Id}").Write();
                UnityQueueHelper.RunInUnityThreadNoWait(
                    () =>
                    {
                        if (!Application.isPlaying)
                            return;
                        var position = new UnityEngine.Vector3(pos.x, pos.y, pos.z);
                        var rotation = new UnityEngine.Quaternion(rot.x, rot.y, rot.z, rot.w);
                        if (prefabRef == null)
                            return;
                        if (prefabRef.Target != null)
                        {
                            prefabRef.Target.SetActive(false); //FIXME: rly? changing prefab on runtime?!
                            //if (def.PrefabDef.Target == null)
                            //    handle.Obj = GameObject.Instantiate(prefab, position, rotation); // !<---
                            //else
                                handle.Obj = JsonToGO.Instance.InstantiateAndMergeWith(
                                    prefabRef.Target,
                                    def.PrefabDef.Target,
                                    position,
                                    rotation,
                                    autoEnable: false,
                                    typeof(EntityGameObject),
                                    typeof(ISimpleMovementSync).IsAssignableFrom(creator.Type)); // !<---
                        }
                        else
                        {
                             Logger.IfError()?.Message("EntityObject without prefab: {0} ({2}) at pos {1}",  creator, pos, def).Write();
                            handle.Obj = new GameObject("EntityObject without prefab");
                            handle.Obj.SetActive(false);
                            handle.Obj.transform.SetPositionAndRotation(position, rotation);
                        }


                        var ego = handle.Obj.GetComponent<EntityGameObject>();
                        if (ego == null)
                            ego = handle.Obj.AddComponent<EntityGameObject>();
                        handle.Ego = ego;



                        handle.Obj.SetActive(true);
                        ego.EntityId = creator.Guid;
                        ego.TypeId = creator.TypeId;
                        ego.EntityDef = def;

                        if (pos == default && !(def is WorldCharacterDef))
                            Logger.IfError()?.Message($"ZERO POS {((BaseResource) def).____GetDebugShortName()}").Write();
                        ego.AddRepo(repo, false);
                        if (repo.TryGetLockfree<IEntity>(new OuterRef<IEntity>(creator.Guid, creator.TypeId), ReplicationLevel.ClientFull) != null &&
                                repo.CloudNodeType == CloudNodeType.Client)
                            ego.NotifyOfClientAuthority();
                        if (repo.TryGetLockfree<IEntity>(new OuterRef<IEntity>(creator.Guid, creator.TypeId), ReplicationLevel.Server) != null &&
                            repo.CloudNodeType == CloudNodeType.Server)
                            ego.NotifyOfServerAuthority();
                        if (repo.CloudNodeType == SharedCode.Cloud.CloudNodeType.Server)
                            ego.AuthorityRepo = repo;
                        handle.Instantiated = true;
                        NotifySubscriptors(creator, handle.Obj);
                    });

                return new ValueTask<IUnityObjectHandle>(handle);
            }
        }

        public SuspendingAwaitable RunInUnityThread(Action action)
        {
            return UnityQueueHelper.RunInUnityThread(action);
        }

        internal void UnregisterObject(EntityGameObject entityGameObject, IEntitiesRepository repo)
        {
            if (entityGameObject.AssertIfNull(nameof(entityGameObject)))
                return;

            UnityObjectHandle handle;
            if (_handles.TryGetValue(entityGameObject.OuterRef, out handle))
            {
                if (handle.Obj != null)
                {
                    handle.Ego.RemoveRepo(repo);
                    if (handle.Ego.HasNoRepos)
                    {
                        _handles.TryRemove(new OuterRef<IEntityObject>(handle.Ego.EntityId, handle.Ego.TypeId), out handle);
                    }
                }
            }
            else
            {
                Logger.IfInfo()?.Message($"Not found handle by OuterRef={entityGameObject.OuterRef}").Write();
            }
        }




        private void NotifySubscriptors(OuterRef<IEntityObject> oRef, GameObject obj)
        {
            TaskCompletionSource<GameObject> task;
            if (_subscriptions.TryGetValue(oRef, out task))
                _subscriptions.TryRemove(oRef, out task);
            task?.SetResult(obj);
        }

        void ClearOfParticles()
        {
            try
            {
                var pss = GameObject.FindObjectsOfType<ParticleSystem>();
                foreach (var ps in pss)
                    ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
            catch (Exception e)
            {
                Logger.IfError()?.Exception(e).Write();
            }
        }

        object _loadedLock = new object();
        MapDef _clientMap;
        MapDef _serverMap;
        Guid _clientSceneId;
        bool _loadedOnClient;
        bool _loadedOnServer;
        private LoadingScreenNode.TokenAsync _showLoaderToken;
        private LoadingScreenNode.TokenAsync _hideLoaderToken;

        public async Task<bool> UnloadLevel(IEntitiesRepository fromRepo)
        {
            if (fromRepo.CloudNodeType == CloudNodeType.Client)
            {
                lock (_loadedLock)
                {
                    if (!_loadedOnClient)
                        return true;
                    _loadedOnClient = false;
                }

                using (await LoadingScreenNode.Instance.ShowAsync(this))
                {
                    var scenes = _clientMap.GlobalScenesClient;
                    if (Constants.WorldConstants.DevMode)
                        scenes = scenes.Where(x => !_clientMap.ExcludeInDevMode.Contains(x)).ToArray();
                    using (((IEntitiesRepositoryExtension) fromRepo).DisableRepositoryEntityUnlockTimeout())
                    {
                        await ScenesLoader.UnloadScenesAsync(scenes);
                        await Task.Delay(60);
                    }

                    await UnityQueueHelper.RunInUnityThread(
                        () =>
                        {
                            _streamer.StatusChanged -= _ml.OnStreamerStatusChanged;
                            _streamer.Deinitialize();
                        });

                    var cachedGameState = GameState.Instance;
                    cachedGameState.CurrentMap = null;
                    await ScenesLoader.UnloadScenesAsync(scenes);
                    await UnityQueueHelper.RunInUnityThread(ScenesLoader.ClearAfterSceneLoading);
                }
            }
            else
            {
                lock (_loadedLock)
                {
                    if (!_loadedOnServer)
                        return true;
                    else
                        _loadedOnServer = false;
                }

                RegionBuildHelper.RemoveRootRegionWithGuid(_clientSceneId);
                string[] scenes = _serverMap.GlobalScenesServer;
                if (Constants.WorldConstants.DevMode)
                    scenes = scenes.Where(x => !_serverMap.ExcludeInDevMode.Contains(x)).ToArray();
                using (((IEntitiesRepositoryExtension) fromRepo)
                    ?.DisableRepositoryEntityUnlockTimeout())
                {
                    await ScenesLoader.UnloadScenesAsync(scenes);
                    await Task.Delay(60);
                }
                /*await UnityQueueHelper.RunInUnityThread(() =>
                {
                    _streamer.StatusChanged -= _ml.OnStreamerStatusChanged;
                    _streamer.Deinitialize();
                    LoadingScreenNode.Instance?.Show(this);
                });*/

                var cachedGameState = GameState.Instance;
                cachedGameState.CurrentMap = null;
                await ScenesLoader.UnloadScenesAsync(scenes);
                await UnityQueueHelper.RunInUnityThread(ScenesLoader.ClearAfterSceneLoading);
            }

            await UnityQueueHelper.RunInUnityThread(() => _currentMap.Value = null);
            return true;
        }

        ISceneStreamerInterface _streamer;
        MapLoader _ml;

        public async Task<bool> LoadLevel(IEntitiesRepository fromRepo, Guid sceneId, MapDef map)
        {
            switch (fromRepo.CloudNodeType)
            {
                case CloudNodeType.Server:
                {
                    lock (_loadedLock)
                    {
                        if (_loadedOnServer)
                            return true;

                        _loadedOnServer = true;
                    }

                    _serverMap = map;
                    string[] scenes = map.GlobalScenesServer;
                    if (Constants.WorldConstants.DevMode)
                        scenes = scenes.Where(x => !map.ExcludeInDevMode.Contains(x)).ToArray();
                    using (((IEntitiesRepositoryExtension) fromRepo).DisableRepositoryEntityUnlockTimeout())
                    {
                        await Bootstrapper.LoadMapFolder(AssetBundleHelper.ProduceBundleFolderName(map.DebugName));
                        var results = await ScenesLoader.LoadScenesAsync(scenes);
                        if (results.Any(x => !x))
                            Logger.IfError()?.Message($"CAN'T LOAD ONE OF THE SCENES, NO PRELOADING (map: {map})").Write();

                        await UnityQueueHelper.RunInUnityThread(ClearOfParticles);
                        await Task.Delay(60);
                        await UnityQueueHelper.RunInUnityThread(ScenesLoader.ClearAfterSceneLoading);
                    }

                    break;
                }
                case CloudNodeType.Client:
                {
                    lock (_loadedLock)
                    {
                        if (_loadedOnClient)
                            return true;

                        _loadedOnClient = true;
                    }

                    if (LoadingScreenNode.Instance != null)
                        using (await LoadingScreenNode.Instance.ShowAsync(this))
                        {
                            await LoadClientLevel(fromRepo, sceneId, map);
                        }
                    else
                        // for bots
                        await LoadClientLevel(fromRepo, sceneId, map);

                    break;
                }
                case CloudNodeType.None:
                    break;
                default:
                    break;
            }

            await UnityQueueHelper.RunInUnityThread(() => _currentMap.Value = map);
            return true;
        }

        private async Task LoadClientLevel(IEntitiesRepository fromRepo, Guid sceneId, MapDef map)
        {
            _clientMap = map;
            var scenes = map.GlobalScenesClient;
            if (Constants.WorldConstants.DevMode)
                scenes = scenes.Where(x => !map.ExcludeInDevMode.Contains(x)).ToArray();

            _clientSceneId = sceneId;
            ColonyShared.GeneratedCode.Regions.RegionBuildHelper.LoadRegionsByMapName(map.AllScenesToBeLoadedViaJdb, sceneId);

            using (((IEntitiesRepositoryExtension) fromRepo).DisableRepositoryEntityUnlockTimeout())
            {
                await Bootstrapper.LoadMapFolder(AssetBundleHelper.ProduceBundleFolderName(map.DebugName));
                var results = await ScenesLoader.LoadScenesAsync(scenes);
                if (results.Any(x => !x))
                    Logger.IfError()?.Message($"CAN'T LOAD ONE OF THE SCENES, NO PRELOADING (map: {map})").Write();
                await Task.Delay(60);
                await UnityQueueHelper.RunInUnityThread(ScenesLoader.ClearAfterSceneLoading);
            }

            var cachedGameState = GameState.Instance;


            cachedGameState.CurrentMap = map;

             Logger.IfInfo()?.Message("Initialize streamer").Write();;
            _ml = new MapLoader(cachedGameState.CharacterRuntimeData);
            _streamer = SceneStreamerSystem.Streamer;


            await UnityQueueHelper.RunInUnityThread(
                () =>
                {
                    _streamer.StatusChanged += _ml.OnStreamerStatusChanged;
                    _streamer.Initialize(map.SceneCollectionClient, map.SceneStreamerClient);
                });
        }

        UnityWorldSpace _uws = new UnityWorldSpace();

        public ValueTask<SharedCode.Utils.Vector3> GetDropPosition(
            SharedCode.Utils.Vector3 playerPosition,
            SharedCode.Utils.Quaternion
                playerRotation)
        {
            return _uws.GetDropPosition(playerPosition, playerRotation);
        }

        public bool IsReady()
        {
            return ServerProvider.ServerInited;
        }

        public void SetGCEnabled(bool enabled)
        {
            UnityQueueHelper.RunInUnityThreadNoWait(
                () =>
                {
                    if (enabled)
                        GarbageCollector.GCMode = GarbageCollector.Mode.Enabled;
                    else
                        GarbageCollector.GCMode = GarbageCollector.Mode.Disabled;
                });
        }

        // --- IUnityCheatsHandler: ------------------------------

        #region IUnityCheatsHandler

        private UnityCheatsHandler _cheatsHandler;
        private UnityCheatsHandler CheatsHandler => _cheatsHandler ?? (_cheatsHandler = new UnityCheatsHandler());

        public void MainUnityThreadOnServerSleep(bool isOn, float sleepTime, float delayBeforeSleep, float repeatTime) =>
            CheatsHandler.MainUnityThreadOnServerSleep(isOn, sleepTime, delayBeforeSleep, repeatTime);

        public async Task<ShrdTransform> GetClosestPlayerSpawnPointTransform(IEntitiesRepository repo, ShrdVec3 pos) =>
            await CheatsHandler.GetClosestPlayerSpawnPointTransform(repo, pos);

        #endregion IUnityCheatsHandler

        public void DrawShape(ShapeDef shapeType, SharedCode.Utils.Color color, float duration)
        {
            UnityQueueHelper.RunInUnityThreadNoWait(
                () =>
                {
                    switch (shapeType)
                    {
                        case BoxShapeDef def:
                            DebugExtension.DebugBox(
                                (UnityEngine.Vector3) def.Position,
                                (UnityEngine.Vector3) def.Extents,
                                UnityEngine.Quaternion.Euler((UnityEngine.Vector3) def.Rotation),
                                color,
                                duration);
                            break;
                        case SphereShapeDef def:
                            DebugExtension.DebugWireSphere(
                                (UnityEngine.Vector3) def.Position,
                                color,
                                radius: def.Radius,
                                duration: duration);
                            break;
                        default:
                            DebugExtension.DrawPoint((UnityEngine.Vector3) shapeType?.Position, color);
                            break;
                    }
                });
        }

        public void PostMessageFromMap(string name, string message)
        {
            _ = UnityQueueHelper.RunInUnityThread(() =>
            {
                if (ChatPanel.Instance.AssertIfNull(nameof(ChatPanel)))
                    return;

                ChatPanel.Instance.AddNewMessage(name, message);
            });
            
        }
    }
}
