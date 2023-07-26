using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using EnumerableExtensions;
using SharedCode.Aspects.Item.Templates;

namespace Assets.Src.Inventory
{
    public class SlotItem : IEquatable<SlotItem>
    {
        public BaseItemResource ItemResource; //м.б. null

        public uint Count;

        public StatModifier[] GeneralStats;

        public StatModifier[] SpecificStats;

        public List<SlotItem> InnerContainers = new List<SlotItem>();

        public Guid ItemGuid;


        //=== Props ===========================================================

        public bool IsEmpty => Count == 0;

        public bool HasInnerContainer => (ItemResource as ItemResource)?.InnerContainerSize > 0;

        public SlotItem InnerContainer => InnerContainers?.FirstOrDefault(slotItem => slotItem != null);

        public int InnerItemsCount => HasInnerContainer && InnerContainer != null ? (int) InnerContainer.Count : 0;

        private WeaponDef WeaponDef => (ItemResource as ItemResource)?.WeaponDef;

        public int AmmoMaxCount => WeaponDef?.MaxInnerStack ?? 0;


        //=== Public ==========================================================

        public override string ToString()
        {
            var sb = new StringBuilder(
                $"[{nameof(SlotItem)}: {nameof(ItemResource)}={ItemResource}, {nameof(Count)}={Count}");
            if (ItemGuid != Guid.Empty)
                sb.Append($", {nameof(ItemGuid)}={ItemGuid}");
            if (HasInnerContainer)
            {
                sb.Append($", SubItems={InnerItemsCount}/{AmmoMaxCount}");
                var innerItemName = InnerContainer?.ItemResource?.ToString();
                if (string.IsNullOrEmpty(innerItemName))
                    sb.Append($" of {innerItemName}");
            }

            sb.Append("]");
            return sb.ToString();
        }

        public override bool Equals(object other)
        {
            if (other == null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (GetType() != other.GetType())
                return false;

            return Equals(other as SlotItem);
        }

        public bool Equals(SlotItem other)
        {
            if (other == null)
                return false;

            return
                ItemResource == other.ItemResource &&
                ItemGuid == other.ItemGuid &&
                Count == other.Count &&
                StatModifier.Equals(GeneralStats, other.GeneralStats) &&
                StatModifier.Equals(SpecificStats, other.SpecificStats) &&
                InnerContainers.NullableSequenceEqual(other.InnerContainers);
        }

        public override int GetHashCode()
        {
            var countHash = Count.GetHashCode();
            var itemHash = ItemResource?.GetHashCode() ?? 0;
            var generalStatsHash = GeneralStats?.GetHashCode() ?? 0;
            var specificStatsHash = SpecificStats?.GetHashCode() ?? 0;
            var ammoContainersHash = InnerContainers?.GetHashCode() ?? 0;

            return countHash ^ itemHash ^ generalStatsHash ^ specificStatsHash ^ ammoContainersHash;
        }

        public void Reset()
        {
            ItemResource = null;
            ItemGuid = Guid.Empty;
            Count = 0;
            GeneralStats = null;
            SpecificStats = null;
            InnerContainers.Clear();
        }

        public void Clone(SlotItem other)
        {
            ItemResource = other.ItemResource;
            ItemGuid = other.ItemGuid;
            Count = other.Count;
            StatsClone(other.GeneralStats, ref GeneralStats);
            StatsClone(other.SpecificStats, ref SpecificStats);
            AmmoContainersClone(other.InnerContainers, InnerContainers);
        }

        public static void StatsClone(StatModifier[] fromStats, ref StatModifier[] toStats)
        {
            if (fromStats == null)
            {
                toStats = null;
                return;
            }

            if (toStats == null || toStats.Length != fromStats.Length)
                toStats = new StatModifier[fromStats.Length];

            if (fromStats.Length == 0)
                return;

            for (int i = 0; i < fromStats.Length; i++)
                toStats[i] = fromStats[i];
        }

        public static void AmmoContainersClone(List<SlotItem> from, List<SlotItem> to)
        {
            if (from.AssertIfNull(nameof(from)) ||
                to.AssertIfNull(nameof(to)))
                return;

            while (to.Count < from.Count)
                to.Add(new SlotItem());

            while (to.Count > from.Count)
                to.RemoveAt(to.Count - 1);

            for (int i = 0; i < to.Count; i++)
                to[i].Clone(from[i]);
        }

        public void SetInnerContainer(int containerIndex, SlotItem containerSlotItem)
        {
            //UI.CallerLog($"{nameof(containerIndex)}={containerIndex}, {nameof(containerSlotItem)}={containerSlotItem}"); //DEBUG
            while (InnerContainers.Count < containerIndex + 1)
                InnerContainers.Add(new SlotItem());

            InnerContainers[containerIndex].Clone(containerSlotItem);
        }
    }
}