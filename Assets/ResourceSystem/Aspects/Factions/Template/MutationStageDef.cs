using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.Src.ResourcesSystem.Base;
using L10n;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Wizardry;
using UnityEngine;

namespace Assets.Src.Aspects.Impl.Factions.Template
{
    [Localized]
    public class MutationStageDef : SaveableBaseResource
    {
        public string Name { get; set; }
        public LocalizedString NameLs { get; set; }

        public float Boundary { get; set; }
        public bool IsHostStage { get; set; }
        public bool AddStagePerksToPermanent { get; set; }
        public ResourceRef<ItemsListDef>[] StagePerks { get; set; }
        public ResourceRef<ItemsListDef>[] Perks { get; set; }
        public ResourceRef<ItemsListDef>[] Items { get; set; }
        public ResourceRef<QuestDef>[] StageQuests { get; set; }
        public ResourceRef<SpellDef>[] StageSpells{ get; set; }
        public UnityRef<GameObject> ResurrectionFX { get; set; }
        public string CharacterSoundStateGroup { get; set; }
        public string CharacterSoundState { get; set; } 
    }
}
