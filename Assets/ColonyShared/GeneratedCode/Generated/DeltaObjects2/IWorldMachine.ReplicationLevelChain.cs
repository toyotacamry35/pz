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
public class WorldMachineAlwaysChainProxy : IChainedEntity
{
    private WorldMachineChainProxy __chain__;
    public WorldMachineAlwaysChainProxy(WorldMachineChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public WorldMachineAlwaysChainProxy NameSet(ChainArgument<string> value)
    {
        __chain__.NameSet(value);
        return this;
    }

    public WorldMachineAlwaysChainProxy PrefabSet(ChainArgument<string> value)
    {
        __chain__.PrefabSet(value);
        return this;
    }

    public WorldMachineAlwaysChainProxy CraftProgressInfoSet(ChainArgument<SharedCode.DeltaObjects.ICraftProgressInfo> value)
    {
        __chain__.CraftProgressInfoSet(value);
        return this;
    }

    public WorldMachineAlwaysChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public WorldMachineAlwaysChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public WorldMachineAlwaysChainProxy StoreResult(string name)
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
    public static WorldMachineAlwaysChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldMachineAlways entity)
    {
        return new WorldMachineAlwaysChainProxy(new WorldMachineChainProxy((SharedCode.Entities.IWorldMachine)entity.GetBaseDeltaObject()));
    }

