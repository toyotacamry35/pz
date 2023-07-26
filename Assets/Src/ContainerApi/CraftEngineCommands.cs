using System;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Aspects.Craft;
using Assets.ColonyShared.SharedCode.Shared;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedCode.Repositories;
using ResourceSystem.Utils;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Entities.Engine;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.EntityPropertyResolvers;
using Uins;

namespace Assets.Src.ContainerApis
{
    public class CraftEngineCommands
    {
        /// <summary>
        /// Получает у персонажа по characterOuterRef адреса контейнеров инвентаря и куклы, которые возвращает в action.
        /// Выполняется в threadpool, возвращает результат в unity thread
        /// </summary>
        public static async void GetCharacterContainersPropertyAddressesAsync(
            OuterRef<IEntityObject> characterOuterRef,
            Action<PropertyAddress, PropertyAddress, OuterRef> action)
        {
            PropertyAddress inventoryPropertyAddress = null;
            PropertyAddress dollPropertyAddress = null;
            OuterRef craftEngineOuterRef;
            if (!characterOuterRef.IsValid)
            {
                UI.Logger.IfError()?.Message($"{nameof(characterOuterRef)} is invalid").Write();
                return;
            }

            var repo = NodeAccessor.Repository;
            if (repo.AssertIfNull(nameof(repo)))
                return;

            var replicationTypeId = EntitiesRepository.GetReplicationTypeId(characterOuterRef.TypeId, ReplicationLevel.ClientFull);
            var entityGuid = characterOuterRef.Guid;

            using (var wrapper = await repo.Get(replicationTypeId, entityGuid, (object) nameof(GetCharacterContainersPropertyAddressesAsync)))
            {
                var worldCharacterClientFull = wrapper?.Get<IWorldCharacterClientFull>(replicationTypeId, entityGuid);
                if (worldCharacterClientFull == null)
                {
                    UI.Logger.IfError()?.Message($"{nameof(worldCharacterClientFull)} is null ({characterOuterRef})").Write();
                    return;
                }

                inventoryPropertyAddress = EntityPropertyResolver.GetPropertyAddress(worldCharacterClientFull.Inventory);
                dollPropertyAddress = EntityPropertyResolver.GetPropertyAddress(worldCharacterClientFull.Doll);
                craftEngineOuterRef = worldCharacterClientFull.CraftEngine.OuterRef;
            }

            UnityQueueHelper.RunInUnityThreadNoWait(() => action?.Invoke(inventoryPropertyAddress, dollPropertyAddress, craftEngineOuterRef));
        }

        /// <summary>
        /// Делает запрос в CraftEngine имеющий craftEngineOuterRef, будет ли успешен крафт рецепта recipe с параметрами, возвращает результат в onResult. 
        /// Выполняется в threadpool, возвращает результат в unity thread
        /// </summary>
        public static async Task IsCraftAvailableAsync(
            OuterRef craftEngineOuterRef,
            PropertyAddress inventoryPropertyAddress,
            PropertyAddress dollPropertyAddress,
            CraftRecipeDef recipe,
            int variant,
            int[] pathItem,
            int[] pathItemOptional,
            int count,
            BaseItemResource missingItem,
            Action<bool> onResult)
        {
            if (!craftEngineOuterRef.IsValid)
            {
                UI.Logger.IfError()?.Message($"{nameof(craftEngineOuterRef)} is invalid").Write();
                return;
            }

            var repo = NodeAccessor.Repository;
            if (repo.AssertIfNull(nameof(repo)))
                return;

            var replicationTypeId = EntitiesRepository.GetReplicationTypeId(craftEngineOuterRef.TypeId, ReplicationLevel.ClientFull);
            var entityGuid = craftEngineOuterRef.Guid;
            using (var wrapper = await repo.Get(replicationTypeId, entityGuid, (object) nameof(IsCraftAvailableAsync)))
            {
                var craftEngineClientFull = wrapper?.Get<ICraftEngineClientFull>(replicationTypeId, entityGuid);
                if (craftEngineClientFull == null)
                {
                    UI.Logger.IfError()?.Message($"{nameof(craftEngineClientFull)} is null ({nameof(craftEngineOuterRef)}: {craftEngineOuterRef})").Write();
                    return;
                }

                var items = await CraftEngine.GetCraftingItems(
                    inventoryPropertyAddress,
                    dollPropertyAddress,
                    recipe,
                    variant,
                    count,
                    pathItem,
                    pathItemOptional,
                    missingItem,
                    repo);

                var isSuccess = items != null;
                UnityQueueHelper.RunInUnityThreadNoWait(() => onResult?.Invoke(isSuccess));
            }
        }

