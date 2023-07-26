using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assets.Src.Aspects.Impl.Stats;
using GeneratedCode.DeltaObjects;
using ResourceSystem.Arithmetic.Templates.Calcers;
using ResourceSystem.Utils;

namespace Assets.Src.Arithmetic
{
    public class CalcerThisEntity : ICalcerBinding<CalcerThisEntityDef, OuterRef>
    {
        public ValueTask<OuterRef> Calc(CalcerThisEntityDef def, CalcerContext ctx)
        {
            return new ValueTask<OuterRef>(ctx.EntityRef);
        }

        public IEnumerable<StatResource> CollectStatNotifiers(CalcerThisEntityDef def) => Enumerable.Empty<StatResource>();
    }
}