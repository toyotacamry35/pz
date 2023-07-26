using AwesomeTechnologies.Billboards;
using AwesomeTechnologies.Colliders;
#if TOUCH_REACT
using AwesomeTechnologies.TouchReact;
#endif 
using AwesomeTechnologies.Vegetation.PersistentStorage;
using UnityEngine;
using UnityEditor;
using System.IO;
using AwesomeTechnologies.VegetationStudio;
using AwesomeTechnologies.Utility;
using System.Collections.Generic;
using System.Linq;

namespace AwesomeTechnologies.Common
{
    public partial class VegetationStudioManagerEditor : VegetationStudioBaseEditor
    {
        private string dirName;
        private string localDirName;
        private string pathName;
        private VegetationStudioManager current;
        private string placingDataName = "new Placing Data";
        private Material mat;
        private GameObject grass;

        private float width;
        public List<int>[] infoListSort;
        private int currentSelected = -1;
        private bool isSelected = false;


        private float borderSize = 12f;
        private float stringSize = 160f;

        private Rect m_SavePresetRect;
        private GrassPresetNamePopup m_PresetNamePopup;
        private VegetationPackage m_WaitingToLoad = null;
        private bool isRealWaiting = false;

        GameObject grassSetup;

        private static readonly string[] Lod =
        {
            "LOD2","LOD1", "LOD0"
        };

        public void DrawDragDrop(Rect rect, int select)
        {
            Event evt = Event.current;
            switch (evt.type)
            {
                case EventType.MouseDown:
                    {
                        if (!rect.Contains(evt.mousePosition))
                            return;

                        //Debug.Log(select.ToString());
                        DragAndDrop.PrepareStartDrag();// reset data

                        CustomDragData dragData = new CustomDragData(select, -1);
                        DragAndDrop.SetGenericData("Drag Grass Manager", dragData);

                        evt.Use();
                    }
                    break;
                case EventType.MouseDrag:
                    {
                        if (!rect.Contains(evt.mousePosition))
                            return;

                        CustomDragData existingDragData = DragAndDrop.GetGenericData("Drag Grass Manager") as CustomDragData;

                        if (existingDragData != null)
                        {
                            DragAndDrop.StartDrag("Dragging List ELement");
                            evt.Use();
                        }
                    }
                    break;
                case EventType.DragUpdated:
                    DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                    evt.Use();
                    break;

                case EventType.Repaint:
                    {
                        if (DragAndDrop.visualMode == DragAndDropVisualMode.None ||
                        DragAndDrop.visualMode == DragAndDropVisualMode.Rejected)
                        {

                            break;
                        }
                        CustomDragData existingDragData = DragAndDrop.GetGenericData("Drag Grass Manager") as CustomDragData;

                        if (rect.Contains(evt.mousePosition))
                        {
                            if (evt.mousePosition.y - (rect.yMin) < borderSize)
                            {
                                EditorGUI.DrawRect(new Rect(rect.x, rect.y, rect.width, borderSize), new Color(0.8f, 0.4f, 0.8f, 0.5f));
                            }
                            else
                             if (rect.yMax - evt.mousePosition.y < borderSize)
                            {
                                EditorGUI.DrawRect(new Rect(rect.x, rect.y + rect.height - borderSize, rect.width, borderSize), new Color(0.8f, 0.4f, 0.8f, 0.5f));
                            }
                            else
                                EditorGUI.DrawRect(new Rect(rect.x, rect.y + borderSize, rect.width, rect.height - borderSize * 2), new Color(0.8f, 0.8f, 0.4f, 0.5f));
                        }

                        if (select == existingDragData.originalIndex)
                            EditorGUI.DrawRect(new Rect(rect.x, rect.y + borderSize, rect.width, rect.height - borderSize * 2), new Color(0.4f, 0.8f, 0.8f, 0.5f));
                    }
                    break;

                case EventType.DragPerform:
                    {

                        if (!rect.Contains(evt.mousePosition))
                        {
                            return;
                        }

                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                        DragAndDrop.AcceptDrag();
                        CustomDragData receivedDragData = DragAndDrop.GetGenericData("Drag Grass Manager") as CustomDragData;
                        int old = receivedDragData.originalIndex;


                        if (evt.mousePosition.y - (rect.yMin) < borderSize)
                        {
                            current.context.VegetationInfoList[old].IncludeDetailLayer = current.context.VegetationInfoList[select].IncludeDetailLayer;
                        }
                        else
                        if (rect.yMax - evt.mousePosition.y < borderSize)
                        {
                            current.context.VegetationInfoList[old].IncludeDetailLayer = current.context.VegetationInfoList[select].IncludeDetailLayer;
                        }
                        else
                        {
                            int tempDetail = current.context.VegetationInfoList[old].IncludeDetailLayer;
                            current.context.VegetationInfoList[old].IncludeDetailLayer = current.context.VegetationInfoList[select].IncludeDetailLayer;
                            current.context.VegetationInfoList[select].IncludeDetailLayer = tempDetail;
                        }
                        evt.Use();
                        return;
                    }
                    break;
                case EventType.MouseUp:
                    DragAndDrop.PrepareStartDrag();

                    break;
            }
        }

