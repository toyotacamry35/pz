using Assets.Src.Camera;

namespace Src.Aspects.Doings
{
    // --- Util types: -----------------------------------------------------------------------------------
    // TODO: в дальнейшем настройки сенсы можно/нужно будет брать из пользовательского конфига
    class CameraGuideControllerSettings : CameraGuideController.ISettings
    {
        private readonly PlayerCameraRig _cameraRid;
        
        public CameraGuideControllerSettings(PlayerCameraRig cameraRig = null)
        {
            _cameraRid = cameraRig;
        }
        public float MaxPitchDown => _cameraRid?.ActiveCamera?.MaxPitchDown ?? 70;
        public float MaxPitchUp => _cameraRid?.ActiveCamera?.MaxPitchUp ?? 40;
        public float YawSensitivity => _cameraRid?.ActiveCamera?.YawSensitivity ?? 100;
        public float PitchSensitivity => _cameraRid?.ActiveCamera?.PitchSensitivity ?? 2;
    }
}