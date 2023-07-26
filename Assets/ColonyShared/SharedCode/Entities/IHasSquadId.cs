using System;
using SharedCode.EntitySystem;

namespace ColonyShared.SharedCode.Entities
{
    public interface IHasSquadId
    {
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        Guid SquadId { get; set; }
    }
}