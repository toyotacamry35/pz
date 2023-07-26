using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using GeneratedCode.DeltaObjects.Chain;
using NLog;
using SharedCode.Entities;
using SharedCode.Refs;
using SharedCode.Entities.Mineable;
using SharedCode.EntitySystem;
using SharedCode.Utils;
using SharedCode.Wizardry;
using System;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Aspects.Damage;
using Assets.ColonyShared.SharedCode.Entities.Service;
using Assets.ColonyShared.SharedCode.Utils;
using Assets.Src.Aspects;
using GeneratedCode.Repositories;
using SharedCode.EntitySystem.ChainCalls;
using SharedCode.Serializers.Protobuf;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem.EntityPropertyResolvers;
using System.Linq;
using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Calcers;
using Assets.Src.Aspects.Impl.Factions.Template;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.AI;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Entities.Service;

namespace GeneratedCode.DeltaObjects
{
    public partial class LegionaryEntity : ILegionaryEntityImplementRemoteMethods, IHookOnInit, IHookOnDatabaseLoad
    {
        public async Task OnInit()
        {
            Wizard = await EntitiesRepository.Create<IWizardEntity>(Id, async w => { w.Owner = new OuterRef<IEntity>(Id, TypeId); w.IsInterestingEnoughToLog = false; });
            Mortal.DieEvent += async (Guid entityId, int typeId, PositionRotation corpsePlace) => { this.Chain().Delay(1).Destroy().Run(); }; // Call via `.Chain()` to destroy entty guaranteed after subscribers got DieEvent
            Faction = (Def as LegionaryEntityDef).Faction;
            SquadId = SpawnedObject.Spawner.Guid;
        }

        public async Task OnDatabaseLoad()
        {
            Wizard = await EntitiesRepository.Create<IWizardEntity>(Id, async w => { w.Owner = new OuterRef<IEntity>(Id, TypeId); });
        }

        public Task<bool> DestroyImpl() => Destroyable.Destroy();

        public Task<bool> StartWatchdogImpl()
        {
            //this.Chain().Delay(30 + (float)_rand.NextDouble(), true, true).UpdateWatchdogChain().Run();
            return Task.FromResult(true);
        }
        
        // --- IHitZonesOwner -----------------------------------------

        public async Task<bool> InvokeHitZonesDamageReceivedEventImpl(Damage damage)
        {
            await OnHitZonesDamageReceivedEvent(damage);
            return true;
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

        public ItemSpecificStats SpecificStats => ((LegionaryEntityDef)Def).DefaultStats;
        
        public ValueTask<CalcerDef<float>> GetIncomingDamageMultiplierImpl() => new ValueTask<CalcerDef<float>>(Faction?.RelationshipRules.Target?.IncomingDamageMultiplier.Target.GetCalcer());
    }
}
