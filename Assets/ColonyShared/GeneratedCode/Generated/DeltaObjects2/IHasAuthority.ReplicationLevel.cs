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
    public class AuthorityOwnerAlways : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IAuthorityOwnerAlways
    {
        public AuthorityOwnerAlways(Assets.ColonyShared.SharedCode.Entities.IAuthorityOwner deltaObject): base(deltaObject)
        {
        }

        Assets.ColonyShared.SharedCode.Entities.IAuthorityOwner __deltaObject__
        {
            get
            {
                return (Assets.ColonyShared.SharedCode.Entities.IAuthorityOwner)__deltaObjectBase__;
            }
        }

        public override int TypeId => 864864020;
    }

    public class AuthorityOwnerClientBroadcast : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IAuthorityOwnerClientBroadcast
    {
        public AuthorityOwnerClientBroadcast(Assets.ColonyShared.SharedCode.Entities.IAuthorityOwner deltaObject): base(deltaObject)
        {
        }

        Assets.ColonyShared.SharedCode.Entities.IAuthorityOwner __deltaObject__
        {
            get
            {
                return (Assets.ColonyShared.SharedCode.Entities.IAuthorityOwner)__deltaObjectBase__;
            }
        }

        public override int TypeId => -2029824746;
    }

    public class AuthorityOwnerClientFullApi : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IAuthorityOwnerClientFullApi
    {
        public AuthorityOwnerClientFullApi(Assets.ColonyShared.SharedCode.Entities.IAuthorityOwner deltaObject): base(deltaObject)
        {
        }

        Assets.ColonyShared.SharedCode.Entities.IAuthorityOwner __deltaObject__
        {
            get
            {
                return (Assets.ColonyShared.SharedCode.Entities.IAuthorityOwner)__deltaObjectBase__;
            }
        }

        public override int TypeId => 1736464132;
    }

    public class AuthorityOwnerClientFull : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IAuthorityOwnerClientFull
    {
        public AuthorityOwnerClientFull(Assets.ColonyShared.SharedCode.Entities.IAuthorityOwner deltaObject): base(deltaObject)
        {
        }

        Assets.ColonyShared.SharedCode.Entities.IAuthorityOwner __deltaObject__
        {
            get
            {
                return (Assets.ColonyShared.SharedCode.Entities.IAuthorityOwner)__deltaObjectBase__;
            }
        }

        public bool HasClientAuthority => __deltaObject__.HasClientAuthority;
        public override int TypeId => -55830274;
    }

    public class AuthorityOwnerServerApi : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IAuthorityOwnerServerApi
    {
        public AuthorityOwnerServerApi(Assets.ColonyShared.SharedCode.Entities.IAuthorityOwner deltaObject): base(deltaObject)
        {
        }

        Assets.ColonyShared.SharedCode.Entities.IAuthorityOwner __deltaObject__
        {
            get
            {
                return (Assets.ColonyShared.SharedCode.Entities.IAuthorityOwner)__deltaObjectBase__;
            }
        }

        public override int TypeId => 138951238;
    }

    public class AuthorityOwnerServer : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IAuthorityOwnerServer
    {
        public AuthorityOwnerServer(Assets.ColonyShared.SharedCode.Entities.IAuthorityOwner deltaObject): base(deltaObject)
        {
        }

        Assets.ColonyShared.SharedCode.Entities.IAuthorityOwner __deltaObject__
        {
            get
            {
                return (Assets.ColonyShared.SharedCode.Entities.IAuthorityOwner)__deltaObjectBase__;
            }
        }

        public bool HasClientAuthority => __deltaObject__.HasClientAuthority;
        public override int TypeId => -1511137869;
    }
}