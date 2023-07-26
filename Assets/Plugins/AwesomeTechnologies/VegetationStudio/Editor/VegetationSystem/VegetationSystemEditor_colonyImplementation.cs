using System;
using System.CodeDom;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using AwesomeTechnologies.Vegetation;
using System.Collections.Generic;
using AwesomeTechnologies.Common;
using AwesomeTechnologies.Utility;
using AwesomeTechnologies.External.CurveEditor;
using AwesomeTechnologies.VegetationStudio;
using AwesomeTechnologies.Vegetation.PersistentStorage;
using System.IO;

namespace AwesomeTechnologies
{
    public partial class VegetationSystemEditor : VegetationStudioBaseEditor
    {
        private bool isDragActive = false;
        private bool isSelected = false;
        public Texture settingsIco;
        private int selectPrototype = -1;
        float width;
        float width2;
        float borderSize = 12f;
        float stringSize = 160f;
        public string DragDown = "";

        private static readonly string[] RGBA =
        {
            "R","G", "B", "A"
        };
        
        void ResetIco()
        {
            if (settingsIco == null)
                settingsIco = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Plugins/AwesomeTechnologies/VegetationStudio/Common/_Resources/SettingsIco.png");
        }

        void DrawSimpleMode()
        {
            DrawSelectPackageSimple();

            if (_vegetationSystem.vegetationSettings.isRenderRuleInstances)
            {
                GUI.color = new Color(0.3f, 0.3f, 0.3f);
                GUILayout.BeginHorizontal(GUI.skin.FindStyle("Box"));
                GUILayout.Space(5);
                GUILayout.BeginVertical();
                GUILayout.Space(5);
                GUI.color = Color.white;
                if (_vegIndex >= 0 && isSelected)
                {
                    DrawVegetationInspectorSecond();
                }
                else
                {
                    width = EditorGUIUtility.currentViewWidth;
                    //GUILayout.Box(EditorGUIUtility.currentViewWidth.ToString() + " " + width.ToString(), GUILayout.Width(width));
                    PresetSettings();
                    DrawDisableItems();
                }

                GUILayout.Space(5);
                GUILayout.EndVertical();
                GUILayout.Space(5);
                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.Box("Runtime Vegetation Studio Renderer is Disabled", GUILayout.Height(30f), GUILayout.ExpandWidth(true));
            }
        }

        void DrawDisableItems()
        {
            
            width = EditorGUIUtility.currentViewWidth - 34f;
            int disableItemWidth = 70;
            
            int fullCount = _vegetationSystem.disabled.details.Count;
            int lineCount = Mathf.Max(1, ((int)width) / (disableItemWidth + 4));
            int columnCount = Mathf.FloorToInt(fullCount / lineCount) + 1;
            int lenghtItems = lineCount * disableItemWidth;
            
            TextAnchor old = GUI.skin.label.alignment;
            FontStyle old2 = GUI.skin.label.fontStyle;
            Color old3 = GUI.skin.label.normal.textColor;
            GUI.skin.label.alignment = TextAnchor.MiddleLeft;
            GUI.skin.label.fontStyle = FontStyle.Bold;
            GUI.skin.label.normal.textColor = Color.white;
            GUILayout.Label("Disabled");
            GUI.skin.label.alignment = old;
            GUI.skin.label.fontStyle = old2;
            GUI.skin.label.normal.textColor = old3;
            GUILayout.Space(1);
            

            bool _isUpdate = false;
            GUILayout.Box("", GUILayout.Width(width), GUILayout.Height((disableItemWidth + 28) * columnCount));


            Rect rectMax = GUILayoutUtility.GetLastRect();
            Rect rect = new Rect(rectMax.x + 3, rectMax.y + 3, rectMax.width - 6, rectMax.height - 6);
            GUI.color = new Color(0.7f, 0.7f, 0.7f, 1f);
            GUI.Box(rect, "");
            GUI.color = Color.white;

            float freeSpace = width - lenghtItems - 6;
            float currentSpace = freeSpace / (lineCount + 1);
            int finalCurrentSpace = Mathf.FloorToInt(currentSpace);
            if (fullCount == 0)
                _isUpdate = DrawDragDrop(rect, rect, -1, -2);
            else
            {
                for (int i = 0; i < columnCount; i++)
                {
                    for (int j = 0; j < lineCount; j++)
                    {
                        int currentCount = i * lineCount + j;
                        if (currentCount < fullCount)
                        {
                            int selectiveNumber = _vegetationSystem.disabled.details[currentCount];

                            Rect thisRect = new Rect(finalCurrentSpace + rect.x + j * (disableItemWidth + finalCurrentSpace), rect.y + (disableItemWidth + 28) * i + 3, disableItemWidth, disableItemWidth+25);
                            Rect button = new Rect(thisRect.x, thisRect.y, disableItemWidth, disableItemWidth);
                            DrawDragDrop(button, rect, selectiveNumber, -2);
                            
                            GUI.Box(button, AssetPreview.GetAssetPreview(_vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[selectiveNumber].VegetationPrefab), GUI.skin.FindStyle("Box"));

                            old = GUI.skin.label.alignment;
                            old2 = GUI.skin.label.fontStyle;
                            old3 = GUI.skin.label.normal.textColor;
                            GUI.skin.label.alignment = TextAnchor.MiddleRight;
                            GUI.skin.label.fontStyle = FontStyle.Normal;
                            GUI.skin.label.normal.textColor = Color.black;
                            GUI.Label(new Rect(thisRect.x, thisRect.y + disableItemWidth, disableItemWidth, 22), _vegetationSystem.GetItemsInfo()[selectiveNumber].Name);
                            GUI.skin.label.alignment = old;
                            GUI.skin.label.fontStyle = old2;
                            GUI.skin.label.normal.textColor = old3;
                            GUI.skin.label.alignment = old;

                            if (currentCount == fullCount - 1)
                                break;

                            GUI.color = Color.white;



                        }

                    }


                }
            }

            //GUILayout.EndVertical();

            if (_isUpdate)
            {
                _vegetationSystem.RefreshPreset();
            }
        }

