// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects.ReplicationInterfaces
{
    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Always, 2131985922, typeof(SharedCode.DeltaObjects.Building.IPositionedFenceElement))]
    public interface IPositionedFenceElementAlways : SharedCode.EntitySystem.IDeltaObject, IPositionedBuildAlways, IBuildTimerAlways, IStatsAlways, IInteractionAlways, IVisualAlways, IHasIdAlways, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, 645139927, typeof(SharedCode.DeltaObjects.Building.IPositionedFenceElement))]
    public interface IPositionedFenceElementClientBroadcast : SharedCode.EntitySystem.IDeltaObject, IPositionedBuildClientBroadcast, IBuildTimerClientBroadcast, IStatsClientBroadcast, IInteractionClientBroadcast, IVisualClientBroadcast, IHasIdClientBroadcast, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFullApi, 803854924, typeof(SharedCode.DeltaObjects.Building.IPositionedFenceElement))]
    public interface IPositionedFenceElementClientFullApi : SharedCode.EntitySystem.IDeltaObject, IPositionedBuildClientFullApi, IBuildTimerClientFullApi, IStatsClientFullApi, IInteractionClientFullApi, IVisualClientFullApi, IHasIdClientFullApi, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFull, -502486711, typeof(SharedCode.DeltaObjects.Building.IPositionedFenceElement))]
    public interface IPositionedFenceElementClientFull : SharedCode.EntitySystem.IDeltaObject, IPositionedBuildClientFull, IBuildTimerClientFull, IStatsClientFull, IInteractionClientFull, IVisualClientFull, IHasIdClientFull, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ServerApi, 1300105907, typeof(SharedCode.DeltaObjects.Building.IPositionedFenceElement))]
    public interface IPositionedFenceElementServerApi : SharedCode.EntitySystem.IDeltaObject, IPositionedBuildServerApi, IBuildTimerServerApi, IStatsServerApi, IInteractionServerApi, IVisualServerApi, IHasIdServerApi, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Server, 1021464109, typeof(SharedCode.DeltaObjects.Building.IPositionedFenceElement))]
    public interface IPositionedFenceElementServer : SharedCode.EntitySystem.IDeltaObject, IPositionedBuildServer, IBuildTimerServer, IStatsServer, IInteractionServer, IVisualServer, IHasIdServer, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }
}