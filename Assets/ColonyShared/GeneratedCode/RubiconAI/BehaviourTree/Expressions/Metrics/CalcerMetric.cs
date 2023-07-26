using System;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Assets.Src.Arithmetic;
using Assets.Src.ResourcesSystem.Base;
using Assets.Src.SpawnSystem;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;

namespace Assets.Src.RubiconAI.BehaviourTree.Expressions.Metrics
{
    class CalcerMetric : Metric
    {
        public Strategy HostStrategy { get; set; }
        private Strategy _host;
        private CalcerDef<float> _calcer;
        private TargetSelector _target;
        public async ValueTask InitWith(Strategy hostStrategy, IBehaviourExpressionDef def)
        {
            _host = hostStrategy;
            _calcer = ((CalcerMetricDef)def).Calcer;
            _target = (TargetSelector)await ((CalcerMetricDef)def).Target.ExprOptional(hostStrategy);
        }


        public async ValueTask<float> Evaluate(Legionary legionary)
        {
            if (_target != null)
            {
                var leg = await _target.SelectTarget(legionary);
                if (leg == null)
                    return 0f;
                using (var wrapper = await legionary.Repository.Get(leg.Ref.TypeId, leg.Ref.Guid))
                    return await _calcer.CalcAsync(new CalcerContext(wrapper, leg.Ref, legionary.Repository));
            }
            using (var wrapper = await legionary.Repository.Get(legionary.Ref.TypeId, legionary.Ref.Guid))
            {
                return await _calcer.CalcAsync(new CalcerContext(wrapper, legionary.Ref, legionary.Repository));
            }
        }

        public async ValueTask<float> EvaluateOther(Legionary legionary, Legionary other)
        {
            return await Evaluate(other);
        }

        public async ValueTask<float> Convert()
        {
            return await Evaluate(_host.CurrentLegionary);
        }
    }
}
