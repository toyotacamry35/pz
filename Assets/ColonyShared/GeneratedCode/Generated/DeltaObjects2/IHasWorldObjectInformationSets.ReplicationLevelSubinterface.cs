// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects.ReplicationInterfaces
{
    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Always, -1046787567, typeof(SharedCode.Entities.IHasWorldObjectInformationSets))]
    public interface IHasWorldObjectInformationSetsAlways : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, 519940961, typeof(SharedCode.Entities.IHasWorldObjectInformationSets))]
    public interface IHasWorldObjectInformationSetsClientBroadcast : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFullApi, -515024959, typeof(SharedCode.Entities.IHasWorldObjectInformationSets))]
    public interface IHasWorldObjectInformationSetsClientFullApi : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFull, -1293894543, typeof(SharedCode.Entities.IHasWorldObjectInformationSets))]
    public interface IHasWorldObjectInformationSetsClientFull : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldObjectInformationSetsEngineClientFull WorldObjectInformationSetsEngine
        {
            get;
        }
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ServerApi, 1495680188, typeof(SharedCode.Entities.IHasWorldObjectInformationSets))]
    public interface IHasWorldObjectInformationSetsServerApi : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Server, 1172259116, typeof(SharedCode.Entities.IHasWorldObjectInformationSets))]
    public interface IHasWorldObjectInformationSetsServer : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldObjectInformationSetsEngineServer WorldObjectInformationSetsEngine
        {
            get;
        }
    }
}