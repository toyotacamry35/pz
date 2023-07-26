using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Shared;
using Assets.Src.ResourcesSystem.Base;
using Assets.Tools;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Aspects.Science;
using SharedCode.Entities.Engine;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using SharedCode.Serializers;
using SharedCode.Utils;
using Uins;
using UnityEngine;

namespace Assets.Src.ContainerApis
{
    public delegate void RPointCountChangedDelegate(CurrencyResource techPointResource, int count, bool isInitial);

    public delegate void TechnologyAddRemoveDelegate(TechnologyDef technologyDef, bool isRemoved, bool isInitial);

    public delegate void KnowledgeAddRemoveDelegate(KnowledgeDef knowledgeDef, bool isRemoved, bool isInitial);

    public delegate void ShownKnowledgeRecordDelegate(KnowledgeRecordDef knowledgeRecordDef, bool isInitial);

    public class KnowledgeEngineFullApi : EntityApi
    {
        protected override ReplicationLevel ReplicationLevel => ReplicationLevel.ClientFull;

        private event TechnologyAddRemoveDelegate TechnologyAddRemove;
        private bool _isTechnologiesFirstBroadcasting;

        private event KnowledgeAddRemoveDelegate KnowledgeAddOrRemove;
        private bool _isKnowledgesFirstBroadcasting;

        private event RPointCountChangedDelegate RPointCountChanged;

        private event ShownKnowledgeRecordDelegate ShownKnowledgeRecord;
        private bool _isShownKnowledgesFirstBroadcasting;

        private bool _isSubscribed;

        private ThreadSafeList<KnowledgeRecordDef> _shownKnowledgeRecords = new ThreadSafeList<KnowledgeRecordDef>();

        private ThreadSafeList<KnowledgeDef> _knownKnowledges = new ThreadSafeList<KnowledgeDef>();

        private ThreadSafeList<TechnologyDef> _knownTechnologies = new ThreadSafeList<TechnologyDef>();

        private ConcurrentDictionary<CurrencyResource, int> _rPoints = new ConcurrentDictionary<CurrencyResource, int>();


        //=== Public ==========================================================

        public void SubscribeToRPoints(RPointCountChangedDelegate onRPointCountChanged)
        {
            if (onRPointCountChanged.AssertIfNull(nameof(onRPointCountChanged)))
                return;

            SubscribeToRPointCountsRequest(onRPointCountChanged).WrapErrors();
        }

        public void UnsubscribeFromRPoints(RPointCountChangedDelegate onRPointCountChanged)
        {
            if (onRPointCountChanged.AssertIfNull(nameof(onRPointCountChanged)))
                return;

            RPointCountChanged -= onRPointCountChanged;
        }

        public void SubscribeToKnowledge(KnowledgeAddRemoveDelegate onKnowledgeAddRemove)
        {
            if (onKnowledgeAddRemove.AssertIfNull(nameof(onKnowledgeAddRemove)))
                return;

            KnowledgeAddOrRemove += onKnowledgeAddRemove;

            if (_knownKnowledges.Count == 0 || _isKnowledgesFirstBroadcasting)
                return;

            var knowledgeDefs = _knownKnowledges.ToList();
            UnityQueueHelper.RunInUnityThreadNoWait(
                () =>
                {
                    foreach (var knowledgeDef in knowledgeDefs)
                    {
                        //UI.CallerLog($"{delegateInfo}: call for knowledgeDef={knowledgeDef}");
                        onKnowledgeAddRemove.Invoke(knowledgeDef, false, true);
                    }
                });
        }

        public void UnsubscribeFromKnowledge(KnowledgeAddRemoveDelegate onKnowledgeAddRemove)
        {
            if (onKnowledgeAddRemove.AssertIfNull(nameof(onKnowledgeAddRemove)))
                return;

            KnowledgeAddOrRemove -= onKnowledgeAddRemove;
        }

        public void SubscribeToTechnologies(TechnologyAddRemoveDelegate onTechnologyAddedOrRemoved)
        {
            if (onTechnologyAddedOrRemoved.AssertIfNull(nameof(onTechnologyAddedOrRemoved)))
                return;

            TechnologyAddRemove += onTechnologyAddedOrRemoved;

            if (_knownTechnologies.Count == 0 || _isTechnologiesFirstBroadcasting)
                return;

            var technologyDefs = _knownTechnologies.ToList();
            UnityQueueHelper.RunInUnityThreadNoWait(
                () =>
                {
                    foreach (var technologyDef in technologyDefs)
                    {
                        onTechnologyAddedOrRemoved.Invoke(technologyDef, false, true);
                    }
                });
        }

