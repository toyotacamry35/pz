// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class AccountEntity
    {
        protected override void RandomFill(int __count__, System.Random __random__, bool __withReadOnly__)
        {
            __count__--;
            if (__count__ <= 0)
                return;
            base.RandomFill(__count__, __random__, __withReadOnly__);
            var random = new System.Random(System.Guid.NewGuid().ToString().GetHashCode());
            {
                var Experience__rnd = (int)__random__.Next(100);
                Experience = Experience__rnd;
            }

            {
                var UnconsumedExperience__rnd = (int)__random__.Next(100);
                UnconsumedExperience = UnconsumedExperience__rnd;
            }

            {
                ((ResourcesSystem.Base.IHasRandomFill)LastSessionResult)?.Fill(__count__, __random__, __withReadOnly__);
            }

            {
                _AvailableRealmQueries.Clear();
                var AvailableRealmQueriesitem0__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<SharedCode.Entities.RealmRulesQueryState>();
                SharedCode.Aspects.Sessions.RealmRulesQueryDef __key0__ = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<SharedCode.Aspects.Sessions.RealmRulesQueryDef>(__random__);
                if (__key0__ != null)
                    AvailableRealmQueries[__key0__] = AvailableRealmQueriesitem0__rnd;
                var AvailableRealmQueriesitem1__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<SharedCode.Entities.RealmRulesQueryState>();
                SharedCode.Aspects.Sessions.RealmRulesQueryDef __key1__ = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<SharedCode.Aspects.Sessions.RealmRulesQueryDef>(__random__);
                if (__key1__ != null)
                    AvailableRealmQueries[__key1__] = AvailableRealmQueriesitem1__rnd;
                var AvailableRealmQueriesitem2__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<SharedCode.Entities.RealmRulesQueryState>();
                SharedCode.Aspects.Sessions.RealmRulesQueryDef __key2__ = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<SharedCode.Aspects.Sessions.RealmRulesQueryDef>(__random__);
                if (__key2__ != null)
                    AvailableRealmQueries[__key2__] = AvailableRealmQueriesitem2__rnd;
                var AvailableRealmQueriesitem3__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<SharedCode.Entities.RealmRulesQueryState>();
                SharedCode.Aspects.Sessions.RealmRulesQueryDef __key3__ = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<SharedCode.Aspects.Sessions.RealmRulesQueryDef>(__random__);
                if (__key3__ != null)
                    AvailableRealmQueries[__key3__] = AvailableRealmQueriesitem3__rnd;
                var AvailableRealmQueriesitem4__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<SharedCode.Entities.RealmRulesQueryState>();
                SharedCode.Aspects.Sessions.RealmRulesQueryDef __key4__ = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<SharedCode.Aspects.Sessions.RealmRulesQueryDef>(__random__);
                if (__key4__ != null)
                    AvailableRealmQueries[__key4__] = AvailableRealmQueriesitem4__rnd;
                var AvailableRealmQueriesitem5__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<SharedCode.Entities.RealmRulesQueryState>();
                SharedCode.Aspects.Sessions.RealmRulesQueryDef __key5__ = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<SharedCode.Aspects.Sessions.RealmRulesQueryDef>(__random__);
                if (__key5__ != null)
                    AvailableRealmQueries[__key5__] = AvailableRealmQueriesitem5__rnd;
                var AvailableRealmQueriesitem6__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<SharedCode.Entities.RealmRulesQueryState>();
                SharedCode.Aspects.Sessions.RealmRulesQueryDef __key6__ = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<SharedCode.Aspects.Sessions.RealmRulesQueryDef>(__random__);
                if (__key6__ != null)
                    AvailableRealmQueries[__key6__] = AvailableRealmQueriesitem6__rnd;
                var AvailableRealmQueriesitem7__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<SharedCode.Entities.RealmRulesQueryState>();
                SharedCode.Aspects.Sessions.RealmRulesQueryDef __key7__ = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<SharedCode.Aspects.Sessions.RealmRulesQueryDef>(__random__);
                if (__key7__ != null)
                    AvailableRealmQueries[__key7__] = AvailableRealmQueriesitem7__rnd;
                var AvailableRealmQueriesitem8__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<SharedCode.Entities.RealmRulesQueryState>();
                SharedCode.Aspects.Sessions.RealmRulesQueryDef __key8__ = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<SharedCode.Aspects.Sessions.RealmRulesQueryDef>(__random__);
                if (__key8__ != null)
                    AvailableRealmQueries[__key8__] = AvailableRealmQueriesitem8__rnd;
            }

            {
                _Characters.Clear();
                var Charactersitem0__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<GeneratedCode.DeltaObjects.AccountCharacter>();
                ((ResourcesSystem.Base.IHasRandomFill)Charactersitem0__rnd)?.Fill(__count__, __random__, __withReadOnly__);
                Characters.Add(Charactersitem0__rnd);
                var Charactersitem1__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<GeneratedCode.DeltaObjects.AccountCharacter>();
                ((ResourcesSystem.Base.IHasRandomFill)Charactersitem1__rnd)?.Fill(__count__, __random__, __withReadOnly__);
                Characters.Add(Charactersitem1__rnd);
            }

            {
                ((ResourcesSystem.Base.IHasRandomFill)CharRealmData)?.Fill(__count__, __random__, __withReadOnly__);
            }

            {
                var AccountId__rnd = "randomAccountId__rnd" + __random__.Next(1000);
                AccountId = AccountId__rnd;
            }

            {
                var Gender__rnd = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<ResourceSystem.Aspects.Misc.GenderDef>(__random__);
                Gender = Gender__rnd;
            }
        }
    }
}