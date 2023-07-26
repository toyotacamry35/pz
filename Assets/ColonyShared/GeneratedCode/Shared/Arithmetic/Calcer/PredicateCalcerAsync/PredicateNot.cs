using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates;
using Assets.Src.Aspects.Impl.Stats;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;
using GeneratedCode.DeltaObjects;

namespace Assets.Src.Arithmetic
{
    [UsedImplicitly]
    public class PredicateNot : ICalcerBinding<PredicateNotDef, bool>
    {
        public async ValueTask<bool> Calc(PredicateNotDef def, CalcerContext ctx)
        {
            return def.Value != null && !await def.Value.Target.CalcAsync(ctx);
        }

        public IEnumerable<StatResource> CollectStatNotifiers(PredicateNotDef def)
        {
            if (def.Value.Target != null)
                foreach (var res in def.Value.Target.GetModifiers())
                    yield return res;
        }
    }
}