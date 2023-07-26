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

namespace AwesomeTechnologies
{

    [CustomEditor(typeof(VegetationSystem))]
    public partial class VegetationSystemEditor : VegetationStudioBaseEditor
    {
        
        private int _vegIndex;
        private int _selectedGridIndex;
        private int _lastVegIndex;
        private bool _advancedTextures;
        private VegetationSystem _vegetationSystem;

        private int _excludeTextureMaskIndex;
        private int _includeTextureMaskIndex;

        private int _excludeTextureMaskRuleIndex;
        private int _densityTextureMaskRuleIndex;
        private int _scaleTextureMaskRuleIndex;
        private int _includeTextureMaskRuleIndex;

        private int _newTextureMaskTypeIndex;

        private InspectorCurveEditor _heightCurveEditor;
        private InspectorCurveEditor _steepnessCurveEditor;

        private static readonly string[] TabNames =
        {
            "Settings","Vegetation", "Editor", "Render","Terrain Textures","Masks","Real-time mask","Advanced","Debug"
        };

        private static readonly string[] Lod =
        {
            "LOD2","LOD1", "LOD0"
        };

        private static readonly string[] RgbaChannelStrings =
        {
            "R Channel","G Channel", "B Channel", "A Channel"
        };

        public void OnEnable()
        {

            ResetIco();

            var settings = InspectorCurveEditor.Settings.DefaultSettings;
            _heightCurveEditor = new InspectorCurveEditor(settings);
            _steepnessCurveEditor = new InspectorCurveEditor(settings) { CurveType = InspectorCurveEditor.InspectorCurveType.Steepness };

            _vegetationSystem = (VegetationSystem)target;

            ConfirmBillboardColorSpace();
        }

        void ConfirmBillboardColorSpace()
        {
            if (_vegetationSystem.currentVegetationPackage)
            {
                bool needRegenerate = false;
                for (int i = 0; i <= _vegetationSystem.currentVegetationPackage.VegetationInfoList.Count - 1; i++)
                {
                    VegetationItemInfo vegetationItemInfo = _vegetationSystem.currentVegetationPackage.VegetationInfoList[i];
                    if (vegetationItemInfo.VegetationType != VegetationType.Tree) continue;
                    if (vegetationItemInfo.UseBillboards == false) continue;
                    if (vegetationItemInfo.BillboardColorSpace == PlayerSettings.colorSpace) continue;
                    if (vegetationItemInfo.VegetationPrefab == null) continue;

                    needRegenerate = true;
                    break;
                }

                if (needRegenerate)
                {
                    if (EditorUtility.DisplayDialog("Vegetation Studio - Tree billboards",
                        "Tree billboards are not generated for the correct colorspace.", "Regenerate"))
                    {
                        for (int i = 0; i <= _vegetationSystem.currentVegetationPackage.VegetationInfoList.Count - 1; i++)
                        {
                            VegetationItemInfo vegetationItemInfo = _vegetationSystem.currentVegetationPackage.VegetationInfoList[i];
                            EditorUtility.DisplayProgressBar("Regenerate billboard: " + vegetationItemInfo.Name, "Generate image atlas", (float)i / (_vegetationSystem.currentVegetationPackage.VegetationInfoList.Count - 1));

                            if (vegetationItemInfo.VegetationType != VegetationType.Tree) continue;
                            if (vegetationItemInfo.UseBillboards == false) continue;

                            if (vegetationItemInfo.VegetationPrefab)
                            {
                                _vegetationSystem.currentVegetationPackage.GenerateBillboard(vegetationItemInfo.VegetationItemID);
                            }
                        }

                        EditorUtility.ClearProgressBar();
                    }

                    EditorUtility.SetDirty(_vegetationSystem.currentVegetationPackage);
                }
            }
        }

        public override void OnInspectorGUI()
        {
            LargeLogo = true;
            HelpTopic = "home/vegetation-studio/components/vegetation-system";

            _vegetationSystem = (VegetationSystem)target;

            base.OnInspectorGUI();


            if (!_vegetationSystem.enabled) return;

            if (_vegetationSystem.GetSleepMode())
            {
                GUILayout.BeginVertical("box");
                var oldColor = GUI.backgroundColor;
                Color backgroundColor = new Color(110f / 255f, 235f / 255f, 110f / 255f);
                GUI.backgroundColor = backgroundColor;
                if (GUILayout.Button("Start Vegetation System", GUILayout.Height(35)))
                {
                    _vegetationSystem.SetSleepMode(false);
                }
                GUI.backgroundColor = oldColor;
                EditorGUILayout.HelpBox("Vegetation Studio starts in sleep mode when scene loads in editor. In playmode and builds it starts as normal.", MessageType.Info);

                GUILayout.EndVertical();

                return;
            }

            
            if (VegetationStudioManager.isAdvancedMode)
            {
                _vegetationSystem.CurrentTabIndex = GUILayout.SelectionGrid(_vegetationSystem.CurrentTabIndex, TabNames, 3, EditorStyles.toolbarButton);
                switch (_vegetationSystem.CurrentTabIndex)
                {
                    case 0:
                        HelpTopic = "home/vegetation-studio/components/vegetation-system/vegetation-system-settings/";
                        DrawSettingsInspector();
                        break;
                    case 1:
                        DrawVegetationInspector();
                        break;
                    case 2:
                        DrawEditorInspector();
                        break;
                    case 3:
                        DrawRenderInspector();
                        break;
                    case 4:
                        DrawTexturesInspector();
                        break;
                    case 5:
                        DrawMaskInspector();
                        break;
                    case 6:
                        DrawRealTimeMaskInspector();
                        break;
                    case 7:
                        DrawAdvancedInspector();
                        break;
                    case 8:
                        DrawDebugInspector();
                        break;
                }
            }
            else
                DrawSimpleMode();

 
            if (_vegetationSystem.LastTabIndex == 1 && _vegetationSystem.CurrentTabIndex != 1) _vegetationSystem.RestoreTerrainMaterial();
            if (_vegetationSystem.LastTabIndex != _vegetationSystem.CurrentTabIndex) GUI.FocusControl(null);
            _vegetationSystem.LastTabIndex = _vegetationSystem.CurrentTabIndex;

            if (GUI.changed)
            {
                EditorUtility.SetDirty(_vegetationSystem);
                if (!Application.isPlaying) EditorSceneManager.MarkSceneDirty(_vegetationSystem.gameObject.scene);
            }
        }

        void DrawAdvancedInspector()
        {
            EditorGUI.BeginChangeCheck();

            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Advanced settings", LabelStyle);
            _vegetationSystem.ManualVegetationRefresh = EditorGUILayout.Toggle("Manual vegetation refresh", _vegetationSystem.ManualVegetationRefresh);
            EditorGUILayout.HelpBox("With manual refresh enabled changes on vegetation items rules will only refresh when restarting or entering playmode. A manual refresh can be triggered with the refresh vegetation button on the Vegetation tab", MessageType.Info);

            _vegetationSystem.AutomaticWakeup = EditorGUILayout.Toggle("Automatic awake", _vegetationSystem.AutomaticWakeup);
            EditorGUILayout.HelpBox("When enabled the vegetation system will awake automatic at scene load or when exiting playmode", MessageType.Info);
            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Instanced GI", LabelStyle);
            EditorGUILayout.HelpBox("Instanced GI rendering requires Unity 2018.1+ and the Vegetation Items set to Instanced rendering. This will sample lightprobes and apply to the instances.", MessageType.Info);
            _vegetationSystem.vegetationSettings.InstancedGIGrass = EditorGUILayout.Toggle("Enable on Grass", _vegetationSystem.vegetationSettings.InstancedGIGrass);
            _vegetationSystem.vegetationSettings.InstancedGIPlant = EditorGUILayout.Toggle("Enable on Plants", _vegetationSystem.vegetationSettings.InstancedGIPlant);
            _vegetationSystem.vegetationSettings.InstancedGITree = EditorGUILayout.Toggle("Enable on Trees", _vegetationSystem.vegetationSettings.InstancedGITree);
            _vegetationSystem.vegetationSettings.InstancedGIObject = EditorGUILayout.Toggle("Enable on Objects", _vegetationSystem.vegetationSettings.InstancedGIObject);
            _vegetationSystem.vegetationSettings.InstancedGILargeObject = EditorGUILayout.Toggle("Enable on Large Objects", _vegetationSystem.vegetationSettings.InstancedGILargeObject);

            EditorGUILayout.LabelField("Occlusion probes", LabelStyle);
            _vegetationSystem.vegetationSettings.InstancedOcclusionProbes = EditorGUILayout.Toggle("Enable occlusion probes", _vegetationSystem.vegetationSettings.InstancedOcclusionProbes);
            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Speedtree settings", LabelStyle);
            _vegetationSystem.vegetationSettings.LODFadeDistance = EditorGUILayout.Slider("LOD fade distance", _vegetationSystem.vegetationSettings.LODFadeDistance, 1, 100);
            EditorGUILayout.HelpBox("This setting controls the distance used for the vertex fade between LODs in Speedtree", MessageType.Info);
            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("vegetation item settings", LabelStyle);
            _vegetationSystem.EnableVegetationItemIDEdit = EditorGUILayout.Toggle("Enable ID editing", _vegetationSystem.EnableVegetationItemIDEdit);
            EditorGUILayout.HelpBox("Enabling this will give you a text box to copy and edit the IDs of VegetationItems. 2 Items on the same vegetation package can not have the same ID.", MessageType.Info);
            GUILayout.EndVertical();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(_vegetationSystem);
            }
        }

        void DrawRealTimeMaskInspector()
        {
            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Real-Time masks", LabelStyle);
            EditorGUILayout.HelpBox("Real-Time masks are used to mask out vegetation in the render loop. This only works with the instanced indirect vegetation on the compute shader render loop. Mask can be changed every frame.", MessageType.Info);
            _vegetationSystem.RealTimeMaskTexture = (Texture)EditorGUILayout.ObjectField("", _vegetationSystem.RealTimeMaskTexture, typeof(Texture), true);
            _vegetationSystem.RealTimeMaskBand = (TextureMaskBand)EditorGUILayout.EnumPopup("Select band", _vegetationSystem.RealTimeMaskBand);
            _vegetationSystem.RealTimeMaskInvert = EditorGUILayout.Toggle("Invert channel", _vegetationSystem.RealTimeMaskInvert);
            _vegetationSystem.RealTimeMaskCutoff = EditorGUILayout.Slider("Cutoff", _vegetationSystem.RealTimeMaskCutoff, 0f, 1f);
            _vegetationSystem.RealTimeMaskEnabled = EditorGUILayout.Toggle("Enable mask", _vegetationSystem.RealTimeMaskEnabled);
            GUILayout.EndVertical();
        }

        void AddTextureMask(Type textureMaskType)
        {
            TextureMaskBase textureMask = Activator.CreateInstance(textureMaskType) as TextureMaskBase;
            _vegetationSystem.currentVegetationPackage.TextureMaskList.Add(textureMask);
        }

