using System;
using System.Collections.Generic;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using SharedCode.Entities;
using SharedCode.Entities.Mineable;
using Uins;
using UnityEngine;

namespace ProcessSourceNamespace
{
    public class ProcessSource : IProcessSource
    {
        public event StateChangedDelegate StateChanged;
        public event ItemsAchievedDelegate ItemsAchieved;
        public event EndingDelegate FailOrCancelEnding;


        //=== Props ===========================================================

        public ProcessSourceId Id { get; }
        public float StartProgress { get; protected set; }
        public float EndProgress { get; protected set; }
        public float ProgressDuration { get; protected set; }

        public List<ProbabilisticItemPack> ExpectedItems { get; protected set; }

        public Sprite ProcessIcon { get; }

        public bool CanBeShown { get; }

        public bool IsEnded { get; protected set; }
        public int ChangeEventsCount { get; protected set; }
        public int AchievedEventsCount { get; protected set; }
        public float CreateTime { get; }


        //=== Ctor ============================================================

        public ProcessSource(ProcessSourceId id, float startProgress, float endProgress,
            float progressDuration, List<ProbabilisticItemPack> expectedItems, Sprite processIcon = null, bool canBeShown = true)
        {
            CreateTime = Time.time;
            Id = id;
            StartProgress = startProgress;
            EndProgress = endProgress;
            ProgressDuration = progressDuration;
            ExpectedItems = expectedItems ?? new List<ProbabilisticItemPack>();
            ProcessIcon = processIcon;
            CanBeShown = canBeShown;
        }


        //=== Public ==========================================================

        public void SetStateChanged(float newStartProgress, float newEndProgress, float newProgressDuration,
            List<ProbabilisticItemPack> changedExpectedItems = null)
        {
            newStartProgress = Mathf.Clamp01(newStartProgress);
            newEndProgress = Mathf.Clamp01(newEndProgress);
            bool isEndProgressChanged = !Mathf.Approximately(newEndProgress, EndProgress);
            EndProgress = newEndProgress;
            bool isStartProgressChanged = !Mathf.Approximately(newStartProgress, StartProgress);
            StartProgress = newStartProgress;
            bool isProgressDurationChanged = !Mathf.Approximately(newProgressDuration, ProgressDuration);
            ProgressDuration = newProgressDuration;
            bool isExpectedItemsChanged = changedExpectedItems != null && !IsExpectedItemsEquivalentTo(changedExpectedItems);
            if (isExpectedItemsChanged)
                ExpectedItems = changedExpectedItems;

            ChangeEventsCount++;
            StateChanged?.Invoke(this, isEndProgressChanged, isStartProgressChanged, isProgressDurationChanged, isExpectedItemsChanged);
        }

        public void SetItemsAchieved([NotNull] IList<ItemResourcePack> achievedItems, IList<uint> inventoryCounts, bool isEnded = true)
        {
            AchievedEventsCount++;
            if (isEnded)
                IsEnded = true;
            ItemsAchieved?.Invoke(this, achievedItems, inventoryCounts, isEnded);
        }

        public void SetEnding(bool isFail)
        {
            IsEnded = true;
            try
            {
                FailOrCancelEnding?.Invoke(this, isFail);
            }
            catch (Exception e)
            {
                UI.Logger.IfError()?.Message($"*-*+* SetEnding({this}) exception: {e}").Write(); //DEBUG
            }
        }

        public override string ToString()
        {
            return $"{nameof(ProcessSource)}: {nameof(Id)}={Id}, Progress={StartProgress}...{EndProgress}, " +
                   $"Duration={ProgressDuration}, {nameof(CanBeShown)}{CanBeShown.AsSign()}, " +
                   $"{nameof(ExpectedItems)}={ExpectedItems?.Count.ToString() ?? "null"}, " +
                   $"{nameof(ProcessIcon)}={ProcessIcon?.name ?? "null"}, {nameof(IsEnded)}{IsEnded.AsSign()}, " +
                   $"{nameof(AchievedEventsCount)}={AchievedEventsCount}, {nameof(ChangeEventsCount)}={ChangeEventsCount}";
        }

        public string ToStringShort()
        {
            return $"{Id.ToStringShort()} {StartProgress}...{EndProgress}, " +
                   $"dur={ProgressDuration}, {nameof(CanBeShown)}{CanBeShown.AsSign()}, " +
                   $"{nameof(ExpectedItems)}={ExpectedItems?.Count.ToString() ?? "null"}, " +
                   $"{nameof(ChangeEventsCount)}={ChangeEventsCount}" +
                   (ProcessIcon != null ? $" {nameof(ProcessIcon)}={ProcessIcon.name}" : null);
        }

        private bool IsExpectedItemsEquivalentTo(List<ProbabilisticItemPack> other)
        {
            if (ExpectedItems == other)
                return true;

            if (other == null)
                return false;

            if (ExpectedItems.Count != other.Count)
                return false;

            if (ExpectedItems.Count == 0)
                return true;

            for (int i = 0, len = ExpectedItems.Count; i < len; i++)
            {
                if (!ExpectedItems[i].Equals(other[i]))
                    return false;
            }

            return true;
        }
    }
}