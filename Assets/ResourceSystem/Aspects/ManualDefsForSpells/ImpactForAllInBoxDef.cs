using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates;
using Assets.Src.ResourcesSystem.Base;
using ColonyShared.SharedCode.Utils;
using SharedCode.Wizardry;
using System.Collections.Generic;

namespace Shared.ManualDefsForSpells
{
    public class ImpactForAllInBoxDef : SpellImpactDef
    {
        public List<ResourceRef<SpellDef>> AppliedSpells { get; set; }
        public Bounds[] AttackBoxes { get; set; }
        public ResourceRef<PredicateDef> PredicateOnTarget { get; set; }
        public override bool UnityAuthorityServerImpact => false;
    }

    public class ImpactNearestInBoxDef : SpellImpactDef
    {
        public List<ResourceRef<SpellDef>> AppliedSpells { get; set; }
        public Bounds[] AttackBoxes { get; set; }
        public ResourceRef<PredicateDef> PredicateOnTarget { get; set; }
        public override bool UnityAuthorityServerImpact => false;
    }
}
