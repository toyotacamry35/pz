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
    public partial class WorldPersonalMachineEngine
    {
        public Task<ResourceSystem.Utils.OuterRef> GetOrAddMachine(SharedCode.Aspects.Item.Templates.WorldPersonalMachineDef def, ResourceSystem.Utils.OuterRef key)
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return GetOrAddMachineRunMaster(def, key).AsTask();
                else
                    return GetOrAddMachineRun(def, key).AsTask();
            else
                return SendFuncs.GetOrAddMachine(def, key, this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId()).AsTask();
        }

        private Func<Task<ResourceSystem.Utils.OuterRef>> GetOrAddMachineCreateDeferredDelegate(SharedCode.Aspects.Item.Templates.WorldPersonalMachineDef def, ResourceSystem.Utils.OuterRef key) => () =>
        {
            return GetOrAddMachine(def, key);
        }

        ;
        public async ValueTask<ResourceSystem.Utils.OuterRef> GetOrAddMachineRunMaster(SharedCode.Aspects.Item.Templates.WorldPersonalMachineDef def, ResourceSystem.Utils.OuterRef key)
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    return await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(GetOrAddMachineCreateDeferredDelegate(def, key), nameof(GetOrAddMachine));
            }

            return await GetOrAddMachineRun(def, key);
        }

        public async ValueTask<ResourceSystem.Utils.OuterRef> GetOrAddMachineRun(SharedCode.Aspects.Item.Templates.WorldPersonalMachineDef def, ResourceSystem.Utils.OuterRef key)
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(Assets.ColonyShared.SharedCode.Entities.Engine.IWorldPersonalMachineEngine), 0);
            var getTask = this.GetThisExclusive();
            var wrapper = getTask.IsCompleted ? getTask.Result : await getTask;
            using (wrapper)
            {
                if (wrapper == null && EntitiesRepository != null)
                    throw new Exception($"{nameof(GetOrAddMachine)} GetExclusive wrapper is null");
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
                    var returnTask = GetOrAddMachineImpl(def, key);
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

        public Task RemoveMachine(ResourceSystem.Utils.OuterRef key)
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return RemoveMachineRunMaster(key).AsTask();
                else
                    return RemoveMachineRun(key).AsTask();
            else
                return SendFuncs.RemoveMachine(key, this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId()).AsTask();
        }

        private Func<Task> RemoveMachineCreateDeferredDelegate(ResourceSystem.Utils.OuterRef key) => () =>
        {
            return RemoveMachine(key);
        }

        ;
        public async ValueTask RemoveMachineRunMaster(ResourceSystem.Utils.OuterRef key)
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(RemoveMachineCreateDeferredDelegate(key), nameof(RemoveMachine));
            }

            await RemoveMachineRun(key);
        }

        public async ValueTask RemoveMachineRun(ResourceSystem.Utils.OuterRef key)
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(Assets.ColonyShared.SharedCode.Entities.Engine.IWorldPersonalMachineEngine), 1);
            var getTask = this.GetThisExclusive();
            var wrapper = getTask.IsCompleted ? getTask.Result : await getTask;
            using (wrapper)
            {
                if (wrapper == null && EntitiesRepository != null)
                    throw new Exception($"{nameof(RemoveMachine)} GetExclusive wrapper is null");
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
                    var returnTask = RemoveMachineImpl(key);
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