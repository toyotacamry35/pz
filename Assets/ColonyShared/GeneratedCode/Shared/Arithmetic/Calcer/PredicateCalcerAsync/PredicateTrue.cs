using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates;
using Assets.Src.Aspects.Impl.Stats;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using GeneratedCode.DeltaObjects;

namespace Assets.Src.Arithmetic
{
    [UsedImplicitly]
    public class PredicateTrue : ICalcerBinding<PredicateTrueDef, bool>
    {
        public ValueTask<bool> Calc(PredicateTrueDef def, CalcerContext ctx) => new ValueTask<bool>(true);
        public IEnumerable<StatResource> CollectStatNotifiers(PredicateTrueDef def) => Enumerable.Empty<StatResource>();
    }
}