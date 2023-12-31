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
    public partial class MobMovementSync
    {
        public Task SetMovementData(SharedCode.MovementSync.MovementData data)
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return SetMovementDataRunMaster(data).AsTask();
                else
                    return SetMovementDataRun(data).AsTask();
            else
                return SendFuncs.SetMovementData(data, this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId()).AsTask();
        }

        private Func<Task> SetMovementDataCreateDeferredDelegate(SharedCode.MovementSync.MovementData data) => () =>
        {
            return SetMovementData(data);
        }

        ;
        public async ValueTask SetMovementDataRunMaster(SharedCode.MovementSync.MovementData data)
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(SetMovementDataCreateDeferredDelegate(data), nameof(SetMovementData));
            }

            await SetMovementDataRun(data);
        }

        public async ValueTask SetMovementDataRun(SharedCode.MovementSync.MovementData data)
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(SharedCode.MovementSync.IMobMovementSync), 0);
            var getTask = this.GetThisExclusive();
            var wrapper = getTask.IsCompleted ? getTask.Result : await getTask;
            using (wrapper)
            {
                if (wrapper == null && EntitiesRepository != null)
                    throw new Exception($"{nameof(SetMovementData)} GetExclusive wrapper is null");
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
                    var returnTask = SetMovementDataImpl(data);
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

        public Task<bool> SetPathFindingOwnerRepositoryId(System.Guid repositoryId)
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return SetPathFindingOwnerRepositoryIdRunMaster(repositoryId).AsTask();
                else
                    return SetPathFindingOwnerRepositoryIdRun(repositoryId).AsTask();
            else
                return SendFuncs.SetPathFindingOwnerRepositoryId(repositoryId, this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId()).AsTask();
        }

        private Func<Task<bool>> SetPathFindingOwnerRepositoryIdCreateDeferredDelegate(System.Guid repositoryId) => () =>
        {
            return SetPathFindingOwnerRepositoryId(repositoryId);
        }

        ;
        public async ValueTask<bool> SetPathFindingOwnerRepositoryIdRunMaster(System.Guid repositoryId)
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    return await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(SetPathFindingOwnerRepositoryIdCreateDeferredDelegate(repositoryId), nameof(SetPathFindingOwnerRepositoryId));
            }

            return await SetPathFindingOwnerRepositoryIdRun(repositoryId);
        }

        public async ValueTask<bool> SetPathFindingOwnerRepositoryIdRun(System.Guid repositoryId)
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(SharedCode.MovementSync.IMobMovementSync), 1);
            var getTask = this.GetThisExclusive();
            var wrapper = getTask.IsCompleted ? getTask.Result : await getTask;
            using (wrapper)
            {
                if (wrapper == null && EntitiesRepository != null)
                    throw new Exception($"{nameof(SetPathFindingOwnerRepositoryId)} GetExclusive wrapper is null");
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
                    var returnTask = SetPathFindingOwnerRepositoryIdImpl(repositoryId);
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

        public Task UpdateMovement(SharedCode.MovementSync.MobMovementStatePacked state, long counter, bool important)
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return UpdateMovementRunMaster(state, counter, important).AsTask();
                else
                    return UpdateMovementRun(state, counter, important).AsTask();
            else
                return SendFuncs.UpdateMovement(state, counter, important, this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId()).AsTask();
        }

        private Func<Task> UpdateMovementCreateDeferredDelegate(SharedCode.MovementSync.MobMovementStatePacked state, long counter, bool important) => () =>
        {
            return UpdateMovement(state, counter, important);
        }

        ;
        public async ValueTask UpdateMovementRunMaster(SharedCode.MovementSync.MobMovementStatePacked state, long counter, bool important)
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(UpdateMovementCreateDeferredDelegate(state, counter, important), nameof(UpdateMovement));
            }

            await UpdateMovementRun(state, counter, important);
        }

        public async ValueTask UpdateMovementRun(SharedCode.MovementSync.MobMovementStatePacked state, long counter, bool important)
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(SharedCode.MovementSync.IMobMovementSync), 2);
            var getTask = this.GetThisExclusive();
            var wrapper = getTask.IsCompleted ? getTask.Result : await getTask;
            using (wrapper)
            {
                if (wrapper == null && EntitiesRepository != null)
                    throw new Exception($"{nameof(UpdateMovement)} GetExclusive wrapper is null");
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
                    var returnTask = UpdateMovementImpl(state, counter, important);
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

        public Task StopMovement(SharedCode.Wizardry.SpellId spellId, GeneratedDefsForSpells.MoveEffectDef moveEffectDef, bool success)
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return StopMovementRunMaster(spellId, moveEffectDef, success).AsTask();
                else
                    return StopMovementRun(spellId, moveEffectDef, success).AsTask();
            else
                return SendFuncs.StopMovement(spellId, moveEffectDef, success, this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId()).AsTask();
        }

        private Func<Task> StopMovementCreateDeferredDelegate(SharedCode.Wizardry.SpellId spellId, GeneratedDefsForSpells.MoveEffectDef moveEffectDef, bool success) => () =>
        {
            return StopMovement(spellId, moveEffectDef, success);
        }

        ;
        public async ValueTask StopMovementRunMaster(SharedCode.Wizardry.SpellId spellId, GeneratedDefsForSpells.MoveEffectDef moveEffectDef, bool success)
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(StopMovementCreateDeferredDelegate(spellId, moveEffectDef, success), nameof(StopMovement));
            }

            await StopMovementRun(spellId, moveEffectDef, success);
        }

        public async ValueTask StopMovementRun(SharedCode.Wizardry.SpellId spellId, GeneratedDefsForSpells.MoveEffectDef moveEffectDef, bool success)
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(SharedCode.MovementSync.IMobMovementSync), 3);
            var getTask = this.GetThisExclusive();
            var wrapper = getTask.IsCompleted ? getTask.Result : await getTask;
            using (wrapper)
            {
                if (wrapper == null && EntitiesRepository != null)
                    throw new Exception($"{nameof(StopMovement)} GetExclusive wrapper is null");
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
                    var returnTask = StopMovementImpl(spellId, moveEffectDef, success);
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

        public Task InvokeSetDebugMobPositionLoggingEvent(bool enabledStatus, bool dump)
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return InvokeSetDebugMobPositionLoggingEventRunMaster(enabledStatus, dump).AsTask();
                else
                    return InvokeSetDebugMobPositionLoggingEventRun(enabledStatus, dump).AsTask();
            else
                return SendFuncs.InvokeSetDebugMobPositionLoggingEvent(enabledStatus, dump, this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId()).AsTask();
        }

        private Func<Task> InvokeSetDebugMobPositionLoggingEventCreateDeferredDelegate(bool enabledStatus, bool dump) => () =>
        {
            return InvokeSetDebugMobPositionLoggingEvent(enabledStatus, dump);
        }

        ;
        public async ValueTask InvokeSetDebugMobPositionLoggingEventRunMaster(bool enabledStatus, bool dump)
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(InvokeSetDebugMobPositionLoggingEventCreateDeferredDelegate(enabledStatus, dump), nameof(InvokeSetDebugMobPositionLoggingEvent));
            }

            await InvokeSetDebugMobPositionLoggingEventRun(enabledStatus, dump);
        }

        public async ValueTask InvokeSetDebugMobPositionLoggingEventRun(bool enabledStatus, bool dump)
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(SharedCode.MovementSync.IMobMovementSync), 4);
            var getTask = this.GetThisExclusive();
            var wrapper = getTask.IsCompleted ? getTask.Result : await getTask;
            using (wrapper)
            {
                if (wrapper == null && EntitiesRepository != null)
                    throw new Exception($"{nameof(InvokeSetDebugMobPositionLoggingEvent)} GetExclusive wrapper is null");
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
                    var returnTask = InvokeSetDebugMobPositionLoggingEventImpl(enabledStatus, dump);
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

        public event System.Func<SharedCode.MovementSync.MobMovementStatePacked, System.Threading.Tasks.Task> __SyncMovementUnreliable;
        public async Task On__SyncMovementUnreliableInvoke(SharedCode.MovementSync.MobMovementStatePacked arg)
        {
            if (__SyncMovementUnreliable == null)
                return;
            foreach (var subscriber in __SyncMovementUnreliable.GetInvocationList().Cast<System.Func<SharedCode.MovementSync.MobMovementStatePacked, System.Threading.Tasks.Task>>())
            {
                try
                {
                    using (var cts = new CancellationTokenSource())
                    {
                        var __timeoutTask__ = Task.Delay(TimeSpan.FromSeconds(ServerCoreRuntimeParameters.EntityEventTimeoutSeconds), cts.Token);
                        var __subscriberTask__ = subscriber(arg);
                        await Task.WhenAny(__subscriberTask__, __timeoutTask__);
                        if (!__subscriberTask__.IsCompleted)
                        {
                            Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("{0} process event obj {1} method {2} timeout: {3}", nameof(__SyncMovementUnreliable), subscriber.Target?.GetType().Name ?? "unknown", subscriber?.Method.Name ?? "unknown", ServerCoreRuntimeParameters.EntityEventTimeoutSeconds).Write();
                            var __sw__ = new System.Diagnostics.Stopwatch();
                            __sw__.Start();
                            await __subscriberTask__;
                            __sw__.Stop();
                            Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("{0} process event obj {1} method {2} executing too long: {3} seconds", nameof(__SyncMovementUnreliable), subscriber.Target?.GetType().Name ?? "unknown", subscriber?.Method.Name ?? "unknown", ServerCoreRuntimeParameters.EntityEventTimeoutSeconds + __sw__.Elapsed.TotalSeconds).Write();
                        }
                        else
                        {
                            cts.Cancel();
                            if (__subscriberTask__.IsFaulted)
                                Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message(__subscriberTask__.Exception, "Exception in {0} process event obj {1} method {2}", nameof(__SyncMovementUnreliable), subscriber.Target?.GetType().Name ?? "unknown", subscriber?.Method.Name ?? "unknown").Write();
                        }
                    }
                }
                catch (Exception e)
                {
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message(e, "Exception in {0} event obj {1} method {2}", nameof(__SyncMovementUnreliable), subscriber?.Target?.GetType().Name ?? "unknown", subscriber?.Method.Name ?? "unknown").Write();
                }
                finally
                {
                    GeneratedCode.Manual.Repositories.AsyncStackHolder.AssertNoChildren();
                }
            }
        }

        public async Task On__SyncMovementUnreliable(SharedCode.MovementSync.MobMovementStatePacked arg)
        {
            if (this.IsMaster())
            {
                GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(SharedCode.MovementSync.IMobMovementSync), 5);
                INetworkProxy[] __networkProxies__ = this.GatherMessageTargets(SharedCode.EntitySystem.ReplicationLevel.Always, typeof(SharedCode.MovementSync.IMobMovementSync), 5).ToArray();
                if (__networkProxies__.Length > 0)
                    await SendFuncs.On__SyncMovementUnreliable(arg, this, __networkProxies__, this.EntitiesRepository, GetActualMigratingId());
            }

            await On__SyncMovementUnreliableInvoke(arg);
        }

        public event System.Func<bool, bool, System.Threading.Tasks.Task> SetDebugMobPositionLoggingEvent;
        public async Task OnSetDebugMobPositionLoggingEventInvoke(bool arg1, bool arg2)
        {
            if (SetDebugMobPositionLoggingEvent == null)
                return;
            foreach (var subscriber in SetDebugMobPositionLoggingEvent.GetInvocationList().Cast<System.Func<bool, bool, System.Threading.Tasks.Task>>())
            {
                try
                {
                    using (var cts = new CancellationTokenSource())
                    {
                        var __timeoutTask__ = Task.Delay(TimeSpan.FromSeconds(ServerCoreRuntimeParameters.EntityEventTimeoutSeconds), cts.Token);
                        var __subscriberTask__ = subscriber(arg1, arg2);
                        await Task.WhenAny(__subscriberTask__, __timeoutTask__);
                        if (!__subscriberTask__.IsCompleted)
                        {
                            Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("{0} process event obj {1} method {2} timeout: {3}", nameof(SetDebugMobPositionLoggingEvent), subscriber.Target?.GetType().Name ?? "unknown", subscriber?.Method.Name ?? "unknown", ServerCoreRuntimeParameters.EntityEventTimeoutSeconds).Write();
                            var __sw__ = new System.Diagnostics.Stopwatch();
                            __sw__.Start();
                            await __subscriberTask__;
                            __sw__.Stop();
                            Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("{0} process event obj {1} method {2} executing too long: {3} seconds", nameof(SetDebugMobPositionLoggingEvent), subscriber.Target?.GetType().Name ?? "unknown", subscriber?.Method.Name ?? "unknown", ServerCoreRuntimeParameters.EntityEventTimeoutSeconds + __sw__.Elapsed.TotalSeconds).Write();
                        }
                        else
                        {
                            cts.Cancel();
                            if (__subscriberTask__.IsFaulted)
                                Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message(__subscriberTask__.Exception, "Exception in {0} process event obj {1} method {2}", nameof(SetDebugMobPositionLoggingEvent), subscriber.Target?.GetType().Name ?? "unknown", subscriber?.Method.Name ?? "unknown").Write();
                        }
                    }
                }
                catch (Exception e)
                {
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message(e, "Exception in {0} event obj {1} method {2}", nameof(SetDebugMobPositionLoggingEvent), subscriber?.Target?.GetType().Name ?? "unknown", subscriber?.Method.Name ?? "unknown").Write();
                }
                finally
                {
                    GeneratedCode.Manual.Repositories.AsyncStackHolder.AssertNoChildren();
                }
            }
        }

        public async Task OnSetDebugMobPositionLoggingEvent(bool arg1, bool arg2)
        {
            if (this.IsMaster())
            {
                GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(SharedCode.MovementSync.IMobMovementSync), 6);
                INetworkProxy[] __networkProxies__ = this.GatherMessageTargets(SharedCode.EntitySystem.ReplicationLevel.Always, typeof(SharedCode.MovementSync.IMobMovementSync), 6).ToArray();
                if (__networkProxies__.Length > 0)
                    await SendFuncs.OnSetDebugMobPositionLoggingEvent(arg1, arg2, this, __networkProxies__, this.EntitiesRepository, GetActualMigratingId());
            }

            await OnSetDebugMobPositionLoggingEventInvoke(arg1, arg2);
        }
    }
}