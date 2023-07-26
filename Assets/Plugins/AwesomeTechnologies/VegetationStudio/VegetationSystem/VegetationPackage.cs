using System;
using System.Collections.Generic;
using System.IO;
using AwesomeTechnologies.Common.Interfaces;
using AwesomeTechnologies.Utility;
using AwesomeTechnologies.Utility.Extentions;
using AwesomeTechnologies.Vegetation;
using JetBrains.Annotations;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace AwesomeTechnologies
{
    public class VegetationInfoComparer : IComparer<int>
    {
        public List<VegetationItemInfo> VegetationInfoList;
        public int Compare(int a, int b)
        {
            var aTypeValue = (int)VegetationInfoList[a].VegetationType;
            var bTypeValue = (int)VegetationInfoList[b].VegetationType;
            return bTypeValue.CompareTo(aTypeValue);
        }
    }

    public class VegetationInfoIDComparer : IComparer<string>
    {
        public List<VegetationItemInfo> VegetationInfoList;
        public int Compare(string a, string b)
        {
            var indexA = GetIndexFromID(a);
            var indexB = GetIndexFromID(b);

            if (indexA < 0 || indexB < 0) return -1;

            var aTypeValue = (int)VegetationInfoList[indexA].VegetationType;
            var bTypeValue = (int)VegetationInfoList[indexB].VegetationType;
            return bTypeValue.CompareTo(aTypeValue);
        }

        private int GetIndexFromID(string id)
        {
            for (var i = 0; i <= VegetationInfoList.Count - 1; i++)
                if (VegetationInfoList[i].VegetationItemID == id) return i;

            return -1;
        }
    }


    public enum TextureMaskRuleType
    {
        Include = 1,
        Exclude = 2,
        Density = 3,
        Scale = 4
    }

    public enum VegetationMaskIndex
    {
        VegetationMask1 = 1,
        VegetationMask2 = 2,
        VegetationMask3 = 3,
        VegetationMask4 = 4,
        VegetationMask5 = 5,
        VegetationMask6 = 6,
        VegetationMask7 = 7,
        VegetationMask8 = 8,
    }

    [Serializable]
    public enum VegetationTypeIndex
    {
        VegetationType1 = 1,
        VegetationType2 = 2,
        VegetationType3 = 3,
        VegetationType4 = 4,
        VegetationType5 = 5,
        VegetationType6 = 6,
        VegetationType7 = 7,
        VegetationType8 = 8,
        VegetationType9 = 9,
        VegetationType10 = 10,
        VegetationType11 = 11,
        VegetationType12 = 12,
        VegetationType13 = 13,
        VegetationType14 = 14,
        VegetationType15 = 15,
        VegetationType16 = 16,
        VegetationType17 = 17,
        VegetationType18 = 18,
        VegetationType19 = 19,
        VegetationType20 = 20,
        VegetationType21 = 21,
        VegetationType22 = 22,
        VegetationType23 = 23,
        VegetationType24 = 24,
        VegetationType25 = 25,
        VegetationType26 = 26,
        VegetationType27 = 27,
        VegetationType28 = 28,
        VegetationType29 = 29,
        VegetationType30 = 30,
        VegetationType31 = 31,
        VegetationType32 = 32
    }


    [Serializable]
    public class TextureMaskInfo
    {
        public int TextureLayer;
        public float MinimumValue;
        public float MaximumValue;

        public TextureMaskInfo()
        {

        }
        public TextureMaskInfo(TextureMaskInfo sourceItem)
        {
            TextureLayer = sourceItem.TextureLayer;
            MinimumValue = sourceItem.MinimumValue;
            MaximumValue = sourceItem.MaximumValue;
        }
    }

    [Serializable]
    public class TextureMaskRule
    {
        public string MaskId;
        public float MinDensity = 0.1f;
        public float MaxDensity = 1;
        public float ScaleMultiplier = 1;
        public float DensityMultiplier = 1;
        public List<TextureMaskProperty> TextureMaskPropertiesList = new List<TextureMaskProperty>();

        public TextureMaskRule(TextureMaskRule sourceItem)
        {
            MaskId = sourceItem.MaskId;
            MinDensity = sourceItem.MinDensity;
            MaxDensity = sourceItem.MaxDensity;
            ScaleMultiplier = sourceItem.ScaleMultiplier;
            DensityMultiplier = sourceItem.DensityMultiplier;

            for (var i = 0; i <= sourceItem.TextureMaskPropertiesList.Count - 1; i++)
                TextureMaskPropertiesList.Add(new TextureMaskProperty(sourceItem.TextureMaskPropertiesList[i]));
        }

        public TextureMaskRule()
        {


        }
    }

    [Serializable]
    public class DetailMaskRule
    {
        public int DetailLayer = 0;
    }

    public class MaskTextureInfo
    {
        public Texture2D Texture;
        public string TextureGuid = "";
        public UnityEngine.Vector2 Uv = new UnityEngine.Vector2(15, 15);
        public UnityEngine.Vector2 Offset;
    }

    [Serializable]
    public enum VegetationRotationType
    {
        RotateY = 0,
        RotateXYZ = 1,
        FollowTerrain = 2,
        FollowTerrainScale = 3,
        NoRotation = 4,
        FollowTerrainScaleWithBlock = 5
    }

    [Serializable]
    public enum TerrainTextureType
    {
        Texture1 = 0,
        Texture2 = 1,
        Texture3 = 2,
        Texture4 = 3,
        Texture5 = 4,
        Texture6 = 5,
        Texture7 = 6,
        Texture8 = 7,
        Texture9 = 8,
        Texture10 = 9,
        Texture11 = 10,
        Texture12 = 11,
        Texture13 = 12,
        Texture14 = 13,
        Texture15 = 14,
        Texture16 = 15
    }

    [Serializable]
    public enum VegetationPrefabType
    {
        Mesh = 0,
        Texture = 1
    }

    [Serializable]
    public enum VegetationRenderType
    {
        Instanced = 0,
        Normal = 1,
        InstancedIndirect = 2
    }


    [Serializable]
    public enum VegetationType
    {
        Grass = 0,
        Plant = 1,
        Tree = 2,
        Objects = 3,
        LargeObjects = 4
    }

    [Serializable]
    public enum VegetationShaderType
    {
        Grass = 0,
        Other = 1,
        Speedtree = 2,
        Foliage = 3,
    }

    [Serializable]
    public enum VegetationSteepnessType
    {
        Simple = 0,
        Advanced = 1
    }

    [Serializable]
    public enum VegetationScaleType
    {
        Simple = 0,
        Advanced = 1
    }

    [Serializable]
    public enum VegetationHeightType
    {
        Simple = 0,
        Advanced = 1
    }

    [Serializable]
    public enum BillboardQuality
    {
        Normal = 0,
        High = 1,
        Max = 2,
        Normal3D = 4,
        High3D = 5,
        Max3D = 6,
        HighSample3D = 7,
        HighSample2D = 8,
        NormalSingle = 9,
        HighSingle = 10,
        MaxSingle = 11,
        NormalQuad = 12,
        HighQuad = 13,
        MaxQuad = 14,
    }

    [Serializable]
    public enum ColliderType
    {
        Disabled = 0,
        Capsule = 1,
        Sphere = 2,
        Mesh = 3,
        CustomMesh = 4
    }


    [Serializable]
    public enum NavMeshObstacleType
    {
        Disabled = 0,
        Capsule = 1,
        Box = 2
    }

    [Serializable]
    public class TerrainTextureSettings
    {
        public AnimationCurve TextureHeightCurve;
        public AnimationCurve TextureAngleCurve;
        public bool TextureUseNoise;
        public float TextureNoiseScale = 5;
        public float TextureWeight = 1;
        public int TextureLayer;
        public bool Enabled = true;
    }

    [Serializable]
    public class VegetationLOD
    {
        public Mesh LODMesh;
        public Material LODMaterial;
        public float LODDistance;
    }

    [Serializable]
    public class VegetationItemInfo
    {
        public string Name = "New vegetation Item";
        public string VegetationItemID;

        public bool EnableRuntimeSpawn = true;

        public VegetationPrefabType PrefabType = VegetationPrefabType.Mesh;
        public VegetationShaderType ShaderType = VegetationShaderType.Other;
        public Mesh sourceMesh;
        public Material[] sourceMaterials;
        public Material sourceSecondMaterial;
        // [SerializeField]
        // public MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
        // [SerializeField]
        //  public MaterialPropertyBlock shadowMaterialProperyBlock = new MaterialPropertyBlock();
        public GameObject VegetationPrefab;
        public string VegetationGuid = "";
        public Texture2D VegetationTexture;
        public string VegetationTextureGuid = "";
        public VegetationType VegetationType = VegetationType.Grass;
        public VegetationRenderType VegetationRenderType = VegetationRenderType.Instanced;
        //public bool UseGPUCulling = false;
        public VegetationScaleType VegetationScaleType = VegetationScaleType.Simple;
        public float MinScale = 0.8f;
        public float MaxScale = 1.2f;
        public Vector3 MinVectorScale = new Vector3(0.8f, 0.8f, 0.8f);
        public Vector3 MaxVectorScale = new Vector3(1.2f, 1.2f, 1.2f);
        public bool UseAutomaticDensityScale = false;
        public int LodIndex;
        public int Seed;
        public float LODFactor = 1f;
        public bool DisableLOD = false;
        public bool DisableShadows = false;

        public List<VegetationLOD> lods;

        public Texture2D VegetationTumbnail = null;
        public string VegetationTumbnailGuid = "";

        public bool UseVegetationMasksOnStorage = false;

        public ColliderType ColliderType = ColliderType.Disabled;
        public float ColliderRadius = 0.25f;
        public float ColliderHeight = 2f;
        public Vector3 ColliderOffset = Vector3.zero;
        public bool ColliderTrigger;
        public Mesh ColliderMesh;
        public string ColliderMeshGUID;
        public bool ColliderUseForBake = true;

        public NavMeshObstacleType NavMeshObstacleType = NavMeshObstacleType.Disabled;
        public Vector3 NavMeshObstacleCenter;
        public Vector3 NavMeshObstacleSize = Vector3.one;
        public float NavMeshObstacleRadius = 0.5f;
        public float NavMeshObstacleHeight = 2f;
        public bool NavMeshObstacleCarve = true;

        public bool UseTouchReact;

        public bool UseBillboards = true;
        public BillboardQuality BillboardQuality = BillboardQuality.Normal;
        public Texture2D BillboardTexture;
        public string BillboardTextureGuid = "";
        public Texture2D BillboardNormalTexture;
        public string BillboardNormalTextureGuid = "";
        public int BillboardLodIndex = 2;
        public ColorSpace BillboardColorSpace =ColorSpace.Uninitialized;
        public float BillboardBrightness = 1;
        public float BillboardCutoff = 0.2f;
        public Color BillboardTintColor = Color.white;
        public Color BillboardAtlasBackgroundColor = new Color(80/255f,80/255f,20/255f);
        public float BillboardMipmapBias = -2;

        public Color ColorTint1 = Color.white;
        public Color ColorTint2 = Color.white;
        public float Brightness = 1f;
        public float RandomDarkening = 0.31f;
        public float RootAmbient = 0.63f;
        public float ColorAreaScale = 100f;
        public Color Hue = new Color(1f, 0.5f, 0f, 25f / 256f);

        public float TextureCutoff = 0.2f;

        public float YScale = 1f;

        public float SampleDistance = 1.5f;
        public float Density = 1f;
        public float Scale = 1f;

        public bool UseCollisionDetection;
        public VegetationSteepnessType VegetationSteepnessType = VegetationSteepnessType.Simple;
        public VegetationHeightType VegetationHeightType = VegetationHeightType.Simple;
        public float MinimumSteepness;
        public float MaximumSteepness = 20f;
        public AnimationCurve SteepnessCurve = new AnimationCurve();
        public AnimationCurve HeightCurve = new AnimationCurve();
        public float[] HeightCurveArray = new float[0];
        public float[] SteepnessCurveArray = new float[0];
        public float MinCurveHeight = 0;
        public float MaxCurveHeight = 1000;
        public bool AutomaticCurveMaxHeight = true;

        public float MinimumHeight;
        public float MaximumHeight = 1000;
        public bool UseHeightLevel = true;
        public bool UseAngle = true;
        public bool RandomizePosition = true;
        public float RandomPositionRelativeDistance = 1f;
        public Vector3 Offset = new Vector3(0, 0, 0);
        public Vector3 RotationOffset = new Vector3(0, 0, 0);
        public float RotationBlock = 15f;
        public VegetationRotationType Rotation = VegetationRotationType.RotateY;

        public bool UsePerlinMask = true;
        public float PerlinCutoff = 0.5f;
        public float PerlinScale = 5;
        public bool InversePerlinMask;
        public UnityEngine.Vector2 PerlinOffset = new UnityEngine.Vector2(0, 0);

        public bool UseExcludeTextueMask;
        public List<TextureMaskInfo> ExcludeTextureMaskList = new List<TextureMaskInfo>();

        public bool UseIncludeTextueMask;
        public List<TextureMaskInfo> IncludeTextureMaskList = new List<TextureMaskInfo>();

        public bool UseVegetationMask;
        public VegetationTypeIndex VegetationTypeIndex = VegetationTypeIndex.VegetationType1;

        public bool UseExcludeTextueMaskRules = false;
        public List<TextureMaskRule> ExcludeTextureMaskRuleList = new List<TextureMaskRule>();

        public bool UseDensityTextueMaskRules = false;
        public List<TextureMaskRule> DensityTextureMaskRuleList = new List<TextureMaskRule>();

        public bool UseScaleTextueMaskRules = false;
        public List<TextureMaskRule> ScaleTextureMaskRuleList = new List<TextureMaskRule>();

        public bool UseIncludeTextueMaskRules = false;
        public List<TextureMaskRule> IncludeTextureMaskRuleList = new List<TextureMaskRule>();

        public bool UseIncludeDetailMaskRules = false;
        public List<DetailMaskRule> IncludeDetailMaskRuleList = new List<DetailMaskRule>();
        public int IncludeDetailLayer = 0;

        //public Texture2D icon;

        [HideInInspector]
        public Bounds Bounds;
        [HideInInspector]
        public float Volume;
       
        [NonSerialized]
        public VegetationItemModelInfo VegetationItemModelInfo;

        public VegetationItemInfo()
        {
            VegetationItemID = Guid.NewGuid().ToString();
        }

        public VegetationItemInfo(string vegetationSystemVegetationItemID)
        {
            VegetationItemID = vegetationSystemVegetationItemID;
        }

        public void SwitchToDetail(int newLayer)
        {
            EnableRuntimeSpawn = true;
            UseIncludeDetailMaskRules = true;

            if (newLayer >= 0)
                IncludeDetailLayer = newLayer;
            else
                IncludeDetailLayer = 0;
            
            //currentSystem.RefreshPreset();
        }

        public void SwitchToManual()
        {
            EnableRuntimeSpawn = true;

            IncludeDetailLayer = -1;
            UseIncludeDetailMaskRules = false;
            //currentSystem.RefreshPreset();
        }

        public void SwitchToDisabled()
        {
            UseIncludeDetailMaskRules = false;

            IncludeDetailLayer = -2;
            EnableRuntimeSpawn = false;
            //currentSystem.RefreshPreset();
        }

        public VegetationItemInfo(string vegetationSystemVegetationItemID, VegetationType vegetationType)
        {
            VegetationItemID = vegetationSystemVegetationItemID;

            switch (vegetationType)
            {
                case VegetationType.Grass:
                    EnableRuntimeSpawn = true;
                    VegetationRenderType = VegetationRenderType.InstancedIndirect;
                    DisableShadows = true;
                    Rotation = VegetationRotationType.FollowTerrainScale;
                    UseIncludeDetailMaskRules = true;
                    break;
                case VegetationType.Plant:
                    EnableRuntimeSpawn = true;
                    VegetationRenderType = VegetationRenderType.InstancedIndirect;
                    DisableShadows = false;
                    Rotation = VegetationRotationType.FollowTerrain;
                    UseIncludeDetailMaskRules = true;
                    break;
                case VegetationType.Objects:
                    EnableRuntimeSpawn = true;
                    VegetationRenderType = VegetationRenderType.Instanced;
                    DisableShadows = true;
                    IncludeDetailLayer = -1;
                    Rotation = VegetationRotationType.FollowTerrain;
                    UseIncludeDetailMaskRules = false;
                    break;
                case VegetationType.Tree:
                    EnableRuntimeSpawn = false;
                    VegetationRenderType = VegetationRenderType.InstancedIndirect;
                    DisableShadows = false;
                    IncludeDetailLayer = -1;
                    Rotation = VegetationRotationType.RotateY;
                    UseIncludeDetailMaskRules = false;
                    break;
            }

        VegetationScaleType = VegetationScaleType.Simple;
        MinScale = 1.0f;
        MaxScale = 2.0f;
        MinVectorScale = new Vector3(0.8f, 0.8f, 0.8f);
        MaxVectorScale = new Vector3(2.0f, 2.0f, 2.0f);
        DisableLOD = true;
        ColliderType = ColliderType.Disabled;
        NavMeshObstacleType = NavMeshObstacleType.Disabled;
            UseTouchReact = true;
            UseBillboards = false;

        ColorTint1 = Color.white;
        ColorTint2 = Color.white;
        Brightness = 1f;
        RandomDarkening = 0.31f;
        RootAmbient = 0.63f;
        ColorAreaScale = 100f;
        Hue = new Color(1f, 0.5f, 0f, 25f / 256f);

        TextureCutoff = 0.2f;

        YScale = 1f;

        SampleDistance = 0.3f;
        Density = 0.65f;
        RotationOffset = new Vector3(10f, 10f, 10f);
        UseHeightLevel = false;
        UseAngle = false;
        RandomizePosition = true;
        UsePerlinMask = false;
        LodIndex = 2;


    }

        public VegetationItemInfo(VegetationItemInfo sourceItem)
        {
            VegetationItemID = Guid.NewGuid().ToString();

            VegetationTexture = sourceItem.VegetationTexture;
            VegetationTextureGuid = sourceItem.VegetationTextureGuid;
            VegetationPrefab = sourceItem.VegetationPrefab;
            VegetationGuid = sourceItem.VegetationGuid;
            PrefabType = sourceItem.PrefabType;
            ShaderType = sourceItem.ShaderType;
            Name = sourceItem.Name;

            VegetationType = sourceItem.VegetationType;
            VegetationRenderType = sourceItem.VegetationRenderType;
            LodIndex = sourceItem.LodIndex;
            UseCollisionDetection = sourceItem.UseCollisionDetection;
            UseTouchReact = sourceItem.UseTouchReact;

            ColliderType = sourceItem.ColliderType;
            ColliderRadius = sourceItem.ColliderRadius;
            ColliderHeight = sourceItem.ColliderHeight;
            ColliderOffset = sourceItem.ColliderOffset;
            ColliderTrigger = sourceItem.ColliderTrigger;
            ColliderMesh = sourceItem.ColliderMesh;
            ColliderMeshGUID = sourceItem.ColliderMeshGUID;
            ColliderUseForBake = sourceItem.ColliderUseForBake;

            NavMeshObstacleType = sourceItem.NavMeshObstacleType;
            NavMeshObstacleCenter = sourceItem.NavMeshObstacleCenter;
            NavMeshObstacleSize = sourceItem.NavMeshObstacleSize;
            NavMeshObstacleRadius = sourceItem.NavMeshObstacleRadius;
            NavMeshObstacleHeight = sourceItem.NavMeshObstacleHeight;
            NavMeshObstacleCarve = sourceItem.NavMeshObstacleCarve;

            UseBillboards = sourceItem.UseBillboards;
            BillboardQuality = sourceItem.BillboardQuality;
            BillboardTintColor = sourceItem.BillboardTintColor;
            BillboardAtlasBackgroundColor = sourceItem.BillboardAtlasBackgroundColor;
            BillboardBrightness = sourceItem.BillboardBrightness;
            BillboardCutoff = sourceItem.BillboardCutoff;
            BillboardMipmapBias = sourceItem.BillboardMipmapBias;

            CopyPlacingValues(sourceItem);          
        }

        public void CopyMainValues(VegetationItemInfo sourceItem)
        {
            VegetationItemID = sourceItem.VegetationItemID;
            VegetationTexture = sourceItem.VegetationTexture;
            VegetationTextureGuid = sourceItem.VegetationTextureGuid;
            VegetationPrefab = sourceItem.VegetationPrefab;
            VegetationGuid = sourceItem.VegetationGuid;
            PrefabType = sourceItem.PrefabType;
            ShaderType = sourceItem.ShaderType;
            Name = sourceItem.Name;

            VegetationType = sourceItem.VegetationType;
            VegetationRenderType = sourceItem.VegetationRenderType;
            LodIndex = sourceItem.LodIndex;
            UseCollisionDetection = sourceItem.UseCollisionDetection;
            UseTouchReact = sourceItem.UseTouchReact;

            ColliderType = sourceItem.ColliderType;
            ColliderRadius = sourceItem.ColliderRadius;
            ColliderHeight = sourceItem.ColliderHeight;
            ColliderOffset = sourceItem.ColliderOffset;
            ColliderTrigger = sourceItem.ColliderTrigger;
            ColliderMesh = sourceItem.ColliderMesh;
            ColliderMeshGUID = sourceItem.ColliderMeshGUID;
            ColliderUseForBake = sourceItem.ColliderUseForBake;

            NavMeshObstacleType = sourceItem.NavMeshObstacleType;
            NavMeshObstacleCenter = sourceItem.NavMeshObstacleCenter;
            NavMeshObstacleSize = sourceItem.NavMeshObstacleSize;
            NavMeshObstacleRadius = sourceItem.NavMeshObstacleRadius;
            NavMeshObstacleHeight = sourceItem.NavMeshObstacleHeight;
            NavMeshObstacleCarve = sourceItem.NavMeshObstacleCarve;

            UseBillboards = sourceItem.UseBillboards;
            BillboardQuality = sourceItem.BillboardQuality;
            BillboardTintColor = sourceItem.BillboardTintColor;
            BillboardAtlasBackgroundColor = sourceItem.BillboardAtlasBackgroundColor;
            BillboardBrightness = sourceItem.BillboardBrightness;
            BillboardCutoff = sourceItem.BillboardCutoff;
            BillboardMipmapBias = sourceItem.BillboardMipmapBias;
        }
        public void CopyPlacingValues(VegetationItemInfo sourceItem)
        {
            PerlinOffset = sourceItem.PerlinOffset;
            Seed = sourceItem.Seed;
            Density = sourceItem.Density;
            EnableRuntimeSpawn = sourceItem.EnableRuntimeSpawn;

            MinScale = sourceItem.MinScale;
            MaxScale = sourceItem.MaxScale;

            SampleDistance = sourceItem.SampleDistance;
            UseCollisionDetection = sourceItem.UseCollisionDetection;
            VegetationSteepnessType = sourceItem.VegetationSteepnessType;
            VegetationHeightType = sourceItem.VegetationHeightType;
            MinimumSteepness = sourceItem.MinimumSteepness;
            MaximumSteepness = sourceItem.MaximumSteepness;
            SteepnessCurve = new AnimationCurve(sourceItem.SteepnessCurve.keys);
            HeightCurve = new AnimationCurve(sourceItem.HeightCurve.keys);
            MinimumHeight = sourceItem.MinimumHeight;
            MaximumHeight = sourceItem.MaximumHeight;
            UseHeightLevel = sourceItem.UseHeightLevel;
            UseAngle = sourceItem.UseAngle;
            RandomizePosition = sourceItem.RandomizePosition;
            Offset = sourceItem.Offset;
            RotationOffset = sourceItem.RotationOffset;
            Rotation = sourceItem.Rotation;

            ColorTint1 = sourceItem.ColorTint1;
            ColorTint2 = sourceItem.ColorTint2;
            Brightness = sourceItem.Brightness;
            RandomDarkening = sourceItem.RandomDarkening;
            RootAmbient = sourceItem.RootAmbient;
            ColorAreaScale = sourceItem.ColorAreaScale;
            TextureCutoff = sourceItem.TextureCutoff;
            YScale = sourceItem.YScale;
            Hue = sourceItem.Hue;

            UseVegetationMasksOnStorage = sourceItem.UseVegetationMasksOnStorage;

            UsePerlinMask = sourceItem.UsePerlinMask;
            PerlinCutoff = sourceItem.PerlinCutoff;
            PerlinScale = sourceItem.PerlinScale;
            InversePerlinMask = sourceItem.InversePerlinMask;

            UseVegetationMask = sourceItem.UseVegetationMask;
            VegetationTypeIndex = sourceItem.VegetationTypeIndex;
            Bounds = sourceItem.Bounds;
            Volume = sourceItem.Volume;

            UseIncludeTextueMask = sourceItem.UseIncludeTextueMask;
            UseExcludeTextueMask = sourceItem.UseExcludeTextueMask;

            for (var i = 0; i <= sourceItem.IncludeTextureMaskList.Count - 1; i++)
                IncludeTextureMaskList.Add(new TextureMaskInfo(sourceItem.IncludeTextureMaskList[i]));

            for (var i = 0; i <= sourceItem.ExcludeTextureMaskList.Count - 1; i++)
                ExcludeTextureMaskList.Add(new TextureMaskInfo(sourceItem.ExcludeTextureMaskList[i]));

            for (var i = 0; i <= sourceItem.ExcludeTextureMaskRuleList.Count - 1; i++)
                ExcludeTextureMaskRuleList.Add(new TextureMaskRule(sourceItem.ExcludeTextureMaskRuleList[i]));

            for (var i = 0; i <= sourceItem.IncludeTextureMaskRuleList.Count - 1; i++)
                IncludeTextureMaskRuleList.Add(new TextureMaskRule(sourceItem.IncludeTextureMaskRuleList[i]));

            for (var i = 0; i <= sourceItem.DensityTextureMaskRuleList.Count - 1; i++)
                DensityTextureMaskRuleList.Add(new TextureMaskRule(sourceItem.DensityTextureMaskRuleList[i]));

            for (var i = 0; i <= sourceItem.ScaleTextureMaskRuleList.Count - 1; i++)
                ScaleTextureMaskRuleList.Add(new TextureMaskRule(sourceItem.ScaleTextureMaskRuleList[i]));

            UseIncludeDetailMaskRules = sourceItem.UseIncludeDetailMaskRules;
            IncludeDetailLayer = sourceItem.IncludeDetailLayer;
            SteepnessCurveArray = SteepnessCurve.GenerateCurveArray();
            HeightCurveArray = HeightCurve.GenerateCurveArray();
        }

        public void CopySettingValues(VegetationItemInfo sourceItem)
        {
            PerlinOffset = sourceItem.PerlinOffset;
            Seed = sourceItem.Seed;
            Density = sourceItem.Density;
            EnableRuntimeSpawn = sourceItem.EnableRuntimeSpawn;

            VegetationType = sourceItem.VegetationType;
            VegetationRenderType = sourceItem.VegetationRenderType;
            MinScale = sourceItem.MinScale;
            MaxScale = sourceItem.MaxScale;
            LodIndex = sourceItem.LodIndex;
            UseCollisionDetection = sourceItem.UseCollisionDetection;

            UseTouchReact = sourceItem.UseTouchReact;

            ColliderType = sourceItem.ColliderType;
            ColliderRadius = sourceItem.ColliderRadius;
            ColliderHeight = sourceItem.ColliderHeight;
            ColliderOffset = sourceItem.ColliderOffset;
            ColliderTrigger = sourceItem.ColliderTrigger;
            ColliderMesh = sourceItem.ColliderMesh;
            ColliderMeshGUID = sourceItem.ColliderMeshGUID;
            ColliderUseForBake = sourceItem.ColliderUseForBake;

            NavMeshObstacleType = sourceItem.NavMeshObstacleType;
            NavMeshObstacleCenter = sourceItem.NavMeshObstacleCenter;
            NavMeshObstacleSize = sourceItem.NavMeshObstacleSize;
            NavMeshObstacleRadius = sourceItem.NavMeshObstacleRadius;
            NavMeshObstacleHeight = sourceItem.NavMeshObstacleHeight;
            NavMeshObstacleCarve = sourceItem.NavMeshObstacleCarve;

            UseBillboards = sourceItem.UseBillboards;
            BillboardQuality = sourceItem.BillboardQuality;
            BillboardTintColor = sourceItem.BillboardTintColor;
            BillboardAtlasBackgroundColor = sourceItem.BillboardAtlasBackgroundColor;
            BillboardBrightness = sourceItem.BillboardBrightness;
            BillboardCutoff = sourceItem.BillboardCutoff;
            BillboardMipmapBias = sourceItem.BillboardMipmapBias;

            ColorTint1 = sourceItem.ColorTint1;
            ColorTint2 = sourceItem.ColorTint2;
            Brightness = sourceItem.Brightness;
            RandomDarkening = sourceItem.RandomDarkening;
            RootAmbient = sourceItem.RootAmbient;
            ColorAreaScale = sourceItem.ColorAreaScale;
            TextureCutoff = sourceItem.TextureCutoff;
            YScale = sourceItem.YScale;
            Hue = sourceItem.Hue;

            SampleDistance = sourceItem.SampleDistance;
            UseCollisionDetection = sourceItem.UseCollisionDetection;
            VegetationSteepnessType = sourceItem.VegetationSteepnessType;
            VegetationHeightType = sourceItem.VegetationHeightType;
            MinimumSteepness = sourceItem.MinimumSteepness;
            MaximumSteepness = sourceItem.MaximumSteepness;
            SteepnessCurve = new AnimationCurve(sourceItem.SteepnessCurve.keys);
            HeightCurve = new AnimationCurve(sourceItem.HeightCurve.keys);
            MinimumHeight = sourceItem.MinimumHeight;
            MaximumHeight = sourceItem.MaximumHeight;
            UseHeightLevel = sourceItem.UseHeightLevel;
            UseAngle = sourceItem.UseAngle;
            RandomizePosition = sourceItem.RandomizePosition;
            Offset = sourceItem.Offset;
            RotationOffset = sourceItem.RotationOffset;
            Rotation = sourceItem.Rotation;

            UseVegetationMasksOnStorage = sourceItem.UseVegetationMasksOnStorage;

            UsePerlinMask = sourceItem.UsePerlinMask;
            PerlinCutoff = sourceItem.PerlinCutoff;
            PerlinScale = sourceItem.PerlinScale;
            InversePerlinMask = sourceItem.InversePerlinMask;

            UseVegetationMask = sourceItem.UseVegetationMask;
            VegetationTypeIndex = sourceItem.VegetationTypeIndex;
            Bounds = sourceItem.Bounds;
            Volume = sourceItem.Volume;

            UseIncludeTextueMask = sourceItem.UseIncludeTextueMask;
            UseExcludeTextueMask = sourceItem.UseExcludeTextueMask;

            for (var i = 0; i <= sourceItem.IncludeTextureMaskList.Count - 1; i++)
                IncludeTextureMaskList.Add(new TextureMaskInfo(sourceItem.IncludeTextureMaskList[i]));

            for (var i = 0; i <= sourceItem.ExcludeTextureMaskList.Count - 1; i++)
                ExcludeTextureMaskList.Add(new TextureMaskInfo(sourceItem.ExcludeTextureMaskList[i]));

            for (var i = 0; i <= sourceItem.ExcludeTextureMaskRuleList.Count - 1; i++)
                ExcludeTextureMaskRuleList.Add(new TextureMaskRule(sourceItem.ExcludeTextureMaskRuleList[i]));

            for (var i = 0; i <= sourceItem.IncludeTextureMaskRuleList.Count - 1; i++)
                IncludeTextureMaskRuleList.Add(new TextureMaskRule(sourceItem.IncludeTextureMaskRuleList[i]));

            for (var i = 0; i <= sourceItem.DensityTextureMaskRuleList.Count - 1; i++)
                DensityTextureMaskRuleList.Add(new TextureMaskRule(sourceItem.DensityTextureMaskRuleList[i]));

            for (var i = 0; i <= sourceItem.ScaleTextureMaskRuleList.Count - 1; i++)
                ScaleTextureMaskRuleList.Add(new TextureMaskRule(sourceItem.ScaleTextureMaskRuleList[i]));

            SteepnessCurveArray = SteepnessCurve.GenerateCurveArray();
            HeightCurveArray = HeightCurve.GenerateCurveArray();

            UseIncludeDetailMaskRules = sourceItem.UseIncludeDetailMaskRules;
            IncludeDetailLayer = sourceItem.IncludeDetailLayer;
        }
    }

    [HelpURL("http://www.awesometech.no/index.php/vegetationpackage")]
    [Serializable]
    // ReSharper disable once RequiredBaseTypesIsNotInherited
    public partial class VegetationPackage : ScriptableObject
    {
        public string PackageName = "No name";
        public List<VegetationItemInfo> VegetationInfoList = new List<VegetationItemInfo>();

        
        public bool UseTerrainTextures;
        public List<TerrainTextureInfo> TerrainTextureList = new List<TerrainTextureInfo>();

        public List<TextureMaskBase> TextureMaskList = new List<TextureMaskBase>();
        public int TerrainTextureCount = 0;

        public float MaxCurveHeight = 0;
        public bool AutomaticMaxCurveHeight = true;
        public List<TerrainTextureSettings> TerrainTextureSettingsList = new List<TerrainTextureSettings>();

        public float MaxVegetationItemHeight;

        

#if UNITY_EDITOR
        public void InitPackage()
        {
            if (TerrainTextureSettingsList == null) TerrainTextureSettingsList = new List<TerrainTextureSettings>();

            UpdateCurves();

            if (TerrainTextureList.Count == 0 && TerrainTextureCount > 0)
                LoadDefaultTextures();
            if (TerrainTextureSettingsList.Count == 0 && TerrainTextureCount > 0)
                SetupTerrainTextureSettings();

            for (var i = 0; i <= VegetationInfoList.Count - 1; i++)
                    if (VegetationInfoList[i].VegetationItemID == "")
                    {
                        VegetationInfoList[i].VegetationItemID = Guid.NewGuid().ToString();
                        EditorUtility.SetDirty(this);
                    }

            VegetationPlacingData[] vegetationPlacingDatas = GetCurrentPackagePlacingData();


            for (int i = 0; i < vegetationPlacingDatas.Length; i++)
                vegetationPlacingDatas[i].UpdatePlacingData();

        }
#endif



        /// <summary>
        /// Get the VegetationItemID from the assetGUID. an empty string will be returned if not found
        /// </summary>
        /// <param name="assetGUID"></param>
        /// <returns></returns>
        public string GetVegetationItemID(string assetGUID)
        {
            for (var i = 0; i <= VegetationInfoList.Count - 1; i++)
            {
                if (VegetationInfoList[i].VegetationGuid == assetGUID)
                {
                    return VegetationInfoList[i].VegetationItemID;
                }
            }

            return "";
        }

        /// <summary>
        /// Creates a new VegetationItem in the vegetation package. Returns the ID of the new VegetationItem. You can provide an optional vegetationItemID parameter if you want to control the ID yourself. This should be a GUID.
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="vegetationType"></param>
        /// <param name="enableRuntimeSpawn"></param>
        /// <param name="vegetationItemID"></param>
        /// <returns></returns>
        public string AddVegetationItem(GameObject prefab, VegetationType vegetationType, bool enableRuntimeSpawn, string vegetationItemID = "")
        {
            if (vegetationItemID == "") vegetationItemID = Guid.NewGuid().ToString();

            var newVegetationInfo = new VegetationItemInfo(vegetationItemID)//, vegetationType, prefab, prefab.GetInstanceID())
            {
                PrefabType = VegetationPrefabType.Mesh,
                Name = prefab.name
            };
            VegetationTypeDetector.SetDefaultVegetationInfoSettings(prefab, vegetationType, newVegetationInfo, prefab.GetInstanceID());
            newVegetationInfo.EnableRuntimeSpawn = enableRuntimeSpawn;

#if UNITY_EDITOR
            string assetPath = AssetDatabase.GetAssetPath(prefab);
            newVegetationInfo.VegetationGuid = AssetDatabase.AssetPathToGUID(assetPath);
#endif

            newVegetationInfo.RotationOffset = MeshUtils.GetMeshRotation(prefab, newVegetationInfo.LodIndex).eulerAngles;

            VegetationInfoList.Add(newVegetationInfo);
            //if (vegetationType == VegetationType.Tree)
            //    GenerateBillboard(vegetationItemID);
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);

#endif
            return vegetationItemID;
        }


        
        /// <summary>
        /// Creates a new VegetationItem in the vegetation package. Returns the ID of the new VegetationItem. You can provide an optional vegetationItemID parameter if you want to control the ID yourself. This should be a GUID.
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="vegetationType"></param>
        /// <param name="enableRuntimeSpawn"></param>
        /// <param name="vegetationItemID"></param>
        /// <returns></returns>
        public string AddVegetationItem(Texture2D texture, GameObject defaultTexturePrefab, VegetationType vegetationType, bool enableRuntimeSpawn, string vegetationItemID = "")
        {
            if (vegetationItemID == "") vegetationItemID = Guid.NewGuid().ToString();

            var newVegetationInfo = new VegetationItemInfo(vegetationItemID)
            {
                PrefabType = VegetationPrefabType.Texture,
                Name = texture.name,              
            };
            
            VegetationTypeDetector.SetDefaultVegetationInfoSettings(defaultTexturePrefab, vegetationType, newVegetationInfo,texture.GetInstanceID());

            newVegetationInfo.VegetationTexture = texture;
            newVegetationInfo.VegetationPrefab = null;
            newVegetationInfo.EnableRuntimeSpawn = enableRuntimeSpawn;

#if UNITY_EDITOR
            string assetPath = AssetDatabase.GetAssetPath(texture);
            newVegetationInfo.VegetationGuid = AssetDatabase.AssetPathToGUID(assetPath);
#endif

            VegetationInfoList.Add(newVegetationInfo);
            //if (vegetationType == VegetationType.Tree)
            //    GenerateBillboard(vegetationItemID);

#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
            return vegetationItemID;
        }

        public void GenerateBillboard(int vegetationItemIndex)
        {

#if UNITY_EDITOR
            EditorUtility.DisplayProgressBar("Generate Billboard Atlas", "Diffuse", 0);

            var assetPath = AssetDatabase.GetAssetPath(this);
            var directory = Path.GetDirectoryName(assetPath);
            var filename = Path.GetFileNameWithoutExtension(assetPath);
            var folderName = filename + "_billboards";

            if (!AssetDatabase.IsValidFolder(directory + "/" + folderName))
                AssetDatabase.CreateFolder(directory, folderName);
            var billboardID = VegetationInfoList[vegetationItemIndex].VegetationItemID;

            var billboardTexturePath = directory + "/" + folderName + "/" + "billboard_" + billboardID + ".png";
            var billboardNormalTexturePath = directory + "/" + folderName + "/" + "billboardNormal_" + billboardID + ".png";

            var billboardTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(billboardTexturePath);
            if (billboardTexture) AssetDatabase.DeleteAsset(billboardTexturePath);

            var billboardNormalTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(billboardNormalTexturePath);
            if (billboardNormalTexture) AssetDatabase.DeleteAsset(billboardNormalTexturePath);

            Quaternion rotationOffset = Quaternion.Euler(VegetationInfoList[vegetationItemIndex].RotationOffset);          

            billboardTexture = BillboardAtlasRenderer.GenerateBillboardTexture(VegetationInfoList[vegetationItemIndex].VegetationPrefab, VegetationInfoList[vegetationItemIndex].BillboardQuality, VegetationInfoList[vegetationItemIndex].BillboardLodIndex, VegetationInfoList[vegetationItemIndex].ShaderType,rotationOffset, VegetationInfoList[vegetationItemIndex].BillboardAtlasBackgroundColor);
            Texture2D paddedBillboardTexture = TextureExtention.CreatePaddedTexture(billboardTexture);
            if (paddedBillboardTexture == null)
            {
                paddedBillboardTexture = billboardTexture;
            }

            BillboardAtlasRenderer.SaveTexture(paddedBillboardTexture, directory + "/" + folderName + "/" + "billboard_" + billboardID);

            EditorUtility.DisplayProgressBar("Generate Billboard Atlas", "Normals", 0.33f);

            billboardNormalTexture = BillboardAtlasRenderer.GenerateBillboardNormalTexture(VegetationInfoList[vegetationItemIndex].VegetationPrefab, VegetationInfoList[vegetationItemIndex].BillboardQuality, VegetationInfoList[vegetationItemIndex].BillboardLodIndex,rotationOffset);
            BillboardAtlasRenderer.SaveTexture(billboardNormalTexture, directory + "/" + folderName + "/" + "billboardNormal_" + billboardID);

            EditorUtility.DisplayProgressBar("Generate Billboard Atlas", "Importing assets", 0.66f);
            AssetDatabase.ImportAsset(billboardTexturePath);
            AssetDatabase.ImportAsset(billboardNormalTexturePath);

            VegetationInfoList[vegetationItemIndex].BillboardTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(billboardTexturePath);
            VegetationInfoList[vegetationItemIndex].BillboardNormalTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(billboardNormalTexturePath);
            VegetationInfoList[vegetationItemIndex].BillboardColorSpace = PlayerSettings.colorSpace;

            BillboardAtlasRenderer.SetTextureImportSettings(VegetationInfoList[vegetationItemIndex].BillboardTexture, false);
            BillboardAtlasRenderer.SetTextureImportSettings(VegetationInfoList[vegetationItemIndex].BillboardNormalTexture, true);
            EditorUtility.ClearProgressBar();
#endif
        }

        public void GenerateBillboard(string vegetationItemID)
        {
            var vegetationItemIndex = GetVegetationItemIndexFromID(vegetationItemID);
            GenerateBillboard(vegetationItemIndex);
        }

        public void LoadTexturesFromTerrain(TerrainData sourceTerrainData)
        {
            for (var i = 0; i <= sourceTerrainData.splatPrototypes.Length - 1; i++)
                if (TerrainTextureList.Count > i)
                    if (TerrainTextureList[i] != null)
                    {
                        TerrainTextureList[i].Texture = sourceTerrainData.splatPrototypes[i].texture;
                        TerrainTextureList[i].TextureNormals = sourceTerrainData.splatPrototypes[i].normalMap;
                    }
        }

        public string GetVegetationItemIDFromIndex(int index)
        {
            if (VegetationInfoList.Count > index) return VegetationInfoList[index].VegetationItemID;
            return "";
        }

        public int GetVegetationItemIndexFromID(string id)
        {
            for (var i = 0; i <= VegetationInfoList.Count - 1; i++)
                if (VegetationInfoList[i].VegetationItemID == id) return i;
            return -1;
        }

        public VegetationItemInfo GetVegetationInfo(string id)
        {
            for (var i = 0; i <= VegetationInfoList.Count - 1; i++)
                if (VegetationInfoList[i].VegetationItemID == id) return VegetationInfoList[i];
            return null;
        }

        public void ResizeTerrainTextureList(int newCount)
        {
            if (newCount <= 0)
                TerrainTextureList.Clear();
            else
                while (TerrainTextureList.Count > newCount) TerrainTextureList.RemoveAt(TerrainTextureList.Count - 1);
        }

        public void ResizeTerrainTextureSettingsList(int newCount)
        {
            if (newCount <= 0)
                TerrainTextureSettingsList.Clear();
            else
                while (TerrainTextureSettingsList.Count > newCount) TerrainTextureSettingsList.RemoveAt(TerrainTextureSettingsList.Count - 1);
        }
        public void UpdateCurves()
        {
            for (var i = 0; i <= VegetationInfoList.Count - 1; i++)
            {
                if (VegetationInfoList[i].HeightCurve.keys.Length == 0)
                {
                    VegetationInfoList[i].HeightCurve.AddKey(0f, 1f);
                    VegetationInfoList[i].HeightCurve.AddKey(1f, 0.2f);
                 
                }

                if (VegetationInfoList[i].SteepnessCurve.keys.Length == 0)
                {
                    VegetationInfoList[i].SteepnessCurve.AddKey(0f, 1f);
                    VegetationInfoList[i].SteepnessCurve.AddKey(1f, 0f);                   
                }

                VegetationInfoList[i].HeightCurveArray =
                    VegetationInfoList[i].HeightCurve.GenerateCurveArray();

                VegetationInfoList[i].SteepnessCurveArray =
                    VegetationInfoList[i].SteepnessCurve.GenerateCurveArray();
            }
        }

#if UNITY_EDITOR
        public void LoadDefaultTextures()
        {           
            if (TerrainTextureCount == 0) return;

            if (TerrainTextureList.Count == 0)
            {
                TerrainTextureList.Add(LoadTexture("TerrainTextures/TerrainTexture1",
                    "TerrainTextures/TerrainTexture1_n", new UnityEngine.Vector2(15, 15)));
                TerrainTextureList.Add(LoadTexture("TerrainTextures/TerrainTexture2",
                    "TerrainTextures/TerrainTexture2_n", new UnityEngine.Vector2(15, 15)));
                TerrainTextureList.Add(LoadTexture("TerrainTextures/TerrainTexture3",
                    "TerrainTextures/TerrainTexture3_n", new UnityEngine.Vector2(15, 15)));
                TerrainTextureList.Add(LoadTexture("TerrainTextures/TerrainTexture4",
                    "TerrainTextures/TerrainTexture4_n", new UnityEngine.Vector2(15, 15)));
            }

            if (TerrainTextureCount == 4) return;
            if (TerrainTextureList.Count == 4)
            {
                TerrainTextureList.Add(LoadTexture("TerrainTextures/TerrainTexture5",
                    "TerrainTextures/TerrainTexture5_n", new UnityEngine.Vector2(15, 15)));
                TerrainTextureList.Add(LoadTexture("TerrainTextures/TerrainTexture6",
                    "TerrainTextures/TerrainTexture6_n", new UnityEngine.Vector2(15, 15)));
                TerrainTextureList.Add(LoadTexture("TerrainTextures/TerrainTexture7",
                    "TerrainTextures/TerrainTexture7_n", new UnityEngine.Vector2(15, 15)));
                TerrainTextureList.Add(LoadTexture("TerrainTextures/TerrainTexture8",
                    "TerrainTextures/TerrainTexture8_n", new UnityEngine.Vector2(15, 15)));
            }

            if (TerrainTextureCount == 8) return;
            if (TerrainTextureList.Count == 8)
            {
                TerrainTextureList.Add(LoadTexture("TerrainTextures/TerrainTexture9",
                    "TerrainTextures/TerrainTexture9_n", new UnityEngine.Vector2(15, 15)));
                TerrainTextureList.Add(LoadTexture("TerrainTextures/TerrainTexture10",
                    "TerrainTextures/TerrainTexture10_n", new UnityEngine.Vector2(15, 15)));
                TerrainTextureList.Add(LoadTexture("TerrainTextures/TerrainTexture11",
                    "TerrainTextures/TerrainTexture11_n", new UnityEngine.Vector2(15, 15)));
                TerrainTextureList.Add(LoadTexture("TerrainTextures/TerrainTexture12",
                    "TerrainTextures/TerrainTexture12_n", new UnityEngine.Vector2(15, 15)));
            }

            if (TerrainTextureCount == 12) return;
            if (TerrainTextureList.Count == 12)
            {
                TerrainTextureList.Add(LoadTexture("TerrainTextures/TerrainTexture13",
                    "TerrainTextures/TerrainTexture13_n", new UnityEngine.Vector2(15, 15)));
                TerrainTextureList.Add(LoadTexture("TerrainTextures/TerrainTexture14",
                    "TerrainTextures/TerrainTexture14_n", new UnityEngine.Vector2(15, 15)));
                TerrainTextureList.Add(LoadTexture("TerrainTextures/TerrainTexture15",
                    "TerrainTextures/TerrainTexture15_n", new UnityEngine.Vector2(15, 15)));
                TerrainTextureList.Add(LoadTexture("TerrainTextures/TerrainTexture16",
                    "TerrainTextures/TerrainTexture16_n", new UnityEngine.Vector2(15, 15)));
            }
        }

        public void SetupTerrainTextureSettings()
        {
            if (TerrainTextureSettingsList == null)
                TerrainTextureSettingsList = new List<TerrainTextureSettings>();
            if (TerrainTextureSettingsList.Count < TerrainTextureCount)
            {
                var currentTerrainTextureCount = TerrainTextureSettingsList.Count;

                for (var i = currentTerrainTextureCount; i <= TerrainTextureCount -1; i++)
                {
                    var terrainTextureSettings =
                        new TerrainTextureSettings {TextureHeightCurve = new AnimationCurve()};
                    terrainTextureSettings.TextureHeightCurve.AddKey(0f, 0.5f);
                    terrainTextureSettings.TextureHeightCurve.AddKey(1f, 0.5f);

                    terrainTextureSettings.TextureAngleCurve = new AnimationCurve();
                    terrainTextureSettings.TextureAngleCurve.AddKey(0f, 0.5f);
                    terrainTextureSettings.TextureAngleCurve.AddKey(1f, 0.5f);

                    terrainTextureSettings.TextureUseNoise = false;
                    terrainTextureSettings.TextureNoiseScale = 5;
                    terrainTextureSettings.TextureWeight = 1;
                    terrainTextureSettings.Enabled = i < 4;
                    terrainTextureSettings.TextureLayer = i;

                    TerrainTextureSettingsList.Add(terrainTextureSettings);
                }

                var defaultPackage = AssetDatabase.LoadAssetAtPath<VegetationPackage>("Assets/Plugins/AwesomeTechnologies/VegetationStudio/VegetationSystem/_Resources/DefaultVegetationPackage.asset");

                if (defaultPackage)
                    if (TerrainTextureSettingsList.Count > 3 && defaultPackage.TerrainTextureSettingsList.Count > 3)
                    {
                        TerrainTextureSettingsList[0].TextureHeightCurve = new AnimationCurve(defaultPackage.TerrainTextureSettingsList[0].TextureHeightCurve.keys);
                        TerrainTextureSettingsList[0].TextureAngleCurve = new AnimationCurve(defaultPackage.TerrainTextureSettingsList[0].TextureAngleCurve.keys);

                        TerrainTextureSettingsList[1].TextureHeightCurve = new AnimationCurve(defaultPackage.TerrainTextureSettingsList[1].TextureHeightCurve.keys);
                        TerrainTextureSettingsList[1].TextureAngleCurve = new AnimationCurve(defaultPackage.TerrainTextureSettingsList[1].TextureAngleCurve.keys);

                        TerrainTextureSettingsList[2].TextureHeightCurve = new AnimationCurve(defaultPackage.TerrainTextureSettingsList[2].TextureHeightCurve.keys);
                        TerrainTextureSettingsList[2].TextureAngleCurve = new AnimationCurve(defaultPackage.TerrainTextureSettingsList[2].TextureAngleCurve.keys);

                        TerrainTextureSettingsList[3].TextureHeightCurve = new AnimationCurve(defaultPackage.TerrainTextureSettingsList[3].TextureHeightCurve.keys);
                        TerrainTextureSettingsList[3].TextureAngleCurve = new AnimationCurve(defaultPackage.TerrainTextureSettingsList[3].TextureAngleCurve.keys);
                    }
            }
        }

        private static TerrainTextureInfo LoadTexture(string textureName, string normalTextureName, UnityEngine.Vector2 uv)
        {
            var newInfo = new TerrainTextureInfo
            {
                TileSize = uv,
                offset = new UnityEngine.Vector2(0, 0)
            };
            if (!string.IsNullOrEmpty(textureName)) newInfo.Texture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Plugins/AwesomeTechnologies/VegetationStudio/Common/_Resources/" + textureName + ".tga");
            if (!string.IsNullOrEmpty(normalTextureName)) newInfo.TextureNormals = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Plugins/AwesomeTechnologies/VegetationStudio/Common/_Resources/" + normalTextureName + ".tga");
            return newInfo;
        }
#endif
    }
}