        void DrawMaskInspector()
        {
            GUILayout.BeginVertical("box");
            if (_vegetationSystem.currentVegetationPackage == null)
            {
                DrawNoVegetationPackageError();
                return;
            }

            if (GUILayout.Button("Refresh masks"))
            {
                _vegetationSystem.RefreshTextureMasks();
                _vegetationSystem.ClearVegetationCellCache();
            }

            EditorGUILayout.HelpBox("Masks can be used with any vegetation item to include or exclude vegetation. In addition you can make density samples in masks.", MessageType.Info);

            _newTextureMaskTypeIndex = EditorGUILayout.Popup("Add mask of type", _newTextureMaskTypeIndex, TextureMaskTypeManager.GetTextureMaskTypeStringArray());

            if (GUILayout.Button("Add texture mask"))
            {
                AddTextureMask(TextureMaskTypeManager.GetTextureMaskTypeFromIndex(_newTextureMaskTypeIndex));
            }

            EditorGUI.BeginChangeCheck();

            for (int i = 0; i <= _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].TextureMaskList.Count - 1; i++)
            {
                TextureMaskBase mask = _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                    .TextureMaskList[i];

                GUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("Texture mask " + (i + 1) + ": " + mask.TextureMaskTypeReadableName, LabelStyle);
                mask.TextureMaskTextureStorage = (TextureMaskTextureStorage)EditorGUILayout.EnumPopup("Texture location", mask.TextureMaskTextureStorage);
                mask.MaskName = EditorGUILayout.TextField("Mask name", mask.MaskName);

                if (mask.UseTiling)
                {
                    mask.Scale = EditorGUILayout.Slider("Terrain scale 1:", mask.Scale, 1, 200);
                    EditorGUILayout.LabelField("Scale - 1:" + mask.Scale.ToString("F0") + " of terrain", LabelStyle);
                }

                if (mask.ShowInverseResult)
                {
                    mask.InverseResult = EditorGUILayout.Toggle("Inverse mask result", mask.InverseResult);
                }

                if (mask.ShowRotateTexture)
                {
                    mask.TextureMaskRotation = (TextureMaskRotation)EditorGUILayout.EnumPopup("Mask rotation", mask.TextureMaskRotation);
                }

                if (mask.UseTexture)
                {
                    EditorGUI.BeginChangeCheck();

                    Texture2D currentMaskTexture;

                    if (mask.TextureMaskTextureStorage == TextureMaskTextureStorage.VegetationPackage)
                    {
                        mask.MaskTexture = (Texture2D)EditorGUILayout.ObjectField(mask.MaskTexture, typeof(Texture2D), false, GUILayout.Height(64), GUILayout.Width(64));
                        currentMaskTexture = mask.MaskTexture;
                    }
                    else
                    {
                        Texture2D localMaskTexture = _vegetationSystem.GetLocalTextureMaskTexture(mask.MaskID);
                        currentMaskTexture = (Texture2D)EditorGUILayout.ObjectField(localMaskTexture, typeof(Texture2D), false, GUILayout.Height(64), GUILayout.Width(64));
                    }

                    if (EditorGUI.EndChangeCheck())
                    {
                        if (mask.RequiredTextureFormatList.Count > 0 && !mask.RequiredTextureFormatList.Contains(mask.MaskTexture.format))
                        {
                            currentMaskTexture = null;
                            EditorUtility.DisplayDialog("Texture format error", "Texture format does not match required format", "OK");
                        }
                        if (mask.MaskTexture && AssetUtility.HasCrunchCompression(currentMaskTexture))
                        {
                            if (EditorUtility.DisplayDialog("Texture format error",
                                "Texture can not use crunch compression", "Fix", "Cancel"))
                            {
                                AssetUtility.RemoveCrunchCompression(currentMaskTexture);
                            }
                            else
                            {
                                currentMaskTexture = null;
                            }
                        }

                        if (mask.MaskTexture && !AssetUtility.HasRgbaFormat(currentMaskTexture))
                        {
                            if (EditorUtility.DisplayDialog("Texture format error",
                                "Texture needs to be in RGBA or ARGB format", "Fix", "Cancel"))
                            {
                                AssetUtility.SetRgbaFormat(currentMaskTexture);
                            }
                            else
                            {
                                currentMaskTexture = null;
                            }
                        }
                        AssetUtility.SetTextureReadable(currentMaskTexture);

                        if (mask.TextureMaskTextureStorage == TextureMaskTextureStorage.VegetationPackage)
                        {
                            mask.MaskTexture = currentMaskTexture;
                        }
                        else
                        {
                            _vegetationSystem.UpdateLocalTextureMaskTexture(mask.MaskID, currentMaskTexture);
                        }
                    }
                }

                if (GUILayout.Button("Remove", GUILayout.Width(100)))
                {
                    _vegetationSystem.currentVegetationPackage.TextureMaskList.Remove(_vegetationSystem
                        .currentVegetationPackage.TextureMaskList[i]);
                    _vegetationSystem.RefreshTextureMasks();
                    _vegetationSystem.DelayedClearVegetationCellCache();
                }

                GUILayout.EndVertical();
            }

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(_vegetationSystem.currentVegetationPackage);
                EditorUtility.SetDirty(target);

