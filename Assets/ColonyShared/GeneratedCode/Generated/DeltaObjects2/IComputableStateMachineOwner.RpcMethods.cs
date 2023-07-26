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
    public partial class ComputableStateMachine
    {
        public ValueTask<Assets.ColonyShared.SharedCode.Aspects.WorldObjects.ComputableStateMachineDef> GetStateMachineDef()
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return GetStateMachineDefRunMaster();
                else
                    return GetStateMachineDefRun();
            else
                return SendFuncs.GetStateMachineDef(this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId());
        }

        private Func<Task<Assets.ColonyShared.SharedCode.Aspects.WorldObjects.ComputableStateMachineDef>> GetStateMachineDefCreateDeferredDelegate() => () =>
        {
            return GetStateMachineDef().AsTask();
        }

        ;
        public async ValueTask<Assets.ColonyShared.SharedCode.Aspects.WorldObjects.ComputableStateMachineDef> GetStateMachineDefRunMaster()
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    return await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(GetStateMachineDefCreateDeferredDelegate(), nameof(GetStateMachineDef));
            }

            return await GetStateMachineDefRun();
        }

        public async ValueTask<Assets.ColonyShared.SharedCode.Aspects.WorldObjects.ComputableStateMachineDef> GetStateMachineDefRun()
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(Assets.ColonyShared.SharedCode.Entities.IComputableStateMachine), 0);
            var getTask = this.GetThisExclusive();
            var wrapper = getTask.IsCompleted ? getTask.Result : await getTask;
            using (wrapper)
            {
                if (wrapper == null && EntitiesRepository != null)
                    throw new Exception($"{nameof(GetStateMachineDef)} GetExclusive wrapper is null");
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
                    var returnTask = GetStateMachineDefImpl();
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

        public Task MarkNotPristine()
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return MarkNotPristineRunMaster().AsTask();
                else
                    return MarkNotPristineRun().AsTask();
            else
                return SendFuncs.MarkNotPristine(this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId()).AsTask();
        }

        private Func<Task> MarkNotPristineCreateDeferredDelegate() => () =>
        {
            return MarkNotPristine();
        }

        ;
        public async ValueTask MarkNotPristineRunMaster()
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(MarkNotPristineCreateDeferredDelegate(), nameof(MarkNotPristine));
            }

            await MarkNotPristineRun();
        }

        public async ValueTask MarkNotPristineRun()
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(Assets.ColonyShared.SharedCode.Entities.IComputableStateMachine), 1);
            var getTask = this.GetThisExclusive();
            var wrapper = getTask.IsCompleted ? getTask.Result : await getTask;
            using (wrapper)
            {
                if (wrapper == null && EntitiesRepository != null)
                    throw new Exception($"{nameof(MarkNotPristine)} GetExclusive wrapper is null");
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
                    var returnTask = MarkNotPristineImpl();
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

        public ValueTask<bool> GetIsPristine()
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.NeedDeferredRpcOnMigrating())
                return GetIsPristineRunMaster();
            else
                return GetIsPristineRun();
        }

        private Func<Task<bool>> GetIsPristineCreateDeferredDelegate() => () =>
        {
            return GetIsPristine().AsTask();
        }

        ;
        public async ValueTask<bool> GetIsPristineRunMaster()
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    return await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(GetIsPristineCreateDeferredDelegate(), nameof(GetIsPristine));
            }

            return await GetIsPristineRun();
        }

        public async ValueTask<bool> GetIsPristineRun()
        {
            this.CheckValidateEntityInAsyncContext();
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(Assets.ColonyShared.SharedCode.Entities.IComputableStateMachine), 2);
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
                    return await GetIsPristineImpl();
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

        public Task<Assets.ColonyShared.SharedCode.Aspects.WorldObjects.ComputableStatesDef> GetCurrentStates()
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return GetCurrentStatesRunMaster().AsTask();
                else
                    return GetCurrentStatesRun().AsTask();
            else
                return SendFuncs.GetCurrentStates(this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId()).AsTask();
        }

        private Func<Task<Assets.ColonyShared.SharedCode.Aspects.WorldObjects.ComputableStatesDef>> GetCurrentStatesCreateDeferredDelegate() => () =>
        {
            return GetCurrentStates();
        }

        ;
        public async ValueTask<Assets.ColonyShared.SharedCode.Aspects.WorldObjects.ComputableStatesDef> GetCurrentStatesRunMaster()
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    return await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(GetCurrentStatesCreateDeferredDelegate(), nameof(GetCurrentStates));
            }

            return await GetCurrentStatesRun();
        }

        public async ValueTask<Assets.ColonyShared.SharedCode.Aspects.WorldObjects.ComputableStatesDef> GetCurrentStatesRun()
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(Assets.ColonyShared.SharedCode.Entities.IComputableStateMachine), 3);
            var getTask = this.GetThisExclusive();
            var wrapper = getTask.IsCompleted ? getTask.Result : await getTask;
            using (wrapper)
            {
                if (wrapper == null && EntitiesRepository != null)
                    throw new Exception($"{nameof(GetCurrentStates)} GetExclusive wrapper is null");
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
                    var returnTask = GetCurrentStatesImpl();
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