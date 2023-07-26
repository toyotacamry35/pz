using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using L10n;
using SharedCode.Wizardry;
using UnityEngine;

namespace ResourceSystem.GeneratedDefsForSpells
{
    public class EffectShowEndGameInterfaceDef : SpellEffectDef
    {
        public LocalizedString TitleLs { get; set; }
        public LocalizedString TextLs { get; set; }
        public UnityRef<Sprite> Image { get; set; }
    }
}