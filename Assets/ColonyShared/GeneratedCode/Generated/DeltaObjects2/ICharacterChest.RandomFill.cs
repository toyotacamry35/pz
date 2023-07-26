// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class CharacterChest
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
                var Name__rnd = "randomName__rnd" + __random__.Next(1000);
                Name = Name__rnd;
            }

            {
                var Prefab__rnd = "randomPrefab__rnd" + __random__.Next(1000);
                Prefab = Prefab__rnd;
            }

            {
                var SomeUnknownResourceThatMayBeUseful__rnd = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<Assets.Src.ResourcesSystem.Base.ISaveableResource>(__random__);
                SomeUnknownResourceThatMayBeUseful = SomeUnknownResourceThatMayBeUseful__rnd;
            }

            {
                var OnSceneObjectNetId__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<SharedCode.Entities.OnSceneObjectNetId>();
                OnSceneObjectNetId = OnSceneObjectNetId__rnd;
            }

            {
                ((ResourcesSystem.Base.IHasRandomFill)AutoAddToWorldSpace)?.Fill(__count__, __random__, __withReadOnly__);
            }

            {
                ((ResourcesSystem.Base.IHasRandomFill)WorldSpaced)?.Fill(__count__, __random__, __withReadOnly__);
            }

            {
                ((ResourcesSystem.Base.IHasRandomFill)MovementSync)?.Fill(__count__, __random__, __withReadOnly__);
            }

            {
                ((ResourcesSystem.Base.IHasRandomFill)Inventory)?.Fill(__count__, __random__, __withReadOnly__);
            }

            {
                ((ResourcesSystem.Base.IHasRandomFill)OwnerInformation)?.Fill(__count__, __random__, __withReadOnly__);
            }

            {
                ((ResourcesSystem.Base.IHasRandomFill)OpenMechanics)?.Fill(__count__, __random__, __withReadOnly__);
            }

            {
                ((ResourcesSystem.Base.IHasRandomFill)ContainerApi)?.Fill(__count__, __random__, __withReadOnly__);
            }

            {
                ((ResourcesSystem.Base.IHasRandomFill)Stats)?.Fill(__count__, __random__, __withReadOnly__);
            }

            {
                ((ResourcesSystem.Base.IHasRandomFill)Health)?.Fill(__count__, __random__, __withReadOnly__);
            }

            {
                var IncomingDamageMultiplier__rnd = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<Assets.ColonyShared.SharedCode.Aspects.Damage.Templates.DamageMultiplierDef>(__random__);
                IncomingDamageMultiplier = IncomingDamageMultiplier__rnd;
            }
        }
    }
}