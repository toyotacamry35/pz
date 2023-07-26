using System;
using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Assets.Src.Aspects.Impl.Stats;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using JetBrains.Annotations;
using SharedCode.EntitySystem;
using System.Collections.Generic;
using System.Threading.Tasks;
using GeneratedCode.DeltaObjects;

namespace Assets.Src.Arithmetic
{
    [UsedImplicitly]
    public class CalcerStat : ICalcerBinding<CalcerStatDef, float>
    {
        public async ValueTask<float> Calc(CalcerStatDef def, CalcerContext ctx)
        {
            var statsengine = ctx.TryGetEntity<IHasStatsEngineClientFull>(ReplicationLevel.ClientFull);
            if (statsengine == null)
                return 0;
            return (await statsengine.Stats.TryGetValue(def.Stat)).Item2;
        }

        public IEnumerable<StatResource> CollectStatNotifiers(CalcerStatDef def)
        {
            if (def.Stat != null && def.Stat.IsValid)
            {
                yield return def.Stat.Target;
                //Logger.Log(NLog.LogLevel.Info, $"      -- {calcer.Stat.Target}");
            }
        }
    }
}