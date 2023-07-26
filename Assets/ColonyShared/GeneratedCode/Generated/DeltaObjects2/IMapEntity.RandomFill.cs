// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class MapEntity
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
                var RealmId__rnd = System.Guid.NewGuid();
                RealmId = RealmId__rnd;
            }

            if (__withReadOnly__)
            {
                var RealmRules__rnd = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<SharedCode.Aspects.Sessions.RealmRulesDef>(__random__);
                RealmRules = RealmRules__rnd;
            }

            if (__withReadOnly__)
            {
                var Map__rnd = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<GeneratedCode.Custom.Config.MapDef>(__random__);
                Map = Map__rnd;
            }

            {
                var allValuesMapEntityState = System.Enum.GetValues(typeof(SharedCode.MapSystem.MapEntityState));
                var valueMapEntityState = (SharedCode.MapSystem.MapEntityState)allValuesMapEntityState.GetValue(__random__.Next() % allValuesMapEntityState.Length);
                var State__rnd = valueMapEntityState;
                State = State__rnd;
            }

            {
                var Dead__rnd = __random__.Next() % 2 == 1;
                Dead = Dead__rnd;
            }

            {
                _WorldSpaces.Clear();
                var WorldSpacesitem0__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<GeneratedCode.DeltaObjects.WorldSpaceDescription>();
                ((ResourcesSystem.Base.IHasRandomFill)WorldSpacesitem0__rnd)?.Fill(__count__, __random__, __withReadOnly__);
                WorldSpaces.Add(WorldSpacesitem0__rnd);
                var WorldSpacesitem1__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<GeneratedCode.DeltaObjects.WorldSpaceDescription>();
                ((ResourcesSystem.Base.IHasRandomFill)WorldSpacesitem1__rnd)?.Fill(__count__, __random__, __withReadOnly__);
                WorldSpaces.Add(WorldSpacesitem1__rnd);
                var WorldSpacesitem2__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<GeneratedCode.DeltaObjects.WorldSpaceDescription>();
                ((ResourcesSystem.Base.IHasRandomFill)WorldSpacesitem2__rnd)?.Fill(__count__, __random__, __withReadOnly__);
                WorldSpaces.Add(WorldSpacesitem2__rnd);
                var WorldSpacesitem3__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<GeneratedCode.DeltaObjects.WorldSpaceDescription>();
                ((ResourcesSystem.Base.IHasRandomFill)WorldSpacesitem3__rnd)?.Fill(__count__, __random__, __withReadOnly__);
                WorldSpaces.Add(WorldSpacesitem3__rnd);
                var WorldSpacesitem4__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<GeneratedCode.DeltaObjects.WorldSpaceDescription>();
                ((ResourcesSystem.Base.IHasRandomFill)WorldSpacesitem4__rnd)?.Fill(__count__, __random__, __withReadOnly__);
                WorldSpaces.Add(WorldSpacesitem4__rnd);
                var WorldSpacesitem5__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<GeneratedCode.DeltaObjects.WorldSpaceDescription>();
                ((ResourcesSystem.Base.IHasRandomFill)WorldSpacesitem5__rnd)?.Fill(__count__, __random__, __withReadOnly__);
                WorldSpaces.Add(WorldSpacesitem5__rnd);
                var WorldSpacesitem6__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<GeneratedCode.DeltaObjects.WorldSpaceDescription>();
                ((ResourcesSystem.Base.IHasRandomFill)WorldSpacesitem6__rnd)?.Fill(__count__, __random__, __withReadOnly__);
                WorldSpaces.Add(WorldSpacesitem6__rnd);
                var WorldSpacesitem7__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<GeneratedCode.DeltaObjects.WorldSpaceDescription>();
                ((ResourcesSystem.Base.IHasRandomFill)WorldSpacesitem7__rnd)?.Fill(__count__, __random__, __withReadOnly__);
                WorldSpaces.Add(WorldSpacesitem7__rnd);
                var WorldSpacesitem8__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<GeneratedCode.DeltaObjects.WorldSpaceDescription>();
                ((ResourcesSystem.Base.IHasRandomFill)WorldSpacesitem8__rnd)?.Fill(__count__, __random__, __withReadOnly__);
                WorldSpaces.Add(WorldSpacesitem8__rnd);
                var WorldSpacesitem9__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<GeneratedCode.DeltaObjects.WorldSpaceDescription>();
                ((ResourcesSystem.Base.IHasRandomFill)WorldSpacesitem9__rnd)?.Fill(__count__, __random__, __withReadOnly__);
                WorldSpaces.Add(WorldSpacesitem9__rnd);
            }

            {
                _SavedScenes.Clear();
                var SavedScenesitem0__rnd = __random__.Next() % 2 == 1;
                SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> __key0__ = (SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>)System.Activator.CreateInstance(typeof(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>));
                SavedScenes[__key0__] = SavedScenesitem0__rnd;
                var SavedScenesitem1__rnd = __random__.Next() % 2 == 1;
                SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> __key1__ = (SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>)System.Activator.CreateInstance(typeof(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>));
                SavedScenes[__key1__] = SavedScenesitem1__rnd;
                var SavedScenesitem2__rnd = __random__.Next() % 2 == 1;
                SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> __key2__ = (SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>)System.Activator.CreateInstance(typeof(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>));
                SavedScenes[__key2__] = SavedScenesitem2__rnd;
                var SavedScenesitem3__rnd = __random__.Next() % 2 == 1;
                SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> __key3__ = (SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>)System.Activator.CreateInstance(typeof(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>));
                SavedScenes[__key3__] = SavedScenesitem3__rnd;
                var SavedScenesitem4__rnd = __random__.Next() % 2 == 1;
                SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> __key4__ = (SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>)System.Activator.CreateInstance(typeof(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>));
                SavedScenes[__key4__] = SavedScenesitem4__rnd;
                var SavedScenesitem5__rnd = __random__.Next() % 2 == 1;
                SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> __key5__ = (SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>)System.Activator.CreateInstance(typeof(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>));
                SavedScenes[__key5__] = SavedScenesitem5__rnd;
                var SavedScenesitem6__rnd = __random__.Next() % 2 == 1;
                SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> __key6__ = (SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>)System.Activator.CreateInstance(typeof(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>));
                SavedScenes[__key6__] = SavedScenesitem6__rnd;
                var SavedScenesitem7__rnd = __random__.Next() % 2 == 1;
                SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> __key7__ = (SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>)System.Activator.CreateInstance(typeof(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>));
                SavedScenes[__key7__] = SavedScenesitem7__rnd;
            }
        }
    }
}

