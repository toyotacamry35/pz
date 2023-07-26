using System.Threading.Tasks;
using ColonyShared.ManualDefsForSpells;
using GeneratedCode.DeltaObjects;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;

namespace Assets.Src.Predicates
{
    public class PredicateLogicalNot : IPredicateBinding<PredicateLogicalNotDef>
    {
        public async ValueTask<bool> True(SpellPredCastData cast, IEntitiesRepository repo, PredicateLogicalNotDef def)
        {
            return !await PredicateHelper.CheckPredicate(def.Predicate, cast, repo);
        }
    }
}