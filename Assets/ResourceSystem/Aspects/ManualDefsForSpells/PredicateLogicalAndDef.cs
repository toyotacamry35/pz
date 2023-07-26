using System.Collections.Generic;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.Wizardry;

namespace ColonyShared.ManualDefsForSpells
{
    public class PredicateLogicalAndDef : SpellPredicateDef
    {
        public List<ResourceRef<SpellPredicateDef>> Predicates { get; set; }
    }
}