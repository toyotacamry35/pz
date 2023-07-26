using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Calcers;
using Assets.Src.Aspects.Impl.Stats;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;
using GeneratedCode.DeltaObjects;
using SharedCode.Aspects.Item.Templates;
using Assets.Src.ResourcesSystem.Base;

namespace Assets.Src.Arithmetic
{
    [UsedImplicitly]
    public class CalcerPiecewiseResource : ICalcerBinding<CalcerPiecewiseResourceDef, BaseResource>
    {
        public async ValueTask<BaseResource> Calc(CalcerPiecewiseResourceDef def, CalcerContext ctx)
        {
            foreach (var r in def.Ranges)
                if (await r.Condition.Target.CalcAsync(ctx))
                    return r.Value.Target;
            return def.Else.Target;
        }

        public IEnumerable<StatResource> CollectStatNotifiers(CalcerPiecewiseResourceDef def)
        {
            foreach (var range in def.Ranges)
            {
                if (range.Condition.Target != null)
                    foreach (var res in range.Condition.GetModifiers())
                        yield return res;                
            }
        }
    }
}

