using System.Collections.Generic;
using System.Threading.Tasks;

namespace Assets.Src.RubiconAI.BehaviourTree.Expressions
{
    abstract class CollectionEvaluator : BehaviourExpression, Metric
    {
        public Strategy HostStrategy { get; set; }
        public abstract ValueTask<float> Evaluate(Legionary legionary);
        public abstract ValueTask<float> EvaluateOther(Legionary legionary, IEnumerable<Legionary> legionaries);
        public abstract ValueTask<float> EvaluateOther(Legionary legionary, Legionary other);
        public abstract ValueTask InitWith(Strategy hostStrategy, IBehaviourExpressionDef def);
        public abstract ValueTask<float> Convert();
    }
}
