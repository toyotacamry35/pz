// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.Logging;
using GeneratedCode.Repositories;
using SharedCode.EntitySystem;
using GeneratedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;

namespace GeneratedCode.DeltaObjects
{
    public class BuffsAlways : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IBuffsAlways
    {
        public BuffsAlways(Assets.ColonyShared.SharedCode.Wizardry.IBuffs deltaObject): base(deltaObject)
        {
        }

        Assets.ColonyShared.SharedCode.Wizardry.IBuffs __deltaObject__
        {
            get
            {
                return (Assets.ColonyShared.SharedCode.Wizardry.IBuffs)__deltaObjectBase__;
            }
        }

        IDeltaDictionaryWrapper<SharedCode.Wizardry.SpellId, GeneratedCode.DeltaObjects.ReplicationInterfaces.IBuffAlways> __All__Wrapper__;
        public IDeltaDictionaryWrapper<SharedCode.Wizardry.SpellId, GeneratedCode.DeltaObjects.ReplicationInterfaces.IBuffAlways> All
        {
            get
            {
                if (__All__Wrapper__ == null || ((IBaseDeltaObjectWrapper)__All__Wrapper__).GetBaseDeltaObject() != __deltaObject__.All)
                    __All__Wrapper__ = __deltaObject__.All == null ? null : new DeltaDictionaryWrapper<SharedCode.Wizardry.SpellId, Assets.ColonyShared.SharedCode.Wizardry.IBuff, GeneratedCode.DeltaObjects.ReplicationInterfaces.IBuffAlways>(__deltaObject__.All);
                return __All__Wrapper__;
            }
        }

        public System.Threading.Tasks.Task<SharedCode.Wizardry.SpellId> TryAddBuff(Scripting.ScriptingContext cast, SharedCode.Wizardry.BuffDef buffDef)
        {
            return __deltaObject__.TryAddBuff(cast, buffDef);
        }

        public System.Threading.Tasks.Task<bool> RemoveBuff(SharedCode.Wizardry.SpellId buffId)
        {
            return __deltaObject__.RemoveBuff(buffId);
        }

        public System.Threading.Tasks.Task<bool> RemoveBuff(SharedCode.Wizardry.BuffDef buffDef)
        {
            return __deltaObject__.RemoveBuff(buffDef);
        }

        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 11:
                    currProperty = All;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => 315981677;
    }

    public class BuffsClientBroadcast : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IBuffsClientBroadcast
    {
        public BuffsClientBroadcast(Assets.ColonyShared.SharedCode.Wizardry.IBuffs deltaObject): base(deltaObject)
        {
        }

        Assets.ColonyShared.SharedCode.Wizardry.IBuffs __deltaObject__
        {
            get
            {
                return (Assets.ColonyShared.SharedCode.Wizardry.IBuffs)__deltaObjectBase__;
            }
        }

        IDeltaDictionaryWrapper<SharedCode.Wizardry.SpellId, GeneratedCode.DeltaObjects.ReplicationInterfaces.IBuffClientBroadcast> __All__Wrapper__;
        public IDeltaDictionaryWrapper<SharedCode.Wizardry.SpellId, GeneratedCode.DeltaObjects.ReplicationInterfaces.IBuffClientBroadcast> All
        {
            get
            {
                if (__All__Wrapper__ == null || ((IBaseDeltaObjectWrapper)__All__Wrapper__).GetBaseDeltaObject() != __deltaObject__.All)
                    __All__Wrapper__ = __deltaObject__.All == null ? null : new DeltaDictionaryWrapper<SharedCode.Wizardry.SpellId, Assets.ColonyShared.SharedCode.Wizardry.IBuff, GeneratedCode.DeltaObjects.ReplicationInterfaces.IBuffClientBroadcast>(__deltaObject__.All);
                return __All__Wrapper__;
            }
        }

        public System.Threading.Tasks.Task<SharedCode.Wizardry.SpellId> TryAddBuff(Scripting.ScriptingContext cast, SharedCode.Wizardry.BuffDef buffDef)
        {
            return __deltaObject__.TryAddBuff(cast, buffDef);
        }

        public System.Threading.Tasks.Task<bool> RemoveBuff(SharedCode.Wizardry.SpellId buffId)
        {
            return __deltaObject__.RemoveBuff(buffId);
        }

