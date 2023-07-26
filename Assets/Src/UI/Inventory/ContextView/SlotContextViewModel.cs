using Assets.Src.Aspects.Impl.Stats;
using Assets.Src.Inventory;
using Assets.Src.ResourceSystem;
using JetBrains.Annotations;
using ReactivePropsNs;
using SharedCode.Aspects.Item.Templates;
using Uins.Inventory;
using Uins.Slots;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class SlotContextViewModel : SlotPropsBaseViewModel
    {
        [SerializeField, UsedImplicitly]
        private OurCharacterSlotsViewModel _ourCharacterSlotsViewModel;

        [SerializeField, UsedImplicitly]
        private ContextViewWithParams _contextView;

        [SerializeField, UsedImplicitly]
        private GameObject _visualGameObject;

        [SerializeField, UsedImplicitly]
        private JdbMetadata _dpsStatJdb;

        [SerializeField, UsedImplicitly]
        private JdbMetadata _damageStatJdb;

        [SerializeField, UsedImplicitly]
        private JdbMetadata _anyWeaponTypeJdb;

        [SerializeField, UsedImplicitly]
        private ContextActionButtons _contextActionButtons;

        private IContextActionsSource _contextActionsSource;


        //=== Public ==========================================================

        public static StatResource DamageStat { get; private set; }

        public static StatResource DpsStat { get; private set; }

        public static ItemTypeResource WeaponGroupType { get; private set; }


        //=== Protected =======================================================

        protected override void OnAwake()
        {
            if (_contextView.AssertIfNull(nameof(_contextView)) ||
                _visualGameObject.AssertIfNull(nameof(_visualGameObject)) ||
                _damageStatJdb.AssertIfNull(nameof(_damageStatJdb)) ||
                _anyWeaponTypeJdb.AssertIfNull(nameof(_anyWeaponTypeJdb)) ||
                _contextActionButtons.AssertIfNull(nameof(_contextActionButtons)) ||
                _ourCharacterSlotsViewModel.AssertIfNull(nameof(_ourCharacterSlotsViewModel)))
                return;

            SetStaticVars();

            _contextView.Vmodel.SubStream(D, vm => vm.CurrentContext).Action(D, OnContextViewTargetChanged);
            _ourCharacterSlotsViewModel.CharacterItemsChanged += OnOurCharacterItemsChanged;
            SwitchVisibility(false);
        }

        private void OnOurCharacterItemsChanged(SlotViewModel slotViewModel, int stackDelta)
        {
            if (TargetSlotViewModel != slotViewModel)
                return;

            UpdateContextButtons();
        }

        private void SetStaticVars()
        {
            DamageStat = _damageStatJdb?.Get<StatResource>();
            DamageStat.AssertIfNull(nameof(DamageStat));

            DpsStat = _dpsStatJdb?.Get<StatResource>();
            DpsStat.AssertIfNull(nameof(DpsStat));

            WeaponGroupType = _anyWeaponTypeJdb?.Get<ItemTypeResource>();
            WeaponGroupType.AssertIfNull(nameof(WeaponGroupType));
        }


        //=== Private =========================================================

        private void OnContextViewTargetChanged(IContextViewTarget target)
        {
            var asSlotViewModel = target as SlotViewModel;
            SwitchVisibility(asSlotViewModel != null);
            TargetSlotViewModel = asSlotViewModel;
            UpdateContextButtons();
        }

        private void UpdateContextButtons()
        {
            _contextActionsSource = TargetSlotViewModel?.ContextActionsSource;
            if (_contextActionsSource != null)
                _contextActionButtons.SetContextButtons(_contextActionsSource.OnContextButtonsRequest(TargetSlotViewModel));
            else
                _contextActionButtons.SetContextButtons();
        }

        private void SwitchVisibility(bool isVisible)
        {
            _visualGameObject.SetActive(isVisible);
        }
    }
}