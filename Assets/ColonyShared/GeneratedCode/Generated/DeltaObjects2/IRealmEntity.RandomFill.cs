// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class RealmsCollectionEntity
    {
        protected override void RandomFill(int __count__, System.Random __random__, bool __withReadOnly__)
        {
            __count__--;
            if (__count__ <= 0)
                return;
            base.RandomFill(__count__, __random__, __withReadOnly__);
            var random = new System.Random(System.Guid.NewGuid().ToString().GetHashCode());
            {
                _Realms.Clear();
                var Realmsitem0__rnd = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<SharedCode.Aspects.Sessions.RealmRulesDef>(__random__);
                System.Guid __key0__ = System.Guid.Parse("b1a1108a-0a68-7ad5-5b6b-ffcfe408e634");
                Realms[__key0__] = Realmsitem0__rnd;
            }
        }
    }
}

namespace GeneratedCode.DeltaObjects
{
    public partial class RealmEntity
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
                var Def__rnd = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<SharedCode.Aspects.Sessions.RealmRulesDef>(__random__);
                Def = Def__rnd;
            }

            {
                var StartTime__rnd = (long)__random__.Next(100);
                StartTime = StartTime__rnd;
            }

            {
                var Active__rnd = __random__.Next() % 2 == 1;
                Active = Active__rnd;
            }

            {
                _AttachedAccounts.Clear();
                var AttachedAccountsitem0__rnd = __random__.Next() % 2 == 1;
                System.Guid __key0__ = System.Guid.Parse("51aa1cc3-dde3-69d8-2009-76e318fbf313");
                AttachedAccounts[__key0__] = AttachedAccountsitem0__rnd;
                var AttachedAccountsitem1__rnd = __random__.Next() % 2 == 1;
                System.Guid __key1__ = System.Guid.Parse("4e0cb09d-9b6b-37a0-36fc-5189fcb84f44");
                AttachedAccounts[__key1__] = AttachedAccountsitem1__rnd;
                var AttachedAccountsitem2__rnd = __random__.Next() % 2 == 1;
                System.Guid __key2__ = System.Guid.Parse("bf448f32-85c0-692d-0321-012ad2e95a6d");
                AttachedAccounts[__key2__] = AttachedAccountsitem2__rnd;
                var AttachedAccountsitem3__rnd = __random__.Next() % 2 == 1;
                System.Guid __key3__ = System.Guid.Parse("6d68daa5-43ed-27f4-7c72-1be68b4f6c68");
                AttachedAccounts[__key3__] = AttachedAccountsitem3__rnd;
                var AttachedAccountsitem4__rnd = __random__.Next() % 2 == 1;
                System.Guid __key4__ = System.Guid.Parse("88b131b9-34b7-1853-83a3-4a499a76644c");
                AttachedAccounts[__key4__] = AttachedAccountsitem4__rnd;
                var AttachedAccountsitem5__rnd = __random__.Next() % 2 == 1;
                System.Guid __key5__ = System.Guid.Parse("21e3e3a1-e24f-ee56-9b1c-434d6946ddb3");
                AttachedAccounts[__key5__] = AttachedAccountsitem5__rnd;
                var AttachedAccountsitem6__rnd = __random__.Next() % 2 == 1;
                System.Guid __key6__ = System.Guid.Parse("a17098b4-d138-71b6-8750-e94af8ab8d06");
                AttachedAccounts[__key6__] = AttachedAccountsitem6__rnd;
                var AttachedAccountsitem7__rnd = __random__.Next() % 2 == 1;
                System.Guid __key7__ = System.Guid.Parse("a845cf0d-4b6c-d718-dac2-ed9af005ae3c");
                AttachedAccounts[__key7__] = AttachedAccountsitem7__rnd;
                var AttachedAccountsitem8__rnd = __random__.Next() % 2 == 1;
                System.Guid __key8__ = System.Guid.Parse("019497c5-c305-d014-2c71-99fff80cf6ef");
                AttachedAccounts[__key8__] = AttachedAccountsitem8__rnd;
            }

            {
                _Maps.Clear();
                var Mapsitem0__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<SharedCode.MapSystem.MapMeta>();
                System.Guid __key0__ = System.Guid.Parse("019a403c-4634-7f69-98c6-76e31c8182a1");
                Maps[__key0__] = Mapsitem0__rnd;
            }
        }
    }
}