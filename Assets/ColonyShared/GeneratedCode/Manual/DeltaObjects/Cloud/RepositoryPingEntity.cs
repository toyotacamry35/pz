using System;
using SharedCode.EntitySystem;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;

namespace GeneratedCode.DeltaObjects
{
    public partial class RepositoryPingEntity
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public Task<bool> PingImpl()
        {
            Logger.IfDebug()?.Message("{0} received ping request from repository {1}", this.ToString(), GeneratedCode.Manual.Repositories.CallbackRepositoryHolder.CurrentCallbackRepositoryId).Write();
            return Task.FromResult(true);
        }

        public async Task PingRepositoryImpl(Guid repositoryId)
        {
            Logger.IfDebug()?.Message("{0} try ping repository {1}", this.ToString(), repositoryId).Write();
            using (var wrapper = await EntitiesRepository.Get<IRepositoryPingEntityServer>(repositoryId))
            {
                var repositoryCommunicationEntity = wrapper.Get<IRepositoryPingEntityServer>(repositoryId);
                if (repositoryCommunicationEntity == null)
                {
                    Logger.IfError()?.Message("Pinged from {0} to repository {1}. entity is null", this.ToString(), repositoryId).Write();
                    return;
                }

                var result = await repositoryCommunicationEntity.Ping();
                if (result)
                    Logger.IfDebug()?.Message("Ping success from {0} to {1}.", this.ToString(), repositoryCommunicationEntity.ToString()).Write();
                else
                    Logger.IfError()?.Message("Ping fail from {0} to {1}.", this.ToString(), repositoryCommunicationEntity.ToString()).Write();
            }
        }
    }
}
