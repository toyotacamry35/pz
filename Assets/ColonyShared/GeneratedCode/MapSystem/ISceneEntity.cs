using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedCode.Repositories;
using GeneratorAnnotations;
using SharedCode.Entities;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.Entities.Service;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using SharedCode.MapSystem;
using SharedCode.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;
using SharedCode.Serializers;
using MongoDB.Bson.Serialization.Attributes;
using SharedCode.Refs;
using NLog;
using SharedCode.MovementSync;
using SharedCode.Repositories;
using Scripting;
using Assets.ColonyShared.SharedCode.Wizardry;
using Assets.ColonyShared.SharedCode.Entities;
using SharedCode.Entities.Engine;
using SharedCode.Wizardry;
using ColonyHelpers;
using Assets.Src.Arithmetic;
using GeneratedCode.MapSystem;
using ColonyShared.SharedCode.Utils;
using System.Threading;
using Assets.ColonyShared.GeneratedCode.Regions;
using Core.Environment.Logging.Extension;
using NLog.Fluent;

namespace GeneratedCode.MapSystem
{
    [ProtoContract]
    public struct SceneLoadableObjectsData
    {
        [ProtoMember(1)]
        public Guid StaticId; // either the same as the current, or the same as the one in the jdb
        [ProtoMember(2)]
        public Vector3 WorldSpacePosId;
        [ProtoMember(3)]
        public long DestructionTime;
    }
    [DatabaseSaveType(DatabaseSaveType.Explicit)]
    [GenerateDeltaObjectCode]
    public interface ISceneEntity : IEntity, IEntityObject, IHasWorldSpaced, IHasSimpleMovementSync
    {
        long EndTime { get; set; }
        long StartTime { get; set; }
        [LockFreeReadonlyProperty]
        OuterRef<IEntity> EventOwner { get; set; }
        [LockFreeReadonlyProperty]
        [OverrideSerializeSettings(OverwriteList = true)]
        List<SceneChunkDef> SceneChunks { get; set; }
        [ReplicationLevel(ReplicationLevel.Master)]
        IDeltaDictionary<OuterRef<IEntity>, SceneLoadableObjectsData> ObjectsToLoad { get; set; }
        [ReplicationLevel(ReplicationLevel.Server)]
        [BsonIgnore]
        [LockFreeReadonlyProperty]
        [OverrideSerializeSettings(DynamicType = false, AsReference = false)]
        Dictionary<Guid, ResourceSystem.Utils.OuterRef> StaticToDynamicData { get; set; }
        Task<bool> Spawn();
        Task<bool> Despawn();
        Task<bool> SetLoadableObj(OuterRef<IEntity> obj, Guid fromStatic, Vector3 wsPos);
        Task<bool> RemoveObject(OuterRef<IEntity> obj);
        Task<bool> LoadEntity(OuterRef<IEntity> obj);
        [LockFreeMutableProperty]
        [BsonIgnore]
        bool Loaded { get; set; }
        Task<bool> FinishLoading();
    }
    [ProtoContract]
    public struct MapOwner
    {
        [ProtoMember(1)]
        public Guid OwnerSceneId { get; set; }
        [ProtoMember(2)]
        public Guid OwnerMapId { get; set; }
        public OuterRef<IEntity> OwnerScene => new OuterRef<IEntity>(OwnerSceneId, ReplicaTypeRegistry.GetIdByType(typeof(ISceneEntity)));
        public OuterRef<IEntity> OwnerMap => new OuterRef<IEntity>(OwnerMapId, ReplicaTypeRegistry.GetIdByType(typeof(IMapEntity)));
        public MapOwner(Guid ownerSceneId, Guid ownerMapId)
        {
            OwnerSceneId = ownerSceneId;
            OwnerMapId = ownerMapId;
        }
    }
    /// <summary>
    /// Добавляет ф-ционал: игромеханических связей с другими сущностями, объединённые, например, логикой взаимодействия и/или lifetime'ом общего ивента и т.п.
    /// Эти связи персистентны - через БД переживают переподъём сервера.
    /// (не связано со сценами Unity - просто коллизия имён)
    /// </summary>
    public interface IScenicEntity
    {
        [BsonIgnore]
        [OverrideSerializeSettings(DynamicType = false, AsReference = false)]
        [LockFreeMutableProperty(NonAtomicThreadsafe = true)]
        MapOwner MapOwner { get; set; }
        [LockFreeReadonlyProperty]
        Guid StaticIdFromExport { get; set; }

    }

