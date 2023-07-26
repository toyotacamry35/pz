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
    public partial class MutationMechanics
    {
        internal static class SendFuncs
        {
            public static ValueTask<bool> CanChangeMutation(float value, Assets.Src.Aspects.Impl.Factions.Template.MutatingFactionDef toFaction, IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                Guid __guid__;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillRequestHeader(serializer, __buffer__, out offset, out __guid__, 0, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (float)value);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (Assets.Src.Aspects.Impl.Factions.Template.MutatingFactionDef)toFaction);
                    return EntitySystem.RpcHelper.SendRequest<bool>(__buffer__, offset, typeof(SharedCode.Entities.IMutationMechanics), 0, SharedCode.Network.MessageSendOptions.ReliableOrdered, __guid__, networkProxy, SharedCode.EntitySystem.ServerCoreRuntimeParameters.RpcTimeoutSeconds, false);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }

            public static ValueTask<bool> ChangeMutation(float value, Assets.Src.Aspects.Impl.Factions.Template.MutatingFactionDef toFaction, float coolDownTime, bool forceChange, IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                Guid __guid__;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillRequestHeader(serializer, __buffer__, out offset, out __guid__, 1, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (float)value);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (Assets.Src.Aspects.Impl.Factions.Template.MutatingFactionDef)toFaction);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (float)coolDownTime);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (bool)forceChange);
                    return EntitySystem.RpcHelper.SendRequest<bool>(__buffer__, offset, typeof(SharedCode.Entities.IMutationMechanics), 1, SharedCode.Network.MessageSendOptions.ReliableOrdered, __guid__, networkProxy, SharedCode.EntitySystem.ServerCoreRuntimeParameters.RpcTimeoutSeconds, false);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }

            public static ValueTask<bool> ApplyMutationChangeForced(Assets.Src.Aspects.Impl.Factions.Template.MutationStageDef newStage, Assets.Src.Aspects.Impl.Factions.Template.MutatingFactionDef newFaction, IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                Guid __guid__;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillRequestHeader(serializer, __buffer__, out offset, out __guid__, 2, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (Assets.Src.Aspects.Impl.Factions.Template.MutationStageDef)newStage);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (Assets.Src.Aspects.Impl.Factions.Template.MutatingFactionDef)newFaction);
                    return EntitySystem.RpcHelper.SendRequest<bool>(__buffer__, offset, typeof(SharedCode.Entities.IMutationMechanics), 2, SharedCode.Network.MessageSendOptions.ReliableOrdered, __guid__, networkProxy, SharedCode.EntitySystem.ServerCoreRuntimeParameters.RpcTimeoutSeconds, false);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }
        }
    }
}