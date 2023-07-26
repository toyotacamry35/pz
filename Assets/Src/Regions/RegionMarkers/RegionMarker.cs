using Assets.Src.ResourcesSystem.Base;
using Assets.Src.ResourceSystem;
using SharedCode.Aspects.Regions;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Src.Regions.RegionMarkers
{
    public abstract class RegionMarker : MonoBehaviour
    {
        public abstract ARegionDef BuildDefs();
        public JdbMetadata BuildBlocker;
        public RegionMarker GetRegionParent()
        {
            var parentGO = gameObject.transform.parent;
            if (parentGO == null)
                return default(RegionMarker);
            else
            {
                var parentRegion = parentGO.GetComponent<RegionMarker>();
                if (parentRegion == default(RegionMarker))
                    Debug.LogError($"{gameObject.name} does not have parent region");
                return parentRegion;
            }
        }

        protected ICollection<RegionMarker> GetChildMarkers()
        {
            var childCount = gameObject.transform.childCount;
            if (childCount <= 0)
                return new RegionMarker[0];
            
            var children = new List<RegionMarker>(gameObject.transform.childCount);
            for (var i = 0; i < gameObject.transform.childCount; i++)
            {
                var childGO = gameObject.transform.GetChild(i);
                var child = childGO.GetComponent<RegionMarker>();
                if (child == null)
                    Debug.LogError($"{childGO.name} does not contain RegionMarker. Can't export {gameObject.name} region");
                else
                    children.Add(child);
            }
            return children;
        }

        protected ResourceRef<ARegionDataDef>[] GetRegionData()
        {
            var data = new List<ResourceRef<ARegionDataDef>>();
            var soundReg = gameObject.GetComponent<RegionSound>();
            if (soundReg != null)
                data.Add(new SoundRegionDataDef() { Switches = soundReg.GetSwitchIDs(), States = soundReg.GetStateIDs() });
            var buildBlockerDef = BuildBlocker?.Get<BuildBlockerDef>();
            if (buildBlockerDef != null)
                data.Add(buildBlockerDef);
            var weatherReg = gameObject.GetComponent<RegionWeather>();
            if (weatherReg != null)
            {
                var weatherDef = weatherReg.GetClimateZone();
                if (weatherDef != null)
                    data.Add(weatherDef);
            }
            var spellReg = gameObject.GetComponent<RegionSpell>();
            if (spellReg != null)
            {
                var spellRegionDef = spellReg.GetSpellRegionDef();
                if (spellRegionDef != default)
                    data.Add(spellRegionDef);
            }

            var fogOfWarReg = gameObject.GetComponent<FogOfWarRegion>();
            if (fogOfWarReg != null)
            {
                var fogOfWarRegionDataDef = fogOfWarReg.GetRegionDef();
                if (fogOfWarRegionDataDef != default)
                    data.Add(fogOfWarRegionDataDef);
            }
            return data.ToArray();
        }
    }
}
