using Assets.Src.Arithmetic;
using Assets.Src.SpawnSystem;
using SharedCode.Entities.GameObjectEntities;
using System;
using System.Threading.Tasks;

namespace Assets.Src.RubiconAI.BehaviourTree.Expressions.Conditions
{
    class CompareSelfWithTarget : Condition
    {
        public Strategy HostStrategy { get; set; }
        private CompareSelfWithTargetDef _def;
        private TargetSelector _selector;
        public async ValueTask InitWith(Strategy hostStrategy, IBehaviourExpressionDef def)
        {
            _def = (CompareSelfWithTargetDef)def;
            _selector = (TargetSelector)await hostStrategy.MetaExpression.Get(hostStrategy, _def.TargetSelector.Target);
        }

        public async ValueTask<bool> Evaluate(Legionary legionary)
        {
            switch (_def.CompareType)
            {
                case CompareSelfWithTargetDef.ComprasionType.SelfIsLess:
                    return await _def.TargetCalcer.Target.CalcAsync((await _selector.SelectTarget(legionary)).Ref, legionary.Repository)
                        >
                          await  _def.SelfCalcer.Target.CalcAsync(legionary.Ref, legionary.Repository);
                case CompareSelfWithTargetDef.ComprasionType.SelfIsMore:
                    return await _def.TargetCalcer.Target.CalcAsync((await _selector.SelectTarget(legionary)).Ref, legionary.Repository)
                        <
                        await _def.SelfCalcer.Target.CalcAsync(legionary.Ref, legionary.Repository);

                case CompareSelfWithTargetDef.ComprasionType.EqualsInRange:
                    return Math.Abs(
                        await _def.TargetCalcer.Target.CalcAsync ((await _selector.SelectTarget(legionary)).Ref, legionary.Repository)
                        - await _def.SelfCalcer.Target.CalcAsync(legionary.Ref, legionary.Repository))
                        < _def.Range;
            }
            return false;
        }

        public async ValueTask<bool> EvaluateOther(Legionary self, Legionary other)
        {
            return await Evaluate(other);
        }
    }
}
