using System;
using ProtoBuf;

namespace SharedCode.Cloud
{
    [ProtoContract]
    public class NodeState
    {
        [ProtoMember(1)]
        public Guid Id { get; set; }

        [ProtoMember(2)]
        public string Host { get; set; }

        [ProtoMember(3)]
        public int Port { get; set; }

        [ProtoMember(4)]
        public int CloudNodeType { get; set; }

        public override string ToString()
        {
            return string.Format("<RemoteNode {0} host:\"{1}\" port:{2} type:\"{3}\" >", Id, Host, Port, ((CloudNodeType)CloudNodeType).ToString());
        }
    }
}
