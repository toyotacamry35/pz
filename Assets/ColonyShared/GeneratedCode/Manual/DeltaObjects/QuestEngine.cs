using Assets.Src.Aspects.Impl.Factions.Template;
using Assets.Src.ResourcesSystem.Base;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedCode.Repositories;
using SharedCode.Entities.Engine;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using SharedCode.Utils;
using SharedCode.Wizardry;
using System;
using System.Linq;
using System.Threading.Tasks;
using ColonyShared.SharedCode.Utils;
using System.Diagnostics;
using SharedCode.Entities;
using SharedCode.Repositories;
using System.Collections.Generic;
using Assets.ColonyShared.GeneratedCode.Manual.QuestStaff;
using Core.Environment.Logging.Extension;


namespace GeneratedCode.DeltaObjects
{
    public partial class QuestEngine
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static HashSet<QuestDef> questBlackList = new HashSet<QuestDef>();
        public async Task OnDatabaseLoadImpl()
        {
            foreach (var quest in Quests.ToList())
            {
                if (quest.Value.HavePhaseSuccCounter)
                {
                    quest.Value.PhaseSuccCounter.OnCounterCompleted += CounterInstance_OnSuccCounterCompleted;
                    await quest.Value.PhaseSuccCounter.OnDatabaseLoad(EntitiesRepository);
                }
                if (quest.Value.HavePhaseFailCounter)
                {
                    quest.Value.PhaseFailCounter.OnCounterCompleted += CounterInstance_OnFailCounterCompleted;
                    await quest.Value.PhaseFailCounter.OnDatabaseLoad(EntitiesRepository);
                }
            }
        }

        public async Task AddQuestImpl(QuestDef questDef)
        {
            if (!Quests.ContainsKey(questDef) && !questBlackList.Contains(questDef))
            {
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Quest '{questDef}' is activated").Write();
                QuestWatchdogSystem.Register(questDef, OnFastComplete: OnFastComplete);
                await AddQuestObject(questDef);
                await RunImpacts(questDef.Phases[0].Target.OnStart);
            }
            else if(questBlackList.Contains(questDef))
                Logger.IfError()?.Message($"Quest '{questDef}' can't be added because it contains in blacklist").Write();
           

        }

        public async Task AddQuestObjectImpl(QuestDef questDef)
        {
            var questObject = new QuestObject() { PhaseIndex = 0, QuestDef = questDef };
            questObject.IsVisible = questDef.IsVisibleDyDefault;
            Quests.Add(questDef, questObject);
            await ChangePhase(questDef, 0);
        }

