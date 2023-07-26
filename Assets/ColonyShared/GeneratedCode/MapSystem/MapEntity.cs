using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Utils;
using ResourcesSystem.Loader;
using Assets.Src.RubiconAI;
using GeneratedCode.Custom.Config;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.Entities.Service;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using SharedCode.MapSystem;
using SharedCode.Utils;
using System.Text;
using SharedCode.Repositories;
using GeneratedCode.MapSystem;
using SharedCode.Entities.Cloud;
using SharedCode.Serializers;
using System.Collections.Immutable;
using Core.Environment.Logging.Extension;

namespace GeneratedCode.DeltaObjects
{
    public partial class MapEntity : IHookOnInit, IHookOnDatabaseLoad, IHookOnDestroy, IHookOnReplicationLevelChanged, IHookOnUnload
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public async Task OnInit()
        {
            Dead = false;
            await InitializeWorldSpaces(new WorldSpacesConfiguration { SizeX = 1, SizeY = 1 });
        }

        private ImmutableList<OuterRef<IEntity>> _scenesToLoad = ImmutableList<OuterRef<IEntity>>.Empty;

        public async Task OnDatabaseLoad()
        {
            Dead = false;
            WorldSpaces = new DeltaList<IWorldSpaceDescription>();
            await InitializeWorldSpaces(new WorldSpacesConfiguration { SizeX = 1, SizeY = 1 });
        }

        ConcurrentDictionary<SceneChunkDef, OuterRef<IEntity>> _mapToSpawnerAssignments = new ConcurrentDictionary<SceneChunkDef, OuterRef<IEntity>>();
        public Task<bool> TryAquireSpawnRightsForPointsSetImpl(OuterRef<IEntity> spawner, SceneChunkDef mapSceneDef)
        {
            var result = _mapToSpawnerAssignments.TryAdd(mapSceneDef, spawner);
            return Task.FromResult(result);

        }

        private async ValueTask InitializeWorldSpaces(WorldSpacesConfiguration config)
        {
            var map = Map;
            var newWorldSpaceDescription = await CreateWorldSpaceDescription(map);
            WorldSpaces.Add(newWorldSpaceDescription);

            await CheckState();

        }

        public async Task<bool> ChangeChunkDescriptionImpl(Guid descriptionId, MapChunkState newState, Guid unityRepositoryId)
        {
            var wsd = WorldSpaces.FirstOrDefault(x => x.WorldSpaceGuid == descriptionId);
            if (wsd == null)
            {
                Logger.IfError()?.Message("Mapentity {0} not contains worldspacedescriptoin {1} to set state {2}", this.Id, descriptionId, newState.ToString()).Write();
                return false;
            }
            var oldState = wsd.State;
            wsd.State = newState;
            wsd.UnityRepositoryId = unityRepositoryId;
            await CheckState();

            return true;
        }
        public async Task<OuterRef<IEntity>> GetWorldSpaceForPointImpl(Vector3 point)
        {
            return new OuterRef<IEntity>(_singularHackGuid, ReplicaTypeRegistry.GetIdByType(typeof(IWorldSpaceServiceEntity)));

        }
        private static List<SceneChunkDef> CollectMaps(string[] allScenesToBeLoadedOnServerViaJdb)
        {
            List<SceneChunkDef> maps = new List<SceneChunkDef>();
            foreach (var scene in allScenesToBeLoadedOnServerViaJdb)
            {
                var sceneParts = scene.Split('/');
                var sceneName = sceneParts.Last().Split('.').First();
                var pathToDaemons = "/SpawnSystemData/" + sceneName + "/" + sceneName;
                if (GameResourcesHolder.Instance.IsResourceExists(pathToDaemons))
                {
                    var res = GameResourcesHolder.Instance.LoadResource<SceneChunkDef>(pathToDaemons);
                    if (res != null)
                    {
                        maps.Add(res);
                    }
                }
            }

            return maps;
        }


