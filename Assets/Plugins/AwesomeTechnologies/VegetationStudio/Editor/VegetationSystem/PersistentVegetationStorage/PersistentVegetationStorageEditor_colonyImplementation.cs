using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using AwesomeTechnologies;
using AwesomeTechnologies.Utility;
using AwesomeTechnologies.VegetationStudio;

namespace AwesomeTechnologies.Vegetation.PersistentStorage
{
    
    public partial class PersistentVegetationStorageEditor : VegetationStudioBaseEditor
    {
#if PERSISTENT_VEGETATION
        string currentSelected = "";
        PersistentVegetationInstanceInfo current = null;
        float width;
       
        List<VegetationItemModelInfo> vegetationItemInfoOther;

        void RecalculateOtherList()
        {
            vegetationItemInfoOther = new List<VegetationItemModelInfo>();
            List<PersistentVegetationInstanceInfo> instanceList = _persistentVegetationStorage.PersistentVegetationStoragePackage.GetPersistentVegetationInstanceInfoList();
            for (int i=0; i< _persistentVegetationStorage.VegetationSystem.VegetationModelInfoList.Count; i++)
            {
                bool isContain = false;
                for (int j = 0; j < instanceList.Count; j++)
                {
                    if (instanceList[j].VegetationItemID == _persistentVegetationStorage.VegetationSystem.VegetationModelInfoList[i].VegetationItemInfo.VegetationItemID)
                    {
                        isContain = true;
                        break;
                    }
                }

                if (!isContain)
                {
                    vegetationItemInfoOther.Add(_persistentVegetationStorage.VegetationSystem.VegetationModelInfoList[i]);
                }

            }
        }

