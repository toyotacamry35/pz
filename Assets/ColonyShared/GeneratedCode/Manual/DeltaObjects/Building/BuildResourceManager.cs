using Assets.ColonyShared.SharedCode.Aspects.Item;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedCode.Transactions;
using SharedCode.Aspects.Building;
using SharedCode.Entities;
using SharedCode.Entities.Service;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.EntityPropertyResolvers;
using SharedCode.MapSystem;
using SharedCode.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

namespace GeneratedCode.DeltaObjects
{
    public enum ResourceOperationResultCode
    {
        Success,
        BuildRecipeIsNull,
        BuildingPlaceIsNull,
        ElementsIsNullOrEmpty,
        CharacterNotFound,
        ContainerItemOperationFailed,
        ContainerItemOperationNotEnoughResources,
        CantGetNewWorldBoxEntity,
        NewWorldBoxAddressIsNull,
    }

    public class ResourcesOperationResult
    {
        public ResourceOperationResultCode Result { get; set; } = ResourceOperationResultCode.Success;
        public ContainerItemOperationResult ContainerItemOperationResult { get; set; } = ContainerItemOperationResult.None;
        public string Message { get { return $"result: {Result}, ContainerResult: {ContainerItemOperationResult}"; } }
    }

    public class CheckResourcesResult : ResourcesOperationResult
    {
        public List<RemoveItemBatchElement> ClaimItems { get; } = new List<RemoveItemBatchElement>();
    }

    public static class BuildResourceManager
    {
        private static uint LerpItemCount(int left, int right, float factor)
        {
            //BuildUtils.Debug?.Report(true, $"left: {left}, right: {right}, factor: {factor}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            var _factor = Math.Max(0.0f, Math.Min(1.0f, factor));
            var result = left * (1.0f - _factor) + right * _factor;
            result = Math.Max(0.0f, result);
            return (uint)Math.Floor(result);
        }

