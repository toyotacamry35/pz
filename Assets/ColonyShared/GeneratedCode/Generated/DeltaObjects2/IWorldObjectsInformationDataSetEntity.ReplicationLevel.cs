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
    public class WorldObjectsInformationDataSetEntityAlways : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldObjectsInformationDataSetEntityAlways
    {
        public WorldObjectsInformationDataSetEntityAlways(SharedCode.Entities.IWorldObjectsInformationDataSetEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.IWorldObjectsInformationDataSetEntity __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.IWorldObjectsInformationDataSetEntity)__deltaObjectBase__;
            }
        }

        public override int TypeId => 1985203466;
    }

    public class WorldObjectsInformationDataSetEntityClientBroadcast : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldObjectsInformationDataSetEntityClientBroadcast
    {
        public WorldObjectsInformationDataSetEntityClientBroadcast(SharedCode.Entities.IWorldObjectsInformationDataSetEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.IWorldObjectsInformationDataSetEntity __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.IWorldObjectsInformationDataSetEntity)__deltaObjectBase__;
            }
        }

        IDeltaDictionaryWrapper<ResourceSystem.Utils.OuterRef, GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldObjectPositionInformationClientBroadcast> __Positions__Wrapper__;
        public IDeltaDictionaryWrapper<ResourceSystem.Utils.OuterRef, GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldObjectPositionInformationClientBroadcast> Positions
        {
            get
            {
                if (__Positions__Wrapper__ == null || ((IBaseDeltaObjectWrapper)__Positions__Wrapper__).GetBaseDeltaObject() != __deltaObject__.Positions)
                    __Positions__Wrapper__ = __deltaObject__.Positions == null ? null : new DeltaDictionaryWrapper<ResourceSystem.Utils.OuterRef, SharedCode.Entities.IWorldObjectPositionInformation, GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldObjectPositionInformationClientBroadcast>(__deltaObject__.Positions);
                return __Positions__Wrapper__;
            }
        }

        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = Positions;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => 723509199;
    }

    public class WorldObjectsInformationDataSetEntityClientFullApi : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldObjectsInformationDataSetEntityClientFullApi
    {
        public WorldObjectsInformationDataSetEntityClientFullApi(SharedCode.Entities.IWorldObjectsInformationDataSetEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.IWorldObjectsInformationDataSetEntity __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.IWorldObjectsInformationDataSetEntity)__deltaObjectBase__;
            }
        }

        public override int TypeId => -1390251286;
    }

    public class WorldObjectsInformationDataSetEntityClientFull : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldObjectsInformationDataSetEntityClientFull
    {
        public WorldObjectsInformationDataSetEntityClientFull(SharedCode.Entities.IWorldObjectsInformationDataSetEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.IWorldObjectsInformationDataSetEntity __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.IWorldObjectsInformationDataSetEntity)__deltaObjectBase__;
            }
        }

        IDeltaDictionaryWrapper<ResourceSystem.Utils.OuterRef, GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldObjectPositionInformationClientFull> __Positions__Wrapper__;
        public IDeltaDictionaryWrapper<ResourceSystem.Utils.OuterRef, GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldObjectPositionInformationClientFull> Positions
        {
            get
            {
                if (__Positions__Wrapper__ == null || ((IBaseDeltaObjectWrapper)__Positions__Wrapper__).GetBaseDeltaObject() != __deltaObject__.Positions)
                    __Positions__Wrapper__ = __deltaObject__.Positions == null ? null : new DeltaDictionaryWrapper<ResourceSystem.Utils.OuterRef, SharedCode.Entities.IWorldObjectPositionInformation, GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldObjectPositionInformationClientFull>(__deltaObject__.Positions);
                return __Positions__Wrapper__;
            }
        }

        public GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldObjectsInformationDataSetEngineClientFull WorldObjectsInformationDataSetEngine => (GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldObjectsInformationDataSetEngineClientFull)__deltaObject__.WorldObjectsInformationDataSetEngine?.GetReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientFull);
        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = Positions;
                    break;
                case 11:
                    currProperty = WorldObjectsInformationDataSetEngine;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => -785615139;
    }

    public class WorldObjectsInformationDataSetEntityServerApi : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldObjectsInformationDataSetEntityServerApi
    {
        public WorldObjectsInformationDataSetEntityServerApi(SharedCode.Entities.IWorldObjectsInformationDataSetEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.IWorldObjectsInformationDataSetEntity __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.IWorldObjectsInformationDataSetEntity)__deltaObjectBase__;
            }
        }

        public override int TypeId => -1901585079;
    }

    public class WorldObjectsInformationDataSetEntityServer : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldObjectsInformationDataSetEntityServer
    {
        public WorldObjectsInformationDataSetEntityServer(SharedCode.Entities.IWorldObjectsInformationDataSetEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.IWorldObjectsInformationDataSetEntity __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.IWorldObjectsInformationDataSetEntity)__deltaObjectBase__;
            }
        }

        IDeltaDictionaryWrapper<ResourceSystem.Utils.OuterRef, GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldObjectPositionInformationServer> __Positions__Wrapper__;
        public IDeltaDictionaryWrapper<ResourceSystem.Utils.OuterRef, GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldObjectPositionInformationServer> Positions
        {
            get
            {
                if (__Positions__Wrapper__ == null || ((IBaseDeltaObjectWrapper)__Positions__Wrapper__).GetBaseDeltaObject() != __deltaObject__.Positions)
                    __Positions__Wrapper__ = __deltaObject__.Positions == null ? null : new DeltaDictionaryWrapper<ResourceSystem.Utils.OuterRef, SharedCode.Entities.IWorldObjectPositionInformation, GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldObjectPositionInformationServer>(__deltaObject__.Positions);
                return __Positions__Wrapper__;
            }
        }

        public GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldObjectsInformationDataSetEngineServer WorldObjectsInformationDataSetEngine => (GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldObjectsInformationDataSetEngineServer)__deltaObject__.WorldObjectsInformationDataSetEngine?.GetReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Server);
        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = Positions;
                    break;
                case 11:
                    currProperty = WorldObjectsInformationDataSetEngine;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => 905339645;
    }
}