        public void DrawElements(VegetationType type)
        {
            int fullCount = infoListSort[(int)type].Count;
            int lineCount = (int)width / 72;

            int columnCount = Mathf.FloorToInt(fullCount / lineCount) + 1;
            bool addedItem = false;

            GUILayout.Label(type.ToString(), EditorStyles.boldLabel);

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
                        int selectiveNumber = infoListSort[(int)type][currentCount];

                        if (selectiveNumber == currentSelected && isSelected)
                            GUI.color = Color.yellow;
                        else
                            GUI.color = Color.white;

                        if (GUILayout.Button(AssetPreview.GetAssetPreview(current.context.VegetationInfoList[infoListSort[(int)type][currentCount]].VegetationPrefab), GUI.skin.FindStyle("Box"), GUILayout.Width(68), GUILayout.Height(68)))
                        {
                            if (isSelected && currentSelected == selectiveNumber)
                                isSelected = false;
                            else
                            {
                                isSelected = true;
                                currentSelected = selectiveNumber;
                            }
                        }

                        GUI.color = Color.white;
                        if (selectiveNumber == currentSelected && isSelected)
                        {
                            isSelectiveInLine = true;
                            selectiveInLine = j;
                        }
                    }
                    else
                    {

                        switch (type)
                        {
                            case VegetationType.Grass:
                                {
                                    DropZoneTools.DrawVegetationItemDropZone(DropZoneType.GrassPrefab, current.context, ref addedItem);
                                    //if (addedItem)
                                    //    current.LoadFromContextPreset(current.context);
                                }
                                break;
                            case VegetationType.Plant:
                                {
                                    DropZoneTools.DrawVegetationItemDropZone(DropZoneType.PlantPrefab, current.context, ref addedItem);
                                    //if (addedItem)
                                    //current.LoadFromContextPreset(current.context);
                                }
                                break;
                            case VegetationType.Tree:
                                {
                                    DropZoneTools.DrawVegetationItemDropZone(DropZoneType.TreePrefab, current.context, ref addedItem);
                                    //if (addedItem)
                                    //current.LoadFromContextPreset(current.context);
                                }
                                break;
                            case VegetationType.Objects:
                                {
                                    DropZoneTools.DrawVegetationItemDropZone(DropZoneType.ObjectPrefab, current.context, ref addedItem);
                                    //if (addedItem)
                                    //current.LoadFromContextPreset(current.context);
                                }
                                break;
                        }
                        break;

                    }


                }
                GUILayout.EndHorizontal();

                
                if (isSelectiveInLine)
                {
                    
                    GUILayout.BeginHorizontal();
                    GUILayout.Box(AssetPreview.GetAssetPreview(current.context.VegetationInfoList[currentSelected].VegetationPrefab), GUILayout.Width(100), GUILayout.Height(100));
                    GUILayout.BeginVertical("box", GUILayout.Width(width - 170));
                    
                    EditorGUI.BeginChangeCheck();

                    VegetationItemInfo currentItem = current.context.VegetationInfoList[currentSelected];

                    currentItem.Name = EditorGUILayout.TextField(type.ToString() + " Name: ", currentItem.Name);

                    currentItem.VegetationType = (VegetationType)EditorGUILayout.EnumPopup("Type: ", currentItem.VegetationType);
                    currentItem.ShaderType = (VegetationShaderType)EditorGUILayout.EnumPopup("ShaderType: ", currentItem.ShaderType);

                    if (current.context.VegetationInfoList[currentSelected].PrefabType == VegetationPrefabType.Mesh)
                    {
                        GUILayout.BeginHorizontal();
                        GameObject oldPrefab = currentItem.VegetationPrefab;
                        currentItem.VegetationPrefab = EditorGUILayout.ObjectField("Vegetation prefab", currentItem.VegetationPrefab, typeof(GameObject), true) as GameObject;

                        if (oldPrefab != currentItem.VegetationPrefab)
                        {
                            currentItem.ShaderType = AwesomeTechnologies.Vegetation.VegetationTypeDetector.GetVegetationShaderType(currentItem.VegetationPrefab);

                            /*
                            if (currentItem.VegetationType == VegetationType.Tree)
                            {
                                current.context.GenerateBillboard(currentSelected);
                            }
                             */
                        }


                        if (GUILayout.Button("Refresh prefab"))
                        {
                            currentItem.ShaderType = AwesomeTechnologies.Vegetation.VegetationTypeDetector.GetVegetationShaderType(currentItem.VegetationPrefab);
                            current.VegetationSystemList[0].SetupVegetationPrefabs();

                            /*
                            if (currentItem.VegetationType == VegetationType.Tree)
                            {
                                current.context.GenerateBillboard(currentSelected);
                            }
                             */

                            var lodGroup = currentItem.VegetationPrefab?.GetComponent<LODGroup>();
                            if (lodGroup)
                            {
                                if (lodGroup.lodCount > 4)
                                {
                                    Debug.LogError($"More than 4 LODs for '{type}' vegetation item '{currentItem.Name}' (prefab: '{currentItem?.VegetationPrefab}')");
                                }
                                else
                                {
                                    var lods = lodGroup.GetLODs();
                                    var startLod = 2 - currentItem.LodIndex;
                                    if (lods.Length == 0)
                                    {
                                        Debug.LogError($"Zero LODs in LOD group for '{type}' vegetation item '{currentItem.Name}' (prefab: '{currentItem?.VegetationPrefab}')");
                                    }
                                    else
                                    {
                                        currentItem.lods = new List<VegetationLOD>();
                                        for (int k = startLod; k < lods.Length; k++)
                                        {
                                            var currentLodIndex = k - startLod;
                                            if (currentLodIndex > 3)
                                            {
                                                Debug.LogError($"More than 4 LODs for '{type}' vegetation item '{currentItem.Name}' (prefab: '{currentItem?.VegetationPrefab}'), skipping");
                                                break;
                                            }

                                            var renderer = lods[k].renderers;
                                            if (renderer.Length != 1)
                                            {
                                                Debug.LogError($"No or many renderers in {k} LOD for '{type}' vegetation item '{currentItem.Name}' (prefab: '{currentItem?.VegetationPrefab}')");
                                            }
                                            else
                                            {
                                                var mesh = renderer[0].GetComponent<MeshFilter>()?.sharedMesh;
                                                if (!mesh)
                                                {
                                                    Debug.LogError($"No mesh found for {k} LOD for '{type}' vegetation item '{currentItem.Name}' (prefab: '{currentItem?.VegetationPrefab}')");
                                                }
                                                else
                                                {
                                                    if (currentLodIndex == 0)
                                                    {
                                                        currentItem.sourceMesh = mesh;
                                                    }
                                                    else
                                                    {
                                                        currentItem.lods.Add(new VegetationLOD() { LODDistance = 100 / (lods.Length - startLod) * currentLodIndex, LODMaterial = currentItem.sourceSecondMaterial, LODMesh = mesh });
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                var mesh = currentItem.VegetationPrefab?.GetComponent<MeshFilter>()?.sharedMesh;
                                if (mesh)
                                {
                                    currentItem.sourceMesh = mesh;
                                }
                                else
                                {
                                    Debug.LogError($"No mesh found for '{type}' vegetation item '{currentItem.Name}' (prefab: '{currentItem?.VegetationPrefab}')");
                                }
                            }
                        }

                        GUILayout.EndHorizontal();

                        if (currentItem.VegetationPrefab == null)
                        {
                            EditorGUILayout.HelpBox("Missing vegetation prefab, item will be skipped", MessageType.Warning);
                        }

                    }
                    else
                    {
                        currentItem.VegetationTexture = EditorGUILayout.ObjectField("Vegetation texture", currentItem.VegetationTexture, typeof(Texture2D), true) as Texture2D;
                        if (currentItem.VegetationTexture == null)
                        {
                            EditorGUILayout.HelpBox("Missing vegetation texture, item will be skipped", MessageType.Warning);
                        }
                    }

                    if (currentItem.PrefabType == VegetationPrefabType.Mesh)
                    {
                        currentItem.LodIndex = EditorGUILayout.Popup("Selected initial LOD", currentItem.LodIndex, Lod);
                    }

                    currentItem.VegetationRenderType = (VegetationRenderType)EditorGUILayout.EnumPopup("Render mode", currentItem.VegetationRenderType);

                    if (EditorGUI.EndChangeCheck())
                    {
                        current.LoadFromContextPreset(current.context);
                        EditorUtility.SetDirty(current.context);
                    }

                    GUI.color = (current.isDebugMaterial) ? Color.red : Color.white;
                    if (GUILayout.Button("Set Vertex Colors"))
                    {
                        
                    }
                    

                    GUI.color = Color.white;
                    GUILayout.EndVertical();

                    GUI.color = Color.red;
                    GUILayout.Space(1);
                    if (GUILayout.Button("X", GUILayout.Width(20), GUILayout.Height(20)))
                    {
                        isSelected = false;
                        if (EditorUtility.DisplayDialog("Delete VegetationItem?",
                            "Do you want to delete the selected VegetationItem?", "Delete", "Cancel"))
                        {
#if PERSISTENT_VEGETATION
                            for (int z = 0; z < current.VegetationSystemList.Count; z++)
                            {
                                PersistentVegetationStorage persistentVegetationStorage = current.VegetationSystemList[i].GetComponent<PersistentVegetationStorage>();
                                persistentVegetationStorage.RemoveVegetationItemInstances(current.context.VegetationInfoList[currentSelected].VegetationItemID);
                                EditorUtility.SetDirty(persistentVegetationStorage.PersistentVegetationStoragePackage);
                                EditorUtility.SetDirty(persistentVegetationStorage.VegetationSystem.currentVegetationPackage);
                            }

                            current.context.VegetationInfoList.RemoveAt(currentSelected);
                            current.LoadFromContextPreset(current.context);
                            EditorUtility.SetDirty(current.context);

#endif

                            return;
                        }
                    }
                    GUI.color = Color.white;

                    GUILayout.EndHorizontal();
                    

                    GUILayout.Space(2);
                    
                    DrawTouchReactSettings(currentSelected);
                    DrawBillboardSettings(currentSelected);
                    DrawColliderSettings(currentSelected);
                    
                    //DrawNavMeshObstacleSettings(currentSelected);
                    DrawShadowSettings(currentSelected);
                    
                    //DrawLODSettings(currentSelected);
                    //DrawVegetationItemGrassSettings(currentSelected);
                    //DrawVegetationItemSpeedtreeSettings(currentSelected);
                    

                    //GUILayout.EndVertical();
                }
                
                }

                GUILayout.EndVertical();

            if (addedItem)
            {
                isSelected = false;
                currentSelected = -1;
                current.LoadFromContextPreset(current.context);
                return;
            }
        }

        void DrawNavMeshObstacleSettings(int _currentSelected)
        {
            VegetationItemInfo vegetationItemInfo = current.context.VegetationInfoList[_currentSelected];
            if (vegetationItemInfo.VegetationType != VegetationType.Tree &&
                vegetationItemInfo.VegetationType != VegetationType.Objects &&
                vegetationItemInfo.VegetationType != VegetationType.LargeObjects) return;


            EditorGUI.BeginChangeCheck();

            vegetationItemInfo.NavMeshObstacleType = (NavMeshObstacleType)EditorGUILayout.EnumPopup("NavMesh Obstacle Type", vegetationItemInfo.NavMeshObstacleType);
            GUILayout.Space(2);

            if (vegetationItemInfo.NavMeshObstacleType != NavMeshObstacleType.Disabled)
            {
                GUILayout.BeginVertical("box");
                switch (vegetationItemInfo.NavMeshObstacleType)
                {
                    case NavMeshObstacleType.Box:
                        {
                            vegetationItemInfo.NavMeshObstacleCenter = EditorGUILayout.Vector3Field("Center",
                                vegetationItemInfo.NavMeshObstacleCenter);
                            vegetationItemInfo.NavMeshObstacleSize = EditorGUILayout.Vector3Field("Size",
                                vegetationItemInfo.NavMeshObstacleSize);
                            vegetationItemInfo.NavMeshObstacleCarve = EditorGUILayout.Toggle("Carve",
                                vegetationItemInfo.NavMeshObstacleCarve);
                            break;
                        }
                    case NavMeshObstacleType.Capsule:
                        vegetationItemInfo.NavMeshObstacleCenter = EditorGUILayout.Vector3Field("Center",
                            vegetationItemInfo.NavMeshObstacleCenter);
                        vegetationItemInfo.NavMeshObstacleRadius = EditorGUILayout.FloatField("Radius",
                            vegetationItemInfo.NavMeshObstacleRadius);
                        vegetationItemInfo.NavMeshObstacleHeight = EditorGUILayout.FloatField("Height",
                            vegetationItemInfo.NavMeshObstacleHeight);
                        vegetationItemInfo.NavMeshObstacleCarve = EditorGUILayout.Toggle("Carve",
                            vegetationItemInfo.NavMeshObstacleCarve);
                        break;

                }
                GUILayout.EndVertical();
            }
            if (EditorGUI.EndChangeCheck())
            {

                //current.VegetationSystemList[0].RefreshColliders();
                EditorUtility.SetDirty(current.context);
            }
        }

        void DrawColliderSettings(int _currentSelected)
        {

            VegetationItemInfo vegetationItemInfo = current.context.VegetationInfoList[_currentSelected];
            if (vegetationItemInfo.VegetationType != VegetationType.Tree &&
                vegetationItemInfo.VegetationType != VegetationType.Objects &&
                vegetationItemInfo.VegetationType != VegetationType.LargeObjects) return;


            EditorGUI.BeginChangeCheck();

            vegetationItemInfo.ColliderType = (ColliderType)EditorGUILayout.EnumPopup("Collider type", vegetationItemInfo.ColliderType);
            GUILayout.Space(2);

            if (vegetationItemInfo.ColliderType != ColliderType.Disabled)
            {
                //GUI.color = new Color(0.8f, 0.8f, 0.8f);
                GUILayout.BeginVertical("box");
                //GUI.color = Color.white;


                switch (vegetationItemInfo.ColliderType)
                {
                    case ColliderType.Capsule:
                        {

                            vegetationItemInfo.ColliderRadius = EditorGUILayout.FloatField("Radius", vegetationItemInfo.ColliderRadius);
                            vegetationItemInfo.ColliderHeight = EditorGUILayout.FloatField("Height", vegetationItemInfo.ColliderHeight);
                            vegetationItemInfo.ColliderOffset = EditorGUILayout.Vector3Field("Offset", vegetationItemInfo.ColliderOffset);
                            break;
                        }
                    case ColliderType.Sphere:
                        {
                            vegetationItemInfo.ColliderRadius = EditorGUILayout.FloatField("Radius", vegetationItemInfo.ColliderRadius);
                            vegetationItemInfo.ColliderOffset = EditorGUILayout.Vector3Field("Offset", vegetationItemInfo.ColliderOffset);

                            break;
                        }
                    case ColliderType.CustomMesh:
                        {
                            vegetationItemInfo.ColliderMesh = (Mesh)EditorGUILayout.ObjectField("Custom mesh", vegetationItemInfo.ColliderMesh, typeof(Mesh), false);
                            break;
                        }
                }
                if (vegetationItemInfo.ColliderType != ColliderType.Disabled)
                {
                    vegetationItemInfo.ColliderTrigger = EditorGUILayout.Toggle("Trigger", vegetationItemInfo.ColliderTrigger);
                    vegetationItemInfo.ColliderUseForBake = EditorGUILayout.Toggle("Include in NavMesh bake", vegetationItemInfo.ColliderUseForBake);
                }

                GUILayout.EndVertical();

            }

            if (EditorGUI.EndChangeCheck())
            {

                //current.VegetationSystemList[0].RefreshColliders();
                EditorUtility.SetDirty(current.context);
            }
        }

        void DrawLODSettings(int _currentSelected)
        {

            VegetationItemInfo vegetationItemInfo = current.context.VegetationInfoList[_currentSelected];

            if ((vegetationItemInfo.VegetationType == VegetationType.Grass || vegetationItemInfo.VegetationType == VegetationType.Plant) && vegetationItemInfo.VegetationRenderType != VegetationRenderType.InstancedIndirect)
                return;

            if (current.VegetationSystemList == null)
                return;
            else
                if (current.VegetationSystemList.Count == 0)
                    return;

            VegetationItemModelInfo vegetationItemModelInfo = current.VegetationSystemList[0].GetVegetationModelInfo(_currentSelected);
            if (vegetationItemModelInfo.LOD1Distance < 1) return;

            EditorGUI.BeginChangeCheck();

            vegetationItemInfo.DisableLOD = EditorGUILayout.Toggle("Disable LODs", vegetationItemInfo.DisableLOD);

            //if (!vegetationItemInfo.DisableLOD)
            //{
            //    EditorGUILayout.BeginVertical("box");
            //    vegetationItemInfo.LODFactor = EditorGUILayout.Slider("LOD distance factor", vegetationItemInfo.LODFactor, 0.15f, 20f);

            //    if (vegetationItemModelInfo != null)
            //    {
            //        float currentLOD1Distance = vegetationItemModelInfo.LOD1Distance * QualitySettings.lodBias * vegetationItemInfo.LODFactor;
            //        float currentLOD2Distance = vegetationItemModelInfo.LOD2Distance * QualitySettings.lodBias * vegetationItemInfo.LODFactor;

            //        VegetationPackageEditorTools.DrawLODRanges(currentLOD1Distance, currentLOD2Distance, current.VegetationSystemList[0].GetVegetationDistance() + current.VegetationSystemList[0].GetTreeDistance());
            //    }
            //    EditorGUILayout.EndVertical();
            //}
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(current.context);
            }

        }

        void DrawVegetationItemSpeedtreeSettings(int _currentSelected)
        {
            VegetationItemInfo vegetationItemInfo = current.context.VegetationInfoList[_currentSelected];
            if (vegetationItemInfo.ShaderType != VegetationShaderType.Speedtree) return;

            GUILayout.BeginVertical("box");
            EditorGUI.BeginChangeCheck();

            vegetationItemInfo.Hue = EditorGUILayout.ColorField("HUE", vegetationItemInfo.Hue);
            vegetationItemInfo.ColorTint1 = EditorGUILayout.ColorField("Color tint", vegetationItemInfo.ColorTint1);

            
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(current.context);

                current.VegetationSystemList[0].RefreshVegetationPackage();
                VegetationStudioManager.VegetationPackageSync_ClearVegetationSystemCellCache(current.VegetationSystemList[0]);

            }
            GUILayout.EndVertical();
        }

        void DrawTouchReactSettings(int _currentSelected)
        {
            VegetationItemInfo vegetationItemInfo = current.context.VegetationInfoList[_currentSelected];
            
            if (vegetationItemInfo.VegetationType == VegetationType.Objects || vegetationItemInfo.VegetationType == VegetationType.LargeObjects)
            {
                EditorGUI.BeginChangeCheck();

                vegetationItemInfo.UseTouchReact = EditorGUILayout.Toggle("Enable touch react", vegetationItemInfo.UseTouchReact);

                if (EditorGUI.EndChangeCheck())
                {
                    current.VegetationSystemList[0].RefreshVegetationPackage();
                    EditorUtility.SetDirty(current.context);
                }
            }
            
        }

        void DrawBillboardSettings(int _currentSelected)
        {
            VegetationItemInfo vegetationItemInfo = current.context.VegetationInfoList[_currentSelected];
            bool billboardChanged = false;

            if (vegetationItemInfo.VegetationType == VegetationType.Tree)
            {

                EditorGUI.BeginChangeCheck();
                vegetationItemInfo.UseBillboards = EditorGUILayout.Toggle("Enable billboards", vegetationItemInfo.UseBillboards);
                GUILayout.Space(2);

                if (EditorGUI.EndChangeCheck())
                {
                    billboardChanged = true;
                    
                    current.VegetationSystemList[0].RefreshBillboards();
                    EditorUtility.SetDirty(current.context);
                }



                if (vegetationItemInfo.UseBillboards)
                {
                    GUILayout.BeginVertical("box");

                    EditorGUILayout.LabelField("Runtime settings", EditorStyles.boldLabel);
                    GUILayout.Space(1);

                    vegetationItemInfo.BillboardCutoff = EditorGUILayout.Slider("Alpha cutoff", vegetationItemInfo.BillboardCutoff, 0f, 1f);
                    vegetationItemInfo.BillboardBrightness = EditorGUILayout.Slider("Brightness", vegetationItemInfo.BillboardBrightness, 0.5f, 5f);
                    vegetationItemInfo.BillboardMipmapBias = EditorGUILayout.Slider("Mipmap bias", vegetationItemInfo.BillboardMipmapBias, -3f, 0f);
                    vegetationItemInfo.BillboardTintColor = EditorGUILayout.ColorField("Tint color", vegetationItemInfo.BillboardTintColor);


                    //EditorGUI.BeginChangeCheck();


                    EditorGUILayout.LabelField("Generation settings", EditorStyles.boldLabel);
                    GUILayout.Space(1);
                    BillboardQuality oldBillboardQuality = vegetationItemInfo.BillboardQuality;
                    vegetationItemInfo.BillboardQuality = (BillboardQuality)EditorGUILayout.EnumPopup("Billboard quality", vegetationItemInfo.BillboardQuality);

                    if (vegetationItemInfo.ShaderType == VegetationShaderType.Speedtree)
                    {
                        EditorGUI.BeginChangeCheck();
                        vegetationItemInfo.BillboardLodIndex = EditorGUILayout.Popup("Source LOD", vegetationItemInfo.BillboardLodIndex, Lod);

                        if (EditorGUI.EndChangeCheck())
                        {
                            current.context.GenerateBillboard(_currentSelected);
                            EditorUtility.SetDirty(current.context);
                        }
                    }

                    if (oldBillboardQuality != vegetationItemInfo.BillboardQuality)
                    {
                        current.context.GenerateBillboard(_currentSelected);
                        EditorUtility.SetDirty(current.context);
                    }

                    GUILayout.BeginHorizontal();
                    vegetationItemInfo.BillboardTexture = EditorGUILayout.ObjectField("Billboard texture", vegetationItemInfo.BillboardTexture, typeof(Texture2D), true) as Texture2D;
                    vegetationItemInfo.BillboardNormalTexture = EditorGUILayout.ObjectField("Billboard normals", vegetationItemInfo.BillboardNormalTexture, typeof(Texture2D), true) as Texture2D;
                    GUILayout.EndHorizontal();
                    if (GUILayout.Button("Regenerate billboard"))
                    {
                        current.context.GenerateBillboard(current.context.VegetationInfoList[_currentSelected].VegetationItemID);
                        EditorUtility.SetDirty(current.context);
                    }

                    GUILayout.EndVertical();
                }



                /*
                if (EditorGUI.EndChangeCheck())
                {
                    billboardChanged = true;
                }
                */
            }

            if (billboardChanged)
            {

                current.VegetationSystemList[0].RefreshBillboards();
                EditorUtility.SetDirty(current.context);
            }
        }

       
        void DrawShadowSettings(int _currentSelected)
        {
            VegetationItemInfo vegetationItemInfo = current.context.VegetationInfoList[_currentSelected];
            EditorGUI.BeginChangeCheck();
            
            vegetationItemInfo.DisableShadows = EditorGUILayout.Toggle("Disable Shadows", vegetationItemInfo.DisableShadows);

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(current.context);
            }
        }

        public void BakeAllGrass()
        {
            current.BakeAllGrass();
        }

        public void ReturnRealtimeGrass()
        {
            current.ReturnRealtimeGrass();
        }

        public void PresetSettings()
        {
            width = EditorGUIUtility.currentViewWidth;

            if (current.context.VegetationInfoList == null)
            {
                GUILayout.Label("Fuck");
                return;
            }

            infoListSort = new List<int>[4];
            for (int i = 0; i < infoListSort.Length; i++)
                infoListSort[i] = new List<int>();


            for (int i = 0; i < current.context.VegetationInfoList.Count; i++)
            {
                infoListSort[(int)current.context.VegetationInfoList[i].VegetationType].Add(i);
            }

            DrawElements(VegetationType.Grass);
            DrawElements(VegetationType.Plant);
            DrawElements(VegetationType.Tree);
            DrawElements(VegetationType.Objects);

            if (current.VegetationSystemList.Count == 0)
            {
                GUI.color = Color.green;
                if (GUILayout.Button("Add To Terrains"))
                {
                    current.AddToTerrains();
                }
                GUI.color = Color.white;
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                {
                    GUI.color = new Color(1, 0.4f, 0);
                    if (GUILayout.Button("Bake All Grass", GUI.skin.FindStyle("ButtonLeft")))
                    {
                        BakeAllGrass();
                    }

                    GUI.color = Color.green;
                    if (GUILayout.Button("Return Realtime Grass", GUI.skin.FindStyle("ButtonMid")))
                    {
                        ReturnRealtimeGrass();
                    }

                    GUI.color = Color.white;
                }
                EditorGUILayout.EndHorizontal();
            }
            

            if (Event.current.type == EventType.Repaint && m_WaitingToLoad != null && isRealWaiting)
            {
                isRealWaiting = false;
                current.LoadFromContextPreset(m_WaitingToLoad);
                m_WaitingToLoad = null;
            }
        }

        void LoadSavePreset()
        {
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Create Preset", GUI.skin.FindStyle("ButtonLeft")))
                {
                    PopupWindow.Show(m_SavePresetRect, m_PresetNamePopup);
                }
                if (Event.current.type == EventType.Repaint) m_SavePresetRect = GUILayoutUtility.GetLastRect();

                if (GUILayout.Button("Load Preset", GUI.skin.FindStyle("ButtonRight")))
                    current.LoadFromContextPreset(current.context);
            }
            EditorGUILayout.EndHorizontal();

            if (Event.current.commandName == "ObjectSelectorClosed")
            {
                m_WaitingToLoad = EditorGUIUtility.GetObjectPickerObject() as VegetationPackage;
                isRealWaiting = true;
            }

            GUILayout.Label("Placing Data", EditorStyles.boldLabel);
            if (current.currentVegetationPlacingDatas.Length > 0)
            {
                for (int i = 0; i < current.currentVegetationPlacingDatas.Length; i++)
                {
                    if (current.currentVegetationPlacingDatas[i])
                    {
                        string packageName = current.currentVegetationPlacingDatas[i].PlacingDataName;
                        if (packageName == "") packageName = "No name";

                        GUILayout.BeginHorizontal();

                        VegetationPlacingData updatedPackage = EditorGUILayout.ObjectField(packageName, current.currentVegetationPlacingDatas[i], typeof(VegetationPlacingData), true) as VegetationPlacingData;
                        if (updatedPackage != current.currentVegetationPlacingDatas[i])
                        {
                            if (updatedPackage == null)
                            {
                                //_vegetationSystem.CurrentVegetationPackage.vegetationPlacingDatas.Remove(_vegetationSystem.CurrentVegetationPackage.vegetationPlacingDatas[i]);
                                /*
                                if (_vegetationSystem.VegetationPackageList.Count > 0)
                                {
                                    _vegetationSystem.CurrentVegetationPackage.vegetationPlacingDatas
                                }
                                else
                                {
                                    _vegetationSystem.CurrentVegetationPackage = null;
                                    _vegetationSystem.InitDone = false;
                                }
                                */
                            }
                            else
                            {
                                current.currentVegetationPlacingDatas[i] = updatedPackage;
                            }
                        }
                        GUILayout.EndHorizontal();
                    }
                    else
                    {
                        //_vegetationSystem.VegetationPackageList[i] = EditorGUILayout.ObjectField("Empty", _vegetationSystem.VegetationPackageList[i], typeof(VegetationPackage), true) as VegetationPackage;
                    }
                }
            }

            GUILayout.BeginHorizontal();
            placingDataName = EditorGUILayout.TextField("New name: ", placingDataName);

            if (GUILayout.Button("Add Placing Data"))
            {
                CreatePlacingData(placingDataName);
            }
            GUILayout.EndHorizontal();
        }