        private static List<ItemResourcePack> GetItemResourcesToAdd(List<KeyValuePair<BuildRecipeDef, float>> elements)
        {
            BuildUtils.Debug?.Report(true, $"elements: {elements.Count}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            var itemResourcesToAdd = new List<ItemResourcePack>();
            foreach (var element in elements)
            {
                if (element.Key == null)
                {
                    return null;
                }
                itemResourcesToAdd.AddRange(element.Key.Resource.Resources.Select(x => new ItemResourcePack(x.Item.Target, LerpItemCount(x.ClaimCount, x.ReclaimCount, element.Value))).ToList());
            }
            return itemResourcesToAdd;
        }

        public static CheckResourcesResult CheckResources(BuildRecipeDef buildRecipeDef, IWorldCharacter worldCharacter)
        {
            BuildUtils.Debug?.Report(true, $"buildRecipeDef: {buildRecipeDef.____GetDebugAddress()}, character: {worldCharacter.Id}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            var result = new CheckResourcesResult();
            if (buildRecipeDef == null)
            {
                result.Result = ResourceOperationResultCode.BuildRecipeIsNull;
                return result;
            }
            bool claimResources = true;
            if (BuildUtils.CheatClaimResourcesEnable)
            {
                claimResources = BuildUtils.CheatClaimResourcesValue;
            }
            else
            {
                claimResources = BuildUtils.BuildParamsDef.ClaimResources;
            }
            if (claimResources)
            {
                var claimFromInventory = BuildUtils.BuildParamsDef.ClaimResourcesfromInventory;
                var claimFromDoll = BuildUtils.BuildParamsDef.ClaimResourcesfromDoll;

                var totalResourceCount = 0;
                if ((buildRecipeDef.Resource.Resources != null) && (buildRecipeDef.Resource.Resources.Count > 0))
                {
                    foreach (var resource in buildRecipeDef.Resource.Resources)
                    {
                        if (resource != null)
                        {
                            totalResourceCount += resource.ClaimCount;
                        }
                    }
                }

                if ((totalResourceCount > 0) && (claimFromInventory || claimFromDoll))
                {
                    var resources = new List<BuildRecipeDef.ElementResource.Resource>();
                    foreach ( var resource in buildRecipeDef.Resource.Resources)
                    {
                        resources.Add(new BuildRecipeDef.ElementResource.Resource()
                        {
                            ClaimCount = resource.ClaimCount,
                            ReclaimCount = resource.ReclaimCount,
                            Item = resource.Item
                        });
                    }
                    if (claimFromInventory && (totalResourceCount > 0))
                    {
                        foreach (var slotItem in worldCharacter.Inventory.Items)
                        {
                            foreach (var resource in resources)
                            {
                                if ((resource.ClaimCount > 0) && (resource.Item.Target == slotItem.Value.Item.ItemResource))
                                {
                                    var removeSlotCount = Math.Min(slotItem.Value.Stack, resource.ClaimCount);
                                    var propertyAddress = EntityPropertyResolver.GetPropertyAddress(worldCharacter.Inventory);
                                    result.ClaimItems.Add(new RemoveItemBatchElement(propertyAddress, slotItem.Key, removeSlotCount, slotItem.Value.Item.Id));
                                    resource.ClaimCount -= removeSlotCount;
                                    totalResourceCount -= removeSlotCount;
                                    break;
                                }
                            }
                            if (totalResourceCount <= 0) { break; }
                        }
                    }
                    if (claimFromDoll && (totalResourceCount > 0))
                    {
                        foreach (var slotItem in worldCharacter.Doll.Items)
                        {
                            foreach (var resource in resources)
                            {
                                if ((resource.ClaimCount > 0) && (resource.Item.Target == slotItem.Value.Item.ItemResource))
                                {
                                    var removeSlotCount = Math.Min(slotItem.Value.Stack, resource.ClaimCount);
                                    var propertyAddress = EntityPropertyResolver.GetPropertyAddress(worldCharacter.Doll);
                                    result.ClaimItems.Add(new RemoveItemBatchElement(propertyAddress, slotItem.Key, removeSlotCount, slotItem.Value.Item.Id));
                                    resource.ClaimCount -= removeSlotCount;
                                    totalResourceCount -= removeSlotCount;
                                    break;
                                }
                            }
                            if (totalResourceCount <= 0) { break; }
                        }
                    }
                }
                if (totalResourceCount > 0)
                {
                    result.Result = ResourceOperationResultCode.ContainerItemOperationNotEnoughResources;
                }
            }
            return result;
        }

        public static async Task<ResourcesOperationResult> ClaimResources(List<RemoveItemBatchElement> claimItems, IEntitiesRepository repository)
        {
            BuildUtils.Debug?.Report(true, $"claimItems: {claimItems.Count}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            var result = new ResourcesOperationResult();
            var removeTransaction = new ItemRemoveBatchManagementTransaction(claimItems, true, repository);
            result.ContainerItemOperationResult = (await removeTransaction.ExecuteTransaction()).Result;
            if (result.ContainerItemOperationResult != ContainerItemOperationResult.Success)
            {
                result.Result = ResourceOperationResultCode.ContainerItemOperationFailed;
            }
            return result;
        }

        public static async Task<ResourcesOperationResult> ReclaimResources(IEntity entity, OuterRef<IEntity> owner, List<KeyValuePair<BuildRecipeDef, float>> elements)
        {
            BuildUtils.Debug?.Report(true, $"entity: {entity.Id}, owner: {owner.Guid}, elements: {elements.Count}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            var result = new ResourcesOperationResult();
            if (entity == null)
            {
                result.Result = ResourceOperationResultCode.BuildingPlaceIsNull;
                return result;
            }
            if ((elements == null) || (elements.Count == 0))
            {
                result.Result = ResourceOperationResultCode.ElementsIsNullOrEmpty;
                return result;
            }
            bool claimResources = true;
            if (BuildUtils.CheatClaimResourcesEnable)
            {
                claimResources = BuildUtils.CheatClaimResourcesValue;
            }
            else
            {
                claimResources = BuildUtils.BuildParamsDef.ClaimResources;
            }
            if (claimResources)
            {
                var entitiesRepository = entity.EntitiesRepository;
                using (var wrapper = await entitiesRepository.Get<IWorldCharacterServer>(owner.Guid))
                {
                    var character = wrapper?.Get<IWorldCharacterServer>(owner.Guid);
                    if (character == null)
                    {
                        result.Result = ResourceOperationResultCode.CharacterNotFound;
                        return result;
                    }
                    var destAddress = EntityPropertyResolver.GetPropertyAddress(character.Inventory);
                    var itemResourcesToAdd = GetItemResourcesToAdd(elements);
                    if (itemResourcesToAdd == null)
                    {
                        result.Result = ResourceOperationResultCode.BuildRecipeIsNull;
                        return result;
                    }
                    if (itemResourcesToAdd.Count > 0)
                    {
                        var itemTransaction = new ItemAddBatchManagementTransaction(itemResourcesToAdd, destAddress, false, entitiesRepository);
                        result.ContainerItemOperationResult = (await itemTransaction.ExecuteTransaction()).Result;
                        if (result.ContainerItemOperationResult != ContainerItemOperationResult.Success)
                        {
                            result.Result = ResourceOperationResultCode.ContainerItemOperationFailed;
                        }
                    }
                }
            }
            return result;
        }

        public static async Task<ResourcesOperationResult> SpawnResources(IEntity entity, Vector3 position, Quaternion rotation, List<KeyValuePair<BuildRecipeDef, float>> elements, Guid worldSpaceId)
        {
            BuildUtils.Debug?.Report(true, $"entity: {entity.Id}, position: {position}, rotation: {rotation}, elements: {elements.Count}, worldSpaceId: {worldSpaceId}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            var result = new ResourcesOperationResult();

            if (entity == null)
            {
                result.Result = ResourceOperationResultCode.BuildingPlaceIsNull;
                return result;
            }
            if ((elements == null) || (elements.Count == 0))
            {
                result.Result = ResourceOperationResultCode.ElementsIsNullOrEmpty;
                return result;
            }
            bool claimResources = true;
            if (BuildUtils.CheatClaimResourcesEnable)
            {
                claimResources = BuildUtils.CheatClaimResourcesValue;
            }
            else
            {
                claimResources = BuildUtils.BuildParamsDef.ClaimResources;
            }

            if (claimResources)
            {
                var entitiesRepository = entity.EntitiesRepository;
                using (var wsWrapper = await entitiesRepository.Get<IWorldSpaceServiceEntityServer>(worldSpaceId))
                {
                    var ws = wsWrapper.Get<IWorldSpaceServiceEntityServer>(worldSpaceId);
                    var boxId = await ws.GetWorldBoxIdToDrop(position, entity.Id);
                    if (boxId == Guid.Empty)
                    {
                        Vector3 boxPosition = (await WorldSpaceServiceEntity.GetDropPosition(entity,
                            entity.EntitiesRepository,
                            new Transform(position, rotation))).Value;

                        var boxResource = BuildUtils.BuildParamsDef.DropBoxDef.Target;
                        var createdBox = await entitiesRepository.Create<IWorldBox>(Guid.NewGuid(), createdItem =>
                        {
                            createdItem.WorldSpaced.OwnWorldSpace =
                                ((IHasWorldSpaced) entity).WorldSpaced.OwnWorldSpace;
                            createdItem.MapOwner = ((IHasMapped) entity).MapOwner;
                            createdItem.Inventory.Size = boxResource.Size;
                            createdItem.Def = boxResource;
                            createdItem.MovementSync.SetPosition = boxPosition;
                            createdItem.OwnerInformation.Owner = new OuterRef<IEntity>(entity);
                            return Task.CompletedTask;
                        });

                        PropertyAddress destAddress = null;
                        using (var createdBoxWrapper = await entitiesRepository.Get(createdBox.TypeId, createdBox.Id))
                        {
                            var createdBoxEntity = createdBoxWrapper.Get<IHasInventoryServer>(createdBox.TypeId,
                                createdBox.Id, ReplicationLevel.Server);
                            if (createdBoxEntity == null)
                            {
                                result.Result = ResourceOperationResultCode.CantGetNewWorldBoxEntity;
                                return result;
                            }

                            destAddress = EntityPropertyResolver.GetPropertyAddress(createdBoxEntity.Inventory);
                        }

                        if (destAddress == null)
                        {
                            result.Result = ResourceOperationResultCode.NewWorldBoxAddressIsNull;
                            return result;
                        }

                        var itemResourcesToAdd = GetItemResourcesToAdd(elements);
                        if (itemResourcesToAdd == null)
                        {
                            result.Result = ResourceOperationResultCode.BuildRecipeIsNull;
                            return result;
                        }

                        if (itemResourcesToAdd.Count > 0)
                        {
                            var itemTransaction = new ItemAddBatchManagementTransaction(itemResourcesToAdd, destAddress,
                                false, entitiesRepository);
                            result.ContainerItemOperationResult = (await itemTransaction.ExecuteTransaction()).Result;
                            if (result.ContainerItemOperationResult != ContainerItemOperationResult.Success)
                            {
                                result.Result = ResourceOperationResultCode.ContainerItemOperationFailed;
                                return result;
                            }
                        }
                    }
                    else
                    {
                        using (var boxWrapper = await entitiesRepository.Get<IWorldBoxServer>(boxId))
                        {
                            var existingBox = boxWrapper.Get<IWorldBoxServer>(boxId);
                            var destAddress = EntityPropertyResolver.GetPropertyAddress(existingBox.Inventory);
                            var itemResourcesToAdd = GetItemResourcesToAdd(elements);
                            if (itemResourcesToAdd == null)
                            {
                                result.Result = ResourceOperationResultCode.BuildRecipeIsNull;
                                return result;
                            }

                            if (itemResourcesToAdd.Count > 0)
                            {
                                var itemTransaction = new ItemAddBatchManagementTransaction(itemResourcesToAdd,
                                    destAddress, false, entitiesRepository);
                                result.ContainerItemOperationResult =
                                    (await itemTransaction.ExecuteTransaction()).Result;
                                if (result.ContainerItemOperationResult != ContainerItemOperationResult.Success)
                                {
                                    result.Result = ResourceOperationResultCode.ContainerItemOperationFailed;
                                    return result;
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }
    }
}
