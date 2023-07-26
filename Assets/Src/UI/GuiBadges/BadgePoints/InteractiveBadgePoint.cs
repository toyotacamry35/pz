using System;
using Assets.ColonyShared.SharedCode.Player;
using Assets.ResourceSystem.Arithmetic.Templates.Predicates;
using Assets.Src.ResourceSystem;
using L10n;
using Assets.Src.Aspects;
using Assets.Src.Aspects.Impl;
using Assets.Src.Aspects.Impl.Interaction;
using Assets.Src.Aspects.VisualMarkers;
using ColonyShared.SharedCode.InputActions;
using JetBrains.Annotations;
using ReactivePropsNs;
using UnityEngine;
using Assets.Src.InteractionSystem;
using GeneratedCode.Repositories;
using SharedCode.Entities.Engine;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.Wizardry;

namespace Uins
{
    public class InteractiveBadgePoint : BadgePoint
    {
        [SerializeField, UsedImplicitly]
        private PredicateIgnoreGroupDefRef _predicateIgnoreGroupDefRef;

        private PredicateIgnoreGroupDef _predicateIgnoreGroupDef;
        private Interactive _interactive;
        private bool _waitingForSpell = false;
        private DateTime _lastCachedTime;
        private readonly TimeSpan _maxCacheTimeSpan = new TimeSpan(0, 0, 0, 0, 200);
        private ReactiveProperty<InteractionSpellDefs> _spellDefsCached = new ReactiveProperty<InteractionSpellDefs>();
        private Action<InteractionSpellDefs> _updateSpellDefCallback;


        //=== Props ===========================================================

        public ReactiveProperty<bool> IsUnknownRp { get; } = new ReactiveProperty<bool>();

        public ReactiveProperty<LocalizedString> ResourceNameRp { get; } = new ReactiveProperty<LocalizedString>();

        public ReactiveProperty<LocalizedString> InteractionNameRp { get; } = new ReactiveProperty<LocalizedString>();

        public ReactiveProperty<InputActionDef> InteractionKeyRp { get; } = new ReactiveProperty<InputActionDef>();

        public ReactiveProperty<LocalizedString> InfoNameRp { get; } = new ReactiveProperty<LocalizedString>();

        public ReactiveProperty<bool> HasInfoNameRp { get; } = new ReactiveProperty<bool>();

        public ReactiveProperty<bool> IsAllowedToShowRp { get; } = new ReactiveProperty<bool>() {Value = true};


        //=== Unity ==============================================================

        protected override void Start()
        {
            _updateSpellDefCallback = UpdateSpellDefCallback;

            base.Start();

            if (VisualMarker == null)
                return;

            VisualMarker.IsOurPlayerRp.Action(D, OnOtherPlayer);
        }


        //=== Private =========================================================

        private void OnOtherPlayer(bool isOurPlayer)
        {
            if (isOurPlayer) //не работает на нашем игроке
                return;

            if (VisualMarker == null) //тихий фэйл, в др. месте сообщили уже
                return;

            IsUnknownRp.Value = false;

            _predicateIgnoreGroupDef = _predicateIgnoreGroupDefRef.Target;
            if (_predicateIgnoreGroupDef == null)
                _predicateIgnoreGroupDef = VisualMarker.PredicateIgnoreGroupDef;

            _interactive = VisualMarker.gameObject.Kind<Interactive>();
            if (_interactive.AssertIfNull(nameof(_interactive), gameObject))
                return;

            VisualMarker.IsImportantBadgeShownRp.Subscribe(D, b => IsVisibleLogicallyRp.Value = !b, () => IsVisibleLogicallyRp.Value = true);

            var hasSpellsInfoStream = _spellDefsCached.Func(
                D,
                defs =>
                {
                    // костыль для того, чтобы маркер не показывался на живых игроках (они теперь тоже interactive)
                    if (VisualMarker?.Ego?.EntityDef is WorldCharacterDef) 
                        return (defs.InfoSpell != null || defs.InteractionSpell != null);
                    return true;
                });

            var isNeedForGuiStream = VisualMarker.IsSelectedRp
                .Zip(D, VisualMarker.IsInRangeRp)
                .Zip(D, IsAllowedToShowRp)
                .Func(D, (isSelected, isInRange, isAllowed) => isAllowed && (isSelected || isInRange));

            isNeedForGuiStream.Subscribe(D, inRange => IsNeedForGuiRp.Value = inRange, () => IsNeedForGuiRp.Value = false);

            TimeTicker.Instance.GetLocalTimer(AVisualMarker.UpdateDelay)
                .Zip(D, isNeedForGuiStream)
                .Where(D, (dt, isSelectedOrInRange) => isSelectedOrInRange)
                .Action(D, (dt, isSelectedOrInRange) => UpdateMarker());

            var isSelectedAndHasInfoStream = VisualMarker.IsSelectedRp
                .Zip(D, hasSpellsInfoStream)
                .Func(D, (isSelected, hasInfo) => isSelected && hasInfo);

            isSelectedAndHasInfoStream.Subscribe(
                D,
                b => IsSelectedRp.Value = b,
                () => IsSelectedRp.Value = false);

            if (IsDebug) //DEBUG
            {
                hasSpellsInfoStream.Log(D, $"{name}.{nameof(hasSpellsInfoStream)}");
                isNeedForGuiStream.Log(D, $"{name}.{nameof(isNeedForGuiStream)}");
                isSelectedAndHasInfoStream.Log(D, $"{name}.{nameof(isSelectedAndHasInfoStream)}");
            }
        }