        public void UnsubscribeFromTechnologies(TechnologyAddRemoveDelegate onTechnologyAddRemove)
        {
            if (onTechnologyAddRemove.AssertIfNull(nameof(onTechnologyAddRemove)))
                return;

            TechnologyAddRemove -= onTechnologyAddRemove;
        }

        public void SubscribeToShownKnowledgeRecords(ShownKnowledgeRecordDelegate onShownKnowledgeRecord)
        {
            if (onShownKnowledgeRecord.AssertIfNull(nameof(onShownKnowledgeRecord)))
                return;

            ShownKnowledgeRecord += onShownKnowledgeRecord;

            if (_shownKnowledgeRecords.Count == 0 || _isShownKnowledgesFirstBroadcasting)
                return;

            var knowledgeRecordDefs = _shownKnowledgeRecords.ToList();
            UnityQueueHelper.RunInUnityThreadNoWait(
                () =>
                {
                    foreach (var knowledgeRecordDef in knowledgeRecordDefs)
                        onShownKnowledgeRecord.Invoke(knowledgeRecordDef, true);
                });
        }

        public void UnsubscribeFromShownKnowledgeRecords(ShownKnowledgeRecordDelegate onShownKnowledgeRecord)
        {
            if (onShownKnowledgeRecord.AssertIfNull(nameof(onShownKnowledgeRecord)))
                return;

            ShownKnowledgeRecord -= onShownKnowledgeRecord;
        }

        public bool GetIsTechnologyAvailable(TechnologyDef technologyDef)
        {
            if (technologyDef.AssertIfNull(nameof(technologyDef)))
                return false;

            return _knownTechnologies.Any(def => def == technologyDef);
        }

        public void GetCanTechnologyBeActivated(TechnologyDef technologyDef, Action<TechnologyOperationResult> onResult)
        {
            if (!IsSubscribedSuccessfully) //too early
                return;

            AsyncUtils.RunAsyncTask(() => TechnologyActivationAsync(technologyDef, false, onResult));
        }

        public void TechnologyActivation(TechnologyDef technologyDef, Action<TechnologyOperationResult> onResult)
        {
            if (technologyDef.AssertIfNull(nameof(technologyDef)))
                return;

            if (!IsSubscribedSuccessfully)
            {
                UI.Logger.IfWarn()?.Message($"Too early use {nameof(TechnologyActivation)}()").Write();
                return;
            }

            AsyncUtils.RunAsyncTask(() => TechnologyActivationAsync(technologyDef, true, onResult));
        }

        public void AddShownKnowledgeRecord(KnowledgeRecordDef knowledgeRecordDef)
        {
            if (knowledgeRecordDef.AssertIfNull(nameof(knowledgeRecordDef)))
                return;

            if (!IsSubscribedSuccessfully)
            {
                UI.Logger.IfWarn()?.Message($"Too early use {nameof(AddShownKnowledgeRecord)}()").Write();
                return;
            }

            AsyncUtils.RunAsyncTask(() => AddShownKnowledgeRecordAsync(knowledgeRecordDef));
        }


        //=== Protected =======================================================

        protected override async Task OnWrapperReceivedAtStart(IEntity wrapper)
        {
            //Метод может вызываться несколько раз, пока не получит подписки на листы известных рецептов/знаний
            if (!await TryGetKnowledgeEngineClientFull(wrapper, OnGetKnowledgeEngineClientFullAtStart))
                UI. Logger.IfError()?.Message("Unable to subscribe to KnowledgeEngine").Write();;
        }

        protected override async Task OnWrapperReceivedAtEnd(IEntity wrapper)
        {
            if (!_isSubscribed)
                return;

            lock (_rPoints)
            {
                _rPoints.Clear();
            }

            lock (_knownKnowledges)
            {
                _knownKnowledges.Clear();
            }

            lock (_knownTechnologies)
            {
                _knownTechnologies.Clear();
            }

            lock (_shownKnowledgeRecords)
            {
                _shownKnowledgeRecords.Clear();
            }

            if (!await TryGetKnowledgeEngineClientFull(wrapper, OnGetKnowledgeEngineClientFullAtEnd))
                UI. Logger.IfError()?.Message("Unable to unsubscribe from KnowledgeEngine").Write();;

            _isSubscribed = false;
        }


