using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using NLog;
using Shared.ManualDefsForSpells;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using System.Threading.Tasks;

namespace Assets.Src.Impacts
{
    public class ImpactSynchronizeMutation : IImpactBinding<ImpactSynchronizeMutationDef>
    {
        private static readonly NLog.Logger Logger = LogManager.GetLogger("ImpactSynchronizeMutation");

        public async ValueTask Apply(SpellWordCastData cast, IEntitiesRepository repository, ImpactSynchronizeMutationDef def)
        {
            var selfDef = (ImpactSynchronizeMutationDef)def;
            var target = cast.Caster;
            if (selfDef.Target.Target != null)
                target = await selfDef.Target.Target.GetOuterRef(cast, repository);

            if (!target.IsValid)
                target = cast.Caster;

            using (var wrapper = await repository.Get(target.TypeId, target.Guid))
            {
                var entity = wrapper.Get<IHasMutationMechanicsServer>(target.TypeId, target.Guid, ReplicationLevel.Server);
                if (entity != null)
                {
                    await entity.MutationMechanics.ApplyMutationChangeForced(entity.MutationMechanics.NewStage, entity.MutationMechanics.NewFaction);
                }
            }

            }
    }
}
