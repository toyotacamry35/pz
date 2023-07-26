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
public class ObeliskAlwaysChainProxy : IChainedEntity
{
    private ObeliskChainProxy __chain__;
    public ObeliskAlwaysChainProxy(ObeliskChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public ObeliskAlwaysChainProxy NameSet(ChainArgument<string> value)
    {
        __chain__.NameSet(value);
        return this;
    }

    public ObeliskAlwaysChainProxy PrefabSet(ChainArgument<string> value)
    {
        __chain__.PrefabSet(value);
        return this;
    }

    public ObeliskAlwaysChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public ObeliskAlwaysChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public ObeliskAlwaysChainProxy StoreResult(string name)
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
    public static ObeliskAlwaysChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IObeliskAlways entity)
    {
        return new ObeliskAlwaysChainProxy(new ObeliskChainProxy((GeneratedCode.DeltaObjects.IObelisk)entity.GetBaseDeltaObject()));
    }

    public static ObeliskAlwaysChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IObeliskAlways entity, IChainedEntity fromChain)
    {
        return new ObeliskAlwaysChainProxy(new ObeliskChainProxy((GeneratedCode.DeltaObjects.IObelisk)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class ObeliskClientBroadcastChainProxy : IChainedEntity
{
    private ObeliskChainProxy __chain__;
    public ObeliskClientBroadcastChainProxy(ObeliskChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public ObeliskClientBroadcastChainProxy NameSet(ChainArgument<string> value)
    {
        __chain__.NameSet(value);
        return this;
    }

    public ObeliskClientBroadcastChainProxy PrefabSet(ChainArgument<string> value)
    {
        __chain__.PrefabSet(value);
        return this;
    }

    public ObeliskClientBroadcastChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public ObeliskClientBroadcastChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public ObeliskClientBroadcastChainProxy StoreResult(string name)
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
    public static ObeliskClientBroadcastChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IObeliskClientBroadcast entity)
    {
        return new ObeliskClientBroadcastChainProxy(new ObeliskChainProxy((GeneratedCode.DeltaObjects.IObelisk)entity.GetBaseDeltaObject()));
    }

    public static ObeliskClientBroadcastChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IObeliskClientBroadcast entity, IChainedEntity fromChain)
    {
        return new ObeliskClientBroadcastChainProxy(new ObeliskChainProxy((GeneratedCode.DeltaObjects.IObelisk)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class ObeliskClientFullApiChainProxy : IChainedEntity
{
    private ObeliskChainProxy __chain__;
    public ObeliskClientFullApiChainProxy(ObeliskChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public ObeliskClientFullApiChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public ObeliskClientFullApiChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public ObeliskClientFullApiChainProxy StoreResult(string name)
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
    public static ObeliskClientFullApiChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IObeliskClientFullApi entity)
    {
        return new ObeliskClientFullApiChainProxy(new ObeliskChainProxy((GeneratedCode.DeltaObjects.IObelisk)entity.GetBaseDeltaObject()));
    }

    public static ObeliskClientFullApiChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IObeliskClientFullApi entity, IChainedEntity fromChain)
    {
        return new ObeliskClientFullApiChainProxy(new ObeliskChainProxy((GeneratedCode.DeltaObjects.IObelisk)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class ObeliskClientFullChainProxy : IChainedEntity
{
    private ObeliskChainProxy __chain__;
    public ObeliskClientFullChainProxy(ObeliskChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public ObeliskClientFullChainProxy NameSet(ChainArgument<string> value)
    {
        __chain__.NameSet(value);
        return this;
    }

    public ObeliskClientFullChainProxy PrefabSet(ChainArgument<string> value)
    {
        __chain__.PrefabSet(value);
        return this;
    }

    public ObeliskClientFullChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public ObeliskClientFullChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public ObeliskClientFullChainProxy StoreResult(string name)
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
    public static ObeliskClientFullChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IObeliskClientFull entity)
    {
        return new ObeliskClientFullChainProxy(new ObeliskChainProxy((GeneratedCode.DeltaObjects.IObelisk)entity.GetBaseDeltaObject()));
    }

    public static ObeliskClientFullChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IObeliskClientFull entity, IChainedEntity fromChain)
    {
        return new ObeliskClientFullChainProxy(new ObeliskChainProxy((GeneratedCode.DeltaObjects.IObelisk)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class ObeliskServerApiChainProxy : IChainedEntity
{
    private ObeliskChainProxy __chain__;
    public ObeliskServerApiChainProxy(ObeliskChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public ObeliskServerApiChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public ObeliskServerApiChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public ObeliskServerApiChainProxy StoreResult(string name)
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
    public static ObeliskServerApiChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IObeliskServerApi entity)
    {
        return new ObeliskServerApiChainProxy(new ObeliskChainProxy((GeneratedCode.DeltaObjects.IObelisk)entity.GetBaseDeltaObject()));
    }

    public static ObeliskServerApiChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IObeliskServerApi entity, IChainedEntity fromChain)
    {
        return new ObeliskServerApiChainProxy(new ObeliskChainProxy((GeneratedCode.DeltaObjects.IObelisk)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class ObeliskServerChainProxy : IChainedEntity
{
    private ObeliskChainProxy __chain__;
    public ObeliskServerChainProxy(ObeliskChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public ObeliskServerChainProxy InvokeHitZonesDamageReceivedEvent(ChainArgument<Assets.ColonyShared.SharedCode.Aspects.Damage.Damage> damage)
    {
        __chain__.InvokeHitZonesDamageReceivedEvent(damage);
        return this;
    }

    public ObeliskServerChainProxy NameSet(ChainArgument<string> value)
    {
        __chain__.NameSet(value);
        return this;
    }

    public ObeliskServerChainProxy PrefabSet(ChainArgument<string> value)
    {
        __chain__.PrefabSet(value);
        return this;
    }

    public ObeliskServerChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public ObeliskServerChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public ObeliskServerChainProxy StoreResult(string name)
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
    public static ObeliskServerChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IObeliskServer entity)
    {
        return new ObeliskServerChainProxy(new ObeliskChainProxy((GeneratedCode.DeltaObjects.IObelisk)entity.GetBaseDeltaObject()));
    }

    public static ObeliskServerChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IObeliskServer entity, IChainedEntity fromChain)
    {
        return new ObeliskServerChainProxy(new ObeliskChainProxy((GeneratedCode.DeltaObjects.IObelisk)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class AltarAlwaysChainProxy : IChainedEntity
{
    private AltarChainProxy __chain__;
    public AltarAlwaysChainProxy(AltarChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public AltarAlwaysChainProxy NameSet(ChainArgument<string> value)
    {
        __chain__.NameSet(value);
        return this;
    }

    public AltarAlwaysChainProxy PrefabSet(ChainArgument<string> value)
    {
        __chain__.PrefabSet(value);
        return this;
    }

    public AltarAlwaysChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public AltarAlwaysChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public AltarAlwaysChainProxy StoreResult(string name)
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
    public static AltarAlwaysChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IAltarAlways entity)
    {
        return new AltarAlwaysChainProxy(new AltarChainProxy((GeneratedCode.DeltaObjects.IAltar)entity.GetBaseDeltaObject()));
    }

    public static AltarAlwaysChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IAltarAlways entity, IChainedEntity fromChain)
    {
        return new AltarAlwaysChainProxy(new AltarChainProxy((GeneratedCode.DeltaObjects.IAltar)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class AltarClientBroadcastChainProxy : IChainedEntity
{
    private AltarChainProxy __chain__;
    public AltarClientBroadcastChainProxy(AltarChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public AltarClientBroadcastChainProxy NameSet(ChainArgument<string> value)
    {
        __chain__.NameSet(value);
        return this;
    }

    public AltarClientBroadcastChainProxy PrefabSet(ChainArgument<string> value)
    {
        __chain__.PrefabSet(value);
        return this;
    }

    public AltarClientBroadcastChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public AltarClientBroadcastChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public AltarClientBroadcastChainProxy StoreResult(string name)
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
    public static AltarClientBroadcastChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IAltarClientBroadcast entity)
    {
        return new AltarClientBroadcastChainProxy(new AltarChainProxy((GeneratedCode.DeltaObjects.IAltar)entity.GetBaseDeltaObject()));
    }

    public static AltarClientBroadcastChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IAltarClientBroadcast entity, IChainedEntity fromChain)
    {
        return new AltarClientBroadcastChainProxy(new AltarChainProxy((GeneratedCode.DeltaObjects.IAltar)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class AltarClientFullApiChainProxy : IChainedEntity
{
    private AltarChainProxy __chain__;
    public AltarClientFullApiChainProxy(AltarChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public AltarClientFullApiChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public AltarClientFullApiChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public AltarClientFullApiChainProxy StoreResult(string name)
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
    public static AltarClientFullApiChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IAltarClientFullApi entity)
    {
        return new AltarClientFullApiChainProxy(new AltarChainProxy((GeneratedCode.DeltaObjects.IAltar)entity.GetBaseDeltaObject()));
    }

    public static AltarClientFullApiChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IAltarClientFullApi entity, IChainedEntity fromChain)
    {
        return new AltarClientFullApiChainProxy(new AltarChainProxy((GeneratedCode.DeltaObjects.IAltar)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class AltarClientFullChainProxy : IChainedEntity
{
    private AltarChainProxy __chain__;
    public AltarClientFullChainProxy(AltarChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public AltarClientFullChainProxy NameSet(ChainArgument<string> value)
    {
        __chain__.NameSet(value);
        return this;
    }

    public AltarClientFullChainProxy PrefabSet(ChainArgument<string> value)
    {
        __chain__.PrefabSet(value);
        return this;
    }

    public AltarClientFullChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public AltarClientFullChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public AltarClientFullChainProxy StoreResult(string name)
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
    public static AltarClientFullChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IAltarClientFull entity)
    {
        return new AltarClientFullChainProxy(new AltarChainProxy((GeneratedCode.DeltaObjects.IAltar)entity.GetBaseDeltaObject()));
    }

    public static AltarClientFullChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IAltarClientFull entity, IChainedEntity fromChain)
    {
        return new AltarClientFullChainProxy(new AltarChainProxy((GeneratedCode.DeltaObjects.IAltar)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class AltarServerApiChainProxy : IChainedEntity
{
    private AltarChainProxy __chain__;
    public AltarServerApiChainProxy(AltarChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public AltarServerApiChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public AltarServerApiChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public AltarServerApiChainProxy StoreResult(string name)
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
    public static AltarServerApiChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IAltarServerApi entity)
    {
        return new AltarServerApiChainProxy(new AltarChainProxy((GeneratedCode.DeltaObjects.IAltar)entity.GetBaseDeltaObject()));
    }

    public static AltarServerApiChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IAltarServerApi entity, IChainedEntity fromChain)
    {
        return new AltarServerApiChainProxy(new AltarChainProxy((GeneratedCode.DeltaObjects.IAltar)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class AltarServerChainProxy : IChainedEntity
{
    private AltarChainProxy __chain__;
    public AltarServerChainProxy(AltarChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public AltarServerChainProxy InvokeHitZonesDamageReceivedEvent(ChainArgument<Assets.ColonyShared.SharedCode.Aspects.Damage.Damage> damage)
    {
        __chain__.InvokeHitZonesDamageReceivedEvent(damage);
        return this;
    }

    public AltarServerChainProxy NameSet(ChainArgument<string> value)
    {
        __chain__.NameSet(value);
        return this;
    }

    public AltarServerChainProxy PrefabSet(ChainArgument<string> value)
    {
        __chain__.PrefabSet(value);
        return this;
    }

    public AltarServerChainProxy GetOpenOuterRef(ChainArgument<ResourceSystem.Utils.OuterRef> oref)
    {
        __chain__.GetOpenOuterRef(oref);
        return this;
    }

    public AltarServerChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public AltarServerChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public AltarServerChainProxy StoreResult(string name)
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
    public static AltarServerChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IAltarServer entity)
    {
        return new AltarServerChainProxy(new AltarChainProxy((GeneratedCode.DeltaObjects.IAltar)entity.GetBaseDeltaObject()));
    }

    public static AltarServerChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IAltarServer entity, IChainedEntity fromChain)
    {
        return new AltarServerChainProxy(new AltarChainProxy((GeneratedCode.DeltaObjects.IAltar)entity.GetBaseDeltaObject(), fromChain));
    }
}