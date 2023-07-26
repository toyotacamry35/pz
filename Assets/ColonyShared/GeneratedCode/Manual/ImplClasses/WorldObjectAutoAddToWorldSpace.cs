using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedCode.Repositories;
using NLog;
using SharedCode.Entities;
using SharedCode.Entities.Service;
using SharedCode.EntitySystem;
using SharedCode.Serializers;
using SharedCode.Utils;

namespace GeneratedCode.DeltaObjects
{
    public partial class AutoAddToWorldSpace : IHookOnStart, IHookOnDestroy, IHookOnUnload
    {
        private IWorldSpaced _wsEntity => ((IHasWorldSpaced)parentEntity)?.WorldSpaced;

        public async Task OnStart()
        {
            if (!ReplicatedObjectsWhitelist.ShouldReplicateToAnyoneEnMasse(parentEntity))
                return;
            var wsGuid = EntitiesRepository.Id;
            if (_wsEntity.OwnWorldSpace.Guid != Guid.Empty)
                wsGuid = _wsEntity.OwnWorldSpace.Guid;

            var ws = EntitiesRepository.TryGetLockfree<IWorldSpaceServiceEntityServer>(wsGuid, ReplicationLevel.Server);
            if (ws == null)
                return;
            var result = await ws.AddWorldObject(parentEntity.TypeId, parentEntity.Id);
            return;
        }

        public Task OnDestroy()
        {
            if (!ReplicatedObjectsWhitelist.ShouldReplicateToAnyoneEnMasse(parentEntity))
                return Task.CompletedTask;
            var wsGuid = EntitiesRepository.Id;
            if (_wsEntity.OwnWorldSpace.Guid != Guid.Empty)
                wsGuid = _wsEntity.OwnWorldSpace.Guid;
            AsyncUtils.RunAsyncTask(async () =>
            {
                var ws = EntitiesRepository.TryGetLockfree<IWorldSpaceServiceEntityServer>(wsGuid, ReplicationLevel.Server);
                if (ws == null)
                    return;
                var result = await ws.RemoveWorldObject(parentEntity.TypeId, parentEntity.Id);
            }, EntitiesRepository);
            return Task.CompletedTask;
        }

        public Task OnUnload()
        {
            return OnDestroy();
        }
    }
}