        public bool DrawDragDrop(Rect rectButton, Rect rectWindow, int select, int layer)
        {
            Event evt = Event.current;

            bool isButton = false;

            switch (evt.type)
            {

                case EventType.MouseDown:
                    {
                        if (rectButton.Contains(evt.mousePosition))
                        {

                            if (select >= 0)
                            {
                                DragDown = _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[select].VegetationGuid;
                                
                                DragAndDrop.PrepareStartDrag();
                                CustomDragData dragData = new CustomDragData(select, layer);
                                DragAndDrop.SetGenericData("Drag Grass Manager", dragData);
                                evt.Use();
                            }
                        }
                    }
                    break;

                case EventType.MouseDrag:
                    {

                        if (rectButton.Contains(evt.mousePosition))
                        {

                            CustomDragData existingDragData = DragAndDrop.GetGenericData("Drag Grass Manager") as CustomDragData;

                            if (existingDragData != null)
                            {
                                isDragActive = true;
                                DragAndDrop.StartDrag("Dragging List ELement");
                                evt.Use();
                            }
                        }

                    }
                    break;
                case EventType.DragUpdated:

                    DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                    evt.Use();
                    break;

                case EventType.Repaint:
                    {

                        if (DragAndDrop.visualMode == DragAndDropVisualMode.None || DragAndDrop.visualMode == DragAndDropVisualMode.Rejected)
                            break;

                        if (rectWindow.Contains(evt.mousePosition))
                        {
                            //EditorGUI.DrawRect(new Rect(rectWindow.x, rectWindow.y, rectWindow.width, rectWindow.height), new Color(0.8f, 0.8f, 0.4f, 0.5f));
                        }


                    }
                    break;

                case EventType.DragPerform:
                    {

                        if (rectWindow.Contains(evt.mousePosition))
                        {
                            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                            DragAndDrop.AcceptDrag();
                            CustomDragData receivedDragData = DragAndDrop.GetGenericData("Drag Grass Manager") as CustomDragData;
                            int old = receivedDragData.originalIndex;

                            if (_vegetationSystem == null)
                                _vegetationSystem = (VegetationSystem)target;

                            _vegetationSystem.SetItem(old).IncludeDetailLayer = layer;

                            _vegetationSystem.RefreshPreset();

                            _vegetationSystem.currentVegetationPackage.UpdateCurves();
                            EditorUtility.SetDirty(_vegetationSystem.currentVegetationPackage);
                            if (_vegetationSystem.currentVegetationPlacingData!=null)
                            EditorUtility.SetDirty(_vegetationSystem.currentVegetationPlacingData);
                            _vegetationSystem.DelayedClearVegetationCellCache();
                            EditorUtility.SetDirty(target);

                            evt.Use();
                            isDragActive = false;
                            DragDown = "";
                        }

                    }
                    break;
                case EventType.MouseUp:
                    {
                        // Clean up, in case MouseDrag never occurred:
                        
                        CustomDragData existingDragData = DragAndDrop.GetGenericData("Drag Grass Manager") as CustomDragData;

                        if (rectButton.Contains(evt.mousePosition))
                        {

                            if (DragDown == _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[select].VegetationGuid)
                            {
                                isSelected = true;
                                _vegIndex = select;
                                isButton = true;

                            }


                            DragDown = "";
                        }

                        

                        DragAndDrop.PrepareStartDrag();
                    }
                    break;

                default:

                    break;
            }

            return isButton;
        }

