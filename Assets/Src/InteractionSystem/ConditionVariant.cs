using System.Collections.Generic;
using EnumerableExtensions;
using JetBrains.Annotations;
using SharedCode.Entities.Mineable;
using UnityEngine;

namespace Assets.Src.InteractionSystem
{
    public class ConditionVariant
    {
        [NotNull]
        public List<ProbabilisticItemPack> Items = new List<ProbabilisticItemPack>();

        public bool IsRandomItems;

        [NotNull]
        public List<Sprite> ConditionMarkers = new List<Sprite>(); //Значки типа урона и т.п. условия при которых получаются Items


        //=== Public ==========================================================

        public override string ToString()
        {
            return $"[{nameof(ConditionVariant)}: {nameof(Items)} {Items.ItemsToStringByLines()}, " +
                   $"{nameof(ConditionMarkers)}: {ConditionMarkers.ItemsToString()}]";
        }
    }
}