using UnityEngine;

namespace Uins
{
    public interface IMapIndicatorSettings
    {
        Sprite MapIcon { get; }
        Color PointColor { get; }
        bool IsSelectable { get; }
        bool IsPlayer { get; }
        int QuestZoneDiameter { get; }
        string Description { get; }
    }
}