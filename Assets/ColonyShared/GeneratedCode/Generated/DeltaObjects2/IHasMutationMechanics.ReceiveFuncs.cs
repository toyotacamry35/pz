// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

using SharedCode.Network;
using SharedCode.Logging;
using System;
using System.Threading.Tasks;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.EntityPropertyResolvers;
using GeneratedCode.EntitySystem;
using SharedCode.Refs;

namespace GeneratedCode.DeltaObjects
{
    public partial class MutationMechanics
    {
        [SharedCode.EntitySystem.RpcClassHashAttribute(-487793267)]
        internal static class ReceiveFuncs
        {
            [SharedCode.EntitySystem.RpcMethodReceiverFunc(0, "IMutationMechanics_CanChangeMutation_Single_MutatingFactionDef_Message")]
            internal static async void IMutationMechanics_CanChangeMutation_Single_MutatingFactionDef_Message_Func(IEntitiesRepository _repository, INetworkProxy networkProxy, Guid callback, byte[] __data__, int __offset__, PropertyAddress __propertyAddress__, IDeltaObject __targetObj__, Guid __transactionId__, Guid __migrationId__)
            {
                try
                {
                    var serializer = ((IEntitiesRepositoryExtension)_repository).Serializer;
                    var value = serializer.Deserialize<float>(__data__, ref __offset__);
                    var toFaction = serializer.Deserialize<Assets.Src.Aspects.Impl.Factions.Template.MutatingFactionDef>(__data__, ref __offset__);
                    bool result;
                    using (GeneratedCode.Manual.AsyncStack.AsyncStackIsolator.IsolateContext())
                    {
                        GeneratedCode.Manual.Repositories.CallbackRepositoryHolder.CurrentCallbackRepositoryId = callback;
                        if (__migrationId__ != Guid.Empty)
                            GeneratedCode.Manual.Repositories.MigrationIdHolder.SetCurrentMigrationId(ref __migrationId__);
                        var __task = _repository.Get(__propertyAddress__.EntityTypeId, __propertyAddress__.EntityId);
                        if (__task.IsCompleted)
                            await System.Threading.Tasks.Task.Yield();
                        using (await __task)
                        {
                            var __entity__ = __targetObj__.To<SharedCode.Entities.IMutationMechanics>();
                            result = await __entity__.CanChangeMutation(value, toFaction);
                        }
                    }

                    int offset;
                    var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                    try
                    {
                        RpcHelper.FillResponseHeader(serializer, __buffer__, out offset, __transactionId__, 0, ref __migrationId__);
                        __buffer__ = serializer.Serialize(__buffer__, ref offset, result);
                        GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcNetworkStatistics>.Instance.SentResponseBytes(typeof(SharedCode.Entities.IMutationMechanics), 0, offset);
                        networkProxy.SendMessage(__buffer__, 0, offset, SharedCode.Network.MessageSendOptions.ReliableOrdered);
                    }
                    finally
                    {
                        EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                    }
                }
                catch (Exception e)
                {
                    RpcHelper.CheckAndSendTransactionException(networkProxy, __transactionId__, typeof(SharedCode.Entities.IMutationMechanics), 0, __propertyAddress__, e, ref __migrationId__);
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message(e, "RPC receiver exception").Write();
                }
            }

