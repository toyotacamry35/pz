using Assets.Src.ResourcesSystem.Base;
using ResourcesSystem.Loader;
using NLog;
using SharedCode.Aspects.Regions;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Collections.Concurrent;
using Core.Environment.Logging.Extension;

namespace Assets.ColonyShared.GeneratedCode.Regions
{
    public static class RegionBuildHelper
    {
        private const string RegionsPath = @"/ColonyShared/GeneratedCode/Regions/";

        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public static void LoadRegionsByMapName(IEnumerable<SceneChunkDef> maps, Guid sceneRef)
        {
            var regionDefs = new List<ARegionDef>();
            foreach (var map in maps.Select(x => ((IResource)x).Address.Root))
            {
                var mapPathSplitted = map.Split('/');
                var mapName = mapPathSplitted[mapPathSplitted.Length - 1];
                var mapNameWithoutExtension = mapName;
                var regionMapName = RegionsPath + mapNameWithoutExtension;
                if (!GameResourcesHolder.Instance.IsResourceExists(regionMapName))
                    continue;

                try
                {
                    regionDefs.Add(GameResourcesHolder.Instance.LoadResource<ARegionDef>(regionMapName));
                    Logger.IfInfo()?.Message("Succesfully loaded region def by name '{0}'", regionMapName).Write();
                }
                catch (Exception e)
                {
                    Logger.IfError()?.Message(e, "Failed to load region def by name: '{0}'", regionMapName).Write();
                }
            }
            RegionsHolder.CreateRootRegionWithGuid(regionDefs, sceneRef);
        }

        public static void RemoveRootRegionWithGuid(Guid id)
        {
            RegionsHolder._____INTERNAL____RemoveRootRegionWithGuid(id);
             
        }
        public static void LoadRegionsByMapName(string[] maps, Guid sceneRef)
        {
            var regionDefs = new List<ARegionDef>();
            foreach (var map in maps)
            {
                var mapPathSplitted = map.Split('/');
                var mapName = mapPathSplitted[mapPathSplitted.Length - 1];
                if (!mapName.EndsWith(".unity"))
                {
                    Logger.IfWarn()?.Message($"Incorrect map name extension ({mapName}) in map {map}").Write();
                    continue;
                }

                var mapNameWithoutExtension = mapName.Substring(0, mapName.Length - 6);
                var regionMapName = RegionsPath + mapNameWithoutExtension;
                if (!GameResourcesHolder.Instance.IsResourceExists(regionMapName))
                    continue;

                try
                {
                    regionDefs.Add(GameResourcesHolder.Instance.LoadResource<ARegionDef>(regionMapName));
                    Logger.IfInfo()?.Message("Succesfully loaded region def by name '{0}'", regionMapName).Write();
                }
                catch (Exception e)
                {
                    Logger.IfError()?.Message(e, "Failed to load region def by name: '{0}'", regionMapName).Write();
                }
            }
            RegionsHolder.CreateRootRegionWithGuid(regionDefs, sceneRef);
        }

        public static (IRegion root, Dictionary<ARegionDef, IRegion> regionsByDef) BuildRootRegion(ICollection<ARegionDef> regionDefs)
        {
            var regionsByDef = new Dictionary<ARegionDef, IRegion>();
            IRegion region = new RootRegion
            {
                RegionDef = new RootRegionDef { Data = new ResourceRef<ARegionDataDef>[0] }
            };
            var roots = new Stack<GeoRegionRoot>();
            foreach (var regionDef in regionDefs)
            {
                var buildRegion = BuildRegion(regionDef, 1, regionsByDef, roots, null);
                buildRegion.ParentRegion = region;
                region.AddChild(buildRegion);
            }

            return (region, regionsByDef);
        }

        public static IRegion BuildRegion(ARegionDef regionDef,
            int level,
            Dictionary<ARegionDef, IRegion> regionsByDef,
            Stack<GeoRegionRoot> roots,
            List<IndexedRegion> parentRegionGroupChildrenIndexedRegions)
        {
            if (regionDef == default(ARegionDef))
            {
                Logger.Log(LogLevel.Error, "Null reference passed to RegionBuildHelper.BuildRegion");
                return default;
            }

            var region = (IRegion) Activator.CreateInstance(DefToType.GetType(regionDef.GetType()));
            region.RegionDef = regionDef;
            region.InitRegionWithDef(regionDef);

            List<IndexedRegion> currentRegionGroupChildrenIndexedRegions = null;

            var indexedRegionGroup = region as IndexedRegionGroup;
            if (indexedRegionGroup != null)
            {
                currentRegionGroupChildrenIndexedRegions = new List<IndexedRegion>();
            }
            else if (region is IndexedRegion indexedRegion)
            {
                if (parentRegionGroupChildrenIndexedRegions != null)
                {
                    parentRegionGroupChildrenIndexedRegions.Add(indexedRegion);
                }
                else
                {
                    // Could IndexedRegion be without IndexedRegionGroup Parent?
                }
            }

            switch (region)
            {
                case GeoRegionRoot geoRegRoot:
                    roots.Push(geoRegRoot);
                    break;
                case GeoRegion gRegion when roots.Count < 1:
                    Logger.IfWarn()?.Message("No GeoRegionRoot found in parents of {0} Def:{1}", gRegion, regionDef.____GetDebugAddress()).Write();
                    break;
                case GeoRegion gRegion:
                    ((AABBHashedContainer<IRegion>) roots.Peek().SpatialTable).AddByRect(gRegion.AABB, gRegion);
                    break;
            }

            var defChildren = regionDef.ChildRegions;
            if (defChildren != default(ResourceRef<ARegionDef>[]))
            {
                foreach (var childDef in defChildren)
                {
                    var child = BuildRegion(childDef, level + 1, regionsByDef, roots, currentRegionGroupChildrenIndexedRegions ?? parentRegionGroupChildrenIndexedRegions);
                    child.ParentRegion = region;
                    region.AddChild(child);
                }
            }

            if (indexedRegionGroup != null)
            {
                indexedRegionGroup.IndexedChildRegions = currentRegionGroupChildrenIndexedRegions;

                if (parentRegionGroupChildrenIndexedRegions != null)
                {
                    // if there are parent region group then copping our indexedRegions to him
                    parentRegionGroupChildrenIndexedRegions.AddRange(currentRegionGroupChildrenIndexedRegions);
                }
            }


            if (region is GeoRegionRoot)
            {
                var r = roots.Pop();
                r.SpatialTable.Compact();
            }

            region.Level = level;
            regionsByDef.Add(regionDef, region);
            return region;
        }
    }
}