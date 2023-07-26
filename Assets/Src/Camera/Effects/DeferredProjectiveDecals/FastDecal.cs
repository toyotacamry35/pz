using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UCamera = UnityEngine.Camera;

namespace Assets.Src.Camera.Effects.DeferredProjectiveDecals
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [ExecuteInEditMode]
    public class FastDecal : MonoBehaviour
    {
        private const string _deferredShaderName = "Custom/DecalFast";

        public enum DecalRenderMode
        {
            Deferred,
            Invalid
        }

        public DecalRenderMode RenderMode = DecalRenderMode.Invalid;

        [Tooltip("Set a Material with a Decalicious shader.")]
        public Material Material;

        [Tooltip("Should this decal be drawn early (low number) or late (high number)?")]
        public int RenderOrder = 100;

        [Tooltip("To which degree should the Decal be drawn? At 1, the Decal will be drawn with full effect. At 0, the Decal will not be drawn. Experiment with values greater than one.")]
        public float Fade = 1.0f;



        private readonly List<(UCamera Camera, FastDecalRenderer Renderer, FastDecalAgent Agent)> _agents =
            new List<(UCamera, FastDecalRenderer, FastDecalAgent)>(1);

#if UNITY_EDITOR
        private void OnValidate()
        {
            SetMeshFilter();
            SetRenderMode();
        }

        private void SetMeshFilter()
        {
            MeshFilter mf = GetComponent<MeshFilter>();
            if (mf.sharedMesh == null)
                mf.sharedMesh = AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Plugins/Decalicious/Resources/DecalCube.asset");
        }
#endif

        private void SetRenderMode()
        {
            if (Material == null)
                RenderMode = DecalRenderMode.Invalid;
            else if (Material.shader.name.StartsWith(_deferredShaderName))
                RenderMode = DecalRenderMode.Deferred;
            else
                RenderMode = DecalRenderMode.Invalid;
        }

        private void Awake()
        {
            SetRenderMode();

            MeshRenderer mr = GetComponent<MeshRenderer>();
            mr.shadowCastingMode = ShadowCastingMode.Off;
            mr.receiveShadows = false;
            mr.materials = new Material[] { };
            mr.lightProbeUsage = LightProbeUsage.BlendProbes;
            mr.reflectionProbeUsage = ReflectionProbeUsage.Off;
        }

        private void OnWillRenderObject()
        {
            if (Fade <= 0.0f)
                return;

            if (Material == null)
                return;

            var currentCamera = GetCurrentCamera();
            var (agent, renderer) = GetAgentForCamera(currentCamera);

            if (!renderer.isActiveAndEnabled)
                return;

#if UNITY_5_6_OR_NEWER
            Material.enableInstancing = renderer.UseInstancing;
#endif
            renderer.Add(agent);
        }

        private UCamera GetCurrentCamera()
        {
#if UNITY_EDITOR
            return UCamera.current;
#else
                return GameCamera.Camera;
#endif
        }

        private (FastDecalAgent Agent, FastDecalRenderer Renderer) GetAgentForCamera(UCamera camera)
        {
            foreach (var tuple in _agents)
                if (tuple.Camera == camera)
                    return (tuple.Agent, tuple.Renderer);

            var renderer = camera.GetComponent<FastDecalRenderer>();
            if (renderer == null)
                renderer = camera.gameObject.AddComponent<FastDecalRenderer>();

            var entry = (camera, renderer, Agent: new FastDecalAgent(this));
            _agents.Add(entry);
            return (entry.Agent, renderer);
        }

        private void OnDrawGizmos()
        {
            // Draw an invisible cube for selection
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.clear;
            Gizmos.DrawCube(Vector3.zero, Vector3.one);

            // Draw a faint wireframe
            Gizmos.color = Color.white * 0.2f;
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        }

        private void OnDrawGizmosSelected()
        {
            // Draw a well-visible wireframe
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.white * 0.5f;
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        }
    }
}