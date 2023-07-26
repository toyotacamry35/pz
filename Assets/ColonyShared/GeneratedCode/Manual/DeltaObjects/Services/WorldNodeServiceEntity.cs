using System;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using GeneratedCode.Custom.Config;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using LocalServer.UnityMocks;
using NLog;
using ResourceSystem.Custom.Config;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Config;
using SharedCode.Entities.Cloud;
using SharedCode.Entities.Engine;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.Entities.Service;
using SharedCode.EntitySystem;
using SharedCode.MapSystem;
using SharedCode.MovementSync;
using SharedCode.Repositories;
using SharedCode.Serializers;
using SharedCode.Utils;

namespace GeneratedCode.DeltaObjects
{
    public partial class WorldNodeServiceEntity : IHasLoadFromJObject, IWorldNodeServiceEntityImplementRemoteMethods, IHookOnStart
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public Task Load(CloudSharedDataConfig sharedConfig, CustomConfig config, IEntitiesRepository entitiesRepository)
        {
            _config = (WorldNodeServiceEntityConfig) config;
            ClientNode = _config.ClientNode;
            return Task.CompletedTask;
        }

        public Task OnStart()
        {
            return InitializePorts();
        }

        public async Task<bool> InitializePortsImpl()
        {
            using (var wrapper = await EntitiesRepository.GetMasterService<IRepositoryCommunicationEntity>())
            {
                var serviceEntity = wrapper.GetMasterService<IRepositoryCommunicationEntity>();
                ExternalAddress = serviceEntity.ExternalAddress;
            }

            await SetState(WorldNodeServiceState.Empty);
            return true;
        }

        public async Task<bool> HostUnityMapChunkImpl(MapDef mapChunk)
        {
            if (EntitytObjectsUnitySpawnService.SpawnService == null)
            {
                Logger.IfError()?.Message("EntitytObjectsUnitySpawnService.SpawnService == null {0}", mapChunk.ToString()).Write();
                return false;
            }

            if (State != WorldNodeServiceState.Empty)
            {
                Logger.IfError()?.Message("HostUnityMapChunkImpl try load {0}, but cant use in state {1}", mapChunk.ToString(), State.ToString()).Write();
                return false;
            }

            await SetState(WorldNodeServiceState.Loading);
            _ = AsyncUtils.RunAsyncTask(async () =>
              {
                // await EntitytObjectsUnitySpawnService.SpawnService.LoadLevel(EntitiesRepository, mapChunk);
                using (var worldNode = await EntitiesRepository.Get(TypeId, Id))
                  {
                      await worldNode.Get<IWorldNodeServiceEntityServer>(TypeId, Id, ReplicationLevel.Server).HostedUnityMapChunk(mapChunk, Guid.Empty, Guid.Empty);
                  }
              });
            return true;
        }
        public Task<bool> IsReadyImpl()
        {
            return Task.FromResult(EntitytObjectsUnitySpawnService.SpawnService.IsReady());
        }
        MockLocomotionWorld _mockLocoWorld;
        Guid loadingMapInstanceId;
        Guid _mapInstanceRepositoryId;
        private WorldNodeServiceEntityConfig _config;

