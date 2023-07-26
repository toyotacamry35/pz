using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using JetBrains.Annotations;
using NLog;
using SharedCode.Entities;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.EntityPropertyResolvers;
using SharedCode.Utils.Extensions;
using SharedCode.Wizardry;

namespace Shared.ManualDefsForSpells
{
    [UsedImplicitly]
    public class ImpactGrantLootTableItem : IImpactBinding<ImpactGrantLootTableItemDef>
    {
        [NotNull]
        private static readonly NLog.Logger Logger = LogManager.GetLogger("ImpactGrantLootTableItem");

        public async ValueTask Apply(SpellWordCastData cast, IEntitiesRepository repo, ImpactGrantLootTableItemDef def)
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"#Dbg:  [A1]  `ImpactGrantLootTableItem()` Def:{def}").Write();

            List<ItemResourcePack> itemsToGrant;

            var target = cast.CastData.GetParameter(typeof(SpellCastParameterTarget)).Value.OuterRef.To<IEntity>();
            using (var lootableWrapper = await repo.Get(target.RepTypeId(ReplicationLevel.Server), target.Guid))
            {
                var lootable = lootableWrapper.Get<IHasLootableServer>(target.RepTypeId(ReplicationLevel.Server), target.Guid);
                if (lootable == null)
                {
                    Logger.IfError()?.Message($"`{nameof(IHasLootableServer)}` target entity is null").Write();
                    return;
                }

                itemsToGrant = (await lootable.Lootable.GetLootList(cast.Caster.Guid))?.ToList();

                if (def.DieAfterLoot)
                {
                    var mortal = lootableWrapper.Get<IHasMortalServer>(target.RepTypeId(ReplicationLevel.Server), target.Guid);
                    if (mortal != null)
                        await mortal.Mortal.Die();
                }
            }

            if (itemsToGrant == null || itemsToGrant.Count == 0)
            {
                if (Logger.IsDebugEnabled)  Logger.IfDebug()?.Message("No items to grant.").Write();;
                return;
            }
            
            using (var casterWrapper = await repo.Get(cast.Caster.RepTypeId(ReplicationLevel.Server), cast.Caster.Guid))
            {
                var character = casterWrapper.Get<IWorldCharacterServer>(cast.Caster.RepTypeId(ReplicationLevel.Server), cast.Caster.Guid);
                if (character == null)
                {
                    Logger.IfError()?.Message($"`{nameof(IWorldCharacterServer)}` caster entity is null").Write();
                    return;
                }
                var containerPropertyAddress = EntityPropertyResolver.GetPropertyAddress(character.Inventory);

                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"#Dbg:  [A1.1]  character.AddItems [{string.Join(", ", itemsToGrant)}]").Write();

                await character.AddItems(itemsToGrant, containerPropertyAddress);
            }

            }
    }
}
