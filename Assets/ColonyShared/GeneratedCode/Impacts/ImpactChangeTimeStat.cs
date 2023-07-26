using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using Shared.ManualDefsForSpells;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using System.Threading.Tasks;
using Assets.Src.Arithmetic;
using Core.Environment.Logging.Extension;
using SharedCode.Logging;

namespace Assets.Src.Impacts
{
    public class ImpactChangeTimeStat : IImpactBinding<ImpactChangeTimeStatDef>
    {
        public async ValueTask Apply(SpellWordCastData cast, IEntitiesRepository repository, ImpactChangeTimeStatDef def)
        {
            var StatNameDef = def.StatName;

            var targetRef = cast.Caster;
            if (def.Target.Target != null)
                targetRef = await def.Target.Target.GetOuterRef(cast, repository);
            if (!targetRef.IsValid)
                targetRef = cast.Caster;

            if (!targetRef.IsValid)
            {
                if (def.Target.Target == null)
                    Log.Logger.IfError()?.Message("selfDef.Target.Target == null in ImpactChangeTimeStat {0}", def.____GetDebugAddress()).Write();
                else if (def.Target.Target.GetType() == typeof(SpellCasterDef))
                    Log.Logger.IfError()?.Message("Missing caster in ImpactChangeTimeStat {0}", def.____GetDebugAddress()).Write();
                else
                    Log.Logger.IfWarn()?.Message("Missing target in ImpactChangeTimeStat {0}", def.____GetDebugAddress()).Write();
                return;
            }

            using (var wrapper = await repository.Get(targetRef.TypeId, targetRef.Guid))
            {
                var entity = wrapper.Get<IHasStatsEngineServer>(targetRef.TypeId, targetRef.Guid, ReplicationLevel.Server);
                if (entity != null)
                {
                    float value = def.Calcer != null ? await def.Calcer.Target.CalcAsync(new CalcerContext(wrapper, targetRef, repository, cast)): def.Value;
                    await entity.Stats.ChangeValue(StatNameDef, value);
                }
            }

        }
    }
}
