// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class WorldObjectsInformationDataSetEntity
    {
        protected override void RandomFill(int __count__, System.Random __random__, bool __withReadOnly__)
        {
            __count__--;
            if (__count__ <= 0)
                return;
            base.RandomFill(__count__, __random__, __withReadOnly__);
            var random = new System.Random(System.Guid.NewGuid().ToString().GetHashCode());
            {
                _Positions.Clear();
                var Positionsitem0__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<GeneratedCode.DeltaObjects.WorldObjectPositionInformation>();
                ((ResourcesSystem.Base.IHasRandomFill)Positionsitem0__rnd)?.Fill(__count__, __random__, __withReadOnly__);
                ResourceSystem.Utils.OuterRef __key0__ = (ResourceSystem.Utils.OuterRef)System.Activator.CreateInstance(typeof(ResourceSystem.Utils.OuterRef));
                Positions[__key0__] = Positionsitem0__rnd;
                var Positionsitem1__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<GeneratedCode.DeltaObjects.WorldObjectPositionInformation>();
                ((ResourcesSystem.Base.IHasRandomFill)Positionsitem1__rnd)?.Fill(__count__, __random__, __withReadOnly__);
                ResourceSystem.Utils.OuterRef __key1__ = (ResourceSystem.Utils.OuterRef)System.Activator.CreateInstance(typeof(ResourceSystem.Utils.OuterRef));
                Positions[__key1__] = Positionsitem1__rnd;
                var Positionsitem2__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<GeneratedCode.DeltaObjects.WorldObjectPositionInformation>();
                ((ResourcesSystem.Base.IHasRandomFill)Positionsitem2__rnd)?.Fill(__count__, __random__, __withReadOnly__);
                ResourceSystem.Utils.OuterRef __key2__ = (ResourceSystem.Utils.OuterRef)System.Activator.CreateInstance(typeof(ResourceSystem.Utils.OuterRef));
                Positions[__key2__] = Positionsitem2__rnd;
                var Positionsitem3__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<GeneratedCode.DeltaObjects.WorldObjectPositionInformation>();
                ((ResourcesSystem.Base.IHasRandomFill)Positionsitem3__rnd)?.Fill(__count__, __random__, __withReadOnly__);
                ResourceSystem.Utils.OuterRef __key3__ = (ResourceSystem.Utils.OuterRef)System.Activator.CreateInstance(typeof(ResourceSystem.Utils.OuterRef));
                Positions[__key3__] = Positionsitem3__rnd;
                var Positionsitem4__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<GeneratedCode.DeltaObjects.WorldObjectPositionInformation>();
                ((ResourcesSystem.Base.IHasRandomFill)Positionsitem4__rnd)?.Fill(__count__, __random__, __withReadOnly__);
                ResourceSystem.Utils.OuterRef __key4__ = (ResourceSystem.Utils.OuterRef)System.Activator.CreateInstance(typeof(ResourceSystem.Utils.OuterRef));
                Positions[__key4__] = Positionsitem4__rnd;
                var Positionsitem5__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<GeneratedCode.DeltaObjects.WorldObjectPositionInformation>();
                ((ResourcesSystem.Base.IHasRandomFill)Positionsitem5__rnd)?.Fill(__count__, __random__, __withReadOnly__);
                ResourceSystem.Utils.OuterRef __key5__ = (ResourceSystem.Utils.OuterRef)System.Activator.CreateInstance(typeof(ResourceSystem.Utils.OuterRef));
                Positions[__key5__] = Positionsitem5__rnd;
                var Positionsitem6__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<GeneratedCode.DeltaObjects.WorldObjectPositionInformation>();
                ((ResourcesSystem.Base.IHasRandomFill)Positionsitem6__rnd)?.Fill(__count__, __random__, __withReadOnly__);
                ResourceSystem.Utils.OuterRef __key6__ = (ResourceSystem.Utils.OuterRef)System.Activator.CreateInstance(typeof(ResourceSystem.Utils.OuterRef));
                Positions[__key6__] = Positionsitem6__rnd;
            }

            {
                ((ResourcesSystem.Base.IHasRandomFill)WorldObjectsInformationDataSetEngine)?.Fill(__count__, __random__, __withReadOnly__);
            }
        }
    }
}

namespace GeneratedCode.DeltaObjects
{
    public partial class WorldObjectPositionInformation
    {
        protected override void RandomFill(int __count__, System.Random __random__, bool __withReadOnly__)
        {
            __count__--;
            if (__count__ <= 0)
                return;
            base.RandomFill(__count__, __random__, __withReadOnly__);
            var random = new System.Random(System.Guid.NewGuid().ToString().GetHashCode());
            {
                ((ResourcesSystem.Base.IHasRandomFill)Position)?.Fill(__count__, __random__, __withReadOnly__);
            }
        }
    }
}

namespace GeneratedCode.DeltaObjects
{
    public partial class CharacterPositionInformation
    {
        protected override void RandomFill(int __count__, System.Random __random__, bool __withReadOnly__)
        {
            __count__--;
            if (__count__ <= 0)
                return;
            base.RandomFill(__count__, __random__, __withReadOnly__);
            var random = new System.Random(System.Guid.NewGuid().ToString().GetHashCode());
            {
                var Mutation__rnd = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<Assets.Src.Aspects.Impl.Factions.Template.MutationStageDef>(__random__);
                Mutation = Mutation__rnd;
            }

            {
                ((ResourcesSystem.Base.IHasRandomFill)Position)?.Fill(__count__, __random__, __withReadOnly__);
            }
        }
    }
}