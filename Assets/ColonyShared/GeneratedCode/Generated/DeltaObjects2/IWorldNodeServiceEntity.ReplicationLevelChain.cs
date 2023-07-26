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
public class WorldNodeServiceEntityAlwaysChainProxy : IChainedEntity
{
    private WorldNodeServiceEntityChainProxy __chain__;
    public WorldNodeServiceEntityAlwaysChainProxy(WorldNodeServiceEntityChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public WorldNodeServiceEntityAlwaysChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public WorldNodeServiceEntityAlwaysChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public WorldNodeServiceEntityAlwaysChainProxy StoreResult(string name)
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
    public static WorldNodeServiceEntityAlwaysChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldNodeServiceEntityAlways entity)
    {
        return new WorldNodeServiceEntityAlwaysChainProxy(new WorldNodeServiceEntityChainProxy((SharedCode.Entities.Service.IWorldNodeServiceEntity)entity.GetBaseDeltaObject()));
    }

    public static WorldNodeServiceEntityAlwaysChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldNodeServiceEntityAlways entity, IChainedEntity fromChain)
    {
        return new WorldNodeServiceEntityAlwaysChainProxy(new WorldNodeServiceEntityChainProxy((SharedCode.Entities.Service.IWorldNodeServiceEntity)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class WorldNodeServiceEntityClientBroadcastChainProxy : IChainedEntity
{
    private WorldNodeServiceEntityChainProxy __chain__;
    public WorldNodeServiceEntityClientBroadcastChainProxy(WorldNodeServiceEntityChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public WorldNodeServiceEntityClientBroadcastChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public WorldNodeServiceEntityClientBroadcastChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public WorldNodeServiceEntityClientBroadcastChainProxy StoreResult(string name)
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
    public static WorldNodeServiceEntityClientBroadcastChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldNodeServiceEntityClientBroadcast entity)
    {
        return new WorldNodeServiceEntityClientBroadcastChainProxy(new WorldNodeServiceEntityChainProxy((SharedCode.Entities.Service.IWorldNodeServiceEntity)entity.GetBaseDeltaObject()));
    }

    public static WorldNodeServiceEntityClientBroadcastChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldNodeServiceEntityClientBroadcast entity, IChainedEntity fromChain)
    {
        return new WorldNodeServiceEntityClientBroadcastChainProxy(new WorldNodeServiceEntityChainProxy((SharedCode.Entities.Service.IWorldNodeServiceEntity)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class WorldNodeServiceEntityClientFullApiChainProxy : IChainedEntity
{
    private WorldNodeServiceEntityChainProxy __chain__;
    public WorldNodeServiceEntityClientFullApiChainProxy(WorldNodeServiceEntityChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public WorldNodeServiceEntityClientFullApiChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public WorldNodeServiceEntityClientFullApiChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public WorldNodeServiceEntityClientFullApiChainProxy StoreResult(string name)
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
    public static WorldNodeServiceEntityClientFullApiChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldNodeServiceEntityClientFullApi entity)
    {
        return new WorldNodeServiceEntityClientFullApiChainProxy(new WorldNodeServiceEntityChainProxy((SharedCode.Entities.Service.IWorldNodeServiceEntity)entity.GetBaseDeltaObject()));
    }

    public static WorldNodeServiceEntityClientFullApiChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldNodeServiceEntityClientFullApi entity, IChainedEntity fromChain)
    {
        return new WorldNodeServiceEntityClientFullApiChainProxy(new WorldNodeServiceEntityChainProxy((SharedCode.Entities.Service.IWorldNodeServiceEntity)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class WorldNodeServiceEntityClientFullChainProxy : IChainedEntity
{
    private WorldNodeServiceEntityChainProxy __chain__;
    public WorldNodeServiceEntityClientFullChainProxy(WorldNodeServiceEntityChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public WorldNodeServiceEntityClientFullChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public WorldNodeServiceEntityClientFullChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public WorldNodeServiceEntityClientFullChainProxy StoreResult(string name)
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
    public static WorldNodeServiceEntityClientFullChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldNodeServiceEntityClientFull entity)
    {
        return new WorldNodeServiceEntityClientFullChainProxy(new WorldNodeServiceEntityChainProxy((SharedCode.Entities.Service.IWorldNodeServiceEntity)entity.GetBaseDeltaObject()));
    }

    public static WorldNodeServiceEntityClientFullChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldNodeServiceEntityClientFull entity, IChainedEntity fromChain)
    {
        return new WorldNodeServiceEntityClientFullChainProxy(new WorldNodeServiceEntityChainProxy((SharedCode.Entities.Service.IWorldNodeServiceEntity)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class WorldNodeServiceEntityServerApiChainProxy : IChainedEntity
{
    private WorldNodeServiceEntityChainProxy __chain__;
    public WorldNodeServiceEntityServerApiChainProxy(WorldNodeServiceEntityChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public WorldNodeServiceEntityServerApiChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public WorldNodeServiceEntityServerApiChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public WorldNodeServiceEntityServerApiChainProxy StoreResult(string name)
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
    public static WorldNodeServiceEntityServerApiChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldNodeServiceEntityServerApi entity)
    {
        return new WorldNodeServiceEntityServerApiChainProxy(new WorldNodeServiceEntityChainProxy((SharedCode.Entities.Service.IWorldNodeServiceEntity)entity.GetBaseDeltaObject()));
    }

    public static WorldNodeServiceEntityServerApiChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldNodeServiceEntityServerApi entity, IChainedEntity fromChain)
    {
        return new WorldNodeServiceEntityServerApiChainProxy(new WorldNodeServiceEntityChainProxy((SharedCode.Entities.Service.IWorldNodeServiceEntity)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class WorldNodeServiceEntityServerChainProxy : IChainedEntity
{
    private WorldNodeServiceEntityChainProxy __chain__;
    public WorldNodeServiceEntityServerChainProxy(WorldNodeServiceEntityChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public WorldNodeServiceEntityServerChainProxy IsReady()
    {
        __chain__.IsReady();
        return this;
    }

    public WorldNodeServiceEntityServerChainProxy HostUnityMapChunk(ChainArgument<GeneratedCode.Custom.Config.MapDef> mapChunk)
    {
        __chain__.HostUnityMapChunk(mapChunk);
        return this;
    }

    public WorldNodeServiceEntityServerChainProxy HostUnityMapChunk(ChainArgument<GeneratedCode.Custom.Config.MapDef> mapChunk, ChainArgument<System.Guid> mapChunkId, ChainArgument<System.Guid> mapInstanceId, ChainArgument<System.Guid> mapInstanceRepositoryId)
    {
        __chain__.HostUnityMapChunk(mapChunk, mapChunkId, mapInstanceId, mapInstanceRepositoryId);
        return this;
    }

    public WorldNodeServiceEntityServerChainProxy HostedUnityMapChunk(ChainArgument<GeneratedCode.Custom.Config.MapDef> mapChunk, ChainArgument<System.Guid> mapChunkId, ChainArgument<System.Guid> mapInstanceId)
    {
        __chain__.HostedUnityMapChunk(mapChunk, mapChunkId, mapInstanceId);
        return this;
    }

    public WorldNodeServiceEntityServerChainProxy CanBuildHere(ChainArgument<SharedCode.Entities.GameObjectEntities.IEntityObjectDef> entityObjectDef, ChainArgument<SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>> ent, ChainArgument<SharedCode.Utils.Vector3> position, ChainArgument<SharedCode.Utils.Vector3> scale, ChainArgument<SharedCode.Utils.Quaternion> rotation)
    {
        __chain__.CanBuildHere(entityObjectDef, ent, position, scale, rotation);
        return this;
    }

    public WorldNodeServiceEntityServerChainProxy GetDropPosition(ChainArgument<SharedCode.Utils.Vector3> playerPosition, ChainArgument<SharedCode.Utils.Quaternion> playerRotation)
    {
        __chain__.GetDropPosition(playerPosition, playerRotation);
        return this;
    }

    public WorldNodeServiceEntityServerChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public WorldNodeServiceEntityServerChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public WorldNodeServiceEntityServerChainProxy StoreResult(string name)
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
    public static WorldNodeServiceEntityServerChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldNodeServiceEntityServer entity)
    {
        return new WorldNodeServiceEntityServerChainProxy(new WorldNodeServiceEntityChainProxy((SharedCode.Entities.Service.IWorldNodeServiceEntity)entity.GetBaseDeltaObject()));
    }

    public static WorldNodeServiceEntityServerChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldNodeServiceEntityServer entity, IChainedEntity fromChain)
    {
        return new WorldNodeServiceEntityServerChainProxy(new WorldNodeServiceEntityChainProxy((SharedCode.Entities.Service.IWorldNodeServiceEntity)entity.GetBaseDeltaObject(), fromChain));
    }
}