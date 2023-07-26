using System;
using System.Collections.Generic;
using AwesomeTechnologies.Vegetation;
using UnityEngine;
using AwesomeTechnologies.Utility;

namespace AwesomeTechnologies
{
    public class LODVegetationInstanceInfo
    {
        public List<Matrix4x4> LOD0InstanceList;
       // public List<Matrix4x4> LOD1InstanceList;
       // public List<Matrix4x4> LOD2InstanceList;
        public List<Matrix4x4> LOD0ShadowInstanceList;
      //  public List<Matrix4x4> LOD1ShadowInstanceList;
       // public List<Matrix4x4> LOD2ShadowInstanceList;

        public List<float> LOD0FadeList;
      //  public List<float> LOD1FadeList;
       // public List<float> LOD2FadeList;
        public List<float> LOD0ShadowFadeList;
      //  public List<float> LOD1ShadowFadeList;
      //  public List<float> LOD2ShadowFadeList;
    }

    [Serializable]
    public class VegetationItemModelInfo
    {
        public delegate void MultiMaterialChangeDelegate();
        public MultiMaterialChangeDelegate OnMaterialChangeDelegate;

        //public GameObject VegetationModel;
        //public List<int> VegetationGrowthList = new List<int>();
        public List<List<Matrix4x4>> SplitVegetationInstanceList = new List<List<Matrix4x4>>();
        public List<List<Matrix4x4>> CulledVegetationInstanceList = new List<List<Matrix4x4>>();
        public List<List<float>> CulledVegetationDistanceList = new List<List<float>>();
        public List<List<int>> SplitVegetationGrowthList = new List<List<int>>();

        public List<LODVegetationInstanceInfo> LODVegetationInstanceList = new List<LODVegetationInstanceInfo>();

        //public Mesh VegetationMeshLod0;
      //  public Mesh VegetationMeshLod1;
       // public Mesh VegetationMeshLod2;

        public float LOD1Distance = -1;
        public float LOD2Distance = -1;

       // public Material[] VegetationMaterialsLOD0;
      //  public Material[] VegetationMaterialsLOD1;
       // public Material[] VegetationMaterialsLOD2;
        //public MeshRenderer VegetationRendererLOD0;
       // public MeshRenderer VegetationRendererLOD1;
        //public MeshRenderer VegetationRendererLOD2;
        public MaterialPropertyBlock VegetationMaterialPropertyBlockLOD0;
       // public MaterialPropertyBlock VegetationMaterialPropertyBlockLOD1;
        //public MaterialPropertyBlock VegetationMaterialPropertyBlockLOD2;
        public MaterialPropertyBlock VegetationMaterialPropertyBlockShadowsLOD0;
        //public MaterialPropertyBlock VegetationMaterialPropertyBlockShadowsLOD1;
       // public MaterialPropertyBlock VegetationMaterialPropertyBlockShadowsLOD2;

        [NonSerialized]
        public MatrixListPool MatrixListPool;
        [NonSerialized]
        public ListPool<float> FloatListPool;

        [NonSerialized]
        public VegetationItemInfo VegetationItemInfo;
        public VegetationRenderType VegetationRenderType = VegetationRenderType.Instanced;
        public VegetationSettings VegetationSettings;
        public VegetationSystem VegetationSystem;
        public int MaxListSize = 1000;
        public bool UseGPUCulling = false;
        public float BoundingSphereRadius;


        private int _stWindVectorID = -1;
        private int _stWindGlobalID = -1;
        private int _stWindBranchID = -1;
        private int _stWindBranchTwitchID = -1;
        private int _stWindBranchWhipID = -1;
        private int _stWindBranchAnchorID = -1;
        private int _stWindBranchAdherencesID = -1;
        private int _stWindTurbulencesID = -1;
        private int _stWindLeaf1RippleID = -1;
        private int _stWindLeaf1TumbleID = -1;
        private int _stWindLeaf1TwitchID = -1;
        private int _stWindLeaf2RippleID = -1;
        private int _stWindLeaf2TumbleID = -1;
        private int _stWindLeaf2TwitchID = -1;
        private int _stWindFrondRippleID = -1;
        private int _stWindAnimationID = -1;

