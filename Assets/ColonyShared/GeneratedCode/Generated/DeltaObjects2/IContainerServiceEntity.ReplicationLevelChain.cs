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
public class ContainerServiceEntityAlwaysChainProxy : IChainedEntity
{
    private ContainerServiceEntityChainProxy __chain__;
    public ContainerServiceEntityAlwaysChainProxy(ContainerServiceEntityChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public ContainerServiceEntityAlwaysChainProxy MoveItem(ChainArgument<SharedCode.EntitySystem.PropertyAddress> source, ChainArgument<int> sourceSlotId, ChainArgument<SharedCode.EntitySystem.PropertyAddress> destination, ChainArgument<int> destinationSlotId, ChainArgument<int> count, ChainArgument<System.Guid> clientSrcEntityId)
    {
        __chain__.MoveItem(source, sourceSlotId, destination, destinationSlotId, count, clientSrcEntityId);
        return this;
    }

    public ContainerServiceEntityAlwaysChainProxy RemoveItem(ChainArgument<SharedCode.EntitySystem.PropertyAddress> source, ChainArgument<int> sourceSlotId, ChainArgument<int> count, ChainArgument<System.Guid> clientEntityId)
    {
        __chain__.RemoveItem(source, sourceSlotId, count, clientEntityId);
        return this;
    }

    public ContainerServiceEntityAlwaysChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public ContainerServiceEntityAlwaysChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public ContainerServiceEntityAlwaysChainProxy StoreResult(string name)
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
    public static ContainerServiceEntityAlwaysChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IContainerServiceEntityAlways entity)
    {
        return new ContainerServiceEntityAlwaysChainProxy(new ContainerServiceEntityChainProxy((SharedCode.Entities.Service.IContainerServiceEntity)entity.GetBaseDeltaObject()));
    }

    public static ContainerServiceEntityAlwaysChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IContainerServiceEntityAlways entity, IChainedEntity fromChain)
    {
        return new ContainerServiceEntityAlwaysChainProxy(new ContainerServiceEntityChainProxy((SharedCode.Entities.Service.IContainerServiceEntity)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class ContainerServiceEntityClientBroadcastChainProxy : IChainedEntity
{
    private ContainerServiceEntityChainProxy __chain__;
    public ContainerServiceEntityClientBroadcastChainProxy(ContainerServiceEntityChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public ContainerServiceEntityClientBroadcastChainProxy MoveItem(ChainArgument<SharedCode.EntitySystem.PropertyAddress> source, ChainArgument<int> sourceSlotId, ChainArgument<SharedCode.EntitySystem.PropertyAddress> destination, ChainArgument<int> destinationSlotId, ChainArgument<int> count, ChainArgument<System.Guid> clientSrcEntityId)
    {
        __chain__.MoveItem(source, sourceSlotId, destination, destinationSlotId, count, clientSrcEntityId);
        return this;
    }

    public ContainerServiceEntityClientBroadcastChainProxy RemoveItem(ChainArgument<SharedCode.EntitySystem.PropertyAddress> source, ChainArgument<int> sourceSlotId, ChainArgument<int> count, ChainArgument<System.Guid> clientEntityId)
    {
        __chain__.RemoveItem(source, sourceSlotId, count, clientEntityId);
        return this;
    }

    public ContainerServiceEntityClientBroadcastChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public ContainerServiceEntityClientBroadcastChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public ContainerServiceEntityClientBroadcastChainProxy StoreResult(string name)
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
    public static ContainerServiceEntityClientBroadcastChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IContainerServiceEntityClientBroadcast entity)
    {
        return new ContainerServiceEntityClientBroadcastChainProxy(new ContainerServiceEntityChainProxy((SharedCode.Entities.Service.IContainerServiceEntity)entity.GetBaseDeltaObject()));
    }

    public static ContainerServiceEntityClientBroadcastChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IContainerServiceEntityClientBroadcast entity, IChainedEntity fromChain)
    {
        return new ContainerServiceEntityClientBroadcastChainProxy(new ContainerServiceEntityChainProxy((SharedCode.Entities.Service.IContainerServiceEntity)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class ContainerServiceEntityClientFullApiChainProxy : IChainedEntity
{
    private ContainerServiceEntityChainProxy __chain__;
    public ContainerServiceEntityClientFullApiChainProxy(ContainerServiceEntityChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public ContainerServiceEntityClientFullApiChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public ContainerServiceEntityClientFullApiChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public ContainerServiceEntityClientFullApiChainProxy StoreResult(string name)
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
    public static ContainerServiceEntityClientFullApiChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IContainerServiceEntityClientFullApi entity)
    {
        return new ContainerServiceEntityClientFullApiChainProxy(new ContainerServiceEntityChainProxy((SharedCode.Entities.Service.IContainerServiceEntity)entity.GetBaseDeltaObject()));
    }

    public static ContainerServiceEntityClientFullApiChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IContainerServiceEntityClientFullApi entity, IChainedEntity fromChain)
    {
        return new ContainerServiceEntityClientFullApiChainProxy(new ContainerServiceEntityChainProxy((SharedCode.Entities.Service.IContainerServiceEntity)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class ContainerServiceEntityClientFullChainProxy : IChainedEntity
{
    private ContainerServiceEntityChainProxy __chain__;
    public ContainerServiceEntityClientFullChainProxy(ContainerServiceEntityChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public ContainerServiceEntityClientFullChainProxy MoveItem(ChainArgument<SharedCode.EntitySystem.PropertyAddress> source, ChainArgument<int> sourceSlotId, ChainArgument<SharedCode.EntitySystem.PropertyAddress> destination, ChainArgument<int> destinationSlotId, ChainArgument<int> count, ChainArgument<System.Guid> clientSrcEntityId)
    {
        __chain__.MoveItem(source, sourceSlotId, destination, destinationSlotId, count, clientSrcEntityId);
        return this;
    }

    public ContainerServiceEntityClientFullChainProxy RemoveItem(ChainArgument<SharedCode.EntitySystem.PropertyAddress> source, ChainArgument<int> sourceSlotId, ChainArgument<int> count, ChainArgument<System.Guid> clientEntityId)
    {
        __chain__.RemoveItem(source, sourceSlotId, count, clientEntityId);
        return this;
    }

    public ContainerServiceEntityClientFullChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public ContainerServiceEntityClientFullChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public ContainerServiceEntityClientFullChainProxy StoreResult(string name)
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
    public static ContainerServiceEntityClientFullChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IContainerServiceEntityClientFull entity)
    {
        return new ContainerServiceEntityClientFullChainProxy(new ContainerServiceEntityChainProxy((SharedCode.Entities.Service.IContainerServiceEntity)entity.GetBaseDeltaObject()));
    }

    public static ContainerServiceEntityClientFullChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IContainerServiceEntityClientFull entity, IChainedEntity fromChain)
    {
        return new ContainerServiceEntityClientFullChainProxy(new ContainerServiceEntityChainProxy((SharedCode.Entities.Service.IContainerServiceEntity)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class ContainerServiceEntityServerApiChainProxy : IChainedEntity
{
    private ContainerServiceEntityChainProxy __chain__;
    public ContainerServiceEntityServerApiChainProxy(ContainerServiceEntityChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public ContainerServiceEntityServerApiChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public ContainerServiceEntityServerApiChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public ContainerServiceEntityServerApiChainProxy StoreResult(string name)
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
    public static ContainerServiceEntityServerApiChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IContainerServiceEntityServerApi entity)
    {
        return new ContainerServiceEntityServerApiChainProxy(new ContainerServiceEntityChainProxy((SharedCode.Entities.Service.IContainerServiceEntity)entity.GetBaseDeltaObject()));
    }

    public static ContainerServiceEntityServerApiChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IContainerServiceEntityServerApi entity, IChainedEntity fromChain)
    {
        return new ContainerServiceEntityServerApiChainProxy(new ContainerServiceEntityChainProxy((SharedCode.Entities.Service.IContainerServiceEntity)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class ContainerServiceEntityServerChainProxy : IChainedEntity
{
    private ContainerServiceEntityChainProxy __chain__;
    public ContainerServiceEntityServerChainProxy(ContainerServiceEntityChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public ContainerServiceEntityServerChainProxy MoveItem(ChainArgument<SharedCode.EntitySystem.PropertyAddress> source, ChainArgument<int> sourceSlotId, ChainArgument<SharedCode.EntitySystem.PropertyAddress> destination, ChainArgument<int> destinationSlotId, ChainArgument<int> count, ChainArgument<System.Guid> clientSrcEntityId)
    {
        __chain__.MoveItem(source, sourceSlotId, destination, destinationSlotId, count, clientSrcEntityId);
        return this;
    }

    public ContainerServiceEntityServerChainProxy RemoveItem(ChainArgument<SharedCode.EntitySystem.PropertyAddress> source, ChainArgument<int> sourceSlotId, ChainArgument<int> count, ChainArgument<System.Guid> clientEntityId)
    {
        __chain__.RemoveItem(source, sourceSlotId, count, clientEntityId);
        return this;
    }

    public ContainerServiceEntityServerChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public ContainerServiceEntityServerChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public ContainerServiceEntityServerChainProxy StoreResult(string name)
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
    public static ContainerServiceEntityServerChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IContainerServiceEntityServer entity)
    {
        return new ContainerServiceEntityServerChainProxy(new ContainerServiceEntityChainProxy((SharedCode.Entities.Service.IContainerServiceEntity)entity.GetBaseDeltaObject()));
    }

    public static ContainerServiceEntityServerChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IContainerServiceEntityServer entity, IChainedEntity fromChain)
    {
        return new ContainerServiceEntityServerChainProxy(new ContainerServiceEntityChainProxy((SharedCode.Entities.Service.IContainerServiceEntity)entity.GetBaseDeltaObject(), fromChain));
    }
}