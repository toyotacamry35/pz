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
public class SpawnDaemonsInfoCollectorAlwaysChainProxy : IChainedEntity
{
    private SpawnDaemonsInfoCollectorChainProxy __chain__;
    public SpawnDaemonsInfoCollectorAlwaysChainProxy(SpawnDaemonsInfoCollectorChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public SpawnDaemonsInfoCollectorAlwaysChainProxy Update()
    {
        __chain__.Update();
        return this;
    }

    public SpawnDaemonsInfoCollectorAlwaysChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public SpawnDaemonsInfoCollectorAlwaysChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public SpawnDaemonsInfoCollectorAlwaysChainProxy StoreResult(string name)
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
    public static SpawnDaemonsInfoCollectorAlwaysChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.ISpawnDaemonsInfoCollectorAlways entity)
    {
        return new SpawnDaemonsInfoCollectorAlwaysChainProxy(new SpawnDaemonsInfoCollectorChainProxy((Assets.ColonyShared.SharedCode.Entities.ISpawnDaemonsInfoCollector)entity.GetBaseDeltaObject()));
    }

    public static SpawnDaemonsInfoCollectorAlwaysChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.ISpawnDaemonsInfoCollectorAlways entity, IChainedEntity fromChain)
    {
        return new SpawnDaemonsInfoCollectorAlwaysChainProxy(new SpawnDaemonsInfoCollectorChainProxy((Assets.ColonyShared.SharedCode.Entities.ISpawnDaemonsInfoCollector)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class SpawnDaemonsInfoCollectorClientBroadcastChainProxy : IChainedEntity
{
    private SpawnDaemonsInfoCollectorChainProxy __chain__;
    public SpawnDaemonsInfoCollectorClientBroadcastChainProxy(SpawnDaemonsInfoCollectorChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public SpawnDaemonsInfoCollectorClientBroadcastChainProxy Update()
    {
        __chain__.Update();
        return this;
    }

    public SpawnDaemonsInfoCollectorClientBroadcastChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public SpawnDaemonsInfoCollectorClientBroadcastChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public SpawnDaemonsInfoCollectorClientBroadcastChainProxy StoreResult(string name)
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
    public static SpawnDaemonsInfoCollectorClientBroadcastChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.ISpawnDaemonsInfoCollectorClientBroadcast entity)
    {
        return new SpawnDaemonsInfoCollectorClientBroadcastChainProxy(new SpawnDaemonsInfoCollectorChainProxy((Assets.ColonyShared.SharedCode.Entities.ISpawnDaemonsInfoCollector)entity.GetBaseDeltaObject()));
    }

    public static SpawnDaemonsInfoCollectorClientBroadcastChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.ISpawnDaemonsInfoCollectorClientBroadcast entity, IChainedEntity fromChain)
    {
        return new SpawnDaemonsInfoCollectorClientBroadcastChainProxy(new SpawnDaemonsInfoCollectorChainProxy((Assets.ColonyShared.SharedCode.Entities.ISpawnDaemonsInfoCollector)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class SpawnDaemonsInfoCollectorClientFullApiChainProxy : IChainedEntity
{
    private SpawnDaemonsInfoCollectorChainProxy __chain__;
    public SpawnDaemonsInfoCollectorClientFullApiChainProxy(SpawnDaemonsInfoCollectorChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public SpawnDaemonsInfoCollectorClientFullApiChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public SpawnDaemonsInfoCollectorClientFullApiChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public SpawnDaemonsInfoCollectorClientFullApiChainProxy StoreResult(string name)
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
    public static SpawnDaemonsInfoCollectorClientFullApiChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.ISpawnDaemonsInfoCollectorClientFullApi entity)
    {
        return new SpawnDaemonsInfoCollectorClientFullApiChainProxy(new SpawnDaemonsInfoCollectorChainProxy((Assets.ColonyShared.SharedCode.Entities.ISpawnDaemonsInfoCollector)entity.GetBaseDeltaObject()));
    }

    public static SpawnDaemonsInfoCollectorClientFullApiChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.ISpawnDaemonsInfoCollectorClientFullApi entity, IChainedEntity fromChain)
    {
        return new SpawnDaemonsInfoCollectorClientFullApiChainProxy(new SpawnDaemonsInfoCollectorChainProxy((Assets.ColonyShared.SharedCode.Entities.ISpawnDaemonsInfoCollector)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class SpawnDaemonsInfoCollectorClientFullChainProxy : IChainedEntity
{
    private SpawnDaemonsInfoCollectorChainProxy __chain__;
    public SpawnDaemonsInfoCollectorClientFullChainProxy(SpawnDaemonsInfoCollectorChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public SpawnDaemonsInfoCollectorClientFullChainProxy Update()
    {
        __chain__.Update();
        return this;
    }

    public SpawnDaemonsInfoCollectorClientFullChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public SpawnDaemonsInfoCollectorClientFullChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public SpawnDaemonsInfoCollectorClientFullChainProxy StoreResult(string name)
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
    public static SpawnDaemonsInfoCollectorClientFullChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.ISpawnDaemonsInfoCollectorClientFull entity)
    {
        return new SpawnDaemonsInfoCollectorClientFullChainProxy(new SpawnDaemonsInfoCollectorChainProxy((Assets.ColonyShared.SharedCode.Entities.ISpawnDaemonsInfoCollector)entity.GetBaseDeltaObject()));
    }

    public static SpawnDaemonsInfoCollectorClientFullChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.ISpawnDaemonsInfoCollectorClientFull entity, IChainedEntity fromChain)
    {
        return new SpawnDaemonsInfoCollectorClientFullChainProxy(new SpawnDaemonsInfoCollectorChainProxy((Assets.ColonyShared.SharedCode.Entities.ISpawnDaemonsInfoCollector)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class SpawnDaemonsInfoCollectorServerApiChainProxy : IChainedEntity
{
    private SpawnDaemonsInfoCollectorChainProxy __chain__;
    public SpawnDaemonsInfoCollectorServerApiChainProxy(SpawnDaemonsInfoCollectorChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public SpawnDaemonsInfoCollectorServerApiChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public SpawnDaemonsInfoCollectorServerApiChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public SpawnDaemonsInfoCollectorServerApiChainProxy StoreResult(string name)
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
    public static SpawnDaemonsInfoCollectorServerApiChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.ISpawnDaemonsInfoCollectorServerApi entity)
    {
        return new SpawnDaemonsInfoCollectorServerApiChainProxy(new SpawnDaemonsInfoCollectorChainProxy((Assets.ColonyShared.SharedCode.Entities.ISpawnDaemonsInfoCollector)entity.GetBaseDeltaObject()));
    }

    public static SpawnDaemonsInfoCollectorServerApiChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.ISpawnDaemonsInfoCollectorServerApi entity, IChainedEntity fromChain)
    {
        return new SpawnDaemonsInfoCollectorServerApiChainProxy(new SpawnDaemonsInfoCollectorChainProxy((Assets.ColonyShared.SharedCode.Entities.ISpawnDaemonsInfoCollector)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class SpawnDaemonsInfoCollectorServerChainProxy : IChainedEntity
{
    private SpawnDaemonsInfoCollectorChainProxy __chain__;
    public SpawnDaemonsInfoCollectorServerChainProxy(SpawnDaemonsInfoCollectorChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public SpawnDaemonsInfoCollectorServerChainProxy Update()
    {
        __chain__.Update();
        return this;
    }

    public SpawnDaemonsInfoCollectorServerChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public SpawnDaemonsInfoCollectorServerChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public SpawnDaemonsInfoCollectorServerChainProxy StoreResult(string name)
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
    public static SpawnDaemonsInfoCollectorServerChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.ISpawnDaemonsInfoCollectorServer entity)
    {
        return new SpawnDaemonsInfoCollectorServerChainProxy(new SpawnDaemonsInfoCollectorChainProxy((Assets.ColonyShared.SharedCode.Entities.ISpawnDaemonsInfoCollector)entity.GetBaseDeltaObject()));
    }

    public static SpawnDaemonsInfoCollectorServerChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.ISpawnDaemonsInfoCollectorServer entity, IChainedEntity fromChain)
    {
        return new SpawnDaemonsInfoCollectorServerChainProxy(new SpawnDaemonsInfoCollectorChainProxy((Assets.ColonyShared.SharedCode.Entities.ISpawnDaemonsInfoCollector)entity.GetBaseDeltaObject(), fromChain));
    }
}