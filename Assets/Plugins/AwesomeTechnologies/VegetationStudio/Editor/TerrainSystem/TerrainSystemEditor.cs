using System;
using System.Globalization;
using AwesomeTechnologies.Common;
using UnityEngine;
using UnityEditor;
using AwesomeTechnologies.External.CurveEditor;

namespace AwesomeTechnologies
{
    [CustomEditor(typeof(TerrainSystem))]
    public class TerrainSystemEditor : VegetationStudioBaseEditor
    {
        private TerrainSystem _terrainSystem;

        private int _currentTabIndex;
        private int _lastTabIndex;
        private int _currentTextureItem;

        private InspectorCurveEditor _heightCurveEditor;
        private InspectorCurveEditor _steepnessCurveEditor;

        private static readonly string[] TabNames =
        {
            "Settings","Terrain texture splatmaps"
        };

        public void OnEnable()
        {
            var settings = InspectorCurveEditor.Settings.DefaultSettings;
            _heightCurveEditor = new InspectorCurveEditor(settings);
            _steepnessCurveEditor = new InspectorCurveEditor(settings) { CurveType = InspectorCurveEditor.InspectorCurveType.Steepness };
        }

        public void OnDisable()
        {
            _heightCurveEditor.RemoveAll();
            _steepnessCurveEditor.RemoveAll();
        }

        public override void OnInspectorGUI()
        {
            OverrideLogoTextureName = "SectionBanner_TerrainSystem";
            HelpTopic = "home/vegetation-studio/components/terrain-system";
            _terrainSystem = (TerrainSystem)target;
            ShowLogo = !_terrainSystem.VegetationSystem.GetSleepMode();

            base.OnInspectorGUI();


            if (_terrainSystem.VegetationSystem.GetSleepMode())
            {
                EditorGUILayout.HelpBox("Wake up the Vegetation System from sleepmode to edit settings", MessageType.Info);
                return;
            }


            if (!_terrainSystem.VegetationSystem.InitDone)
            {
                GUILayout.BeginVertical("box");
                EditorGUILayout.HelpBox("Vegetation system component has configuration errors. Fix to enable component.", MessageType.Error);
                GUILayout.EndVertical();
                return;
            }

            _currentTabIndex = GUILayout.Toolbar(_currentTabIndex, TabNames, EditorStyles.toolbarButton);
            switch (_currentTabIndex)
            {
                case 0:
                    DrawSettingsInspector();
                    break;
                case 1:
                    DrawAutoTextureInspector();
                    break;
            }

            if (_lastTabIndex == 1 && _currentTabIndex != 1) _terrainSystem.RestoreTerrainMaterial();
            _lastTabIndex = _currentTabIndex;
        }

        void DrawGenerateInspector()
        {
            GUILayout.BeginVertical("box");
            EditorGUILayout.HelpBox("When doing automatic texture distribution the splat map of the terrain will be changed. Any texture enabled will be overwritten. Texture drawn on diabled layers will be kept as normal. To remove disabled texture areas paint over them with any of the enabled layers.", MessageType.Info);
            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");
            if (GUILayout.Button("Generate terrain splat map"))
            {
                if (!_terrainSystem.GetVegetationPackage().AutomaticMaxCurveHeight &&
                    Math.Abs(_terrainSystem.GetVegetationPackage().MaxCurveHeight) < 0.1f)
                {
                    EditorUtility.DisplayDialog("Maximum height",
                        "Maximum curve height need to be more than 0", "OK");
                }
                else
                {

                    _terrainSystem.GenerateTerrainSplatMap(new Bounds(), false);
                    _terrainSystem.VegetationSystem.RestoreTerrainMaterial();
                }


            }
            if (GUILayout.Button("Generate terrain splat map, clear all"))
            {
                _terrainSystem.GenerateTerrainSplatMap(new Bounds(), false, true);
                _terrainSystem.VegetationSystem.RestoreTerrainMaterial();
            }
            GUILayout.EndVertical();

        }

