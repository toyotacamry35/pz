using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Src.Aspects.Doings;
using Assets.Src.Aspects.Impl;
using Assets.Src.Inventory;
using Assets.Src.Wizardry;
using Assets.Src.InteractionSystem;
using Assets.Src.SpawnSystem;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using L10n;
using ProcessSourceNamespace;
using ReactivePropsNs;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Entities;
using UnityEngine;
using UnityWeld.Binding;
using Vector3 = UnityEngine.Vector3;
using SharedCode.Entities.Mineable;
using SharedCode.Wizardry;
using GeneratedDefsForSpells;
using Src.Aspects.Impl;
using Uins.Slots;

namespace Uins
{
    [Binding]
    public class PlayerInteractionViewModel : BindingViewModel, IProcessSourceOps
    {
        private const float OpenedProcessWithoutTargetStayingTime = 0.5f;
        private const float GatheringProcessLifeLimitRatio = 1.5f;

        public event Action<IProcessSource> NewProcessSource;
        public event JustItemsAchievedDelegate JustItemsAchieved;

        public Sprite RandomItemSprite;
        public Sprite UndefinedSpellSprite;

        [UsedImplicitly, SerializeField]
        private HotkeyListener _debugHotkeyListener;

        private MiningIndication _miningIndication;
        private GatheringIndication _gatheringIndication;

        private GameObject _ourPawnGameObject;
        private ICharacterBrain _characterBrain;

        private ICharacterItemsNotifier _characterItemsNotifier;
        private Dictionary<BaseItemResource, uint> _inventoryResourcesCounts;

        private ulong _indexCounter = 0;

        private Dictionary<ISpellDoerCastPipeline, ulong> _pipelinesToIndex = new Dictionary<ISpellDoerCastPipeline, ulong>();
        private DisposableComposite _pawnLifetimeD = new DisposableComposite();


        //=== Props ==============================================================

        public Dictionary<ProcessSourceId, ProcessSource> ProcessSources { get; private set; }
            = new Dictionary<ProcessSourceId, ProcessSource>();

        public SpellDescriptions SpellDescriptions { get; private set; }

        public SpellDef LastInteractionSpell { get; private set; }

        private GameObject _selectedTarget;