    [DatabaseSaveType(DatabaseSaveType.Explicit)]
    [GenerateDeltaObjectCode]
    public interface IEventPoint : IEntity, IEntityObject, IHasWorldSpaced, IHasSimpleMovementSync, IHasLinksEngine, IDatabasedMapedEntity
    {
        OuterRef<IEntity> RunningEvent { get; set; }
        Task<IEntityObjectDef> LoadEvent();
        Task<bool> AssignEvent(OuterRef<IEntity> newEvent, EventInstanceDef eventDef, ScriptingContext ctx);
        Task<bool> RemoveEvent();
    }


    [GenerateDeltaObjectCode]
    public interface IStoryteller : IEntity, IEntityObject, IHasBuffs, IHasLinksEngine, IHasStatsEngine
    {
        Task<bool> Tick();
        Task<bool> RegisterFromStaticScene();
    }

    [DatabaseSaveType(DatabaseSaveType.Explicit)]
    [GenerateDeltaObjectCode]
    public interface IEventInstance : IEntity, IEntityObject, IHasWorldSpaced, IHasBuffs, IHasLinksEngine, IHasStatsEngine, IHasProvidedScriptingContext, IHasSimpleMovementSync
    {
        OuterRef<IEntity> SceneEntity { get; set; }
        [LockFreeMutableProperty]
        bool Finished { get; set; }
        SpellId MainBuff { get; set; } //event is going on while this buff is going on
        EventPhaseDef CurrentPhase { get; set; }

        Task<bool> Stop();
    }

    public interface IHasProvidedScriptingContext
    {
        ScriptingContext ProvidedContext { get; set; }
    }


}

namespace GeneratedCode.DeltaObjects
{
    public partial class Storyteller : IHookOnInit, IHookOnDatabaseLoad
    {
        Dictionary<EventPointDef, List<OuterRef<IEntity>>> _eventPoints = new Dictionary<EventPointDef, List<OuterRef<IEntity>>>();
        Dictionary<OuterRef<IEntity>, EventInstanceDef> _currentEvents = new Dictionary<OuterRef<IEntity>, EventInstanceDef>();
        List<OuterRef<IEntity>> _freeEventPoints = new List<OuterRef<IEntity>>();

        public Task OnInit()
        {
            var sceneLockfree = EntitiesRepository.TryGetLockfree<ISceneEntityServer>(MapOwner.OwnerScene, ReplicationLevel.Server);
            if (sceneLockfree.Loaded)
                return RegisterFromStaticScene();
            sceneLockfree.SubscribePropertyChanged(nameof(ISceneEntity.Loaded), OnLoaded);
            if (sceneLockfree.Loaded)
                return RegisterFromStaticScene();
            return Task.CompletedTask;
        }

        async Task OnLoaded(EntityEventArgs args)
        {
            using (var w = await this.GetThisWrite())
                await RegisterFromStaticScene();
        }

