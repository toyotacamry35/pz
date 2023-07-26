using System.Collections.Generic;
using UnityEngine;

namespace AwesomeTechnologies.Vegetation
{
    public class VegetationCachePool
    {
        public VegetationPackage VegetationPackage;
        public VegetationSettings VegetationSettings;
        public int CacheCellCount;
        public float CellSize;

        private readonly List<VegetationCellCache> _availableCellCacheList = new List<VegetationCellCache>();
        private readonly List<VegetationCellCache> _inUseCellCacheList = new List<VegetationCellCache>();

        //private readonly List<VegetationCellCache> _inUseVisibleCellCacheList = new List<VegetationCellCache>();
        //private readonly List<VegetationCellCache> _inUseInvissibleCellCacheList = new List<VegetationCellCache>();

        public void InitCachePool()
        {
            ClearCachePool();

            float cellsMaxDistance = (VegetationSettings.VegetationDistance + VegetationSettings.TreeDistance) / CellSize;
            CacheCellCount = Mathf.CeilToInt(cellsMaxDistance * cellsMaxDistance * 4);

            for (int i = 0; i <= CacheCellCount - 1; i++)
            {
                VegetationCellCache vegetationCellCache =
                    new VegetationCellCache
                    {
                        CurrentVegetationPackage = VegetationPackage,
                        CurrentVegetationSettings = VegetationSettings,
                        CellSize = CellSize                        
                    };
                vegetationCellCache.InitCache();
                _availableCellCacheList.Add(vegetationCellCache);
            }
        }

        public void ClearCachePool()
        {
            
        }

        public void OnVegetationCellVisible(VegetationCell vegetationCell, int distanceBand)
        {
            
        }

        public void OnVegetationCellInvisible(VegetationCell vegetationCell, int distanceBand)
        {

        }
        public VegetationCellCache GetVegetationCellCache()
        {
            if (_availableCellCacheList.Count > 0)
            {
                VegetationCellCache vegetationCellCache = _availableCellCacheList[_availableCellCacheList.Count - 1];
                _availableCellCacheList.Remove(vegetationCellCache);
                _inUseCellCacheList.Add(vegetationCellCache);
                return vegetationCellCache;
            }
            else
            {
                VegetationCellCache vegetationCellCache =
                    new VegetationCellCache
                    {
                        CurrentVegetationPackage = VegetationPackage,
                        CurrentVegetationSettings = VegetationSettings,
                        CellSize = CellSize
                    };
                vegetationCellCache.InitCache();
                _inUseCellCacheList.Add(vegetationCellCache);
                return vegetationCellCache;
            }
        }

        public void ReleaseVegetationCellCache(VegetationCellCache vegetationCellCache)
        {
            if (_inUseCellCacheList.Contains(vegetationCellCache))
            {
                _inUseCellCacheList.Remove(vegetationCellCache);
                vegetationCellCache.ClearCache();
                _availableCellCacheList.Add(vegetationCellCache);
            }
        }
    }
}
