using System;
using System.Collections.Concurrent;
using System.Net;
using Core.Environment.Logging.Extension;
using GeneratedCode.HandlerProxyImplementations;
using GeneratedCode.Network;
using NLog;
using ProtoBuf;
using SharedCode.EntitySystem;
using SharedCode.Interfaces.Network;
using SharedCode.Serializers;

namespace SharedCode.Network
{
    public readonly struct NetworkProxyTransactionInfo
    {
        public readonly RemoteCallCompletionFunc Func;

        public readonly RemoteCallCompletionFunc ExceptionFunc;

        public NetworkProxyTransactionInfo(RemoteCallCompletionFunc func, RemoteCallCompletionFunc exceptionFunc)
        {
            Func = func;
            ExceptionFunc = exceptionFunc;
        }
    }

    public class NetworkProxy : INetworkProxy
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ConcurrentDictionary<Guid, NetworkProxyTransactionInfo> _completeHandlerActions = new ConcurrentDictionary<Guid, NetworkProxyTransactionInfo>();

        private readonly ServiceHandlersProxy _serviceHandlersProxy;

        private readonly INetworkChannel _networkChannel;

        public ISerializer Serializer { get; }

        public IPEndPoint RemoteEndpoint => _networkChannel.RemoteEndpoint;

        public Guid RemoteRepoId => _networkChannel.RemoteRepoId;

        public NetworkProxy(INetworkChannel networkChannel, in NetworkMessageDispatcher dispatcher, IEntitiesRepository repo)
        {
            if (networkChannel == null)
                throw new ArgumentException("networkChannel is null");
            _networkChannel = networkChannel;
            Serializer = ((IEntitiesRepositoryExtension)repo).Serializer;
            _serviceHandlersProxy = new ServiceHandlersProxy(dispatcher, repo);
        }

        [ProtoContract]
        public struct PacketHeader
        {
            [ProtoMember(1)]
            public PacketType PacketType;

            [ProtoMember(2)]
            public byte MethodId;

            [ProtoMember(3)]
            public bool HasMigrationId;
        }

        public void OnMessageReceived(byte[] data)
        {
            try
            {
                int offset = 0;
                var header = Serializer.Deserialize<PacketHeader>(data, ref offset);
                var guid = Guid.Empty;
                var migrationId = header.HasMigrationId ? Serializer.Deserialize<Guid>(data, ref offset) : Guid.Empty;

                switch (header.PacketType)
                {
                    case PacketType.Send:
                        _serviceHandlersProxy.OnMessageReceived(header.MethodId, data, offset, migrationId, this);
                        break;
                    case PacketType.Response:
                        {
                            //получили ответ на наш rpc
                            guid = Serializer.Deserialize<Guid>(data, ref offset);

                            if (!_completeHandlerActions.TryRemove(guid, out var networkProxyTransactionInfo))
                            {
                                GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcNetworkStatistics>.Instance.ReceiveResponseUnknownCompletedBytes(data.Length);
                                return;
                            }

                            _serviceHandlersProxy.OnTransactionInfoReceived(networkProxyTransactionInfo.Func, data, offset);
                            break;
                        }
                    case PacketType.Request:
                        //получили входящее rpc с guid транзакции
                        guid = Serializer.Deserialize<Guid>(data, ref offset);

                        _serviceHandlersProxy.OnMessageReceived(header.MethodId, data, offset, guid, migrationId, this);
                        break;
                    case PacketType.Exception:
                        {
                            //получили exception rpc с guid транзакции
                            guid = Serializer.Deserialize<Guid>(data, ref offset);

                            if (!_completeHandlerActions.TryRemove(guid, out var networkProxyTransactionInfo))
                            {
                                GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcNetworkStatistics>.Instance.ReceiveResponseUnknownExceptionBytes(data.Length);
                                return;
                            }

                            _serviceHandlersProxy.OnTransactionInfoReceived(networkProxyTransactionInfo.ExceptionFunc, data, offset);
                            break;
                        }
                    default:
                        Logger.IfError()?.Message("Network: Corrupted message header {0}, skipping message", header).Write();
                        return;
                }
            }
            catch (Exception e)
            {
                Logger.IfError()?.Message(e, "Exception on receive message").Write();;
            }
        }

        public bool TransactionTimeout(Guid guid)
        {
            return _completeHandlerActions.TryRemove(guid, out _);
        }

        public bool SendMessage(byte[] data, int start, int length, MessageSendOptions sendOptions, Guid guid, RemoteCallCompletionFunc action, RemoteCallCompletionFunc exceptionAction)
        {
            if (_completeHandlerActions.TryAdd(guid, new NetworkProxyTransactionInfo(action, exceptionAction)))
            {
                SendMessage(data, start, length, sendOptions);
                return true;
            }

            return false;

        }

        public void SendMessage(byte[] data, int start, int length, MessageSendOptions sendOptions)
        {
            _networkChannel.SendMessage(data, start, length, sendOptions);
        }

        public void Close()
        {
            _networkChannel.Close();
        }
    }
}
