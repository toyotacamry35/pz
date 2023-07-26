using Assets.ColonyShared.SharedCode.Entities;
using ColonyShared.SharedCode.Entities.Reactions;
using GeneratorAnnotations;
using NLog;
using SharedCode.Entities;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.ChainCalls;
using SharedCode.MovementSync;
using SharedCode.Wizardry;
using System;
using System.Threading.Tasks;
using GeneratedCode.DeltaObjects.Chain;
using Assets.ColonyShared.SharedCode.Aspects.Damage;
using ResourceSystem.Aspects.Templates;
using ColonyShared.SharedCode.Utils;
using SharedCode.Entities.Engine;
using Assets.ResourceSystem.Entities;
using SharedCode.AI;
using Assets.ColonyShared.SharedCode.Wizardry;
using Assets.Src.Aspects.Impl.Factions.Template;
using System.Linq;
using GeneratedCode.Repositories;
using SharedCode.Utils;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using Scripting;
using System.Collections.Generic;
using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Repositories;
using SharedCode.EntitySystem.EntityPropertyResolvers;
using SharedCode.MapSystem;
using ResourceSystem.Utils;

namespace GeneratedCode.DeltaObjects
{
    [DatabaseSaveType(DatabaseSaveType.Explicit)]
    [GenerateDeltaObjectCode]
    public interface IObelisk : IEntity, IHasWizardEntity, IHasHealth,
                                        IHasMortal, IHasCorpseSpawner, IHasBrute,
                                        IHitZonesOwner, IWorldObject, IHasStatsEngine,
                                        IHasSimpleMovementSync, IHasSpawnedObject, IHasDestroyable,
                                        IHasReactionsOwner, IHasAnimationDoerOwner, IHasSpatialDataHandlers,
        IIsDummyLegionary, IMountable, IHasBuffs, IDatabasedMapedEntity
    {
    }

    [DatabaseSaveType(DatabaseSaveType.Explicit)]
    [GenerateDeltaObjectCode]
    public interface IAltar : IObelisk, IHasInventory, IHasQuestEngine, IHasOpenMechanics, IHasContainerApi, IOpenable
    {
        QuestDef CurrentQuestOfObject { get; set; }
        OuterRef<IEntity> CurrentObject { get; set; }
        IEntityObjectDef CurrentObjectDef { get; set; }
        [ReplicationLevel(ReplicationLevel.Master)]
        Task<bool> OnQuestFinished(QuestDef quest);
        [ReplicationLevel(ReplicationLevel.Master)]
        Task<bool> OnShouldRestartEverything();
    }
    //trigger
    //event queries -> basically you can post an event and there are several ways of people declaratively subscribing to those events
    //databased buffs (simple are simple, while databased spells are complex)



    public partial class Altar : IHookOnInit, IHookOnDatabaseLoad, IHookOnStart
    {
        private static readonly NLog.Logger Logger = LogManager.GetLogger("Altar");

        public bool QuerySpatialData => ((IHasSpatialDataHandlersDef)Def).QuerySpatialData;

        public async Task OnInit()
        {
            Inventory.Size = 10;
            Wizard = await EntitiesRepository.Create<IWizardEntity>(Id, async w => { w.Owner = new OuterRef<IEntity>(Id, TypeId); w.IsInterestingEnoughToLog = false; });
            Mortal.DieEvent += async (Guid entityId, int typeId, PositionRotation corpsePlace) => { this.Chain().Destroy().Run(); }; // Call via `.Chain()` to destroy entty guaranteed after subscribers got DieEvent
            var selfDef = ((AltarDef)Def);
            foreach (var sacrifice in selfDef.Sacrifices)
            {
                await Quest.AddQuest(sacrifice.Quest.Target);
                if (Quest.Quests.TryGetValue(sacrifice.Quest.Target, out var qo))
                {
                    if (sacrifice.ObjectOnQuestFinish != null)
                        qo.SubscribePropertyChanged(nameof(IQuestObject.Status), OnQuestFinishedSucesfully);
                }
            }
        }

