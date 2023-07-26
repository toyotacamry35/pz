using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Assets.ColonyShared.GeneratedCode.Regions;
using Assets.ColonyShared.GeneratedCode.Shared;
using Assets.ColonyShared.SharedCode.Entities;
using Assets.ColonyShared.SharedCode.Utils;
using Assets.ResourceSystem.Aspects.Links;
using Assets.Src.Arithmetic;
using Assets.Src.Aspects.Doings;
using ResourcesSystem.Loader;
using Assets.Src.RubiconAI;
using ColonyShared.GeneratedCode.Shared.Aspects;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using GeneratedCode.Custom.Config;
using GeneratedCode.DatabaseUtils;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedCode.Manual.Repositories;
using GeneratedCode.MapSystem;
using GeneratedCode.Repositories;
using GeneratedCode.Transactions;
using JetBrains.Annotations;
using NLog;
using ResourceSystem.Utils;
using Scripting;
using SharedCode.Aspects.Item.Templates;
using SharedCode.DeltaObjects.Building;
using SharedCode.Entities;
using SharedCode.Entities.Building;
using SharedCode.Entities.Cloud;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.Entities.Service;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.EntityPropertyResolvers;
using SharedCode.MovementSync;
using SharedCode.Repositories;
using SharedCode.Serializers;
using SharedCode.Utils;
using SharedCode.EntitySystem.Delta;

namespace GeneratedCode.DeltaObjects
{
    public partial class WorldSpaceServiceEntity : IHookOnInit, IHookOnStart, IHookOnDestroy
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        private static int ___interactiveEntityTypeId = InteractiveEntity.StaticTypeId;
        private static int ___mineableEntityTypeId = MineableEntity.StaticTypeId;
        bool ShouldBeReplicatedToStreamingRepository(int typeId) => !(typeId == ___interactiveEntityTypeId || typeId == ___mineableEntityTypeId);
        ConcurrentDictionary<Guid, ConcurrentDictionary<ValueTuple<int, Guid>, bool>> _characterIdByUserId = new ConcurrentDictionary<Guid, ConcurrentDictionary<ValueTuple<int, Guid>, bool>>();
        ConcurrentDictionary<ValueTuple<int, Guid>, Guid> _userIdByCharacterId = new ConcurrentDictionary<ValueTuple<int, Guid>, Guid>();
        ConcurrentDictionary<EntityId, bool> _worldObjects = new ConcurrentDictionary<EntityId, bool>();
        ConcurrentDictionary<Guid, bool> _streamingRepos = new ConcurrentDictionary<Guid, bool>();
        private SpawnPointData[] _spawnPoints;
        private object __lock__ = new object();
        private int _currentUsersCount = 0;

        private int _worldCharacterTypeId;
        private int _fencePlaceTypeId;

        private bool _debugOutputForFencePlaces = false;

        private readonly ConcurrentBag<Guid> _worldChars = new ConcurrentBag<Guid>();
        private readonly ConcurrentQueue<ValueTuple<OuterRef, bool>> _worldObjectOperations = new ConcurrentQueue<(OuterRef, bool)>();


        private static ThreadLocal<Random> _random = new ThreadLocal<Random>(() => new Random());


        protected override void constructor()
        {
            base.constructor();

            AllUsersAndTheirCharacters = _characterIdByUserId;
            _worldCharacterTypeId = WorldCharacter.StaticTypeId;
            _fencePlaceTypeId = FencePlace.StaticTypeId;
        }
        AIWorld world;
        Guid _visibilityGuid;
        public Task OnInit()
        {
            world = new AIWorld(EntitiesRepository, false, AIWorldMode.Mob);
            world.Register(Id);
            var id = Id;
            Logger.IfDebug()?.Message(".............#Dbg (not Warn): [WSSE].OnInit. Cre `VisibilityEntity` with WorldSpace==" + Id).Write();
            _visibilityGuid = Guid.NewGuid();
            return EntitiesRepository.Create<IVisibilityEntity>(_visibilityGuid, async (ve) => { ve.WorldSpace = id; });
        }

        public Task OnStart()
        {
            EntitiesRepository.UserDisconnected += EntitiesRepository_UserDisconnected;
            return Task.CompletedTask;
        }

        public async Task OnDestroy()
        {
            if (_sceneEntityIdForRegions != default)
                RegionBuildHelper.RemoveRootRegionWithGuid(_sceneEntityIdForRegions);
            EntitiesRepository.UserDisconnected -= EntitiesRepository_UserDisconnected;
            await EntitiesRepository.Destroy(VisibilityEntity.StaticTypeId, _visibilityGuid);
        }

        private async Task EntitiesRepository_UserDisconnected(Guid repoId)
        {
            Logger.IfInfo()?.Message("User disconnected {0}", repoId).Write();
            if (_streamingRepos.ContainsKey(repoId))
            {
                using (var wswrap = await EntitiesRepository.Get<IWorldSpaceServiceEntity>(Id))
                {
                    await wswrap.Get<IWorldSpaceServiceEntity>(Id).DisconnectStreamingRepo(repoId);
                }
            }
            else if (_characterIdByUserId.ContainsKey(repoId))
                using (var wswrap = await EntitiesRepository.Get<IWorldSpaceServiceEntity>(Id))
                {
                    await wswrap.Get<IWorldSpaceServiceEntity>(Id).RemoveClient(repoId, false);
                }
        }

        public async Task<bool> AddWorldObjectImpl(int typeId, Guid entityId)
        {
            if (_worldObjects.TryAdd(new EntityId(typeId, entityId), true))
            {
                if (ShouldBeReplicatedToStreamingRepository(typeId))
                    foreach (var streamingRepo in _streamingRepos)
                    {
                        await EntitiesRepository.SubscribeReplication(typeId, entityId, streamingRepo.Key, ReplicationLevel.Always);
                    }
            }

            _worldObjectOperations.Enqueue((new OuterRef(entityId, typeId), true));
            checkProcessWorldObjectOperations();

            if (typeId == _fencePlaceTypeId)
            {
                using(await this.GetThisRead())
                    return await RegisterFencePlace(new OuterRef<IFencePlace>(entityId, typeId));
            }

            return true;
        }

        private SuspendingAwaitable _processWorldObjectOperationsTask;

        private void checkProcessWorldObjectOperations()
        {
            if (_processWorldObjectOperationsTask == default || _processWorldObjectOperationsTask.IsCompleted ||
                _processWorldObjectOperationsTask.IsFaulted)
            {
                _processWorldObjectOperationsTask = AsyncUtils.RunAsyncTask(ProcessWorldObjectOperations);
            }
        }

        private async Task ProcessWorldObjectOperations()
        {
            try
            {
                int count = 0;
                while (_worldObjectOperations.Any())
                {
                    count = 0;
                    using (var wrapper = await this.GetThisRead())
                    {
                        while (_worldObjectOperations.TryDequeue(out var tuple))
                        {
                            if (tuple.Item2)
                                await WorldObjectsInformationSetsMapEngine.AddWorldObject(tuple.Item1);
                            else
                                await WorldObjectsInformationSetsMapEngine.RemoveWorldObject(tuple.Item1);

                            count++;
                            if (count > 500)
                                break;
                        }
                    }

                    await Task.Delay(TimeSpan.FromSeconds(0.1));
                }
            }
            finally
            {
                _processWorldObjectOperationsTask = default;
            }
        }

