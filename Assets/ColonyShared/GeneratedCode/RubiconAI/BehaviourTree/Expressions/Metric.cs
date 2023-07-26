using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Assets.Src.RubiconAI.BehaviourTree.Expressions
{
    interface IEvaluator
    {
        ValueTask<float> Evaluate([NotNull]Legionary legionary);
        ValueTask<float> EvaluateOther([NotNull]Legionary legionary, [NotNull]Legionary other);
    }

    interface Metric : BehaviourExpression, IEvaluator
    {
    }

    class ConstantMetric : Metric
    {
        public Strategy HostStrategy { get; set; }
        float _value;
        public async ValueTask<float> Evaluate([NotNull] Legionary legionary)
        {
            return _value;
        }

        public async ValueTask<float> EvaluateOther([NotNull] Legionary legionary, [NotNull] Legionary other)
        {
            return _value;
        }

        public async ValueTask InitWith(Strategy hostStrategy, IBehaviourExpressionDef def)
        {
            _value = ((ConstantMetricDef)def).Value;

        }

        public void OnIMGUI()
        {
        }
    }
}
