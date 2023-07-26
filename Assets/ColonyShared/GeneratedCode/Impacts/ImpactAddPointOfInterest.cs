using System.Threading.Tasks;
using ColonyShared.ManualDefsForSpells;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;

namespace Src.Impacts
{
    public class ImpactAddPointOfInterest : IImpactBinding<ImpactAddPointOfInterestDef>
    {
        public async ValueTask Apply(SpellWordCastData cast, IEntitiesRepository repo, ImpactAddPointOfInterestDef indef)
        {
            var def = (ImpactAddPointOfInterestDef) indef;
            var targetRef = cast.Caster;
            if (def.Target.Target != null)
                targetRef = await def.Target.Target.GetOuterRef(cast, repo);
            if (!targetRef.IsValid)
                targetRef = cast.Caster;
            using (var entityContainer = await repo.Get<IWorldCharacterClientFull>(targetRef.Guid))
            {
                var targetEntity = entityContainer.Get<IWorldCharacterClientFull>(targetRef.Guid);
                if (targetEntity != null)
                {
                    if (def.PointOfInterest != null)
                        await targetEntity.AddPointOfInterest(def.PointOfInterest);
                    if (def.PointsOfInterest != null)
                        foreach (var poi in def.PointsOfInterest)
                            await targetEntity.AddPointOfInterest(poi);
                }
            }
            }
    }
}