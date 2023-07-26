using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Src.ResourcesSystem.Base;

namespace Assets.Src.RubiconAI.BehaviourTree.Expressions.Conditions
{
    class True : Condition
    {

        public Strategy HostStrategy { get; set; }
        public ValueTask InitWith(Strategy hostStrategy, IBehaviourExpressionDef def)
        {
            return default;
        }


        public ValueTask<bool> Evaluate(Legionary legionary)
        {
            return new ValueTask<bool>(true);
        }

        public ValueTask<bool> EvaluateOther(Legionary self, Legionary other)
        {
            return new ValueTask<bool>(true);
        }
    }

}