        public void AddVegetationModelLight()
        {
            VegetationMaterialPropertyBlockLOD0 = new MaterialPropertyBlock();
           // VegetationMaterialPropertyBlockLOD1 = new MaterialPropertyBlock();
            //VegetationMaterialPropertyBlockLOD2 = new MaterialPropertyBlock();
          

            VegetationMaterialPropertyBlockShadowsLOD0 = new MaterialPropertyBlock();
            //VegetationMaterialPropertyBlockShadowsLOD1 = new MaterialPropertyBlock();
           // VegetationMaterialPropertyBlockShadowsLOD2 = new MaterialPropertyBlock();

            CreateComputeBuffer();
        }

        public void AddVegetationModel(int lodIndex)
        {
            //RegisterWindIDs();

            //GameObject selectedVegetationModelLOD0 = MeshUtils.SelectMeshObject(rootVegetationModel, MeshType.Normal, lodIndex);
           // GameObject selectedVegetationModelLOD1 = MeshUtils.SelectMeshObject(rootVegetationModel, MeshType.Normal, Mathf.Clamp(lodIndex - 1, 0, 2));
            //GameObject selectedVegetationModelLOD2 = MeshUtils.SelectMeshObject(rootVegetationModel, MeshType.Normal, Mathf.Clamp(lodIndex - 2, 0, 2));

            //VegetationModel = rootVegetationModel;
            //VegetationMeshLod0 = GetVegetationMesh(rootVegetationModel, lodIndex);
            //VegetationMeshLod1 = GetVegetationMesh(rootVegetationModel, Mathf.Clamp(lodIndex - 1, 0, 2));
            //VegetationMeshLod2 = GetVegetationMesh(rootVegetationModel, Mathf.Clamp(lodIndex - 2, 0, 2));

           // VegetationRendererLOD0 = selectedVegetationModelLOD0.GetComponentInChildren<MeshRenderer>();
            //VegetationMaterialsLOD0 = CreateMaterials(VegetationRendererLOD0.sharedMaterials, 0);
            //VegetationRendererLOD0.sharedMaterials = VegetationMaterialsLOD0;

            /*
            VegetationRendererLOD1 = selectedVegetationModelLOD1.GetComponentInChildren<MeshRenderer>();
            VegetationMaterialsLOD1 = CreateMaterials(VegetationRendererLOD1.sharedMaterials, 1);
            VegetationRendererLOD1.sharedMaterials = VegetationMaterialsLOD1;

            VegetationRendererLOD2 = selectedVegetationModelLOD2.GetComponentInChildren<MeshRenderer>();
            VegetationMaterialsLOD2 = CreateMaterials(VegetationRendererLOD2.sharedMaterials, 2);
            VegetationRendererLOD2.sharedMaterials = VegetationMaterialsLOD2;
            */
            VegetationMaterialPropertyBlockLOD0 = new MaterialPropertyBlock();
           // VegetationRendererLOD0.GetPropertyBlock(VegetationMaterialPropertyBlockLOD0);
           // if (VegetationMaterialPropertyBlockLOD0 == null) VegetationMaterialPropertyBlockLOD0 = new MaterialPropertyBlock();
            /*
            VegetationMaterialPropertyBlockLOD1 = new MaterialPropertyBlock();
            VegetationRendererLOD1.GetPropertyBlock(VegetationMaterialPropertyBlockLOD1);
            if (VegetationMaterialPropertyBlockLOD1 == null) VegetationMaterialPropertyBlockLOD1 = new MaterialPropertyBlock();

            VegetationMaterialPropertyBlockLOD2 = new MaterialPropertyBlock();
            VegetationRendererLOD2.GetPropertyBlock(VegetationMaterialPropertyBlockLOD2);
            if (VegetationMaterialPropertyBlockLOD2 == null) VegetationMaterialPropertyBlockLOD2 = new MaterialPropertyBlock();
            */
            VegetationMaterialPropertyBlockShadowsLOD0 = new MaterialPropertyBlock();
            //VegetationRendererLOD0.GetPropertyBlock(VegetationMaterialPropertyBlockShadowsLOD0);
            //if (VegetationMaterialPropertyBlockShadowsLOD0 == null) VegetationMaterialPropertyBlockShadowsLOD0 = new MaterialPropertyBlock();
            /*
            VegetationMaterialPropertyBlockShadowsLOD1 = new MaterialPropertyBlock();
            VegetationRendererLOD1.GetPropertyBlock(VegetationMaterialPropertyBlockShadowsLOD1);
            if (VegetationMaterialPropertyBlockShadowsLOD1 == null) VegetationMaterialPropertyBlockShadowsLOD1 = new MaterialPropertyBlock();

            VegetationMaterialPropertyBlockShadowsLOD2 = new MaterialPropertyBlock();
            VegetationRendererLOD2.GetPropertyBlock(VegetationMaterialPropertyBlockShadowsLOD2);
            if (VegetationMaterialPropertyBlockShadowsLOD2 == null) VegetationMaterialPropertyBlockShadowsLOD2 = new MaterialPropertyBlock();
            */
            VegetationItemInfo.VegetationItemModelInfo = this;

            LOD1Distance = 1;// GetLODDistance(rootVegetationModel, 0);
            LOD2Distance = 1;// GetLODDistance(rootVegetationModel, 1);
              
            if (VegetationRenderType == VegetationRenderType.InstancedIndirect && Application.isPlaying) MaxListSize = 1000000;
            BoundingSphereRadius = (VegetationItemInfo.Bounds.extents.magnitude * VegetationItemInfo.MaxScale * VegetationItemInfo.YScale) + 1;

            CreateComputeBuffer();
        }

