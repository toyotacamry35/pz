// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects.ReplicationInterfaces
{
    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Always, -1838666181, typeof(ColonyShared.SharedCode.Aspects.Combat.IHasAttackEngine))]
    public interface IHasAttackEngineAlways : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, -153937536, typeof(ColonyShared.SharedCode.Aspects.Combat.IHasAttackEngine))]
    public interface IHasAttackEngineClientBroadcast : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFullApi, 578773617, typeof(ColonyShared.SharedCode.Aspects.Combat.IHasAttackEngine))]
    public interface IHasAttackEngineClientFullApi : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFull, 2096018849, typeof(ColonyShared.SharedCode.Aspects.Combat.IHasAttackEngine))]
    public interface IHasAttackEngineClientFull : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        GeneratedCode.DeltaObjects.ReplicationInterfaces.IAttackEngineClientFull AttackEngine
        {
            get;
        }
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ServerApi, 1694931564, typeof(ColonyShared.SharedCode.Aspects.Combat.IHasAttackEngine))]
    public interface IHasAttackEngineServerApi : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Server, -122785139, typeof(ColonyShared.SharedCode.Aspects.Combat.IHasAttackEngine))]
    public interface IHasAttackEngineServer : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        GeneratedCode.DeltaObjects.ReplicationInterfaces.IAttackEngineServer AttackEngine
        {
            get;
        }
    }
}