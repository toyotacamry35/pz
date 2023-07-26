using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using AwesomeTechnologies.Utility;
using AwesomeTechnologies.VegetationStudio;
using Core.Reflection;

namespace AwesomeTechnologies.Vegetation.PersistentStorage
{
#if PERSISTENT_VEGETATION
    [CustomEditor(typeof(PersistentVegetationStorage))]
    public partial class PersistentVegetationStorageEditor : VegetationStudioBaseEditor
    {
        private PersistentVegetationStorage _persistentVegetationStorage;
        private int _changedCellIndex = -1;

        private VegetationBrush _vegetationBrush;

        private static Texture[] _brushTextures;

        private SceneMeshRaycaster _sceneMeshRaycaster;

        private bool _painting;

        private static readonly string[] TabNames =
        {
            "Settings","Stored Vegetation", "Bake Vegetation", "Edit Vegetation","Paint Vegetation","Precision Painting","Import"
        };

        // ReSharper disable once UnusedMember.Local
        void OnEnable()
        {
            _persistentVegetationStorage = (PersistentVegetationStorage)target;
            LoadBrushIcons();
            LoadImporters();
        }

        // ReSharper disable once UnusedMember.Local
        void OnDisable()
        {
            DisableBrush();
        }

        public override void OnInspectorGUI()
        {
            OverrideLogoTextureName = "SectionBanner_PersistentVegetationStorage";
            HelpTopic = "persistent-vegetation-storage";

            _persistentVegetationStorage = (PersistentVegetationStorage)target;
            ShowLogo = !_persistentVegetationStorage.VegetationSystem.GetSleepMode();

            base.OnInspectorGUI();

            if (!_persistentVegetationStorage.VegetationSystem)
            {
                EditorGUILayout.HelpBox("The PersistentVegetationStorage Component needs to be added to a GameObject with a VegetationSystem Component.", MessageType.Error);
                return;
            }

            if (_persistentVegetationStorage.VegetationSystem.GetSleepMode())
            {
                EditorGUILayout.HelpBox("Wake up the Vegetation System from sleepmode to edit settings", MessageType.Info);
                return;
            }

            if (!_persistentVegetationStorage.VegetationSystem.InitDone)
            {
                GUILayout.BeginVertical("box");
                EditorGUILayout.HelpBox("Vegetation system component has configuration errors. Fix to enable component.", MessageType.Error);
                GUILayout.EndVertical();
                return;
            }

            if (VegetationStudioManager.isAdvancedMode)
            {

                EditorGUI.BeginChangeCheck();
                _persistentVegetationStorage.CurrentTabIndex = GUILayout.SelectionGrid(_persistentVegetationStorage.CurrentTabIndex, TabNames, 3, EditorStyles.toolbarButton);
                if (EditorGUI.EndChangeCheck())
                {
                    SceneView.RepaintAll();
                }

                switch (_persistentVegetationStorage.CurrentTabIndex)
                {
                    case 0:
                        DrawSettingsInspector();
                        break;
                    case 1:
                        DrawStoredVegetationInspector();
                        break;
                    case 2:
                        DrawBakeVegetationInspector();
                        break;
                    case 3:
                        DrawEditVegetationInspector();
                        break;
                    case 4:
                        DrawPaintVegetationInspector();
                        break;
                    case 5:
                        DrawPrecisionPaintingInspector();
                        break;
                    case 6:
                        DrawImportInspector();
                        break;
                }

                if (_persistentVegetationStorage.CurrentTabIndex != 4) DisableBrush();
            }
            else
            {
                DrawHead();
                DrawStoredVegetationInspectorStored();
                DrawStoredVegetationInspectorColonyOther();
            }
            

            
        }

        void DrawPrecisionPaintingInspector()
        {
            if (!IsPersistentoragePackagePresent()) return;

            if (_sceneMeshRaycaster == null)
            {
                _sceneMeshRaycaster = new SceneMeshRaycaster();
            }

            EditorGUILayout.HelpBox("Precision Painting will allow you to fine place vegetation. Position is based on a screen ray and will even allow you to place vegetation upside down if the rotation settings is set to follow terrain.", MessageType.Info);

            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Select Vegetation Item", LabelStyle);
            VegetationPackageEditorTools.DrawVegetationItemSelector(_persistentVegetationStorage.VegetationSystem.currentVegetationPackage, VegetationPackageEditorTools.CreateVegetationInfoIdList( _persistentVegetationStorage.VegetationSystem.currentVegetationPackage,
                new[] { VegetationType.Grass, VegetationType.Plant }), 60, ref _persistentVegetationStorage.SelectedPrecisionPaintingVegetationID);

            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Settings", LabelStyle);
            _persistentVegetationStorage.PrecisionPaintingMode = (PrecisionPaintingMode)EditorGUILayout.EnumPopup("Painting mode", (PrecisionPaintingMode)_persistentVegetationStorage.PrecisionPaintingMode);
            if (_persistentVegetationStorage.PrecisionPaintingMode == PrecisionPaintingMode.TerrainAndMeshes)
            {
                EditorGUILayout.HelpBox("This will raycast any enabled meshes in the scene for position.", MessageType.Info);
            }
            _persistentVegetationStorage.UseSteepnessRules = EditorGUILayout.Toggle("Use steepness/angle rules", _persistentVegetationStorage.UseSteepnessRules);
            _persistentVegetationStorage.SampleDistance = EditorGUILayout.Slider("Sample distance", _persistentVegetationStorage.SampleDistance, 0.25f, 5f);
            GUILayout.EndVertical();
        }

        bool IsPersistentoragePackagePresent()
        {
            if (!_persistentVegetationStorage.PersistentVegetationStoragePackage)
            {
                EditorGUILayout.HelpBox("You need to add a persistent vegetation package to the component.", MessageType.Error);
                return false;
            }
            else
            {
                return true;
            }
        }