            [SharedCode.EntitySystem.RpcMethodReceiverFunc(1, "IMutationMechanics_ChangeMutation_Single_MutatingFactionDef_Single_Boolean_Message")]
            internal static async void IMutationMechanics_ChangeMutation_Single_MutatingFactionDef_Single_Boolean_Message_Func(IEntitiesRepository _repository, INetworkProxy networkProxy, Guid callback, byte[] __data__, int __offset__, PropertyAddress __propertyAddress__, IDeltaObject __targetObj__, Guid __transactionId__, Guid __migrationId__)
            {
                try
                {
                    var serializer = ((IEntitiesRepositoryExtension)_repository).Serializer;
                    var value = serializer.Deserialize<float>(__data__, ref __offset__);
                    var toFaction = serializer.Deserialize<Assets.Src.Aspects.Impl.Factions.Template.MutatingFactionDef>(__data__, ref __offset__);
                    var coolDownTime = serializer.Deserialize<float>(__data__, ref __offset__);
                    var forceChange = serializer.Deserialize<bool>(__data__, ref __offset__);
                    bool result;
                    using (GeneratedCode.Manual.AsyncStack.AsyncStackIsolator.IsolateContext())
                    {
                        GeneratedCode.Manual.Repositories.CallbackRepositoryHolder.CurrentCallbackRepositoryId = callback;
                        if (__migrationId__ != Guid.Empty)
                            GeneratedCode.Manual.Repositories.MigrationIdHolder.SetCurrentMigrationId(ref __migrationId__);
                        var __task = ((IEntitiesRepositoryExtension)_repository).GetExclusive(__propertyAddress__.EntityTypeId, __propertyAddress__.EntityId);
                        if (__task.IsCompleted)
                            await System.Threading.Tasks.Task.Yield();
                        using (await __task)
                        {
                            var __entity__ = __targetObj__.To<SharedCode.Entities.IMutationMechanics>();
                            result = await __entity__.ChangeMutation(value, toFaction, coolDownTime, forceChange);
                        }
                    }

                    int offset;
                    var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                    try
                    {
                        RpcHelper.FillResponseHeader(serializer, __buffer__, out offset, __transactionId__, 1, ref __migrationId__);
                        __buffer__ = serializer.Serialize(__buffer__, ref offset, result);
                        GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcNetworkStatistics>.Instance.SentResponseBytes(typeof(SharedCode.Entities.IMutationMechanics), 1, offset);
                        networkProxy.SendMessage(__buffer__, 0, offset, SharedCode.Network.MessageSendOptions.ReliableOrdered);
                    }
                    finally
                    {
                        EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                    }
                }
                catch (Exception e)
                {
                    RpcHelper.CheckAndSendTransactionException(networkProxy, __transactionId__, typeof(SharedCode.Entities.IMutationMechanics), 1, __propertyAddress__, e, ref __migrationId__);
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message(e, "RPC receiver exception").Write();
                }
            }

            [SharedCode.EntitySystem.RpcMethodReceiverFunc(2, "IMutationMechanics_ApplyMutationChangeForced_MutationStageDef_MutatingFactionDef_Message")]
            internal static async void IMutationMechanics_ApplyMutationChangeForced_MutationStageDef_MutatingFactionDef_Message_Func(IEntitiesRepository _repository, INetworkProxy networkProxy, Guid callback, byte[] __data__, int __offset__, PropertyAddress __propertyAddress__, IDeltaObject __targetObj__, Guid __transactionId__, Guid __migrationId__)
            {
                try
                {
                    var serializer = ((IEntitiesRepositoryExtension)_repository).Serializer;
                    var newStage = serializer.Deserialize<Assets.Src.Aspects.Impl.Factions.Template.MutationStageDef>(__data__, ref __offset__);
                    var newFaction = serializer.Deserialize<Assets.Src.Aspects.Impl.Factions.Template.MutatingFactionDef>(__data__, ref __offset__);
                    bool result;
                    using (GeneratedCode.Manual.AsyncStack.AsyncStackIsolator.IsolateContext())
                    {
                        GeneratedCode.Manual.Repositories.CallbackRepositoryHolder.CurrentCallbackRepositoryId = callback;
                        if (__migrationId__ != Guid.Empty)
                            GeneratedCode.Manual.Repositories.MigrationIdHolder.SetCurrentMigrationId(ref __migrationId__);
                        var __task = ((IEntitiesRepositoryExtension)_repository).GetExclusive(__propertyAddress__.EntityTypeId, __propertyAddress__.EntityId);
                        if (__task.IsCompleted)
                            await System.Threading.Tasks.Task.Yield();
                        using (await __task)
                        {
                            var __entity__ = __targetObj__.To<SharedCode.Entities.IMutationMechanics>();
                            result = await __entity__.ApplyMutationChangeForced(newStage, newFaction);
                        }
                    }

                    int offset;
                    var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                    try
                    {
                        RpcHelper.FillResponseHeader(serializer, __buffer__, out offset, __transactionId__, 2, ref __migrationId__);
                        __buffer__ = serializer.Serialize(__buffer__, ref offset, result);
                        GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcNetworkStatistics>.Instance.SentResponseBytes(typeof(SharedCode.Entities.IMutationMechanics), 2, offset);
                        networkProxy.SendMessage(__buffer__, 0, offset, SharedCode.Network.MessageSendOptions.ReliableOrdered);
                    }
                    finally
                    {
                        EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                    }
                }
                catch (Exception e)
                {
                    RpcHelper.CheckAndSendTransactionException(networkProxy, __transactionId__, typeof(SharedCode.Entities.IMutationMechanics), 2, __propertyAddress__, e, ref __migrationId__);
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message(e, "RPC receiver exception").Write();
                }
            }
        }
    }
}