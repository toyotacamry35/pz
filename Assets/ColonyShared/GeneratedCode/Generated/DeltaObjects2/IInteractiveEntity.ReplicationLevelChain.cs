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
public class InteractiveEntityAlwaysChainProxy : IChainedEntity
{
    private InteractiveEntityChainProxy __chain__;
    public InteractiveEntityAlwaysChainProxy(InteractiveEntityChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public InteractiveEntityAlwaysChainProxy NameSet(ChainArgument<string> value)
    {
        __chain__.NameSet(value);
        return this;
    }

    public InteractiveEntityAlwaysChainProxy PrefabSet(ChainArgument<string> value)
    {
        __chain__.PrefabSet(value);
        return this;
    }

    public InteractiveEntityAlwaysChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public InteractiveEntityAlwaysChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public InteractiveEntityAlwaysChainProxy StoreResult(string name)
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
    public static InteractiveEntityAlwaysChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IInteractiveEntityAlways entity)
    {
        return new InteractiveEntityAlwaysChainProxy(new InteractiveEntityChainProxy((Assets.ColonyShared.SharedCode.Entities.IInteractiveEntity)entity.GetBaseDeltaObject()));
    }

    public static InteractiveEntityAlwaysChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IInteractiveEntityAlways entity, IChainedEntity fromChain)
    {
        return new InteractiveEntityAlwaysChainProxy(new InteractiveEntityChainProxy((Assets.ColonyShared.SharedCode.Entities.IInteractiveEntity)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class InteractiveEntityClientBroadcastChainProxy : IChainedEntity
{
    private InteractiveEntityChainProxy __chain__;
    public InteractiveEntityClientBroadcastChainProxy(InteractiveEntityChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public InteractiveEntityClientBroadcastChainProxy NameSet(ChainArgument<string> value)
    {
        __chain__.NameSet(value);
        return this;
    }

    public InteractiveEntityClientBroadcastChainProxy PrefabSet(ChainArgument<string> value)
    {
        __chain__.PrefabSet(value);
        return this;
    }

    public InteractiveEntityClientBroadcastChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public InteractiveEntityClientBroadcastChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public InteractiveEntityClientBroadcastChainProxy StoreResult(string name)
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
    public static InteractiveEntityClientBroadcastChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IInteractiveEntityClientBroadcast entity)
    {
        return new InteractiveEntityClientBroadcastChainProxy(new InteractiveEntityChainProxy((Assets.ColonyShared.SharedCode.Entities.IInteractiveEntity)entity.GetBaseDeltaObject()));
    }

    public static InteractiveEntityClientBroadcastChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IInteractiveEntityClientBroadcast entity, IChainedEntity fromChain)
    {
        return new InteractiveEntityClientBroadcastChainProxy(new InteractiveEntityChainProxy((Assets.ColonyShared.SharedCode.Entities.IInteractiveEntity)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class InteractiveEntityClientFullApiChainProxy : IChainedEntity
{
    private InteractiveEntityChainProxy __chain__;
    public InteractiveEntityClientFullApiChainProxy(InteractiveEntityChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public InteractiveEntityClientFullApiChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public InteractiveEntityClientFullApiChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public InteractiveEntityClientFullApiChainProxy StoreResult(string name)
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
    public static InteractiveEntityClientFullApiChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IInteractiveEntityClientFullApi entity)
    {
        return new InteractiveEntityClientFullApiChainProxy(new InteractiveEntityChainProxy((Assets.ColonyShared.SharedCode.Entities.IInteractiveEntity)entity.GetBaseDeltaObject()));
    }

    public static InteractiveEntityClientFullApiChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IInteractiveEntityClientFullApi entity, IChainedEntity fromChain)
    {
        return new InteractiveEntityClientFullApiChainProxy(new InteractiveEntityChainProxy((Assets.ColonyShared.SharedCode.Entities.IInteractiveEntity)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class InteractiveEntityClientFullChainProxy : IChainedEntity
{
    private InteractiveEntityChainProxy __chain__;
    public InteractiveEntityClientFullChainProxy(InteractiveEntityChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public InteractiveEntityClientFullChainProxy NameSet(ChainArgument<string> value)
    {
        __chain__.NameSet(value);
        return this;
    }

    public InteractiveEntityClientFullChainProxy PrefabSet(ChainArgument<string> value)
    {
        __chain__.PrefabSet(value);
        return this;
    }

    public InteractiveEntityClientFullChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public InteractiveEntityClientFullChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public InteractiveEntityClientFullChainProxy StoreResult(string name)
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
    public static InteractiveEntityClientFullChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IInteractiveEntityClientFull entity)
    {
        return new InteractiveEntityClientFullChainProxy(new InteractiveEntityChainProxy((Assets.ColonyShared.SharedCode.Entities.IInteractiveEntity)entity.GetBaseDeltaObject()));
    }

    public static InteractiveEntityClientFullChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IInteractiveEntityClientFull entity, IChainedEntity fromChain)
    {
        return new InteractiveEntityClientFullChainProxy(new InteractiveEntityChainProxy((Assets.ColonyShared.SharedCode.Entities.IInteractiveEntity)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class InteractiveEntityServerApiChainProxy : IChainedEntity
{
    private InteractiveEntityChainProxy __chain__;
    public InteractiveEntityServerApiChainProxy(InteractiveEntityChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public InteractiveEntityServerApiChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public InteractiveEntityServerApiChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public InteractiveEntityServerApiChainProxy StoreResult(string name)
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
    public static InteractiveEntityServerApiChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IInteractiveEntityServerApi entity)
    {
        return new InteractiveEntityServerApiChainProxy(new InteractiveEntityChainProxy((Assets.ColonyShared.SharedCode.Entities.IInteractiveEntity)entity.GetBaseDeltaObject()));
    }

    public static InteractiveEntityServerApiChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IInteractiveEntityServerApi entity, IChainedEntity fromChain)
    {
        return new InteractiveEntityServerApiChainProxy(new InteractiveEntityChainProxy((Assets.ColonyShared.SharedCode.Entities.IInteractiveEntity)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class InteractiveEntityServerChainProxy : IChainedEntity
{
    private InteractiveEntityChainProxy __chain__;
    public InteractiveEntityServerChainProxy(InteractiveEntityChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public InteractiveEntityServerChainProxy NameSet(ChainArgument<string> value)
    {
        __chain__.NameSet(value);
        return this;
    }

    public InteractiveEntityServerChainProxy PrefabSet(ChainArgument<string> value)
    {
        __chain__.PrefabSet(value);
        return this;
    }

    public InteractiveEntityServerChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public InteractiveEntityServerChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public InteractiveEntityServerChainProxy StoreResult(string name)
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
    public static InteractiveEntityServerChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IInteractiveEntityServer entity)
    {
        return new InteractiveEntityServerChainProxy(new InteractiveEntityChainProxy((Assets.ColonyShared.SharedCode.Entities.IInteractiveEntity)entity.GetBaseDeltaObject()));
    }

    public static InteractiveEntityServerChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IInteractiveEntityServer entity, IChainedEntity fromChain)
    {
        return new InteractiveEntityServerChainProxy(new InteractiveEntityChainProxy((Assets.ColonyShared.SharedCode.Entities.IInteractiveEntity)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class CorpseInteractiveEntityAlwaysChainProxy : IChainedEntity
{
    private CorpseInteractiveEntityChainProxy __chain__;
    public CorpseInteractiveEntityAlwaysChainProxy(CorpseInteractiveEntityChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public CorpseInteractiveEntityAlwaysChainProxy NameSet(ChainArgument<string> value)
    {
        __chain__.NameSet(value);
        return this;
    }

    public CorpseInteractiveEntityAlwaysChainProxy PrefabSet(ChainArgument<string> value)
    {
        __chain__.PrefabSet(value);
        return this;
    }

    public CorpseInteractiveEntityAlwaysChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public CorpseInteractiveEntityAlwaysChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public CorpseInteractiveEntityAlwaysChainProxy StoreResult(string name)
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
    public static CorpseInteractiveEntityAlwaysChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.ICorpseInteractiveEntityAlways entity)
    {
        return new CorpseInteractiveEntityAlwaysChainProxy(new CorpseInteractiveEntityChainProxy((Assets.ColonyShared.SharedCode.Entities.ICorpseInteractiveEntity)entity.GetBaseDeltaObject()));
    }

    public static CorpseInteractiveEntityAlwaysChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.ICorpseInteractiveEntityAlways entity, IChainedEntity fromChain)
    {
        return new CorpseInteractiveEntityAlwaysChainProxy(new CorpseInteractiveEntityChainProxy((Assets.ColonyShared.SharedCode.Entities.ICorpseInteractiveEntity)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class CorpseInteractiveEntityClientBroadcastChainProxy : IChainedEntity
{
    private CorpseInteractiveEntityChainProxy __chain__;
    public CorpseInteractiveEntityClientBroadcastChainProxy(CorpseInteractiveEntityChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public CorpseInteractiveEntityClientBroadcastChainProxy NameSet(ChainArgument<string> value)
    {
        __chain__.NameSet(value);
        return this;
    }

    public CorpseInteractiveEntityClientBroadcastChainProxy PrefabSet(ChainArgument<string> value)
    {
        __chain__.PrefabSet(value);
        return this;
    }

    public CorpseInteractiveEntityClientBroadcastChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public CorpseInteractiveEntityClientBroadcastChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public CorpseInteractiveEntityClientBroadcastChainProxy StoreResult(string name)
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
    public static CorpseInteractiveEntityClientBroadcastChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.ICorpseInteractiveEntityClientBroadcast entity)
    {
        return new CorpseInteractiveEntityClientBroadcastChainProxy(new CorpseInteractiveEntityChainProxy((Assets.ColonyShared.SharedCode.Entities.ICorpseInteractiveEntity)entity.GetBaseDeltaObject()));
    }

    public static CorpseInteractiveEntityClientBroadcastChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.ICorpseInteractiveEntityClientBroadcast entity, IChainedEntity fromChain)
    {
        return new CorpseInteractiveEntityClientBroadcastChainProxy(new CorpseInteractiveEntityChainProxy((Assets.ColonyShared.SharedCode.Entities.ICorpseInteractiveEntity)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class CorpseInteractiveEntityClientFullApiChainProxy : IChainedEntity
{
    private CorpseInteractiveEntityChainProxy __chain__;
    public CorpseInteractiveEntityClientFullApiChainProxy(CorpseInteractiveEntityChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public CorpseInteractiveEntityClientFullApiChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public CorpseInteractiveEntityClientFullApiChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public CorpseInteractiveEntityClientFullApiChainProxy StoreResult(string name)
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
    public static CorpseInteractiveEntityClientFullApiChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.ICorpseInteractiveEntityClientFullApi entity)
    {
        return new CorpseInteractiveEntityClientFullApiChainProxy(new CorpseInteractiveEntityChainProxy((Assets.ColonyShared.SharedCode.Entities.ICorpseInteractiveEntity)entity.GetBaseDeltaObject()));
    }

    public static CorpseInteractiveEntityClientFullApiChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.ICorpseInteractiveEntityClientFullApi entity, IChainedEntity fromChain)
    {
        return new CorpseInteractiveEntityClientFullApiChainProxy(new CorpseInteractiveEntityChainProxy((Assets.ColonyShared.SharedCode.Entities.ICorpseInteractiveEntity)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class CorpseInteractiveEntityClientFullChainProxy : IChainedEntity
{
    private CorpseInteractiveEntityChainProxy __chain__;
    public CorpseInteractiveEntityClientFullChainProxy(CorpseInteractiveEntityChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public CorpseInteractiveEntityClientFullChainProxy MoveAllItems(ChainArgument<SharedCode.EntitySystem.PropertyAddress> source, ChainArgument<SharedCode.EntitySystem.PropertyAddress> destination)
    {
        __chain__.MoveAllItems(source, destination);
        return this;
    }

    public CorpseInteractiveEntityClientFullChainProxy NameSet(ChainArgument<string> value)
    {
        __chain__.NameSet(value);
        return this;
    }

    public CorpseInteractiveEntityClientFullChainProxy PrefabSet(ChainArgument<string> value)
    {
        __chain__.PrefabSet(value);
        return this;
    }

    public CorpseInteractiveEntityClientFullChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public CorpseInteractiveEntityClientFullChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public CorpseInteractiveEntityClientFullChainProxy StoreResult(string name)
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
    public static CorpseInteractiveEntityClientFullChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.ICorpseInteractiveEntityClientFull entity)
    {
        return new CorpseInteractiveEntityClientFullChainProxy(new CorpseInteractiveEntityChainProxy((Assets.ColonyShared.SharedCode.Entities.ICorpseInteractiveEntity)entity.GetBaseDeltaObject()));
    }

    public static CorpseInteractiveEntityClientFullChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.ICorpseInteractiveEntityClientFull entity, IChainedEntity fromChain)
    {
        return new CorpseInteractiveEntityClientFullChainProxy(new CorpseInteractiveEntityChainProxy((Assets.ColonyShared.SharedCode.Entities.ICorpseInteractiveEntity)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class CorpseInteractiveEntityServerApiChainProxy : IChainedEntity
{
    private CorpseInteractiveEntityChainProxy __chain__;
    public CorpseInteractiveEntityServerApiChainProxy(CorpseInteractiveEntityChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public CorpseInteractiveEntityServerApiChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public CorpseInteractiveEntityServerApiChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public CorpseInteractiveEntityServerApiChainProxy StoreResult(string name)
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
    public static CorpseInteractiveEntityServerApiChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.ICorpseInteractiveEntityServerApi entity)
    {
        return new CorpseInteractiveEntityServerApiChainProxy(new CorpseInteractiveEntityChainProxy((Assets.ColonyShared.SharedCode.Entities.ICorpseInteractiveEntity)entity.GetBaseDeltaObject()));
    }

    public static CorpseInteractiveEntityServerApiChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.ICorpseInteractiveEntityServerApi entity, IChainedEntity fromChain)
    {
        return new CorpseInteractiveEntityServerApiChainProxy(new CorpseInteractiveEntityChainProxy((Assets.ColonyShared.SharedCode.Entities.ICorpseInteractiveEntity)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class CorpseInteractiveEntityServerChainProxy : IChainedEntity
{
    private CorpseInteractiveEntityChainProxy __chain__;
    public CorpseInteractiveEntityServerChainProxy(CorpseInteractiveEntityChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public CorpseInteractiveEntityServerChainProxy MoveAllItems(ChainArgument<SharedCode.EntitySystem.PropertyAddress> source, ChainArgument<SharedCode.EntitySystem.PropertyAddress> destination)
    {
        __chain__.MoveAllItems(source, destination);
        return this;
    }

    public CorpseInteractiveEntityServerChainProxy NameSet(ChainArgument<string> value)
    {
        __chain__.NameSet(value);
        return this;
    }

    public CorpseInteractiveEntityServerChainProxy PrefabSet(ChainArgument<string> value)
    {
        __chain__.PrefabSet(value);
        return this;
    }

    public CorpseInteractiveEntityServerChainProxy GetOpenOuterRef(ChainArgument<ResourceSystem.Utils.OuterRef> oref)
    {
        __chain__.GetOpenOuterRef(oref);
        return this;
    }

    public CorpseInteractiveEntityServerChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public CorpseInteractiveEntityServerChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public CorpseInteractiveEntityServerChainProxy StoreResult(string name)
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
    public static CorpseInteractiveEntityServerChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.ICorpseInteractiveEntityServer entity)
    {
        return new CorpseInteractiveEntityServerChainProxy(new CorpseInteractiveEntityChainProxy((Assets.ColonyShared.SharedCode.Entities.ICorpseInteractiveEntity)entity.GetBaseDeltaObject()));
    }

    public static CorpseInteractiveEntityServerChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.ICorpseInteractiveEntityServer entity, IChainedEntity fromChain)
    {
        return new CorpseInteractiveEntityServerChainProxy(new CorpseInteractiveEntityChainProxy((Assets.ColonyShared.SharedCode.Entities.ICorpseInteractiveEntity)entity.GetBaseDeltaObject(), fromChain));
    }
}