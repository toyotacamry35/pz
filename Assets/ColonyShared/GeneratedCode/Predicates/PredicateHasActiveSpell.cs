using System.Threading.Tasks;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedDefsForSpells;
using SharedCode.EntitySystem;

namespace Assets.Src.Predicates
{
    public class PredicateHasActiveSpell : IPredicateBinding<PredicateHasActiveSpellDef>
    {
        public async ValueTask<bool> True(SpellPredCastData cast, IEntitiesRepository repo, PredicateHasActiveSpellDef def)
        {
            using (var wrapper = await repo.Get(cast.Wizard.TypeId, cast.Wizard.Guid))
            {
                var wizardEntity = wrapper.Get<IWizardEntityClientFull>(cast.Wizard.Guid);
                var selfDef = ((PredicateHasActiveSpellDef) def);
                var res = await wizardEntity.HasActiveSpell(selfDef._spell.Target);
                return selfDef.Inversed ? !res : res;
            }
        }
    }
}
