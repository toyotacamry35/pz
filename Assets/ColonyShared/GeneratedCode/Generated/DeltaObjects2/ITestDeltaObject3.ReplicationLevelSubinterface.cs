// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects.ReplicationInterfaces
{
    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Always, 1980000559, typeof(Assets.ColonyShared.SharedCode.Entities.Test.ITestDeltaObject3))]
    public interface ITestDeltaObject3Always : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, 1980055369, typeof(Assets.ColonyShared.SharedCode.Entities.Test.ITestDeltaObject3))]
    public interface ITestDeltaObject3ClientBroadcast : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFullApi, 1788960972, typeof(Assets.ColonyShared.SharedCode.Entities.Test.ITestDeltaObject3))]
    public interface ITestDeltaObject3ClientFullApi : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFull, 2041882540, typeof(Assets.ColonyShared.SharedCode.Entities.Test.ITestDeltaObject3))]
    public interface ITestDeltaObject3ClientFull : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ServerApi, -1385192561, typeof(Assets.ColonyShared.SharedCode.Entities.Test.ITestDeltaObject3))]
    public interface ITestDeltaObject3ServerApi : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Server, 1973487600, typeof(Assets.ColonyShared.SharedCode.Entities.Test.ITestDeltaObject3))]
    public interface ITestDeltaObject3Server : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        int Test
        {
            get;
        }
    }
}