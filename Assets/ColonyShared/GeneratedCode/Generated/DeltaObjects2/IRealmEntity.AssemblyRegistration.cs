// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    [SharedCode.EntitySystem.ReplicationClassImplementation(typeof(SharedCode.Entities.IRealmsCollectionEntity), 1140165166)]
    public partial class RealmsCollectionEntity
    {
        public override int TypeId => 1140165166;
        public static int StaticTypeId => 1140165166;
        public static SharedCode.EntitySystem.IEntity Construct(System.Guid id) => new RealmsCollectionEntity(id);
    }
}

namespace GeneratedCode.DeltaObjects
{
    [SharedCode.EntitySystem.ReplicationClassImplementation(typeof(SharedCode.Entities.IRealmEntity), 926552518)]
    public partial class RealmEntity
    {
        public override int TypeId => 926552518;
        public static int StaticTypeId => 926552518;
        public static SharedCode.EntitySystem.IEntity Construct(System.Guid id) => new RealmEntity(id);
    }
}