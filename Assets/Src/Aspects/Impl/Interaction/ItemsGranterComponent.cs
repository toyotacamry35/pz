using System.Collections.Generic;
using Assets.Src.Inventory;
using Assets.Src.Shared;
using System;
using NLog;


namespace Assets.Src.Aspects.Impl
{
    /// #TC-5094(3): most likely REMOVE it (Vitaly said to).
    [Obsolete("Is gradually removing from use", false)]
    public class ItemsGranterComponent : ColonyBehaviour
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public bool GrantRandom;
        //public List<ItemStack> GrantingItems = new List<ItemStack>();
    }
}