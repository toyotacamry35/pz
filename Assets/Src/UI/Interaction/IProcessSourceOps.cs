using System.Collections.Generic;
using ProcessSourceNamespace;
using SharedCode.Entities;
using UnityEngine;

namespace Uins
{
    public interface IProcessSourceOps
    {
        GameObject SelectedTarget { get; }
        Dictionary<ProcessSourceId, ProcessSource> ProcessSources { get; }
        void AddProcessSource(ProcessSource processSource);
        void RemoveProcessSource(ProcessSource processSource);
        IList<uint> GetInventoryCounts(IList<ItemResourcePack> achievedResources);
        void SetJustItemsAchieved(IList<ItemResourcePack> achievedItems);
    }
}