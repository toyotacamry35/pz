using ProtoBuf;

namespace SharedCode.Cloud
{
    [ProtoContract]
    public class ConnectionToMasterNodeResult
    {
        [ProtoMember(1)]
        public string YourIp { get; set; }

        [ProtoMember(2)]
        public CloudNodesCommunicationState CloudNodesCommunicationState { get; set; }
    }
}
