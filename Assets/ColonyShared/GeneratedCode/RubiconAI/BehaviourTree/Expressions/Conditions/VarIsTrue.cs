using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Src.ResourcesSystem.Base;

namespace Assets.Src.RubiconAI.BehaviourTree.Expressions.Conditions
{
    class VarIsTrue : Condition
    {
        public Strategy HostStrategy { get; set; }
        private Strategy _strategy;
        private VarIsTrueDef _def;
        public async ValueTask InitWith(Strategy hostStrategy, IBehaviourExpressionDef def)
        {
            _strategy = hostStrategy;
            _def = (VarIsTrueDef) def;
        }


        public async ValueTask<bool> Evaluate(Legionary legionary)
        {
            if (!_strategy.CurrentLegionary.TemporaryBlackboard.ContainsKey(_def.VarName))
                return false;
            return _strategy.CurrentLegionary.TemporaryBlackboard[_def.VarName] > 0.5;
        }

        public async ValueTask<bool> EvaluateOther(Legionary self, Legionary other)
        {
            if (!_strategy.CurrentLegionary.TemporaryBlackboard.ContainsKey(_def.VarName))
                return false;
            return _strategy.CurrentLegionary.TemporaryBlackboard[_def.VarName] > 0.5;
        }
    }

}
