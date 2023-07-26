using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedCode.Repositories;
using NLog;
using SharedCode.EntitySystem;
using SharedCode.MovementSync;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.Src.Aspects.Impl.Factions.Template;
using Core.Environment.Logging.Extension;
using ResourceSystem.Utils;
using SharedCode.Entities;
using SharedCode.EntitySystem.EntityPropertyResolvers;

namespace GeneratedCode.DeltaObjects
{
    public partial class WorldObjectsInformationDataSetEntity
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        //квадрат разницы старой и новой позиций, меньше которой не происходит апдейта
        private const float CheckedPositionDistanceSquare = 100f;

        private static ConcurrentDictionary<OuterRef, ValueTuple<PropertyChangedDelegate, PropertyChangedDelegate>>
            _subscribersCache = new ConcurrentDictionary<OuterRef, ValueTuple<PropertyChangedDelegate, PropertyChangedDelegate>>();

        public Task RegisterWorldObjectsInNewInformationSetImpl(EntityId worldObjectId)
        {
            return registerWorldObjectsInNewInformationSetInternal(worldObjectId);
        }

        public async Task RegisterWorldObjectsInNewInformationSetBatchImpl(List<EntityId> worldObjectsIds)
        {
            foreach (var worldObjectsId in worldObjectsIds)
                await registerWorldObjectsInNewInformationSetInternal(worldObjectsId);
        }

        private async Task registerWorldObjectsInNewInformationSetInternal(EntityId worldObjectId)
        {
            using (var wrapper = await EntitiesRepository.Get(worldObjectId.TypeId, worldObjectId.Guid))
            {
                var entity = wrapper.Get<IEntity>(worldObjectId.TypeId, worldObjectId.Guid, ReplicationLevel.Server);
                if (entity == null)
                {
                    Logger.IfError()?.Message("registerWorldObjectsInNewInformationSetInternal world object not found", worldObjectId.TypeId, worldObjectId.Guid).Write();
                }

                if (entity is IHasCharacterMovementSyncServer)
                {
                    var characterMovementSyncEntity = (IHasCharacterMovementSyncServer) entity;
                    var mutatingEntity = (IHasMutationMechanicsServer)entity;
                    var newWorldObjectInformation = new CharacterPositionInformation();
                    newWorldObjectInformation.Position = characterMovementSyncEntity.MovementSync.__SyncMovementStateReliable.State.Position;
                    newWorldObjectInformation.Mutation = mutatingEntity.MutationMechanics.Stage;
                    var outerRef = new OuterRef(entity.Id, entity.TypeId);
                    Positions.Add(outerRef, newWorldObjectInformation);
                    var propertyAddress = EntityPropertyResolver.GetPropertyAddress(newWorldObjectInformation);
                    var repository = EntitiesRepository;
                    PropertyChangedDelegate movementDelegate = async (args) => { await syncMovementStateChanged(propertyAddress, (CharacterMovementStateFrame)args.NewValue, repository); };
                    PropertyChangedDelegate mutationDelegate = async (args) => { await mutationStateChanged(propertyAddress, (MutationStageDef)args.NewValue, repository); };
                    _subscribersCache[outerRef] = (movementDelegate, mutationDelegate);
                    characterMovementSyncEntity.MovementSync.SubscribePropertyChanged(nameof(characterMovementSyncEntity.MovementSync.__SyncMovementStateReliable), movementDelegate);
                    mutatingEntity.MutationMechanics.SubscribePropertyChanged(nameof(mutatingEntity.MutationMechanics.Stage), mutationDelegate);
                }
            }
        }

        private async Task syncMovementStateChanged(PropertyAddress deltaObjectAddress, CharacterMovementStateFrame movementFrame, IEntitiesRepository repository)
        {
            using (var wrapper = await repository.Get(deltaObjectAddress.EntityTypeId, deltaObjectAddress.EntityId))
            {
                var entity = wrapper.Get<IEntity>(deltaObjectAddress.EntityTypeId, deltaObjectAddress.EntityId);
                var worldObjectInformation =  EntityPropertyResolver.Resolve<ICharacterPositionInformation>(entity, deltaObjectAddress);
                if (SharedCode.Utils.Vector3.GetSqrDistance(worldObjectInformation.Position, movementFrame.State.Position) >= CheckedPositionDistanceSquare)
                    await worldObjectInformation.SetPosition(movementFrame.State.Position);
            }
        }

        private async Task mutationStateChanged(PropertyAddress deltaObjectAddress, MutationStageDef stage, IEntitiesRepository repository)
        {
            using (var wrapper = await repository.Get(deltaObjectAddress.EntityTypeId, deltaObjectAddress.EntityId))
            {
                var entity = wrapper.Get<IEntity>(deltaObjectAddress.EntityTypeId, deltaObjectAddress.EntityId);
                var worldObjectInformation = EntityPropertyResolver.Resolve<ICharacterPositionInformation>(entity, deltaObjectAddress);
                await worldObjectInformation.SetMutation(stage);
            }
        }

        public Task UnregisterWorldObjectsInNewInformationSetImpl(EntityId worldObjectId)
        {
            return unregisterWorldObjectsInNewInformationSetInternal(worldObjectId);
        }

        public async Task UnregisterWorldObjectsInNewInformationSetBatchImpl(List<EntityId> worldObjectsIds)
        {
            foreach (var worldObjectsId in worldObjectsIds)
                await unregisterWorldObjectsInNewInformationSetInternal(worldObjectsId);
        }

        private async Task unregisterWorldObjectsInNewInformationSetInternal(EntityId worldObjectId)
        {
            using (var wrapper = await EntitiesRepository.Get(worldObjectId.TypeId, worldObjectId.Guid))
            {
                var entity = wrapper.Get<IEntity>(worldObjectId.TypeId, worldObjectId.Guid, ReplicationLevel.Server);
                if (entity == null)
                {
                    Logger.IfError()?.Message("registerWorldObjectsInNewInformationSetInternal world object not found", worldObjectId.TypeId, worldObjectId.Guid).Write();
                }

                if (entity is IHasCharacterMovementSyncServer)
                {
                    var characterMovementSyncEntity = (IHasCharacterMovementSyncServer)entity;
                    var mutatingEntity = (IHasMutationMechanicsServer)entity;
                    var outerRef = new OuterRef(entity.Id, entity.TypeId);
                    Positions.Remove(outerRef);
                    var delegatesTuple = _subscribersCache[outerRef];
                    characterMovementSyncEntity.MovementSync.UnsubscribePropertyChanged(nameof(characterMovementSyncEntity.MovementSync.__SyncMovementStateReliable), delegatesTuple.Item1);
                    mutatingEntity.MutationMechanics.UnsubscribePropertyChanged(nameof(mutatingEntity.MutationMechanics.Stage), delegatesTuple.Item2);
                }
            }
        }


        public Task<bool> ContainsWorldObjectInformationImpl(EntityId worldObjectId)
        {
            return Task.FromResult(Positions.ContainsKey(new OuterRef(worldObjectId.Guid, worldObjectId.TypeId)));
        }

        public Task<List<EntityId>> ContainsWorldObjectInformationListImpl(List<EntityId> worldObjectsId)
        {
            var result = new List<EntityId>();
            foreach (var entityId in worldObjectsId)
                if (Positions.ContainsKey(new OuterRef(entityId.Guid, entityId.TypeId)))
                    result.Add(entityId);
            return Task.FromResult(result);
        }
    }
}
