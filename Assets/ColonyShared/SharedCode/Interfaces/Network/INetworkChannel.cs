using SharedCode.Network;
using System;
using System.Net;

namespace SharedCode.Interfaces.Network
{
    public interface INetworkChannel
    {
        void SendMessage(byte[] data, int start, int length, MessageSendOptions sendOptions);

        void Close();

        IPEndPoint RemoteEndpoint { get; }

        Guid RemoteRepoId { get; }

        event Action<byte[]> OnMessage;

        object Tag { get; set; }
    }
}