        [Binding]
        public GameObject SelectedTarget
        {
            get => _selectedTarget;
            set
            {
                if (_selectedTarget != value || (HasSelectedTarget && value == null))
                {
                    _selectedTarget = value;
                    HasSelectedTarget = _selectedTarget != null;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool HasSelectedTarget { get; private set; }


        //=== Unity ===============================================================

        private void Awake()
        {
            RandomItemSprite.AssertIfNull(nameof(RandomItemSprite));
            UndefinedSpellSprite.AssertIfNull(nameof(UndefinedSpellSprite));
            _debugHotkeyListener.AssertIfNull(nameof(_debugHotkeyListener));

            _miningIndication = new MiningIndication(this, gameObject);
            _gatheringIndication = new GatheringIndication(this, gameObject);
            SpellDescriptions = new SpellDescriptions();
            this.StartInstrumentedCoroutine(OpenedProcessesCancelling(), "OpenedProcessesCancelling");
        }


        //=== Public ==============================================================

        public void Init(IPawnSource pawnSource, ICharacterItemsNotifier characterItemsNotifier)
        {
            if (!pawnSource.AssertIfNull(nameof(pawnSource)))
                pawnSource.PawnChangesStream.Action(D, OnOurPawnChanged);

            _characterItemsNotifier = characterItemsNotifier;
            _characterItemsNotifier.CharacterItemsChanged += OnCharacterItemsChanged;
        }

        public static Vector3 GetColliderClosestPoint(Vector3 fromPoint, Collider toCollider)
        {
            MeshCollider meshCollider = toCollider as MeshCollider;
            return (!meshCollider || meshCollider.convex)
                ? toCollider.ClosestPoint(fromPoint)
                : toCollider.ClosestPointOnBounds(fromPoint);
        }

        public static float DistanceFromPointToCollider(Vector3 fromPoint, Collider toCollider)
        {
            return Vector3.Distance(fromPoint, GetColliderClosestPoint(fromPoint, toCollider));
        }

        public void AddProcessSource([NotNull] ProcessSource processSource)
        {
            ProcessSource existingProcessSource;
            if (ProcessSources.TryGetValue(processSource.Id, out existingProcessSource))
            {
                UI.Logger.Error(
                    $"{nameof(AddProcessSource)}({processSource}) {nameof(processSource)} with such id={processSource.Id} already exists: " +
                    existingProcessSource);
                return;
            }

            ProcessSources.Add(processSource.Id, processSource);

            NewProcessSource?.Invoke(processSource);
        }

        public void RemoveProcessSource([NotNull] ProcessSource processSource)
        {
            if (ProcessSources.ContainsKey(processSource.Id))
                ProcessSources.Remove(processSource.Id);
            else if (UI.Logger.IsDebugEnabled)
                UI.Logger.IfDebug()?.Message(
                    $"{nameof(RemoveProcessSource)}({processSource}) Attempt to remove not existing {nameof(processSource)}")
                    .Write();
        }

        public IList<uint> GetInventoryCounts([NotNull] IList<ItemResourcePack> achievedResources)
        {
            List<uint> inventoryCounts = new List<uint>();
            if (_inventoryResourcesCounts == null)
                _inventoryResourcesCounts = new Dictionary<BaseItemResource, uint>();
            for (int i = 0, len = achievedResources.Count; i < len; i++)
            {
                var itemResourcePack = achievedResources[i];
                if (!_inventoryResourcesCounts.ContainsKey(itemResourcePack.ItemResource))
                    _inventoryResourcesCounts[itemResourcePack.ItemResource] =
                        (uint) _characterItemsNotifier.GetItemResourceCount(itemResourcePack.ItemResource);

                _inventoryResourcesCounts[itemResourcePack.ItemResource] += itemResourcePack.Count;
                inventoryCounts.Add(_inventoryResourcesCounts[itemResourcePack.ItemResource]);
            }

            return inventoryCounts;
        }

        public void SetJustItemsAchieved(IList<ItemResourcePack> achievedItems)
        {
            JustItemsAchieved?.Invoke(achievedItems, GetInventoryCounts(achievedItems));
        }

        /// <summary>
        /// Получаем название из go: 1) ищем в EntityGameObject 2) ищем в Interactive (для не имеющих entity)
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public static LocalizedString GetInteractiveName(GameObject gameObject)
        {
            if (gameObject.AssertIfNull(nameof(gameObject)))
                return LsExtensions.EmptyWarning;

            var ego = gameObject.GetComponent<EntityGameObject>();

            if (ego != null)
            {
                return ego.EntityDef != null && !ego.EntityDef.NameLs.IsEmpty()
                    ? ego.EntityDef.NameLs
                    : new LocalizedString(gameObject.name);
            }

            var interactive = gameObject.GetComponent<Interactive>();
            return interactive != null && interactive.Def != null
                ? interactive.Def.ObjectNameLs
                : new LocalizedString(gameObject.name);
        }

//        public string GetProcessSourceDebugInfo()
//        {
//            return ProcessSources.Select(ps => $"{ps.Value.ToStringShort()}").ItemsToStringByLines();
//        }


        //=== Private =============================================================

        private void OnOurPawnChanged(EntityGameObject prevEgo, EntityGameObject newEgo)
        {
            if (prevEgo != null)
            {
                if (_characterBrain != null)
                {
                    _characterBrain.TryToInteract -= OnTryToInteract;
                    _characterBrain.InteractionFinished -= OnSpellDoerOrderFinished;
                }

                _characterBrain = null;
                _pawnLifetimeD.Clear();
            }

            if (newEgo != null)
            {
                _characterBrain = newEgo.GetComponent<ICharacterPawn>().Brain;
                if (_characterBrain != null)
                {
                    _characterBrain.TryToInteract += OnTryToInteract;
                    _characterBrain.InteractionFinished += OnSpellDoerOrderFinished;
                }

                _ourPawnGameObject = newEgo.gameObject;
                _miningIndication.SetPawn(_ourPawnGameObject);
                _gatheringIndication.SetPawn(_ourPawnGameObject);

                var targetHolder = _ourPawnGameObject.GetComponentInChildren<TargetHolder>();
                if (!targetHolder.AssertIfNull(nameof(targetHolder)))
                {
                    targetHolder.CurrentTarget.Subscribe(_pawnLifetimeD, go => { SelectedTarget = go; }, () => SelectedTarget = null);
                }
            }
        }

        private void OnTryToInteract(ISpellDoerCastPipeline spellOrderId, SpellDef spell)
        {
            UI.Logger.IfDebug()?.Message($"{nameof(spellOrderId)}='{spellOrderId}', spell='{spell}'").Write();
            //UI.CallerLog($"{nameof(spellOrderId)}='{spellOrderId}', spell='{spell}'"); //DEBUG
            if (spell == null)
                return;

            LastInteractionSpell = spell;
            var interactionDescription = SpellDescription.GetSpellDescription(spell, SelectedTarget, true);
            if (interactionDescription.DontShowProgress)
                return;

            Sprite processSprite = null;
            var expectedItems = new List<ProbabilisticItemPack>();
            if (interactionDescription.IsEmpty)
            {
                processSprite = UndefinedSpellSprite; //рассматриваем как визуализацию ошибки
                //UI.Logger.Error($"{nameof(OnTryToInteract)}({nameof(spellOrderId)}={spellOrderId}, {spell})" +
                //               " interactionDescription is empty");
            }
            else
            {
                if (interactionDescription.HasOverridenInteractionSprite)
                {
                    processSprite = interactionDescription.InteractionSprite;
                }
                else
                {
                    expectedItems = interactionDescription.ConditionVariants.SelectMany(cv => cv.Items).ToList();
                    if (interactionDescription.ConditionVariants.Any(conditionVariant => conditionVariant.IsRandomItems))
                        processSprite = RandomItemSprite;
                }
            }

            var counter = ++_indexCounter;
            _pipelinesToIndex.Add(spellOrderId, counter);
            var processSource = new ProcessSource(
                new ProcessSourceId(Guid.Empty, 0, ProcessSourceId.ProcessType.CommonInteraction, counter),
                0,
                1,
                spell.Duration,
                expectedItems,
                processSprite);

//            UI.CallerLog($"spell='{spell}' duration={spell.Duration} " + //DEBUG
//                         $"expected={expectedItems.ItemsToString()}, processSprite='{processSprite}'");
            AddProcessSource(processSource);
        }

        private void OnSpellDoerOrderFinished(ISpellDoerCastPipeline spellOrderId)
        {
            if (!_pipelinesToIndex.ContainsKey(spellOrderId))
                return;
            var counter = _pipelinesToIndex[spellOrderId];
            var processSourceId = new ProcessSourceId(Guid.Empty, 0, ProcessSourceId.ProcessType.CommonInteraction, counter);
            ProcessSource processSource;
            if (!ProcessSources.TryGetValue(processSourceId, out processSource))
                return;

            processSource.SetEnding(spellOrderId.FinishReason == FinishReasonType.Fail);
            RemoveProcessSource(processSource);
        }

        private IEnumerator OpenedProcessesCancelling()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.5f);

                if (_ourPawnGameObject == null)
                    continue;

                List<ProcessSource> toCancel = null;

                foreach (var processSource in ProcessSources.Values)
                {
                    if (processSource.IsEnded)
                        continue;

                    toCancel = toCancel ?? new List<ProcessSource>();
                    
                    var processLifeTime = Time.time - processSource.CreateTime;
                    switch (processSource.Id.Type)
                    {
                        case ProcessSourceId.ProcessType.Mining:
                            if ((SelectedTarget == null ||
                                 SelectedTarget.GetComponent<EntityGameObject>() == null ||
                                 SelectedTarget.GetComponent<EntityGameObject>().EntityId != processSource.Id.EntityId) &&
                                processLifeTime > OpenedProcessWithoutTargetStayingTime)
                                toCancel.Add(processSource);
                            break;

                        case ProcessSourceId.ProcessType.CommonInteraction:
                            var processMaxDuration = processSource.ProgressDuration > 0
                                ? processSource.ProgressDuration * GatheringProcessLifeLimitRatio
                                : GatheringProcessLifeLimitRatio;
                            if (processLifeTime > processMaxDuration)
                                toCancel.Add(processSource);
                            break;
                    }
                }

                if (toCancel != null && toCancel.Count > 0)
                    for (int i = 0; i < toCancel.Count; i++)
                    {
                        toCancel[i].SetEnding(false);
                        RemoveProcessSource(toCancel[i]);
                    }
            }
        }

        private void OnCharacterItemsChanged(SlotViewModel slotViewModel, int stackDelta)
        {
            _inventoryResourcesCounts = null;
        }
    }
}