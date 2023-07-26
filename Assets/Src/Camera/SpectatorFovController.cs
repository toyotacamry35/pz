using Cinemachine;
using UnityEngine;

namespace  Assets.Src.Camera
{
    [SaveDuringPlay]
    public class SpectatorFovController : CinemachineExtension
    {
        public float CurrentFOV;
        [Tooltip("Default FOV. If -1, then get default FOV from controlled virtual camera")]
        public float DefaultFOV = -1;
        [Tooltip("Min FOV value")]
        public float MinFOV = 20;
        [Tooltip("Max FOV value")]
        public float MaxFOV = 120;
        [Tooltip("Speed of FOV changing")]
        public float Speed = 50f;
        [Tooltip("The name of axis for increasing/decreasing FOV as specified in Unity Input manager")]
        public string InputAxis = "FlyCameraFOV";
        [Tooltip("The name of axis for reseting FOV to default value as specified in Unity Input manager")]
        public string ResetInputKey = "FlyCameraResetFOV";
        
        protected override void Awake()
        {
            base.Awake();
            if (DefaultFOV == -1)
                DefaultFOV = VirtualCamera.State.Lens.FieldOfView;
            CurrentFOV = DefaultFOV;
        }
        
        private void Update()
        {
            CurrentFOV = Mathf.Clamp(CurrentFOV + CinemachineCore.GetInputAxis(InputAxis) * Speed * Time.deltaTime, MinFOV, MaxFOV);
            if(CinemachineCore.GetInputAxis(ResetInputKey) != 0)
                CurrentFOV = DefaultFOV;
        }
        
        protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
        {
            if (stage == CinemachineCore.Stage.Finalize)
            {
                var lens = state.Lens;
                lens.FieldOfView = CurrentFOV;
                state.Lens = lens;
            }
        }
    }
}
