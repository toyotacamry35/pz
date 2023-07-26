using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace AwesomeTechnologies.Billboards
{
    [CustomEditor(typeof(BillboardSystem))]
    public class BillboardSystemEditor : VegetationStudioBaseEditor
    {
        private int _currentTabIndex;
        private static readonly string[] TabNames =
        {
            "Settings", "Editor","Baked billboards", "Advanced","Debug"
        };

        private BillboardSystem _billboardSystem;
        public override void OnInspectorGUI()
        {
            OverrideLogoTextureName = "SectionBanner_BillboardSystem";
            HelpTopic = "home/vegetation-studio/components/billboard-system";
            _billboardSystem = (BillboardSystem)target;
            ShowLogo = !_billboardSystem.VegetationSystem.GetSleepMode();

            base.OnInspectorGUI();

            if (_billboardSystem.VegetationSystem.GetSleepMode())
            {
                EditorGUILayout.HelpBox("Wake up the Vegetation System from sleepmode to edit settings", MessageType.Info);
                return;
            }

            _currentTabIndex = GUILayout.SelectionGrid(_currentTabIndex, TabNames, 3, EditorStyles.toolbarButton);

            if (!_billboardSystem.VegetationSystem.InitDone)
            {
                GUILayout.BeginVertical("box");
                EditorGUILayout.HelpBox("Vegetation system component has configuration errors. Fix to enable component.", MessageType.Error);
                GUILayout.EndVertical();
                return;
            }

            switch (_currentTabIndex)
            {
                case 0:
                    DrawSettingsInspector();
                    break;
                case 1:
                    DrawEditorInspector();
                    break;
                case 2:
                    DrawBakedBillboardsInspector();
                    break;
                case 3:
                    DrawAdvancedInspector();
                    break;
                case 4:
                    DrawDebugInspector();
                    break;
            }
        }

        private void DrawAdvancedInspector()
        {
            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Advanced render settings", LabelStyle);

            EditorGUI.BeginChangeCheck();
            _billboardSystem.OverrideRenderQueue = EditorGUILayout.Toggle("Override render queue", _billboardSystem.OverrideRenderQueue);

            if (_billboardSystem.OverrideRenderQueue)
            {
                _billboardSystem.RenderQueue = EditorGUILayout.IntField("RenderQueue", _billboardSystem.RenderQueue);
            }

            _billboardSystem.OverrideLayer = EditorGUILayout.Toggle("Override render layer", _billboardSystem.OverrideLayer);
            if (_billboardSystem.OverrideLayer)
            {
                _billboardSystem.BillboardLayer = EditorGUILayout.LayerField("Billboard layer", _billboardSystem.BillboardLayer);
            }

            if (EditorGUI.EndChangeCheck())
            {
                _billboardSystem.SetupBillboardCells();
                _billboardSystem.SetupCullingGroup();
                EditorUtility.SetDirty(target);
            }

            GUILayout.EndVertical();
        }

        private void DrawBakedBillboardsInspector()
        {
            GUILayout.BeginVertical("box");

            EditorGUILayout.LabelField("Settings", LabelStyle);
            _billboardSystem.TerrainBakeParrent =
                EditorGUILayout.Toggle("Use terrain as parent", _billboardSystem.TerrainBakeParrent);
            _billboardSystem.BakeToProject =
                EditorGUILayout.Toggle("Save to project", _billboardSystem.BakeToProject);
            _billboardSystem.ClearBakedBillboardsOnBake =
                EditorGUILayout.Toggle("Clear existing on bake", _billboardSystem.ClearBakedBillboardsOnBake);

            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");

            if (GUILayout.Button("Bake billboards"))
            {
                //string path = EditorUtility.SaveFolderPanel("Save meshes/materials to folder", "", "");
                //if (path.Length != 0)
                {
                    //Debug.Log(path);
                    if (_billboardSystem.TerrainBakeParrent)
                    {
                        _billboardSystem.BakeBillboards(_billboardSystem.BakeToProject, _billboardSystem.ClearBakedBillboardsOnBake,
                            _billboardSystem.VegetationSystem.currentTerrain.transform);
                    }
                    else
                    {
                        _billboardSystem.BakeBillboards(_billboardSystem.BakeToProject, _billboardSystem.ClearBakedBillboardsOnBake);
                    }

                    if (!Application.isPlaying)
                    {
                        EditorSceneManager.MarkSceneDirty(_billboardSystem.TerrainBakeParrent
                            ? _billboardSystem.VegetationSystem.currentTerrain.gameObject.scene
                            : _billboardSystem.gameObject.scene);
                    }
                }
            }
            EditorGUILayout.HelpBox("Baking billboards will pre-generate all billboard meshes for the terrain. They will not change when rules changes or VegetationMasks move run-time, but loading is much faster. This also disables dynamic billboards.", MessageType.Info);

            if (GUILayout.Button("Clear baked billboards"))
            {
                _billboardSystem.ClearBakedBillboards();
                if (!Application.isPlaying) EditorSceneManager.MarkSceneDirty(_billboardSystem.gameObject.scene);
            }
            EditorGUILayout.HelpBox("This will clear any baked billboards. This will enable dynamic billboards again.", MessageType.Info);
            GUILayout.EndVertical();
        }
        private void DrawSettingsInspector()
        {

            //if (!Application.isPlaying)
            //{
            //    GUILayout.BeginVertical("box");
            //    if (GUILayout.Button("Refresh billboards"))
            //    {
            //        _billboardSystem.CreateBillboards();
            //    }
            //    EditorGUILayout.HelpBox("This will refresh the billboards with current rules. In playmode and builds this will happen automatic.", MessageType.Info);
            //    GUILayout.EndVertical();
            //}

            if (_billboardSystem.VegetationSystem == null)
            {
                GUILayout.BeginVertical("box");
                EditorGUILayout.HelpBox("Billboard System requires a VegetationSystem component on same GameObject.", MessageType.Error);
                GUILayout.EndVertical();
            }


            GUILayout.BeginVertical("box");

            EditorGUILayout.HelpBox("Billboard System manages billboards for one vegetation system.", MessageType.Info);
            GUILayout.EndVertical();




            EditorGUI.BeginChangeCheck();
            //GUILayout.BeginVertical("box");
            //EditorGUILayout.LabelField("Settings", LabelStyle);
            //_billboardSystem.BillboardSystemType = (BillboardSystemType)EditorGUILayout.EnumPopup("Billboard system", _billboardSystem.BillboardSystemType);
            //GUILayout.EndVertical();


            //GUILayout.BeginVertical("box");
            //EditorGUILayout.LabelField("Billboard image alpha", LabelStyle);
            //_billboardSystem.BillboardCutoff = EditorGUILayout.Slider("Cutoff", _billboardSystem.BillboardCutoff, 0, 1f);
            //GUILayout.EndVertical();


            //GUILayout.BeginVertical("box");
            //EditorGUILayout.LabelField("Cameras", LabelStyle);
            //_billboardSystem.RenderAllCameras = EditorGUILayout.Toggle("Render to all cameras", _billboardSystem.RenderAllCameras);
            //GUILayout.EndVertical();


            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Billboard shadows", LabelStyle);
            _billboardSystem.BillboardShadows = EditorGUILayout.Toggle("Billboard shadows", _billboardSystem.BillboardShadows);
            GUILayout.EndVertical();

            if (_billboardSystem.BillboardSystemType == BillboardSystemType.Realtime) DrawRuntimeBillboardInspector();
        }

        private void DrawEditorInspector()
        {
            GUILayout.BeginVertical("box");
            if (GUILayout.Button("Regenerate all billboards atlases"))
            {
                if (_billboardSystem.VegetationSystem)
                {
                    if (_billboardSystem.VegetationSystem != null && _billboardSystem.VegetationSystem.currentVegetationPackage)
                    {
                        for (int i = 0; i <= _billboardSystem.VegetationSystem.currentVegetationPackage.VegetationInfoList.Count - 1; i++)
                        {
                            if (_billboardSystem.VegetationSystem.currentVegetationPackage.VegetationInfoList[i].UseBillboards &&
                                _billboardSystem.VegetationSystem.currentVegetationPackage.VegetationInfoList[i].VegetationType == VegetationType.Tree)
                            {
                                _billboardSystem.VegetationSystem.currentVegetationPackage.GenerateBillboard(
                                    _billboardSystem.VegetationSystem.currentVegetationPackage.VegetationInfoList[i]
                                        .VegetationItemID);
                            }
                        }
                        EditorUtility.SetDirty(_billboardSystem.VegetationSystem.currentVegetationPackage);
                    }
                }
            }
            GUILayout.EndVertical();


            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Render", LabelStyle);
            _billboardSystem.RenderBillboards = EditorGUILayout.Toggle("Render billboards", _billboardSystem.RenderBillboards);
            GUILayout.EndVertical();

            EditorGUI.BeginChangeCheck();
            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Editor", LabelStyle);
            _billboardSystem.ShowCellGrid = EditorGUILayout.Toggle("Show billboard cells", _billboardSystem.ShowCellGrid);
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
            }

            EditorGUI.BeginChangeCheck();
            _billboardSystem.DisableInEditorMode = EditorGUILayout.Toggle("Disable in editor mode", _billboardSystem.DisableInEditorMode);
            GUILayout.EndVertical();
            if (EditorGUI.EndChangeCheck())
            {
                if (_billboardSystem.DisableInEditorMode)
                {
                    _billboardSystem.ClearBillboards();
                }
                else
                {
                    _billboardSystem.CreateBillboards();
                }
                EditorUtility.SetDirty(target);
            }

        }

        private void DrawDebugInspector()
        {


            DrawBillboardInfo();
        }

        void DrawRuntimeBillboardInspector()
        {
            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Settings", LabelStyle);

            //EditorGUI.BeginChangeCheck();
            //BillboardSystem.AutoBakeOnSetVegetationPackage = EditorGUILayout.Toggle("Auto generate billboards.", BillboardSystem.AutoBakeOnSetVegetationPackage);

            //if (EditorGUI.EndChangeCheck())
            //{
            //    EditorUtility.SetDirty(target);
            //}

            EditorGUI.BeginChangeCheck();
            _billboardSystem.CellSize = EditorGUILayout.Slider("Billboard cell size", _billboardSystem.CellSize, 250, 4000);
            EditorGUILayout.HelpBox("Normal cell size is set to 500, larger on huge terrains. 20 km +", MessageType.Info);

            _billboardSystem.PreloadAroundCamera = EditorGUILayout.Toggle("Preload around camera", _billboardSystem.PreloadAroundCamera);
            EditorGUILayout.HelpBox("When enabled system will preload potential visible cells around camera. Will reduce initial loading time when first rotating camera etc. ", MessageType.Info);

            if (_billboardSystem.PreloadAroundCamera)
            {
                _billboardSystem.AdditonalCacheDistance = EditorGUILayout.IntSlider("Additional cache distance", _billboardSystem.AdditonalCacheDistance, 0, 10000);
                EditorGUILayout.HelpBox("At vegetation system load or package change billboard system will preload potential visual areas withing billboard range. Adding distance here will pre cache a larger area.", MessageType.Info);
            }

            _billboardSystem.ClearInvisibleCacheAtFloatingOrigin = EditorGUILayout.Toggle(
                "Clear invisible cells at Floating Origin change", _billboardSystem.ClearInvisibleCacheAtFloatingOrigin);
            EditorGUILayout.HelpBox("When enabled invisible cells will be cleared at terrain floating origin change. This will reduce time moving the terrain but cells will have to be respawned later when they become visible", MessageType.Info);

            if (EditorGUI.EndChangeCheck())
            {
                _billboardSystem.SetupBillboardCells();
                _billboardSystem.SetupCullingGroup();
                EditorUtility.SetDirty(target);
            }

            //EditorGUILayout.HelpBox("This will pre-cache all tree billboards based on current vegetation settings and tree density.", MessageType.Info);
            //EditorGUILayout.HelpBox("Baking billboards could take a few seconds depending on terrain size.", MessageType.Warning);
            GUILayout.EndVertical();


        }

        //void DrawBakedBillboardInspector()
        //{
        //    GUILayout.BeginVertical("box");
        //    EditorGUILayout.LabelField("Baked billboards", LabelStyle);
        //    EditorGUILayout.HelpBox("Baked billboards is not yet implemented and is for use with baked tree option in Vegetation System.", MessageType.Info);
        //    GUILayout.EndVertical();
        //}

        void DrawBillboardInfo()
        {
            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Current billboards", LabelStyle);

            //EditorGUILayout.LabelField("Billboard types: " + BillboardSystem.GetBillboardTypeCount().ToString(), LabelStyle);
            EditorGUILayout.LabelField("Total count: " + _billboardSystem.GetBillboardCount().ToString(), LabelStyle);

            GUILayout.EndVertical();
        }
    }
}
