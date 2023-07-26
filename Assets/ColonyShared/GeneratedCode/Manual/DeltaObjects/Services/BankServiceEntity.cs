using Assets.ResourceSystem.Aspects.Banks;
using GeneratedCode.Custom.Config;
using NLog;
using SharedCode.Config;
using SharedCode.EntitySystem;
using System.Threading.Tasks;
using SharedCode.Entities;
using SharedCode.Refs;
using GeneratedCode.Repositories;
using SharedCode.Repositories;

namespace GeneratedCode.DeltaObjects
{
    public partial class BankServiceEntity : IHasLoadFromJObject
    {
        private BankerServiceEntityConfig _config;
        private OuterRef<IEntity> _bankerRef;
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public Task Load(CloudSharedDataConfig sharedConfig, CustomConfig config, IEntitiesRepository entitiesRepository)
        {
            _config = (BankerServiceEntityConfig)config;
            _bankerRef = new OuterRef<IEntity>(_config.BankerGuid, ReplicaTypeRegistry.GetIdByType(typeof(IBankerEntity)));
            return Task.CompletedTask;
        }

        public Task<OuterRef<IEntity>> GetBankerImpl()
        {
            return Task.FromResult(_bankerRef);
        }
    }
}
