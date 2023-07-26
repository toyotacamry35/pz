using System;
using System.Collections.Generic;
using System.Linq;
using ProtoBuf;

namespace SharedCode.Cloud
{
    [ProtoContract]
    public class CloudNodesCommunicationState
    {
        [ProtoMember(1)]
        public bool IsReady { get; set; }

        [ProtoMember(2)]
        public List<NodeState> Nodes { get; set; } = new List<NodeState>();
    }
}
