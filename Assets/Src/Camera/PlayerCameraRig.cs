using SharedCode.Entities.Engine;
using Src.Aspects.Doings;
using Src.Camera;
using UnityEngine;

namespace Assets.Src.Camera
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [AddComponentMenu("Cinemachine/Player Camera Rig")]
    public class PlayerCameraRig : BaseCameraRig
    {
        private IGuideProvider _cameraGuideProvider;

        public void SetGuideProvider(IGuideProvider cameraGuideProvider)
        {
             _cameraGuideProvider = cameraGuideProvider;
             SetGuideProviderToChildren(_cameraGuideProvider);
        }

        protected override void SetupExtra()
        {
            SetGuideProviderToChildren(_cameraGuideProvider);
        }

        private void SetGuideProviderToChildren(IGuideProvider cameraGuideProvider)
        {
            foreach (var element in Cameras)
            {
                var aimingCamera = element.Camera as ICameraWithAiming;
                aimingCamera?.SetGuideProvider(cameraGuideProvider);
            }
        }
    }
}
