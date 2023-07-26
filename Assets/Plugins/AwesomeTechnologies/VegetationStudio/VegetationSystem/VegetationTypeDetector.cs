using UnityEngine;

namespace AwesomeTechnologies.Vegetation
{
    public class VegetationTypeDetector
    {
        public static void SetDefaultVegetationInfoSettings(GameObject prefab, VegetationType vegetationType,
            VegetationItemInfo vegetationItemInfo, int seed)
        {           
            Random.InitState(Mathf.RoundToInt(Time.realtimeSinceStartup)+seed);

            vegetationItemInfo.VegetationType = vegetationType;
            vegetationItemInfo.VegetationPrefab = prefab;

            if (vegetationItemInfo.PrefabType == VegetationPrefabType.Mesh)
            {
                vegetationItemInfo.Name = prefab.name;
            }

            switch (vegetationType)
            {
                case VegetationType.Grass:
                    vegetationItemInfo.EnableRuntimeSpawn = true;
                    vegetationItemInfo.VegetationRenderType = VegetationRenderType.InstancedIndirect;
                    vegetationItemInfo.DisableShadows = true;
                    vegetationItemInfo.Rotation = VegetationRotationType.FollowTerrainScale;
                    vegetationItemInfo.UseIncludeDetailMaskRules = true;
                    vegetationItemInfo.SampleDistance = 0.3f;
                    vegetationItemInfo.Density = 0.65f;
                    break;
                case VegetationType.Plant:
                    vegetationItemInfo.EnableRuntimeSpawn = true;
                    vegetationItemInfo.VegetationRenderType = VegetationRenderType.InstancedIndirect;
                    vegetationItemInfo.DisableShadows = false;
                    vegetationItemInfo.Rotation = VegetationRotationType.FollowTerrain;
                    vegetationItemInfo.UseIncludeDetailMaskRules = true;
                    vegetationItemInfo.SampleDistance = 0.3f;
                    vegetationItemInfo.Density = 0.65f;
                    break;
                case VegetationType.Objects:
                    vegetationItemInfo.EnableRuntimeSpawn = true;
                    vegetationItemInfo.VegetationRenderType = VegetationRenderType.Instanced;
                    vegetationItemInfo.DisableShadows = true;
                    vegetationItemInfo.SwitchToDisabled();
                    vegetationItemInfo.Rotation = VegetationRotationType.FollowTerrain;
                    vegetationItemInfo.UseIncludeDetailMaskRules = false;
                    vegetationItemInfo.SampleDistance = 1.5f;
                    vegetationItemInfo.Density = 0.35f;
                    break;
                case VegetationType.Tree:
                    vegetationItemInfo.EnableRuntimeSpawn = false;
                    vegetationItemInfo.VegetationRenderType = VegetationRenderType.InstancedIndirect;
                    vegetationItemInfo.DisableShadows = false;
                    vegetationItemInfo.SwitchToDisabled();
                    vegetationItemInfo.Rotation = VegetationRotationType.RotateY;
                    vegetationItemInfo.UseIncludeDetailMaskRules = false;
                    vegetationItemInfo.SampleDistance = 1.5f;
                    vegetationItemInfo.Density = 0.35f;
                    break;
            }

            vegetationItemInfo.VegetationScaleType = VegetationScaleType.Simple;
            vegetationItemInfo.MinScale = 1.0f;
            vegetationItemInfo.MaxScale = 2.0f;
            vegetationItemInfo.MinVectorScale = new Vector3(0.8f, 0.8f, 0.8f);
            vegetationItemInfo.MaxVectorScale = new Vector3(2.0f, 2.0f, 2.0f);
            vegetationItemInfo.DisableLOD = true;
            vegetationItemInfo.ColliderType = ColliderType.Disabled;
            vegetationItemInfo.NavMeshObstacleType = NavMeshObstacleType.Disabled;
            vegetationItemInfo.UseTouchReact = true;
            vegetationItemInfo.UseBillboards = false;

            vegetationItemInfo.ColorTint1 = Color.white;
            vegetationItemInfo.ColorTint2 = Color.white;
            vegetationItemInfo.Brightness = 1f;
            vegetationItemInfo.RandomDarkening = 0.31f;
            vegetationItemInfo.RootAmbient = 0.63f;
            vegetationItemInfo.ColorAreaScale = 100f;
            vegetationItemInfo.Hue = new Color(1f, 0.5f, 0f, 25f / 256f);

            vegetationItemInfo.TextureCutoff = 0.2f;

            vegetationItemInfo.YScale = 1f;

            vegetationItemInfo.SampleDistance = 0.3f;
            vegetationItemInfo.Density = 0.65f;
            vegetationItemInfo.RotationOffset = new Vector3(10f, 10f, 10f);
            vegetationItemInfo.UseHeightLevel = false;
            vegetationItemInfo.UseAngle = false;
            vegetationItemInfo.RandomizePosition = true;
            vegetationItemInfo.UsePerlinMask = false;
            vegetationItemInfo.LodIndex = 2;

            /*
            vegetationItemInfo.VegetationHeightType = VegetationHeightType.Simple;
            vegetationItemInfo.UseHeightLevel = true;
            vegetationItemInfo.MinimumHeight = 0f;
            vegetationItemInfo.MaximumHeight = 1000f;
            vegetationItemInfo.LodIndex = 2;
            vegetationItemInfo.VegetationRenderType = VegetationRenderType.Instanced;
            vegetationItemInfo.ColliderType = ColliderType.Disabled;
            vegetationItemInfo.RandomizePosition = true;

            vegetationItemInfo.UseAngle = true;
            vegetationItemInfo.VegetationSteepnessType = VegetationSteepnessType.Simple;
            vegetationItemInfo.MinimumSteepness = 0;

            vegetationItemInfo.MaxScale = 0.8f;
            vegetationItemInfo.MaxScale = 1.2f;

            vegetationItemInfo.UsePerlinMask = true;
            vegetationItemInfo.PerlinCutoff = Random.Range(0.4f, 0.6f);
            vegetationItemInfo.InversePerlinMask = RandomBoolean();
            vegetationItemInfo.Seed = Random.Range(0, 100);
            vegetationItemInfo.ShaderType = GetVegetationShaderType(prefab);
            vegetationItemInfo.PerlinScale = Random.Range(3, 10);

            switch (vegetationItemInfo.ShaderType)
            {
                case VegetationShaderType.VegetationStudioGrass:
                    vegetationItemInfo.VegetationRenderType = VegetationRenderType.InstancedIndirect;
                    vegetationItemInfo.LodIndex = 2;

                    Material material = GetVegetationItemMaterial(prefab);
                    if (material)
                    {
                        vegetationItemInfo.ColorTint1 = material.GetColor("_Color");
                        vegetationItemInfo.ColorTint2 = material.GetColor("_ColorB");
                        vegetationItemInfo.TextureCutoff = material.GetFloat("_Cutoff");

                        vegetationItemInfo.RandomDarkening = material.GetFloat("_RandomDarkening");
                        vegetationItemInfo.RootAmbient = material.GetFloat("_RootAmbient");
                        vegetationItemInfo.ColorAreaScale = material.GetVector("_AG_ColorNoiseArea").y;
                    }
                    break;
            }

            switch (vegetationType)
            {
                case VegetationType.Grass:
                    vegetationItemInfo.Rotation = VegetationRotationType.FollowTerrainScale;
                    vegetationItemInfo.MaximumSteepness = 35f;
                    vegetationItemInfo.SampleDistance = Random.Range(0.8f, 1.2f);
                    break;

                case VegetationType.Plant:
                    vegetationItemInfo.Rotation = VegetationRotationType.RotateY;
                    vegetationItemInfo.MaximumSteepness = 35f;
                    vegetationItemInfo.SampleDistance = Random.Range(1.8f, 2.2f);
                    vegetationItemInfo.MinScale = 1.6f;
                    vegetationItemInfo.MaxScale = 2.2f;
                    break;

                case VegetationType.Tree:
                    vegetationItemInfo.Rotation = VegetationRotationType.RotateY;
                    vegetationItemInfo.SampleDistance = Random.Range(5f, 20f);
                    vegetationItemInfo.MaximumSteepness = 25f;
                    vegetationItemInfo.UseBillboards = false;
                    //vegetationItemInfo.BillboardQuality = BillboardQuality.High;
                    break;

                case VegetationType.Objects:
                    vegetationItemInfo.Rotation = VegetationRotationType.RotateY;
                    vegetationItemInfo.SampleDistance = Random.Range(5f, 7f);
                    vegetationItemInfo.MaximumSteepness = 25f;
                    break;
                case VegetationType.LargeObjects:
                    vegetationItemInfo.Rotation = VegetationRotationType.RotateY;
                    vegetationItemInfo.SampleDistance = Random.Range(8f, 10f);
                    vegetationItemInfo.MaximumSteepness = 25f;
                    break;
            }
            */
        }

