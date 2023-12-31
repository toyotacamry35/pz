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
    public partial class ValueStat
    {
        public ValueTask<bool> ChangeValue(float delta)
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return ChangeValueRunMaster(delta);
                else
                    return ChangeValueRun(delta);
            else
                return SendFuncs.ChangeValue(delta, this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId());
        }

        private Func<Task<bool>> ChangeValueCreateDeferredDelegate(float delta) => () =>
        {
            return ChangeValue(delta).AsTask();
        }

        ;
        public async ValueTask<bool> ChangeValueRunMaster(float delta)
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    return await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(ChangeValueCreateDeferredDelegate(delta), nameof(ChangeValue));
            }

            return await ChangeValueRun(delta);
        }

        public async ValueTask<bool> ChangeValueRun(float delta)
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(Src.Aspects.Impl.Stats.IValueStat), 0);
            var getTask = this.GetThisExclusive();
            var wrapper = getTask.IsCompleted ? getTask.Result : await getTask;
            using (wrapper)
            {
                if (wrapper == null && EntitiesRepository != null)
                    throw new Exception($"{nameof(ChangeValue)} GetExclusive wrapper is null");
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
                    var returnTask = ChangeValueImpl(delta);
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

        public ValueTask Copy(Src.Aspects.Impl.Stats.IValueStat valueStat)
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return CopyRunMaster(valueStat);
                else
                    return CopyRun(valueStat);
            else
                return SendFuncs.Copy(valueStat, this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId());
        }

        private Func<Task> CopyCreateDeferredDelegate(Src.Aspects.Impl.Stats.IValueStat valueStat) => () =>
        {
            return Copy(valueStat).AsTask();
        }

        ;
        public async ValueTask CopyRunMaster(Src.Aspects.Impl.Stats.IValueStat valueStat)
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(CopyCreateDeferredDelegate(valueStat), nameof(Copy));
            }

            await CopyRun(valueStat);
        }

        public async ValueTask CopyRun(Src.Aspects.Impl.Stats.IValueStat valueStat)
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(Src.Aspects.Impl.Stats.IValueStat), 1);
            var getTask = this.GetThisExclusive();
            var wrapper = getTask.IsCompleted ? getTask.Result : await getTask;
            using (wrapper)
            {
                if (wrapper == null && EntitiesRepository != null)
                    throw new Exception($"{nameof(Copy)} GetExclusive wrapper is null");
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
                    var returnTask = CopyImpl(valueStat);
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

        public ValueTask Initialize(Assets.Src.Aspects.Impl.Stats.StatDef statDef, bool resetState)
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return InitializeRunMaster(statDef, resetState);
                else
                    return InitializeRun(statDef, resetState);
            else
                return SendFuncs.Initialize(statDef, resetState, this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId());
        }

        private Func<Task> InitializeCreateDeferredDelegate(Assets.Src.Aspects.Impl.Stats.StatDef statDef, bool resetState) => () =>
        {
            return Initialize(statDef, resetState).AsTask();
        }

        ;
        public async ValueTask InitializeRunMaster(Assets.Src.Aspects.Impl.Stats.StatDef statDef, bool resetState)
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(InitializeCreateDeferredDelegate(statDef, resetState), nameof(Initialize));
            }

            await InitializeRun(statDef, resetState);
        }

        public async ValueTask InitializeRun(Assets.Src.Aspects.Impl.Stats.StatDef statDef, bool resetState)
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(Src.Aspects.Impl.Stats.IValueStat), 2);
            var getTask = this.GetThisExclusive();
            var wrapper = getTask.IsCompleted ? getTask.Result : await getTask;
            using (wrapper)
            {
                if (wrapper == null && EntitiesRepository != null)
                    throw new Exception($"{nameof(Initialize)} GetExclusive wrapper is null");
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
                    var returnTask = InitializeImpl(statDef, resetState);
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

        public ValueTask<bool> RecalculateCaches(bool calcersOnly)
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return RecalculateCachesRunMaster(calcersOnly);
                else
                    return RecalculateCachesRun(calcersOnly);
            else
                return SendFuncs.RecalculateCaches(calcersOnly, this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId());
        }

        private Func<Task<bool>> RecalculateCachesCreateDeferredDelegate(bool calcersOnly) => () =>
        {
            return RecalculateCaches(calcersOnly).AsTask();
        }

        ;
        public async ValueTask<bool> RecalculateCachesRunMaster(bool calcersOnly)
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    return await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(RecalculateCachesCreateDeferredDelegate(calcersOnly), nameof(RecalculateCaches));
            }

            return await RecalculateCachesRun(calcersOnly);
        }

        public async ValueTask<bool> RecalculateCachesRun(bool calcersOnly)
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(Src.Aspects.Impl.Stats.IValueStat), 3);
            var getTask = this.GetThisExclusive();
            var wrapper = getTask.IsCompleted ? getTask.Result : await getTask;
            using (wrapper)
            {
                if (wrapper == null && EntitiesRepository != null)
                    throw new Exception($"{nameof(RecalculateCaches)} GetExclusive wrapper is null");
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
                    var returnTask = RecalculateCachesImpl(calcersOnly);
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

        public ValueTask<float> GetValue()
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.NeedDeferredRpcOnMigrating())
                return GetValueRunMaster();
            else
                return GetValueRun();
        }

        private Func<Task<float>> GetValueCreateDeferredDelegate() => () =>
        {
            return GetValue().AsTask();
        }

        ;
        public async ValueTask<float> GetValueRunMaster()
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    return await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(GetValueCreateDeferredDelegate(), nameof(GetValue));
            }

            return await GetValueRun();
        }

        public async ValueTask<float> GetValueRun()
        {
            this.CheckValidateEntityInAsyncContext();
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(Src.Aspects.Impl.Stats.IValueStat), 4);
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
                    return await GetValueImpl();
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
    }
}