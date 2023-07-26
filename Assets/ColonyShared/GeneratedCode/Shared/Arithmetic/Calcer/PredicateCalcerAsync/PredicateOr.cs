using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates;
using Assets.Src.Aspects.Impl.Stats;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;
using GeneratedCode.DeltaObjects;

namespace Assets.Src.Arithmetic
{
    [UsedImplicitly]
    public class PredicateOr : ICalcerBinding<PredicateOrDef, bool>
    {
        public async ValueTask<bool> Calc(PredicateOrDef def, CalcerContext ctx)
        {
            for (int i = 0; i < def.Predicates.Count; ++i)
            {
                if (await def.Predicates[i].Target.CalcAsync(ctx))
                    return true;
            }

            return false;
        }

        public IEnumerable<StatResource> CollectStatNotifiers(PredicateOrDef def)
        {
            foreach (var predicate in def.Predicates)
            foreach (var res in predicate.GetModifiers())
                yield return res;
        }
    }
}