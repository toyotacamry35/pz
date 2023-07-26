using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Assets.Src.Tools;
using Core.Environment.Logging.Extension;
using LiteNetLib;
using LiteNetLib.Utils;
using NLog;
using SharedCode.Config;

namespace GeneratedCode.Custom.Network.Udp
{
    public class UdpNetConfig
    {
        public IPAddress Address { get; set; }
        public int Port { get; set; }
        public int MaxConnectionCount { get; set; } = 20000;
        public int UpdatePeriodMilliseconds { get; set; } = 15;
        public int DisconnectTimeout { get; set; } = 60000;
        public string ConnectionKey { get; set; } = "ColonyGame";
    }

    public class UdpNetworkWrapper : INetworkWrapper, INetEventListener
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly NetManager _server;
        private IPAddress _currentBoundAddress;

        private readonly UdpNetConfig _config;

        private readonly INetworkChannelsNode _channelsNode;

        private readonly string _connectionKey;

#if ENABLE_NETWORK_STATISTICS
        private static readonly NLog.Logger _overallStatisticsLog = LogManager.GetLogger("Telemetry-NetworkStatistics");

        private DateTime _lastStatisticsLoggingTime = DateTime.UtcNow;

        private const int LogPeriodSeconds = 1;
#endif

        private readonly ConcurrentDictionary<UdpChannel, UdpChannel> _connections = new ConcurrentDictionary<UdpChannel, UdpChannel>();

        private readonly CancellationTokenSource _cts;

        static UdpNetworkWrapper()
        {
            NetDebug.Logger = new NetLogger();
        }

        public UdpNetworkWrapper(UdpNetConfig config, INetworkChannelsNode channelsNode)
        {
            _channelsNode = channelsNode;
            _config = config;
            _connectionKey = _config.ConnectionKey + SharedCode.Repositories.ReplicaTypeRegistry.NetworkHash.ToString();
            _server = new NetManager(this);
            _server.UnsyncedEvents = true;
            _server.NatPunchEnabled = true;
            _server.UpdateTime = _config.UpdatePeriodMilliseconds;
            _server.DisconnectTimeout = _config.DisconnectTimeout;
            _server.ReuseAddress = true;
            try
            {
                if (_config.Port != 0)
                {
                    Logger.IfInfo()?.Message("Server init, address is {address}, [prt {port}", _config.Address, _config.Port).Write();
                    _server.Start(_config.Address, IPAddress.IPv6None, _config.Port);
                    _currentBoundAddress = _config.Address;
                }
                else
                    Logger.IfInfo()?.Message("Skipping server init, port is 0").Write();
            }
            catch (Exception e)
            {
                Logger.IfError()?.Message(e, $"Couldn't start server on port {_config.Port}").Write();
                throw;
            }   
            _cts = new CancellationTokenSource();

            Thread t = new Thread(update);
            t.IsBackground = true;
            t.Start();

            Logger.IfInfo()?.Message("Started. Port {0} MaxConnection {1} Key {2}", _config.Port, _config.MaxConnectionCount, _connectionKey).Write();
        }

        public Task Connect(IPEndPoint endpoint, CancellationToken ct)
        {
            if(_config.Port == 0)
            {
                IPEndPoint localEndPoint;
                using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
                {
                    socket.Connect(endpoint);
                    localEndPoint = socket.LocalEndPoint as IPEndPoint;
                }
                Logger.IfInfo()?.Message("Found local endpoint {local_endpoint} for remote endpoint {remote_endpoint}", localEndPoint.Address, endpoint.Address).Write();

                if (_server.IsRunning)
                {
                    if (!_currentBoundAddress.Equals(localEndPoint.Address))
                    {
                        Logger.IfInfo()?.Message("Restarting server from address {old_address} to {new_address}", _currentBoundAddress, localEndPoint.Address).Write();
                        _server.Stop();
                        _currentBoundAddress = null;

                        _server.Start(localEndPoint.Address, IPAddress.IPv6None, 0);
                        _currentBoundAddress = localEndPoint.Address;
                    }
                }
                else
                {
                    _server.Start(localEndPoint.Address, IPAddress.IPv6None, 0);
                    _currentBoundAddress = localEndPoint.Address;
                }
            }

            var dataWriter = new NetDataWriter(true);
            var repositoryId = _channelsNode.RepositoryId;
            dataWriter.Put(repositoryId.ToByteArray());
            dataWriter.Put(_connectionKey);
            var peer = _server.Connect(endpoint, dataWriter);
            var tcs = new TaskCompletionSource<bool>();
            peer.Tag = tcs;
            ct.Register(CancelConnect, peer, false);

            return tcs.Task;
        }

