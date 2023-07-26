// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects.ReplicationInterfaces
{
    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Always, 1573476758, typeof(SharedCode.Entities.Engine.IHealthEngine))]
    public interface IHealthEngineAlways : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, 114874855, typeof(SharedCode.Entities.Engine.IHealthEngine))]
    public interface IHealthEngineClientBroadcast : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        System.Threading.Tasks.ValueTask<float> GetHealthCurrent();
        System.Threading.Tasks.ValueTask<float> GetMaxHealth();
        System.Threading.Tasks.ValueTask<float> GetMinHealth();
        System.Threading.Tasks.ValueTask<float> GetMaxHealthAbsolute();
        event SharedCode.Entities.Engine.DamageReceivedDelegate DamageReceivedEvent;
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFullApi, -1130297125, typeof(SharedCode.Entities.Engine.IHealthEngine))]
    public interface IHealthEngineClientFullApi : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFull, -1459462475, typeof(SharedCode.Entities.Engine.IHealthEngine))]
    public interface IHealthEngineClientFull : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        System.Threading.Tasks.ValueTask<float> GetHealthCurrent();
        System.Threading.Tasks.ValueTask<float> GetMaxHealth();
        System.Threading.Tasks.ValueTask<float> GetMinHealth();
        System.Threading.Tasks.ValueTask<float> GetMaxHealthAbsolute();
        event SharedCode.Entities.Engine.DamageReceivedDelegate DamageReceivedEvent;
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ServerApi, -1897746581, typeof(SharedCode.Entities.Engine.IHealthEngine))]
    public interface IHealthEngineServerApi : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Server, 1837901186, typeof(SharedCode.Entities.Engine.IHealthEngine))]
    public interface IHealthEngineServer : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        System.Threading.Tasks.ValueTask<float> GetHealthCurrent();
        System.Threading.Tasks.ValueTask<float> GetMaxHealth();
        System.Threading.Tasks.ValueTask<float> GetMinHealth();
        System.Threading.Tasks.ValueTask<float> GetMaxHealthAbsolute();
        System.Threading.Tasks.Task ChangeHealth(float deltaValue);
        System.Threading.Tasks.Task<Assets.ColonyShared.SharedCode.Aspects.Damage.DamageResult> ReceiveDamage(Assets.ColonyShared.SharedCode.Aspects.Damage.Damage damage, SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> aggressor);
        event SharedCode.Entities.Engine.DamageReceivedDelegate DamageReceivedEvent;
    }
}