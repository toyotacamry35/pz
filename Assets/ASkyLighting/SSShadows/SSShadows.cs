using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Assets.ASkyLighting.SSShadows
{
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("Image Effects/Screen Space Shadows")]
    [ExecuteInEditMode()]
    public class SSShadows : MonoBehaviour
    {
        CommandBuffer blendShadowsCommandBuffer;
        CommandBuffer renderShadowsCommandBuffer;
        Camera attachedCamera;

        public Light sun;
        Material material;
        object initChecker;
        bool sunInitialized = false;
        int temporalJitterCounter = 0;

        [Range(0.0f, 1.0f)]
        public float blendStrength = 1.0f;

        [Range(0.0f, 1.0f)]
        public float accumulation = 0.9f;

        [Range(0.1f, 2.0f)]
        public float lengthFade = 0.7f;

        [Range(0.01f, 5.0f)]
        public float range = 0.7f;

        [Range(0.0f, 1.0f)]
        public float zThickness = 0.1f;

        [Range(2, 92)]
        public int samples = 32;

        [Range(0.5f, 4.0f)]
        public float nearSampleQuality = 1.5f;

        [Range(0.0f, 1.0f)]
        public float traceBias = 0.03f;

        public bool stochasticSampling = true;
        public bool leverageTemporalAA = false;
        public bool bilateralBlur = true;

        [Range(1, 2)]
        public int blurPasses = 1;

        [Range(0.01f, 0.5f)]
        public float blurDepthTolerance = 0.1f;

        bool previousBilateralBlurSetting = false;
        int previousBlurPassesSetting = 1;

        Texture2D noBlendTex;

        void AddCommandBufferClean(Light light, CommandBuffer commandBuffer, LightEvent lightEvent)
        {
            bool alreadyExists = false;
            CommandBuffer[] attachedCommandBuffers = light.GetCommandBuffers(lightEvent);
            foreach (CommandBuffer cb in attachedCommandBuffers)
            {
                if (cb.name == commandBuffer.name)
                    alreadyExists = true;
            }

            if (!alreadyExists)
                light.AddCommandBuffer(lightEvent, commandBuffer);
        }

        void AddCommandBufferClean(Camera camera, CommandBuffer commandBuffer, CameraEvent cameraEvent)
        {
            bool alreadyExists = false;
            CommandBuffer[] attachedCommandBuffers = camera.GetCommandBuffers(cameraEvent);
            foreach (CommandBuffer cb in attachedCommandBuffers)
            {
                if (cb.name == commandBuffer.name)
                    alreadyExists = true;
            }

            if (!alreadyExists)
                camera.AddCommandBuffer(cameraEvent, commandBuffer);
        }

        void RemoveCommandBuffer(Light light, CommandBuffer commandBuffer, LightEvent lightEvent)
        {
            CommandBuffer[] attachedCommandBuffers = light.GetCommandBuffers(lightEvent);
            List<CommandBuffer> commandBuffersToAdd = new List<CommandBuffer>();
            foreach (CommandBuffer cb in attachedCommandBuffers)
            {
                if (cb.name != commandBuffer.name)
                    commandBuffersToAdd.Add(cb);
            }

            light.RemoveCommandBuffers(lightEvent);
            foreach (CommandBuffer cb in commandBuffersToAdd)
            {
                light.AddCommandBuffer(lightEvent, cb);
            }
        }

        void RemoveCommandBuffer(Camera camera, CommandBuffer commandBuffer, CameraEvent cameraEvent)
        {
            CommandBuffer[] attachedCommandBuffers = camera.GetCommandBuffers(cameraEvent);
            List<CommandBuffer> commandBuffersToAdd = new List<CommandBuffer>();
            foreach (CommandBuffer cb in attachedCommandBuffers)
            {
                if (cb.name != commandBuffer.name)
                    commandBuffersToAdd.Add(cb);
            }

            camera.RemoveCommandBuffers(cameraEvent);
            foreach (CommandBuffer cb in commandBuffersToAdd)
            {
                camera.AddCommandBuffer(cameraEvent, cb);
            }
        }

        void RemoveCommandBuffers()
        {
            if (attachedCamera != null && renderShadowsCommandBuffer != null)
                RemoveCommandBuffer(attachedCamera, renderShadowsCommandBuffer, CameraEvent.BeforeLighting);
            if (sun != null && blendShadowsCommandBuffer != null)
                RemoveCommandBuffer(sun, blendShadowsCommandBuffer, LightEvent.AfterScreenspaceMask);
        }

        public bool GetCompatibleRenderPath()
        {
            if (attachedCamera)
                return attachedCamera.actualRenderingPath == RenderingPath.DeferredShading;
            else
            {
                Camera c = GetComponent<Camera>();
                return c.actualRenderingPath == RenderingPath.DeferredShading;
            }
        }

        void Init()
        {
            if (!SSShadowsControl.SSShadowsEnabled)
                return;

            if (TOD.ASkyLighting._instance != null)
                sun = TOD.ASkyLighting._instance.GetDirectionalLight();

            if (initChecker != null)
                return;


            if (!attachedCamera)
            {
                attachedCamera = GetComponent<Camera>();
                attachedCamera.depthTextureMode |= DepthTextureMode.Depth;
            }

            if (!GetCompatibleRenderPath())
            {
                initChecker = null;
                return;
            }


            material = new Material(Shader.Find("Hidden/SSShadows"));
            sunInitialized = false;


            blendShadowsCommandBuffer = new CommandBuffer {name = "SSShadows: Blend"};
            blendShadowsCommandBuffer.Blit(BuiltinRenderTextureType.None, BuiltinRenderTextureType.CurrentActive, material, 0);


            renderShadowsCommandBuffer = new CommandBuffer {name = "SSShadows: Render"};

            int ssBuf0 = Shader.PropertyToID("SSShadowBuffer0");
            int ssBuf1 = Shader.PropertyToID("SSShadowBuffer1");
            int depthSource = Shader.PropertyToID("DepthSource");
            renderShadowsCommandBuffer.GetTemporaryRT(ssBuf0, -1, -1, 0, FilterMode.Bilinear, RenderTextureFormat.R8);
            if (bilateralBlur)
                renderShadowsCommandBuffer.GetTemporaryRT(ssBuf1, -1, -1, 0, FilterMode.Bilinear, RenderTextureFormat.R8);
            renderShadowsCommandBuffer.GetTemporaryRT(depthSource, -1, -1, 0, FilterMode.Point, RenderTextureFormat.RFloat);


            renderShadowsCommandBuffer.Blit(ssBuf0, depthSource, material, 2);
            renderShadowsCommandBuffer.Blit(depthSource, ssBuf0, material, 1);
            if (bilateralBlur)
            {
                for (int i = 0; i < blurPasses; i++)
                {
                    renderShadowsCommandBuffer.SetGlobalVector("SSSBlurKernel", new Vector2(0.0f, 1.0f));
                    renderShadowsCommandBuffer.Blit(ssBuf0, ssBuf1, material, 3);
                    renderShadowsCommandBuffer.SetGlobalVector("SSSBlurKernel", new Vector2(1.0f, 0.0f));
                    renderShadowsCommandBuffer.Blit(ssBuf1, ssBuf0, material, 3);
                }
            }

            renderShadowsCommandBuffer.SetGlobalTexture("SSShadowsTexture", ssBuf0);

            AddCommandBufferClean(attachedCamera, renderShadowsCommandBuffer, CameraEvent.BeforeLighting);

            noBlendTex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            noBlendTex.SetPixel(0, 0, Color.white);
            noBlendTex.Apply();

            initChecker = new object();
        }

        void OnEnable()
        {
            Init();
        }

        void OnDisable()
        {
            RemoveCommandBuffers();
            initChecker = null;
        }

        void OnPreRender()
        {
            Init();



            if (initChecker == null)
                return;

            if (sun != null && !sunInitialized)
            {
                AddCommandBufferClean(sun, blendShadowsCommandBuffer, LightEvent.AfterScreenspaceMask);
                sunInitialized = true;
            }

            if (leverageTemporalAA)
                temporalJitterCounter = (temporalJitterCounter + 1) % 8;

            if (previousBilateralBlurSetting != bilateralBlur || previousBlurPassesSetting != blurPasses)
            {
                RemoveCommandBuffers();
                initChecker = null;
                Init();
            }

            previousBilateralBlurSetting = bilateralBlur;
            previousBlurPassesSetting = blurPasses;

            if (!sunInitialized)
                return;



            material.SetMatrix("ProjectionMatrix", attachedCamera.projectionMatrix);
            material.SetMatrix("ProjectionMatrixInverse", attachedCamera.projectionMatrix.inverse);
            material.SetVector("SunlightVector", transform.InverseTransformDirection(sun.transform.forward));
            material.SetVector("ScreenRes", new Vector4(attachedCamera.pixelWidth, attachedCamera.pixelHeight, 1.0f / attachedCamera.pixelWidth, 1.0f / attachedCamera.pixelHeight));

            material.SetFloat("BlendStrength", blendStrength);
            material.SetFloat("Accumulation", accumulation);
            material.SetFloat("Range", range);
            material.SetFloat("ZThickness", zThickness);
            material.SetInt("Samples", samples);
            material.SetFloat("NearQualityCutoff", 1.0f / nearSampleQuality);
            material.SetFloat("TraceBias", traceBias);
            material.SetFloat("StochasticSampling", stochasticSampling ? 1.0f : 0.0f);
            material.SetInt("TJitter", temporalJitterCounter);
            material.SetFloat("LengthFade", lengthFade);
            material.SetFloat("BlurDepthTolerance", blurDepthTolerance);
        }

        void OnPostRender()
        {
            Shader.SetGlobalTexture("SSShadowsTexture", noBlendTex);
        }
    }
}