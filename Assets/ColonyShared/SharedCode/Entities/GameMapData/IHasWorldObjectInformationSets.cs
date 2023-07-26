using System;
using System.Collections.Generic;
using System.Text;
using SharedCode.EntitySystem;

namespace SharedCode.Entities
{
    public interface IHasWorldObjectInformationSets
    {
        [ReplicationLevel(ReplicationLevel.ClientFull)]
        IWorldObjectInformationSetsEngine WorldObjectInformationSetsEngine { get; set; }
    }
}
