using GeneratedCode.DeltaObjects;
using Shared.ManualDefsForSpells;
using SharedCode.Entities;
using SharedCode.EntitySystem;
using System.Threading.Tasks;

namespace Assets.Src.Impacts
{
    class ImpactInvokeTrauma : IImpactBinding<ImpactInvokeTraumaDef>
    {
        public async ValueTask Apply(SpellWordCastData cast, IEntitiesRepository repository, ImpactInvokeTraumaDef selfDef)
        {
            var target = cast.Caster;

            if (selfDef.Target.Target != null)
                target = await selfDef.Target.Target.GetOuterRef(cast, repository);

            if (!target.IsValid)
                target = cast.Caster;

            using (var wrapper = await repository.Get(target.TypeId, target.Guid))
            {
                var entity = wrapper.Get<IHasTraumas>(target.TypeId, target.Guid, ReplicationLevel.Master);
                if (entity != null)
                {
                    await entity.Traumas.StartTrauma(selfDef.TraumaType);
                }
            }
        }
    }
}
