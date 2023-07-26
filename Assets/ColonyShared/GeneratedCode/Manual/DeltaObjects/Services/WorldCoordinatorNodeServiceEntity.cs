using GeneratedCode.Custom.Config;
using GeneratedCode.DeltaObjects.Chain;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedCode.Repositories;
using NLog;
using SharedCode.Aspects.Sessions;
using SharedCode.Config;
using SharedCode.Entities;
using SharedCode.Entities.Cloud;
using SharedCode.Entities.Service;
using SharedCode.EntitySystem;
using SharedCode.MapSystem;
using SharedCode.Refs;
using SharedCode.Serializers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;

namespace GeneratedCode.DeltaObjects
{
    public partial class WorldCoordinatorNodeServiceEntity : IHasLoadFromJObject, IHookOnInit
    {
        private static readonly NLog.Logger Logger = LogManager.GetLogger("WorldCoordinator");
        private WorldCoordinatorConfig _selfConfig;
        private ServerServicesConfigDef _webServicesConfig;

        private readonly ConcurrentQueue<ChunkQueueElement> _mapChunkWaitQueue = new ConcurrentQueue<ChunkQueueElement>();
        private readonly ConcurrentDictionary<Guid /*MapId*/, (Guid, MapDef)> _currentlyLoadingMapOnMapHost = new ConcurrentDictionary<Guid, (Guid, MapDef)>();
        private readonly ConcurrentDictionary<Guid /*MapId*/, List<Guid> /*UsersList*/> _waitingUsersForMapID = new ConcurrentDictionary<Guid, List<Guid>>();
        private readonly ConcurrentDictionary<Guid /*MapId*/, List<TaskCompletionSource<bool>> /*UsersList*/> _waitingTasksForMapID = new ConcurrentDictionary<Guid, List<TaskCompletionSource<bool>>>();
        private readonly ConcurrentDictionary<Guid /*UserId*/, MapElement /*MapId*/> _mapPerUser = new ConcurrentDictionary<Guid, MapElement>();

        class ChunkQueueElement
        {
            public Guid WorldSpaceId { get; }

            public MapDef ChunkDef { get; }

            public Guid MapInstanceId { get; }

            public Guid MapInstanceRepositoryId { get; }

            public ChunkQueueElement(Guid id, MapDef chunkDef, Guid mapInstanceId, Guid mapInstanceRepositoryId)
            {
                WorldSpaceId = id;
                ChunkDef = chunkDef;
                MapInstanceId = mapInstanceId;
                MapInstanceRepositoryId = mapInstanceRepositoryId;
            }
        }

        public Task Load(CloudSharedDataConfig sharedConfig, CustomConfig config, IEntitiesRepository entitiesRepository)
        {
            _selfConfig = (WorldCoordinatorConfig)config;
            _webServicesConfig = sharedConfig.WebServicesConfig.Target;
            return Task.CompletedTask;
        }

        public Task OnInit()
        {
            this.EntitiesRepository.CloudRequirementsMet += EntitiesRepository_CloudRequirementsMet;
            return Task.CompletedTask;
        }

        private async Task EntitiesRepository_CloudRequirementsMet()
        {
            await PreloadMaps();

            using (await EntitiesRepository.Get<IWorldCoordinatorNodeServiceEntity>(Id))
            {
                this.Chain().Delay(10, true, fromUtcNow: true).UpdateMapQueue().Run();
            }
        }

        private async Task<HostOrInstallMapResult> CreateMap(Guid mapId, Guid realmId, MapDef mapDef, RealmRulesDef realmRules)
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message("CreateMap | {@}", new { mapId, mapDef, realmId, realmRules}).Write();
            
