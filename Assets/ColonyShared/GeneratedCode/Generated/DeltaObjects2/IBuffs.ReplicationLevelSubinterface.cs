// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects.ReplicationInterfaces
{
    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Always, 1967796668, typeof(Assets.ColonyShared.SharedCode.Wizardry.IBuffs))]
    public interface IBuffsAlways : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        SharedCode.EntitySystem.Delta.IDeltaDictionaryWrapper<SharedCode.Wizardry.SpellId, GeneratedCode.DeltaObjects.ReplicationInterfaces.IBuffAlways> All
        {
            get;
        }

        System.Threading.Tasks.Task<SharedCode.Wizardry.SpellId> TryAddBuff(Scripting.ScriptingContext cast, SharedCode.Wizardry.BuffDef buffDef);
        System.Threading.Tasks.Task<bool> RemoveBuff(SharedCode.Wizardry.SpellId buffId);
        System.Threading.Tasks.Task<bool> RemoveBuff(SharedCode.Wizardry.BuffDef buffDef);
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, -1972793031, typeof(Assets.ColonyShared.SharedCode.Wizardry.IBuffs))]
    public interface IBuffsClientBroadcast : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        SharedCode.EntitySystem.Delta.IDeltaDictionaryWrapper<SharedCode.Wizardry.SpellId, GeneratedCode.DeltaObjects.ReplicationInterfaces.IBuffClientBroadcast> All
        {
            get;
        }

        System.Threading.Tasks.Task<SharedCode.Wizardry.SpellId> TryAddBuff(Scripting.ScriptingContext cast, SharedCode.Wizardry.BuffDef buffDef);
        System.Threading.Tasks.Task<bool> RemoveBuff(SharedCode.Wizardry.SpellId buffId);
        System.Threading.Tasks.Task<bool> RemoveBuff(SharedCode.Wizardry.BuffDef buffDef);
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFullApi, -435880544, typeof(Assets.ColonyShared.SharedCode.Wizardry.IBuffs))]
    public interface IBuffsClientFullApi : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFull, 422191179, typeof(Assets.ColonyShared.SharedCode.Wizardry.IBuffs))]
    public interface IBuffsClientFull : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        SharedCode.EntitySystem.Delta.IDeltaDictionaryWrapper<SharedCode.Wizardry.SpellId, GeneratedCode.DeltaObjects.ReplicationInterfaces.IBuffClientFull> All
        {
            get;
        }

        System.Threading.Tasks.Task<SharedCode.Wizardry.SpellId> TryAddBuff(Scripting.ScriptingContext cast, SharedCode.Wizardry.BuffDef buffDef);
        System.Threading.Tasks.Task<bool> RemoveBuff(SharedCode.Wizardry.SpellId buffId);
        System.Threading.Tasks.Task<bool> RemoveBuff(SharedCode.Wizardry.BuffDef buffDef);
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ServerApi, 387827082, typeof(Assets.ColonyShared.SharedCode.Wizardry.IBuffs))]
    public interface IBuffsServerApi : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Server, -436929412, typeof(Assets.ColonyShared.SharedCode.Wizardry.IBuffs))]
    public interface IBuffsServer : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        SharedCode.EntitySystem.Delta.IDeltaDictionaryWrapper<SharedCode.Wizardry.SpellId, GeneratedCode.DeltaObjects.ReplicationInterfaces.IBuffServer> All
        {
            get;
        }

        System.Threading.Tasks.Task<SharedCode.Wizardry.SpellId> TryAddBuff(Scripting.ScriptingContext cast, SharedCode.Wizardry.BuffDef buffDef);
        System.Threading.Tasks.Task<bool> RemoveBuff(SharedCode.Wizardry.SpellId buffId);
        System.Threading.Tasks.Task<bool> RemoveBuff(SharedCode.Wizardry.BuffDef buffDef);
    }
}