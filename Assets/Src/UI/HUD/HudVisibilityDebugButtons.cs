using Uins.Sound;
using UnityEngine;

namespace Uins
{
    /// <summary>
    /// DEBUG class
    /// </summary>
    public class HudVisibilityDebugButtons : MonoBehaviour
    {
        public HudVisibility HudVisibility;
        public HudBlocksVisibility Block;
        public bool IsAll;

        public void SetOn()
        {
            if (HudVisibility.AssertIfNull(nameof(HudVisibility)))
                return;

            HudVisibility.VisibleOn(Block);
        }

        public void SetOff()
        {
            if (HudVisibility.AssertIfNull(nameof(HudVisibility)))
                return;

            HudVisibility.VisibleOff(Block);
        }
    }
}