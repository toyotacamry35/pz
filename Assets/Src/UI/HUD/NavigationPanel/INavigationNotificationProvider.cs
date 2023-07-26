using UnityEngine;

namespace Uins
{
    public delegate void AddNavigationTargetDelegate(Transform target, INavigationIndicatorSettings navigationIndicatorSettings);

    public interface INavigationNotificationProvider
    {
        event AddNavigationTargetDelegate AddNavigationIndicatorTarget;
        event TargetTransformDelegate RemoveNavigationIndicatorTarget;
        event TargetTransformDelegate SelectedTargetChanged;
    }
}