using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Calcers;
using Assets.Src.Aspects.Impl.Stats;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;
using GeneratedCode.DeltaObjects;
using System.Linq;

namespace Assets.Src.Arithmetic
{
    [UsedImplicitly]
    public class CalcerSum : ICalcerBinding<CalcerSumDef, float>
    {
        public async ValueTask<float> Calc(CalcerSumDef def, CalcerContext ctx)
        {
            float result = 0.0f;
            foreach (var mult in def.Summands)
                result += await mult.CalcAsync(ctx);
            return result;
        }

        public IEnumerable<StatResource> CollectStatNotifiers(CalcerSumDef def) => def.Summands.SelectMany(v => v.GetModifiers());
    }
}