// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class SpawnDaemon
    {
        public override void Visit(System.Action<SharedCode.EntitySystem.IDeltaObject> visitor)
        {
            base.Visit(visitor);
            ((SharedCode.EntitySystem.IDeltaObjectExt)SpawnedObjectsAmounts)?.Visit(visitor);
            ((SharedCode.EntitySystem.IDeltaObjectExt)Maps)?.Visit(visitor);
            ((SharedCode.EntitySystem.IDeltaObjectExt)WorldSpaced)?.Visit(visitor);
            ((SharedCode.EntitySystem.IDeltaObjectExt)LinksEngine)?.Visit(visitor);
            ((SharedCode.EntitySystem.IDeltaObjectExt)MovementSync)?.Visit(visitor);
        }
    }
}