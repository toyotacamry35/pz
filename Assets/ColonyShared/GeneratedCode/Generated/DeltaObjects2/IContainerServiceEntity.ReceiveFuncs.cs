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
    public partial class ContainerServiceEntity
    {
        [SharedCode.EntitySystem.RpcClassHashAttribute(1631350192)]
        internal static class ReceiveFuncs
        {
            [SharedCode.EntitySystem.RpcMethodReceiverFunc(0, "IContainerServiceEntity_MoveItem_PropertyAddress_Int32_PropertyAddress_Int32_Int32_Guid_Message")]
            internal static async void IContainerServiceEntity_MoveItem_PropertyAddress_Int32_PropertyAddress_Int32_Int32_Guid_Message_Func(IEntitiesRepository _repository, INetworkProxy networkProxy, Guid callback, byte[] __data__, int __offset__, PropertyAddress __propertyAddress__, IDeltaObject __targetObj__, Guid __transactionId__, Guid __migrationId__)
            {
                try
                {
                    var serializer = ((IEntitiesRepositoryExtension)_repository).Serializer;
                    var source = serializer.Deserialize<SharedCode.EntitySystem.PropertyAddress>(__data__, ref __offset__);
                    var sourceSlotId = serializer.Deserialize<int>(__data__, ref __offset__);
                    var destination = serializer.Deserialize<SharedCode.EntitySystem.PropertyAddress>(__data__, ref __offset__);
                    var destinationSlotId = serializer.Deserialize<int>(__data__, ref __offset__);
                    var count = serializer.Deserialize<int>(__data__, ref __offset__);
                    var clientSrcEntityId = serializer.Deserialize<System.Guid>(__data__, ref __offset__);
                    Assets.ColonyShared.SharedCode.Aspects.Item.ContainerItemOperation result;
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
                            var __entity__ = __targetObj__.To<SharedCode.Entities.Service.IContainerServiceEntity>();
                            result = await __entity__.MoveItem(source, sourceSlotId, destination, destinationSlotId, count, clientSrcEntityId);
                        }
                    }

                    int offset;
                    var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                    try
                    {
                        RpcHelper.FillResponseHeader(serializer, __buffer__, out offset, __transactionId__, 0, ref __migrationId__);
                        __buffer__ = serializer.Serialize(__buffer__, ref offset, result);
                        GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcNetworkStatistics>.Instance.SentResponseBytes(typeof(SharedCode.Entities.Service.IContainerServiceEntity), 0, offset);
                        networkProxy.SendMessage(__buffer__, 0, offset, SharedCode.Network.MessageSendOptions.ReliableOrdered);
                    }
                    finally
                    {
                        EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                    }
                }
                catch (Exception e)
                {
                    RpcHelper.CheckAndSendTransactionException(networkProxy, __transactionId__, typeof(SharedCode.Entities.Service.IContainerServiceEntity), 0, __propertyAddress__, e, ref __migrationId__);
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message(e, "RPC receiver exception").Write();
                }
            }

            [SharedCode.EntitySystem.RpcMethodReceiverFunc(1, "IContainerServiceEntity_RemoveItem_PropertyAddress_Int32_Int32_Guid_Message")]
            internal static async void IContainerServiceEntity_RemoveItem_PropertyAddress_Int32_Int32_Guid_Message_Func(IEntitiesRepository _repository, INetworkProxy networkProxy, Guid callback, byte[] __data__, int __offset__, PropertyAddress __propertyAddress__, IDeltaObject __targetObj__, Guid __transactionId__, Guid __migrationId__)
            {
                try
                {
                    var serializer = ((IEntitiesRepositoryExtension)_repository).Serializer;
                    var source = serializer.Deserialize<SharedCode.EntitySystem.PropertyAddress>(__data__, ref __offset__);
                    var sourceSlotId = serializer.Deserialize<int>(__data__, ref __offset__);
                    var count = serializer.Deserialize<int>(__data__, ref __offset__);
                    var clientEntityId = serializer.Deserialize<System.Guid>(__data__, ref __offset__);
                    Assets.ColonyShared.SharedCode.Aspects.Item.ContainerItemOperation result;
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
                            var __entity__ = __targetObj__.To<SharedCode.Entities.Service.IContainerServiceEntity>();
                            result = await __entity__.RemoveItem(source, sourceSlotId, count, clientEntityId);
                        }
                    }

                    int offset;
                    var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                    try
                    {
                        RpcHelper.FillResponseHeader(serializer, __buffer__, out offset, __transactionId__, 1, ref __migrationId__);
                        __buffer__ = serializer.Serialize(__buffer__, ref offset, result);
                        GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcNetworkStatistics>.Instance.SentResponseBytes(typeof(SharedCode.Entities.Service.IContainerServiceEntity), 1, offset);
                        networkProxy.SendMessage(__buffer__, 0, offset, SharedCode.Network.MessageSendOptions.ReliableOrdered);
                    }
                    finally
                    {
                        EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                    }
                }
                catch (Exception e)
                {
                    RpcHelper.CheckAndSendTransactionException(networkProxy, __transactionId__, typeof(SharedCode.Entities.Service.IContainerServiceEntity), 1, __propertyAddress__, e, ref __migrationId__);
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message(e, "RPC receiver exception").Write();
                }
            }
        }
    }
}