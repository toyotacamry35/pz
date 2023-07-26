using System;

namespace Uins.Sound
{
    [Flags]
    public enum HudBlocksVisibility
    {
        BottomSlots = 1,
        HpBlock = 1 << 1,
        NavigationBlock = 1 << 2,
        EnvironmentBlock = 1 << 3,
        ChatBlock = 1 << 4,
        HelpBlock = 1 << 5,
        FactionBlock = 1 << 6,
        Other = 1 << 12,
        AllVisible = BottomSlots | HpBlock | NavigationBlock | EnvironmentBlock | ChatBlock | HelpBlock | FactionBlock | Other
    }
}