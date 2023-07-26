using System.Collections.Generic;
using System.Linq;
using Assets.Src.ContainerApis;
using Assets.Src.Inventory;
using Uins.Slots;
using UnityEngine;

namespace Uins
{
    public class SavedPerksCollection : PerksCollection<SavedPerkSlotViewModel, PerksSavedFullApi, PerkSlotsSavedFullApi>
    {
        protected override bool DoSortEmptyPerks => false;
    }
}