using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates;
using Assets.Src.Arithmetic;
using Assets.Src.ResourcesSystem.Base;
using Assets.Src.RubiconAI.BehaviourTree.Expressions;
using UnityEngine;

namespace Assets.Src.RubiconAI.BehaviourTree.NodeTypes
{
    class CheckCondition : BehaviourNode
    {
        private CheckConditionDef _def;
        private Condition _condition;
        protected override async ValueTask OnCreate(BehaviourNodeDef def)
        {
            _def = (CheckConditionDef)def;
            _condition = (Condition)await HostStrategy.MetaExpression.Get(HostStrategy, _def.Condition.Target);

        }
        public override async ValueTask<ScriptResultType> OnStart()
        {
            if (await _condition.Evaluate(HostStrategy.CurrentLegionary))
                return ScriptResultType.Succeeded;
            else
                return ScriptResultType.Failed;
        }
    }

    class CheckPredicate : Condition
    {
        public Strategy HostStrategy { get; set; }
        private PredicateDef _predicate;
        public async ValueTask InitWith(Strategy hostStrategy, IBehaviourExpressionDef def)
        {
            _predicate = ((CheckPredicateDef)def).Predicate;
        }

        public async ValueTask<bool> Evaluate(Legionary legionary)
        {
            return await _predicate.CalcAsync(legionary.Ref, legionary.Repository);
        }

        public async ValueTask<bool> EvaluateOther(Legionary self, Legionary other)
        {
            return await Evaluate(other);
        }
    }

}
