using UnityEngine;

namespace AwesomeTechnologies
{

    public enum ShadowCullinMode
    {
        None = 0,
        Normal =1,
        Long   =2,
        VeryLong=3,
        Max =  4

    }

    [System.Serializable]
    public class VegetationSettings
    {
        
        public float WaterLevel;
        public int RandomSeed;
        public float VegetationDistance = 100;
        public float TreeDistance = 300;
        public float BillboardDistance = 20000;
        public float GrassDensity = 1;
        public float PlantDensity = 1;
        public float TreeDensity = 1;
        public float ObjectDensity = 1;
        public float LargeObjectDensity = 1;
        public float VegetationScale = 1f;
        public float DistanceLod0;
        public float DistanceLod1;
        public float DistanceLod2;
        public float DistanceLod3;
        public float DistanceBillboards;
        public float LODFadeDistance = 20;
        public LayerMask GrassLayer = 0;
        public LayerMask PlantLayer = 0;
        public LayerMask TreeLayer = 0;
        public LayerMask ObjectLayer = 0;
        public LayerMask LargeObjectLayer = 0;
        public bool GrassShadows = false;
        public bool PlantShadows = false;
        public bool TreeShadows = true;
        public bool ObjectShadows = false;
        public bool LargeObjectShadows = true;
        public bool isRenderRuleInstances = true;

        public bool EditorGrassShadows = false;
        public bool EditorPlantShadows = false;
        public bool EditorTreeShadows = true;
        public bool EditorObjectShadows = false;
        public bool EditorLargeObjectShadows = true;

        public bool InstancedGIGrass = false;
        public bool InstancedGIPlant = false;
        public bool InstancedGITree = false;
        public bool InstancedGIObject = false;
        public bool InstancedGILargeObject= false;
        public bool InstancedOcclusionProbes = false;
        
        public ShadowCullinMode ShadowCullinMode = ShadowCullinMode.Long;

        public bool UseTouchReact = true;

        public float GrassWetnesss = 0;
        public float WindRange = 100f;
        //public bool UseDLL = true;

        public static float GetVegetationItemDensity(VegetationType vegetationType, VegetationSettings vegetationSettings)
        {
            switch (vegetationType)
            {
                case VegetationType.Grass:
                    return vegetationSettings.GrassDensity;
                case VegetationType.Plant:
                    return vegetationSettings.PlantDensity;
                case VegetationType.Tree:
                    return vegetationSettings.TreeDensity;
                case VegetationType.Objects:
                    return vegetationSettings.ObjectDensity;
                case VegetationType.LargeObjects:
                    return vegetationSettings.LargeObjectDensity;
                default:
                    return 1;
            }
        }

    }
}
