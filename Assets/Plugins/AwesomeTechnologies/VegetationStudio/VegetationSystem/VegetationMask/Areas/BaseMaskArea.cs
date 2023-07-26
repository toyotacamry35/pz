using System.Collections.Generic;
using UnityEngine;

namespace AwesomeTechnologies
{
    public class BaseMaskArea
    {
        public Bounds MaskBounds;

        public bool RemoveGrass = true;
        public bool RemovePlants = true;
        public bool RemoveTrees = true;
        public bool RemoveObjects = true;
        public bool RemoveLargeObjects = true;
        public float AdditionalGrassWidth = 0;
        public float AdditionalPlantWidth = 0;
        public float AdditionalTreeWidth = 0;
        public float AdditionalObjectWidth = 0;
        public float AdditionalLargeObjectWidth = 0;

        public float AdditionalGrassWidthMax = 0;
        public float AdditionalPlantWidthMax = 0;
        public float AdditionalTreeWidthMax = 0;
        public float AdditionalObjectWidthMax = 0;
        public float AdditionalLargeObjectWidthMax = 0;

        public float NoiseScaleGrass = 5;
        public float NoiseScalePlant = 5;
        public float NoiseScaleTree = 5;
        public float NoiseScaleObject = 5;
        public float NoiseScaleLargeObject = 5;

        public List<VegetationTypeSettings> VegetationTypeList = new List<VegetationTypeSettings>();

        public delegate void MultionMaskDeleteDelegate(BaseMaskArea baseMaskArea);
        public MultionMaskDeleteDelegate OnMaskDeleteDelegate;

        public virtual bool Contains(Vector3 point, VegetationType vegetationType, bool useAdditionalDistance, bool useExcludeFilter)
        {
            return false;
        }

        public virtual bool ContainsMask(Vector3 point, VegetationType vegetationType, VegetationTypeIndex vegetationTypeIndex, ref float size, ref float density)
        {
            bool hasVegetationType = HasVegetationType(vegetationTypeIndex,ref size,ref density);
            if (!hasVegetationType) return false;
            return Contains(point, vegetationType, false, false);
        }


        public bool HasVegetationType(VegetationTypeIndex vegetationTypeIndex, ref float size, ref float density)
        {
            for (int i = 0; i <= VegetationTypeList.Count - 1; i++)
            {
                if (VegetationTypeList[i].Index == vegetationTypeIndex)
                {
                    size = VegetationTypeList[i].Size;
                    density = VegetationTypeList[i].Density;
                    return true;
                }
            }

            return false;
        }

        public void CallDeleteEvent()
        {
            if (OnMaskDeleteDelegate != null) OnMaskDeleteDelegate(this);
        }

        public float GetMaxAdditionalDistance()
        {
            float[] values = { AdditionalGrassWidth, AdditionalPlantWidth, AdditionalTreeWidth, AdditionalObjectWidth,AdditionalLargeObjectWidth, AdditionalGrassWidthMax, AdditionalPlantWidthMax, AdditionalTreeWidthMax, AdditionalObjectWidthMax, AdditionalLargeObjectWidthMax };
            return Mathf.Max(values) * 1.5f;
        }

        public float SamplePerlinNoise(Vector3 point, float perlinNoiceScale)
        {
            return Mathf.PerlinNoise((point.x) / perlinNoiceScale, (point.z) / perlinNoiceScale);
        }
    }
}
