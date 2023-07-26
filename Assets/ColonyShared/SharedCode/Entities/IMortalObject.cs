using System;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Entities.Service;
using Assets.Src.ResourcesSystem.Base;
using MongoDB.Bson.Serialization.Attributes;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Entities;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using SharedCode.Wizardry;
using TimeUnits = System.Int64;

namespace Assets.ColonyShared.SharedCode.Entities
{
    public interface IHasMortal
    {
        [LockFreeMutableProperty]
        [ReplicationLevel(ReplicationLevel.Always)]
        IMortal Mortal { get; set; }
    }

    [GeneratorAnnotations.GenerateDeltaObjectCode]
    public interface IMortal : IDeltaObject
    {
        [LockFreeMutableProperty]
        [ReplicationLevel(ReplicationLevel.Always)] bool IsAlive { get; set; }
        [LockFreeMutableProperty]
        [ReplicationLevel(ReplicationLevel.Always)] bool PermaDead { get; set; }
        [LockFreeMutableProperty]
        [BsonIgnore] [ReplicationLevel(ReplicationLevel.Always)] bool IsKnockedDown { get; set; }
        [BsonIgnore] [ReplicationLevel(ReplicationLevel.Master)] SpellId KnockDownSpellId { get; set; }
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)] TimeUnits LastResurrectTime { get; set; }
        [ReplicationLevel(ReplicationLevel.Master)] long LastDeathTime { get; set; }
        // Implement it this way: "{ await OnDieEvent(Id, TypeId, (PositionRotation)corpsePlace);  return true; }"
        //                        "{ await OnResurrectEvent(Id, TypeId, at);  return true; }"
        [ReplicationLevel(ReplicationLevel.Master)] Task<bool> InvokeDieEvent(PositionRotation corpsePlace);
        [ReplicationLevel(ReplicationLevel.Master)] Task<bool> InvokeResurrectEvent(PositionRotation at);
        
        [ReplicationLevel(ReplicationLevel.Always)]  
        event Func<Guid/*entityId*/, int/*entityTypeId*/, PositionRotation/*corpsePosition*/, Task> DieEvent;
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        event Func<Guid/*entityId*/, int/*entityTypeId*/, Task> BeforeResurrectEvent;
        ///#TC-4657: #TODO: Rename to `BornOrResurrectEvent`
        [ReplicationLevel(ReplicationLevel.Always)]
        event Func<Guid/*entityId*/, int/*entityTypeId*/, PositionRotation/*at*/, Task> ResurrectEvent;
        [ReplicationLevel(ReplicationLevel.Always)] event Func<Task> KnockedDown;
        [ReplicationLevel(ReplicationLevel.Always)] event Func<Task> ReviveFromKnockdown;

        [ReplicationLevel(ReplicationLevel.Master)] IDeltaDictionary<OuterRef<IEntity>, TimeUnits> LastStrike { get; set; }

        
        [EntityMethodCallType(EntityMethodCallType.ImmutableLocal)] //#TODO(aK): Удалить этот метод - просто читать `IsAlive` напрямую. Пока так, чтобы избежать объёмного (по кол-ву файлов) коммита
        [ReplicationLevel(ReplicationLevel.Always)] Task<bool> GetIsAlive();
        // Should be called from `ChangeHealth` of `I(Has)HealthEngine`. & Mortal logic decides, what to do in this case.
        [ReplicationLevel(ReplicationLevel.Server)] Task<bool> ZeroHealthReached();
        // Namely "Die". On just 0-health reached call `ZeroHealthReached` instead.
        [ReplicationLevel(ReplicationLevel.Server)] Task<bool> Die();
        [ReplicationLevel(ReplicationLevel.Server)] Task<bool> Resurrect(PositionRotation at);
        [ReplicationLevel(ReplicationLevel.Server)] Task<bool> KnockDown();
        [ReplicationLevel(ReplicationLevel.Server)] Task<bool> Revive();
        [ReplicationLevel(ReplicationLevel.ClientFull)] Task<bool> FinishOff();
        [ReplicationLevel(ReplicationLevel.Server)] Task<bool> DeactivatePreDeathState();
        // Save data pack about agressor to victim. In case victim 'll be dead after this hit:
        //  a) if it's mob, it 'll increase agressors' counter of killed mobs of its kind
        //  b) if it's player - this data 'll be shown to player - who killed you.
        [ReplicationLevel(ReplicationLevel.Server)] Task AddStrike(OuterRef<IEntity> objectRef);
        [ReplicationLevel(ReplicationLevel.Master)] ValueTask<MortalState> GetState();
        [ReplicationLevel(ReplicationLevel.Master)] Task HandleKnockdownSpellFinished(SpellId spellId, SpellFinishReason finishReason);
    }
    
    public enum MortalState { Alive, KnockedDown, Dead }
}