            var mapHostEntities = ((EntitiesRepository)EntitiesRepository).GetEntitiesCollection(typeof(IMapHostEntity)).Values;
            Logger.IfInfo()?.Message($"Enumerating map hosts => {string.Join(", ", mapHostEntities.Select(v => v.Id))}").Write();
            var mapHostedCount = new Dictionary<Guid, int>();
            foreach (var mapHostRef in mapHostEntities)
            {
                using (var wrapper = await EntitiesRepository.Get<IMapHostEntityServer>(mapHostRef.Id))
                {
                    var mapHost = wrapper.Get<IMapHostEntityServer>(mapHostRef.Id);
                    if (mapHost == null)
                        continue;

                    if (!mapHost.HostedMaps.Any())
                    {
                        var res = await mapHost.HostMap(mapId, realmId, mapDef, realmRules);
                        if (res == HostOrInstallMapResult.Error)
                            continue;
                        if (res == HostOrInstallMapResult.None)
                        {
                            Logger.IfError()?.Message($"Can't create mapDef {mapDef.____GetDebugShortName()}, something is wrong, answer is None").Write();
                            return HostOrInstallMapResult.Error;
                        }
                        return res;
                    }
                    else
                    {
                        mapHostedCount[mapHostRef.Id] = mapHost.HostedMaps.Count();
                    }
                }
            }

            if (mapHostedCount.Any())
            {
                var mapHostId = mapHostedCount.OrderBy(v => v.Value).First().Key;
                using (var wrapper = await EntitiesRepository.Get<IMapHostEntityServer>(mapHostId))
                {
                    var mapHost = wrapper.Get<IMapHostEntityServer>(mapHostId);
                    if (mapHost != null)
                    {
                        return await mapHost.HostMap(mapId, realmId, mapDef, realmRules);
                    }
                }
            }

            Logger.IfError()?.Message($"Can't create mapDef {mapDef.____GetDebugShortName()}, there are no mapHosts available").Write();
            return HostOrInstallMapResult.Error;
        }

        public Task<CreateOrConfirmMapResult> RequestLoginToMapImpl(MapLoginMeta meta) => RequestLoginToMapImplImpl(meta, null);

        public async Task<CreateOrConfirmMapResult> RequestLoginToMapImplImpl(MapLoginMeta meta, TaskCompletionSource<bool> onRequestCompleted)
        {
            var mapId = meta.TargetMapId;
            var mapDef = meta.TargetMap;
            var userId = meta.UserId;
            if(meta.TargetRealmGuid == default)
            {
                using(var w = await EntitiesRepository.GetFirstService<ILoginServiceEntityServer>())
                {
                    var le = w.GetFirstService<ILoginServiceEntityServer>();
                    meta.TargetRealmGuid = await le.GetUserRealm(userId);
                }
            }
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"RequestLoginToMapImpl({meta})").Write();