        public void CreatePlacingData(string _name)
        {
#if UNITY_EDITOR

            string[] paths = Directory.GetDirectories(Application.dataPath, "PresetsVegetation", SearchOption.AllDirectories);
            if (paths.Length == 0 || paths.Length > 1)
            {
                Debug.LogError("GrassMergerEditor::CreateGrassPreset: Unable to find the Grass Manager folder! Has it been renamed?");
                return;
            }
            int assetind = paths[0].IndexOf("Assets", 0);
            string rootpath = paths[0].Substring(assetind);
            string contextpath = rootpath + Path.DirectorySeparatorChar + "Contexts";

            if (!AssetDatabase.IsValidFolder(contextpath))
            {
                AssetDatabase.CreateFolder(rootpath, "Contexts");
            }

            string localName = _name + ".asset";

            VegetationPlacingData asset = ScriptableObject.CreateInstance<VegetationPlacingData>();

            asset.parentPackage = current.context;
            asset.VegetationInfoList = new VegetationItemInfo[current.context.VegetationInfoList.Count];

            asset.PlacingDataName = _name;
            current.context.VegetationInfoList.CopyTo(asset.VegetationInfoList);

            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(contextpath + Path.DirectorySeparatorChar + localName + ".asset");
            AssetDatabase.CreateAsset(asset, assetPathAndName);

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();

            current.LoadFromContextPreset(current.context);
#endif
        }
        void DisplayMerger()
        {
            GUILayout.Label("Atlas Blueprint", EditorStyles.boldLabel);
            GUILayout.Space(1);


            if (current.cells == null)
                current.cells = new List<GrassAtlasCell>();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Atlas Size: " + 512 * current.size, EditorStyles.largeLabel);

            GUILayout.Space(8);
            if (current.size > 2)
                GUI.enabled = true;
            else
                GUI.enabled = false;

            if (GUILayout.Button("▼", GUILayout.Width(25)))
            {
                current.size--;
                current.Create();
            }

            GUI.enabled = true;

            GUILayout.Space(8);
            GUILayout.Label(current.size.ToString(), EditorStyles.largeLabel, GUILayout.Width(15));
            GUILayout.Space(8);
            if (current.size < 6)
                GUI.enabled = true;
            else
                GUI.enabled = false;

            if (GUILayout.Button("▲", GUILayout.Width(25)))
            {
                current.size++;
                current.Create();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUI.enabled = true;
            float currentWidth = EditorGUIUtility.currentViewWidth;
            float xSize = 2 + 102 * current.size;
            float x = (currentWidth - xSize) / 2;
            Rect rect = GUILayoutUtility.GetRect(1f, (float)(xSize - x), 1f, (float)(2 + 67 * current.size), EditorStyles.boldLabel);

            rect.x = x;
            rect.width = xSize;
            GUI.BeginGroup(rect, GUI.skin.FindStyle("Box"));
            for (int i = 0; i < current.size; i++)
            {
                for (int j = 0; j < current.size; j++)
                {
                    int arrayNum = i * current.size + j;
                    switch (current.array[arrayNum].arrayType)
                    {
                        case ArrayType.Free:
                            {
                                GUI.BeginGroup(new Rect(2f + 102f * i, 2f + 67f * j, 100f, 65f));
                                DrawDragDrop(new Rect(2, 2, 96, 61), i, j);
                                GUI.EndGroup();
                            }
                            break;
                        case ArrayType.Used:
                            {
                                GrassAtlasCell currentCell = current.GetCell(i, j);
                                float box_x = 100 * currentCell.size_x + 2 * Mathf.Clamp(currentCell.size_x - 1, 0, 3);
                                float box_y = 65 * currentCell.size_y + 2 * Mathf.Clamp(currentCell.size_y - 1, 0, 3);

                                GUI.BeginGroup(new Rect(2f + 102f * i, 2f + 67f * j, box_x, box_y));
                                GUI.color = Color.gray;

                                GUI.Box(new Rect(0, 0, box_x, box_y), "", GUI.skin.FindStyle("Box"));
                                GUI.color = Color.white;

                                float box2_x = 100 * currentCell.size_x + 2 * Mathf.Clamp(currentCell.size_x - 1, 0, 3) - 4;
                                float box2_y = 65 * currentCell.size_y + 2 * Mathf.Clamp(currentCell.size_y - 1, 0, 3) - 4;



                                if (currentCell == current.currentAtlasCell && current.isSelected)
                                    GUI.color = Color.yellow;

                                if (GUI.Button(new Rect(2, 2, box2_x - 15, box2_y), currentCell.tex2D, GUI.skin.FindStyle("Box")))
                                {
                                    if (currentCell == current.currentAtlasCell)
                                    {
                                        current.isSelected = false;
                                    }
                                    else
                                    {
                                        if (current.array[j * current.size + i] != null)

                                            for (int _x = 0; _x < current.size; _x++)
                                                for (int _y = 0; _y < current.size; _y++)
                                                {
                                                    if (_x == i && _y == j)
                                                    {
                                                        current.isSelected = true;
                                                        current.currentAtlasCell = currentCell;
                                                    }
                                                }
                                    }
                                    //current.currentCell = current.cells[j * current.size + i].;
                                }

                                GUI.color = Color.red;
                                if (GUI.Button(new Rect(2 + box2_x - 15, 2, 15, 15), "X"))
                                {
                                    current.DeleteCell(i, j);
                                }
                                GUI.color = Color.white;



                                GUI.EndGroup();
                            }
                            break;

                    }

                }
            }
            GUI.EndGroup();

            GUILayout.Label("New Atlas Save Data", EditorStyles.boldLabel);
            GUILayout.Space(1);
            GUILayout.BeginVertical(GUI.skin.GetStyle("Box"));
            GUILayout.Space(3);

            GUILayout.Label("Atlas Name");
            current.atlasName = EditorGUILayout.TextField("", current.atlasName);
            GUILayout.Label("Directory Name");
            GUILayout.BeginHorizontal();
            current.directory = EditorGUILayout.TextField("", current.directory);
            if (GUILayout.Button("..."))
            {
                string temp = current.directory;
                current.directory = EditorUtility.SaveFolderPanel("Select Atlas Directory", current.directory, current.atlasName);

                if (current.directory == "")
                    current.directory = temp;
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUI.color = Color.green;
            if (GUILayout.Button("Bake Atlas", GUILayout.Height(35)))
            {
                SaveAtlasMode();
            }
            GUI.color = Color.white;
            /*
            if (GUILayout.Button("Atlas Rebake", GUILayout.Height(35)))
            {
                CreateAtlasPreset();
            }
            */
        }

        public void DrawMerger()
        {
            current = (VegetationStudioManager)target;

            if (!current.isCreated)
                current.Create();

            current.mode = 1;

            GUI.color = Color.white;

                        //EditorGUI.BeginChangeCheck();

                        current.context = (VegetationPackage)EditorGUILayout.ObjectField("Default Preset", current.context, typeof(VegetationPackage), false);
                        LoadSavePreset();
                        PresetSettings();
                        /*
                        if (EditorGUI.EndChangeCheck())
                        {
                            current.LoadFromContextPreset(current.context);
                            EditorUtility.SetDirty(current.context);
                        }
                         */
        }

        public void DrawDragDrop(Rect rect, int _x, int _y)
        {
            Event evt = Event.current;

            GUI.Box(rect, new GUIContent(""), GUI.skin.FindStyle("Button"));

            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:

                    if (!rect.Contains(evt.mousePosition))
                    {
                        return;
                    }

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        if (DragAndDrop.objectReferences != null)
                        {
                            Texture2D tex2D = current.GetTextureFromObject(DragAndDrop.objectReferences[0]);
                            int size_x = tex2D.width / 512;
                            int size_y = tex2D.height / 512;
                            //Debug.Log(tex2D.width + " " + size_x + " " + size_y);

                            GrassAtlasCell currentCell = current.AddCell(_x, _y, size_x, size_y);

                            currentCell.sourceGrass = DragAndDrop.objectReferences[0];
                            currentCell.GetTexture();

                        }
                    }
                    break;
            }
        }