namespace GeneratedCode.DeltaObjects
{
    public class WorldObjectPositionInformationAlways : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldObjectPositionInformationAlways
    {
        public WorldObjectPositionInformationAlways(SharedCode.Entities.IWorldObjectPositionInformation deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.IWorldObjectPositionInformation __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.IWorldObjectPositionInformation)__deltaObjectBase__;
            }
        }

        public override int TypeId => 1921661402;
    }

    public class WorldObjectPositionInformationClientBroadcast : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldObjectPositionInformationClientBroadcast
    {
        public WorldObjectPositionInformationClientBroadcast(SharedCode.Entities.IWorldObjectPositionInformation deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.IWorldObjectPositionInformation __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.IWorldObjectPositionInformation)__deltaObjectBase__;
            }
        }

        public SharedCode.Utils.Vector3 Position => __deltaObject__.Position;
        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = Position;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => 99721692;
    }

    public class WorldObjectPositionInformationClientFullApi : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldObjectPositionInformationClientFullApi
    {
        public WorldObjectPositionInformationClientFullApi(SharedCode.Entities.IWorldObjectPositionInformation deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.IWorldObjectPositionInformation __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.IWorldObjectPositionInformation)__deltaObjectBase__;
            }
        }

        public override int TypeId => -374946138;
    }

    public class WorldObjectPositionInformationClientFull : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldObjectPositionInformationClientFull
    {
        public WorldObjectPositionInformationClientFull(SharedCode.Entities.IWorldObjectPositionInformation deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.IWorldObjectPositionInformation __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.IWorldObjectPositionInformation)__deltaObjectBase__;
            }
        }

        public SharedCode.Utils.Vector3 Position => __deltaObject__.Position;
        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = Position;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => -333225790;
    }

    public class WorldObjectPositionInformationServerApi : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldObjectPositionInformationServerApi
    {
        public WorldObjectPositionInformationServerApi(SharedCode.Entities.IWorldObjectPositionInformation deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.IWorldObjectPositionInformation __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.IWorldObjectPositionInformation)__deltaObjectBase__;
            }
        }

        public override int TypeId => 978474238;
    }

    public class WorldObjectPositionInformationServer : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldObjectPositionInformationServer
    {
        public WorldObjectPositionInformationServer(SharedCode.Entities.IWorldObjectPositionInformation deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.IWorldObjectPositionInformation __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.IWorldObjectPositionInformation)__deltaObjectBase__;
            }
        }

        public SharedCode.Utils.Vector3 Position => __deltaObject__.Position;
        public System.Threading.Tasks.Task<bool> SetPosition(SharedCode.Utils.Vector3 position)
        {
            return __deltaObject__.SetPosition(position);
        }

        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = Position;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => -1173208922;
    }
}

