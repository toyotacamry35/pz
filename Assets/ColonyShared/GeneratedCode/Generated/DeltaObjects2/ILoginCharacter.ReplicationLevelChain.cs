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
public class LoginCharacterAlwaysChainProxy : IChainedEntity
{
    private LoginCharacterChainProxy __chain__;
    public LoginCharacterAlwaysChainProxy(LoginCharacterChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public LoginCharacterAlwaysChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public LoginCharacterAlwaysChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public LoginCharacterAlwaysChainProxy StoreResult(string name)
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
    public static LoginCharacterAlwaysChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.ILoginCharacterAlways entity)
    {
        return new LoginCharacterAlwaysChainProxy(new LoginCharacterChainProxy((SharedCode.Entities.ILoginCharacter)entity.GetBaseDeltaObject()));
    }

    public static LoginCharacterAlwaysChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.ILoginCharacterAlways entity, IChainedEntity fromChain)
    {
        return new LoginCharacterAlwaysChainProxy(new LoginCharacterChainProxy((SharedCode.Entities.ILoginCharacter)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class LoginCharacterClientBroadcastChainProxy : IChainedEntity
{
    private LoginCharacterChainProxy __chain__;
    public LoginCharacterClientBroadcastChainProxy(LoginCharacterChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public LoginCharacterClientBroadcastChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public LoginCharacterClientBroadcastChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public LoginCharacterClientBroadcastChainProxy StoreResult(string name)
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
    public static LoginCharacterClientBroadcastChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.ILoginCharacterClientBroadcast entity)
    {
        return new LoginCharacterClientBroadcastChainProxy(new LoginCharacterChainProxy((SharedCode.Entities.ILoginCharacter)entity.GetBaseDeltaObject()));
    }

    public static LoginCharacterClientBroadcastChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.ILoginCharacterClientBroadcast entity, IChainedEntity fromChain)
    {
        return new LoginCharacterClientBroadcastChainProxy(new LoginCharacterChainProxy((SharedCode.Entities.ILoginCharacter)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class LoginCharacterClientFullApiChainProxy : IChainedEntity
{
    private LoginCharacterChainProxy __chain__;
    public LoginCharacterClientFullApiChainProxy(LoginCharacterChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public LoginCharacterClientFullApiChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public LoginCharacterClientFullApiChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public LoginCharacterClientFullApiChainProxy StoreResult(string name)
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
    public static LoginCharacterClientFullApiChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.ILoginCharacterClientFullApi entity)
    {
        return new LoginCharacterClientFullApiChainProxy(new LoginCharacterChainProxy((SharedCode.Entities.ILoginCharacter)entity.GetBaseDeltaObject()));
    }

    public static LoginCharacterClientFullApiChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.ILoginCharacterClientFullApi entity, IChainedEntity fromChain)
    {
        return new LoginCharacterClientFullApiChainProxy(new LoginCharacterChainProxy((SharedCode.Entities.ILoginCharacter)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class LoginCharacterClientFullChainProxy : IChainedEntity
{
    private LoginCharacterChainProxy __chain__;
    public LoginCharacterClientFullChainProxy(LoginCharacterChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public LoginCharacterClientFullChainProxy TestMethod1(ChainArgument<int> a, ChainArgument<System.Guid> itemId)
    {
        __chain__.TestMethod1(a, itemId);
        return this;
    }

    public LoginCharacterClientFullChainProxy TestMethodSimple()
    {
        __chain__.TestMethodSimple();
        return this;
    }

    public LoginCharacterClientFullChainProxy TestMethodSimpleParam(ChainArgument<string> param1)
    {
        __chain__.TestMethodSimpleParam(param1);
        return this;
    }

    public LoginCharacterClientFullChainProxy TestMethod2222(ChainArgument<int> a, ChainArgument<System.Guid> itemId)
    {
        __chain__.TestMethod2222(a, itemId);
        return this;
    }

    public LoginCharacterClientFullChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public LoginCharacterClientFullChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public LoginCharacterClientFullChainProxy StoreResult(string name)
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
    public static LoginCharacterClientFullChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.ILoginCharacterClientFull entity)
    {
        return new LoginCharacterClientFullChainProxy(new LoginCharacterChainProxy((SharedCode.Entities.ILoginCharacter)entity.GetBaseDeltaObject()));
    }

    public static LoginCharacterClientFullChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.ILoginCharacterClientFull entity, IChainedEntity fromChain)
    {
        return new LoginCharacterClientFullChainProxy(new LoginCharacterChainProxy((SharedCode.Entities.ILoginCharacter)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class LoginCharacterServerApiChainProxy : IChainedEntity
{
    private LoginCharacterChainProxy __chain__;
    public LoginCharacterServerApiChainProxy(LoginCharacterChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public LoginCharacterServerApiChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public LoginCharacterServerApiChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public LoginCharacterServerApiChainProxy StoreResult(string name)
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
    public static LoginCharacterServerApiChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.ILoginCharacterServerApi entity)
    {
        return new LoginCharacterServerApiChainProxy(new LoginCharacterChainProxy((SharedCode.Entities.ILoginCharacter)entity.GetBaseDeltaObject()));
    }

    public static LoginCharacterServerApiChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.ILoginCharacterServerApi entity, IChainedEntity fromChain)
    {
        return new LoginCharacterServerApiChainProxy(new LoginCharacterChainProxy((SharedCode.Entities.ILoginCharacter)entity.GetBaseDeltaObject(), fromChain));
    }
}

[GeneratedCode("CodeGen", "1.0")]
public class LoginCharacterServerChainProxy : IChainedEntity
{
    private LoginCharacterChainProxy __chain__;
    public LoginCharacterServerChainProxy(LoginCharacterChainProxy chain)
    {
        __chain__ = chain;
    }

    public ChainCancellationToken Run()
    {
        return __chain__.Run();
    }

    public LoginCharacterServerChainProxy TestMethod1(ChainArgument<int> a, ChainArgument<System.Guid> itemId)
    {
        __chain__.TestMethod1(a, itemId);
        return this;
    }

    public LoginCharacterServerChainProxy TestMethodSimple()
    {
        __chain__.TestMethodSimple();
        return this;
    }

    public LoginCharacterServerChainProxy TestMethodSimpleParam(ChainArgument<string> param1)
    {
        __chain__.TestMethodSimpleParam(param1);
        return this;
    }

    public LoginCharacterServerChainProxy TestMethod2222(ChainArgument<int> a, ChainArgument<System.Guid> itemId)
    {
        __chain__.TestMethod2222(a, itemId);
        return this;
    }

    public LoginCharacterServerChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
    {
        __chain__.Delay(duration, repeat, fromUtcNow);
        return this;
    }

    public LoginCharacterServerChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
    {
        __chain__.DelayCount(duration, count, fromUtcNow);
        return this;
    }

    public LoginCharacterServerChainProxy StoreResult(string name)
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
    public static LoginCharacterServerChainProxy Chain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.ILoginCharacterServer entity)
    {
        return new LoginCharacterServerChainProxy(new LoginCharacterChainProxy((SharedCode.Entities.ILoginCharacter)entity.GetBaseDeltaObject()));
    }

    public static LoginCharacterServerChainProxy ContinueChain(this GeneratedCode.DeltaObjects.ReplicationInterfaces.ILoginCharacterServer entity, IChainedEntity fromChain)
    {
        return new LoginCharacterServerChainProxy(new LoginCharacterChainProxy((SharedCode.Entities.ILoginCharacter)entity.GetBaseDeltaObject(), fromChain));
    }
}