using Assets.Src.Lib.Extensions;
using Cinemachine;
using Cinemachine.Utility;
using Core.Environment.Logging.Extension;
using NLog;
using SharedCode.Entities.Engine;
using Src.Aspects.Doings;
using Src.Camera;
using Src.Locomotion;
using Src.Locomotion.Unity;
using UnityEngine;
using Utilities;

namespace Assets.Src.Camera
{
    // Note: данная камера нормально работает только CinemachineBrain.UpdateMethod.LateUpdate 
    // Note: данная камера нормально работает только при worldUp == Vector3.up
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [AddComponentMenu("Cinemachine/Player Free Look")]
    public class PlayerFreeLookCamera : CinemachineVirtualCameraBase, ICameraWithFollow, ICameraWithAiming
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        
        [SerializeField] private float _yaw;
        [SerializeField] private float _pitch;
        [NoSaveDuringPlay, SerializeField] private Transform _follow;
        [LensSettingsProperty, SerializeField] private LensSettings _lens = LensSettings.Default;
        [SerializeField] private AnimationCurve _distanceByPitch = AnimationCurve.Linear(0, 1, 1, 1);
        [SerializeField] private float _distanceFactor = 1;
        [SerializeField] private float _minDistance = 0.2f; // помагает коллижену камеры не влезать в тушку
        [SerializeField] private Vector3 _offset;
        [SerializeField] private Vector3 _damping;
        [SerializeField] private bool _enableCompensation = true;
        [Header("Debug")] 
        [SerializeField] private DebugTimelineChart _yawDeltaChart; 

        private CameraState _state;
        private Vector3 _pivotPosition;
        private float _minPitch => -90;
        private float _maxPitch => 90;
        private float _yawCompensation;
        private float _pitchCompensation;
        private IGuideProvider _guideProvider;
        
        public override CameraState State => _state;

        public override Transform Follow { get { return _follow; } set { _follow = value; } }

        public override Transform LookAt { get { return _follow; } set { _follow = value; } }

        public void SetGuideProvider(IGuideProvider guideProvider)
        {
            _guideProvider = guideProvider;
        }
 
        private void AimAt(Vector3 point)
        { 
            var toAimPoint = point - Vector3.up * _offset.y - _pivotPosition;
            var toAimPointXZ = toAimPoint.SetY(0);
            if (toAimPoint.sqrMagnitude > 0.0001f)
            {
                var yaw = (Mathf.Atan2(toAimPointXZ.x, toAimPointXZ.z) - Mathf.Asin(_offset.x / toAimPointXZ.magnitude)) * Mathf.Rad2Deg;
                var pitch = Mathf.Asin(toAimPoint.y / toAimPoint.magnitude) * Mathf.Rad2Deg;
                _yaw = ClampYaw(yaw);
                _pitch = ClampPitch(pitch);
            }            
        }

        private void AimToward(Vector3 direction)
        { 
            var directionXZ = direction.SetY(0);
            if (direction.sqrMagnitude > 0.0001f)
            {
                var pitch = Mathf.Asin(direction.y / direction.magnitude) * Mathf.Rad2Deg;
                //var distance = Distance(pitch);
                //var yaw = (Mathf.Atan2(directionXZ.x, directionXZ.z) - Mathf.Atan2(_offset.x, distance)) * Mathf.Rad2Deg;
                var yaw = Mathf.Atan2(directionXZ.x, directionXZ.z) * Mathf.Rad2Deg;
                _yaw = ClampYaw(yaw);
                _pitch = ClampPitch(pitch);
                if(Logger.IsTraceEnabled) 
                {
                    var forward = new Vector2(direction.x, direction.z).normalized;
                    Logger.IfTrace()?.Message($"AimToward | Frame:{Time.frameCount} Forward:({forward.x:F4}, {forward.y:F4})").Write();
                }
            }
        }
        
