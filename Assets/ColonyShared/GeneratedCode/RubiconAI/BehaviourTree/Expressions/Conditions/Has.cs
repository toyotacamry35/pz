using System.Threading.Tasks;

namespace Assets.Src.RubiconAI.BehaviourTree.Expressions.Conditions
{
    class Has : Condition
    {
        public Strategy HostStrategy { get; set; }
        private TargetSelector _targetSelector;
        private HasDef _def;
        public async ValueTask InitWith(Strategy hostStrategy, IBehaviourExpressionDef def)
        {
            _def = (HasDef) def;
            _targetSelector = (TargetSelector)await _def.Target.Expr(hostStrategy);
        }

        private float _distance;
        public async ValueTask<bool> Evaluate(Legionary legionary)
        {
            return _def.Not?!(await _targetSelector.SelectTarget(legionary) != null):(await _targetSelector.SelectTarget(legionary) != null);
        }

        public ValueTask<bool> EvaluateOther(Legionary self, Legionary other)
        {
            return Evaluate(other);
        }
    }
}
