// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    [SharedCode.EntitySystem.ReplicationClassImplementation(typeof(GeneratedCode.MapSystem.ISceneEntity), -1403397074)]
    public partial class SceneEntity
    {
        public override int TypeId => -1403397074;
        public static int StaticTypeId => -1403397074;
        public static SharedCode.EntitySystem.IEntity Construct(System.Guid id) => new SceneEntity(id);
    }
}

namespace GeneratedCode.DeltaObjects
{
    [SharedCode.EntitySystem.ReplicationClassImplementation(typeof(GeneratedCode.MapSystem.IEventPoint), -22513115)]
    public partial class EventPoint
    {
        public override int TypeId => -22513115;
        public static int StaticTypeId => -22513115;
        public static SharedCode.EntitySystem.IEntity Construct(System.Guid id) => new EventPoint(id);
    }
}

namespace GeneratedCode.DeltaObjects
{
    [SharedCode.EntitySystem.ReplicationClassImplementation(typeof(GeneratedCode.MapSystem.IStoryteller), -729442611)]
    public partial class Storyteller
    {
        public override int TypeId => -729442611;
        public static int StaticTypeId => -729442611;
        public static SharedCode.EntitySystem.IEntity Construct(System.Guid id) => new Storyteller(id);
    }
}

namespace GeneratedCode.DeltaObjects
{
    [SharedCode.EntitySystem.ReplicationClassImplementation(typeof(GeneratedCode.MapSystem.IEventInstance), -1050019558)]
    public partial class EventInstance
    {
        public override int TypeId => -1050019558;
        public static int StaticTypeId => -1050019558;
        public static SharedCode.EntitySystem.IEntity Construct(System.Guid id) => new EventInstance(id);
    }
}