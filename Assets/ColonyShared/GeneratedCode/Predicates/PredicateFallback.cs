using System.Threading.Tasks;
using GeneratedCode.DeltaObjects;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using Src.ManualDefsForSpells;

namespace Assets.Src.Predicates
{
    public class PredicateFallback : IPredicateBinding<PredicateFallbackDef>
    {
        public async ValueTask<bool> True(SpellPredCastData cast, IEntitiesRepository repo, PredicateFallbackDef indef)
        {
            var def = (PredicateFallbackDef) indef;
            foreach (var predicate in def.Predicates)
                if(predicate!=null)
                    if (!await PredicateHelper.CheckPredicate(predicate, cast, repo))
                        return false;
            return true;
        }
    }
}