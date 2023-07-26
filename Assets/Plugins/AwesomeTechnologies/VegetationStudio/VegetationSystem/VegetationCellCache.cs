using AwesomeTechnologies.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace AwesomeTechnologies.Vegetation
{
    public class VegetationCellCache
    {
        public VegetationPackage CurrentVegetationPackage;
        public VegetationSettings CurrentVegetationSettings;
        public float CellSize;

        public List<List<Matrix4x4>> VegetationInfoList;
        public List<CustomList<Matrix4x4>> VegetationInverseInfoList;

        public void InitCache()
        {
            VegetationInfoList = new List<List<Matrix4x4>>(CurrentVegetationPackage.VegetationInfoList.Count);
            VegetationInverseInfoList = new List<CustomList<Matrix4x4>>(CurrentVegetationPackage.VegetationInfoList.Count);

            for (int i = 0; i <= CurrentVegetationPackage.VegetationInfoList.Count - 1; i++)
            {
                float density = VegetationSettings.GetVegetationItemDensity(CurrentVegetationPackage.VegetationInfoList[i].VegetationType, CurrentVegetationSettings);
                if (density < 0.01) density = 0.01f;

                float sampleDistance = Mathf.Clamp(CurrentVegetationPackage.VegetationInfoList[i].SampleDistance / density, 0.1f, CellSize/2f);

                int xSamples = Mathf.CeilToInt(CellSize / sampleDistance);
                int zSamples = Mathf.CeilToInt(CellSize / sampleDistance);

                int maxSize = xSamples * zSamples;

                if (!Application.isPlaying)
                {
                    VegetationInfoList.Add(new List<Matrix4x4>(maxSize));
                    VegetationInverseInfoList.Add(new CustomList<Matrix4x4>(0));
                }
                else
                {                   
                    if (CurrentVegetationPackage.VegetationInfoList[i].VegetationRenderType ==
                        VegetationRenderType.InstancedIndirect)
                    {
                        VegetationInfoList.Add(new List<Matrix4x4>(0));
                        VegetationInverseInfoList.Add(new CustomList<Matrix4x4>(maxSize));
                    }
                    else
                    {
                        VegetationInfoList.Add(new List<Matrix4x4>(maxSize));
                        VegetationInverseInfoList.Add(new CustomList<Matrix4x4>(0));
                    }
                }
            }
        }

        public void ClearCache()
        {
            for (int i = 0; i <= CurrentVegetationPackage.VegetationInfoList.Count - 1; i++)
            {
                VegetationInfoList[i].Clear();
                VegetationInverseInfoList[i].Clear();
            }
        }
    }
}
