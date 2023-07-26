using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Assets.Src.ResourcesSystem.Base;
using ResourcesSystem.Loader;
using Assets.Src.RubiconAI.KnowledgeSystem.Memory;
using JetBrains.Annotations;

namespace Assets.Src.RubiconAI.BehaviourTree.Expressions
{
    public interface EvaluatorDef : IBehaviourExpressionDef
    {
    }
    //TODO: switch to DefaultImplementation and consturctor(float/string/whatever) instead of attribute
    [CanBeCreatedFromAliasedPrimitive(typeof(float), nameof(CreateConstantMetric))]
    public abstract class MetricDef : BehaviourExpressionDef, EvaluatorDef
    {
        [UsedImplicitly]
        public static MetricDef CreateConstantMetric(float value)
        {
            return new ConstantMetricDef() { Value = value };
        }
    }
    public class ConstantMetricDef : MetricDef
    {
        //public override Type ExpressionType
        //{
        //    get
        //    {
        //        return typeof(ConstantMetric);
        //    }
        //}
        public float Value { get; set; }
    }

    public class CalcerMetricDef : MetricDef
    {
        public ResourceRef<CalcerDef<float>> Calcer { get; set; }
        public ResourceRef<TargetSelectorDef> Target { get; set; }
        //public override Type ExpressionType { get; } = typeof(CalcerMetric);
    }

    public class CollectionSumMetricDef : MetricDef, CollectionEvaluatorDef
    {
        //public override Type ExpressionType { get; } = typeof(CollectionSumMetric);
        public ResourceRef<MetricDef> Metric { get; set; }
        public ResourceRef<ICollectionSelectorDef> CollectionSelector { get; set; }
    }

    public class RangeMetricDef : MetricDef
    {
        //public override Type ExpressionType { get; } = typeof(RangeMetric);
        public ResourceRef<TargetSelectorDef> Target { get; set; }
    }

    public class RememberedValueDef : MetricDef
    {
        public bool MemoryOfTarget { get; set; } = false;
        public ResourceRef<TargetSelectorDef> Target { get; set; }
        public ResourceRef<TargetSelectorDef> TargetOfStat { get; set; }
        public ResourceRef<MemorizedStatDef> MemorizedStat { get; set; }
        //public override Type ExpressionType { get; } = typeof(RememberedValue);
    }

}
