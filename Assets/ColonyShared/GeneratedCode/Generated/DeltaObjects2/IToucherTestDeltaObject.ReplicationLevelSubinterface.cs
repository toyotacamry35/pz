// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects.ReplicationInterfaces
{
    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Always, 548008729, typeof(GeneratedCode.EntityModel.Test.IToucherTestDeltaObject))]
    public interface IToucherTestDeltaObjectAlways : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, -1267025700, typeof(GeneratedCode.EntityModel.Test.IToucherTestDeltaObject))]
    public interface IToucherTestDeltaObjectClientBroadcast : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        int IntProperty
        {
            get;
        }

        System.Threading.Tasks.Task SetIntProperty(int i);
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFullApi, -413432885, typeof(GeneratedCode.EntityModel.Test.IToucherTestDeltaObject))]
    public interface IToucherTestDeltaObjectClientFullApi : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFull, 1376917665, typeof(GeneratedCode.EntityModel.Test.IToucherTestDeltaObject))]
    public interface IToucherTestDeltaObjectClientFull : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        int IntProperty
        {
            get;
        }

        System.Threading.Tasks.Task SetIntProperty(int i);
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ServerApi, 2067990712, typeof(GeneratedCode.EntityModel.Test.IToucherTestDeltaObject))]
    public interface IToucherTestDeltaObjectServerApi : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Server, -1238343443, typeof(GeneratedCode.EntityModel.Test.IToucherTestDeltaObject))]
    public interface IToucherTestDeltaObjectServer : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        int IntProperty
        {
            get;
        }

        System.Threading.Tasks.Task SetIntProperty(int i);
    }
}