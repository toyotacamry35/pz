using ReactivePropsNs;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class HudVisibilityRepeater : HudVisibilityBase
    {
        //=== Public ==========================================================

        public void Init()
        {
            var masterHudVisibility = HudVisibility.Instance;
            if (masterHudVisibility.AssertIfNull(nameof(masterHudVisibility)))
                return;

            masterHudVisibility.BottomSlotsIsVisibleRp.Action(D, b => BottomSlotsIsVisibleRp.Value = b);
            masterHudVisibility.HpIsVisibleRp.Action(D, b => HpIsVisibleRp.Value = b);
            masterHudVisibility.NavigationIsVisibleRp.Action(D, b => NavigationIsVisibleRp.Value = b);
            masterHudVisibility.EnvironmentIsVisibleRp.Action(D, b => EnvironmentIsVisibleRp.Value = b);
            masterHudVisibility.ChatIsVisibleRp.Action(D, b => ChatIsVisibleRp.Value = b);
            masterHudVisibility.HelpBlockIsVisibleRp.Action(D, b => HelpBlockIsVisibleRp.Value = b);
            masterHudVisibility.FactionBlockIsVisibleRp.Action(D, b => FactionBlockIsVisibleRp.Value = b);
            masterHudVisibility.OtherIsVisibleRp.Action(D, b => OtherIsVisibleRp.Value = b);
        }
    }
}