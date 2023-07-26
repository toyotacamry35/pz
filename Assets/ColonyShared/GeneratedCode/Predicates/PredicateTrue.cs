using System.Threading.Tasks;
using GeneratedCode.DeltaObjects;
using JetBrains.Annotations;
using ResourceSystem.Aspects.ManualDefsForSpells;
using SharedCode.EntitySystem;

namespace ColonyShared.GeneratedCode.Predicates
{
    [UsedImplicitly] 
    public class PredicateTrue : IPredicateBinding<SpellPredicateTrueDef>
    {
        public ValueTask<bool> True(SpellPredCastData cast, IEntitiesRepository repo, SpellPredicateTrueDef def)
        {
            return new ValueTask<bool>(true);
        }
    }
}