        public static Material GetVegetationItemMaterial(GameObject prefab)
        {
            GameObject selectedVegetationModel = MeshUtils.SelectMeshObject(prefab, MeshType.Normal, 0);
            MeshRenderer meshrenderer = selectedVegetationModel.GetComponent<MeshRenderer>();
            if (meshrenderer)
                return meshrenderer.sharedMaterial;

            return null;
        }

        public static VegetationShaderType GetVegetationShaderType(GameObject prefab)
        {
            GameObject selectedVegetationModel = MeshUtils.SelectMeshObject(prefab, MeshType.Normal, 0);
            if (selectedVegetationModel)
            {
                MeshRenderer meshrenderer = selectedVegetationModel.GetComponentInChildren<MeshRenderer>();
                if (meshrenderer && meshrenderer.sharedMaterial)
                {
                    Shader shader = meshrenderer.sharedMaterial.shader;

                    return GetVegetationShaderTypeFromName(shader.name);
                }
            }           
            return VegetationShaderType.Other;
        }

        public static VegetationShaderType GetVegetationShaderTypeFromName(string name)
        {
            switch (name)
            {
                case "AwesomeTechnologies/AGrassStandard":
                    return VegetationShaderType.Grass;
                case "AwesomeTechnologies/AFoliageStandard":
                    return VegetationShaderType.Foliage;
                case "AwesomeTechnologies/ASpeedTreeStandard":
                    return VegetationShaderType.Speedtree;
                default:
                    return VegetationShaderType.Other;
            }
        }

        public static void SetDefaultTexture3DSettings(Texture2D texture, VegetationType vegetationType,
            VegetationItemInfo vegetationItemInfo)
        {
        }

        public static bool RandomBoolean()
        {
            if (Random.value >= 0.5)
                return true;
            return false;
        }
    }
}


