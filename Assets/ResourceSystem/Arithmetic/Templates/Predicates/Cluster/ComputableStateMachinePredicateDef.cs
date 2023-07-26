using Assets.Src.ResourcesSystem.Base;
using System.Collections.Generic;
using Assets.ColonyShared.SharedCode.Aspects.Templates;
using Assets.ColonyShared.SharedCode.Aspects.WorldObjects;
 
namespace Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates.Cluster
{
    // It's adapted copy-paste from `Predicate`.
    public abstract class ComputableStateMachinePredicateDef : BaseResource
    {
    }

    // --- Logic Predicates: ----------------------------------------------
    
    public class ComputableStateMachinePredicateTrueDef : ComputableStateMachinePredicateDef
    {
    }
    
    public class ComputableStateMachinePredicateFalseDef : ComputableStateMachinePredicateDef
    {
    }
    
    public class ComputableStateMachinePredicateInverseDef : ComputableStateMachinePredicateDef
    {
        public ResourceRef<ComputableStateMachinePredicateDef> Predicate { get; set; }
    }
    
    public class ComputableStateMachinePredicateAndDef : ComputableStateMachinePredicateDef
    {
        public List<ResourceRef<ComputableStateMachinePredicateDef>> Predicates { get; set; }
    }
    
    public class ComputableStateMachinePredicateOrDef : ComputableStateMachinePredicateDef
    {
        public List<ResourceRef<ComputableStateMachinePredicateDef>> Predicates { get; set; }
    }
    
    public class ComputableStateMachinePredicateInGameTimeDef : ComputableStateMachinePredicateDef
    {
        public ResourceRef<InGameTimeIntervalDef> TimeInterval { get; set; }
    }

    public class ComputableStateMachinePredicateExpiredLifespanPercentDef : ComputableStateMachinePredicateDef
    {
        public float FromIncluding { get; set; }
        public float TillExcluding { get; set; }
    }

}
