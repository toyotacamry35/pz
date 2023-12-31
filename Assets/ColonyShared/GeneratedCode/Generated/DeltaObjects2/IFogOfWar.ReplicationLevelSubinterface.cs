// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects.ReplicationInterfaces
{
    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Always, -845129412, typeof(SharedCode.FogOfWar.IFogOfWar))]
    public interface IFogOfWarAlways : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, -2059784024, typeof(SharedCode.FogOfWar.IFogOfWar))]
    public interface IFogOfWarClientBroadcast : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        System.Threading.Tasks.Task ClearRegions();
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFullApi, 1526989059, typeof(SharedCode.FogOfWar.IFogOfWar))]
    public interface IFogOfWarClientFullApi : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFull, 2083386938, typeof(SharedCode.FogOfWar.IFogOfWar))]
    public interface IFogOfWarClientFull : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        SharedCode.EntitySystem.Delta.IDeltaDictionary<Assets.Src.SpatialSystem.IndexedRegionDef, bool> Regions
        {
            get;
        }

        SharedCode.EntitySystem.Delta.IDeltaList<Assets.Src.SpatialSystem.IndexedRegionDef> CurrentRegions
        {
            get;
        }

        SharedCode.EntitySystem.Delta.IDeltaDictionary<Assets.Src.SpatialSystem.IndexedRegionGroupDef, bool> RegionGroups
        {
            get;
        }

        SharedCode.EntitySystem.Delta.IDeltaList<Assets.Src.SpatialSystem.IndexedRegionGroupDef> CurrentGroups
        {
            get;
        }

        System.Threading.Tasks.Task<bool> SetRegionVisited(Assets.Src.SpatialSystem.IndexedRegionDef region);
        System.Threading.Tasks.Task<bool> SetGroupVisited(Assets.Src.SpatialSystem.IndexedRegionGroupDef group);
        System.Threading.Tasks.Task<float> GetGroupPercent(Assets.Src.SpatialSystem.IndexedRegionGroupDef group);
        System.Threading.Tasks.Task ClearRegions();
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ServerApi, -810791039, typeof(SharedCode.FogOfWar.IFogOfWar))]
    public interface IFogOfWarServerApi : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Server, 83290219, typeof(SharedCode.FogOfWar.IFogOfWar))]
    public interface IFogOfWarServer : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        SharedCode.EntitySystem.Delta.IDeltaDictionary<Assets.Src.SpatialSystem.IndexedRegionDef, bool> Regions
        {
            get;
        }

        SharedCode.EntitySystem.Delta.IDeltaList<Assets.Src.SpatialSystem.IndexedRegionDef> CurrentRegions
        {
            get;
        }

        SharedCode.EntitySystem.Delta.IDeltaDictionary<Assets.Src.SpatialSystem.IndexedRegionGroupDef, bool> RegionGroups
        {
            get;
        }

        SharedCode.EntitySystem.Delta.IDeltaList<Assets.Src.SpatialSystem.IndexedRegionGroupDef> CurrentGroups
        {
            get;
        }

        System.Threading.Tasks.Task<bool> DiscoverRegion(Assets.Src.SpatialSystem.IndexedRegionDef region);
        System.Threading.Tasks.Task<bool> SetRegionVisited(Assets.Src.SpatialSystem.IndexedRegionDef region);
        System.Threading.Tasks.Task<bool> DiscoverGroup(Assets.Src.SpatialSystem.IndexedRegionGroupDef group);
        System.Threading.Tasks.Task<bool> SetGroupVisited(Assets.Src.SpatialSystem.IndexedRegionGroupDef group);
        System.Threading.Tasks.Task<float> GetGroupPercent(Assets.Src.SpatialSystem.IndexedRegionGroupDef group);
        System.Threading.Tasks.Task ClearRegions();
    }
}