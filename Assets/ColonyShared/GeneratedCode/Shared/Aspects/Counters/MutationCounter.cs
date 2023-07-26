using Assets.ColonyShared.SharedCode.Entities.Service;
using Assets.Src.Aspects.Impl.Factions.Template;
using ColonyShared.SharedCode.Aspects.Counters.Template;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratorAnnotations;
using NLog;
using SharedCode.Entities.Engine;
using SharedCode.EntitySystem;
using System;
using System.Threading.Tasks;
using SharedCode.Entities.GameObjectEntities;

namespace GeneratedCode.DeltaObjects
{

    [GenerateDeltaObjectCode]
    public interface IMutationCounter : IQuestCounter
    {
        [ReplicationLevel(ReplicationLevel.Master)]
        MutationStageDef Stage { get; set; }
        [ReplicationLevel(ReplicationLevel.Master)]
        FactionDef Faction { get; set; }

        [ReplicationLevel(ReplicationLevel.Master)]
        Task WorldCharacter_ResurrectEvent();
        [ReplicationLevel(ReplicationLevel.Master)]
        Task Faction_Changed(FactionDef newFaction);
        [ReplicationLevel(ReplicationLevel.Master)]
        Task MutationStage_Changed(MutationStageDef newStage);
    }


    public partial class MutationCounter 
    {
        private MutationCounterDef Def => (MutationCounterDef)CounterDef;
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public Task OnInitImpl(QuestDef questDef, QuestCounterDef counterDef, IEntitiesRepository repository)
        {
            QuestDef = questDef;
            CounterDef = counterDef;
            return Task.CompletedTask;
        }

        public async Task OnDatabaseLoadImpl(IEntitiesRepository repository)
        {
            await Subscribe(Def, repository);
        }

        public async Task OnDestroyImpl(IEntitiesRepository repository)
        {
            await Subscribe(Def, repository, false);
        }

        private async Task Subscribe(MutationCounterDef counterDef, IEntitiesRepository repository, bool subscribe = true)
        {
            using (var wrapper = await repository.Get(parentEntity.TypeId, parentEntity.Id))
            {

                if (wrapper.TryGet<IHasFactionServer>(parentEntity.TypeId, parentEntity.Id, ReplicationLevel.Server, out var hasFaction))
                {
                    if (Def.Faction != null)
                    {
                        if (subscribe)
                        {
                            hasFaction.SubscribePropertyChanged(nameof(hasFaction.Faction), Faction_ChangedWrap);
                            //Logger.IfInfo()?.Message("MutationCounter.Faction Subscribed").Write();
                            await Faction_Changed(hasFaction.Faction);
                        }
                        else
                        {
                            hasFaction.UnsubscribePropertyChanged(nameof(hasFaction.Faction), Faction_ChangedWrap);
                            //Logger.IfInfo()?.Message("MutationCounter.Faction Unsubscribed").Write();
                        }
                    }
                }

                if (wrapper.TryGet<IHasMutationMechanicsServer>(parentEntity.TypeId, parentEntity.Id, ReplicationLevel.Server, out var hasMutationMechanics))
                {
                    var mutationMechanics = hasMutationMechanics.MutationMechanics;
                    if (Def.Stage != null)
                    {
                        if (subscribe)
                        {
                            mutationMechanics.SubscribePropertyChanged(nameof(mutationMechanics.Stage), MutationStage_ChangedWrap);
                            //Logger.IfInfo()?.Message("MutationCounter.Stage Subscribed").Write();
                            await MutationStage_Changed(mutationMechanics.Stage);
                        }
                        else
                        {
                            mutationMechanics.UnsubscribePropertyChanged(nameof(mutationMechanics.Stage), MutationStage_ChangedWrap);
                            //Logger.IfInfo()?.Message("MutationCounter.Stage Unsubscribed").Write();
                        }
                    }
                }

                if (wrapper.TryGet<IHasMortalServer>(parentEntity.TypeId, parentEntity.Id, ReplicationLevel.Server, out var hasMortal))
                {
                    if (Def.Faction != null || Def.Stage != null)
                    {
                        if (subscribe)
                        {
                            hasMortal.Mortal.ResurrectEvent += WorldCharacter_ResurrectEventWrap;
                            using (var entityWrapper = await EntitiesRepository.Get(parentEntity.TypeId, parentEntity.Id))
                            {
                                await WorldCharacter_ResurrectEvent();
                            }
                        }
                        else
                        {
                            hasMortal.Mortal.ResurrectEvent -= WorldCharacter_ResurrectEventWrap;
                        }
                    }
                }
            }
        }

        private async Task WorldCharacter_ResurrectEventWrap(Guid id, int typeId, PositionRotation arg3)
        {
            if (parentEntity == null)
                return;

            using (var wrapper = await EntitiesRepository.Get(parentEntity.TypeId, parentEntity.Id))
            {
                await WorldCharacter_ResurrectEvent();
            }
        }
        public async Task WorldCharacter_ResurrectEventImpl()
        {
            if ((Def.Faction != null && Faction == Def.Faction.Target) || (Def.Stage != null && Stage == Def.Stage))
            {
                //Logger.IfInfo()?.Message($"{this.ToString()}. await OnCounterCompletedEvent();").Write();
                Count = CountForClient = 1;
                await OnOnCounterChangedInvoke(this);
                await OnOnCounterCompletedInvoke(QuestDef, this);
                return;
            }
        }

        private async Task Faction_ChangedWrap(EntityEventArgs args)
        {
            if (parentEntity == null)
                return;

            using (var wrapper = await EntitiesRepository.Get(parentEntity.TypeId, parentEntity.Id))
            {
                await Faction_Changed((FactionDef)args.NewValue);
            }
        }
        public Task Faction_ChangedImpl(FactionDef newFaction)
        {
            //Logger.IfInfo()?.Message($"newFaction({newFaction.____GetDebugShortName()}) == Def.Faction({Def.Faction.Target.____GetDebugShortName()})").Write();
            Faction = newFaction;
            return Task.CompletedTask;
        }

        private async Task MutationStage_ChangedWrap(EntityEventArgs args)
        {
            if (parentEntity == null)
                return;

            using (var wrapper = await EntitiesRepository.Get(parentEntity.TypeId, parentEntity.Id))
            {
                var newStage = (MutationStageDef)args.NewValue;
                await MutationStage_Changed(newStage);
            }
        }

        public Task MutationStage_ChangedImpl(MutationStageDef newStage)
        {
            //Logger.IfInfo()?.Message($"newStage({newStage.____GetDebugShortName()}) == Def.Stage({Def.Stage.Target.____GetDebugShortName()})").Write();
            Stage = newStage;
            return Task.CompletedTask;
        }

        public Task PreventOnCompleteEventImpl()
        {
            return Task.CompletedTask;
        }

        public override string ToString()
        {
            return "MutationCounter: Def = " + (Def?.____GetDebugShortName() ?? "null") + ", Faction = " + (Def.Faction.Target?.____GetDebugShortName() ?? "null") + ", Stage = " + (Def.Stage.Target?.____GetDebugShortName() ?? "null");
        }
    }
}
