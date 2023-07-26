using System.Collections.Generic;
using Assets.Src.ResourcesSystem.Base;
using Newtonsoft.Json;
using SharedCode.Utils;
using SharedCode.Wizardry;

namespace Assets.ColonyShared.SharedCode.Aspects.Damage.Templates
{
    public class HitZonesDef : BaseResource
    {
        public Vector3Int GridSizeCells { get; set; }
        public Vector3 CellSize         { get; set; }
        public List<HitZone> Zones      { get; set; }
    }

    public struct HitZone
    {
        public ResourceRef<HitZoneTypeDef> ZoneType    { get; set; }
        // % of mob total health                       
        public float HealthPercent                     { get; set; }
        public ResourceRef<SpellDef> SpellOnHit        { get; set; }
        public ResourceRef<SpellDef> SpellOnZeroHealth { get; set; }
        // if ODD  cells number at curr dimension, their addresses are: [... , -1 , 0 , 1 , ...]. The `_geometricCenter` is in the very mid.of cell #0.
        // if EVEN cells number at curr dimension, their addresses are: [... , -1 , 1 , ...].     The `_geometricCenter` is on the very border b/w cells #1 & #-1.
        public List<Vector3Int> CellsAddresses         { get; set; }

        // Should be set to `true` manually where is needed
        [JsonIgnore]
        public bool IsInvalid;

        [JsonIgnore]
        public static HitZone Invalid { get; } = new HitZone { IsInvalid = true };
    }

    public class HitZoneTypeDef : BaseResource
    {
    }
}
