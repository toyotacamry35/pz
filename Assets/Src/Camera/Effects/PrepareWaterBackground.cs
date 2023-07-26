using System;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

namespace Assets.Src.Camera.Effects.DeferredProjectiveDecals
{
    public class PrepareWaterBackground : MonoBehaviour
    {
        protected UnityEngine.Camera _camera;
        protected CommandBuffer _bufferDeferred;
        protected const string _bufferDeferredName = "Grab Water Background";
        protected const CameraEvent _camEventDeferred = CameraEvent.BeforeForwardAlpha;
        protected const string _waterShaderNameTexture = "_WaterBackground";
        protected int _screenCopyID;
        protected RenderTargetIdentifier _screenCopyRTI;

        void Start()
        {
            _camera = GetComponent<UnityEngine.Camera>();
            _screenCopyID = Shader.PropertyToID(_waterShaderNameTexture);
            _screenCopyRTI = new RenderTargetIdentifier(_screenCopyID);
            CreateBuffer(ref _bufferDeferred, _camera, _bufferDeferredName, _camEventDeferred);
        }

        private void OnPreRender()
        {
            _bufferDeferred.Clear();
            _bufferDeferred.GetTemporaryRT (_screenCopyID, -1, -1, 0, FilterMode.Bilinear,GraphicsFormat.R16G16B16A16_SFloat);
            _bufferDeferred.Blit (BuiltinRenderTextureType.CurrentActive, _screenCopyID);
            _bufferDeferred.SetGlobalTexture(_screenCopyID, _screenCopyRTI);
        }

        private void OnPostRender()
        {
            _bufferDeferred.ReleaseTemporaryRT(_screenCopyID);
        }

        private void OnDestroy()
        {
            if (_bufferDeferred == null) return;
            
            _camera.RemoveCommandBuffer(_camEventDeferred, _bufferDeferred);
            _bufferDeferred = null;
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
    }
}