        private void LoadImporters()
        {
            if (_persistentVegetationStorage.VegetationImporterList.Count != 0) return;

            var interfaceType = typeof(IVegetationImporter);
            var importerTypes = AppDomain.CurrentDomain.GetAssembliesSafe()
                .SelectMany(x => x.GetTypesSafe())
                .Where(x => interfaceType.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                .Select(Activator.CreateInstance);

            foreach (var importer in importerTypes)
            {
                IVegetationImporter importerInterface = importer as IVegetationImporter;
                if (importerInterface != null)
                {
                    _persistentVegetationStorage.VegetationImporterList.Add(importerInterface);
                }
            }
        }

        private static void LoadBrushIcons()
        {
            _brushTextures = new Texture[20];

            for (int i = 0; i < _brushTextures.Length; ++i)
            {
                _brushTextures[i] = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Plugins/AwesomeTechnologies/VegetationStudio/VegetationSystem/_Resources/Brushes/Brush_" + i + ".png");
            }
        }

        private void DrawStoredVegetationInspector()
        {
            if (!IsPersistentoragePackagePresent()) return;

            List<PersistentVegetationInstanceInfo> instanceList = _persistentVegetationStorage.PersistentVegetationStoragePackage.GetPersistentVegetationInstanceInfoList();

            if (instanceList.Count == 0)
                return;

            int totalCount = 0;
            for (int i = 0; i <= instanceList.Count - 1; i++)
            {
                totalCount += instanceList[i].Count;
            }

            long fileSize = AssetUtility.GetAssetSize(_persistentVegetationStorage.PersistentVegetationStoragePackage);


            float storageSize = (float)fileSize / (1024 * 1024);
            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Storage size: " + storageSize.ToString("F2") + " mbyte", LabelStyle);
            EditorGUILayout.LabelField("Total item count: " + totalCount.ToString("N0"), LabelStyle);

            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Status", LabelStyle);
            EditorGUI.BeginChangeCheck();
            _persistentVegetationStorage.DisablePersistentStorage = EditorGUILayout.Toggle("Disable persistent storage",
                _persistentVegetationStorage.DisablePersistentStorage);
            if (EditorGUI.EndChangeCheck())
            {
                _persistentVegetationStorage.VegetationSystem.RefreshVegetationPackage();
                EditorUtility.SetDirty(_persistentVegetationStorage);
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Select Vegetation Item", LabelStyle);

            List<string> vegetationItemIdList = new List<string>();
            for (int i = 0; i <= instanceList.Count - 1; i++)
            {
                vegetationItemIdList.Add(instanceList[i].VegetationItemID);
            }

            VegetationPackageEditorTools.DrawVegetationItemSelector(
                _persistentVegetationStorage.VegetationSystem.currentVegetationPackage, vegetationItemIdList, 60,
                ref _persistentVegetationStorage.SelectedStorageVegetationID);
            GUILayout.EndVertical();

            VegetationItemInfo vegetationItemInfo =
                _persistentVegetationStorage.VegetationSystem.currentVegetationPackage.GetVegetationInfo(
                    _persistentVegetationStorage.SelectedStorageVegetationID);


            GUILayout.BeginVertical("box");

            if (vegetationItemInfo != null)
            {
                EditorGUILayout.LabelField("Information : " + vegetationItemInfo.Name, LabelStyle);
            }

            if (vegetationItemInfo != null)
            {
                EditorGUI.BeginChangeCheck();
                vegetationItemInfo.UseVegetationMasksOnStorage = EditorGUILayout.Toggle("Use vegetation masks",
                    vegetationItemInfo.UseVegetationMasksOnStorage);
                if (EditorGUI.EndChangeCheck())
                {
                    _persistentVegetationStorage.VegetationSystem.RefreshVegetationPackage();
                    EditorUtility.SetDirty(_persistentVegetationStorage.VegetationSystem.currentVegetationPackage);
                }
            }


            int instanceCount = 0;
            PersistentVegetationInstanceInfo selectedPersistentVegetationInstanceInfo = null;
            for (int i = 0; i <= instanceList.Count - 1; i++)
            {
                if (instanceList[i].VegetationItemID == _persistentVegetationStorage.SelectedStorageVegetationID)
                {
                    selectedPersistentVegetationInstanceInfo = instanceList[i];
                    instanceCount = instanceList[i].Count;
                }
            }

            EditorGUILayout.LabelField("Instance count: " + instanceCount.ToString("N0"), LabelStyle);

            if (selectedPersistentVegetationInstanceInfo != null)
            {
                for (int i = 0; i <= selectedPersistentVegetationInstanceInfo.SourceCountList.Count - 1; i++)
                {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(PersistentVegetationStorageTools.GetSourceName(selectedPersistentVegetationInstanceInfo.SourceCountList[i].VegetationSourceID) + " : " + selectedPersistentVegetationInstanceInfo.SourceCountList[i].Count.ToString("N0"), LabelStyle);
                    if (GUILayout.Button("Clear instances", GUILayout.Width(120)))
                    {
                        _persistentVegetationStorage.RemoveVegetationItemInstances(_persistentVegetationStorage.SelectedStorageVegetationID, selectedPersistentVegetationInstanceInfo.SourceCountList[i].VegetationSourceID);
                        _persistentVegetationStorage.VegetationSystem.DelayedClearVegetationCellCache();
                        EditorUtility.SetDirty(_persistentVegetationStorage.PersistentVegetationStoragePackage);
                        EditorUtility.SetDirty(_persistentVegetationStorage.VegetationSystem.currentVegetationPackage);
                    }

                    GUILayout.EndHorizontal();
                }
            }

            if (GUILayout.Button("Clear selected Vegetation Item from storage"))
            {
                _persistentVegetationStorage.RemoveVegetationItemInstances(_persistentVegetationStorage.SelectedStorageVegetationID);
                _persistentVegetationStorage.VegetationSystem.DelayedClearVegetationCellCache();
                EditorUtility.SetDirty(_persistentVegetationStorage.PersistentVegetationStoragePackage);
                EditorUtility.SetDirty(_persistentVegetationStorage.VegetationSystem.currentVegetationPackage);
            }

            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");
            EditorGUILayout.HelpBox("This will clear all Vegetation Items from all sources from storage.", MessageType.Info);
            if (GUILayout.Button("Clear ALL items from storage"))
            {
                int buttonResult = EditorUtility.DisplayDialogComplex("Clear storage",
                    "Are you sure you want to clear the entire storage", "Clear", "Clear/enable run-time spawn", "Cancel");

                switch (buttonResult)
                {
                    case 0:
                        for (int i = 0; i <= vegetationItemIdList.Count - 1; i++)
                        {
                            _persistentVegetationStorage.RemoveVegetationItemInstances(vegetationItemIdList[i]);
                        }
                        _persistentVegetationStorage.VegetationSystem.DelayedClearVegetationCellCache();
                        EditorUtility.SetDirty(_persistentVegetationStorage.PersistentVegetationStoragePackage);
                        break;
                    case 1:
                        for (int i = 0; i <= vegetationItemIdList.Count - 1; i++)
                        {
                            _persistentVegetationStorage.RemoveVegetationItemInstances(vegetationItemIdList[i]);
                            VegetationItemInfo tempVegetationItemInfo =
                                _persistentVegetationStorage.VegetationSystem.currentVegetationPackage
                                    .GetVegetationInfo(vegetationItemIdList[i]);

                            if (tempVegetationItemInfo != null) tempVegetationItemInfo.EnableRuntimeSpawn = true;
                        }

                        _persistentVegetationStorage.VegetationSystem.DelayedClearVegetationCellCache();
                        EditorUtility.SetDirty(_persistentVegetationStorage.PersistentVegetationStoragePackage);
                        EditorUtility.SetDirty(_persistentVegetationStorage.VegetationSystem.currentVegetationPackage);
                        break;
                }
            }
            GUILayout.EndVertical();
        }

        private void DisableBrush()
        {
            if (_vegetationBrush != null)
            {
                _vegetationBrush.Dispose();
                _vegetationBrush = null;
            }
        }

        private void DrawPaintVegetationInspector()
        {
            if (!IsPersistentoragePackagePresent()) return;

            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Select Vegetation Item", LabelStyle);

            VegetationPackageEditorTools.DrawVegetationItemSelector(
                _persistentVegetationStorage.VegetationSystem.currentVegetationPackage,
                VegetationPackageEditorTools.CreateVegetationInfoIdList(
                    _persistentVegetationStorage.VegetationSystem.currentVegetationPackage,
                    new[] { VegetationType.Grass, VegetationType.Plant, VegetationType.Tree, VegetationType.Objects, VegetationType.LargeObjects }), 60,
                ref _persistentVegetationStorage.SelectedPaintVegetationID);

            GUILayout.EndVertical();

            bool flag;
            _persistentVegetationStorage.SelectedBrushIndex = AspectSelectionGrid(_persistentVegetationStorage.SelectedBrushIndex, _brushTextures, 32, "No brushes defined.", out flag);
            EditorGUILayout.LabelField("Delete Vegetation: Ctrl-Click", LabelStyle);
            EditorGUILayout.HelpBox("Delete Vegetation will only remove vegetatio of the selected type.", MessageType.Info);

            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Settings", LabelStyle);

            _persistentVegetationStorage.RandomizePosition = EditorGUILayout.Toggle("Randomize Position", _persistentVegetationStorage.RandomizePosition);
            _persistentVegetationStorage.PaintOnColliders = EditorGUILayout.Toggle("Paint on colliders", _persistentVegetationStorage.PaintOnColliders);
            _persistentVegetationStorage.UseSteepnessRules = EditorGUILayout.Toggle("Use steepness/angle rules", _persistentVegetationStorage.UseSteepnessRules);

            _persistentVegetationStorage.SampleDistance = EditorGUILayout.Slider("Sample distance", _persistentVegetationStorage.SampleDistance, 0.25f, 25f);
            _persistentVegetationStorage.BrushSize = EditorGUILayout.Slider("Brush Size", _persistentVegetationStorage.BrushSize, 0.25f, 30);

            EditorGUILayout.HelpBox("Vegetation items will follow the rotation mode set in the VegetationPackage", MessageType.Info);

            GUILayout.EndVertical();
        }

        private void DrawEditVegetationInspector()
        {
            if (!IsPersistentoragePackagePresent()) return;

            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Select Vegetation Item", LabelStyle);
            EditorGUI.BeginChangeCheck();

            VegetationPackageEditorTools.DrawVegetationItemSelector(
                _persistentVegetationStorage.VegetationSystem.currentVegetationPackage,
                VegetationPackageEditorTools.CreateVegetationInfoIdList(
                    _persistentVegetationStorage.VegetationSystem.currentVegetationPackage,
                    new[] { VegetationType.Objects, VegetationType.Tree, VegetationType.LargeObjects }), 60,
                ref _persistentVegetationStorage.SelectedEditVegetationID);

            if (EditorGUI.EndChangeCheck())
            {
                SceneView.RepaintAll();
            }

            GUILayout.EndVertical();

            EditorGUILayout.LabelField("Insert Vegetation Item: Ctrl-Click", LabelStyle);
            EditorGUILayout.LabelField("Delete Vegetation Item: Ctrl-Shift-Click", LabelStyle);

            EditorGUILayout.HelpBox("Select the Vegetation item to edit. Move/scale and rotate handles will show up in the sceneview. ", MessageType.Info);
        }

        private void DrawImportInspector()
        {

            if (!IsPersistentoragePackagePresent()) return;

            for (int i = 0; i <= _persistentVegetationStorage.VegetationImporterList.Count - 1; i++)
            {
                _persistentVegetationStorage.VegetationImporterList[i].PersistentVegetationStoragePackage =  _persistentVegetationStorage.PersistentVegetationStoragePackage;
                _persistentVegetationStorage.VegetationImporterList[i].VegetationPackage = _persistentVegetationStorage.VegetationSystem.currentVegetationPackage;
                _persistentVegetationStorage.VegetationImporterList[i].PersistentVegetationStorage =    _persistentVegetationStorage;
            }

            string[] importerNames = GetImporterNameArray();
            GUILayout.BeginVertical("box");
            _persistentVegetationStorage.SelectedImporterIndex = EditorGUILayout.Popup(_persistentVegetationStorage.SelectedImporterIndex, importerNames);
            GUILayout.EndVertical();

            if (_persistentVegetationStorage.VegetationImporterList.Count == 0) return;

            GUILayout.BeginVertical("box");
            IVegetationImporter importer =
                _persistentVegetationStorage.VegetationImporterList[_persistentVegetationStorage.SelectedImporterIndex];

            EditorGUILayout.LabelField(importer.ImporterName, LabelStyle);
            GUILayout.EndVertical();

            importer.OnGUI();


        }

        private string[] GetImporterNameArray()
        {
            string[] resultArray = new string[_persistentVegetationStorage.VegetationImporterList.Count];
            for (int i = 0; i <= _persistentVegetationStorage.VegetationImporterList.Count - 1; i++)
            {
                resultArray[i] = _persistentVegetationStorage.VegetationImporterList[i].ImporterName;
            }
            return resultArray;
        }

        private void DrawBakeVegetationInspector()
        {
            if (!IsPersistentoragePackagePresent()) return;

            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Select Vegetation Item", LabelStyle);

            List<string> vegetationItemIdList =
                VegetationPackageEditorTools.CreateVegetationInfoIdList(_persistentVegetationStorage.VegetationSystem
                    .currentVegetationPackage);
            VegetationPackageEditorTools.DrawVegetationItemSelector(
                _persistentVegetationStorage.VegetationSystem.currentVegetationPackage, vegetationItemIdList, 60,
                ref _persistentVegetationStorage.SelectedBakeVegetationID);

            GUILayout.EndVertical();

            if (vegetationItemIdList.Count == 0)
            {
                EditorGUILayout.HelpBox("There is no Vegetation Items configured in the Vegetation Package.", MessageType.Info);
                return;
            }

            GUILayout.BeginVertical("box");

            if (GUILayout.Button("Bake Vegetation Item from ruleset"))
            {
                _persistentVegetationStorage.BakeVegetationItem(_persistentVegetationStorage.SelectedBakeVegetationID);
                _persistentVegetationStorage.VegetationSystem.DelayedClearVegetationCellCache();
                EditorUtility.SetDirty(_persistentVegetationStorage.PersistentVegetationStoragePackage);
                EditorUtility.SetDirty(_persistentVegetationStorage.VegetationSystem.currentVegetationPackage);
            }
            EditorGUILayout.HelpBox("Bake vegetation item will calculate all instances of the vegetation item in the terrain and store this in the persistent storage. This will also disable 'Enable run-time spawn' on the vegetation item.", MessageType.Info);

            if (GUILayout.Button("Bake ALL Vegetation Items from ruleset"))
            {
                int buttonResult = EditorUtility.DisplayDialogComplex("Bake vegetation",
                    "this will bake all Vegetation Items to the persistent storage. 'Enable run-time spawn' will be set to false after bake. ", "Bake", "Clear and bake", "Cancel");

                switch (buttonResult)
                {
                    case 0:

                        for (int i = 0; i <= vegetationItemIdList.Count - 1; i++)
                        {
                            _persistentVegetationStorage.BakeVegetationItem(vegetationItemIdList[i]);
                        }
                        _persistentVegetationStorage.VegetationSystem.DelayedClearVegetationCellCache();
                        EditorUtility.SetDirty(_persistentVegetationStorage.PersistentVegetationStoragePackage);
                        EditorUtility.SetDirty(_persistentVegetationStorage.VegetationSystem.currentVegetationPackage);
                        break;
                    case 1:
                        for (int i = 0; i <= vegetationItemIdList.Count - 1; i++)
                        {
                            _persistentVegetationStorage.RemoveVegetationItemInstances(vegetationItemIdList[i], 0);
                            _persistentVegetationStorage.BakeVegetationItem(vegetationItemIdList[i]);

                        }
                        _persistentVegetationStorage.VegetationSystem.DelayedClearVegetationCellCache();
                        EditorUtility.SetDirty(_persistentVegetationStorage.PersistentVegetationStoragePackage);
                        EditorUtility.SetDirty(_persistentVegetationStorage.VegetationSystem.currentVegetationPackage);
                        break;
                }
            }

            GUILayout.EndVertical();
        }

        private void DrawSettingsInspector()
        {
            EditorGUILayout.HelpBox(
                "The PersistentVegetationStorage Component is designed to store baked vegetation generated from the rules in the VegetationSystem Component or from 3rd party systems. The Vegetation Item locations are stored in a scriptable object.",
                MessageType.Info);

            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Vegetation storage", LabelStyle);
            EditorGUI.BeginChangeCheck();

            _persistentVegetationStorage.PersistentVegetationStoragePackage = EditorGUILayout.ObjectField("Storage",
                _persistentVegetationStorage.PersistentVegetationStoragePackage,
                typeof(PersistentVegetationStoragePackage), true) as PersistentVegetationStoragePackage;
            if (EditorGUI.EndChangeCheck())
            {
                if (_persistentVegetationStorage.PersistentVegetationStoragePackage != null &&
                    !_persistentVegetationStorage.PersistentVegetationStoragePackage.Initialized)
                {
                    if (EditorUtility.DisplayDialog("Initialize persistent storage",
                        "Do you want to initialize the storage for the current VegetationSystem?", "OK", "Cancel"))
                    {
                        _persistentVegetationStorage.InitializePersistentStorage();

                    }
                }

                EditorUtility.SetDirty(target);
                EditorUtility.SetDirty(_persistentVegetationStorage.PersistentVegetationStoragePackage);
                _persistentVegetationStorage.VegetationSystem.RefreshVegetationPackage();
            }

            EditorGUILayout.HelpBox(
                "Create a new PersistentVegetationStoragePackage object by right clicking in a project folder and select Create/AwesomeTechnologies/Persistent Vegetation Storage Package. Then drag and drop this here.",
                MessageType.Info);
            GUILayout.EndVertical();

            if (_persistentVegetationStorage.PersistentVegetationStoragePackage == null) return;

            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Setup", LabelStyle);
            if (GUILayout.Button("Initialize persistent storrage"))
            {
                if (EditorUtility.DisplayDialog("Initialize persistent storage",
                    "Are you sure you want to initialize the storage for the current VegetationSystem? This will remove any existing vegetation in the storage.",
                    "OK", "Cancel"))
                {
                    _persistentVegetationStorage.InitializePersistentStorage();
                    EditorUtility.SetDirty(target);
                    EditorUtility.SetDirty(_persistentVegetationStorage.PersistentVegetationStoragePackage);
                    _persistentVegetationStorage.VegetationSystem.RefreshVegetationPackage();
                }
            }

            EditorGUILayout.HelpBox(
                "Initialize persistent storrage will clear the current storrage and configure it to store vegetation items for the current configuration of the VegetationSystem component",
                MessageType.Info);
            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Status", LabelStyle);
            EditorGUILayout.LabelField("Cell count: " + _persistentVegetationStorage.GetPersistentVegetationCellCount(),
                LabelStyle);
            GUILayout.EndVertical();


            //if (GUILayout.Button("Export"))
            //{
            //    _persistentVegetationStorage.PersistentVegetationStoragePackage.ExportToFile("d:\\serializedData2.blob");
            //}

            //if (GUILayout.Button("Import"))
            //{
            //    _persistentVegetationStorage.PersistentVegetationStoragePackage.ImportFromFile("d:\\serializedData2.blob");
            //}
        }

        private static int AspectSelectionGrid(int selected, Texture[] textures, int approxSize, string emptyString, out bool doubleClick)
        {
            GUILayout.BeginVertical("box", GUILayout.MinHeight(10f));
            int result = 0;
            doubleClick = false;
            if (textures.Length != 0)
            {
                float num = (EditorGUIUtility.currentViewWidth - 20f) / approxSize;
                int num2 = (int)Mathf.Ceil(textures.Length / num);
                Rect aspectRect = GUILayoutUtility.GetAspectRect(num / num2);
                Event current = Event.current;
                if (current.type == EventType.MouseDown && current.clickCount == 2 && aspectRect.Contains(current.mousePosition))
                {
                    doubleClick = true;
                    current.Use();
                }
                result = GUI.SelectionGrid(aspectRect, Math.Min(selected, textures.Length - 1), textures, Mathf.RoundToInt(EditorGUIUtility.currentViewWidth - 20f) / approxSize, "GridList");
            }
            else
            {
                GUILayout.Label(emptyString);
            }
            GUILayout.EndVertical();
            return result;
        }

        private void OnSceneGuiEditVegetation()
        {
            if (_persistentVegetationStorage.SelectedEditVegetationID == "") return;

            if (Event.current.type == EventType.MouseDown)
            {
                _changedCellIndex = -1;
            }

            VegetationItemInfo vegetationItemInfo = _persistentVegetationStorage.VegetationSystem.currentVegetationPackage
                .GetVegetationInfo(_persistentVegetationStorage.SelectedEditVegetationID);

            if (Event.current.type == EventType.MouseUp)
            {
                if (_changedCellIndex != -1)
                {
                    _persistentVegetationStorage.RepositionCellItems(_changedCellIndex, _persistentVegetationStorage.SelectedEditVegetationID);
                    EditorUtility.SetDirty(_persistentVegetationStorage.PersistentVegetationStoragePackage);
                }
            }


            List<int> closeCellIndexList = new List<int>();
            for (int i = 0; i <= _persistentVegetationStorage.VegetationSystem.VisibleVegetationCellList.Count - 1; i++)
            {
                if (_persistentVegetationStorage.VegetationSystem.VisibleVegetationCellList[i].DistanceBand == 0)
                {
                    closeCellIndexList.Add(_persistentVegetationStorage.VegetationSystem.VisibleVegetationCellList[i]
                        .CellIndex);
                }
            }

            Event currentEvent = Event.current;

            if (currentEvent.shift || currentEvent.control)
            {
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
            }

            if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0 && currentEvent.control && !currentEvent.shift && !currentEvent.alt)
            {
                Ray ray = HandleUtility.GUIPointToWorldRay(currentEvent.mousePosition);
                var hits = Physics.RaycastAll(ray, 10000f);
                for (int i = 0; i <= hits.Length - 1; i++)
                {
                    if (hits[i].collider is TerrainCollider)
                    {
                        float scale = UnityEngine.Random.Range(vegetationItemInfo.MinScale, vegetationItemInfo.MaxScale);
                        _persistentVegetationStorage.AddVegetationItemInstance(_persistentVegetationStorage.SelectedEditVegetationID, hits[i].point,
                            new Vector3(scale, scale, scale), Quaternion.Euler(0, UnityEngine.Random.Range(0, 365), 0), true, 1, true);
                        EditorUtility.SetDirty(_persistentVegetationStorage.PersistentVegetationStoragePackage);
                        break;
                    }
                }
            }

            if (currentEvent.shift && currentEvent.control)
            {
                for (int i = 0; i <= closeCellIndexList.Count - 1; i++)
                {
                    PersistentVegetationCell persistentVegetationCell = _persistentVegetationStorage
                        .PersistentVegetationStoragePackage.PersistentVegetationCellList[closeCellIndexList[i]];

                    PersistentVegetationInfo persistentVegetationInfo =
                        persistentVegetationCell.GetPersistentVegetationInfo(_persistentVegetationStorage.SelectedEditVegetationID);

                    if (persistentVegetationInfo != null)
                    {
                        for (int j = persistentVegetationInfo.VegetationItemList.Count - 1; j >= 0; j--)
                        {
                            PersistentVegetationItem persistentVegetationItem =
                                persistentVegetationInfo.VegetationItemList[j];

                            Vector3 cameraPosition = SceneView.currentDrawingSceneView.camera.transform.position;
                            float distance = Vector3.Distance(cameraPosition, persistentVegetationItem.Position + _persistentVegetationStorage.VegetationSystem.UnityTerrainData.terrainPosition);

                            Handles.color = Color.red;
                            if (Handles.Button(persistentVegetationItem.Position + _persistentVegetationStorage.VegetationSystem.UnityTerrainData.terrainPosition, Quaternion.LookRotation(persistentVegetationItem.Position - cameraPosition, Vector3.up), 0.025f * distance, 0.025f * distance, Handles.CircleHandleCap))
                            {
                                persistentVegetationInfo.RemovePersistentVegetationItemInstance(ref persistentVegetationItem);
                                _persistentVegetationStorage.PersistentVegetationStoragePackage.SetInstanceInfoDirty();

                                _persistentVegetationStorage.VegetationSystem.VegetationCellList[closeCellIndexList[i]].ClearCache();
                                _persistentVegetationStorage.VegetationSystem.SetDirty();
                                EditorUtility.SetDirty(_persistentVegetationStorage.PersistentVegetationStoragePackage);
                            }
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i <= closeCellIndexList.Count - 1; i++)
                {
                    PersistentVegetationCell persistentVegetationCell = _persistentVegetationStorage.PersistentVegetationStoragePackage.PersistentVegetationCellList[closeCellIndexList[i]];

                    PersistentVegetationInfo persistentVegetationInfo = persistentVegetationCell.GetPersistentVegetationInfo(_persistentVegetationStorage.SelectedEditVegetationID);

                    if (persistentVegetationInfo != null)
                    {
                        for (int j = persistentVegetationInfo.VegetationItemList.Count - 1; j >= 0; j--)
                        {
                            PersistentVegetationItem persistentVegetationItem =
                                persistentVegetationInfo.VegetationItemList[j];

                            Vector3 worldspacePosition = persistentVegetationItem.Position +
                                                         _persistentVegetationStorage.VegetationSystem.UnityTerrainData
                                                             .terrainPosition;
                            EditorGUI.BeginChangeCheck();

                            if (UnityEditor.Tools.current == Tool.Move)
                            {
                                Vector3 newPosition = Handles.PositionHandle(worldspacePosition, Quaternion.identity);

                                float xAxisMovement = Mathf.Abs(worldspacePosition.x - newPosition.x);
                                float zAxisMovement = Mathf.Abs(worldspacePosition.z - newPosition.z);

                                if (EditorGUI.EndChangeCheck())
                                {
                                    if (xAxisMovement < 0.01f && zAxisMovement < 0.01f)
                                    {
                                        persistentVegetationItem.Position = newPosition - _persistentVegetationStorage.VegetationSystem.UnityTerrainData.terrainPosition;
                                    }
                                    else
                                    {
                                        Vector3 newTerrainPosition = PositionVegetationItem(newPosition);
                                        persistentVegetationItem.Position = newTerrainPosition - _persistentVegetationStorage.VegetationSystem.UnityTerrainData.terrainPosition;
                                    }

                                    persistentVegetationInfo.UpdatePersistentVegetationItemInstanceSourceId(
                                        ref persistentVegetationItem, 1);
                                    _persistentVegetationStorage.PersistentVegetationStoragePackage
                                        .SetInstanceInfoDirty();

                                    _changedCellIndex = closeCellIndexList[i];

                                    persistentVegetationInfo.VegetationItemList[j] = persistentVegetationItem;

                                    _persistentVegetationStorage.VegetationSystem.VegetationCellList[closeCellIndexList[i]]
                                        .ClearCache();
                                    _persistentVegetationStorage.VegetationSystem.SetDirty();

                                    EditorUtility.SetDirty(_persistentVegetationStorage.PersistentVegetationStoragePackage);
                                }
                            }

                            if (UnityEditor.Tools.current == Tool.Rotate)
                            {
                                float size = HandleUtility.GetHandleSize(worldspacePosition) * 1f;
                                const float snap = 0.1f;

                                Handles.color = Color.red;
                                Quaternion newRotation = Quaternion.identity;
                                
                                if (vegetationItemInfo.VegetationType == VegetationType.Tree)
                                {
                                    newRotation = Handles.Disc(persistentVegetationItem.Rotation, worldspacePosition, Vector3.up, size, true, snap);
                                }
                                else
                                {
                                    newRotation = Handles.RotationHandle(persistentVegetationItem.Rotation, worldspacePosition);
                                }                             

                                if (EditorGUI.EndChangeCheck())
                                {
                                    persistentVegetationItem.Rotation = newRotation;

                                    persistentVegetationInfo.UpdatePersistentVegetationItemInstanceSourceId(
                                        ref persistentVegetationItem, 1);
                                    _persistentVegetationStorage.PersistentVegetationStoragePackage
                                        .SetInstanceInfoDirty();

                                    persistentVegetationInfo.VegetationItemList[j] = persistentVegetationItem;
                                    EditorUtility.SetDirty(_persistentVegetationStorage.PersistentVegetationStoragePackage);

                                    _persistentVegetationStorage.VegetationSystem.VegetationCellList[closeCellIndexList[i]]
                                        .ClearCache();
                                    _persistentVegetationStorage.VegetationSystem.SetDirty();
                                }
                            }

                            if (UnityEditor.Tools.current == Tool.Scale)
                            {
                                Handles.color = Color.red;

                                float size = HandleUtility.GetHandleSize(worldspacePosition) * 1f;
                                const float snap = 0.1f;
                                float newScale = Handles.ScaleSlider(persistentVegetationItem.Scale.x, worldspacePosition, Vector3.right, persistentVegetationItem.Rotation, size, snap);

                                if (EditorGUI.EndChangeCheck())
                                {
                                    persistentVegetationItem.Scale = new Vector3(newScale, newScale, newScale);

                                    persistentVegetationInfo.UpdatePersistentVegetationItemInstanceSourceId(
                                        ref persistentVegetationItem, 1);
                                    _persistentVegetationStorage.PersistentVegetationStoragePackage
                                        .SetInstanceInfoDirty();

                                    persistentVegetationInfo.VegetationItemList[j] = persistentVegetationItem;
                                    EditorUtility.SetDirty(_persistentVegetationStorage.PersistentVegetationStoragePackage);

                                    _persistentVegetationStorage.VegetationSystem.VegetationCellList[closeCellIndexList[i]]
                                        .ClearCache();
                                    _persistentVegetationStorage.VegetationSystem.SetDirty();
                                }
                            }
                        }
                    }
                }
            }
        }

        private static Vector3 PositionVegetationItem(Vector3 position)
        {
            Ray ray = new Ray(position + new Vector3(0, 2000f, 0), Vector3.down);

            var hits = Physics.RaycastAll(ray);
            for (int j = 0; j <= hits.Length - 1; j++)
            {
                if (hits[j].collider is TerrainCollider)
                {
                    return hits[j].point;
                }
            }

            return position;
        }

        // ReSharper disable once UnusedMember.Local
        void OnSceneGUI()
        {
            if (!_persistentVegetationStorage) return;
            if (_persistentVegetationStorage.VegetationSystem == null) return;
            if (_persistentVegetationStorage.VegetationSystem.GetSleepMode()) return;
            if (!_persistentVegetationStorage.PersistentVegetationStoragePackage) return;

            if (VegetationStudioManager.isAdvancedMode)
            {
                if (_persistentVegetationStorage.CurrentTabIndex == 3)
                    OnSceneGuiEditVegetation();

                if (_persistentVegetationStorage.CurrentTabIndex == 4)
                    OnSceneGUIPaintVegetation();

                if (_persistentVegetationStorage.CurrentTabIndex == 5)
                    OnSceneGUIPrecisionPainting();
            }
            else
            {
                switch (_persistentVegetationStorage.mode)
                {
                    case PaintMode.Edit:
                        OnSceneGuiEditVegetation();
                        break;
                    case PaintMode.Paint:
                        OnSceneGUIPaintVegetation();
                        break;
                    case PaintMode.Precision:
                        OnSceneGUIPrecisionPainting();
                        break;
                }
            }
            
        }

        void OnSceneGUIPrecisionPainting()
        {
            if (_persistentVegetationStorage.SelectedPrecisionPaintingVegetationID == "") return;
            int controlId = GUIUtility.GetControlID(FocusType.Passive);

            if (Event.current.type == EventType.Repaint)
            {
                PrecisionPaintItem(false);
            }

            if (Event.current.type == EventType.MouseUp)
            {
                HandleUtility.Repaint();
                _painting = false;
            }

            if (Event.current.type == EventType.MouseMove)
            {
                HandleUtility.Repaint();
                PrecisionPaintItem(false);
            }

            if (Event.current.type == EventType.MouseDrag)
            {
                HandleUtility.Repaint();
                if (_painting)
                {
                    PrecisionPaintItem(true);
                }
            }

            if (Event.current.type == EventType.MouseDown)
            {
                HandleUtility.Repaint();
                if (Event.current.button == 0)
                {
                    _painting = true;
                    GUIUtility.hotControl = controlId;
                    Event.current.Use();
                    PrecisionPaintItem(true);
                }
                else
                {
                    GUIUtility.hotControl = 0;
                }
            }
        }

        void PrecisionPaintItem(bool addVegetationItem)
        {
            if (_sceneMeshRaycaster == null) return;

            bool includeMeshes = true;
            bool includeColliders = false;
            switch (_persistentVegetationStorage.PrecisionPaintingMode)
            {
                case PrecisionPaintingMode.Terrain:
                    includeMeshes = false;
                    includeColliders = false;
                    break;
                case PrecisionPaintingMode.TerrainAndColliders:
                    includeMeshes = false;
                    includeColliders = true;
                    break;
                case PrecisionPaintingMode.TerrainAndMeshes:
                    includeMeshes = true;
                    includeColliders = false;
                    break;
            }

            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            RaycastHit raycastHit;
            if (_sceneMeshRaycaster.RaycastSceneMeshes(ray, out raycastHit, true, includeColliders, includeMeshes))
            {
                float size = HandleUtility.GetHandleSize(raycastHit.point) * 0.1f;

                Gizmos.color = Color.white;
                Handles.SphereHandleCap(0, raycastHit.point, Quaternion.identity, size, EventType.Repaint);

                Gizmos.color = Color.green;
                Vector3 normal = raycastHit.normal.normalized;
                Handles.DrawLine(raycastHit.point, raycastHit.point + normal *2);

                if (!addVegetationItem) return;
                if (Event.current.control)
                {
                    EraseVegetationItem(ray.origin, raycastHit.point, _persistentVegetationStorage.SelectedPrecisionPaintingVegetationID, _persistentVegetationStorage.SampleDistance);
                }
                else
                {
                    AddVegetationItem(raycastHit.point, normal, _persistentVegetationStorage.SelectedPrecisionPaintingVegetationID, _persistentVegetationStorage.SampleDistance);
                }
            }
        }

        void OnSceneGUIPaintVegetation()
        {
            if (Event.current.alt) return;
            if (_persistentVegetationStorage.SelectedPaintVegetationID == "") return;
            int controlId = GUIUtility.GetControlID(FocusType.Passive);

            bool raycastHit = false;

          

            if (Event.current.type == EventType.Repaint)
            {
                Vector3 hitPosition = Vector3.zero;
                UpdatePreviewBrush(ref raycastHit, ref hitPosition);
                if (raycastHit)
                {
                    PaintVegetationItems(hitPosition, false);
                }
            }


            if (Event.current.type == EventType.MouseUp)
            {
                HandleUtility.Repaint();
                _painting = false;
            }

            if (Event.current.type == EventType.MouseMove)
            {
                HandleUtility.Repaint();

                Vector3 hitPosition = Vector3.zero;
                UpdatePreviewBrush(ref raycastHit, ref hitPosition);
                if (raycastHit)
                {
                    PaintVegetationItems(hitPosition, false);
                }
            }

            if (Event.current.type == EventType.MouseDrag)
            {
                HandleUtility.Repaint();
                if (_painting)
                {

                    Vector3 hitPosition = Vector3.zero;
                    UpdatePreviewBrush(ref raycastHit, ref hitPosition);
                    if (raycastHit)
                    {
                        PaintVegetationItems(hitPosition, true);
                    }
                }
            }

            if (Event.current.type == EventType.MouseDown)
            {
                HandleUtility.Repaint();
                if (Event.current.button == 0)
                {
                    _painting = true;
                    GUIUtility.hotControl = controlId;
                    Event.current.Use();

                    Vector3 hitPosition = Vector3.zero;
                    UpdatePreviewBrush(ref raycastHit, ref hitPosition);
                    if (raycastHit)
                    {
                        PaintVegetationItems(hitPosition, true);
                    }
                }
                else
                {
                    GUIUtility.hotControl = 0;
                }
            }
        }



        private void PaintVegetationItems(Vector3 hitPosition, bool addVegetationItems)
        {
            Vector3 corner = hitPosition + new Vector3(-_persistentVegetationStorage.BrushSize, 0f, -_persistentVegetationStorage.BrushSize);
            float currentSampleDistance = _persistentVegetationStorage.SampleDistance;

            int xCount = Mathf.RoundToInt(_persistentVegetationStorage.BrushSize * 2 / currentSampleDistance);
            int zCount = xCount;

            for (int x = 0; x <= xCount - 1; x++)
            {
                for (int z = 0; z <= zCount - 1; z++)
                {
                    Vector3 samplePosition = corner + new Vector3(x * currentSampleDistance, 0, z * currentSampleDistance);
                    Vector3 normal;

                    var randomizedPosition = _persistentVegetationStorage.RandomizePosition ? RandomizePosition(samplePosition, currentSampleDistance) : samplePosition;

                    samplePosition = _persistentVegetationStorage.PaintOnColliders ? AllignToCollider(samplePosition, out normal) : AllignToTerrain(samplePosition, out normal);

                    if (addVegetationItems)
                    {
                        randomizedPosition = _persistentVegetationStorage.PaintOnColliders ? AllignToCollider(randomizedPosition, out normal) : AllignToTerrain(randomizedPosition, out normal);
                    }

                    if (!SampleBrushPosition(samplePosition, corner)) continue;

                    float size = HandleUtility.GetHandleSize(samplePosition) * 0.1f;
                    Handles.SphereHandleCap(0, samplePosition, Quaternion.identity, size, EventType.Repaint);
                    normal = normal.normalized;
                    Handles.DrawLine(samplePosition, samplePosition + normal);

                    if (!addVegetationItems) continue;
                    if (Event.current.control)
                    {
                        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                        EraseVegetationItem(ray.origin, randomizedPosition, _persistentVegetationStorage.SelectedPaintVegetationID, currentSampleDistance);
                    }
                    else
                    {
                        AddVegetationItem(randomizedPosition, normal, _persistentVegetationStorage.SelectedPaintVegetationID, currentSampleDistance);
                    }
                }
            }
        }

        void EraseVegetationItem(Vector3 origin, Vector3 worldPosition, string vegetationItemID, float sampleDistance)
        {
            _persistentVegetationStorage.RemoveVegetationItemInstance(vegetationItemID, origin, worldPosition, sampleDistance, true);
            //VegetationStudioManager.RemoveVegetationItemInstance(vegetationItemID, worldPosition, sampleDistance);
        }
        void AddVegetationItem(Vector3 worldPosition, Vector3 terrainNormal, string vegetationItemID, float sampleDistance)
        {
            VegetationItemInfo vegetationItemInfo = null;
            if (_persistentVegetationStorage.VegetationSystem.currentVegetationPlacingData != null)
                vegetationItemInfo = _persistentVegetationStorage.VegetationSystem.currentVegetationPlacingData.GetVegetationInfo(vegetationItemID);
            else
                vegetationItemInfo = _persistentVegetationStorage.VegetationSystem.currentVegetationPackage.GetVegetationInfo(vegetationItemID);

            float randomScale = UnityEngine.Random.Range(vegetationItemInfo.MinScale, vegetationItemInfo.MaxScale);
            Quaternion rotation;
            Vector3 lookAt;
            var slopeCos = Vector3.Dot(terrainNormal, Vector3.up);
            float slopeAngle = Mathf.Acos(slopeCos) * Mathf.Rad2Deg;
            Vector3 angleScale = Vector3.zero;
            Vector3 scale = new Vector3(randomScale, randomScale, randomScale);

            switch (vegetationItemInfo.Rotation)
            {
                case VegetationRotationType.RotateY:
                    rotation = Quaternion.Euler(new Vector3(0, UnityEngine.Random.Range(0, 365f), 0));
                    break;
                case VegetationRotationType.FollowTerrain:
                    lookAt = Vector3.Cross(-terrainNormal, Vector3.right);
                    // reverse it if it is down.
                    lookAt = lookAt.y < 0 ? -lookAt : lookAt;
                    // look at the hit's relative up, using the normal as the up vector
                    rotation = Quaternion.LookRotation(lookAt, terrainNormal);
                    //targetUp = Rotation * Vector3.up;
                    rotation *= Quaternion.AngleAxis(UnityEngine.Random.Range(0, 365f), new Vector3(0, 1, 0));
                    break;
                case VegetationRotationType.FollowTerrainScale:
                    {
                        lookAt = Vector3.Cross(-terrainNormal, Vector3.right);
                        // reverse it if it is down.
                        lookAt = lookAt.y < 0 ? -lookAt : lookAt;
                        // look at the hit's relative up, using the normal as the up vector
                        rotation = Quaternion.LookRotation(lookAt, terrainNormal);
                        //targetUp = Rotation * Vector3.up;
                        rotation *= Quaternion.AngleAxis(UnityEngine.Random.Range(0, 365f), new Vector3(0, 1, 0));

                        float newScale = Mathf.Clamp01(slopeAngle / 45f);
                        angleScale = new Vector3(newScale, 0, newScale);
                    }
                    break;
                case VegetationRotationType.FollowTerrainScaleWithBlock:
                    {
                        lookAt = Vector3.Cross(-terrainNormal, Vector3.right);
                        lookAt = lookAt.y < 0 ? -lookAt : lookAt;
                        rotation = Quaternion.LookRotation(lookAt, terrainNormal);
                        rotation *= Quaternion.AngleAxis(UnityEngine.Random.Range(0, vegetationItemInfo.RotationBlock), new Vector3(0, 1, 0));

                        float newScale = Mathf.Clamp01(slopeAngle / 45f);
                        angleScale = new Vector3(newScale, 0, newScale);
                    }
                    break;
                default:
                    rotation = Quaternion.Euler(new Vector3(0, UnityEngine.Random.Range(0, 365f), 0));
                    break;
            }


            if (vegetationItemInfo.UseAngle && _persistentVegetationStorage.UseSteepnessRules)
            {
                if (vegetationItemInfo.VegetationSteepnessType == VegetationSteepnessType.Simple)
                {
                    if (slopeAngle >= vegetationItemInfo.MaximumSteepness || slopeAngle < vegetationItemInfo.MinimumSteepness) return;
                }
                else
                {
                    float steepnessSpawnChance = SampleCurveArray(vegetationItemInfo.SteepnessCurveArray, slopeAngle, 90);
                    if (RandomCutoff(steepnessSpawnChance)) return;
                }
            }

            //Debug.Log(vegetationItemInfo.Name);
            _persistentVegetationStorage.AddVegetationItemInstanceEx(vegetationItemID, worldPosition, new Vector3(scale.x + angleScale.x, scale.y + angleScale.y, scale.z + angleScale.z), rotation,
                            5, sampleDistance, true);
            //VegetationStudioManager.AddVegetationItemInstanceEx(vegetationItemID, worldPosition, new Vector3(scale.x + angleScale.x, scale.y + angleScale.y, scale.z + angleScale.z),
            //    rotation, 5, sampleDistance);
        }

        private bool RandomCutoff(float value)
        {
            float randomNumber = UnityEngine.Random.Range(0, 1);
            return !(value > randomNumber);
        }
        private float SampleCurveArray(float[] curveArray, float value, float maxValue)
        {
            if (curveArray.Length == 0) return 0f;

            int index = Mathf.RoundToInt((value / maxValue) * curveArray.Length);
            index = Mathf.Clamp(index, 0, curveArray.Length - 1);
            return curveArray[index];
        }

        Vector3 RandomizePosition(Vector3 position, float sampleDistance)
        {
            float randomDistanceFactor = 4f;

            UnityEngine.Random.InitState(Mathf.RoundToInt(position.x * 10) + Mathf.RoundToInt(position.z * 10));
            return position + new Vector3(UnityEngine.Random.Range(-sampleDistance / randomDistanceFactor, sampleDistance / randomDistanceFactor),
                       UnityEngine.Random.Range(-sampleDistance / randomDistanceFactor, sampleDistance / randomDistanceFactor),
                       UnityEngine.Random.Range(-sampleDistance / randomDistanceFactor, sampleDistance / randomDistanceFactor));
        }

        bool SampleBrushPosition(Vector3 worldPosition, Vector3 corner)
        {
            Vector3 position = worldPosition - corner;
            float xNormalized = position.x / _vegetationBrush.Size / 2f;
            float zNormalized = position.z / _vegetationBrush.Size / 2f;

            Texture2D currentBrushTexture = _brushTextures[_persistentVegetationStorage.SelectedBrushIndex] as Texture2D;
            if (currentBrushTexture == null) return false;

            int x = Mathf.Clamp(Mathf.RoundToInt(xNormalized * currentBrushTexture.width), 0, currentBrushTexture.width);
            int z = Mathf.Clamp(Mathf.RoundToInt(zNormalized * currentBrushTexture.height), 0, currentBrushTexture.height);

            Color color = currentBrushTexture.GetPixel(x, z);

            if (color.a > 0.1f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        Vector3 AllignToTerrain(Vector3 position, out Vector3 normal)
        {

            Ray ray = new Ray(position + new Vector3(0, 1000, 0), Vector3.down);
            RaycastHit[] hits = Physics.RaycastAll(ray, 2000);
            for (int i = 0; i <= hits.Length - 1; i++)
            {
                if (hits[i].collider is TerrainCollider)
                {
                    normal = hits[i].normal;
                    return hits[i].point;
                }
            }
            normal = Vector3.up;
            return position;
        }

        Vector3 AllignToCollider(Vector3 position, out Vector3 normal)
        {
            Ray ray = new Ray(position + new Vector3(0, 1000, 0), Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, float.PositiveInfinity))
            {
                normal = hit.normal;
                return hit.point;
            }
            else
            {
                normal = Vector3.up;
                return position;
            }
            //for (int i = 0; i <= hits.Length - 1; i++)
            //{
            //    normal = hits[i].normal;
            //    return hits[i].point;
            //}

        }

        private VegetationBrush GetActiveBrush(int size)
        {
            if (_vegetationBrush == null)
            {
                _vegetationBrush = new VegetationBrush();
            }
            _vegetationBrush.Load(_brushTextures[_persistentVegetationStorage.SelectedBrushIndex] as Texture2D, size);
            return _vegetationBrush;
        }

        private void UpdatePreviewBrush(ref bool raycastHit, ref Vector3 hitPosition)
        {
            if (!_persistentVegetationStorage.VegetationSystem) return;

            Terrain currentTerrain = _persistentVegetationStorage.VegetationSystem.currentTerrain;
            if (currentTerrain == null) return;
            {
                Projector previewProjector = GetActiveBrush(Mathf.CeilToInt(_persistentVegetationStorage.BrushSize)).GetPreviewProjector();
                UnityEngine.Vector2 vector;
                Vector3 vector2;

                var hitTarget = _persistentVegetationStorage.PaintOnColliders ? RaycastAllColliders(out vector, out vector2) : Raycast(out vector, out vector2, currentTerrain);

                if (hitTarget)
                {
                    previewProjector.material.mainTexture = _brushTextures[_persistentVegetationStorage.SelectedBrushIndex];
                    var num = _persistentVegetationStorage.BrushSize;

                    previewProjector.enabled = true;

                    vector2.y = currentTerrain.transform.position.y + currentTerrain.SampleHeight(vector2);
                    previewProjector.transform.position = vector2 + new Vector3(0f, 50f, 0f);

                    hitPosition = vector2;

                    previewProjector.orthographicSize = num;
                    raycastHit = true;
                }
                else
                {
                    previewProjector.enabled = false;
                }
            }
        }

        private static bool Raycast(out UnityEngine.Vector2 uv, out Vector3 pos, Terrain terrain)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            RaycastHit raycastHit;
            bool result;
            if (terrain.GetComponent<Collider>().Raycast(ray, out raycastHit, float.PositiveInfinity))
            {
                uv = raycastHit.textureCoord;
                pos = raycastHit.point;
                result = true;
            }
            else
            {
                uv = UnityEngine.Vector2.zero;
                pos = Vector3.zero;
                result = false;
            }
            return result;
        }

        private static bool RaycastAllColliders(out UnityEngine.Vector2 uv, out Vector3 pos)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            RaycastHit raycastHit;
            bool result;
            if (Physics.Raycast(ray, out raycastHit, float.PositiveInfinity))
            {
                uv = raycastHit.textureCoord;
                pos = raycastHit.point;
                result = true;
            }
            else
            {
                uv = UnityEngine.Vector2.zero;
                pos = Vector3.zero;
                result = false;
            }
            return result;
        }
    }
#endif
}
