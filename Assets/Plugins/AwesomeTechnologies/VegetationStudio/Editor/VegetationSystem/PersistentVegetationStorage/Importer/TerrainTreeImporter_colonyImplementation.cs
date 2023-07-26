using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AwesomeTechnologies.Vegetation.PersistentStorage
{
#if PERSISTENT_VEGETATION
    public partial class TerrainTreeImporter : IVegetationImporter
    {
        public void OnGUI()
        {

            if (_sourceTerrain==null)
                _sourceTerrain = PersistentVegetationStorage.VegetationSystem.UnityTerrainData.terrain;

            if (GUILayout.Button("Import trees: " + _sourceTerrain.terrainData.treeInstanceCount))
            {
                ImportTrees();
            }
        }


        public void ImportTrees()
        {
                List<GameObject> prefabList = new List<GameObject>();
                for (int i = 0; i <= _sourceTerrain.terrainData.treePrototypes.Length - 1; i++)
                {
                    TreePrototype treePrototype = _sourceTerrain.terrainData.treePrototypes[i];
                    prefabList.Add(treePrototype.prefab);
                }

                if (prefabList.Count > 0)
                {
                    List<string> vegetationItemIdList = VegetationPackageEditorTools.CreateVegetationInfoIdList(PersistentVegetationStorage.VegetationSystem.currentVegetationPackage);

                    for (int i = 0; i <= vegetationItemIdList.Count - 1; i++)
                    {
                        PersistentVegetationStorage.RemoveVegetationItemInstances(vegetationItemIdList[i], 2);
                    }

                    ImportTrees(prefabList);
                    PersistentVegetationStorage.VegetationSystem.SetVegetationPackage(PersistentVegetationStorage.VegetationSystem.VegetationPackageIndex, false);
                }
        }
    }
#endif
}
