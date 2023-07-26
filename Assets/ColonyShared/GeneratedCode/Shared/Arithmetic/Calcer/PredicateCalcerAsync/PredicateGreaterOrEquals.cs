using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates;
using Assets.Src.Aspects.Impl.Stats;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;
using GeneratedCode.DeltaObjects;

namespace Assets.Src.Arithmetic
{
    [UsedImplicitly]
    public class PredicateGreaterOrEquals : ICalcerBinding<PredicateGreaterOrEqualsDef, bool>
    {
        public async ValueTask<bool> Calc(PredicateGreaterOrEqualsDef def, CalcerContext ctx)
        {
            if (def.Lhs == null || def.Rhs == null)
                return false;
            return await def.Lhs.Target.CalcAsync(ctx) >= await def.Rhs.Target.CalcAsync(ctx);
        }
        
        public IEnumerable<StatResource> CollectStatNotifiers(PredicateGreaterOrEqualsDef pred)
        {
            //Logger.Log(NLog.LogLevel.Info, $"        PredicateGreater.Collect");
            foreach (var res in pred.Lhs.GetModifiers())
                yield return res;
            foreach (var res in pred.Rhs.GetModifiers())
                yield return res;
            //Logger.Log(NLog.LogLevel.Info, $"    /// PredicateGreater.Collect");
        }
    }
}
