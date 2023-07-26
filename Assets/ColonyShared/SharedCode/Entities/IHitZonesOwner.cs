using System;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Aspects.Damage;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;
using SharedCode.MovementSync;
using SharedCode.Utils;

namespace SharedCode.Entities
{
    public interface IHitZonesOwner : IEntityObject
    {
        [ReplicationLevel(ReplicationLevel.Server)]  event Func<Damage, Task> HitZonesDamageReceivedEvent;

        // It's proxy to call `On..Event` not only from inside entity (but from implementation class or by Interface ref, f.e.) (Boris: This restriction is for security reasons)
        [ReplicationLevel(ReplicationLevel.Server)]  Task<bool> InvokeHitZonesDamageReceivedEvent(Damage damage);
    }
}