        public async Task<bool> RemoveWorldObjectImpl(int typeId, Guid entityId)
        {
            bool unused;
            if (_worldObjects.TryRemove(new EntityId(typeId, entityId), out unused))
            {
                if (ShouldBeReplicatedToStreamingRepository(typeId))
                    foreach (var streamingRepo in _streamingRepos)
                    {
                        await EntitiesRepository.UnsubscribeReplication(typeId, entityId, streamingRepo.Key, ReplicationLevel.Always);
                    }
            }

            _worldObjectOperations.Enqueue((new OuterRef(entityId, typeId), false));
            checkProcessWorldObjectOperations();

            if (typeId == _worldCharacterTypeId)
            {
                Guid userId;
                if (!_userIdByCharacterId.TryGetValue(new ValueTuple<int, Guid>(typeId, entityId), out userId))
                    return true;
                await EntitiesRepository.UnsubscribeReplication(typeId, entityId, userId, ReplicationLevel.ClientFull);
            }
            else if (typeId == _fencePlaceTypeId)
            {
                return await UnregisterFencePlace(new OuterRef<IFencePlace>(entityId, typeId));
            }
            return true;
        }

        public Task<bool> UpdateTransformImpl(int typeId, Guid entityId)
        {
            return Task.FromResult(true);
        }

        public async Task<AddClientResult> AddClientImpl(Guid characterId, Guid repositoryId)
        {
            Logger.IfWarn()?.Message($"Add Client {characterId} {repositoryId}").Write();
            ConcurrentDictionary<ValueTuple<int, Guid>, bool> list;
            if (_characterIdByUserId.TryGetValue(repositoryId, out list))
            {
                if (list.ContainsKey(new ValueTuple<int, Guid>(_worldCharacterTypeId, characterId)))
                {
                    Logger.IfWarn()?.Message("User {0} character {1} already logined in worldSpace with same character Id", repositoryId, characterId).Write();
                    return AddClientResult.AlreadyExist;
                }
            }

            var characterPair = new ValueTuple<int, Guid>(_worldCharacterTypeId, characterId);
            _userIdByCharacterId[characterPair] = repositoryId;

            if (_characterIdByUserId.ContainsKey(repositoryId))
                _characterIdByUserId[repositoryId].TryAdd(characterPair, true);
            else
            {
                Logger.IfWarn()?.Message($"Create set {characterId} {repositoryId}").Write();
                var repositoryHashSet = new ConcurrentDictionary<ValueTuple<int, Guid>, bool>();
                repositoryHashSet.TryAdd(characterPair, true);
                _characterIdByUserId.TryAdd(repositoryId, repositoryHashSet);
                using (var charWrapper = await EntitiesRepository.Get<IWorldCharacterServer>(characterPair.Item2))
                {
                    var charEntity = charWrapper.Get<IWorldCharacterServer>(characterPair.Item2);
                    await charEntity.NotifyThatClientIsBack();
                }
            }

            Interlocked.Increment(ref _currentUsersCount);
            Logger.IfInfo()?.Message("User {0} character {1} added to world space. Users count {2}", repositoryId, characterId, _currentUsersCount).Write();

            await EntitiesRepository.SubscribeReplication(_worldCharacterTypeId, characterId, repositoryId, ReplicationLevel.ClientFull);
            return AddClientResult.Added;
        }
        public async Task<bool> RemoveClientImpl(Guid repositoryId, bool immediate)
        {
            Logger.IfInfo()?.Message($"Remove client {repositoryId}").Write();
            ConcurrentDictionary<ValueTuple<int, Guid>, bool> characterPairHashSet;
            if (!_characterIdByUserId.TryRemove(repositoryId, out characterPairHashSet))
            {
                Logger.IfWarn()?.Message("WorldSpaceServiceEntity {0} not contains user {1} ", this.Id, repositoryId).Write();
                return false;
            }

            foreach (var pair in _userIdByCharacterId.Where(x => x.Value == repositoryId))
            {
                Logger.IfWarn()?.Message($"found pair {repositoryId}").Write();
                Guid characterVisibilityInfo;
                if (pair.Key.Item2 != Guid.Empty)
                    _userIdByCharacterId.TryRemove(pair.Key, out characterVisibilityInfo);
                using (var charWrapper = await EntitiesRepository.Get<IWorldCharacterServer>(pair.Key.Item2))
                {
                    var charEntity = charWrapper.Get<IWorldCharacterServer>(pair.Key.Item2);
                    Logger.IfWarn()?.Message($"Notify that client is gone {repositoryId} {pair.Key.Item2}").Write();
                    if (!immediate && charEntity != null)
                        await charEntity.NotifyThatClientIsGone();
                }
                if (immediate)
                    await EntitiesRepository.Destroy(pair.Key.Item1, pair.Key.Item2, true);
            }

            foreach (var characterPair in characterPairHashSet)
            {
                Guid characterVisibilityInfo;
                _userIdByCharacterId.TryRemove(characterPair.Key, out characterVisibilityInfo);

                Interlocked.Decrement(ref _currentUsersCount);
                Logger.IfInfo()?.Message("User {0} character {1} removed from world space. Users count {2}", repositoryId, characterPair.Value, _currentUsersCount).Write();
                await EntitiesRepository.UnsubscribeReplication(characterPair.Key.Item1, characterPair.Key.Item2, repositoryId, ReplicationLevel.ClientFull);
            }

            using (var wrapper2 = await EntitiesRepository.GetFirstService<IBotCoordinatorServer>())
            {
                if (wrapper2 == null)
                    Logger.IfError()?.Message("IBotCoordinator not found {0}", EntitiesRepository.Id).Write();

                var service = wrapper2?.GetFirstService<IBotCoordinatorServer>();
                if (service == null)
                    Logger.IfWarn()?.Message("IBotCoordinator not found {0}", EntitiesRepository.Id).Write();
                else
                    await service.DeactivateBots(repositoryId);
            }
            if (_characterIdByUserId.Count == 0)
                using (var wmap = await EntitiesRepository.Get<IMapEntityServer>(OwnMap.Guid))
                {
                    var map = wmap.Get<IMapEntityServer>(OwnMap.Guid);
                    await map.OnLastUserLeft();
                }

            return true;
        }

        public async Task<Guid> GetWorldBoxIdToDropImpl(Vector3 position, Guid characterOwnerId)
        {
            return await ClusterCommands.GetWorldBoxIdToDrop(position, characterOwnerId, Id, EntitiesRepository);
        }

        public async Task<bool> RegisterFencePlaceImpl(OuterRef<IFencePlace> fencePlace)
        {
            using (var fencePlaceWrapper = await EntitiesRepository.Get<IFencePlace>(fencePlace.Guid))
            {
                var fencePlaceEntity = fencePlaceWrapper.Get<IFencePlace>(fencePlace.Guid);
                if (fencePlaceEntity != null)
                {
                    var fencePlaceKey = BuildUtils.GetFencePlaceKey(fencePlaceEntity.MovementSync.Position);
                    if (!FencePlaces.ContainsKey(fencePlaceKey)) // can already register from CreateFencePlaceId
                    {
                        FencePlaces.Add(fencePlaceKey, new OuterRef<IFencePlace>(fencePlaceEntity));
                        if (_debugOutputForFencePlaces)
                        {
                            Logger.IfError()?.Message($"WORLD SPACE SERVICE: {Id}, RegisterFencePlaceImp(): FencePlaces.Add([{fencePlaceKey.x}, {fencePlaceKey.y}]), {fencePlaceEntity.Id}").Write();
                        }
                    }
                }
            }
            return true;
        }

