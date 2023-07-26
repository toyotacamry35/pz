using L10n;
using ReactivePropsNs;
using UnityEngine;

namespace Uins
{
    public struct AchievedItemVmodel
    {
        public Sprite ResourceSprite;
        public LocalizedString Description;
        public int AchievedItemsCount;
        public int TotalItemsCount;
        public ListStream<AchievedItemVmodel> ShownVmodels;
        public int Id;

        public bool IsEmpty => Id == 0;

        public override string ToString()
        {
            return $"({nameof(AchievedItemVmodel)}[{Id}] {Description} +{AchievedItemsCount}/{TotalItemsCount})";
        }
    }
}