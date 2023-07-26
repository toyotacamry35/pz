using Assets.ColonyShared.GeneratedCode.Time;
using Assets.Src.ResourcesSystem.Base;
using Assets.Src.SpatialSystem;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using NLog;
using SharedCode.Aspects.Regions;
using SharedCode.EntitySystem;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.ColonyShared.GeneratedCode.Regions.RegionProcessors
{
    public class WeatherRegionProcessor : MonoBehaviour
    {
        internal static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public enum WeatherType
        {
            Toxicity,
            Humidity, Oxygen,
            Temperature,
            Wind
        }

        public static float ProcessWeatherRegions(IEnumerable<IRegion> regions, WeatherType weatherType, float defaultValue = 0)
        {
            var maxLevel = -1;
            ClimateZoneDef currentClimateZone = default(ClimateZoneDef);
            if (regions != null)
                foreach (var region in regions)
                {
                    if (region.Level > maxLevel && region.RegionDef != default(ARegionDef))
                        foreach (var data in region.RegionDef.Data)
                        {
                            var climateZoneCandidate = data.Target as ClimateZoneDef;
                            if (climateZoneCandidate != null)
                            {
                                maxLevel = region.Level;
                                currentClimateZone = climateZoneCandidate;
                                continue;
                            }
                        }
                }
            if (currentClimateZone != default(ClimateZoneDef))
                return GetCurrentClimateStat(currentClimateZone, weatherType, defaultValue);
            else
                return defaultValue;
        }

        public static float GetCurrentClimateStat(ClimateZoneDef climateZoneDef, WeatherType weatherType, float defaultValue)
        {
            if (climateZoneDef == null)
                return defaultValue;

            var inGameTime = RegionTime.IngameTime;
            var discreteHour = inGameTime.Hour;
            ClimateZoneDef.ScheduleRecord scheduledModifier = null;
            int hour = -1;
            foreach (var scheduledMod in climateZoneDef.Schedule)
            {
                if (scheduledMod.Key <= discreteHour && scheduledMod.Key > hour)
                {
                    hour = scheduledMod.Key;
                    scheduledModifier = scheduledMod.Value;
                }
            }
            if (scheduledModifier == null)
                return defaultValue;

            float result = defaultValue;
            switch (weatherType)
            {
                case WeatherType.Toxicity:
                    result = scheduledModifier.Toxic;
                    break;
                case WeatherType.Humidity:
                    result = scheduledModifier.Humidity;
                    break;
                case WeatherType.Oxygen:
                    result = scheduledModifier.Oxygen;
                    break;
                case WeatherType.Temperature:
                    result = scheduledModifier.Temperature;
                    break;
                case WeatherType.Wind:
                    result = scheduledModifier.Wind;
                    break;
            }
            return result;
        }
    }
}
