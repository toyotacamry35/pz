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
    public class CharRealmDataAlways : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ICharRealmDataAlways
    {
        public CharRealmDataAlways(SharedCode.Entities.ICharRealmData deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.ICharRealmData __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.ICharRealmData)__deltaObjectBase__;
            }
        }

        public override int TypeId => 671924858;
    }

    public class CharRealmDataClientBroadcast : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ICharRealmDataClientBroadcast
    {
        public CharRealmDataClientBroadcast(SharedCode.Entities.ICharRealmData deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.ICharRealmData __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.ICharRealmData)__deltaObjectBase__;
            }
        }

        public System.Threading.Tasks.Task ChangeCurrentRealmActivity(System.Guid accountId, bool active)
        {
            return __deltaObject__.ChangeCurrentRealmActivity(accountId, active);
        }

        public System.Threading.Tasks.Task DestroyCurrentRealm(System.Guid accountId)
        {
            return __deltaObject__.DestroyCurrentRealm(accountId);
        }

        public override int TypeId => 244975326;
    }

    public class CharRealmDataClientFullApi : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ICharRealmDataClientFullApi
    {
        public CharRealmDataClientFullApi(SharedCode.Entities.ICharRealmData deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.ICharRealmData __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.ICharRealmData)__deltaObjectBase__;
            }
        }

        public override int TypeId => -956451435;
    }

    public class CharRealmDataClientFull : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ICharRealmDataClientFull
    {
        public CharRealmDataClientFull(SharedCode.Entities.ICharRealmData deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.ICharRealmData __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.ICharRealmData)__deltaObjectBase__;
            }
        }

        public SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> CurrentRealm => __deltaObject__.CurrentRealm;
        public SharedCode.Entities.RealmCharStateEnum CurrentRealmCharState => __deltaObject__.CurrentRealmCharState;
        public SharedCode.Aspects.Sessions.RealmRulesDef CurrentRealmRulesCached => __deltaObject__.CurrentRealmRulesCached;
        public System.Threading.Tasks.Task<SharedCode.Entities.RealmOperationResult> FindRealm(SharedCode.Aspects.Sessions.RealmRulesQueryDef settings)
        {
            return __deltaObject__.FindRealm(settings);
        }

        public System.Threading.Tasks.Task<SharedCode.Entities.RealmOperationResult> EnterCurrentRealm(bool autoPlay)
        {
            return __deltaObject__.EnterCurrentRealm(autoPlay);
        }

        public System.Threading.Tasks.Task<bool> LeaveCurrentRealm()
        {
            return __deltaObject__.LeaveCurrentRealm();
        }

        public System.Threading.Tasks.Task<bool> GiveUpCurrentRealm()
        {
            return __deltaObject__.GiveUpCurrentRealm();
        }

        public System.Threading.Tasks.Task ChangeCurrentRealmActivity(System.Guid accountId, bool active)
        {
            return __deltaObject__.ChangeCurrentRealmActivity(accountId, active);
        }

        public System.Threading.Tasks.Task DestroyCurrentRealm(System.Guid accountId)
        {
            return __deltaObject__.DestroyCurrentRealm(accountId);
        }

        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = CurrentRealm;
                    break;
                case 11:
                    currProperty = CurrentRealmCharState;
                    break;
                case 12:
                    currProperty = CurrentRealmRulesCached;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => 1097030832;
    }

    public class CharRealmDataServerApi : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ICharRealmDataServerApi
    {
        public CharRealmDataServerApi(SharedCode.Entities.ICharRealmData deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.ICharRealmData __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.ICharRealmData)__deltaObjectBase__;
            }
        }

        public override int TypeId => -358742251;
    }

    public class CharRealmDataServer : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ICharRealmDataServer
    {
        public CharRealmDataServer(SharedCode.Entities.ICharRealmData deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.ICharRealmData __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.ICharRealmData)__deltaObjectBase__;
            }
        }

        public SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> CurrentRealm => __deltaObject__.CurrentRealm;
        public SharedCode.Entities.RealmCharStateEnum CurrentRealmCharState => __deltaObject__.CurrentRealmCharState;
        public SharedCode.Aspects.Sessions.RealmRulesDef CurrentRealmRulesCached => __deltaObject__.CurrentRealmRulesCached;
        public System.Threading.Tasks.Task<SharedCode.Entities.RealmOperationResult> FindRealm(SharedCode.Aspects.Sessions.RealmRulesQueryDef settings)
        {
            return __deltaObject__.FindRealm(settings);
        }

        public System.Threading.Tasks.Task<SharedCode.Entities.RealmOperationResult> EnterCurrentRealm(bool autoPlay)
        {
            return __deltaObject__.EnterCurrentRealm(autoPlay);
        }

        public System.Threading.Tasks.Task<bool> LeaveCurrentRealm()
        {
            return __deltaObject__.LeaveCurrentRealm();
        }

        public System.Threading.Tasks.Task<bool> GiveUpCurrentRealm()
        {
            return __deltaObject__.GiveUpCurrentRealm();
        }

        public System.Threading.Tasks.Task<SharedCode.Entities.Cloud.FindRealmRequestResult> GetRealm(SharedCode.Aspects.Sessions.RealmRulesQueryDef settings)
        {
            return __deltaObject__.GetRealm(settings);
        }

        public System.Threading.Tasks.Task ChangeCurrentRealmActivity(System.Guid accountId, bool active)
        {
            return __deltaObject__.ChangeCurrentRealmActivity(accountId, active);
        }

        public System.Threading.Tasks.Task DestroyCurrentRealm(System.Guid accountId)
        {
            return __deltaObject__.DestroyCurrentRealm(accountId);
        }

        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = CurrentRealm;
                    break;
                case 11:
                    currProperty = CurrentRealmCharState;
                    break;
                case 12:
                    currProperty = CurrentRealmRulesCached;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => -19203555;
    }
}