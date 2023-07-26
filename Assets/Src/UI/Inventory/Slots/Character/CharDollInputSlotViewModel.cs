using System;
using System.Collections.Generic;
using Assets.Src.Inventory;
using Assets.Src.ResourcesSystem.Base;
using JetBrains.Annotations;
using NLog;
using ReactivePropsNs;
using Src.Input;
using Src.InputActions;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins.Slots
{
    [Binding]
    public class CharDollInputSlotViewModel : CharDollSlotViewModel
    {
        private static readonly NLog.Logger Logger = LogManager.GetLogger(nameof(CharDollInputSlotViewModel));
        private const float ListenerFireMaxInterval = 1;

        /// <summary>
        /// Имеет право переключать свойство IsWeaponSelected
        /// </summary>
        public bool CanWeaponSelect;

        [UsedImplicitly, SerializeField]
        private InputActionTriggerRef _selectAction;
        [UsedImplicitly, SerializeField]
        private InputActionTriggerRef _switchAction;

        private ReactivePropsNs.ThreadSafe.DisposableComposite _subscriptions = new ReactivePropsNs.ThreadSafe.DisposableComposite();

        private static float _lastListenerFireTime;


        //=== Props ===========================================================

        private string _shortcutText;

        [Binding]
        public string ShortcutText
        {
            get => _shortcutText;
            private set
            {
                if (_shortcutText != value)
                {
                    _shortcutText = value;
                    NotifyPropertyChanged();
                }
            }
        }


        //=== Public ==========================================================

        /// <summary> Список уничтожимых в жизненном цикле Open - Close </summary>
        public ReactivePropsNs.ThreadSafe.DisposableComposite DO = new ReactivePropsNs.ThreadSafe.DisposableComposite();
        public void OnOpen(ReactivePropsNs.IStream<bool> slotsIsOpened)
        {
            var isOpened = slotsIsOpened.Last(DO);
            if (_selectAction != null && _selectAction.Target != null)
            {
                InputManager.Instance.Stream(_selectAction.Target)
                    .ThreadSafeToStream(DO)
                    .Where(DO, state => state.Activated && isOpened.Value && !IsEmpty && !IsInaccessible)
                    .Where(DO, state => NotOftenThenFireMaxInterval())
                    .Action(DO, state => OnKeyDown(true));
            }
            if (_switchAction != null && _switchAction.Target != null)
            {
                InputManager.Instance.Stream(_switchAction.Target)
                    .ThreadSafeToStream(DO)
                    .Where(DO, state => state.Activated && isOpened.Value && !IsEmpty && !IsInaccessible)
                    .Where(DO, state => NotOftenThenFireMaxInterval())
                    .Action(DO, state => OnKeyDown(false));
            }
        }

        public void OnClose()
        {
            DO.Clear();
        }

        //=== Protected =======================================================

        protected override void OnItemChanged(int slotIndex, SlotItem slotItem, int stackDelta)
        {
            var oldIsEmpty = IsEmpty;
            base.OnItemChanged(slotIndex, slotItem, stackDelta);

            ShortcutText = _selectAction == null || _selectAction.Target == null ? "" : _selectAction.Target.Description; //TODOM Переместить в более удачное место

            if (!CanWeaponSelect || WeaponSelector == null)
                return;

            if (oldIsEmpty && !IsEmpty)
            {
                //Предмет появился
                WeaponSelector.SlotSelectionRequest(SlotId);
                WeaponSelector.SlotUsageRequest(SlotDef, true);
            }
            else
            {
                if (!oldIsEmpty && IsEmpty)
                {
                    WeaponSelector.SlotSelectionRequest();
                    if (IsInUse)
                        WeaponSelector.SlotUsageRequest(SlotDef, false);
                }
            }
        }

        protected override void OnUsedSlotsChanged(IList<ResourceIDFull> usedSlotsIndices)
        {
            base.OnUsedSlotsChanged(usedSlotsIndices);
            if (!CanWeaponSelect || WeaponSelector == null)
                return;

            if (IsInUse && !IsWeaponSelected)
                WeaponSelector.SlotSelectionRequest(SlotId);
        }


        //=== Private =========================================================

        private bool NotOftenThenFireMaxInterval()
        {
            var time = Time.time;
            if (time - _lastListenerFireTime < ListenerFireMaxInterval)
                return false;
            _lastListenerFireTime = Time.time; // В отличии от прошлой версии время последнего нажатия выставляется даже если в слоте ничего нажимательного нету.
            return true;
        }

        /// <summary> Обработка нажатия клавиши на быстром слоте в HUD </summary>
        /// <returns>Было ли выполнено действие с оружием</returns>
        private bool OnKeyDown(bool selectNotSwitch = true)
        {
            if (CanWeaponSelect)
            {
                //WeaponSlot
                if (selectNotSwitch)
                {
                    //Цифровая кнопка выбора слота:, запрашиваем себе IsWeaponSelected и обратный IsInUse
                    if (!IsWeaponSelected)
                        WeaponSelector?.SlotSelectionRequest(SlotId);

                    WeaponSelector?.SlotUsageRequest(SlotDef, !IsInUse);
                    return true;
                }

                //Кнопка переключения используемости оружия: и мы IsWeaponSelected: запрашиваем обратный IsInUse
                if (IsWeaponSelected)
                {
                    WeaponSelector?.SlotUsageRequest(SlotDef, !IsInUse);
                    return true;
                }
            }
            else
            {
                //HotSlot
                if (ItemIsNullOrDefault || !selectNotSwitch)
                    return false;

                ContextActionsSource.ExecuteDefaultAction(this, true);
            }

            return false;
        }
    }
}