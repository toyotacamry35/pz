using UnityEngine;

namespace Assets.Src.Camera
{
    public class PlayerCameraRigSpawner : MonoBehaviour
    {
        [SerializeField] private PlayerCameraRig CameraRigPrefab;

        public PlayerCameraRig SpawnCameraRig()
        {
            var cineMachine = Instantiate(CameraRigPrefab);
            return cineMachine;
        }

        public void ReleaseCameraRig(PlayerCameraRig playerCameraRig)
        {
            if (playerCameraRig != default(PlayerCameraRig))
                DestroyImmediate(playerCameraRig.gameObject);
        }
    }
}
