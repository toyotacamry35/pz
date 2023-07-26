using UnityEngine;

namespace Utilities
{
    public class DebugTimelineChartTransformSlots : MonoBehaviour
    {
        [SerializeField] private DebugTimelineChart _position;
        [SerializeField] private DebugTimelineChart _positionX;
        [SerializeField] private DebugTimelineChart _positionY;
        [SerializeField] private DebugTimelineChart _positionZ;
        [SerializeField] private DebugTimelineChart _rotation;
        [SerializeField] private DebugTimelineChart _rotationX;
        [SerializeField] private DebugTimelineChart _rotationY;
        [SerializeField] private DebugTimelineChart _rotationZ;

        void Awake()
        {
            Setup();
        }

        void OnValidate()
        {
            Setup();
        }
        
        void Setup()
        {
            _position.SetValueProvider(_ => transform.position);
            _positionX.SetValueProvider(_ => transform.position.x);
            _positionY.SetValueProvider(_ => transform.position.y);
            _positionZ.SetValueProvider(_ => transform.position.z);
            _rotation.SetValueProvider(_ => transform.rotation * Vector3.forward);
            _rotationX.SetValueProvider(_ => transform.rotation.eulerAngles.x);
            _rotationY.SetValueProvider(_ => transform.rotation.eulerAngles.y);
            _rotationZ.SetValueProvider(_ => transform.rotation.eulerAngles.z);
        }
    }
}