        public async Task ChangePhaseImpl(QuestDef questDef, int phaseIndex)
        {
            var succCounterDef = (questDef.Phases.Length > phaseIndex && phaseIndex >= 0) ? questDef.Phases[phaseIndex].Target.Counter.Target : null;
            var failCounterDef = (questDef.Phases.Length > phaseIndex && phaseIndex >= 0) ? questDef.Phases[phaseIndex].Target.FailCounter.Target : null;

            var questObject = Quests[questDef];

            IQuestCounter succCounterInstance = null;
            if (succCounterDef != null)
            {
                var succCounterType = DefToType.GetType(succCounterDef.GetType());
                succCounterInstance = (IQuestCounter)Activator.CreateInstance(succCounterType);
            }

            IQuestCounter failCounterInstance = null;
            if (failCounterDef != null)
            {
                var failCounterType = DefToType.GetType(failCounterDef.GetType());
                failCounterInstance = (IQuestCounter)Activator.CreateInstance(failCounterType);
            }

            if (succCounterInstance != null)
            {
                await succCounterInstance.OnInit(questDef, succCounterDef, EntitiesRepository);

                if (failCounterInstance != null)
                {
                    //Logger.IfWarn()?.Message("ChangePhaseImpl -> failCounterInstance.OnCounterCompleted +").Write();

                    await failCounterInstance.OnInit(questDef, failCounterDef, EntitiesRepository);
                }

                await questObject.AddPhaseCounters(succCounterInstance, failCounterInstance);
                questObject.PhaseIndex = phaseIndex;
                succCounterInstance.OnCounterCompleted += CounterInstance_OnSuccCounterCompleted;
                await succCounterInstance.OnDatabaseLoad(EntitiesRepository);

                if (failCounterInstance != null)
                {
                    failCounterInstance.OnCounterCompleted += CounterInstance_OnFailCounterCompleted;
                    await failCounterInstance.OnDatabaseLoad(EntitiesRepository);
                }

                // Logger.IfWarn()?.Message("ChangePhaseImpl -> succCounterInstance.OnCounterCompleted +").Write();



                if(Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Quest ({questDef.____GetDebugShortName()}) next Phase({phaseIndex}) ({succCounterInstance.ToString()}|{failCounterInstance?.ToString() ?? "null"}) was activated").Write();
            }
            else
            {
                await questObject.AddPhaseCounters(new QuestCounter(), null);
                questObject.PhaseIndex = phaseIndex;
            }
        }

        public async Task RemoveQuestImpl(QuestDef quest)
        {
            if (Quests.ContainsKey(quest))
            {
                await Quests[quest].AddPhaseCounters(null, null);
                Quests.Remove(quest);
                QuestWatchdogSystem.QuestFinished(quest);
            }
        }
        public async Task<bool> RemoveAllQuestsImpl()
        {
            var quests = Quests.Keys.ToList();
            foreach (var q in quests)
                await RemoveQuestImpl(q);

            return !Quests.Any();
        }

        public Task SetVisibleImpl(QuestDef quest, bool visible)
        {
            IQuestObject questObject;
            if (Quests.TryGetValue(quest, out questObject))
                questObject.IsVisible = visible;
            return Task.CompletedTask;
        }

        private async Task CounterInstance_OnSuccCounterCompleted(QuestDef questDef, IQuestCounter counter)
        {
            await CounterInstance_OnCounterCompleted(questDef, counter, true);
        }

        private async Task CounterInstance_OnFailCounterCompleted(QuestDef questDef, IQuestCounter counter)
        {
            await CounterInstance_OnCounterCompleted(questDef, counter, false);
        }

        private async Task CounterInstance_OnCounterCompleted(QuestDef questDef, IQuestCounter counter, bool success)
        {
            using (var wrapper = await EntitiesRepository.Get(parentEntity.TypeId, parentEntity.Id))
            {
                var questEntity = wrapper.Get<IHasQuestEngine>(parentEntity.TypeId, parentEntity.Id, ReplicationLevel.Master);
                if (questEntity == null)
                    return;

                IQuestObject questObject;
                if (questEntity.Quest.Quests.TryGetValue(questDef, out questObject))
                {
                    if (questObject.HavePhaseSuccCounter)
                    {
                        await questObject.PhaseSuccCounter.OnDestroy(EntitiesRepository);
                        questObject.PhaseSuccCounter.OnCounterCompleted -= CounterInstance_OnSuccCounterCompleted;
                        //Logger.IfWarn()?.Message("PhaseSuccCounter.OnCounterCompleted -> PhaseSuccCounter.OnCounterCompleted -").Write();
                    }
                    if (questObject.HavePhaseFailCounter)
                    {
                        await questObject.PhaseFailCounter.OnDestroy(EntitiesRepository);
                        questObject.PhaseFailCounter.OnCounterCompleted -= CounterInstance_OnFailCounterCompleted;
                        //Logger.IfWarn()?.Message("CounterInstance_OnCounterCompleted -> PhaseFailCounter.OnCounterCompleted -").Write();
                    }

                    var currentPhaseIndex = questObject.PhaseIndex;
                    if(Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"CounterComplited: {questDef.____GetDebugShortName()}, counter = {counter.ToString()}, currentPhaseIndex = {currentPhaseIndex}").Write();
                    if (currentPhaseIndex >= questDef.Phases.Length || currentPhaseIndex < 0)
                    {
                        Logger.IfError()?.Message("QuestEngine currentPhase {0} indexoutofrange phases count {1}. QuestDef {2}. Remove broken quest", currentPhaseIndex, questDef.Phases.Length, questDef).Write();
                        await questEntity.Quest.RemoveQuest(questDef);
                    }

                    var currentPhase = questDef.Phases[currentPhaseIndex].Target;

                    if (success)
                        await RunImpacts(currentPhase.OnSuccess);
                    else
                        await RunImpacts(currentPhase.OnFail);
                    await RunImpacts(currentPhase.OnEnd);

                    if (!currentPhase.IsFinalPhase)
                    {
                        var nextPhase = currentPhaseIndex + 1;
                        var nextPhaseLabel = success ? currentPhase.OnSuccessPhase : currentPhase.OnFailPhase;
                        if (!string.IsNullOrWhiteSpace(nextPhaseLabel))
                            for (int i = 0; i < questDef.Phases.Length; i++)
                                if (questDef.Phases[i].Target.Label == nextPhaseLabel)
                                {
                                    nextPhase = i;
                                    break;
                                }

                        if(Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"New Phase: {nextPhase} / {questDef.Phases.Count()}").Write();

                        if (nextPhase < questDef.Phases.Length)
                        {
                            await questEntity.Quest.ChangePhase(questDef, nextPhase);
                            await RunImpacts(questDef.Phases[nextPhase].Target.OnStart);
                            return; //Next phase activated
                        }
                    }

                    if(Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Quest '{questDef.____GetDebugShortName()}' Finished").Write();

                    if (success)
                    {
                        await questObject.SetStatus(QuestStatus.Sucess);
                        await RunImpacts(questDef.OnSuccess);
                    }
                    else
                        await RunImpacts(questDef.OnFail);
                    await RunImpacts(questDef.OnEnd);

                    switch (questDef.AfterComplete)
                    {
                        case QuestAfterCompleteAction.Keep:
                            await questEntity.Quest.ChangePhase(questDef, -1);
                            break;
                        case QuestAfterCompleteAction.Loop:
                            await questEntity.Quest.RemoveQuest(questDef);
                            await questEntity.Quest.AddQuest(questDef);
                            break;
                        case QuestAfterCompleteAction.Remove:
                            await questEntity.Quest.RemoveQuest(questDef);
                            break;
                    }
                }
                else
                {
                    Logger.IfError()?.Message($"There is no Active Quest ({questDef.____GetDebugShortName()}) for Counter ({counter.GetType().Name})").Write();
                }
            }
        }

        private async Task RunImpacts(ResourceRef<SpellImpactDef>[] impacts)
        {
            if (impacts == null || impacts.Length == 0)
                return;

            var mockCastData = new SpellWordCastData(
                wizard: new OuterRef<IWizardEntity>(parentEntity.Id, ReplicaTypeRegistry.GetIdByType(typeof(IWizardEntity))),
                castData: new SpellCast(),
                caster: new OuterRef<IEntity>(parentEntity.Id, parentEntity.TypeId),
                spellId: SpellId.FirstMasterValid,
                subSpellCount: 0,
                currentTime: 0,
                wordTimeRange: default(TimeRange),
                spellStartTime: 0,
                parentSubSpellStartTime: 0,
                slaveMark: null,
                firstOrLast: false,
                canceled: false,
                context: null,
                modifiers: null,
                repo:EntitiesRepository
            );

            foreach (var impact in impacts)
            {
                if (impact == null)
                    continue;

                await SpellImpacts.CastImpact(mockCastData, impact, EntitiesRepository);
            }
        }

        private static void OnFastComplete(QuestDiagnosticInfo questInfo)
        {

            if (questInfo.Quest.AfterComplete == QuestAfterCompleteAction.Loop)
            {
                questBlackList.Add(questInfo.Quest);
                Logger.IfError()?.Message($"ADDED TO BLACKLIST:{questInfo}\n").Write();
                if (Logger.IsDebugEnabled) QuestWatchdogSystem.Dump();
            }
        }
        private void QuestDump()
        {
            Logger.IfError()?.Message($"Quests.Keys:\n{string.Join("\n", Quests.Keys.Select(x => x.ToString()))}").Write();
        }
    }
}
