using UnityEngine;
using UnityEditor;

namespace Assets.Src.Cartographer.Editor
{
    public class ImportModelProcessor : AssetPostprocessor
    {
        void OnPreprocessModel()
        {
            string secretCode = GUIUtility.systemCopyBuffer;

            if (secretCode.Length < 3) return;

            if (secretCode.Substring(0, 2) != "UE") return;

            int type = int.Parse(secretCode.Substring(2, 1));

            switch (type)
            {
                case 0:
                {
                    ModelImporter mi = assetImporter as ModelImporter;
                    mi.importMaterials = false;
                }
                break;
                case 1:
                {
                    ModelImporter mi = assetImporter as ModelImporter;
                    mi.materialName = ModelImporterMaterialName.BasedOnMaterialName;
                }
                break;
            }
        }

        void OnPostprocessModel(GameObject fbx)
        {
            string secretCode = GUIUtility.systemCopyBuffer;

            if (secretCode.Length < 3) return;

            if (secretCode.Substring(0, 2) == "EE")
            {
                ModelImporter mp = assetImporter as ModelImporter;
                string assetPath = GetFolder(mp);
                string assetName = GetName(mp);

                FileUtil.DeleteFileOrDirectory(assetPath + "/" + assetName + ".fbm");
                GUIUtility.systemCopyBuffer = "";
                return;
            }

            if (secretCode.Substring(0, 2) != "UE") return;

            int type = int.Parse(secretCode.Substring(2, 1));

            if (!assetImporter) return;

            ModelImporter mi = assetImporter as ModelImporter;

            if (!mi) return;

            mi.isReadable = false;
            mi.useFileScale = false;
            mi.importBlendShapes = false;
            mi.weldVertices = false;
            mi.importVisibility = false;
            mi.importCameras = false;
            mi.importLights = false;
            mi.importAnimation = false;

            switch (type)
            {
                case 0:
                {
                    break;
                }
                case 1:
                {
                    string assetPath = GetFolder(mi);
                    string assetName = GetName(mi);

                    mi.ExtractTextures(assetPath + "/Textures");
                    mi.importMaterials = false;

                    AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                    AssetDatabase.SaveAssets();
                    GUIUtility.systemCopyBuffer = "EE1";

                    FileUtil.DeleteFileOrDirectory(assetPath + "/" + assetName + ".fbm");
                    AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                    AssetDatabase.SaveAssets();
                }
                break;
            }
        }

        string GetFolder(ModelImporter current)
        {
            string currentPath = AssetDatabase.GetAssetPath(current);
            int index = currentPath.LastIndexOf("/");
            string result = currentPath.Substring(0, index);

            return result;
        }

        string GetName(ModelImporter current)
        {
            string currentPath = AssetDatabase.GetAssetPath(current);
            int index = currentPath.LastIndexOf("/");
            string assetNameFbx = currentPath.Substring(index + 1);
            string result = assetNameFbx.Substring(0, assetNameFbx.Length - 4);

            return result;
        }
    }
}