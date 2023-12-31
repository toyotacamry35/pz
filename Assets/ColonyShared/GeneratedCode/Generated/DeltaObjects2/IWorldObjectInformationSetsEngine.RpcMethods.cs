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
    public partial class WorldObjectInformationSetsEngine
    {
        public Task<SharedCode.Entities.AddWorldObjectInformationSubSetResult> AddWorldObjectInformationSubSet(Entities.GameMapData.WorldObjectInformationClientSubSetDef subSetDef)
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return AddWorldObjectInformationSubSetRunMaster(subSetDef).AsTask();
                else
                    return AddWorldObjectInformationSubSetRun(subSetDef).AsTask();
            else
                return SendFuncs.AddWorldObjectInformationSubSet(subSetDef, this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId()).AsTask();
        }

        private Func<Task<SharedCode.Entities.AddWorldObjectInformationSubSetResult>> AddWorldObjectInformationSubSetCreateDeferredDelegate(Entities.GameMapData.WorldObjectInformationClientSubSetDef subSetDef) => () =>
        {
            return AddWorldObjectInformationSubSet(subSetDef);
        }

        ;
        public async ValueTask<SharedCode.Entities.AddWorldObjectInformationSubSetResult> AddWorldObjectInformationSubSetRunMaster(Entities.GameMapData.WorldObjectInformationClientSubSetDef subSetDef)
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    return await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(AddWorldObjectInformationSubSetCreateDeferredDelegate(subSetDef), nameof(AddWorldObjectInformationSubSet));
            }

            return await AddWorldObjectInformationSubSetRun(subSetDef);
        }

        public async ValueTask<SharedCode.Entities.AddWorldObjectInformationSubSetResult> AddWorldObjectInformationSubSetRun(Entities.GameMapData.WorldObjectInformationClientSubSetDef subSetDef)
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(SharedCode.Entities.IWorldObjectInformationSetsEngine), 0);
            var getTask = this.GetThisExclusive();
            var wrapper = getTask.IsCompleted ? getTask.Result : await getTask;
            using (wrapper)
            {
                if (wrapper == null && EntitiesRepository != null)
                    throw new Exception($"{nameof(AddWorldObjectInformationSubSet)} GetExclusive wrapper is null");
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
                    var returnTask = AddWorldObjectInformationSubSetImpl(subSetDef);
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

        public Task<SharedCode.Entities.RemoveWorldObjectInformationSubSetResult> RemoveWorldObjectInformationSubSet(Entities.GameMapData.WorldObjectInformationClientSubSetDef subSetDef)
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return RemoveWorldObjectInformationSubSetRunMaster(subSetDef).AsTask();
                else
                    return RemoveWorldObjectInformationSubSetRun(subSetDef).AsTask();
            else
                return SendFuncs.RemoveWorldObjectInformationSubSet(subSetDef, this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId()).AsTask();
        }

        private Func<Task<SharedCode.Entities.RemoveWorldObjectInformationSubSetResult>> RemoveWorldObjectInformationSubSetCreateDeferredDelegate(Entities.GameMapData.WorldObjectInformationClientSubSetDef subSetDef) => () =>
        {
            return RemoveWorldObjectInformationSubSet(subSetDef);
        }

        ;
        public async ValueTask<SharedCode.Entities.RemoveWorldObjectInformationSubSetResult> RemoveWorldObjectInformationSubSetRunMaster(Entities.GameMapData.WorldObjectInformationClientSubSetDef subSetDef)
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    return await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(RemoveWorldObjectInformationSubSetCreateDeferredDelegate(subSetDef), nameof(RemoveWorldObjectInformationSubSet));
            }

            return await RemoveWorldObjectInformationSubSetRun(subSetDef);
        }

        public async ValueTask<SharedCode.Entities.RemoveWorldObjectInformationSubSetResult> RemoveWorldObjectInformationSubSetRun(Entities.GameMapData.WorldObjectInformationClientSubSetDef subSetDef)
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(SharedCode.Entities.IWorldObjectInformationSetsEngine), 1);
            var getTask = this.GetThisExclusive();
            var wrapper = getTask.IsCompleted ? getTask.Result : await getTask;
            using (wrapper)
            {
                if (wrapper == null && EntitiesRepository != null)
                    throw new Exception($"{nameof(RemoveWorldObjectInformationSubSet)} GetExclusive wrapper is null");
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
                    var returnTask = RemoveWorldObjectInformationSubSetImpl(subSetDef);
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

        public Task<SharedCode.Entities.AddWorldObjectInformationSubSetResult> AddWorldObjectInformationSubSetCheat(Entities.GameMapData.WorldObjectInformationClientSubSetDef subSetDef)
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return AddWorldObjectInformationSubSetCheatRunMaster(subSetDef).AsTask();
                else
                    return AddWorldObjectInformationSubSetCheatRun(subSetDef).AsTask();
            else
                return SendFuncs.AddWorldObjectInformationSubSetCheat(subSetDef, this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId()).AsTask();
        }

        private Func<Task<SharedCode.Entities.AddWorldObjectInformationSubSetResult>> AddWorldObjectInformationSubSetCheatCreateDeferredDelegate(Entities.GameMapData.WorldObjectInformationClientSubSetDef subSetDef) => () =>
        {
            return AddWorldObjectInformationSubSetCheat(subSetDef);
        }

        ;
        public async ValueTask<SharedCode.Entities.AddWorldObjectInformationSubSetResult> AddWorldObjectInformationSubSetCheatRunMaster(Entities.GameMapData.WorldObjectInformationClientSubSetDef subSetDef)
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    return await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(AddWorldObjectInformationSubSetCheatCreateDeferredDelegate(subSetDef), nameof(AddWorldObjectInformationSubSetCheat));
            }

            return await AddWorldObjectInformationSubSetCheatRun(subSetDef);
        }

        public async ValueTask<SharedCode.Entities.AddWorldObjectInformationSubSetResult> AddWorldObjectInformationSubSetCheatRun(Entities.GameMapData.WorldObjectInformationClientSubSetDef subSetDef)
        {
            if (!await global::GeneratedCode.Shared.Utils.AccountTypeUtils.CheckAccountType(GeneratedCode.Manual.Repositories.CallbackRepositoryHolder.CurrentCallbackRepositoryId, SharedCode.Entities.Service.AccountType.TechnicalSupport, EntitiesRepository))
                throw new System.UnauthorizedAccessException(string.Format("User {0} has no rights to use cheat {1}", GeneratedCode.Manual.Repositories.CallbackRepositoryHolder.CurrentCallbackRepositoryId, nameof(AddWorldObjectInformationSubSetCheat)));
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(SharedCode.Entities.IWorldObjectInformationSetsEngine), 2);
            var getTask = this.GetThisExclusive();
            var wrapper = getTask.IsCompleted ? getTask.Result : await getTask;
            using (wrapper)
            {
                if (wrapper == null && EntitiesRepository != null)
                    throw new Exception($"{nameof(AddWorldObjectInformationSubSetCheat)} GetExclusive wrapper is null");
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
                    var returnTask = AddWorldObjectInformationSubSetCheatImpl(subSetDef);
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

        public Task<SharedCode.Entities.RemoveWorldObjectInformationSubSetResult> RemoveWorldObjectInformationSubSetCheat(Entities.GameMapData.WorldObjectInformationClientSubSetDef subSetDef)
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return RemoveWorldObjectInformationSubSetCheatRunMaster(subSetDef).AsTask();
                else
                    return RemoveWorldObjectInformationSubSetCheatRun(subSetDef).AsTask();
            else
                return SendFuncs.RemoveWorldObjectInformationSubSetCheat(subSetDef, this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId()).AsTask();
        }

        private Func<Task<SharedCode.Entities.RemoveWorldObjectInformationSubSetResult>> RemoveWorldObjectInformationSubSetCheatCreateDeferredDelegate(Entities.GameMapData.WorldObjectInformationClientSubSetDef subSetDef) => () =>
        {
            return RemoveWorldObjectInformationSubSetCheat(subSetDef);
        }

        ;
        public async ValueTask<SharedCode.Entities.RemoveWorldObjectInformationSubSetResult> RemoveWorldObjectInformationSubSetCheatRunMaster(Entities.GameMapData.WorldObjectInformationClientSubSetDef subSetDef)
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    return await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(RemoveWorldObjectInformationSubSetCheatCreateDeferredDelegate(subSetDef), nameof(RemoveWorldObjectInformationSubSetCheat));
            }

            return await RemoveWorldObjectInformationSubSetCheatRun(subSetDef);
        }

        public async ValueTask<SharedCode.Entities.RemoveWorldObjectInformationSubSetResult> RemoveWorldObjectInformationSubSetCheatRun(Entities.GameMapData.WorldObjectInformationClientSubSetDef subSetDef)
        {
            if (!await global::GeneratedCode.Shared.Utils.AccountTypeUtils.CheckAccountType(GeneratedCode.Manual.Repositories.CallbackRepositoryHolder.CurrentCallbackRepositoryId, SharedCode.Entities.Service.AccountType.TechnicalSupport, EntitiesRepository))
                throw new System.UnauthorizedAccessException(string.Format("User {0} has no rights to use cheat {1}", GeneratedCode.Manual.Repositories.CallbackRepositoryHolder.CurrentCallbackRepositoryId, nameof(RemoveWorldObjectInformationSubSetCheat)));
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(SharedCode.Entities.IWorldObjectInformationSetsEngine), 3);
            var getTask = this.GetThisExclusive();
            var wrapper = getTask.IsCompleted ? getTask.Result : await getTask;
            using (wrapper)
            {
                if (wrapper == null && EntitiesRepository != null)
                    throw new Exception($"{nameof(RemoveWorldObjectInformationSubSetCheat)} GetExclusive wrapper is null");
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
                    var returnTask = RemoveWorldObjectInformationSubSetCheatImpl(subSetDef);
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