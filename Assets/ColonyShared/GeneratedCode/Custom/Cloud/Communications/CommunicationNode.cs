using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using GeneratedCode.Custom.Network;
using GeneratedCode.Custom.Network.Udp;
using NLog;
using SharedCode.EntitySystem;
using SharedCode.Network;
using SharedCode.Interfaces.Network;
using GeneratedCode.Network;
using SharedCode.Cloud;

namespace GeneratedCode.Custom.Cloud.Communications
{
    public class CommunicationNode : INetworkChannelsNode, ICommunicationNode
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IEntitiesRepository _repository;
        private readonly Action<INetworkProxy> _onOpenDelegate;
        private readonly Action<INetworkProxy, bool> _onCloseDelegate;
        private readonly UdpNetworkWrapper _networkWrapper;

        private readonly NetworkMessageDispatcher _networkMessageDispatcher;

        public CommunicationNode(IEntitiesRepository repository,
            int port,
            IPAddress currentNodeAddress,
            int num,
            Action<INetworkProxy> onOpen,
            Action<INetworkProxy, bool> onClose, 
            CloudNodeType nodeType)
        {
            _repository = repository;
            _onOpenDelegate = onOpen;
            _onCloseDelegate = onClose;

            CurrentNodeAddress = currentNodeAddress;
            if (port > 0)
                Port = port + num;

            _networkMessageDispatcher = new NetworkMessageDispatcher(repository.Id, nodeType);

            var networkConfig = new UdpNetConfig()
            {
                Address = currentNodeAddress,
                Port = Port
            };

            _networkWrapper = new UdpNetworkWrapper(networkConfig, this);
            if (Port > 0)
                Logger.IfInfo()?.Message("Node {0} open host on address {1}:{2}", this, CurrentNodeAddress, Port).Write();
        }

        public IPAddress CurrentNodeAddress { get; }

        public int Port { get; }

        public void ConnectionOpened(INetworkChannel channel)
        {
            var proxy = new NetworkProxy(channel, _networkMessageDispatcher, _repository);
            channel.OnMessage += proxy.OnMessageReceived;
            channel.Tag = proxy;
            _onOpenDelegate(proxy);
        }

        public void ConnectionClosed(INetworkChannel channel, bool graceful)
        {
            var proxy = (NetworkProxy)channel.Tag;

            _onCloseDelegate(proxy, graceful);
            channel.Tag = null;
        }

        public Guid RepositoryId => _repository.Id;

        public async Task ConnectNode(IPEndPoint nodeIpEndpoint,  CancellationToken ct)
        {
            if (nodeIpEndpoint.Address.Equals(CurrentNodeAddress) && nodeIpEndpoint.Port == Port)
            {
                Logger.IfWarn()?.Message("{0}:{1} Skipping connection to {2}:{3}", CurrentNodeAddress, Port, nodeIpEndpoint.Address, nodeIpEndpoint.Port).Write();
                return;
            }

            using (var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, _repository.StopToken))
            {
                var token = linkedCts.Token;

                while (true)
                {
                    Logger.IfInfo()?.Message("{0}:{1} Try connect to cloudEntryPoint {2}:{3}", CurrentNodeAddress, Port, nodeIpEndpoint.Address, nodeIpEndpoint.Port).Write();
                    var delayTask = Task.Delay(TimeSpan.FromSeconds(5), token);
                    try
                    {
                        await _networkWrapper.Connect(nodeIpEndpoint, token);
                        Logger.IfInfo()?.Message("{0}:{1} connected to node {2}:{3}", CurrentNodeAddress, Port, nodeIpEndpoint.Address, nodeIpEndpoint.Port).Write();
                        return;
                    }
                    catch (Exception e)
                    {
                        Logger.IfError()?.Message(e, "{0}:{1} Connect to cloudEntryPoint {2}:{3} exception", CurrentNodeAddress, Port, nodeIpEndpoint.Address, nodeIpEndpoint.Port).Write();
                        if (token.IsCancellationRequested)
                            throw;
                    }

                    await delayTask;
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
                    _networkWrapper.Dispose();
                    _networkMessageDispatcher.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~CommunicationNode()
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
}