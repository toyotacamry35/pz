using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedCode.Network;

namespace SharedCode.Cloud
{
    public delegate void RemoteNodeDelegate(IRemoteNode node);

    public interface IRemoteNode
    {
        Guid Id { get; }

        CloudNodeType NodeType { get; }

        string Host { get; }

        int ProcessId { get; }

        int Port { get; }

        INetworkProxy GetNetworkProxy();
    }
}
