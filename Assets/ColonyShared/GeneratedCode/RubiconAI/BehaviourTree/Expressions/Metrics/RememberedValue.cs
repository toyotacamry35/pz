using System.Threading.Tasks;
using JetBrains.Annotations;
using Assets.Src.RubiconAI.KnowledgeSystem.Memory;

namespace Assets.Src.RubiconAI.BehaviourTree.Expressions.Metrics
{
    class RememberedValue : Metric
    {
        public Strategy HostStrategy { get; set; }

        TargetSelector _selector;
        TargetSelector _targetOfStat;
        MemorizedStatDef _def;
        bool _memOfTarget;
        public ValueTask<float> Convert()
        {
            return Evaluate(HostStrategy.CurrentLegionary);
        }

        public async ValueTask<float> Evaluate([NotNull] Legionary legionary)
        {
            var target = _selector != null ? await _selector.SelectTarget(HostStrategy.CurrentLegionary) : legionary;
            if (target == null)
                return 0f;
            float? value = null;
            if(_memOfTarget)
            {
                if(target is MobLegionary ml)
                {
                    if (_targetOfStat != null)
                    {
                        var statTarget = await _targetOfStat.SelectTarget(HostStrategy.CurrentLegionary);
                        if (statTarget != null)
                        {
                            value = ml.Knowledge.Memory.GetStat(statTarget, _def);
                        }
                    }
                    else
                    {
                        value = ml.Knowledge.Memory.GetStat(legionary, _def);
                    }
                }
                
            }
            else if (legionary is MobLegionary ml)
            {
                value = ml.Knowledge.Memory.GetStat(target, _def);
            }
            if (value.HasValue)
                return value.Value;
            return 0f;
        }

        public ValueTask<float> EvaluateOther([NotNull] Legionary legionary, [NotNull] Legionary other)
        {
            return Evaluate(other);
        }

        public async ValueTask InitWith(Strategy hostStrategy, IBehaviourExpressionDef def)
        {
            var rDef = (RememberedValueDef)def;
            _memOfTarget = rDef.MemoryOfTarget;
            _def = rDef.MemorizedStat;
            _selector = (TargetSelector)await rDef.Target.ExprOptional(hostStrategy);
            _targetOfStat = (TargetSelector)await rDef.TargetOfStat.ExprOptional(hostStrategy);
        }
    }
}
