using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedDefsForSpells;
using Shared.ManualDefsForSpells;
using SharedCode.Entities;
using SharedCode.EntitySystem;
using System.Threading.Tasks;

namespace Assets.Src.Impacts
{
    class ImpactReviveFromKnockDown : IImpactBinding<ImpactReviveFromKnockDownDef>
    {
        public async ValueTask Apply(SpellWordCastData cast, IEntitiesRepository repository, ImpactReviveFromKnockDownDef selfDef)
        {
            var target = cast.Caster;

            if (selfDef.Target.Target != null)
                target = await selfDef.Target.Target.GetOuterRef(cast, repository);

            if (!target.IsValid)
                target = cast.Caster;

            using (var wrapper = await repository.Get(target.TypeId, target.Guid))
            {
                var entity = wrapper.Get<IHasMortalServer>(target.TypeId, target.Guid, ReplicationLevel.Server);
                if (entity != null)
                {
                    await entity.Mortal.Revive();
                }
            }

            }
    }
}
