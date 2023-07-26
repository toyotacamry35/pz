using System.Threading.Tasks;
using SharedCode.Entities;
using System;
using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using SharedCode.Aspects.Item.Templates;
using SharedCode.EntitySystem;
using ResourceSystem.Utils;

namespace GeneratedCode.DeltaObjects
{
    public partial class WorldBench : IHookOnInit, IHookOnDatabaseLoad
    {
        public Task OnInit()
        {
            Health.ZeroHealthEvent += OnZeroHealthEvent;
            return Task.CompletedTask;
        }

        public Task OnDatabaseLoad()
        {
            Health.ZeroHealthEvent += OnZeroHealthEvent;
            return Task.CompletedTask;
        }

        private async Task OnZeroHealthEvent(Guid arg1, int arg2)
        {
            if (await Health.GetMaxHealthAbsolute() <= 0)
                return;

            await EntitiesRepository.Destroy(TypeId, Id);
        }

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

        public ItemSpecificStats SpecificStats => ((WorldBenchDef)Def).DefaultStats;
    }
}