        public async Task<bool> HostUnityMapChunkImpl(MapDef mapChunk, Guid mapChunkId, Guid mapInstanceId, Guid mapInstanceRepositoryId)
        {
            Logger.IfWarn()?.Message($"Host unity map chunk {mapChunk.____GetDebugShortName()} {mapChunkId} {mapInstanceId} {mapInstanceRepositoryId}").Write();
            if (EntitytObjectsUnitySpawnService.SpawnService == null)
            {
                Logger.IfError()?.Message("EntitytObjectsUnitySpawnService.SpawnService == null mapChunk {0} mapInstance {1} repository {2}", mapChunk.ToString(), mapInstanceId, mapInstanceRepositoryId).Write();
                return false;
            }
            if(EntitytObjectsUnitySpawnService.SpawnService is UnityMock)
            {
                if(Constants.WorldConstants.UseMockLocomotion == MockLocomotionUsage.Cluster)
                {
                    _mockLocoWorld = new MockLocomotionWorld(EntitiesRepository);
                    _mockLocoWorld.Register(mapChunkId);
                }
            }
            else
            {
                if (Constants.WorldConstants.UseMockLocomotion == MockLocomotionUsage.Unity)
                {
                    _mockLocoWorld = new MockLocomotionWorld(EntitiesRepository);
                    _mockLocoWorld.Register(mapChunkId);
                }
            }
            if (State == WorldNodeServiceState.Loading && loadingMapInstanceId != mapInstanceId)
            {
                Logger.IfError()?.Message("loadingMapInstanceId != mapInstanceId State {0} mapChunk {1} mapInstance {2} repository {2}", State, mapChunk.ToString(), mapInstanceId, mapInstanceRepositoryId).Write();
                return false;
            }

            if (State == WorldNodeServiceState.Loading && loadingMapInstanceId == mapInstanceId)
                return true;

            if (State == WorldNodeServiceState.Loaded && mapChunk != Map)
            {
                Logger.IfError()?.Message("Loaded another map {0} mapChunk {1} mapInstance {2} repository {2}", this.Map, mapChunk.ToString(), mapInstanceId, mapInstanceRepositoryId).Write();
                return false;
            }

            if (MapInstanceId != Guid.Empty && MapInstanceId != mapInstanceId)
            {
                Logger.IfError()?.Message("Loaded another mapInstance {0} mapChunk {1} mapInstance {2} repository {2}", this.MapInstanceId, mapChunk.ToString(), mapInstanceId, mapInstanceRepositoryId).Write();
                return false;
            }

            await SetState(WorldNodeServiceState.Loading);
            loadingMapInstanceId = mapInstanceId;
            _mapInstanceRepositoryId = mapInstanceRepositoryId;
            if (mapInstanceRepositoryId != Guid.Empty && mapInstanceRepositoryId != EntitiesRepository.Id)
            {
                using (var wrapper = await EntitiesRepository.Get<IRepositoryCommunicationEntityServer>(mapInstanceRepositoryId))
                {
                    var repositoryEntity = wrapper.Get<IRepositoryCommunicationEntityServer>(mapInstanceRepositoryId);
                    await repositoryEntity.SubscribeReplication(ReplicaTypeRegistry.GetIdByType(typeof(IMapEntity)), mapInstanceId, EntitiesRepository.Id, ReplicationLevel.Server);
                }
            }

            _ = AsyncUtils.RunAsyncTask(async () =>
              {
                  Logger.IfWarn()?.Message($"Call server load level {mapChunk.____GetDebugShortName()}").Write();
                // await EntitytObjectsUnitySpawnService.SpawnService.LoadLevel(EntitiesRepository, mapChunk);
                Logger.IfWarn()?.Message($"Loaded {mapChunk.____GetDebugShortName()}").Write();
                  using (var worldNode = await EntitiesRepository.Get(TypeId, Id))
                  {
                      await worldNode.Get<IWorldNodeServiceEntityServer>(TypeId, Id, ReplicationLevel.Server).HostedUnityMapChunk(mapChunk, mapChunkId, mapInstanceId);
                  }
              });
            return true;
        }

        public async Task<bool> HostedUnityMapChunkImpl(MapDef mapChunk, Guid mapChunkId, Guid mapInstanceId)
        {
            if (EntitytObjectsUnitySpawnService.SpawnService == null)
                return false;
            Logger.IfWarn()?.Message($"Hosted unity map chunk {mapChunk.____GetDebugShortName()}").Write();
            Map = mapChunk;
            MapInstanceId = mapInstanceId;
            MapChunkId = mapChunkId;
            await SetState(WorldNodeServiceState.Loaded);

            return true;
        }

        public Task<bool> SetStateImpl(WorldNodeServiceState state)
        {
            State = state;
            return Task.FromResult(true);
        }
        public async ValueTask<bool> CanBuildHereImpl(IEntityObjectDef entityObjectDef, OuterRef<IEntity> ent, Vector3 position, Vector3 scale, Quaternion rotation)
        {
            return BuildingEngineHelper.CanBuildHere(position, EntitiesRepository.GetSceneForEntity(ent), entityObjectDef, false);
        }

        public ValueTask<Vector3> GetDropPositionImpl(Vector3 playerPosition, Quaternion playerRotation)
        {
            return EntitytObjectsUnitySpawnService.SpawnService.GetDropPosition(playerPosition, playerRotation);
        }
        }
}