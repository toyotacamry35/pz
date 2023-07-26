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
    public partial class TimerCounter
    {
        public Task PreventOnCompleteEvent()
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return PreventOnCompleteEventRunMaster().AsTask();
                else
                    return PreventOnCompleteEventRun().AsTask();
            else
                return SendFuncs.PreventOnCompleteEvent(this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId()).AsTask();
        }

        private Func<Task> PreventOnCompleteEventCreateDeferredDelegate() => () =>
        {
            return PreventOnCompleteEvent();
        }

        ;
        public async ValueTask PreventOnCompleteEventRunMaster()
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(PreventOnCompleteEventCreateDeferredDelegate(), nameof(PreventOnCompleteEvent));
            }

            await PreventOnCompleteEventRun();
        }

        public async ValueTask PreventOnCompleteEventRun()
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(GeneratedCode.DeltaObjects.ITimerCounter), 0);
            var getTask = this.GetThisExclusive();
            var wrapper = getTask.IsCompleted ? getTask.Result : await getTask;
            using (wrapper)
            {
                if (wrapper == null && EntitiesRepository != null)
                    throw new Exception($"{nameof(PreventOnCompleteEvent)} GetExclusive wrapper is null");
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
                    var returnTask = PreventOnCompleteEventImpl();
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

        public Task OnInit(Assets.Src.Aspects.Impl.Factions.Template.QuestDef questDef, Assets.Src.Aspects.Impl.Factions.Template.QuestCounterDef counterDef, SharedCode.EntitySystem.IEntitiesRepository repository)
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return OnInitRunMaster(questDef, counterDef, repository).AsTask();
                else
                    return OnInitRun(questDef, counterDef, repository).AsTask();
            else
                return SendFuncs.OnInit(questDef, counterDef, repository, this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId()).AsTask();
        }

        private Func<Task> OnInitCreateDeferredDelegate(Assets.Src.Aspects.Impl.Factions.Template.QuestDef questDef, Assets.Src.Aspects.Impl.Factions.Template.QuestCounterDef counterDef, SharedCode.EntitySystem.IEntitiesRepository repository) => () =>
        {
            return OnInit(questDef, counterDef, repository);
        }

        ;
        public async ValueTask OnInitRunMaster(Assets.Src.Aspects.Impl.Factions.Template.QuestDef questDef, Assets.Src.Aspects.Impl.Factions.Template.QuestCounterDef counterDef, SharedCode.EntitySystem.IEntitiesRepository repository)
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(OnInitCreateDeferredDelegate(questDef, counterDef, repository), nameof(OnInit));
            }

            await OnInitRun(questDef, counterDef, repository);
        }

        public async ValueTask OnInitRun(Assets.Src.Aspects.Impl.Factions.Template.QuestDef questDef, Assets.Src.Aspects.Impl.Factions.Template.QuestCounterDef counterDef, SharedCode.EntitySystem.IEntitiesRepository repository)
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(GeneratedCode.DeltaObjects.ITimerCounter), 1);
            var getTask = this.GetThisExclusive();
            var wrapper = getTask.IsCompleted ? getTask.Result : await getTask;
            using (wrapper)
            {
                if (wrapper == null && EntitiesRepository != null)
                    throw new Exception($"{nameof(OnInit)} GetExclusive wrapper is null");
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
                    var returnTask = OnInitImpl(questDef, counterDef, repository);
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

        public Task OnDatabaseLoad(SharedCode.EntitySystem.IEntitiesRepository repository)
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return OnDatabaseLoadRunMaster(repository).AsTask();
                else
                    return OnDatabaseLoadRun(repository).AsTask();
            else
                return SendFuncs.OnDatabaseLoad(repository, this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId()).AsTask();
        }

        private Func<Task> OnDatabaseLoadCreateDeferredDelegate(SharedCode.EntitySystem.IEntitiesRepository repository) => () =>
        {
            return OnDatabaseLoad(repository);
        }

        ;
        public async ValueTask OnDatabaseLoadRunMaster(SharedCode.EntitySystem.IEntitiesRepository repository)
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(OnDatabaseLoadCreateDeferredDelegate(repository), nameof(OnDatabaseLoad));
            }

            await OnDatabaseLoadRun(repository);
        }

        public async ValueTask OnDatabaseLoadRun(SharedCode.EntitySystem.IEntitiesRepository repository)
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(GeneratedCode.DeltaObjects.ITimerCounter), 2);
            var getTask = this.GetThisExclusive();
            var wrapper = getTask.IsCompleted ? getTask.Result : await getTask;
            using (wrapper)
            {
                if (wrapper == null && EntitiesRepository != null)
                    throw new Exception($"{nameof(OnDatabaseLoad)} GetExclusive wrapper is null");
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
                    var returnTask = OnDatabaseLoadImpl(repository);
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

        public Task OnDestroy(SharedCode.EntitySystem.IEntitiesRepository repository)
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return OnDestroyRunMaster(repository).AsTask();
                else
                    return OnDestroyRun(repository).AsTask();
            else
                return SendFuncs.OnDestroy(repository, this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId()).AsTask();
        }

        private Func<Task> OnDestroyCreateDeferredDelegate(SharedCode.EntitySystem.IEntitiesRepository repository) => () =>
        {
            return OnDestroy(repository);
        }

        ;
        public async ValueTask OnDestroyRunMaster(SharedCode.EntitySystem.IEntitiesRepository repository)
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(OnDestroyCreateDeferredDelegate(repository), nameof(OnDestroy));
            }

            await OnDestroyRun(repository);
        }

        public async ValueTask OnDestroyRun(SharedCode.EntitySystem.IEntitiesRepository repository)
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(GeneratedCode.DeltaObjects.ITimerCounter), 3);
            var getTask = this.GetThisExclusive();
            var wrapper = getTask.IsCompleted ? getTask.Result : await getTask;
            using (wrapper)
            {
                if (wrapper == null && EntitiesRepository != null)
                    throw new Exception($"{nameof(OnDestroy)} GetExclusive wrapper is null");
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
                    var returnTask = OnDestroyImpl(repository);
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

        public event System.Func<Assets.Src.Aspects.Impl.Factions.Template.QuestDef, SharedCode.Entities.Engine.IQuestCounter, System.Threading.Tasks.Task> OnCounterCompleted;
        public async Task OnOnCounterCompletedInvoke(Assets.Src.Aspects.Impl.Factions.Template.QuestDef arg1, SharedCode.Entities.Engine.IQuestCounter arg2)
        {
            if (OnCounterCompleted == null)
                return;
            foreach (var subscriber in OnCounterCompleted.GetInvocationList().Cast<System.Func<Assets.Src.Aspects.Impl.Factions.Template.QuestDef, SharedCode.Entities.Engine.IQuestCounter, System.Threading.Tasks.Task>>())
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
                            Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("{0} process event obj {1} method {2} timeout: {3}", nameof(OnCounterCompleted), subscriber.Target?.GetType().Name ?? "unknown", subscriber?.Method.Name ?? "unknown", ServerCoreRuntimeParameters.EntityEventTimeoutSeconds).Write();
                            var __sw__ = new System.Diagnostics.Stopwatch();
                            __sw__.Start();
                            await __subscriberTask__;
                            __sw__.Stop();
                            Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("{0} process event obj {1} method {2} executing too long: {3} seconds", nameof(OnCounterCompleted), subscriber.Target?.GetType().Name ?? "unknown", subscriber?.Method.Name ?? "unknown", ServerCoreRuntimeParameters.EntityEventTimeoutSeconds + __sw__.Elapsed.TotalSeconds).Write();
                        }
                        else
                        {
                            cts.Cancel();
                            if (__subscriberTask__.IsFaulted)
                                Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message(__subscriberTask__.Exception, "Exception in {0} process event obj {1} method {2}", nameof(OnCounterCompleted), subscriber.Target?.GetType().Name ?? "unknown", subscriber?.Method.Name ?? "unknown").Write();
                        }
                    }
                }
                catch (Exception e)
                {
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message(e, "Exception in {0} event obj {1} method {2}", nameof(OnCounterCompleted), subscriber?.Target?.GetType().Name ?? "unknown", subscriber?.Method.Name ?? "unknown").Write();
                }
                finally
                {
                    GeneratedCode.Manual.Repositories.AsyncStackHolder.AssertNoChildren();
                }
            }
        }

        public async Task OnOnCounterCompleted(Assets.Src.Aspects.Impl.Factions.Template.QuestDef arg1, SharedCode.Entities.Engine.IQuestCounter arg2)
        {
            if (this.IsMaster())
            {
                GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(GeneratedCode.DeltaObjects.ITimerCounter), 4);
                INetworkProxy[] __networkProxies__ = this.GatherMessageTargets(SharedCode.EntitySystem.ReplicationLevel.Always, typeof(GeneratedCode.DeltaObjects.ITimerCounter), 4).ToArray();
                if (__networkProxies__.Length > 0)
                    await SendFuncs.OnOnCounterCompleted(arg1, arg2, this, __networkProxies__, this.EntitiesRepository, GetActualMigratingId());
            }

            await OnOnCounterCompletedInvoke(arg1, arg2);
        }

        public event System.Func<SharedCode.Entities.Engine.IQuestCounter, System.Threading.Tasks.Task> OnCounterChanged;
        public async Task OnOnCounterChangedInvoke(SharedCode.Entities.Engine.IQuestCounter arg)
        {
            if (OnCounterChanged == null)
                return;
            foreach (var subscriber in OnCounterChanged.GetInvocationList().Cast<System.Func<SharedCode.Entities.Engine.IQuestCounter, System.Threading.Tasks.Task>>())
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
                            Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("{0} process event obj {1} method {2} timeout: {3}", nameof(OnCounterChanged), subscriber.Target?.GetType().Name ?? "unknown", subscriber?.Method.Name ?? "unknown", ServerCoreRuntimeParameters.EntityEventTimeoutSeconds).Write();
                            var __sw__ = new System.Diagnostics.Stopwatch();
                            __sw__.Start();
                            await __subscriberTask__;
                            __sw__.Stop();
                            Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("{0} process event obj {1} method {2} executing too long: {3} seconds", nameof(OnCounterChanged), subscriber.Target?.GetType().Name ?? "unknown", subscriber?.Method.Name ?? "unknown", ServerCoreRuntimeParameters.EntityEventTimeoutSeconds + __sw__.Elapsed.TotalSeconds).Write();
                        }
                        else
                        {
                            cts.Cancel();
                            if (__subscriberTask__.IsFaulted)
                                Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message(__subscriberTask__.Exception, "Exception in {0} process event obj {1} method {2}", nameof(OnCounterChanged), subscriber.Target?.GetType().Name ?? "unknown", subscriber?.Method.Name ?? "unknown").Write();
                        }
                    }
                }
                catch (Exception e)
                {
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message(e, "Exception in {0} event obj {1} method {2}", nameof(OnCounterChanged), subscriber?.Target?.GetType().Name ?? "unknown", subscriber?.Method.Name ?? "unknown").Write();
                }
                finally
                {
                    GeneratedCode.Manual.Repositories.AsyncStackHolder.AssertNoChildren();
                }
            }
        }

        public async Task OnOnCounterChanged(SharedCode.Entities.Engine.IQuestCounter arg)
        {
            if (this.IsMaster())
            {
                GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(GeneratedCode.DeltaObjects.ITimerCounter), 5);
                INetworkProxy[] __networkProxies__ = this.GatherMessageTargets(SharedCode.EntitySystem.ReplicationLevel.Always, typeof(GeneratedCode.DeltaObjects.ITimerCounter), 5).ToArray();
                if (__networkProxies__.Length > 0)
                    await SendFuncs.OnOnCounterChanged(arg, this, __networkProxies__, this.EntitiesRepository, GetActualMigratingId());
            }

            await OnOnCounterChangedInvoke(arg);
        }
    }
}