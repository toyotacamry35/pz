using System.Collections.Generic;
using AwesomeTechnologies.VegetationStudio;
using UnityEditor;
using UnityEngine;

namespace AwesomeTechnologies
{
    [CustomEditor(typeof(VegetationPackage))]
    public class VegetationPackageEditor : VegetationStudioBaseEditor
    {
        private static readonly string[] NumberTextureStrings =
        {
            "No textures","4 textures", "8 textures", "12 textures", "16 textures"
        };

        private int _newTextureIndex;

        private VegetationPackage _vegetationPackage;
        private int _currentTextureItem;

        [MenuItem("Assets/Create/Awesome Technologies/VegetationPackage/No Textures")]
        public static void CreateYourScriptableObject()
        {
            VegetationPackage vegetationPakcage = ScriptableObjectUtility2.CreateAndReturnAsset<VegetationPackage>();
            vegetationPakcage.TerrainTextureCount = 0;
        }

        [MenuItem("Assets/Create/Awesome Technologies/VegetationPackage/4 terrain textures")]
        public static void CreateVegetationPackageObject4Textures()
        {
            VegetationPackage vegetationPakcage = ScriptableObjectUtility2.CreateAndReturnAsset<VegetationPackage>();
            vegetationPakcage.TerrainTextureCount = 4;
            vegetationPakcage.LoadDefaultTextures();
            vegetationPakcage.SetupTerrainTextureSettings();
        }

        [MenuItem("Assets/Create/Awesome Technologies/VegetationPackage/8 terrain textures")]
        public static void CreateVegetationPackageObject8Textures()
        {
            VegetationPackage vegetationPakcage = ScriptableObjectUtility2.CreateAndReturnAsset<VegetationPackage>();
            vegetationPakcage.TerrainTextureCount = 8;
            vegetationPakcage.LoadDefaultTextures();
            vegetationPakcage.SetupTerrainTextureSettings();
        }

        [MenuItem("Assets/Create/Awesome Technologies/VegetationPackage/12 terrain textures")]
        public static void CreateVegetationPackageObject12Textures()
        {
            VegetationPackage vegetationPakcage = ScriptableObjectUtility2.CreateAndReturnAsset<VegetationPackage>();
            vegetationPakcage.TerrainTextureCount = 12;
            vegetationPakcage.LoadDefaultTextures();
            vegetationPakcage.SetupTerrainTextureSettings();
        }

        [MenuItem("Assets/Create/Awesome Technologies/VegetationPackage/16 terrain textures")]
        public static void CreateVegetationPackageObject16Textures()
        {
            VegetationPackage vegetationPakcage = ScriptableObjectUtility2.CreateAndReturnAsset<VegetationPackage>();
            vegetationPakcage.TerrainTextureCount = 16;
            vegetationPakcage.LoadDefaultTextures();
            vegetationPakcage.SetupTerrainTextureSettings();
        }

        private int GetTerrainTextureIndex(VegetationPackage vegetationPackage)
        {
            switch (vegetationPackage.TerrainTextureCount)
            {
                case 0:
                    return 0;
                case 4:
                    return 1;
                case 8:
                    return 2;
                case 12:
                    return 3;
                case 16:
                    return 4;
            }

            return 0;
        }

        private int GetTerrainTextureCountFromIndex(int index)
        {
            switch (index)
            {
                case 0:
                    return 0;
                case 1:
                    return 4;
                case 2:
                    return 8;
                case 3:
                    return 12;
                case 4:
                    return 16;
            }

            return 0;
        }

