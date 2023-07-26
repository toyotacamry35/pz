using System;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using Assets.ColonyShared.SharedCode.Entities;
using Assets.ColonyShared.SharedCode.Entities.Service;
using GeneratedCode.DeltaObjects.Chain;
using NLog;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem.ChainCalls;
using SharedCode.EntitySystem;
using ResourceSystem.Utils;

namespace GeneratedCode.DeltaObjects
{
    //#NOTE: `CorpseInteractiveEntity` manual implementation should be totally (as is) copy of this
    public partial class InteractiveEntity : IHookOnInit
    {
        // ReSharper disable once UnusedMember.Local
        private static readonly NLog.Logger Logger = LogManager.GetLogger("InteractiveEntity");

        public Task OnInit()
        {
            Mortal.DieEvent += async (Guid entityId, int typeId, PositionRotation corpsePlace) => { this.Chain().Destroy().Run(); }; // Call via `.Chain()` to destroy entty guaranteed after subscribers got DieEvent
            return Task.CompletedTask;
        }

        // -- Destroyable -----------------------------------------

        public Task<bool> DestroyImpl() => Destroyable.Destroy();

        // --- IHasLifespan -----------------------------------------

        public Task LifespanExpiredImpl() => Lifespan.LifespanExpired();

        // --- Setters: -----------------------------------------------------------------

        public Task<bool> NameSetImpl(string name)
        {
            Name = name;
            return Task.FromResult(true);
        }

        public Task<bool> PrefabSetImpl(string prefab)
        {
            Prefab = prefab;
            return Task.FromResult(true);
        }
        public async Task<OuterRef> GetOpenOuterRefImpl(OuterRef oref)
        {
            return new OuterRef(ParentEntityId, ParentTypeId);
        }

        public ItemSpecificStats SpecificStats => ((InteractiveEntityDef)Def).DefaultStats;
    }

    // Can't avoid code copy-pasting (among `InteractiveEntity` & `CorpseInteractiveEntity`)
    //#NOTE: Should be totally (as is) copy of `InteractiveEntity` manual implementation
    public partial class CorpseInteractiveEntity : IHookOnInit
    {
        // ReSharper disable once UnusedMember.Local
        private static readonly NLog.Logger Logger = LogManager.GetLogger("CorpseInteractiveEntity");

        public Task OnInit()
        {

            Mortal.DieEvent += async (Guid entityId, int typeId, PositionRotation corpsePlace) => { this.Chain().Destroy().Run(); }; // Call via `.Chain()` to destroy entty guaranteed after subscribers got DieEvent
            return Task.CompletedTask;
        }

        // -- Destroyable -----------------------------------------

        public Task<bool> DestroyImpl() => Destroyable.Destroy();
        public Task LifespanExpiredImpl() => Lifespan.LifespanExpired();

        public Task<bool> NameSetImpl(string name)
        {
            Name = name;
            return Task.FromResult(true);
        }

        public Task<bool> PrefabSetImpl(string prefab)
        {
            Prefab = prefab;
            return Task.FromResult(true);
        }
    }

}
