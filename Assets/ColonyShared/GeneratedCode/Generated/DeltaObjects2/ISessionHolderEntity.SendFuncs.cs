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
    public partial class SessionHolderEntity
    {
        internal static class SendFuncs
        {
            public static ValueTask Register(System.Guid guid, System.Guid session, IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillSendHeader(serializer, __buffer__, out offset, 0, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (System.Guid)guid);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (System.Guid)session);
                    return EntitySystem.RpcHelper.SendMessage(__buffer__, offset, typeof(GeneratedCode.Telemetry.ISessionHolderEntity), 0, SharedCode.Network.MessageSendOptions.ReliableOrdered, networkProxy, false);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }

            public static ValueTask Unregister(System.Guid guid, IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillSendHeader(serializer, __buffer__, out offset, 1, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (System.Guid)guid);
                    return EntitySystem.RpcHelper.SendMessage(__buffer__, offset, typeof(GeneratedCode.Telemetry.ISessionHolderEntity), 1, SharedCode.Network.MessageSendOptions.ReliableOrdered, networkProxy, false);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }
        }
    }
}