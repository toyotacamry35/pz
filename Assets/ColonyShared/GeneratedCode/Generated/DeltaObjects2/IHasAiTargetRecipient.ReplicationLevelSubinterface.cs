// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects.ReplicationInterfaces
{
    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Always, -1383879721, typeof(SharedCode.AI.IHasAiTargetRecipient))]
    public interface IHasAiTargetRecipientAlways : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        GeneratedCode.DeltaObjects.ReplicationInterfaces.IAiTargetRecipientAlways AiTargetRecipient
        {
            get;
        }
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, -546352229, typeof(SharedCode.AI.IHasAiTargetRecipient))]
    public interface IHasAiTargetRecipientClientBroadcast : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        GeneratedCode.DeltaObjects.ReplicationInterfaces.IAiTargetRecipientClientBroadcast AiTargetRecipient
        {
            get;
        }
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFullApi, -1257738417, typeof(SharedCode.AI.IHasAiTargetRecipient))]
    public interface IHasAiTargetRecipientClientFullApi : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFull, -160317619, typeof(SharedCode.AI.IHasAiTargetRecipient))]
    public interface IHasAiTargetRecipientClientFull : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        GeneratedCode.DeltaObjects.ReplicationInterfaces.IAiTargetRecipientClientFull AiTargetRecipient
        {
            get;
        }
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ServerApi, 819726419, typeof(SharedCode.AI.IHasAiTargetRecipient))]
    public interface IHasAiTargetRecipientServerApi : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Server, -725306403, typeof(SharedCode.AI.IHasAiTargetRecipient))]
    public interface IHasAiTargetRecipientServer : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        GeneratedCode.DeltaObjects.ReplicationInterfaces.IAiTargetRecipientServer AiTargetRecipient
        {
            get;
        }
    }
}