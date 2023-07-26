using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates;
using Assets.Src.Aspects.Impl.Stats;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;
using GeneratedCode.DeltaObjects;

namespace Assets.Src.Arithmetic
{
    [UsedImplicitly]
    public class PredicateInRange : ICalcerBinding<PredicateInRangeDef, bool>
    {
        public async ValueTask<bool> Calc(PredicateInRangeDef def, CalcerContext ctx)
        {
            if (def.Min == null || def.Max == null || def.Value == null)
                return false;
            var value = await def.Value.Target.CalcAsync(ctx);
            var min = await def.Min.Target.CalcAsync(ctx);
            var max = await def.Max.Target.CalcAsync(ctx);
            return value >= min && value < max;
        }

        public IEnumerable<StatResource> CollectStatNotifiers(PredicateInRangeDef def)
        {
            foreach (var res in def.Value.GetModifiers())
                yield return res;
            foreach (var res in def.Min.GetModifiers())
                yield return res;
            foreach (var res in def.Max.GetModifiers())
                yield return res;
        }
    }
}