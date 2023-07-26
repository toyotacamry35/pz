using Cinemachine.Utility;
using Core.Environment.Logging.Extension;
using NLog;
using SharedCode.Entities.Engine;
using Src.Aspects.Doings;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets.Src.Camera
{
    /// <summary>
    /// Штука, которая задаёт направление "вперёд" для камеры и движения персонажа
    /// </summary>
    internal class CameraGuideController
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        
        private static readonly Vector3 VectorUp = Vector3.up; // (пока) корректно работает исключительно с Vector3.up
        
        private readonly ISettings _settings;
        private float _yaw;
        private float _pitch;
        private Quaternion _orientation;
        private float _minPitch => -_settings.MaxPitchDown;
        private float _maxPitch => _settings.MaxPitchUp;

        public float Yaw { get { return _yaw; } set { _orientation = CalcOrientation(_yaw = value, _pitch); } }

        public float Pitch { get { return _pitch; } set { _orientation = CalcOrientation(_yaw, _pitch = value); } }

        public Vector3 Guide { get { return _orientation * Vector3.forward; } set { FromDirection(value); } }

        public Quaternion Orientation => _orientation;

        public CameraGuideController(ISettings settings)
        {
            Assert.IsNotNull(settings);
            _settings = settings;
        }

        public void Update(Vector2 inputAxes)
        {
            var yawDelta = inputAxes.x * _settings.YawSensitivity;
            _yaw = UpdateYaw(_yaw, yawDelta);
            var pitchDelta = inputAxes.y * _settings.PitchSensitivity;
            _pitch = UpdatePitch(_pitch, pitchDelta);
            _orientation = CalcOrientation(_yaw, _pitch);

            if (Logger.IsTraceEnabled)
            {
                var guide = Guide;
                var forward = new Vector2(guide.x, guide.z).normalized;
                Logger.IfTrace()?.Message($"Update | Frame:{Time.frameCount} yawDelta:{yawDelta} Guide:({forward.x:F4}, {forward.y:F4})").Write();
            }
        }

        private void FromDirection(Vector3 direction)
        {
            _yaw = ClampYaw(Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg);
            _pitch = !direction.AlmostZero() ? ClampPitch(Mathf.Asin(direction.y / direction.magnitude) * Mathf.Rad2Deg) : 0;
            _orientation = CalcOrientation(_yaw, _pitch);
        }
        
        private static Quaternion CalcOrientation(float yaw, float pitch)
        {
            var yawRot = Quaternion.AngleAxis(yaw, VectorUp);
            var pitchRot = Quaternion.AngleAxis(pitch, Vector3.Cross(Vector3.forward, VectorUp).normalized);
            var orientation = yawRot * pitchRot;
            return orientation;
        }

        private float UpdateYaw(float yaw, float delta) => ClampYaw(Mathf.MoveTowardsAngle(yaw, yaw + delta, float.MaxValue));

        private float UpdatePitch(float pitch, float delta) => ClampPitch(pitch + delta);

        private float ClampYaw(float yaw) => Mathf.Repeat(yaw, 360);

        private float ClampPitch(float pitch) => Mathf.Clamp(pitch, _minPitch, _maxPitch);
        
        public interface ISettings
        {
            float MaxPitchDown { get; }
            float MaxPitchUp { get; }
            float YawSensitivity { get; }
            float PitchSensitivity { get; }
        }
    }
}
