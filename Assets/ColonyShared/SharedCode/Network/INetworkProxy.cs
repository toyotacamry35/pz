using System;
using System.Net;
using SharedCode.Serializers;

namespace SharedCode.Network
{
    public delegate void RemoteCallCompletionFunc(byte[] data, int offset);

    public interface INetworkProxy
    {
        void OnMessageReceived(byte[] data);

        void SendMessage(byte[] data, int start, int length, MessageSendOptions sendOptions);

        bool SendMessage(byte[] data, int start, int length, MessageSendOptions sendOptions, Guid guid, RemoteCallCompletionFunc action, RemoteCallCompletionFunc exceptionAction);

        bool TransactionTimeout(Guid guid);

        void Close();

        ISerializer Serializer { get; }

        IPEndPoint RemoteEndpoint { get; }

        Guid RemoteRepoId { get; }
    }
}
