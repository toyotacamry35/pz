using System.Threading.Tasks;
using GeneratedCode.DeltaObjects;
using GeneratedDefsForSpells;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;

namespace Assets.Src.Predicates.Specific
{
    public class CheckPredicateHasWorldBuildingItems : IPredicateBinding<CheckPredicateHasWorldBuildingItemsDef>
    {
        //public CasterValue Caster;
        //public TargetValue Target;

        //protected override bool TrueInternal(PredicateIgnoreGroup[] predicateIgnoreGroups)
        //{
        //    var casterEntityData = Caster?.Go?.transform?.root?.GetComponent<PawnEntityData>();
        //    if (casterEntityData == null)
        //        return false;

        //    var buildingEntityData = Target?.Go?.transform?.root?.GetComponent<PawnEntityData>();
        //    if (buildingEntityData == null)
        //        return false;

        //    var repository = ServerProvider.Server != null ? ServerProvider.Server.UnityEntitiesRepository : NodeAccessor.Repository;
        //    if (repository.AssertIfNull(nameof(repository)))
        //        return false;

        //    return true;
        //}

        public ValueTask<bool> True(SpellPredCastData cast, IEntitiesRepository repo, CheckPredicateHasWorldBuildingItemsDef def)
        {
            return new ValueTask<bool>(true);
        }
    }
}