        //=== Private =========================================================

        private async Task TechnologyActivationAsync(
            TechnologyDef technologyDef,
            bool doActivate,
            Action<TechnologyOperationResult> onResult)
        {
            using (var worldCharacterWrapper = await NodeAccessor.Repository.Get<IWorldCharacterClientFull>(EntityGuid))
            {
                if (worldCharacterWrapper.AssertIfNull(nameof(worldCharacterWrapper)))
                {
                    UnityQueueHelper.RunInUnityThreadNoWait(() => onResult?.Invoke(TechnologyOperationResult.Error));
                    return;
                }

                var worldCharacterClientFull = worldCharacterWrapper.Get<IWorldCharacterClientFull>(EntityGuid);
                if (worldCharacterClientFull.AssertIfNull(nameof(worldCharacterClientFull)))
                {
                    UnityQueueHelper.RunInUnityThreadNoWait(() => onResult?.Invoke(TechnologyOperationResult.Error));
                    return;
                }

                using (var knowledgeEngineWrapper =
                    await NodeAccessor.Repository.Get<IKnowledgeEngineClientFull>(worldCharacterClientFull.KnowledgeEngine.Id))
                {
                    if (knowledgeEngineWrapper.AssertIfNull(nameof(knowledgeEngineWrapper)))
                    {
                        UnityQueueHelper.RunInUnityThreadNoWait(() => onResult?.Invoke(TechnologyOperationResult.Error));
                        return;
                    }

                    var knowledgeEngineClientFull =
                        knowledgeEngineWrapper.Get<IKnowledgeEngineClientFull>(worldCharacterClientFull.KnowledgeEngine.Id);
                    if (knowledgeEngineClientFull.AssertIfNull(nameof(knowledgeEngineClientFull)))
                    {
                        UnityQueueHelper.RunInUnityThreadNoWait(() => onResult?.Invoke(TechnologyOperationResult.Error));
                        return;
                    }

                    var res = await knowledgeEngineClientFull.TryToActivate(technologyDef, doActivate);
                    UnityQueueHelper.RunInUnityThreadNoWait(() => onResult?.Invoke(res));
                }
            }
        }

        private async Task AddShownKnowledgeRecordAsync(KnowledgeRecordDef knowledgeRecordDef)
        {
            if (knowledgeRecordDef.AssertIfNull(nameof(knowledgeRecordDef)))
                return;

            using (var worldCharacterWrapper = await NodeAccessor.Repository.Get<IWorldCharacterClientFull>(EntityGuid))
            {
                if (worldCharacterWrapper.AssertIfNull(nameof(worldCharacterWrapper)))
                    return;

                var worldCharacterClientFull = worldCharacterWrapper.Get<IWorldCharacterClientFull>(EntityGuid);
                if (worldCharacterClientFull.AssertIfNull(nameof(worldCharacterClientFull)))
                    return;

                using (var knowledgeEngineWrapper =
                    await NodeAccessor.Repository.Get<IKnowledgeEngineClientFull>(worldCharacterClientFull.KnowledgeEngine.Id))
                {
                    if (knowledgeEngineWrapper.AssertIfNull(nameof(knowledgeEngineWrapper)))
                        return;

                    var knowledgeEngineClientFull =
                        knowledgeEngineWrapper.Get<IKnowledgeEngineClientFull>(worldCharacterClientFull.KnowledgeEngine.Id);
                    if (knowledgeEngineClientFull.AssertIfNull(nameof(knowledgeEngineClientFull)))
                        return;

                    await knowledgeEngineClientFull.AddShownKnowledgeRecord(knowledgeRecordDef);
                }
            }
        }

