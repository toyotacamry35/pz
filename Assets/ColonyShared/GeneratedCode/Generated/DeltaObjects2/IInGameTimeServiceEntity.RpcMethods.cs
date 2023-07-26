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
    public partial class InGameTimeServiceEntity
    {
        public Task<System.DateTime> GetCurrentIngameTime()
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.NeedDeferredRpcOnMigrating())
                return GetCurrentIngameTimeRunMaster().AsTask();
            else
                return GetCurrentIngameTimeRun().AsTask();
        }

        private Func<Task<System.DateTime>> GetCurrentIngameTimeCreateDeferredDelegate() => () =>
        {
            return GetCurrentIngameTime();
        }

        ;
        public async ValueTask<System.DateTime> GetCurrentIngameTimeRunMaster()
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    return await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(GetCurrentIngameTimeCreateDeferredDelegate(), nameof(GetCurrentIngameTime));
            }

            return await GetCurrentIngameTimeRun();
        }

        public async Task<System.DateTime> GetCurrentIngameTimeRun()
        {
            this.CheckValidateEntityInAsyncContext();
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(Assets.ColonyShared.SharedCode.Entities.Service.IInGameTimeServiceEntity), 0);
            Guid __oldMigrationId__ = default;
            var __needSetMigrationgId__ = MigratingId != Guid.Empty;
            if (__needSetMigrationgId__)
            {
                __oldMigrationId__ = global::GeneratedCode.Manual.Repositories.MigrationIdHolder.CurrentMigrationId;
                global::GeneratedCode.Manual.Repositories.MigrationIdHolder.SetCurrentMigrationId(ref MigratingId);
            }

            try
            {
                var __needDecrement__ = IncrementExecutedMethodsCounter(out var __parentEntity__);
                try
                {
                    return await GetCurrentIngameTimeImpl();
                }
                finally
                {
                    ((IEntityExt)__parentEntity__)?.DecrementExecutedMethodsCounter(__needDecrement__);
                }
            }
            finally
            {
                if (__needSetMigrationgId__)
                    global::GeneratedCode.Manual.Repositories.MigrationIdHolder.SetCurrentMigrationId(ref __oldMigrationId__);
            }
        }

        public Task<bool> SetCurrentIngameTime(System.DateTime time)
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return SetCurrentIngameTimeRunMaster(time).AsTask();
                else
                    return SetCurrentIngameTimeRun(time).AsTask();
            else
                return SendFuncs.SetCurrentIngameTime(time, this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId()).AsTask();
        }

        private Func<Task<bool>> SetCurrentIngameTimeCreateDeferredDelegate(System.DateTime time) => () =>
        {
            return SetCurrentIngameTime(time);
        }

        ;
        public async ValueTask<bool> SetCurrentIngameTimeRunMaster(System.DateTime time)
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    return await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(SetCurrentIngameTimeCreateDeferredDelegate(time), nameof(SetCurrentIngameTime));
            }

            return await SetCurrentIngameTimeRun(time);
        }

        public async ValueTask<bool> SetCurrentIngameTimeRun(System.DateTime time)
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(Assets.ColonyShared.SharedCode.Entities.Service.IInGameTimeServiceEntity), 1);
            var getTask = this.GetThisExclusive();
            var wrapper = getTask.IsCompleted ? getTask.Result : await getTask;
            using (wrapper)
            {
                if (wrapper == null && EntitiesRepository != null)
                    throw new Exception($"{nameof(SetCurrentIngameTime)} GetExclusive wrapper is null");
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
                    var returnTask = SetCurrentIngameTimeImpl(time);
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

        public Task<bool> SetTimeFromRealm(System.DateTime serverStartTime)
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return SetTimeFromRealmRunMaster(serverStartTime).AsTask();
                else
                    return SetTimeFromRealmRun(serverStartTime).AsTask();
            else
                return SendFuncs.SetTimeFromRealm(serverStartTime, this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId()).AsTask();
        }

        private Func<Task<bool>> SetTimeFromRealmCreateDeferredDelegate(System.DateTime serverStartTime) => () =>
        {
            return SetTimeFromRealm(serverStartTime);
        }

        ;
        public async ValueTask<bool> SetTimeFromRealmRunMaster(System.DateTime serverStartTime)
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    return await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(SetTimeFromRealmCreateDeferredDelegate(serverStartTime), nameof(SetTimeFromRealm));
            }

            return await SetTimeFromRealmRun(serverStartTime);
        }

        public async ValueTask<bool> SetTimeFromRealmRun(System.DateTime serverStartTime)
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(Assets.ColonyShared.SharedCode.Entities.Service.IInGameTimeServiceEntity), 2);
            var getTask = this.GetThisExclusive();
            var wrapper = getTask.IsCompleted ? getTask.Result : await getTask;
            using (wrapper)
            {
                if (wrapper == null && EntitiesRepository != null)
                    throw new Exception($"{nameof(SetTimeFromRealm)} GetExclusive wrapper is null");
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
                    var returnTask = SetTimeFromRealmImpl(serverStartTime);
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