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
    public class WorldObjectsInformationSetsMapEngineAlways : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldObjectsInformationSetsMapEngineAlways
    {
        public WorldObjectsInformationSetsMapEngineAlways(SharedCode.Entities.IWorldObjectsInformationSetsMapEngine deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.IWorldObjectsInformationSetsMapEngine __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.IWorldObjectsInformationSetsMapEngine)__deltaObjectBase__;
            }
        }

        public override int TypeId => 770353439;
    }

    public class WorldObjectsInformationSetsMapEngineClientBroadcast : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldObjectsInformationSetsMapEngineClientBroadcast
    {
        public WorldObjectsInformationSetsMapEngineClientBroadcast(SharedCode.Entities.IWorldObjectsInformationSetsMapEngine deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.IWorldObjectsInformationSetsMapEngine __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.IWorldObjectsInformationSetsMapEngine)__deltaObjectBase__;
            }
        }

        public override int TypeId => -399046480;
    }

    public class WorldObjectsInformationSetsMapEngineClientFullApi : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldObjectsInformationSetsMapEngineClientFullApi
    {
        public WorldObjectsInformationSetsMapEngineClientFullApi(SharedCode.Entities.IWorldObjectsInformationSetsMapEngine deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.IWorldObjectsInformationSetsMapEngine __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.IWorldObjectsInformationSetsMapEngine)__deltaObjectBase__;
            }
        }

        public override int TypeId => -683880371;
    }

    public class WorldObjectsInformationSetsMapEngineClientFull : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldObjectsInformationSetsMapEngineClientFull
    {
        public WorldObjectsInformationSetsMapEngineClientFull(SharedCode.Entities.IWorldObjectsInformationSetsMapEngine deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.IWorldObjectsInformationSetsMapEngine __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.IWorldObjectsInformationSetsMapEngine)__deltaObjectBase__;
            }
        }

        public override int TypeId => 133414825;
    }

    public class WorldObjectsInformationSetsMapEngineServerApi : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldObjectsInformationSetsMapEngineServerApi
    {
        public WorldObjectsInformationSetsMapEngineServerApi(SharedCode.Entities.IWorldObjectsInformationSetsMapEngine deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.IWorldObjectsInformationSetsMapEngine __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.IWorldObjectsInformationSetsMapEngine)__deltaObjectBase__;
            }
        }

        public override int TypeId => -75578232;
    }

    public class WorldObjectsInformationSetsMapEngineServer : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IWorldObjectsInformationSetsMapEngineServer
    {
        public WorldObjectsInformationSetsMapEngineServer(SharedCode.Entities.IWorldObjectsInformationSetsMapEngine deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.IWorldObjectsInformationSetsMapEngine __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.IWorldObjectsInformationSetsMapEngine)__deltaObjectBase__;
            }
        }

        public System.Threading.Tasks.Task<System.Collections.Generic.Dictionary<Entities.GameMapData.WorldObjectInformationSetDef, ResourceSystem.Utils.OuterRef>> Subscribe(System.Collections.Generic.List<Entities.GameMapData.WorldObjectInformationSetDef> worldObjectSetsDef, System.Guid repositoryId)
        {
            return __deltaObject__.Subscribe(worldObjectSetsDef, repositoryId);
        }

        public System.Threading.Tasks.Task<bool> Unsubscribe(System.Collections.Generic.List<Entities.GameMapData.WorldObjectInformationSetDef> worldObjectSetsDef, System.Guid repositoryId)
        {
            return __deltaObject__.Unsubscribe(worldObjectSetsDef, repositoryId);
        }

        public System.Threading.Tasks.Task<bool> AddWorldObject(ResourceSystem.Utils.OuterRef worldObjectRef)
        {
            return __deltaObject__.AddWorldObject(worldObjectRef);
        }

        public System.Threading.Tasks.Task<bool> RemoveWorldObject(ResourceSystem.Utils.OuterRef worldObjectRef)
        {
            return __deltaObject__.RemoveWorldObject(worldObjectRef);
        }

        public override int TypeId => -591455027;
    }
}