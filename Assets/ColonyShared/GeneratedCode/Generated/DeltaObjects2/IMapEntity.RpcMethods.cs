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
    public partial class MapEntity
    {
        public Task<bool> SetMapEntityState(SharedCode.MapSystem.MapEntityState state)
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return SetMapEntityStateRunMaster(state).AsTask();
                else
                    return SetMapEntityStateRun(state).AsTask();
            else
                return SendFuncs.SetMapEntityState(state, this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId()).AsTask();
        }

        private Func<Task<bool>> SetMapEntityStateCreateDeferredDelegate(SharedCode.MapSystem.MapEntityState state) => () =>
        {
            return SetMapEntityState(state);
        }

        ;
        public async ValueTask<bool> SetMapEntityStateRunMaster(SharedCode.MapSystem.MapEntityState state)
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    return await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(SetMapEntityStateCreateDeferredDelegate(state), nameof(SetMapEntityState));
            }

            return await SetMapEntityStateRun(state);
        }

        public async ValueTask<bool> SetMapEntityStateRun(SharedCode.MapSystem.MapEntityState state)
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(SharedCode.MapSystem.IMapEntity), 0);
            var getTask = this.GetThisExclusive();
            var wrapper = getTask.IsCompleted ? getTask.Result : await getTask;
            using (wrapper)
            {
                if (wrapper == null && EntitiesRepository != null)
                    throw new Exception($"{nameof(SetMapEntityState)} GetExclusive wrapper is null");
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
                    var returnTask = SetMapEntityStateImpl(state);
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

        public Task<bool> ChangeChunkDescription(System.Guid descriptionId, SharedCode.MapSystem.MapChunkState newState, System.Guid unityRepositoryId)
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return ChangeChunkDescriptionRunMaster(descriptionId, newState, unityRepositoryId).AsTask();
                else
                    return ChangeChunkDescriptionRun(descriptionId, newState, unityRepositoryId).AsTask();
            else
                return SendFuncs.ChangeChunkDescription(descriptionId, newState, unityRepositoryId, this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId()).AsTask();
        }

        private Func<Task<bool>> ChangeChunkDescriptionCreateDeferredDelegate(System.Guid descriptionId, SharedCode.MapSystem.MapChunkState newState, System.Guid unityRepositoryId) => () =>
        {
            return ChangeChunkDescription(descriptionId, newState, unityRepositoryId);
        }

        ;
        public async ValueTask<bool> ChangeChunkDescriptionRunMaster(System.Guid descriptionId, SharedCode.MapSystem.MapChunkState newState, System.Guid unityRepositoryId)
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    return await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(ChangeChunkDescriptionCreateDeferredDelegate(descriptionId, newState, unityRepositoryId), nameof(ChangeChunkDescription));
            }

            return await ChangeChunkDescriptionRun(descriptionId, newState, unityRepositoryId);
        }

        public async ValueTask<bool> ChangeChunkDescriptionRun(System.Guid descriptionId, SharedCode.MapSystem.MapChunkState newState, System.Guid unityRepositoryId)
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(SharedCode.MapSystem.IMapEntity), 1);
            var getTask = this.GetThisExclusive();
            var wrapper = getTask.IsCompleted ? getTask.Result : await getTask;
            using (wrapper)
            {
                if (wrapper == null && EntitiesRepository != null)
                    throw new Exception($"{nameof(ChangeChunkDescription)} GetExclusive wrapper is null");
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
                    var returnTask = ChangeChunkDescriptionImpl(descriptionId, newState, unityRepositoryId);
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

        public Task<bool> OnLastUserLeft()
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return OnLastUserLeftRunMaster().AsTask();
                else
                    return OnLastUserLeftRun().AsTask();
            else
                return SendFuncs.OnLastUserLeft(this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId()).AsTask();
        }

        private Func<Task<bool>> OnLastUserLeftCreateDeferredDelegate() => () =>
        {
            return OnLastUserLeft();
        }

        ;
        public async ValueTask<bool> OnLastUserLeftRunMaster()
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    return await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(OnLastUserLeftCreateDeferredDelegate(), nameof(OnLastUserLeft));
            }

            return await OnLastUserLeftRun();
        }

        public async ValueTask<bool> OnLastUserLeftRun()
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(SharedCode.MapSystem.IMapEntity), 2);
            var getTask = this.GetThisExclusive();
            var wrapper = getTask.IsCompleted ? getTask.Result : await getTask;
            using (wrapper)
            {
                if (wrapper == null && EntitiesRepository != null)
                    throw new Exception($"{nameof(OnLastUserLeft)} GetExclusive wrapper is null");
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
                    var returnTask = OnLastUserLeftImpl();
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

        public Task SpawnNewBots(System.Collections.Generic.List<System.Guid> botIds, string spawnPointTypePath)
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return SpawnNewBotsRunMaster(botIds, spawnPointTypePath).AsTask();
                else
                    return SpawnNewBotsRun(botIds, spawnPointTypePath).AsTask();
            else
                return SendFuncs.SpawnNewBots(botIds, spawnPointTypePath, this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId()).AsTask();
        }

        private Func<Task> SpawnNewBotsCreateDeferredDelegate(System.Collections.Generic.List<System.Guid> botIds, string spawnPointTypePath) => () =>
        {
            return SpawnNewBots(botIds, spawnPointTypePath);
        }

        ;
        public async ValueTask SpawnNewBotsRunMaster(System.Collections.Generic.List<System.Guid> botIds, string spawnPointTypePath)
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(SpawnNewBotsCreateDeferredDelegate(botIds, spawnPointTypePath), nameof(SpawnNewBots));
            }

            await SpawnNewBotsRun(botIds, spawnPointTypePath);
        }

        public async ValueTask SpawnNewBotsRun(System.Collections.Generic.List<System.Guid> botIds, string spawnPointTypePath)
        {
            if (!await global::GeneratedCode.Shared.Utils.AccountTypeUtils.CheckAccountType(GeneratedCode.Manual.Repositories.CallbackRepositoryHolder.CurrentCallbackRepositoryId, SharedCode.Entities.Service.AccountType.TechnicalSupport, EntitiesRepository))
                throw new System.UnauthorizedAccessException(string.Format("User {0} has no rights to use cheat {1}", GeneratedCode.Manual.Repositories.CallbackRepositoryHolder.CurrentCallbackRepositoryId, nameof(SpawnNewBots)));
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(SharedCode.MapSystem.IMapEntity), 3);
            GeneratedCode.Manual.Repositories.RpcCurrentObject.ThisObj = this;
            Guid __oldMigrationId__ = default;
            var __needSetMigrationgId__ = MigratingId != Guid.Empty;
            if (__needSetMigrationgId__)
            {
                __oldMigrationId__ = global::GeneratedCode.Manual.Repositories.MigrationIdHolder.CurrentMigrationId;
                global::GeneratedCode.Manual.Repositories.MigrationIdHolder.SetCurrentMigrationId(ref MigratingId);
            }

            try
            {
                var returnTask = SpawnNewBotsImpl(botIds, spawnPointTypePath);
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
                    global::GeneratedCode.Manual.Repositories.MigrationIdHolder.SetCurrentMigrationId(ref __oldMigrationId__);
            }
        }

        public Task<bool> TryAquireSpawnRightsForPointsSet(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> spawner, SharedCode.Entities.GameObjectEntities.SceneChunkDef mapSceneDef)
        {
            this.CheckValidateEntityInAsyncContext();
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return TryAquireSpawnRightsForPointsSetRunMaster(spawner, mapSceneDef).AsTask();
                else
                    return TryAquireSpawnRightsForPointsSetRun(spawner, mapSceneDef).AsTask();
            else
                return SendFuncs.TryAquireSpawnRightsForPointsSet(spawner, mapSceneDef, this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId()).AsTask();
        }

        private Func<Task<bool>> TryAquireSpawnRightsForPointsSetCreateDeferredDelegate(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> spawner, SharedCode.Entities.GameObjectEntities.SceneChunkDef mapSceneDef) => () =>
        {
            return TryAquireSpawnRightsForPointsSet(spawner, mapSceneDef);
        }

        ;
        public async ValueTask<bool> TryAquireSpawnRightsForPointsSetRunMaster(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> spawner, SharedCode.Entities.GameObjectEntities.SceneChunkDef mapSceneDef)
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    return await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(TryAquireSpawnRightsForPointsSetCreateDeferredDelegate(spawner, mapSceneDef), nameof(TryAquireSpawnRightsForPointsSet));
            }

            return await TryAquireSpawnRightsForPointsSetRun(spawner, mapSceneDef);
        }

        public async ValueTask<bool> TryAquireSpawnRightsForPointsSetRun(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> spawner, SharedCode.Entities.GameObjectEntities.SceneChunkDef mapSceneDef)
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(SharedCode.MapSystem.IMapEntity), 4);
            var getTask = this.GetThisExclusive();
            var wrapper = getTask.IsCompleted ? getTask.Result : await getTask;
            using (wrapper)
            {
                if (wrapper == null && EntitiesRepository != null)
                    throw new Exception($"{nameof(TryAquireSpawnRightsForPointsSet)} GetExclusive wrapper is null");
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
                    var returnTask = TryAquireSpawnRightsForPointsSetImpl(spawner, mapSceneDef);
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

        public Task<SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>> GetWorldSpaceForPoint(SharedCode.Utils.Vector3 point)
        {
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return GetWorldSpaceForPointRunMaster(point).AsTask();
                else
                    return GetWorldSpaceForPointRun(point).AsTask();
            else
                return SendFuncs.GetWorldSpaceForPoint(point, this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId()).AsTask();
        }

        private Func<Task<SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>>> GetWorldSpaceForPointCreateDeferredDelegate(SharedCode.Utils.Vector3 point) => () =>
        {
            return GetWorldSpaceForPoint(point);
        }

        ;
        public async ValueTask<SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>> GetWorldSpaceForPointRunMaster(SharedCode.Utils.Vector3 point)
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    return await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(GetWorldSpaceForPointCreateDeferredDelegate(point), nameof(GetWorldSpaceForPoint));
            }

            return await GetWorldSpaceForPointRun(point);
        }

        public async ValueTask<SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>> GetWorldSpaceForPointRun(SharedCode.Utils.Vector3 point)
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(SharedCode.MapSystem.IMapEntity), 5);
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
                var returnTask = GetWorldSpaceForPointImpl(point);
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

        public Task<bool> NotifyAllCharactersViaChat(string text)
        {
            if (this.IsMaster())
                if (this.NeedDeferredRpcOnMigrating())
                    return NotifyAllCharactersViaChatRunMaster(text).AsTask();
                else
                    return NotifyAllCharactersViaChatRun(text).AsTask();
            else
                return SendFuncs.NotifyAllCharactersViaChat(text, this, this.GetNetworkProxyForSerialization(), this.EntitiesRepository, GetActualMigratingId()).AsTask();
        }

        private Func<Task<bool>> NotifyAllCharactersViaChatCreateDeferredDelegate(string text) => () =>
        {
            return NotifyAllCharactersViaChat(text);
        }

        ;
        public async ValueTask<bool> NotifyAllCharactersViaChatRunMaster(string text)
        {
            if (parentEntity != null)
            {
                var __checkTask__ = ((IEntityExt)parentEntity).NeedPutToDeferredRpcQueue();
                if (!__checkTask__.IsCompleted)
                    await __checkTask__;
                if (__checkTask__.Result)
                    return await ((IEntityExt)parentEntity).AddDeferredMigratingRpc(NotifyAllCharactersViaChatCreateDeferredDelegate(text), nameof(NotifyAllCharactersViaChat));
            }

            return await NotifyAllCharactersViaChatRun(text);
        }

        public async ValueTask<bool> NotifyAllCharactersViaChatRun(string text)
        {
            GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(SharedCode.MapSystem.IMapEntity), 6);
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
                var returnTask = NotifyAllCharactersViaChatImpl(text);
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

        public event System.Func<string, string, System.Threading.Tasks.Task> NewChatMessageEvent;
        public async Task OnNewChatMessageEventInvoke(string arg1, string arg2)
        {
            if (NewChatMessageEvent == null)
                return;
            foreach (var subscriber in NewChatMessageEvent.GetInvocationList().Cast<System.Func<string, string, System.Threading.Tasks.Task>>())
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
                            Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("{0} process event obj {1} method {2} timeout: {3}", nameof(NewChatMessageEvent), subscriber.Target?.GetType().Name ?? "unknown", subscriber?.Method.Name ?? "unknown", ServerCoreRuntimeParameters.EntityEventTimeoutSeconds).Write();
                            var __sw__ = new System.Diagnostics.Stopwatch();
                            __sw__.Start();
                            await __subscriberTask__;
                            __sw__.Stop();
                            Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("{0} process event obj {1} method {2} executing too long: {3} seconds", nameof(NewChatMessageEvent), subscriber.Target?.GetType().Name ?? "unknown", subscriber?.Method.Name ?? "unknown", ServerCoreRuntimeParameters.EntityEventTimeoutSeconds + __sw__.Elapsed.TotalSeconds).Write();
                        }
                        else
                        {
                            cts.Cancel();
                            if (__subscriberTask__.IsFaulted)
                                Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message(__subscriberTask__.Exception, "Exception in {0} process event obj {1} method {2}", nameof(NewChatMessageEvent), subscriber.Target?.GetType().Name ?? "unknown", subscriber?.Method.Name ?? "unknown").Write();
                        }
                    }
                }
                catch (Exception e)
                {
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message(e, "Exception in {0} event obj {1} method {2}", nameof(NewChatMessageEvent), subscriber?.Target?.GetType().Name ?? "unknown", subscriber?.Method.Name ?? "unknown").Write();
                }
                finally
                {
                    GeneratedCode.Manual.Repositories.AsyncStackHolder.AssertNoChildren();
                }
            }
        }

        public async Task OnNewChatMessageEvent(string arg1, string arg2)
        {
            if (this.IsMaster())
            {
                GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcInnerStatistics>.Instance.Used(typeof(SharedCode.MapSystem.IMapEntity), 7);
                INetworkProxy[] __networkProxies__ = this.GatherMessageTargets(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, typeof(SharedCode.MapSystem.IMapEntity), 7).ToArray();
                if (__networkProxies__.Length > 0)
                    await SendFuncs.OnNewChatMessageEvent(arg1, arg2, this, __networkProxies__, this.EntitiesRepository, GetActualMigratingId());
            }

            await OnNewChatMessageEventInvoke(arg1, arg2);
        }
    }
}