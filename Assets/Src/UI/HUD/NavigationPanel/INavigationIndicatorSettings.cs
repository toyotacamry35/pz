using UnityEngine;

namespace Uins
{
    public interface INavigationIndicatorSettings
    {
        Sprite Icon { get; }
        Color IconColor { get; }
        float FovToDisplay { get; }
        float DistanceToDisplay { get; }
        bool IsSelectable { get; }
        bool IsShowDist { get; }
    }
}