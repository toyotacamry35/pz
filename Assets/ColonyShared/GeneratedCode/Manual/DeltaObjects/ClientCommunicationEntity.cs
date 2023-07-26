using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using SharedCode.Logging;
using NLog;
using SharedCode.Entities.Cloud;
using SharedCode.EntitySystem;
using SharedCode.Serializers;

namespace GeneratedCode.DeltaObjects
{
    public partial class ClientCommunicationEntity
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public Task GracefullLogoutImpl()
        {
            var id = Id;
            var repository = EntitiesRepository;
            AsyncUtils.RunAsyncTask(async () =>
            {
                using (var wrapper = await repository.Get<IClientCommunicationEntity>(id))
                {
                    await OnGracefullLogoutEvent();
                }
            }, repository);
            return Task.CompletedTask;
        }

        public Task DisconnectByAnotherConnectionImpl()
        {
             Log.Logger.IfError()?.Message("Your account nave been connected from another client. Your client disconnected").Write();
            var id = Id;
            var repository = EntitiesRepository;
            AsyncUtils.RunAsyncTask(async () =>
            {
                using (var wrapper = await repository.Get<IClientCommunicationEntity>(id))
                {
                    await OnDisconnectedByAnotherConnection();
                }
            }, repository);
            return Task.CompletedTask;
        }

        public Task SetLevelLoadedImpl()
        {
            LevelLoaded = true;
            return Task.CompletedTask;
        }

        public Task DisconnectByErrorImpl(string reason, string details)
        {
            AsyncUtils.RunAsyncTask(async () =>
            {
                await Task.Delay(1000);
                await OnDisconnectedByError(reason, details);
            }, EntitiesRepository);
            return Task.CompletedTask;
        }

        public Task ConfirmLoginImpl()
        {
            AsyncUtils.RunAsyncTask(async () =>
            {
                await Task.Delay(1000);
                await OnLoginConfirmed();
            }, EntitiesRepository);
            return Task.CompletedTask;
        }

        public Task<MapHostInitialInformation> GetMapHostInitialInformationImpl()
        {
            var result = new MapHostInitialInformation();
            return Task.FromResult(result);
        }

        public Task<bool> AddConectionImpl(string host, int port, Guid nodeId)
        {
            if (Connections.Any(x => x.Host == host && x.Port == port))
            {
                Logger.IfError()?.Message("Try to add already existing connection host {0} port {1}", host, port).Write();
                return Task.FromResult(false);
            }

            Connections.Add(new ConnectionInfo
            {
                Host = host,
                Port = port,
                NodeId = nodeId,

            });
            var repo = EntitiesRepository;
            AsyncUtils.RunAsyncTask(async () =>
            {
                try
                {
                    var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                    await ((IEntitiesRepositoryExtension)repo).ConnectExternal(host, port, cts.Token);
                }
                catch(Exception e)
                {
                    Logger.IfError()?.Message(e, "Cannot open connection to {0}:{1}", host, port).Write();
                    await OnDisconnectedByError(e.Message, e.ToString());
                }
            });

            return Task.FromResult(true);
        }

        public async Task<bool> RemoveConectionImpl(Guid nodeId)
        {
            var connection = Connections.FirstOrDefault(x => x.NodeId == nodeId);
            if (connection == null)
            {
                Logger.IfError()?.Message("Try to remove not existing connection host {0} port ", nodeId).Write();
                return false;
            }

            Connections.Remove(connection);

            Logger.IfInfo()?.Message("Remove connection {client_connection_host} {client_connection_port}", connection.Host, connection.Port).Write();
            var repo = EntitiesRepository;
            _ = AsyncUtils.RunAsyncTask(() =>
              {
                  ((IEntitiesRepositoryExtension)repo).DisconnectExternal(nodeId);
                  return Task.CompletedTask;
              });
            await Task.Delay(TimeSpan.FromSeconds(1));
            return true;
        }
    }
}