        public void DrawHead()
        {
            width = EditorGUIUtility.currentViewWidth - 40;
            float currentSize = width / 3;
            float currentSize2 = width / 4 - 3;
            GUILayout.Space(5);
            List<PersistentVegetationInstanceInfo> instanceList = _persistentVegetationStorage.PersistentVegetationStoragePackage.GetPersistentVegetationInstanceInfoList();
            GUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();

            _persistentVegetationStorage.PersistentVegetationStoragePackage = EditorGUILayout.ObjectField("",
                _persistentVegetationStorage.PersistentVegetationStoragePackage, typeof(PersistentVegetationStoragePackage), true, GUILayout.Width(currentSize * 2)) as PersistentVegetationStoragePackage;
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

            GUI.color = (_persistentVegetationStorage.DisablePersistentStorage) ? Color.red : Color.green;
            if (GUILayout.Button((_persistentVegetationStorage.DisablePersistentStorage) ? "Disabled Render" : "Enabled Render", GUILayout.Height(19), GUILayout.Width(currentSize + 5)))
            {
                _persistentVegetationStorage.DisablePersistentStorage = !_persistentVegetationStorage.DisablePersistentStorage;
                _persistentVegetationStorage.VegetationSystem.RefreshVegetationPackage();
                EditorUtility.SetDirty(_persistentVegetationStorage);
            }
            GUI.color = Color.white;

            GUILayout.EndHorizontal();
            
            int totalCount = 0;
            int bakedCount = 0;
            int paintCount = 0;
            int importCount = 0;

            for (int i = 0; i < instanceList.Count; i++)
            {
                totalCount += instanceList[i].Count;

                for (int j = 0; j < instanceList[i].SourceCountList.Count; j++)
                {
                    switch (instanceList[i].SourceCountList[j].VegetationSourceID)
                    {
                        case (byte)0:
                            bakedCount += instanceList[i].SourceCountList[j].Count;
                            break;
                        case (byte)1:
                            paintCount += instanceList[i].SourceCountList[j].Count;
                            break;
                        case (byte)2:
                            importCount+= instanceList[i].SourceCountList[j].Count;
                            break;
                        case (byte)5:
                            paintCount += instanceList[i].SourceCountList[j].Count;
                            break;
                    }
                        
                }
            }

            long fileSize = AssetUtility.GetAssetSize(_persistentVegetationStorage.PersistentVegetationStoragePackage);

            
            float storageSize = (float)fileSize / (1024 * 1024);
            GUILayout.BeginHorizontal(GUILayout.Width(currentSize2 * 4));
                GUILayout.BeginVertical("box", GUILayout.Width(currentSize2 * 2));
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Storage size: " + storageSize.ToString("F2") + " mbyte", GUILayout.Width(currentSize2));
                    EditorGUILayout.LabelField("Cell count: " + _persistentVegetationStorage.GetPersistentVegetationCellCount(), GUILayout.Width(currentSize2));
                    GUILayout.EndHorizontal();
                GUILayout.EndVertical();

                if (GUILayout.Button("Convert only persistent objects2"))
                {
                List<PersistentVegetationInstanceInfo> currentInstanceList = _persistentVegetationStorage.PersistentVegetationStoragePackage.GetPersistentVegetationInstanceInfoList();

                if (instanceList.Count == 0)
                    return;

                List<string> toDelete = new List<string>();
                for (int i = 0; i <= currentInstanceList.Count - 1; i++)
                {
                    //Debug.Log(currentInstanceList[i].VegetationItemID);
                    VegetationItemInfo itemInfo = _persistentVegetationStorage.VegetationSystem.currentVegetationPackage.GetVegetationInfo(currentInstanceList[i].VegetationItemID);
                    if (itemInfo.IncludeDetailLayer == -2)
                    {
                        toDelete.Add(currentInstanceList[i].VegetationItemID);
                        //Debug.Log("!" + currentInstanceList[i].VegetationItemID);

                        if (_persistentVegetationStorage.PersistentVegetationStoragePackage.PersistentVegetationCellList == null) continue;
                        var cellCount = _persistentVegetationStorage.PersistentVegetationStoragePackage.PersistentVegetationCellList.Count;
                        for (int j = 0; j <= cellCount -1; j++)
                        {
                            if (_persistentVegetationStorage.PersistentVegetationStoragePackage.PersistentVegetationCellList == null) continue;
                            PersistentVegetationCell cell = _persistentVegetationStorage.PersistentVegetationStoragePackage.PersistentVegetationCellList[j];
                            if (cell == null) continue;

                            if (cell.PersistentVegetationInfoList == null) continue;
                            for (int z=0; z<cell.PersistentVegetationInfoList.Count; z++)
                            {
                                if (cell.PersistentVegetationInfoList[z].VegetationItemID == currentInstanceList[i].VegetationItemID)
                                {
                                    int listCount = cell.PersistentVegetationInfoList[z].VegetationItemList.Count;
                                    for (int x = 0; x < listCount; x++)
                                    {
                                        GameObject go = (GameObject)PrefabUtility.InstantiatePrefab(itemInfo.VegetationPrefab);
                                        go.transform.parent = _persistentVegetationStorage.transform.parent.parent.parent;
                                        go.transform.position = cell.PersistentVegetationInfoList[z].VegetationItemList[x].Position + _persistentVegetationStorage.VegetationSystem.UnityTerrainData.terrainPosition;
                                        go.transform.rotation = cell.PersistentVegetationInfoList[z].VegetationItemList[x].Rotation;
                                        go.transform.localScale = cell.PersistentVegetationInfoList[z].VegetationItemList[x].Scale;
                                        
                                    }
                                }
                                
                            }
                           
                            
                        }
                        
                    }




                }
                //Debug.Log("ff" + toDelete.Count);

                for (int i = 0; i <= toDelete.Count - 1; i++)
                {
                    //Debug.Log(toDelete[i]);
                    
                    _persistentVegetationStorage.RemoveVegetationItemInstances(toDelete[i]);
                    _persistentVegetationStorage.VegetationSystem.DelayedClearVegetationCellCache();
                    EditorUtility.SetDirty(_persistentVegetationStorage.PersistentVegetationStoragePackage);
                    EditorUtility.SetDirty(_persistentVegetationStorage.VegetationSystem.currentVegetationPackage);
                    
                }

                toDelete.Clear();
                    //for (int i = 0; i <= _persistentVegetationStorage..Count - 1; i++)
                    // {
            }   
                /*
                if (IsPersistentoragePackagePresent())
                {

                    GUI.color = new Color(0.85f, 0.75f, 0.45f);
                    for (int i = 0; i <= _persistentVegetationStorage.VegetationImporterList.Count - 1; i++)
                    {
                        _persistentVegetationStorage.VegetationImporterList[i].PersistentVegetationStoragePackage = _persistentVegetationStorage.PersistentVegetationStoragePackage;
                        _persistentVegetationStorage.VegetationImporterList[i].VegetationPackage = _persistentVegetationStorage.VegetationSystem.currentVegetationPackage;
                        _persistentVegetationStorage.VegetationImporterList[i].PersistentVegetationStorage = _persistentVegetationStorage;
                    }
                    if (_persistentVegetationStorage.VegetationImporterList.Count > 0)
                    {
                        if (_persistentVegetationStorage.VegetationImporterList[1] != null)
                        {
                            IVegetationImporter importer = _persistentVegetationStorage.VegetationImporterList[1];
                            GUILayout.Space(5);
                            importer.OnGUI();
                        }

                        if (_persistentVegetationStorage.VegetationImporterList[2] != null)
                        {
                            IVegetationImporter importer2 = _persistentVegetationStorage.VegetationImporterList[2];
                            GUILayout.Space(5);
                            importer2.OnGUI();
                        }
                        GUI.color = Color.white;
                    }
                }
                */
            GUILayout.EndHorizontal();

            //
            GUILayout.Space(2);
            GUILayout.BeginVertical("box", GUILayout.Width(currentSize2 * 4));
            

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Clear Storage\nTotal: " + totalCount.ToString(), GUILayout.Height(36), GUILayout.Width(currentSize2)))
            {
                _persistentVegetationStorage.InitializePersistentStorage();
                _persistentVegetationStorage.VegetationSystem.RefreshVegetationPackage();
                EditorUtility.SetDirty(_persistentVegetationStorage);
            }

            List<string> vegetationItemIdList = VegetationPackageEditorTools.CreateVegetationInfoIdList(_persistentVegetationStorage.VegetationSystem.currentVegetationPackage);

            GUI.color = new Color(0.9f, 0.9f, 0.9f);

            if (GUILayout.Button("Clear Baked\nTotal: " + bakedCount.ToString(), GUI.skin.FindStyle("ButtonLeft"),  GUILayout.Height(36), GUILayout.Width(currentSize2)))
            {
                for (int i = 0; i <= vegetationItemIdList.Count - 1; i++)
                    _persistentVegetationStorage.RemoveVegetationItemInstances(vegetationItemIdList[i], 0);
                _persistentVegetationStorage.VegetationSystem.RefreshVegetationPackage();
                EditorUtility.SetDirty(_persistentVegetationStorage);
            }

            
            if (GUILayout.Button("Clear Painted\nTotal: " + paintCount.ToString(), GUI.skin.FindStyle("ButtonMid"), GUILayout.Height(36), GUILayout.Width(currentSize2)))
            {
                for (int i = 0; i <= vegetationItemIdList.Count - 1; i++)
                {
                    _persistentVegetationStorage.RemoveVegetationItemInstances(vegetationItemIdList[i], 1);
                    _persistentVegetationStorage.RemoveVegetationItemInstances(vegetationItemIdList[i], 5);
                }
                _persistentVegetationStorage.VegetationSystem.RefreshVegetationPackage();
                EditorUtility.SetDirty(_persistentVegetationStorage);
            }

            if (GUILayout.Button("Clear Imported\nTotal: " + importCount.ToString(), GUI.skin.FindStyle("ButtonRight"), GUILayout.Height(36), GUILayout.Width(currentSize2)))
            {
                for (int i = 0; i <= vegetationItemIdList.Count - 1; i++)
                    _persistentVegetationStorage.RemoveVegetationItemInstances(vegetationItemIdList[i], 2);
                _persistentVegetationStorage.VegetationSystem.RefreshVegetationPackage();
                EditorUtility.SetDirty(_persistentVegetationStorage);
            }

            GUI.color = Color.white;

            GUILayout.EndHorizontal();
            
            GUILayout.EndVertical();
            GUILayout.Space(5);
                
        }


