using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Assets.Src.Aspects.Impl.Factions.Template;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using NLog;
using ReactivePropsNs;
using ReactivePropsNs.Touchables;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using Uins;

namespace Assets.Src.ContainerApis
{
    public class QuestEngineFullApi : EntityApi
    {
        public delegate void QuestStateDelegate(QuestState questState, bool isFirstTime);

        private event QuestStateDelegate QuestStateChanged;

        private event Action<QuestDef> QuestRemoved;

        private ConcurrentDictionary<QuestDef, QuestListener> _questListeners = new ConcurrentDictionary<QuestDef, QuestListener>();

        private ITouchable<IQuestEngineClientFull> _questEngine;

        /// <summary>
        /// Источник данных, в котором рано или поздно появится место, из которого нужно извлекать ключ
        /// </summary>
        private ReactiveProperty<IDictionaryStream<QuestDef, QuestStateReactive>> _questsState =
            new ReactiveProperty<IDictionaryStream<QuestDef, QuestStateReactive>>();

        private QuestGroup[] _ignoredGroups = new[] {QuestGroup.Hidden};


        //=== Props ===========================================================

        protected override ReplicationLevel ReplicationLevel => ReplicationLevel.ClientFull;


        //=== Public ==========================================================

        public void SetQuestEngine(ITouchable<IQuestEngineClientFull> questEngine)
        {
            // PZ-15197 // Logger.IfError()?.Message($"$$$$$$$$$$ QuestEngineFullApi.SetQuestEngine({questEngine})").Write();
            // PZ-15197 // questEngine.Log(D, "$$$$$$$$$$ QuestEngineFullApi.questEngine:");
            // PZ-15197 // questEngine.ToStream(D, qe => qe.Quests).Action(D, quests => Logger.IfError()?.Message($"$$$$$$$$$$ QuestEngineFullApi.questEngine.Quests = {quests})")).Write();


            _questEngine = questEngine;
            _questsState.Value = _questEngine.ToDictionaryStream(
                D,
                engine => engine.Quests,
                (def, quest, touchableFactory) => new QuestStateReactive(def, quest, touchableFactory()));
            // PZ-15197 // _questsState.Value.AddStream.Action(D, questPair => Logger.IfError()?.Message($"$$$$$$$$$$ IQuestEngine.Quests.AddStream => {questPair.Key} : {questPair.Value}")).Write();
        }

        public void SubscribeToQuests(QuestStateDelegate onQuestStateAddedOrChanged, Action<QuestDef> onRemoveQuest)
        {
            if (onQuestStateAddedOrChanged.AssertIfNull(nameof(onQuestStateAddedOrChanged)) ||
                onRemoveQuest.AssertIfNull(nameof(onRemoveQuest)))
                return;

            SubscribeToQuestsRequest(onQuestStateAddedOrChanged, onRemoveQuest);
        }

        public void UnsubscribeFromQuests(QuestStateDelegate onQuestStateAddedOrChanged, Action<QuestDef> onRemoveQuest)
        {
            if (onQuestStateAddedOrChanged.AssertIfNull(nameof(onQuestStateAddedOrChanged)) ||
                onRemoveQuest.AssertIfNull(nameof(onRemoveQuest)))
                return;

            QuestStateChanged -= onQuestStateAddedOrChanged;
            QuestRemoved -= onRemoveQuest;
        }


        //=== Protected =======================================================

        protected override async Task OnWrapperReceivedAtStart(IEntity wrapper)
        {
            var characterClientFull = (IWorldCharacterClientFull) wrapper;
            if (characterClientFull.AssertIfNull(nameof(characterClientFull)))
                return;

            var questEngine = characterClientFull.Quest;
            if (questEngine.AssertIfNull(nameof(questEngine)))
                return;

            questEngine.Quests.OnItemAddedOrUpdated += OnQuestAddedOrUpdated;
            questEngine.Quests.OnItemRemoved += OnQuestRemoved;

            if (questEngine.Quests.Count > 0)
            {
                foreach (var questKvp in questEngine.Quests)
                {
                    var questDef = questKvp.Key;
                    var questObjectClientFull = questKvp.Value;
                    if (questDef.AssertIfNull(nameof(questDef)) ||
                        questObjectClientFull.AssertIfNull(nameof(questObjectClientFull)) ||
                        _ignoredGroups.Contains(questDef.Group))
                        continue;

                    var listener = GetQuestListener(questDef);
                    //UI.CallerLog($"NEW {questDef}, {questObjectClientFull.PhaseIndex}"); //DEBUG
                    listener.SubscribeAndSetQuest(questObjectClientFull, false);
                }

                if (QuestStateChanged != null)
                {
                    UnityQueueHelper.RunInUnityThreadNoWait(
                        () =>
                        {
                            foreach (var listener in _questListeners.Values)
                                QuestStateChanged?.Invoke(listener.QuestState, true);
                        });
                }
            }
        }