        public Task<Guid?> GetWorldNodeIdImpl(OuterRef entityRef)
        {
            if (_userIdByCharacterId.TryGetValue((entityRef.TypeId, entityRef.Guid), out var userId))
            {
                return Task.FromResult((Guid?)userId);
            }

            var worldNodes =
                ((EntitiesRepository)EntitiesRepository).GetEntitiesCollection(typeof(IWorldNodeServiceEntity));
            var worldNodesCount = worldNodes.Count;

            Guid? selectedNodeId = null;
            if (worldNodesCount > 0)
            {
                int tryCount = 0;
                // there is a possibility that dictionary become smaller
                while (selectedNodeId == null && tryCount < 3)
                {
                    var selectedIndex = _random.Value.Next(worldNodesCount);
                    var currentIndex = 0;
                    foreach (var worldNode in worldNodes)
                    {
                        if (selectedIndex == currentIndex)
                        {
                            selectedNodeId = worldNode.Key;
                        }

                        currentIndex++;
                    }

                    tryCount++;
                }
            }

            return Task.FromResult(selectedNodeId);
        }

        public async Task<bool> UnregisterFencePlaceImpl(OuterRef<IFencePlace> fencePlace)
        {
            using (var fencePlaceWrapper = await EntitiesRepository.Get<IFencePlace>(fencePlace.Guid))
            {
                var fencePlaceEntity = fencePlaceWrapper.Get<IFencePlace>(fencePlace.Guid);
                if (fencePlaceEntity != null)
                {
                    var fencePlaceKey = BuildUtils.GetFencePlaceKey(fencePlaceEntity.MovementSync.Position);
                    FencePlaces.Remove(fencePlaceKey);
                    if (_debugOutputForFencePlaces)
                    {
                        Logger.IfError()?.Message($"WORLD SPACE SERVICE: {Id}, UnregisterFencePlaceImpl(): FencePlaces.Remove([{fencePlaceKey.x}, {fencePlaceKey.y}]), {fencePlaceEntity.Id}").Write();
                    }
                }
            }
            return true;
        }

        public async Task<Guid> CreateFencePlaceIdImpl(Vector3 position)
        {
            var fencePlaceKey = BuildUtils.GetFencePlaceKey(position);
            var fencePlaceRef = await EntitiesRepository.Create<IFencePlace>(Guid.NewGuid(), (entity) =>
            {
                //TODO building: move fence place def to game designer constants
                entity.Def = BuildUtils.DefaultFencePlaceDef;
                entity.MovementSync.SetPosition = BuildUtils.GetFencePlacePosition(fencePlaceKey, position.y);
                entity.State = BuildState.Completed;
                entity.MapOwner = new MapOwner(OwnMap.Guid, OwnMap.Guid);
                entity.WorldSpaced.OwnWorldSpace = new OuterRef<IWorldSpaceServiceEntity>(this);
                return Task.CompletedTask;
            });
            FencePlaces.Add(fencePlaceKey, new OuterRef<IFencePlace>(fencePlaceRef)); // avoid double register
            if (_debugOutputForFencePlaces)
            {
                Logger.IfError()?.Message($"WORLD SPACE SERVICE: {Id}, CreateFencePlaceIdImpl(): FencePlaces.Add([{fencePlaceKey.x}, {fencePlaceKey.y}]), {fencePlaceRef.Id}").Write();
            }
            using (var wrapper = await EntitiesRepository.Get<IFencePlace>(fencePlaceRef.Id))
            {
                var fence = wrapper.Get<IFencePlace>(fencePlaceRef.Id);
                if (fence != null)
                {
                    var result = await fence.WorldSpaced.AssignToWorldSpace(new OuterRef<IWorldSpaceServiceEntity>(this));
                }
            }
            return fencePlaceRef.Id;
        }

        public async Task<Guid> GetFencePlaceIdImpl(Vector3 position, bool onlyExisted)
        {
            var fencePlaceKey = BuildUtils.GetFencePlaceKey(position);
            OuterRef<IFencePlace> fencePlaceRef;
            if (!FencePlaces.TryGetValue(fencePlaceKey, out fencePlaceRef))
            {
                if (_debugOutputForFencePlaces)
                {
                    Logger.IfError()?.Message($"WORLD SPACE SERVICE: {Id}, GetFencePlaceIdImpl(): FALSE, FencePlaces.TryGetValue([{fencePlaceKey.x}, {fencePlaceKey.y}]), FencePlaces.Count : {FencePlaces.Count}").Write();
                }
                if (onlyExisted)
                {
                    return Guid.Empty;
                }
                var fenceId = await CreateFencePlaceId(position);
                return fenceId;
            }
            else if (_debugOutputForFencePlaces)
            {
                Logger.IfError()?.Message($"WORLD SPACE SERVICE: {Id}, GetFencePlaceIdImpl(): TRUE, FencePlaces.TryGetValue([{fencePlaceKey.x}, {fencePlaceKey.y}]), FencePlaces.Count : {FencePlaces.Count}").Write();
            }
            return fencePlaceRef.Guid;
        }

        public Task<int> GetCCUImpl()
        {
            return Task.FromResult(_currentUsersCount);
        }

        public async Task OnVisibilityChangedImpl(int subjectTypeId, Guid subjectEntityId, List<ValueTuple<int, Guid>> addedObjects, List<ValueTuple<int, Guid>> removedObjects)
        {

        }

        public Task EnableReplicationsImpl(int subjectTypeId, Guid subjectEntityId, bool enable)
        {

            return Task.CompletedTask;
        }

        private async Task<AccountData> ResolveAccountByUserId(Guid user)
        {
            using (var wrapper = await EntitiesRepository.GetFirstService<ILoginInternalServiceEntityServer>())
            {
                if (wrapper == null)
                {
                    Logger.IfError()?.Message("ILoginInternalServiceEntity not found {0}", EntitiesRepository.Id).Write();
                    return new AccountData { AccountId = Guid.Empty };
                }

                var loginInternalService = wrapper.GetFirstService<ILoginInternalServiceEntityServer>();
                var accountData = await loginInternalService.GetAccountDataByUserId(user);
                Logger.IfInfo()?.Message("GetEntityIdByUserId return account userId {0} accountName {1}", accountData.AccountId, accountData.AccountName).Write();
                return accountData;
            }
        }

        public async Task SpawnNewBotImpl(string spawnPointPath, List<Guid> botIds, Guid userId)
        {
            using (var wrapper2 = await EntitiesRepository.GetFirstService<IBotCoordinatorServer>())
            {
                if (wrapper2 == null)
                    Logger.IfError()?.Message("IBotCoordinator not found {0}", EntitiesRepository.Id).Write();

                var service = wrapper2?.GetFirstService<IBotCoordinatorServer>();
                await service.ActivateBots(userId, botIds);
            }

            var accountData = await ResolveAccountByUserId(userId);
            foreach (var botId in botIds)
            {
                var charData = new CharacterData()
                {
                    CharacterId = botId,
                    CharacterName = $"Bot {botId} from {accountData.AccountName}"
                };
                await SpawnClusterEntityIfAbsent(charData, null, spawnPointPath, new MapOwner(OwnMap.Guid, OwnMap.Guid), default);
                //#order! 2
                if (await AddClient(botId, userId) == AddClientResult.Error)
                {
                    Logger.IfInfo()?.Message("Error adding bot to worldSpace user with {0} characterId {1}", userId, botId).Write();
                    continue;
                }

                Logger.IfInfo()?.Message("Login bot added to worldSpace user with {0} characterId {1}", userId, botId).Write();
            }
        }

