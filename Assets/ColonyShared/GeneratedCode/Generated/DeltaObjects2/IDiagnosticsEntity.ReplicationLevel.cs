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
    public class DiagnosticsEntityAlways : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IDiagnosticsEntityAlways
    {
        public DiagnosticsEntityAlways(SharedCode.Entities.IDiagnosticsEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.IDiagnosticsEntity __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.IDiagnosticsEntity)__deltaObjectBase__;
            }
        }

        public override int TypeId => 730093953;
    }

    public class DiagnosticsEntityClientBroadcast : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IDiagnosticsEntityClientBroadcast
    {
        public DiagnosticsEntityClientBroadcast(SharedCode.Entities.IDiagnosticsEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.IDiagnosticsEntity __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.IDiagnosticsEntity)__deltaObjectBase__;
            }
        }

        public override int TypeId => 995680110;
    }

    public class DiagnosticsEntityClientFullApi : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IDiagnosticsEntityClientFullApi
    {
        public DiagnosticsEntityClientFullApi(SharedCode.Entities.IDiagnosticsEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.IDiagnosticsEntity __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.IDiagnosticsEntity)__deltaObjectBase__;
            }
        }

        public override int TypeId => 1760478631;
    }

    public class DiagnosticsEntityClientFull : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IDiagnosticsEntityClientFull
    {
        public DiagnosticsEntityClientFull(SharedCode.Entities.IDiagnosticsEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.IDiagnosticsEntity __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.IDiagnosticsEntity)__deltaObjectBase__;
            }
        }

        public override int TypeId => 184287024;
    }

    public class DiagnosticsEntityServerApi : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IDiagnosticsEntityServerApi
    {
        public DiagnosticsEntityServerApi(SharedCode.Entities.IDiagnosticsEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.IDiagnosticsEntity __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.IDiagnosticsEntity)__deltaObjectBase__;
            }
        }

        public override int TypeId => -1610911537;
    }

    public class DiagnosticsEntityServer : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IDiagnosticsEntityServer
    {
        public DiagnosticsEntityServer(SharedCode.Entities.IDiagnosticsEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.IDiagnosticsEntity __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.IDiagnosticsEntity)__deltaObjectBase__;
            }
        }

        public override int TypeId => 1389366816;
    }
}