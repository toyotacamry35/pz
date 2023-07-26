using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedCode.MapSystem;
using GeneratedCode.Repositories;
using ResourceSystem.Aspects.AccessRights;
using SharedCode.Aspects.Item.Templates;
using SharedCode.CustomData;
using SharedCode.DeltaObjects;
using SharedCode.Entities;
using SharedCode.Entities.Engine;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.Entities.Service;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.EntityPropertyResolvers;
using SharedCode.MapSystem;
using SharedCode.Refs;
using SharedCode.Repositories;
using SharedCode.Utils;
using System;
using System.Threading.Tasks;
using ColonyShared.SharedCode.Entities;

// костры, верстаки
namespace GeneratedCode.DeltaObjects
{
    public partial class BuildingEngine
    {
        public async Task<BuildOperationResult> BuildImpl(PropertyAddress address, int slodIds, Vector3 position, Quaternion rotation)
        {
            using (var wrapper2 = await EntitiesRepository.Get<IWorldCharacter>(OwnerInformation.Owner.Guid))
            {
                var worldCharacter = wrapper2.Get<IWorldCharacter>(OwnerInformation.Owner.Guid);

                var container = EntityPropertyResolver.Resolve<IItemsContainer>(worldCharacter, address, ReplicationLevel.Master);
                if (container.Items.TryGetValue(slodIds, out ISlotItem slotItem))
                {
                    IEntityObjectDef entityObjectDef = (slotItem.Item?.ItemResource as ItemResource)?.MountingData?.EntityObjectDef.Target;
                    if (entityObjectDef.AssertIfNull(nameof(entityObjectDef)))
                        return BuildOperationResult.Error;

                    var createdEntityType = DefToType.GetEntityType(entityObjectDef.GetType());
                    if (!typeof(IMountable).IsAssignableFrom(createdEntityType))
                        return BuildOperationResult.ErrorIsNotBuildingType;

                    //Log.Logger.IfError()?.Message("Try can place {0}", prefabPath).Write();
                    var ownWorldSpace = new OuterRef<IWorldSpaceServiceEntity>();
                    MapOwner mapOwner = default;
                    using (var worldChar = await EntitiesRepository.Get<IWorldCharacter>(OwnerInformation.Owner.Guid))
                    {
                        var wc = worldChar.Get<IWorldCharacter>(OwnerInformation.Owner.Guid);
                        ownWorldSpace = wc.WorldSpaced.OwnWorldSpace;
                        mapOwner = wc.MapOwner;
                    }
                    bool isSuccessfullCreation = true;
                    var cantBuildHere = !BuildingEngineHelper.CanBuildHere(position, EntitiesRepository.GetSceneForEntity(OwnerInformation.Owner), entityObjectDef, false);
                    if (cantBuildHere)
                    {
                        return BuildOperationResult.ErrorCanNotPlaceHere;
                    }
                    bool hasCraftEngine = false;
                    var createdEntityTypeId = ReplicaTypeRegistry.GetIdByType(createdEntityType);
                    IEntityRef newBuildingRef = await EntitiesRepository.Create(createdEntityTypeId, Guid.NewGuid(),
                        createdEntity =>
                        {
                            var mountable = createdEntity as IMountable;
                            if (mountable.AssertIfNull(nameof(mountable)))
                            {
                                isSuccessfullCreation = false;
                                return Task.CompletedTask;
                            }

                            mountable.WorldSpaced.OwnWorldSpace = ownWorldSpace;
                            mountable.MapOwner = mapOwner;
                            mountable.Def = entityObjectDef;
                            mountable.MovementSync.SetTransform = new Transform(position, rotation);

                            if (entityObjectDef is WorldBoxDef worldBoxDef && createdEntity is IHasInventory hasInventory)
                                hasInventory.Inventory.Size = worldBoxDef.Size;

                            hasCraftEngine = createdEntity is IHasCraftEngine;

                            if (createdEntity is IHasOwner hasOwner)
                            {
                                hasOwner.OwnerInformation.Owner = OwnerInformation.Owner;
                            }

                            if (createdEntity is IHasFaction hasFaction)
                            {
                                hasFaction.Faction = worldCharacter.Faction;
                            }
                            return Task.CompletedTask;
                        }
                    );
                    if (createdEntityType == typeof(IWorldBaken))
                    {
                        await worldCharacter.ActivateCommonBaken(default);
                    }

                    if (hasCraftEngine)
                    {
                        var newCraftEngineRef = await EntitiesRepository.Create<ICraftEngine>(Guid.NewGuid(),
                            newCraftEngine =>
                            {
                                newCraftEngine.OwnerInformation.Owner = new OuterRef<IEntity>(newBuildingRef.Id, newBuildingRef.TypeId);
                                return Task.CompletedTask;
                            }
                        );

                        var batch = EntityBatch.Create()
                            .Add(newBuildingRef.TypeId, newBuildingRef.Id)
                            .Add(newCraftEngineRef.TypeId, newCraftEngineRef.Id);

                        using (var entities = await EntitiesRepository.Get(batch))
                        {
                            var worldBuildingEntity = entities.Get<IWorldMachineServer>(newBuildingRef.Id);
                            var craftEngineEntity = entities.Get<ICraftEngineServer>(newCraftEngineRef.Id);

                            var resultContainerAddress = EntityPropertyResolver.GetPropertyAddress(worldBuildingEntity.Inventory);
                            await craftEngineEntity.SetResultContainerAddress(resultContainerAddress);
                        }
                    }

                    if (isSuccessfullCreation)
                    {
                        await worldCharacter.RemoveItem(address, slodIds, 1, slotItem.Item.Id);

                        if (OwnerInformation.Owner.IsValid)
                            using (var statWrapper = await EntitiesRepository.Get(OwnerInformation.Owner.TypeId, OwnerInformation.Owner.Guid))
                            {
                                var hasStatistics = statWrapper.Get<IHasStatisticsServer>(OwnerInformation.Owner.TypeId, OwnerInformation.Owner.Guid, ReplicationLevel.Server);
                                if (hasStatistics != null)
                                    await hasStatistics.StatisticEngine.PostStatisticsEvent(new SharedCode.Quests.PlaceObjectEventArgs() { PlacedObject = entityObjectDef });
                            }
                    }

                    return isSuccessfullCreation ? BuildOperationResult.Success : BuildOperationResult.Error;
                }
                else
                {
                    return BuildOperationResult.ErrorNoItemFound;
                }
            }
        }
    }
}