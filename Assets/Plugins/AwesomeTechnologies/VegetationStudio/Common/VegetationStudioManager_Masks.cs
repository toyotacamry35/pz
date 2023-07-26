using System.Collections.Generic;
using AwesomeTechnologies.Utility.Quadtree;
using UnityEngine;

namespace AwesomeTechnologies.VegetationStudio
{
    public partial class VegetationStudioManager : MonoBehaviour
    {
        private readonly List<BaseMaskArea> _vegetationMaskList = new List<BaseMaskArea>();
        public static void AddVegetationMask(BaseMaskArea maskArea)
        {
            if (!Instance) FindInstance();
            if (Instance) Instance.Instance_AddVegetationMask(maskArea);
        }

        public void OnAddVegetationSystem(VegetationSystem vegetationSystem)
        {
            vegetationSystem.OnVegetationCellRefreshDelegate += OnVegetationCellRefresh;
        }

        public void OnRemoveVegetationSystem(VegetationSystem vegetationSystem)
        {
            vegetationSystem.OnVegetationCellRefreshDelegate -= OnVegetationCellRefresh;
        }

        public void OnVegetationCellRefresh(VegetationSystem vegetationSystem)
        {
            for (int i = 0; i <= _vegetationMaskList.Count - 1; i++)
            {
                AddVegetationMaskToVegetationSystem(vegetationSystem, _vegetationMaskList[i]);
            }
        }

        public static void RemoveVegetationMask(BaseMaskArea maskArea)
        {
            if (!Instance) FindInstance();
            if (Instance) Instance.Instance_RemoveVegetationMask(maskArea);
        }

    private static void AddVegetationMaskToVegetationSystem(VegetationSystem vegetationSystem, BaseMaskArea maskArea)
        {         
            Rect maskRect = RectExtension.CreateRectFromBounds(maskArea.MaskBounds);
            if (vegetationSystem.VegetationCellQuadTree != null)
            {
                List<VegetationCell> selectedCellList = vegetationSystem.VegetationCellQuadTree.Query(maskRect);

                for (int i = 0; i <= selectedCellList.Count - 1; i++)
                {
                    selectedCellList[i].AddVegetationMask(maskArea);
                    vegetationSystem.RefreshVegetationBillboards(maskArea.MaskBounds);
                    vegetationSystem.SetDirty();
                }
            }      
        }

        protected void Instance_AddVegetationMask(BaseMaskArea maskArea)
        {
            if (!_vegetationMaskList.Contains(maskArea))
            {
                _vegetationMaskList.Add(maskArea);
            }

            for (int i = 0; i <= VegetationSystemList.Count - 1; i++)
            {
                AddVegetationMaskToVegetationSystem(VegetationSystemList[i], maskArea);
            }
        }

        protected void Instance_RemoveVegetationMask(BaseMaskArea maskArea)
        {
            _vegetationMaskList.Remove(maskArea);
            maskArea.CallDeleteEvent();
            for (int i = 0; i <= VegetationSystemList.Count - 1; i++)
            {
                if (VegetationSystemList[i] != null)
                {
                    VegetationSystemList[i].RefreshVegetationBillboards(maskArea.MaskBounds);
                    VegetationSystemList[i].SetDirty();
                }
            }
        }
    }
}
