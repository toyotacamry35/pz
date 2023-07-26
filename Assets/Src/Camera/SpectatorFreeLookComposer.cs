using Cinemachine;
using UnityEngine;

namespace Assets.Src.Camera
{
    [AddComponentMenu("")] // Don't display in add component menu
    [RequireComponent(typeof(CinemachinePipeline))]
    [SaveDuringPlay]
    public class SpectatorFreeLookComposer : CinemachineComponentBase
    {
        [SerializeField] public float Yaw;
        [SerializeField] public float Pitch;
        [SerializeField] public SpectatorInputSettings InputSettings;

        public override bool IsValid => enabled;

        public override CinemachineCore.Stage Stage => CinemachineCore.Stage.Aim;

        public override void MutateCameraState(ref CameraState curState, float deltaTime)
        {
            if (!IsValid)
                return;

            if (deltaTime >= 0.0 || CinemachineCore.Instance.IsLive(VirtualCamera))
            {
                var inputAxes = GetInputAxes();
                var yawDelta = inputAxes.x * InputSettings.YawSensitivity;
                Yaw = UpdateYaw(Yaw, yawDelta);
                var pitchDelta = inputAxes.y * InputSettings.PitchSensitivity;
                Pitch = UpdatePitch(Pitch, pitchDelta);
            }

            Quaternion rot = Quaternion.Euler(Pitch, Yaw, 0);
            rot = rot * Quaternion.FromToRotation(Vector3.up, curState.ReferenceUp);
            curState.RawOrientation = rot;
        }

        private Vector2 GetInputAxes()
        {
            return new Vector2(
                Input.GetAxis(InputSettings.YawAxis),
                Input.GetAxis(InputSettings.PitchAxis)
                );
        }
        
        private float UpdateYaw(float yaw, float delta) => ClampYaw(Mathf.MoveTowardsAngle(yaw, yaw + delta, float.MaxValue));

        private float UpdatePitch(float pitch, float delta) => ClampPitch(pitch + delta);

        private static float ClampYaw(float yaw) => Mathf.Repeat(yaw, 360);

        private float ClampPitch(float pitch) => Mathf.Clamp(pitch, -90, 90);
    }
}
