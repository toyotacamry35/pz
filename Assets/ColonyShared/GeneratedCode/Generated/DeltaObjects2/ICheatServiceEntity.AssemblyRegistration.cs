// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    [SharedCode.EntitySystem.ReplicationClassImplementation(typeof(SharedCode.Entities.Service.ICheatServiceAgentEntity), 1197878879)]
    public partial class CheatServiceAgentEntity
    {
        public override int TypeId => 1197878879;
        public static int StaticTypeId => 1197878879;
        public static SharedCode.EntitySystem.IEntity Construct(System.Guid id) => new CheatServiceAgentEntity(id);
    }
}

namespace GeneratedCode.DeltaObjects
{
    [SharedCode.EntitySystem.ReplicationClassImplementation(typeof(SharedCode.Entities.Service.ICheatServiceEntity), -28966299)]
    public partial class CheatServiceEntity
    {
        public override int TypeId => -28966299;
        public static int StaticTypeId => -28966299;
        public static SharedCode.EntitySystem.IEntity Construct(System.Guid id) => new CheatServiceEntity(id);
    }
}