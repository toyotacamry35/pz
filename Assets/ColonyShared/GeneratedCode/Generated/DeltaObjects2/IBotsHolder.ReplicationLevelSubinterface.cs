// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects.ReplicationInterfaces
{
    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Always, -1943423960, typeof(GeneratedCode.EntityModel.Bots.IBotsHolder))]
    public interface IBotsHolderAlways : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        SharedCode.EntitySystem.Delta.IDeltaDictionary<System.Guid, bool> Bots
        {
            get;
        }
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, 24457465, typeof(GeneratedCode.EntityModel.Bots.IBotsHolder))]
    public interface IBotsHolderClientBroadcast : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        SharedCode.EntitySystem.Delta.IDeltaDictionary<System.Guid, bool> Bots
        {
            get;
        }
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFullApi, 607970127, typeof(GeneratedCode.EntityModel.Bots.IBotsHolder))]
    public interface IBotsHolderClientFullApi : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFull, -1502945935, typeof(GeneratedCode.EntityModel.Bots.IBotsHolder))]
    public interface IBotsHolderClientFull : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        SharedCode.EntitySystem.Delta.IDeltaDictionary<System.Guid, bool> Bots
        {
            get;
        }
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ServerApi, 1273747890, typeof(GeneratedCode.EntityModel.Bots.IBotsHolder))]
    public interface IBotsHolderServerApi : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Server, -2025655432, typeof(GeneratedCode.EntityModel.Bots.IBotsHolder))]
    public interface IBotsHolderServer : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        SharedCode.EntitySystem.Delta.IDeltaDictionary<System.Guid, bool> Bots
        {
            get;
        }
    }
}