        private void UpdateMarker()
        {
            var spellDoerAsync = VisualMarker?.VisualMarkerSelectorRp.Value?.SpellDoer;
            if (spellDoerAsync == null)
                return;

            if (_spellDefsCached.HasValue && DateTime.UtcNow - _lastCachedTime < _maxCacheTimeSpan)
            {
                if (IsDebug)
                    UI.CallerLog($"UpdateMarkerWithActualState() -- {transform.FullName()}"); //DEBUG
                UpdateMarkerWithActualState();
            }
            else if (!_waitingForSpell)
            {
                if (IsDebug)
                    UI.CallerLog($"Enqueue() -- {transform.FullName()}"); //DEBUG
                _waitingForSpell = true;
                _interactive.Enqueue(spellDoerAsync, _predicateIgnoreGroupDef, _updateSpellDefCallback);
            }
        }

        private void UpdateSpellDefCallback(InteractionSpellDefs interactionSpellDefs)
        {
            if (this == null)
                return;

            _waitingForSpell = false;
            _spellDefsCached.Value = interactionSpellDefs;
            _lastCachedTime = DateTime.UtcNow;
            UpdateMarkerWithActualState();
        }

        private void UpdateMarkerWithActualState()
        {
            if (VisualMarker.IsInRangeRp.Value)
            {
                bool isMineableOrHasDurability = false;
                LocalizedString nameLs = BadgesGui.Instance?.ObjectLs ?? LsExtensions.EmptyWarning;
                if (VisualMarker.Ego != null)
                {
                    var entityDef = VisualMarker.Ego.EntityDef;
                    if (!entityDef.NameLs.IsEmpty())
                        nameLs = entityDef.NameLs;
                    isMineableOrHasDurability = (IsMineable(entityDef) || HasDurability(VisualMarker.TypeId));
                }

                if (!_interactive.LocalNameLs.IsEmpty())
                {
                    nameLs = _interactive.LocalNameLs;
                }

                LocalizedString interactionNameLs, infoNameLs;
                InteractionMarkerType interactionType = InteractionMarkerType.NotAvailable;
                if (_spellDefsCached.HasValue)
                {
                    interactionType = InteractionTypeFromSpellDef(_spellDefsCached.Value.InteractionSpell, out interactionNameLs, isMineableOrHasDurability);
                    InteractionTypeFromSpellDef(_spellDefsCached.Value.InfoSpell, out infoNameLs, attackInteraction: false);
                }

                UpdateProps(nameLs, interactionType, interactionNameLs, infoNameLs);
            }
        }

        private void UpdateProps(
            LocalizedString resourceName,
            InteractionMarkerType interactionType,
            LocalizedString interactionNameLs,
            LocalizedString infoNameLs = new LocalizedString())
        {
//                UI.Logger.Info($"{name}.UpdateProps(res={resourceName}, iType={interactionType}, interactionNameLs={interactionNameLs}, " +
//                               $"infoNameLs={infoNameLs})"); //DEBUG
            ResourceNameRp.Value = resourceName;
            InteractionNameRp.Value = interactionNameLs;
            InfoNameRp.Value = infoNameLs;
            HasInfoNameRp.Value = !infoNameLs.IsEmpty();

            switch (interactionType)
            {
                case InteractionMarkerType.NotAvailable:
                    IsUnknownRp.Value = false;
                    InteractionKeyRp.Value = null;
                    return;
                case InteractionMarkerType.Attack:
                    IsUnknownRp.Value = false;
                    InteractionKeyRp.Value = InteractionNameRp.Value.IsEmpty() ? null : SpellDescription.AttackAction;
                    break;
                case InteractionMarkerType.Explore:
                    IsUnknownRp.Value = true;
                    InteractionKeyRp.Value = InteractionNameRp.Value.IsEmpty() ? null : SpellDescription.InteractionAction;
                    break;
                case InteractionMarkerType.Interact:
                    IsUnknownRp.Value = false;
                    InteractionKeyRp.Value = InteractionNameRp.Value.IsEmpty() ? null : SpellDescription.InteractionAction;
                    break;
            }
        }

        private InteractionMarkerType InteractionTypeFromSpellDef(
            SpellDef spellDef,
            out LocalizedString interactionNameLs,
            bool isMineableOrHasDurability = false,
            bool attackInteraction = true)
        {
            if (spellDef == null)
            {
                if (attackInteraction)
                {
                    if (isMineableOrHasDurability)
                    {
                        interactionNameLs = BadgesGui.Instance.AttackActionLs;
                        return InteractionMarkerType.Attack;
                    }

                    interactionNameLs = LsExtensions.Empty;
                    return InteractionMarkerType.NotAvailable;
                }

                interactionNameLs = LsExtensions.Empty;
                return InteractionMarkerType.NotAvailable;
            }

            interactionNameLs = spellDef.InteractionDescriptionLs;

            switch (spellDef.OutlineColorIndex)
            {
                case 0:
                    return InteractionMarkerType.Interact;
                case 1:
                    return InteractionMarkerType.Explore;
                default:
                    return InteractionMarkerType.NotAvailable;
            }
        }

        private bool IsMineable(IEntityObjectDef entityDef) => entityDef is MineableEntityDef;

        private bool HasDurability(int typeId)
        {
            var masterTypeId = EntitiesRepository.GetMasterTypeIdByReplicationLevelType(typeId);
            return EntitiesRepository.IsImplements<IHealthEngine>(masterTypeId);
        }

#if UNITY_EDITOR
        [UsedImplicitly]
        private void OnDrawGizmosSelected()
        {
            if (VisualMarker == null || VisualMarker.SphereRadius <= 0)
                return;

            var prevDraw = DebugExtension.Draw;
            DebugExtension.Draw = true;
            DebugExtension.DebugWireSphere(transform.position, VisualMarker.SphereRadius);
            DebugExtension.Draw = prevDraw;
        }
#endif
    }
}