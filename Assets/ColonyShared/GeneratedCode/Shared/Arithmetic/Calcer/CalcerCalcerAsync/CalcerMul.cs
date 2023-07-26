using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Calcers;
using Assets.Src.Aspects.Impl.Stats;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;
using GeneratedCode.DeltaObjects;

namespace Assets.Src.Arithmetic
{
    [UsedImplicitly]
    public class CalcerMul : ICalcerBinding<CalcerMulDef,float>
    {
        public async ValueTask<float> Calc(CalcerMulDef def, CalcerContext ctx)
        {
            float result = 1.0f;
            foreach (var mult in def.Multipliers)
                result *= await mult.CalcAsync(ctx);
            return result;
        }

        public IEnumerable<StatResource> CollectStatNotifiers(CalcerMulDef calcer)
        {
            foreach (var value in calcer.Multipliers)
            foreach (var res in value.GetModifiers())
                yield return res;
        }
    }
}
