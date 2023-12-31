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
public class UnityCheatServiceEntityAlwaysChainProxy : IChainedEntity
{
    private UnityCheatServiceEntityChainProxy __chain__;
    public UnityCheatServiceEntityAlwaysChainProxy(UnityCheatServiceEntityChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public UnityCheatServiceEntityAlwaysChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public UnityCheatServiceEntityAlwaysChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public UnityCheatServiceEntityAlwaysChainProxy StoreResult(string name)
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
    public static UnityCheatServiceEntityAlwaysChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IUnityCheatServiceEntityAlways entity)
    {
        return new UnityCheatServiceEntityAlwaysChainProxy(new UnityCheatServiceEntityChainProxy((Assets.ColonyShared.SharedCode.Entities.Service.IUnityCheatServiceEntity)entity.GetBaseDeltaObject()));
    }

    public static UnityCheatServiceEntityAlwaysChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IUnityCheatServiceEntityAlways entity, IChainedEntity fromChain)
    {
        return new UnityCheatServiceEntityAlwaysChainProxy(new UnityCheatServiceEntityChainProxy((Assets.ColonyShared.SharedCode.Entities.Service.IUnityCheatServiceEntity)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class UnityCheatServiceEntityClientBroadcastChainProxy : IChainedEntity
{
    private UnityCheatServiceEntityChainProxy __chain__;
    public UnityCheatServiceEntityClientBroadcastChainProxy(UnityCheatServiceEntityChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public UnityCheatServiceEntityClientBroadcastChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public UnityCheatServiceEntityClientBroadcastChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public UnityCheatServiceEntityClientBroadcastChainProxy StoreResult(string name)
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
    public static UnityCheatServiceEntityClientBroadcastChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IUnityCheatServiceEntityClientBroadcast entity)
    {
        return new UnityCheatServiceEntityClientBroadcastChainProxy(new UnityCheatServiceEntityChainProxy((Assets.ColonyShared.SharedCode.Entities.Service.IUnityCheatServiceEntity)entity.GetBaseDeltaObject()));
    }

    public static UnityCheatServiceEntityClientBroadcastChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IUnityCheatServiceEntityClientBroadcast entity, IChainedEntity fromChain)
    {
        return new UnityCheatServiceEntityClientBroadcastChainProxy(new UnityCheatServiceEntityChainProxy((Assets.ColonyShared.SharedCode.Entities.Service.IUnityCheatServiceEntity)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class UnityCheatServiceEntityClientFullApiChainProxy : IChainedEntity
{
    private UnityCheatServiceEntityChainProxy __chain__;
    public UnityCheatServiceEntityClientFullApiChainProxy(UnityCheatServiceEntityChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public UnityCheatServiceEntityClientFullApiChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public UnityCheatServiceEntityClientFullApiChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public UnityCheatServiceEntityClientFullApiChainProxy StoreResult(string name)
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
    public static UnityCheatServiceEntityClientFullApiChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IUnityCheatServiceEntityClientFullApi entity)
    {
        return new UnityCheatServiceEntityClientFullApiChainProxy(new UnityCheatServiceEntityChainProxy((Assets.ColonyShared.SharedCode.Entities.Service.IUnityCheatServiceEntity)entity.GetBaseDeltaObject()));
    }

    public static UnityCheatServiceEntityClientFullApiChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IUnityCheatServiceEntityClientFullApi entity, IChainedEntity fromChain)
    {
        return new UnityCheatServiceEntityClientFullApiChainProxy(new UnityCheatServiceEntityChainProxy((Assets.ColonyShared.SharedCode.Entities.Service.IUnityCheatServiceEntity)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class UnityCheatServiceEntityClientFullChainProxy : IChainedEntity
{
    private UnityCheatServiceEntityChainProxy __chain__;
    public UnityCheatServiceEntityClientFullChainProxy(UnityCheatServiceEntityChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public UnityCheatServiceEntityClientFullChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public UnityCheatServiceEntityClientFullChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public UnityCheatServiceEntityClientFullChainProxy StoreResult(string name)
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
    public static UnityCheatServiceEntityClientFullChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IUnityCheatServiceEntityClientFull entity)
    {
        return new UnityCheatServiceEntityClientFullChainProxy(new UnityCheatServiceEntityChainProxy((Assets.ColonyShared.SharedCode.Entities.Service.IUnityCheatServiceEntity)entity.GetBaseDeltaObject()));
    }

    public static UnityCheatServiceEntityClientFullChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IUnityCheatServiceEntityClientFull entity, IChainedEntity fromChain)
    {
        return new UnityCheatServiceEntityClientFullChainProxy(new UnityCheatServiceEntityChainProxy((Assets.ColonyShared.SharedCode.Entities.Service.IUnityCheatServiceEntity)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class UnityCheatServiceEntityServerApiChainProxy : IChainedEntity
{
    private UnityCheatServiceEntityChainProxy __chain__;
    public UnityCheatServiceEntityServerApiChainProxy(UnityCheatServiceEntityChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public UnityCheatServiceEntityServerApiChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public UnityCheatServiceEntityServerApiChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public UnityCheatServiceEntityServerApiChainProxy StoreResult(string name)
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
    public static UnityCheatServiceEntityServerApiChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IUnityCheatServiceEntityServerApi entity)
    {
        return new UnityCheatServiceEntityServerApiChainProxy(new UnityCheatServiceEntityChainProxy((Assets.ColonyShared.SharedCode.Entities.Service.IUnityCheatServiceEntity)entity.GetBaseDeltaObject()));
    }

    public static UnityCheatServiceEntityServerApiChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IUnityCheatServiceEntityServerApi entity, IChainedEntity fromChain)
    {
        return new UnityCheatServiceEntityServerApiChainProxy(new UnityCheatServiceEntityChainProxy((Assets.ColonyShared.SharedCode.Entities.Service.IUnityCheatServiceEntity)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class UnityCheatServiceEntityServerChainProxy : IChainedEntity
{
    private UnityCheatServiceEntityChainProxy __chain__;
    public UnityCheatServiceEntityServerChainProxy(UnityCheatServiceEntityChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public UnityCheatServiceEntityServerChainProxy MainUnityThreadOnServerSleep(ChainArgument<bool> isOn, ChainArgument<float> sleepTime, ChainArgument<float> delayBeforeSleep, ChainArgument<float> repeatTime)
    {
        __chain__.MainUnityThreadOnServerSleep(isOn, sleepTime, delayBeforeSleep, repeatTime);
        return this;
    }

    public UnityCheatServiceEntityServerChainProxy SetCurveLoggerState(ChainArgument<bool> enabledStatus, ChainArgument<bool> dump, ChainArgument<string> loggerName, ChainArgument<System.Guid> dumpId)
    {
        __chain__.SetCurveLoggerState(enabledStatus, dump, loggerName, dumpId);
        return this;
    }

    public UnityCheatServiceEntityServerChainProxy GetClosestPlayerSpawnPointTransform(ChainArgument<SharedCode.Utils.Vector3> pos)
    {
        __chain__.GetClosestPlayerSpawnPointTransform(pos);
        return this;
    }

    public UnityCheatServiceEntityServerChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public UnityCheatServiceEntityServerChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public UnityCheatServiceEntityServerChainProxy StoreResult(string name)
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
    public static UnityCheatServiceEntityServerChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IUnityCheatServiceEntityServer entity)
    {
        return new UnityCheatServiceEntityServerChainProxy(new UnityCheatServiceEntityChainProxy((Assets.ColonyShared.SharedCode.Entities.Service.IUnityCheatServiceEntity)entity.GetBaseDeltaObject()));
    }

    public static UnityCheatServiceEntityServerChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IUnityCheatServiceEntityServer entity, IChainedEntity fromChain)
    {
        return new UnityCheatServiceEntityServerChainProxy(new UnityCheatServiceEntityChainProxy((Assets.ColonyShared.SharedCode.Entities.Service.IUnityCheatServiceEntity)entity.GetBaseDeltaObject(), fromChain));
    }
}