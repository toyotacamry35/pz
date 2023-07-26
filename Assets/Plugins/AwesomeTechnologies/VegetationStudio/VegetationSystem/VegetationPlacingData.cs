using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AwesomeTechnologies
{
  
    public class VegetationPlacingData : ScriptableObject
    {
        public VegetationPackage parentPackage;
        public string PlacingDataName = "No name";
        public VegetationItemInfo[] VegetationInfoList;

#if UNITY_EDITOR
        public void UpdatePlacingData()
        {
            List<VegetationItemInfo> tempInfo = new List<VegetationItemInfo>();
            bool[] sourceInfo = new bool[parentPackage.VegetationInfoList.Count];

            List<VegetationItemInfo> dataInfo = new List<VegetationItemInfo>();

            tempInfo.AddRange(VegetationInfoList);

            for (int i=0; i<parentPackage.VegetationInfoList.Count; i++)
            {
                VegetationItemInfo newInfo = tempInfo.Find(Client => Client.VegetationItemID == parentPackage.VegetationInfoList[i].VegetationItemID);
                if (newInfo!=null)
                {
                    dataInfo.Add(newInfo);
                    sourceInfo[i] = true;
                }
            }
            
            for (int i=0; i< parentPackage.VegetationInfoList.Count; i++)
                if (!sourceInfo[i])
                    dataInfo.Add(parentPackage.VegetationInfoList[i]);
            VegetationInfoList = dataInfo.ToArray();
        }
#endif

        public VegetationItemInfo GetVegetationInfo(string id)
        {
            for (var i = 0; i < VegetationInfoList.Length; i++)
            {
                if (VegetationInfoList[i].VegetationItemID == id)
                    return VegetationInfoList[i];
            }
            return null;
        }
    }
}