        public void PresetSettings()
        {


            bool isUpdate = false;
            stringSize = (width-50) / 3f;

            EditorGUI.BeginChangeCheck();

            if (_vegetationSystem.GetItemsInfo() != null)
            {
                int detailCount = 2;
                //GUILayout.BeginHorizontal();
                // GUILayout.FlexibleSpace();

                for (int i = 0; i < detailCount; i++)
                {
                    //GUILayout.BeginVertical(GUILayout.Width(stringSize));
                    GUILayout.BeginVertical();

                    TextAnchor old = GUI.skin.label.alignment;
                    FontStyle old2 = GUI.skin.label.fontStyle;
                    Color old3 = GUI.skin.label.normal.textColor;
                    GUI.skin.label.alignment = TextAnchor.MiddleLeft;
                    GUI.skin.label.fontStyle = FontStyle.Bold;
                    GUI.skin.label.normal.textColor = Color.white;
                    GUILayout.Label("DetailMask", GUI.skin.FindStyle("Label"));
                    GUI.skin.label.alignment = old;
                    GUI.skin.label.fontStyle = old2;
                    GUI.skin.label.normal.textColor = old3;

                    GUILayout.Space(1);

                    for (int w = 4 * i; w < 4 * (i + 1); w++)
                    {
                        GUILayout.BeginHorizontal("", GUI.skin.FindStyle("Box"));
                        
                        GUI.color = Color.white;

                        GUIStyle newStyle = new GUIStyle(EditorStyles.largeLabel);
                        newStyle.fontSize = 18;
                        newStyle.alignment = TextAnchor.MiddleCenter;

                        GUILayout.Label(RGBA[w % 4], newStyle, GUILayout.Width(20), GUILayout.Height(Mathf.Max(66, 66 * _vegetationSystem.layerDetails[w].details.Count)));
                        
                        Rect rect = GUILayoutUtility.GetRect(stringSize-30, Mathf.Max(66, 66 * _vegetationSystem.layerDetails[w].details.Count));
                        

                        
                        if (rect.Contains(Event.current.mousePosition) && isDragActive)
                            GUI.color = new Color(0.8f, 0.8f, 0.4f, 0.75f);
                        else
                            GUI.color = new Color(0.7f, 0.7f, 0.7f, 1f);

                        
                        GUI.Box(new Rect(rect.x, rect.y, rect.width, rect.height), "", GUI.skin.FindStyle("Box"));
                        GUI.color = Color.white;

                        isUpdate = DrawButtonDetailCombine(rect, _vegetationSystem.layerDetails[w].details.ToArray(), w, isUpdate);
                        

                        GUILayout.EndHorizontal();

                    }
                    GUILayout.EndVertical();
                    GUILayout.Space(5);
                }

                
                if (_vegetationSystem.other != null)
                {

                    GUILayout.BeginVertical();

                    GUILayout.Space(1);
                    TextAnchor old = GUI.skin.label.alignment;
                    FontStyle old2 = GUI.skin.label.fontStyle;
                    Color old3 = GUI.skin.label.normal.textColor;
                    GUI.skin.label.alignment = TextAnchor.MiddleLeft;
                    GUI.skin.label.fontStyle = FontStyle.Bold;
                    GUI.skin.label.normal.textColor = Color.white;
                    GUILayout.Label("Manual");
                    GUI.skin.label.alignment = old;
                    GUI.skin.label.fontStyle = old2;
                    GUI.skin.label.normal.textColor = old3;
                    GUILayout.Space(1);


                    GUILayout.BeginHorizontal("", GUI.skin.FindStyle("Box"));

                    Rect rect = GUILayoutUtility.GetRect(stringSize-10, Mathf.Max(73 * 4, 68 * Mathf.Max(4, _vegetationSystem.other.details.Count)));

                    if (rect.Contains(Event.current.mousePosition) && isDragActive)
                        GUI.color = new Color(0.8f, 0.8f, 0.4f, 0.75f);
                    else
                        GUI.color = new Color(0.7f, 0.7f, 0.7f, 1f);

                   
                    GUI.Box(new Rect(rect.x, rect.y, rect.width, rect.height), "", GUI.skin.FindStyle("Box"));
                    GUI.color = Color.white;

                    isUpdate = DrawButtonDetailCombine(rect, _vegetationSystem.other.details.ToArray(), -1, isUpdate);

                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                }
                
                //GUILayout.EndHorizontal();

                if (EditorGUI.EndChangeCheck())
                {
                    _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].UpdateCurves();
                    EditorUtility.SetDirty(_vegetationSystem.currentVegetationPackage);
                    if (_vegetationSystem.currentVegetationPlacingData != null)
                        EditorUtility.SetDirty(_vegetationSystem.currentVegetationPlacingData);
                    _vegetationSystem.DelayedClearVegetationCellCache();
                    EditorUtility.SetDirty(target);
                }

                if (isUpdate)
                {
                    _vegetationSystem.RefreshPreset();
                }
            }
            
        }

        private GUISkin backgroundStyle;

