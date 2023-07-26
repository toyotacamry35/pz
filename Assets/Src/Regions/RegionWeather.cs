using Assets.Src.ResourceSystem;
using Assets.Src.SpatialSystem;
using UnityEngine;

namespace Assets.Src.Regions
{
    public class RegionWeather : MonoBehaviour
    {
        public JdbMetadata _weatherDef;

        public ClimateZoneDef GetClimateZone()
        {
            if (_weatherDef == null)
            {
                Debug.LogError($"No Weather Def in RegionWeather component of {gameObject.name} gameobject. Can not export regions.");
                return default(ClimateZoneDef);
            }
            var wDef = _weatherDef.Get<ClimateZoneDef>();
            if (wDef == default(ClimateZoneDef))
            {
                Debug.LogError($"Wrong Weather Def type in RegionWeather component of {gameObject.name} gameobject. Can not export regions.");
                return default(ClimateZoneDef);
            }
            return wDef;
        }
    }
}