        public void DrawStoredVegetationInspectorColonyOther()
        {
            if (!IsPersistentoragePackagePresent()) return;

            RecalculateOtherList();

            if (vegetationItemInfoOther.Count == 0)

                return;


            width = EditorGUIUtility.currentViewWidth-35;
            int fullCount = vegetationItemInfoOther.Count;
            int lineCount = (int)width / 68;

            int columnCount = Mathf.FloorToInt(fullCount / lineCount) + 1;

            GUILayout.Label("Other Objects " + fullCount.ToString(), EditorStyles.boldLabel);


            GUILayout.BeginVertical();
            for (int i = 0; i < columnCount; i++)
            {
                GUI.color = Color.gray;
                GUILayout.BeginHorizontal(GUI.skin.FindStyle("Box"));
                GUI.color = Color.white;
                bool isSelectiveInLine = false;
                int selectiveInLine = -1;
                for (int j = 0; j < lineCount; j++)
                {
                    int currentCount = i * lineCount + j;
                    if (currentCount < fullCount)
                    {

                        if (vegetationItemInfoOther[currentCount].VegetationItemInfo.VegetationItemID == currentSelected)
                        {
                            isSelectiveInLine = true;
                            selectiveInLine = j;
                            GUI.color = Color.yellow;
                        }
                        else
                            GUI.color = Color.white;

                        VegetationItemInfo itemInfo = vegetationItemInfoOther[currentCount].VegetationItemInfo;
                        if (itemInfo!=null)
                        if (GUILayout.Button(AssetPreview.GetAssetPreview(itemInfo.VegetationPrefab), GUI.skin.FindStyle("Box"), GUILayout.Width(68), GUILayout.Height(68)))
                        {
                            if (currentSelected == vegetationItemInfoOther[currentCount].VegetationItemInfo.VegetationItemID)
                                currentSelected = "";
                            else
                            {
                                currentSelected = vegetationItemInfoOther[currentCount].VegetationItemInfo.VegetationItemID;

                            }
                        }
                        else
                            {

                            }
                        GUI.color = Color.white;
                    }

                }
                GUILayout.EndHorizontal();

                if (isSelectiveInLine)
                    SelectiveWindow(null, _persistentVegetationStorage.VegetationSystem.currentVegetationPackage.GetVegetationInfo(currentSelected));
            }
            GUILayout.EndVertical();
        }

