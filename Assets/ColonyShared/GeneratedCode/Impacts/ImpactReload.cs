using Assets.Src.Aspects;
using GeneratedCode.Custom.Containers;
using SharedCode.Entities;
using System;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Aspects.Item;
using SharedCode.EntitySystem;
using GeneratedCode.DeltaObjects;
using SharedCode.DeltaObjects;
using SharedCode.Aspects.Item.Templates;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.Wizardry;
using GeneratedCode.Transactions;
using SharedCode.EntitySystem.EntityPropertyResolvers;
using NLog;
using Assets.ColonyShared.GeneratedCode.Shared;
using Core.Environment.Logging.Extension;
using GeneratedDefsForSpells;
using GeneratedCode.EntitySystem;

namespace Assets.Src.Impacts
{
    public class ImpactReload : IImpactBinding<ImpactReloadDef>
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public async ValueTask Apply(SpellWordCastData cast, IEntitiesRepository repo, ImpactReloadDef def)
        {
            if (cast.IsSlave)
                return;

            var characterGuid = cast.Caster.Guid;
            var activeWeapon = await ClusterCommands.GetActiveWeaponResource(characterGuid, repo);
            var weaponDef = (activeWeapon.Key as ItemResource)?.WeaponDef.Target;
            if (weaponDef.AssertIfNull(nameof(weaponDef)))
                return;
            await Reload(weaponDef, characterGuid, repo);
        }

        protected async Task Reload(WeaponDef rangeWeaponDef, Guid characterGuid, IEntitiesRepository repo)
        {
            var repository = repo;
            using (var worldCharacterWrapper = await repository.Get<IWorldCharacterClientFull>(characterGuid))
            {
                if (worldCharacterWrapper.AssertIfNull(nameof(worldCharacterWrapper)))
                    return;

                var worldCharacter = worldCharacterWrapper.Get<IWorldCharacter>(characterGuid);
                if (worldCharacter.AssertIfNull(nameof(worldCharacter)))
                    return;

                if (worldCharacter.Doll.UsedSlots.Count > 0)
                {
                    var slotResId = worldCharacter.Doll.UsedSlots[0];
                    var usedIndex = ItemHelper.TmpPlug_GetIndexBySlotResourceIdFull(slotResId);

                    if (worldCharacter.Doll.Items.ContainsKey(usedIndex))
                    {
                        var activeWeapon = worldCharacter.Doll.Items[usedIndex].Item;

                        if (activeWeapon == null)
                            return;

                        var ammoContainer = activeWeapon.AmmoContainer;
                        if (ammoContainer == null)
                            return;

                        int ammoLoadedCount = (ammoContainer.Items == null || ammoContainer.Items.Count == 0) ? 0 : ammoContainer.Items[0].Stack;

                        if (rangeWeaponDef.ContainerUsage == ContainerUsageType.ItSelf)
                        {
                            // Слишком типа халявно перезаряжатся я могу, типа так нельзя... 
                            // готов поспорить, что потом дизайнеры скажут что надо бы вернуть это функционал, поэтому нижние строчки не удаляем
                            //int slotIndex = ContainerUtils.FindResourceSlotIndex(worldCharacter.Inventory, activeWeapon.ItemResource.Resource);
                            //if (slotIndex >= 0)
                            //    await ContainerUtils.MoveItem(slotIndex, usedIndex, worldCharacter.Inventory.Items[slotIndex].Stack, Guid.Empty, worldCharacter.Inventory, worldCharacter.Doll, false, repository);
                            return;
                        }
                        else if (rangeWeaponDef.ContainerUsage == ContainerUsageType.InnerContainer)
                        {
                            var ammoSlot = GetAmmoSlotIndex(ammoLoadedCount, ammoContainer, activeWeapon.ItemResource as ItemResource, worldCharacter.To<IWorldCharacterClientFull>());
                            if (ammoSlot.Index < 0)
                                return;

                            var ammoNeedToLoadCount = rangeWeaponDef.MaxInnerStack - ammoLoadedCount;
                            if (ammoNeedToLoadCount <= 0)
                                return;

                            var count = Math.Min(ammoSlot.IsInventory ? 
                                worldCharacter.Inventory.Items[ammoSlot.Index].Stack :
                                worldCharacter.Doll.Items[ammoSlot.Index].Stack
                                , ammoNeedToLoadCount);

                            PropertyAddress ammoAddress;
                            if (EntityPropertyResolver.TryGetPropertyAddress(ammoContainer, out ammoAddress))
                            {
                                var result = await (new ItemMoveManagementTransaction(
                                    EntityPropertyResolver.GetPropertyAddress(ammoSlot.IsInventory ? worldCharacter.Inventory as IDeltaObject : worldCharacter.Doll as IDeltaObject),
                                    ammoSlot.Index,
                                    ammoAddress,
                                    -1,
                                    count,
                                    Guid.Empty,
                                    false,
                                    repository)).ExecuteTransaction();

                                if (!result.IsSuccess)
                                    Logger.IfError()?.Message($"ItemMoveManagementTransaction Error: {result.Result}").Write();
                            }
                            else
                            {
                                Logger.IfError()?.Message("Can not get Ammo Container Address").Write();
                            }
                        }
                    }
                }
            }
        }

        private SlotIndex GetAmmoSlotIndex(int ammoLoadedCount, IContainer ammoContainer, ItemResource itemResource, IWorldCharacterClientFull worldCharacter)
        {
            SlotIndex ammoSlot = new SlotIndex() { Index = -1 };
            if (itemResource == null)
                return ammoSlot;

            int ammoSlotIndex = -1;
            if (ammoLoadedCount == 0)
            {
                foreach (var ammo in itemResource.ApplicableInnerItems)
                {
                    ammoSlotIndex = ContainerUtils.FindResourceSlotIndex(worldCharacter.Inventory, ammo.Target);
                    if (ammoSlotIndex >= 0)
                    {
                        ammoSlot.Index = ammoSlotIndex;
                        ammoSlot.IsInventory = true;
                        break;
                    }
                    else
                    {
                        ammoSlotIndex = ContainerUtils.FindResourceSlotIndex(worldCharacter.Doll, ammo.Target);
                        if (ammoSlotIndex >= 0)
                        {
                            ammoSlot.Index = ammoSlotIndex;
                            ammoSlot.IsInventory = false;
                            break;
                        }
                    }
                }
            }
            else
            {
                ammoSlotIndex = ContainerUtils.FindResourceSlotIndex(worldCharacter.Inventory, ammoContainer.Items[0].Item.ItemResource);
                if (ammoSlotIndex >= 0)
                {
                    ammoSlot.Index = ammoSlotIndex;
                    ammoSlot.IsInventory = true;
                }
                else
                {
                    ammoSlotIndex = ContainerUtils.FindResourceSlotIndex(worldCharacter.Doll, ammoContainer.Items[0].Item.ItemResource);
                    if (ammoSlotIndex >= 0)
                    {
                        ammoSlot.Index = ammoSlotIndex;
                        ammoSlot.IsInventory = false;
                    }
                }
            }

            return ammoSlot;
        }

        struct SlotIndex
        {
            public int Index;
            public bool IsInventory;
        }
    }
}