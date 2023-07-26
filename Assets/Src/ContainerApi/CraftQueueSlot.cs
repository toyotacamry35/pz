using System.Collections.Generic;
using Assets.ColonyShared.SharedCode.Aspects.Craft;

namespace Assets.Src.ContainerApis
{
    public class CraftQueueSlot
    {
        public CraftRecipeDef CraftRecipe;
        public int Count;
        public int SelectedVariantIndex;
        public bool IsActive;
        public long CraftStartTime;
        public int KeyIndex = -1;

        public bool IsEmpty => CraftRecipe == null;

        public void Reset()
        {
            CraftRecipe = null;
            Count = 0;
            SelectedVariantIndex = 0;
            IsActive = false;
            CraftStartTime = 0;
            KeyIndex = -1;
        }

        public override string ToString()
        {
            return
                $"[{nameof(CraftQueueSlot)}[{KeyIndex}]: IsActive{IsActive.AsSign()}, Count={Count}, SelectedVariantIndex={SelectedVariantIndex}, " +
                $"CraftStartTime={CraftStartTime}, CraftRecipe={CraftRecipe}]";
        }

        public void SetFrom(CraftQueueSlot newCraftQueueSlot)
        {
            CraftRecipe = newCraftQueueSlot.CraftRecipe;
            Count = newCraftQueueSlot.Count;
            SelectedVariantIndex = newCraftQueueSlot.SelectedVariantIndex;
            IsActive = newCraftQueueSlot.IsActive;
            CraftStartTime = newCraftQueueSlot.CraftStartTime;
            KeyIndex = newCraftQueueSlot.KeyIndex;
        }
    }

    public class CraftQueueSlotComparer : EqualityComparer<CraftQueueSlot>
    {
        public override bool Equals(CraftQueueSlot x, CraftQueueSlot y)
        {
            if (ReferenceEquals(x, y)) return true;

            return x != null && y != null &&
                   x.CraftRecipe == y.CraftRecipe &&
                   x.Count == y.Count &&
                   x.SelectedVariantIndex == y.SelectedVariantIndex &&
                   x.IsActive == y.IsActive &&
                   x.CraftStartTime == y.CraftStartTime &&
                   x.KeyIndex == y.KeyIndex;
        }

        public override int GetHashCode(CraftQueueSlot obj)
        {
            var countHash = obj.Count.GetHashCode();
            var crHash = obj.CraftRecipe?.GetHashCode() ?? 0;
            var sviHash = obj.SelectedVariantIndex.GetHashCode();
            var iHash = obj.KeyIndex.GetHashCode();
            var iaHash = obj.IsActive.GetHashCode();
            var tacHash = obj.CraftStartTime.GetHashCode();

            return countHash ^ crHash ^ sviHash ^ iHash ^ iaHash ^ tacHash;
        }
    }
}