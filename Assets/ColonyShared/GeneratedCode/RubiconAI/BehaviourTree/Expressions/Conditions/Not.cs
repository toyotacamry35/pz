using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Src.ResourcesSystem.Base;

namespace Assets.Src.RubiconAI.BehaviourTree.Expressions.Conditions
{
    class Not : Condition
    {

        public Strategy HostStrategy { get; set; }
        private Condition _condition;
        public async ValueTask InitWith(Strategy hostStrategy, IBehaviourExpressionDef def)
        {
            _condition = (Condition) await hostStrategy.MetaExpression.Get(hostStrategy, ((NotDef) def).Condition.Target);
        }

        public void OnIMGUI()
        {
        }

        public async ValueTask<bool> Evaluate(Legionary legionary)
        {
            return !await _condition.Evaluate(legionary);
        }

        public async ValueTask<bool> EvaluateOther(Legionary self, Legionary other)
        {
            return !await _condition.EvaluateOther(self, other);
        }
    }

}
