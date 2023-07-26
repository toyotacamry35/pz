using Assets.Src.ResourcesSystem.Base;
using Assets.Src.RubiconAI.BehaviourTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates;
using Assets.Src.RubiconAI.BehaviourTree.Expressions;
using Assets.Src.RubiconAI.KnowledgeSystem.Memory;

namespace Assets.Src.RubiconAI.BehaviourTree
{

    public abstract class BehaviourNodeDef : BaseResource
    {
        public string Comment { get; set; }
    }
    public class StrategyDef : BaseResource
    {
        public ResourceRef<BehaviourNodeDef> Plan { get; set; }
        public SelectorsDictionary Selectors { get; set; } = new SelectorsDictionary();
        public List<EventHandler> EventHandlers { get; set; } = new List<EventHandler>();

    }
    public class SelectorsDictionary : Dictionary<string, ResourceRef<BehaviourExpressionDef>> { }


    public interface IBehaviourExpressionDef : IResource
    {
        //Type ExpressionType { get; }
    }
    public abstract class BehaviourExpressionDef : BaseResource, IBehaviourExpressionDef
    {
        //public abstract Type ExpressionType { get; }
    }
    
    public class BehaviourSelectorDef : BehaviourExpressionDef
    {
        //public override Type ExpressionType { get; } = typeof(BehaviourSelectorDef);
    }

    public class CheckPredicateDef : BehaviourExpressionDef
    {
        public ResourceRef<PredicateDef> Predicate { get; set; } = new PredicateFalseDef();
        //public override Type ExpressionType { get; } = typeof(CheckPredicate);
    }

    public class CompareSelfWithTargetDef : BehaviourExpressionDef
    {
        public ResourceRef<TargetSelectorDef> TargetSelector { get; set; }
        public ResourceRef<CalcerDef<float>> TargetCalcer { get; set; }
        public ResourceRef<CalcerDef<float>> SelfCalcer { get; set; }
        public enum ComprasionType
        {
            SelfIsLess,
            SelfIsMore,
            EqualsInRange
        }
        public ComprasionType CompareType { get; set; }
        public float Range { get; set; }
        //public override Type ExpressionType { get; } = typeof(CompareSelfWithTarget);
    }

    public class SpellSelectorDef : BehaviourExpressionDef
    {
        public List<string> Spells { get; set; }
        //public override Type ExpressionType { get; } = typeof(SpellSelector);
    }

}

namespace Assets.Src.RubiconAI
{

    public class AIEventDef : BaseResource
    {

    }
    public struct EventHandler
    {
        public string EventType { get; set; }
        public bool NonInterruptable { get; set; }
        public ResourceRef<AIEventDef> EventDef { get; set; }
        public ResourceRef<StrategyDef> HandlerStrategy { get; set; }
    }
    public class KnowledgeCategoryDef : MemorizedStatDef
    {
    }
    public class KnowledgeSourceDef : BaseResource
    {

        public ResourceRef<KnowledgeCategoryDef> Category { get; set; }
    }
    public class KnowledgeSourceTransformerDef : KnowledgeSourceDef
    {
        public ResourceRef<KnowledgeCategoryDef> GetFrom { get; set; }
        public ResourceRef<MemorizedStatDef> InterpretAsStat { get; set; }
        public float TimeToRemember { get; set; }
        public ResourceRef<ConditionDef> Filter { get; set; }
    }
}