namespace GeneratedCode.DeltaObjects
{
    public partial class WorldSpaceDescription
    {
        protected override void RandomFill(int __count__, System.Random __random__, bool __withReadOnly__)
        {
            __count__--;
            if (__count__ <= 0)
                return;
            base.RandomFill(__count__, __random__, __withReadOnly__);
            var random = new System.Random(System.Guid.NewGuid().ToString().GetHashCode());
            {
                var ChunkDef__rnd = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<GeneratedCode.Custom.Config.MapDef>(__random__);
                ChunkDef = ChunkDef__rnd;
            }

            {
                var UnityRepositoryId__rnd = System.Guid.NewGuid();
                UnityRepositoryId = UnityRepositoryId__rnd;
            }

            {
                var WorldSpaceRepositoryId__rnd = System.Guid.NewGuid();
                WorldSpaceRepositoryId = WorldSpaceRepositoryId__rnd;
            }

            {
                var WorldSpaceGuid__rnd = System.Guid.NewGuid();
                WorldSpaceGuid = WorldSpaceGuid__rnd;
            }

            {
                var allValuesMapChunkState = System.Enum.GetValues(typeof(SharedCode.MapSystem.MapChunkState));
                var valueMapChunkState = (SharedCode.MapSystem.MapChunkState)allValuesMapChunkState.GetValue(__random__.Next() % allValuesMapChunkState.Length);
                var State__rnd = valueMapChunkState;
                State = State__rnd;
            }
        }
    }
}