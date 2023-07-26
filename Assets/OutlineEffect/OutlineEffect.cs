/*
//  Copyright (c) 2015 José Guerreiro. All rights reserved.
//
//  MIT license, see http://www.opensource.org/licenses/mit-license.php
//  
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//  
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
//  
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
*/

using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
using TOD;

namespace OutlineEffect
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    [ExecuteInEditMode]
    public class OutlineEffect : MonoBehaviour
    {
        private static OutlineEffect m_instance;
        public static OutlineEffect Instance
        {
            get
            {
                if (Equals(m_instance, null))
                {
                    return m_instance = FindObjectOfType(typeof(OutlineEffect)) as OutlineEffect;
                }

                return m_instance;
            }
        }
        private OutlineEffect() { }

        private readonly LinkedSet<Outline> outlines = new LinkedSet<Outline>();

        [HideInInspector]
        public FloatCurve lineThickness;
        [HideInInspector]
        public FloatCurve lineIntensity;
        [HideInInspector]
        public FloatCurve fillAmount;
        //public Texture2D waves02;
        //public Vector4 _TestVector;
        [HideInInspector]
        public float CGTIME = 0;
        public Color lineColor0 = Color.red;
        public Color lineColor1 = Color.green;
        public Color lineColor2 = Color.blue;

        public bool additiveRendering = false;

        public bool backfaceCulling = true;

        [Header("These settings can affect performance!")]
        public bool cornerOutlines = false;
        public bool addLinesBetweenColors = false;

        [Header("Advanced settings")]
        public bool scaleWithScreenSize = true;
        [Range(0.1f, .9f)]
        public float alphaCutoff = .5f;
        public bool flipY = false;
        public Camera sourceCamera;

        [HideInInspector]
        public Camera outlineCamera;
        Material outline1Material;
        Material outline2Material;
        Material outline3Material;
        Material outlineEraseMaterial;
        [SerializeField]
        Shader outlineShader;
        [SerializeField]
        Shader outlineBufferShader;
        [HideInInspector]
        public Material outlineShaderMaterial;
        [HideInInspector]
        public RenderTexture renderTexture;
        [HideInInspector]
        public RenderTexture extraRenderTexture;

        CommandBuffer commandBuffer;

        Material GetMaterialFromID(OutlineColor outlineColor)
        {
            if (outlineColor == OutlineColor.Blue)
                return outline1Material;
            else if (outlineColor == OutlineColor.Yellow)
                return outline2Material;
            else if (outlineColor == OutlineColor.Red)
                return outline3Material;
            else
                return outline1Material;
        }
        List<Material> materialBuffer = new List<Material>();
        Material CreateMaterial(Color emissionColor)
        {
            Material m = new Material(outlineBufferShader);
            m.SetColor("_Color", emissionColor);
            m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            m.SetInt("_ZWrite", 0);
            m.DisableKeyword("_ALPHATEST_ON");
            m.EnableKeyword("_ALPHABLEND_ON");
            m.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            m.renderQueue = 3000;
            return m;
        }

        private void Awake()
        {
            m_instance = this;
        }

        void Start()
        {
            CreateMaterialsIfNeeded();
            UpdateMaterialsPublicProperties();

            if (sourceCamera == null)
            {
                sourceCamera = GetComponent<Camera>();

                if (sourceCamera == null)
                    sourceCamera = Camera.main;
            }

            if (outlineCamera == null)
            {
                GameObject cameraGameObject = new GameObject("Outline Camera");
                cameraGameObject.transform.parent = sourceCamera.transform;
                outlineCamera = cameraGameObject.AddComponent<Camera>();
                outlineCamera.enabled = false;
            }

            renderTexture = new RenderTexture(sourceCamera.pixelWidth, sourceCamera.pixelHeight, 16, RenderTextureFormat.Default);
            extraRenderTexture = new RenderTexture(sourceCamera.pixelWidth, sourceCamera.pixelHeight, 16, RenderTextureFormat.Default);
            UpdateOutlineCameraFromSource();
            commandBuffer = new CommandBuffer();
            
            outlineCamera.AddCommandBuffer(CameraEvent.BeforeImageEffects, commandBuffer);
        }

        public void OnPreRender()
        {
            if (commandBuffer == null)
                return;

            CreateMaterialsIfNeeded();

            if (renderTexture == null || renderTexture.width != sourceCamera.pixelWidth || renderTexture.height != sourceCamera.pixelHeight)
            {
                renderTexture = new RenderTexture(sourceCamera.pixelWidth, sourceCamera.pixelHeight, 16, RenderTextureFormat.Default);
                extraRenderTexture = new RenderTexture(sourceCamera.pixelWidth, sourceCamera.pixelHeight, 16, RenderTextureFormat.Default);
                outlineCamera.targetTexture = renderTexture;
            }
            UpdateMaterialsPublicProperties();
            UpdateOutlineCameraFromSource();
            outlineCamera.targetTexture = renderTexture;

            commandBuffer.Clear();
            commandBuffer.SetRenderTarget(renderTexture);
            commandBuffer.ClearRenderTarget(false, true, Color.clear);

            
            if (outlines != null)
            {
                foreach (Outline outline in outlines)
                {
                    if (!outline.isActiveAndEnabled || !outline.Renderer.enabled)
                        continue;
                    LayerMask l = sourceCamera.cullingMask;
                    if (outline != null && l == (l | (1 << outline.gameObject.layer)))
                    {
                        var materials = outline.Renderer.sharedMaterials;
                        for (int v = 0; v < materials.Length; v++)
                        {
                            Material m = null;
                            if (materials[v] && materials[v].mainTexture != null)
                            {

                                var materialCount = materialBuffer.Count;
                                for (int materialIndex = 0; materialIndex < materialCount; ++materialIndex)
                                {
                                    var g = materialBuffer[materialIndex];
                                    if (g.mainTexture == materials[v].mainTexture)
                                    {
                                        if (outline.eraseRenderer && g.color == outlineEraseMaterial.color)
                                        {
                                            m = g;
                                            break;
                                        }
                                        
                                        if (g.color == GetMaterialFromID(outline.Color).color)
                                        {
                                            m = g;
                                            break;
                                        }
                                    }
                                }

                                if (m == null)
                                {
                                    if (outline.eraseRenderer)
                                        m = new Material(outlineEraseMaterial);
                                    else
                                        m = new Material(GetMaterialFromID(outline.Color));
                                    m.mainTexture = materials[v].mainTexture;
                                    materialBuffer.Add(m);
                                }
                            }
                            else
                            {
                                if (outline.eraseRenderer)
                                    m = outlineEraseMaterial;
                                else
                                    m = GetMaterialFromID(outline.Color);
                            }
                            if (m == null)
                                continue;

                            if (backfaceCulling)
                                m.SetInt("_Culling", (int)UnityEngine.Rendering.CullMode.Back);
                            else
                                m.SetInt("_Culling", (int)UnityEngine.Rendering.CullMode.Off);

                            commandBuffer.DrawRenderer(outline.GetComponent<Renderer>(), m, 0, 0);
                            MeshFilter mL = outline.GetComponent<MeshFilter>();
                            if (mL)
                            {
                                if (mL.sharedMesh != null)
                                {
                                    for (int i = 1; i < mL.sharedMesh.subMeshCount; i++)
                                        commandBuffer.DrawRenderer(outline.GetComponent<Renderer>(), m, i, 0);
                                }
                            }
                            SkinnedMeshRenderer sMR = outline.GetComponent<SkinnedMeshRenderer>();
                            if (sMR)
                            {
                                if (sMR.sharedMesh != null)
                                {
                                    for (int i = 1; i < sMR.sharedMesh.subMeshCount; i++)
                                        commandBuffer.DrawRenderer(outline.GetComponent<Renderer>(), m, i, 0);
                                }
                            }
                        }
                    }
                }
            }

            outlineCamera.Render();
        }

        private void OnEnable()
        {
            Outline[] o = FindObjectsOfType<Outline>();

            if (lineThickness.curve == null) lineThickness = new FloatCurve();
            if (lineIntensity.curve == null) lineIntensity = new FloatCurve();
            if (fillAmount.curve == null) fillAmount = new FloatCurve();

            foreach (Outline oL in o)
            {
                oL.enabled = false;
                oL.enabled = true;
            }
        }

        private void Reset()
        {
            if (lineThickness.curve == null) lineThickness = new FloatCurve();
            if (lineIntensity.curve == null) lineIntensity = new FloatCurve();
            if (fillAmount.curve == null) fillAmount = new FloatCurve();
        }

        void OnDestroy()
        {
            if (renderTexture != null)
                renderTexture.Release();
            if (extraRenderTexture != null)
                extraRenderTexture.Release();
            DestroyMaterials();
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            outlineShaderMaterial.SetTexture("_OutlineSource", renderTexture);

            if (addLinesBetweenColors)
            {
                Graphics.Blit(source, extraRenderTexture, outlineShaderMaterial, 0);
                outlineShaderMaterial.SetTexture("_OutlineSource", extraRenderTexture);
            }
            Graphics.Blit(source, destination, outlineShaderMaterial, 1);
        }

#if UNITY_EDITOR
        public void OnValidate()
        {
            if (outlineShader == null)
                outlineShader = UnityEditor.AssetDatabase.LoadAssetAtPath<Shader>("Assets/OutlineEffect/_Resources/OutlineShader.shader");
            if (outlineBufferShader == null)
                outlineBufferShader = UnityEditor.AssetDatabase.LoadAssetAtPath<Shader>("Assets/OutlineEffect/_Resources/OutlineBufferShader.shader");
        }
#endif

        private void CreateMaterialsIfNeeded()
        {
            if (outlineShaderMaterial == null)
            {
                outlineShaderMaterial = new Material(outlineShader);
                outlineShaderMaterial.hideFlags = HideFlags.HideAndDontSave;
                UpdateMaterialsPublicProperties();
            }
            if (outlineEraseMaterial == null)
                outlineEraseMaterial = CreateMaterial(new Color(0, 0, 0, 0));
            if (outline1Material == null)
                outline1Material = CreateMaterial(new Color(1, 0, 0, 0));
            if (outline2Material == null)
                outline2Material = CreateMaterial(new Color(0, 1, 0, 0));
            if (outline3Material == null)
                outline3Material = CreateMaterial(new Color(0, 0, 1, 0));
        }

        private void DestroyMaterials()
        {
            foreach (Material m in materialBuffer)
                DestroyImmediate(m);
            materialBuffer.Clear();
            DestroyImmediate(outlineShaderMaterial);
            DestroyImmediate(outlineEraseMaterial);
            DestroyImmediate(outline1Material);
            DestroyImmediate(outline2Material);
            DestroyImmediate(outline3Material);
            outlineShaderMaterial = null;
            outlineEraseMaterial = null;
            outline1Material = null;
            outline2Material = null;
            outline3Material = null;
        }

        public void UpdateMaterialsPublicProperties()
        {

            if (TOD.ASkyLighting._instance != null)
                CGTIME = TOD.ASkyLighting.CGTime;

            lineThickness.Evaluate(CGTIME);
            lineIntensity.Evaluate(CGTIME);
            fillAmount.Evaluate(CGTIME);


            if (outlineShaderMaterial)
            {
                float scalingFactor = 1;
                if (scaleWithScreenSize)
                {
                    // If Screen.height gets bigger, outlines gets thicker
                    scalingFactor = Screen.height / 360.0f;
                }

                // If scaling is too small (height less than 360 pixels), make sure you still render the outlines, but render them with 1 thickness
                if (scaleWithScreenSize && scalingFactor < 1)
                {
                    if (UnityEngine.XR.XRSettings.isDeviceActive && sourceCamera.stereoTargetEye != StereoTargetEyeMask.None)
                    {
                        outlineShaderMaterial.SetFloat("_LineThicknessX", (1 / 1000.0f) * (1.0f / UnityEngine.XR.XRSettings.eyeTextureWidth) * 1000.0f);
                        outlineShaderMaterial.SetFloat("_LineThicknessY", (1 / 1000.0f) * (1.0f / UnityEngine.XR.XRSettings.eyeTextureHeight) * 1000.0f);
                    }
                    else
                    {
                        outlineShaderMaterial.SetFloat("_LineThicknessX", (1 / 1000.0f) * (1.0f / Screen.width) * 1000.0f);
                        outlineShaderMaterial.SetFloat("_LineThicknessY", (1 / 1000.0f) * (1.0f / Screen.height) * 1000.0f);
                    }
                }
                else
                {
                    if (UnityEngine.XR.XRSettings.isDeviceActive && sourceCamera.stereoTargetEye != StereoTargetEyeMask.None)
                    {
                        outlineShaderMaterial.SetFloat("_LineThicknessX", scalingFactor * (lineThickness.value / 1000.0f) * (1.0f / UnityEngine.XR.XRSettings.eyeTextureWidth) * 1000.0f);
                        outlineShaderMaterial.SetFloat("_LineThicknessY", scalingFactor * (lineThickness.value / 1000.0f) * (1.0f / UnityEngine.XR.XRSettings.eyeTextureHeight) * 1000.0f);
                    }
                    else
                    {
                        outlineShaderMaterial.SetFloat("_LineThicknessX", scalingFactor * (lineThickness.value / 1000.0f) * (1.0f / Screen.width) * 1000.0f);
                        outlineShaderMaterial.SetFloat("_LineThicknessY", scalingFactor * (lineThickness.value / 1000.0f) * (1.0f / Screen.height) * 1000.0f);
                    }
                }
                outlineShaderMaterial.SetFloat("_LineIntensity", lineIntensity.value);
                outlineShaderMaterial.SetFloat("_FillAmount", fillAmount.value);
                outlineShaderMaterial.SetColor("_LineColor1", lineColor0 * lineColor0);
                outlineShaderMaterial.SetColor("_LineColor2", lineColor1 * lineColor1);
                outlineShaderMaterial.SetColor("_LineColor3", lineColor2 * lineColor2);
                if (flipY)
                    outlineShaderMaterial.SetInt("_FlipY", 1);
                else
                    outlineShaderMaterial.SetInt("_FlipY", 0);
                if (!additiveRendering)
                    outlineShaderMaterial.SetInt("_Dark", 1);
                else
                    outlineShaderMaterial.SetInt("_Dark", 0);
                if (cornerOutlines)
                    outlineShaderMaterial.SetInt("_CornerOutlines", 1);
                else
                    outlineShaderMaterial.SetInt("_CornerOutlines", 0);

                Shader.SetGlobalFloat("_OutlineAlphaCutoff", alphaCutoff);
            }
        }

        void UpdateOutlineCameraFromSource()
        {
            outlineCamera.CopyFrom(sourceCamera);
            outlineCamera.farClipPlane = 500;
            outlineCamera.renderingPath = RenderingPath.Forward;
            outlineCamera.backgroundColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);
            outlineCamera.clearFlags = CameraClearFlags.SolidColor;
            outlineCamera.rect = new Rect(0, 0, 1, 1);
            outlineCamera.cullingMask = 0;
            outlineCamera.targetTexture = renderTexture;
            outlineCamera.enabled = false;
#if UNITY_5_6_OR_NEWER
            outlineCamera.allowHDR = false;
#else
            outlineCamera.hdr = false;
#endif
        }

        public void AddOutline(Outline outline)
        {
            if (!outlines.Contains(outline))
                outlines.Add(outline);
        }

        public void RemoveOutline(Outline outline)
        {
            if (outlines.Contains(outline))
                outlines.Remove(outline);
        }
    }
}