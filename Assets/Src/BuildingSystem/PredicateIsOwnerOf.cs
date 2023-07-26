using Assets.Src.Wizardry;
using GeneratedCode.DeltaObjects;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using System.Threading.Tasks;

namespace Assets.Src.BuildingSystem
{
    class PredicateIsOwnerOf : IPredicateBinding<PredicateIsOwnerOfDef>
    {
        public async ValueTask<bool> True(SpellPredCastData cast, IEntitiesRepository repo, PredicateIsOwnerOfDef def)
        {
            var selfDef = (PredicateIsOwnerOfDef)def;
            if (selfDef.Override) { return selfDef.Result; }
            if (!cast.IsSlave || !cast.SlaveMark.OnClient) { return true; }
            var result = false;
            var casterGo = cast.GetCaster();
            if ((casterGo != null) && (cast.Caster != null))
            {
                var ownerID = cast.Caster.Guid;
                result = await UnityQueueHelper.RunInUnityThread(() =>
                {
                    if (!(BuildSystem.Builder?.IsEnabled ?? false)) { return false; }
                    var targetHolder = casterGo.GetComponentInChildren<TargetHolder>();
                    var interactiveBuildingElement = targetHolder?.CurrentTarget.Value;
                    if (interactiveBuildingElement != null)
                    {
                        var buildingElementBehaviour = interactiveBuildingElement.GetComponentInParent<BuildingElementBehaviour>();
                        if (buildingElementBehaviour != null)
                        {
                            var data = buildingElementBehaviour.GetData();
                            return ownerID == data.OwnerId;
                        }
                        var fenceElementBehaviour = interactiveBuildingElement.GetComponentInParent<FenceElementBehaviour>();
                        if (fenceElementBehaviour != null)
                        {
                            var data = fenceElementBehaviour.GetData();
                            return ownerID == data.OwnerId;
                        }
                    }
                    return false;
                });
                if (selfDef.Negate) { result = !result; }
            }
            return result;
        }
    }
}