        public static async Task CraftAvailableMaxCountAsync(
            OuterRef craftEngineOuterRef,
            PropertyAddress inventoryPropertyAddress,
            PropertyAddress dollPropertyAddress,
            CraftRecipeDef recipe,
            int variant,
            int[] pathItem,
            int[] pathItemOptional,
            BaseItemResource missingItem,
            Action<int> maxCountAction)
        {
            if (!craftEngineOuterRef.IsValid)
            {
                UI.Logger.IfError()?.Message($"{nameof(craftEngineOuterRef)} is invalid").Write();
                return;
            }

            var repo = NodeAccessor.Repository;
            if (repo.AssertIfNull(nameof(repo)))
                return;

            var replicationTypeId = EntitiesRepository.GetReplicationTypeId(craftEngineOuterRef.TypeId, ReplicationLevel.ClientFull);
            var entityGuid = craftEngineOuterRef.Guid;
            using (var wrapper = await repo.Get(replicationTypeId, entityGuid, (object) nameof(IsCraftAvailableAsync)))
            {
                var craftEngineClientFull = wrapper?.Get<ICraftEngineClientFull>(replicationTypeId, entityGuid);
                if (craftEngineClientFull == null)
                {
                    UI.Logger.IfError()?.Message($"{nameof(craftEngineClientFull)} is null ({nameof(craftEngineOuterRef)}: {craftEngineOuterRef})").Write();
                    return;
                }

                var maxCount = await CraftEngine.GetCraftingItemsMaxCount(
                    inventoryPropertyAddress,
                    dollPropertyAddress,
                    recipe,
                    variant,
                    pathItem,
                    pathItemOptional,
                    missingItem,
                    repo);

                UnityQueueHelper.RunInUnityThreadNoWait(() => maxCountAction?.Invoke(maxCount));
            }
        }

        /// <summary>
        /// Используется после клиентских проверок на достаточность ингридиентов и корректность данных.
        /// Выполняется в threadpool, возвращает результат в unity thread
        /// </summary>
        public static async Task DoCraftAsync(
            OuterRef craftEngineOuterRef,
            PropertyAddress inventoryPropertyAddress,
            PropertyAddress dollPropertyAddress,
            CraftRecipeDef recipe,
            int variant,
            int[] pathItem,
            int[] pathItemOptional,
            int count,
            Action<CraftOperationResult> onResult)
        {
            if (!craftEngineOuterRef.IsValid)
            {
                UI.Logger.IfError()?.Message($"{nameof(craftEngineOuterRef)} is invalid").Write();
                return;
            }

            var repo = NodeAccessor.Repository;
            if (repo.AssertIfNull(nameof(repo)))
                return;

            var replicationTypeId = EntitiesRepository.GetReplicationTypeId(craftEngineOuterRef.TypeId, ReplicationLevel.ClientFull);
            var entityGuid = craftEngineOuterRef.Guid;
            using (var wrapper = await repo.Get(replicationTypeId, entityGuid, (object) nameof(DoCraftAsync)))
            {
                var craftEngineClientFull = wrapper?.Get<ICraftEngineClientFull>(replicationTypeId, entityGuid);
                if (craftEngineClientFull == null)
                {
                    UI.Logger.IfError()?.Message($"{nameof(craftEngineClientFull)} is null ({nameof(craftEngineOuterRef)}: {craftEngineOuterRef})").Write();
                    return;
                }

                var res = await craftEngineClientFull.Craft(recipe, variant, count, pathItem, pathItemOptional, inventoryPropertyAddress, dollPropertyAddress);
                UnityQueueHelper.RunInUnityThreadNoWait(() => onResult.Invoke(res));
            }
        }


        public static async Task CancelCraftAsync(OuterRef craftEngineOuterRef, int queueIndex)
        {
            if (!craftEngineOuterRef.IsValid)
            {
                UI.Logger.IfError()?.Message($"{nameof(craftEngineOuterRef)} is invalid").Write();
                return;
            }

            var repo = NodeAccessor.Repository;
            if (repo.AssertIfNull(nameof(repo)))
                return;

            var replicationTypeId = EntitiesRepository.GetReplicationTypeId(craftEngineOuterRef.TypeId, ReplicationLevel.ClientFull);
            var entityGuid = craftEngineOuterRef.Guid;
            using (var wrapper = await repo.Get(replicationTypeId, entityGuid, (object) nameof(CancelCraftAsync)))
            {
                var craftEngineClientFull = wrapper?.Get<ICraftEngineClientFull>(replicationTypeId, entityGuid);
                if (craftEngineClientFull == null)
                {
                    UI.Logger.IfError()?.Message($"{nameof(craftEngineClientFull)} is null ({nameof(craftEngineOuterRef)}: {craftEngineOuterRef})").Write();
                    return;
                }

                var res = await craftEngineClientFull.RemoveCraft(queueIndex);
                UI.Logger.IfDebug()?.Message($"{nameof(CancelCraftAsync)}: {res}").Write();
            }
        }
    }
}