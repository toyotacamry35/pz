using System.Threading.Tasks;
using GeneratedCode.DeltaObjects;
using GeneratedDefsForSpells;
using JetBrains.Annotations;
using SharedCode.Aspects.Item.Templates;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;

namespace Assets.Src.Predicates
{
    [UsedImplicitly]
    public class PredicateCheckIfPlayerInBuildingRange : IPredicateBinding<PredicateCheckIfPlayerInBuildingRangeDef>
    {
        public ValueTask<bool> True(SpellPredCastData cast, IEntitiesRepository repo, PredicateCheckIfPlayerInBuildingRangeDef def)
        {
            return new ValueTask<bool>(true);
            //return await PredicateCheckIfPlayerInRange.True(cast, repo, def, Constants.WorldConstants.BuildingInteractDistance, Constants.WorldConstants.PlayerHeight);
        }
    }
}
