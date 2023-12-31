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
    public class AccumulatedStatAlways : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IAccumulatedStatAlways
    {
        public AccumulatedStatAlways(Src.Aspects.Impl.Stats.IAccumulatedStat deltaObject): base(deltaObject)
        {
        }

        Src.Aspects.Impl.Stats.IAccumulatedStat __deltaObject__
        {
            get
            {
                return (Src.Aspects.Impl.Stats.IAccumulatedStat)__deltaObjectBase__;
            }
        }

        public Src.Aspects.Impl.Stats.AccumulatedStatModifiers Modifiers => __deltaObject__.Modifiers;
        public override int TypeId => 288646717;
    }

    public class AccumulatedStatClientBroadcast : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IAccumulatedStatClientBroadcast
    {
        public AccumulatedStatClientBroadcast(Src.Aspects.Impl.Stats.IAccumulatedStat deltaObject): base(deltaObject)
        {
        }

        Src.Aspects.Impl.Stats.IAccumulatedStat __deltaObject__
        {
            get
            {
                return (Src.Aspects.Impl.Stats.IAccumulatedStat)__deltaObjectBase__;
            }
        }

        public float ValueCache => __deltaObject__.ValueCache;
        public Src.Aspects.Impl.Stats.AccumulatedStatModifiers Modifiers => __deltaObject__.Modifiers;
        public float LimitMinCache => __deltaObject__.LimitMinCache;
        public float LimitMaxCache => __deltaObject__.LimitMaxCache;
        public Assets.Src.Aspects.Impl.Stats.StatType StatType => __deltaObject__.StatType;
        public System.Threading.Tasks.ValueTask<float> GetValue()
        {
            return __deltaObject__.GetValue();
        }

        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 11:
                    currProperty = ValueCache;
                    break;
                case 12:
                    currProperty = LimitMinCache;
                    break;
                case 13:
                    currProperty = LimitMaxCache;
                    break;
                case 14:
                    currProperty = StatType;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => -1763528140;
    }

    public class AccumulatedStatClientFullApi : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IAccumulatedStatClientFullApi
    {
        public AccumulatedStatClientFullApi(Src.Aspects.Impl.Stats.IAccumulatedStat deltaObject): base(deltaObject)
        {
        }

        Src.Aspects.Impl.Stats.IAccumulatedStat __deltaObject__
        {
            get
            {
                return (Src.Aspects.Impl.Stats.IAccumulatedStat)__deltaObjectBase__;
            }
        }

        public override int TypeId => 1941680736;
    }

    public class AccumulatedStatClientFull : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IAccumulatedStatClientFull
    {
        public AccumulatedStatClientFull(Src.Aspects.Impl.Stats.IAccumulatedStat deltaObject): base(deltaObject)
        {
        }

        Src.Aspects.Impl.Stats.IAccumulatedStat __deltaObject__
        {
            get
            {
                return (Src.Aspects.Impl.Stats.IAccumulatedStat)__deltaObjectBase__;
            }
        }

        public float InitialValue => __deltaObject__.InitialValue;
        public float ValueCache => __deltaObject__.ValueCache;
        public Src.Aspects.Impl.Stats.AccumulatedStatModifiers Modifiers => __deltaObject__.Modifiers;
        public float LimitMinCache => __deltaObject__.LimitMinCache;
        public float LimitMaxCache => __deltaObject__.LimitMaxCache;
        public Assets.Src.Aspects.Impl.Stats.StatType StatType => __deltaObject__.StatType;
        public System.Threading.Tasks.ValueTask<float> GetValue()
        {
            return __deltaObject__.GetValue();
        }

        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = InitialValue;
                    break;
                case 11:
                    currProperty = ValueCache;
                    break;
                case 12:
                    currProperty = LimitMinCache;
                    break;
                case 13:
                    currProperty = LimitMaxCache;
                    break;
                case 14:
                    currProperty = StatType;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => 897159415;
    }

    public class AccumulatedStatServerApi : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IAccumulatedStatServerApi
    {
        public AccumulatedStatServerApi(Src.Aspects.Impl.Stats.IAccumulatedStat deltaObject): base(deltaObject)
        {
        }

        Src.Aspects.Impl.Stats.IAccumulatedStat __deltaObject__
        {
            get
            {
                return (Src.Aspects.Impl.Stats.IAccumulatedStat)__deltaObjectBase__;
            }
        }

        public override int TypeId => 1533003002;
    }

    public class AccumulatedStatServer : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IAccumulatedStatServer
    {
        public AccumulatedStatServer(Src.Aspects.Impl.Stats.IAccumulatedStat deltaObject): base(deltaObject)
        {
        }

        Src.Aspects.Impl.Stats.IAccumulatedStat __deltaObject__
        {
            get
            {
                return (Src.Aspects.Impl.Stats.IAccumulatedStat)__deltaObjectBase__;
            }
        }

        public float InitialValue => __deltaObject__.InitialValue;
        public float ValueCache => __deltaObject__.ValueCache;
        public Src.Aspects.Impl.Stats.AccumulatedStatModifiers Modifiers => __deltaObject__.Modifiers;
        public float LimitMinCache => __deltaObject__.LimitMinCache;
        public float LimitMaxCache => __deltaObject__.LimitMaxCache;
        public Assets.Src.Aspects.Impl.Stats.StatType StatType => __deltaObject__.StatType;
        public System.Threading.Tasks.ValueTask<bool> AddModifier(SharedCode.Entities.Engine.ModifierCauser causer, Src.Aspects.Impl.Stats.StatModifierType modifierType, float value)
        {
            return __deltaObject__.AddModifier(causer, modifierType, value);
        }

        public System.Threading.Tasks.ValueTask<bool> IsGetAffectedBy(SharedCode.Entities.Engine.ModifierCauser causer)
        {
            return __deltaObject__.IsGetAffectedBy(causer);
        }

        public System.Threading.Tasks.ValueTask<bool> RemoveModifier(SharedCode.Entities.Engine.ModifierCauser causer, Src.Aspects.Impl.Stats.StatModifierType modifierType)
        {
            return __deltaObject__.RemoveModifier(causer, modifierType);
        }

        public System.Threading.Tasks.ValueTask<bool> RemoveModifiers(SharedCode.Entities.Engine.ModifierCauser causer)
        {
            return __deltaObject__.RemoveModifiers(causer);
        }

        public System.Threading.Tasks.ValueTask<float> GetValue()
        {
            return __deltaObject__.GetValue();
        }

        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = InitialValue;
                    break;
                case 11:
                    currProperty = ValueCache;
                    break;
                case 12:
                    currProperty = LimitMinCache;
                    break;
                case 13:
                    currProperty = LimitMaxCache;
                    break;
                case 14:
                    currProperty = StatType;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => -252692068;
    }
}