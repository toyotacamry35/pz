using System.Collections.Generic;
using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.Aspects.Science;
using SharedCode.Entities;

namespace SharedCode.Aspects.Item.Templates
{
    public class DefaultCharacterDef : BaseResource, IHasDollDef
    {
        public DefaultContainer DefaultInventory { get; set; }

        public List<DefaultItemsStack> FirstRunInventory { get; set; }

        public DefaultContainer DefaultDoll { get; set; }

        public List<DefaultItemsStack> FirstRunDoll { get; set; }

        public DefaultContainer DefaultTemporaryPerks { get; set; }

        public DefaultContainer DefaultPermanentPerks { get; set; }

        public DefaultContainer DefaultSavedPerks { get; set; }

        public ResourceRef<PlayerPoints> StartPoints { get; set; }

        public ResourceRef<SlotDef>[] DefaultBlockedSlots { get; set; }

    }

    public class DefaultContainer
    {
        public int Size { get; set; }

        public List<DefaultItemsStack> DefaultItems { get; set; }
    }

    public class DefaultItemsStack
    {
        public ResourceRef<SlotDef> Slot { get; set; }

        public ResourceRef<BaseItemResource> Item { get; set; }

        public ResourceRef<CalcerDef<BaseResource>> ItemCalcer { get; set; }

        public uint Count { get; set; }
    }
}
