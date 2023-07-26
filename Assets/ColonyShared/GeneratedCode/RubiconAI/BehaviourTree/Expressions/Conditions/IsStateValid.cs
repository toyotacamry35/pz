using Assets.Src.RubiconAI.BehaviourTree.NodeTypes;
using System.Threading.Tasks;

namespace Assets.Src.RubiconAI.BehaviourTree.Expressions.Conditions
{
    class IsStateValid : Condition
    {
        public Strategy HostStrategy { get; set; }
        IInvalidatableSelector _selector;
        public async ValueTask<bool> Evaluate(Legionary legionary)
        {
            return !await _selector.Invalid();
        }

        public async ValueTask<bool> EvaluateOther(Legionary self, Legionary other)
        {
            return !(await _selector.Invalid());
        }

        public async ValueTask InitWith(Strategy hostStrategy, IBehaviourExpressionDef def)
        {
            _selector = (IInvalidatableSelector)await ((IsStateValidDef)def).ValidSelector.Expr(hostStrategy);
        }
        
    }
}
