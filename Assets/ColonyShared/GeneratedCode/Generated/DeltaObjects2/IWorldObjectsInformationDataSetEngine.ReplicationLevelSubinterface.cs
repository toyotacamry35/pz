// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects.ReplicationInterfaces
{
    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Always, -1185598739, typeof(SharedCode.Entities.IWorldObjectsInformationDataSetEngine))]
    public interface IWorldObjectsInformationDataSetEngineAlways : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, 1694502647, typeof(SharedCode.Entities.IWorldObjectsInformationDataSetEngine))]
    public interface IWorldObjectsInformationDataSetEngineClientBroadcast : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFullApi, -2082391760, typeof(SharedCode.Entities.IWorldObjectsInformationDataSetEngine))]
    public interface IWorldObjectsInformationDataSetEngineClientFullApi : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFull, -585557191, typeof(SharedCode.Entities.IWorldObjectsInformationDataSetEngine))]
    public interface IWorldObjectsInformationDataSetEngineClientFull : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        Entities.GameMapData.WorldObjectInformationSetDef WorldObjectInformationSetDef
        {
            get;
        }
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ServerApi, 920274795, typeof(SharedCode.Entities.IWorldObjectsInformationDataSetEngine))]
    public interface IWorldObjectsInformationDataSetEngineServerApi : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Server, -748527024, typeof(SharedCode.Entities.IWorldObjectsInformationDataSetEngine))]
    public interface IWorldObjectsInformationDataSetEngineServer : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        Entities.GameMapData.WorldObjectInformationSetDef WorldObjectInformationSetDef
        {
            get;
        }
    }
}