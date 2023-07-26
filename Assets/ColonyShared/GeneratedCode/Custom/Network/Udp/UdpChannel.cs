using System;
using System.Net;
using Core.Environment.Logging.Extension;
using LiteNetLib;
using NLog;
using SharedCode.Interfaces.Network;
using SharedCode.Network;

namespace GeneratedCode.Custom.Network.Udp
{
    public class UdpChannel : INetworkChannel
    {
        private static readonly NLog.Logger _perPeerStatisticsLog = LogManager.GetLogger("Telemetry-NetworkStatisticsPerPeer");

        public event Action<byte[]> OnMessage;

        public object Tag { get; set; }

        public Guid RemoteRepoId { get; }
        private readonly NetPeer _peer;

        private readonly string _ipStr;

        public UdpChannel(NetPeer peer, Guid remoteRepoId)
        {
            _peer = peer;
            _ipStr = _peer.EndPoint.Address.MapToIPv4().ToString();
            RemoteRepoId = remoteRepoId;
        }

        public void InvokeOnMessage(byte[] data) => OnMessage?.Invoke(data);

        public void SendMessage(byte[] data, int start, int length, MessageSendOptions sendOptions)
        {
            _peer.Send(data, start, length, (DeliveryMethod)(int)sendOptions);
        }

        public void Close()
        {
            _peer.NetManager.DisconnectPeer(_peer);
        }

        public IPEndPoint RemoteEndpoint => _peer.EndPoint;

#if ENABLE_NETWORK_STATISTICS
        public void LogStatistics(int port)
        {
            _perPeerStatisticsLog.IfInfo()?.Message("Channel statistics for " +
                "port {port}, " +
                "peer {peer}, " +
                "remote address {remote_address}, " +
                "remote port {remote_port}: " +
                "Received {received}, " +
                "Sent {sent}",
                    port,
                    _peer.Id,
                    _ipStr,
                    _peer.EndPoint.Port,
                    _peer.Statistics.BytesReceived,
                    _peer.Statistics.BytesSent
                ).Write();
            _peer.Statistics.Reset();
        }
#endif

        public override string ToString()
        {
            return string.Format("<UdpChannel Id {0} endpoint {1}:{2}>", _peer.Id, _peer.EndPoint.Address, _peer.EndPoint.Port);
        }
    }
}
