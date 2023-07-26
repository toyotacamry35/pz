using System.Collections.Generic;
using System.Linq;
using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using JetBrains.Annotations;
using System.Threading.Tasks;
using Assets.Src.Aspects.Impl.Stats;
using GeneratedCode.DeltaObjects;
using SharedCode.EntitySystem;

namespace Assets.Src.Arithmetic
{
    [UsedImplicitly]
    public class PredicateHasPremium : ICalcerBinding<PredicateHasPremiumDef, bool>
    {
        public ValueTask<bool> Calc(PredicateHasPremiumDef def, CalcerContext ctx)
        {
            return new ValueTask<bool>(false);
        }

        public IEnumerable<StatResource> CollectStatNotifiers(PredicateHasPremiumDef def) { yield return null; }
    }
}