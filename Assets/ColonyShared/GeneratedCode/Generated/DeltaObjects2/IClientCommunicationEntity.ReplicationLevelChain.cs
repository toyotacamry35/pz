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
public class ClientCommunicationEntityAlwaysChainProxy : IChainedEntity
{
    private ClientCommunicationEntityChainProxy __chain__;
    public ClientCommunicationEntityAlwaysChainProxy(ClientCommunicationEntityChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public ClientCommunicationEntityAlwaysChainProxy SetLevelLoaded()
    {
        __chain__.SetLevelLoaded();
        return this;
    }

    public ClientCommunicationEntityAlwaysChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public ClientCommunicationEntityAlwaysChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public ClientCommunicationEntityAlwaysChainProxy StoreResult(string name)
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
    public static ClientCommunicationEntityAlwaysChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IClientCommunicationEntityAlways entity)
    {
        return new ClientCommunicationEntityAlwaysChainProxy(new ClientCommunicationEntityChainProxy((SharedCode.Entities.Cloud.IClientCommunicationEntity)entity.GetBaseDeltaObject()));
    }

    public static ClientCommunicationEntityAlwaysChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IClientCommunicationEntityAlways entity, IChainedEntity fromChain)
    {
        return new ClientCommunicationEntityAlwaysChainProxy(new ClientCommunicationEntityChainProxy((SharedCode.Entities.Cloud.IClientCommunicationEntity)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class ClientCommunicationEntityClientBroadcastChainProxy : IChainedEntity
{
    private ClientCommunicationEntityChainProxy __chain__;
    public ClientCommunicationEntityClientBroadcastChainProxy(ClientCommunicationEntityChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public ClientCommunicationEntityClientBroadcastChainProxy SetLevelLoaded()
    {
        __chain__.SetLevelLoaded();
        return this;
    }

    public ClientCommunicationEntityClientBroadcastChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public ClientCommunicationEntityClientBroadcastChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public ClientCommunicationEntityClientBroadcastChainProxy StoreResult(string name)
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
    public static ClientCommunicationEntityClientBroadcastChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IClientCommunicationEntityClientBroadcast entity)
    {
        return new ClientCommunicationEntityClientBroadcastChainProxy(new ClientCommunicationEntityChainProxy((SharedCode.Entities.Cloud.IClientCommunicationEntity)entity.GetBaseDeltaObject()));
    }

    public static ClientCommunicationEntityClientBroadcastChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IClientCommunicationEntityClientBroadcast entity, IChainedEntity fromChain)
    {
        return new ClientCommunicationEntityClientBroadcastChainProxy(new ClientCommunicationEntityChainProxy((SharedCode.Entities.Cloud.IClientCommunicationEntity)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class ClientCommunicationEntityClientFullApiChainProxy : IChainedEntity
{
    private ClientCommunicationEntityChainProxy __chain__;
    public ClientCommunicationEntityClientFullApiChainProxy(ClientCommunicationEntityChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public ClientCommunicationEntityClientFullApiChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public ClientCommunicationEntityClientFullApiChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public ClientCommunicationEntityClientFullApiChainProxy StoreResult(string name)
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
    public static ClientCommunicationEntityClientFullApiChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IClientCommunicationEntityClientFullApi entity)
    {
        return new ClientCommunicationEntityClientFullApiChainProxy(new ClientCommunicationEntityChainProxy((SharedCode.Entities.Cloud.IClientCommunicationEntity)entity.GetBaseDeltaObject()));
    }

    public static ClientCommunicationEntityClientFullApiChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IClientCommunicationEntityClientFullApi entity, IChainedEntity fromChain)
    {
        return new ClientCommunicationEntityClientFullApiChainProxy(new ClientCommunicationEntityChainProxy((SharedCode.Entities.Cloud.IClientCommunicationEntity)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class ClientCommunicationEntityClientFullChainProxy : IChainedEntity
{
    private ClientCommunicationEntityChainProxy __chain__;
    public ClientCommunicationEntityClientFullChainProxy(ClientCommunicationEntityChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public ClientCommunicationEntityClientFullChainProxy SetLevelLoaded()
    {
        __chain__.SetLevelLoaded();
        return this;
    }

    public ClientCommunicationEntityClientFullChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public ClientCommunicationEntityClientFullChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public ClientCommunicationEntityClientFullChainProxy StoreResult(string name)
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
    public static ClientCommunicationEntityClientFullChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IClientCommunicationEntityClientFull entity)
    {
        return new ClientCommunicationEntityClientFullChainProxy(new ClientCommunicationEntityChainProxy((SharedCode.Entities.Cloud.IClientCommunicationEntity)entity.GetBaseDeltaObject()));
    }

    public static ClientCommunicationEntityClientFullChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IClientCommunicationEntityClientFull entity, IChainedEntity fromChain)
    {
        return new ClientCommunicationEntityClientFullChainProxy(new ClientCommunicationEntityChainProxy((SharedCode.Entities.Cloud.IClientCommunicationEntity)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class ClientCommunicationEntityServerApiChainProxy : IChainedEntity
{
    private ClientCommunicationEntityChainProxy __chain__;
    public ClientCommunicationEntityServerApiChainProxy(ClientCommunicationEntityChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public ClientCommunicationEntityServerApiChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public ClientCommunicationEntityServerApiChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public ClientCommunicationEntityServerApiChainProxy StoreResult(string name)
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
    public static ClientCommunicationEntityServerApiChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IClientCommunicationEntityServerApi entity)
    {
        return new ClientCommunicationEntityServerApiChainProxy(new ClientCommunicationEntityChainProxy((SharedCode.Entities.Cloud.IClientCommunicationEntity)entity.GetBaseDeltaObject()));
    }

    public static ClientCommunicationEntityServerApiChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IClientCommunicationEntityServerApi entity, IChainedEntity fromChain)
    {
        return new ClientCommunicationEntityServerApiChainProxy(new ClientCommunicationEntityChainProxy((SharedCode.Entities.Cloud.IClientCommunicationEntity)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class ClientCommunicationEntityServerChainProxy : IChainedEntity
{
    private ClientCommunicationEntityChainProxy __chain__;
    public ClientCommunicationEntityServerChainProxy(ClientCommunicationEntityChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public ClientCommunicationEntityServerChainProxy SetLevelLoaded()
    {
        __chain__.SetLevelLoaded();
        return this;
    }

    public ClientCommunicationEntityServerChainProxy DisconnectByAnotherConnection()
    {
        __chain__.DisconnectByAnotherConnection();
        return this;
    }

    public ClientCommunicationEntityServerChainProxy GracefullLogout()
    {
        __chain__.GracefullLogout();
        return this;
    }

    public ClientCommunicationEntityServerChainProxy DisconnectByError(ChainArgument<string> reason, ChainArgument<string> details)
    {
        __chain__.DisconnectByError(reason, details);
        return this;
    }

    public ClientCommunicationEntityServerChainProxy ConfirmLogin()
    {
        __chain__.ConfirmLogin();
        return this;
    }

    public ClientCommunicationEntityServerChainProxy GetMapHostInitialInformation()
    {
        __chain__.GetMapHostInitialInformation();
        return this;
    }

    public ClientCommunicationEntityServerChainProxy AddConection(ChainArgument<string> host, ChainArgument<int> port, ChainArgument<System.Guid> nodeId)
    {
        __chain__.AddConection(host, port, nodeId);
        return this;
    }

    public ClientCommunicationEntityServerChainProxy RemoveConection(ChainArgument<System.Guid> nodeId)
    {
        __chain__.RemoveConection(nodeId);
        return this;
    }

    public ClientCommunicationEntityServerChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public ClientCommunicationEntityServerChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public ClientCommunicationEntityServerChainProxy StoreResult(string name)
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
    public static ClientCommunicationEntityServerChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IClientCommunicationEntityServer entity)
    {
        return new ClientCommunicationEntityServerChainProxy(new ClientCommunicationEntityChainProxy((SharedCode.Entities.Cloud.IClientCommunicationEntity)entity.GetBaseDeltaObject()));
    }

    public static ClientCommunicationEntityServerChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.IClientCommunicationEntityServer entity, IChainedEntity fromChain)
    {
        return new ClientCommunicationEntityServerChainProxy(new ClientCommunicationEntityChainProxy((SharedCode.Entities.Cloud.IClientCommunicationEntity)entity.GetBaseDeltaObject(), fromChain));
    }
}