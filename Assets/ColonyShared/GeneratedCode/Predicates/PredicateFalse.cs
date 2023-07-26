using System.Threading.Tasks;
using GeneratedCode.DeltaObjects;
using JetBrains.Annotations;
using ResourceSystem.Aspects.ManualDefsForSpells;
using SharedCode.EntitySystem;

namespace ColonyShared.GeneratedCode.Predicates
{
    [UsedImplicitly] 
    public class PredicateFalse : IPredicateBinding<SpellPredicateFalseDef>
    {
        public ValueTask<bool> True(SpellPredCastData cast, IEntitiesRepository repo, SpellPredicateFalseDef def)
        {
            return new ValueTask<bool>(false);
        }
    }
}