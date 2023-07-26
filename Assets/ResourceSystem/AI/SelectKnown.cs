using System.Collections.Generic;
using Assets.ColonyShared.SharedCode.Aspects.Templates;
using Assets.Src.Aspects.Impl.Stats;
using Assets.Src.ResourcesSystem.Base;
using Assets.Src.RubiconAI.KnowledgeSystem.Memory;

namespace Assets.Src.RubiconAI.BehaviourTree.Expressions
{

    public interface ICollectionSelectorDef : IBehaviourExpressionDef
    {

    }

    public class SelectEventSourceDef : TargetSelectorDef
    {
        //public override Type ExpressionType { get; } = typeof(SelectEventSource);
    }

    public class SelectEventTargetDef : TargetSelectorDef
    {
        //public override Type ExpressionType { get; } = typeof(SelectEventTarget);
    }

    public class SelectAndRememberDef : TargetSelectorDef, ICollectionSelectorDef
    {
        //public override Type ExpressionType { get; } = typeof(SelectAndRemember);
        public ResourceRef<TargetSelectorDef> Selector { get; set; }
    }
    public class EvasionSelectorDef : TargetSelectorDef
    {
        public float HowFarToRun { get; set; }
        public float InRange { get; set; }
        //public override Type ExpressionType { get; } = typeof(EvasionSelector);
    }
    public abstract class TargetSelectorDef : BehaviourExpressionDef
    {
    }

    public class TargetLegionDef : TargetSelectorDef
    {
        public ResourceRef<TargetSelectorDef> TargetSelector { get; set; }
        //public override Type ExpressionType { get; } = typeof(TargetLegion);
    }

    public class NearestTargetSelectorDef : TargetSelectorDef
    {
        public float InRange { get; set; }
        //public override Type ExpressionType { get; } = typeof(NearestTargetSelector);
    }

    public class RandomTargetSelectorDef : TargetSelectorDef
    {
        public float InRange { get; set; }
        //public override Type ExpressionType { get; } = typeof(RandomTargetSelector);
    }

    public class RandomPointTargetSelectorDef : TargetSelectorDef
    {
        public ResourceRef<MetricDef> InRange { get; set; }
        // Leave it uninitialized to use self pos. as basis point
        public ResourceRef<TargetSelectorDef> BasisSelectorDef { get; set; }
        //public override Type ExpressionType { get; } = typeof(RandomPointTargetSelector);
    }

    public abstract class ConditionDef : BehaviourExpressionDef
    {
    }

    public class SelfDef : TargetSelectorDef
    {
        public ResourceRef<TargetSelectorDef> FromSelf { get; set; }
        //public override Type ExpressionType { get; } = typeof(Self);
    }

    public class SelectOffsetPointOfDef : TargetSelectorDef, ICollectionSelectorDef
    {
        //public override Type ExpressionType { get; } = typeof(SelectAndRemember);
        public ResourceRef<TargetSelectorDef> Selector { get; set; } = new SelfDef();
        public SharedCode.Utils.Vector3 Offset { get; set; }
        public ResourceRef<TargetSelectorDef> Towards { get; set; } = new SelfDef();
    }

    public class SelectKnownDef : TargetSelectorDef, ICollectionSelectorDef
    {
        //public override Type ExpressionType { get; } = typeof(SelectKnown);
        public ResourceRef<MemorizedStatDef> MemoryCategory { get; set; }
        public ResourceRef<KnowledgeCategoryDef> Category { get; set; }
        public ResourceRef<ConditionDef> Filter { get; set; }
        public ResourceRef<MetricDef> Metric { get; set; }
        public bool InverseMetric { get; set; } = false;
        public bool Shuffle { get; set; } = false;

    }

    public class AndDef : ConditionDef
    {
        public List<ResourceRef<ConditionDef>> Conditions { get; set; }
        //public override Type ExpressionType { get; } = typeof(And);
    }

    public class CheckStatDef : ConditionDef
    {
        public ResourceRef<StatResource> Stat { get; set; }
        public float Value { get; set; }
        public CompareType Operation { get; set; }
        public enum CompareType
        {
            More,
            Less
        }

        //public override Type ExpressionType { get; } = typeof(CheckStat);
    }

    public class EvaluatesToMoreThanDef : ConditionDef
    {
        public ResourceRef<MetricDef> Amount { get; set; }
        public ResourceRef<EvaluatorDef> Evaluator { get; set; }
        //public override Type ExpressionType { get; } = typeof(EvaluatesToMoreThan);
    }

    public class HasDef : ConditionDef
    {
        public bool Not { get; set; }
        public ResourceRef<TargetSelectorDef> Target { get; set; }
        //public override Type ExpressionType { get; } = typeof(Has);
    }

    public class IsInRangeDef : ConditionDef
    {
        public bool Not { get; set; }
        public bool InverseSector { get; set; }
        public ResourceRef<MetricDef> Range { get; set; }
        // Leave them uninitialized (or st both to 0) to skip sector filter:
        public ResourceRef<MetricDef> SectorBorderL { get; set; }
        public ResourceRef<MetricDef> SectorBorderR { get; set; }

        public ResourceRef<MetricDef> HeightDeltaUpMax { get; set; }
        public ResourceRef<MetricDef> HeightDeltaDownMax { get; set; }
        public ResourceRef<TargetSelectorDef> TargetSelectorDef { get; set; }
        // Leave it uninitialized to use self pos. as basis point
        public ResourceRef<TargetSelectorDef> BasisSelectorDef { get; set; }
        //public override Type ExpressionType { get; } = typeof(IsInRange);
        // Try to don't forget to delete it from jdb before commit:
        public bool DebugDraw { get; set; }
    }

    public class IsLegionOfTypeDef : ConditionDef
    {

        public ResourceRef<TargetSelectorDef> Target { get; set; }
        public ResourceRef<LegionDef> LegionType { get; set; }
        //public override Type ExpressionType { get; } = typeof(IsLegionOfType);
    }

    public class IsStateValidDef : ConditionDef
    {
        //public override Type ExpressionType => typeof(IsStateValid);
        public ResourceRef<TargetSelectorDef> ValidSelector { get; set; }
    }

    public class NotDef : ConditionDef
    {
        public ResourceRef<ConditionDef> Condition { get; set; }
        //public override Type ExpressionType { get; } = typeof(Not);
    }

    public class OrDef : ConditionDef
    {
        public List<ResourceRef<ConditionDef>> Conditions { get; set; }
        //public override Type ExpressionType { get; } = typeof(Or);
    }

    public class SameLegionDef : ConditionDef
    {
        public ResourceRef<TargetSelectorDef> Target { get; set; }
        //public override Type ExpressionType { get; } = typeof(SameLegion);
    }

    public class TimeIsWithinIntervalDef : ConditionDef
    {
        public ResourceRef<InGameTimeIntervalDef> Interval { get; set; }

        //public override Type ExpressionType
        //{
        //    get
        //    {
        //        return typeof(TimeIsWithinInterval);
        //    }
        //}
    }

    public class TrueDef : ConditionDef
    {
        //public override Type ExpressionType { get; } = typeof(True);
    }

    public class VarIsTrueDef : ConditionDef
    {
        public string VarName { get; set; }
        //public override Type ExpressionType { get; } = typeof(VarIsTrue);
    }
}