        public System.Threading.Tasks.Task<bool> RemoveBuff(SharedCode.Wizardry.BuffDef buffDef)
        {
            return __deltaObject__.RemoveBuff(buffDef);
        }

        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 11:
                    currProperty = All;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => -174313818;
    }

    public class BuffsClientFullApi : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IBuffsClientFullApi
    {
        public BuffsClientFullApi(Assets.ColonyShared.SharedCode.Wizardry.IBuffs deltaObject): base(deltaObject)
        {
        }

        Assets.ColonyShared.SharedCode.Wizardry.IBuffs __deltaObject__
        {
            get
            {
                return (Assets.ColonyShared.SharedCode.Wizardry.IBuffs)__deltaObjectBase__;
            }
        }

        public override int TypeId => 995744981;
    }

    public class BuffsClientFull : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IBuffsClientFull
    {
        public BuffsClientFull(Assets.ColonyShared.SharedCode.Wizardry.IBuffs deltaObject): base(deltaObject)
        {
        }

        Assets.ColonyShared.SharedCode.Wizardry.IBuffs __deltaObject__
        {
            get
            {
                return (Assets.ColonyShared.SharedCode.Wizardry.IBuffs)__deltaObjectBase__;
            }
        }

        IDeltaDictionaryWrapper<SharedCode.Wizardry.SpellId, GeneratedCode.DeltaObjects.ReplicationInterfaces.IBuffClientFull> __All__Wrapper__;
        public IDeltaDictionaryWrapper<SharedCode.Wizardry.SpellId, GeneratedCode.DeltaObjects.ReplicationInterfaces.IBuffClientFull> All
        {
            get
            {
                if (__All__Wrapper__ == null || ((IBaseDeltaObjectWrapper)__All__Wrapper__).GetBaseDeltaObject() != __deltaObject__.All)
                    __All__Wrapper__ = __deltaObject__.All == null ? null : new DeltaDictionaryWrapper<SharedCode.Wizardry.SpellId, Assets.ColonyShared.SharedCode.Wizardry.IBuff, GeneratedCode.DeltaObjects.ReplicationInterfaces.IBuffClientFull>(__deltaObject__.All);
                return __All__Wrapper__;
            }
        }

        public System.Threading.Tasks.Task<SharedCode.Wizardry.SpellId> TryAddBuff(Scripting.ScriptingContext cast, SharedCode.Wizardry.BuffDef buffDef)
        {
            return __deltaObject__.TryAddBuff(cast, buffDef);
        }

        public System.Threading.Tasks.Task<bool> RemoveBuff(SharedCode.Wizardry.SpellId buffId)
        {
            return __deltaObject__.RemoveBuff(buffId);
        }

        public System.Threading.Tasks.Task<bool> RemoveBuff(SharedCode.Wizardry.BuffDef buffDef)
        {
            return __deltaObject__.RemoveBuff(buffDef);
        }

        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 11:
                    currProperty = All;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => -2093428276;
    }

    public class BuffsServerApi : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IBuffsServerApi
    {
        public BuffsServerApi(Assets.ColonyShared.SharedCode.Wizardry.IBuffs deltaObject): base(deltaObject)
        {
        }

        Assets.ColonyShared.SharedCode.Wizardry.IBuffs __deltaObject__
        {
            get
            {
                return (Assets.ColonyShared.SharedCode.Wizardry.IBuffs)__deltaObjectBase__;
            }
        }

        public override int TypeId => 230686266;
    }

    public class BuffsServer : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IBuffsServer
    {
        public BuffsServer(Assets.ColonyShared.SharedCode.Wizardry.IBuffs deltaObject): base(deltaObject)
        {
        }

        Assets.ColonyShared.SharedCode.Wizardry.IBuffs __deltaObject__
        {
            get
            {
                return (Assets.ColonyShared.SharedCode.Wizardry.IBuffs)__deltaObjectBase__;
            }
        }

