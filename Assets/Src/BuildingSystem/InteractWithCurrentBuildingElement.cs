using SharedCode.EntitySystem;
using SharedCode.DeltaObjects.Building;
using GeneratedCode.DeltaObjects;
using System.Threading.Tasks;
using Assets.Src.Wizardry;
using JetBrains.Annotations;

namespace Assets.Src.BuildingSystem
{
    [UsedImplicitly, PredictableEffect]
    public class InteractWithCurrentBuildingElement : IClientOnlyEffectBinding<InteractWithCurrentBuildingElementDef>
    {
        public ValueTask AttachOnClient(SpellWordCastData cast, IEntitiesRepository repo, InteractWithCurrentBuildingElementDef def)
        {
            var selfDef = def;

            var casterGo = cast.GetCaster();
            if (casterGo != null)
            {
                UnityQueueHelper.RunInUnityThread(() =>
                {
                    if (!(BuildSystem.Builder?.IsEnabled ?? false)) { return; }

                    var targetHolder = casterGo.GetComponentInChildren<TargetHolder>();
                    var interactiveBuildingElement = targetHolder?.CurrentTarget.Value;
                    if (interactiveBuildingElement != null)
                    {
                        var buildingElementBehaviour = interactiveBuildingElement.GetComponentInParent<BuildingElementBehaviour>();
                        if (buildingElementBehaviour != null)
                        {
                            var data = buildingElementBehaviour.GetData();
                            if ((data != null) && (data.BuildRecipeDef != null))
                            {
                                int nextInteraction;
                                if (data.BuildRecipeDef.GetNextInteraction(data.Interaction, out nextInteraction))
                                {
                                    BuildSystem.Builder.InteractElement(repo, BuildType.BuildingElement, data.PlaceId, data.ElementId, nextInteraction);
                                }
                            }
                            return;
                        }
                        var fenceElementBehaviour = interactiveBuildingElement.GetComponentInParent<FenceElementBehaviour>();
                        if (fenceElementBehaviour != null)
                        {
                            var data = fenceElementBehaviour.GetData();
                            if ((data != null) && (data.BuildRecipeDef != null))
                            {
                                int nextInteraction;
                                if (data.BuildRecipeDef.GetNextInteraction(data.Interaction, out nextInteraction))
                                {
                                    BuildSystem.Builder.InteractElement(repo, BuildType.FenceElement, data.PlaceId, data.ElementId, nextInteraction);
                                }
                            }
                            return;
                        }
                    }
                });
            }
            return new ValueTask();
        }

        public ValueTask DetachOnClient(SpellWordCastData cast, IEntitiesRepository repo, InteractWithCurrentBuildingElementDef def)
        {
            return new ValueTask();
        }
    }
}