        protected override async Task OnWrapperReceivedAtEnd(IEntity wrapper)
        {
            var characterClientFull = (IWorldCharacterClientFull) wrapper;
            if (characterClientFull.AssertIfNull(nameof(characterClientFull)))
                return;

            var questEngine = characterClientFull.Quest;
            if (questEngine.AssertIfNull(nameof(questEngine)))
                return;

            questEngine.Quests.OnItemAddedOrUpdated -= OnQuestAddedOrUpdated;
            questEngine.Quests.OnItemRemoved -= OnQuestRemoved;

            if (questEngine.Quests.Count > 0)
            {
                foreach (var questKvp in questEngine.Quests)
                {
                    var questDef = questKvp.Key;
                    var questObjectClientFull = questKvp.Value;
                    if (questDef.AssertIfNull(nameof(questDef)) ||
                        questObjectClientFull.AssertIfNull(nameof(questObjectClientFull)) ||
                        _ignoredGroups.Contains(questDef.Group))
                        continue;

                    GetQuestListener(questDef, false)?.Unsubscribe(questObjectClientFull);
                }
            }

            _questListeners.Clear();
        }


        //=== Private =========================================================

        private void SubscribeToQuestsRequest(QuestStateDelegate onQuestStateChanged, Action<QuestDef> onRemoveQuest)
        {
            QuestStateChanged += onQuestStateChanged;
            QuestRemoved += onRemoveQuest;

            if (_questListeners.Count > 0)
            {
                foreach (var listener in _questListeners.Values)
                {
                    //UI.CallerLog($"NEW (request) {listener}"); //DEBUG
                    onQuestStateChanged.Invoke(listener.QuestState, true);
                }
            }
        }

        private Task OnQuestAddedOrUpdated(DeltaDictionaryChangedEventArgs<QuestDef, IQuestObjectClientFull> questChanges)
        {
            if (questChanges.AssertIfNull(nameof(questChanges)))
                return Task.CompletedTask;

            var questDef = questChanges.Key;
            var questObjectClientFull = questChanges.Value;
            if (questDef.AssertIfNull(nameof(questDef)) ||
                questObjectClientFull.AssertIfNull(nameof(questObjectClientFull)) ||
                _ignoredGroups.Contains(questDef.Group))
                return Task.CompletedTask;

            //UI.CallerLog($"ADD {questDef}, {questObjectClientFull.PhaseIndex}"); //DEBUG
            var listener = GetQuestListener(questDef);
            listener.ResetSubscribedStatus();
            listener.SubscribeAndSetQuest(questObjectClientFull);
            return Task.CompletedTask;
        }

        private Task OnQuestRemoved(DeltaDictionaryChangedEventArgs<QuestDef, IQuestObjectClientFull> questChanges)
        {
            if (questChanges.AssertIfNull(nameof(questChanges)))
                return Task.CompletedTask;

            var questDef = questChanges.Key;
            var questObjectClientFull = questChanges.Value;
            if (questDef.AssertIfNull(nameof(questDef)) ||
                questObjectClientFull.AssertIfNull(nameof(questObjectClientFull)) ||
                _ignoredGroups.Contains(questDef.Group))
                return Task.CompletedTask;

            //UI.CallerLog($"REMOVE {questDef}, {questObjectClientFull.PhaseIndex}"); //DEBUG
            RemoveQuestListener(questDef, questObjectClientFull);
            FireOnQuestRemove(questDef);
            return Task.CompletedTask;
        }

        private void FireOnSlotItemChanged(QuestState questState)
        {
            if (QuestStateChanged != null)
                UnityQueueHelper.RunInUnityThreadNoWait(() => { QuestStateChanged?.Invoke(questState, false); });
        }

        private void FireOnQuestRemove(QuestDef questDef)
        {
            if (QuestRemoved != null)
                UnityQueueHelper.RunInUnityThreadNoWait(() => { QuestRemoved?.Invoke(questDef); });
        }

