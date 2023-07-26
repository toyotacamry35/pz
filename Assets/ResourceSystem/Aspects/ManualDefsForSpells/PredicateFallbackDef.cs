using System.Collections.Generic;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.Wizardry;

namespace Src.ManualDefsForSpells
{
    public class PredicateFallbackDef : SpellPredicateDef
    {
        public List<ResourceRef<SpellPredicateDef>> Predicates { get; set; }
        public ResourceRef<SpellDef> Spell { get; set; }
    }
}