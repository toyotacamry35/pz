using System;
using SharedCode.Entities.Engine;
using UnityEngine;

namespace Uins
{
    public delegate void AddMapTargetDelegate(Transform target, Guid markerGuid, IMapIndicatorSettings mapIndicatorSettings, 
        bool isInteractable, IGuideProvider cameraGuideProvider = null);

    public delegate void TargetGuidAndTransformDelegate(Guid markerGuid, Transform target);
    public delegate void TargetTransformDelegate(Transform target);

    public interface IMapNotificationProvider
    {
        event AddMapTargetDelegate AddMapIndicatorTarget;
        event TargetGuidAndTransformDelegate RemoveMapIndicatorTarget;
        void SetSelectedTarget(Transform newTarget);
    }
}