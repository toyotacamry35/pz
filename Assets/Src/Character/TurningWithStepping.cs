using System;
using Plugins.DebugDrawingExtension;
using UnityEngine;
using static UnityEngine.Mathf;

namespace Assets.Src.Character
{
    public class TurningWithStepping : MonoBehaviour
    {
        private static readonly DebugDraw Drawer = DebugDraw.Manager.GetDrawer("TurningWithStepping");
        
        [SerializeField] private float _thresholdAngle = 25;
        [SerializeField] private float _toleranceAngle = 1;
        [SerializeField] private float _stepTime = 0.5333f;
        [SerializeField] private Animator _animator;
        [SerializeField] private string _stateTurningLeft = "TurningLeft";
        [SerializeField] private string _stateTurningRight = "TurningRight";
        [SerializeField] private float _fadeInTime = 0.1f;
        [SerializeField] private float _fadeOutTime = 0.1f;

        
        private float _currentOrientation; // Degrees
        private float _targetOrientation; // Degrees
        private float _currentVelocity; // Degrees
        private int _stateHashTurningLeft;
        private int _stateHashTurningRight;
        private float _fadeFactor;
        private bool _enabled;

        public void Enable()
        {
            _enabled = true;
        }

        public void Disable()
        {
            _enabled = false;
        }
        
        private void Awake()
        {
            _animator = (_animator ? _animator : GetComponent<Animator>()) ?? throw new NullReferenceException($"No animator on {transform.FullName()}");
            _stateHashTurningLeft = Animator.StringToHash(_stateTurningLeft);
            _stateHashTurningRight = Animator.StringToHash(_stateTurningRight);
        }

        private bool Enabled => _enabled;

        private void Start()
        {
            _currentOrientation = _targetOrientation = transform.parent.rotation.eulerAngles.y;
        }
        
        private void Update()
        {
            if (Enabled && _fadeFactor < 1)
                _fadeFactor = _fadeInTime > 0 ? Min(_fadeFactor + Time.deltaTime / _fadeInTime, 1) : 1;
            else
            if (!Enabled && _fadeFactor > 0)
                _fadeFactor = _fadeOutTime > 0 ? Max(_fadeFactor - Time.deltaTime / _fadeOutTime, 0) : 0;

            var pawnOrientation = transform.parent.rotation.eulerAngles.y;
            
            if (_fadeFactor > 0)
            {
                var thresholdAngle = Lerp(0, _thresholdAngle, _fadeFactor);
                if (Abs(DeltaAngle(pawnOrientation, _targetOrientation)) > thresholdAngle)
                    _targetOrientation = pawnOrientation;

                var angleDelta = DeltaAngle(_currentOrientation, _targetOrientation);
                var angleDeltaAbs = Abs(angleDelta);
                if (Abs(angleDelta) > _toleranceAngle)
                {
                    var turningSpeed = _stepTime > 0 ? _thresholdAngle / _stepTime : Abs(angleDelta) / Time.deltaTime; 
                    _currentOrientation = MoveTowardsAngle(_currentOrientation, _targetOrientation, Lerp(angleDeltaAbs, turningSpeed * Time.deltaTime, _fadeFactor));
                }

                transform.localRotation = Quaternion.Euler(0, DeltaAngle(pawnOrientation, _currentOrientation), 0);

                _animator.SetBool(_stateHashTurningLeft, _fadeFactor >= 1 && angleDelta < -_toleranceAngle);
                _animator.SetBool(_stateHashTurningRight, _fadeFactor >= 1 && angleDelta > _toleranceAngle);
            }
            else
            {
                _targetOrientation = _currentOrientation = pawnOrientation;
                
                transform.localRotation = Quaternion.identity;
                
                _animator.SetBool(_stateHashTurningLeft, false);
                _animator.SetBool(_stateHashTurningRight, false);
            }
            
            if (Drawer.IsDebugEnabled)
            {
                var origin = transform.parent.position;
                Drawer.Debug?
                    .Line(origin, origin + Quaternion.Euler(0, _currentOrientation, 0) * Vector3.forward, Color.green)
                    .Line(origin, origin + Quaternion.Euler(0, _targetOrientation + _thresholdAngle, 0) * Vector3.forward, Color.white)
                    .Line(origin, origin + Quaternion.Euler(0, _targetOrientation - _thresholdAngle, 0) * Vector3.forward, Color.white)
                    .Line(origin, origin + Quaternion.Euler(0, _targetOrientation, 0) * Vector3.forward, Color.red)
                    .Line(origin, origin + transform.parent.forward, Color.blue);
            }
        }
    }
}