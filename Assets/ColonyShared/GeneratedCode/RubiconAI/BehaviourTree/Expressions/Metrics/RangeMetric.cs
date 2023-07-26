using JetBrains.Annotations;
using System.Threading.Tasks;

namespace Assets.Src.RubiconAI.BehaviourTree.Expressions.Metrics
{
    class RangeMetric : Metric
    {
        public Strategy HostStrategy { get; set; }

        public ValueTask<float> Convert()
        {
            return Evaluate(HostStrategy.CurrentLegionary);
        }

        public async ValueTask<float> Evaluate([NotNull] Legionary legionary)
        {
            var t = _selector.SelectTarget(legionary);
            var target = t.IsCompleted ? t.Result : await t;
            var tp = HostStrategy.CurrentLegionary.GetPos(target);
            var lp = HostStrategy.CurrentLegionary.GetPos(legionary);
            if (target == null || !tp.HasValue || !lp.HasValue)
                return float.MaxValue;
            var distanceVec = tp - lp;
            return distanceVec.Value.magnitude;
        }

        public ValueTask<float> EvaluateOther([NotNull] Legionary legionary, [NotNull] Legionary other)
        {
            return Evaluate(other);
        }
        TargetSelector _selector;
        public async ValueTask InitWith(Strategy hostStrategy, IBehaviourExpressionDef def)
        {
            var t = hostStrategy.MetaExpression.Get(hostStrategy, ((RangeMetricDef)def).Target.Target);
            _selector = t.IsCompleted ? (TargetSelector)t.Result : (TargetSelector)await t;
        }

    }
}
