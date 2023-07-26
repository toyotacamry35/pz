// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects.ReplicationInterfaces
{
    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Always, -1313954807, typeof(Assets.ColonyShared.SharedCode.Wizardry.IBuff))]
    public interface IBuffAlways : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        SharedCode.Wizardry.BuffDef Def
        {
            get;
        }

        Scripting.ScriptingContext Context
        {
            get;
        }

        SharedCode.Wizardry.SpellId Id
        {
            get;
        }

        long StartTime
        {
            get;
        }

        long EndTime
        {
            get;
        }

        long Duration
        {
            get;
        }

        bool Started
        {
            get;
        }

        bool Finished
        {
            get;
        }

        bool IsInfinite
        {
            get;
        }
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, -1829787027, typeof(Assets.ColonyShared.SharedCode.Wizardry.IBuff))]
    public interface IBuffClientBroadcast : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        SharedCode.Wizardry.BuffDef Def
        {
            get;
        }

        Scripting.ScriptingContext Context
        {
            get;
        }

        SharedCode.Wizardry.SpellId Id
        {
            get;
        }

        long StartTime
        {
            get;
        }

        long EndTime
        {
            get;
        }

        long Duration
        {
            get;
        }

        bool Started
        {
            get;
        }

        bool Finished
        {
            get;
        }

        bool IsInfinite
        {
            get;
        }
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFullApi, 577043993, typeof(Assets.ColonyShared.SharedCode.Wizardry.IBuff))]
    public interface IBuffClientFullApi : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFull, 752228887, typeof(Assets.ColonyShared.SharedCode.Wizardry.IBuff))]
    public interface IBuffClientFull : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        SharedCode.Wizardry.BuffDef Def
        {
            get;
        }

        Scripting.ScriptingContext Context
        {
            get;
        }

        SharedCode.Wizardry.SpellId Id
        {
            get;
        }

        long StartTime
        {
            get;
        }

        long EndTime
        {
            get;
        }

        long Duration
        {
            get;
        }

        bool Started
        {
            get;
        }

        bool Finished
        {
            get;
        }

        bool IsInfinite
        {
            get;
        }
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ServerApi, -211750289, typeof(Assets.ColonyShared.SharedCode.Wizardry.IBuff))]
    public interface IBuffServerApi : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Server, -1209107254, typeof(Assets.ColonyShared.SharedCode.Wizardry.IBuff))]
    public interface IBuffServer : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        SharedCode.Wizardry.BuffDef Def
        {
            get;
        }

        Scripting.ScriptingContext Context
        {
            get;
        }

        SharedCode.Wizardry.SpellId Id
        {
            get;
        }

        long StartTime
        {
            get;
        }

        long EndTime
        {
            get;
        }

        long Duration
        {
            get;
        }

        bool Started
        {
            get;
        }

        bool Finished
        {
            get;
        }

        bool IsInfinite
        {
            get;
        }
    }
}