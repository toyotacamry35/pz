using Cinemachine;
using UnityEngine;

namespace  Assets.Src.Camera
{
    [AddComponentMenu("")]
    [RequireComponent(typeof (CinemachinePipeline))]
    [SaveDuringPlay]
    public class SpectatorFreeFlyTransposer : CinemachineComponentBase
    {
        public int SpeedIndex = 4;
        public float[] MoveSpeeds = {0.3f, 0.5f, 1, 2, 3};
        public float RunFactor = 3f;
        public float MoveAccelTime = 0.1f;
        public float MoveDecelTime = 0.1f;
        public SpectatorInputSettings InputSettings;

        private float MoveSpeed => MoveSpeeds[SpeedIndex < MoveSpeeds.Length ? (SpeedIndex >= 0 ? SpeedIndex : 0) : MoveSpeeds.Length - 1];
        
        private Vector3 _moveSpeed;
        private float _moveTime;

        public Vector3 WorldPosition
        {
            get { return transform.position; }
            set { transform.position = value; }
        }

        public override bool IsValid => true;

        public override CinemachineCore.Stage Stage => CinemachineCore.Stage.Body;

        public void SelectSpeed(int index)
        {
            SpeedIndex = index;
        }

        public override void MutateCameraState(ref CameraState curState, float deltaTime)
        {
            var axisForw = Input.GetAxis(InputSettings.MoveForwardInputAxis) * InputSettings.MoveForwardSensitivity;
            var axisSide = Input.GetAxis(InputSettings.MoveSideInputAxis) * InputSettings.MoveSideSensitivity;
            var axisUp = Input.GetAxis(InputSettings.MoveUpInputAxis) * InputSettings.MoveUpSensitivity;
            var maxSpeed = Mathf.Lerp(MoveSpeed, MoveSpeed * RunFactor, Input.GetAxis(InputSettings.RunTrigger));

            UpdateSpeed(ref _moveSpeed.z, axisForw, maxSpeed, MoveAccelTime, MoveDecelTime, deltaTime);
            UpdateSpeed(ref _moveSpeed.y, axisUp, maxSpeed, MoveAccelTime, MoveDecelTime, deltaTime);
            UpdateSpeed(ref _moveSpeed.x, axisSide, maxSpeed, MoveAccelTime, MoveDecelTime, deltaTime);

            var speed = _moveSpeed.magnitude;
            if (speed > maxSpeed)
                _moveSpeed = _moveSpeed / speed * maxSpeed; 
            
            transform.position += VirtualCamera.State.RawOrientation * _moveSpeed * deltaTime;
            curState.RawPosition = transform.position;
        }

        static bool UpdateSpeed(ref float speed, float axisValue, float maxSpeed, float accelTime, float decelTime, float deltaTime)
        {
            float f = axisValue * maxSpeed;
            if (Mathf.Abs(f) < 9.99999974737875E-05 || Mathf.Sign(speed) == Mathf.Sign(f) &&
                Mathf.Abs(f) < Mathf.Abs(speed))
            {
                speed -= Mathf.Sign(speed) *
                         Mathf.Min(Mathf.Abs(f - speed) / Mathf.Max(0.0001f, decelTime) * deltaTime,
                             Mathf.Abs(speed));
            }
            else
            {
                float num = Mathf.Abs(f - speed) / Mathf.Max(0.0001f, accelTime);
                speed += Mathf.Sign(f) * num * deltaTime;
                if (Mathf.Sign(speed) == Mathf.Sign(f) && Mathf.Abs(speed) > Mathf.Abs(f))
                    speed = f;
            }
            
            speed = Mathf.Clamp(speed, -maxSpeed, maxSpeed);
            return Mathf.Abs(axisValue) > 9.99999974737875E-05;
        }
    }
}