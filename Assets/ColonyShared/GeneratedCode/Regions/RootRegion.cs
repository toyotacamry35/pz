using System.Collections.Generic;
using SharedCode.Aspects.Regions;
using SharedCode.Utils;
using SharedCode.Entities;
using System;
using SharedCode.Entities.Engine;
using System.Collections.Concurrent;
using SharedCode.EntitySystem;

namespace Assets.ColonyShared.GeneratedCode.Regions
{
    public class RootRegion : BaseRegion
    {
        public Lazy<ConcurrentAABBHashedContainer<IRegion>> SpatialTable { get; set; } = new Lazy<ConcurrentAABBHashedContainer<IRegion>>(() => new ConcurrentAABBHashedContainer<IRegion>());
        public Lazy<ConcurrentDictionary<(ModifierCauser key, OuterRef<IEntity>), IRegion>> CauserToRegion { get; set; } = new Lazy<ConcurrentDictionary<(ModifierCauser key, OuterRef<IEntity>), IRegion>>(() => new ConcurrentDictionary<(ModifierCauser key, OuterRef<IEntity>), IRegion>());
        
        public override void GetAllContainingRegionsNonAlloc(List<IRegion> regions, Vector3 pointCoords)
        {
            regions.Add(this);
            if (ChildRegions != null)
                foreach (var childRegion in ChildRegions)
                    childRegion.GetAllContainingRegionsNonAlloc(regions, pointCoords);

            if (SpatialTable.Value.GetByPoint(out var regionsSp, out var bigObjects, pointCoords))
            {
                foreach(var spReg in regionsSp)
                    if (spReg.IsInside(pointCoords))
                        regions.Add(spReg);
                foreach (var spReg in bigObjects)
                    if (spReg.IsInside(pointCoords))
                        regions.Add(spReg);
            }
        }

        public override void InitRegionWithDef(ARegionDef def, Transform providedTransform = default)
        {
            var defClass = def as RootRegionDef;
            RegionDef = defClass;
        }

        public override bool IsInside(Vector3 coords) => true;

        public void AddDynamicRegion((ModifierCauser, OuterRef<IEntity>) key, Transform t, GeoRegionDef def)
        {
            var reg = (GeoRegion)Activator.CreateInstance(DefToType.GetType(def.GetType()));
            reg.InitRegionWithDef(def, t);
            CauserToRegion.Value.TryAdd(key, reg);
            SpatialTable.Value.AddByRect(reg.AABB.StartCoords, reg.AABB.Dimensions, reg);
        }
        public void RemoveDynamicRegion((ModifierCauser, OuterRef<IEntity>) key)
        {
            CauserToRegion.Value.TryRemove(key, out var reg);
            if (reg != null)
                SpatialTable.Value.Remove(reg);
        }
    }
}