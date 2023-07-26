using L10n;
using SharedCode.Utils;
using SharedCode.Wizardry;

namespace Assets.ColonyShared.SharedCode.GeneratedDefsForSpells
{
    [Localized]
    public class EffectShowTextDef : SpellEffectDef
    {
        public string Text { get; set; }
        public LocalizedString TextLs { get; set; }
        public LocalizedString TextEndLs { get; set; }
        public bool ShouldShowEnd { get; set; } = false;
        public Color Color { get; set; } = Color.white;
        public bool ShowForEveryone {get; set;} = false;
        public bool IsError { get; set; } = false;
    }
}
