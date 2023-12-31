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
    public partial class QuestEngine
    {
        public Task AddQuest(Assets.Src.Aspects.Impl.Factions.Template.QuestDef quest)
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return AddQuestRunMaster(quest).AsTask();
                else
                    return AddQuestRun(quest).AsTask();
            else
                return SendFuncs.AddQuest(quest, this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId()).AsTask();
        }

        private Func<Task> AddQuestCreateDeferredDelegate(Assets.Src.Aspects.Impl.Factions.Template.QuestDef quest) => () =>
        {
            return AddQuest(quest);
        }

        ;
        public async ValueTask AddQuestRunMaster(Assets.Src.Aspects.Impl.Factions.Template.QuestDef quest)
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(AddQuestCreateDeferredDelegate(quest), nameof(AddQuest));
            }

            await AddQuestRun(quest);
        }

        public async ValueTask AddQuestRun(Assets.Src.Aspects.Impl.Factions.Template.QuestDef quest)
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(SharedCode.Entities.Engine.IQuestEngine), 0);
            var getTask = this.GetThisExclusive();
            var wrapper = getTask.IsCompleted ? getTask.Result : await getTask;
            using (wrapper)
            {
                if (wrapper == null && EntitiesRepository != null)
                    throw new Exception($"{nameof(AddQuest)} GetExclusive wrapper is null");
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
                    var returnTask = AddQuestImpl(quest);
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

        public Task AddQuestObject(Assets.Src.Aspects.Impl.Factions.Template.QuestDef questDef)
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return AddQuestObjectRunMaster(questDef).AsTask();
                else
                    return AddQuestObjectRun(questDef).AsTask();
            else
                return SendFuncs.AddQuestObject(questDef, this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId()).AsTask();
        }

        private Func<Task> AddQuestObjectCreateDeferredDelegate(Assets.Src.Aspects.Impl.Factions.Template.QuestDef questDef) => () =>
        {
            return AddQuestObject(questDef);
        }

        ;
        public async ValueTask AddQuestObjectRunMaster(Assets.Src.Aspects.Impl.Factions.Template.QuestDef questDef)
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(AddQuestObjectCreateDeferredDelegate(questDef), nameof(AddQuestObject));
            }

            await AddQuestObjectRun(questDef);
        }

        public async ValueTask AddQuestObjectRun(Assets.Src.Aspects.Impl.Factions.Template.QuestDef questDef)
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(SharedCode.Entities.Engine.IQuestEngine), 1);
            var getTask = this.GetThisExclusive();
            var wrapper = getTask.IsCompleted ? getTask.Result : await getTask;
            using (wrapper)
            {
                if (wrapper == null && EntitiesRepository != null)
                    throw new Exception($"{nameof(AddQuestObject)} GetExclusive wrapper is null");
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
                    var returnTask = AddQuestObjectImpl(questDef);
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

        public Task ChangePhase(Assets.Src.Aspects.Impl.Factions.Template.QuestDef questDef, int newPhaseIndex)
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return ChangePhaseRunMaster(questDef, newPhaseIndex).AsTask();
                else
                    return ChangePhaseRun(questDef, newPhaseIndex).AsTask();
            else
                return SendFuncs.ChangePhase(questDef, newPhaseIndex, this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId()).AsTask();
        }

        private Func<Task> ChangePhaseCreateDeferredDelegate(Assets.Src.Aspects.Impl.Factions.Template.QuestDef questDef, int newPhaseIndex) => () =>
        {
            return ChangePhase(questDef, newPhaseIndex);
        }

        ;
        public async ValueTask ChangePhaseRunMaster(Assets.Src.Aspects.Impl.Factions.Template.QuestDef questDef, int newPhaseIndex)
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(ChangePhaseCreateDeferredDelegate(questDef, newPhaseIndex), nameof(ChangePhase));
            }

            await ChangePhaseRun(questDef, newPhaseIndex);
        }

        public async ValueTask ChangePhaseRun(Assets.Src.Aspects.Impl.Factions.Template.QuestDef questDef, int newPhaseIndex)
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(SharedCode.Entities.Engine.IQuestEngine), 2);
            var getTask = this.GetThisExclusive();
            var wrapper = getTask.IsCompleted ? getTask.Result : await getTask;
            using (wrapper)
            {
                if (wrapper == null && EntitiesRepository != null)
                    throw new Exception($"{nameof(ChangePhase)} GetExclusive wrapper is null");
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
                    var returnTask = ChangePhaseImpl(questDef, newPhaseIndex);
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

        public Task RemoveQuest(Assets.Src.Aspects.Impl.Factions.Template.QuestDef quest)
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return RemoveQuestRunMaster(quest).AsTask();
                else
                    return RemoveQuestRun(quest).AsTask();
            else
                return SendFuncs.RemoveQuest(quest, this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId()).AsTask();
        }

        private Func<Task> RemoveQuestCreateDeferredDelegate(Assets.Src.Aspects.Impl.Factions.Template.QuestDef quest) => () =>
        {
            return RemoveQuest(quest);
        }

        ;
        public async ValueTask RemoveQuestRunMaster(Assets.Src.Aspects.Impl.Factions.Template.QuestDef quest)
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(RemoveQuestCreateDeferredDelegate(quest), nameof(RemoveQuest));
            }

            await RemoveQuestRun(quest);
        }

        public async ValueTask RemoveQuestRun(Assets.Src.Aspects.Impl.Factions.Template.QuestDef quest)
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(SharedCode.Entities.Engine.IQuestEngine), 3);
            var getTask = this.GetThisExclusive();
            var wrapper = getTask.IsCompleted ? getTask.Result : await getTask;
            using (wrapper)
            {
                if (wrapper == null && EntitiesRepository != null)
                    throw new Exception($"{nameof(RemoveQuest)} GetExclusive wrapper is null");
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
                    var returnTask = RemoveQuestImpl(quest);
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

        public Task<bool> RemoveAllQuests()
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return RemoveAllQuestsRunMaster().AsTask();
                else
                    return RemoveAllQuestsRun().AsTask();
            else
                return SendFuncs.RemoveAllQuests(this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId()).AsTask();
        }

        private Func<Task<bool>> RemoveAllQuestsCreateDeferredDelegate() => () =>
        {
            return RemoveAllQuests();
        }

        ;
        public async ValueTask<bool> RemoveAllQuestsRunMaster()
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    return await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(RemoveAllQuestsCreateDeferredDelegate(), nameof(RemoveAllQuests));
            }

            return await RemoveAllQuestsRun();
        }

        public async ValueTask<bool> RemoveAllQuestsRun()
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(SharedCode.Entities.Engine.IQuestEngine), 4);
            var getTask = this.GetThisExclusive();
            var wrapper = getTask.IsCompleted ? getTask.Result : await getTask;
            using (wrapper)
            {
                if (wrapper == null && EntitiesRepository != null)
                    throw new Exception($"{nameof(RemoveAllQuests)} GetExclusive wrapper is null");
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
                    var returnTask = RemoveAllQuestsImpl();
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

        public Task SetVisible(Assets.Src.Aspects.Impl.Factions.Template.QuestDef quest, bool visible)
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return SetVisibleRunMaster(quest, visible).AsTask();
                else
                    return SetVisibleRun(quest, visible).AsTask();
            else
                return SendFuncs.SetVisible(quest, visible, this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId()).AsTask();
        }

        private Func<Task> SetVisibleCreateDeferredDelegate(Assets.Src.Aspects.Impl.Factions.Template.QuestDef quest, bool visible) => () =>
        {
            return SetVisible(quest, visible);
        }

        ;
        public async ValueTask SetVisibleRunMaster(Assets.Src.Aspects.Impl.Factions.Template.QuestDef quest, bool visible)
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(SetVisibleCreateDeferredDelegate(quest, visible), nameof(SetVisible));
            }

            await SetVisibleRun(quest, visible);
        }

        public async ValueTask SetVisibleRun(Assets.Src.Aspects.Impl.Factions.Template.QuestDef quest, bool visible)
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(SharedCode.Entities.Engine.IQuestEngine), 5);
            var getTask = this.GetThisExclusive();
            var wrapper = getTask.IsCompleted ? getTask.Result : await getTask;
            using (wrapper)
            {
                if (wrapper == null && EntitiesRepository != null)
                    throw new Exception($"{nameof(SetVisible)} GetExclusive wrapper is null");
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
                    var returnTask = SetVisibleImpl(quest, visible);
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

        public Task OnDatabaseLoad()
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return OnDatabaseLoadRunMaster().AsTask();
                else
                    return OnDatabaseLoadRun().AsTask();
            else
                return SendFuncs.OnDatabaseLoad(this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId()).AsTask();
        }

        private Func<Task> OnDatabaseLoadCreateDeferredDelegate() => () =>
        {
            return OnDatabaseLoad();
        }

        ;
        public async ValueTask OnDatabaseLoadRunMaster()
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(OnDatabaseLoadCreateDeferredDelegate(), nameof(OnDatabaseLoad));
            }

            await OnDatabaseLoadRun();
        }

        public async ValueTask OnDatabaseLoadRun()
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(SharedCode.Entities.Engine.IQuestEngine), 6);
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
                    var returnTask = OnDatabaseLoadImpl();
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

