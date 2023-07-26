using System;
using System.Threading.Tasks;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedCode.Repositories;
using NLog;
using SharedCode.Cloud;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;

namespace GeneratedCode.DeltaObjects
{
    public partial class EntityObjects : IHookOnInit
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        public Task OnInit()
        {
            if (EntitytObjectsUnitySpawnService.SpawnService != null)
                EntitytObjectsUnitySpawnService.SpawnService.RegisterRepo(this.EntitiesRepository);
            return Task.CompletedTask;
        }
    }
}
