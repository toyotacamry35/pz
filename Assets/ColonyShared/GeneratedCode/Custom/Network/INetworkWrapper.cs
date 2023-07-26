using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace GeneratedCode.Custom.Network
{
    public enum NetworkConnectRejectReason : int
    {
        Unknown,
        IncorrectVersion
    }

    /// <summary>
    /// Mirror of <See cref="LiteNetLib.DisconnectReason" />
    /// </summary>
    public enum DisconnectReason
    {
        ConnectionFailed,
        Timeout,
        HostUnreachable,
        RemoteConnectionClose,
        DisconnectPeerCalled,
        ConnectionRejected,
        InvalidProtocol,
    }

    public class ConnectionClosedException : Exception
    {
        public DisconnectReason Reason { get; }

        public NetworkConnectRejectReason RejectReason { get; }

        public IPEndPoint TargetEndpoint { get; }

        public ConnectionClosedException(string message, IPEndPoint endpoint, DisconnectReason reason, NetworkConnectRejectReason rejectReason)
            : base(message)
        {
            TargetEndpoint = endpoint;
            Reason = reason;
            RejectReason = rejectReason;
        }
    }

    public interface INetworkWrapper : IDisposable
    {
        Task Connect(IPEndPoint endpoint, CancellationToken ct);
    }
}
