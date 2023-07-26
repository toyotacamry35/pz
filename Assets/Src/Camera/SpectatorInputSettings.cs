using UnityEngine;

namespace Assets.Src.Camera
{
    [CreateAssetMenu(menuName = "Scriptables/SpectatorInputSettings")]
    public class SpectatorInputSettings : ScriptableObject
    {
        [SerializeField] private string _yawAxis = "Mouse X";
        [SerializeField] private float _yawSensitivity = 1;
        [SerializeField] private string _pitchAxis = "Mouse Y";
        [SerializeField] private float _pitchSensitivity = 1;
        [SerializeField] public string _moveForwardAxis = "FlyCameraMoveAxis";
        [SerializeField] private float _moveForwardSensitivity = 1;
        [SerializeField] public string _moveSideAxis = "FlyCameraStrafeAxis";
        [SerializeField] private float _moveSideSensitivity = 1;
        [SerializeField] public string _moveUpAxis = "FlyCameraUpAxis";
        [SerializeField] private float _moveUpSensitivity = 1;
        [SerializeField] public string _runTrigger = "FlyCameraRun";

        public string YawAxis => _yawAxis;
        public float YawSensitivity => _yawSensitivity;
        public string PitchAxis => _pitchAxis;
        public float PitchSensitivity => _pitchSensitivity;
        public string MoveForwardInputAxis => _moveForwardAxis;
        public float MoveForwardSensitivity => _moveForwardSensitivity;
        public string MoveSideInputAxis => _moveSideAxis;
        public float MoveSideSensitivity => _moveSideSensitivity;
        public string MoveUpInputAxis => _moveUpAxis;
        public float MoveUpSensitivity => _moveUpSensitivity;
        public string RunTrigger => _runTrigger;
    }
}