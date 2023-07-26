using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace SharedCode.EntitySystem
{
    [ProtoContract]
    public class DowngradeBatchContainer
    {
        [ProtoMember(1)]
        public List<DowngradeBatch> Batches = new List<DowngradeBatch>();
    }

    [ProtoContract]
    public class DowngradeBatch
    {
        [ProtoMember(1)]
        public int EntityTypeId { get; set; }

        [ProtoMember(2)]
        public Guid EntityId { get; set; }

        [ProtoMember(3)]
        public long DowngradeMask { get; set; }

        [ProtoMember(4)]
        public int Version { get; set; }

        [ProtoMember(5)]
        public int PreviousVersion { get; set; }
    }
}