        float GetLODDistance(GameObject rootVegetationModel, int lodIndex)
        {
            LODGroup lodGroup = rootVegetationModel.GetComponentInChildren<LODGroup>();
            if (lodGroup)
            {
                LOD[] lods = lodGroup.GetLODs();
                if (lodIndex >= 0 && lodIndex < lods.Length)
                {
                    return (lodGroup.size/ lods[lodIndex].screenRelativeTransitionHeight);
                }
            }
            return -1;
        }

        public static Mesh GetVegetationMesh(GameObject rootVegetationModel,int lodIndex)
        {
            GameObject selectedVegetationModel = MeshUtils.SelectMeshObject(rootVegetationModel, MeshType.Normal, lodIndex);
            MeshFilter vegetationMeshFilter = selectedVegetationModel.GetComponentInChildren<MeshFilter>();
            return vegetationMeshFilter.sharedMesh;
        }

        public static Material[] CreateMaterials(Material[] sharedMaterials, VegetationItemModelInfo info)
        {
            for (int i = 0; i <= sharedMaterials.Length - 1; i++)
            {
                if (sharedMaterials[i])
                {

                }
                else
                {
                    sharedMaterials[i] = new Material(Shader.Find("AwesomeTechnologies/AFoliageStandard")) {enableInstancing = true};
                }
                
                RefreshMaterial(sharedMaterials[i], info);
            }
            return sharedMaterials;
        }




