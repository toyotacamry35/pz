using System.Threading.Tasks;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;

namespace ColonyShared.ManualDefsForSpells
{
    public class ImpactRemovePointOfInterest: IImpactBinding<ImpactRemovePointOfInterestDef>
    {
        public async ValueTask Apply(SpellWordCastData cast, IEntitiesRepository repo, ImpactRemovePointOfInterestDef indef)
        {
            var def = (ImpactRemovePointOfInterestDef) indef;

            OuterRef<IEntity> targetRef = cast.Caster;
            if (def.Target.Target != null)
                targetRef = await def.Target.Target.GetOuterRef(cast, repo);

            if(!targetRef.IsValid)
                targetRef = cast.Caster;

            using (var entityContainer = await repo.Get<IWorldCharacterClientFull>(targetRef.Guid))
            {
                var targetEntity = entityContainer.Get<IWorldCharacterClientFull>(targetRef.Guid);
                if (targetEntity != null)
                {
                    if (def.PointOfInterest != null)
                        await targetEntity.RemovePointOfInterest(def.PointOfInterest);
                    if (def.PointsOfInterest != null)
                        foreach (var poi in def.PointsOfInterest)
                            await targetEntity.RemovePointOfInterest(poi);
                }
            }
            }
    }
}