        public override void InternalUpdateCameraState(Vector3 worldUp, float deltaTime)
        {
            if (_guideProvider  != null)
               AimToward(_guideProvider.Guide.ToUnity()); 
            
            _state = PullStateFromVirtualCamera(worldUp, ref _lens);
            _lens = _state.Lens; 

            var follow = Follow;
            if (!follow)
            {
                _state.OrientationCorrection = Quaternion.identity;
                return;
            }

            _state.ReferenceLookAt = follow.position;

            if (!PreviousStateIsValid)
            {
                deltaTime = -1f;
                _pivotPosition = follow.position;
            }
            else
            {
                var side = Vector3.Cross(worldUp, _state.RawOrientation * Vector3.forward).normalized;
                var look = Vector3.Cross(side, worldUp);
                var rot = Quaternion.LookRotation(look, worldUp);
                var offset = Quaternion.Inverse(rot) * (follow.position - _pivotPosition);
                offset = Damper.Damp(offset, _damping, deltaTime);
                _pivotPosition += rot * offset;
            }

            float yaw = _yaw, pitch = _pitch;            
            yaw += _yawCompensation;
            pitch += _pitchCompensation;

            var yawRot = Quaternion.AngleAxis(yaw, worldUp);
            var pitchRot = Quaternion.AngleAxis(pitch, Vector3.Cross(Vector3.forward, worldUp).normalized);
            var distance = Distance(pitch);
            var right = yawRot * Vector3.right;
            var forward = yawRot * Vector3.forward;
            var orientation = yawRot * pitchRot;
            var position = _pivotPosition
                           + right * _offset.x
                           + worldUp * _offset.y
                           + forward * _offset.z
                           + orientation * Vector3.back * distance;
            _state.RawPosition = position;
            InvokePostPipelineStageCallback(this, CinemachineCore.Stage.Body, ref _state, deltaTime);
            _state.RawOrientation = orientation;
            InvokePostPipelineStageCallback(this, CinemachineCore.Stage.Aim, ref _state, deltaTime);
            InvokePostPipelineStageCallback(this, CinemachineCore.Stage.Noise, ref _state, deltaTime);
            InvokePostPipelineStageCallback(this, CinemachineCore.Stage.Finalize, ref _state, deltaTime);
            _state.PositionCorrection += PullUpFromBody(_state.FinalPosition);

            float actualYaw = yaw, actualPitch = pitch;
            if (_enableCompensation)
                CameraPositionToYawAndPitch(_state.FinalPosition, ref actualYaw, ref actualPitch);
            
            if (!actualYaw.ApproximatelyEqual(yaw, 0.01f) || !actualPitch.ApproximatelyEqual(pitch, 0.01f))
            {
                _yawCompensation = yaw - actualYaw;
                _pitchCompensation = pitch - actualPitch;
                _yaw = actualYaw;
                _pitch = actualPitch;
            }

            _yawDeltaChart.Sample(_yaw, deltaTime);
            
            transform.position = _state.FinalPosition;
            transform.rotation = _state.FinalOrientation;
            
            if(Logger.IsTraceEnabled)
            {
                var fwd = _state.FinalOrientation * Vector3.forward;
                var fwd2 = new Vector2(fwd.x, fwd.z).normalized;
                Logger.IfTrace()?.Message($"InternalUpdateCameraState | Frame:{Time.frameCount} Forward:({fwd2.x:F4}, {fwd2.y:F4})").Write();
            }
            
            PreviousStateIsValid = true;
        }
        
        private float ClampYaw(float yaw)
        {
            yaw = Mathf.Repeat(yaw, 360);
            return yaw;
        }

        private float ClampPitch(float pitch)
        {
            pitch = Mathf.Clamp(pitch, _minPitch, _maxPitch);
            return pitch;
        }

        private float Distance(float pitch)
        {
            var distance = _distanceFactor * _distanceByPitch.Evaluate(Mathf.InverseLerp(_minPitch, _maxPitch, pitch));
            return distance;
        }

        private Vector3 PullUpFromBody(Vector3 position)
        {
            var pivot = _pivotPosition.ToVector2XZ();
            var toPivot = pivot - position.ToVector2XZ();
            if (toPivot.sqrMagnitude < _minDistance * _minDistance)
            {
                var newPosition = pivot - toPivot.normalized * _minDistance;    
                return new Vector3(newPosition.x, position.y, newPosition.y) - position;
            }
            return Vector3.zero;
        }

        private void CameraPositionToYawAndPitch(Vector3 position, ref float yaw, ref float pitch)
        { //TODO: работает только при worldUp == Vector3.up
            var pivotPosition = _pivotPosition + Vector3.up * _offset.y;
            var forward = pivotPosition - position;
            var forwardXZ = forward.SetY(0);
            var distanceXZ = forwardXZ.magnitude;
            if (distanceXZ > 0.001f)
            {
                var skewAngle = Mathf.Asin(Mathf.Min(_offset.x,distanceXZ) / distanceXZ);
                yaw = ClampYaw(Mathf.Atan2(forwardXZ.x, forwardXZ.z) + skewAngle * Mathf.Rad2Deg);
                pitch = ClampPitch(Mathf.Atan2(forward.y, distanceXZ * Mathf.Cos(skewAngle)) * Mathf.Rad2Deg);
            }
        }
        
        protected override void OnEnable()
        {
            base.OnEnable();
            _state = PullStateFromVirtualCamera(Vector3.up, ref _lens);
        }
    }
}