namespace GeneratedCode.DeltaObjects
{
    public class CharacterPositionInformationAlways : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ICharacterPositionInformationAlways
    {
        public CharacterPositionInformationAlways(SharedCode.Entities.ICharacterPositionInformation deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.ICharacterPositionInformation __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.ICharacterPositionInformation)__deltaObjectBase__;
            }
        }

        public override int TypeId => 1124690644;
    }

    public class CharacterPositionInformationClientBroadcast : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ICharacterPositionInformationClientBroadcast
    {
        public CharacterPositionInformationClientBroadcast(SharedCode.Entities.ICharacterPositionInformation deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.ICharacterPositionInformation __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.ICharacterPositionInformation)__deltaObjectBase__;
            }
        }

        public Assets.Src.Aspects.Impl.Factions.Template.MutationStageDef Mutation => __deltaObject__.Mutation;
        public SharedCode.Utils.Vector3 Position => __deltaObject__.Position;
        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = Mutation;
                    break;
                case 11:
                    currProperty = Position;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => -472089685;
    }

    public class CharacterPositionInformationClientFullApi : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ICharacterPositionInformationClientFullApi
    {
        public CharacterPositionInformationClientFullApi(SharedCode.Entities.ICharacterPositionInformation deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.ICharacterPositionInformation __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.ICharacterPositionInformation)__deltaObjectBase__;
            }
        }

        public override int TypeId => -254246932;
    }

    public class CharacterPositionInformationClientFull : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ICharacterPositionInformationClientFull
    {
        public CharacterPositionInformationClientFull(SharedCode.Entities.ICharacterPositionInformation deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.ICharacterPositionInformation __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.ICharacterPositionInformation)__deltaObjectBase__;
            }
        }

        public Assets.Src.Aspects.Impl.Factions.Template.MutationStageDef Mutation => __deltaObject__.Mutation;
        public SharedCode.Utils.Vector3 Position => __deltaObject__.Position;
        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = Mutation;
                    break;
                case 11:
                    currProperty = Position;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => -70487775;
    }

    public class CharacterPositionInformationServerApi : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ICharacterPositionInformationServerApi
    {
        public CharacterPositionInformationServerApi(SharedCode.Entities.ICharacterPositionInformation deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.ICharacterPositionInformation __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.ICharacterPositionInformation)__deltaObjectBase__;
            }
        }

        public override int TypeId => 1624134540;
    }

    public class CharacterPositionInformationServer : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ICharacterPositionInformationServer
    {
        public CharacterPositionInformationServer(SharedCode.Entities.ICharacterPositionInformation deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.ICharacterPositionInformation __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.ICharacterPositionInformation)__deltaObjectBase__;
            }
        }

        public Assets.Src.Aspects.Impl.Factions.Template.MutationStageDef Mutation => __deltaObject__.Mutation;
        public SharedCode.Utils.Vector3 Position => __deltaObject__.Position;
        public System.Threading.Tasks.Task<bool> SetMutation(Assets.Src.Aspects.Impl.Factions.Template.MutationStageDef mutation)
        {
            return __deltaObject__.SetMutation(mutation);
        }

        public System.Threading.Tasks.Task<bool> SetPosition(SharedCode.Utils.Vector3 position)
        {
            return __deltaObject__.SetPosition(position);
        }

        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = Mutation;
                    break;
                case 11:
                    currProperty = Position;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => -1366107112;
    }
}