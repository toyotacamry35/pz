// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class WorldPersonalMachine
    {
        public override void Visit(System.Action<SharedCode.EntitySystem.IDeltaObject> visitor)
        {
            base.Visit(visitor);
            ((SharedCode.EntitySystem.IDeltaObjectExt)AutoAddToWorldSpace)?.Visit(visitor);
            ((SharedCode.EntitySystem.IDeltaObjectExt)WorldSpaced)?.Visit(visitor);
            ((SharedCode.EntitySystem.IDeltaObjectExt)MovementSync)?.Visit(visitor);
            ((SharedCode.EntitySystem.IDeltaObjectExt)Stats)?.Visit(visitor);
            ((SharedCode.EntitySystem.IDeltaObjectExt)Health)?.Visit(visitor);
            ((SharedCode.EntitySystem.IDeltaObjectExt)SlaveWizardHolder)?.Visit(visitor);
            ((SharedCode.EntitySystem.IDeltaObjectExt)OpenMechanics)?.Visit(visitor);
        }
    }
}