        public Task OnDatabaseLoad()
        {
            var sceneLockfree = EntitiesRepository.TryGetLockfree<ISceneEntityServer>(MapOwner.OwnerScene, ReplicationLevel.Server);
            if (sceneLockfree.Loaded)
                return RegisterFromStaticScene();
            sceneLockfree.SubscribePropertyChanged(nameof(ISceneEntity.Loaded), OnLoaded);
            if (sceneLockfree.Loaded)
                return RegisterFromStaticScene();
            return Task.CompletedTask;
        }
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public Task<bool> RegisterFromStaticSceneImpl()
        {
            if (!((StorytellerDef)Def).ContentKey.IsEnabled())
                return Task.FromResult(false);
            var scene = MapOwner.OwnerScene;
            var sceneLockfree = EntitiesRepository.TryGetLockfree<ISceneEntityServer>(scene, ReplicationLevel.Server);
            sceneLockfree.UnsubscribePropertyChanged(nameof(ISceneEntity.Loaded), OnLoaded);
            for (int i = 0; i < sceneLockfree.SceneChunks.Count; i++)
            {
                var chunk = sceneLockfree.SceneChunks[i];
                var points = chunk.Entities.Where(x => x != null).Where(x => x.Target.Object != null).Where(x => x.Target.Object.Target.GetType() == typeof(EventPointDef));
                foreach (var point in points.Select(x => x.Target))
                {
                    if (point.JdbLocator.Target == Def)
                    {
                        if (!_eventPoints.TryGetValue((EventPointDef)point.Object.Target, out var list))
                            _eventPoints.TryAdd((EventPointDef)point.Object.Target, list = new List<OuterRef<IEntity>>());
                        if (sceneLockfree.StaticToDynamicData.TryGetValue(point.RefId, out var dynObj))
                        {
                            list.Add(new OuterRef<IEntity>(dynObj));
                        }
                    }
                }
            }
            AsyncUtils.RunAsyncTask(async () =>
            {
                foreach (var ePoint in _eventPoints.SelectMany(x => x.Value))
                {
                    using (var epw = await EntitiesRepository.Get<IEventPoint>(ePoint.Guid))
                    {
                        var ep = epw.Get<IEventPointServer>(ePoint.Guid);
                        ep.SubscribePropertyChanged(nameof(IEventPoint.RunningEvent), OnPointStateChanged);
                        if (ep.RunningEvent != default)
                        {
                            var eDef = await ep.LoadEvent();
                            if (eDef != null)
                                _currentEvents.Add(ep.RunningEvent, (EventInstanceDef)eDef);
                            else
                                _freeEventPoints.Add(ePoint);
                        }
                        else
                            _freeEventPoints.Add(ePoint);
                    }
                }
                _ = AsyncUtils.RunAsyncTask(async () =>
                  {
                      while (((IEntityExt)this.parentEntity).State != EntityState.Destroying && ((IEntityExt)this.parentEntity).State != EntityState.Destroyed)
                      {
                          using (var w = await this.GetThisWrite())
                              await Tick();
                          await Task.Delay(10000);
                      }
                  });
            });
            return Task.FromResult(true);
        }

        private async Task OnPointStateChanged(EntityEventArgs args)
        {
            if ((OuterRef<IEntity>)args.NewValue == default)
                using (var w = await this.GetThisWrite())
                    _freeEventPoints.Add(new OuterRef<IEntity>(args.Sender.ParentEntityId, args.Sender.ParentTypeId));
        }

        StorytellerDef _selfDef => (StorytellerDef)Def;
        public async Task<bool> TickImpl()
        {
            foreach (var storyPack in _selfDef.StoryPacks.Select(x => x.Target))
            {
                if (AmountOfEventsInAStoryPackRightNow(storyPack) < storyPack.MaxAtTheSameTime)
                {
                    await ProcessStoryPack(storyPack);
                }
            }
            return true;
        }
        Random _rand = new Random();
        async Task ProcessStoryPack(StoriesPackDef pack)
        {
            var array = pack.Stories.Select(x => x.Target).Where(x => x.ContentKey.IsEnabled()).Shuffle();
            foreach (var story in array)
            {
                if (AmountOfEventsInAStoryRightNow(story) < story.MaxAtTheSameTime)
                {
                    await ProcessStory(story);
                }
            }
        }
        async Task ProcessStory(StoryDef story)
        {
            if (story.PredicateToRun.Target == null || await story.PredicateToRun.Target.CalcAsync(new OuterRef<IEntity>(this), new ScriptingContext() { Host = new OuterRef<IEntity>(this) }, EntitiesRepository))
            {
                if (_freeEventPoints.Count > 0)
                {
                    var index = _rand.Next(0, _freeEventPoints.Count);
                    var point = _freeEventPoints[index];
                    _freeEventPoints.RemoveAt(index);
                    using (var pointW = await EntitiesRepository.Get<IEventPoint>(point.Guid))
                    {
                        var pointE = pointW.Get<IEventPointServer>(point.Guid);
                        pointE.SubscribePropertyChanged(nameof(IEventPoint.RunningEvent), OnPointStateChanged);
                        var eventORef = new OuterRef<IEntity>(Guid.NewGuid(),
                            EventInstance.StaticTypeId);
                        if (await pointE.AssignEvent(eventORef, story.Event.Target,
                            await story.ContextOnStart.Target.CalcFromDef(
                                new OuterRef<IEntity>(this), new ScriptingContext() { Host = new OuterRef<IEntity>(this) }, EntitiesRepository)))
                            _currentEvents.Add(eventORef, story.Event.Target);


                    }
                }
            }
        }

