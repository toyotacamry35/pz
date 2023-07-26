using SharedCode.Aspects.Regions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Core.Environment.Logging.Extension;

namespace Assets.ColonyShared.GeneratedCode.Regions
{
    public static class RegionsHolder
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private static readonly ConcurrentDictionary<Guid, (IRegion rootRegion, Dictionary<ARegionDef, IRegion> regionsByDef)> GuidToRegion = 
            new ConcurrentDictionary<Guid, (IRegion, Dictionary<ARegionDef, IRegion>)>();

        private static readonly Dictionary<Guid, int> _guidRefCounts = new Dictionary<Guid, int>();

        public static void CreateRootRegionWithGuid(List<ARegionDef> regionDefs, Guid guid)
        {
            lock (_guidRefCounts)
            {
                _guidRefCounts.TryGetValue(guid, out var prev);
                _guidRefCounts[guid] = prev + 1;

                if (prev > 0)
                    return;
            }

            var result = RegionBuildHelper.BuildRootRegion(regionDefs);
            GuidToRegion.TryAdd(guid, result);
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Root Region Created| Guid:{guid} Def:[{string.Join(",", regionDefs.Select(x => x.____GetDebugRootName()))}]").Write();
        }
        public static void _____INTERNAL____RemoveRootRegionWithGuid(Guid id)
        {
            lock (_guidRefCounts)
            {
                if (!_guidRefCounts.TryGetValue(id, out var cur))
                {
                    Logger.IfError()?.Message("Inconsistency in region load/unload logic").Write();
                    return;
                }
                if (cur > 1)
                    _guidRefCounts[id] = cur - 1;
                else
                {
                    _guidRefCounts.Remove(id);
                    GuidToRegion.TryRemove(id, out var _);
                }

            }
        }

        public static IRegion GetRegionByGuid(Guid guid)
        {
            GuidToRegion.TryGetValue(guid, out var region);
            return region.rootRegion;
        }

        public static IRegion GetRegionByDef(ARegionDef def, Guid rootGuid)
        {
            if (!GuidToRegion.TryGetValue(rootGuid, out var rootRegion))
                return null;

            rootRegion.regionsByDef.TryGetValue(def, out var region);
            return region;
        }
        
        public static (IRegion region, Dictionary<ARegionDef, IRegion> regionsByDef) GetRegionsByGuid(Guid guid)
        {
            GuidToRegion.TryGetValue(guid, out var tuple);
            return tuple;
        }

    }
}