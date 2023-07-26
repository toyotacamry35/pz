using Assets.Src.ResourcesSystem.Base;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Wizardry;
using System;

namespace ColonyShared.ManualDefsForSpells
{
    [Obsolete]
    public class EffectInventoryObserverDef : SpellEffectDef
    {
        public ResourceRef<BaseItemResource> Item { get; set; }
        public ResourceRef<SpellDef> SpellOnAppearance { get; set; } // Вызывается когда первый экземпляр предмета появляется в инвентаре
        public ResourceRef<SpellDef> SpellOnDisappearance { get; set; } // Вызывается когда последний экземпляр предмета убирается из инвентаря
        public ResourceRef<SpellDef> SpellOnPresence { get; set; } // Вызывается в момент attach'а если предмет есть в инвентаре
        public ResourceRef<SpellDef> SpellOnAbsence { get; set; } // Вызывается в момент attach'а если предмета нет в инвентаре
        public ResourceRef<SpellEntityDef> Target { get; set; }
        public int Count { get; set; } = 1;
    }
}