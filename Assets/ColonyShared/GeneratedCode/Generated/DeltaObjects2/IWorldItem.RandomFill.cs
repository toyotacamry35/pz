// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class Item
    {
        protected override void RandomFill(int __count__, System.Random __random__, bool __withReadOnly__)
        {
            __count__--;
            if (__count__ <= 0)
                return;
            base.RandomFill(__count__, __random__, __withReadOnly__);
            var random = new System.Random(System.Guid.NewGuid().ToString().GetHashCode());
            {
                var ItemResource__rnd = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<SharedCode.Aspects.Item.Templates.BaseItemResource>(__random__);
                ItemResource = ItemResource__rnd;
            }

            {
                ((ResourcesSystem.Base.IHasRandomFill)AmmoContainer)?.Fill(__count__, __random__, __withReadOnly__);
            }

            {
                var Id__rnd = System.Guid.NewGuid();
                Id = Id__rnd;
            }

            {
                ((ResourcesSystem.Base.IHasRandomFill)Stats)?.Fill(__count__, __random__, __withReadOnly__);
            }

            {
                ((ResourcesSystem.Base.IHasRandomFill)Health)?.Fill(__count__, __random__, __withReadOnly__);
            }
        }
    }
}