        void RegisterWindIDs()
        {
            _stWindVectorID = Shader.PropertyToID("_ST_WindVectorID");
            _stWindGlobalID = Shader.PropertyToID("_ST_WindGlobalID");
            _stWindBranchID = Shader.PropertyToID("_ST_WindBranchID");
            _stWindBranchTwitchID = Shader.PropertyToID("_ST_WindBranchTwitchID");
            _stWindBranchWhipID = Shader.PropertyToID("_ST_WindBranchWhipID");
            _stWindBranchAnchorID = Shader.PropertyToID("_ST_WindBranchAnchorID");
            _stWindBranchAdherencesID = Shader.PropertyToID("_ST_WindBranchAdherencesID");
            _stWindTurbulencesID = Shader.PropertyToID("_ST_WindTurbulencesID");
            _stWindLeaf1RippleID = Shader.PropertyToID("_ST_WindLeaf1RippleID");
            _stWindLeaf1TumbleID = Shader.PropertyToID("_ST_WindLeaf1TumbleID");
            _stWindLeaf1TwitchID = Shader.PropertyToID("_ST_WindLeaf1TwitchID");
            _stWindLeaf2RippleID = Shader.PropertyToID("_ST_WindLeaf2RippleID");
            _stWindLeaf2TumbleID = Shader.PropertyToID("_ST_WindLeaf2TumbleID");
            _stWindLeaf2TwitchID = Shader.PropertyToID("_ST_WindLeaf2TwitchID");
            _stWindFrondRippleID = Shader.PropertyToID("_ST_WindFrondRippleID");
            _stWindAnimationID = Shader.PropertyToID("_ST_WindAnimationID");
        }
        /*
        public void UpdateMaterialPropertyBlockSpeedtreeWind(MaterialPropertyBlock targetMaterialPropertyBlock)
        {         
            VegetationRendererLOD0.GetPropertyBlock(VegetationMaterialPropertyBlockLOD0);
            targetMaterialPropertyBlock.SetVector(_stWindVectorID, VegetationMaterialPropertyBlockLOD0.GetVector(_stWindVectorID));
            targetMaterialPropertyBlock.SetVector(_stWindGlobalID, VegetationMaterialPropertyBlockLOD0.GetVector(_stWindGlobalID));
            targetMaterialPropertyBlock.SetVector(_stWindBranchID, VegetationMaterialPropertyBlockLOD0.GetVector(_stWindBranchID));
            targetMaterialPropertyBlock.SetVector(_stWindBranchTwitchID, VegetationMaterialPropertyBlockLOD0.GetVector(_stWindBranchTwitchID));
            targetMaterialPropertyBlock.SetVector(_stWindBranchWhipID, VegetationMaterialPropertyBlockLOD0.GetVector(_stWindBranchWhipID));
            targetMaterialPropertyBlock.SetVector(_stWindBranchAnchorID, VegetationMaterialPropertyBlockLOD0.GetVector(_stWindBranchAnchorID));
            targetMaterialPropertyBlock.SetVector(_stWindBranchAdherencesID, VegetationMaterialPropertyBlockLOD0.GetVector(_stWindBranchAdherencesID));
            targetMaterialPropertyBlock.SetVector(_stWindTurbulencesID, VegetationMaterialPropertyBlockLOD0.GetVector(_stWindTurbulencesID));
            targetMaterialPropertyBlock.SetVector(_stWindLeaf1RippleID, VegetationMaterialPropertyBlockLOD0.GetVector(_stWindLeaf1RippleID));
            targetMaterialPropertyBlock.SetVector(_stWindLeaf1TumbleID, VegetationMaterialPropertyBlockLOD0.GetVector(_stWindLeaf1TumbleID));
            targetMaterialPropertyBlock.SetVector(_stWindLeaf1TwitchID, VegetationMaterialPropertyBlockLOD0.GetVector(_stWindLeaf1TwitchID));
            targetMaterialPropertyBlock.SetVector(_stWindLeaf2RippleID, VegetationMaterialPropertyBlockLOD0.GetVector(_stWindLeaf2RippleID));
            targetMaterialPropertyBlock.SetVector(_stWindLeaf2TumbleID, VegetationMaterialPropertyBlockLOD0.GetVector(_stWindLeaf2TumbleID));
            targetMaterialPropertyBlock.SetVector(_stWindLeaf2TwitchID, VegetationMaterialPropertyBlockLOD0.GetVector(_stWindLeaf2TwitchID));
            targetMaterialPropertyBlock.SetVector(_stWindFrondRippleID, VegetationMaterialPropertyBlockLOD0.GetVector(_stWindFrondRippleID));
            targetMaterialPropertyBlock.SetVector(_stWindAnimationID, VegetationMaterialPropertyBlockLOD0.GetVector(_stWindAnimationID));
        }
        */
        public void ReplaceMainTexture(Texture2D texture)
        {           
            for (int i = 0; i <= VegetationItemInfo.sourceMaterials.Length - 1; i++)
            {
                VegetationItemInfo.sourceMaterials[i].SetTexture("_MainTex", texture);
            }
            /*
            for (int i = 0; i <= VegetationMaterialsLOD1.Length - 1; i++)
            {
                VegetationMaterialsLOD1[i].SetTexture("_MainTex", texture);
            }

            for (int i = 0; i <= VegetationMaterialsLOD2.Length - 1; i++)
            {
                VegetationMaterialsLOD2[i].SetTexture("_MainTex", texture);
            }
            */
        }