        private void OnEnable()
        {

            m_PresetNamePopup = new GrassPresetNamePopup();
            m_PresetNamePopup.OnCreate += CreateGrassPreset;
        }

        public void CreateGrassPreset(string name)
        {
            CreateAtlasPreset(name);

           

        }




        Mesh CalculateMeshUV(GameObject _grass, string assetName, GrassAtlasCell _cell, out float power)
        {

            string pathName = localDirName + "/Sources/" + assetName + ".asset";

            Mesh mesh = _grass.GetComponent<MeshFilter>().sharedMesh;

            Mesh newMesh = new Mesh();
            newMesh.vertices = mesh.vertices;
            newMesh.triangles = mesh.triangles;
            newMesh.uv = mesh.uv;
            newMesh.uv2 = mesh.uv2;
            newMesh.normals = mesh.normals;
            newMesh.colors32 = mesh.colors32;
            newMesh.tangents = mesh.tangents;
            newMesh.subMeshCount = mesh.subMeshCount;


            Vector2 newCenterPos = _cell.center;
            //Debug.Log(newCenterPos);
            float newSizeX = 1f / current.size * _cell.size_x;
            float newSizeY = 1f / current.size * _cell.size_y;
            int vertexCount = newMesh.vertexCount;

            Vector2[] uv = newMesh.uv;
            Color[] color = newMesh.colors;
            power = 0;
            for (int z = 0; z < vertexCount; z++)
            {
                power += color[z].a;
                float xOldOffset = uv[z].x - 0.5f;
                float yOldOffset = uv[z].y - 0.5f;

                float xOldResize = xOldOffset * newSizeX;
                float yOldResize = yOldOffset * newSizeY;

                float xNewPos = newCenterPos.x + xOldResize;
                float yNewPos = newCenterPos.y + yOldResize;

                uv[z] = new Vector2(xNewPos, yNewPos);
            }
            newMesh.uv = uv;
            for (int j = 0; j < mesh.subMeshCount; j++)
                newMesh.SetTriangles(mesh.GetTriangles(j), j);
            newMesh.RecalculateNormals();

            AssetDatabase.CreateAsset(newMesh, pathName);
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();

            return newMesh;
        }

