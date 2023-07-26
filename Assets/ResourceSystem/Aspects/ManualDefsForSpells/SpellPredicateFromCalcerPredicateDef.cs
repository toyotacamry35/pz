using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.Wizardry;

namespace ResourceSystem.Aspects.ManualDefsForSpells
{
    public class SpellPredicateFromCalcerPredicateDef : SpellPredicateDef
    {
        public ResourceRef<PredicateDef> Predicate;
    }
}