// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects.ReplicationInterfaces
{
    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Always, 170775661, typeof(SharedCode.Aspects.Utils.ILogableEntity))]
    public interface ILogableEntityAlways : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        bool IsCurveLoggerEnable
        {
            get;
        }
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, 1886893336, typeof(SharedCode.Aspects.Utils.ILogableEntity))]
    public interface ILogableEntityClientBroadcast : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        bool IsCurveLoggerEnable
        {
            get;
        }
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFullApi, -60750537, typeof(SharedCode.Aspects.Utils.ILogableEntity))]
    public interface ILogableEntityClientFullApi : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFull, 339279134, typeof(SharedCode.Aspects.Utils.ILogableEntity))]
    public interface ILogableEntityClientFull : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        bool IsCurveLoggerEnable
        {
            get;
        }
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ServerApi, 196623955, typeof(SharedCode.Aspects.Utils.ILogableEntity))]
    public interface ILogableEntityServerApi : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Server, 1077381630, typeof(SharedCode.Aspects.Utils.ILogableEntity))]
    public interface ILogableEntityServer : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        bool IsCurveLoggerEnable
        {
            get;
        }

        System.Threading.Tasks.Task SetCurveLoggerEnable(bool val);
    }
}