        private async Task OnGetKnowledgeEngineClientFullAtStart(
            IKnowledgeEngineClientFull knowledgeEngineClientFull,
            IWorldCharacterClientFull worldCharacterClientFull)
        {
            if (knowledgeEngineClientFull.AssertIfNull(nameof(knowledgeEngineClientFull)) ||
                _isSubscribed)
                return;

            if (!worldCharacterClientFull.Currency.AssertIfNull(nameof(worldCharacterClientFull.Currency)))
            {
                worldCharacterClientFull.Currency.ItemAddedToContainer += OnRPointsItemAdded;
                worldCharacterClientFull.Currency.ItemRemovedToContainer += OnRPointsItemRemoved;
                if (worldCharacterClientFull.Currency.Items.Count > 0)
                {
                    foreach (var currency in worldCharacterClientFull.Currency.Items
                        .Select(kvp => kvp.Value.Item.ItemResource)
                        .Where(baseItemResource => baseItemResource is CurrencyResource)
                        .Cast<CurrencyResource>()
                        .Distinct())
                    {
                        var value = (int) await worldCharacterClientFull.GetCurrencyValue(currency);
                        lock (_rPoints)
                        {
                            _rPoints[currency] = value;
                        }
                    }
                }

                if (RPointCountChanged != null && _rPoints.Count > 0)
                {
                    var rPointsCopy = _rPoints.ToDictionary(x => x.Key, x => x.Value);
                    UnityQueueHelper.RunInUnityThreadNoWait(
                        () =>
                        {
                            foreach (var kvp in rPointsCopy)
                            {
                                RPointCountChanged?.Invoke(kvp.Key, kvp.Value, true);
                            }
                        });
                }
            }

            if (!knowledgeEngineClientFull.KnownKnowledges.AssertIfNull(nameof(knowledgeEngineClientFull.KnownKnowledges)))
            {
                var knownKnowledges = knowledgeEngineClientFull.KnownKnowledges;

                knownKnowledges.OnItemAdded += OnKnownKnowledgeAdded;
                knownKnowledges.OnItemRemoved += OnKnownKnowledgeRemoved;

                AddToLocalCollection(knownKnowledges, _knownKnowledges, nameof(_knownKnowledges));

                if (KnowledgeAddOrRemove != null && _knownKnowledges.Count > 0)
                {
                    _isKnowledgesFirstBroadcasting = true;
                    var knowledgeDefs = _knownKnowledges.ToList();
                    UnityQueueHelper.RunInUnityThreadNoWait(
                        () =>
                        {
                            _isKnowledgesFirstBroadcasting = false;
                            foreach (var knowledgeDef in knowledgeDefs)
                            {
                                KnowledgeAddOrRemove?.Invoke(knowledgeDef, false, true);
                            }
                        }
                    );
                }
            }

            if (!knowledgeEngineClientFull.KnownTechnologies.AssertIfNull(nameof(knowledgeEngineClientFull.KnownTechnologies)))
            {
                var knownTechnologies = knowledgeEngineClientFull.KnownTechnologies;
                knownTechnologies.OnItemAdded += OnKnownTechnologyAdded;
                knownTechnologies.OnItemRemoved += OnKnownTechnologyRemoved;

                AddToLocalCollection(knownTechnologies, _knownTechnologies, nameof(_knownTechnologies));

                if (TechnologyAddRemove != null && _knownTechnologies.Count > 0)
                {
                    _isTechnologiesFirstBroadcasting = true;
                    var technologyDefs = _knownTechnologies.ToList();
                    UnityQueueHelper.RunInUnityThreadNoWait(
                        () =>
                        {
                            _isTechnologiesFirstBroadcasting = false;
                            foreach (var technologyDef in technologyDefs)
                            {
                                TechnologyAddRemove?.Invoke(technologyDef, false, true);
                            }
                        }
                    );
                }
            }

            if (!knowledgeEngineClientFull.ShownKnowledgeRecords.AssertIfNull(nameof(knowledgeEngineClientFull.ShownKnowledgeRecords)))
            {
                var shownKnowledgeRecords = knowledgeEngineClientFull.ShownKnowledgeRecords;
                shownKnowledgeRecords.OnItemAdded += OnShownKnowledgeRecordAdded;

                AddToLocalCollection(shownKnowledgeRecords, _shownKnowledgeRecords, nameof(_shownKnowledgeRecords));

                if (ShownKnowledgeRecord != null && _shownKnowledgeRecords.Count > 0)
                {
                    _isShownKnowledgesFirstBroadcasting = true;
                    var knowledgeRecordDefs = _shownKnowledgeRecords.ToList();
                    UnityQueueHelper.RunInUnityThreadNoWait(
                        () =>
                        {
                            _isShownKnowledgesFirstBroadcasting = false;
                            foreach (var knowledgeRecordDef in knowledgeRecordDefs)
                                ShownKnowledgeRecord?.Invoke(knowledgeRecordDef, true);
                        }
                    );
                }
            }

            _isSubscribed = true;
        }

