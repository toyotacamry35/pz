// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class TimeoutTestEntity
    {
        public override void Visit(System.Action<SharedCode.EntitySystem.IDeltaObject> visitor)
        {
            base.Visit(visitor);
            ((SharedCode.EntitySystem.IDeltaObjectExt)TestDeltaListInt)?.Visit(visitor);
            ((SharedCode.EntitySystem.IDeltaObjectExt)TestDeltaListDeltaObject)?.Visit(visitor);
            ((SharedCode.EntitySystem.IDeltaObjectExt)TestDeltaDictionaryInt)?.Visit(visitor);
            ((SharedCode.EntitySystem.IDeltaObjectExt)TestDeltaDictionaryIntDeltaObject)?.Visit(visitor);
        }
    }
}

namespace GeneratedCode.DeltaObjects
{
    public partial class TimeoutSubTestEntity
    {
        public override void Visit(System.Action<SharedCode.EntitySystem.IDeltaObject> visitor)
        {
            base.Visit(visitor);
            ((SharedCode.EntitySystem.IDeltaObjectExt)TestDeltaListInt)?.Visit(visitor);
            ((SharedCode.EntitySystem.IDeltaObjectExt)TestDeltaListDeltaObject)?.Visit(visitor);
            ((SharedCode.EntitySystem.IDeltaObjectExt)TestDeltaDictionaryInt)?.Visit(visitor);
            ((SharedCode.EntitySystem.IDeltaObjectExt)TestDeltaDictionaryIntDeltaObject)?.Visit(visitor);
        }
    }
}