        Texture2D SaveAtlasTexture(Texture2D atlas, string suffix)
        {
            pathName = "/Sources/" + current.atlasName + "Atlas" + suffix + ".tga";

            byte[] bytes = VacuumShaders.TextureExtensions.TGA.EncodeToTGA(atlas);
            System.IO.File.WriteAllBytes(dirName + pathName, bytes);

            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            AssetDatabase.SaveAssets();


            /*

            TextureImporter textureImporter = AssetImporter.GetAtPath(localDirName + pathName) as TextureImporter;
            textureImporter.isReadable = true;
            textureImporter.npotScale = TextureImporterNPOTScale.ToNearest;
            AssetDatabase.ImportAsset(localDirName + pathName, ImportAssetOptions.ForceUpdate);

            if (atlas != null)
            {
                if (AssetDatabase.GetAssetPath(atlas) == "")
                {
                    DestroyImmediate(atlas);
                    atlas = null;
                }
            }

            AssetDatabase.SaveAssets();
            */
            return (Texture2D)AssetDatabase.LoadAssetAtPath(localDirName + pathName, typeof(Texture2D));
        }

        void AddPrototypeTextureToAtlas(ref Texture2D atlas, ref Texture2D atlasEmission, GrassAtlasCell _cell, int atlasSize)
        {
            grass = GameObject.Instantiate(_cell.sourceGrass) as GameObject;
            mat = grass.GetComponent<Renderer>().sharedMaterial;

            int curWidth = _cell.tex2D.width;
            int curHeight = _cell.tex2D.height;
            Color[] cols = new Color[curWidth * curHeight];
            if (_cell.tex2D != null)
            {
                cols = _cell.tex2D.GetPixels();
                atlas.SetPixels(_cell.x * (int)current.atlasSize, atlasSize - curHeight - _cell.y * (int)current.atlasSize, (int)current.atlasSize * _cell.size_x, (int)current.atlasSize * _cell.size_y, cols);
            }

            if (_cell.tex2D_EM != null)
            {
                cols = _cell.tex2D_EM.GetPixels();
                atlasEmission.SetPixels(_cell.x * (int)current.atlasSize, atlasSize - curHeight - _cell.y * (int)current.atlasSize, (int)current.atlasSize * _cell.size_x, (int)current.atlasSize * _cell.size_y, cols);
            }
            GameObject.DestroyImmediate(grass);
        }

