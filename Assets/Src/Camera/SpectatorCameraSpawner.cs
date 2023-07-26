using Assets.Src.Lib.Cheats;
using Src.Input;
using Uins;
using Uins.Sound;
using UnityEngine;
using static Uins.Sound.HudBlocksVisibility;

namespace Assets.Src.Camera
{
    public class SpectatorCameraSpawner : MonoBehaviour
    {
        public SpectatorCameraController FlyCameraPrefab;
        public Vector3 Offset = new Vector3(0,0.5f,-0.5f);
        public KeyCode SpawnKey = KeyCode.F5;
        public HudBlocksVisibility HideHud = BottomSlots | ChatBlock | EnvironmentBlock | FactionBlock | HelpBlock | HpBlock | NavigationBlock;
        
        private SpectatorCameraController _cameraController;

        private bool IsFlyCameraActive { get; set; }
        
        private void Update()
        {
            if (!ClientCheatsState.Spectator && !IsFlyCameraActive)
                return;

            var modifiers = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ||
                            Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) ||
                            Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);

            if (!modifiers && Input.GetKeyDown(SpawnKey))
            {
                if(IsFlyCameraActive)
                    DestroyFlyCamera();
                else
                    SpawnFlyCamera();
            }
        }
        
        private void SpawnFlyCamera()
        {
            IsFlyCameraActive = true;
            var mainCameraTransform = GameCamera.Camera.transform;
            if(_cameraController)
                DestroyImmediate(_cameraController);
            _cameraController = Instantiate(FlyCameraPrefab);
            _cameraController.gameObject.SetActive(true);
            _cameraController.Position = mainCameraTransform.position + Offset;
            _cameraController.Rotation = new Vector2(0, mainCameraTransform.rotation.eulerAngles.y);
            _cameraController.name = FlyCameraPrefab.name; // бленды в CinemachineBrain ссылаются на камеры по именам
            InputManager.Instance.PushBindings(this, UI.BlockedActionsMovementAndCamera);
            HudVisibility.Instance.VisibleOff(HideHud);
        }

        private void DestroyFlyCamera()
        {
            if(_cameraController)
                DestroyImmediate(_cameraController.gameObject);
            _cameraController = null;
            IsFlyCameraActive = false;
            InputManager.Instance.PopBindings(this);
            HudVisibility.Instance.VisibleOn(HideHud);
        }
    }
}