        public async Task<bool> NotifyAllCharactersViaChatImpl(string text)
        {
            await OnNewChatMessageEvent("Server", text);
            return true;
        }
        private async Task<bool> LoadScenes()
        {
            var mapName = Map.DebugName;
            var map = Map;
            var mapId = Id;

            var scenesToLoad = SavedScenes.Select(x => x.Key).ToList();

            SavedScenes[new OuterRef<IEntity>(Id, SceneEntity.StaticTypeId)] = true;

            var mainScenechunks = await AsyncUtils.RunAsyncTask(() =>
            {
                var allScenesToBeLoadedOnServerViaJdb = map.AllScenesExportedJdbsPaths;
                return new List<SceneChunkDef>(allScenesToBeLoadedOnServerViaJdb
                    .Where(x => GameResourcesHolder.Instance.IsResourceExists(x))
                    .Select(x => GameResourcesHolder.Instance.TryLoadResourceLogError<SceneChunkDef>(x)).Where(x => x != null));
            });
            foreach (var sceneToLoad in scenesToLoad)
            {
                await EntitiesRepository.Load<ISceneEntity>(sceneToLoad.Guid, (se) =>
                {
                    se.MapOwner = new MapOwner(mapId, mapId);
                    if (se.Id == mapId)
                        se.SceneChunks = mainScenechunks;
                    return Task.CompletedTask;
                });
            }

            if (scenesToLoad.SingleOrDefault(x => x.Guid == Id) == default)
            {
                var allScenesToBeLoadedOnServerViaJdb = map.AllScenesExportedJdbsPaths;
                var mainScene = await EntitiesRepository.Create<ISceneEntity>(mapId, (e) =>
                {
                    e.MapOwner = new MapOwner(mapId, mapId);
                    e.SceneChunks = mainScenechunks;
                    return Task.CompletedTask;
                });
                scenesToLoad.Add(new OuterRef<IEntity>(mainScene));
            }

            _scenesToLoad = ImmutableList.CreateRange(scenesToLoad);

            foreach (var worldSpaceDesc in WorldSpaces)
            {
                var worldSpaceGuid = worldSpaceDesc.WorldSpaceGuid;
                var chunkDef = worldSpaceDesc.ChunkDef;
                var unityRepoId = worldSpaceDesc.UnityRepositoryId;

                using (var wrapper = await EntitiesRepository.Get<IWorldSpaceServiceEntityServer>(worldSpaceGuid))
                {
                    var wsEntity = wrapper.Get<IWorldSpaceServiceEntityServer>(worldSpaceGuid);
                    if (Constants.WorldConstants.ReplicateAnythingToUnity)
                    {
                        await wsEntity.ConnectStreamingRepo(unityRepoId);
                    }
                    await wsEntity.PrepareStaticsFor(new OuterRef<IEntity>(Id, SceneEntity.StaticTypeId));
                }
            }

            MapIsReadyVisualizer.PrintMapIsReadyToConsole(mapName);
            return true;
        }

        public Task<bool> SetMapEntityStateImpl(MapEntityState state)
        {
            State = state;
            return Task.FromResult(true);
        }

        public async Task SpawnNewBotsImpl(List<Guid> botIds, string spawnPointTypePath)
        {
            using (var wrapper = await EntitiesRepository.Get<IWorldSpaceServiceEntityServer>(WorldSpaces.First().WorldSpaceGuid))
            {
                var wsEntity = wrapper?.Get<IWorldSpaceServiceEntityServer>(WorldSpaces.First().WorldSpaceGuid);
                if (wsEntity == null)
                {
                    Logger.IfError()?.Message("World space {0} not found", WorldSpaces.First().WorldSpaceGuid).Write();
                    return;
                }

                var userId = GeneratedCode.Manual.Repositories.CallbackRepositoryHolder.CurrentCallbackRepositoryId;
                await wsEntity.SpawnNewBot(spawnPointTypePath, botIds, userId);
            }
        }

        private async ValueTask CheckState()
        {
            var state = MapEntityState.Loaded;
            foreach (var worldSpaceDescription in WorldSpaces)
            {
                if (worldSpaceDescription.State == MapChunkState.Failed)
                {
                    state = MapEntityState.Failed;
                    break;
                }
                if (worldSpaceDescription.State == MapChunkState.Loading || worldSpaceDescription.State == MapChunkState.None)
                    state = MapEntityState.Loading;
            }

            if (state == MapEntityState.Loaded)
            {
                await LoadScenes();
            }

            await SetMapEntityState(state);
        }

        Guid _singularHackGuid;
        private async Task<WorldSpaceDescription> CreateWorldSpaceDescription(MapDef chunkDef)
        {
            var worldSpaceGuid = Guid.NewGuid();
            _singularHackGuid = worldSpaceGuid;
            var wsd = new WorldSpaceDescription()
            {
                State = (!Map.NeedsUnity || Map.ClientMap) ? MapChunkState.Loaded : MapChunkState.None,
                ChunkDef = chunkDef,
                WorldSpaceGuid = worldSpaceGuid,
                WorldSpaceRepositoryId = EntitiesRepository.Id
            };
            var ownRef = new OuterRef<IMapEntity>(this);
            var wsRef = await EntitiesRepository.Create<IWorldSpaceServiceEntity>(worldSpaceGuid, (ws) => { ws.OwnMap = ownRef; return Task.CompletedTask; });
            return wsd;
        }


