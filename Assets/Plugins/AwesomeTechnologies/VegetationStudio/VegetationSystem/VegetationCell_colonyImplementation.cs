using System.Collections.Generic;
using UnityEngine;
using AwesomeTechnologies.Utility;
using AwesomeTechnologies.Utility.Quadtree;
using AwesomeTechnologies.Vegetation;
using AwesomeTechnologies.Vegetation.PersistentStorage;
using UnityEngine.Profiling;

namespace AwesomeTechnologies
{

    public partial class VegetationCell : IHasRect
    {
        public VegetationPlacingData currentVegetationPlacingData;

        public VegetationItemInfo[] GetItemsInfo()
        {
            if (currentVegetationPlacingData == null)
                return CurrentvegetationPackage.VegetationInfoList.ToArray();
            else
                return currentVegetationPlacingData.VegetationInfoList;
        }

        public VegetationItemInfo SetItem(int index)
        {
            if (currentVegetationPlacingData == null)
                return CurrentvegetationPackage.VegetationInfoList[index];
            else
                return currentVegetationPlacingData.VegetationInfoList[index];
        }
        public VegetationItemInfo GetVegetationInfo(string id)
        {

            for (var i = 0; i <= GetItemsInfo().Length - 1; i++)
                if (GetItemsInfo()[i].VegetationItemID == id)
                    return SetItem(i);
            return null;
        }
    }
}
