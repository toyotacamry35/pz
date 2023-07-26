using System.Threading.Tasks;
using ColonyShared.ManualDefsForSpells;
using GeneratedCode.DeltaObjects;
using SharedCode.EntitySystem;
 
namespace Assets.Src.Predicates
{
    public class PredicateLogicalOr : IPredicateBinding<PredicateLogicalOrDef>
    {
        public async ValueTask<bool> True(SpellPredCastData cast, IEntitiesRepository repo, PredicateLogicalOrDef def)
        {
            foreach (var predicate in def.Predicates)
                if (await PredicateHelper.CheckPredicate(predicate, cast, repo))
                    return true;
            return false;
        }
    }
}