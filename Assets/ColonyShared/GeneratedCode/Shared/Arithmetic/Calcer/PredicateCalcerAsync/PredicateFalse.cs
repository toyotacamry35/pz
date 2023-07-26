using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates;
using Assets.Src.Aspects.Impl.Stats;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeneratedCode.DeltaObjects;

namespace Assets.Src.Arithmetic
{
    [UsedImplicitly]
    public class PredicateFalse : ICalcerBinding<PredicateFalseDef, bool>
    {
        public ValueTask<bool> Calc(PredicateFalseDef def, CalcerContext ctx) => new ValueTask<bool>(false);
        public IEnumerable<StatResource> CollectStatNotifiers(PredicateFalseDef def) => Enumerable.Empty<StatResource>();
    }
}
