using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AwesomeTechnologies.Vegetation.PersistentStorage
{
#if PERSISTENT_VEGETATION
    public partial class TerrainTreeImporter : IVegetationImporter
    {

        private Terrain _sourceTerrain;
        private int _selectedGridIndex;
        private PersistentVegetationStorage _persistentVegetationStorage;

        public string ImporterName
        {
            get
            {
               return "Terrain tree importer";
            }
        }

        public PersistentVegetationStoragePackage PersistentVegetationStoragePackage { get; set; }
        public VegetationPackage VegetationPackage { get; set; }

        public PersistentVegetationStorage PersistentVegetationStorage
        {
            get
            {
                return _persistentVegetationStorage;
            }

            set
            {
                if (_sourceTerrain == null && _persistentVegetationStorage)
                {
                    _sourceTerrain = _persistentVegetationStorage.VegetationSystem.currentTerrain;
                }

                _persistentVegetationStorage = value;
            }
        }

        public void ImportTrees(List<GameObject> prefabList)
        {
            for (int i = 0; i <= prefabList.Count - 1; i++)
            {
                bool isContains = false;
                int selectContain = -1;
                for (int j = 0; j < VegetationPackage.VegetationInfoList.Count; j++)
                {
                    if (VegetationPackage.VegetationInfoList[j].VegetationPrefab.name == prefabList[i].name)
                    {
                        isContains = true;
                        selectContain = j;
                        break;
                    }
                }

                if (!isContains)
                {
                    string vegetationItemId = VegetationPackage.AddVegetationItem(prefabList[i], VegetationType.Tree, false);

                    for (int j = 0; j <= _sourceTerrain.terrainData.treeInstances.Length - 1; j++)
                    {
                        TreeInstance treeInstance = _sourceTerrain.terrainData.treeInstances[j];
                        if (treeInstance.prototypeIndex == i)
                        {
                            Vector3 position = new Vector3(treeInstance.position.x * _sourceTerrain.terrainData.size.x,
                                treeInstance.position.y * _sourceTerrain.terrainData.size.y,
                                treeInstance.position.z * _sourceTerrain.terrainData.size.z) + _sourceTerrain.transform.position;

                            PersistentVegetationStorage.AddVegetationItemInstance(vegetationItemId, position,
                                new Vector3(treeInstance.widthScale, treeInstance.heightScale, treeInstance.widthScale),
                                Quaternion.Euler(0, treeInstance.rotation * Mathf.Rad2Deg, 0), true, 2);
                        }
                    }
                }
                else
                {
                    for (int j = 0; j <= _sourceTerrain.terrainData.treeInstances.Length - 1; j++)
                    {
                        TreeInstance treeInstance = _sourceTerrain.terrainData.treeInstances[j];
                        if (treeInstance.prototypeIndex == i)
                        {
                            Vector3 position = new Vector3(treeInstance.position.x * _sourceTerrain.terrainData.size.x,
                                treeInstance.position.y * _sourceTerrain.terrainData.size.y,
                                treeInstance.position.z * _sourceTerrain.terrainData.size.z) + _sourceTerrain.transform.position;

                            PersistentVegetationStorage.AddVegetationItemInstance(VegetationPackage.VegetationInfoList[selectContain].VegetationItemID, position,
                                new Vector3(treeInstance.widthScale, treeInstance.heightScale, treeInstance.widthScale),
                                Quaternion.Euler(0, treeInstance.rotation * Mathf.Rad2Deg, 0), true, 2);
                        }
                    }
                }
            }
        }

        /*

        public void OnGUI()
        {
            GUILayout.BeginVertical("box");
            var labelStyle = new GUIStyle("Label") { fontStyle = FontStyle.Italic};

            _sourceTerrain = EditorGUILayout.ObjectField("Source terrain", _sourceTerrain, typeof(Terrain), true) as Terrain;

            if (_sourceTerrain != null)
            {
                List<GameObject> prefabList = new List<GameObject>();
                for (int i = 0; i <= _sourceTerrain.terrainData.treePrototypes.Length - 1; i++)
                {
                    TreePrototype treePrototype = _sourceTerrain.terrainData.treePrototypes[i];
                    prefabList.Add(treePrototype.prefab);
                }

                if (prefabList.Count > 0)
                {
                    EditorGUILayout.LabelField("Total trees: " + _sourceTerrain.terrainData.treeInstanceCount, labelStyle);

                    VegetationPackageEditorTools.DrawPrefabSelectorGrid(prefabList,60,ref _selectedGridIndex);

                    if (GUILayout.Button("Import trees"))
                    {
                        ImportTrees(prefabList);
                        PersistentVegetationStorage.VegetationSystem.SetVegetationPackage(PersistentVegetationStorage.VegetationSystem.VegetationPackageIndex, false);
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("The terrain contains no trees.", MessageType.Info);
                }                 
            }
            else
            {
                EditorGUILayout.HelpBox("Select a terrain to import trees from.", MessageType.Info);
            }
            GUILayout.EndVertical();
        }
        */

}
#endif
}
