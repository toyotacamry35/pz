using System.Threading.Tasks;
using Assets.Src.ResourcesSystem.Base;
using Assets.Src.RubiconAI.BehaviourTree;
using Assets.Src.RubiconAI.BehaviourTree.Expressions;

namespace Assets.Src.RubiconAI
{

    public static class ContextualResourceRefExtension
    {
        public static ValueTask<BehaviourExpression> Expr<TDef>(this ResourceRef<TDef> res, Strategy strategy) where TDef : class, IBehaviourExpressionDef
        {
            return strategy.MetaExpression.Get(strategy, res.Target);
        }
        public static ValueTask<BehaviourExpression> ExprOptional<TDef>(this ResourceRef<TDef> res, Strategy strategy) where TDef : class, IBehaviourExpressionDef
        {
            return strategy.MetaExpression.GetOptional(strategy, res.Target);
        }
        public static async ValueTask<float> Get(this ResourceRef<MetricDef> res, Strategy strategy, float defaultValue = 0f)
        {
            if (res.Target == null)
                return defaultValue;
            return await ((Metric)await strategy.MetaExpression.Get(strategy, res.Target)).Evaluate(strategy.CurrentLegionary);
        }
        public static async ValueTask<float> GetOptional(this ResourceRef<MetricDef> res, Strategy strategy, float defaultValue = 0f)
        {
            if (res.Target == null)
                return defaultValue;
            var expr = ((Metric)await strategy.MetaExpression.GetOptional(strategy, res.Target));
            return expr != null ? await expr.Evaluate(strategy.CurrentLegionary) : 0f;
        }
    }

}

