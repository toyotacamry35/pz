using Cinemachine;
using UnityEngine;

namespace  Assets.Src.Camera
{
    public class SpectatorSphericalTransposer : CinemachineTransposer
    {
        [SerializeField] public float Yaw;
        [SerializeField] public float Pitch;
        [SerializeField] public float Distance;
        [SerializeField] public SpectatorInputSettings InputSettings;
        [SerializeField] public float DistanceSpeed = 0.1f;
        [SerializeField] public float DistanceRunMod = 3;
        [SerializeField] public float MinDistance = 1;
        [SerializeField] public float MaxDistance = 100;
     
        private Vector3? _setWorldPosition;
        
        public Vector3 WorldPosition
        {
            set
            {
                _setWorldPosition = value;
            }
        }
        
        public override void MutateCameraState(ref CameraState curState, float deltaTime)
        {
            InitPrevFrameStateInfo(ref curState, deltaTime);

            if (!IsValid)
                return;

            if (deltaTime >= 0.0 || CinemachineCore.Instance.IsLive(VirtualCamera))
            {
                var inputAxes = GetInputAxes();
                var yawDelta = inputAxes.x * InputSettings.YawSensitivity;
                Yaw = UpdateYaw(Yaw, yawDelta);
                var pitchDelta = inputAxes.y * InputSettings.PitchSensitivity;
                Pitch = UpdatePitch(Pitch, pitchDelta);
                var speed = Mathf.Lerp(DistanceSpeed, DistanceSpeed * DistanceRunMod, Input.GetAxis(InputSettings.RunTrigger));
                var distanceDelta = -inputAxes.z * InputSettings.MoveForwardSensitivity * speed;
                Distance = UpdateDistance(Distance, distanceDelta);
            }
            
            if (_setWorldPosition != null)
            {
                var offset = Quaternion.Inverse(GetReferenceOrientation(curState.ReferenceUp)) * (_setWorldPosition.Value - FollowTargetPosition)  - EffectiveOffset;
                var distance = offset.magnitude;
                Distance = distance;
                Pitch = ClampPitch(Mathf.Asin(offset.y / distance) * Mathf.Rad2Deg);
                Yaw = ClampYaw(Mathf.Atan2(-offset.x, -offset.z) * Mathf.Rad2Deg);
                _setWorldPosition = null;
            }

            Quaternion quaternion = Quaternion.Euler(Pitch, Yaw, 0);
            Vector3 effectiveOffset = EffectiveOffset + quaternion * Vector3.back * Distance; // смещение камеры в ReferenceOrientation
            Vector3 outTargetPosition;
            Quaternion outTargetOrient;
            TrackTarget(deltaTime,  curState.ReferenceUp, effectiveOffset, out outTargetPosition, out outTargetOrient);
            curState.ReferenceUp = outTargetOrient * Vector3.up;
            curState.RawPosition = outTargetPosition + outTargetOrient * effectiveOffset;
            
            transform.position = curState.RawPosition;
        }
        
        protected override void OnValidate()
        {
            base.OnValidate();
        }

        private Vector3 GetInputAxes()
        {
            return new Vector3(
                Input.GetAxis(InputSettings.YawAxis),
                Input.GetAxis(InputSettings.PitchAxis),
                Input.GetAxis(InputSettings.MoveForwardInputAxis)
            );
        }
        
        private float UpdateYaw(float yaw, float delta) => ClampYaw(Mathf.MoveTowardsAngle(yaw, yaw + delta, float.MaxValue));

        private float UpdatePitch(float pitch, float delta) => ClampPitch(pitch + delta);

        private float UpdateDistance(float distance, float delta) => ClampDistance(distance + delta);
        
        private static float ClampYaw(float yaw) => Mathf.Repeat(yaw, 360);

        private float ClampPitch(float pitch) => Mathf.Clamp(pitch, -90, 90);
        
        private float ClampDistance(float pitch) => Mathf.Clamp(pitch, MinDistance, MaxDistance);
    }
}
