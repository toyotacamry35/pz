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
    public partial class MapHostEntity
    {
        public Task<SharedCode.MapSystem.HostOrInstallMapResult> HostMap(System.Guid id, System.Guid realmId, GeneratedCode.Custom.Config.MapDef map, SharedCode.Aspects.Sessions.RealmRulesDef realmRules)
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return HostMapRunMaster(id, realmId, map, realmRules).AsTask();
                else
                    return HostMapRun(id, realmId, map, realmRules).AsTask();
            else
                return SendFuncs.HostMap(id, realmId, map, realmRules, this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId()).AsTask();
        }

        private Func<Task<SharedCode.MapSystem.HostOrInstallMapResult>> HostMapCreateDeferredDelegate(System.Guid id, System.Guid realmId, GeneratedCode.Custom.Config.MapDef map, SharedCode.Aspects.Sessions.RealmRulesDef realmRules) => () =>
        {
            return HostMap(id, realmId, map, realmRules);
        }

        ;
        public async ValueTask<SharedCode.MapSystem.HostOrInstallMapResult> HostMapRunMaster(System.Guid id, System.Guid realmId, GeneratedCode.Custom.Config.MapDef map, SharedCode.Aspects.Sessions.RealmRulesDef realmRules)
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    return await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(HostMapCreateDeferredDelegate(id, realmId, map, realmRules), nameof(HostMap));
            }

            return await HostMapRun(id, realmId, map, realmRules);
        }

        public async ValueTask<SharedCode.MapSystem.HostOrInstallMapResult> HostMapRun(System.Guid id, System.Guid realmId, GeneratedCode.Custom.Config.MapDef map, SharedCode.Aspects.Sessions.RealmRulesDef realmRules)
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(SharedCode.MapSystem.IMapHostEntity), 0);
            var getTask = this.GetThisExclusive();
            var wrapper = getTask.IsCompleted ? getTask.Result : await getTask;
            using (wrapper)
            {
                if (wrapper == null && EntitiesRepository != null)
                    throw new Exception($"{nameof(HostMap)} GetExclusive wrapper is null");
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
                    var returnTask = HostMapImpl(id, realmId, map, realmRules);
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

        public Task<bool> LogoutUserFromMap(System.Guid userId, System.Guid map, bool terminal)
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return LogoutUserFromMapRunMaster(userId, map, terminal).AsTask();
                else
                    return LogoutUserFromMapRun(userId, map, terminal).AsTask();
            else
                return SendFuncs.LogoutUserFromMap(userId, map, terminal, this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId()).AsTask();
        }

        private Func<Task<bool>> LogoutUserFromMapCreateDeferredDelegate(System.Guid userId, System.Guid map, bool terminal) => () =>
        {
            return LogoutUserFromMap(userId, map, terminal);
        }

        ;
        public async ValueTask<bool> LogoutUserFromMapRunMaster(System.Guid userId, System.Guid map, bool terminal)
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    return await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(LogoutUserFromMapCreateDeferredDelegate(userId, map, terminal), nameof(LogoutUserFromMap));
            }

            return await LogoutUserFromMapRun(userId, map, terminal);
        }

        public async ValueTask<bool> LogoutUserFromMapRun(System.Guid userId, System.Guid map, bool terminal)
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(SharedCode.MapSystem.IMapHostEntity), 1);
            var getTask = this.GetThisExclusive();
            var wrapper = getTask.IsCompleted ? getTask.Result : await getTask;
            using (wrapper)
            {
                if (wrapper == null && EntitiesRepository != null)
                    throw new Exception($"{nameof(LogoutUserFromMap)} GetExclusive wrapper is null");
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
                    var returnTask = LogoutUserFromMapImpl(userId, map, terminal);
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

        public ValueTask<bool> AddUsersDirect(System.Collections.Generic.List<System.Guid> users)
        {
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return AddUsersDirectRunMaster(users);
                else
                    return AddUsersDirectRun(users);
            else
                return SendFuncs.AddUsersDirect(users, this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId());
        }

        private Func<Task<bool>> AddUsersDirectCreateDeferredDelegate(System.Collections.Generic.List<System.Guid> users) => () =>
        {
            return AddUsersDirect(users).AsTask();
        }

        ;
        public async ValueTask<bool> AddUsersDirectRunMaster(System.Collections.Generic.List<System.Guid> users)
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    return await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(AddUsersDirectCreateDeferredDelegate(users), nameof(AddUsersDirect));
            }

            return await AddUsersDirectRun(users);
        }

        public async ValueTask<bool> AddUsersDirectRun(System.Collections.Generic.List<System.Guid> users)
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(SharedCode.MapSystem.IMapHostEntity), 2);
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
                var returnTask = AddUsersDirectImpl(users);
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