        public void DrawStoredVegetationInspectorStored()
        {
            
            if (!IsPersistentoragePackagePresent()) return;

            List<PersistentVegetationInstanceInfo> instanceList = _persistentVegetationStorage.PersistentVegetationStoragePackage.GetPersistentVegetationInstanceInfoList();

            if (instanceList == null)
                return;
            
            if (instanceList.Count == 0)
                return;

            width = EditorGUIUtility.currentViewWidth-35;
            int fullCount = instanceList.Count;
            int lineCount = (int)width / 68;
            int columnCount = Mathf.FloorToInt(fullCount / lineCount) + 1;
            GUILayout.Label("Stored Objects " + fullCount, EditorStyles.boldLabel, GUILayout.Width(width));
            
            
            GUILayout.BeginVertical();
            for (int i = 0; i < columnCount; i++)
            {
                GUI.color = Color.gray;
                GUILayout.BeginHorizontal(GUI.skin.FindStyle("Box"));
                GUI.color = Color.white;
                bool isSelectiveInLine = false;
                int selectiveInLine = -1;
                for (int j = 0; j < lineCount; j++)
                {
                    int currentCount = i * lineCount + j;
                    if (currentCount < fullCount)
                    {

                        if (instanceList[currentCount].VegetationItemID == currentSelected)
                        {
                            isSelectiveInLine = true;
                            selectiveInLine = j;
                            GUI.color = Color.yellow;
                        }
                        else
                            GUI.color = Color.white;
                        VegetationItemInfo itemInfo = _persistentVegetationStorage.VegetationSystem.currentVegetationPackage.GetVegetationInfo(instanceList[currentCount].VegetationItemID);
                        if (itemInfo != null)
                        {
                            if (GUILayout.Button(AssetPreview.GetAssetPreview(itemInfo.VegetationPrefab), GUI.skin.FindStyle("Box"), GUILayout.Width(68), GUILayout.Height(68)))
                            {
                                if (currentSelected == instanceList[currentCount].VegetationItemID)
                                {
                                    currentSelected = "";
                                    current = null;
                                }
                                else
                                {
                                    currentSelected = instanceList[currentCount].VegetationItemID;
                                    current = instanceList[currentCount];
                                }
                            }
                        }
                        else
                        {
                            _persistentVegetationStorage.PersistentVegetationStoragePackage.RemoveVegetationItemInstances(instanceList[currentCount].VegetationItemID);
                            //GUILayout.Label(instanceList[currentCount].VegetationItemID, GUILayout.Width(68), GUILayout.Height(68));
                        }
                        GUI.color = Color.white;
                    }


                }
                GUILayout.EndHorizontal();

                
                if (isSelectiveInLine)
                {
                    SelectiveWindow(current, _persistentVegetationStorage.VegetationSystem.currentVegetationPackage.GetVegetationInfo(currentSelected));
                }
                
            }

            GUILayout.EndVertical();
            
        }