        public static void RefreshMaterial(Material material, VegetationItemModelInfo info)
        {
            if (info.VegetationRenderType == VegetationRenderType.InstancedIndirect)
            {
                if (!info.VegetationSystem.UseGPUCulling || !info.VegetationSystem.UseComputeShaders || !SystemInfo.supportsComputeShaders)
                {
                    material.DisableKeyword("GPU_FRUSTUM_ON");
                }
                else
                {
                    material.EnableKeyword("GPU_FRUSTUM_ON");
                }
            }

            VegetationShaderType vegetationShaderType =
                VegetationTypeDetector.GetVegetationShaderTypeFromName(material.shader.name);
            switch (vegetationShaderType)
            {
                /*
                case VegetationShaderType.Grass:

                    material.SetColor("_Color", VegetationItemInfo.ColorTint1);
                    material.SetColor("_ColorB", VegetationItemInfo.ColorTint2);
                    material.SetFloat("_Cutoff", VegetationItemInfo.TextureCutoff);

                    material.SetFloat("_RandomDarkening", VegetationItemInfo.RandomDarkening);
                    material.SetFloat("_RootAmbient", VegetationItemInfo.RootAmbient);
                    material.SetFloat("_Wetness", VegetationSettings.GrassWetnesss);

                    Vector4 colorScale = material.GetVector("_AG_ColorNoiseArea");
                    colorScale = new Vector4(colorScale.x, VegetationItemInfo.ColorAreaScale, colorScale.z, colorScale.w);
                    material.SetVector("_AG_ColorNoiseArea", colorScale);

                  
                    material.SetFloat("_WindAffectDistance", VegetationSettings.WindRange);

                    material.EnableKeyword("FAR_CULL_ON");

                    //TUDO check touch react status better. bug in builds

                    //if (VegetationSettings.UseTouchReact)
                        material.EnableKeyword("TOUCH_BEND_ON");
                    //else
                    //    material.DisableKeyword("TOUCH_BEND_ON");
                    break;
                    
                    /
                case VegetationShaderType.Speedtree:
                      material.SetFloat("_Cutoff", material.GetFloat("_Cutoff"));
                      material.SetColor("_HueVariation", VegetationItemInfo.Hue);
                      material.SetColor("_Color", VegetationItemInfo.ColorTint1);

#if UNITY_2018_1_OR_NEWER
                    if (lod == 0 || lod == 1)
                    {    
                        material.DisableKeyword("LOD_FADE_CROSSFADE");
                        material.EnableKeyword("LOD_FADE_PERCENTAGE");
                    }
#endif
                    break;
                    */
            }

            if (material.HasProperty("_Cutoff"))
            {
                material.SetFloat("_Cutoff", material.GetFloat("_Cutoff"));
            }
            
            material.SetFloat("_CullFarStart", info.VegetationSettings.VegetationDistance);
            material.SetFloat("_CullFarDistance", 20);

            float minVegetationDistance = Mathf.Clamp(info.VegetationSettings.VegetationDistance, 20,
                info.VegetationSettings.VegetationDistance - 20);
            Shader.SetGlobalVector("_VSGrassFade", new Vector4(minVegetationDistance, 20, 0, 0));
        }
       
        public void RefreshMaterials()
        {
            for (int i = 0; i <= VegetationItemInfo.sourceMaterials.Length - 1; i++)
            {
                RefreshMaterial(VegetationItemInfo.sourceMaterials[i], this);
            }
            /*
            for (int i = 0; i <= VegetationMaterialsLOD1.Length - 1; i++)
            {
                RefreshMaterial(VegetationMaterialsLOD1[i], 1);
            }

            for (int i = 0; i <= VegetationMaterialsLOD2.Length - 1; i++)
            {
                RefreshMaterial(VegetationMaterialsLOD2[i], 1);
            }
            */
            if (OnMaterialChangeDelegate != null) OnMaterialChangeDelegate();
        }

