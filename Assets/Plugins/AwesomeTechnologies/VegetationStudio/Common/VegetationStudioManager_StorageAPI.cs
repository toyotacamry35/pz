using System;
using System.Collections.Generic;
using AwesomeTechnologies.Vegetation.PersistentStorage;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace AwesomeTechnologies.VegetationStudio
{
    public partial class VegetationStudioManager
    {
        /// <summary>
        /// Add a new vegetation item to all vegetation packages used in the scene. If multiple terrains is using the same package it will only be added once. 
        /// Set includeInTerrain to false if item is to be used from a PersistentVegetationStorage
        /// </summary>
        /// <param name="prefab">The prefab containing the VegetationMesh with material you want to add to the VegetationPackage</param>
        /// <param name="vegetationType">Type of vegetation. This will affect default rules and LOD behaviour.</param>
        /// <param name="enableRuntimeSpawn">Set this to true in order to enable runtime spawning of the new Vegetation Item.</param>
        /// <returns>Returns the ID of the new VegetaionItem added to the VegetationPackage</returns>
        public static string AddVegetationItem(GameObject prefab, VegetationType vegetationType, bool enableRuntimeSpawn)
        {
            if (!Instance) FindInstance();

            if (Instance)
            {
                string newVegetationItemID = System.Guid.NewGuid().ToString();
                List<VegetationPackage> vegetationPackageList = GetVegetationPackageList();

                for (int i = 0; i <= vegetationPackageList.Count - 1; i++)
                {                   
                    vegetationPackageList[i]
                        .AddVegetationItem(prefab, vegetationType, enableRuntimeSpawn, newVegetationItemID);
                }
                RefreshVegetationPackage();
                return newVegetationItemID;
            }
            return "";
        }

        /// <summary>
        /// Get the VegetationItemID from the assetGUID. an empty string will be returned if not found
        /// </summary>
        /// <param name="assetGUID"></param>
        /// <returns>returns the VegetationItemID of the prefab/texture with matching assetGUID</returns>
        public static string GetVegetationItemID(string assetGUID)
        {
            if (!Instance) FindInstance();

            if (Instance)
            {               
                List<VegetationPackage> vegetationPackageList = GetVegetationPackageList();
                for (int i = 0; i <= vegetationPackageList.Count - 1; i++)
                {
                    string vegetationitemID = vegetationPackageList[i].GetVegetationItemID(assetGUID);
                    if (vegetationitemID != "") return vegetationitemID;
                }
            }
            return "";
        }

        /// <summary>
        /// This will add a new instance of a vegetationItem to the PersistentVegetationStorage. The VegetationItemID is the one you get from the AddVegetationItem function or from an existing item in the VegetationPackage. You provide it with worldspace position and a vegetationSourceID. See PersistentVegetationStorageTools.cs for info about the vegetationSourceID.
        /// </summary>
        /// <param name="vegetationItemID"></param>
        /// <param name="worldPosition"></param>
        /// <param name="scale"></param>
        /// <param name="rotation"></param>
        /// <param name="applyMeshRotation"></param>
        /// <param name="vegetationSourceID"></param>
        /// <param name="clearCellCache"></param>
        public static void AddVegetationItemInstance(string vegetationItemID, Vector3 worldPosition, Vector3 scale, Quaternion rotation,bool applyMeshRotation, byte vegetationSourceID, bool clearCellCache = true)
        {
            if (!Instance) FindInstance();
#if PERSISTENT_VEGETATION
            if (Instance)
            {
                for (int i = 0; i <= Instance.VegetationSystemList.Count - 1; i++)
                {
                    PersistentVegetationStorage persistentVegetationStorage = Instance.VegetationSystemList[i].GetComponent<PersistentVegetationStorage>();
                    if (persistentVegetationStorage)
                    {
                        persistentVegetationStorage.AddVegetationItemInstance(vegetationItemID, worldPosition, scale, rotation, applyMeshRotation, vegetationSourceID, clearCellCache);
                    }
                }
            }
#endif
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vegetationItemID"></param>
        /// <param name="worldPosition"></param>
        /// <param name="minimumDistance"></param>
        /// <param name="clearCellCache"></param>
        public static void RemoveVegetationItemInstance(string vegetationItemID, Vector3 origin, Vector3 worldPosition, float minimumDistance, bool clearCellCache = true)
        {
            if (!Instance) FindInstance();
#if PERSISTENT_VEGETATION
            if (Instance)
            {
                for (int i = 0; i <= Instance.VegetationSystemList.Count - 1; i++)
                {
                    PersistentVegetationStorage persistentVegetationStorage = Instance.VegetationSystemList[i].GetComponent<PersistentVegetationStorage>();
                    if (persistentVegetationStorage)
                    {
                        persistentVegetationStorage.RemoveVegetationItemInstance(vegetationItemID, origin, worldPosition, minimumDistance, clearCellCache);
                    }
                }
            }
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vegetationItemID"></param>
        /// <param name="vegetationSourceID"></param>
        public static void RemoveVegetationItemInstances(string vegetationItemID, byte vegetationSourceID)
        {
            if (!Instance) FindInstance();
#if PERSISTENT_VEGETATION
            if (Instance)
            {
                for (int i = 0; i <= Instance.VegetationSystemList.Count - 1; i++)
                {
                    PersistentVegetationStorage persistentVegetationStorage = Instance.VegetationSystemList[i].GetComponent<PersistentVegetationStorage>();
                    if (persistentVegetationStorage)
                    {
                        persistentVegetationStorage.RemoveVegetationItemInstances(vegetationItemID,vegetationSourceID);
                    }
                }
            }
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vegetationItemID"></param>
        public static void RemoveVegetationItemInstances(string vegetationItemID)
        {
            if (!Instance) FindInstance();
#if PERSISTENT_VEGETATION
            if (Instance)
            {
                for (int i = 0; i <= Instance.VegetationSystemList.Count - 1; i++)
                {
                    PersistentVegetationStorage persistentVegetationStorage = Instance.VegetationSystemList[i].GetComponent<PersistentVegetationStorage>();
                    if (persistentVegetationStorage)
                    {
                        persistentVegetationStorage.RemoveVegetationItemInstances(vegetationItemID);
                    }
                }
            }
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vegetationItemID"></param>
        /// <param name="worldPosition"></param>
        /// <param name="scale"></param>
        /// <param name="rotation"></param>
        /// <param name="vegetationSourceID"></param>
        /// <param name="minimumDistance"></param>
        /// <param name="clearCellCache"></param>
        public static void AddVegetationItemInstanceEx(string vegetationItemID, Vector3 worldPosition, Vector3 scale, Quaternion rotation, byte vegetationSourceID, float minimumDistance, bool clearCellCache = true)
        {
            if (!Instance) FindInstance();
#if PERSISTENT_VEGETATION
            if (Instance)
            {
                for (int i = 0; i <= Instance.VegetationSystemList.Count - 1; i++)
                {
                    PersistentVegetationStorage persistentVegetationStorage = Instance.VegetationSystemList[i].GetComponent<PersistentVegetationStorage>();
                    if (persistentVegetationStorage)
                    {
                        persistentVegetationStorage.AddVegetationItemInstanceEx(vegetationItemID, worldPosition, scale, rotation,
                            vegetationSourceID,minimumDistance, clearCellCache);
                    }
                }
            }
#endif
        }


        /// <summary>
        /// Refresh all VegetationPackages on all active terrains.
        /// </summary>
        public static void RefreshVegetationPackage()
        {
            if (!Instance) FindInstance();

            if (Instance)
            {
                for (int i = 0; i <= Instance.VegetationSystemList.Count - 1; i++)
                {
                    Instance.VegetationSystemList[i].RefreshVegetationPackage();
                }
            }
        }

        /// <summary>
        /// Returns a unique list of all VegetationPackages used in the scene. 
        /// </summary>
        /// <returns></returns>
        public static List<VegetationPackage> GetVegetationPackageList()
        {
            List<VegetationPackage> vegetationPackageList = new List<VegetationPackage>();

            if (!Instance) FindInstance();

            if (!Instance) return vegetationPackageList;

            for (int i = 0; i <= Instance.VegetationSystemList.Count - 1; i++)
            {
                if (!Instance.VegetationSystemList[i].currentVegetationPackage) continue;
                if (!vegetationPackageList.Contains(Instance.VegetationSystemList[i].currentVegetationPackage))
                {
                    vegetationPackageList.Add(Instance.VegetationSystemList[i].currentVegetationPackage);
                }
            }

            return vegetationPackageList;
        }

        /// <summary>
        /// Will clear VegetationItemInstances of a VegetationType within an area.
        /// </summary>
        /// <param name="vegetationItemID"></param>
        /// <param name="bounds"></param>
        public static void ClearVegetationItemInstancesArea(string vegetationItemID, Bounds bounds)
        {
            Debug.Log("Not implemented");
        }

        /// <summary>
        /// Will clear VegetationItemInstances of a VegetationType from a VegetationSource within an area.
        /// </summary>
        /// <param name="vegetationItemID"></param>
        /// <param name="vegetationSourceID"></param>
        /// <param name="bounds"></param>
        public static void ClearVegetationItemInstancesArea(string vegetationItemID, byte vegetationSourceID, Bounds bounds)
        {
            Debug.Log("Not implemented");
        }
    }
}
