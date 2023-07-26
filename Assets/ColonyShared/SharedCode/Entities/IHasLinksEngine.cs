using Assets.ColonyShared.SharedCode.Entities.Engine;
using SharedCode.EntitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.ColonyShared.SharedCode.Entities
{
    public interface IHasLinksEngine
    {
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        ILinksEngine LinksEngine { get; set; }

    }

}
