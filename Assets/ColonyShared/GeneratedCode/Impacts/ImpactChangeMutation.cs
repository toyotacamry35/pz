using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using NLog;
using Shared.ManualDefsForSpells;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using System.Threading.Tasks;

namespace Assets.Src.Impacts
{
    class ImpactChangeMutation : IImpactBinding<ImpactChangeMutationDef>
    {
        private static readonly NLog.Logger Logger = LogManager.GetLogger("ImpactChangeMutation");

        public async ValueTask Apply(SpellWordCastData cast, IEntitiesRepository repository, ImpactChangeMutationDef def)
        {
            var selfDef = (ImpactChangeMutationDef)def;

            var target = cast.Caster;
            if (selfDef.Target.Target != null)
                target = await selfDef.Target.Target.GetOuterRef(cast, repository);
            if (!target.IsValid)
                target = cast.Caster;

            // Logger.IfInfo()?.Message($"target = {target}").Write();
            using (var wrapper = await repository.Get(target.TypeId, target.Guid))
            {
                var entity = wrapper.Get<IHasMutationMechanicsServer>(target.TypeId, target.Guid, ReplicationLevel.Server);
                if (entity != null)
                {
                 //   Logger.IfInfo()?.Message($"StartTrauma {selfDef.TraumaType}").Write();
                    await entity.MutationMechanics.ChangeMutation(selfDef.DeltaValue, selfDef.Faction, selfDef.CoolDownTime, false);
                }
            }

            }
    }
}
