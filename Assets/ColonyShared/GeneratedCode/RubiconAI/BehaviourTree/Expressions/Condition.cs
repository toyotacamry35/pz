using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Src.RubiconAI.BehaviourTree.Expressions
{
    interface Condition : BehaviourExpression
    {
        ValueTask<bool> Evaluate(Legionary legionary);
        ValueTask<bool> EvaluateOther(Legionary self, Legionary other);
    }

}
