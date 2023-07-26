using System.Collections.Generic;
using UnityEngine;

namespace AwesomeTechnologies.Billboards
{
    [ExecuteInEditMode]
    public class BakedBillboardController : MonoBehaviour
    {
        public List<Material> VegetationItemBillboardMaterialList = new List<Material>();
        public VegetationSystem VegetationSystem;
        public BillboardSystem BillboardSystem;

        public void AddMaterialList(List<Material> materialList)
        {
            VegetationItemBillboardMaterialList.Clear();
            for (int i = 0; i <= materialList.Count - 1; i++)
            {
                VegetationItemBillboardMaterialList.Add(materialList[i]);
            }
        }

        // ReSharper disable once UnusedMember.Local
        private void OnRenderObject()
        {
            if (!Application.isPlaying)
                UpdateBillboardClipping();
        }

        // ReSharper disable once UnusedMember.Local
        private void LateUpdate()
        {
            UpdateBillboardClipping();            
        }

        private void UpdateBillboardClipping()
        {           
            if (!BillboardSystem || !VegetationSystem) return;
            if (!VegetationSystem.InitDone) return;

            var cullDistance = Mathf.RoundToInt(VegetationSystem.GetVegetationDistance() +
                                                VegetationSystem.GetTreeDistance() - VegetationSystem.CellSize);
            var farCullDistance = Mathf.RoundToInt(VegetationSystem.GetTotalDistance());

            for (var i = 0; i <= VegetationItemBillboardMaterialList.Count - 1; i++)
            {
                if (VegetationItemBillboardMaterialList[i] == null) continue;
                if (VegetationSystem)
                {
                    VegetationItemInfo vegetationItemInfo =
                        VegetationSystem.currentVegetationPackage.VegetationInfoList[i];

                    VegetationItemBillboardMaterialList[i].SetFloat("_Cutoff", vegetationItemInfo.BillboardCutoff);
                VegetationItemBillboardMaterialList[i].SetInt("_CullDistance", cullDistance);
                VegetationItemBillboardMaterialList[i].SetInt("_FarCullDistance", farCullDistance);
              

                    VegetationItemBillboardMaterialList[i]
                        .SetVector("_CameraPosition", VegetationSystem.GetCameraPosition());

                    if (vegetationItemInfo.ShaderType ==
                        VegetationShaderType.Speedtree)
                    {
                        Color tempColor = vegetationItemInfo.ColorTint1;
                        tempColor.r = Mathf.Clamp01(tempColor.r * 1.3f);
                        tempColor.g = Mathf.Clamp01(tempColor.g * 1.3f);
                        tempColor.b = Mathf.Clamp01(tempColor.b * 1.3f);
                        VegetationItemBillboardMaterialList[i].SetColor("_Color", tempColor);
                        VegetationItemBillboardMaterialList[i].SetColor("_HueVariation", vegetationItemInfo.Hue);
                    }
                }
            }
        }
    }
}