        //public void RefreshFrustumCullingPlanes(Vector4 frustrumPlane0, Vector4 frustrumPlane1, Vector4 frustrumPlane2, Vector4 frustrumPlane3, Vector4 frustrumPlane4, Vector4 frustrumPlane5)                                
        //{
        //    if (VegetationRenderType != VegetationRenderType.InstancedIndirect) return;

        //    for (int i = 0; i <= VegetationMaterialsLOD0.Length - 1; i++)
        //    {
        //        SetFrustumCullingPlanes(VegetationMaterialsLOD0[i],frustrumPlane0,frustrumPlane1,frustrumPlane2,frustrumPlane3,frustrumPlane4,frustrumPlane5);
        //    }

        //    for (int i = 0; i <= VegetationMaterialsLOD1.Length - 1; i++)
        //    {
        //        SetFrustumCullingPlanes(VegetationMaterialsLOD1[i], frustrumPlane0, frustrumPlane1, frustrumPlane2, frustrumPlane3, frustrumPlane4, frustrumPlane5);
        //    }

        //    for (int i = 0; i <= VegetationMaterialsLOD2.Length - 1; i++)
        //    {
        //        SetFrustumCullingPlanes(VegetationMaterialsLOD2[i], frustrumPlane0, frustrumPlane1, frustrumPlane2, frustrumPlane3, frustrumPlane4, frustrumPlane5);
        //    }
        //}

        //void SetFrustumCullingPlanes(Material material, Vector4 frustrumPlane0, Vector4 frustrumPlane1, Vector4 frustrumPlane2, Vector4 frustrumPlane3,
        //    Vector4 frustrumPlane4, Vector4 frustrumPlane5)
        //{
        //    material.SetVector("_VS_CameraFrustumPlane0", frustrumPlane0);
        //    material.SetVector("_VS_CameraFrustumPlane1", frustrumPlane1);
        //    material.SetVector("_VS_CameraFrustumPlane2", frustrumPlane2);
        //    material.SetVector("_VS_CameraFrustumPlane3", frustrumPlane3);
        //    material.SetVector("_VS_CameraFrustumPlane4", frustrumPlane4);
        //    material.SetVector("_VS_CameraFrustumPlane5", frustrumPlane5);
        //}

        public ComputeBuffer MergeBuffer;
        public ComputeBuffer VisibleBufferLOD0;
        //public ComputeBuffer VisibleBufferLOD1;
        //public ComputeBuffer VisibleBufferLOD2;
        private readonly uint[] _args = { 0, 0, 0, 0, 0 };
        public List<ComputeBuffer> ArgsBufferMergedLOD0List = new List<ComputeBuffer>();
        //public List<ComputeBuffer> ArgsBufferMergedLOD1List = new List<ComputeBuffer>();
        //public List<ComputeBuffer> ArgsBufferMergedLOD2List = new List<ComputeBuffer>();

