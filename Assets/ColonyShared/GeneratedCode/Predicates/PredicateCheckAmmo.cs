
using Assets.Src.Aspects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using System.Linq;
using System.Threading.Tasks;
using GeneratedCode.DeltaObjects;
using GeneratedCode.Repositories;
using SharedCode.Aspects.Item.Templates;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.EntityPropertyResolvers;
using SharedCode.Wizardry;
using Assets.ColonyShared.SharedCode.Aspects.Item;
using Assets.ColonyShared.GeneratedCode.Shared;
using GeneratedDefsForSpells;

namespace Assets.Src.Predicates
{
    class PredicateCheckAmmo : IPredicateBinding<PredicateCheckAmmoDef>
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public async ValueTask<bool> True(SpellPredCastData cast, IEntitiesRepository repository, PredicateCheckAmmoDef def)
        {
            //Logger.IfInfo()?.Message("PredicateCheckAmmo - 0").Write();
            var characterGuid = cast.Caster.Guid;
            var item = await ClusterCommands.GetActiveWeaponResource(characterGuid, repository);
            var weaponDef = (item.Key as ItemResource)?.WeaponDef.Target;
            //Logger.IfInfo()?.Message($"characterGuid = {characterGuid}, item = {item.Key.ItemName}, weaponDef = {weaponDef}").Write();
            if (weaponDef.AssertIfNull(nameof(weaponDef)))
            {
             //   Logger.IfInfo()?.Message($"weaponDef == null").Write();
                return false;
            }

            using (var worldCharacterWrapper = await repository.Get<IWorldCharacterClientFull>(characterGuid))
            {
                var worldCharacter = worldCharacterWrapper.Get<IWorldCharacterClientFull>(characterGuid);
                if (worldCharacter.Doll.UsedSlots.Count <= 0)
                    return false;

                var slotResId = worldCharacter.Doll.UsedSlots[0];
                var usedIndex = ItemHelper.TmpPlug_GetIndexBySlotResourceIdFull(slotResId);

                if (!worldCharacter.Doll.Items.ContainsKey(usedIndex))
                    return false;

                var usedItem = worldCharacter.Doll.Items[usedIndex].Item;
               // Logger.IfInfo()?.Message($"usedItem = {usedItem.ItemResource.ItemName}").Write();

                if (weaponDef.ContainerUsage == ContainerUsageType.ItSelf)
                {
                //    Logger.IfInfo()?.Message("activeWeaponDef.ContainerUsage == ContainerUsageType.ItSelf").Write();
                    return true;
                }
                else if (weaponDef.ContainerUsage == ContainerUsageType.InnerContainer)
                {
                  //  Logger.IfInfo()?.Message("activeWeaponDef.ContainerUsage == ContainerUsageType.InnerContainer").Write();
                    var typeId = EntitiesRepository.GetReplicationTypeId(item.Value.EntityTypeId, ReplicationLevel.ClientFull);
                    using (var wrapper = await repository.Get(typeId, item.Value.EntityId))
                    {
                        var entity = wrapper.Get<IEntity>(typeId, item.Value.EntityId);
                        var items = EntityPropertyResolver.Resolve<IItemClientFull>(entity, item.Value);
                    //    Logger.IfInfo()?.Message("PredicateCheckAmmo - 10").Write();
                        if (items.AmmoContainer.Items.Any())
                        {
                     //       Logger.IfInfo()?.Message("items.AmmoContainer.Items.Any() == true").Write();
                            return true;
                        }
                    }
                    return false;
                }
            }
            /*
            Logger.IfInfo()?.Message("PredicateCheckAmmo - 0").Write();

            if (cast.IsSlave)
                return true;

            Logger.IfInfo()?.Message("PredicateCheckAmmo - 1").Write();

            var characterGuid = cast.Caster.Guid;
            var activeWeapon = await ClusterCommands.GetActiveWeaponResource(characterGuid, repo);
            Logger.IfInfo()?.Message($"activeWeapon = {activeWeapon.Key}").Write();
            if (activeWeapon.Key == null)
                return false;

            WeaponDef activeWeaponDef = activeWeapon.Key.WeaponDef;
            Logger.IfInfo()?.Message($"activeWeaponDef = {activeWeaponDef}").Write();
            if (activeWeaponDef.AssertIfNull(nameof(activeWeaponDef)))
                return false;

            if (activeWeaponDef.ContainerUsage == ContainerUsageType.ItSelf)
            {
                Logger.IfInfo()?.Message("activeWeaponDef.ContainerUsage == ContainerUsageType.ItSelf").Write();
                return true;
            }
            
            if (activeWeaponDef.ContainerUsage == ContainerUsageType.InnerContainer)
            {
                Logger.IfInfo()?.Message("activeWeaponDef.ContainerUsage == ContainerUsageType.InnerContainer").Write();
                var typeId = EntitiesRepositoryBase.GetReplicationTypeId(activeWeapon.Value.EntityTypeId, ReplicationLevel.Server);
                using (var wrapper = await repo.Get(typeId, activeWeapon.Value.EntityId))
                {
                    var entity = wrapper.Get<IEntity>(typeId, activeWeapon.Value.EntityId);
                    var item = EntityPropertyResolver.Resolve<IItemServer>(entity, activeWeapon.Value);
                    Logger.IfInfo()?.Message("PredicateCheckAmmo - 0").Write();
                    if (item.AmmoContainer.Items.Any())
                        return true;
                }
                return false;
            }*/

            return false;
        }
    }
}