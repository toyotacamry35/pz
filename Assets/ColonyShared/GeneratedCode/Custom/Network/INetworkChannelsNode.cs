using System;
using SharedCode.Interfaces.Network;
using SharedCode.Network;

namespace GeneratedCode.Custom.Network
{
    public interface INetworkChannelsNode
    {
        void ConnectionOpened(INetworkChannel channel);
        void ConnectionClosed(INetworkChannel channel, bool graceful);

        Guid RepositoryId { get; }
    }
}
