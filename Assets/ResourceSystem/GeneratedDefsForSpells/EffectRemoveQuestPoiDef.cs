using System.Collections.Generic;
using System.Linq;
using Assets.ColonyShared.SharedCode.Aspects.WorldObjects;
using Assets.Src.ResourcesSystem.Base;
using EnumerableExtensions;
using SharedCode.Wizardry;

namespace GeneratedDefsForSpells
{
    public class EffectRemoveQuestPoiDef : SpellEffectDef
    {
        public ResourceRef<SpellTargetDef> Target { get; set; }
        public ResourceRef<PointOfInterestDef> PointOfInterest { get; set; }
        public List<ResourceRef<PointOfInterestDef>> PointsOfInterest { get; set; }

        public override string ToString()
        {
            return $"Poi={(PointOfInterest.IsValid ? PointOfInterest.Target.ToString() : "null")}, " +
                   $"PoiList={(PointsOfInterest != null ? PointsOfInterest.Select(elemRef => elemRef.Target).ItemsToString() : "null")}";
        }
    }
}