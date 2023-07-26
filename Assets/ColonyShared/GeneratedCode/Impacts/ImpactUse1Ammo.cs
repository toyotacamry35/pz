using System;
using Assets.ColonyShared.SharedCode.Aspects.Item;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.EntitySystem.EntityPropertyResolvers;
using System.Threading.Tasks;
using SharedCode.EntitySystem;
using Assets.Src.Aspects;
using GeneratedCode.DeltaObjects;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Wizardry;
using Assets.ColonyShared.GeneratedCode.Shared;
using GeneratedDefsForSpells;

namespace Assets.Src.Impacts
{
    public class ImpactUse1Ammo : IImpactBinding<ImpactUse1AmmoDef>
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public async ValueTask Apply(SpellWordCastData cast, IEntitiesRepository repo, ImpactUse1AmmoDef def)
        {
         //   Logger.IfInfo()?.Message($"-----------------------ImpactUse1Ammo-------------------------").Write();

            var characterGuid = cast.Caster.Guid;
            var item = await Assets.ColonyShared.GeneratedCode.Shared.ClusterCommands.GetActiveWeaponResource(characterGuid, repo);
            var weaponDef = (item.Key as ItemResource)?.WeaponDef.Target;
           // Logger.IfInfo()?.Message($"characterGuid = {characterGuid}, item = {item.Key.ItemName}, weaponDef = {weaponDef}").Write();
            if (weaponDef.AssertIfNull(nameof(weaponDef)))
            {
             //   Logger.IfInfo()?.Message($"weaponDef == null").Write();
                return;
            }

            await RemoveOneAmmo(weaponDef, characterGuid, repo);
        }

        private async Task RemoveOneAmmo(WeaponDef rangeWeaponDef, Guid characterGuid, IEntitiesRepository repository)
        {
            //Logger.IfInfo()?.Message($"RemoveOneAmmo").Write();

            using (var worldCharacterWrapper = await repository.Get<IWorldCharacterClientFull>(characterGuid))
            {
                var worldCharacter = worldCharacterWrapper.Get<IWorldCharacterClientFull>(characterGuid);
                if (worldCharacter.Doll.UsedSlots.Count <= 0)
                    return;

                var slotResId = worldCharacter.Doll.UsedSlots[0];
                var usedIndex = ItemHelper.TmpPlug_GetIndexBySlotResourceIdFull(slotResId);

                if (!worldCharacter.Doll.Items.ContainsKey(usedIndex))
                    return;

                var usedItem = worldCharacter.Doll.Items[usedIndex].Item;
              //  Logger.IfInfo()?.Message($"usedItem = {usedItem.ItemResource.ItemName}").Write();

                if (rangeWeaponDef.ContainerUsage == ContainerUsageType.ItSelf)
                {
                    var propertyAddress = EntityPropertyResolver.GetPropertyAddress(worldCharacter.Doll);
                    await worldCharacter.RemoveItem(propertyAddress, usedIndex, 1, worldCharacter.Doll.Items[usedIndex].Item.Id);
                   // Logger.IfInfo()?.Message($"RemoveItem = {usedIndex}, ItSelf").Write();
                }
                else if (rangeWeaponDef.ContainerUsage == ContainerUsageType.InnerContainer)
                {
                    if (!usedItem.AmmoContainer.Items.ContainsKey(0))
                        return;

                    var propertyAddress = EntityPropertyResolver.GetPropertyAddress(usedItem.AmmoContainer);
                    await worldCharacter.RemoveItem(propertyAddress, 0, 1, usedItem.AmmoContainer.Items[0].Item.Id);
                 //   Logger.IfInfo()?.Message($"RemoveItem = {usedIndex}, InnerContainer").Write();
                }
            }
        }
    }
}