        public Task<bool> RegisterObjectImpl(OuterRef<IEntity> databasedObject)
        {
            return Task.FromResult(true);
        }

        public Task<bool> UnregisterObjectImpl(OuterRef<IEntity> databasedObject)
        {
            return Task.FromResult(true);
        }

        public void OnReplicationLevelChanged(long oldReplicationMask, long newReplicationMask)
        {
            if (ReplicationMaskUtils.IsReplicationLevelAdded(oldReplicationMask, newReplicationMask, ReplicationLevel.Server)
                && !ReplicationMaskUtils.IsReplicationLevelRemoved(oldReplicationMask, newReplicationMask, ReplicationLevel.Server))
            {
                ServerRepoReadyNotifier.ServerRepoIsReadyChanged(true, EntitiesRepository.Id);
                AsyncUtils.RunAsyncTask(async () =>
                {
                    if (EntitytObjectsUnitySpawnService.SpawnService != null)
                    {
                        await EntitytObjectsUnitySpawnService.SpawnService.LoadLevel(EntitiesRepository, Id, Map);
                    }
                });
            }
            else if (ReplicationMaskUtils.IsReplicationLevelRemoved(oldReplicationMask, newReplicationMask, ReplicationLevel.Server)
            && !ReplicationMaskUtils.IsReplicationLevelAdded(oldReplicationMask, newReplicationMask, ReplicationLevel.Server))
            {
                AsyncUtils.RunAsyncTask(async () =>
                    {
                        if (EntitytObjectsUnitySpawnService.SpawnService != null)
                        {
                            await EntitytObjectsUnitySpawnService.SpawnService.UnloadLevel(EntitiesRepository);
                        }
                    }
                );
                ServerRepoReadyNotifier.ServerRepoIsReadyChanged(false, EntitiesRepository.Id);
            }

            if (EntitiesRepository.CloudNodeType == SharedCode.Cloud.CloudNodeType.Client)
            {
                if (ReplicationMaskUtils.IsReplicationLevelAdded(oldReplicationMask, newReplicationMask,
                        ReplicationLevel.Always)
                    && !ReplicationMaskUtils.IsReplicationLevelRemoved(oldReplicationMask, newReplicationMask,
                        ReplicationLevel.Always))
                    AsyncUtils.RunAsyncTask(async () =>
                    {
                        if (EntitytObjectsUnitySpawnService.SpawnService != null)
                        {
                            await EntitytObjectsUnitySpawnService.SpawnService.LoadLevel(EntitiesRepository, Id, Map);
                            using (var cheatWrapper = await EntitiesRepository.GetMasterService<IClientCommunicationEntity>())
                            {
                                if (cheatWrapper.TryGetMasterService<IClientCommunicationEntity>(out var clientCommunicationEntity))
                                {
                                    await clientCommunicationEntity.SetLevelLoaded();
                                }
                            }
                            this.NewChatMessageEvent += MapEntity_NewChatMessageEvent;
                        }
                    });
                else if (ReplicationMaskUtils.IsReplicationLevelRemoved(oldReplicationMask, newReplicationMask,
                             ReplicationLevel.Always)
                         && !ReplicationMaskUtils.IsReplicationLevelAdded(oldReplicationMask, newReplicationMask,
                             ReplicationLevel.Always))
                    AsyncUtils.RunAsyncTask(async () =>
                    {
                        if (EntitytObjectsUnitySpawnService.SpawnService != null)
                        {
                            this.NewChatMessageEvent -= MapEntity_NewChatMessageEvent;
                            await EntitytObjectsUnitySpawnService.SpawnService.UnloadLevel(EntitiesRepository);
                        }
                    });
            }
        }

        private Task MapEntity_NewChatMessageEvent(string arg1, string arg2)
        {
            EntitytObjectsUnitySpawnService.SpawnService?.PostMessageFromMap(arg1, arg2);
            return Task.CompletedTask;
        }

