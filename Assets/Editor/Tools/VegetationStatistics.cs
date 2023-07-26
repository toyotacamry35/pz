using AwesomeTechnologies.Vegetation.PersistentStorage;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor.Tools
{
    public class VegetationStatistics : ScriptableObject
    {
        [MenuItem("Level Design/Grass/Statistics", priority = 10)]
        public static void CollectStatistics()
        {
            var vegetationStorages = FindObjectsOfType<PersistentVegetationStorage>();
            var statististicsList = new List<ItemStatistics>();

            foreach (var vegetationStorage in vegetationStorages)
            {
                List<PersistentVegetationInstanceInfo> instanceList = vegetationStorage.PersistentVegetationStoragePackage.GetPersistentVegetationInstanceInfoList();


                for (int i = 0; i < instanceList.Count; i++)
                {
                    var itemStatistics = new ItemStatistics();
                    itemStatistics.prefab = vegetationStorage.VegetationSystem.currentVegetationPackage.GetVegetationInfo(instanceList[i].VegetationItemID).VegetationPrefab;
                    itemStatistics.totalCount += instanceList[i].Count;

                    for (int j = 0; j < instanceList[i].SourceCountList.Count; j++)
                    {
                        switch (instanceList[i].SourceCountList[j].VegetationSourceID)
                        {
                            case (byte)0:
                                itemStatistics.generatedCount += instanceList[i].SourceCountList[j].Count;
                                break;
                            case (byte)1:
                                itemStatistics.paintedCount += instanceList[i].SourceCountList[j].Count;
                                break;
                            case (byte)5:
                                itemStatistics.paintedCount += instanceList[i].SourceCountList[j].Count;
                                break;
                        }
                    }
                    statististicsList.Add(itemStatistics);
                }

                foreach (var statistics in statististicsList)
                {
                    Debug.Log($"Prefab: '{statistics.prefab.name}',\nPainted: '{statistics.paintedCount}', \tGenerated: '{statistics.generatedCount}'", statistics.prefab);
                }
            }
        }

        private class ItemStatistics
        {
            public int totalCount;
            public int paintedCount;
            public int generatedCount;
            public GameObject prefab;
        }
    }
}
