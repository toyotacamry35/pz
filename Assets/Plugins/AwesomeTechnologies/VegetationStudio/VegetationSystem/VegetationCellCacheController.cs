using System.Collections.Generic;

namespace AwesomeTechnologies.Vegetation
{
    public class VegetationCellCacheController
    {
        private readonly List<bool> _clearCacheList = new List<bool>();
        private readonly VegetationPackage _vegetationPackage;

        public VegetationCellCacheController(VegetationPackage vegetationPackage)
        {
            _vegetationPackage = vegetationPackage;
            InitClearCacheList();
        }

        private void InitClearCacheList()
        {
            _clearCacheList.Clear();
            for (int i = 0; i <= _vegetationPackage.VegetationInfoList.Count - 1; i++)
            {
                _clearCacheList.Add(false);
            }
        }

        public void ResetClearCacheList()
        {
            for (int i = 0; i <= _clearCacheList.Count - 1; i++)
            {
                _clearCacheList[i] = false;
            }
        }

        public void ClearVegetationItem(int index)
        {
            _clearCacheList[index] = true;
        }

        public void ClearPlants()
        {
            for (int i = 0; i <= _vegetationPackage.VegetationInfoList.Count - 1; i++)
            {
                if (_vegetationPackage.VegetationInfoList[i].VegetationType == VegetationType.Grass ||
                    _vegetationPackage.VegetationInfoList[i].VegetationType == VegetationType.Plant ||
                    _vegetationPackage.VegetationInfoList[i].VegetationType == VegetationType.Objects)
                {
                    _clearCacheList[i] = true;
                }
            }
        }

        public void ClearTrees()
        {
            for (int i = 0; i <= _vegetationPackage.VegetationInfoList.Count - 1; i++)
            {
                if (_vegetationPackage.VegetationInfoList[i].VegetationType == VegetationType.Tree)
                {
                    _clearCacheList[i] = true;
                }
            }
        }

        public void ClearEverything()
        {
            for (int i = 0; i <= _vegetationPackage.VegetationInfoList.Count - 1; i++)
            {
                _clearCacheList[i] = true;               
            }
        }
    }
}
