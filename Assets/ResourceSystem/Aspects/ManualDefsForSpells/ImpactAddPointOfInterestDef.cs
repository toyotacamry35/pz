using System.Collections.Generic;
using Assets.ColonyShared.SharedCode.Aspects.WorldObjects;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.Wizardry;

namespace ColonyShared.ManualDefsForSpells
{
    public class ImpactAddPointOfInterestDef : SpellImpactDef, IPointOfInterestDefSource
    {
        public ResourceRef<PointOfInterestDef> PointOfInterest { get; set; }
        public List<ResourceRef<PointOfInterestDef>> PointsOfInterest { get; set; }
        public ResourceRef<SpellEntityDef> Target { get; set; }

        public PointOfInterestDef PoiDef => PointOfInterest.Target;
    }
}