                _vegetationSystem.RefreshTextureMasks();
                _vegetationSystem.DelayedClearVegetationCellCache();
            }

            GUILayout.EndVertical();
        }

        void DrawTexturesInspector()
        {
            var textStyle = new GUIStyle
            {
                alignment = TextAnchor.MiddleLeft,
                normal = { textColor = GUI.skin.label.normal.textColor }
            };

            if (_vegetationSystem.currentVegetationPackage.TerrainTextureCount == 0)
            {
                EditorGUILayout.HelpBox("In order to manage terrain textures create a VegetationPackage with 4 or more textures.", MessageType.Info);
                return;
            }

            EditorGUILayout.HelpBox("Any texture can be used for automatic splatmap generation or normal painting. Settings in Terrain System Component.", MessageType.Info);
            _advancedTextures = EditorGUILayout.Toggle("Advanced", _advancedTextures);
            if (_advancedTextures)
            {
                EditorGUILayout.HelpBox("The advanced mode with occlusion and height map textures is for use with 3rd party shaders that need more channels than the normal shader. A shader plug-in is used to recieve an event when switching package and update the shader.", MessageType.Info);

            }

            EditorGUI.BeginChangeCheck();

            _vegetationSystem.currentVegetationPackage.UseTerrainTextures = EditorGUILayout.Toggle("Update terrain textures on init", _vegetationSystem.currentVegetationPackage.UseTerrainTextures);


            for (int i = 0; i <= _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].TerrainTextureList.Count - 1; i++)
            {
                GUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("Terrain texture layer " + (i + 1).ToString(), LabelStyle);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(new GUIContent("Texture", ""), textStyle, GUILayout.Width(64));
                EditorGUILayout.LabelField(new GUIContent("Normal", ""), textStyle, GUILayout.Width(64));
                if (_advancedTextures)
                {
                    EditorGUILayout.LabelField(new GUIContent("Occlusion", ""), textStyle, GUILayout.Width(64));
                    EditorGUILayout.LabelField(new GUIContent("Height map", ""), textStyle, GUILayout.Width(64));
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].TerrainTextureList[i].Texture = (Texture2D)EditorGUILayout.ObjectField(_vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].TerrainTextureList[i].Texture, typeof(Texture2D), false, GUILayout.Height(64), GUILayout.Width(64));
                _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].TerrainTextureList[i].TextureNormals = (Texture2D)EditorGUILayout.ObjectField(_vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].TerrainTextureList[i].TextureNormals, typeof(Texture2D), false, GUILayout.Height(64), GUILayout.Width(64));
                if (_advancedTextures)
                {
                    _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].TerrainTextureList[i].TextureOcclusion = (Texture2D)EditorGUILayout.ObjectField(_vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].TerrainTextureList[i].TextureOcclusion, typeof(Texture2D), false, GUILayout.Height(64), GUILayout.Width(64));
                    _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].TerrainTextureList[i].TextureHeightMap = (Texture2D)EditorGUILayout.ObjectField(_vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].TerrainTextureList[i].TextureHeightMap, typeof(Texture2D), false, GUILayout.Height(64), GUILayout.Width(64));
                }
                EditorGUILayout.EndHorizontal();

                Rect space = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(18));
                _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].TerrainTextureList[i].TileSize = EditorGUI.Vector2Field(space, "Texture tile size", _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].TerrainTextureList[i].TileSize);

                if (_vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].TerrainTextureList[i].TileSize.x < 1 || _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].TerrainTextureList[i].TileSize.y < 1)
                {
                    EditorGUILayout.HelpBox("Texture tile size should be set for a higher value, normal is around 15.", MessageType.Warning);
                }

                GUILayout.EndVertical();
            }

            if (EditorGUI.EndChangeCheck())
            {

                for (int i = 0; i <= _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].TerrainTextureList.Count - 1; i++)
                {
                    AssetUtility.SetTextureReadable(_vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].TerrainTextureList[i].Texture, false);
                    AssetUtility.SetTextureReadable(_vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].TerrainTextureList[i].TextureNormals, true);
                    AssetUtility.SetTextureReadable(_vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].TerrainTextureList[i].TextureOcclusion, false);
                    AssetUtility.SetTextureReadable(_vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].TerrainTextureList[i].TextureHeightMap, false);
                }

                _vegetationSystem.SetupTerrainTextures();
                EditorUtility.SetDirty(_vegetationSystem.currentVegetationPackage);
                EditorUtility.SetDirty(target);
            }

        }

        void DuplicateVegetationItem(VegetationItemInfo vegetationItemInfo)
        {
            VegetationItemInfo newVegetationItemInfo = new VegetationItemInfo(vegetationItemInfo);
            newVegetationItemInfo.Name += " Copy";
            _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList.Add(newVegetationItemInfo);
            _vegIndex = _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList.Count - 1;
            if (newVegetationItemInfo.PrefabType == VegetationPrefabType.Mesh &&
                newVegetationItemInfo.VegetationType == VegetationType.Tree)
            {
                _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                    .GenerateBillboard(newVegetationItemInfo.VegetationItemID);
            }
            _vegetationSystem.RefreshVegetationPackage();
            EditorUtility.SetDirty(_vegetationSystem.currentVegetationPackage);
        }

        void PasteVegetationItemValues(VegetationItemInfo targetVegetationItemInfo, VegetationItemInfo sourceVegetationItemInfo)
        {
            targetVegetationItemInfo.CopySettingValues(sourceVegetationItemInfo);

        }

        void DrawDebugInspector()
        {
            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Cache info", LabelStyle);
            EditorGUILayout.LabelField("Cache tree count: " + _vegetationSystem.GetCacheTreeCount().ToString());
            EditorGUILayout.LabelField("Cache grass/plant count: " + _vegetationSystem.GetCacheGrassCount().ToString());

            float cacheSize = _vegetationSystem.GetCacheTreeCount() + _vegetationSystem.GetCacheGrassCount();
            cacheSize = (cacheSize / (1024f * 1024f) * 64f);
            EditorGUILayout.LabelField("Cache size: " + cacheSize.ToString("F2") + " MByte");

            if (GUILayout.Button("Clear cache"))
            {
                _vegetationSystem.ClearVegetationCellCache();
                EditorUtility.SetDirty(target);
            }
            GUILayout.EndVertical();


            GUILayout.BeginVertical("box");
            if (GUILayout.Button("Refresh heightmap"))
            {
                VegetationStudioManager.RefreshTerrainHeightMap();
                EditorUtility.SetDirty(target);
            }

            GUILayout.EndVertical();
        }

        void DrawShadowSettings()
        {
            VegetationItemInfo vegetationItemInfo = _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex];
            string prev = (vegetationItemInfo.DisableShadows) ? "Disabled" : "Enabled";
            _vegetationSystem.ShowShadowsMenu = VegetationPackageEditorTools.DrawHeader("Shadows: " + prev, _vegetationSystem.ShowShadowsMenu);
            if (_vegetationSystem.ShowShadowsMenu == false) return;

            GUILayout.BeginVertical("box");
            
            EditorGUI.BeginChangeCheck();

            vegetationItemInfo.DisableShadows = EditorGUILayout.Toggle("Disable shadows", vegetationItemInfo.DisableShadows);
            GUILayout.EndVertical();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(_vegetationSystem.currentVegetationPackage);
            }
        }

        

        void DrawNavMeshObstacleSettings()
        {
            VegetationItemInfo vegetationItemInfo = _vegetationSystem
                .VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                .VegetationInfoList[_vegIndex];
            if (vegetationItemInfo.VegetationType != VegetationType.Tree &&
                vegetationItemInfo.VegetationType != VegetationType.Objects &&
                vegetationItemInfo.VegetationType != VegetationType.LargeObjects) return;

            _vegetationSystem.ShowNavMeshObstacleMenu = VegetationPackageEditorTools.DrawHeader("NavMesh Obstacle", _vegetationSystem.ShowNavMeshObstacleMenu);
            if (_vegetationSystem.ShowNavMeshObstacleMenu == false) return;

            GUILayout.BeginVertical("box");
            EditorGUI.BeginChangeCheck();

            vegetationItemInfo.NavMeshObstacleType = (NavMeshObstacleType)EditorGUILayout.EnumPopup("NavMesh Obstacle Type",
                vegetationItemInfo.NavMeshObstacleType);

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

            if (EditorGUI.EndChangeCheck())
            {
                _vegetationSystem.RefreshColliders();
                EditorUtility.SetDirty(_vegetationSystem.currentVegetationPackage);
            }
        }

        void DrawColliderSettings()
        {

            VegetationItemInfo vegetationItemInfo = _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex];
            if (vegetationItemInfo.VegetationType != VegetationType.Tree &&
                vegetationItemInfo.VegetationType != VegetationType.Objects &&
                vegetationItemInfo.VegetationType != VegetationType.LargeObjects) return;

            _vegetationSystem.ShowColliderMenu = VegetationPackageEditorTools.DrawHeader("Colliders", _vegetationSystem.ShowColliderMenu);
            if (_vegetationSystem.ShowColliderMenu == false) return;

            GUILayout.BeginVertical("box");
            EditorGUI.BeginChangeCheck();

            vegetationItemInfo.ColliderType = (ColliderType)EditorGUILayout.EnumPopup("Collider type",
                vegetationItemInfo.ColliderType);
            switch (vegetationItemInfo.ColliderType)
            {
                case ColliderType.Capsule:
                    {
                        vegetationItemInfo.ColliderRadius = EditorGUILayout.FloatField("Radius",
                            vegetationItemInfo.ColliderRadius);
                        vegetationItemInfo.ColliderHeight = EditorGUILayout.FloatField("Height",
                            vegetationItemInfo.ColliderHeight);
                        vegetationItemInfo.ColliderOffset = EditorGUILayout.Vector3Field("Offset",
                            vegetationItemInfo.ColliderOffset);
                        break;
                    }
                case ColliderType.Sphere:
                    {
                        vegetationItemInfo.ColliderRadius = EditorGUILayout.FloatField("Radius",
                            vegetationItemInfo.ColliderRadius);
                        vegetationItemInfo.ColliderOffset = EditorGUILayout.Vector3Field("Offset",
                            vegetationItemInfo.ColliderOffset);

                        break;
                    }
                case ColliderType.CustomMesh:
                    {
                        vegetationItemInfo.ColliderMesh = (Mesh)EditorGUILayout.ObjectField("Custom mesh",
                            vegetationItemInfo.ColliderMesh, typeof(Mesh), false);
                        break;
                    }
            }
            if (_vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].ColliderType != ColliderType.Disabled)
            {
                vegetationItemInfo.ColliderTrigger = EditorGUILayout.Toggle("Trigger", vegetationItemInfo.ColliderTrigger);
                vegetationItemInfo.ColliderUseForBake = EditorGUILayout.Toggle("Include in NavMesh bake", vegetationItemInfo.ColliderUseForBake);
            }

            GUILayout.EndVertical();
            if (EditorGUI.EndChangeCheck())
            {
                _vegetationSystem.RefreshColliders();
                EditorUtility.SetDirty(_vegetationSystem.currentVegetationPackage);
            }
        }

        /*
        private void OnSceneGUI()
        {
            if (_vegetationSystem.gameObject.GetInstanceID() != VegetationStudioManager.Instance.selectedVegetationSystem)
            {
                VegetationStudioManager.Instance.selectedVegetationSystem = _vegetationSystem.gameObject.GetInstanceID();
                _vegetationSystem.SetupVegetationSystem();
            }
        }
        */
        void DrawLODSettings()
        {

            VegetationItemInfo vegetationItemInfo = _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex];

            if ((vegetationItemInfo.VegetationType == VegetationType.Grass || vegetationItemInfo.VegetationType == VegetationType.Plant) && vegetationItemInfo.VegetationRenderType != VegetationRenderType.InstancedIndirect) return;

            VegetationItemModelInfo vegetationItemModelInfo = _vegetationSystem.GetVegetationModelInfo(_vegIndex);
            if (vegetationItemModelInfo.LOD1Distance < 1) return;

            _vegetationSystem.ShowLODMenu = VegetationPackageEditorTools.DrawHeader("LODs", _vegetationSystem.ShowLODMenu);
            if (_vegetationSystem.ShowLODMenu == false) return;
            GUILayout.BeginVertical("box");
            EditorGUI.BeginChangeCheck();

            if (vegetationItemInfo.VegetationRenderType == VegetationRenderType.InstancedIndirect &&
                !_vegetationSystem.UseIndirectLoDs)
            {
                EditorGUILayout.HelpBox("Instanced indirect LODs are disabled on the render tab.", MessageType.Warning);
            }

            vegetationItemInfo.DisableLOD = EditorGUILayout.Toggle("Disable LODs", vegetationItemInfo.DisableLOD);
            vegetationItemInfo.LODFactor = EditorGUILayout.Slider("LOD distance factor", vegetationItemInfo.LODFactor, 0.15f, 20f);

            if (vegetationItemModelInfo != null)
            {
                float currentLOD1Distance = vegetationItemModelInfo.LOD1Distance * QualitySettings.lodBias *
                                            vegetationItemInfo.LODFactor;
                float currentLOD2Distance = vegetationItemModelInfo.LOD2Distance * QualitySettings.lodBias *
                                            vegetationItemInfo.LODFactor;

                VegetationPackageEditorTools.DrawLODRanges(currentLOD1Distance, currentLOD2Distance, _vegetationSystem.GetVegetationDistance() + _vegetationSystem.GetTreeDistance());
            }

            GUILayout.EndVertical();
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(_vegetationSystem.currentVegetationPackage);
            }
        }

        void DrawVegetationItemSpeedtreeSettings()
        {
            VegetationItemInfo vegetationItemInfo = _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex];
            if (vegetationItemInfo.ShaderType != VegetationShaderType.Speedtree) return;

            _vegetationSystem.ShowSpeedtreeMenu = VegetationPackageEditorTools.DrawHeader("Speedtree", _vegetationSystem.ShowSpeedtreeMenu);
            if (_vegetationSystem.ShowSpeedtreeMenu == false) return;

            GUILayout.BeginVertical("box");
            EditorGUI.BeginChangeCheck();

            vegetationItemInfo.Hue = EditorGUILayout.ColorField("HUE", vegetationItemInfo.Hue);
            vegetationItemInfo.ColorTint1 = EditorGUILayout.ColorField("Color tint", vegetationItemInfo.ColorTint1);

            GUILayout.EndVertical();
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(_vegetationSystem.currentVegetationPackage);
                _vegetationSystem.RefreshVegetationModelInfoMaterials();
                VegetationStudioManager.VegetationPackageSync_ClearVegetationSystemCellCache(_vegetationSystem);
            }
        }

        void DrawTouchReactSettings()
        {
            VegetationItemInfo vegetationItemInfo = _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex];
            EditorGUI.BeginChangeCheck();
            if (vegetationItemInfo.VegetationType == VegetationType.Objects || vegetationItemInfo.VegetationType == VegetationType.LargeObjects)
            {
                _vegetationSystem.ShowTouchReactMenu = VegetationPackageEditorTools.DrawHeader("Touch React", _vegetationSystem.ShowTouchReactMenu);
                if (_vegetationSystem.ShowTouchReactMenu == false) return;
                GUILayout.BeginVertical("box");
                _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].UseTouchReact = EditorGUILayout.Toggle("Enable touch react", _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].UseTouchReact);
                EditorGUILayout.HelpBox("Enable Touch React will bend grass under the mesh shape of the object.", MessageType.Info);
                GUILayout.EndVertical();
            }
            if (EditorGUI.EndChangeCheck())
            {
                _selectedGridIndex = GetSelectedGridIndex(_vegIndex);
                _vegetationSystem.RefreshVegetationPackage();
                EditorUtility.SetDirty(_vegetationSystem.currentVegetationPackage);
            }
        }

        void DrawBillboardSettings()
        {
            VegetationItemInfo vegetationItemInfo = _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex];
            bool billboardChanged = false;

            if (vegetationItemInfo.VegetationType == VegetationType.Tree)
            {
                _vegetationSystem.ShowBillboardsMenu = VegetationPackageEditorTools.DrawHeader("Billboards", _vegetationSystem.ShowBillboardsMenu);
                if (!_vegetationSystem.ShowBillboardsMenu) return;

                GUILayout.BeginVertical("box");
                EditorGUI.BeginChangeCheck();
                _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                    .VegetationInfoList[_vegIndex].UseBillboards = EditorGUILayout.Toggle("Enable billboards",
                    _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                        .VegetationInfoList[_vegIndex].UseBillboards);
                if (EditorGUI.EndChangeCheck())
                {
                    billboardChanged = true;
                    _vegetationSystem.RefreshBillboards();
                    EditorUtility.SetDirty(_vegetationSystem.currentVegetationPackage);
                }

                if (!_vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                    .VegetationInfoList[_vegIndex].UseBillboards)
                {
                    GUILayout.EndVertical();
                    return;
                }

                if (_vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                    .VegetationInfoList[_vegIndex]
                    .UseBillboards)
                {
                    GUILayout.BeginVertical("box");
                    EditorGUILayout.LabelField("Runtime settings", LabelStyle);
                    vegetationItemInfo.BillboardCutoff = EditorGUILayout.Slider("Alpha cutoff",
                        vegetationItemInfo.BillboardCutoff, 0f, 1f);
                    vegetationItemInfo.BillboardBrightness = EditorGUILayout.Slider("Brightness",
                        vegetationItemInfo.BillboardBrightness, 0.5f, 5f);
                    vegetationItemInfo.BillboardMipmapBias = EditorGUILayout.Slider("Mipmap bias",
                        vegetationItemInfo.BillboardMipmapBias, -3f, 0f);

                    vegetationItemInfo.BillboardTintColor =
                        EditorGUILayout.ColorField("Tint color", vegetationItemInfo.BillboardTintColor);
                    GUILayout.EndVertical();
                    EditorGUI.BeginChangeCheck();

                    GUILayout.BeginVertical("box");
                    EditorGUILayout.LabelField("Generation settings", LabelStyle);
                    BillboardQuality oldBillboardQuality = _vegetationSystem
                        .VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                        .VegetationInfoList[_vegIndex].BillboardQuality;
                    _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                            .VegetationInfoList[_vegIndex].BillboardQuality =
                        (BillboardQuality)EditorGUILayout.EnumPopup("Billboard quality",
                            _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                                .VegetationInfoList[_vegIndex].BillboardQuality);

                    if ((int)_vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                            .VegetationInfoList[_vegIndex].BillboardQuality > 2)
                    {
                        EditorGUILayout.HelpBox("3D billboards are experimental. Designed for high flying cameras.",
                            MessageType.Warning);
                    }

                    // vegetationItemInfo.BillboardAtlasBackgroundColor = EditorGUILayout.ColorField("Background color", vegetationItemInfo.BillboardAtlasBackgroundColor);

                    if (_vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                            .VegetationInfoList[_vegIndex]
                            .ShaderType == VegetationShaderType.Speedtree)
                    {
                        EditorGUI.BeginChangeCheck();
                        _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                            .VegetationInfoList[_vegIndex]
                            .BillboardLodIndex = EditorGUILayout.Popup("Source LOD",
                            _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                                .VegetationInfoList[_vegIndex]
                                .BillboardLodIndex, Lod);
                        EditorGUILayout.HelpBox("Selects what LOD on the tree to use as source for the billboard",
                            MessageType.Info);
                        if (EditorGUI.EndChangeCheck())
                        {
                            _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                                .GenerateBillboard(_vegIndex);
                            EditorUtility.SetDirty(_vegetationSystem.currentVegetationPackage);
                        }
                    }

                    if (oldBillboardQuality != _vegetationSystem
                            .VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                            .VegetationInfoList[_vegIndex].BillboardQuality)
                    {
                        _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                            .GenerateBillboard(_vegIndex);
                        EditorUtility.SetDirty(_vegetationSystem.currentVegetationPackage);
                    }

                    _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                        .VegetationInfoList[_vegIndex].BillboardTexture = EditorGUILayout.ObjectField(
                        "Billboard texture",
                        _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                            .VegetationInfoList[_vegIndex].BillboardTexture, typeof(Texture2D), true) as Texture2D;
                    _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                        .VegetationInfoList[_vegIndex].BillboardNormalTexture = EditorGUILayout.ObjectField(
                        "Billboard normals",
                        _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                            .VegetationInfoList[_vegIndex].BillboardNormalTexture, typeof(Texture2D),
                        true) as Texture2D;


                    if (GUILayout.Button("Regenerate billboard"))
                    {
                        _vegetationSystem.currentVegetationPackage.GenerateBillboard(_vegetationSystem
                            .currentVegetationPackage.VegetationInfoList[_vegIndex]
                            .VegetationItemID);
                        EditorUtility.SetDirty(_vegetationSystem.currentVegetationPackage);
                    }

                    GUILayout.EndVertical();
                }

                GUILayout.EndVertical();

                if (EditorGUI.EndChangeCheck())
                {
                    billboardChanged = true;
                }
            }

            if (billboardChanged)
            {

                _vegetationSystem.RefreshBillboards();
                EditorUtility.SetDirty(_vegetationSystem.currentVegetationPackage);

            }
        }

        void DrawVegetationItemGrassSettings()
        {
            /*
            VegetationItemInfo vegetationItemInfo = _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex];

            if (vegetationItemInfo.ShaderType != VegetationShaderType.VegetationStudioGrass && vegetationItemInfo.ShaderType != VegetationShaderType.AdvancedTerrainGrass)
            {
                return;
            }

            
            _vegetationSystem.ShowVegetationItemGrassMenu = VegetationPackageEditorTools.DrawHeader("Grass Settings", _vegetationSystem.ShowVegetationItemGrassMenu);

            if (_vegetationSystem.ShowVegetationItemGrassMenu == false) return;
            GUILayout.BeginVertical("box");
            EditorGUI.BeginChangeCheck();

            vegetationItemInfo.ColorTint1 = EditorGUILayout.ColorField("Tint color 1", vegetationItemInfo.ColorTint1);
            vegetationItemInfo.ColorTint2 = EditorGUILayout.ColorField("Tint color 2", vegetationItemInfo.ColorTint2);
            vegetationItemInfo.ColorAreaScale = EditorGUILayout.Slider("Tint area scale", vegetationItemInfo.ColorAreaScale, 10f, 150f);

            vegetationItemInfo.RandomDarkening = EditorGUILayout.Slider("Random darkening", vegetationItemInfo.RandomDarkening, 0f, 1f);
            vegetationItemInfo.RootAmbient = EditorGUILayout.Slider("Root ambient", vegetationItemInfo.RootAmbient, 0f, 1f);

            vegetationItemInfo.TextureCutoff = EditorGUILayout.Slider("Alpha cutoff", vegetationItemInfo.TextureCutoff, 0f, 1f);

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(_vegetationSystem.CurrentVegetationPackage);
                _vegetationSystem.RefreshVegetationModelInfoMaterials();
                VegetationStudioManager.VegetationPackageSync_ClearVegetationSystemCellCache(_vegetationSystem);
            }

            EditorGUI.BeginChangeCheck();

            vegetationItemInfo.YScale = EditorGUILayout.Slider("grass Y-Scale", vegetationItemInfo.YScale, 0.5f, 5f);

            GUILayout.EndVertical();
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(_vegetationSystem.CurrentVegetationPackage);
                _vegetationSystem.DelayedClearVegetationCellCache();
            }
            */

        }

        bool HasRuntimeEnabledSpawn()
        {
            for (int i = 0; i <= _vegetationSystem.currentVegetationPackage.VegetationInfoList.Count - 1; i++)
            {
                if (_vegetationSystem.currentVegetationPackage.VegetationInfoList[i].EnableRuntimeSpawn) return true;
            }

            if (_vegetationSystem.currentVegetationPackage.VegetationInfoList.Count == 0) return true;

            return false;
        }

        void DrawRenderInspector()
        {
            HelpTopic = "vegetation-system-render";
            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("General render settings", LabelStyle);
            _vegetationSystem.RenderVegetation = EditorGUILayout.Toggle("Render vegetation", _vegetationSystem.RenderVegetation);
            _vegetationSystem.RenderSingleCamera = EditorGUILayout.Toggle("Render only to selected camera", _vegetationSystem.RenderSingleCamera);

            _vegetationSystem.RenderInstanced = EditorGUILayout.Toggle("Use Instanced rendering", _vegetationSystem.RenderInstanced);
            EditorGUILayout.HelpBox("With use Instanced rendering disabled any non instanced indirect vegetation item will fallback to Graphics.DrawMesh.", MessageType.Info);


            _vegetationSystem.UseMultithreading = EditorGUILayout.Toggle("Use Multithreading", _vegetationSystem.UseMultithreading);
            if (_vegetationSystem.UseMultithreading)
            {
                EditorGUILayout.HelpBox("Multithreading is used in playmode and builds.", MessageType.Info);
            }
            _vegetationSystem.UseListMultithreading = EditorGUILayout.Toggle("Use Render List Multithreading", _vegetationSystem.UseListMultithreading);
            if (_vegetationSystem.UseListMultithreading)
            {
                EditorGUILayout.HelpBox("Render list multithreading can give a CPU speedup on scenes where you have a lot of vegetation items (grass, plants) that does not use Instanced Indirect rendering.", MessageType.Info);
            }

            _vegetationSystem.UseIndirectLoDs = EditorGUILayout.Toggle("Use Instanced Indirect LODs", _vegetationSystem.UseIndirectLoDs);
            EditorGUILayout.HelpBox("This will enable or disable LODs on the Instanced Indirect grass. With compute shaders enabled you get fine tuned control of distances.", MessageType.Info);
            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Compute shaders", LabelStyle);
            EditorGUI.BeginChangeCheck();
            _vegetationSystem.UseComputeShaders = EditorGUILayout.Toggle("Use Compute shaders", _vegetationSystem.UseComputeShaders);
            if (_vegetationSystem.UseComputeShaders)
            {
                _vegetationSystem.UseGPUCulling = EditorGUILayout.Toggle("Use GPU Culling", _vegetationSystem.UseGPUCulling);
                EditorGUILayout.HelpBox("This will enable or disable GPU Culling on the Instanced Indirect grass.", MessageType.Info);
            }
            if (EditorGUI.EndChangeCheck())
            {
                _vegetationSystem.RefreshVegetationModelInfoMaterials();
                VegetationStudioManager.VegetationPackageSync_ClearVegetationSystemCellCache(_vegetationSystem);
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("CPU Frustum culling and LODs", LabelStyle);
            _vegetationSystem.UseObjectCPUCulling = EditorGUILayout.Toggle("Enable on Objects", _vegetationSystem.UseObjectCPUCulling);
            _vegetationSystem.UseLargeObjectCPUCulling = EditorGUILayout.Toggle("Enable on Large Objects", _vegetationSystem.UseLargeObjectCPUCulling);
            EditorGUILayout.HelpBox("You should only use this if you are GPU bound and have high poly objects and large objects. Doing per item LOD and frustum culling on the CPU takes a bit of time. Upgrading the shader to support instanced indirect and doing this on the GPU is recomended.", MessageType.Info);

            GUILayout.EndVertical();
            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Shadows", LabelStyle);
            _vegetationSystem.vegetationSettings.GrassShadows = EditorGUILayout.Toggle("Grass cast shadows", _vegetationSystem.vegetationSettings.GrassShadows);
            _vegetationSystem.vegetationSettings.PlantShadows = EditorGUILayout.Toggle("Plants cast shadows", _vegetationSystem.vegetationSettings.PlantShadows);
            _vegetationSystem.vegetationSettings.TreeShadows = EditorGUILayout.Toggle("Trees cast shadows", _vegetationSystem.vegetationSettings.TreeShadows);
            _vegetationSystem.vegetationSettings.ObjectShadows = EditorGUILayout.Toggle("Objects cast shadows", _vegetationSystem.vegetationSettings.ObjectShadows);
            _vegetationSystem.vegetationSettings.LargeObjectShadows = EditorGUILayout.Toggle("Large objects cast shadows", _vegetationSystem.vegetationSettings.LargeObjectShadows);
            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Editor Shadows", LabelStyle);
            _vegetationSystem.vegetationSettings.EditorGrassShadows = EditorGUILayout.Toggle("Grass cast shadows", _vegetationSystem.vegetationSettings.EditorGrassShadows);
            _vegetationSystem.vegetationSettings.EditorPlantShadows = EditorGUILayout.Toggle("Plants cast shadows", _vegetationSystem.vegetationSettings.EditorPlantShadows);
            _vegetationSystem.vegetationSettings.EditorTreeShadows = EditorGUILayout.Toggle("Trees cast shadows", _vegetationSystem.vegetationSettings.EditorTreeShadows);
            _vegetationSystem.vegetationSettings.EditorObjectShadows = EditorGUILayout.Toggle("Objects cast shadows", _vegetationSystem.vegetationSettings.EditorObjectShadows);
            _vegetationSystem.vegetationSettings.EditorLargeObjectShadows = EditorGUILayout.Toggle("Large objects cast shadows", _vegetationSystem.vegetationSettings.EditorLargeObjectShadows);
            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Layers", LabelStyle);
            EditorGUI.BeginChangeCheck();
            _vegetationSystem.vegetationSettings.GrassLayer = EditorGUILayout.LayerField("Grass layer", _vegetationSystem.vegetationSettings.GrassLayer);
            _vegetationSystem.vegetationSettings.PlantLayer = EditorGUILayout.LayerField("Plant layer", _vegetationSystem.vegetationSettings.PlantLayer);
            _vegetationSystem.vegetationSettings.TreeLayer = EditorGUILayout.LayerField("Tree layer", _vegetationSystem.vegetationSettings.TreeLayer);
            _vegetationSystem.vegetationSettings.ObjectLayer = EditorGUILayout.LayerField("Object layer", _vegetationSystem.vegetationSettings.ObjectLayer);
            _vegetationSystem.vegetationSettings.LargeObjectLayer = EditorGUILayout.LayerField("Large object layer", _vegetationSystem.vegetationSettings.LargeObjectLayer);
            EditorGUILayout.HelpBox("Select what layers vegetation should render on.", MessageType.Info);
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Sun directional light", LabelStyle);
            EditorGUI.BeginChangeCheck();
            _vegetationSystem.SunDirectionalLight = (Light)EditorGUILayout.ObjectField("Sun light", _vegetationSystem.SunDirectionalLight, typeof(Light), true);
            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Shadow culling", LabelStyle);
            _vegetationSystem.vegetationSettings.ShadowCullinMode = (ShadowCullinMode)EditorGUILayout.EnumPopup("Shadow culling range", _vegetationSystem.vegetationSettings.ShadowCullinMode);
            EditorGUILayout.HelpBox("This sets the distance from camera where invisible trees trees will be tested for visible shadows.", MessageType.Info);
            if (EditorGUI.EndChangeCheck())
            {
                _vegetationSystem.RefreshCulling();
                EditorUtility.SetDirty(target);
            }
            GUILayout.EndVertical();
        }

        void DrawNoVegetationPackageError()
        {
            GUILayout.BeginVertical("box");
            EditorGUILayout.HelpBox("You need to add a Vegetation Package to the component. See Settings Tab", MessageType.Error);

            GUILayout.EndVertical();
        }

        void DrawVegetationInspector()
        {
            if (_vegetationSystem.currentVegetationPackage == null)
            {
                DrawNoVegetationPackageError();
                return;
            }

            //VegetationPackage vegetationPackage = _vegetationSystem.CurrentVegetationPackage;

            if (_vegetationSystem.ManualVegetationRefresh)
            {
                GUILayout.BeginVertical("box");
                if (GUILayout.Button("Refresh vegetation"))
                {
                    _vegetationSystem.ClearVegetationCellCache();
                }
                GUILayout.EndVertical();
            }

            GUILayout.BeginVertical("box");
            _vegetationSystem.ShowGlobalVegetationItemSettingsMenu = VegetationPackageEditorTools.DrawHeader("Vegetation Global Settings", _vegetationSystem.ShowGlobalVegetationItemSettingsMenu);
            if (_vegetationSystem.ShowGlobalVegetationItemSettingsMenu)
            {

                EditorGUILayout.LabelField("Vegetation distance: (meter)", LabelStyle);
                float newDistance = EditorGUILayout.Slider("Vegetation distance", _vegetationSystem.GetVegetationDistance(), 10f, 1000f);
                if (Math.Abs(newDistance - _vegetationSystem.GetVegetationDistance()) > 0.01f)
                {
                    _vegetationSystem.SetVegetationDistance(newDistance);
                    EditorUtility.SetDirty(target);
                }

                float newTreeDistance = EditorGUILayout.Slider("Additional mesh tree distance", _vegetationSystem.GetTreeDistance(), 0f, 1000f);
                if (Math.Abs(newTreeDistance - _vegetationSystem.GetTreeDistance()) > 0.01f)
                {
                    _vegetationSystem.SetTreeDistance(newTreeDistance);
                    EditorUtility.SetDirty(target);
                }

                float newBillboardDistance = EditorGUILayout.Slider("Additional Billboard tree distance", _vegetationSystem.GetBillboardDistance(), 0f, 20000f);
                if (Math.Abs(newBillboardDistance - _vegetationSystem.GetBillboardDistance()) > 0.01f)
                {
                    _vegetationSystem.SetBillboardDistance(newBillboardDistance);
                    EditorUtility.SetDirty(target);
                }

                float totalLength = _vegetationSystem.GetVegetationDistance() + _vegetationSystem.GetTreeDistance() + _vegetationSystem.GetBillboardDistance();
                EditorGUILayout.LabelField("Total vegetation distance: " + totalLength.ToString("N0") + " meter", LabelStyle);

                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Vegetation density", LabelStyle);

                float newGrassDensity = EditorGUILayout.Slider("Grass density", _vegetationSystem.vegetationSettings.GrassDensity, 0f, 2f);
                if (Math.Abs(newGrassDensity - _vegetationSystem.vegetationSettings.GrassDensity) > 0.01f)
                {
                    _vegetationSystem.DelayedSetGrassDensity(newGrassDensity);
                    EditorUtility.SetDirty(target);
                }

                float newPlantDensity = EditorGUILayout.Slider("Plant density", _vegetationSystem.vegetationSettings.PlantDensity, 0f, 2f);
                if (Math.Abs(newPlantDensity - _vegetationSystem.vegetationSettings.PlantDensity) > 0.01f)
                {
                    _vegetationSystem.DelayedSetPlantDensity(newPlantDensity);
                    EditorUtility.SetDirty(target);
                }

                float newTreeDensity = EditorGUILayout.Slider("Tree density", _vegetationSystem.vegetationSettings.TreeDensity, 0f, 2f);
                if (Math.Abs(newTreeDensity - _vegetationSystem.vegetationSettings.TreeDensity) > 0.01f)
                {
                    _vegetationSystem.DelayedSetTreeDensity(newTreeDensity);
                    EditorUtility.SetDirty(target);
                }

                float newObjectDensity = EditorGUILayout.Slider("Object density", _vegetationSystem.vegetationSettings.ObjectDensity, 0f, 2f);
                if (Math.Abs(newObjectDensity - _vegetationSystem.vegetationSettings.ObjectDensity) > 0.01f)
                {
                    _vegetationSystem.DelayedSetObjectDensity(newObjectDensity);
                    EditorUtility.SetDirty(target);
                }

                float newLargeObjectDensity = EditorGUILayout.Slider("Large Object density", _vegetationSystem.vegetationSettings.LargeObjectDensity, 0f, 2f);
                if (Math.Abs(newLargeObjectDensity - _vegetationSystem.vegetationSettings.LargeObjectDensity) > 0.01f)
                {
                    _vegetationSystem.DelayedSetLargeObjectDensity(newLargeObjectDensity);
                    EditorUtility.SetDirty(target);
                }

                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Vegetation scale", LabelStyle);
                float newVegetationScale = EditorGUILayout.Slider("Vegetation scale multiplier", _vegetationSystem.vegetationSettings.VegetationScale, 0.1f, 2f);
                if (Math.Abs(newVegetationScale - _vegetationSystem.vegetationSettings.VegetationScale) > 0.01f)
                {
                    _vegetationSystem.DelayedSetVegetationScale(newVegetationScale);
                    EditorUtility.SetDirty(target);
                }


                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Random vegetation", LabelStyle);

                int newRandomSeed = EditorGUILayout.IntSlider("Random seed", _vegetationSystem.vegetationSettings.RandomSeed, 0, 9999);
                if (newRandomSeed != _vegetationSystem.vegetationSettings.RandomSeed)
                {
                    _vegetationSystem.SetRandomSeed(newRandomSeed);
                    EditorUtility.SetDirty(target);
                }

            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");
            _vegetationSystem.ShowAddVegetationItemMenu = VegetationPackageEditorTools.DrawHeader("Add Vegetation Items", _vegetationSystem.ShowAddVegetationItemMenu);
            if (_vegetationSystem.ShowAddVegetationItemMenu)
            {
                

                EditorGUILayout.HelpBox(
                    "Drop a Prefab with the vegetation item here to create new vegetation item. Drop on selected category to get initial rules and settings correct.",
                    MessageType.Info);

                bool addedItem = false;
                GUILayout.BeginHorizontal();
                DropZoneTools.DrawVegetationItemDropZone(DropZoneType.GrassPrefab, _vegetationSystem, ref addedItem);
                DropZoneTools.DrawVegetationItemDropZone(DropZoneType.PlantPrefab, _vegetationSystem, ref addedItem);
                DropZoneTools.DrawVegetationItemDropZone(DropZoneType.TreePrefab, _vegetationSystem, ref addedItem);
                DropZoneTools.DrawVegetationItemDropZone(DropZoneType.ObjectPrefab, _vegetationSystem, ref addedItem);
                DropZoneTools.DrawVegetationItemDropZone(DropZoneType.LargeObjectPrefab, _vegetationSystem,
                    ref addedItem);
                GUILayout.EndHorizontal();

                EditorGUILayout.HelpBox(
                    "Drop a Texture2D to create a new grass or plant vegetation item. Drop on selected category to get initial rules and settings correct.",
                    MessageType.Info);

                GUILayout.BeginHorizontal();
                DropZoneTools.DrawVegetationItemDropZone(DropZoneType.GrassTexture, _vegetationSystem, ref addedItem);
                DropZoneTools.DrawVegetationItemDropZone(DropZoneType.PlantTexture, _vegetationSystem, ref addedItem);
                GUILayout.EndHorizontal();

                if (addedItem)
                {
                    _vegIndex = _vegetationSystem.currentVegetationPackage.VegetationInfoList.Count - 1;
                    _selectedGridIndex = GetSelectedGridIndex(_vegIndex);
                    _vegetationSystem.RefreshVegetationPackage();
                    GUILayout.EndVertical();
                    return;
                }


                //if (newGrassPrefab != null) vegetationPackage.AddVegetationItem(newGrassPrefab, VegetationType.Grass, true);
                //if (newPlantPrefab != null) vegetationPackage.AddVegetationItem(newPlantPrefab, VegetationType.Plant, true);
                //if (newTreePrefab != null) vegetationPackage.AddVegetationItem(newTreePrefab, VegetationType.Tree, true);
                //if (newObjectPrefab != null) vegetationPackage.AddVegetationItem(newObjectPrefab, VegetationType.Objects, true);
                //if (newLargeObjectPrefab != null) vegetationPackage.AddVegetationItem(newLargeObjectPrefab, VegetationType.LargeObjects, true);
                //if (newGrassTexture != null) vegetationPackage.AddVegetationItem(newGrassTexture, VegetationType.Grass, true);
                //if (newPlantTexture != null) vegetationPackage.AddVegetationItem(newPlantTexture, VegetationType.Plant, true);


                //if (newGrassPrefab != null || newPlantPrefab != null || newTreePrefab != null || newObjectPrefab != null ||
                //    newLargeObjectPrefab != null || newPlantTexture != null || newGrassTexture != null)
                //{
                //    _vegetationSystem.RefreshVegetationPackage();
                //}

                if (VegetationStudioManager.GetVegetationItemFromClipboard() != null)
                {
                    if (GUILayout.Button("Paste vegetation item"))
                    {
                        DuplicateVegetationItem(VegetationStudioManager.GetVegetationItemFromClipboard());
                    }
                }
            }
            GUILayout.EndVertical();

            if (!HasRuntimeEnabledSpawn())
            {
                GUILayout.BeginVertical("box");
                EditorGUILayout.HelpBox("All vegetation items have disabled Run-time spawning", MessageType.Warning);

                if (GUILayout.Button("Enable run-time spawn on all VegetationItems"))
                {
                    for (int i = 0; i <= _vegetationSystem.currentVegetationPackage.VegetationInfoList.Count - 1; i++)
                    {
                        _vegetationSystem.currentVegetationPackage.VegetationInfoList[i].EnableRuntimeSpawn = true;
                    }
                    EditorUtility.SetDirty(_vegetationSystem.currentVegetationPackage);
                    _vegetationSystem.DelayedClearVegetationCellCache();
                }
                GUILayout.EndVertical();
            }

            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Select Vegetation Item", LabelStyle);
            VegetationPackageEditorTools.DrawVegetationItemSelector(
                _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex],
                ref _selectedGridIndex, ref _vegIndex, VegetationPackageEditorTools.VegetationItemTypeSelection.AllVegetationItems, 70);

            if (_lastVegIndex != _vegIndex) GUI.FocusControl(null);
            GUILayout.EndVertical();



            if (_vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList.Count > 0)
            {
                GUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("Visualize height/steepness in terrain", LabelStyle);
                EditorGUI.BeginChangeCheck();
                bool overrideMaterial = EditorGUILayout.Toggle("Show heatmap", _vegetationSystem.TerrainMaterialOverridden);
                EditorGUILayout.HelpBox("Enabling heatmap will show the spawn area of the selected vegetation item based on the current height and steepness setting for that item.", MessageType.Info);

                if (EditorGUI.EndChangeCheck())
                {
                    if (overrideMaterial)
                    {
                        _vegetationSystem.UpdateHeatmapMaterial(_vegIndex);
                        _vegetationSystem.OverrideTerrainMaterial(_vegetationSystem.VegetationHeatMapMaterial);
                        EditorUtility.SetDirty(_vegetationSystem);
                    }
                    else
                    {
                        _vegetationSystem.RestoreTerrainMaterial();
                    }
                }
                GUILayout.EndVertical();

                _vegetationSystem.UpdateHeatmapMaterial(_vegIndex);



                GUILayout.BeginVertical("box");
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Delete selected item"))
                {
                    if (EditorUtility.DisplayDialog("Delete VegetationItem?",
                        "Do you want to delete the selected VegetationItem?", "Delete", "Cancel"))
                    {
                        _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList.RemoveAt(_vegIndex);
                        _vegetationSystem.RefreshVegetationPackage();
                        EditorUtility.SetDirty(_vegetationSystem.currentVegetationPackage);
                        return;
                    }
                }
                if (GUILayout.Button("Copy selected item"))
                {
                    VegetationStudioManager.AddVegetationItemToClipboard(_vegetationSystem
                        .VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                        .VegetationInfoList[_vegIndex]);
                    return;
                }

                if (VegetationStudioManager.GetVegetationItemFromClipboard() != null)
                {
                    if (_vegetationSystem
                        .VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                        .VegetationInfoList[_vegIndex].PrefabType == VegetationStudioManager.GetVegetationItemFromClipboard().PrefabType)
                        if (GUILayout.Button("Paste values"))
                        {
                            PasteVegetationItemValues(_vegetationSystem
                                .VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                                .VegetationInfoList[_vegIndex], VegetationStudioManager.GetVegetationItemFromClipboard());

                            _vegetationSystem.DelayedClearVegetationCellCache();
                            EditorUtility.SetDirty(_vegetationSystem.currentVegetationPackage);
                        }
                }
                GUILayout.EndHorizontal();

                _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].Name = EditorGUILayout.TextField("Item Name", _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].Name);
                EditorGUILayout.LabelField("Type: " + _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].VegetationType.ToString(), LabelStyle);
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
                    }


                    if (GUILayout.Button("Refresh prefab"))
                    {
                        _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].ShaderType = VegetationTypeDetector.GetVegetationShaderType(_vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].VegetationPrefab);

                        if (_vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].VegetationType == VegetationType.Tree)
                        {
                           // _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].GenerateBillboard(_vegIndex);
                        }
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

                if (_vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].VegetationRenderType == VegetationRenderType.InstancedIndirect)
                {
                    EditorGUILayout.HelpBox("Instanced indirect will only work on shaders that supports Vegetation Studios indirect implementation. Like the custom grass shader.", MessageType.Warning);
                }

                if (EditorGUI.EndChangeCheck())
                {
                    _selectedGridIndex = GetSelectedGridIndex(_vegIndex);
                    _vegetationSystem.RefreshVegetationPackage();
                    EditorUtility.SetDirty(_vegetationSystem.currentVegetationPackage);
                }

                EditorGUI.BeginChangeCheck();
                _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].EnableRuntimeSpawn = EditorGUILayout.Toggle("Enable run-time spawn", _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].EnableRuntimeSpawn);
                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(_vegetationSystem.currentVegetationPackage);
                    EditorUtility.SetDirty(target);
                    _vegetationSystem.DelayedClearVegetationCellCache();
                }

                DrawTouchReactSettings();
                DrawBillboardSettings();
                DrawColliderSettings();
                DrawNavMeshObstacleSettings();
                DrawShadowSettings();
                DrawLODSettings();
                //DrawVegetationItemGrassSettings();
                DrawVegetationItemSpeedtreeSettings();

                EditorGUI.BeginChangeCheck();

                _vegetationSystem.ShowPositionMenu = VegetationPackageEditorTools.DrawHeader("Position", _vegetationSystem.ShowPositionMenu);
                if (_vegetationSystem.ShowPositionMenu)
                {
                    GUILayout.BeginVertical("box");
                    _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                        .VegetationInfoList[_vegIndex]
                        .Seed = EditorGUILayout.IntSlider("Seed",
                        _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                            .VegetationInfoList[_vegIndex]
                            .Seed, 0, 100);
                    _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                        .VegetationInfoList[_vegIndex]
                        .SampleDistance = EditorGUILayout.Slider("Sample distance(meter)",
                        _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                            .VegetationInfoList[_vegIndex]
                            .SampleDistance, 0.3f, 100f);
                    _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                        .VegetationInfoList[_vegIndex]
                        .Density = EditorGUILayout.Slider("Density",
                        _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                            .VegetationInfoList[_vegIndex]
                            .Density, 0, 1f);
                    _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                        .VegetationInfoList[_vegIndex]
                        .RandomizePosition = EditorGUILayout.Toggle("Randomize distribution",
                        _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                            .VegetationInfoList[_vegIndex]
                            .RandomizePosition);

                    if (_vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                        .VegetationInfoList[_vegIndex]
                        .RandomizePosition == true)
                    {
                        
                        _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                            .VegetationInfoList[_vegIndex]
                            .RandomPositionRelativeDistance = EditorGUILayout.Slider("Random distance",
                            _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                                .VegetationInfoList[_vegIndex]
                                .RandomPositionRelativeDistance, 0, 1f);

                        if (_vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                                .VegetationInfoList[_vegIndex]
                                .VegetationType == VegetationType.Tree ||
                            _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                                .VegetationInfoList[_vegIndex]
                                .VegetationType == VegetationType.LargeObjects)
                        {
                            _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                                .VegetationInfoList[_vegIndex]
                                .UseCollisionDetection = EditorGUILayout.Toggle("Collision Detection",
                                _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                                    .VegetationInfoList[_vegIndex]
                                    .UseCollisionDetection);

                        }

                        Rect space = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(18));
                        _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                            .VegetationInfoList[_vegIndex]
                            .Offset = EditorGUI.Vector3Field(space, "Position offset",
                            _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                                .VegetationInfoList[_vegIndex]
                                .Offset);
                                
                    }
                    GUILayout.EndVertical();
                }

                _vegetationSystem.ShowScaleMenu = VegetationPackageEditorTools.DrawHeader("Scale/Rotation", _vegetationSystem.ShowScaleMenu);
                if (_vegetationSystem.ShowScaleMenu)
                {
                    GUILayout.BeginVertical("box");
                    _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                        .VegetationInfoList[_vegIndex]
                        .VegetationScaleType = (VegetationScaleType)EditorGUILayout.EnumPopup("Scale type",
                        _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                            .VegetationInfoList[_vegIndex]
                            .VegetationScaleType);

                    if (_vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                            .VegetationInfoList[_vegIndex]
                            .VegetationScaleType == VegetationScaleType.Simple)
                    {
                        EditorFunctions.FloatRangeField("Min/Max scale", ref
                            _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                                .VegetationInfoList[_vegIndex]
                                .MinScale,
                            ref _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                                .VegetationInfoList[_vegIndex]
                                .MaxScale, 0.1f, 10f);
                    }
                    else
                    {
                        Rect spaceMin = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(18));
                        _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                            .VegetationInfoList[_vegIndex]
                            .MinVectorScale = EditorGUI.Vector3Field(spaceMin, "Minimum scale",
                            _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                                .VegetationInfoList[_vegIndex]
                                .MinVectorScale);
                        Rect spaceMax = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(18));
                        _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                            .VegetationInfoList[_vegIndex]
                            .MaxVectorScale = EditorGUI.Vector3Field(spaceMax, "Maximum scale",
                            _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                                .VegetationInfoList[_vegIndex]
                                .MaxVectorScale);
                    }
                    GUILayout.EndVertical();

                    GUILayout.BeginVertical("box");
                    _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                        .VegetationInfoList[_vegIndex]
                        .Rotation = (VegetationRotationType)EditorGUILayout.EnumPopup("Rotation",
                        _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                            .VegetationInfoList[_vegIndex]
                            .Rotation);

                    if (_vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                            .VegetationInfoList[_vegIndex]
                            .Rotation != VegetationRotationType.FollowTerrainScale && _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                            .VegetationInfoList[_vegIndex]
                            .Rotation != VegetationRotationType.FollowTerrainScaleWithBlock &&
                        _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                            .VegetationInfoList[_vegIndex]
                            .VegetationType == VegetationType.Grass)
                    {
                        EditorGUILayout.HelpBox(
                            "For better grass coverage and look in steep areas. FollowTerrainScale setting is recomended.",
                            MessageType.Warning);
                    }

                    Rect space = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(18));
                    _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                        .VegetationInfoList[_vegIndex]
                        .RotationOffset = EditorGUI.Vector3Field(space, "Rotation offset",
                        _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                            .VegetationInfoList[_vegIndex]
                            .RotationOffset);
                    GUILayout.EndVertical();
                }

                _vegetationSystem.ShowHeightMenu = VegetationPackageEditorTools.DrawHeader("Height/Steepness", _vegetationSystem.ShowHeightMenu);
                if (_vegetationSystem.ShowHeightMenu)
                {
                    GUILayout.BeginVertical("box");
                    _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                        .VegetationInfoList[_vegIndex]
                        .UseHeightLevel = EditorGUILayout.Toggle("Use height level",
                        _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                            .VegetationInfoList[_vegIndex]
                            .UseHeightLevel);
                    if (_vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                        .VegetationInfoList[_vegIndex]
                        .UseHeightLevel)
                    {
                        _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                            .VegetationInfoList[_vegIndex]
                            .VegetationHeightType = (VegetationHeightType)EditorGUILayout.EnumPopup("Selection type",
                            _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                                .VegetationInfoList[_vegIndex]
                                .VegetationHeightType);
                        if (_vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                                .VegetationInfoList[_vegIndex]
                                .VegetationHeightType == VegetationHeightType.Simple)
                        {

                            EditorFunctions.FloatRangeField("Min/Max height",
                                ref _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                                    .VegetationInfoList[_vegIndex]
                                    .MinimumHeight,
                                ref _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                                    .VegetationInfoList[_vegIndex]
                                    .MaximumHeight, -500f, 10000f);
                        }
                        else
                        {
                            EditorFunctions.FloatRangeField("Min/Max cuve height",
                                ref _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                                    .VegetationInfoList[_vegIndex]
                                    .MinCurveHeight,
                                ref _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                                    .VegetationInfoList[_vegIndex]
                                    .MaxCurveHeight, -500, 10000);

                            _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                                    .VegetationInfoList[_vegIndex]
                                    .AutomaticCurveMaxHeight =
                                EditorGUILayout.Toggle("Max curve height from terrain",
                                    _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                                        .VegetationInfoList[_vegIndex]
                                        .AutomaticCurveMaxHeight);


                            if (_heightCurveEditor.EditCurve(
                                _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                                    .VegetationInfoList[_vegIndex]
                                    .HeightCurve, this))
                            {
                                _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                                    .UpdateCurves();
                                EditorUtility.SetDirty(_vegetationSystem.currentVegetationPackage);
                                _vegetationSystem.DelayedClearVegetationCellCache();
                                EditorUtility.SetDirty(target);
                            }
                        }
                    }
                    GUILayout.EndVertical();

                    GUILayout.BeginVertical("box");
                    _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                        .VegetationInfoList[_vegIndex]
                        .UseAngle = EditorGUILayout.Toggle("Use steepness cutoff",
                        _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                            .VegetationInfoList[_vegIndex]
                            .UseAngle);
                    if (_vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                        .VegetationInfoList[_vegIndex]
                        .UseAngle)
                    {
                        _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                            .VegetationInfoList[_vegIndex]
                            .VegetationSteepnessType = (VegetationSteepnessType)EditorGUILayout.EnumPopup(
                            "Selection type",
                            _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                                .VegetationInfoList[_vegIndex]
                                .VegetationSteepnessType);
                        if (_vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                                .VegetationInfoList[_vegIndex]
                                .VegetationSteepnessType == VegetationSteepnessType.Simple)
                        {

                            EditorFunctions.FloatRangeField("Min/Max steepness",
                                ref _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                                    .VegetationInfoList[_vegIndex]
                                    .MinimumSteepness,
                                ref _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                                    .VegetationInfoList[_vegIndex]
                                    .MaximumSteepness, 0f, 90f);
                        }
                        else
                        {
                            if (_steepnessCurveEditor.EditCurve(
                                _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                                    .VegetationInfoList[_vegIndex]
                                    .SteepnessCurve, this))
                            {
                                _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                                    .UpdateCurves();
                                EditorUtility.SetDirty(_vegetationSystem.currentVegetationPackage);
                                _vegetationSystem.DelayedClearVegetationCellCache();
                                EditorUtility.SetDirty(target);
                            }
                        }
                    }
                    GUILayout.EndVertical();
                }

                _vegetationSystem.ShowPerlinMenu = VegetationPackageEditorTools.DrawHeader("Perlin noise", _vegetationSystem.ShowPerlinMenu);
                if (_vegetationSystem.ShowPerlinMenu)
                {
                    GUILayout.BeginVertical("box");
                    _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                        .VegetationInfoList[_vegIndex]
                        .UsePerlinMask = EditorGUILayout.Toggle("Use perlin noise",
                        _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                            .VegetationInfoList[_vegIndex]
                            .UsePerlinMask);
                    if (_vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                        .VegetationInfoList[_vegIndex]
                        .UsePerlinMask)
                    {
                        _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                            .VegetationInfoList[_vegIndex]
                            .PerlinCutoff = EditorGUILayout.Slider("Perlin noise cutoff",
                            _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                                .VegetationInfoList[_vegIndex]
                                .PerlinCutoff, 0f, 1f);
                        _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                            .VegetationInfoList[_vegIndex]
                            .PerlinScale = EditorGUILayout.Slider("Perlin noise scale",
                            _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                                .VegetationInfoList[_vegIndex]
                                .PerlinScale, 1f, 500f);
                        _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                            .VegetationInfoList[_vegIndex]
                            .PerlinOffset = EditorGUILayout.Vector2Field("Perlin noise offset",
                            _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                                .VegetationInfoList[_vegIndex]
                                .PerlinOffset);
                        _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                            .VegetationInfoList[_vegIndex]
                            .InversePerlinMask = EditorGUILayout.Toggle("Inverse perlin noise",
                            _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                                .VegetationInfoList[_vegIndex]
                                .InversePerlinMask);
                    }
                    GUILayout.EndVertical();
                }


                _vegetationSystem.ShowTerrainTextureRulesMenu = VegetationPackageEditorTools.DrawHeader("Terrain texture rules", _vegetationSystem.ShowTerrainTextureRulesMenu);
                if (_vegetationSystem.ShowTerrainTextureRulesMenu)
                {
                    GUILayout.BeginVertical("box");
                    EditorGUILayout.LabelField("Terrain textures", LabelStyle);

                    EditorGUILayout.LabelField("Exclude terrain textures", LabelStyle);
                    _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].UseExcludeTextueMask = EditorGUILayout.Toggle("Use exclude texture rules", _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].UseExcludeTextueMask);
                    if (_vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].UseExcludeTextueMask)
                    {
                        DrawExcludeTextures();
                    }

                    EditorGUILayout.LabelField("Include terrain textures", LabelStyle);
                    _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].UseIncludeTextueMask = EditorGUILayout.Toggle("Use include texture rules", _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].UseIncludeTextueMask);
                    if (_vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].UseIncludeTextueMask)
                    {
                        DrawIncludeTextures();
                    }
                    GUILayout.EndVertical();
                }


                _vegetationSystem.ShowTextureMaskRulesMenu = VegetationPackageEditorTools.DrawHeader("Texture mask rules", _vegetationSystem.ShowTextureMaskRulesMenu);
                if (_vegetationSystem.ShowTextureMaskRulesMenu)
                {
                    GUILayout.BeginVertical("box");

                    EditorGUILayout.LabelField("Scale texture mask rules", LabelStyle);
                    _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].UseScaleTextueMaskRules = EditorGUILayout.Toggle("Use scale rules", _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].UseScaleTextueMaskRules);
                    if (_vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].UseScaleTextueMaskRules)
                    {
                        DrawTextureMasksRules(_vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].ScaleTextureMaskRuleList, ref _scaleTextureMaskRuleIndex, TextureMaskRuleType.Scale);
                    }

                    EditorGUILayout.LabelField("Densisty texture mask rules", LabelStyle);
                    _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].UseDensityTextueMaskRules = EditorGUILayout.Toggle("Use density rules", _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].UseDensityTextueMaskRules);
                    if (_vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].UseDensityTextueMaskRules)
                    {
                        DrawTextureMasksRules(_vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].DensityTextureMaskRuleList, ref _densityTextureMaskRuleIndex, TextureMaskRuleType.Density);
                    }

                    EditorGUILayout.LabelField("Exclude texture mask rules", LabelStyle);
                    _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].UseExcludeTextueMaskRules = EditorGUILayout.Toggle("Use exclude rules", _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].UseExcludeTextueMaskRules);
                    if (_vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].UseExcludeTextueMaskRules)
                    {
                        DrawTextureMasksRules(_vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].ExcludeTextureMaskRuleList, ref _excludeTextureMaskRuleIndex, TextureMaskRuleType.Exclude);
                    }

                    EditorGUILayout.LabelField("Include texture mask rules", LabelStyle);
                    _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].UseIncludeTextueMaskRules = EditorGUILayout.Toggle("Use include rules", _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].UseIncludeTextueMaskRules);
                    if (_vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].UseIncludeTextueMaskRules)
                    {
                        DrawTextureMasksRules(_vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].IncludeTextureMaskRuleList, ref _includeTextureMaskRuleIndex, TextureMaskRuleType.Include);
                    }

                    GUILayout.EndVertical();
                }

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

                _vegetationSystem.ShowDetailMaskMenu = VegetationPackageEditorTools.DrawHeader("Terrain Detail Mask", _vegetationSystem.ShowDetailMaskMenu);
                if (_vegetationSystem.ShowDetailMaskMenu)
                {
                    if (!_vegetationSystem.LoadUnityTerrainDetails)
                    {
                        EditorGUILayout.HelpBox("Enable load terrain details mask data on Settings tab to use terrain detail mask rules.", MessageType.Warning);
                    }

                    _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].UseIncludeDetailMaskRules = EditorGUILayout.Toggle("Use detail mask include rule", _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].UseIncludeDetailMaskRules);
                    _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].IncludeDetailLayer = EditorGUILayout.IntField("Selected detail layer", _vegetationSystem.VegetationPackageList[_vegetationSystem.VegetationPackageIndex].VegetationInfoList[_vegIndex].IncludeDetailLayer);
                    EditorGUILayout.HelpBox("Terrain detail masks are normally used for importing detail distribution from exiting terrain. This is configured automatically from the Terrain Detail Importer", MessageType.Info);
                }

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

        

        int GetSelectedGridIndex(int vegIndex)
        {
            List<int> vegetationItemIndexList = new List<int>();
            for (int i = 0;
                i <= _vegetationSystem
                    .VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                    .VegetationInfoList.Count - 1;
                i++)
            {
                vegetationItemIndexList.Add(i);
            }

            VegetationInfoComparer vIc = new VegetationInfoComparer
            {
                VegetationInfoList = _vegetationSystem
                    .VegetationPackageList[_vegetationSystem.VegetationPackageIndex]
                    .VegetationInfoList
            };
            vegetationItemIndexList.Sort(vIc.Compare);

            for (int i = 0; i <= vegetationItemIndexList.Count - 1; i++)
            {
                if (_vegetationSystem.currentVegetationPackage.VegetationInfoList[vegetationItemIndexList[i]].VegetationItemID ==
                    _vegetationSystem.currentVegetationPackage.VegetationInfoList[vegIndex].VegetationItemID)
                {
                    return i;
                }
            }
            return 0;
        }

        void DrawTextureMasksRules(List<TextureMaskRule> textureMaskRuleList, ref int gridIndex, TextureMaskRuleType ruleType)
        {
            if (!_vegetationSystem.HasTextureMask())
            {
                EditorGUILayout.HelpBox("VegetationPackage has no texture masks. Add one to make texture mask rules",
                    MessageType.Warning);
            }

            if (_vegetationSystem.HasTextureMask())
            {
                if (GUILayout.Button("Add new item"))
                {
                    TextureMaskRule newTextureMaskRule =
                        new TextureMaskRule { MaskId = _vegetationSystem.GetDefaultTextureMaskID() };
                    _vegetationSystem.AddTextureMaskProperties(newTextureMaskRule);
                    textureMaskRuleList.Add(newTextureMaskRule);
                }
            }

            GUIContent[] textureImageButtons = new GUIContent[textureMaskRuleList.Count];

            for (int i = 0; i <= textureMaskRuleList.Count - 1; i++)
            {
                textureImageButtons[i] = new GUIContent { image = AssetPreviewCache.GetAssetPreview(_vegetationSystem.GetTextureMaskTexture(textureMaskRuleList[i].MaskId)) };
            }

            if (textureImageButtons.Length > 0)
            {
                int oldGridIndex = gridIndex;

                int imageWidth = 80;
                int columns = Mathf.FloorToInt((EditorGUIUtility.currentViewWidth - 50f) / imageWidth);
                int rows = Mathf.CeilToInt((float)textureImageButtons.Length / columns);
                int gridHeight = (rows) * imageWidth;
                if (gridIndex > textureImageButtons.Length - 1) gridIndex = 0;
                gridIndex = GUILayout.SelectionGrid(gridIndex, textureImageButtons,
                    columns, GUILayout.MaxWidth(columns * imageWidth), GUILayout.MaxHeight(gridHeight));

                if (oldGridIndex != gridIndex) GUI.FocusControl(null);

                GUILayout.BeginVertical("box");

                if (GUILayout.Button("Delete selected item"))
                {
                    textureMaskRuleList.RemoveAt(gridIndex);
                    EditorUtility.SetDirty(_vegetationSystem.currentVegetationPackage);
                    _vegetationSystem.DelayedClearVegetationCellCache();
                    EditorUtility.SetDirty(target);
                    return;
                }

                DrawTextureMaskSelector(textureMaskRuleList[gridIndex]);

                for (int j = 0; j <= textureMaskRuleList[gridIndex].TextureMaskPropertiesList.Count - 1; j++)
                {
                    DrawTextureMaskProperty(textureMaskRuleList[gridIndex]
                        .TextureMaskPropertiesList[j]);
                }

                if (ruleType == TextureMaskRuleType.Include || ruleType == TextureMaskRuleType.Exclude)
                {
                    DrawTextureMaskDensitySelector(
                        textureMaskRuleList[gridIndex]);
                }

                if (ruleType == TextureMaskRuleType.Density)
                {
                    textureMaskRuleList[gridIndex].DensityMultiplier =
                        EditorGUILayout.Slider("Density multiplier", textureMaskRuleList[gridIndex].DensityMultiplier, 0, 1);
                }

                if (ruleType == TextureMaskRuleType.Scale)
                {
                    textureMaskRuleList[gridIndex].ScaleMultiplier =
                        EditorGUILayout.Slider("Scale multiplier", textureMaskRuleList[gridIndex].ScaleMultiplier, 0, 1);
                }
                GUILayout.EndVertical();
            }
        }
        
        void DrawTextureMaskDensitySelector(TextureMaskRule textureMaskRule)
        {
            EditorFunctions.FloatRangeField("Min/Max texture mask density", ref textureMaskRule.MinDensity,
                ref textureMaskRule.MaxDensity, 0, 1);
        }

        void DrawTextureMaskSelector(TextureMaskRule textureMaskRule)
        {
            if (!_vegetationSystem.HasTextureMask()) return;

            int textureMaskIndex = _vegetationSystem.GetTextureMaskIndexFromID(textureMaskRule.MaskId);
            int newTextureMaskIndex = EditorGUILayout.Popup("Selected texture mask", textureMaskIndex, _vegetationSystem.GetTextureMaskNameArray());
            if (textureMaskIndex != newTextureMaskIndex)
            {
                textureMaskRule.MaskId = _vegetationSystem.GetTextureMaskIDFromIndex(newTextureMaskIndex);
                _vegetationSystem.AddTextureMaskProperties(textureMaskRule);

                _vegetationSystem.RefreshTextureMasks();
            }
        }

        void DrawTextureMaskProperty(TextureMaskProperty textureMaskProperty)
        {
            switch (textureMaskProperty.TextureMaskPropertyType)
            {
                case TextureMaskPropertyType.Integer:
                    textureMaskProperty.IntValue = EditorGUILayout.IntSlider(textureMaskProperty.PropertyDescription, textureMaskProperty.IntValue,
                        textureMaskProperty.IntMinValue, textureMaskProperty.IntMaxValue);
                    break;

                case TextureMaskPropertyType.Float:
                    textureMaskProperty.FloatValue = EditorGUILayout.Slider(textureMaskProperty.PropertyDescription, textureMaskProperty.FloatValue,
                        textureMaskProperty.FloatMinValue, textureMaskProperty.FloatMaxValue);
                    break;
                case TextureMaskPropertyType.RgbaSelector:
                    DrawRgbaChannelSelector(textureMaskProperty);
                    break;
                case TextureMaskPropertyType.DropDownStringList:
                    DropdownStringListSelector(textureMaskProperty);
                    break;
                case TextureMaskPropertyType.Boolean:
                    textureMaskProperty.BoolValue = EditorGUILayout.Toggle(textureMaskProperty.PropertyDescription,
                        textureMaskProperty.BoolValue);
                    break;
                case TextureMaskPropertyType.ColorSelector:
                    textureMaskProperty.ColorValue = EditorGUILayout.ColorField(textureMaskProperty.PropertyDescription,
                        textureMaskProperty.ColorValue);
                    break;
            }
        }

        void DropdownStringListSelector(TextureMaskProperty textureMaskProperty)
        {
            textureMaskProperty.IntValue = EditorGUILayout.Popup(textureMaskProperty.PropertyDescription, textureMaskProperty.IntValue, textureMaskProperty.StringList.ToArray());
        }

        void DrawRgbaChannelSelector(TextureMaskProperty textureMaskProperty)
        {
            textureMaskProperty.IntValue = EditorGUILayout.Popup(textureMaskProperty.PropertyDescription, textureMaskProperty.IntValue, RgbaChannelStrings);
        }

        void DrawExcludeTextures()
        {
            VegetationItemInfo itemInfo = (_vegetationSystem.currentVegetationPlacingData == null) ? _vegetationSystem.currentVegetationPackage.VegetationInfoList[_vegIndex] :
                  _vegetationSystem.currentVegetationPlacingData.VegetationInfoList[_vegIndex];


            if (GUILayout.Button("Add new item"))
            {
                TextureMaskInfo newTextureMaskInfo = new TextureMaskInfo
                {
                    MinimumValue = 0.1f,
                    MaximumValue = 1,
                    TextureLayer = 0
                };

                itemInfo.ExcludeTextureMaskList.Add(newTextureMaskInfo);
                _excludeTextureMaskIndex = itemInfo.ExcludeTextureMaskList.Count - 1;
            }

            GUIContent[] textureImageButtons = new GUIContent[itemInfo.ExcludeTextureMaskList.Count];

            for (int i = 0; i <= itemInfo.ExcludeTextureMaskList.Count - 1; i++)
            {
                TextureMaskInfo tempMaskInfo = itemInfo.ExcludeTextureMaskList[i];


                if (_vegetationSystem.currentVegetationPackage.TerrainTextureCount == 0)
                {
                    textureImageButtons[i] = new GUIContent { image = GetTerrainTexture(tempMaskInfo.TextureLayer) };
                }
                else
                {
                    if (tempMaskInfo.TextureLayer >= _vegetationSystem.currentVegetationPackage.TerrainTextureCount)
                    {
                        textureImageButtons[i] = new GUIContent { image = new Texture2D(80, 80) };
                    }
                    else
                    {
                        textureImageButtons[i] = new GUIContent
                        {
                            image = AssetPreviewCache.GetAssetPreview(_vegetationSystem.currentVegetationPackage.TerrainTextureList[tempMaskInfo.TextureLayer].Texture)
                        };
                    }
                }
            }
            if (textureImageButtons.Length > 0)
            {
                GUILayout.EndVertical();
                int imageWidth = 80;
                int columns = Mathf.FloorToInt((EditorGUIUtility.currentViewWidth - 50f) / imageWidth);
                int rows = Mathf.CeilToInt((float)textureImageButtons.Length / columns);
                int gridHeight = (rows) * imageWidth;
                if (_excludeTextureMaskIndex > textureImageButtons.Length - 1) _excludeTextureMaskIndex = 0;
                _excludeTextureMaskIndex = GUILayout.SelectionGrid(_excludeTextureMaskIndex, textureImageButtons, columns, GUILayout.MaxWidth(columns * imageWidth), GUILayout.MaxHeight(gridHeight));
                GUILayout.BeginVertical("box");

                GUILayout.BeginVertical("box");

                if (GUILayout.Button("Delete selected item"))
                {
                    itemInfo.ExcludeTextureMaskList.RemoveAt(_excludeTextureMaskIndex);
                    return;
                }

                TextureMaskInfo tempMaskInfo = itemInfo.ExcludeTextureMaskList[_excludeTextureMaskIndex];
                tempMaskInfo.TextureLayer = (int)(TerrainTextureType)EditorGUILayout.EnumPopup("Selected texture", (TerrainTextureType)tempMaskInfo.TextureLayer);

                float minDensity = tempMaskInfo.MinimumValue;
                float maxDensity = tempMaskInfo.MaximumValue;
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Min/Max Texture Placement Density", GUILayout.MaxWidth(196));
                minDensity = EditorGUILayout.FloatField(minDensity);
                EditorGUILayout.MinMaxSlider(ref minDensity, ref maxDensity, 0.01f, 1f);
                maxDensity = EditorGUILayout.FloatField(maxDensity);
                GUILayout.EndHorizontal();
                tempMaskInfo.MinimumValue = minDensity;
                tempMaskInfo.MaximumValue = maxDensity;

                GUILayout.EndVertical();
            }
        }

        Texture2D GetTerrainTexture(int index)
        {
            Terrain terrain = _vegetationSystem.currentTerrain;
            if (terrain)
            {
                if (terrain.terrainData.splatPrototypes.Length > index)
                {
                    return AssetPreviewCache.GetAssetPreview(terrain.terrainData.splatPrototypes[index].texture);
                }
            }
            return new Texture2D(80, 80);
        }

        void DrawIncludeTextures()
        {
            VegetationItemInfo itemInfo = (_vegetationSystem.currentVegetationPlacingData == null) ? _vegetationSystem.currentVegetationPackage.VegetationInfoList[_vegIndex] :
                   _vegetationSystem.currentVegetationPlacingData.VegetationInfoList[_vegIndex];

            if (GUILayout.Button("Add new item"))
            {
                TextureMaskInfo newTextureMaskInfo = new TextureMaskInfo
                {
                    MinimumValue = 0.1f,
                    MaximumValue = 1,
                    TextureLayer = 0
                };

               

                itemInfo.IncludeTextureMaskList.Add(newTextureMaskInfo);
                _includeTextureMaskIndex = itemInfo.IncludeTextureMaskList.Count - 1;
            }

            GUIContent[] textureImageButtons = new GUIContent[itemInfo.IncludeTextureMaskList.Count];

            for (int i = 0; i <= itemInfo.IncludeTextureMaskList.Count - 1; i++)
            {
                TextureMaskInfo tempMaskInfo = itemInfo.IncludeTextureMaskList[i];

                if (_vegetationSystem.currentVegetationPackage.TerrainTextureCount == 0)
                {
                    textureImageButtons[i] = new GUIContent { image = GetTerrainTexture(tempMaskInfo.TextureLayer) };
                }
                else
                {
                    if (tempMaskInfo.TextureLayer >= _vegetationSystem.currentVegetationPackage.TerrainTextureCount)
                    {
                        textureImageButtons[i] = new GUIContent { image = new Texture2D(80, 80) };
                    }
                    else
                    {
                        textureImageButtons[i] = new GUIContent
                        {
                            image = AssetPreviewCache.GetAssetPreview(_vegetationSystem.currentVegetationPackage.TerrainTextureList[tempMaskInfo.TextureLayer].Texture)
                        };

                    }
                }
            }

            if (textureImageButtons.Length > 0)
            {
                GUILayout.EndVertical();
                int imageWidth = 80;
                int columns = Mathf.FloorToInt((EditorGUIUtility.currentViewWidth - 50) / imageWidth);
                int rows = Mathf.CeilToInt((float)textureImageButtons.Length / columns);
                int gridHeight = (rows) * imageWidth;
                if (_includeTextureMaskIndex > textureImageButtons.Length - 1) _includeTextureMaskIndex = 0;
                _includeTextureMaskIndex = GUILayout.SelectionGrid(_includeTextureMaskIndex, textureImageButtons, columns, GUILayout.MaxWidth(columns * imageWidth), GUILayout.MaxHeight(gridHeight));
                GUILayout.BeginVertical("box");

                GUILayout.BeginVertical("box");

                if (GUILayout.Button("Delete selected item"))
                {
                    itemInfo.IncludeTextureMaskList.RemoveAt(_includeTextureMaskIndex);
                    return;
                }

                TextureMaskInfo tempMaskInfo = itemInfo.IncludeTextureMaskList[_includeTextureMaskIndex];

                tempMaskInfo.TextureLayer = (int)(TerrainTextureType)EditorGUILayout.EnumPopup("Selected texture", (TerrainTextureType)tempMaskInfo.TextureLayer);


                float minDensity = tempMaskInfo.MinimumValue;
                float maxDensity = tempMaskInfo.MaximumValue;
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Min/Max Texture Placement Density", GUILayout.MaxWidth(196));
                minDensity = EditorGUILayout.FloatField(minDensity);
                EditorGUILayout.MinMaxSlider(ref minDensity, ref maxDensity, 0.01f, 1f);
                maxDensity = EditorGUILayout.FloatField(maxDensity);
                GUILayout.EndHorizontal();
                tempMaskInfo.MinimumValue = minDensity;
                tempMaskInfo.MaximumValue = maxDensity;

                GUILayout.EndVertical();
            }
        }
        
        void DrawEditorInspector()
        {
            GUILayout.BeginVertical("box");
            _vegetationSystem.DisableWhenSaving = EditorGUILayout.Toggle("Disable & enable when saving", _vegetationSystem.DisableWhenSaving);
            EditorGUILayout.Space();

            _vegetationSystem.ShowCellGrid = EditorGUILayout.Toggle("Show vegetation cells", _vegetationSystem.ShowCellGrid);
            _vegetationSystem.ShowCellLoadState = EditorGUILayout.Toggle("Show cells loadstate", _vegetationSystem.ShowCellLoadState);
            EditorGUILayout.HelpBox("Will show the LOD loaded in cell, cache state and not loaded cells", MessageType.Info);
            GUILayout.EndVertical();
        }

        void DrawSelectPackage()
        {
            GUILayout.BeginVertical("box");

            string[] packageNameList = new string[_vegetationSystem.VegetationPackageList.Count];
            for (int i = 0; i <= _vegetationSystem.VegetationPackageList.Count - 1; i++)
            {
                if (_vegetationSystem.VegetationPackageList[i])
                {
                    packageNameList[i] = (i + 1).ToString() + " " + _vegetationSystem.VegetationPackageList[i].PackageName;
                }
                else
                {
                    packageNameList[i] = "Not found";
                }

            }

            EditorGUI.BeginChangeCheck();
            _vegetationSystem.VegetationPackageIndex = EditorGUILayout.Popup("Selected vegetation package", _vegetationSystem.VegetationPackageIndex, packageNameList);

            if (EditorGUI.EndChangeCheck())
            {
                _vegetationSystem.RefreshVegetationPackage();
                EditorUtility.SetDirty(_vegetationSystem);
            }

            EditorGUI.BeginChangeCheck();

            if (_vegetationSystem.currentVegetationPackage)
            {
                _vegetationSystem.currentVegetationPackage.PackageName = EditorGUILayout.TextField("Package name", _vegetationSystem.currentVegetationPackage.PackageName);

            }
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(_vegetationSystem.currentVegetationPackage);
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Available packages", LabelStyle);

            bool deletedPackage = false;
            for (int i = _vegetationSystem.VegetationPackageList.Count - 1; i >= 0; i--)
            {
                if (!_vegetationSystem.VegetationPackageList[i])
                {
                    _vegetationSystem.VegetationPackageList.Remove(_vegetationSystem.VegetationPackageList[i]);
                    deletedPackage = true;

                }
            }
            if (deletedPackage) _vegetationSystem.SetVegetationPackage(0, false);

            for (int i = 0; i <= _vegetationSystem.VegetationPackageList.Count - 1; i++)
            {
                if (_vegetationSystem.VegetationPackageList[i])
                {
                    string packageName = _vegetationSystem.VegetationPackageList[i].PackageName;
                    if (packageName == "") packageName = "No name";

                    GUILayout.BeginHorizontal();

                    VegetationPackage updatedPackage = EditorGUILayout.ObjectField(packageName, _vegetationSystem.VegetationPackageList[i], typeof(VegetationPackage), true) as VegetationPackage;
                    if (updatedPackage != _vegetationSystem.VegetationPackageList[i])
                    {
                        if (updatedPackage == null)
                        {
                            _vegetationSystem.VegetationPackageList.Remove(_vegetationSystem.VegetationPackageList[i]);
                            if (_vegetationSystem.VegetationPackageList.Count > 0)
                            {
                                _vegetationSystem.SetVegetationPackage(0, false);
                            }
                            else
                            {
                                _vegetationSystem.currentVegetationPackage = null;
                                _vegetationSystem.InitDone = false;
                            }
                        }
                        else
                        {
                            _vegetationSystem.VegetationPackageList[i] = updatedPackage;
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                else
                {
                    _vegetationSystem.VegetationPackageList[i] = EditorGUILayout.ObjectField("Empty", _vegetationSystem.VegetationPackageList[i], typeof(VegetationPackage), true) as VegetationPackage;
                }
            }

            if (_vegetationSystem.VegetationPackageList.Count == 0)
            {
                EditorGUILayout.HelpBox("No vegetation package set up. Drag and drop an existing package or create a new. To create a new package right click in a project folder. Choose Create/AwesomeTechnologies/Vegetation package. Packages can be re-used between scenes and terrains.", MessageType.Error);
            }

            Color tempColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.78f, 0.468f, 0.390f);
            VegetationPackage newPackage = EditorGUILayout.ObjectField("Add package", null, typeof(VegetationPackage), true) as VegetationPackage;
            GUI.backgroundColor = tempColor;
            if (newPackage != null)
            {
                _vegetationSystem.VegetationPackageList.Add(newPackage);
                if (_vegetationSystem.VegetationPackageList.Count == 1)
                    _vegetationSystem.SetVegetationPackage(0, false);
            }
            EditorGUILayout.HelpBox("To create a new package right click in a project folder. Choose Create/AwesomeTechnologies/Vegetation package. Then drag and drop it here.", MessageType.Info);

            GUILayout.EndVertical();
        }

        

        void DrawSettingsInspector()
        {
            HelpTopic = "vegetation-system-settings";

            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("General settings", LabelStyle);

            EditorGUI.BeginChangeCheck();
            _vegetationSystem.AutoselectTerrain = EditorGUILayout.Toggle("Auto select terrain", _vegetationSystem.AutoselectTerrain);
            if (EditorGUI.EndChangeCheck())
            {
                if (_vegetationSystem.AutoselectTerrain) _vegetationSystem.SetTerrain(null);
            }
            if (!_vegetationSystem.AutoselectTerrain)
            {
                Terrain newTerrain = EditorGUILayout.ObjectField("Terrain", _vegetationSystem.GetTerrain(), typeof(Terrain), true) as Terrain;
                if (newTerrain != _vegetationSystem.GetTerrain())
                {
                    _vegetationSystem.SetTerrain(newTerrain);
                    EditorUtility.SetDirty(target);
                }
            }

            DrawSelectPackage();

            if (_vegetationSystem.GetTerrain() == null)
            {
                EditorGUILayout.HelpBox("You need to select the terrain for the vegetation system", MessageType.Error);
            }

            EditorGUI.BeginChangeCheck();
            _vegetationSystem.AutoselectCamera = EditorGUILayout.Toggle("Auto select camera", _vegetationSystem.AutoselectCamera);
            if (EditorGUI.EndChangeCheck())
            {
                if (_vegetationSystem.AutoselectCamera) _vegetationSystem.SetCamera(Camera.current);
            }

            if (!_vegetationSystem.AutoselectCamera)
            {
                Camera camera = EditorGUILayout.ObjectField("Camera", _vegetationSystem.GetCamera(), typeof(Camera), true) as Camera;

                if (camera != _vegetationSystem.GetCamera()) 
                _vegetationSystem.SetCamera(camera);
            }

            if (_vegetationSystem.SelectedCamera == null)
            {
                EditorGUILayout.HelpBox("You need to select what camera will control the vegetation system. Vegetation will not render in Playmode or Builds. Editor mode will still work.", MessageType.Error);
            }

            GUILayout.EndVertical();

            

            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Unity terrain settings", LabelStyle);
            _vegetationSystem.DisableUnityDetails = EditorGUILayout.Toggle("Disable unity terrain details", _vegetationSystem.DisableUnityDetails);
            _vegetationSystem.DisableUnityTrees = EditorGUILayout.Toggle("Disable unity terrain trees", _vegetationSystem.DisableUnityTrees);
            _vegetationSystem.SetUnityTerrainPixelError = EditorGUILayout.Toggle("Adjust minimum pixel error", _vegetationSystem.SetUnityTerrainPixelError);
            EditorGUI.BeginChangeCheck();
            _vegetationSystem.LoadUnityTerrainDetails = EditorGUILayout.Toggle("Load terrain details", _vegetationSystem.LoadUnityTerrainDetails);
            EditorGUILayout.HelpBox("Enable this setting to load the unity internal details mask data to memory. This allows for Terrain Detail Mask Rules.", MessageType.Info);

            if (EditorGUI.EndChangeCheck())
            {
                _vegetationSystem.SetupVegetationSystem();
                EditorUtility.SetDirty(target);
            }

            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");

            EditorGUILayout.LabelField("Water", LabelStyle);

            float waterLevel = EditorGUILayout.Slider("Water level", _vegetationSystem.GetWaterLevel(), 0, 5000);
            if (Math.Abs(waterLevel - _vegetationSystem.GetWaterLevel()) > 0.01f)
            {
                _vegetationSystem.SetWaterLevel(waterLevel);

                EditorUtility.SetDirty(target);
            }
            EditorGUILayout.HelpBox("Water level is set in meters above terrain 0 height", MessageType.Info);

            EditorGUI.BeginChangeCheck();
            _vegetationSystem.ExcludeSeaLevelCells = EditorGUILayout.Toggle("Exclude sea level cells. ", _vegetationSystem.ExcludeSeaLevelCells);
            EditorGUILayout.HelpBox("This will exclude all cells with height equal or lower than sea level. Recomended  for terrains with no underwater vegetation.", MessageType.Info);
            if (EditorGUI.EndChangeCheck())
            {
                _vegetationSystem.SetupVegetationSystem();
                EditorUtility.SetDirty(target);
            }

            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.LabelField("Vegetation cells", LabelStyle);
            _vegetationSystem.OverrideCellSize = EditorGUILayout.Toggle("Override vegetation cell size", _vegetationSystem.OverrideCellSize);
            if (EditorGUI.EndChangeCheck())
            {
                _vegetationSystem.SetupVegetationSystem();
                EditorUtility.SetDirty(target);
            }
            if (_vegetationSystem.OverrideCellSize)
            {
                _vegetationSystem.CustomCellSize = EditorGUILayout.Slider("", _vegetationSystem.CustomCellSize, 20, 200);

                if (GUILayout.Button("Refresh cells"))
                {
                    _vegetationSystem.SetupVegetationSystem();
                    EditorUtility.SetDirty(target);
                }
            }
            EditorGUILayout.HelpBox("It is possible to override the calculated cell size. Might be usefull on special terrain setups. Larger cells for terrains with only trees.", MessageType.Info);

            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");

            EditorGUILayout.LabelField("Grass wind", LabelStyle);

            EditorGUI.BeginChangeCheck();

            _vegetationSystem.SelectedWindZone = EditorGUILayout.ObjectField("WindZone", _vegetationSystem.SelectedWindZone, typeof(WindZone), true) as WindZone;

            if (!_vegetationSystem.SelectedWindZone)
            {
                EditorGUILayout.HelpBox("No directional wind zone found. Select one.", MessageType.Error);
            }

            _vegetationSystem.WindWavesTexture = (Texture2D)EditorGUILayout.ObjectField("Wind wave noise texture", _vegetationSystem.WindWavesTexture, typeof(Texture2D), false);
            _vegetationSystem.WindSpeedFactor = EditorGUILayout.Slider("Wind speed factor", _vegetationSystem.WindSpeedFactor, 0, 5);
            _vegetationSystem.WindWavesSize = EditorGUILayout.Slider("Wind wave size", _vegetationSystem.WindWavesSize, 0, 30);
            EditorGUILayout.HelpBox("Wind speed factor is used for Vegetation Studio custom grass. Base speed comes from the directional wind zone:", MessageType.Info);
            _vegetationSystem.vegetationSettings.WindRange = EditorGUILayout.Slider("Max wind effect range", _vegetationSystem.vegetationSettings.WindRange, 0, 250);
            EditorGUILayout.HelpBox("Max wind effect range limits wind effect range. Distance is from selected camera in meters.", MessageType.Info);
            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");

            GUILayout.EndVertical();

            if (EditorGUI.EndChangeCheck())
            {
                _vegetationSystem.SetupWind();
                _vegetationSystem.RefreshVegetationModelInfoMaterials();
                VegetationStudioManager.VegetationPackageSync_ClearVegetationSystemCellCache(_vegetationSystem);
                EditorUtility.SetDirty(target);
            }
        }

        // ReSharper disable once UnusedMember.Local
        void OnInspectorUpdate()
        {
            Repaint();
        }

        private void OnDisable()
        {
            if (_vegetationSystem) _vegetationSystem.RestoreTerrainMaterial();
            _heightCurveEditor.RemoveAll();
            _steepnessCurveEditor.RemoveAll();
        }


    }
}