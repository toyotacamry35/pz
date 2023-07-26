using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Src.RubiconAI.BehaviourTree
{
    public  interface BehaviourExpression
    {
        Strategy HostStrategy { get; set; }
        ValueTask InitWith(Strategy hostStrategy, IBehaviourExpressionDef def);
        
    }
}