        int AmountOfEventsInAStoryPackRightNow(StoriesPackDef pack)
        {
            return pack.Stories.Select(x => x.Target).Sum(x => AmountOfEventsInAStoryRightNow(x));
        }

        int AmountOfEventsInAStoryRightNow(StoryDef story)
        {
            _currentEvents.RemoveAll((or, eo) => EntitiesRepository.TryGetLockfree<IEventInstanceServer>(or.Guid, ReplicationLevel.Server) == null);
            return _currentEvents.Where(x => x.Value == story.Event.Target).Sum(x => 1);
        }

    }

    public partial class EventInstance : IHookOnInit, IHookOnDatabaseLoad, IHookOnUnload, IHookOnDestroy
    {
        EventInstanceDef _selfDef => (EventInstanceDef)Def;
        public async Task<bool> StopImpl()
        {
            return await EntitiesRepository.Destroy(this.TypeId, this.Id);
        }
        public async Task OnInit()
        {
            CurrentPhase = _selfDef.Phases.First();
            if (_selfDef.ControlBuff != null)
                MainBuff = await Buffs.TryAddBuff(new ScriptingContext() { Host = new OuterRef<IEntity>(this), CustomArgs = ProvidedContext.CustomArgs }, _selfDef.ControlBuff);
            var pos = MovementSync.Position;
            var rot = MovementSync.Rotation;
            var se = await EntitiesRepository.Create<ISceneEntity>(Guid.NewGuid(), async (e) =>
            {
                e.EventOwner = new OuterRef<IEntity>(this);
                e.MapOwner = MapOwner;
                e.MovementSync.SetPosition = pos;
                e.MovementSync.SetRotation = rot;
                e.SceneChunks = new List<SceneChunkDef>(
                    _selfDef.Phases.First().Target.SceneChunks.Select(x => x.Target));
            });
            SceneEntity = new OuterRef<IEntity>(se);
        }
        public async Task OnDatabaseLoad()
        {
            var pos = MovementSync.Position;
            var rot = MovementSync.Rotation;
            var se = await EntitiesRepository.Load<ISceneEntity>(SceneEntity.Guid, async (e) =>
            {
                e.EventOwner = new OuterRef<IEntity>(this);
                e.MapOwner = MapOwner;
                e.MovementSync.SetPosition = pos;
                e.MovementSync.SetRotation = rot;
                e.SceneChunks = new List<SceneChunkDef>(
                    _selfDef.Phases.First().Target.SceneChunks.Select(x => x.Target));
            });
        }

        public async Task OnDestroy()
        {
            OuterRef<IEntity> _curScene;
            using (var w = await this.GetThisRead())
                _curScene = SceneEntity;
            await EntitiesRepository.Destroy(_curScene.TypeId, _curScene.Guid);
        }

        public async Task OnUnload()
        {
            OuterRef<IEntity> _curScene;
            using (var w = await this.GetThisRead())
                _curScene = SceneEntity;
            await EntitiesRepository.Destroy(_curScene.TypeId, _curScene.Guid, true);

        }
    }

    public partial class EventPoint : IHookOnUnload, IHookOnDatabaseLoad
    {
        public async Task<IEntityObjectDef> LoadEventImpl()
        {
            var ws = EntitiesRepository.TryGetLockfree<IWorldSpaceServiceEntityServer>(WorldSpaced.OwnWorldSpace.Guid, ReplicationLevel.Server);
            var result = await ws.SpawnEntity(
                default,
                RunningEvent, MovementSync.Transform.Position, MovementSync.Transform.Rotation, MapOwner, default, default, default, null, null);
            if (result)
            {
                var e = EntitiesRepository.TryGetLockfree<IEventInstanceServer>(RunningEvent.Guid, ReplicationLevel.Server);
                EntitiesRepository.SubscribeOnDestroyOrUnload(RunningEvent.TypeId, RunningEvent.Guid, OnEventEnd);
                return e.Def;
            }
            return null;
        }
        public async Task<bool> AssignEventImpl(OuterRef<IEntity> newEvent, EventInstanceDef eventDef, ScriptingContext ctx)
        {
            var ws = EntitiesRepository.TryGetLockfree<IWorldSpaceServiceEntityServer>(WorldSpaced.OwnWorldSpace.Guid, ReplicationLevel.Server);
            RunningEvent = newEvent;
            var result = await ws.SpawnEntity(
                default,
                RunningEvent, MovementSync.Transform.Position, MovementSync.Transform.Rotation, MapOwner, default, eventDef, default, null, ctx);
            EntitiesRepository.SubscribeOnDestroyOrUnload(RunningEvent.TypeId, RunningEvent.Guid, OnEventEnd);
            if (result)
            {
                var e = EntitiesRepository.TryGetLockfree<IEventInstanceServer>(RunningEvent.Guid, ReplicationLevel.Server);
                return true;
            }
            return false;
        }

