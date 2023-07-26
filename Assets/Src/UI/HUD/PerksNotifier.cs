using Assets.Src.Aspects;
using Assets.Src.Aspects.Impl;
using Assets.Src.Inventory;
using Assets.Src.ResourceSystem;
using Assets.Src.SpawnSystem;
using JetBrains.Annotations;
using ReactivePropsNs;
using Uins;
using Uins.Inventory;
using UnityEngine;
using UnityWeld.Binding;

[Binding]
public class PerksNotifier : BindingViewModel
{
    [SerializeField, UsedImplicitly]
    private UnseenYetPanel _unseenYetPanel;

    [SerializeField, UsedImplicitly]
    private UnseenYetIndicatorDefRef _indicatorDefRef;

    [SerializeField, UsedImplicitly]
    private WindowId _inventoryWindowId;

    private UnseenYetIndicator _unseenYetIndicator;


    //=== Props ===============================================================

    private bool _isPerksTabVisible;

    public bool IsPerksTabVisible
    {
        get => _isPerksTabVisible;
        private set
        {
            if (_isPerksTabVisible != value)
            {
                _isPerksTabVisible = value;
                if (_isPerksTabVisible)
                    UnseenPerksCount = 0;
            }
        }
    }

    private int _unseenPerksCount;

    public int UnseenPerksCount
    {
        get => _unseenPerksCount;
        set
        {
            if (_isPerksTabVisible)
                value = 0;
            if (_unseenPerksCount != value)
            {
                _unseenPerksCount = value;
                _unseenYetIndicator?.SetCount(_unseenPerksCount);
            }
        }
    }


    //=== Unity ===============================================================

    private void Awake()
    {
        _unseenYetPanel.AssertIfNull(nameof(_unseenYetPanel));
        _indicatorDefRef.Target.AssertIfNull(nameof(_indicatorDefRef));
        _inventoryWindowId.AssertIfNull(nameof(_inventoryWindowId));
    }


    //=== Public ==========================================================

    public void Init(IPawnSource pawnSource, PerksPanelViewModel perksPanelViewModel, IStream<ContextViewWithParamsVmodel> cvwpVmodelStream, 
        WindowsManager windowsManager)
    {
        if (pawnSource.AssertIfNull(nameof(pawnSource)) ||
            perksPanelViewModel.AssertIfNull(nameof(perksPanelViewModel)) ||
            cvwpVmodelStream.AssertIfNull(nameof(cvwpVmodelStream)) ||
            windowsManager.AssertIfNull(nameof(windowsManager)))
            return;

        pawnSource.PawnChangesStream.Action(D, OnOurPawnChanged);
        perksPanelViewModel.TemporaryPerkAdded += OnTemporaryPerkAdded;
        var inventoryWindow = windowsManager.GetWindow(_inventoryWindowId);
        _unseenYetIndicator = _unseenYetPanel.GetNewIndicator(_indicatorDefRef.Target);

        var perksInventoryTabIsOpenStream = cvwpVmodelStream.SubStream(D, vm => vm.GetTabVmodel(InventoryTabType.Perks).IsOpenTabRp);
        inventoryWindow.State
            .Zip(D, perksInventoryTabIsOpenStream)
            .Action(D, (state, isTabOpened) => OnPerksTabVisibilityChanged(state == GuiWindowState.Opened && isTabOpened));
    }


    //=== Private =========================================================

    private void OnOurPawnChanged(EntityGameObject prevEgo, EntityGameObject newEgo)
    {
        UnseenPerksCount = 0;
    }

    private void OnTemporaryPerkAdded(SlotItem slotItem)
    {
        var perkResource = slotItem?.ItemResource;
        if (perkResource == null)
            return;

        UnseenPerksCount++;
        CenterNotificationQueue.Instance.SendNotification(new PerkNotificationInfo(perkResource));
    }

    private void OnPerksTabVisibilityChanged(bool isVisible)
    {
        IsPerksTabVisible = isVisible;
    }
}