            if (mapId != Guid.Empty)
            {
                var (success, createOrConfirmMapResult) = await TryConnectToConcreteMap(mapId, userId);
                if (success)
                {
                    return createOrConfirmMapResult;
                }
            }
            else
            {
                var (success, createOrConfirmMapResult) = await TryFindMap(meta, mapDef, userId);
                if (success)
                {
                    return createOrConfirmMapResult;
                }
            }
            
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Before CreateMap( {meta.TargetRealmGuid} {mapDef?.____GetDebugShortName()}").Write();

            var newMapId = meta.TargetMapId == default ? Guid.NewGuid() : meta.TargetMapId;

            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Before CreateMap mapId( {newMapId}").Write();

            _currentlyLoadingMapOnMapHost[newMapId] = (meta.TargetRealmGuid, mapDef);
            if (userId != Guid.Empty)
                _waitingUsersForMapID.TryAdd(newMapId, new List<Guid>(new Guid[] { userId }));
            if (onRequestCompleted != null)
            {
                var list = _waitingTasksForMapID.GetOrAdd(newMapId, (k) => new List<TaskCompletionSource<bool>>());
                lock (list)
                    list.Add(onRequestCompleted);
            }

            HostOrInstallMapResult result;
            try
            {
                result = await CreateMap(newMapId, meta.TargetRealmGuid, mapDef, meta.RealmRules);
                Logger.IfInfo()?.Message($"CreateMap({newMapId}, {meta.TargetRealmGuid} {mapDef?.____GetDebugShortName()}) => result = {result}").Write();

            }
            finally
            {
                _currentlyLoadingMapOnMapHost.TryRemove(newMapId, out var toRemove);
            }

            if (result != HostOrInstallMapResult.Success)
            {
                Logger.IfError()?.Message("Cant create map id {0} def {1} result {2}", newMapId, mapDef, result.ToString()).Write();
                return new CreateOrConfirmMapResult { Result = CreateOrConfirmMapResultType.Error };
            }

            //.Boris Есть временный баг (race condition), что добавление вновь подписанных реплик еще не сработало, так как получило репозиторный лок (в Repository.EntityUpload) позже чем обработка ответа RPC
            //Поэтому пробуем взять три раза в цикле c ожиданием в секунду
            //Потом когда этот баг исправится, цикл надо убрать
            int tryGetCount = 0;
            while (tryGetCount < 3)
            {
                tryGetCount++;
                using (var wrapper = await EntitiesRepository.Get<IMapEntityServer>(newMapId))
                {
                    var mapEntity = wrapper?.Get<IMapEntityServer>(newMapId);
                    if (mapEntity == null)
                    {
                        if (tryGetCount < 3)
                        {
                            await Task.Delay(TimeSpan.FromSeconds(1));
                            continue;
                        }
                        Logger.IfWarn()?.Message($"mapEntity replication {newMapId} not exist in worldcoordinator repository {EntitiesRepository.Id}").Write();
                        return new CreateOrConfirmMapResult { Result = CreateOrConfirmMapResultType.Error };
                    }

                    EntitiesRepository.SubscribeOnDestroyOrUnload(MapEntity.StaticTypeId, newMapId, async (tid, guid, mapE) =>
                    {
                        foreach(var id in _mapPerUser.ToList())
                        {
                            if (id.Value.MapId == newMapId)
                                _mapPerUser.TryRemove(id.Key, out _);
                            //ideally here I should relog everyone
                        }

                    });

                    Logger.IfWarn()?.Message($"Add wsds to queue {mapEntity.WorldSpaces.Count} {mapEntity.Id}").Write();

                    if (mapEntity.State == MapEntityState.Loaded)
                    {
                        await DirectUsersToNewlyLoadedMap(newMapId, MapEntityState.Loaded);
                    }
                    else
                    {
                        mapEntity.SubscribePropertyChanged(nameof(mapEntity.State), MapStateChangedCallback);

                        foreach (var worldSpaceDescription in mapEntity.WorldSpaces.Where(x => x.State != MapChunkState.Loaded))
                        {
                            Logger.IfWarn()?.Message($"Add wsd to worldNodes queue {mapDef?.____GetDebugShortName()} {userId}").Write();
                            _mapChunkWaitQueue.Enqueue(new ChunkQueueElement(worldSpaceDescription.WorldSpaceGuid,
                                worldSpaceDescription.ChunkDef, newMapId, mapEntity.OwnerRepositoryId));
                        }
                    }

                    break;
                }
            }

            return new CreateOrConfirmMapResult { Result = CreateOrConfirmMapResultType.Exist, MapId = newMapId };
        }

        private async Task<(bool, CreateOrConfirmMapResult)> TryFindMap(MapLoginMeta meta, MapDef mapDef, Guid userId)
        {
            if (mapDef.IsForSingleUserOnly)
                return (default, default);
            _mapPerUser.TryGetValue(meta.UserId, out var mapElement);
            var currentlyLoadingMap = _currentlyLoadingMapOnMapHost.FirstOrDefault(x => 
            (((x.Value.Item2 == mapDef && meta.TargetRealmGuid == default) || 
            (x.Value.Item1 == meta.TargetRealmGuid && 
            (mapDef == null || x.Value.Item2 == mapDef)))));
            if (currentlyLoadingMap.Key != default)
            {
                Logger.IfError()?.Message($"_currentlyLoadingMaps => {currentlyLoadingMap}").Write();
                var usersWaitingForMap = _waitingUsersForMapID[currentlyLoadingMap.Key];
                lock (usersWaitingForMap)
                {
                    usersWaitingForMap.Add(userId);
                }

                return (true, new CreateOrConfirmMapResult {Result = CreateOrConfirmMapResultType.Exist, MapId = currentlyLoadingMap.Key});
            }
            else
            {
                var usersNumberOnMap = new Dictionary<Guid, int>();
                var mapEntities = ((EntitiesRepository) EntitiesRepository).GetEntitiesCollection(typeof(IMapEntity)).Values;
                Logger.IfInfo()?.Message($"Enumerating map entities => {string.Join(", ", mapEntities.Select(v => v.Id))}").Write();

                foreach (var entityRef in mapEntities)
                {
                    using (var wrapper = await EntitiesRepository.Get<IMapEntityServer>(entityRef.Id))
                    {
                        var mapEntity = wrapper?.Get<IMapEntityServer>(entityRef.Id);
                        if (mapEntity == null)
                        {
                            Logger.IfError()?.Message("Map entity wrapper get error {0}", entityRef.Id).Write();
                            continue;
                        }

                        Logger.Error($"Map with realm {mapEntity.RealmId} {mapEntity.Map?.____GetDebugShortName()}" +
                            $" {meta.TargetRealmGuid} {mapDef?.____GetDebugShortName()}");

                        if (((mapEntity.Map == mapDef && meta.TargetRealmGuid == default) || 
                            (mapEntity.RealmId == meta.TargetRealmGuid && (mapDef == null || mapEntity.Map == mapDef))))
                        {
                            Logger.IfError()?.Message($"Got map entities => mapEntity.Map = {mapEntity.Map}, mapEntity.Id = {mapEntity.Id}").Write();

                            if (userId == Guid.Empty)
                            {
                                return (true, new CreateOrConfirmMapResult
                                    {Result = CreateOrConfirmMapResultType.Exist, MapId = mapEntity.Id});
                            }

                            var maps = _mapPerUser.Where(v => v.Value.MapId == mapEntity.Id);
                            if (!maps.Any()) // no user at this map
                            {
                                Logger.IfError()?.Message($"Got map with no user on it. Loggin in... ").Write();
                                await ConnectUsersToMap(mapEntity, new List<Guid> {userId});

                                return (true, new CreateOrConfirmMapResult
                                {
                                    Result = CreateOrConfirmMapResultType.Exist, MapId = mapEntity.Id
                                });
                            }
                            else
                            {
                                var userCount = maps.Count();
                                usersNumberOnMap[mapEntity.Id] = userCount;
                                Logger.IfError()?.Message($"Got map with {userCount} no user on it.").Write();
                            }
                        }
                    }
                }

                var mapHostsCount = ((EntitiesRepository) EntitiesRepository).GetEntitiesCollection(typeof(IMapHostEntity)).Values.Count();
                var mapEntitiesCount = ((EntitiesRepository) EntitiesRepository).GetEntitiesCollection(typeof(IMapEntity)).Values.Count();
                Logger.IfError()?.Message($"mapHostsCount = {mapHostsCount}, mapEntitiesCount = {mapEntitiesCount}").Write();
                
                
                var usersCount = usersNumberOnMap.OrderBy(v => v.Value).ToArray();
                Logger.IfError()?.Message($"Have maps with users: {string.Join(", ", usersCount.Select(v => v.Key + ": " + v.Value))}").Write();;

                var mapID = usersCount.FirstOrDefault().Key;
                if(mapID != default)
                {
                    using (var wrapper = await EntitiesRepository.Get<IMapEntityServer>(mapID))
                    {
                        var mapEntity = wrapper?.Get<IMapEntityServer>(mapID);
                        if (mapEntity == null)
                        {
                            Logger.IfError()?.Message("Map entity wrapper get error {0}", mapID).Write();
                        }

                        if (userId != Guid.Empty)
                            await ConnectUsersToMap(mapEntity, new List<Guid> { userId });

                        //Logger.IfWarn()?.Message($"Connect user map {mapDef.____GetDebugShortName()} {userId}").Write();
                        {
                            return (true, new CreateOrConfirmMapResult
                            {
                                Result = CreateOrConfirmMapResultType.Exist,
                                MapId = mapEntity.Id
                            });
                        }
                    }

                }
                // TODO Vova подозрительный код
                // return new CreateOrConfirmMapResult {Result = CreateOrConfirmMapResultType.Exist, MapId = mapID};

            }

            return (false, default);
        }

        private async Task<(bool, CreateOrConfirmMapResult)> TryConnectToConcreteMap(Guid mapId, Guid userId)
        {
            var currentlyLoadingMap = _currentlyLoadingMapOnMapHost.FirstOrDefault(x => x.Key == mapId);
            if (currentlyLoadingMap.Key != default)
            {
                Logger.IfError()?.Message($"currentlyloadingMap => {currentlyLoadingMap}").Write();
                var usersWaitingForMap = _waitingUsersForMapID[currentlyLoadingMap.Key];
                lock (usersWaitingForMap)
                {
                    usersWaitingForMap.Add(userId);
                }

                return (true, new CreateOrConfirmMapResult {Result = CreateOrConfirmMapResultType.Exist, MapId = currentlyLoadingMap.Key});
            }

            using (var wrapper = await EntitiesRepository.Get<IMapEntityServer>(mapId))
            {
                var mapEntity = wrapper?.Get<IMapEntityServer>(mapId);
                if (mapEntity != null)
                {
                    if (userId != Guid.Empty)
                    {
                        await ConnectUsersToMap(mapEntity, new List<Guid> {userId});
                    }

                    return (true, new CreateOrConfirmMapResult
                    {
                        Result = CreateOrConfirmMapResultType.Exist, MapId = mapId
                    });
                }
            }

            return (false, default);
        }

        public async Task<CreateOrConfirmMapResult> RequestLogoutFromMapImpl(Guid userId, bool terminal)
        {
            MapElement mapElement;
            if (_mapPerUser.TryRemove(userId, out mapElement))
            {
                using (var wrapper = await EntitiesRepository.Get<IMapHostEntityServer>(mapElement.MapRepositoryId))
                {
                    var mapHost = wrapper.Get<IMapHostEntityServer>(mapElement.MapRepositoryId);
                    Logger.IfError()?.Message($"Has map host {mapElement.MapRepositoryId} {mapHost != null}").Write();
                    await mapHost.LogoutUserFromMap(userId, mapElement.MapId, terminal);
                }
                //Борис сказал, что саб/ансабскрайбы не дожидаются ответа удалённого репозитория, так что не факт что всё успело выгрузится
                await Task.Delay(TimeSpan.FromSeconds(1f));
                //using (var wrapper = await EntitiesRepository.GetFirstService<ILoginInternalServiceEntityServer>())
                //{
                //    var loginInternalEntity = wrapper.GetFirstService<ILoginInternalServiceEntityServer>();
                //    await loginInternalEntity.RemoveClientConnection(new List<Guid>(new[] { userId }), mapElement.MapRepositoryId);
                //}
                await Task.Delay(TimeSpan.FromSeconds(1f));

            }
            return new CreateOrConfirmMapResult { Result = CreateOrConfirmMapResultType.Exist };
        }

        private Task MapStateChangedCallback(EntityEventArgs args)
        {
            var newState = (MapEntityState)args.NewValue;
            var mapEntityId = args.Sender.ParentEntityId;
            var repo = EntitiesRepository;
            Logger.IfInfo()?.Message($"MapStateChanged(newState: {newState}, mapEntityId: {mapEntityId})").Write();

            Logger.IfWarn()?.Message($"MapStateChangedCallback {mapEntityId} {newState}").Write();
            AsyncUtils.RunAsyncTask(() => DirectUsersToNewlyLoadedMap(mapEntityId, newState));

            return Task.CompletedTask;
        }

        private async Task DirectUsersToNewlyLoadedMap(Guid mapEntityId, MapEntityState mapState)
        {
            using (var wrapper = await EntitiesRepository.Get<IMapEntityServer>(mapEntityId))
            {
                var mapEntity = wrapper?.Get<IMapEntityServer>(mapEntityId);
                if (mapEntity == null)
                {
                    Logger.IfError()?.Message("MapStateChangedCallback not found map entity {0}", mapEntityId).Write();
                    return;
                }
                if (mapState == MapEntityState.Loaded)
                {
                    var tasks = GetAndRemoveTasks(mapEntityId);
                    foreach (var task in tasks)
                        task.SetResult(true);

                    await ConnectUsersToMap(mapEntity, GetAndRemoveUsers(mapEntityId));
                }
                else if (mapState == MapEntityState.Failed)
                {
                    Logger.IfError()?.Message("Map {0} state changed to failed", mapEntityId).Write();

                    var tasks = GetAndRemoveTasks(mapEntityId);
                    var exception = new Exception($"Failed to load map {mapEntityId}");
                    foreach (var task in tasks)
                        task.SetException(exception);

                    await SendUsersFailedMap(mapEntity, GetAndRemoveUsers(mapEntityId));
                }
            }
        }

        List<Guid> GetAndRemoveUsers(Guid mapId)
        {
            List<Guid> users;
            if (_waitingUsersForMapID.TryRemove(mapId, out users))
            {
                lock (users)
                {
                    var usersCopy = users.ToList();
                    return usersCopy;
                }
            }
            return new List<Guid>();
        }

        IReadOnlyList<TaskCompletionSource<bool>> GetAndRemoveTasks(Guid mapId)
        {
            if (_waitingTasksForMapID.TryRemove(mapId, out var tasks))
            {
                lock (tasks)
                {
                    var usersCopy = tasks.ToList();
                    return usersCopy;
                }
            }
            return Array.Empty<TaskCompletionSource<bool>>();
        }

        private async Task ConnectUsersToMap(IMapEntityServer mapEntity, List<Guid> users)
        {
            var mapRepositoryid = mapEntity.OwnerRepositoryId;

            Logger.IfInfo()?.Message($"ConnectUsersToMap(mapEntity: ({mapEntity.Id}, {mapEntity.Map.____GetDebugShortName()}), users: {string.Join(", ", users.Select(v => v.ToString()))})").Write();

            foreach (var user in users)
            {
                if (_mapPerUser.TryGetValue(user, out var prevMap))
                {
                    await RequestLogoutFromMap(user,false);
                }
                _mapPerUser[user] = new MapElement() { MapRepositoryId = mapEntity.OwnerRepositoryId, MapId = mapEntity.Id, MapDef = mapEntity.Map };

            }

            using (var wrapper = await EntitiesRepository.Get<IMapHostEntityServer>(mapRepositoryid))
            {
                var mapHostEntity = wrapper.Get<IMapHostEntityServer>(mapRepositoryid);
                if (mapHostEntity == null)
                {
                    Logger.IfError()?.Message("Cant find target map host {repo_id}", mapRepositoryid).Write();
                    return;
                }

                await mapHostEntity.AddUsersDirect(users);
            }
        }

        private Task SendUsersFailedMap(IMapEntityServer mapEntity, List<Guid> users)
        {
            //TODO дописать отправку на клиент (через логин сервер) сообщения о провале карты
            return Task.CompletedTask;
        }

        public Task<bool> CancelMapRequestImpl(Guid requestId)
        {
            throw new NotImplementedException();
        }
        public async Task<string> GlobalNotificationImpl(string notificationText)
        {
            foreach (var pair in ((EntitiesRepository)EntitiesRepository).GetEntitiesCollection(typeof(IMapEntity)))
            {
                using(var mw = await EntitiesRepository.Get<IMapEntityServer>(pair.Key))
                {
                    var mapEntity = mw.Get<IMapEntityServer>(pair.Key);
                    await mapEntity.NotifyAllCharactersViaChat(notificationText);
                }
            }
            return "OK";
        }
        // NOT an rpc
        public async Task<bool> PreloadMaps()
        {
            if (_selfConfig == null)
                return false;
            if (_selfConfig.PreloadedMaps == null)
                return false;
            if (!_selfConfig.PreloadedMaps.Any())
                return false;

            List<Task> tasksToWait = new List<Task>();
            using (await EntitiesRepository.Get<IWorldCoordinatorNodeServiceEntity>(Id))
            using (var loginW = await EntitiesRepository.GetFirstService<ILoginServiceEntity>())
            {
                var loginEntity = loginW.GetFirstService<ILoginServiceEntityServer>();

                foreach (var map in _selfConfig.PreloadedMaps)
                {
                    Guid.TryParse(_webServicesConfig.RealmId, out var rid);
                    var realmRef = await loginEntity.FindRealm(map.Target.RealmRulesQuery.Target, default, rid);
                    if(realmRef == null)
                        throw new Exception($"Failed to find realm with query {map.Target.RealmRulesQuery.Target} to preload. Check GUID {rid}(Text representation: {_webServicesConfig.RealmId}) in cluster external services config");

                    var tcs = new TaskCompletionSource<bool>();

                    var result = await RequestLoginToMapImplImpl(new MapLoginMeta()
                    {
                        TargetRealmGuid = realmRef.Realm.Guid,
                        CurrentRealmId = realmRef.Realm.Guid,
                        TargetMapId = map.Target.MapId,
                        TargetMap = map.Target.RealmRulesQuery.Target.RealmRules.Target.DefaultMap,
						RealmRules = map.Target?.RealmRulesQuery.Target?.RealmRules,
                        UserId = Guid.Empty
                    }, tcs);

                    tasksToWait.Add(tcs.Task);

                    if (!result.IsSuccess)
                        Logger.IfError()?.Message("Error on preload mapDef {0} result {1}", map.ToString(), result.Result.ToString()).Write();
                }
            }

            if(Manual.Repositories.AsyncEntitiesRepositoryRequestContext.Head != null)
                throw new AsyncContextException("Should not call this from RPC");

            await Task.WhenAll(tasksToWait);

            return true;
        }

        public async Task UpdateMapQueueImpl()
        {
            if (_mapChunkWaitQueue.IsEmpty)
                return;

            Logger.IfWarn()?.Message($"Update map queue").Write();
            foreach (var pair in ((EntitiesRepository)EntitiesRepository).GetEntitiesCollection(typeof(IWorldNodeServiceEntity)))
            {
                var worldNodeRef = pair.Value;
                using (var wrapper = await EntitiesRepository.Get<IWorldNodeServiceEntityServer>(worldNodeRef.Id))
                {
                    var worldNodeServiceEntity = wrapper?.Get<IWorldNodeServiceEntityServer>(worldNodeRef.Id);
                    if (worldNodeServiceEntity == null)
                    {
                        Logger.IfError()?.Message("Error get WorldNodeServiceEntity {0} from wrapper", worldNodeRef.Id).Write();
                        continue;
                    }

                    if (worldNodeServiceEntity.ClientNode || !await worldNodeServiceEntity.IsReady())
                    {
                        continue;
                    }
                    
                    Logger.IfWarn()?.Message($"check wnse {worldNodeServiceEntity.State}").Write();
                    if (worldNodeServiceEntity.State == WorldNodeServiceState.Empty || worldNodeServiceEntity.State == WorldNodeServiceState.Loaded)
                    {
                        ChunkQueueElement element = null;
                        if (!_mapChunkWaitQueue.TryDequeue(out element))
                            return;
                        if ((worldNodeServiceEntity.State == WorldNodeServiceState.Loaded && (worldNodeServiceEntity.MapInstanceId != Guid.Empty || worldNodeServiceEntity.Map != element.ChunkDef)))
                        {
                            _mapChunkWaitQueue.Enqueue(element);
                            continue;
                        }
                        worldNodeServiceEntity.SubscribePropertyChanged(nameof(IWorldNodeServiceEntity.State), OnStateChanged);
                        Logger.IfWarn()?.Message($"Request host unity map chunk {element.ChunkDef.____GetDebugShortName()}").Write();
                        var result = await worldNodeServiceEntity.HostUnityMapChunk(element.ChunkDef, element.WorldSpaceId, element.MapInstanceId, element.MapInstanceRepositoryId);
                        if (result)
                        {
                            using (var wrapper2 = await EntitiesRepository.Get<IMapEntityServer>(element.MapInstanceId))
                            {
                                var mapEntity = wrapper2.Get<IMapEntityServer>(element.MapInstanceId);
                                var result2 = await mapEntity.ChangeChunkDescription(element.WorldSpaceId, MapChunkState.Loading, worldNodeServiceEntity.Id);
                            }
                        }
                        else
                        {
                            worldNodeServiceEntity.UnsubscribePropertyChanged(nameof(IWorldNodeServiceEntity.State), OnStateChanged);
                            Logger.IfError()?.Message("WorldNodeServiceEntity {0} cant host map {1} instanceId {2}", worldNodeServiceEntity.Id, element.ChunkDef, element.MapInstanceId).Write();
                            _mapChunkWaitQueue.Enqueue(element);
                        }
                    }

                    if (_mapChunkWaitQueue.IsEmpty)
                        return;
                }
            }
        }

        async Task OnStateChanged(EntityEventArgs args)
        {
            var state = (WorldNodeServiceState)args.NewValue;
            var worldNodeId = args.Sender.ParentEntityId;
            Logger.IfWarn()?.Message($"OnStateChanged {worldNodeId} {state}").Write();
            _ = AsyncUtils.RunAsyncTask(async () =>
              {
                  if (state == WorldNodeServiceState.Loaded)
                  {
                      Guid worldSpaceId;
                      Guid mapId;
                      using (var wrapper = await EntitiesRepository.Get<IWorldNodeServiceEntityServer>(worldNodeId))
                      {
                          var worldNode = wrapper.Get<IWorldNodeServiceEntityServer>(worldNodeId);
                          worldSpaceId = worldNode.MapChunkId;
                          mapId = worldNode.MapInstanceId;
                      }
                      using (var wrapper = await EntitiesRepository.Get<IMapEntityServer>(mapId))
                      {
                          var mapEntity = wrapper.Get<IMapEntityServer>(mapId);
                          Logger.IfWarn()?.Message($"Change chunk description {worldSpaceId} {MapChunkState.Loaded}").Write();
                          var result = await mapEntity.ChangeChunkDescription(worldSpaceId, MapChunkState.Loaded, worldNodeId);
                          if (result == false)
                              Logger.IfError()?.Message("Error ChangeChunkDescription chunk {0} id {1} mapId {2}", mapEntity.Map.____GetDebugShortName(), worldSpaceId, mapId).Write();
                      }
                  }
              });

        }
        public Task<Guid> GetMapIdByUserIdImpl(Guid userId)
        {
            MapElement mapElement;
            _mapPerUser.TryGetValue(userId, out mapElement);
            return Task.FromResult(mapElement.MapId);
        }
    }
}