        Material CreateMaterial(Texture2D diffuseAtlas, Texture2D emissionAtlas)
        {
            grass = GameObject.Instantiate(current.cells[0].sourceGrass) as GameObject;
            mat = grass.GetComponent<Renderer>().sharedMaterial;

            Material atlasMat = new Material(mat);
            atlasMat.CopyPropertiesFromMaterial(mat);
            atlasMat.shader = Shader.Find("AwesomeTechnologies/Grass/GrassStandard");
            atlasMat.SetTexture("_MainTex", diffuseAtlas);
            atlasMat.SetTexture("_SpecTex", emissionAtlas);
            atlasMat.SetColor("_Color", new Color(1, 1, 1, 0.2f));
            atlasMat.SetColor("_ColorB", new Color(1, 1, 1, 0.2f));
            atlasMat.SetFloat("_Cutoff", 0.2f);
            atlasMat.SetFloat("_Smootness", 0.45f);

            pathName = "/Sources/" + current.atlasName + ".mat";

            AssetDatabase.CreateAsset(atlasMat, localDirName + pathName);
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            AssetDatabase.SaveAssets();

            GameObject.DestroyImmediate(grass);
            return atlasMat;
        }

        void CreatePrefab(Material atlasMat, GrassAtlasCell _cell)
        {
            grass = GameObject.Instantiate(_cell.sourceGrass) as GameObject;
            string localName = _cell.sourceGrass.name;
            grass.GetComponent<Renderer>().sharedMaterial = atlasMat;
            float power;
            grass.GetComponent<MeshFilter>().sharedMesh = CalculateMeshUV(grass, localName, _cell, out power);
            //if (power < 0.1 * ad)
            //    current.ApplyVertex(grass.GetComponent<MeshFilter>().sharedMesh);

            pathName = "/" + localName + ".prefab";

            Object newSourcePrefab = PrefabUtility.CreateEmptyPrefab(localDirName + pathName);

            PrefabUtility.ReplacePrefab(grass, newSourcePrefab, ReplacePrefabOptions.ConnectToPrefab);
            EditorUtility.SetDirty(newSourcePrefab);
            AssetDatabase.SetLabels(newSourcePrefab, new string[1] { "Grass" });
            AssetDatabase.Refresh(ImportAssetOptions.Default);
            AssetDatabase.SaveAssets();

            GameObject.DestroyImmediate(grass);
        }
        public void SaveAtlasMode()
        {
            //EditorUtility.ClearProgressBar();
            //EditorUtility.DisplayProgressBar("Create Atlas", "Create Atlases", 0);
            current = (VegetationStudioManager)target;
            dirName = current.directory + "/" + current.atlasName;
            int index = dirName.IndexOf("Assets");
            localDirName = dirName.Substring(index, dirName.Length - index);
            pathName = "";

            if (!Directory.Exists(dirName))
                Directory.CreateDirectory(dirName);

            if (!Directory.Exists(dirName + "/Sources"))
                Directory.CreateDirectory(dirName + "/Sources");

            int atlasSize = current.size * (int)current.atlasSize;
            Texture2D atlasAlbedo = new Texture2D(atlasSize, atlasSize, TextureFormat.ARGB32, true, true);
            Texture2D atlasEmission = new Texture2D(atlasSize, atlasSize, TextureFormat.ARGB32, true, true);

            Color[] colors = new Color[atlasSize * atlasSize];
            for (int i = 0; i < colors.Length; i++)
                colors[i] = new Color(0, 0, 0, 0);

            atlasAlbedo.SetPixels(colors);
            atlasEmission.SetPixels(colors);

            for (int i = 0; i < current.cells.Count; i++)
                AddPrototypeTextureToAtlas(ref atlasAlbedo, ref atlasEmission, current.cells[i], atlasSize);

            atlasAlbedo = SaveAtlasTexture(atlasAlbedo, "_DM");

            AwesomeTechnologies.Utility.AssetUtility.SetTextureReadable(atlasAlbedo, false);

            AssetDatabase.Refresh(ImportAssetOptions.Default);

            atlasEmission = SaveAtlasTexture(atlasEmission, "_EM");
            AwesomeTechnologies.Utility.AssetUtility.SetTextureReadable(atlasEmission, false);

            Material atlasMat = CreateMaterial(atlasAlbedo, atlasEmission);

            for (int i = 0; i < current.cells.Count; i++)
                CreatePrefab(atlasMat, current.cells[i]);

            CreateAtlasPreset(current.atlasName);
            EditorUtility.UnloadUnusedAssetsImmediate();


        }