        bool DrawButtonDetailCombine(Rect rect, int[] levelDetails, int detailLayer, bool _isUpdate)
        {
            if (backgroundStyle == null)
                backgroundStyle = AssetDatabase.LoadAssetAtPath<GUISkin>("Assets/Plugins/AwesomeTechnologies/VegetationStudio/Common/_Resources/Vegetation.guiskin");

            if (levelDetails.Length == 0)
                DrawDragDrop(new Rect(rect.x + rect.width - 86, rect.y + 1, 64, 64), rect, -1, detailLayer);
            else
                for (int j = 0; j < levelDetails.Length; j++)
                {
                    int currentNumber = levelDetails[j];
                    VegetationItemInfo currentItem = _vegetationSystem.GetItemsInfo()[currentNumber];

                    DrawDragDrop(new Rect(rect.x + rect.width - 86, rect.y + 1 + 66 * j, 64, 64), rect, currentNumber, detailLayer);
                    GUI.BeginGroup(new Rect(rect.x, rect.y + 66 * j, rect.width, 66 * (j + 1)));


                    TextAnchor oldAlign = GUI.skin.label.alignment;
                    GUI.skin.label.alignment = TextAnchor.MiddleCenter;

                    float thisWidth = rect.width - 86;
                    int count = Math.Min(((int)thisWidth / 25), 5);
                    float border = (thisWidth - count * 25) / 2;
                    GUI.skin.label.normal.textColor = Color.black;
                    GUI.Label(new Rect(0, 0, rect.width - 86, 16), currentItem.Name);


                    currentItem.Density = GUI.HorizontalSlider(new Rect(5, 18, rect.width - 75, 12), currentItem.Density, 1f, 0f, backgroundStyle.FindStyle("horizontalslider"), backgroundStyle.FindStyle("horizontalsliderthumb"));
                    currentItem.SampleDistance = GUI.HorizontalSlider(new Rect(5, 34, rect.width - 75, 12), currentItem.SampleDistance, 2.0f, 0.3f, backgroundStyle.FindStyle("horizontalslider"), backgroundStyle.FindStyle("horizontalsliderthumb"));
                    GUI.Label(new Rect(0, 16, rect.width - 84, 16), "Density: " + currentItem.Density.ToString("F1"), backgroundStyle.FindStyle("label"));
                    GUI.Label(new Rect(0, 32, rect.width - 84, 16), "Distance: " + currentItem.SampleDistance.ToString("F1"), backgroundStyle.FindStyle("label"));

                    if (count > 0) currentItem.UsePerlinMask = GUI.Toggle(new Rect(border, 48, 25, 16), currentItem.UsePerlinMask, "P", GUI.skin.FindStyle("Toggle"));

                    if (count > 1) currentItem.UseIncludeTextueMask = GUI.Toggle(new Rect(border + 25, 48, 25, 16), currentItem.UseIncludeTextueMask, "I", GUI.skin.FindStyle("Toggle"));
                    if (count > 2) currentItem.UseExcludeTextueMask = GUI.Toggle(new Rect(border + 50, 48, 25, 16), currentItem.UseExcludeTextueMask, "E", GUI.skin.FindStyle("Toggle"));
                    GUI.color = Color.white;

                    if (count > 3) currentItem.UseHeightLevel = GUI.Toggle(new Rect(border + 75, 48, 25, 16), currentItem.UseHeightLevel, "H", GUI.skin.FindStyle("Toggle"));
                    if (count > 4) currentItem.UseAngle = GUI.Toggle(new Rect(border + 100, 48, 25, 16), currentItem.UseAngle, "S", GUI.skin.FindStyle("Toggle"));

                    GUI.skin.label.alignment = oldAlign;

                    GUI.color = Color.white;

                    Rect button = new Rect(rect.width - 66, 1, 64, 64);

                    if (DragDown == currentItem.VegetationGuid)
                        GUI.color = new Color(0.4f, 0.4f, 0.4f, 1f);
                    GUI.Box(button, AssetPreview.GetAssetPreview(_vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[currentNumber].VegetationPrefab), GUI.skin.FindStyle("Button"));

                    GUI.color = Color.white;
                    GUI.EndGroup();

                }
            return _isUpdate;
        }

