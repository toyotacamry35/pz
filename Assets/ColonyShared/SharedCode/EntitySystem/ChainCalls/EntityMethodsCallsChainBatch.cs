using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace SharedCode.EntitySystem.ChainCalls
{
    [ProtoContract]
    public class EntityMethodsCallsChainBatch
    {
        [ProtoMember(1)]
        public Guid Id { get; set; } = Guid.NewGuid();

        [ProtoMember(2, OverwriteList = true, DynamicType = true)]
        public List<ChainBlockBase> Chain = new List<ChainBlockBase>();
    }
}
