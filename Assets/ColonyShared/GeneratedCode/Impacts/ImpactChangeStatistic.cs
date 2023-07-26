using SharedCode.Logging;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using Shared.ManualDefsForSpells;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;

namespace Assets.Src.Impacts
{
    public class ImpactChangeStatistic : IImpactBinding<ImpactChangeStatisticDef>
    {
        public async ValueTask Apply(SpellWordCastData cast, IEntitiesRepository repository, ImpactChangeStatisticDef def)
        {
            var selfDef = (ImpactChangeStatisticDef)def;
            var targetRef = cast.Caster;

            if (!targetRef.IsValid)
            {
                Log.Logger.IfWarn()?.Message("Missing target in ImpactChangeStatistic {0}", def.____GetDebugAddress()).Write();
                return;
            }

            //Log.Logger.IfInfo()?.Message("ImpactChangeStatistic").Write();
            using (var wrapper = await repository.Get(targetRef.TypeId, targetRef.Guid))
            {
                var entity = wrapper.Get<IHasStatisticsServer>(targetRef.TypeId, targetRef.Guid, ReplicationLevel.Server);
                if (entity != null)
                {
                    //Log.Logger.IfInfo()?.Message($"ChangeStatistic: Statistic = {selfDef.Statistic}, TargetType = {selfDef.TargetType}, Value = {selfDef.Value}").Write();
                    await entity.ChangeStatistic(selfDef.TargetType, selfDef.Statistic, selfDef.Value);
                }
            }

            }
    }
}
