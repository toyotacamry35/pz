// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects.ReplicationInterfaces
{
    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Always, -704137464, typeof(Assets.ColonyShared.SharedCode.Entities.IHasComputableStateMachine))]
    public interface IHasComputableStateMachineAlways : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        GeneratedCode.DeltaObjects.ReplicationInterfaces.IComputableStateMachineAlways ComputableStateMachine
        {
            get;
        }
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, -1045602393, typeof(Assets.ColonyShared.SharedCode.Entities.IHasComputableStateMachine))]
    public interface IHasComputableStateMachineClientBroadcast : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        GeneratedCode.DeltaObjects.ReplicationInterfaces.IComputableStateMachineClientBroadcast ComputableStateMachine
        {
            get;
        }
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFullApi, -1610076128, typeof(Assets.ColonyShared.SharedCode.Entities.IHasComputableStateMachine))]
    public interface IHasComputableStateMachineClientFullApi : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFull, -116264273, typeof(Assets.ColonyShared.SharedCode.Entities.IHasComputableStateMachine))]
    public interface IHasComputableStateMachineClientFull : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        GeneratedCode.DeltaObjects.ReplicationInterfaces.IComputableStateMachineClientFull ComputableStateMachine
        {
            get;
        }
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ServerApi, -487952987, typeof(Assets.ColonyShared.SharedCode.Entities.IHasComputableStateMachine))]
    public interface IHasComputableStateMachineServerApi : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Server, -224522304, typeof(Assets.ColonyShared.SharedCode.Entities.IHasComputableStateMachine))]
    public interface IHasComputableStateMachineServer : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        GeneratedCode.DeltaObjects.ReplicationInterfaces.IComputableStateMachineServer ComputableStateMachine
        {
            get;
        }
    }
}