        void SelectiveWindow(PersistentVegetationInstanceInfo currentItem, VegetationItemInfo vegetationItemInfo)
        {
            if (vegetationItemInfo == null)
            {
                return;
            }
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Box(AssetPreview.GetAssetPreview(vegetationItemInfo.VegetationPrefab), GUILayout.Width(100), GUILayout.Height(100));
            GUILayout.BeginVertical("box", GUILayout.Width(width - 170));

            EditorGUI.BeginChangeCheck();

            
            EditorGUILayout.LabelField("Name: " + vegetationItemInfo.VegetationPrefab.name);
            

            if (currentItem != null)
            {
                EditorGUI.BeginChangeCheck();
                vegetationItemInfo.UseVegetationMasksOnStorage = EditorGUILayout.Toggle("Use vegetation masks",
                    vegetationItemInfo.UseVegetationMasksOnStorage);
                if (EditorGUI.EndChangeCheck())
                {
                    _persistentVegetationStorage.VegetationSystem.RefreshVegetationPackage();
                    EditorUtility.SetDirty(_persistentVegetationStorage.VegetationSystem.currentVegetationPackage);
                }

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Instance count: " + currentItem.Count.ToString("N0"));
                if (vegetationItemInfo.IncludeDetailLayer > -2)
                {
                    if (GUILayout.Button("Bake This", GUILayout.Width(120)))
                    {
                        _persistentVegetationStorage.BakeVegetationItem(vegetationItemInfo.VegetationItemID);
                        _persistentVegetationStorage.VegetationSystem.DelayedClearVegetationCellCache();
                        EditorUtility.SetDirty(_persistentVegetationStorage.PersistentVegetationStoragePackage);
                        EditorUtility.SetDirty(_persistentVegetationStorage.VegetationSystem.currentVegetationPackage);
                    }
                }
                GUILayout.EndHorizontal();
                for (int j = 0; j <= currentItem.SourceCountList.Count - 1; j++)
                {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(PersistentVegetationStorageTools.GetSourceName(currentItem.SourceCountList[j].VegetationSourceID) + " : " + currentItem.SourceCountList[j].Count.ToString("N0"));
                    if (GUILayout.Button("Clear instances", GUILayout.Width(120)))
                    {
                        _persistentVegetationStorage.RemoveVegetationItemInstances(vegetationItemInfo.VegetationItemID, currentItem.SourceCountList[j].VegetationSourceID);
                        _persistentVegetationStorage.VegetationSystem.DelayedClearVegetationCellCache();
                        EditorUtility.SetDirty(_persistentVegetationStorage.PersistentVegetationStoragePackage);
                        EditorUtility.SetDirty(_persistentVegetationStorage.VegetationSystem.currentVegetationPackage);
                    }

                    GUILayout.EndHorizontal();
                }
            }
            else
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Not Persistent");
                if (vegetationItemInfo.IncludeDetailLayer > -2)
                {
                    if (GUILayout.Button("Bake This", GUILayout.Width(120)))
                    {
                        _persistentVegetationStorage.BakeVegetationItem(vegetationItemInfo.VegetationItemID);
                        _persistentVegetationStorage.VegetationSystem.DelayedClearVegetationCellCache();
                        EditorUtility.SetDirty(_persistentVegetationStorage.PersistentVegetationStoragePackage);
                        EditorUtility.SetDirty(_persistentVegetationStorage.VegetationSystem.currentVegetationPackage);
                    }
                }
                GUILayout.EndHorizontal();
            }
            

