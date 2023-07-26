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
public class BankCellAlwaysChainProxy : IChainedEntity
{
    private BankCellChainProxy __chain__;
    public BankCellAlwaysChainProxy(BankCellChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public BankCellAlwaysChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public BankCellAlwaysChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public BankCellAlwaysChainProxy StoreResult(string name)
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
    public static BankCellAlwaysChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IBankCellAlways entity)
    {
        return new BankCellAlwaysChainProxy(new BankCellChainProxy((SharedCode.Entities.IBankCell)entity.GetBaseDeltaObject()));
    }

    public static BankCellAlwaysChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IBankCellAlways entity, IChainedEntity fromChain)
    {
        return new BankCellAlwaysChainProxy(new BankCellChainProxy((SharedCode.Entities.IBankCell)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class BankCellClientBroadcastChainProxy : IChainedEntity
{
    private BankCellChainProxy __chain__;
    public BankCellClientBroadcastChainProxy(BankCellChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public BankCellClientBroadcastChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public BankCellClientBroadcastChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public BankCellClientBroadcastChainProxy StoreResult(string name)
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
    public static BankCellClientBroadcastChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IBankCellClientBroadcast entity)
    {
        return new BankCellClientBroadcastChainProxy(new BankCellChainProxy((SharedCode.Entities.IBankCell)entity.GetBaseDeltaObject()));
    }

    public static BankCellClientBroadcastChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IBankCellClientBroadcast entity, IChainedEntity fromChain)
    {
        return new BankCellClientBroadcastChainProxy(new BankCellChainProxy((SharedCode.Entities.IBankCell)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class BankCellClientFullApiChainProxy : IChainedEntity
{
    private BankCellChainProxy __chain__;
    public BankCellClientFullApiChainProxy(BankCellChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public BankCellClientFullApiChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public BankCellClientFullApiChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public BankCellClientFullApiChainProxy StoreResult(string name)
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
    public static BankCellClientFullApiChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IBankCellClientFullApi entity)
    {
        return new BankCellClientFullApiChainProxy(new BankCellChainProxy((SharedCode.Entities.IBankCell)entity.GetBaseDeltaObject()));
    }

    public static BankCellClientFullApiChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IBankCellClientFullApi entity, IChainedEntity fromChain)
    {
        return new BankCellClientFullApiChainProxy(new BankCellChainProxy((SharedCode.Entities.IBankCell)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class BankCellClientFullChainProxy : IChainedEntity
{
    private BankCellChainProxy __chain__;
    public BankCellClientFullChainProxy(BankCellChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public BankCellClientFullChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public BankCellClientFullChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public BankCellClientFullChainProxy StoreResult(string name)
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
    public static BankCellClientFullChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IBankCellClientFull entity)
    {
        return new BankCellClientFullChainProxy(new BankCellChainProxy((SharedCode.Entities.IBankCell)entity.GetBaseDeltaObject()));
    }

    public static BankCellClientFullChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IBankCellClientFull entity, IChainedEntity fromChain)
    {
        return new BankCellClientFullChainProxy(new BankCellChainProxy((SharedCode.Entities.IBankCell)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class BankCellServerApiChainProxy : IChainedEntity
{
    private BankCellChainProxy __chain__;
    public BankCellServerApiChainProxy(BankCellChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public BankCellServerApiChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public BankCellServerApiChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public BankCellServerApiChainProxy StoreResult(string name)
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
    public static BankCellServerApiChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IBankCellServerApi entity)
    {
        return new BankCellServerApiChainProxy(new BankCellChainProxy((SharedCode.Entities.IBankCell)entity.GetBaseDeltaObject()));
    }

    public static BankCellServerApiChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IBankCellServerApi entity, IChainedEntity fromChain)
    {
        return new BankCellServerApiChainProxy(new BankCellChainProxy((SharedCode.Entities.IBankCell)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class BankCellServerChainProxy : IChainedEntity
{
    private BankCellChainProxy __chain__;
    public BankCellServerChainProxy(BankCellChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public BankCellServerChainProxy GetOpenOuterRef(ChainArgument<ResourceSystem.Utils.OuterRef> oref)
    {
        __chain__.GetOpenOuterRef(oref);
        return this;
    }

    public BankCellServerChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public BankCellServerChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public BankCellServerChainProxy StoreResult(string name)
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
    public static BankCellServerChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IBankCellServer entity)
    {
        return new BankCellServerChainProxy(new BankCellChainProxy((SharedCode.Entities.IBankCell)entity.GetBaseDeltaObject()));
    }

    public static BankCellServerChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IBankCellServer entity, IChainedEntity fromChain)
    {
        return new BankCellServerChainProxy(new BankCellChainProxy((SharedCode.Entities.IBankCell)entity.GetBaseDeltaObject(), fromChain));
    }
}