        private void AddToLocalCollection<T>(IDeltaList<T> deltaList, IList<T> localList, string localCollectionName) where T : BaseResource
        {
            if (deltaList.Count > 0)
            {
                for (int i = 0, len = deltaList.Count; i < len; i++)
                {
                    var def = deltaList[i];
                    if (localList.Contains(def))
                    {
                        UI.Logger.IfError()?.Message($"Double entrance of {def} in {localCollectionName}").Write();
                    }
                    else
                    {
                        lock (localList)
                        {
                            localList.Add(def);
                        }
                    }
                }
            }
        }

        private Task OnGetKnowledgeEngineClientFullAtEnd(
            IKnowledgeEngineClientFull knowledgeEngineClientFull,
            IWorldCharacterClientFull worldCharacterClientFull)
        {
            if (knowledgeEngineClientFull.AssertIfNull(nameof(knowledgeEngineClientFull)))
                return Task.CompletedTask;

            if (!worldCharacterClientFull.Currency.AssertIfNull(nameof(worldCharacterClientFull.Currency)))
            {
                worldCharacterClientFull.Currency.ItemAddedToContainer -= OnRPointsItemAdded;
                worldCharacterClientFull.Currency.ItemRemovedToContainer -= OnRPointsItemRemoved;
            }

            if (!knowledgeEngineClientFull.KnownKnowledges.AssertIfNull(nameof(knowledgeEngineClientFull.KnownKnowledges)))
            {
                knowledgeEngineClientFull.KnownKnowledges.OnItemAdded -= OnKnownKnowledgeAdded;
                knowledgeEngineClientFull.KnownKnowledges.OnItemRemoved -= OnKnownKnowledgeRemoved;
            }

            if (!knowledgeEngineClientFull.KnownTechnologies.AssertIfNull(nameof(knowledgeEngineClientFull.KnownTechnologies)))
            {
                knowledgeEngineClientFull.KnownTechnologies.OnItemAdded -= OnKnownTechnologyAdded;
                knowledgeEngineClientFull.KnownTechnologies.OnItemRemoved -= OnKnownTechnologyRemoved;
            }

            if (!knowledgeEngineClientFull.ShownKnowledgeRecords.AssertIfNull(nameof(knowledgeEngineClientFull.ShownKnowledgeRecords)))
            {
                knowledgeEngineClientFull.ShownKnowledgeRecords.OnItemAdded -= OnShownKnowledgeRecordAdded;
            }

            return Task.CompletedTask;
        }

        private async Task<bool> TryGetKnowledgeEngineClientFull(
            IEntity chracterWrapper,
            Func<IKnowledgeEngineClientFull, IWorldCharacterClientFull, Task> onSuccess)
        {
            if (chracterWrapper.AssertIfNull(nameof(chracterWrapper)) ||
                onSuccess.AssertIfNull(nameof(onSuccess)))
                return false;

            var worldCharacterClientFull = chracterWrapper as IWorldCharacterClientFull;
            if (worldCharacterClientFull.AssertIfNull(nameof(worldCharacterClientFull)))
                return false;

            using (var knowledgeEngineWrapper =
                await NodeAccessor.Repository.Get<IKnowledgeEngineClientFull>(worldCharacterClientFull.KnowledgeEngine.Id))
            {
                var knowledgeEngineClientFull =
                    knowledgeEngineWrapper.Get<IKnowledgeEngineClientFull>(worldCharacterClientFull.KnowledgeEngine.Id);
                if (knowledgeEngineClientFull.AssertIfNull(nameof(knowledgeEngineClientFull)))
                    return false;

                await onSuccess.Invoke(knowledgeEngineClientFull, worldCharacterClientFull);
            }

            return true;
        }

        private async Task SubscribeToRPointCountsRequest(RPointCountChangedDelegate onRPointCountChanged)
        {
            RPointCountChanged += onRPointCountChanged;
            UnityQueueHelper.RunInUnityThreadNoWait(
                () =>
                {
                    if (_rPoints.Count > 0)
                    {
                        var copy = _rPoints.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                        foreach (var kvp in copy)
                        {
                            onRPointCountChanged?.Invoke(kvp.Key, kvp.Value, true);
                        }
                    }
                });
        }

        private async Task OnRPointsItemAdded(BaseItemResource itemResource, int index, int count, bool manual)
        {
            var techPointDef = itemResource as CurrencyResource;
            if (techPointDef.AssertIfNull(nameof(techPointDef)))
                return;

            var newValue = 0;
            lock (_rPoints)
            {
                if (_rPoints.ContainsKey(techPointDef))
                    _rPoints[techPointDef] += count;
                else
                    _rPoints[techPointDef] = count;

                newValue = _rPoints[techPointDef];
            }

            if (RPointCountChanged != null)
                UnityQueueHelper.RunInUnityThreadNoWait(() => { RPointCountChanged?.Invoke(techPointDef, newValue, false); });
        }

