using Cinemachine;
using UnityEngine;
using UnityEngine.Assertions;

namespace  Assets.Src.Camera
{
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class SpectatorPathPositionController : MonoBehaviour
    {
        public int SpeedIndex = 4;
        public float[] MoveSpeeds = {0.3f, 0.5f, 1, 2, 3};
        public float RunFactor = 3f;
        public float MoveAccelTime = 0.1f;
        public float MoveDecelTime = 0.1f;
        public string MoveForwardInputAxis = "FlyCameraMoveAxis";
        public string RunTrigger = "FlyCameraRun";

        public void SelectSpeed(int index)
        {
            SpeedIndex = index;
        }
        
        private float MoveSpeed => MoveSpeeds[SpeedIndex < MoveSpeeds.Length ? (SpeedIndex >= 0 ? SpeedIndex : 0) : MoveSpeeds.Length - 1];
        
        private CinemachineTrackedDolly _trackedDolly;
        private float _moveSpeed;
        private float _moveTime;

        private void Awake()
        {
            _trackedDolly = GetComponentInChildren<CinemachineTrackedDolly>();
            _trackedDolly.m_PositionUnits = CinemachinePathBase.PositionUnits.Distance;
            Assert.IsNotNull(_trackedDolly);
        }

        private void Update()
        {
            if(!_trackedDolly || !_trackedDolly.m_Path || !_trackedDolly.VirtualCamera.IsValid || !CinemachineCore.Instance.IsLive(_trackedDolly.VirtualCamera))
                return;
            var deltaTime = Time.deltaTime;
            var axisForw = Input.GetAxis(MoveForwardInputAxis);
            var maxSpeed = Mathf.Lerp(MoveSpeed, MoveSpeed * RunFactor, Input.GetAxis(RunTrigger));
            UpdateSpeed(ref _moveSpeed, axisForw, maxSpeed, MoveAccelTime, MoveDecelTime, deltaTime);
            var newPathPostition = Mathf.Clamp(_trackedDolly.m_PathPosition + _moveSpeed * deltaTime, 0, _trackedDolly.m_Path.PathLength);
            _trackedDolly.m_PathPosition = newPathPostition;
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
