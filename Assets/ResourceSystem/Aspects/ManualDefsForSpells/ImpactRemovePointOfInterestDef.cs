using System.Collections.Generic;
using Assets.ColonyShared.SharedCode.Aspects.WorldObjects;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.Wizardry;

namespace ColonyShared.ManualDefsForSpells
{
    public class ImpactRemovePointOfInterestDef : SpellImpactDef
    {
        public ResourceRef<PointOfInterestDef> PointOfInterest { get; set; }
        public List<ResourceRef<PointOfInterestDef>> PointsOfInterest { get; set; }
        public ResourceRef<SpellEntityDef> Target { get; set; }
    }
}