namespace GeneratedCode.DeltaObjects
{
    public partial class QuestObject
    {
        public Task SetStatus(SharedCode.Entities.Engine.QuestStatus newStatus)
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return SetStatusRunMaster(newStatus).AsTask();
                else
                    return SetStatusRun(newStatus).AsTask();
            else
                return SendFuncs.SetStatus(newStatus, this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId()).AsTask();
        }

        private Func<Task> SetStatusCreateDeferredDelegate(SharedCode.Entities.Engine.QuestStatus newStatus) => () =>
        {
            return SetStatus(newStatus);
        }

        ;
        public async ValueTask SetStatusRunMaster(SharedCode.Entities.Engine.QuestStatus newStatus)
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(SetStatusCreateDeferredDelegate(newStatus), nameof(SetStatus));
            }

            await SetStatusRun(newStatus);
        }

        public async ValueTask SetStatusRun(SharedCode.Entities.Engine.QuestStatus newStatus)
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(SharedCode.Entities.Engine.IQuestObject), 0);
            var getTask = this.GetThisExclusive();
            var wrapper = getTask.IsCompleted ? getTask.Result : await getTask;
            using (wrapper)
            {
                if (wrapper == null && EntitiesRepository != null)
                    throw new Exception($"{nameof(SetStatus)} GetExclusive wrapper is null");
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
                    var returnTask = SetStatusImpl(newStatus);
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

        public Task AddPhaseCounters(SharedCode.Entities.Engine.IQuestCounter succCounter, SharedCode.Entities.Engine.IQuestCounter failCounter)
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return AddPhaseCountersRunMaster(succCounter, failCounter).AsTask();
                else
                    return AddPhaseCountersRun(succCounter, failCounter).AsTask();
            else
                return SendFuncs.AddPhaseCounters(succCounter, failCounter, this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId()).AsTask();
        }

        private Func<Task> AddPhaseCountersCreateDeferredDelegate(SharedCode.Entities.Engine.IQuestCounter succCounter, SharedCode.Entities.Engine.IQuestCounter failCounter) => () =>
        {
            return AddPhaseCounters(succCounter, failCounter);
        }

        ;
        public async ValueTask AddPhaseCountersRunMaster(SharedCode.Entities.Engine.IQuestCounter succCounter, SharedCode.Entities.Engine.IQuestCounter failCounter)
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(AddPhaseCountersCreateDeferredDelegate(succCounter, failCounter), nameof(AddPhaseCounters));
            }

            await AddPhaseCountersRun(succCounter, failCounter);
        }

        public async ValueTask AddPhaseCountersRun(SharedCode.Entities.Engine.IQuestCounter succCounter, SharedCode.Entities.Engine.IQuestCounter failCounter)
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(SharedCode.Entities.Engine.IQuestObject), 1);
            var getTask = this.GetThisExclusive();
            var wrapper = getTask.IsCompleted ? getTask.Result : await getTask;
            using (wrapper)
            {
                if (wrapper == null && EntitiesRepository != null)
                    throw new Exception($"{nameof(AddPhaseCounters)} GetExclusive wrapper is null");
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
                    var returnTask = AddPhaseCountersImpl(succCounter, failCounter);
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