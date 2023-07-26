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
    public class CalcerPow : ICalcerBinding<CalcerPowDef, float>
    {
        public async ValueTask<float> Calc(CalcerPowDef def, CalcerContext ctx)
        {
            return (float) Math.Pow(await def.Value.Target.CalcAsync(ctx), def.Power);
        }

        public IEnumerable<StatResource> CollectStatNotifiers(CalcerPowDef def)
        {
            return def.Value.GetModifiers();
        }
    }
}
