using System.Collections.Generic;
using System.Threading.Tasks;
using ColonyShared.ManualDefsForSpells;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.Entities;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.EntityPropertyResolvers;
using SharedCode.Wizardry;
using Assets.ColonyShared.SharedCode.Arithmetic;
using Assets.ColonyShared.SharedCode.Aspects.WorldObjects;
using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;

namespace Src.Impacts
{
    public class ImpactAddPerk : IImpactBinding<ImpactAddPerkDef>
    {
        public async ValueTask Apply(SpellWordCastData cast, IEntitiesRepository repo, ImpactAddPerkDef indef)
        {
            var def = (ImpactAddPerkDef)indef;
            var targetRef = cast.Caster;
            if (def.Target.Target != null)
                targetRef = await def.Target.Target.GetOuterRef(cast, repo);
            if (!targetRef.IsValid)
                targetRef = cast.Caster;
            using (var ec = await repo.Get(targetRef.TypeId, targetRef.Guid))
            {
                var character = ec.Get<IWorldCharacterServer>(targetRef.TypeId, targetRef.Guid, ReplicationLevel.Server);
                var inventoryAddress = EntityPropertyResolver.GetPropertyAddress(character.TemporaryPerks);

                var perks = new List<ItemResourcePack>();
                switch (def.IsItemPackOrSubTable)
                {
                    case ItemPackOrSubTable.ItemPack:
                        perks.Add(new ItemResourcePack(def.Perk.Target, 1));
                        break;
                    case ItemPackOrSubTable.SubTable:
                        perks.AddRange(await def.LootTable.Target.CalcItems(new LootListRequest(character.Id), cast.Caster.Guid, cast.Caster.TypeId, repo));
                        break;
                }
                await character.AddItems(perks, inventoryAddress);
            }
            }
    }
}
