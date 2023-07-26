// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

using System;
using SharedCode.Entities.Service;
using SharedCode.EntitySystem.ChainCalls;
using SharedCode.Logging;
using SharedCode.OurSimpleIoC;
using SharedCode.Serializers;
using SharedCode.Utils;
using System.Linq;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using GeneratedCode.DeltaObjects.Chain;

[GeneratedCode("CodeGen", "1.0")]
public class ClusterAddressResolverServiceEntityAlwaysChainProxy : IChainedEntity
{
    private ClusterAddressResolverServiceEntityChainProxy __chain__;
    public ClusterAddressResolverServiceEntityAlwaysChainProxy(ClusterAddressResolverServiceEntityChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public ClusterAddressResolverServiceEntityAlwaysChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public ClusterAddressResolverServiceEntityAlwaysChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public ClusterAddressResolverServiceEntityAlwaysChainProxy StoreResult(string name)
    {
        __chain__.StoreResult(name);
        return this;
    }

    BaseChainEntity IChainedEntity.GetBaseChainEntity()
    {
        return (BaseChainEntity)__chain__;
    }
}

public static partial class ChainProxyExtensions
{
    public static ClusterAddressResolverServiceEntityAlwaysChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IClusterAddressResolverServiceEntityAlways entity)
    {
        return new ClusterAddressResolverServiceEntityAlwaysChainProxy(new ClusterAddressResolverServiceEntityChainProxy((SharedCode.Entities.Service.IClusterAddressResolverServiceEntity)entity.GetBaseDeltaObject()));
    }

