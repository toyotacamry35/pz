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
    public class AccountCharacterAlways : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IAccountCharacterAlways
    {
        public AccountCharacterAlways(SharedCode.Entities.IAccountCharacter deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.IAccountCharacter __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.IAccountCharacter)__deltaObjectBase__;
            }
        }

        public System.Guid Id => __deltaObject__.Id;
        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 13:
                    currProperty = Id;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => -773811047;
    }

    public class AccountCharacterClientBroadcast : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IAccountCharacterClientBroadcast
    {
        public AccountCharacterClientBroadcast(SharedCode.Entities.IAccountCharacter deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.IAccountCharacter __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.IAccountCharacter)__deltaObjectBase__;
            }
        }

        public System.Guid Id => __deltaObject__.Id;
        public System.Threading.Tasks.Task AddReward(ResourceSystem.Aspects.Rewards.RewardDef value)
        {
            return __deltaObject__.AddReward(value);
        }

        public System.Threading.Tasks.Task ClearRewards()
        {
            return __deltaObject__.ClearRewards();
        }

        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 13:
                    currProperty = Id;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => 33275748;
    }

    public class AccountCharacterClientFullApi : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IAccountCharacterClientFullApi
    {
        public AccountCharacterClientFullApi(SharedCode.Entities.IAccountCharacter deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.IAccountCharacter __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.IAccountCharacter)__deltaObjectBase__;
            }
        }

        public override int TypeId => 494747857;
    }

    public class AccountCharacterClientFull : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IAccountCharacterClientFull
    {
        public AccountCharacterClientFull(SharedCode.Entities.IAccountCharacter deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.IAccountCharacter __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.IAccountCharacter)__deltaObjectBase__;
            }
        }

        public string CharacterName => __deltaObject__.CharacterName;
        public System.Guid MapId => __deltaObject__.MapId;
        public IDeltaDictionary<ResourceSystem.Aspects.Rewards.RewardDef, int> CurrentSessionRewards
        {
            get
            {
                return __deltaObject__.CurrentSessionRewards;
            }
        }

        public System.Guid Id => __deltaObject__.Id;
        public System.Threading.Tasks.Task AddReward(ResourceSystem.Aspects.Rewards.RewardDef value)
        {
            return __deltaObject__.AddReward(value);
        }

        public System.Threading.Tasks.Task ClearRewards()
        {
            return __deltaObject__.ClearRewards();
        }

        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = CharacterName;
                    break;
                case 11:
                    currProperty = MapId;
                    break;
                case 12:
                    currProperty = CurrentSessionRewards;
                    break;
                case 13:
                    currProperty = Id;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => 1265710012;
    }

    public class AccountCharacterServerApi : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IAccountCharacterServerApi
    {
        public AccountCharacterServerApi(SharedCode.Entities.IAccountCharacter deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.IAccountCharacter __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.IAccountCharacter)__deltaObjectBase__;
            }
        }

        public override int TypeId => -115764783;
    }

    public class AccountCharacterServer : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IAccountCharacterServer
    {
        public AccountCharacterServer(SharedCode.Entities.IAccountCharacter deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.IAccountCharacter __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.IAccountCharacter)__deltaObjectBase__;
            }
        }

        public string CharacterName => __deltaObject__.CharacterName;
        public System.Guid MapId => __deltaObject__.MapId;
        public IDeltaDictionary<ResourceSystem.Aspects.Rewards.RewardDef, int> CurrentSessionRewards
        {
            get
            {
                return __deltaObject__.CurrentSessionRewards;
            }
        }

        public System.Guid Id => __deltaObject__.Id;
        public System.Threading.Tasks.ValueTask<bool> AssignData(System.Guid mapId)
        {
            return __deltaObject__.AssignData(mapId);
        }

        public System.Threading.Tasks.ValueTask<bool> GrantReward(ResourceSystem.Aspects.Rewards.RewardDef value)
        {
            return __deltaObject__.GrantReward(value);
        }

        public System.Threading.Tasks.Task AddReward(ResourceSystem.Aspects.Rewards.RewardDef value)
        {
            return __deltaObject__.AddReward(value);
        }

        public System.Threading.Tasks.Task ClearRewards()
        {
            return __deltaObject__.ClearRewards();
        }

        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = CharacterName;
                    break;
                case 11:
                    currProperty = MapId;
                    break;
                case 12:
                    currProperty = CurrentSessionRewards;
                    break;
                case 13:
                    currProperty = Id;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => 167219836;
    }
}