            GUILayout.BeginHorizontal();
            GUI.color = (_persistentVegetationStorage.mode == PaintMode.Paint) ? Color.green : Color.white;
            if (GUILayout.Button("Paint", GUI.skin.FindStyle("ButtonLeft")))
            {
                if (_persistentVegetationStorage.mode == PaintMode.Paint)
                    _persistentVegetationStorage.mode = PaintMode.None;
                else
                    _persistentVegetationStorage.mode = PaintMode.Paint;
            }
            
            if (vegetationItemInfo.VegetationType == VegetationType.Grass || vegetationItemInfo.VegetationType == VegetationType.Plant)
            {
                GUI.color = (_persistentVegetationStorage.mode == PaintMode.Precision) ? Color.green : Color.white;
                if (GUILayout.Button("Precision Paint", GUI.skin.FindStyle("ButtonRight")))
                {
                    if (_persistentVegetationStorage.mode == PaintMode.Precision)
                        _persistentVegetationStorage.mode = PaintMode.None;
                    else
                        _persistentVegetationStorage.mode = PaintMode.Precision;
                    
                }
            }
            else
            {
                GUI.color = (_persistentVegetationStorage.mode == PaintMode.Edit) ? Color.green : Color.white;
                if (GUILayout.Button("Edit", GUI.skin.FindStyle("ButtonRight")))
                {
                    if (_persistentVegetationStorage.mode == PaintMode.Edit)
                        _persistentVegetationStorage.mode = PaintMode.None;
                    else
                        _persistentVegetationStorage.mode = PaintMode.Edit;
                }
            }
            GUI.color = Color.white;
            GUILayout.EndHorizontal();

            


            GUILayout.EndVertical();

            GUI.color = Color.red;
            GUILayout.Space(1);
            if (GUILayout.Button("X", GUILayout.Width(20), GUILayout.Height(20)))
            {

                if (EditorUtility.DisplayDialog("Delete VegetationItem?",
                    "Do you want to delete the selected VegetationItem?", "Delete", "Cancel"))
                {
                    _persistentVegetationStorage.RemoveVegetationItemInstances(vegetationItemInfo.VegetationItemID);
                    _persistentVegetationStorage.VegetationSystem.DelayedClearVegetationCellCache();
                    EditorUtility.SetDirty(_persistentVegetationStorage.PersistentVegetationStoragePackage);
                    EditorUtility.SetDirty(_persistentVegetationStorage.VegetationSystem.currentVegetationPackage);


                    return;
                }

            }
            GUI.color = Color.white;

