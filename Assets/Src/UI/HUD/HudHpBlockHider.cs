using JetBrains.Annotations;
using ReactivePropsNs;
using Uins.Inventory;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class HudHpBlockHider : BindingViewModel
    {
        [SerializeField, UsedImplicitly]
        private HudVisibility _hudVisibility;


        //=== Props ===========================================================

        [Binding]
        public bool IsVisible { get; set; }


        //=== Unity ===========================================================

        private void Awake()
        {
            _hudVisibility.AssertIfNull(nameof(_hudVisibility));
        }


        //=== Public ==========================================================

        public void Init(InventoryNode inventoryNode)
        {
            if (inventoryNode.AssertIfNull(nameof(inventoryNode)))
                return;

            var isVisibleStream = _hudVisibility.HelpBlockIsVisibleRp
                .Zip(D, inventoryNode.State)
                .Zip(D, inventoryNode.ContextViewWithParamsVmodelStream.SubStream(D, vm => vm.ContextParamsStream, new ContextViewParams()))
                .Func(D, (isHelpBlockIsVisible, state, cvParams) => isHelpBlockIsVisible && !(state != GuiWindowState.Closed && cvParams.NeedForExtraSpace));

            Bind(isVisibleStream, () => IsVisible);
        }
    }
}