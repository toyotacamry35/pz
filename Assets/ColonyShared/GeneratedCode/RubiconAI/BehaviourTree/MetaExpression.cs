using SharedCode.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Assets.Src.RubiconAI.BehaviourTree
{
    // Manager of run-time instances of selectors in strategy def.
    // It lazy instantiates them from def. & provide ability of having state for them & provides access to them
    public class MetaExpression
    {
        public MetaExpression(Strategy hostStrategy)
        {
            _hostStrategy = hostStrategy;
        }
        Dictionary<IBehaviourExpressionDef, BehaviourExpression> _instances = new Dictionary<IBehaviourExpressionDef, BehaviourExpression>();
        private readonly Strategy _hostStrategy;

        public async ValueTask Clear(Strategy strategy)
        {
            foreach (var instance in _instances)
            {
                if (instance.Value.HostStrategy == strategy)
                {
                    var t = instance.Value.InitWith(strategy, instance.Key);
                    if (!t.IsCompleted)
                        await t;
                }
            }
        }
        public async ValueTask<BehaviourExpression> GetOptional(Strategy hostStrategy, IBehaviourExpressionDef def)
        {
            if (hostStrategy == null)
                hostStrategy = _hostStrategy;
            if (def == null)
                return null;
            if (_instances.ContainsKey(def))
                return _instances[def];
            var exprType = DefToType.GetType(def.GetType());
            var expr = (BehaviourExpression)Activator.CreateInstance(exprType);
            _instances.Add(def, expr);
            var t = expr.InitWith(hostStrategy, def);
            if (!t.IsCompleted) await t;
            expr.HostStrategy = hostStrategy;
            return expr;
        }
        public ValueTask<BehaviourExpression> Get(Strategy hostStrategy, IBehaviourExpressionDef def)
        {
            if (def == null)
                throw new Exception("Missing def for expression in strategy");

            return GetOptional(hostStrategy, def);
        }
    }
}
