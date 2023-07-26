using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace SharedCode.EntitySystem
{
    [ProtoContract]
    public class DestroyBatchContainer
    {
        [ProtoMember(1)]
        public List<DestroyBatch> Batches = new List<DestroyBatch>();
    }

    [ProtoContract]
    public class DestroyBatch
    {
        [ProtoMember(1)]
        public int EntityTypeId { get; set; }

        [ProtoMember(2)]
        public Guid EntityId { get; set; }
    }
}
