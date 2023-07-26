// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

using System;
using System.Threading;
using System.Threading.Tasks;
using GeneratedCode.EntitySystem;
using SharedCode.Logging;
using System.Linq;
using SharedCode.EntitySystem;
using SharedCode.Network;

namespace GeneratedCode.DeltaObjects
{
    public partial class CorpseSpawner
    {
        public Task SpawnCorpse(System.Guid entityId, int entityTypeId, SharedCode.Entities.GameObjectEntities.PositionRotation corpsePlace)
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return SpawnCorpseRunMaster(entityId, entityTypeId, corpsePlace).AsTask();
                else
                    return SpawnCorpseRun(entityId, entityTypeId, corpsePlace).AsTask();
            else
                return SendFuncs.SpawnCorpse(entityId, entityTypeId, corpsePlace, this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId()).AsTask();
        }

        private Func<Task> SpawnCorpseCreateDeferredDelegate(System.Guid entityId, int entityTypeId, SharedCode.Entities.GameObjectEntities.PositionRotation corpsePlace) => () =>
        {
            return SpawnCorpse(entityId, entityTypeId, corpsePlace);
        }

        ;
        public async ValueTask SpawnCorpseRunMaster(System.Guid entityId, int entityTypeId, SharedCode.Entities.GameObjectEntities.PositionRotation corpsePlace)
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(SpawnCorpseCreateDeferredDelegate(entityId, entityTypeId, corpsePlace), nameof(SpawnCorpse));
            }

            await SpawnCorpseRun(entityId, entityTypeId, corpsePlace);
        }

        public async ValueTask SpawnCorpseRun(System.Guid entityId, int entityTypeId, SharedCode.Entities.GameObjectEntities.PositionRotation corpsePlace)
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(Assets.ColonyShared.SharedCode.Entities.ICorpseSpawner), 0);
            var getTask = this.GetThisExclusive();
            var wrapper = getTask.IsCompleted ? getTask.Result : await getTask;
            using (wrapper)
            {
                if (wrapper == null && EntitiesRepository != null)
                    throw new Exception($"{nameof(SpawnCorpse)} GetExclusive wrapper is null");
                GeneratedCode.Manual.Repositories.RpcCurrentObject.ThisObj = this;
                Guid __oldMigrationId__ = default;
                var __needSetMigrationgId__ = MigratingId != Guid.Empty;
                if (__needSetMigrationgId__)
                {
                    __oldMigrationId__ = GeneratedCode.Manual.Repositories.MigrationIdHolder.CurrentMigrationId;
                    GeneratedCode.Manual.Repositories.MigrationIdHolder.SetCurrentMigrationId(ref MigratingId);
                }

                try
                {
                    var returnTask = SpawnCorpseImpl(entityId, entityTypeId, corpsePlace);
                    if (!returnTask.IsCompleted || returnTask.IsFaulted)
                    {
                        var __needDecrement__ = IncrementExecutedMethodsCounter(out var __parentEntity__);
                        try
                        {
                            await returnTask;
                        }
                        finally
                        {
                            ((IEntityExt)__parentEntity__)?.DecrementExecutedMethodsCounter(__needDecrement__);
                        }
                    }
                }
                finally
                {
                    if (__needSetMigrationgId__)
                        GeneratedCode.Manual.Repositories.MigrationIdHolder.SetCurrentMigrationId(ref __oldMigrationId__);
                }
            }
        }
    }
}