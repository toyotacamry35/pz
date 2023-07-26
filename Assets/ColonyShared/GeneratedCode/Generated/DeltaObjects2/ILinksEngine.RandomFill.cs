// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class LinksEngine
    {
        protected override void RandomFill(int __count__, System.Random __random__, bool __withReadOnly__)
        {
            __count__--;
            if (__count__ <= 0)
                return;
            base.RandomFill(__count__, __random__, __withReadOnly__);
            var random = new System.Random(System.Guid.NewGuid().ToString().GetHashCode());
            {
                _Links.Clear();
                var Linksitem0__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<GeneratedCode.DeltaObjects.LinksHolder>();
                ((ResourcesSystem.Base.IHasRandomFill)Linksitem0__rnd)?.Fill(__count__, __random__, __withReadOnly__);
                Assets.ResourceSystem.Aspects.Links.LinkTypeDef __key0__ = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<Assets.ResourceSystem.Aspects.Links.LinkTypeDef>(__random__);
                if (__key0__ != null)
                    Links[__key0__] = Linksitem0__rnd;
                var Linksitem1__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<GeneratedCode.DeltaObjects.LinksHolder>();
                ((ResourcesSystem.Base.IHasRandomFill)Linksitem1__rnd)?.Fill(__count__, __random__, __withReadOnly__);
                Assets.ResourceSystem.Aspects.Links.LinkTypeDef __key1__ = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<Assets.ResourceSystem.Aspects.Links.LinkTypeDef>(__random__);
                if (__key1__ != null)
                    Links[__key1__] = Linksitem1__rnd;
                var Linksitem2__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<GeneratedCode.DeltaObjects.LinksHolder>();
                ((ResourcesSystem.Base.IHasRandomFill)Linksitem2__rnd)?.Fill(__count__, __random__, __withReadOnly__);
                Assets.ResourceSystem.Aspects.Links.LinkTypeDef __key2__ = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<Assets.ResourceSystem.Aspects.Links.LinkTypeDef>(__random__);
                if (__key2__ != null)
                    Links[__key2__] = Linksitem2__rnd;
                var Linksitem3__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<GeneratedCode.DeltaObjects.LinksHolder>();
                ((ResourcesSystem.Base.IHasRandomFill)Linksitem3__rnd)?.Fill(__count__, __random__, __withReadOnly__);
                Assets.ResourceSystem.Aspects.Links.LinkTypeDef __key3__ = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<Assets.ResourceSystem.Aspects.Links.LinkTypeDef>(__random__);
                if (__key3__ != null)
                    Links[__key3__] = Linksitem3__rnd;
                var Linksitem4__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<GeneratedCode.DeltaObjects.LinksHolder>();
                ((ResourcesSystem.Base.IHasRandomFill)Linksitem4__rnd)?.Fill(__count__, __random__, __withReadOnly__);
                Assets.ResourceSystem.Aspects.Links.LinkTypeDef __key4__ = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<Assets.ResourceSystem.Aspects.Links.LinkTypeDef>(__random__);
                if (__key4__ != null)
                    Links[__key4__] = Linksitem4__rnd;
                var Linksitem5__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<GeneratedCode.DeltaObjects.LinksHolder>();
                ((ResourcesSystem.Base.IHasRandomFill)Linksitem5__rnd)?.Fill(__count__, __random__, __withReadOnly__);
                Assets.ResourceSystem.Aspects.Links.LinkTypeDef __key5__ = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<Assets.ResourceSystem.Aspects.Links.LinkTypeDef>(__random__);
                if (__key5__ != null)
                    Links[__key5__] = Linksitem5__rnd;
                var Linksitem6__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<GeneratedCode.DeltaObjects.LinksHolder>();
                ((ResourcesSystem.Base.IHasRandomFill)Linksitem6__rnd)?.Fill(__count__, __random__, __withReadOnly__);
                Assets.ResourceSystem.Aspects.Links.LinkTypeDef __key6__ = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<Assets.ResourceSystem.Aspects.Links.LinkTypeDef>(__random__);
                if (__key6__ != null)
                    Links[__key6__] = Linksitem6__rnd;
                var Linksitem7__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<GeneratedCode.DeltaObjects.LinksHolder>();
                ((ResourcesSystem.Base.IHasRandomFill)Linksitem7__rnd)?.Fill(__count__, __random__, __withReadOnly__);
                Assets.ResourceSystem.Aspects.Links.LinkTypeDef __key7__ = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<Assets.ResourceSystem.Aspects.Links.LinkTypeDef>(__random__);
                if (__key7__ != null)
                    Links[__key7__] = Linksitem7__rnd;
                var Linksitem8__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<GeneratedCode.DeltaObjects.LinksHolder>();
                ((ResourcesSystem.Base.IHasRandomFill)Linksitem8__rnd)?.Fill(__count__, __random__, __withReadOnly__);
                Assets.ResourceSystem.Aspects.Links.LinkTypeDef __key8__ = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<Assets.ResourceSystem.Aspects.Links.LinkTypeDef>(__random__);
                if (__key8__ != null)
                    Links[__key8__] = Linksitem8__rnd;
            }

            {
                _SavedLinks.Clear();
                var SavedLinksitem0__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<GeneratedCode.DeltaObjects.LinksHolder>();
                ((ResourcesSystem.Base.IHasRandomFill)SavedLinksitem0__rnd)?.Fill(__count__, __random__, __withReadOnly__);
                Assets.ResourceSystem.Aspects.Links.LinkTypeDef __key0__ = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<Assets.ResourceSystem.Aspects.Links.LinkTypeDef>(__random__);
                if (__key0__ != null)
                    SavedLinks[__key0__] = SavedLinksitem0__rnd;
                var SavedLinksitem1__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<GeneratedCode.DeltaObjects.LinksHolder>();
                ((ResourcesSystem.Base.IHasRandomFill)SavedLinksitem1__rnd)?.Fill(__count__, __random__, __withReadOnly__);
                Assets.ResourceSystem.Aspects.Links.LinkTypeDef __key1__ = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<Assets.ResourceSystem.Aspects.Links.LinkTypeDef>(__random__);
                if (__key1__ != null)
                    SavedLinks[__key1__] = SavedLinksitem1__rnd;
                var SavedLinksitem2__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<GeneratedCode.DeltaObjects.LinksHolder>();
                ((ResourcesSystem.Base.IHasRandomFill)SavedLinksitem2__rnd)?.Fill(__count__, __random__, __withReadOnly__);
                Assets.ResourceSystem.Aspects.Links.LinkTypeDef __key2__ = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<Assets.ResourceSystem.Aspects.Links.LinkTypeDef>(__random__);
                if (__key2__ != null)
                    SavedLinks[__key2__] = SavedLinksitem2__rnd;
                var SavedLinksitem3__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<GeneratedCode.DeltaObjects.LinksHolder>();
                ((ResourcesSystem.Base.IHasRandomFill)SavedLinksitem3__rnd)?.Fill(__count__, __random__, __withReadOnly__);
                Assets.ResourceSystem.Aspects.Links.LinkTypeDef __key3__ = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<Assets.ResourceSystem.Aspects.Links.LinkTypeDef>(__random__);
                if (__key3__ != null)
                    SavedLinks[__key3__] = SavedLinksitem3__rnd;
                var SavedLinksitem4__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<GeneratedCode.DeltaObjects.LinksHolder>();
                ((ResourcesSystem.Base.IHasRandomFill)SavedLinksitem4__rnd)?.Fill(__count__, __random__, __withReadOnly__);
                Assets.ResourceSystem.Aspects.Links.LinkTypeDef __key4__ = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<Assets.ResourceSystem.Aspects.Links.LinkTypeDef>(__random__);
                if (__key4__ != null)
                    SavedLinks[__key4__] = SavedLinksitem4__rnd;
            }
        }
    }
}

