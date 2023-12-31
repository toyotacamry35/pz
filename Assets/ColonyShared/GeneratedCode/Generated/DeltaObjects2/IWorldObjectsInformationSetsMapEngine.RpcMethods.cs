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
    public partial class WorldObjectsInformationSetsMapEngine
    {
        public Task<System.Collections.Generic.Dictionary<Entities.GameMapData.WorldObjectInformationSetDef, ResourceSystem.Utils.OuterRef>> Subscribe(System.Collections.Generic.List<Entities.GameMapData.WorldObjectInformationSetDef> worldObjectSetsDef, System.Guid repositoryId)
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return SubscribeRunMaster(worldObjectSetsDef, repositoryId).AsTask();
                else
                    return SubscribeRun(worldObjectSetsDef, repositoryId).AsTask();
            else
                return SendFuncs.Subscribe(worldObjectSetsDef, repositoryId, this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId()).AsTask();
        }

        private Func<Task<System.Collections.Generic.Dictionary<Entities.GameMapData.WorldObjectInformationSetDef, ResourceSystem.Utils.OuterRef>>> SubscribeCreateDeferredDelegate(System.Collections.Generic.List<Entities.GameMapData.WorldObjectInformationSetDef> worldObjectSetsDef, System.Guid repositoryId) => () =>
        {
            return Subscribe(worldObjectSetsDef, repositoryId);
        }

        ;
        public async ValueTask<System.Collections.Generic.Dictionary<Entities.GameMapData.WorldObjectInformationSetDef, ResourceSystem.Utils.OuterRef>> SubscribeRunMaster(System.Collections.Generic.List<Entities.GameMapData.WorldObjectInformationSetDef> worldObjectSetsDef, System.Guid repositoryId)
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    return await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(SubscribeCreateDeferredDelegate(worldObjectSetsDef, repositoryId), nameof(Subscribe));
            }

            return await SubscribeRun(worldObjectSetsDef, repositoryId);
        }

        public async ValueTask<System.Collections.Generic.Dictionary<Entities.GameMapData.WorldObjectInformationSetDef, ResourceSystem.Utils.OuterRef>> SubscribeRun(System.Collections.Generic.List<Entities.GameMapData.WorldObjectInformationSetDef> worldObjectSetsDef, System.Guid repositoryId)
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(SharedCode.Entities.IWorldObjectsInformationSetsMapEngine), 0);
            var getTask = this.GetThisExclusive();
            var wrapper = getTask.IsCompleted ? getTask.Result : await getTask;
            using (wrapper)
            {
                if (wrapper == null && EntitiesRepository != null)
                    throw new Exception($"{nameof(Subscribe)} GetExclusive wrapper is null");
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
                    var returnTask = SubscribeImpl(worldObjectSetsDef, repositoryId);
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

        public Task<bool> Unsubscribe(System.Collections.Generic.List<Entities.GameMapData.WorldObjectInformationSetDef> worldObjectSetsDef, System.Guid repositoryId)
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return UnsubscribeRunMaster(worldObjectSetsDef, repositoryId).AsTask();
                else
                    return UnsubscribeRun(worldObjectSetsDef, repositoryId).AsTask();
            else
                return SendFuncs.Unsubscribe(worldObjectSetsDef, repositoryId, this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId()).AsTask();
        }

        private Func<Task<bool>> UnsubscribeCreateDeferredDelegate(System.Collections.Generic.List<Entities.GameMapData.WorldObjectInformationSetDef> worldObjectSetsDef, System.Guid repositoryId) => () =>
        {
            return Unsubscribe(worldObjectSetsDef, repositoryId);
        }

        ;
        public async ValueTask<bool> UnsubscribeRunMaster(System.Collections.Generic.List<Entities.GameMapData.WorldObjectInformationSetDef> worldObjectSetsDef, System.Guid repositoryId)
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    return await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(UnsubscribeCreateDeferredDelegate(worldObjectSetsDef, repositoryId), nameof(Unsubscribe));
            }

            return await UnsubscribeRun(worldObjectSetsDef, repositoryId);
        }

        public async ValueTask<bool> UnsubscribeRun(System.Collections.Generic.List<Entities.GameMapData.WorldObjectInformationSetDef> worldObjectSetsDef, System.Guid repositoryId)
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(SharedCode.Entities.IWorldObjectsInformationSetsMapEngine), 1);
            var getTask = this.GetThisExclusive();
            var wrapper = getTask.IsCompleted ? getTask.Result : await getTask;
            using (wrapper)
            {
                if (wrapper == null && EntitiesRepository != null)
                    throw new Exception($"{nameof(Unsubscribe)} GetExclusive wrapper is null");
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
                    var returnTask = UnsubscribeImpl(worldObjectSetsDef, repositoryId);
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

        public Task<bool> AddWorldObject(ResourceSystem.Utils.OuterRef worldObjectRef)
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return AddWorldObjectRunMaster(worldObjectRef).AsTask();
                else
                    return AddWorldObjectRun(worldObjectRef).AsTask();
            else
                return SendFuncs.AddWorldObject(worldObjectRef, this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId()).AsTask();
        }

        private Func<Task<bool>> AddWorldObjectCreateDeferredDelegate(ResourceSystem.Utils.OuterRef worldObjectRef) => () =>
        {
            return AddWorldObject(worldObjectRef);
        }

        ;
        public async ValueTask<bool> AddWorldObjectRunMaster(ResourceSystem.Utils.OuterRef worldObjectRef)
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    return await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(AddWorldObjectCreateDeferredDelegate(worldObjectRef), nameof(AddWorldObject));
            }

            return await AddWorldObjectRun(worldObjectRef);
        }

        public async ValueTask<bool> AddWorldObjectRun(ResourceSystem.Utils.OuterRef worldObjectRef)
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(SharedCode.Entities.IWorldObjectsInformationSetsMapEngine), 2);
            var getTask = this.GetThisExclusive();
            var wrapper = getTask.IsCompleted ? getTask.Result : await getTask;
            using (wrapper)
            {
                if (wrapper == null && EntitiesRepository != null)
                    throw new Exception($"{nameof(AddWorldObject)} GetExclusive wrapper is null");
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
                    var returnTask = AddWorldObjectImpl(worldObjectRef);
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

        public Task<bool> RemoveWorldObject(ResourceSystem.Utils.OuterRef worldObjectRef)
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return RemoveWorldObjectRunMaster(worldObjectRef).AsTask();
                else
                    return RemoveWorldObjectRun(worldObjectRef).AsTask();
            else
                return SendFuncs.RemoveWorldObject(worldObjectRef, this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId()).AsTask();
        }

        private Func<Task<bool>> RemoveWorldObjectCreateDeferredDelegate(ResourceSystem.Utils.OuterRef worldObjectRef) => () =>
        {
            return RemoveWorldObject(worldObjectRef);
        }

        ;
        public async ValueTask<bool> RemoveWorldObjectRunMaster(ResourceSystem.Utils.OuterRef worldObjectRef)
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    return await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(RemoveWorldObjectCreateDeferredDelegate(worldObjectRef), nameof(RemoveWorldObject));
            }

            return await RemoveWorldObjectRun(worldObjectRef);
        }

        public async ValueTask<bool> RemoveWorldObjectRun(ResourceSystem.Utils.OuterRef worldObjectRef)
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(SharedCode.Entities.IWorldObjectsInformationSetsMapEngine), 3);
            var getTask = this.GetThisExclusive();
            var wrapper = getTask.IsCompleted ? getTask.Result : await getTask;
            using (wrapper)
            {
                if (wrapper == null && EntitiesRepository != null)
                    throw new Exception($"{nameof(RemoveWorldObject)} GetExclusive wrapper is null");
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
                    var returnTask = RemoveWorldObjectImpl(worldObjectRef);
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
    }
}