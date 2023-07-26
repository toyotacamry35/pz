using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.Wizardry;
using UnityEngine;

namespace SharedCode.Aspects.Item.Templates
{
    public class SlotDef : BaseResource
    {
        public string DisplayName { get; set; }

        public int SlotId { get; set; }

        public int SelectionPriority { get; set; }

        public int MaxStack { get; set; }

        public ResourceRef<ItemTypeResource>[] AcceptsItems { get; set; } = { };

        public UnityRef<Sprite> BackgroundIcon { get; set; }

        public ResourceArray<SpellGroupDef> StopSpellGroupsOnUnuse;
        
        public override string ToString()
        {
            return $"[{GetType()}: {nameof(DisplayName)}={DisplayName}, {nameof(SlotId)}={SlotId}]";
            //+ $"{nameof(AcceptsItems)}={AcceptsItems.ItemsToString()}]";
        }
    }

    public class SlotsListDef : BaseResource
    {
        public ResourceRef<SlotDef>[] Slots { get; set; } = { };
    }
}