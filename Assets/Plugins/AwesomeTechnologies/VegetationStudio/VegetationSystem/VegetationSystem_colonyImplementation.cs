    using System.Collections.Generic;
    using AwesomeTechnologies.Vegetation;
    using UnityEngine;
    using AwesomeTechnologies.VegetationStudio;

namespace AwesomeTechnologies
{
    public partial class VegetationSystem
    {
        public bool isRenderRuleInstances = true;
        public bool isActivePlacingData = false;

        public VegetationPlacingData currentVegetationPlacingData;
        
        public LayerDetails[] layerDetails;
        public LayerDetails other;
        public LayerDetails disabled;

        [ContextMenu("Switch")]
        public void GetAdvanced()
        {
            VegetationStudioManager.isAdvancedMode = !VegetationStudioManager.isAdvancedMode;
            //VegetationStudioManager.Instance?.UpdateIcons(this);
        }

        public VegetationItemInfo[] GetItemsInfo()
        {
            if (currentVegetationPlacingData == null)
                return currentVegetationPackage.VegetationInfoList.ToArray();
            else
                return currentVegetationPlacingData.VegetationInfoList;
        }

        public VegetationItemInfo GetVegetationInfo(string id)
        {

            for (var i = 0; i <= GetItemsInfo().Length - 1; i++)
                if (GetItemsInfo()[i].VegetationItemID == id)
                    return SetItem(i);
            return null;
        }

        public VegetationItemInfo SetItem(int index)
        {
            if (currentVegetationPlacingData == null)
                return currentVegetationPackage.VegetationInfoList[index];
            else
                return currentVegetationPlacingData.VegetationInfoList[index];
        }

        private void Awake()
        {
            vegetationSettings.GrassLayer = LayerMask.NameToLayer("Grass");
            vegetationSettings.PlantLayer = LayerMask.NameToLayer("Grass");
            vegetationSettings.PlantShadows = true;
            vegetationSettings.EditorPlantShadows = true;
            UseListMultithreading = true;
        }
        public void RefreshPreset()
        {
            //Debug.Log("Refresh Preset");
            if (VegetationPackageList == null)
                return;

            if (VegetationPackageList.Count <= VegetationPackageIndex)
                return;

            if (VegetationPackageList[VegetationPackageIndex] == null)
                return;

            int textureCount = VegetationPackageList[VegetationPackageIndex].TerrainTextureCount;
            layerDetails = new LayerDetails[textureCount];


            for (int i = 0; i < textureCount; i++)
            {
                layerDetails[i] = new LayerDetails();

                for (int j = 0; j < GetItemsInfo().Length; j++)
                {           
                        if (GetItemsInfo()[j].IncludeDetailLayer == i)
                        {
                            if (currentVegetationPlacingData == null)
                                currentVegetationPackage.VegetationInfoList[j].SwitchToDetail(i);
                            else
                                currentVegetationPlacingData.VegetationInfoList[j].SwitchToDetail(i);
                            layerDetails[i].details.Add(j);
                        }                 
                }
            }

            other = new LayerDetails();

            for (int j = 0; j < GetItemsInfo().Length; j++)
            {
                if (GetItemsInfo()[j].IncludeDetailLayer == -1)
                {
                    if (currentVegetationPlacingData == null)
                        currentVegetationPackage.VegetationInfoList[j].SwitchToManual();
                    else
                        currentVegetationPlacingData.VegetationInfoList[j].SwitchToManual();
                    other.details.Add(j);
                }
            }

            disabled = new LayerDetails();

            for (int j = 0; j < GetItemsInfo().Length; j++)
            {
                if (GetItemsInfo()[j].IncludeDetailLayer < -1)
                {
                    if (currentVegetationPlacingData == null)
                        currentVegetationPackage.VegetationInfoList[j].SwitchToDisabled();
                    else
                        currentVegetationPlacingData.VegetationInfoList[j].SwitchToDisabled();
                    disabled.details.Add(j);
                }
            }

        }

        
        
         
         
    }
}