using GeneratedCode.DeltaObjects;
using Shared.ManualDefsForSpells;
using SharedCode.Entities;
using SharedCode.EntitySystem;
using System.Threading.Tasks;

namespace Assets.Src.Impacts
{
    class ImpactChangeTraumaPoints : IImpactBinding<ImpactChangeTraumaPointsDef>
    {
        public async ValueTask Apply(SpellWordCastData cast, IEntitiesRepository repository, ImpactChangeTraumaPointsDef selfDef)
        {
            using (var wrapper = await repository.Get(cast.Caster.TypeId, cast.Caster.Guid))
            {
                var entity = wrapper.Get<IHasTraumas>(cast.Caster.TypeId, cast.Caster.Guid, ReplicationLevel.Master);
                if (entity != null)
                    await entity.Traumas.ChangeTraumaPoints(selfDef.TraumaType, selfDef.Delta);
            }
        }
    }
}
