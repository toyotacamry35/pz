using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedCode.EntitySystem;
using Assets.ColonyShared.SharedCode.Entities.Engine;
using SharedCode.Entities;
using SharedCode.EntitySystem.Delta;
using ResourceSystem.Utils;

namespace Assets.ColonyShared.SharedCode.Entities
{
    public interface IHasWorldPersonalMachineEngine
    {
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)] IWorldPersonalMachineEngine worldPersonalMachineEngine { get; set; }
    }
}

