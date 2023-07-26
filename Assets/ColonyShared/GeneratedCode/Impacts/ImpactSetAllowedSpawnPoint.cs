using ColonyShared.ManualDefsForSpells;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using System.Threading.Tasks;

namespace Src.Impacts
{
    public class ImpactSetAllowedSpawnPoint : IImpactBinding<ImpactSetAllowedSpawnPointDef>
    {
        public async ValueTask Apply(SpellWordCastData cast, IEntitiesRepository repo, ImpactSetAllowedSpawnPointDef indef)
        {
            ImpactSetAllowedSpawnPointDef def = (ImpactSetAllowedSpawnPointDef)indef;
            var targetRef = cast.Caster;
            if (def.Target.Target != null)
                targetRef = await def.Target.Target.GetOuterRef(cast, repo);

            if (!targetRef.IsValid)
                targetRef = cast.Caster;

            using (var entityContainer = await repo.Get<IWorldCharacterClientFull>(targetRef.Guid))
            {
                var targetEntity = entityContainer.Get<IWorldCharacterClientFull>(targetRef.Guid);
                if (targetEntity != null && def.SpawnPointType != null)
                    await targetEntity.AllowedSpawnPointSet(def.SpawnPointType);
            }

            }
    }
}