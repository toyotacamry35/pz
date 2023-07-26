// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects.ReplicationInterfaces
{
    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Always, -1575482461, typeof(SharedCode.Entities.IWorldObjectPositionInformation))]
    public interface IWorldObjectPositionInformationAlways : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, -1204208307, typeof(SharedCode.Entities.IWorldObjectPositionInformation))]
    public interface IWorldObjectPositionInformationClientBroadcast : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        SharedCode.Utils.Vector3 Position
        {
            get;
        }
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFullApi, 828281744, typeof(SharedCode.Entities.IWorldObjectPositionInformation))]
    public interface IWorldObjectPositionInformationClientFullApi : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFull, -404424610, typeof(SharedCode.Entities.IWorldObjectPositionInformation))]
    public interface IWorldObjectPositionInformationClientFull : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        SharedCode.Utils.Vector3 Position
        {
            get;
        }
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ServerApi, -1730264060, typeof(SharedCode.Entities.IWorldObjectPositionInformation))]
    public interface IWorldObjectPositionInformationServerApi : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Server, -1898446413, typeof(SharedCode.Entities.IWorldObjectPositionInformation))]
    public interface IWorldObjectPositionInformationServer : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        SharedCode.Utils.Vector3 Position
        {
            get;
        }

        System.Threading.Tasks.Task<bool> SetPosition(SharedCode.Utils.Vector3 position);
    }
}