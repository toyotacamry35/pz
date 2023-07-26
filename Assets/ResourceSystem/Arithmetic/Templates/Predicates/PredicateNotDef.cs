using Assets.Src.ResourcesSystem.Base;

namespace Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates
{
    public class PredicateNotDef : PredicateDef
    {
        public ResourceRef<PredicateDef> Value { get; set; }
    }
}
