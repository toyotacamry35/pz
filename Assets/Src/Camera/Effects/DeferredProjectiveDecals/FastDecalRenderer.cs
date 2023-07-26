using Assets.Src.Lib.ProfileTools;
using System.Collections.Generic;
using Core.Environment.Logging.Extension;
using UnityEngine;
using UnityEngine.Rendering;

namespace Assets.Src.Camera.Effects.DeferredProjectiveDecals
{
    [ExecuteInEditMode]
    public class FastDecalRenderer : MonoBehaviour
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
#if UNITY_5_5_OR_NEWER
        [HideInInspector]
        public bool UseInstancing = true;
#endif

        protected CommandBuffer _bufferDeferred = null;
        protected SortedList<int, Dictionary<Material, FastDecalAgent>> _deferredDecals;

        protected const string _bufferDeferredName = "Deferred Decal";
        protected const CameraEvent _camEventDeferred = CameraEvent.BeforeReflections;

        protected UnityEngine.Camera _camera;
        protected RenderTargetIdentifier[] _mrt;
        protected bool _camLastKnownHDR;
        protected static Mesh _cubeMesh = null;

        protected Matrix4x4[] _matrices;
        protected float[] _fadeValues;
        protected MaterialPropertyBlock _instancedBlock;
        protected MaterialPropertyBlock _directBlock;
        protected static Vector4[] _avCoeff = new Vector4[7];
        private static readonly int MaskMultiplier = Shader.PropertyToID("_MaskMultiplier");

        private void OnEnable()
        {
            _deferredDecals = new SortedList<int, Dictionary<Material, FastDecalAgent>>();
            _matrices = new Matrix4x4[1023];
            _fadeValues = new float[1023];
            _instancedBlock = new MaterialPropertyBlock();
            _directBlock = new MaterialPropertyBlock();
            _camera = GetComponent<UnityEngine.Camera>();
            _cubeMesh = Profile.Load<Mesh>("DecalCube");
            _mrt = new RenderTargetIdentifier[] { BuiltinRenderTextureType.GBuffer0, BuiltinRenderTextureType.GBuffer1, BuiltinRenderTextureType.GBuffer2 };
        }

        private void OnDisable()
        {
            if (_bufferDeferred != null)
            {
                GetComponent<UnityEngine.Camera>().RemoveCommandBuffer(_camEventDeferred, _bufferDeferred);
                _bufferDeferred = null;
            }
            _deferredDecals = null;
        }

        private void OnPreRender()
        {
            // Make sure that command buffers are created
            CreateBuffer(ref _bufferDeferred, _camera, _bufferDeferredName, _camEventDeferred);

            // Prepare command buffer for deferred decals
            _bufferDeferred.Clear();
            DrawDeferredDecals(_camera);

            // Clear deferred decal list for next frame
            foreach (var key in _deferredDecals.Keys)
            {
                foreach (var first in _deferredDecals[key].Values)
                {
                    var agent = first;
                    while (agent != null)
                    {
                        var curr = agent;
                        agent = agent.Next;
                        curr.Next = null;
                        curr.InList = false;
                    }
                }
                _deferredDecals[key].Clear();
            }
            /*var decalEnum = _deferredDecals.GetEnumerator();
            while (decalEnum.MoveNext())
            {
                foreach (var first in decalEnum.Current.Value.Values)
                {
                    var agent = first;
                    while (agent != null)
                    {
                        var curr = agent;
                        agent = agent.Next;
                        curr.Next = null;
                        curr.InList = false;
                    }
                }
                decalEnum.Current.Value.Clear();
            }
            decalEnum.Dispose();*/
        }

        private void DrawDeferredDecals(UnityEngine.Camera cam)
        {
            if (_deferredDecals.Count == 0)
                return;

            _bufferDeferred.SetRenderTarget(_mrt, BuiltinRenderTextureType.CameraTarget);

            // Traverse over decal render order values
            foreach (var allRenderOrderEnum in _deferredDecals)
            {
                var allMaterialEnum = allRenderOrderEnum.Value;
                foreach (var materialToDecals in allMaterialEnum)
                {
                    Material material = materialToDecals.Key;
                    int n = 0;

                    for (FastDecalAgent decalAgent = materialToDecals.Value; decalAgent != null; decalAgent = decalAgent.Next)
                    {
                        var decal = decalAgent.Decal; 
                        if (decal)
                        {
#if UNITY_5_5_OR_NEWER
                            if (UseInstancing)
                            {
                                _matrices[n] = decal.transform.localToWorldMatrix;
                                _fadeValues[n] = decal.Fade;
                                ++n;

                                if (n == 1023)
                                {
                                    _instancedBlock.Clear();
                                    _instancedBlock.SetFloatArray(MaskMultiplier, _fadeValues);
                                    _bufferDeferred.DrawMeshInstanced(_cubeMesh, 0, material, 0, _matrices, n, _instancedBlock);
                                    n = 0;
                                }
                            }
                            else
#endif
                            {
                                // Fall back to non-instanced rendering
                                _directBlock.Clear();
                                _directBlock.SetFloat(MaskMultiplier, decal.Fade);
                                _bufferDeferred.DrawMesh(_cubeMesh, decal.transform.localToWorldMatrix, material, 0, 0, _directBlock);
                            }
                        }
                    }

#if UNITY_5_5_OR_NEWER
                    if (UseInstancing && n > 0)
                    {
                        _instancedBlock.Clear();
                        _instancedBlock.SetFloatArray(MaskMultiplier, _fadeValues);
                        _bufferDeferred.DrawMeshInstanced(_cubeMesh, 0, material, 0, _matrices, n, _instancedBlock);
                    }
#endif
                }
            }
        }

        private static void CreateBuffer(ref CommandBuffer buffer, UnityEngine.Camera cam, string name, CameraEvent evt)
        {
            if (buffer == null)
            {
                // See if the camera already has a command buffer to avoid duplicates
                foreach (CommandBuffer existingCommandBuffer in cam.GetCommandBuffers(evt))
                {
                    if (existingCommandBuffer.name == name)
                    {
                        buffer = existingCommandBuffer;
                        break;
                    }
                }

                // Not found? Create a new command buffer
                if (buffer == null)
                {
                    buffer = new CommandBuffer();
                    buffer.name = name;
                    cam.AddCommandBuffer(evt, buffer);
                }
            }
        }

        public void Add(FastDecalAgent agent)
        {
            switch (agent.Decal.RenderMode)
            {
                case FastDecal.DecalRenderMode.Deferred:
                    AddDeferred(agent);
                    break;
                default:
                    break;
            }
        }

        protected void AddDeferred(FastDecalAgent agent)
        {
            var decal = agent.Decal;
            if (agent.InList)
            {
                if (Logger.IsWarnEnabled) Logger.IfWarn()?.Message("Decal already in list! | Decal:{0} Material:{1}", decal.name, decal.Material.name).Write();
                return;
            }
            if (!_deferredDecals.TryGetValue(decal.RenderOrder, out var dict))
                _deferredDecals.Add(decal.RenderOrder, dict = new Dictionary<Material, FastDecalAgent>());
            dict.TryGetValue(decal.Material, out var first);
            dict[decal.Material] = agent;
            agent.Next = first;
            agent.InList = true;
        }
    }
}
