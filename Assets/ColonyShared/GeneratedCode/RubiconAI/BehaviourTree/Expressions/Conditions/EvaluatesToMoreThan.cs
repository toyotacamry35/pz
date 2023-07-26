using System.Threading.Tasks;

namespace Assets.Src.RubiconAI.BehaviourTree.Expressions.Conditions
{
    class EvaluatesToMoreThan : Condition
    {
        public Strategy HostStrategy { get; set; }
        private IEvaluator _eval;
        private float _amount;
        public async ValueTask InitWith(Strategy hostStrategy, IBehaviourExpressionDef def)
        {
            var eDef = (EvaluatesToMoreThanDef) def;
            _eval = (IEvaluator)await eDef.Evaluator.Expr(hostStrategy);
            _amount = await eDef.Amount.Get(hostStrategy);
        }


        public async ValueTask<bool> Evaluate(Legionary legionary)
        {
            return await _eval.Evaluate(legionary) > _amount;
        }

        public async ValueTask<bool> EvaluateOther(Legionary self, Legionary other)
        {
            return await _eval.EvaluateOther(self, other) > _amount;
        }
    }
}
