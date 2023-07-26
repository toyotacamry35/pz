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
    public class WorldObjectInformationSetsEngineAlways : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldObjectInformationSetsEngineAlways
    {
        public WorldObjectInformationSetsEngineAlways(SharedCode.Entities.IWorldObjectInformationSetsEngine deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.IWorldObjectInformationSetsEngine __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.IWorldObjectInformationSetsEngine)__deltaObjectBase__;
            }
        }

        public override int TypeId => 906474325;
    }

    public class WorldObjectInformationSetsEngineClientBroadcast : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldObjectInformationSetsEngineClientBroadcast
    {
        public WorldObjectInformationSetsEngineClientBroadcast(SharedCode.Entities.IWorldObjectInformationSetsEngine deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.IWorldObjectInformationSetsEngine __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.IWorldObjectInformationSetsEngine)__deltaObjectBase__;
            }
        }

        public override int TypeId => 1810407392;
    }

    public class WorldObjectInformationSetsEngineClientFullApi : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldObjectInformationSetsEngineClientFullApi
    {
        public WorldObjectInformationSetsEngineClientFullApi(SharedCode.Entities.IWorldObjectInformationSetsEngine deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.IWorldObjectInformationSetsEngine __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.IWorldObjectInformationSetsEngine)__deltaObjectBase__;
            }
        }

        public override int TypeId => 1342501370;
    }

    public class WorldObjectInformationSetsEngineClientFull : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldObjectInformationSetsEngineClientFull
    {
        public WorldObjectInformationSetsEngineClientFull(SharedCode.Entities.IWorldObjectInformationSetsEngine deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.IWorldObjectInformationSetsEngine __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.IWorldObjectInformationSetsEngine)__deltaObjectBase__;
            }
        }

        public IDeltaDictionary<Entities.GameMapData.WorldObjectInformationClientSubSetDef, ResourceSystem.Utils.OuterRef> CurrentWorldObjectInformationRefs
        {
            get
            {
                return __deltaObject__.CurrentWorldObjectInformationRefs;
            }
        }

        public System.Threading.Tasks.Task<SharedCode.Entities.AddWorldObjectInformationSubSetResult> AddWorldObjectInformationSubSetCheat(Entities.GameMapData.WorldObjectInformationClientSubSetDef subSetDef)
        {
            return __deltaObject__.AddWorldObjectInformationSubSetCheat(subSetDef);
        }

        public System.Threading.Tasks.Task<SharedCode.Entities.RemoveWorldObjectInformationSubSetResult> RemoveWorldObjectInformationSubSetCheat(Entities.GameMapData.WorldObjectInformationClientSubSetDef subSetDef)
        {
            return __deltaObject__.RemoveWorldObjectInformationSubSetCheat(subSetDef);
        }

        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 11:
                    currProperty = CurrentWorldObjectInformationRefs;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => -1910509351;
    }

    public class WorldObjectInformationSetsEngineServerApi : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldObjectInformationSetsEngineServerApi
    {
        public WorldObjectInformationSetsEngineServerApi(SharedCode.Entities.IWorldObjectInformationSetsEngine deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.IWorldObjectInformationSetsEngine __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.IWorldObjectInformationSetsEngine)__deltaObjectBase__;
            }
        }

        public override int TypeId => 1066475187;
    }

    public class WorldObjectInformationSetsEngineServer : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldObjectInformationSetsEngineServer
    {
        public WorldObjectInformationSetsEngineServer(SharedCode.Entities.IWorldObjectInformationSetsEngine deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.IWorldObjectInformationSetsEngine __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.IWorldObjectInformationSetsEngine)__deltaObjectBase__;
            }
        }

        public IDeltaDictionary<Entities.GameMapData.WorldObjectInformationClientSubSetDef, ResourceSystem.Utils.OuterRef> CurrentWorldObjectInformationRefs
        {
            get
            {
                return __deltaObject__.CurrentWorldObjectInformationRefs;
            }
        }

        public System.Threading.Tasks.Task<SharedCode.Entities.AddWorldObjectInformationSubSetResult> AddWorldObjectInformationSubSet(Entities.GameMapData.WorldObjectInformationClientSubSetDef subSetDef)
        {
            return __deltaObject__.AddWorldObjectInformationSubSet(subSetDef);
        }

        public System.Threading.Tasks.Task<SharedCode.Entities.RemoveWorldObjectInformationSubSetResult> RemoveWorldObjectInformationSubSet(Entities.GameMapData.WorldObjectInformationClientSubSetDef subSetDef)
        {
            return __deltaObject__.RemoveWorldObjectInformationSubSet(subSetDef);
        }

        public System.Threading.Tasks.Task<SharedCode.Entities.AddWorldObjectInformationSubSetResult> AddWorldObjectInformationSubSetCheat(Entities.GameMapData.WorldObjectInformationClientSubSetDef subSetDef)
        {
            return __deltaObject__.AddWorldObjectInformationSubSetCheat(subSetDef);
        }

        public System.Threading.Tasks.Task<SharedCode.Entities.RemoveWorldObjectInformationSubSetResult> RemoveWorldObjectInformationSubSetCheat(Entities.GameMapData.WorldObjectInformationClientSubSetDef subSetDef)
        {
            return __deltaObject__.RemoveWorldObjectInformationSubSetCheat(subSetDef);
        }

        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 11:
                    currProperty = CurrentWorldObjectInformationRefs;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => -528260189;
    }
}