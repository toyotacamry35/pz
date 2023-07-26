using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates;
using Assets.Src.Aspects.Impl.Stats;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;
using GeneratedCode.DeltaObjects;

namespace Assets.Src.Arithmetic
{
    [UsedImplicitly]
    public class PredicateLess : ICalcerBinding<PredicateLessDef, bool>
    {
        public async ValueTask<bool> Calc(PredicateLessDef def, CalcerContext ctx)
        {
            if (def.Lhs == null || def.Rhs == null)
                return false;
            return await def.Lhs.Target.CalcAsync(ctx) < await def.Rhs.Target.CalcAsync(ctx);
        }

        public IEnumerable<StatResource> CollectStatNotifiers(PredicateLessDef def)
        {
            foreach (var res in def.Lhs.GetModifiers())
                yield return res;
            foreach (var res in def.Rhs.GetModifiers())
                yield return res;
        }
    }
}