// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    [SharedCode.EntitySystem.ReplicationClassImplementation(typeof(SharedCode.MapSystem.IMapEntity), -1002562130)]
    public partial class MapEntity
    {
        public override int TypeId => -1002562130;
        public static int StaticTypeId => -1002562130;
        public static SharedCode.EntitySystem.IEntity Construct(System.Guid id) => new MapEntity(id);
    }
}

namespace GeneratedCode.DeltaObjects
{
    [SharedCode.EntitySystem.ReplicationClassImplementation(typeof(SharedCode.MapSystem.IWorldSpaceDescription), 1205230460)]
    public partial class WorldSpaceDescription
    {
        public override int TypeId => 1205230460;
    }
}