        private async Task OnEventEnd(int arg1, Guid arg2, IEntity arg3)
        {
            using (var w = await this.GetThisWrite())
                RunningEvent = default;
        }

        public async Task<bool> RemoveEventImpl()
        {
            return true;
        }

        public async Task OnUnload()
        {
            OuterRef<IEntity> re;
            using (await this.GetThisRead())
            {
                re = RunningEvent;
                EntitiesRepository.UnsubscribeOnDestroyOrUnload(RunningEvent.TypeId, RunningEvent.Guid, OnEventEnd);
            }
            await EntitiesRepository.Destroy(re.TypeId, re.Guid, true);
        }

        internal static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        public Task OnDatabaseLoad()
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"POINT LOADED : {RunningEvent}").Write();
            return Task.CompletedTask;
        }
    }
    public static class SceneEntityHelper
    {
        public static Guid GetSceneForEntity(this IEntitiesRepository repo, OuterRef<IEntity> ent)
        {
            var osid = repo.TryGetLockfree<IScenicEntityAlways>(ent, ReplicationLevel.Always)?.MapOwner.OwnerSceneId;
            if (!osid.HasValue)
                return default;
            return osid.Value;
        }
    }
    public partial class SceneEntity : IHookOnInit, IHookOnDatabaseLoad, IHookOnDestroy, IHookOnUnload
    {

        CancellationTokenSource _cts;

        internal static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        //some types are on demand only, like character, they are registered, but not loaded before specifically asked
        static bool LoadObjectWithScene(int typeId) => typeId != ReplicaTypeRegistry.GetIdByType(typeof(IWorldCharacter));
        ConcurrentDictionary<Guid, ScenicEntityDef> _defs = new ConcurrentDictionary<Guid, ScenicEntityDef>();
        ConcurrentDictionary<OuterRef<IEntity>, bool> _objectsToUnloadLater = new ConcurrentDictionary<OuterRef<IEntity>, bool>();
        public async Task OnInit()
        {
            _cts = new CancellationTokenSource();
            StartTime = SyncTime.Now;
            await Spawn();
        }
        public async Task OnDatabaseLoad()
        {
            _cts = new CancellationTokenSource();
            StartTime = SyncTime.Now;
            await Spawn();
        }
        public async Task<bool> FinishLoadingImpl()
        {
            foreach (var objToRespawnLater in _objectsToRespawnLater)
            {
                if (StaticToDynamicData.TryGetValue(objToRespawnLater, out var dynRef))
                    if (ObjectsToLoad.TryGetValue(new OuterRef<IEntity>(dynRef), out var obl))
                        if (!RespawnObjectEventually(obl))
                            ObjectsToLoad.Remove(new OuterRef<IEntity>(dynRef));

            }
            _objectsToRespawnLater.Clear();
            Loaded = true;
            return true;
        }
        List<Guid> _objectsToRespawnLater = new List<Guid>();
        public async Task<bool> SpawnImpl()
        {
            var sid = Id;
            var chunks = SceneChunks.ToList();
            await AsyncUtils.RunAsyncTask(() => RegionBuildHelper.LoadRegionsByMapName(chunks, sid));

            _objectsToRespawnLater = new List<Guid>();
            TaskCompletionSource<Guid> tcs;
            SceneEntityDef def = (SceneEntityDef)Def;
            var mid = new MapOwner(Id, MapOwner.OwnerMapId);
            var loadNow = ObjectsToLoad.Where(x => LoadObjectWithScene(x.Key.TypeId)).ToDictionary(x => x.Key, x => x.Value);
            var objectsToLoadIds = loadNow.Where(x => x.Value.StaticId != default).ToDictionary(x => x.Value.StaticId, x => x.Key);

            //sceneEntity loads 2 types of objects. Those from static Ids and those that are not
            //static objects are reloaded, non static loaded as is

            var _staticToDynamicGuids = new Dictionary<Guid, ResourceSystem.Utils.OuterRef>();
            foreach (var chunk in chunks)
            {
                foreach (var obj in chunk.Entities.Select(x => x.Target).Where(x => x != null && x.Object.Target != null))
                {
                    if (objectsToLoadIds.TryGetValue(obj.RefId, out var oldId))
                        _staticToDynamicGuids[obj.RefId] = oldId;
                    else
                        _staticToDynamicGuids[obj.RefId] = new OuterRef<IEntity>(Guid.NewGuid(), ReplicaTypeRegistry.GetIdByType(DefToType.GetEntityType(obj.Object.Target.GetType())));
                }
            }
            StaticToDynamicData = new Dictionary<Guid, ResourceSystem.Utils.OuterRef>(_staticToDynamicGuids);
            var pos = MovementSync.Position;
            var rot = MovementSync.Rotation;
            _ = AsyncUtils.RunAsyncTask(async () =>
              {
                  await Task.Delay(500);//this is a hack until I have a way to ensure every WS has definitely received StaticToDynamicData dictionary
                  foreach (var entityToLoad in loadNow)
                  {
                      Logger.IfDebug()?.Message("entityToLoad {entity_type} {entity_id}", entityToLoad.Key.Type.Name, entityToLoad.Key.Guid).Write();
                      try
                      {
                          if (entityToLoad.Value.StaticId == default)
                          {
                              Guid wsId;
                              var map = EntitiesRepository.TryGetLockfree<IMapEntityServer>(mid.OwnerMapId, ReplicationLevel.Server);
                              wsId = (await map.GetWorldSpaceForPoint(entityToLoad.Value.WorldSpacePosId)).Guid;
                              var ws = EntitiesRepository.TryGetLockfree<IWorldSpaceServiceEntityServer>(wsId, ReplicationLevel.Server);
                              var result = await ws.SpawnEntity(
                                  default, entityToLoad.Key, default, default, mid, default, default, default, null, null);
                              if (result)
                                  _objectsToUnloadLater.TryAdd(entityToLoad.Key, true);
                          }
                      }
                      catch (Exception e)
                      {
                          Logger.IfError()?.Message(e, "Exception during scene load").Write();;
                      }
                  }


                  foreach (var chunk in chunks)
                  {
                      foreach (var obj in chunk.Entities.Select(x => x.Target).Where(x => x.Object.Target != null))
                      {
                          try
                          {
                              var objectToPlace = obj;
                              if (!objectToPlace.ContentKey.IsEnabled())
                                  continue;
                              var typeId = ReplicaTypeRegistry.GetIdByType(DefToType.GetEntityType(objectToPlace.Object.Target.GetType()));

                              Guid id = _staticToDynamicGuids[objectToPlace.RefId].Guid;
                              _defs.TryAdd(objectToPlace.RefId, objectToPlace);
                              loadNow.TryGetValue(new OuterRef<IEntity>(id, typeId), out var loadedObj);
                              if (loadedObj.DestructionTime == 0)
                              {
                                  var map = EntitiesRepository.TryGetLockfree<IMapEntityServer>(mid.OwnerMapId, ReplicationLevel.Server);
                                  var wsId = (await map.GetWorldSpaceForPoint(objectToPlace.Position)).Guid;
                                  var ws = EntitiesRepository.TryGetLockfree<IWorldSpaceServiceEntityServer>(wsId, ReplicationLevel.Server);
                                  var result = await ws.SpawnEntity(
                                          objectToPlace.RefId,
                                          new OuterRef<IEntity>(id, typeId), objectToPlace.Position + pos, objectToPlace.Rotation, mid, default, objectToPlace.Object.Target, default, objectToPlace, null);
                                  if (result)
                                      _objectsToUnloadLater.TryAdd(new OuterRef<IEntity>(id, typeId), true);
                              }
                              else
                                  _objectsToRespawnLater.Add(objectToPlace.RefId);
                          }
                          catch (Exception e)
                          {
                              Logger.IfError()?.Message(e, "Exception during scene load").Write();;
                          }
                      }
                  }
                  using (var w = await this.GetThisWrite())
                      await FinishLoading();

              });

            return true;
        }

        public Task<bool> SetLoadableObjImpl(OuterRef<IEntity> obj, Guid fromStatic, Vector3 wsPos)
        {
            ObjectsToLoad[obj] = new GeneratedCode.MapSystem.SceneLoadableObjectsData() { StaticId = fromStatic, WorldSpacePosId = wsPos };
            _objectsToUnloadLater.TryAdd(obj, true);
            return Task.FromResult(true);
        }
        public Task<bool> RemoveObjectImpl(OuterRef<IEntity> obj)
        {
            if (ObjectsToLoad.TryGetValue(obj, out var obl))
            {
                obl.DestructionTime = SyncTime.Now;
                ObjectsToLoad[obj] = obl;
                if (!RespawnObjectEventually(obl))
                    ObjectsToLoad.Remove(obj);
            }

            return Task.FromResult(true);
        }

        private bool RespawnObjectEventually(SceneLoadableObjectsData obl)
        {
            if (!_defs.TryGetValue(obl.StaticId, out var def))
                return false;
            if (def.TimeToRespawn == 0 || obl.DestructionTime == 0)
                return false;
            var mid = new MapOwner(Id, MapOwner.OwnerMapId);
            Guid id = StaticToDynamicData[def.RefId].Guid;
            var typeId = ReplicaTypeRegistry.GetIdByType(DefToType.GetEntityType(def.Object.Target.GetType()));
            var pos = MovementSync.Position;
            ObjectsToLoad[new OuterRef<IEntity>(id, typeId)] = obl;
            AsyncUtils.RunAsyncTask(async () =>
            {
                var deltaBeforeEnd = obl.DestructionTime < EndTime ? EndTime - obl.DestructionTime : 0;

                var secondsDelay = def.TimeToRespawn - deltaBeforeEnd;
                if (secondsDelay < 10)
                    secondsDelay = 10;//this is a small hack to ensure the objects is actually destroyed, as we do not change the guid
                await Task.Delay(TimeSpan.FromSeconds(secondsDelay), _cts.Token);
                //RESPAWN ENTITY
                var map = EntitiesRepository.TryGetLockfree<IMapEntityServer>(mid.OwnerMapId, ReplicationLevel.Server);
                var wsId = (await map.GetWorldSpaceForPoint(def.Position)).Guid;
                var ws = EntitiesRepository.TryGetLockfree<IWorldSpaceServiceEntityServer>(wsId, ReplicationLevel.Server);
                var result = await ws.SpawnEntity(
                       def.RefId,
                       new OuterRef<IEntity>(id, typeId), def.Position + pos, def.Rotation, mid, default, def.Object.Target, default, def, null);
                using (var w = await this.GetThisWrite())
                {
                    var oobl = ObjectsToLoad[new OuterRef<IEntity>(id, typeId)];
                    oobl.DestructionTime = 0;
                    ObjectsToLoad[new OuterRef<IEntity>(id, typeId)] = oobl;
                }
            });
            return true;
        }

        public Task<bool> LoadEntityImpl(OuterRef<IEntity> obj)
        {
            return Task.FromResult(true);
        }
        public async Task<bool> DespawnImpl()
        {

            return true;
        }

        public async Task OnUnload()
        {
            await AsyncUtils.RunAsyncTask(async () => {
                _cts.Cancel();
                _cts.Dispose();
                foreach (var obj in _objectsToUnloadLater)
                    try
                    {
                        await EntitiesRepository.Destroy(obj.Key.TypeId, obj.Key.Guid, true);
                    }
                    catch (Exception e)
                    {
                        Logger.IfError()?.Exception(e).Write();
                    }
                using (var w = await this.GetThisWrite())
                {
                    await Despawn();
                    EndTime = SyncTime.Now;
                }
            });
        }
        public async Task OnDestroy()
        {
            await OnUnload();
        }
    }

}

namespace SharedCode.EntitySystem
{
    public static class BaseEntityExt
    {
        public static ValueTask<IEntitiesContainer> GetThisWrite(this IEntity ent)
        {
            return ((IEntitiesRepositoryExtension)ent.EntitiesRepository).GetExclusive(ent.TypeId, ent.Id);
        }

        public static ValueTask<IEntitiesContainer> GetThisRead(this IEntity ent)
        {
            return (ent.EntitiesRepository).Get(ent.TypeId, ent.Id);
        }
    }

}