        public void DropAreaGui()
        {
            Event evt = Event.current;
            Rect dropArea = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
            GUI.contentColor = Color.white;
            GUI.Box(dropArea, "To add multiple grass vegetation items drag and drop the Texture2D assets here");

            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!dropArea.Contains(evt.mousePosition))
                        return;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        foreach (Object draggedObject in DragAndDrop.objectReferences)
                        {
                            //Debug.Log(draggedObject.name);
                        }
                    }
                    break;
            }
        }

        private void ChangeTerrainTextureCount(VegetationPackage vegetationPackage, int newCount)
        {
            if (vegetationPackage.TerrainTextureCount == newCount) return;

            if (newCount > vegetationPackage.TerrainTextureCount)
            {
                _vegetationPackage.TerrainTextureCount = newCount;
                _vegetationPackage.LoadDefaultTextures();
                _vegetationPackage.SetupTerrainTextureSettings();
            }
            else
            {
                _vegetationPackage.TerrainTextureCount = newCount;
                _vegetationPackage.ResizeTerrainTextureList(newCount);
                _vegetationPackage.ResizeTerrainTextureSettingsList(newCount);
            }

            if (vegetationPackage.TerrainTextureCount == 0)
            {
                vegetationPackage.UseTerrainTextures = false;
            }
        }

        public void OnEnable()
        {
            _vegetationPackage = (VegetationPackage)target;
            _newTextureIndex = GetTerrainTextureIndex(_vegetationPackage);
        }

        public override void OnInspectorGUI()
        {
            HelpTopic = "vegetationpackage";
            IsScriptableObject = true;

            _vegetationPackage = (VegetationPackage)target;
            _vegetationPackage.InitPackage();

            base.OnInspectorGUI();

            GUILayout.BeginVertical("box");
            EditorGUILayout.HelpBox("Add this package to a Vegetation System component to use or set up", MessageType.Info);
            GUILayout.EndVertical();

            EditorGUI.BeginChangeCheck();
            GUILayout.BeginVertical("box");
            _vegetationPackage.PackageName = EditorGUILayout.TextField("Package name", _vegetationPackage.PackageName);         
            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");
            _vegetationPackage.UseTerrainTextures = EditorGUILayout.Toggle("Update terrain textures on init", _vegetationPackage.UseTerrainTextures);
           
            GUILayout.EndVertical();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(_vegetationPackage);
            }

            GUILayout.BeginVertical("box");
           
            _newTextureIndex = EditorGUILayout.Popup("Number of terrain textures", _newTextureIndex, NumberTextureStrings);
            if (GUILayout.Button("Change number of textures in package"))
            {
                ChangeTerrainTextureCount(_vegetationPackage,GetTerrainTextureCountFromIndex(_newTextureIndex));
                VegetationStudioManager.VegetationPackageSync_RefreshVegetationPackage(_vegetationPackage);
            }
            GUILayout.EndVertical();


            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Copy textures from terrain", LabelStyle);
            EditorGUI.BeginChangeCheck();
            Terrain newTerrain = EditorGUILayout.ObjectField("Source terrain", null, typeof(Terrain), true) as Terrain;
            TerrainData newTerrainData = EditorGUILayout.ObjectField("Source terrain data", null, typeof(TerrainData), true) as TerrainData;
            EditorGUILayout.HelpBox("The vegetation package must be set up to have at least the amount of textures the terrain has.", MessageType.Info);
            if (EditorGUI.EndChangeCheck())
            {
                if (newTerrain != null)
                {
                    _vegetationPackage.LoadTexturesFromTerrain(newTerrain.terrainData);
                }

                if (newTerrainData != null)
                {
                    _vegetationPackage.LoadTexturesFromTerrain(newTerrainData);
                }

                EditorUtility.SetDirty(_vegetationPackage);
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Vegetation items", LabelStyle);


            string selectedVegetationItem = "";
            List<string> vegetationItemIdList =
                VegetationPackageEditorTools.CreateVegetationInfoIdList(_vegetationPackage);
            VegetationPackageEditorTools.DrawVegetationItemSelector(_vegetationPackage, vegetationItemIdList, 60,
                ref selectedVegetationItem);

            if (_vegetationPackage.TerrainTextureCount > 0)
            {
                EditorGUILayout.LabelField("Terrain textures", LabelStyle);
                GUIContent[] textureImageButtons = new GUIContent[_vegetationPackage.TerrainTextureList.Count];
                for (int i = 0; i <= _vegetationPackage.TerrainTextureList.Count - 1; i++)
                {
                    var textureItemTexture = AssetPreview.GetAssetPreview(_vegetationPackage.TerrainTextureList[i].Texture);
                    Texture2D convertedTexture = new Texture2D(2, 2, TextureFormat.ARGB32, true, true);
                    if (textureItemTexture)
                    {
                        convertedTexture.LoadImage(textureItemTexture.EncodeToPNG());
                    }

                    textureImageButtons[i] = new GUIContent {image = convertedTexture};
                }
                int imageWidth = 60;
                int columns = Mathf.FloorToInt((EditorGUIUtility.currentViewWidth - imageWidth/2f) / imageWidth);
                int rows = Mathf.CeilToInt((float)textureImageButtons.Length / columns);
                int gridHeight = (rows) * imageWidth;
                _currentTextureItem = GUILayout.SelectionGrid(_currentTextureItem, textureImageButtons, columns, GUILayout.MaxWidth(columns * imageWidth), GUILayout.MaxHeight(gridHeight));

                GUIStyle variantStyle = new GUIStyle(EditorStyles.helpBox);

                GUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("Terrain layer: " + (_currentTextureItem + 1).ToString(), LabelStyle);
                EditorGUI.BeginDisabledGroup(true);
                _vegetationPackage.TerrainTextureSettingsList[_currentTextureItem].Enabled = EditorGUILayout.Toggle("Enable", _vegetationPackage.TerrainTextureSettingsList[_currentTextureItem].Enabled);

                EditorGUILayout.BeginHorizontal(variantStyle);
                EditorGUILayout.LabelField("Texture " + (_currentTextureItem + 1).ToString() + " Height", LabelStyle, GUILayout.Width(150));
                _vegetationPackage.TerrainTextureSettingsList[_currentTextureItem].TextureHeightCurve = EditorGUILayout.CurveField(_vegetationPackage.TerrainTextureSettingsList[_currentTextureItem].TextureHeightCurve, Color.green, new Rect(0, 0, 1, 1), GUILayout.Height(75));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal(variantStyle);
                EditorGUILayout.LabelField("Texture " + (_currentTextureItem + 1).ToString() + " Steepness", LabelStyle, GUILayout.Width(150));
                _vegetationPackage.TerrainTextureSettingsList[_currentTextureItem].TextureAngleCurve = EditorGUILayout.CurveField(_vegetationPackage.TerrainTextureSettingsList[_currentTextureItem].TextureAngleCurve, Color.green, new Rect(0, 0, 1, 1), GUILayout.Height(75));
                EditorGUILayout.EndHorizontal();

                _vegetationPackage.TerrainTextureSettingsList[_currentTextureItem].TextureUseNoise = EditorGUILayout.Toggle("Use perlin noise", _vegetationPackage.TerrainTextureSettingsList[_currentTextureItem].TextureUseNoise);
                if (_vegetationPackage.TerrainTextureSettingsList[_currentTextureItem].TextureUseNoise)
                {
                    _vegetationPackage.TerrainTextureSettingsList[_currentTextureItem].TextureNoiseScale = EditorGUILayout.Slider("Noise tiling", _vegetationPackage.TerrainTextureSettingsList[_currentTextureItem].TextureNoiseScale, 1, 50f);
                }
                _vegetationPackage.TerrainTextureSettingsList[_currentTextureItem].TextureWeight = EditorGUILayout.Slider("Texture weight", _vegetationPackage.TerrainTextureSettingsList[_currentTextureItem].TextureWeight, 0, 5f);

                EditorGUI.EndDisabledGroup();
                GUILayout.EndVertical();
            }

            GUILayout.EndVertical();
        }
    }
}