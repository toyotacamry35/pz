// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class BakenCharacterEntity
    {
        protected override void RandomFill(int __count__, System.Random __random__, bool __withReadOnly__)
        {
            __count__--;
            if (__count__ <= 0)
                return;
            base.RandomFill(__count__, __random__, __withReadOnly__);
            var random = new System.Random(System.Guid.NewGuid().ToString().GetHashCode());
            {
                var CharacterId__rnd = System.Guid.NewGuid();
                CharacterId = CharacterId__rnd;
            }

            {
                var CharacterLoaded__rnd = __random__.Next() % 2 == 1;
                CharacterLoaded = CharacterLoaded__rnd;
            }

            {
                var Logined__rnd = __random__.Next() % 2 == 1;
                Logined = Logined__rnd;
            }

            {
                _RegisteredBakens.Clear();
                var RegisteredBakensitem0__rnd = __random__.Next() % 2 == 1;
                SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> __key0__ = (SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>)System.Activator.CreateInstance(typeof(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>));
                RegisteredBakens[__key0__] = RegisteredBakensitem0__rnd;
                var RegisteredBakensitem1__rnd = __random__.Next() % 2 == 1;
                SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> __key1__ = (SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>)System.Activator.CreateInstance(typeof(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>));
                RegisteredBakens[__key1__] = RegisteredBakensitem1__rnd;
                var RegisteredBakensitem2__rnd = __random__.Next() % 2 == 1;
                SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> __key2__ = (SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>)System.Activator.CreateInstance(typeof(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>));
                RegisteredBakens[__key2__] = RegisteredBakensitem2__rnd;
            }

            {
                var ActiveBaken__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>>();
                ActiveBaken = ActiveBaken__rnd;
            }
        }
    }
}