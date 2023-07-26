using System.Threading.Tasks;
using GeneratedCode.DeltaObjects;
using JetBrains.Annotations;
using Shared.ManualDefsForSpells;
using SharedCode.EntitySystem;

namespace ColonyShared.GeneratedCode.Predicates
{

    [UsedImplicitly]
    public class PredicateCheckCheatVar : IPredicateBinding<PredicateCheckCheatVarDef>
    {
        public ValueTask<bool> True(SpellPredCastData cast, IEntitiesRepository repo, PredicateCheckCheatVarDef def)
        {
            return new ValueTask<bool>(CheatVariables.CheckValue(def.CheatVar.Target, def.Value));
        }
    }
}