        private async Task<(CharacterData charData, AccountData accountData)> ResolveCharacterByUserId(Guid user)
        {
            using (var wrapper = await EntitiesRepository.GetFirstService<ILoginInternalServiceEntityServer>())
            {
                if (wrapper == null)
                {
                    Logger.IfError()?.Message("ILoginInternalServiceEntity not found {0}", EntitiesRepository.Id).Write();
                    return (null, null);
                }

                var loginInternalService = wrapper.GetFirstService<ILoginInternalServiceEntityServer>();
                var characterEntityId = await loginInternalService.GetCharacterDataByUserId(user);
                var accountData = await loginInternalService.GetAccountDataByUserId(user);
                Logger.IfInfo()?.Message("GetEntityIdByUserId return characterId userId {0}", characterEntityId).Write();
                return (characterEntityId, accountData);
            }
        }
        public async Task<bool> LogoutAllImpl()
        {
            foreach (var uid in _characterIdByUserId.Keys)
            {
                if (_characterIdByUserId.TryGetValue(uid, out var chars))
                {
                    foreach (var c in chars)
                        await EntitiesRepository.Destroy(WorldCharacter.StaticTypeId, c.Key.Item2, true);
                    await EntitiesRepository.TryGetLockfree<IVisibilityEntity>(_visibilityGuid, ReplicationLevel.Master).ForceUnsubscribeAll(uid);
                }
            }
            return true;
        }