        void OnInspectorUpdate()
        {
            Repaint();
        }

        public void CreateAtlasPreset(string _name)
        {
            current = (VegetationStudioManager)target;

            string[] paths = Directory.GetDirectories(Application.dataPath, "PresetsVegetation", SearchOption.AllDirectories);
            if (paths.Length == 0 || paths.Length > 1)
            {
                Debug.LogError("GrassMergerEditor::CreateGrassPreset: Unable to find the Grass Manager folder! Has it been renamed?");
                return;
            }
            int assetind = paths[0].IndexOf("Assets", 0);
            string rootpath = paths[0].Substring(assetind);
            string contextpath = rootpath + Path.DirectorySeparatorChar + "Contexts";

            if (!AssetDatabase.IsValidFolder(contextpath))
            {
                AssetDatabase.CreateFolder(rootpath, "Contexts");
            }

            VegetationPackage asset = ScriptableObject.CreateInstance<VegetationPackage>();
            asset.TerrainTextureCount = 8;
            asset.InitPackage();
            asset.PackageName = current.atlasName;
            current.context = asset;
            if (current.context != null)
                asset = current.context;

            AssetDatabase.CreateAsset(asset, contextpath + Path.DirectorySeparatorChar + _name + ".asset");
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();


            //VegetationPackage vegetationPackage = ScriptableObjectUtility2.CreateAndReturnAsset<VegetationPackage>();
            //vegetationPackage.TerrainTextureCount = 8;
            //vegetationPackage.InitPackage();
            //current.context = asset;

            //vegetationPackage.PackageName = current.atlasName;

            if (current.cells != null)
                for (int i = 0; i < current.cells.Count; i++)
                {
                    string localName = current.cells[i].sourceGrass.name;

                    if (localDirName == "")
                        localDirName = "Assets";
                    pathName = "/" + localName + ".prefab";

                    Object source = AssetDatabase.LoadAssetAtPath<Object>(localDirName + pathName);
                    current.context.AddVegetationItem((GameObject)source, VegetationType.Grass, true);

                    VegetationItemInfo newItem = current.context.VegetationInfoList[current.context.VegetationInfoList.Count - 1];
                    newItem.Density = 0.65f;
                    newItem.SampleDistance = 0.3f;
                    newItem.VegetationRenderType = VegetationRenderType.InstancedIndirect;
                    newItem.DisableShadows = true;
                    newItem.Seed = UnityEngine.Random.Range(0, 100);
                    newItem.RandomizePosition = false;
                    newItem.VegetationScaleType = VegetationScaleType.Simple;
                    newItem.MinScale = current.context.VegetationInfoList[i].MinScale;
                    newItem.MaxScale = current.context.VegetationInfoList[i].MaxScale;
                    newItem.Rotation = VegetationRotationType.FollowTerrainScale;
                    newItem.RotationOffset = new Vector3(0f, 45f, 0f);
                    newItem.UseHeightLevel = false;
                    newItem.UseAngle = false;
                    newItem.UsePerlinMask = false;
                    if (i < 8)
                        newItem.SwitchToDetail(i);
                    else
                        newItem.SwitchToDisabled();

                    current.context.VegetationInfoList[i].CopySettingValues(newItem);
                }

            //current.UpdateIcons();



        }
    }

    public delegate void GrassPresetEventHandler(string name);

    public class GrassPresetNamePopup : PopupWindowContent
    {
        public string m_Name = "";
        public event GrassPresetEventHandler OnCreate;

        public override Vector2 GetWindowSize()
        {
            return new Vector2(200, 75);
        }

        public override void OnGUI(Rect rect)
        {
            GUILayout.Label("Preset Name:", EditorStyles.boldLabel);
            m_Name = EditorGUILayout.TextField(m_Name);

            if (GUILayout.Button("Create"))
            {
                if (OnCreate != null && m_Name != "")
                {
                    OnCreate(m_Name);
                    editorWindow.Close();
                }
            }
        }
    }
}

