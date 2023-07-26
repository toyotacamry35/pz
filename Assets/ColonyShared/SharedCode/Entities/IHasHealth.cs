using Assets.ColonyShared.SharedCode.Aspects.Damage;
using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using SharedCode.Entities.Engine;
using SharedCode.Entities.Mineable;
using SharedCode.EntitySystem;
using System;
using System.Threading.Tasks;

namespace SharedCode.Entities
{
    public interface IHasHealth
    {
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        IHealthEngine Health { get; set; }
    }

    public interface IHasHealthWithCustomMechanics : IHasHealth
    {
        [ReplicationLevel(ReplicationLevel.Master)] ValueTask<DamageResult> ReceiveDamageInternal(Damage incomingDamage, Guid attackerId, int attackerTypeId);
        [ReplicationLevel(ReplicationLevel.Master)] ValueTask<bool> ChangeHealthInternal(float health);
    }
}