        public async Task<bool> LogoutImpl(Guid userId, bool terminal)
        {
            if (_characterIdByUserId.TryGetValue(userId, out var chars))
            {
                await EntitiesRepository.TryGetLockfree<IVisibilityEntity>(_visibilityGuid, ReplicationLevel.Master).ForceUnsubscribeAll(userId);
                await RemoveClient(userId, !terminal);
            }
            return true;
        }
        public async Task<AddClientResult> LoginImpl(BotActionDef botDef, string spawnPointPath, Guid userRepository, MapOwner mapOwner)
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"[WNSE].LoginImpl").Write();

            Logger.IfInfo()?.Message("Try login user with {0}", userRepository).Write();
            (var userData, var accountData) = await ResolveCharacterByUserId(userRepository);

            if (userData == null || accountData == null)
            {
                Logger.IfError()?.Message("GetEntityIdByUserId return empty characterId userId {0}", userRepository).Write();
                return AddClientResult.Error;
            }

            Logger.IfInfo()?.Message("Login try add to worldSpace user with {0} characterId {1}, account name {2}", userRepository, userData, accountData.AccountName).Write();

            //#order! 1
            await SpawnClusterEntityIfAbsent(userData, botDef, spawnPointPath, mapOwner, userRepository);
            var result = await AddClient(userData.CharacterId, userRepository);
            if (result == AddClientResult.Error)
                Logger.IfError()?.Message("Login added to worldSpace user error. with {0} characterId {1}", userRepository, userData).Write();
            return result;
        }

        #region Spawn_char_pawn
        ConcurrentDictionary<Guid, ScenicEntityDef> _bakens = new ConcurrentDictionary<Guid, ScenicEntityDef>();
        public void PrepareCommonBakens(MapDef map)
        {
            foreach (var scenePath in map.AllScenesExportedJdbsPaths)
            {
                if (GameResourcesHolder.Instance.IsResourceExists(scenePath))
                {
                    var scene = GameResourcesHolder.Instance.LoadResource<SceneChunkDef>(scenePath);
                    foreach (var entity in scene.Entities.Where(x => x.Target != null && x.Target.Object.Target != null))
                        if (entity.Target.Object.Target.GetType() == typeof(WorldBakenDef))
                            _bakens.TryAdd(entity.Target.RefId, entity.Target);
                }
            }
        }

        public PositionRotation GetRespawnPointFromCommonBakens(Guid bakenId, MapDef map)
        {
            PrepareCommonBakens(map);
            if (!_bakens.TryGetValue(bakenId, out var bakenSceneEnt))
                return PositionRotation.InvalidInstatnce;
            return new PositionRotation(bakenSceneEnt.Position + new Vector3(0, 0.3f, 0), bakenSceneEnt.Rotation);
        }
        public async Task<PositionRotation> AcquireFirstReadyBakenPositionToSpawn(Guid charId, bool anyCommonBaken, Guid commonBakenId)
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"[WNSE].AcquireFirstReadyBakenPositionToSpawn ({charId})").Write();

            using (var wrapper = await EntitiesRepository.Get<IWorldCharacterClientFull>(charId))
            {
                var @char = wrapper?.Get<IWorldCharacterClientFull>(charId);
                if (@char == null)
                    return PositionRotation.InvalidInstatnce;

                var activatedBaken = default(OuterRef<IEntity>);
                if (!@char.LastActivatedWasCommonBaken)
                {
                    using (var wrapper2 = await EntitiesRepository.GetFirstService<IBakenCoordinatorServiceEntityServer>())
                    {
                        var bakenCoordinator = wrapper2.GetFirstService<IBakenCoordinatorServiceEntityServer>();
                        activatedBaken = await bakenCoordinator.GetActiveBaken(charId);
                    }
                    if (activatedBaken == default(OuterRef<IEntity>))
                    {
                        Logger.IfError()?.Message($"No active bakens for char id {charId} - aborting spawn").Write();
                    }
                }
                else
                if (activatedBaken == default(OuterRef<IEntity>))
                {
                    if (anyCommonBaken)
                    {
                        if (commonBakenId == default)
                            commonBakenId = @char.ActivatedCommonBakens.FirstOrDefault().Key;
                        if (commonBakenId != default)
                        {
                            using (var wrapper2 = await EntitiesRepository.Get<IMapEntityServer>(OwnMap.Guid))
                            {
                                var mapEntity = wrapper2.Get<IMapEntityServer>(OwnMap.Guid);
                                var map = mapEntity.Map;
                                return GetRespawnPointFromCommonBakens(commonBakenId, map);
                            }
                        }
                    }
                    Logger.IfError()?.Message($"No active bakens for char id {charId} - aborting spawn").Write();
                    return PositionRotation.InvalidInstatnce;
                }

                using (var wrapperBaken = await EntitiesRepository.Get<IWorldBakenClientBroadcast>(activatedBaken.Guid))
                {
                    var baken = wrapperBaken.Get<IWorldBakenClientBroadcast>(activatedBaken.Guid);
                    if (baken != null)
                    {
                        await baken.SetCooldown();
                        var trans = baken.MovementSync.Transform;
                        var verticalDistance = await baken.GetVerticalSpawnPointDistance();
                        return new PositionRotation(trans.Position
                                                      + new Vector3(0, verticalDistance, 0)
                                                    , trans.Rotation);
                    }
                }

                //Logger.IfInfo()?.Message($"All bakens (N=={@char.Bakens.Count}) cooldown is not expired yet or not exist.").Write();
                return PositionRotation.InvalidInstatnce;
            }
        }

        // --- Spawn_char_pawn: -------------------------------------------------------------
        #endregion Spawn_char_pawn

        public async Task<bool> RespawnImpl(Guid charId, bool checkBakens, bool anyCommonBaken, Guid commonBakenId)
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"[WNSE].RespawnImpl ({charId}, checkBakens: {checkBakens})").Write();

            await DoOnRespawn(charId, checkBakens, anyCommonBaken, commonBakenId);
            return true;
        }

        private async Task DoOnRespawn(Guid charId, bool checkBakens, bool anyCommonBaken, Guid commonBakenId)
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"[WNSE].DoOnRespawn ({charId}, checkBakens: {checkBakens})").Write();

            var repo = EntitiesRepository;
            using (var wrapper = await repo.Get<IWorldCharacterServer>(charId))
            {
                var character = wrapper?.Get<IWorldCharacter>(charId);
                if (character == null)
                {
                    Logger.IfError()?.Message($"Can't get {nameof(IWorldCharacterServer)}").Write();
                    return;
                }

                var spawnPointPosition = await GetPositionToSpawnAtImpl(charId, checkBakens, anyCommonBaken, commonBakenId);
                character.MovementSync.SetTransform = new Transform(spawnPointPosition.Position, spawnPointPosition.Rotation);
                await character.Mortal.Resurrect(spawnPointPosition);
            }
        }

        private async Task<bool> HasBaken(Guid charId)
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"[WNSE].HasBakenImpl ({charId})").Write();
            using (var cw = await EntitiesRepository.Get<IWorldCharacterServer>(charId))
            {
                var entity = cw.Get<IWorldCharacterServer>(charId);
                if (entity != null)
                    if (entity.ActivatedCommonBakens.Count > 0)
                        return true;
            }
            IBakenCoordinatorServiceEntityServer lockFreeBC = null;
            using (var wrapper = await EntitiesRepository.GetFirstService<IBakenCoordinatorServiceEntityServer>())
            {
                lockFreeBC = wrapper.GetFirstService<IBakenCoordinatorServiceEntityServer>();
            }
            return (await lockFreeBC.GetActiveBaken(charId)) != default;
        }

        public async Task<PositionRotation> GetPositionToSpawnAtImpl(Guid charId, bool checkBakens, bool anyCommonBaken, Guid commonBakenId, SpawnPointTypeDef overrideAllowedPointType = null)
        {
            PositionRotation spawnPointPosition = PositionRotation.InvalidInstatnce;
            var hasBaken = checkBakens && await HasBaken(charId);
            if (hasBaken)
                spawnPointPosition = await AcquireFirstReadyBakenPositionToSpawn(charId, anyCommonBaken, commonBakenId);

            if (!spawnPointPosition.IsValid)
            {
                var allowedSpawnPointType = overrideAllowedPointType ?? await GetAllowedSpawnPointType(charId);
                spawnPointPosition = await Get1stSuitedSpawnPointPosition(allowedSpawnPointType);
            }

            return spawnPointPosition;
        }

        public async Task<SpawnPointTypeDef> GetAllowedSpawnPointType(Guid charId)
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"[WNSE].GetAllowedSpawnPointType ({charId})").Write();

            var repo = EntitiesRepository;
            using (var wrapper = await repo.Get<IWorldCharacterClientFull>(charId))
            {
                var worldCharacterClientFull = wrapper?.Get<IWorldCharacterClientFull>(charId);
                if (worldCharacterClientFull == null)
                {
                    Logger.IfError()?.Message($"Can't get {nameof(IWorldCharacterClientFull)}").Write();
                    return null;
                }

                return worldCharacterClientFull.AllowedSpawnPoint;
            }
        }

        /// -----------------------------------------------------------------------------------------------------------------------------------

        private async Task SpawnClusterEntityIfAbsent(CharacterData charData, [CanBeNull]BotActionDef botDef, string spawnPointTypePath, MapOwner mapOwner, Guid userId) //Spawns Plyr entty:
        {
            MapDef map;
            using (var wrapper = await EntitiesRepository.Get<IMapEntityServer>(this.OwnMap.Guid))
            {
                var mapEntity = wrapper?.Get<IMapEntityServer>(this.OwnMap.Guid);
                if (mapEntity == null)
                {
                    Logger.IfError()?.Message($"Can't get {nameof(IMapEntityServer)}.").Write();
                    return;
                }
                map = mapEntity.Map;
            }

            await CheckInitSpawnPoints();

            var charId = charData.CharacterId;
            spawnPointTypePath = "/SpawnSystem/SpawnPointTypes/Dropzone";//пока так, с клиента теперь null приходит

            bool useSpawnPoint = false;
            Guid accountId = Guid.Empty;
            if (userId != default)
            {
                var realmId = EntitiesRepository.TryGetLockfree<IMapEntityServer>(mapOwner.OwnerMapId, ReplicationLevel.Server).RealmId;
                using (var w = await EntitiesRepository.GetFirstService<ILoginServiceEntityServer>())
                {
                    var loginService = w?.GetFirstService<ILoginServiceEntityServer>();
                    accountId = await loginService.GetAccountIdByUserId(userId);
                    if (await loginService.AssignAccountToMap(accountId, mapOwner.OwnerMapId))
                        useSpawnPoint = true;
                }
            }

            var allowedPointType = map.SpawnPoint.Target ?? GameResourcesHolder.Instance.LoadResource<SpawnPointTypeDef>(spawnPointTypePath);
            if (allowedPointType == default(SpawnPointTypeDef))
                Logger.IfError()?.Message($"Can not load spawn point type at specified path: {spawnPointTypePath}").Write();
            
            var spawnPointPosition = await GetPositionToSpawnAtImpl(
                 charId, true, true, default, allowedPointType);


            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"[WNSE].SpawnClusterEntityIfAbsent ({charId})").Write();
            var repo = EntitiesRepository;

            AccountStatsData accountDataForAccStats = default;
            if (accountId != Guid.Empty)
            {
                using (var w = await repo.GetFirstService<ILoginServiceEntityServer>())
                {
                    var loginService = w?.GetFirstService<ILoginServiceEntityServer>();
                    if (loginService != null)
                        accountDataForAccStats = await loginService.GetAccountDataForAccStats(accountId);
                    else
                        Logger.IfError()?.Message($"Can't get {nameof(ILoginServiceEntityServer)}").Write();
                }
            }
            else // else if (botDef != null) Не работает - для ботов тоже == null
            {
                Logger.IfError()?.Message($"accountId == Guid.Empty({accountId}). So can't get `accountDataForAccStats`. This should be happen only for bots & so acc.stats-data 'll be all by dflt: male gander & 0 exp, so 1st lvl.  [ It's Ok, if this happens for bot. ]").Write();
                accountDataForAccStats = new AccountStatsData() { Gender = Constants.CharacterConstants.DefaultGender };
            }

            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"AccountStatsData:{accountDataForAccStats}").Write();
            var dbId = await ((IEntitiesRepositoryDataExtension)repo).GetDataBaseServiceEntityid(ReplicaTypeRegistry.GetIdByType(typeof(IWorldCharacter)), charId);

            bool datasetExists = true;
            using (var wrapper = await repo.Get<IDatabaseServiceEntityServer>(dbId))
            {
                var dbService = wrapper.Get<IDatabaseServiceEntityServer>(dbId);
                if (dbService == null)    // <-- !
                {
                    Logger.IfError()?.Message("Failed to get database service to load character {character_id} for account {account_id}", charId, accountId).Write();
                    throw new Exception($"Failed to get database service to load character {charId} for account {accountId}");
                }
                datasetExists = await dbService.DataSetExists(ReplicaTypeRegistry.GetIdByType(typeof(IWorldCharacter)), charId);
            }

            if (datasetExists)
            {
                Logger.IfInfo()?.Message("Loading existing character with id {character_id} for account {account_id}", charId, accountId).Write();

                var loadedCharacter = await repo.Load<IWorldCharacter>(charId, async (lc) =>
                {
                    if (useSpawnPoint)
                    {
                        var spawnPointPositionOverride = await GetPositionToSpawnAtImpl(
                            charId, true, true, default, null);
                        if (spawnPointPositionOverride.Invalid)
                        {
                            spawnPointPositionOverride = spawnPointPosition;
                        }
                        if (spawnPointPositionOverride.IsValid)
                        {
                            lc.MovementSync.SetPosition = spawnPointPositionOverride.Position;
                        }
                    }
                    lc.MapOwner = mapOwner;
                    lc.WorldSpaced.OwnWorldSpace = new OuterRef<IWorldSpaceServiceEntity>(this);
                    lc.AccountId = accountId;
                    await lc.AccountStats.SetAccountStats(accountDataForAccStats);
                });

                if (loadedCharacter == null)
                {
                    Logger.IfFatal()?.Message("Failed to load existing character with id {character_id} for account {account_id}", charId, accountId).Write();
                    throw new Exception($"Failed to load existing character {charId} for account {accountId}");
                }

                var batch = EntityBatch.Create().Add<IWorldCharacterServer>(charId);
                var ownMap = OwnMap;
                var ownWS = new OuterRef<IWorldSpaceServiceEntity>(this);
                using (var wrapper = await repo.Get(batch))
                {
                    var characterServer = wrapper.Get<IWorldCharacterServer>(charId);
                    if (characterServer == null)    // <-- !
                    {
                        Logger.IfError()?.Message("Failed to get existing character with id {character_id} for account {account_id}", charId, accountId).Write();
                        throw new Exception($"Failed to get existing character {charId} for account {accountId}");
                    }

                    // A. Entity already exist
                    if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"{nameof(IWorldCharacterServer)} entity with id {charId} already exist. So do nothing.").Write();
                    if (await characterServer.Mortal.GetIsAlive() == false)
                    {
                        if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"[WNSE].SpawnClusterEntityIfAbsent({charId}). !IsAlive, so call `Respawn` here. - - - - * - - - - - - * - - - - - ").Write();
                        await Respawn(charId, true, true, default);
                    }
                    WorldCharacter.LogEnteredWorld(characterServer);
                }

                return;
            }

            Logger.IfInfo()?.Message("Creating new character with id {character_id} for account {account_id}", charId, accountId).Write();
            // B. No entity by this id. So it's 1st connect
            var defaultCharacterDef = accountDataForAccStats.Gender.DefaultCharacter.Target;
            var playerPawnDef = Constants.CharacterConstants.WorldCharacter.Target;
            var characterRef = await repo.Create<IWorldCharacter>(charId, async createdCharacter =>     // <-- !
            {
                createdCharacter.MapOwner = mapOwner;
                createdCharacter.WorldSpaced.OwnWorldSpace = new OuterRef<IWorldSpaceServiceEntity>(this);
                createdCharacter.Doll.Size = defaultCharacterDef.DefaultDoll.Size;
                createdCharacter.Currency.Size = 100;
                createdCharacter.Inventory.Size = defaultCharacterDef.DefaultInventory.Size;
                createdCharacter.PermanentPerks.Size = defaultCharacterDef.DefaultPermanentPerks.Size;
                createdCharacter.TemporaryPerks.Size = defaultCharacterDef.DefaultTemporaryPerks.Size;
                createdCharacter.SavedPerks.Size = defaultCharacterDef.DefaultSavedPerks.Size;
                createdCharacter.MovementSync.SetTransform = new Transform(spawnPointPosition.Position, spawnPointPosition.Rotation);
                createdCharacter.Def = playerPawnDef;
                if (charData.Packs != null)
                    createdCharacter.FounderPack.Packs = new DeltaDictionary<string, bool>(charData.Packs.ToDictionary(x => x, x => true));
                createdCharacter.AllowedSpawnPoint = allowedPointType;
                createdCharacter.Name = charData.CharacterName;
                createdCharacter.AccountId = accountId;
                await createdCharacter.AccountStats.SetAccountStats(accountDataForAccStats);
            });

            _worldChars.Add(characterRef.Id);

            using (var wrapper2 = await ((IEntitiesRepositoryExtension)repo).GetExclusive<IWorldCharacter>(characterRef.Id))
            {
                var createdCharacter = wrapper2.Get<IWorldCharacterServer>(characterRef.Id);
                // Doll:
                var dollAddress = EntityPropertyResolver.GetPropertyAddress(createdCharacter.Doll);
                if (defaultCharacterDef.FirstRunDoll != null)
                    await createdCharacter.AddItems(await ClusterHelpers.ResolveDefaultItems(defaultCharacterDef.FirstRunDoll, new CalcerContext(wrapper2, characterRef.OuterRef, repo)), dollAddress);

                // Inventory:
                var inventoryAddress = EntityPropertyResolver.GetPropertyAddress(createdCharacter.Inventory);
                if (defaultCharacterDef.FirstRunInventory != null)
                    await createdCharacter.AddItems(await ClusterHelpers.ResolveDefaultItems(defaultCharacterDef.FirstRunInventory, new CalcerContext(wrapper2, characterRef.OuterRef, repo)), inventoryAddress);

                //Player start points:
                using (var knowledgeEngineWrapper = await ((IEntitiesRepositoryExtension)repo).GetExclusive<SharedCode.Entities.Engine.IKnowledgeEngine>(createdCharacter.KnowledgeEngine.Id))
                {
                    var startPoints = defaultCharacterDef.StartPoints.Target;
                    var knowledgeEngine = knowledgeEngineWrapper.Get<SharedCode.Entities.Engine.IKnowledgeEngine>(createdCharacter.KnowledgeEngine.Id);
                    await knowledgeEngine.ChangeRPoints(startPoints.TechPointCounts, true);
                    foreach (var knowledge in startPoints.StartKnowledges.Where(k => k.IsValid).Select(k => k.Target))
                        await knowledgeEngine.AddKnowledge(knowledge);
                    foreach (var technology in startPoints.StartTechnologies.Where(t => t.IsValid).Select(t => t.Target))
                        await knowledgeEngine.AddTechnology(technology);
                }

                WorldCharacter.LogEnteredWorld(createdCharacter);
            }
        }

        private async Task<PositionRotation> Get1stSuitedSpawnPointPosition(SpawnPointTypeDef allowedPointType, string pointName = null)
        {
            var resultData = await GetSpawnPoint(allowedPointType, pointName);
            return resultData.ToPositionRotation();
        }

        private async Task CheckInitSpawnPoints()
        {
            if (_spawnPoints != null)
                return;
            MapDef map;
            using (var wrapper = await EntitiesRepository.Get<IMapEntityServer>(this.OwnMap.Guid))
            {
                var mapEntity = wrapper?.Get<IMapEntityServer>(this.OwnMap.Guid);
                if (mapEntity == null)
                {
                    Logger.IfError()?.Message($"Can't get {nameof(IMapEntityServer)}.").Write();
                    return;
                }
                map = mapEntity.Map;
            }
            await AsyncUtils.RunAsyncTask(async () =>
            {
                lock (__lock__)
                {
                    if (_spawnPoints != null)
                        return;
                    var spawnPoints = new List<SpawnPointData>();
                    foreach (var scene in map.AllScenesToBeLoadedOnServerViaJdb)
                    {
                        var sceneParts = scene.Split('/');
                        var sceneName = sceneParts.Last().Split('.').First();
                        var pathToDaemons = "/SpawnSystemData/" + sceneName + "/" + sceneName;
                        if (GameResourcesHolder.Instance.IsResourceExists(pathToDaemons))
                        {
                            var res = GameResourcesHolder.Instance.TryLoadResourceLogError<SceneChunkDef>(pathToDaemons);
                            if (res == null)
                                continue;
                            for (int i = 0; i < res.PlayerSpawnPoints.Count; i++)
                            {
                                var rsp = res.PlayerSpawnPoints[i];
                                res.PlayerSpawnPoints[i] = new SpawnPointData(rsp.Position, rsp.Rotation, rsp.Name, rsp.SpawnRadius, rsp.SpawnPointTypeRef.Target);
                            }
                            if (res != null)
                                spawnPoints.AddRange(res.PlayerSpawnPoints);
                        }
                    }
                    _spawnPoints = spawnPoints.ToArray();
                }
            });
        }

        private async Task<SpawnPointData> GetSpawnPoint(SpawnPointTypeDef allowedPointType, string pointName)
        {
            await CheckInitSpawnPoints();

            if (_spawnPoints == null)
            {
                Logger.IfError()?.Message($"GetSpawnPoint({allowedPointType}, {pointName}).   Not found. Invalid returned.").Write();
                return SpawnPointData.InvalidData;
            }

            lock (__lock__)
            {
                var allowedSpawnPoints = allowedPointType == null
                    ? _spawnPoints
                    : _spawnPoints.Where(v => v.SpawnPointType == allowedPointType);

                if (pointName != null)
                    allowedSpawnPoints = allowedSpawnPoints.Where(x => x.Name.Contains(pointName));

                var count = allowedSpawnPoints.Count();
                if (count <= 0)
                {
                    Logger.IfError()?.Message($"GetSpawnPoint({allowedPointType}, {pointName}). Not found. Invalid returned.").Write();
                    return SpawnPointData.InvalidData;
                }

                var resultPoint = allowedSpawnPoints.Skip(SharedHelpers.Random.Next(count - 1)).First();
                return resultPoint;
            }
        }

        public async Task<bool> TeleportImpl(Guid oldRepositoryGuid)
        {
            CharacterData userData;
            using (var wrapper = await EntitiesRepository.GetFirstService<ILoginInternalServiceEntityServer>())
            {
                if (wrapper == null)
                {
                    Logger.IfError()?.Message("ILoginInternalServiceEntity not found {0}", EntitiesRepository.Id).Write();
                    return false;
                }

                var loginInternalService = wrapper.GetFirstService<ILoginInternalServiceEntityServer>();
                var userId = CallbackRepositoryHolder.CurrentCallbackRepositoryId;
                userData = await loginInternalService.GetCharacterDataByUserId(userId);
            }

            using (var wrapper = await EntitiesRepository.Get<IRepositoryCommunicationEntityServer>(oldRepositoryGuid))
            {
                if (wrapper == null)
                {
                    Logger.IfError()?.Message("Repository not found {0} ", oldRepositoryGuid).Write();
                    return false;
                }

                var repositoryCommunicationEntity = wrapper.Get<IRepositoryCommunicationEntityServer>(oldRepositoryGuid);

                if (repositoryCommunicationEntity == null)
                {
                    Logger.IfError()?.Message("Repository not found {0} ", oldRepositoryGuid).Write();
                    return false;
                }

                var result = await repositoryCommunicationEntity.SubscribeReplication(
                    WorldCharacter.StaticTypeId, userData.CharacterId, EntitiesRepository.Id,
                    ReplicationLevel.Server);

                if (!result)
                    Logger.IfError()?.Message("Error subscribe worldCharacter {0} to repository {1}", userData, EntitiesRepository.Id).Write();
            }
            {
                //wrapperWorldSpace.Entity.CharacterEngine.Entity.Spawn(characterEntityId.ToString(), "", default(Vector3), "");
                var result = await AddClient(userData.CharacterId, CallbackRepositoryHolder.CurrentCallbackRepositoryId);
            }

            return true;
        }


        public async Task<bool> ConnectStreamingRepoImpl(Guid repo)
        {
            if (repo == default)
                return false;
            if (_streamingRepos.TryAdd(repo, true))
                foreach (var worldObj in _worldObjects)
                {
                    Logger.IfTrace()?.Message($"ConnectStreamingRepo worldObj:{worldObj} EntitiesRepository:{EntitiesRepository} repo:{repo}").Write();
                    if (ShouldBeReplicatedToStreamingRepository(worldObj.Key.TypeId))
                        await EntitiesRepository.SubscribeReplication(worldObj.Key.TypeId, worldObj.Key.Guid, repo, ReplicationLevel.Always);
                }

            return true;
        }
        public async Task<bool> DisconnectStreamingRepoImpl(Guid repo)
        {
            bool unused;
            if (_streamingRepos.TryRemove(repo, out unused))
                foreach (var worldObj in _worldObjects)
                    if (ShouldBeReplicatedToStreamingRepository(worldObj.Key.TypeId))
                        await EntitiesRepository.UnsubscribeReplication(worldObj.Key.TypeId, worldObj.Key.Guid, repo, ReplicationLevel.Always);
            return true;
        }
        Guid _sceneEntityIdForRegions;
        public async Task<bool> PrepareStaticsForImpl(OuterRef<IEntity> sceneEntity)
        {
            List<SceneChunkDef> allScenesToBeLoadedOnServerViaJdb;
            Guid guid;
            using (var sw = await EntitiesRepository.Get(sceneEntity))
            {
                var se = sw.Get<ISceneEntityServer>(sceneEntity.Guid);
                world.InitForScene(se);
                world.Run();
                allScenesToBeLoadedOnServerViaJdb = se.SceneChunks;

                _sceneEntityIdForRegions = sceneEntity.Guid;
                guid = sceneEntity.Guid;
            }
            await AsyncUtils.RunAsyncTask(() => RegionBuildHelper.LoadRegionsByMapName(allScenesToBeLoadedOnServerViaJdb, guid));
            return true;
        }
        public async Task<bool> DespawnEntityImpl(OuterRef<IEntity> ent)
        {
            return await EntitiesRepository.Destroy(ent.TypeId, ent.Guid, true);
        }
        public async Task<bool> SpawnEntityImpl(Guid staticIdFromExport, OuterRef<IEntity> ent, Vector3 pos, Quaternion rot, MapOwner mapOwner, Guid spawner, IEntityObjectDef def, SpawnPointTypeDef point, ScenicEntityDef scenicDef, ScriptingContext ctx = null)
        {
            Dictionary<LinkTypeDef, List<OuterRef<IEntity>>> refs = null;
            if ((scenicDef?.LinksToStatics?.Count ?? 0) > 0)
            {
                using (var sceneW = await EntitiesRepository.Get<ISceneEntity>(mapOwner.OwnerSceneId))
                {
                    var scene = sceneW.Get<ISceneEntityServer>(mapOwner.OwnerSceneId);
                    refs = new Dictionary<LinkTypeDef, List<OuterRef<IEntity>>>();
                    foreach (var lts in scenicDef.LinksToStatics)
                    {
                        if (!refs.TryGetValue(lts.Key.Target, out var list))
                            refs.Add(lts.Key.Target, list = new List<OuterRef<IEntity>>());
                        foreach (var refToGuid in lts.Value)
                        {
                            if (scene.StaticToDynamicData.TryGetValue(refToGuid, out var outerRefId))
                            {

                                list.Add(new OuterRef<IEntity>(outerRefId));
                            }
                        }
                    }
                }
            }
            //rot = sceneRot * rot; //hope I'm not an idiot and it's actually rotating the object

            Func<IEntity, Task> lambda = async x =>
            {
                if (x is IHasProvidedScriptingContext psc && ctx != null)
                    psc.ProvidedContext = ctx;
                if (x is ISpawnDaemon sd && scenicDef != null)
                {
                    sd.SceneDef = scenicDef.SpawnDaemonSceneDef.Target;
                }
                if (x is IHasLinksEngine le)
                {
                    await le.LinksEngine.SetLinksFromScene(refs);
                }
                if (x is IScenicEntity scenicEntity)
                {
                    scenicEntity.StaticIdFromExport = staticIdFromExport;
                    scenicEntity.MapOwner = mapOwner;
                }
                if (x is IHasWorldSpaced worldSpaced)
                    worldSpaced.WorldSpaced.OwnWorldSpace = new OuterRef<IWorldSpaceServiceEntity>(this);

                var eObj = (IEntityObject)x;
                if (def != null)
                    eObj.Def = def;
                var positionable = PositionedObjectHelper.GetPositionable(x);
                if (positionable != null)
                {
                    if (pos != default)
                        positionable.SetTransform = new Transform(pos);
                    if (pos != default && rot != default)
                        positionable.SetTransform = new Transform(pos, rot);
                }
                if (x is IHasSpawnedObject spawnedObj)
                {
                    spawnedObj.SpawnedObject.Spawner = new OuterRef<ISpawnDaemon>(spawner, SpawnDaemon.StaticTypeId);
                    spawnedObj.SpawnedObject.PointType = point;
                }

                if (x is IHasDoll && def is IHasDollDef)
                {
                    ((IHasDoll)x).Doll.Size = ((IHasDollDef)def).DefaultDoll?.Size ?? 0;
                }

                if (x is IBank)
                {
                    ((IBank)x).Bank.BankDef = ((IBankDef)def).BankDef;
                }

            };

            var oldEnt = DatabaseSaveTypeChecker.GetDatabaseSaveType(ent.TypeId) == DatabaseSaveType.None ? null :
                await EntitiesRepository.Load(ent.TypeId, ent.Guid, lambda);
            if (oldEnt == null)
            {
                var newEnt = await EntitiesRepository.Create(ent.TypeId, ent.Guid, lambda);

                var defaultDollItems = (def as IHasDollDef)?.DefaultDoll?.DefaultItems;
                if (defaultDollItems != null)
                {
                    using (var wrapper = await EntitiesRepository.Get(newEnt.TypeId, newEnt.Id))
                    {
                        var hasDollEntity = wrapper?.Get<IHasDollServer>(newEnt.TypeId, newEnt.Id, ReplicationLevel.Server);
                        if (hasDollEntity != null)
                        {
                            var dollAddress = EntityPropertyResolver.GetPropertyAddress(hasDollEntity.Doll);
                            var itemResourcesToAdd = await ClusterHelpers.ResolveDefaultItems(defaultDollItems, new CalcerContext(wrapper, new OuterRef(newEnt.Id, newEnt.TypeId), EntitiesRepository));
                            var itemTransaction = new ItemAddBatchManagementTransaction(itemResourcesToAdd, dollAddress, false, EntitiesRepository);
                            await itemTransaction.ExecuteTransaction();
                        }
                    }
                }
            }
            return true;
        }

        public async Task<bool> RegisterWorldObjectsInNewInformationSetImpl(OuterRef worldObjectSetRef)
        {
            using (var wrapper = await EntitiesRepository.Get(worldObjectSetRef.TypeId, worldObjectSetRef.Guid))
            {
                var entity = wrapper.Get<IHasWorldObjectsInformationDataSetEngine>(worldObjectSetRef.TypeId, worldObjectSetRef.Guid);
                if (entity == null)
                {
                    Logger.IfError()?.Message("WorldObjectSetEntity {0} not found", worldObjectSetRef).Write();
                    return false;
                }

                var def = entity.WorldObjectsInformationDataSetEngine.WorldObjectInformationSetDef;
                var typeIds = def.ObjectsTypeFilter == null ? null : new HashSet<int>(def.ObjectsTypeFilter.Select(x => ReplicaTypeRegistry.GetIdByType(ReplicaTypeRegistry.GetTypeByName(def.EntityTypeName))));

                if (typeIds == null || typeIds.Count == 0)
                {
                    Logger.IfError()?.Message("RegisterWorldObjectsInNewInformationSetImpl {0} ObjectsTypeFilter is null or empty!!!", def).Write();
                    return false; // Требуем от дизайнеров чтобы типы указывали явно, иначе они забьют и будет проверятся на предикаты вся туева хуча кактусов, камней и т.д.
                }

                //фильтруем мировые объекты по типам
                var worldObjects = _worldObjects.Where(x => typeIds.Contains(x.Key.TypeId)).Select(x => x.Key).ToList();
                await entity.WorldObjectsInformationDataSetEngine.RegisterWorldObjectsInNewInformationSetBatch(worldObjects);
            }
            return true;
        }

        public static async Task<Vector3?> GetDropPosition(IEntity currentEntity,
            IEntitiesRepository entitiesRepository, Transform transform)
        {
            var parentEntityOutRef = new OuterRef(currentEntity.Id, currentEntity.TypeId);
            var ownWorldSpaceRef = ((IHasWorldSpaced)currentEntity).WorldSpaced.OwnWorldSpace;
            using (var ownWorldSpaceWrapper = await entitiesRepository.Get(ownWorldSpaceRef))
            {
                var ownWorldSpace = ownWorldSpaceWrapper.Get<IWorldSpaceServiceEntityServer>(ownWorldSpaceRef,
                    ReplicationLevel.Server);
                var worldNodeId = await ownWorldSpace.GetWorldNodeId(parentEntityOutRef);
                if (worldNodeId == null)
                {
                    Logger.IfError()?.Message("Coulnd't find world node id ParentEntity={0}", parentEntityOutRef).Write();
                    return null;
                }

                using (var wrapper = await entitiesRepository.Get<IWorldNodeServiceEntity>(worldNodeId.Value))
                {
                    var worldNode = wrapper.Get<IWorldNodeServiceEntityServer>(
                        WorldNodeServiceEntity.StaticTypeId,
                        worldNodeId.Value, ReplicationLevel.Server);
                    if (worldNode == null)
                    {
                        Logger.IfError()?.Message("Coulnd't get world node ParentEntity={0}", parentEntityOutRef).Write();
                        return null;
                    }

                    return await worldNode.GetDropPosition(transform.Position, transform.Rotation);

                }
            }
        }
    }
}