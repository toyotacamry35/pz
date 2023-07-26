using System.Threading.Tasks;
using GeneratedCode.DeltaObjects;
using JetBrains.Annotations;
using ResourceSystem.Aspects.ManualDefsForSpells;
using SharedCode.Entities;
using SharedCode.EntitySystem;

namespace ColonyShared.GeneratedCode.Impacts
{
    [UsedImplicitly]
    public class ImpactTeleport : IImpactBinding<ImpactTeleportDef>
    {
        public async ValueTask Apply(SpellWordCastData cast, IEntitiesRepository repo, ImpactTeleportDef def)
        {
            using (var cnt = await repo.Get(cast.Caster))
            {
                var caster = cnt.Get<IWorldCharacter>(cast.Caster);
                await caster.UnstuckTeleportDo();
            }
        }
    }
}