            GUILayout.EndHorizontal();

          

            if (_persistentVegetationStorage.mode == PaintMode.Precision)
                DrawPrecisionPaintingInspectorSimple(vegetationItemInfo.VegetationItemID);

            if (_persistentVegetationStorage.mode == PaintMode.Paint)
                DrawPaintVegetationInspectorSimple(vegetationItemInfo.VegetationItemID);

            if (_persistentVegetationStorage.mode == PaintMode.Edit)
                DrawEditVegetationInspectorSimple(vegetationItemInfo.VegetationItemID);

            GUILayout.EndVertical();
        }

        void DrawPrecisionPaintingInspectorSimple(string id)
        {
            if (_sceneMeshRaycaster == null)
            {
                _sceneMeshRaycaster = new SceneMeshRaycaster();
            }

            _persistentVegetationStorage.SelectedPrecisionPaintingVegetationID = id;
            GUILayout.Space(5);
            GUILayout.Label("Precision Paint Settings", EditorStyles.boldLabel);

            GUILayout.BeginVertical("box");

            _persistentVegetationStorage.PrecisionPaintingMode = (PrecisionPaintingMode)EditorGUILayout.EnumPopup("Painting mode", (PrecisionPaintingMode)_persistentVegetationStorage.PrecisionPaintingMode);
            _persistentVegetationStorage.UseSteepnessRules = EditorGUILayout.Toggle("Use steepness/angle rules", _persistentVegetationStorage.UseSteepnessRules);
            _persistentVegetationStorage.SampleDistance = EditorGUILayout.Slider("Sample distance", _persistentVegetationStorage.SampleDistance, 0.25f, 2.5f);
            GUILayout.EndVertical();
        }

        private void DrawPaintVegetationInspectorSimple(string id)
        {
            _persistentVegetationStorage.SelectedPaintVegetationID = id;

            GUILayout.Space(5);
            GUILayout.Label("Paint Settings", EditorStyles.boldLabel);

            bool flag;
            _persistentVegetationStorage.SelectedBrushIndex = AspectSelectionGrid(_persistentVegetationStorage.SelectedBrushIndex, _brushTextures, 32, "No brushes defined.", out flag);
            EditorGUILayout.LabelField("Delete Vegetation: Ctrl-Click", LabelStyle);
           
            GUILayout.BeginVertical("box");

            _persistentVegetationStorage.RandomizePosition = EditorGUILayout.Toggle("Randomize Position", _persistentVegetationStorage.RandomizePosition);
            _persistentVegetationStorage.PaintOnColliders = EditorGUILayout.Toggle("Paint on colliders", _persistentVegetationStorage.PaintOnColliders);
            _persistentVegetationStorage.UseSteepnessRules = EditorGUILayout.Toggle("Use steepness/angle rules", _persistentVegetationStorage.UseSteepnessRules);

            _persistentVegetationStorage.SampleDistance = EditorGUILayout.Slider("Sample distance", _persistentVegetationStorage.SampleDistance, 0.25f, 2.5f);
            _persistentVegetationStorage.BrushSize = EditorGUILayout.Slider("Brush Size", _persistentVegetationStorage.BrushSize, 0.25f, 8);

         
            GUILayout.EndVertical();
        }

        private void DrawEditVegetationInspectorSimple(string id)
        {
            GUILayout.Space(5);
            GUILayout.Label("Edit Settings", EditorStyles.boldLabel);

            GUILayout.BeginVertical("box");
            _persistentVegetationStorage.SelectedEditVegetationID = id;

            EditorGUILayout.LabelField("Insert Vegetation Item: Ctrl-Click", LabelStyle);
            EditorGUILayout.LabelField("Delete Vegetation Item: Ctrl-Shift-Click", LabelStyle);
       
            GUILayout.EndVertical();

            

            
        }
#endif
    }

}
