// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class SceneEntity
    {
        public override void Visit(System.Action<SharedCode.EntitySystem.IDeltaObject> visitor)
        {
            base.Visit(visitor);
            ((SharedCode.EntitySystem.IDeltaObjectExt)ObjectsToLoad)?.Visit(visitor);
            ((SharedCode.EntitySystem.IDeltaObjectExt)WorldSpaced)?.Visit(visitor);
            ((SharedCode.EntitySystem.IDeltaObjectExt)MovementSync)?.Visit(visitor);
        }
    }
}

namespace GeneratedCode.DeltaObjects
{
    public partial class EventPoint
    {
        public override void Visit(System.Action<SharedCode.EntitySystem.IDeltaObject> visitor)
        {
            base.Visit(visitor);
            ((SharedCode.EntitySystem.IDeltaObjectExt)WorldSpaced)?.Visit(visitor);
            ((SharedCode.EntitySystem.IDeltaObjectExt)MovementSync)?.Visit(visitor);
            ((SharedCode.EntitySystem.IDeltaObjectExt)LinksEngine)?.Visit(visitor);
        }
    }
}

namespace GeneratedCode.DeltaObjects
{
    public partial class Storyteller
    {
        public override void Visit(System.Action<SharedCode.EntitySystem.IDeltaObject> visitor)
        {
            base.Visit(visitor);
            ((SharedCode.EntitySystem.IDeltaObjectExt)WorldSpaced)?.Visit(visitor);
            ((SharedCode.EntitySystem.IDeltaObjectExt)Buffs)?.Visit(visitor);
            ((SharedCode.EntitySystem.IDeltaObjectExt)LinksEngine)?.Visit(visitor);
            ((SharedCode.EntitySystem.IDeltaObjectExt)Stats)?.Visit(visitor);
        }
    }
}

namespace GeneratedCode.DeltaObjects
{
    public partial class EventInstance
    {
        public override void Visit(System.Action<SharedCode.EntitySystem.IDeltaObject> visitor)
        {
            base.Visit(visitor);
            ((SharedCode.EntitySystem.IDeltaObjectExt)WorldSpaced)?.Visit(visitor);
            ((SharedCode.EntitySystem.IDeltaObjectExt)Buffs)?.Visit(visitor);
            ((SharedCode.EntitySystem.IDeltaObjectExt)LinksEngine)?.Visit(visitor);
            ((SharedCode.EntitySystem.IDeltaObjectExt)Stats)?.Visit(visitor);
            ((SharedCode.EntitySystem.IDeltaObjectExt)MovementSync)?.Visit(visitor);
        }
    }
}