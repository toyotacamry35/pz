using System.Collections.Generic;
using SharedCode.Entities;
using SharedCode.Entities.Mineable;
using UnityEngine;

namespace ProcessSourceNamespace
{
    public delegate void StateChangedDelegate(IProcessSource processSource, bool isEndProgressChanged,
        bool isStartProgressChanged = false, bool isProgressDurationChanged = false, bool isExpectedItemsChanged = false);

    public delegate void ItemsAchievedDelegate(IProcessSource processSource, IList<ItemResourcePack> achievedItems, 
        IList<uint> inventoryCounts, bool isEnded = true);

    public delegate void JustItemsAchievedDelegate(IList<ItemResourcePack> achievedItems, IList<uint> inventoryCounts);

    public delegate void EndingDelegate(IProcessSource processSource, bool isFail);

    public interface IProcessSource
    {
        ProcessSourceId Id { get; }
        float StartProgress { get; }
        float EndProgress { get; }
        float ProgressDuration { get; }

        List<ProbabilisticItemPack> ExpectedItems { get; } //NotNull
        Sprite ProcessIcon { get; } //MayBeNull

        bool CanBeShown { get; }
        bool IsEnded { get; }

        int ChangeEventsCount { get; }
        int AchievedEventsCount { get; }

        event StateChangedDelegate StateChanged;
        event ItemsAchievedDelegate ItemsAchieved;
        event EndingDelegate FailOrCancelEnding;
    }
}