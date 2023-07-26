using Assets.ColonyShared.SharedCode.Aspects.Item;
using Assets.Src.Aspects;
using GeneratedCode.Custom.Containers;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.EntitySystem;
using System;
using System.Threading.Tasks;
using GeneratedCode.DeltaObjects;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Wizardry;
using Assets.ColonyShared.GeneratedCode.Shared;
using GeneratedDefsForSpells;

namespace Assets.Src.Predicates
{
    class PredicateCheckReload : IPredicateBinding<PredicateCheckReloadDef>
    {
        public async ValueTask<bool> True(SpellPredCastData cast, IEntitiesRepository repo, PredicateCheckReloadDef def)
        {
            if (cast.IsSlave)
                return true;

            var characterGuid = cast.Caster.Guid;
            var weapon = await ClusterCommands.GetActiveWeaponResource(characterGuid, repo);
            var weaponDef = (weapon.Key as ItemResource)?.WeaponDef.Target;
            if (weaponDef.AssertIfNull(nameof(weaponDef)))
                return false;

            return await CanReload(weaponDef, characterGuid, repo);
        }

        public async ValueTask<bool> CanReload(WeaponDef rangeWeaponDef, Guid characterGuid, IEntitiesRepository repository)
        {
            using (var worldCharacterWrapper = await repository.Get<IWorldCharacterClientFull>(characterGuid))
            {
                var worldCharacter = worldCharacterWrapper.Get<IWorldCharacterClientFull>(characterGuid);
                if (worldCharacter.Doll.UsedSlots.Count > 0)
                {
                    var slotResId = worldCharacter.Doll.UsedSlots[0];
                    var usedIndex = ItemHelper.TmpPlug_GetIndexBySlotResourceIdFull(slotResId);

                    if (worldCharacter.Doll.Items.ContainsKey(usedIndex))
                    {
                        var activeWeapon = worldCharacter.Doll.Items[usedIndex].Item;
                        if (rangeWeaponDef.ContainerUsage == ContainerUsageType.ItSelf)
                        {
                            //int slotIndex = ContainerUtils.FindResourceSlotIndex(worldCharacter.Inventory, activeWeapon.ItemResource.Resource);
                            //if (slotIndex >= 0)
                            //    return true;
                            return false;
                        }
                        else if (rangeWeaponDef.ContainerUsage == ContainerUsageType.InnerContainer)
                        {
                            var resource = activeWeapon.ItemResource as ItemResource;
                            if (resource == null)
                                return false;

                            if (activeWeapon.AmmoContainer.Items.Count == 0 || activeWeapon.AmmoContainer.Items[0].Stack == 0)
                            {
                                foreach (var ammo in resource.ApplicableInnerItems)
                                {
                                    return ContainerUtils.FindResourceSlotIndex(worldCharacter.Inventory, ammo.Target) >= 0 || ContainerUtils.FindResourceSlotIndex(worldCharacter.Doll, ammo.Target) >= 0;
                                }
                            }
                            else if (activeWeapon.AmmoContainer.Items[0].Stack < resource.WeaponDef.Target.MaxInnerStack)
                            {
                                var slotItem = activeWeapon.AmmoContainer.Items[0];
                                if (slotItem.Stack < slotItem.Item.ItemResource.MaxStack)
                                {
                                    var itemResource = activeWeapon.AmmoContainer.Items[0].Item.ItemResource;
                                    return ContainerUtils.FindResourceSlotIndex(worldCharacter.Inventory, itemResource) >= 0 || ContainerUtils.FindResourceSlotIndex(worldCharacter.Doll, itemResource) >= 0;
                                }
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                }
            }

            return false;
        }
    }
}