using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Src.ResourcesSystem.Base;

namespace Assets.Src.RubiconAI.BehaviourTree.Expressions.Conditions
{
    class And : Condition
    {
        public Strategy HostStrategy { get; set; }
        private List<Condition> _conditions = new List<Condition>();
        private Condition _condition;
        public async ValueTask InitWith(Strategy hostStrategy, IBehaviourExpressionDef def)
        {
            _conditions.Clear();
            foreach (var cDef in ((AndDef)def).Conditions)
            {
                _conditions.Add((Condition)await hostStrategy.MetaExpression.Get(hostStrategy, cDef.Target));
            }
        }


        public async ValueTask<bool> Evaluate(Legionary legionary)
        {
            foreach (var condition in _conditions)
            {
                if (!await condition.Evaluate(legionary))
                    return false;
            }
            return true;
        }

        public async ValueTask<bool> EvaluateOther(Legionary self, Legionary other)
        {
            foreach (var condition in _conditions)
            {
                if (!await condition.EvaluateOther(self, other))
                    return false;
            }
            return true;
        }
    }

}
