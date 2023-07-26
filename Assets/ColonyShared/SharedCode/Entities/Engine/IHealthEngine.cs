using SharedCode.EntitySystem;
using System;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Aspects.Damage;
using Assets.ColonyShared.SharedCode.Entities;
using SharedCode.Entities.GameObjectEntities;

namespace SharedCode.Entities.Engine
{
    
    public delegate Task DamageReceivedDelegate(float prevHealthVal, float newHealthVal, float maxHealth, Damage damage);
    public delegate Task DamageReceivedExtDelegate(float prevHealthVal, float newHealthVal, float maxHealth, MortalState prevState, MortalState newState, Damage damage);

    [GeneratorAnnotations.GenerateDeltaObjectCode]
    public interface IHealthEngine : IDeltaObject
    {
        [ReplicationLevel(ReplicationLevel.ClientBroadcast), EntityMethodCallType(EntityMethodCallType.ImmutableLocal)] ValueTask<float> GetHealthCurrent();
        [ReplicationLevel(ReplicationLevel.ClientBroadcast), EntityMethodCallType(EntityMethodCallType.ImmutableLocal)] ValueTask<float> GetMaxHealth();
        [ReplicationLevel(ReplicationLevel.ClientBroadcast), EntityMethodCallType(EntityMethodCallType.ImmutableLocal)] ValueTask<float> GetMinHealth();
        [ReplicationLevel(ReplicationLevel.ClientBroadcast), EntityMethodCallType(EntityMethodCallType.ImmutableLocal)] ValueTask<float> GetMaxHealthAbsolute();

        [ReplicationLevel(ReplicationLevel.Master)] bool AreadyInvokeZerohealth { get; set; } //TODO hack, исправить
        [ReplicationLevel(ReplicationLevel.Server)] Task ChangeHealth(float deltaValue); //TODO hack, исправить
        [ReplicationLevel(ReplicationLevel.Master)] Task ParseHealth();

        // It's proxy to call `On..Event` not only from inside entity (but from implementation class or by Interface ref, f.e.) (Boris: This restriction is for security reasons)
        [ReplicationLevel(ReplicationLevel.Master)] event Func<Guid, int, Task> ZeroHealthEvent;
        [ReplicationLevel(ReplicationLevel.Master)] Task<bool> InvokeZeroHealthEvent();

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)] event DamageReceivedDelegate DamageReceivedEvent;
        [ReplicationLevel(ReplicationLevel.Master)] event DamageReceivedExtDelegate DamageReceivedExtEvent; // Вызывается только если entity - IHasMortal
        [ReplicationLevel(ReplicationLevel.Server)] Task<DamageResult> ReceiveDamage(Damage damage, OuterRef<IEntity> aggressor);

        [ReplicationLevel(ReplicationLevel.Master)] Task OnResurrect(Guid arg1, int arg2, PositionRotation dummy);

        [ReplicationLevel(ReplicationLevel.Master)] Task StopDeathTimer();
        [ReplicationLevel(ReplicationLevel.Master)] Task StartDeathTimer(float timeToDeath);

        [ReplicationLevel(ReplicationLevel.Master)] Task DeathTimerElapsed();
    }
}