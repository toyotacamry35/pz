using NLog;
using SharedCode.Entities;
using SharedCode.EntitySystem.EntityPropertyResolvers;
using System;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Calcers;
using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using ColonyShared.SharedCode.Entities;
using SharedCode.Aspects.Item.Templates;
using SharedCode.EntitySystem;
using ResourceSystem.Utils;

namespace GeneratedCode.DeltaObjects
{
    public partial class CharacterChest : IHookOnInit, IHookOnDatabaseLoad
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public async Task OnInit()
        {
            Health.ZeroHealthEvent += OnZeroHealthEvent;
            if (OwnerInformation.Owner.IsValid)
                using (var cnt = await EntitiesRepository.Get(OwnerInformation.Owner.TypeId, OwnerInformation.Owner.Guid))
                    if (cnt.TryGet<IHasFaction>(OwnerInformation.Owner.TypeId, OwnerInformation.Owner.Guid, ReplicationLevel.Master, out var hasFaction))
                    {
                        OwnerInformation.AccessPredicate = hasFaction?.Faction?.RelationshipRules.Target?.ChestAccessPredicate;
                        IncomingDamageMultiplier = hasFaction?.Faction?.RelationshipRules.Target?.ChestIncomingDamageMultiplier;
                    }
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

            var inventoryAddress = EntityPropertyResolver.GetPropertyAddress(Inventory);
            await ContainerApi.ContainerOperationSetSize(inventoryAddress, 0);
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

        public ItemSpecificStats SpecificStats => ((IHasDefaultStatsDef)Def).DefaultStats;

        public ValueTask<CalcerDef<float>> GetIncomingDamageMultiplierImpl() => new ValueTask<CalcerDef<float>>(IncomingDamageMultiplier.GetCalcer());
    }
}
