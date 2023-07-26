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
    public partial class UnityCheatServiceEntity
    {
        [SharedCode.EntitySystem.RpcClassHashAttribute(1434780675)]
        internal static class ReceiveFuncs
        {
            [SharedCode.EntitySystem.RpcMethodReceiverFunc(0, "IUnityCheatServiceEntity_MainUnityThreadOnServerSleep_Boolean_Single_Single_Single_Message")]
            internal static async void IUnityCheatServiceEntity_MainUnityThreadOnServerSleep_Boolean_Single_Single_Single_Message_Func(IEntitiesRepository _repository, INetworkProxy networkProxy, Guid callback, byte[] __data__, int __offset__, PropertyAddress __propertyAddress__, IDeltaObject __targetObj__, Guid _, Guid __migrationId__)
            {
                try
                {
                    var serializer = ((IEntitiesRepositoryExtension)_repository).Serializer;
                    var isOn = serializer.Deserialize<bool>(__data__, ref __offset__);
                    var sleepTime = serializer.Deserialize<float>(__data__, ref __offset__);
                    var delayBeforeSleep = serializer.Deserialize<float>(__data__, ref __offset__);
                    var repeatTime = serializer.Deserialize<float>(__data__, ref __offset__);
                    using (GeneratedCode.Manual.AsyncStack.AsyncStackIsolator.IsolateContext())
                    {
                        GeneratedCode.Manual.Repositories.CallbackRepositoryHolder.CurrentCallbackRepositoryId = callback;
                        if (__migrationId__ != Guid.Empty)
                            GeneratedCode.Manual.Repositories.MigrationIdHolder.SetCurrentMigrationId(ref __migrationId__);
                        await System.Threading.Tasks.Task.Yield();
                        var __entityRef__ = ((IEntityRefExt)((IEntitiesRepositoryExtension)_repository).GetRef(__propertyAddress__.EntityTypeId, __propertyAddress__.EntityId));
                        var __entity__ = __targetObj__.To<Assets.ColonyShared.SharedCode.Entities.Service.IUnityCheatServiceEntity>();
                        await __entity__.MainUnityThreadOnServerSleep(isOn, sleepTime, delayBeforeSleep, repeatTime);
                    }
                }
                catch (Exception e)
                {
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message(e, "RPC receiver exception").Write();
                }
            }

            [SharedCode.EntitySystem.RpcMethodReceiverFunc(1, "IUnityCheatServiceEntity_SetCurveLoggerState_Boolean_Boolean_String_Guid_Message")]
            internal static async void IUnityCheatServiceEntity_SetCurveLoggerState_Boolean_Boolean_String_Guid_Message_Func(IEntitiesRepository _repository, INetworkProxy networkProxy, Guid callback, byte[] __data__, int __offset__, PropertyAddress __propertyAddress__, IDeltaObject __targetObj__, Guid _, Guid __migrationId__)
            {
                try
                {
                    var serializer = ((IEntitiesRepositoryExtension)_repository).Serializer;
                    var enabledStatus = serializer.Deserialize<bool>(__data__, ref __offset__);
                    var dump = serializer.Deserialize<bool>(__data__, ref __offset__);
                    var loggerName = serializer.Deserialize<string>(__data__, ref __offset__);
                    var dumpId = serializer.Deserialize<System.Guid>(__data__, ref __offset__);
                    using (GeneratedCode.Manual.AsyncStack.AsyncStackIsolator.IsolateContext())
                    {
                        GeneratedCode.Manual.Repositories.CallbackRepositoryHolder.CurrentCallbackRepositoryId = callback;
                        if (__migrationId__ != Guid.Empty)
                            GeneratedCode.Manual.Repositories.MigrationIdHolder.SetCurrentMigrationId(ref __migrationId__);
                        await System.Threading.Tasks.Task.Yield();
                        var __entityRef__ = ((IEntityRefExt)((IEntitiesRepositoryExtension)_repository).GetRef(__propertyAddress__.EntityTypeId, __propertyAddress__.EntityId));
                        var __entity__ = __targetObj__.To<Assets.ColonyShared.SharedCode.Entities.Service.IUnityCheatServiceEntity>();
                        await __entity__.SetCurveLoggerState(enabledStatus, dump, loggerName, dumpId);
                    }
                }
                catch (Exception e)
                {
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message(e, "RPC receiver exception").Write();
                }
            }

            [SharedCode.EntitySystem.RpcMethodReceiverFunc(2, "IUnityCheatServiceEntity_GetClosestPlayerSpawnPointTransform_Vector3_Message")]
            internal static async void IUnityCheatServiceEntity_GetClosestPlayerSpawnPointTransform_Vector3_Message_Func(IEntitiesRepository _repository, INetworkProxy networkProxy, Guid callback, byte[] __data__, int __offset__, PropertyAddress __propertyAddress__, IDeltaObject __targetObj__, Guid __transactionId__, Guid __migrationId__)
            {
                try
                {
                    var serializer = ((IEntitiesRepositoryExtension)_repository).Serializer;
                    var pos = serializer.Deserialize<SharedCode.Utils.Vector3>(__data__, ref __offset__);
                    SharedCode.Entities.Transform result;
                    using (GeneratedCode.Manual.AsyncStack.AsyncStackIsolator.IsolateContext())
                    {
                        GeneratedCode.Manual.Repositories.CallbackRepositoryHolder.CurrentCallbackRepositoryId = callback;
                        if (__migrationId__ != Guid.Empty)
                            GeneratedCode.Manual.Repositories.MigrationIdHolder.SetCurrentMigrationId(ref __migrationId__);
                        await System.Threading.Tasks.Task.Yield();
                        var __entityRef__ = ((IEntityRefExt)((IEntitiesRepositoryExtension)_repository).GetRef(__propertyAddress__.EntityTypeId, __propertyAddress__.EntityId));
                        var __entity__ = __targetObj__.To<Assets.ColonyShared.SharedCode.Entities.Service.IUnityCheatServiceEntity>();
                        result = await __entity__.GetClosestPlayerSpawnPointTransform(pos);
                    }

                    int offset;
                    var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                    try
                    {
                        RpcHelper.FillResponseHeader(serializer, __buffer__, out offset, __transactionId__, 2, ref __migrationId__);
                        __buffer__ = serializer.Serialize(__buffer__, ref offset, result);
                        GeneratedCode.Network.Statistic.Statistics<GeneratedCode.Network.Statistic.RpcNetworkStatistics>.Instance.SentResponseBytes(typeof(Assets.ColonyShared.SharedCode.Entities.Service.IUnityCheatServiceEntity), 2, offset);
                        networkProxy.SendMessage(__buffer__, 0, offset, SharedCode.Network.MessageSendOptions.ReliableOrdered);
                    }
                    finally
                    {
                        EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                    }
                }
                catch (Exception e)
                {
                    RpcHelper.CheckAndSendTransactionException(networkProxy, __transactionId__, typeof(Assets.ColonyShared.SharedCode.Entities.Service.IUnityCheatServiceEntity), 2, __propertyAddress__, e, ref __migrationId__);
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message(e, "RPC receiver exception").Write();
                }
            }
        }
    }
}