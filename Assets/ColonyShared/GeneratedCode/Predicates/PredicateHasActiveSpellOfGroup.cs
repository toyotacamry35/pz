using System.Threading.Tasks;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedDefsForSpells;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;

namespace Assets.Src.Predicates
{
    public class PredicateHasActiveSpellOfGroup : IPredicateBinding<PredicateHasActiveSpellOfGroupDef>
    {
        public async ValueTask<bool> True(SpellPredCastData cast, IEntitiesRepository repo, PredicateHasActiveSpellOfGroupDef def)
        {
            using (var wrapper = await repo.Get(cast.Wizard.TypeId, cast.Wizard.Guid))
            {
                var wizardEntity = wrapper.Get<IWizardEntityClientFull>(cast.Wizard.Guid);
                var selfDef = ((PredicateHasActiveSpellOfGroupDef) def);
                var res = await wizardEntity.HasActiveSpellGroup(selfDef._spellGroup.Target);
                return selfDef.Inversed ? !res : res;
            }
        }
    }
}
