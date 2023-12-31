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
    public partial class BakenCharacterEntity
    {
        public ValueTask<bool> BakenCanBeActivated(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> bakenRef)
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.NeedDeferredRpcOnMigrating())
                return BakenCanBeActivatedRunMaster(bakenRef);
            else
                return BakenCanBeActivatedRun(bakenRef);
        }

        private Func<Task<bool>> BakenCanBeActivatedCreateDeferredDelegate(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> bakenRef) => () =>
        {
            return BakenCanBeActivated(bakenRef).AsTask();
        }

        ;
        public async ValueTask<bool> BakenCanBeActivatedRunMaster(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> bakenRef)
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    return await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(BakenCanBeActivatedCreateDeferredDelegate(bakenRef), nameof(BakenCanBeActivated));
            }

            return await BakenCanBeActivatedRun(bakenRef);
        }

        public async ValueTask<bool> BakenCanBeActivatedRun(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> bakenRef)
        {
            this.CheckValidateEntityInAsyncContext();
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(SharedCode.Entities.IBakenCharacterEntity), 0);
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
                    return await BakenCanBeActivatedImpl(bakenRef);
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

        public ValueTask<bool> ActivateBaken(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> bakenRef)
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return ActivateBakenRunMaster(bakenRef);
                else
                    return ActivateBakenRun(bakenRef);
            else
                return SendFuncs.ActivateBaken(bakenRef, this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId());
        }

        private Func<Task<bool>> ActivateBakenCreateDeferredDelegate(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> bakenRef) => () =>
        {
            return ActivateBaken(bakenRef).AsTask();
        }

        ;
        public async ValueTask<bool> ActivateBakenRunMaster(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> bakenRef)
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    return await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(ActivateBakenCreateDeferredDelegate(bakenRef), nameof(ActivateBaken));
            }

            return await ActivateBakenRun(bakenRef);
        }

        public async ValueTask<bool> ActivateBakenRun(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> bakenRef)
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(SharedCode.Entities.IBakenCharacterEntity), 1);
            var getTask = this.GetThisExclusive();
            var wrapper = getTask.IsCompleted ? getTask.Result : await getTask;
            using (wrapper)
            {
                if (wrapper == null && EntitiesRepository != null)
                    throw new Exception($"{nameof(ActivateBaken)} GetExclusive wrapper is null");
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
                    var returnTask = ActivateBakenImpl(bakenRef);
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

        public ValueTask RegisterBaken(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> bakenRef, bool loaded)
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return RegisterBakenRunMaster(bakenRef, loaded);
                else
                    return RegisterBakenRun(bakenRef, loaded);
            else
                return SendFuncs.RegisterBaken(bakenRef, loaded, this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId());
        }

        private Func<Task> RegisterBakenCreateDeferredDelegate(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> bakenRef, bool loaded) => () =>
        {
            return RegisterBaken(bakenRef, loaded).AsTask();
        }

        ;
        public async ValueTask RegisterBakenRunMaster(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> bakenRef, bool loaded)
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(RegisterBakenCreateDeferredDelegate(bakenRef, loaded), nameof(RegisterBaken));
            }

            await RegisterBakenRun(bakenRef, loaded);
        }

        public async ValueTask RegisterBakenRun(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> bakenRef, bool loaded)
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(SharedCode.Entities.IBakenCharacterEntity), 2);
            var getTask = this.GetThisExclusive();
            var wrapper = getTask.IsCompleted ? getTask.Result : await getTask;
            using (wrapper)
            {
                if (wrapper == null && EntitiesRepository != null)
                    throw new Exception($"{nameof(RegisterBaken)} GetExclusive wrapper is null");
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
                    var returnTask = RegisterBakenImpl(bakenRef, loaded);
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

        public ValueTask BakenIsDestroyed(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> bakenRef)
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return BakenIsDestroyedRunMaster(bakenRef);
                else
                    return BakenIsDestroyedRun(bakenRef);
            else
                return SendFuncs.BakenIsDestroyed(bakenRef, this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId());
        }

        private Func<Task> BakenIsDestroyedCreateDeferredDelegate(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> bakenRef) => () =>
        {
            return BakenIsDestroyed(bakenRef).AsTask();
        }

        ;
        public async ValueTask BakenIsDestroyedRunMaster(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> bakenRef)
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(BakenIsDestroyedCreateDeferredDelegate(bakenRef), nameof(BakenIsDestroyed));
            }

            await BakenIsDestroyedRun(bakenRef);
        }

        public async ValueTask BakenIsDestroyedRun(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> bakenRef)
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(SharedCode.Entities.IBakenCharacterEntity), 3);
            var getTask = this.GetThisExclusive();
            var wrapper = getTask.IsCompleted ? getTask.Result : await getTask;
            using (wrapper)
            {
                if (wrapper == null && EntitiesRepository != null)
                    throw new Exception($"{nameof(BakenIsDestroyed)} GetExclusive wrapper is null");
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
                    var returnTask = BakenIsDestroyedImpl(bakenRef);
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

        public ValueTask SetCharacterLoaded(bool loaded)
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return SetCharacterLoadedRunMaster(loaded);
                else
                    return SetCharacterLoadedRun(loaded);
            else
                return SendFuncs.SetCharacterLoaded(loaded, this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId());
        }

        private Func<Task> SetCharacterLoadedCreateDeferredDelegate(bool loaded) => () =>
        {
            return SetCharacterLoaded(loaded).AsTask();
        }

        ;
        public async ValueTask SetCharacterLoadedRunMaster(bool loaded)
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(SetCharacterLoadedCreateDeferredDelegate(loaded), nameof(SetCharacterLoaded));
            }

            await SetCharacterLoadedRun(loaded);
        }

        public async ValueTask SetCharacterLoadedRun(bool loaded)
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(SharedCode.Entities.IBakenCharacterEntity), 4);
            var getTask = this.GetThisExclusive();
            var wrapper = getTask.IsCompleted ? getTask.Result : await getTask;
            using (wrapper)
            {
                if (wrapper == null && EntitiesRepository != null)
                    throw new Exception($"{nameof(SetCharacterLoaded)} GetExclusive wrapper is null");
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
                    var returnTask = SetCharacterLoadedImpl(loaded);
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

        public ValueTask SetLogin(bool logined)
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return SetLoginRunMaster(logined);
                else
                    return SetLoginRun(logined);
            else
                return SendFuncs.SetLogin(logined, this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId());
        }

        private Func<Task> SetLoginCreateDeferredDelegate(bool logined) => () =>
        {
            return SetLogin(logined).AsTask();
        }

        ;
        public async ValueTask SetLoginRunMaster(bool logined)
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(SetLoginCreateDeferredDelegate(logined), nameof(SetLogin));
            }

            await SetLoginRun(logined);
        }

        public async ValueTask SetLoginRun(bool logined)
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(SharedCode.Entities.IBakenCharacterEntity), 5);
            var getTask = this.GetThisExclusive();
            var wrapper = getTask.IsCompleted ? getTask.Result : await getTask;
            using (wrapper)
            {
                if (wrapper == null && EntitiesRepository != null)
                    throw new Exception($"{nameof(SetLogin)} GetExclusive wrapper is null");
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
                    var returnTask = SetLoginImpl(logined);
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

        public ValueTask<bool> CanBeUnloaded()
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return CanBeUnloadedRunMaster();
                else
                    return CanBeUnloadedRun();
            else
                return SendFuncs.CanBeUnloaded(this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId());
        }

        private Func<Task<bool>> CanBeUnloadedCreateDeferredDelegate() => () =>
        {
            return CanBeUnloaded().AsTask();
        }

        ;
        public async ValueTask<bool> CanBeUnloadedRunMaster()
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    return await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(CanBeUnloadedCreateDeferredDelegate(), nameof(CanBeUnloaded));
            }

            return await CanBeUnloadedRun();
        }

        public async ValueTask<bool> CanBeUnloadedRun()
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(SharedCode.Entities.IBakenCharacterEntity), 6);
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
                var returnTask = CanBeUnloadedImpl();
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