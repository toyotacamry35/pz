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
    public class ComputableStateMachineAlways : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IComputableStateMachineAlways
    {
        public ComputableStateMachineAlways(Assets.ColonyShared.SharedCode.Entities.IComputableStateMachine deltaObject): base(deltaObject)
        {
        }

        Assets.ColonyShared.SharedCode.Entities.IComputableStateMachine __deltaObject__
        {
            get
            {
                return (Assets.ColonyShared.SharedCode.Entities.IComputableStateMachine)__deltaObjectBase__;
            }
        }

        public override int TypeId => 1468792564;
    }

    public class ComputableStateMachineClientBroadcast : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IComputableStateMachineClientBroadcast
    {
        public ComputableStateMachineClientBroadcast(Assets.ColonyShared.SharedCode.Entities.IComputableStateMachine deltaObject): base(deltaObject)
        {
        }

        Assets.ColonyShared.SharedCode.Entities.IComputableStateMachine __deltaObject__
        {
            get
            {
                return (Assets.ColonyShared.SharedCode.Entities.IComputableStateMachine)__deltaObjectBase__;
            }
        }

        public bool IsPristineInternal => __deltaObject__.IsPristineInternal;
        public Assets.ColonyShared.SharedCode.Aspects.WorldObjects.ComputableStatesDef FixedStates => __deltaObject__.FixedStates;
        public System.Threading.Tasks.ValueTask<bool> GetIsPristine()
        {
            return __deltaObject__.GetIsPristine();
        }

        public System.Threading.Tasks.Task<Assets.ColonyShared.SharedCode.Aspects.WorldObjects.ComputableStatesDef> GetCurrentStates()
        {
            return __deltaObject__.GetCurrentStates();
        }

        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = IsPristineInternal;
                    break;
                case 11:
                    currProperty = FixedStates;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => 1734792160;
    }

    public class ComputableStateMachineClientFullApi : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IComputableStateMachineClientFullApi
    {
        public ComputableStateMachineClientFullApi(Assets.ColonyShared.SharedCode.Entities.IComputableStateMachine deltaObject): base(deltaObject)
        {
        }

        Assets.ColonyShared.SharedCode.Entities.IComputableStateMachine __deltaObject__
        {
            get
            {
                return (Assets.ColonyShared.SharedCode.Entities.IComputableStateMachine)__deltaObjectBase__;
            }
        }

        public override int TypeId => 62133191;
    }

    public class ComputableStateMachineClientFull : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IComputableStateMachineClientFull
    {
        public ComputableStateMachineClientFull(Assets.ColonyShared.SharedCode.Entities.IComputableStateMachine deltaObject): base(deltaObject)
        {
        }

        Assets.ColonyShared.SharedCode.Entities.IComputableStateMachine __deltaObject__
        {
            get
            {
                return (Assets.ColonyShared.SharedCode.Entities.IComputableStateMachine)__deltaObjectBase__;
            }
        }

        public bool IsPristineInternal => __deltaObject__.IsPristineInternal;
        public Assets.ColonyShared.SharedCode.Aspects.WorldObjects.ComputableStatesDef FixedStates => __deltaObject__.FixedStates;
        public System.Threading.Tasks.ValueTask<bool> GetIsPristine()
        {
            return __deltaObject__.GetIsPristine();
        }

        public System.Threading.Tasks.Task<Assets.ColonyShared.SharedCode.Aspects.WorldObjects.ComputableStatesDef> GetCurrentStates()
        {
            return __deltaObject__.GetCurrentStates();
        }

        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = IsPristineInternal;
                    break;
                case 11:
                    currProperty = FixedStates;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => -1522311555;
    }

    public class ComputableStateMachineServerApi : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IComputableStateMachineServerApi
    {
        public ComputableStateMachineServerApi(Assets.ColonyShared.SharedCode.Entities.IComputableStateMachine deltaObject): base(deltaObject)
        {
        }

        Assets.ColonyShared.SharedCode.Entities.IComputableStateMachine __deltaObject__
        {
            get
            {
                return (Assets.ColonyShared.SharedCode.Entities.IComputableStateMachine)__deltaObjectBase__;
            }
        }

        public override int TypeId => 157505305;
    }

    public class ComputableStateMachineServer : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IComputableStateMachineServer
    {
        public ComputableStateMachineServer(Assets.ColonyShared.SharedCode.Entities.IComputableStateMachine deltaObject): base(deltaObject)
        {
        }

        Assets.ColonyShared.SharedCode.Entities.IComputableStateMachine __deltaObject__
        {
            get
            {
                return (Assets.ColonyShared.SharedCode.Entities.IComputableStateMachine)__deltaObjectBase__;
            }
        }

        public bool IsPristineInternal => __deltaObject__.IsPristineInternal;
        public Assets.ColonyShared.SharedCode.Aspects.WorldObjects.ComputableStatesDef FixedStates => __deltaObject__.FixedStates;
        public System.Threading.Tasks.ValueTask<bool> GetIsPristine()
        {
            return __deltaObject__.GetIsPristine();
        }

        public System.Threading.Tasks.Task<Assets.ColonyShared.SharedCode.Aspects.WorldObjects.ComputableStatesDef> GetCurrentStates()
        {
            return __deltaObject__.GetCurrentStates();
        }

        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = IsPristineInternal;
                    break;
                case 11:
                    currProperty = FixedStates;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => -674254054;
    }
}