        private static void CancelConnect(object peer) => ((NetPeer)peer).Disconnect();

        void update()
        {
#if ENABLE_NETWORK_STATISTICS
            var token = _cts.Token;
            while (true)
            {
                if (token.IsCancellationRequested)
                    break;


                if ((DateTime.UtcNow - _lastStatisticsLoggingTime).TotalSeconds >= LogPeriodSeconds)
                {
                    _lastStatisticsLoggingTime = _lastStatisticsLoggingTime.AddSeconds(LogPeriodSeconds);
                    logStatistics();
                }

                Thread.Sleep(LogPeriodSeconds * 1000);
            }
#endif
        }

#if ENABLE_NETWORK_STATISTICS
        private void logStatistics()
        {
            try
            {
                if (_overallStatisticsLog.IsInfoEnabled)
                    _overallStatisticsLog.Info("Overall statistics for port {port}: Received {received}, Sent {sent}",
                        _config.Port,
                        _server.Statistics.BytesReceived,
                        _server.Statistics.BytesSent);

                _server.Statistics.Reset();

                foreach (var udpChannel in _connections)
                    udpChannel.Value.LogStatistics(_config.Port);
            }
            catch (Exception e)
            {                
                Logger.IfError()?.Message(e, "logStatistics Exception").Write();;
            }
        }
#endif
        public void OnPeerConnected(NetPeer peer, NetDataReader dataReader)
        {
            if(peer == null)
            {
                Logger.IfFatal()?.Message("Port {0}: Peer is null", _config.Port).Write();
                return;
            }

            Logger.IfInfo()?.Message("Port {0}: New udp connection Id {1} peer {2}:{3}", _config.Port, peer.Id, peer.EndPoint.Address, peer.EndPoint.Port).Write();
            Guid remoteRepoId;
            switch (peer.Tag)
            {
                case Guid guid: // Accepted connection
                    remoteRepoId = guid;
                    break;
                case UdpChannel _: // Established connection
                    Logger.IfWarn()?.Message("Port {0}: connection {1} got OnPeerConnected for already connected peer {2}:{3}. Ignoring", _config.Port, peer.Id, peer.EndPoint.Address, peer.EndPoint.Port).Write();
                    return;
                case TaskCompletionSource<bool> tcs: // Pending connection
                    try
                    {
                        var byteArr = new byte[16];
                        dataReader.GetBytes(byteArr, 16);
                        remoteRepoId = new Guid(byteArr);
                        tcs.SetResult(true);
                        break;
                    }
                    catch (Exception e)
                    {
                        tcs.SetException(e);
                        return;
                    }
                default:
                    Logger.IfError()?.Message("Port {0}: connection {1} DataReader is null or empty and tag is not GUID. Disconnecting peer {2}:{3}", _config.Port, peer.Id, peer.EndPoint.Address, peer.EndPoint.Port).Write();
                    peer.Disconnect();
                    return;
            }

            var udpChannel = new UdpChannel(peer, remoteRepoId);
            _connections.TryAdd(udpChannel, udpChannel);
            _channelsNode.ConnectionOpened(udpChannel);
            peer.Tag = udpChannel;
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            switch (peer.Tag)
            {
                case Guid _: // Accepted connection
                    // Do nothing
                    break;
                case UdpChannel udpChannel: // Established connection
                    Logger.IfInfo()?.Message("Port {0}: Udp connection {1} disconnected. Reason {2} socket error code {3}", _config.Port, peer.Id, disconnectInfo.Reason, disconnectInfo.SocketErrorCode).Write();
                    _channelsNode.ConnectionClosed(udpChannel, disconnectInfo.Reason == LiteNetLib.DisconnectReason.RemoteConnectionClose);
                    _connections.TryRemove(udpChannel, out _);
                    break;
                case TaskCompletionSource<bool> tcs: // Pending connection
                    NetworkConnectRejectReason reason = NetworkConnectRejectReason.Unknown;
                    if (disconnectInfo.AdditionalData.AvailableBytes >= 4)
                    {
                        var rejectReason = (NetworkConnectRejectReason)disconnectInfo.AdditionalData.GetInt();
                        if (rejectReason == NetworkConnectRejectReason.IncorrectVersion)
                            Logger.IfError()?.Message("Port {0}: Connection rejected. Incorrect network version. Peer {1} Address {2} Port {3}", _config.Port, peer.Id, peer.EndPoint.Address.MapToIPv4().ToString(), peer.EndPoint.Port).Write();

                    }
                    tcs.SetException(new ConnectionClosedException($"Connection to {peer.EndPoint} was closed with reason {(DisconnectReason)(int)disconnectInfo.Reason}, Reject reason is {reason}", 
                        peer.EndPoint, (DisconnectReason)(int)disconnectInfo.Reason, reason));
                    break;
                default:
                    Logger.IfError()?.Message("Port {0}: Disconnected udp connection {1}. cant find. Disconnect reason {2} socket error code {3}", _config.Port, peer.Id, disconnectInfo.Reason, disconnectInfo.SocketErrorCode).Write();
                    break;
            }
            peer.Tag = null;
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
            Logger.IfError()?.Message("Port {0}: endpoint {1} receive error {2}", _config.Port, endPoint?.ToString() ?? "<null>", socketError.ToString()).Write();
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            var udpChannel = peer.Tag as UdpChannel;
            if(udpChannel != null)
            {
                udpChannel.InvokeOnMessage(reader.GetRemainingBytes());
                reader.Recycle();
                return;
            }

            Logger.IfError()?.Message("Port {0}: Udp channel not found for peer {1} Peer state {2}. DeliveryMethod {3}", _config.Port, peer.Id, peer.ConnectionState, deliveryMethod).Write();
        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
            var bytes = reader.GetBytesWithLength();
            Logger.IfWarn()?.Message("Port {0}: endpoint {1} receive unconnected message {2} size {3}", _config.Port, remoteEndPoint.ToString(), messageType.ToString(), bytes.Length).Write();
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
            //_log.Info("Udp channel {0} latency updated {1}", peer.ConnectId, latency);
        }