        IDeltaDictionaryWrapper<SharedCode.Wizardry.SpellId, GeneratedCode.DeltaObjects.ReplicationInterfaces.IBuffServer> __All__Wrapper__;
        public IDeltaDictionaryWrapper<SharedCode.Wizardry.SpellId, GeneratedCode.DeltaObjects.ReplicationInterfaces.IBuffServer> All
        {
            get
            {
                if (__All__Wrapper__ == null || ((IBaseDeltaObjectWrapper)__All__Wrapper__).GetBaseDeltaObject() != __deltaObject__.All)
                    __All__Wrapper__ = __deltaObject__.All == null ? null : new DeltaDictionaryWrapper<SharedCode.Wizardry.SpellId, Assets.ColonyShared.SharedCode.Wizardry.IBuff, GeneratedCode.DeltaObjects.ReplicationInterfaces.IBuffServer>(__deltaObject__.All);
                return __All__Wrapper__;
            }
        }

        public System.Threading.Tasks.Task<SharedCode.Wizardry.SpellId> TryAddBuff(Scripting.ScriptingContext cast, SharedCode.Wizardry.BuffDef buffDef)
        {
            return __deltaObject__.TryAddBuff(cast, buffDef);
        }

        public System.Threading.Tasks.Task<bool> RemoveBuff(SharedCode.Wizardry.SpellId buffId)
        {
            return __deltaObject__.RemoveBuff(buffId);
        }

        public System.Threading.Tasks.Task<bool> RemoveBuff(SharedCode.Wizardry.BuffDef buffDef)
        {
            return __deltaObject__.RemoveBuff(buffDef);
        }

        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 11:
                    currProperty = All;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => -965646914;
    }
}

namespace GeneratedCode.DeltaObjects
{
    public class BuffAlways : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IBuffAlways
    {
        public BuffAlways(Assets.ColonyShared.SharedCode.Wizardry.IBuff deltaObject): base(deltaObject)
        {
        }

        Assets.ColonyShared.SharedCode.Wizardry.IBuff __deltaObject__
        {
            get
            {
                return (Assets.ColonyShared.SharedCode.Wizardry.IBuff)__deltaObjectBase__;
            }
        }

        public SharedCode.Wizardry.BuffDef Def => __deltaObject__.Def;
        public Scripting.ScriptingContext Context => __deltaObject__.Context;
        public SharedCode.Wizardry.SpellId Id => __deltaObject__.Id;
        public long StartTime => __deltaObject__.StartTime;
        public long EndTime => __deltaObject__.EndTime;
        public long Duration => __deltaObject__.Duration;
        public bool Started => __deltaObject__.Started;
        public bool Finished => __deltaObject__.Finished;
        public bool IsInfinite => __deltaObject__.IsInfinite;
        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = Def;
                    break;
                case 11:
                    currProperty = Context;
                    break;
                case 12:
                    currProperty = Id;
                    break;
                case 13:
                    currProperty = StartTime;
                    break;
                case 14:
                    currProperty = EndTime;
                    break;
                case 15:
                    currProperty = Duration;
                    break;
                case 16:
                    currProperty = Started;
                    break;
                case 17:
                    currProperty = Finished;
                    break;
                case 18:
                    currProperty = IsInfinite;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => -1311163347;
    }

    public class BuffClientBroadcast : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IBuffClientBroadcast
    {
        public BuffClientBroadcast(Assets.ColonyShared.SharedCode.Wizardry.IBuff deltaObject): base(deltaObject)
        {
        }

        Assets.ColonyShared.SharedCode.Wizardry.IBuff __deltaObject__
        {
            get
            {
                return (Assets.ColonyShared.SharedCode.Wizardry.IBuff)__deltaObjectBase__;
            }
        }

        public SharedCode.Wizardry.BuffDef Def => __deltaObject__.Def;
        public Scripting.ScriptingContext Context => __deltaObject__.Context;
        public SharedCode.Wizardry.SpellId Id => __deltaObject__.Id;
        public long StartTime => __deltaObject__.StartTime;
        public long EndTime => __deltaObject__.EndTime;
        public long Duration => __deltaObject__.Duration;
        public bool Started => __deltaObject__.Started;
        public bool Finished => __deltaObject__.Finished;
        public bool IsInfinite => __deltaObject__.IsInfinite;
        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = Def;
                    break;
                case 11:
                    currProperty = Context;
                    break;
                case 12:
                    currProperty = Id;
                    break;
                case 13:
                    currProperty = StartTime;
                    break;
                case 14:
                    currProperty = EndTime;
                    break;
                case 15:
                    currProperty = Duration;
                    break;
                case 16:
                    currProperty = Started;
                    break;
                case 17:
                    currProperty = Finished;
                    break;
                case 18:
                    currProperty = IsInfinite;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => -754138394;
    }