        private async Task OnRPointsItemRemoved(BaseItemResource itemResource, int index, int count, bool manual)
        {
            var techPointDef = itemResource as CurrencyResource;
            if (techPointDef.AssertIfNull(nameof(techPointDef)))
                return;

            var newValue = 0;
            lock (_rPoints)
            {
                if (_rPoints.ContainsKey(techPointDef))
                    _rPoints[techPointDef] -= count;
                else
                    _rPoints[techPointDef] = -count; //со стороны сервера бред конечно, но если
                newValue = _rPoints[techPointDef];
            }

            if (RPointCountChanged != null)
                UnityQueueHelper.RunInUnityThreadNoWait(() => { RPointCountChanged?.Invoke(techPointDef, newValue, false); });
        }

        private async Task OnKnownTechnologyAdded(DeltaListChangedEventArgs<TechnologyDef> eventArgs)
        {
            var technologyDef = eventArgs.Value;
            if (!AddDefToLocalList(_knownTechnologies, technologyDef))
                return;

            if (TechnologyAddRemove != null)
                UnityQueueHelper.RunInUnityThreadNoWait(() => { TechnologyAddRemove?.Invoke(technologyDef, false, false); });
        }

        private async Task OnKnownTechnologyRemoved(DeltaListChangedEventArgs<TechnologyDef> eventArgs)
        {
            var technologyDef = eventArgs.Value;
            if (!RemoveDefFromLocalList(_knownTechnologies, technologyDef))
                return;

            if (TechnologyAddRemove != null)
                UnityQueueHelper.RunInUnityThreadNoWait(() => { TechnologyAddRemove?.Invoke(technologyDef, true, false); });
        }

        private async Task OnKnownKnowledgeAdded(DeltaListChangedEventArgs<KnowledgeDef> eventArgs)
        {
            var knowledgeDef = eventArgs.Value;
            if (!AddDefToLocalList(_knownKnowledges, knowledgeDef))
                return;

            if (KnowledgeAddOrRemove != null)
                UnityQueueHelper.RunInUnityThreadNoWait(() => { KnowledgeAddOrRemove?.Invoke(knowledgeDef, false, false); });
        }

        private async Task OnKnownKnowledgeRemoved(DeltaListChangedEventArgs<KnowledgeDef> eventArgs)
        {
            var knowledgeDef = eventArgs.Value;
            if (!RemoveDefFromLocalList(_knownKnowledges, knowledgeDef))
                return;

            if (KnowledgeAddOrRemove != null)
                UnityQueueHelper.RunInUnityThreadNoWait(() => { KnowledgeAddOrRemove?.Invoke(knowledgeDef, true, false); });
        }

        private async Task OnShownKnowledgeRecordAdded(DeltaListChangedEventArgs<KnowledgeRecordDef> eventArgs)
        {
            var knowledgeRecordDef = eventArgs.Value;
            if (!AddDefToLocalList(_shownKnowledgeRecords, knowledgeRecordDef))
                return;

            if (ShownKnowledgeRecord != null)
                UnityQueueHelper.RunInUnityThreadNoWait(() => ShownKnowledgeRecord?.Invoke(knowledgeRecordDef, false));
        }

        private bool AddDefToLocalList<T>(IList<T> locaList, T newDef) where T : BaseResource
        {
            if (newDef.AssertIfNull(nameof(newDef)))
                return false;

            if (locaList.Contains(newDef))
            {
                UI.Logger.IfError()?.Message($"{nameof(AddDefToLocalList)}<{typeof(T)}>() Already exists {newDef}").Write();
                return false;
            }

            lock (locaList)
            {
                locaList.Add(newDef);
            }

            return true;
        }

        private bool RemoveDefFromLocalList<T>(IList<T> locaList, T removedDef) where T : BaseResource
        {
            if (removedDef.AssertIfNull(nameof(removedDef)))
                return false;

            if (!locaList.Contains(removedDef))
            {
                UI.Logger.IfError()?.Message($"{nameof(AddDefToLocalList)}<{typeof(T)}>() Not found {removedDef}").Write();
                return false;
            }

            lock (locaList)
            {
                locaList.Remove(removedDef);
            }

            return true;
        }
    }
}