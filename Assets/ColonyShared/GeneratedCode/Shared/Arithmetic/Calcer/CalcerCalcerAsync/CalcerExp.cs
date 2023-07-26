using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Calcers;
using Assets.Src.Aspects.Impl.Stats;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GeneratedCode.DeltaObjects;

namespace Assets.Src.Arithmetic
{
    [UsedImplicitly]
    public class CalcerExp : ICalcerBinding<CalcerExpDef,float>
    {
        public async ValueTask<float> Calc(CalcerExpDef def, CalcerContext ctx) => (float)Math.Exp(await def.Pow.Target.CalcAsync(ctx));

        public IEnumerable<StatResource> CollectStatNotifiers(CalcerExpDef def)
        {
            //Logger.Log(NLog.LogLevel.Info, $"        CalcerExp.Collect");
            //Logger.Log(NLog.LogLevel.Info, $"    /// CalcerExp.Collect");
            return def.Pow.GetModifiers();
        }
    }
}
