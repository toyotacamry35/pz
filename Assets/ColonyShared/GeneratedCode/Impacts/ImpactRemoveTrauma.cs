using System.Threading.Tasks;
using GeneratedCode.DeltaObjects;
using JetBrains.Annotations;
using Shared.ManualDefsForSpells;
using SharedCode.Entities;
using SharedCode.EntitySystem;

namespace ColonyShared.GeneratedCode.Impacts
{
    [UsedImplicitly]
    public class ImpactRemoveTrauma : IImpactBinding<ImpactRemoveTraumaDef>
    {
        public async ValueTask Apply(SpellWordCastData cast, IEntitiesRepository repo, ImpactRemoveTraumaDef def)
        {
            using (var wrapper = await repo.Get(cast.Caster.TypeId, cast.Caster.Guid))
            {
                var entity = wrapper.Get<IHasTraumas>(cast.Caster.TypeId, cast.Caster.Guid, ReplicationLevel.Master);
                if (entity != null)
                    await entity.Traumas.StopTrauma(def.TraumaType);
            }
        }
    }
}

