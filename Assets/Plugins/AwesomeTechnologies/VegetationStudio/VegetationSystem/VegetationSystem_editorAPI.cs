#if UNITY_EDITOR
using UnityEditor;
#endif
using AwesomeTechnologies.Billboards;
using AwesomeTechnologies.VegetationStudio;
using UnityEngine;

namespace AwesomeTechnologies
{
    public partial class VegetationSystem
    {
        public bool ShowColliderMenu = false;
        public bool ShowNavMeshObstacleMenu = false;
        public bool ShowShadowsMenu = false;
        public bool ShowLODMenu = false;
        public bool ShowSpeedtreeMenu = false;
        public bool ShowTextureMaskRulesMenu = false;
        public bool ShowVegetationMaskMenu = false;
        public bool ShowDetailMaskMenu = false;
        public bool ShowTerrainTextureRulesMenu = false;
        public bool ShowPositionMenu = true;
        public bool ShowScaleMenu = true;
        public bool ShowRotationMenu = true;
        public bool ShowHeightMenu = true;
        public bool ShowSteepnessMenu = true;
        public bool ShowPerlinMenu = false;

        public bool ShowBillboardsMenu = false;
        public bool ShowVegetationItemGrassMenu = false;
        public bool ShowTouchReactMenu = false;

        public bool ShowGlobalVegetationItemSettingsMenu = false;
        public bool ShowAddVegetationItemMenu = false;

        private float _clearVegetationCellCacheTriggerTime;
        private bool _triggerClearVegetationCellCache;

        void EnableEditorApi()
        {
            //Debug.Log("FF1");
#if UNITY_EDITOR
            EditorApplication.update += UpdateEditorCallback;
#endif
        }

        void DisableEditorApi()
        {
            //Debug.Log("FFF");
#if UNITY_EDITOR
            EditorApplication.update -= UpdateEditorCallback;            
#endif
        }

        void UpdateEditorCallback()
        {
            if (!_triggerClearVegetationCellCache) return;
            if (!(Time.realtimeSinceStartup - _clearVegetationCellCacheTriggerTime > 1f)) return;

            ClearVegetationCellCache();
            VegetationStudioManager.VegetationPackageSync_ClearVegetationSystemCellCache(this);
            _triggerClearVegetationCellCache = false;

            //Debug.Log("Editor");
            
            /*
#if UNITY_EDITOR
            BillboardSystem billboardSystem = GetComponent<BillboardSystem>();
            if (billboardSystem)
            {
                billboardSystem.CreateBillboards();
            }             
#endif
*/
        }
    
        public void DelayedClearVegetationCellCache()
        {
            if (ManualVegetationRefresh) return;

            _clearVegetationCellCacheTriggerTime = Time.realtimeSinceStartup;
            _triggerClearVegetationCellCache = true;
        }

        public void DelayedSetGrassDensity(float density)
        {
            vegetationSettings.GrassDensity = density;
            DelayedClearVegetationCellCache();
        }

        public void DelayedSetVegetationScale(float scale)
        {
            vegetationSettings.VegetationScale = scale;
            DelayedClearVegetationCellCache();
        }

        public void DelayedSetPlantDensity(float density)
        {
            vegetationSettings.PlantDensity = density;
            DelayedClearVegetationCellCache();
        }
      
        public void DelayedSetTreeDensity(float density)
        {
            vegetationSettings.TreeDensity = density;
            DelayedClearVegetationCellCache();
        }

        public void DelayedSetObjectDensity(float density)
        {
            vegetationSettings.ObjectDensity = density;
            DelayedClearVegetationCellCache();
        }

        public void DelayedSetLargeObjectDensity(float density)
        {
            vegetationSettings.LargeObjectDensity = density;
            DelayedClearVegetationCellCache();
        }

        public float GetGrassDensity()
        {
            return vegetationSettings.GrassDensity;
        }
        public float GetPlantDensity()
        {
            return vegetationSettings.PlantDensity;
        }

        public float GetTreeDensity()
        {
            return vegetationSettings.TreeDensity;
        }

        public float GetObjectDensity()
        {
            return vegetationSettings.ObjectDensity;
        }

        public float GetLargeObjectDensity()
        {
            return vegetationSettings.LargeObjectDensity;
        }

        public void SetGrassDensity(float density)
        {
            vegetationSettings.GrassDensity = density;
            ClearVegetationCellCache();
        }
        public void SetPlantDensity(float density)
        {
            vegetationSettings.PlantDensity = density;
            ClearVegetationCellCache();
        }

        public void SetTreeDensity(float density)
        {
            vegetationSettings.TreeDensity = density;
            ClearVegetationCellCache();
        }

        public void SetObjectDensity(float density)
        {
            vegetationSettings.ObjectDensity = density;
            ClearVegetationCellCache();
        }

        public void SetLargeObjectDensity(float density)
        {
            vegetationSettings.LargeObjectDensity = density;
            ClearVegetationCellCache();
        }

        //if (_clearVegetationCellCacheCoroutine != null)
        //{
        //    StopCoroutine(_clearVegetationCellCacheCoroutine);
        //    _clearVegetationCellCacheCoroutine = null;
        //}
        //_clearVegetationCellCacheCoroutine = StartCoroutine(WaitClearVegetationCellCache());


        //        private IEnumerator WaitClearVegetationCellCache()
        //        {
        //            yield return new WaitForSeconds(1f);
        //            ClearVegetationCellCache();
        //            _clearVegetationCellCacheCoroutine = null;

        //#if UNITY_EDITOR
        //            EditorUtility.SetDirty(this);
        //            SceneView.RepaintAll();
        //            if (!Application.isPlaying)
        //            {
        //                ExecuteRenderVegetation(true);
        //                Debug.Log("Execute render vegetation");
        //            }
        //#endif
        //        }

        //private IEnumerator WaitAndSetVegetationPackage(int index, bool resetCullingGroup)
        //{
        //    yield return new WaitForSeconds(1f);
        //    SetVegetationPackage(index, resetCullingGroup);
        //    _setVegetationPackageCoroutine = null;
        //}
    }
}
