using JetBrains.Annotations;
using ReactivePropsNs;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class HudSidePanelSwitcher : BindingViewModel
    {
        [SerializeField, UsedImplicitly]
        private bool _isRightSide;

        [Binding]
        public bool IsOpen { get; private set; }


        //=== Public ==========================================================

        public void Init(WindowsManager windowsManager)
        {
            var resourceUsageNotifier = _isRightSide ? windowsManager.HudRightPartUsageNotifier : windowsManager.HudLeftPartUsageNotifier;
            var isOpenStream = resourceUsageNotifier.InUsage.Func(D, inUsage => !inUsage);
            Bind(isOpenStream, () => IsOpen);
        }
    }
}