        async Task OnQuestFinishedSucesfully(EntityEventArgs finishedArgs)
        {
            if ((QuestStatus)finishedArgs.NewValue == QuestStatus.Sucess)
                await OnQuestFinished(((IQuestObject)finishedArgs.Sender).QuestDef);

        }
        public async Task<bool> OnQuestFinishedImpl(QuestDef def)
        {
            var invProp = EntityPropertyResolver.GetPropertyAddress(Inventory);
            await ContainerApi.ContainerOperationRemoveBatchItem(Inventory.Items.Select(x => new RemoveItemBatchElement(invProp, x.Key, -1, Guid.Empty)).ToList(), false);
            CurrentQuestOfObject = def;
            foreach (var quest in Quest.Quests.ToList())
                await Quest.RemoveQuest(quest.Key);
            var selfDef = ((AltarDef)Def);
            var sacrifice = selfDef.Sacrifices.Single(x => x.Quest.Target == def);
            if (sacrifice.ObjectOnQuestFinish != null)
            {
                var type = ReplicaTypeRegistry.GetIdByType(DefToType.GetEntityType(sacrifice.ObjectOnQuestFinish.Target.GetType()));
                var wsId = WorldSpaced.OwnWorldSpace.Guid;
                var ws = EntitiesRepository.TryGetLockfree<IWorldSpaceServiceEntityServer>(wsId, ReplicationLevel.Server);
                var oRef = new OuterRef<IEntity>(Guid.NewGuid(), type);
                CurrentObject = oRef;
                var result = await ws.SpawnEntity(
                    default, oRef, MovementSync.__SyncTransform.Position, MovementSync.__SyncTransform.Rotation, MapOwner, default, sacrifice.ObjectOnQuestFinish.Target, default, null, null);
                EntitiesRepository.SubscribeOnDestroyOrUnload(oRef.TypeId, oRef.Guid, OnAltarObjectDestoryed);

                if (sacrifice.BuffObjectOnStart != null)
                {
                    using (var buffedObjW = await EntitiesRepository.Get(oRef))
                    {
                        var buffedObj = buffedObjW.Get<IHasBuffsServer>(oRef, ReplicationLevel.Server);
                        await buffedObj.Buffs.TryAddBuff(new ScriptingContext() { Host = new OuterRef<IEntity>(this) }, sacrifice.BuffObjectOnStart);
                    }
                }
            }
            return true;
        }

        private async Task OnAltarObjectDestoryed(int arg1, Guid arg2, IEntity arg3)
        {
            using (var w = await this.GetThisWrite())
            {
                await OnShouldRestartEverything();
                await StartQuests();
            }
        }

        public async Task<bool> OnShouldRestartEverythingImpl()
        {
            var wsId = WorldSpaced.OwnWorldSpace.Guid;
            var ws = EntitiesRepository.TryGetLockfree<IWorldSpaceServiceEntityServer>(wsId, ReplicationLevel.Server);
            var co = CurrentObject;
            CurrentObject = default;
            CurrentQuestOfObject = null;
            if (co != default)
                await ws.DespawnEntity(co);
            return true;
        }
        public async Task OnStart()
        {
            using (var wizard = await EntitiesRepository.Get<IWizardEntity>(Id))
                foreach (var spell in ((ObeliskDef)Def).InitialSpells)
                {
                    await wizard.Get<IWizardEntity>(Id).CastSpell(new SpellCast() { Def = spell.Target, StartAt = SyncTime.Now });
                }
        }
        async Task StartQuests()
        {
            var selfDef = ((AltarDef)Def);
            foreach (var sacrifice in selfDef.Sacrifices)
            {
                if (!Quest.Quests.TryGetValue(sacrifice.Quest.Target, out var _))
                    await Quest.AddQuest(sacrifice.Quest.Target);
                if (Quest.Quests.TryGetValue(sacrifice.Quest.Target, out var qo))
                {
                    if (sacrifice.ObjectOnQuestFinish != null)
                        qo.SubscribePropertyChanged(nameof(IQuestObject.Status), OnQuestFinishedSucesfully);
                }
            }
        }
        public async Task OnDatabaseLoad()
        {
            Wizard = await EntitiesRepository.Create<IWizardEntity>(Id, async w => { w.Owner = new OuterRef<IEntity>(Id, TypeId); });
            Mortal.DieEvent += async (Guid entityId, int typeId, PositionRotation corpsePlace) => { this.Chain().Destroy().Run(); }; // Call via `.Chain()` to destroy entty guaranteed after subscribers got DieEvent
            var selfDef = ((AltarDef)Def);
            List<QuestDef> removeQuests = new List<QuestDef>();
            foreach (var quest in Quest.Quests)
                if (!selfDef.Sacrifices.Any(x => x.Quest.Target == quest.Key))
                    removeQuests.Add(quest.Key);
            foreach (var rq in removeQuests)
                Quest.Quests.Remove(rq);
            if (removeQuests.Any(x => x == CurrentQuestOfObject))
                await OnShouldRestartEverything();

            if (CurrentObject == default)
                await StartQuests();

            await Quest.OnDatabaseLoad();
            if (CurrentObject != default)
            {
                var wsId = WorldSpaced.OwnWorldSpace.Guid;
                var ws = EntitiesRepository.TryGetLockfree<IWorldSpaceServiceEntityServer>(wsId, ReplicationLevel.Server);
                var curSacr = selfDef.Sacrifices.Single(x => x.Quest == CurrentQuestOfObject);
                var result = await ws.SpawnEntity(
                    default, CurrentObject, MovementSync.__SyncTransform.Position, MovementSync.__SyncTransform.Rotation, MapOwner, default, curSacr.ObjectOnQuestFinish.Target, default, null, null);
                if (!result)
                {
                    await OnShouldRestartEverything();
                }
                else
                {
                    if (curSacr.BuffObjectOnStart != null && ReplicaTypeRegistry.GetDatabaseSaveType(CurrentObject.Type) == DatabaseSaveType.None)
                    {
                        using (var buffedObjW = await EntitiesRepository.Get(CurrentObject))
                        {
                            var buffedObj = buffedObjW.Get<IHasBuffsServer>(CurrentObject, ReplicationLevel.Server);
                            await buffedObj.Buffs.TryAddBuff(new ScriptingContext() { Host = new OuterRef<IEntity>(this) }, curSacr.BuffObjectOnStart);
                        }
                    }
                    EntitiesRepository.SubscribeOnDestroyOrUnload(CurrentObject.TypeId, CurrentObject.Guid, OnAltarObjectDestoryed);
                }

            }
        }