        void DrawAutoTextureInspector()
        {          
            DrawGenerateInspector();

            GUILayout.BeginVertical("box");
            EditorGUILayout.HelpBox("Automatic terrain texture distribution is done based on the curve settings for height over water level and steepness(angle) of the terrain", MessageType.Info);
            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Terrain height", LabelStyle);

            _terrainSystem.GetVegetationPackage().AutomaticMaxCurveHeight =
                EditorGUILayout.Toggle("Use max height in terrain", _terrainSystem.GetVegetationPackage().AutomaticMaxCurveHeight);

            if (!_terrainSystem.GetVegetationPackage().AutomaticMaxCurveHeight)
            {
                _terrainSystem.GetVegetationPackage().MaxCurveHeight = EditorGUILayout.FloatField("Max curve height", _terrainSystem.GetVegetationPackage().MaxCurveHeight);
                EditorGUILayout.LabelField("Max possible terrain height: " + _terrainSystem.VegetationSystem.UnityTerrainData.size.y.ToString(CultureInfo.InvariantCulture) + " meters", LabelStyle);

                if (GUILayout.Button("Calculate max height in terrain."))
                {
                    _terrainSystem.GetVegetationPackage().MaxCurveHeight = _terrainSystem.VegetationSystem.UnityTerrainData.MaxTerrainHeight;
                    EditorUtility.SetDirty(_terrainSystem.GetVegetationPackage());
                }

                if (_terrainSystem.GetVegetationPackage().MaxCurveHeight < 1)
                {
                    EditorGUILayout.HelpBox("You need to set or calculate terrain max height in order to set the max height value for the curves.", MessageType.Error);
                }
            }
            EditorGUILayout.HelpBox("Max curve height sets the height at max curve value. For easiest control of the curves it should be set to just above max height in current terrain.", MessageType.Info);
            GUILayout.EndVertical();

            if (_terrainSystem.GetVegetationPackage().TerrainTextureCount > 0)
            {
                GUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("Select terrain texture", LabelStyle);
                GUIContent[] textureImageButtons = new GUIContent[_terrainSystem.GetVegetationPackage().TerrainTextureSettingsList.Count];
                for (int i = 0; i <= _terrainSystem.GetVegetationPackage().TerrainTextureSettingsList.Count - 1; i++)
                {

                    textureImageButtons[i] = new GUIContent { image = AssetPreviewCache.GetAssetPreview(_terrainSystem.GetTerrainTexture(i)) };

                    //var textureItemTexture = AssetPreview.GetAssetPreview(_terrainSystem.GetTerrainTexture(i));
                    //if (Application.isPlaying)
                    //{
                    //    textureImageButtons[i] = new GUIContent { image = textureItemTexture };
                    //}
                    //else
                    //{
                    //    Texture2D convertedTexture = new Texture2D(2, 2, TextureFormat.ARGB32, true, true);

                    //    if (textureItemTexture)
                    //    {
                    //        convertedTexture.LoadImage(textureItemTexture.EncodeToPNG());
                    //    }


                    //    textureImageButtons[i] = new GUIContent { image = convertedTexture };
                    //}


                    // _textureItemTexture;
                    //if (textureItemTexture == null) textureImageButtons[i].image = new Texture2D(80, 80);
                }
                int imageWidth = 80;
                int columns = Mathf.FloorToInt((EditorGUIUtility.currentViewWidth -50)/ imageWidth);
                int rows = Mathf.CeilToInt((float)textureImageButtons.Length / columns);
                int gridHeight = (rows) * imageWidth;
                _currentTextureItem = GUILayout.SelectionGrid(_currentTextureItem, textureImageButtons, columns, GUILayout.MaxWidth(columns * imageWidth), GUILayout.MaxHeight(gridHeight));
                GUILayout.EndVertical();

                if (_terrainSystem.VegetationSystem)
                {
                    GUILayout.BeginVertical("box");
                    EditorGUILayout.LabelField("Visualize height/steepness in terrain", LabelStyle);
                    EditorGUI.BeginChangeCheck();
                    bool overrideMaterial = EditorGUILayout.Toggle("Show heatmap", _terrainSystem.VegetationSystem.TerrainMaterialOverridden);
                    EditorGUILayout.HelpBox("Enabling heatmap will show the spawn area of the selected texture based on the current height and steepness curves.", MessageType.Info);

                    if (EditorGUI.EndChangeCheck())
                    {
                        if (overrideMaterial)
                        {
                            _terrainSystem.UpdateHeatmapMaterial(_currentTextureItem);
                            _terrainSystem.VegetationSystem.OverrideTerrainMaterial(_terrainSystem.TerrainHeatMapMaterial);
                            EditorUtility.SetDirty(_terrainSystem.VegetationSystem);
                        }
                        else
                        {
                            _terrainSystem.VegetationSystem.RestoreTerrainMaterial();
                        }
                    }
                    GUILayout.EndVertical();

                    _terrainSystem.UpdateHeatmapMaterial(_currentTextureItem);
                }

                //for (int i = 0; i <= terrainSystem.GetSettings().TerrainTextureSettingsList.Count - 1; i++)
                //{
                GUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("Terrain layer: " + (_currentTextureItem + 1).ToString(), LabelStyle);
                EditorGUI.BeginChangeCheck();

                _terrainSystem.GetVegetationPackage().TerrainTextureSettingsList[_currentTextureItem].Enabled = EditorGUILayout.Toggle("Use with auto splat generation", _terrainSystem.GetVegetationPackage().TerrainTextureSettingsList[_currentTextureItem].Enabled);

                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.ObjectField("Texture", _terrainSystem.GetTerrainTexture(_currentTextureItem), typeof(Texture2D), allowSceneObjects: false);
                EditorGUI.EndDisabledGroup();

               // EditorGUILayout.BeginHorizontal(variantStyle);
                EditorGUILayout.LabelField("Texture " + (_currentTextureItem + 1).ToString() + " Height", LabelStyle, GUILayout.Width(150));

                if (_heightCurveEditor.EditCurve(_terrainSystem.GetVegetationPackage().TerrainTextureSettingsList[_currentTextureItem].TextureHeightCurve, this))
                {
                    EditorUtility.SetDirty(_terrainSystem.GetVegetationPackage());
                }

                //_terrainSystem.GetVegetationPackage().TerrainTextureSettingsList[_currentTextureItem].TextureHeightCurve = EditorGUILayout.CurveField(_terrainSystem.GetVegetationPackage().TerrainTextureSettingsList[_currentTextureItem].TextureHeightCurve, Color.green, new Rect(0, 0, 1, 1), GUILayout.Height(75));
               // EditorGUILayout.EndHorizontal();

                //EditorGUILayout.BeginHorizontal(variantStyle);
                EditorGUILayout.LabelField("Texture " + (_currentTextureItem + 1).ToString() + " Steepness", LabelStyle, GUILayout.Width(150));
                if (_steepnessCurveEditor.EditCurve(_terrainSystem.GetVegetationPackage().TerrainTextureSettingsList[_currentTextureItem].TextureAngleCurve, this))
                {
                    EditorUtility.SetDirty(_terrainSystem.GetVegetationPackage());
                }

                //_terrainSystem.GetVegetationPackage().TerrainTextureSettingsList[_currentTextureItem].TextureAngleCurve = EditorGUILayout.CurveField(_terrainSystem.GetVegetationPackage().TerrainTextureSettingsList[_currentTextureItem].TextureAngleCurve, Color.green, new Rect(0, 0, 1, 1), GUILayout.Height(75));
                //EditorGUILayout.EndHorizontal();

                _terrainSystem.GetVegetationPackage().TerrainTextureSettingsList[_currentTextureItem].TextureUseNoise = EditorGUILayout.Toggle("Use perlin noise", _terrainSystem.GetVegetationPackage().TerrainTextureSettingsList[_currentTextureItem].TextureUseNoise);
                if (_terrainSystem.GetVegetationPackage().TerrainTextureSettingsList[_currentTextureItem].TextureUseNoise)
                {
                    _terrainSystem.GetVegetationPackage().TerrainTextureSettingsList[_currentTextureItem].TextureNoiseScale = EditorGUILayout.Slider("Noise tiling", _terrainSystem.GetVegetationPackage().TerrainTextureSettingsList[_currentTextureItem].TextureNoiseScale, 1, 50f);
                }
                _terrainSystem.GetVegetationPackage().TerrainTextureSettingsList[_currentTextureItem].TextureWeight = EditorGUILayout.Slider("Texture weight", _terrainSystem.GetVegetationPackage().TerrainTextureSettingsList[_currentTextureItem].TextureWeight, 0, 5f);

                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(_terrainSystem.GetVegetationPackage());
                }
                GUILayout.EndVertical();
            }         
        }
        void DrawSettingsInspector()
        {

            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Water/sea level: ", LabelStyle);
            EditorGUILayout.HelpBox("Selected terrain, Water level etc are set by the Vegetation system component.", MessageType.Info);

            GUILayout.EndVertical();

            //GUILayout.BeginVertical("box");
            //EditorGUILayout.LabelField("Settings object: ", LabelStyle);
            //if (_terrainSystem.GetSettings() == null)
            //{
            //    EditorGUILayout.HelpBox("You need to apply a settings object in order to use this component. Right click in  a project folder and select Create/AwesomeTechnologies/TerrainSystemSettings to create one. Then drag and drop to this component.", MessageType.Error);
            //}
            //TerrainSystemSettings currentSettings = EditorGUILayout.ObjectField("Current settings", _terrainSystem.GetSettings(), typeof(TerrainSystemSettings), true) as TerrainSystemSettings;
            //if (currentSettings != _terrainSystem.GetSettings())
            //{
            //    _terrainSystem.SetSettings(currentSettings);
            //    EditorUtility.SetDirty(target);
            //}
            //GUILayout.EndVertical();

            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Automatic update: ", LabelStyle);
            _terrainSystem.AutomaticApply = EditorGUILayout.Toggle("Update on Editor terrain changes", _terrainSystem.AutomaticApply);
            EditorGUILayout.HelpBox("With automatic update enabled any changes you do to the terrain heights with the Unity terrain editor will be updated with the new terrain texture distribution.", MessageType.Info);
            GUILayout.EndVertical();
        }
    }
}