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
public class ChainCallServiceEntityExternalAlwaysChainProxy : IChainedEntity
{
    private ChainCallServiceEntityExternalChainProxy __chain__;
    public ChainCallServiceEntityExternalAlwaysChainProxy(ChainCallServiceEntityExternalChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public ChainCallServiceEntityExternalAlwaysChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public ChainCallServiceEntityExternalAlwaysChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public ChainCallServiceEntityExternalAlwaysChainProxy StoreResult(string name)
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
    public static ChainCallServiceEntityExternalAlwaysChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IChainCallServiceEntityExternalAlways entity)
    {
        return new ChainCallServiceEntityExternalAlwaysChainProxy(new ChainCallServiceEntityExternalChainProxy((SharedCode.Entities.Service.IChainCallServiceEntityExternal)entity.GetBaseDeltaObject()));
    }

    public static ChainCallServiceEntityExternalAlwaysChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IChainCallServiceEntityExternalAlways entity, IChainedEntity fromChain)
    {
        return new ChainCallServiceEntityExternalAlwaysChainProxy(new ChainCallServiceEntityExternalChainProxy((SharedCode.Entities.Service.IChainCallServiceEntityExternal)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class ChainCallServiceEntityExternalClientBroadcastChainProxy : IChainedEntity
{
    private ChainCallServiceEntityExternalChainProxy __chain__;
    public ChainCallServiceEntityExternalClientBroadcastChainProxy(ChainCallServiceEntityExternalChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public ChainCallServiceEntityExternalClientBroadcastChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public ChainCallServiceEntityExternalClientBroadcastChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public ChainCallServiceEntityExternalClientBroadcastChainProxy StoreResult(string name)
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
    public static ChainCallServiceEntityExternalClientBroadcastChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IChainCallServiceEntityExternalClientBroadcast entity)
    {
        return new ChainCallServiceEntityExternalClientBroadcastChainProxy(new ChainCallServiceEntityExternalChainProxy((SharedCode.Entities.Service.IChainCallServiceEntityExternal)entity.GetBaseDeltaObject()));
    }

    public static ChainCallServiceEntityExternalClientBroadcastChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IChainCallServiceEntityExternalClientBroadcast entity, IChainedEntity fromChain)
    {
        return new ChainCallServiceEntityExternalClientBroadcastChainProxy(new ChainCallServiceEntityExternalChainProxy((SharedCode.Entities.Service.IChainCallServiceEntityExternal)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class ChainCallServiceEntityExternalClientFullApiChainProxy : IChainedEntity
{
    private ChainCallServiceEntityExternalChainProxy __chain__;
    public ChainCallServiceEntityExternalClientFullApiChainProxy(ChainCallServiceEntityExternalChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public ChainCallServiceEntityExternalClientFullApiChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public ChainCallServiceEntityExternalClientFullApiChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public ChainCallServiceEntityExternalClientFullApiChainProxy StoreResult(string name)
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
    public static ChainCallServiceEntityExternalClientFullApiChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IChainCallServiceEntityExternalClientFullApi entity)
    {
        return new ChainCallServiceEntityExternalClientFullApiChainProxy(new ChainCallServiceEntityExternalChainProxy((SharedCode.Entities.Service.IChainCallServiceEntityExternal)entity.GetBaseDeltaObject()));
    }

    public static ChainCallServiceEntityExternalClientFullApiChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IChainCallServiceEntityExternalClientFullApi entity, IChainedEntity fromChain)
    {
        return new ChainCallServiceEntityExternalClientFullApiChainProxy(new ChainCallServiceEntityExternalChainProxy((SharedCode.Entities.Service.IChainCallServiceEntityExternal)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class ChainCallServiceEntityExternalClientFullChainProxy : IChainedEntity
{
    private ChainCallServiceEntityExternalChainProxy __chain__;
    public ChainCallServiceEntityExternalClientFullChainProxy(ChainCallServiceEntityExternalChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public ChainCallServiceEntityExternalClientFullChainProxy ChainCall(ChainArgument<SharedCode.EntitySystem.ChainCalls.EntityMethodsCallsChainBatch> batch)
    {
        __chain__.ChainCall(batch);
        return this;
    }

    public ChainCallServiceEntityExternalClientFullChainProxy CancelChain(ChainArgument<int> typeId, ChainArgument<System.Guid> entityId, ChainArgument<System.Guid> chainId)
    {
        __chain__.CancelChain(typeId, entityId, chainId);
        return this;
    }

    public ChainCallServiceEntityExternalClientFullChainProxy CancelAllChain(ChainArgument<int> typeId, ChainArgument<System.Guid> entityId)
    {
        __chain__.CancelAllChain(typeId, entityId);
        return this;
    }

    public ChainCallServiceEntityExternalClientFullChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public ChainCallServiceEntityExternalClientFullChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public ChainCallServiceEntityExternalClientFullChainProxy StoreResult(string name)
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
    public static ChainCallServiceEntityExternalClientFullChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IChainCallServiceEntityExternalClientFull entity)
    {
        return new ChainCallServiceEntityExternalClientFullChainProxy(new ChainCallServiceEntityExternalChainProxy((SharedCode.Entities.Service.IChainCallServiceEntityExternal)entity.GetBaseDeltaObject()));
    }

    public static ChainCallServiceEntityExternalClientFullChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IChainCallServiceEntityExternalClientFull entity, IChainedEntity fromChain)
    {
        return new ChainCallServiceEntityExternalClientFullChainProxy(new ChainCallServiceEntityExternalChainProxy((SharedCode.Entities.Service.IChainCallServiceEntityExternal)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class ChainCallServiceEntityExternalServerApiChainProxy : IChainedEntity
{
    private ChainCallServiceEntityExternalChainProxy __chain__;
    public ChainCallServiceEntityExternalServerApiChainProxy(ChainCallServiceEntityExternalChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public ChainCallServiceEntityExternalServerApiChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public ChainCallServiceEntityExternalServerApiChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public ChainCallServiceEntityExternalServerApiChainProxy StoreResult(string name)
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
    public static ChainCallServiceEntityExternalServerApiChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IChainCallServiceEntityExternalServerApi entity)
    {
        return new ChainCallServiceEntityExternalServerApiChainProxy(new ChainCallServiceEntityExternalChainProxy((SharedCode.Entities.Service.IChainCallServiceEntityExternal)entity.GetBaseDeltaObject()));
    }

    public static ChainCallServiceEntityExternalServerApiChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IChainCallServiceEntityExternalServerApi entity, IChainedEntity fromChain)
    {
        return new ChainCallServiceEntityExternalServerApiChainProxy(new ChainCallServiceEntityExternalChainProxy((SharedCode.Entities.Service.IChainCallServiceEntityExternal)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class ChainCallServiceEntityExternalServerChainProxy : IChainedEntity
{
    private ChainCallServiceEntityExternalChainProxy __chain__;
    public ChainCallServiceEntityExternalServerChainProxy(ChainCallServiceEntityExternalChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public ChainCallServiceEntityExternalServerChainProxy ChainCall(ChainArgument<SharedCode.EntitySystem.ChainCalls.EntityMethodsCallsChainBatch> batch)
    {
        __chain__.ChainCall(batch);
        return this;
    }

    public ChainCallServiceEntityExternalServerChainProxy CancelChain(ChainArgument<int> typeId, ChainArgument<System.Guid> entityId, ChainArgument<System.Guid> chainId)
    {
        __chain__.CancelChain(typeId, entityId, chainId);
        return this;
    }

    public ChainCallServiceEntityExternalServerChainProxy CancelAllChain(ChainArgument<int> typeId, ChainArgument<System.Guid> entityId)
    {
        __chain__.CancelAllChain(typeId, entityId);
        return this;
    }

    public ChainCallServiceEntityExternalServerChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public ChainCallServiceEntityExternalServerChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public ChainCallServiceEntityExternalServerChainProxy StoreResult(string name)
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
    public static ChainCallServiceEntityExternalServerChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IChainCallServiceEntityExternalServer entity)
    {
        return new ChainCallServiceEntityExternalServerChainProxy(new ChainCallServiceEntityExternalChainProxy((SharedCode.Entities.Service.IChainCallServiceEntityExternal)entity.GetBaseDeltaObject()));
    }

    public static ChainCallServiceEntityExternalServerChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IChainCallServiceEntityExternalServer entity, IChainedEntity fromChain)
    {
        return new ChainCallServiceEntityExternalServerChainProxy(new ChainCallServiceEntityExternalChainProxy((SharedCode.Entities.Service.IChainCallServiceEntityExternal)entity.GetBaseDeltaObject(), fromChain));
    }
}