        private QuestListener GetQuestListener(QuestDef questDef, bool createIfNotExists = true)
        {
            QuestListener listener;
            lock (_questListeners)
            {
                if (!_questListeners.TryGetValue(questDef, out listener))
                {
                    if (createIfNotExists)
                    {
                        // PZ-15197 // Logger.IfError()?.Message($"$$$$$$$$$$ GetQuestListener()._questsState:{_questsState}; {_questsState.StreamState()}").Write();
                        // Ща тут делаю суперкостыль. Теоретически может быть такой экстеншен, но это какой-то адово редкий случай, так что напишу прямо напрямую.
                        var keyStream = new ReactiveProperty<QuestStateReactive>();
                        DisposableComposite subConnection = new DisposableComposite();
                        _questsState.Action(
                            D,
                            dictionaryStream =>
                            {
                                // PZ-15197 // Logger.IfError()?.Message($"$$$$$$$$$$ GetQuestListener()._questsState.OnNext:{dictionaryStream};").Write();
                                subConnection.Clear();
                                keyStream.Value = null;
                                if (dictionaryStream != null)
                                {
                                    dictionaryStream.KeyStream(subConnection, questDef)
                                        .Action(
                                            subConnection,
                                            questStateReactive =>
                                            {
                                                // PZ-15197 // Logger.IfError()?.Message($"$$$$$$$$$$ GetQuestListener().keyStream.Value = {questStateReactive}").Write();
                                                keyStream.Value = questStateReactive;
                                            });
                                }
                            });
                        listener = new QuestListener(questDef, this, keyStream);
                        _questListeners.TryAdd(questDef, listener);
                    }
                }
            }

            return listener;
        }

        private void RemoveQuestListener(QuestDef questDef, IQuestObjectClientFull questObjectClientFull)
        {
            lock (_questListeners)
            {
                if (_questListeners.TryRemove(questDef, out var listener))
                {
                    listener.Unsubscribe(questObjectClientFull);
                }
                else
                {
                    UI.Logger.Error($"Unable to remove listener by {questDef}");
                }
            }
        }

        //=== Class ===================================================================================================

        private class QuestListener
        {
            public QuestState QuestState { get; }

            private QuestEngineFullApi _parentQuestEngineFullApi;
            private bool _isSubscribed;


            //=== Ctor ========================================================

            public QuestListener(QuestDef questDef, QuestEngineFullApi parentQuestEngineFullApi, IStream<QuestStateReactive> questReactive)
            {
                _parentQuestEngineFullApi = parentQuestEngineFullApi;
                QuestState = new QuestState(questDef, questReactive);
            }


            //=== Public ======================================================

            public override string ToString()
            {
                return $"[{nameof(QuestListener)}: {QuestState}, isSubscribed{_isSubscribed.AsSign()}]";
            }

            public void SubscribeAndSetQuest(IQuestObjectClientFull questObjectClientFull, bool callFireOnSlotItemChanged = true)
            {
                QuestState.PhaseIndex = questObjectClientFull.PhaseIndex;
                if (!_isSubscribed)
                {
                    questObjectClientFull.SubscribePropertyChanged(nameof(IQuestObjectClientFull.PhaseIndex), OnPhaseIndexChanged);
                    _isSubscribed = true;
                }

                if (callFireOnSlotItemChanged)
                    _parentQuestEngineFullApi.FireOnSlotItemChanged(QuestState);
            }

            public void ResetSubscribedStatus()
            {
                _isSubscribed = false;
            }

            public void Unsubscribe(IQuestObjectClientFull questObjectClientFull)
            {
                if (!_isSubscribed)
                    return;

                questObjectClientFull.UnsubscribePropertyChanged(nameof(IQuestObjectClientFull.PhaseIndex), OnPhaseIndexChanged);
                _isSubscribed = false;
            }


            //=== Private =====================================================

            private Task OnPhaseIndexChanged(EntityEventArgs args)
            {
                var newPhaseIndex = (int) args.NewValue;
                if (QuestState.PhaseIndex == newPhaseIndex)
                    return Task.CompletedTask;

                QuestState.PhaseIndex = newPhaseIndex;
                //UI.CallerLog($"CHANGED {QuestState}"); //DEBUG
                _parentQuestEngineFullApi.FireOnSlotItemChanged(QuestState);
                return Task.CompletedTask;
            }
        }
    }
}