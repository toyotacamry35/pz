// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects.ReplicationInterfaces
{
    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Always, -986654998, typeof(SharedCode.Entities.Engine.IInputActionHandlers))]
    public interface IInputActionHandlersAlways : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, 1513991416, typeof(SharedCode.Entities.Engine.IInputActionHandlers))]
    public interface IInputActionHandlersClientBroadcast : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFullApi, 684823258, typeof(SharedCode.Entities.Engine.IInputActionHandlers))]
    public interface IInputActionHandlersClientFullApi : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFull, -2062621380, typeof(SharedCode.Entities.Engine.IInputActionHandlers))]
    public interface IInputActionHandlersClientFull : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        ColonyShared.SharedCode.InputActions.IInputActionLayersStack LayersStack
        {
            get;
        }

        ColonyShared.SharedCode.InputActions.IInputActionBindingsSource BindingsSource
        {
            get;
        }
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ServerApi, 802433268, typeof(SharedCode.Entities.Engine.IInputActionHandlers))]
    public interface IInputActionHandlersServerApi : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Server, 1038308452, typeof(SharedCode.Entities.Engine.IInputActionHandlers))]
    public interface IInputActionHandlersServer : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        ColonyShared.SharedCode.InputActions.IInputActionLayersStack LayersStack
        {
            get;
        }

        ColonyShared.SharedCode.InputActions.IInputActionBindingsSource BindingsSource
        {
            get;
        }
    }
}