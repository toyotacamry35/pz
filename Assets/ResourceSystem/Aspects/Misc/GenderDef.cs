using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.Aspects.Item.Templates;
using UnityEngine;

namespace ResourceSystem.Aspects.Misc
{
    public class GenderDef : SaveableBaseResource
    {
        public ResourceRef<DefaultCharacterDef> DefaultCharacter { get; set; }
        public UnityRef<Sprite> InventoryImage { get; set; }
    }
}