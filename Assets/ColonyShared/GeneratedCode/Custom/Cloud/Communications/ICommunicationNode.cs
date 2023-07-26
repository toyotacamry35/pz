using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace GeneratedCode.Custom.Cloud.Communications
{
    public interface ICommunicationNode : IDisposable
    {
        IPAddress CurrentNodeAddress { get; }

        int Port { get; }

        Task ConnectNode(IPEndPoint endpoint, CancellationToken ct);
    }
}
