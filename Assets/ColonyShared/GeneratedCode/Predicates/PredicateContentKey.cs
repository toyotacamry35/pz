using GeneratedCode.DeltaObjects;
using ResourceSystem.GeneratedDefsForSpells;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using System.Threading.Tasks;

namespace GeneratedCode.Predicates
{
    public class PredicateContentKey : IPredicateBinding<PredicateContentKeyDef>
    {
        public ValueTask<bool> True(SpellPredCastData cast, IEntitiesRepository repo, PredicateContentKeyDef def)
        {
            return new ValueTask<bool>(ContentKeyServiceEntity.ContainsKey(def?.Key));
        }
    }
}
