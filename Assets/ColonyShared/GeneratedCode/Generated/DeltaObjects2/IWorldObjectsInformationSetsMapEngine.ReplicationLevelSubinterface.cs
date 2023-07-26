// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects.ReplicationInterfaces
{
    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Always, -1695000986, typeof(SharedCode.Entities.IWorldObjectsInformationSetsMapEngine))]
    public interface IWorldObjectsInformationSetsMapEngineAlways : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, -1329417852, typeof(SharedCode.Entities.IWorldObjectsInformationSetsMapEngine))]
    public interface IWorldObjectsInformationSetsMapEngineClientBroadcast : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFullApi, 469711808, typeof(SharedCode.Entities.IWorldObjectsInformationSetsMapEngine))]
    public interface IWorldObjectsInformationSetsMapEngineClientFullApi : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFull, -368581527, typeof(SharedCode.Entities.IWorldObjectsInformationSetsMapEngine))]
    public interface IWorldObjectsInformationSetsMapEngineClientFull : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ServerApi, -1630334827, typeof(SharedCode.Entities.IWorldObjectsInformationSetsMapEngine))]
    public interface IWorldObjectsInformationSetsMapEngineServerApi : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Server, 603913021, typeof(SharedCode.Entities.IWorldObjectsInformationSetsMapEngine))]
    public interface IWorldObjectsInformationSetsMapEngineServer : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        System.Threading.Tasks.Task<System.Collections.Generic.Dictionary<Entities.GameMapData.WorldObjectInformationSetDef, ResourceSystem.Utils.OuterRef>> Subscribe(System.Collections.Generic.List<Entities.GameMapData.WorldObjectInformationSetDef> worldObjectSetsDef, System.Guid repositoryId);
        System.Threading.Tasks.Task<bool> Unsubscribe(System.Collections.Generic.List<Entities.GameMapData.WorldObjectInformationSetDef> worldObjectSetsDef, System.Guid repositoryId);
        System.Threading.Tasks.Task<bool> AddWorldObject(ResourceSystem.Utils.OuterRef worldObjectRef);
        System.Threading.Tasks.Task<bool> RemoveWorldObject(ResourceSystem.Utils.OuterRef worldObjectRef);
    }
}