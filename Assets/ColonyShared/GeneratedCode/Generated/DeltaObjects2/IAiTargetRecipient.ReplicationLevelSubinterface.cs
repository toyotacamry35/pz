// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects.ReplicationInterfaces
{
    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Always, -1222607271, typeof(SharedCode.AI.IAiTargetRecipient))]
    public interface IAiTargetRecipientAlways : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        ResourceSystem.Utils.OuterRef Target
        {
            get;
        }
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, -299210294, typeof(SharedCode.AI.IAiTargetRecipient))]
    public interface IAiTargetRecipientClientBroadcast : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        ResourceSystem.Utils.OuterRef Target
        {
            get;
        }
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFullApi, -35987081, typeof(SharedCode.AI.IAiTargetRecipient))]
    public interface IAiTargetRecipientClientFullApi : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFull, -666835090, typeof(SharedCode.AI.IAiTargetRecipient))]
    public interface IAiTargetRecipientClientFull : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        ResourceSystem.Utils.OuterRef Target
        {
            get;
        }
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ServerApi, 375101020, typeof(SharedCode.AI.IAiTargetRecipient))]
    public interface IAiTargetRecipientServerApi : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Server, 572165213, typeof(SharedCode.AI.IAiTargetRecipient))]
    public interface IAiTargetRecipientServer : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        ResourceSystem.Utils.OuterRef Target
        {
            get;
        }
    }
}