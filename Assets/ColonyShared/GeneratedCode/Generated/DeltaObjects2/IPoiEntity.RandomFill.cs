// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class PoiEntity
    {
        protected override void RandomFill(int __count__, System.Random __random__, bool __withReadOnly__)
        {
            __count__--;
            if (__count__ <= 0)
                return;
            base.RandomFill(__count__, __random__, __withReadOnly__);
            var random = new System.Random(System.Guid.NewGuid().ToString().GetHashCode());
            if (__withReadOnly__)
            {
                var Def__rnd = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<SharedCode.Entities.GameObjectEntities.IEntityObjectDef>(__random__);
                Def = Def__rnd;
            }

            {
                var MapOwner__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<GeneratedCode.MapSystem.MapOwner>();
                MapOwner = MapOwner__rnd;
            }

            if (__withReadOnly__)
            {
                var StaticIdFromExport__rnd = System.Guid.NewGuid();
                StaticIdFromExport = StaticIdFromExport__rnd;
            }

            {
                ((ResourcesSystem.Base.IHasRandomFill)MovementSync)?.Fill(__count__, __random__, __withReadOnly__);
            }

            {
                ((ResourcesSystem.Base.IHasRandomFill)AutoAddToWorldSpace)?.Fill(__count__, __random__, __withReadOnly__);
            }

            {
                ((ResourcesSystem.Base.IHasRandomFill)WorldSpaced)?.Fill(__count__, __random__, __withReadOnly__);
            }
        }
    }
}