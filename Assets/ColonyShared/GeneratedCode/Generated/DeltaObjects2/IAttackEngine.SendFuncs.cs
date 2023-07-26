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
    public partial class AttackEngine
    {
        internal static class SendFuncs
        {
            public static ValueTask<bool> StartAttack(GeneratedCode.DeltaObjects.SpellPartCastId attackId, long finishTime, ColonyShared.SharedCode.Aspects.Misc.AttackDef attackDef, System.Collections.Generic.IReadOnlyList<ResourceSystem.Aspects.AttackModifierDef> modifiers, IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                Guid __guid__;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillRequestHeader(serializer, __buffer__, out offset, out __guid__, 0, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (GeneratedCode.DeltaObjects.SpellPartCastId)attackId);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (long)finishTime);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (ColonyShared.SharedCode.Aspects.Misc.AttackDef)attackDef);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (System.Collections.Generic.IReadOnlyList<ResourceSystem.Aspects.AttackModifierDef>)modifiers);
                    return EntitySystem.RpcHelper.SendRequest<bool>(__buffer__, offset, typeof(ColonyShared.SharedCode.Aspects.Combat.IAttackEngine), 0, SharedCode.Network.MessageSendOptions.ReliableOrdered, __guid__, networkProxy, SharedCode.EntitySystem.ServerCoreRuntimeParameters.RpcTimeoutSeconds, false);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }

            public static ValueTask FinishAttack(GeneratedCode.DeltaObjects.SpellPartCastId attackId, long currentTime, IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillSendHeader(serializer, __buffer__, out offset, 1, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (GeneratedCode.DeltaObjects.SpellPartCastId)attackId);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (long)currentTime);
                    return EntitySystem.RpcHelper.SendMessage(__buffer__, offset, typeof(ColonyShared.SharedCode.Aspects.Combat.IAttackEngine), 1, SharedCode.Network.MessageSendOptions.ReliableOrdered, networkProxy, false);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }

            public static ValueTask PushAttackTargets(GeneratedCode.DeltaObjects.SpellPartCastId attackId, System.Collections.Generic.List<ColonyShared.SharedCode.Aspects.Combat.AttackTargetInfo> targets, IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillSendHeader(serializer, __buffer__, out offset, 2, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (GeneratedCode.DeltaObjects.SpellPartCastId)attackId);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (System.Collections.Generic.List<ColonyShared.SharedCode.Aspects.Combat.AttackTargetInfo>)targets);
                    return EntitySystem.RpcHelper.SendMessage(__buffer__, offset, typeof(ColonyShared.SharedCode.Aspects.Combat.IAttackEngine), 2, SharedCode.Network.MessageSendOptions.ReliableOrdered, networkProxy, false);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }

            public static ValueTask SetAttackDoer(ColonyShared.SharedCode.Aspects.Combat.IAttackDoer newDoer, IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillSendHeader(serializer, __buffer__, out offset, 3, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (ColonyShared.SharedCode.Aspects.Combat.IAttackDoer)newDoer);
                    return EntitySystem.RpcHelper.SendMessage(__buffer__, offset, typeof(ColonyShared.SharedCode.Aspects.Combat.IAttackEngine), 3, SharedCode.Network.MessageSendOptions.ReliableOrdered, networkProxy, false);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }

            public static ValueTask UnsetAttackDoer(ColonyShared.SharedCode.Aspects.Combat.IAttackDoer oldDoer, IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillSendHeader(serializer, __buffer__, out offset, 4, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (ColonyShared.SharedCode.Aspects.Combat.IAttackDoer)oldDoer);
                    return EntitySystem.RpcHelper.SendMessage(__buffer__, offset, typeof(ColonyShared.SharedCode.Aspects.Combat.IAttackEngine), 4, SharedCode.Network.MessageSendOptions.ReliableOrdered, networkProxy, false);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }
        }
    }
}