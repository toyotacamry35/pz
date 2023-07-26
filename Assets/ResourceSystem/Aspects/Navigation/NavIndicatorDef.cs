using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.Src.ResourcesSystem.Base;
using UnityEngine;

namespace Assets.ColonyShared.SharedCode.Aspects.Navigation
{
    public class NavIndicatorDef : SaveableBaseResource
    {
        public UnityRef<Sprite> Icon { get; set; }
        public string IconColorHexCode { get; set; }
        public float FovToDisplay { get; set; }
        public float DistanceToDisplay { get; set; }

        public UnityRef<Sprite> MapIcon { get; set; }
        public bool IsSelectable { get; set; }
        public bool IsShowDist { get; set; }
        public int QuestZoneDiameter { get; set; }
        public bool IsPlayer { get; set; }

        public int AccLevel { get; set; }
    }
}