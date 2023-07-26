using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Src.ResourcesSystem.Base;

namespace Assets.Src.RubiconAI.BehaviourTree.Expressions.CollectionEvaluators
{
    class CollectionSumMetric : CollectionEvaluator
    {
        List<Legionary> _cachedLegionariesList = new List<Legionary>();
        public override async ValueTask<float> Evaluate(Legionary legionary)
        {
            _cachedLegionariesList.Clear();
            await _selector.GetLegionaries(legionary, _cachedLegionariesList);
            return await EvaluateOther(legionary, _cachedLegionariesList);
        }

        public override async ValueTask<float> EvaluateOther(Legionary legionary, Legionary other)
        {
            return await Evaluate(other);
        }

        public override async ValueTask<float> EvaluateOther(Legionary legionary, IEnumerable<Legionary> legionaries)
        {
            float sum = 0;
            foreach (var collectionLeg in legionaries)
            {
                sum += await _metric.EvaluateOther(legionary, collectionLeg);
            }
            return sum;
        }

        private Strategy _host;
        private ICollectionSelector _selector;
        private Metric _metric;
        public override async ValueTask InitWith(Strategy hostStrategy, IBehaviourExpressionDef def)
        {
            _host = hostStrategy;
            var sumDef = (CollectionSumMetricDef)def;
            _metric = (Metric)await hostStrategy.MetaExpression.Get(hostStrategy, sumDef.Metric.Target);
            _selector = (ICollectionSelector)await hostStrategy.MetaExpression.Get(hostStrategy, sumDef.CollectionSelector.Target);
        }

        public override ValueTask<float> Convert()
        {
            return Evaluate(_host.CurrentLegionary);
        }
    }
}
