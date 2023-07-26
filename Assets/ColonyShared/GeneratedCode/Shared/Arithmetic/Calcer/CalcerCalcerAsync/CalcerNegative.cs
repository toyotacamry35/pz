using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Calcers;
using Assets.Src.Aspects.Impl.Stats;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;
using GeneratedCode.DeltaObjects;

namespace Assets.Src.Arithmetic
{
    [UsedImplicitly]
    public class CalcerNegative : ICalcerBinding<CalcerNegativeDef,float>
    {
        public async ValueTask<float> Calc(CalcerNegativeDef def, CalcerContext ctx)
        {
            return -await def.Value.Target.CalcAsync(ctx);
        }

        public IEnumerable<StatResource> CollectStatNotifiers(CalcerNegativeDef calcer)
        {
            return calcer.Value.GetModifiers();
        }
    }
}