        void DrawVegetationInspectorSecond()
        {
            GUILayout.BeginHorizontal();

            GUILayout.Box(AssetPreview.GetAssetPreview(_vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].VegetationPrefab), GUILayout.Width(100), GUILayout.Height(100));
            GUILayout.BeginVertical("box");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Name", GUILayout.Width(40));
            _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].Name = EditorGUILayout.TextField("", _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].Name);
            GUI.color = Color.red;
            if (GUILayout.Button("X", GUILayout.Width(20), GUILayout.Height(20)))
                isSelected = false;
            GUI.color = Color.white;
            GUILayout.EndHorizontal();

            //EditorGUILayout.LabelField("Type: ");// + _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].VegetationType.ToString());
            _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].VegetationType = (VegetationType)EditorGUILayout.EnumPopup("Type: ", _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].VegetationType);
            EditorGUI.BeginChangeCheck();

            if (_vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].PrefabType == VegetationPrefabType.Mesh)
            {
                GUILayout.BeginHorizontal();
                GameObject oldPrefab = _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].VegetationPrefab;
                _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].VegetationPrefab = EditorGUILayout.ObjectField("Vegetation prefab", _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].VegetationPrefab, typeof(GameObject), true) as GameObject;

                if (oldPrefab != _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].VegetationPrefab)
                {
                    _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                        .VegetationInfoList[_vegIndex]
                        .ShaderType = VegetationTypeDetector.GetVegetationShaderType(_vegetationSystem
                        .VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                        .VegetationInfoList[_vegIndex]
                        .VegetationPrefab);

                    if (_vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                            .VegetationInfoList[_vegIndex]
                            .VegetationType == VegetationType.Tree)
                    {
                        //_vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                        //    .GenerateBillboard(_vegIndex);
                    }

                    _vegetationSystem.GetModelData(_vegIndex);
                }


                if (GUILayout.Button("Refresh prefab"))
                {
                    _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].ShaderType = VegetationTypeDetector.GetVegetationShaderType(_vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].VegetationPrefab);
                    _vegetationSystem.GetModelData(_vegIndex);
                    _vegetationSystem.SetupVegetationPrefabs();
                }

                GUILayout.EndHorizontal();

                if (_vegetationSystem.EnableVegetationItemIDEdit)
                {
                    _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                        .VegetationInfoList[_vegIndex].VegetationItemID = EditorGUILayout.TextField(
                        "Vegetation Item ID",
                        _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                            .VegetationInfoList[_vegIndex].VegetationItemID);
                }

                if (_vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].VegetationPrefab == null)
                {
                    EditorGUILayout.HelpBox("Missing vegetation prefab, item will be skipped", MessageType.Warning);
                }
            }
            else
            {
                _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].VegetationTexture = EditorGUILayout.ObjectField("Vegetation texture", _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].VegetationTexture, typeof(Texture2D), true) as Texture2D;
                if (_vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].VegetationTexture == null)
                {
                    EditorGUILayout.HelpBox("Missing vegetation texture, item will be skipped", MessageType.Warning);
                }
            }

            if (_vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].PrefabType == VegetationPrefabType.Mesh)
            {
                _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].LodIndex = EditorGUILayout.Popup("Selected initial LOD", _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].LodIndex, Lod);
            }

            _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].VegetationRenderType = (VegetationRenderType)EditorGUILayout.EnumPopup("Render mode", _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].VegetationRenderType);
            if (_vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].VegetationRenderType == VegetationRenderType.Normal)
            {
                EditorGUILayout.HelpBox("Normal rendering is slow and should only be used on objects with materials that does not support instancing.", MessageType.Warning);
            }

            if (EditorGUI.EndChangeCheck())
            {
                _selectedGridIndex = GetSelectedGridIndex(_vegIndex);
                _vegetationSystem.RefreshVegetationPackage();
                EditorUtility.SetDirty(_vegetationSystem.currentVegetationPackage);
            }

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            if (_vegetationSystem.currentVegetationPackage == null)
            {
                DrawNoVegetationPackageError();
                return;
            }

            if (_vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList.Count > 0)
            {

                GUI.color = new Color(0.85f, 0.85f, 0.85f);
                GUILayout.BeginVertical("box");
                GUI.color = Color.white;

                EditorGUI.BeginChangeCheck();

                VegetationItemInfo currentItem = _vegetationSystem.GetItemsInfo()[_vegIndex];

                GUILayout.Label("Position", EditorStyles.boldLabel);

                GUILayout.BeginVertical("box");
                currentItem.Seed = EditorGUILayout.IntSlider("Seed", currentItem.Seed, 0, 100);
                currentItem.SampleDistance = EditorGUILayout.Slider("Sample distance(meter)", currentItem.SampleDistance, 0.3f, 2.5f);
                currentItem.Density = EditorGUILayout.Slider("Density", currentItem.Density, 0, 1f);
                if (ItemHeader(ref currentItem.RandomizePosition, "Randomize distribution"))
                {
                    GUI.color = new Color(0.85f, 0.85f, 0.85f);
                    GUILayout.BeginVertical("box");
                    GUI.color = Color.white;
                    currentItem.RandomPositionRelativeDistance = EditorGUILayout.Slider("Random distance", currentItem.RandomPositionRelativeDistance, 0, 1f);

                    if (currentItem.VegetationType == VegetationType.Tree || currentItem.VegetationType == VegetationType.LargeObjects)
                        currentItem.UseCollisionDetection = EditorGUILayout.Toggle("Collision Detection", currentItem.UseCollisionDetection);

                    Rect space = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(18));
                    currentItem.Offset = EditorGUI.Vector3Field(space, "Position offset", currentItem.Offset);
                    GUILayout.EndVertical();
                }
                GUILayout.Space(2);
                GUILayout.EndVertical();


                GUILayout.Label("Scale", EditorStyles.boldLabel);

                GUILayout.BeginVertical("box");
                {
                    currentItem.VegetationScaleType = (VegetationScaleType)EditorGUILayout.EnumPopup("Scale type", currentItem.VegetationScaleType);

                    if (currentItem.VegetationScaleType == VegetationScaleType.Simple)
                        EditorFunctions.FloatRangeField("Min/Max scale", ref currentItem.MinScale, ref currentItem.MaxScale, 0.1f, 10f);
                    else
                    {
                        Rect spaceMin = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(18));
                        currentItem.MinVectorScale = EditorGUI.Vector3Field(spaceMin, "Minimum scale", currentItem.MinVectorScale);
                        Rect spaceMax = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(18));
                        currentItem.MaxVectorScale = EditorGUI.Vector3Field(spaceMax, "Maximum scale", currentItem.MaxVectorScale);
                    }
                }
                GUILayout.EndVertical();

                GUILayout.Label("Rotation", EditorStyles.boldLabel);
                GUILayout.BeginVertical("box");
                {
                    currentItem.Rotation = (VegetationRotationType)EditorGUILayout.EnumPopup("Rotation", currentItem.Rotation);

                    if (currentItem.Rotation != VegetationRotationType.FollowTerrainScale && currentItem.VegetationType == VegetationType.Grass)
                    {
                        EditorGUILayout.HelpBox(
                            "For better grass coverage and look in steep areas. FollowTerrainScale setting is recomended.",
                            MessageType.Warning);
                    }

                    Rect space = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(18));
                    currentItem.RotationOffset = EditorGUI.Vector3Field(space, "Rotation offset", currentItem.RotationOffset);
                    Rect space2 = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(18));
                    if (currentItem.Rotation == VegetationRotationType.FollowTerrainScaleWithBlock)
                    {
                        currentItem.RotationBlock = EditorGUI.FloatField(space2, "Rotation Block", currentItem.RotationBlock);
                    }

                }
                GUILayout.EndVertical();

                if (ItemHeader(ref currentItem.UseHeightLevel, "Height Level"))
                {
                    GUILayout.BeginVertical("box");

                    currentItem.VegetationHeightType = (VegetationHeightType)EditorGUILayout.EnumPopup("Selection type",
                        currentItem.VegetationHeightType);
                    if (currentItem.VegetationHeightType == VegetationHeightType.Simple)
                    {

                        EditorFunctions.FloatRangeField("Min/Max height",
                            ref currentItem.MinimumHeight,
                            ref currentItem.MaximumHeight, -500f, 10000f);
                    }
                    else
                    {
                        EditorFunctions.FloatRangeField("Min/Max cuve height",
                            ref currentItem.MinCurveHeight,
                            ref currentItem.MaxCurveHeight, -500, 10000);

                        currentItem.AutomaticCurveMaxHeight =
                            EditorGUILayout.Toggle("Max curve height from terrain",
                                currentItem.AutomaticCurveMaxHeight);


                        if (_heightCurveEditor.EditCurve(
                            currentItem.HeightCurve, this))
                        {
                            _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                                .UpdateCurves();
                            EditorUtility.SetDirty(_vegetationSystem.currentVegetationPackage);
                            _vegetationSystem.DelayedClearVegetationCellCache();
                            EditorUtility.SetDirty(target);
                        }
                    }

                    GUILayout.EndVertical();
                }

                if (ItemHeader(ref currentItem.UseAngle, "Stepness"))
                {
                    GUILayout.BeginVertical("box");
                    currentItem.VegetationSteepnessType = (VegetationSteepnessType)EditorGUILayout.EnumPopup("Selection type", currentItem.VegetationSteepnessType);
                    if (currentItem.VegetationSteepnessType == VegetationSteepnessType.Simple)
                        EditorFunctions.FloatRangeField("Min/Max steepness", ref currentItem.MinimumSteepness, ref currentItem.MaximumSteepness, 0f, 90f);
                    else
                    {
                        if (_steepnessCurveEditor.EditCurve(currentItem.SteepnessCurve, this))
                        {
                            _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].UpdateCurves();
                            EditorUtility.SetDirty(_vegetationSystem.currentVegetationPackage);
                            _vegetationSystem.DelayedClearVegetationCellCache();
                            EditorUtility.SetDirty(target);
                        }
                    }

                    GUILayout.EndVertical();
                }

                if (ItemHeader(ref currentItem.UsePerlinMask, "Perlin Mask"))
                {
                    GUILayout.BeginVertical("box");

                    currentItem.PerlinCutoff = EditorGUILayout.Slider("Perlin noise cutoff", currentItem.PerlinCutoff, 0f, 1f);
                    currentItem.PerlinScale = EditorGUILayout.Slider("Perlin noise scale", currentItem.PerlinScale, 1f, 150f);
                    currentItem.PerlinOffset = EditorGUILayout.Vector2Field("Perlin noise offset", currentItem.PerlinOffset);
                    ItemHeader(ref currentItem.InversePerlinMask, "Inverse Mask");
                    GUILayout.Space(2);
                    GUILayout.EndVertical();
                }


                GUILayout.Label("Texture Terrain Rules", EditorStyles.boldLabel);

                GUI.color = new Color(0.8f, 0.8f, 0.8f);
                GUILayout.Box("", GUILayout.Height(1), GUILayout.ExpandWidth(true));
                GUI.color = Color.white;
                GUILayout.Space(2);

                if (ItemHeader(ref currentItem.UseExcludeTextueMask, "Exclude Texture"))
                    DrawExcludeTextures();

                if (ItemHeader(ref currentItem.UseIncludeTextueMask, "Include Texture"))
                    DrawIncludeTextures();

                GUILayout.Space(2);

                GUILayout.Label("Texture Mask Rules", EditorStyles.boldLabel);

                GUI.color = new Color(0.8f, 0.8f, 0.8f);
                GUILayout.Box("", GUILayout.Height(1), GUILayout.ExpandWidth(true));
                GUI.color = Color.white;
                GUILayout.Space(2);

                if (ItemHeader(ref currentItem.UseScaleTextueMaskRules, "Scale From Mask"))
                    DrawTextureMasksRules(currentItem.ScaleTextureMaskRuleList, ref _scaleTextureMaskRuleIndex, TextureMaskRuleType.Scale);

                if (ItemHeader(ref currentItem.UseDensityTextueMaskRules, "Density From Mask"))
                    DrawTextureMasksRules(currentItem.DensityTextureMaskRuleList, ref _densityTextureMaskRuleIndex, TextureMaskRuleType.Density);

                if (ItemHeader(ref currentItem.UseExcludeTextueMaskRules, "Exclude From Mask"))
                    DrawTextureMasksRules(currentItem.ExcludeTextureMaskRuleList, ref _excludeTextureMaskRuleIndex, TextureMaskRuleType.Exclude);

                if (ItemHeader(ref currentItem.UseIncludeTextueMaskRules, "Include From Mask"))
                    DrawTextureMasksRules(currentItem.IncludeTextureMaskRuleList, ref _includeTextureMaskRuleIndex, TextureMaskRuleType.Include);

                GUILayout.Space(2);



                /*              

                 _vegetationSystem.ShowVegetationMaskMenu = VegetationPackageEditorTools.DrawHeader("Vegetation Masks", _vegetationSystem.ShowVegetationMaskMenu);
                 if (_vegetationSystem.ShowVegetationMaskMenu)
                 {
                     GUILayout.BeginVertical("box");

                     EditorGUILayout.LabelField("Vegetation masks", LabelStyle);
                     _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].UseVegetationMask = EditorGUILayout.Toggle("Use with vegetation mask", _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].UseVegetationMask);
                     if (_vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].UseVegetationMask)
                     {
                         _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].VegetationTypeIndex = (VegetationTypeIndex)EditorGUILayout.EnumPopup("Vegetation type", _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].VegetationTypeIndex);
                     }
                     GUILayout.EndVertical();
                 }
                 */
                if (EditorGUI.EndChangeCheck())
                {
                    _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].UpdateCurves();

                    EditorUtility.SetDirty(_vegetationSystem.currentVegetationPackage);
                    _vegetationSystem.DelayedClearVegetationCellCache();
                    EditorUtility.SetDirty(target);
                }


                GUILayout.EndVertical();
            }

            _lastVegIndex = _vegIndex;
        }
        //ItemHeader(ref currentItem.UseHeightLevel, "Height Level");
        bool ItemHeader(ref bool itemParam, string name)
        {
            GUILayout.BeginHorizontal();
            GUI.color = (itemParam) ? Color.red : Color.yellow;
            string buttonName = (itemParam) ? "X" : "+";
            if (GUILayout.Button(buttonName, GUILayout.Width(20), GUILayout.Height(20)))
                itemParam = !itemParam;
            GUI.color = Color.white;
            TextAnchor old = GUI.skin.label.alignment;
            FontStyle old2 = GUI.skin.label.fontStyle;
            GUI.skin.label.alignment = TextAnchor.MiddleLeft;
            GUI.skin.label.fontStyle = (itemParam) ? FontStyle.Bold : FontStyle.Normal;
            GUILayout.Label(name, GUI.skin.FindStyle("Label"), GUILayout.Width(150f), GUILayout.Height(20));
            GUI.skin.label.alignment = old;
            GUI.skin.label.fontStyle = old2;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            return itemParam;
        }

        

        

        void DrawSelectPackageSimple()
        {
            float width = EditorGUIUtility.currentViewWidth;
            stringSize = (width - 50) / 3f;
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.Space(5);
            
            GUILayout.BeginVertical(GUILayout.Width(stringSize));

            //GUILayout.Label("Selected vegetation package", EditorStyles.boldLabel);
            GUILayout.Space(5);

            int _placingIndex = 0;
            string[] packagePlacingDataNameList = new string[_vegetationSystem.vsm.currentVegetationPlacingDatas.Length + 1];
            
            packagePlacingDataNameList[0] = "default from Package";
            for (int i = 0; i < _vegetationSystem.vsm.currentVegetationPlacingDatas.Length; i++)
            {
                if (_vegetationSystem.currentVegetationPlacingData == _vegetationSystem.vsm.currentVegetationPlacingDatas[i])
                    _placingIndex = i+1;
                if (_vegetationSystem.vsm.currentVegetationPlacingDatas[i])
                    packagePlacingDataNameList[i+1] = (i + 1).ToString() + " " + _vegetationSystem.vsm.currentVegetationPlacingDatas[i].PlacingDataName;
                else
                    packagePlacingDataNameList[i+1] = "Not found";
            }
            

            //int oldPlacingIndex = _placingIndex;
            EditorGUI.BeginChangeCheck();
            _placingIndex = EditorGUILayout.Popup("", _placingIndex, packagePlacingDataNameList, GUILayout.Width(stringSize));
//
            if (EditorGUI.EndChangeCheck())
            {
                if (_placingIndex == 0)
                {
                    _vegetationSystem.currentVegetationPlacingData = null;
                     _vegetationSystem.RefreshVegetationPackage();
                }
                else
                {
                    _vegetationSystem.currentVegetationPlacingData = _vegetationSystem.vsm.currentVegetationPlacingDatas[_placingIndex-1];
                    _vegetationSystem.RefreshVegetationPackage();
                    _vegetationSystem.currentVegetationPlacingData.UpdatePlacingData();
                }
                
                EditorUtility.SetDirty(_vegetationSystem);
           }
            GUILayout.EndVertical();
            
            GUILayout.Space(5);

            GUI.color = new Color(1, 0.4f, 0);
            if (GUILayout.Button("Bake All", GUILayout.Width(stringSize)))
            {
#if PERSISTENT_VEGETATION
                PersistentVegetationStorage persistentVegetationStorage = _vegetationSystem.GetComponent<PersistentVegetationStorage>();
                if (persistentVegetationStorage != null)
                {
                    List<string> vegetationItemIdList = VegetationPackageEditorTools.CreateVegetationInfoIdList(persistentVegetationStorage.VegetationSystem.currentVegetationPackage);

                    for (int i = 0; i <= vegetationItemIdList.Count - 1; i++)
                    {
                        if (_vegetationSystem.GetVegetationInfo(vegetationItemIdList[i]).IncludeDetailLayer > -2)
                        {
                            persistentVegetationStorage.RemoveVegetationItemInstances(vegetationItemIdList[i], 0);
                            persistentVegetationStorage.BakeVegetationItem(vegetationItemIdList[i]);
                        }
                    }
                    persistentVegetationStorage.VegetationSystem.DelayedClearVegetationCellCache();
                    EditorUtility.SetDirty(persistentVegetationStorage.PersistentVegetationStoragePackage);
                    EditorUtility.SetDirty(persistentVegetationStorage.VegetationSystem.currentVegetationPackage);
                    if (persistentVegetationStorage.VegetationSystem.currentVegetationPlacingData!=null)
                    EditorUtility.SetDirty(persistentVegetationStorage.VegetationSystem.currentVegetationPlacingData);
                   // _vegetationSystem.SetupVegetationCells();
                   // _vegetationSystem.SetupCullingGroup();
                }
#endif
            }
            GUI.color = Color.white;
            GUILayout.Space(5);

            GUI.color = (_vegetationSystem.vegetationSettings.isRenderRuleInstances) ? Color.green : Color.red;
            if (GUILayout.Button((_vegetationSystem.vegetationSettings.isRenderRuleInstances) ? "Enabled Renderer" : "Disabled Renderer", GUILayout.Width(stringSize)))
            {
                _vegetationSystem.vegetationSettings.isRenderRuleInstances = !_vegetationSystem.vegetationSettings.isRenderRuleInstances;
                //_vegetationSystem.Enable();
                //_vegetationSystem.SetupTerrainData(true, true);
                //SetDirty();
                /*_vegetationSystem.SetupVegetationSystem(true, true);
                //_vegetationSystem.UpdateVegetationCells();
                //_vegetationSystem.ClearVegetationCellCache();
                //_vegetationSystem.SetupVegetationCells();
               // _vegetationSystem.SetupCullingGroup();

                

                
                
                /*
                if (_vegetationSystem.InitDone)
                {
                    if (_vegetationSystem.OnSetVegetationPackageDelegate != null) _vegetationSystem.OnSetVegetationPackageDelegate(_vegetationSystem.currentVegetationPackage);
                }
                */
            }
            GUI.color = Color.white;
            GUILayout.Space(5);


            GUILayout.EndHorizontal();

            

            
        }
    }
}