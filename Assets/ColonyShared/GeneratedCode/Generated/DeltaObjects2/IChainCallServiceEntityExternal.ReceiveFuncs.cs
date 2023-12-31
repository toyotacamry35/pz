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
    public partial class ChainCallServiceEntityExternal
    {
        [SharedCode.EntitySystem.RpcClassHashAttribute(1520658228)]
        internal static class ReceiveFuncs
        {
            [SharedCode.EntitySystem.RpcMethodReceiverFunc(0, "IChainCallServiceEntityExternal_ChainCall_EntityMethodsCallsChainBatch_Message")]
            internal static async void IChainCallServiceEntityExternal_ChainCall_EntityMethodsCallsChainBatch_Message_Func(IEntitiesRepository _repository, INetworkProxy networkProxy, Guid callback, byte[] __data__, int __offset__, PropertyAddress __propertyAddress__, IDeltaObject __targetObj__, Guid _, Guid __migrationId__)
            {
                try
                {
                    var serializer = ((IEntitiesRepositoryExtension)_repository).Serializer;
                    var batch = serializer.Deserialize<SharedCode.EntitySystem.ChainCalls.EntityMethodsCallsChainBatch>(__data__, ref __offset__);
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
                            var __entity__ = __targetObj__.To<SharedCode.Entities.Service.IChainCallServiceEntityExternal>();
                            await __entity__.ChainCall(batch);
                        }
                    }
                }
                catch (Exception e)
                {
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message(e, "RPC receiver exception").Write();
                }
            }

            [SharedCode.EntitySystem.RpcMethodReceiverFunc(1, "IChainCallServiceEntityExternal_CancelChain_Int32_Guid_Guid_Message")]
            internal static async void IChainCallServiceEntityExternal_CancelChain_Int32_Guid_Guid_Message_Func(IEntitiesRepository _repository, INetworkProxy networkProxy, Guid callback, byte[] __data__, int __offset__, PropertyAddress __propertyAddress__, IDeltaObject __targetObj__, Guid _, Guid __migrationId__)
            {
                try
                {
                    var serializer = ((IEntitiesRepositoryExtension)_repository).Serializer;
                    var typeId = serializer.Deserialize<int>(__data__, ref __offset__);
                    var entityId = serializer.Deserialize<System.Guid>(__data__, ref __offset__);
                    var chainId = serializer.Deserialize<System.Guid>(__data__, ref __offset__);
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
                            var __entity__ = __targetObj__.To<SharedCode.Entities.Service.IChainCallServiceEntityExternal>();
                            await __entity__.CancelChain(typeId, entityId, chainId);
                        }
                    }
                }
                catch (Exception e)
                {
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message(e, "RPC receiver exception").Write();
                }
            }

            [SharedCode.EntitySystem.RpcMethodReceiverFunc(2, "IChainCallServiceEntityExternal_CancelAllChain_Int32_Guid_Message")]
            internal static async void IChainCallServiceEntityExternal_CancelAllChain_Int32_Guid_Message_Func(IEntitiesRepository _repository, INetworkProxy networkProxy, Guid callback, byte[] __data__, int __offset__, PropertyAddress __propertyAddress__, IDeltaObject __targetObj__, Guid __transactionId__, Guid __migrationId__)
            {
                try
                {
                    var serializer = ((IEntitiesRepositoryExtension)_repository).Serializer;
                    var typeId = serializer.Deserialize<int>(__data__, ref __offset__);
                    var entityId = serializer.Deserialize<System.Guid>(__data__, ref __offset__);
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
                            var __entity__ = __targetObj__.To<SharedCode.Entities.Service.IChainCallServiceEntityExternal>();
                            result = await __entity__.CancelAllChain(typeId, entityId);
                        }
                    }

                    int offset;
                    var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                    try
                    {
                        RpcHelper.FillResponseHeader(serializer, __buffer__, out offset, __transactionId__, 2, ref __migrationId__);
                        __buffer__ = serializer.Serialize(__buffer__, ref offset, result);
                        GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcNetworkStatistics>.Instance.SentResponseBytes(typeof(SharedCode.Entities.Service.IChainCallServiceEntityExternal), 2, offset);
                        networkProxy.SendMessage(__buffer__, 0, offset, SharedCode.Network.MessageSendOptions.ReliableOrdered);
                    }
                    finally
                    {
                        EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                    }
                }
                catch (Exception e)
                {
                    RpcHelper.CheckAndSendTransactionException(networkProxy, __transactionId__, typeof(SharedCode.Entities.Service.IChainCallServiceEntityExternal), 2, __propertyAddress__, e, ref __migrationId__);
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message(e, "RPC receiver exception").Write();
                }
            }
        }
    }
}