        public void OnConnectionRequest(ConnectionRequest request)
        {
            var byteArr = new byte[16];
            request.Data.GetBytes(byteArr, 16);
            var remoteRepoId = new Guid(byteArr);
            var connectionKey = request.Data.GetString();

            if (_connectionKey == connectionKey)
            {
                var dataWriter = new NetDataWriter(true);
                
                var localRepoId = _channelsNode.RepositoryId;
                dataWriter.Put(localRepoId.ToByteArray());

                request.Peer.Tag = remoteRepoId;
                request.Accept(dataWriter);
            }
            else
            {
                var rejectDataWriter = new NetDataWriter(true);
                rejectDataWriter.Put((int)NetworkConnectRejectReason.IncorrectVersion);
                request.Reject(rejectDataWriter);
                try
                {
                    Logger.IfWarn()?.Message("Port {0}: Connection rejected. Incorrect client network version: {3}. RemoteAddress: {1}, RemotePort: {2}", _config.Port, request.RemoteEndPoint.Address.MapToIPv4().ToString(), request.RemoteEndPoint.Port, connectionKey).Write();
                }
                catch(Exception e)
                {
                    Logger.IfError()?.Exception(e).Write();
                }
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (!_server.IsRunning)
                    {
                        Logger.IfError()?.Message("Port {0}: Server is not running", _config.Port).Write();
                        return;
                    }

                    Logger.IfInfo()?.Message("Port {0}: Stopping ...", _config.Port).Write();
                    _cts.Cancel();
                    _server.DisconnectAll();
                    _server.Stop();
                    _currentBoundAddress = null;

                    Logger.IfInfo()?.Message("Port {0}: Stopped", _config.Port).Write();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~UdpNetworkWrapper()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }

    public class NetLogger : INetLogger
    {
        private static readonly NLog.Logger _log = LogManager.GetCurrentClassLogger();

        public void WriteNet(NetLogLevel level, string str, params object[] args)
        {
            switch (level)
            {
                case NetLogLevel.Error:
                _log.IfError()?.Message(str, args).Write();
                break;
                case NetLogLevel.Warning:
                _log.IfWarn()?.Message(str, args).Write();
                break;
                case NetLogLevel.Info:
                _log.IfInfo()?.Message(str, args).Write();
                break;
                default:
                _log.IfInfo()?.Message(str, args).Write();
                break;
            }
        }
    }
}
