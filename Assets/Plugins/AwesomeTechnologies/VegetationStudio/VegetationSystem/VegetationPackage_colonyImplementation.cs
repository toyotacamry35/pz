using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AwesomeTechnologies.Common.Interfaces;
using AwesomeTechnologies.Utility;
using AwesomeTechnologies.Utility.Extentions;
using AwesomeTechnologies.Vegetation;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace AwesomeTechnologies
{
    public partial class VegetationPackage : ScriptableObject
    {
#if UNITY_EDITOR
        public VegetationPlacingData[] GetCurrentPackagePlacingData()
        {
            if (Application.isPlaying)
                throw new InvalidOperationException("This should never be used in play mode");

            List<VegetationPlacingData> vpd = new List<VegetationPlacingData>();

            string[] guids;

            // search for a ScriptObject called ScriptObj
            guids = AssetDatabase.FindAssets("t:VegetationPlacingData");
            foreach (string guid in guids)
            {
                VegetationPlacingData newAsset = AssetDatabase.LoadAssetAtPath<VegetationPlacingData>(AssetDatabase.GUIDToAssetPath(guid));
                if (newAsset != null)
                {
                    if (newAsset.parentPackage == this)
                    {
                        vpd.Add(newAsset);
                    }
                }
            }

            return vpd.ToArray();
        }
#endif

    }


}