    public static WorldMachineAlwaysChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldMachineAlways entity, IChainedEntity fromChain)
    {
        return new WorldMachineAlwaysChainProxy(new WorldMachineChainProxy((SharedCode.Entities.IWorldMachine)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class WorldMachineClientBroadcastChainProxy : IChainedEntity
{
    private WorldMachineChainProxy __chain__;
    public WorldMachineClientBroadcastChainProxy(WorldMachineChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public WorldMachineClientBroadcastChainProxy NameSet(ChainArgument<string> value)
    {
        __chain__.NameSet(value);
        return this;
    }

    public WorldMachineClientBroadcastChainProxy PrefabSet(ChainArgument<string> value)
    {
        __chain__.PrefabSet(value);
        return this;
    }

    public WorldMachineClientBroadcastChainProxy SetActive(ChainArgument<bool> activate)
    {
        __chain__.SetActive(activate);
        return this;
    }

    public WorldMachineClientBroadcastChainProxy CraftProgressInfoSet(ChainArgument<SharedCode.DeltaObjects.ICraftProgressInfo> value)
    {
        __chain__.CraftProgressInfoSet(value);
        return this;
    }

    public WorldMachineClientBroadcastChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public WorldMachineClientBroadcastChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public WorldMachineClientBroadcastChainProxy StoreResult(string name)
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
    public static WorldMachineClientBroadcastChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldMachineClientBroadcast entity)
    {
        return new WorldMachineClientBroadcastChainProxy(new WorldMachineChainProxy((SharedCode.Entities.IWorldMachine)entity.GetBaseDeltaObject()));
    }

    public static WorldMachineClientBroadcastChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldMachineClientBroadcast entity, IChainedEntity fromChain)
    {
        return new WorldMachineClientBroadcastChainProxy(new WorldMachineChainProxy((SharedCode.Entities.IWorldMachine)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class WorldMachineClientFullApiChainProxy : IChainedEntity
{
    private WorldMachineChainProxy __chain__;
    public WorldMachineClientFullApiChainProxy(WorldMachineChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public WorldMachineClientFullApiChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public WorldMachineClientFullApiChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public WorldMachineClientFullApiChainProxy StoreResult(string name)
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
    public static WorldMachineClientFullApiChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldMachineClientFullApi entity)
    {
        return new WorldMachineClientFullApiChainProxy(new WorldMachineChainProxy((SharedCode.Entities.IWorldMachine)entity.GetBaseDeltaObject()));
    }

    public static WorldMachineClientFullApiChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldMachineClientFullApi entity, IChainedEntity fromChain)
    {
        return new WorldMachineClientFullApiChainProxy(new WorldMachineChainProxy((SharedCode.Entities.IWorldMachine)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class WorldMachineClientFullChainProxy : IChainedEntity
{
    private WorldMachineChainProxy __chain__;
    public WorldMachineClientFullChainProxy(WorldMachineChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public WorldMachineClientFullChainProxy SetRecipeActivity(ChainArgument<Assets.ColonyShared.SharedCode.Aspects.Craft.CraftRecipeDef> recipeDef, ChainArgument<bool> activate)
    {
        __chain__.SetRecipeActivity(recipeDef, activate);
        return this;
    }

    public WorldMachineClientFullChainProxy SetRecipePriority(ChainArgument<Assets.ColonyShared.SharedCode.Aspects.Craft.CraftRecipeDef> recipeDef, ChainArgument<int> priority)
    {
        __chain__.SetRecipePriority(recipeDef, priority);
        return this;
    }

    public WorldMachineClientFullChainProxy NameSet(ChainArgument<string> value)
    {
        __chain__.NameSet(value);
        return this;
    }

    public WorldMachineClientFullChainProxy PrefabSet(ChainArgument<string> value)
    {
        __chain__.PrefabSet(value);
        return this;
    }

    public WorldMachineClientFullChainProxy SetActive(ChainArgument<bool> activate)
    {
        __chain__.SetActive(activate);
        return this;
    }

    public WorldMachineClientFullChainProxy CraftProgressInfoSet(ChainArgument<SharedCode.DeltaObjects.ICraftProgressInfo> value)
    {
        __chain__.CraftProgressInfoSet(value);
        return this;
    }

    public WorldMachineClientFullChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public WorldMachineClientFullChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public WorldMachineClientFullChainProxy StoreResult(string name)
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
    public static WorldMachineClientFullChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldMachineClientFull entity)
    {
        return new WorldMachineClientFullChainProxy(new WorldMachineChainProxy((SharedCode.Entities.IWorldMachine)entity.GetBaseDeltaObject()));
    }

    public static WorldMachineClientFullChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldMachineClientFull entity, IChainedEntity fromChain)
    {
        return new WorldMachineClientFullChainProxy(new WorldMachineChainProxy((SharedCode.Entities.IWorldMachine)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class WorldMachineServerApiChainProxy : IChainedEntity
{
    private WorldMachineChainProxy __chain__;
    public WorldMachineServerApiChainProxy(WorldMachineChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public WorldMachineServerApiChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public WorldMachineServerApiChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public WorldMachineServerApiChainProxy StoreResult(string name)
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
    public static WorldMachineServerApiChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldMachineServerApi entity)
    {
        return new WorldMachineServerApiChainProxy(new WorldMachineChainProxy((SharedCode.Entities.IWorldMachine)entity.GetBaseDeltaObject()));
    }

    public static WorldMachineServerApiChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldMachineServerApi entity, IChainedEntity fromChain)
    {
        return new WorldMachineServerApiChainProxy(new WorldMachineChainProxy((SharedCode.Entities.IWorldMachine)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class WorldMachineServerChainProxy : IChainedEntity
{
    private WorldMachineChainProxy __chain__;
    public WorldMachineServerChainProxy(WorldMachineChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public WorldMachineServerChainProxy SetRecipeActivity(ChainArgument<Assets.ColonyShared.SharedCode.Aspects.Craft.CraftRecipeDef> recipeDef, ChainArgument<bool> activate)
    {
        __chain__.SetRecipeActivity(recipeDef, activate);
        return this;
    }

    public WorldMachineServerChainProxy SetRecipePriority(ChainArgument<Assets.ColonyShared.SharedCode.Aspects.Craft.CraftRecipeDef> recipeDef, ChainArgument<int> priority)
    {
        __chain__.SetRecipePriority(recipeDef, priority);
        return this;
    }

    public WorldMachineServerChainProxy UpdateCraftProgressInfo()
    {
        __chain__.UpdateCraftProgressInfo();
        return this;
    }

    public WorldMachineServerChainProxy NameSet(ChainArgument<string> value)
    {
        __chain__.NameSet(value);
        return this;
    }

    public WorldMachineServerChainProxy PrefabSet(ChainArgument<string> value)
    {
        __chain__.PrefabSet(value);
        return this;
    }

    public WorldMachineServerChainProxy SetActive(ChainArgument<bool> activate)
    {
        __chain__.SetActive(activate);
        return this;
    }

    public WorldMachineServerChainProxy CraftProgressInfoSet(ChainArgument<SharedCode.DeltaObjects.ICraftProgressInfo> value)
    {
        __chain__.CraftProgressInfoSet(value);
        return this;
    }

    public WorldMachineServerChainProxy GetOpenOuterRef(ChainArgument<ResourceSystem.Utils.OuterRef> oref)
    {
        __chain__.GetOpenOuterRef(oref);
        return this;
    }

    public WorldMachineServerChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public WorldMachineServerChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public WorldMachineServerChainProxy StoreResult(string name)
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
    public static WorldMachineServerChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldMachineServer entity)
    {
        return new WorldMachineServerChainProxy(new WorldMachineChainProxy((SharedCode.Entities.IWorldMachine)entity.GetBaseDeltaObject()));
    }

    public static WorldMachineServerChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldMachineServer entity, IChainedEntity fromChain)
    {
        return new WorldMachineServerChainProxy(new WorldMachineChainProxy((SharedCode.Entities.IWorldMachine)entity.GetBaseDeltaObject(), fromChain));
    }
}