using UnityEngine;

namespace Assets.Src.Character.Events
{
    public interface IHitPointAdjuster
    {
        (Transform Object, Vector3 Point) AdjustHitPoint(Vector3 worldPoint);
    }
}