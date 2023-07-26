using UnityEngine;

namespace Assets.Src.FX
{
    public class CustomParticleCulling : MonoBehaviour
    {
        public float cullingRadius = 1;

        private CullingGroup _cullingGroup;
        private Renderer[] _particleRenderers;
        private ParticleSystem[] _particleSystems;

        private void Awake()
        {
            _particleSystems = GetComponentsInChildren<ParticleSystem>();
            _particleRenderers = GetComponentsInChildren<Renderer>();

            _cullingGroup = new CullingGroup();
            _cullingGroup.enabled = false;
            _cullingGroup.SetBoundingSpheres(new[] { new BoundingSphere(transform.position, cullingRadius) });
            _cullingGroup.SetBoundingSphereCount(1);
            _cullingGroup.onStateChanged += OnStateChanged;

            enabled = MainCamera.Camera != null;

            MainCamera.OnCameraCreated += MainCameraNotifier_OnCameraCreated;
            MainCamera.OnCameraDestroyed += MainCameraNotifier_OnCameraDestroyed;
        }

        void OnEnable()
        {
            _cullingGroup.targetCamera = MainCamera.Camera;

            // We need to start in a culled state
            Cull(_cullingGroup.IsVisible(0));

            _cullingGroup.enabled = true;
        }

        private void MainCameraNotifier_OnCameraDestroyed()
        {
            enabled = false;
        }

        private void MainCameraNotifier_OnCameraCreated(UnityEngine.Camera camera)
        {
            enabled = true;
        }

        void OnDisable()
        {
            _cullingGroup.enabled = false;

            SetPlaying(true);

            SetRenderers(true);
        }

        void OnDestroy()
        {
            _cullingGroup.Dispose();
            MainCamera.OnCameraCreated -= MainCameraNotifier_OnCameraCreated;
            MainCamera.OnCameraDestroyed -= MainCameraNotifier_OnCameraDestroyed;
        }

        void OnStateChanged(CullingGroupEvent sphere)
        {
            Cull(sphere.isVisible);
        }

        void Cull(bool visible)
        {
            if (visible)
            {
                // We could simulate forward a little here to hide that the system was not updated off-screen.
                SetPlaying(true);
                SetRenderers(true);

            }
            else
            {
                SetPlaying(false);
                SetRenderers(false);
            }
        }

        void SetRenderers(bool enable)
        {
            // We also need to disable the renderer to prevent drawing the particles, such as when occlusion occurs.
            foreach (var particleRenderer in _particleRenderers)
            {
                particleRenderer.enabled = enable;
            }
        }

        void SetPlaying(bool enable)
        {
            if (enable)
            {
                foreach (var ps in _particleSystems)
                    ps.Play(true);
            }
            else
            {
                foreach (var ps in _particleSystems)
                    ps.Pause(true);
            }
        }

        void OnDrawGizmos()
        {
            if (enabled)
            {
                // Draw gizmos to show the culling sphere.
                Color col = Color.yellow;
                if (_cullingGroup != null && !_cullingGroup.IsVisible(0))
                    col = Color.gray;

                Gizmos.color = col;
                Gizmos.DrawWireSphere(transform.position, cullingRadius);
            }
        }
    }
}