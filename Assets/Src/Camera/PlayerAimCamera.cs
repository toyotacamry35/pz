using Cinemachine;
using Cinemachine.Utility;
using SharedCode.Entities.Engine;
using Src.Aspects.Doings;
using Src.Camera;
using Src.Locomotion.Unity;
using UnityEngine;

namespace Assets.Src.Camera
{
    // Note: данная камера нормально работает только CinemachineBrain.UpdateMethod.LateUpdate 
    // Note: данная камера нормально работает только при worldUp == Vector3.up
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [AddComponentMenu("Cinemachine/Player Aim")]
    public class PlayerAimCamera : CinemachineVirtualCameraBase, ICameraWithFollow, ICameraWithAiming
    {
        [SerializeField] private float _pitch;
        [NoSaveDuringPlay, SerializeField] private Transform _follow;
        [LensSettingsProperty, SerializeField] private LensSettings _lens = LensSettings.Default;
        [SerializeField] private AnimationCurve _distanceByPitch = AnimationCurve.Linear(0, 1, 1, 1);
        [SerializeField] private float _distanceFactor = 1;
        [SerializeField] private float _minDistance = 0.2f; // помагает коллижену камеры не влезать в тушку
        [SerializeField] private Vector3 _offset;
        [SerializeField] private Vector3 _damping;

        private CameraState _state;
        private Vector3 _pivotPosition;
        private float _minPitch => -90;
        private float _maxPitch => 90;
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
            AimToward(point - Vector3.up * _offset.y - _pivotPosition);
        }

        private void AimToward(Vector3 direction)
        { 
            if (direction.sqrMagnitude > 0.0001f)
            {
                var pitch = Mathf.Asin(direction.y / direction.magnitude) * Mathf.Rad2Deg;
                _pitch = ClampPitch(pitch);
            }
        }
        
        public override void InternalUpdateCameraState(Vector3 worldUp, float deltaTime)
        {
            var follow = Follow;
            
            if(!follow)
                return;
         
            if (_guideProvider  != null)
                AimToward(_guideProvider.Guide.ToUnity()); 
            
            _state = PullStateFromVirtualCamera(worldUp, ref _lens);
            _state.ReferenceLookAt = follow.position;
            _lens = _state.Lens; 

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

            var yawRot = Quaternion.LookRotation(_follow.forward.SetY(0).normalized, worldUp);
            var pitchRot = Quaternion.AngleAxis(_pitch, Vector3.Cross(Vector3.forward, worldUp).normalized);
            var distance = _distanceFactor * _distanceByPitch.Evaluate(Mathf.InverseLerp(_minPitch, _maxPitch, _pitch));
            var position = _pivotPosition 
                           + yawRot * (Vector3.right * _offset.x 
                                     + Vector3.forward * _offset.z 
                                     + pitchRot * (Vector3.up * _offset.y + Vector3.back * distance));
            _state.RawPosition = position;
            InvokePostPipelineStageCallback(this, CinemachineCore.Stage.Body, ref _state, deltaTime);
            _state.RawOrientation = yawRot * pitchRot;
            InvokePostPipelineStageCallback(this, CinemachineCore.Stage.Aim, ref _state, deltaTime);
            InvokePostPipelineStageCallback(this, CinemachineCore.Stage.Noise, ref _state, deltaTime);
            InvokePostPipelineStageCallback(this, CinemachineCore.Stage.Finalize, ref _state, deltaTime);

            transform.position = _state.FinalPosition;
            transform.rotation = _state.FinalOrientation;
            
            PreviousStateIsValid = true;
        }
        
        private float ClampPitch(float pitch)
        {
            pitch = Mathf.Clamp(pitch, _minPitch, _maxPitch);
            return pitch;
        }
        
        protected override void OnEnable()
        {
            base.OnEnable();
            _state = PullStateFromVirtualCamera(Vector3.up, ref _lens);
        }
    }
}