// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

using System;
using SharedCode.Network;
using System.Threading.Tasks;
using GeneratedCode.EntitySystem;
using SharedCode.EntitySystem;
using System.Collections.Generic;

namespace GeneratedCode.DeltaObjects
{
    public partial class UnityCheatServiceEntity
    {
        internal static class SendFuncs
        {
            public static ValueTask MainUnityThreadOnServerSleep(bool isOn, float sleepTime, float delayBeforeSleep, float repeatTime, IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillSendHeader(serializer, __buffer__, out offset, 0, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (bool)isOn);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (float)sleepTime);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (float)delayBeforeSleep);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (float)repeatTime);
                    return EntitySystem.RpcHelper.SendMessage(__buffer__, offset, typeof(Assets.ColonyShared.SharedCode.Entities.Service.IUnityCheatServiceEntity), 0, SharedCode.Network.MessageSendOptions.ReliableOrdered, networkProxy, false);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }

            public static ValueTask SetCurveLoggerState(bool enabledStatus, bool dump, string loggerName, System.Guid dumpId, IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillSendHeader(serializer, __buffer__, out offset, 1, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (bool)enabledStatus);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (bool)dump);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (string)loggerName);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (System.Guid)dumpId);
                    return EntitySystem.RpcHelper.SendMessage(__buffer__, offset, typeof(Assets.ColonyShared.SharedCode.Entities.Service.IUnityCheatServiceEntity), 1, SharedCode.Network.MessageSendOptions.ReliableOrdered, networkProxy, false);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }

            public static ValueTask<SharedCode.Entities.Transform> GetClosestPlayerSpawnPointTransform(SharedCode.Utils.Vector3 pos, IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                Guid __guid__;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillRequestHeader(serializer, __buffer__, out offset, out __guid__, 2, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (SharedCode.Utils.Vector3)pos);
                    return EntitySystem.RpcHelper.SendRequest<SharedCode.Entities.Transform>(__buffer__, offset, typeof(Assets.ColonyShared.SharedCode.Entities.Service.IUnityCheatServiceEntity), 2, SharedCode.Network.MessageSendOptions.ReliableOrdered, __guid__, networkProxy, SharedCode.EntitySystem.ServerCoreRuntimeParameters.RpcTimeoutSeconds, false);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }
        }
    }
}