        void CreateComputeBuffer()
        {
            if (VegetationItemInfo == null) { return; }
            if (VegetationItemInfo.sourceMesh == null) { return; }
            MergeBuffer = new ComputeBuffer(5000, (16 * 4 * 2) + 16, ComputeBufferType.Append);
            MergeBuffer.SetCounterValue(0);
            
            VisibleBufferLOD0 = new ComputeBuffer(5000, (16 * 4 * 2) + 16, ComputeBufferType.Append);
            VisibleBufferLOD0.SetCounterValue(0);
            /*
            VisibleBufferLOD1 = new ComputeBuffer(5000, (16 * 4 * 2) + 16, ComputeBufferType.Append);
            VisibleBufferLOD1.SetCounterValue(0);

            VisibleBufferLOD2 = new ComputeBuffer(5000, (16 * 4 * 2) + 16, ComputeBufferType.Append);
            VisibleBufferLOD2.SetCounterValue(0);
            */
            for (int i = 0; i <= VegetationItemInfo.sourceMesh.subMeshCount - 1; i++)
            {
                ComputeBuffer argsBufferMergedLod0 =
                    new ComputeBuffer(1, _args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
                _args[0] = VegetationItemInfo.sourceMesh.GetIndexCount(i);
                _args[2] = VegetationItemInfo.sourceMesh.GetIndexStart(i);
                argsBufferMergedLod0.SetData(_args);
                ArgsBufferMergedLOD0List.Add(argsBufferMergedLod0);
            }
            /*
            for (int i = 0; i <= VegetationMeshLod1.subMeshCount - 1; i++)
            {
                ComputeBuffer argsBufferMergedLod1 =
                    new ComputeBuffer(1, _args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
                _args[0] = VegetationMeshLod1.GetIndexCount(i);
                _args[2] = VegetationMeshLod1.GetIndexStart(i);
                argsBufferMergedLod1.SetData(_args);
                ArgsBufferMergedLOD1List.Add(argsBufferMergedLod1);
            }

            for (int i = 0; i <= VegetationMeshLod2.subMeshCount - 1; i++)
            {
                ComputeBuffer argsBufferMergedLod2 =
                    new ComputeBuffer(1, _args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
                _args[0] = VegetationMeshLod2.GetIndexCount(i);
                _args[2] = VegetationMeshLod2.GetIndexStart(i);
                argsBufferMergedLod2.SetData(_args);
                ArgsBufferMergedLOD2List.Add(argsBufferMergedLod2);
            }
            */
        }

        public void UpdateComputeBufferSize(int newInstanceCount)
        {
            if (MergeBuffer != null) MergeBuffer.Release();
            MergeBuffer = null;
            
            if (VisibleBufferLOD0 != null) VisibleBufferLOD0.Release();
            VisibleBufferLOD0 = null;
            /*
            if (VisibleBufferLOD1 != null) VisibleBufferLOD1.Release();
            VisibleBufferLOD1 = null;

            if (VisibleBufferLOD2 != null) VisibleBufferLOD2.Release();
            VisibleBufferLOD2 = null;
            */
            MergeBuffer = new ComputeBuffer(newInstanceCount, (16 * 4 * 2) + 16, ComputeBufferType.Append);
            MergeBuffer.SetCounterValue(0);
            
            VisibleBufferLOD0 = new ComputeBuffer(newInstanceCount, (16 * 4 * 2) + 16, ComputeBufferType.Append);
            VisibleBufferLOD0.SetCounterValue(0);
            /*
            VisibleBufferLOD1 = new ComputeBuffer(newInstanceCount, (16 * 4 * 2) + 16, ComputeBufferType.Append);
            VisibleBufferLOD1.SetCounterValue(0);

            VisibleBufferLOD2 = new ComputeBuffer(newInstanceCount, (16 * 4 * 2) + 16, ComputeBufferType.Append);
            VisibleBufferLOD2.SetCounterValue(0);
            */
        }

        public void DestroyComputeBuffers()
        {
            if (MergeBuffer != null) MergeBuffer.Release();
            MergeBuffer = null;
            
            if (VisibleBufferLOD0 != null) VisibleBufferLOD0.Release();
            VisibleBufferLOD0 = null;
            /*
            if (VisibleBufferLOD1 != null) VisibleBufferLOD1.Release();
            VisibleBufferLOD1 = null;

            if (VisibleBufferLOD2 != null) VisibleBufferLOD2.Release();
            VisibleBufferLOD2 = null;
            */
            ReleaseArgsBuffers();
        }

        void ReleaseArgsBuffers()
        {
            
            for (int i = 0; i <= ArgsBufferMergedLOD0List.Count - 1; i++)
            {
                if (ArgsBufferMergedLOD0List[i] != null) ArgsBufferMergedLOD0List[i].Release();
            }
            /*
            for (int i = 0; i <= ArgsBufferMergedLOD1List.Count - 1; i++)
            {
                if (ArgsBufferMergedLOD1List[i] != null) ArgsBufferMergedLOD1List[i].Release();
            }

            for (int i = 0; i <= ArgsBufferMergedLOD2List.Count - 1; i++)
            {
                if (ArgsBufferMergedLOD2List[i] != null) ArgsBufferMergedLOD2List[i].Release();
            }
            */
            ArgsBufferMergedLOD0List.Clear();
           // ArgsBufferMergedLOD1List.Clear();
            //ArgsBufferMergedLOD2List.Clear();
            
        }
    }
}