        public async Task OnUnload()
        {
            await OnDestroy();
        }
        private static readonly NLog.Logger ConsoleLogger = NLog.LogManager.GetLogger("Console");
        public async Task OnDestroy()
        {
            foreach (var ent in _scenesToLoad)
            {
                try
                {

                    await EntitiesRepository.Destroy(ent.TypeId, ent.Guid, true);

                }
                catch (Exception e)
                {
                    Logger.IfError()?.Exception(e).Write();
                }
            }
            await EntitiesRepository.Destroy(WorldSpaceServiceEntity.StaticTypeId, _singularHackGuid);
            ConsoleLogger.IfInfo()?.Message($"{this.Map.____GetDebugShortName()} DESTROYED").Write();
        }
        public async Task<bool> OnLastUserLeftImpl()
        {
            if (Map.DestroyOnLastUser)
                Dead = true;
            return true;
        }
    }



    public static class MapIsReadyVisualizer
    {
        private static readonly NLog.Logger ConsoleLogger = NLog.LogManager.GetLogger("Console");

        public static void PrintMapIsReadyToConsole(string mapName)
        {
            if (mapName.Contains("Savannah"))
            {
                ConsoleLogger.IfInfo()?.Message(@"Savannah is ready

▄▄▄█▄▄▄▄██▄░░░░░░░░░░░░▄░░░░░░░░▄░░░░░░░░░░░░▄▄
█▀░░▀▀██▀█▀░░░░░░░░░░░░░░░░░░░░░█▄░░░░░░░░░░░██
██░░░████████░░░░░░░░░░░█▄░░░░░░▀▀▄░░░░░░░░▄██░░░░░░░░░░░░█░░░░▄
▄▄▄▄████████░░░░░░░░░░░▄░░░▄░░░░░░▀█░▄▄░▄██▀▀░░░░░░░░░░░░░░░░░░▀█▄
██▄█████████▄░░░░░░░░░░░░░░░░░░░░░░░░░▀░░░░░░░░░▄░░░▀░░░░░░░░░░░▄█░▄█
████▄████████▀█▄▄▄▄░░░░░░░░░░░░░░▄░░░░░░░░░░░░░░░░░░░░░░░░░░░░░▀██▄░█▄
██▀▄█████▄███▀██▀▀▀▀██▄░░░░░░░░▀░░░░░░░░░░░░░░░░░░░░░░░░░░░▀█▄██▄███▀▀██
█████████░▄█▄██░░░░▄░░██▄░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░▄▄█████▀██░░████▄█▄
██████████████▄░░░░░▄░░▀█▄░░░░░░░░▀░░░░░░░░░░░░░░░░░░░░░▄█████▀▀▄███░░▀█▀███▀
██████▀███░█████▄▄▄▄░▄███▀░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░█████▀███████████░░▄███
█████▄███░▄██▀▀████▀████░░░░░░░░░▄░░░░░░░░░░░░░░░░░░░░▄░▄░▀░░░░▀█████░░███▄░███
███████████▄██▄░░▀█████░░░░░▄░░░░░█▀░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░▀░▄▄█▀█████▀
░██████▀█▀▀█▀██░░░░░▄██░░░░░░░░░░░▄▄░░░░░░░░░░░░░░░░░░░░░░░░░░▄░░░░░░░▀▀░░▄████
▄█▄██░░░░░░░░░▀██▄░░██▀█▀▄▄░░░░░▄░▄▄░░░░░░░░░░░░░▄░░░░░░▄▄▄▄▄░██▄▀▄▄▄░░░▄██████
███▀▀░░░▄░░░▄░░▄▄███▀░░░░░░░▀░░█▀▀░░░▀░░░░░░░░░░░░░░░░░░░█▄███████████▄░▀████▀█
██▀▀░░░▀░░░░░░░░░░░░░░░░▄█▄░░░░░▀░░░░░░░░░░░░░░░░░░░░░░░░░█████▀░▀████░░████▄
█████▄░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██▀▀▀▀░░░░░░░░░██████▄▄
███▄███▄████░░█░░░▀░░░░░░░░░░░░░░░░░░░░░░░░░░▀░█░░░░░░░▄██▄▄███▄▄░░░░░░▀███▀▀▀
██████░████░░░░░░░░░░░▄░░░░░░░░░▄░░░░░░░░░░░░░░░░▄░░░░░▀░▀▀▀▀▀██████▄░░████
██░▀▀▀░██▀░░░░░░░▀░░▀▄▄▀▄▄▄░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░▀▀▀▀██▄████▄
▄▄▄░▄░░▄▄░░░░▄░░░░▄░░▄▄▄░▄░░░░░░░░▄░░░░░░░░░░░░░░░░░░░░░░░▄░░░░░░░░░░▀███▄██▄
████░░▄██░░░░░▄░▄▄▀░░▀▄░░░░░░░░░░░░▀▀░░░░▄░▀░░░░░░░░░░▄█▄░░░░░░░░░░░░░████████▄
░██▀▄░░▀█▀░░█░░██░░░░░░░░░▄░░░▀▀░░█░░░░░░▀▀░▄░░░░▄░░░░▀█▀░░░░░░░░░░░░▄████████
░█▀▄░▄▄█░░░░░▄░░░▄▄░▄▄░░░▀▀░░░░░░░░█░░░░░░░░░▀█▄▄▀█░░▄█░░░███▄░░▀░░▄██░▀▀█████▀
████▄▀▀▀█▀░░█▄██░░░▄░░░▀▀░░░░░░░░░░░░▄░░▄░▄░▀░░█▀▀█░░░░░░▀▄░░▀▀░░░░░▀░░░▄▄▀████
░██▀░░░░░░░░░░▄░▀░▀░░░▀▀░▄░░░░░░░░░░░░▄░░░░░▀░░░░░░░░░░▄▀░░░▄░░░░░░░░░░░░░░██░█
▄███▄░░░░░▄░▄░░░░░░░░░░▄▄░░░░░▀▀░░▀░░▀░░░░░░▀░▄░░░░░░░░░██▀░█▀░░░░░░░░░░█▀░▀▀░▀
████▀░▄▄░░░░░░░░░░▀░░░▀▀░░░░░░░░░░░░░░░░░░▄░░░░░░▄█▄░▀▄░░▀░░░▄░░░░░░▄░░░░░░█▄▄▄
███▄░░░░░▀▄░▀▀▄█░▀░░▄░░░░░░░░░░░░░░░░░░░░░▀░░░░░░░░░░██▄░░░░▀▀░░▀▄▄░░░░▀█░░░█▀▀
████▄░░░░░▄░░░░▀░░░▄░▄░░░░░░░▄░░▄▄░▄░░░░░░░▄██▀░░░░░░░█▀░░░░░░▄▄▄░░░░░░▄░░░▄█░▄
█████▄▄████░░░░░░█░░░░▀░░░░▄░░░░░▀░░█░▄░░░▄█▀░░░█▄█░▀░░░░░░░░░░███░▀█▄▄░░██████
░▀████████░▀░▄░▀▀░░░░█░█░░░▀░░░▀▀░░▀█▀█▄░▄█▀░░▄██▀░░░░░░░░░░▄▄░▀░▀███▀█▄█████▀▀
░░░▀▀████████░░░█░░░▀▀░▀░░░░░▄▄▄░░░▄█▄████▀█▀░▀▀█░░░░░░░░░▄▄░▄▄▄█░█████▀██▀▀
░░░░░███████████▄█▄▄▄█▄░▄█▄▄▄███▀░▄████░░▀▀░░▄░░░█░░░░░░░▀░▄████▄████▄▄██
░░░███████░▀▀▀██████████████▄███████▄█▄░██▄░░░░▄▄█▄▄████████████████████
░░▄████▀██▀░░░░░▀▀▀███▄████████████████▀█████████████████████░░░░░███▀██▄
░░░░▀▀░▀▀░░░░░░░░░▄██▀░▀▀████▀▀▀█▀█████▀██▀▀▀▀▀▀▀▀▀▀▀████▀▀░░░░░░░██████▀▀
░░░░░░░░░░░░░░░░▄███▄░▄▄▄███▄░░░░░░░▀░░░░░░░░░░░░░▄██████░░░░░░░░░░░░░▀
░░░░░░░░░░░░░░░█████▀░█████▀█▄░░░░░░░░░░░░░░░░░░░░████▀▀
░░░░░░░░░░░░░░░░░░░░░░▀▀▀░░█
").Write();
            }
            else
            {
                string mapString = $"=== {mapName} is ready ===";
                ConsoleLogger.IfInfo()?.Message(new StringBuilder().Append('=', mapString.Length).ToString()).Write();
                ConsoleLogger.IfInfo()?.Message(mapString).Write();
                ConsoleLogger.IfInfo()?.Message(new StringBuilder().Append('=', mapString.Length).ToString()).Write();
            }
        }

    }
}