namespace GeneratedCode.DeltaObjects
{
    public partial class LinksHolder
    {
        protected override void RandomFill(int __count__, System.Random __random__, bool __withReadOnly__)
        {
            __count__--;
            if (__count__ <= 0)
                return;
            base.RandomFill(__count__, __random__, __withReadOnly__);
            var random = new System.Random(System.Guid.NewGuid().ToString().GetHashCode());
            {
                _Links.Clear();
                var Linksitem0__rnd = __random__.Next() % 2 == 1;
                ResourceSystem.Utils.OuterRef __key0__ = (ResourceSystem.Utils.OuterRef)System.Activator.CreateInstance(typeof(ResourceSystem.Utils.OuterRef));
                Links[__key0__] = Linksitem0__rnd;
                var Linksitem1__rnd = __random__.Next() % 2 == 1;
                ResourceSystem.Utils.OuterRef __key1__ = (ResourceSystem.Utils.OuterRef)System.Activator.CreateInstance(typeof(ResourceSystem.Utils.OuterRef));
                Links[__key1__] = Linksitem1__rnd;
                var Linksitem2__rnd = __random__.Next() % 2 == 1;
                ResourceSystem.Utils.OuterRef __key2__ = (ResourceSystem.Utils.OuterRef)System.Activator.CreateInstance(typeof(ResourceSystem.Utils.OuterRef));
                Links[__key2__] = Linksitem2__rnd;
                var Linksitem3__rnd = __random__.Next() % 2 == 1;
                ResourceSystem.Utils.OuterRef __key3__ = (ResourceSystem.Utils.OuterRef)System.Activator.CreateInstance(typeof(ResourceSystem.Utils.OuterRef));
                Links[__key3__] = Linksitem3__rnd;
                var Linksitem4__rnd = __random__.Next() % 2 == 1;
                ResourceSystem.Utils.OuterRef __key4__ = (ResourceSystem.Utils.OuterRef)System.Activator.CreateInstance(typeof(ResourceSystem.Utils.OuterRef));
                Links[__key4__] = Linksitem4__rnd;
            }
        }
    }
}