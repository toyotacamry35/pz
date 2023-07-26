using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates;
using Assets.Src.Aspects.Impl.Stats;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;
using GeneratedCode.DeltaObjects;

namespace Assets.Src.Arithmetic
{
    [UsedImplicitly]
    public class PredicateAnd : ICalcerBinding<PredicateAndDef, bool>
    {
        public async ValueTask<bool> Calc(PredicateAndDef def, CalcerContext ctx)
        {
            for (int i = 0; i < def.Predicates.Count; ++i)
                if (!await def.Predicates[i].Target.CalcAsync(ctx))
                    return false;
            return true;
        }

        public IEnumerable<StatResource> CollectStatNotifiers(PredicateAndDef def)
        {
            foreach (var predicate in def.Predicates)
            foreach (var res in predicate.GetModifiers())
                yield return res;
        }
    }
}