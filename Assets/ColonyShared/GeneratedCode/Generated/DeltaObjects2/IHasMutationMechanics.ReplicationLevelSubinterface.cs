// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects.ReplicationInterfaces
{
    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Always, -206200206, typeof(SharedCode.Entities.IHasMutationMechanics))]
    public interface IHasMutationMechanicsAlways : SharedCode.EntitySystem.IDeltaObject, IHasFactionAlways, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, 446877049, typeof(SharedCode.Entities.IHasMutationMechanics))]
    public interface IHasMutationMechanicsClientBroadcast : SharedCode.EntitySystem.IDeltaObject, IHasFactionClientBroadcast, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        GeneratedCode.DeltaObjects.ReplicationInterfaces.IMutationMechanicsClientBroadcast MutationMechanics
        {
            get;
        }
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFullApi, 528220943, typeof(SharedCode.Entities.IHasMutationMechanics))]
    public interface IHasMutationMechanicsClientFullApi : SharedCode.EntitySystem.IDeltaObject, IHasFactionClientFullApi, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFull, 2090463166, typeof(SharedCode.Entities.IHasMutationMechanics))]
    public interface IHasMutationMechanicsClientFull : SharedCode.EntitySystem.IDeltaObject, IHasFactionClientFull, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        GeneratedCode.DeltaObjects.ReplicationInterfaces.IMutationMechanicsClientFull MutationMechanics
        {
            get;
        }
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ServerApi, -593444228, typeof(SharedCode.Entities.IHasMutationMechanics))]
    public interface IHasMutationMechanicsServerApi : SharedCode.EntitySystem.IDeltaObject, IHasFactionServerApi, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Server, -1856604869, typeof(SharedCode.Entities.IHasMutationMechanics))]
    public interface IHasMutationMechanicsServer : SharedCode.EntitySystem.IDeltaObject, IHasFactionServer, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        GeneratedCode.DeltaObjects.ReplicationInterfaces.IMutationMechanicsServer MutationMechanics
        {
            get;
        }
    }
}