    public static ClusterAddressResolverServiceEntityAlwaysChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IClusterAddressResolverServiceEntityAlways entity, IChainedEntity fromChain)
    {
        return new ClusterAddressResolverServiceEntityAlwaysChainProxy(new ClusterAddressResolverServiceEntityChainProxy((SharedCode.Entities.Service.IClusterAddressResolverServiceEntity)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class ClusterAddressResolverServiceEntityClientBroadcastChainProxy : IChainedEntity
{
    private ClusterAddressResolverServiceEntityChainProxy __chain__;
    public ClusterAddressResolverServiceEntityClientBroadcastChainProxy(ClusterAddressResolverServiceEntityChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public ClusterAddressResolverServiceEntityClientBroadcastChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public ClusterAddressResolverServiceEntityClientBroadcastChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public ClusterAddressResolverServiceEntityClientBroadcastChainProxy StoreResult(string name)
    {
        __chain__.StoreResult(name);
        return this;
    }

    BaseChainEntity IChainedEntity.GetBaseChainEntity()
    {
        return (BaseChainEntity)__chain__;
    }
}

public static partial class ChainProxyExtensions
{
    public static ClusterAddressResolverServiceEntityClientBroadcastChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IClusterAddressResolverServiceEntityClientBroadcast entity)
    {
        return new ClusterAddressResolverServiceEntityClientBroadcastChainProxy(new ClusterAddressResolverServiceEntityChainProxy((SharedCode.Entities.Service.IClusterAddressResolverServiceEntity)entity.GetBaseDeltaObject()));
    }

    public static ClusterAddressResolverServiceEntityClientBroadcastChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IClusterAddressResolverServiceEntityClientBroadcast entity, IChainedEntity fromChain)
    {
        return new ClusterAddressResolverServiceEntityClientBroadcastChainProxy(new ClusterAddressResolverServiceEntityChainProxy((SharedCode.Entities.Service.IClusterAddressResolverServiceEntity)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class ClusterAddressResolverServiceEntityClientFullApiChainProxy : IChainedEntity
{
    private ClusterAddressResolverServiceEntityChainProxy __chain__;
    public ClusterAddressResolverServiceEntityClientFullApiChainProxy(ClusterAddressResolverServiceEntityChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public ClusterAddressResolverServiceEntityClientFullApiChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public ClusterAddressResolverServiceEntityClientFullApiChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public ClusterAddressResolverServiceEntityClientFullApiChainProxy StoreResult(string name)
    {
        __chain__.StoreResult(name);
        return this;
    }

    BaseChainEntity IChainedEntity.GetBaseChainEntity()
    {
        return (BaseChainEntity)__chain__;
    }
}

public static partial class ChainProxyExtensions
{
    public static ClusterAddressResolverServiceEntityClientFullApiChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IClusterAddressResolverServiceEntityClientFullApi entity)
    {
        return new ClusterAddressResolverServiceEntityClientFullApiChainProxy(new ClusterAddressResolverServiceEntityChainProxy((SharedCode.Entities.Service.IClusterAddressResolverServiceEntity)entity.GetBaseDeltaObject()));
    }

    public static ClusterAddressResolverServiceEntityClientFullApiChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IClusterAddressResolverServiceEntityClientFullApi entity, IChainedEntity fromChain)
    {
        return new ClusterAddressResolverServiceEntityClientFullApiChainProxy(new ClusterAddressResolverServiceEntityChainProxy((SharedCode.Entities.Service.IClusterAddressResolverServiceEntity)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class ClusterAddressResolverServiceEntityClientFullChainProxy : IChainedEntity
{
    private ClusterAddressResolverServiceEntityChainProxy __chain__;
    public ClusterAddressResolverServiceEntityClientFullChainProxy(ClusterAddressResolverServiceEntityChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public ClusterAddressResolverServiceEntityClientFullChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public ClusterAddressResolverServiceEntityClientFullChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public ClusterAddressResolverServiceEntityClientFullChainProxy StoreResult(string name)
    {
        __chain__.StoreResult(name);
        return this;
    }

    BaseChainEntity IChainedEntity.GetBaseChainEntity()
    {
        return (BaseChainEntity)__chain__;
    }
}

public static partial class ChainProxyExtensions
{
    public static ClusterAddressResolverServiceEntityClientFullChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IClusterAddressResolverServiceEntityClientFull entity)
    {
        return new ClusterAddressResolverServiceEntityClientFullChainProxy(new ClusterAddressResolverServiceEntityChainProxy((SharedCode.Entities.Service.IClusterAddressResolverServiceEntity)entity.GetBaseDeltaObject()));
    }

    public static ClusterAddressResolverServiceEntityClientFullChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IClusterAddressResolverServiceEntityClientFull entity, IChainedEntity fromChain)
    {
        return new ClusterAddressResolverServiceEntityClientFullChainProxy(new ClusterAddressResolverServiceEntityChainProxy((SharedCode.Entities.Service.IClusterAddressResolverServiceEntity)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class ClusterAddressResolverServiceEntityServerApiChainProxy : IChainedEntity
{
    private ClusterAddressResolverServiceEntityChainProxy __chain__;
    public ClusterAddressResolverServiceEntityServerApiChainProxy(ClusterAddressResolverServiceEntityChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public ClusterAddressResolverServiceEntityServerApiChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public ClusterAddressResolverServiceEntityServerApiChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public ClusterAddressResolverServiceEntityServerApiChainProxy StoreResult(string name)
    {
        __chain__.StoreResult(name);
        return this;
    }

    BaseChainEntity IChainedEntity.GetBaseChainEntity()
    {
        return (BaseChainEntity)__chain__;
    }
}

public static partial class ChainProxyExtensions
{
    public static ClusterAddressResolverServiceEntityServerApiChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IClusterAddressResolverServiceEntityServerApi entity)
    {
        return new ClusterAddressResolverServiceEntityServerApiChainProxy(new ClusterAddressResolverServiceEntityChainProxy((SharedCode.Entities.Service.IClusterAddressResolverServiceEntity)entity.GetBaseDeltaObject()));
    }

    public static ClusterAddressResolverServiceEntityServerApiChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IClusterAddressResolverServiceEntityServerApi entity, IChainedEntity fromChain)
    {
        return new ClusterAddressResolverServiceEntityServerApiChainProxy(new ClusterAddressResolverServiceEntityChainProxy((SharedCode.Entities.Service.IClusterAddressResolverServiceEntity)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class ClusterAddressResolverServiceEntityServerChainProxy : IChainedEntity
{
    private ClusterAddressResolverServiceEntityChainProxy __chain__;
    public ClusterAddressResolverServiceEntityServerChainProxy(ClusterAddressResolverServiceEntityChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public ClusterAddressResolverServiceEntityServerChainProxy GetEntityAddressRepositoryId(ChainArgument<int> typeId, ChainArgument<System.Guid> entityId)
    {
        __chain__.GetEntityAddressRepositoryId(typeId, entityId);
        return this;
    }

    public ClusterAddressResolverServiceEntityServerChainProxy GetAllEntitiesByType(ChainArgument<int> typeId)
    {
        __chain__.GetAllEntitiesByType(typeId);
        return this;
    }

    public ClusterAddressResolverServiceEntityServerChainProxy SetEntityAddressRepositoryId(ChainArgument<int> typeId, ChainArgument<System.Guid> entityId, ChainArgument<System.Guid> repositoryId)
    {
        __chain__.SetEntityAddressRepositoryId(typeId, entityId, repositoryId);
        return this;
    }

    public ClusterAddressResolverServiceEntityServerChainProxy SetEntitiesAddressRepositoryId(ChainArgument<System.Collections.Generic.Dictionary<int, System.Guid>> entities, ChainArgument<System.Guid> repositoryId)
    {
        __chain__.SetEntitiesAddressRepositoryId(entities, repositoryId);
        return this;
    }

    public ClusterAddressResolverServiceEntityServerChainProxy RemoveEntityAddressRepositoryId(ChainArgument<int> typeId, ChainArgument<System.Guid> entityId)
    {
        __chain__.RemoveEntityAddressRepositoryId(typeId, entityId);
        return this;
    }

    public ClusterAddressResolverServiceEntityServerChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public ClusterAddressResolverServiceEntityServerChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public ClusterAddressResolverServiceEntityServerChainProxy StoreResult(string name)
    {
        __chain__.StoreResult(name);
        return this;
    }

    BaseChainEntity IChainedEntity.GetBaseChainEntity()
    {
        return (BaseChainEntity)__chain__;
    }
}

public static partial class ChainProxyExtensions
{
    public static ClusterAddressResolverServiceEntityServerChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IClusterAddressResolverServiceEntityServer entity)
    {
        return new ClusterAddressResolverServiceEntityServerChainProxy(new ClusterAddressResolverServiceEntityChainProxy((SharedCode.Entities.Service.IClusterAddressResolverServiceEntity)entity.GetBaseDeltaObject()));
    }

    public static ClusterAddressResolverServiceEntityServerChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IClusterAddressResolverServiceEntityServer entity, IChainedEntity fromChain)
    {
        return new ClusterAddressResolverServiceEntityServerChainProxy(new ClusterAddressResolverServiceEntityChainProxy((SharedCode.Entities.Service.IClusterAddressResolverServiceEntity)entity.GetBaseDeltaObject(), fromChain));
    }
}