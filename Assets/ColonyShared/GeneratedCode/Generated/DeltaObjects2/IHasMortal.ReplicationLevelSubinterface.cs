// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects.ReplicationInterfaces
{
    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Always, 8530783, typeof(Assets.ColonyShared.SharedCode.Entities.IHasMortal))]
    public interface IHasMortalAlways : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        GeneratedCode.DeltaObjects.ReplicationInterfaces.IMortalAlways Mortal
        {
            get;
        }
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, -1486926935, typeof(Assets.ColonyShared.SharedCode.Entities.IHasMortal))]
    public interface IHasMortalClientBroadcast : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        GeneratedCode.DeltaObjects.ReplicationInterfaces.IMortalClientBroadcast Mortal
        {
            get;
        }
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFullApi, 2422233, typeof(Assets.ColonyShared.SharedCode.Entities.IHasMortal))]
    public interface IHasMortalClientFullApi : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFull, 822924480, typeof(Assets.ColonyShared.SharedCode.Entities.IHasMortal))]
    public interface IHasMortalClientFull : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        GeneratedCode.DeltaObjects.ReplicationInterfaces.IMortalClientFull Mortal
        {
            get;
        }
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ServerApi, 2829094, typeof(Assets.ColonyShared.SharedCode.Entities.IHasMortal))]
    public interface IHasMortalServerApi : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Server, 1044530207, typeof(Assets.ColonyShared.SharedCode.Entities.IHasMortal))]
    public interface IHasMortalServer : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        GeneratedCode.DeltaObjects.ReplicationInterfaces.IMortalServer Mortal
        {
            get;
        }
    }
}