        public Task<bool> DestroyImpl() => Destroyable.Destroy();

        static System.Random _rand = new Random();
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
        public async Task<OuterRef> GetOpenOuterRefImpl(OuterRef oref)
        {
            return new OuterRef(ParentEntityId, ParentTypeId);
        }

        public ItemSpecificStats SpecificStats => ((AltarDef)Def).DefaultStats;
    }

    public partial class Obelisk : IHookOnInit, IHookOnDatabaseLoad, IHookOnStart
    {

        private static readonly NLog.Logger Logger = LogManager.GetLogger("Obelisk");

        public bool QuerySpatialData => ((IHasSpatialDataHandlersDef)Def).QuerySpatialData;

        public async Task OnInit()
        {
            Wizard = await EntitiesRepository.Create<IWizardEntity>(Id, async w => { w.Owner = new OuterRef<IEntity>(Id, TypeId); w.IsInterestingEnoughToLog = false; });
            Mortal.DieEvent += async (Guid entityId, int typeId, PositionRotation corpsePlace) => { this.Chain().Destroy().Run(); }; // Call via `.Chain()` to destroy entty guaranteed after subscribers got DieEvent

        }

        public async Task OnStart()
        {
            using (var wizard = await EntitiesRepository.Get<IWizardEntity>(Id))
                foreach (var spell in ((ObeliskDef)Def).InitialSpells)
                {
                    await wizard.Get<IWizardEntity>(Id).CastSpell(new SpellCast() { Def = spell.Target, StartAt = SyncTime.Now });
                }
        }

        public async Task OnDatabaseLoad()
        {
            Wizard = await EntitiesRepository.Create<IWizardEntity>(Id, async w => { w.Owner = new OuterRef<IEntity>(Id, TypeId); });
            Mortal.DieEvent += async (Guid entityId, int typeId, PositionRotation corpsePlace) => { this.Chain().Destroy().Run(); }; // Call via `.Chain()` to destroy entty guaranteed after subscribers got DieEvent


        }

        public Task<bool> DestroyImpl() => Destroyable.Destroy();

        static System.Random _rand = new Random();
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
        public async Task<OuterRef> GetOpenOuterRefImpl(OuterRef oref)
        {
            return new OuterRef(ParentEntityId, ParentTypeId);
        }

        public ItemSpecificStats SpecificStats => ((ObeliskDef)Def).DefaultStats;
    }

}
