using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Calcers;
using Assets.Src.Aspects.Impl.Stats;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;
using GeneratedCode.DeltaObjects;

namespace Assets.Src.Arithmetic
{
    [UsedImplicitly]
    public class CalcerPiecewise : ICalcerBinding<CalcerPiecewiseDef, float>
    {
        public async ValueTask<float> Calc(CalcerPiecewiseDef def, CalcerContext ctx)
        {
            foreach (var r in def.Ranges)
                if (await r.Condition.Target.CalcAsync(ctx))
                    return r.Value.Target != null ? await r.Value.Target.CalcAsync(ctx) : 0;
            return def.Else.Target != null ? await def.Else.Target.CalcAsync(ctx) : 0;
        }

        public IEnumerable<StatResource> CollectStatNotifiers(CalcerPiecewiseDef def)
        {
            foreach (var range in def.Ranges)
            {
                if (range.Condition.Target != null)
                    foreach (var res in range.Condition.GetModifiers())
                        yield return res;
                if (range.Value.Target != null)
                    foreach (var res in range.Value.GetModifiers())
                        yield return res;
            }

            if (def.Else.Target != null)
                foreach (var res in def.Else.Target.GetModifiers())
                    yield return res;
        }
    }
}

