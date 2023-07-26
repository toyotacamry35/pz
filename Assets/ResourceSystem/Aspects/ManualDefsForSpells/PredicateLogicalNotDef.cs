using Assets.Src.ResourcesSystem.Base;
using SharedCode.Wizardry;

namespace ColonyShared.ManualDefsForSpells
{
    public class PredicateLogicalNotDef: SpellPredicateDef
    {
        public ResourceRef<SpellPredicateDef> Predicate { get; set; }
    }
}