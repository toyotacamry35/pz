using System.IO;
using AwesomeTechnologies.Billboards;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace AwesomeTechnologies
{
    public partial class VegetationSystem
    {
        #region Billboards

//        public void GenerateBillboard(int vegetationItemIndex)
//        {
//#if UNITY_EDITOR
//            if (!CurrentVegetationPackage) return;

//            string assetPath = AssetDatabase.GetAssetPath(CurrentVegetationPackage);
//            string directory = Path.GetDirectoryName(assetPath);
//            string filename = Path.GetFileNameWithoutExtension(assetPath);
//            string folderName = filename + "_billboards";

//            if (!AssetDatabase.IsValidFolder(directory + "/" + folderName))
//            {
//                AssetDatabase.CreateFolder(directory, folderName);
//            }
//            string billboardID = CurrentVegetationPackage.VegetationInfoList[vegetationItemIndex].VegetationItemID;

//            string billboardTexturePath = directory + "/" + folderName + "/" + "billboard_" + billboardID + ".png";
//            string billboardNormalTexturePath = directory + "/" + folderName + "/" + "billboardNormal_" + billboardID + ".png";
              
//            Texture2D billboardTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(billboardTexturePath);
//            if (billboardTexture) AssetDatabase.DeleteAsset(billboardTexturePath);

//            Texture2D billboardNormalTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(billboardNormalTexturePath);
//            if (billboardNormalTexture) AssetDatabase.DeleteAsset(billboardNormalTexturePath);

//            billboardTexture = BillboardAtlasRenderer.GenerateBillboardTexture(CurrentVegetationPackage.VegetationInfoList[vegetationItemIndex].VegetationPrefab, CurrentVegetationPackage.VegetationInfoList[vegetationItemIndex].BillboardQuality, CurrentVegetationPackage.VegetationInfoList[vegetationItemIndex].BillboardLodIndex);
//            BillboardAtlasRenderer.SaveTexture(billboardTexture, directory + "/" + folderName + "/" + "billboard_" + billboardID);

//            billboardNormalTexture = BillboardAtlasRenderer.GenerateBillboardNormalTexture(CurrentVegetationPackage.VegetationInfoList[vegetationItemIndex].VegetationPrefab, CurrentVegetationPackage.VegetationInfoList[vegetationItemIndex].BillboardQuality, CurrentVegetationPackage.VegetationInfoList[vegetationItemIndex].BillboardLodIndex);
//            BillboardAtlasRenderer.SaveTexture(billboardNormalTexture, directory + "/" + folderName + "/" + "billboardNormal_" + billboardID);
//            //AssetDatabase.CreateAsset(BillboardNormalTexture, billboardNormalTexturePath);

//            AssetDatabase.ImportAsset(billboardTexturePath);
//            AssetDatabase.ImportAsset(billboardNormalTexturePath);
//            //AssetDatabase.Refresh();
//            CurrentVegetationPackage.VegetationInfoList[vegetationItemIndex].BillboardTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(billboardTexturePath);
//            CurrentVegetationPackage.VegetationInfoList[vegetationItemIndex].BillboardNormalTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(billboardNormalTexturePath);

//            BillboardAtlasRenderer.SetTextureImportSettings(CurrentVegetationPackage.VegetationInfoList[vegetationItemIndex].BillboardTexture, false);
//            BillboardAtlasRenderer.SetTextureImportSettings(CurrentVegetationPackage.VegetationInfoList[vegetationItemIndex].BillboardNormalTexture, true);
//#endif
//        }


        void SetBillboardShaderVariables()
        {
            var cullDistance = Mathf.RoundToInt(GetVegetationDistance() +
                                                GetTreeDistance() - CellSize);
            var farCullDistance = Mathf.RoundToInt(GetTotalDistance());
            Shader.SetGlobalFloat("_VS_CullDistance", cullDistance);
            Shader.SetGlobalInt("_VS_FarCullDistance", farCullDistance);
            Shader.SetGlobalVector("_VS_CameraPosition", GetCameraPosition());
        }

        public void RefreshBillboards()
        {
            BillboardSystem billboardSystem = gameObject.GetComponent<BillboardSystem>();
            if (billboardSystem)
            {
                billboardSystem.CreateBillboards();
            }
        }

        #endregion
    }
}
