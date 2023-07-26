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
    public partial class WorldNodeServiceEntity
    {
        public Task<bool> IsReady()
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return IsReadyRunMaster().AsTask();
                else
                    return IsReadyRun().AsTask();
            else
                return SendFuncs.IsReady(this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId()).AsTask();
        }

        private Func<Task<bool>> IsReadyCreateDeferredDelegate() => () =>
        {
            return IsReady();
        }

        ;
        public async ValueTask<bool> IsReadyRunMaster()
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    return await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(IsReadyCreateDeferredDelegate(), nameof(IsReady));
            }

            return await IsReadyRun();
        }

        public async ValueTask<bool> IsReadyRun()
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(SharedCode.Entities.Service.IWorldNodeServiceEntity), 0);
            var getTask = this.GetThisExclusive();
            var wrapper = getTask.IsCompleted ? getTask.Result : await getTask;
            using (wrapper)
            {
                if (wrapper == null && EntitiesRepository != null)
                    throw new Exception($"{nameof(IsReady)} GetExclusive wrapper is null");
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
                    var returnTask = IsReadyImpl();
                    if (!returnTask.IsCompleted || returnTask.IsFaulted)
                    {
                        var __needDecrement__ = IncrementExecutedMethodsCounter(out var __parentEntity__);
                        try
                        {
                            return await returnTask;
                        }
                        finally
                        {
                            ((IEntityExt)__parentEntity__)?.DecrementExecutedMethodsCounter(__needDecrement__);
                        }
                    }
                    else
                        return returnTask.Result;
                }
                finally
                {
                    if (__needSetMigrationgId__)
                        GeneratedCode.Manual.Repositories.MigrationIdHolder.SetCurrentMigrationId(ref __oldMigrationId__);
                }
            }
        }

        public Task<bool> HostUnityMapChunk(GeneratedCode.Custom.Config.MapDef mapChunk)
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return HostUnityMapChunkRunMaster(mapChunk).AsTask();
                else
                    return HostUnityMapChunkRun(mapChunk).AsTask();
            else
                return SendFuncs.HostUnityMapChunk(mapChunk, this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId()).AsTask();
        }

        private Func<Task<bool>> HostUnityMapChunkCreateDeferredDelegate(GeneratedCode.Custom.Config.MapDef mapChunk) => () =>
        {
            return HostUnityMapChunk(mapChunk);
        }

        ;
        public async ValueTask<bool> HostUnityMapChunkRunMaster(GeneratedCode.Custom.Config.MapDef mapChunk)
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    return await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(HostUnityMapChunkCreateDeferredDelegate(mapChunk), nameof(HostUnityMapChunk));
            }

            return await HostUnityMapChunkRun(mapChunk);
        }

        public async ValueTask<bool> HostUnityMapChunkRun(GeneratedCode.Custom.Config.MapDef mapChunk)
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(SharedCode.Entities.Service.IWorldNodeServiceEntity), 1);
            var getTask = this.GetThisExclusive();
            var wrapper = getTask.IsCompleted ? getTask.Result : await getTask;
            using (wrapper)
            {
                if (wrapper == null && EntitiesRepository != null)
                    throw new Exception($"{nameof(HostUnityMapChunk)} GetExclusive wrapper is null");
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
                    var returnTask = HostUnityMapChunkImpl(mapChunk);
                    if (!returnTask.IsCompleted || returnTask.IsFaulted)
                    {
                        var __needDecrement__ = IncrementExecutedMethodsCounter(out var __parentEntity__);
                        try
                        {
                            return await returnTask;
                        }
                        finally
                        {
                            ((IEntityExt)__parentEntity__)?.DecrementExecutedMethodsCounter(__needDecrement__);
                        }
                    }
                    else
                        return returnTask.Result;
                }
                finally
                {
                    if (__needSetMigrationgId__)
                        GeneratedCode.Manual.Repositories.MigrationIdHolder.SetCurrentMigrationId(ref __oldMigrationId__);
                }
            }
        }

        public Task<bool> HostUnityMapChunk(GeneratedCode.Custom.Config.MapDef mapChunk, System.Guid mapChunkId, System.Guid mapInstanceId, System.Guid mapInstanceRepositoryId)
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return HostUnityMapChunkRunMaster(mapChunk, mapChunkId, mapInstanceId, mapInstanceRepositoryId).AsTask();
                else
                    return HostUnityMapChunkRun(mapChunk, mapChunkId, mapInstanceId, mapInstanceRepositoryId).AsTask();
            else
                return SendFuncs.HostUnityMapChunk(mapChunk, mapChunkId, mapInstanceId, mapInstanceRepositoryId, this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId()).AsTask();
        }

        private Func<Task<bool>> HostUnityMapChunkCreateDeferredDelegate(GeneratedCode.Custom.Config.MapDef mapChunk, System.Guid mapChunkId, System.Guid mapInstanceId, System.Guid mapInstanceRepositoryId) => () =>
        {
            return HostUnityMapChunk(mapChunk, mapChunkId, mapInstanceId, mapInstanceRepositoryId);
        }

        ;
        public async ValueTask<bool> HostUnityMapChunkRunMaster(GeneratedCode.Custom.Config.MapDef mapChunk, System.Guid mapChunkId, System.Guid mapInstanceId, System.Guid mapInstanceRepositoryId)
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    return await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(HostUnityMapChunkCreateDeferredDelegate(mapChunk, mapChunkId, mapInstanceId, mapInstanceRepositoryId), nameof(HostUnityMapChunk));
            }

            return await HostUnityMapChunkRun(mapChunk, mapChunkId, mapInstanceId, mapInstanceRepositoryId);
        }

        public async ValueTask<bool> HostUnityMapChunkRun(GeneratedCode.Custom.Config.MapDef mapChunk, System.Guid mapChunkId, System.Guid mapInstanceId, System.Guid mapInstanceRepositoryId)
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(SharedCode.Entities.Service.IWorldNodeServiceEntity), 2);
            var getTask = this.GetThisExclusive();
            var wrapper = getTask.IsCompleted ? getTask.Result : await getTask;
            using (wrapper)
            {
                if (wrapper == null && EntitiesRepository != null)
                    throw new Exception($"{nameof(HostUnityMapChunk)} GetExclusive wrapper is null");
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
                    var returnTask = HostUnityMapChunkImpl(mapChunk, mapChunkId, mapInstanceId, mapInstanceRepositoryId);
                    if (!returnTask.IsCompleted || returnTask.IsFaulted)
                    {
                        var __needDecrement__ = IncrementExecutedMethodsCounter(out var __parentEntity__);
                        try
                        {
                            return await returnTask;
                        }
                        finally
                        {
                            ((IEntityExt)__parentEntity__)?.DecrementExecutedMethodsCounter(__needDecrement__);
                        }
                    }
                    else
                        return returnTask.Result;
                }
                finally
                {
                    if (__needSetMigrationgId__)
                        GeneratedCode.Manual.Repositories.MigrationIdHolder.SetCurrentMigrationId(ref __oldMigrationId__);
                }
            }
        }

        public Task<bool> HostedUnityMapChunk(GeneratedCode.Custom.Config.MapDef mapChunk, System.Guid mapChunkId, System.Guid mapInstanceId)
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return HostedUnityMapChunkRunMaster(mapChunk, mapChunkId, mapInstanceId).AsTask();
                else
                    return HostedUnityMapChunkRun(mapChunk, mapChunkId, mapInstanceId).AsTask();
            else
                return SendFuncs.HostedUnityMapChunk(mapChunk, mapChunkId, mapInstanceId, this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId()).AsTask();
        }

        private Func<Task<bool>> HostedUnityMapChunkCreateDeferredDelegate(GeneratedCode.Custom.Config.MapDef mapChunk, System.Guid mapChunkId, System.Guid mapInstanceId) => () =>
        {
            return HostedUnityMapChunk(mapChunk, mapChunkId, mapInstanceId);
        }

        ;
        public async ValueTask<bool> HostedUnityMapChunkRunMaster(GeneratedCode.Custom.Config.MapDef mapChunk, System.Guid mapChunkId, System.Guid mapInstanceId)
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    return await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(HostedUnityMapChunkCreateDeferredDelegate(mapChunk, mapChunkId, mapInstanceId), nameof(HostedUnityMapChunk));
            }

            return await HostedUnityMapChunkRun(mapChunk, mapChunkId, mapInstanceId);
        }

        public async ValueTask<bool> HostedUnityMapChunkRun(GeneratedCode.Custom.Config.MapDef mapChunk, System.Guid mapChunkId, System.Guid mapInstanceId)
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(SharedCode.Entities.Service.IWorldNodeServiceEntity), 3);
            var getTask = this.GetThisExclusive();
            var wrapper = getTask.IsCompleted ? getTask.Result : await getTask;
            using (wrapper)
            {
                if (wrapper == null && EntitiesRepository != null)
                    throw new Exception($"{nameof(HostedUnityMapChunk)} GetExclusive wrapper is null");
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
                    var returnTask = HostedUnityMapChunkImpl(mapChunk, mapChunkId, mapInstanceId);
                    if (!returnTask.IsCompleted || returnTask.IsFaulted)
                    {
                        var __needDecrement__ = IncrementExecutedMethodsCounter(out var __parentEntity__);
                        try
                        {
                            return await returnTask;
                        }
                        finally
                        {
                            ((IEntityExt)__parentEntity__)?.DecrementExecutedMethodsCounter(__needDecrement__);
                        }
                    }
                    else
                        return returnTask.Result;
                }
                finally
                {
                    if (__needSetMigrationgId__)
                        GeneratedCode.Manual.Repositories.MigrationIdHolder.SetCurrentMigrationId(ref __oldMigrationId__);
                }
            }
        }

        public Task<bool> SetState(SharedCode.Entities.Service.WorldNodeServiceState state)
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return SetStateRunMaster(state).AsTask();
                else
                    return SetStateRun(state).AsTask();
            else
                return SendFuncs.SetState(state, this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId()).AsTask();
        }

        private Func<Task<bool>> SetStateCreateDeferredDelegate(SharedCode.Entities.Service.WorldNodeServiceState state) => () =>
        {
            return SetState(state);
        }

        ;
        public async ValueTask<bool> SetStateRunMaster(SharedCode.Entities.Service.WorldNodeServiceState state)
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    return await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(SetStateCreateDeferredDelegate(state), nameof(SetState));
            }

            return await SetStateRun(state);
        }

        public async ValueTask<bool> SetStateRun(SharedCode.Entities.Service.WorldNodeServiceState state)
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(SharedCode.Entities.Service.IWorldNodeServiceEntity), 4);
            var getTask = this.GetThisExclusive();
            var wrapper = getTask.IsCompleted ? getTask.Result : await getTask;
            using (wrapper)
            {
                if (wrapper == null && EntitiesRepository != null)
                    throw new Exception($"{nameof(SetState)} GetExclusive wrapper is null");
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
                    var returnTask = SetStateImpl(state);
                    if (!returnTask.IsCompleted || returnTask.IsFaulted)
                    {
                        var __needDecrement__ = IncrementExecutedMethodsCounter(out var __parentEntity__);
                        try
                        {
                            return await returnTask;
                        }
                        finally
                        {
                            ((IEntityExt)__parentEntity__)?.DecrementExecutedMethodsCounter(__needDecrement__);
                        }
                    }
                    else
                        return returnTask.Result;
                }
                finally
                {
                    if (__needSetMigrationgId__)
                        GeneratedCode.Manual.Repositories.MigrationIdHolder.SetCurrentMigrationId(ref __oldMigrationId__);
                }
            }
        }

        public Task<bool> InitializePorts()
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return InitializePortsRunMaster().AsTask();
                else
                    return InitializePortsRun().AsTask();
            else
                return SendFuncs.InitializePorts(this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId()).AsTask();
        }

        private Func<Task<bool>> InitializePortsCreateDeferredDelegate() => () =>
        {
            return InitializePorts();
        }

        ;
        public async ValueTask<bool> InitializePortsRunMaster()
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    return await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(InitializePortsCreateDeferredDelegate(), nameof(InitializePorts));
            }

            return await InitializePortsRun();
        }

        public async ValueTask<bool> InitializePortsRun()
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(SharedCode.Entities.Service.IWorldNodeServiceEntity), 5);
            var getTask = this.GetThisExclusive();
            var wrapper = getTask.IsCompleted ? getTask.Result : await getTask;
            using (wrapper)
            {
                if (wrapper == null && EntitiesRepository != null)
                    throw new Exception($"{nameof(InitializePorts)} GetExclusive wrapper is null");
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
                    var returnTask = InitializePortsImpl();
                    if (!returnTask.IsCompleted || returnTask.IsFaulted)
                    {
                        var __needDecrement__ = IncrementExecutedMethodsCounter(out var __parentEntity__);
                        try
                        {
                            return await returnTask;
                        }
                        finally
                        {
                            ((IEntityExt)__parentEntity__)?.DecrementExecutedMethodsCounter(__needDecrement__);
                        }
                    }
                    else
                        return returnTask.Result;
                }
                finally
                {
                    if (__needSetMigrationgId__)
                        GeneratedCode.Manual.Repositories.MigrationIdHolder.SetCurrentMigrationId(ref __oldMigrationId__);
                }
            }
        }

        public ValueTask<bool> CanBuildHere(SharedCode.Entities.GameObjectEntities.IEntityObjectDef entityObjectDef, SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> ent, SharedCode.Utils.Vector3 position, SharedCode.Utils.Vector3 scale, SharedCode.Utils.Quaternion rotation)
        {
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return CanBuildHereRunMaster(entityObjectDef, ent, position, scale, rotation);
                else
                    return CanBuildHereRun(entityObjectDef, ent, position, scale, rotation);
            else
                return SendFuncs.CanBuildHere(entityObjectDef, ent, position, scale, rotation, this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId());
        }

        private Func<Task<bool>> CanBuildHereCreateDeferredDelegate(SharedCode.Entities.GameObjectEntities.IEntityObjectDef entityObjectDef, SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> ent, SharedCode.Utils.Vector3 position, SharedCode.Utils.Vector3 scale, SharedCode.Utils.Quaternion rotation) => () =>
        {
            return CanBuildHere(entityObjectDef, ent, position, scale, rotation).AsTask();
        }

        ;
        public async ValueTask<bool> CanBuildHereRunMaster(SharedCode.Entities.GameObjectEntities.IEntityObjectDef entityObjectDef, SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> ent, SharedCode.Utils.Vector3 position, SharedCode.Utils.Vector3 scale, SharedCode.Utils.Quaternion rotation)
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    return await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(CanBuildHereCreateDeferredDelegate(entityObjectDef, ent, position, scale, rotation), nameof(CanBuildHere));
            }

            return await CanBuildHereRun(entityObjectDef, ent, position, scale, rotation);
        }

        public async ValueTask<bool> CanBuildHereRun(SharedCode.Entities.GameObjectEntities.IEntityObjectDef entityObjectDef, SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> ent, SharedCode.Utils.Vector3 position, SharedCode.Utils.Vector3 scale, SharedCode.Utils.Quaternion rotation)
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(SharedCode.Entities.Service.IWorldNodeServiceEntity), 6);
            global::GeneratedCode.Manual.Repositories.RpcCurrentObject.ThisObj = this;
            Guid __oldMigrationId__ = default;
            var __needSetMigrationgId__ = MigratingId != Guid.Empty;
            if (__needSetMigrationgId__)
            {
                __oldMigrationId__ = global::GeneratedCode.Manual.Repositories.MigrationIdHolder.CurrentMigrationId;
                global::GeneratedCode.Manual.Repositories.MigrationIdHolder.SetCurrentMigrationId(ref MigratingId);
            }

            try
            {
                var returnTask = CanBuildHereImpl(entityObjectDef, ent, position, scale, rotation);
                if (!returnTask.IsCompleted || returnTask.IsFaulted)
                {
                    var __needDecrement__ = IncrementExecutedMethodsCounter(out var __parentEntity__);
                    try
                    {
                        return await returnTask;
                    }
                    finally
                    {
                        ((IEntityExt)__parentEntity__)?.DecrementExecutedMethodsCounter(__needDecrement__);
                    }
                }
                else
                    return returnTask.Result;
            }
            finally
            {
                if (__needSetMigrationgId__)
                    global::GeneratedCode.Manual.Repositories.MigrationIdHolder.SetCurrentMigrationId(ref __oldMigrationId__);
            }
        }

        public ValueTask<SharedCode.Utils.Vector3> GetDropPosition(SharedCode.Utils.Vector3 playerPosition, SharedCode.Utils.Quaternion playerRotation)
        {
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return GetDropPositionRunMaster(playerPosition, playerRotation);
                else
                    return GetDropPositionRun(playerPosition, playerRotation);
            else
                return SendFuncs.GetDropPosition(playerPosition, playerRotation, this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId());
        }

        private Func<Task<SharedCode.Utils.Vector3>> GetDropPositionCreateDeferredDelegate(SharedCode.Utils.Vector3 playerPosition, SharedCode.Utils.Quaternion playerRotation) => () =>
        {
            return GetDropPosition(playerPosition, playerRotation).AsTask();
        }

        ;
        public async ValueTask<SharedCode.Utils.Vector3> GetDropPositionRunMaster(SharedCode.Utils.Vector3 playerPosition, SharedCode.Utils.Quaternion playerRotation)
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    return await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(GetDropPositionCreateDeferredDelegate(playerPosition, playerRotation), nameof(GetDropPosition));
            }

            return await GetDropPositionRun(playerPosition, playerRotation);
        }

        public async ValueTask<SharedCode.Utils.Vector3> GetDropPositionRun(SharedCode.Utils.Vector3 playerPosition, SharedCode.Utils.Quaternion playerRotation)
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(SharedCode.Entities.Service.IWorldNodeServiceEntity), 7);
            global::GeneratedCode.Manual.Repositories.RpcCurrentObject.ThisObj = this;
            Guid __oldMigrationId__ = default;
            var __needSetMigrationgId__ = MigratingId != Guid.Empty;
            if (__needSetMigrationgId__)
            {
                __oldMigrationId__ = global::GeneratedCode.Manual.Repositories.MigrationIdHolder.CurrentMigrationId;
                global::GeneratedCode.Manual.Repositories.MigrationIdHolder.SetCurrentMigrationId(ref MigratingId);
            }

            try
            {
                var returnTask = GetDropPositionImpl(playerPosition, playerRotation);
                if (!returnTask.IsCompleted || returnTask.IsFaulted)
                {
                    var __needDecrement__ = IncrementExecutedMethodsCounter(out var __parentEntity__);
                    try
                    {
                        return await returnTask;
                    }
                    finally
                    {
                        ((IEntityExt)__parentEntity__)?.DecrementExecutedMethodsCounter(__needDecrement__);
                    }
                }
                else
                    return returnTask.Result;
            }
            finally
            {
                if (__needSetMigrationgId__)
                    global::GeneratedCode.Manual.Repositories.MigrationIdHolder.SetCurrentMigrationId(ref __oldMigrationId__);
            }
        }
    }
}