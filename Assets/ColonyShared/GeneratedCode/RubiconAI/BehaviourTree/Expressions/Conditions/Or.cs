using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Src.ResourcesSystem.Base;

namespace Assets.Src.RubiconAI.BehaviourTree.Expressions.Conditions
{
    class Or : Condition
    {
        public Strategy HostStrategy { get; set; }
        private List<Condition> _conditions = new List<Condition>();
        public async ValueTask InitWith(Strategy hostStrategy, IBehaviourExpressionDef def)
        {
            _conditions.Clear();
            foreach (var cDef in ((OrDef)def).Conditions)
            {
                _conditions.Add((Condition)await hostStrategy.MetaExpression.Get(hostStrategy, cDef.Target));
            }
        }

        public async ValueTask<bool> Evaluate(Legionary legionary)
        {
            foreach (var condition in _conditions)
            {
                var t = condition.Evaluate(legionary);
                if (t.IsCompleted)
                {
                    if (t.Result)
                        return true;
                }
                else if (await t)
                    return true;
            }
            return false;
        }

        public async ValueTask<bool> EvaluateOther(Legionary self, Legionary other)
        {
            foreach (var condition in _conditions)
            {
                var t = condition.EvaluateOther(self, other);
                if (t.IsCompleted)
                {
                    if (t.Result)
                        return true;
                }
                else if (await t)
                    return true;
            }
            return false;
        }
    }

}
