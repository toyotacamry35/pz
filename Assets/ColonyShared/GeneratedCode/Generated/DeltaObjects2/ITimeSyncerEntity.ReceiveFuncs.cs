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
    public partial class TimeSyncerEntity
    {
        [SharedCode.EntitySystem.RpcClassHashAttribute(504017995)]
        internal static class ReceiveFuncs
        {
            [SharedCode.EntitySystem.RpcMethodReceiverFunc(0, "ITimeSyncerEntity_UpdateTime__Message")]
            internal static async void ITimeSyncerEntity_UpdateTime__Message_Func(IEntitiesRepository _repository, INetworkProxy networkProxy, Guid callback, byte[] __data__, int __offset__, PropertyAddress __propertyAddress__, IDeltaObject __targetObj__, Guid __transactionId__, Guid __migrationId__)
            {
                try
                {
                    var serializer = ((IEntitiesRepositoryExtension)_repository).Serializer;
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
                            var __entity__ = __targetObj__.To<SharedCode.Entities.ITimeSyncerEntity>();
                            result = await __entity__.UpdateTime();
                        }
                    }

                    int offset;
                    var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                    try
                    {
                        RpcHelper.FillResponseHeader(serializer, __buffer__, out offset, __transactionId__, 0, ref __migrationId__);
                        __buffer__ = serializer.Serialize(__buffer__, ref offset, result);
                        GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcNetworkStatistics>.Instance.SentResponseBytes(typeof(SharedCode.Entities.ITimeSyncerEntity), 0, offset);
                        networkProxy.SendMessage(__buffer__, 0, offset, SharedCode.Network.MessageSendOptions.ReliableOrdered);
                    }
                    finally
                    {
                        EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                    }
                }
                catch (Exception e)
                {
                    RpcHelper.CheckAndSendTransactionException(networkProxy, __transactionId__, typeof(SharedCode.Entities.ITimeSyncerEntity), 0, __propertyAddress__, e, ref __migrationId__);
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message(e, "RPC receiver exception").Write();
                }
            }

            [SharedCode.EntitySystem.RpcMethodReceiverFunc(1, "ITimeSyncerEntity_Ping__Message")]
            internal static async void ITimeSyncerEntity_Ping__Message_Func(IEntitiesRepository _repository, INetworkProxy networkProxy, Guid callback, byte[] __data__, int __offset__, PropertyAddress __propertyAddress__, IDeltaObject __targetObj__, Guid __transactionId__, Guid __migrationId__)
            {
                try
                {
                    var serializer = ((IEntitiesRepositoryExtension)_repository).Serializer;
                    long result;
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
                            var __entity__ = __targetObj__.To<SharedCode.Entities.ITimeSyncerEntity>();
                            result = await __entity__.Ping();
                        }
                    }

                    int offset;
                    var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                    try
                    {
                        RpcHelper.FillResponseHeader(serializer, __buffer__, out offset, __transactionId__, 1, ref __migrationId__);
                        __buffer__ = serializer.Serialize(__buffer__, ref offset, result);
                        GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcNetworkStatistics>.Instance.SentResponseBytes(typeof(SharedCode.Entities.ITimeSyncerEntity), 1, offset);
                        networkProxy.SendMessage(__buffer__, 0, offset, SharedCode.Network.MessageSendOptions.ReliableOrdered);
                    }
                    finally
                    {
                        EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                    }
                }
                catch (Exception e)
                {
                    RpcHelper.CheckAndSendTransactionException(networkProxy, __transactionId__, typeof(SharedCode.Entities.ITimeSyncerEntity), 1, __propertyAddress__, e, ref __migrationId__);
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message(e, "RPC receiver exception").Write();
                }
            }
        }
    }
}