    public class BuffClientFullApi : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IBuffClientFullApi
    {
        public BuffClientFullApi(Assets.ColonyShared.SharedCode.Wizardry.IBuff deltaObject): base(deltaObject)
        {
        }

        Assets.ColonyShared.SharedCode.Wizardry.IBuff __deltaObject__
        {
            get
            {
                return (Assets.ColonyShared.SharedCode.Wizardry.IBuff)__deltaObjectBase__;
            }
        }

        public override int TypeId => -1263747862;
    }

    public class BuffClientFull : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IBuffClientFull
    {
        public BuffClientFull(Assets.ColonyShared.SharedCode.Wizardry.IBuff deltaObject): base(deltaObject)
        {
        }

        Assets.ColonyShared.SharedCode.Wizardry.IBuff __deltaObject__
        {
            get
            {
                return (Assets.ColonyShared.SharedCode.Wizardry.IBuff)__deltaObjectBase__;
            }
        }

        public SharedCode.Wizardry.BuffDef Def => __deltaObject__.Def;
        public Scripting.ScriptingContext Context => __deltaObject__.Context;
        public SharedCode.Wizardry.SpellId Id => __deltaObject__.Id;
        public long StartTime => __deltaObject__.StartTime;
        public long EndTime => __deltaObject__.EndTime;
        public long Duration => __deltaObject__.Duration;
        public bool Started => __deltaObject__.Started;
        public bool Finished => __deltaObject__.Finished;
        public bool IsInfinite => __deltaObject__.IsInfinite;
        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = Def;
                    break;
                case 11:
                    currProperty = Context;
                    break;
                case 12:
                    currProperty = Id;
                    break;
                case 13:
                    currProperty = StartTime;
                    break;
                case 14:
                    currProperty = EndTime;
                    break;
                case 15:
                    currProperty = Duration;
                    break;
                case 16:
                    currProperty = Started;
                    break;
                case 17:
                    currProperty = Finished;
                    break;
                case 18:
                    currProperty = IsInfinite;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => -87946475;
    }

    public class BuffServerApi : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IBuffServerApi
    {
        public BuffServerApi(Assets.ColonyShared.SharedCode.Wizardry.IBuff deltaObject): base(deltaObject)
        {
        }

        Assets.ColonyShared.SharedCode.Wizardry.IBuff __deltaObject__
        {
            get
            {
                return (Assets.ColonyShared.SharedCode.Wizardry.IBuff)__deltaObjectBase__;
            }
        }

        public override int TypeId => -1269424910;
    }

    public class BuffServer : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IBuffServer
    {
        public BuffServer(Assets.ColonyShared.SharedCode.Wizardry.IBuff deltaObject): base(deltaObject)
        {
        }

        Assets.ColonyShared.SharedCode.Wizardry.IBuff __deltaObject__
        {
            get
            {
                return (Assets.ColonyShared.SharedCode.Wizardry.IBuff)__deltaObjectBase__;
            }
        }

        public SharedCode.Wizardry.BuffDef Def => __deltaObject__.Def;
        public Scripting.ScriptingContext Context => __deltaObject__.Context;
        public SharedCode.Wizardry.SpellId Id => __deltaObject__.Id;
        public long StartTime => __deltaObject__.StartTime;
        public long EndTime => __deltaObject__.EndTime;
        public long Duration => __deltaObject__.Duration;
        public bool Started => __deltaObject__.Started;
        public bool Finished => __deltaObject__.Finished;
        public bool IsInfinite => __deltaObject__.IsInfinite;
        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = Def;
                    break;
                case 11:
                    currProperty = Context;
                    break;
                case 12:
                    currProperty = Id;
                    break;
                case 13:
                    currProperty = StartTime;
                    break;
                case 14:
                    currProperty = EndTime;
                    break;
                case 15:
                    currProperty = Duration;
                    break;
                case 16:
                    currProperty = Started;
                    break;
                case 17:
                    currProperty = Finished;
                    break;
                case 18:
                    currProperty = IsInfinite;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => 206300469;
    }
}