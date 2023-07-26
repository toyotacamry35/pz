using SharedCode.Wizardry;
using Uins;
using Uins.Sound;

namespace Shared.ManualDefsForSpells
{
    public class ImpactSetHudVisibilityDef : SpellImpactDef
    {
        public HudBlocksVisibility Hud { get; set; }
        public bool Enable { get; set; } = true;
    }
}