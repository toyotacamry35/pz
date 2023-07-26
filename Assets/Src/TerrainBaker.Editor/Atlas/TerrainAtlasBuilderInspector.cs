using Assets.ResourceSystem.Aspects.Effects;
using Assets.Src.Lib.ProfileTools;
using Assets.Src.ResourceSystem;
using Assets.Src.ResourceSystem.JdbRefs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Assets.TerrainBaker.Editor
{

    [CustomEditor(typeof(TerrainAtlasBuilder)), CanEditMultipleObjects]
    public class TerrainAtlasBuilderInspector : UnityEditor.Editor
    {

        private string errorString = null;

        private static bool isShowTextureParameters;
        private static bool isShowLayers;
        private static bool isDisableAutoUpdate;
        private Vector2 scrollPos;
        private GUIStyle textureFieldStyle;
        private GUIStyle headerFieldStyle;

        private struct ChangeDetector
        {
            public bool isChange { private set; get; }
            private float savedValue;

            public float Begin(float v)
            {
                savedValue = v;
                return v;
            }

            public float End(float v)
            {
                isChange |= (v != savedValue);
                return v;
            }

            public void MarkDirty(bool isDirty)
            {
                isChange |= isDirty;
            }
        }


        private void OnEnable()
        {


        }

        private void OnDisable()
        {

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            TerrainAtlasBuilder atlasBuilder = target as TerrainAtlasBuilder;
            if (atlasBuilder == null) return;

            GUILayout.BeginVertical();

            if (GUILayout.Button("Update Layers Parameters And Terrains"))
            {
                if (atlasBuilder.terrainAtlas != null)
                {
                    Build(atlasBuilder, true);
                    UpdateTerrains(atlasBuilder);
                }
                else
                {
                    errorString = "Terrain Atlas no set";
                }
            }

            if (GUILayout.Button("Build Atlas"))
            {
                Build(atlasBuilder, false);
                UpdateTerrains(atlasBuilder);
            }

            GUILayout.BeginVertical();
            EditorGUILayout.Space();
            if (!string.IsNullOrEmpty(errorString))
            {
                EditorGUILayout.HelpBox(errorString, MessageType.Error);
            }
            else
            {
                EditorGUILayout.HelpBox("No errors", MessageType.Info);
            }
            EditorGUILayout.Space();
            GUILayout.EndVertical();

            if (GUILayout.Button("Atlas texture info  " + (isShowTextureParameters ? "[-]" : "[+]")))
            {
                isShowTextureParameters = !isShowTextureParameters;
            }

            if (isShowTextureParameters)
            {
                GUILayout.BeginVertical();

                atlasBuilder.baseTextureSize = InputPowTwo("Base textures size (_DM, _NM, _EM)", atlasBuilder.baseTextureSize, atlasBuilder);
                atlasBuilder.parametersSize = InputPowTwo("Parameters (_AM) texture size", atlasBuilder.parametersSize, atlasBuilder);
                atlasBuilder.macroSize = InputPowTwo("Macro (_MM) texture size", atlasBuilder.macroSize, atlasBuilder);


                var newNamePrefixForTextureArrays = EditorGUILayout.TextField("Prefix for names of texture arrays", atlasBuilder.namePrefixForTextureArrays);
                if (atlasBuilder.namePrefixForTextureArrays != newNamePrefixForTextureArrays)
                {
                    Undo.RecordObject(atlasBuilder, "change name prefix in terrain atlas");
                    atlasBuilder.namePrefixForTextureArrays = newNamePrefixForTextureArrays;
                }
                var newTerrainLayersFolder = EditorGUILayout.TextField("Terrain layers folder", atlasBuilder.terrainLayersFolder);
                if (atlasBuilder.terrainLayersFolder != newTerrainLayersFolder)
                {
                    Undo.RecordObject(atlasBuilder, "change terrain layers folder in terrain atlas");
                    atlasBuilder.terrainLayersFolder = newTerrainLayersFolder;
                }

                GUILayout.EndVertical();
            }

            GUILayout.Space(10);
            if (GUILayout.Button("Layers  " + (isShowLayers ? "[-]" : "[+]")))
            {
                isShowLayers = !isShowLayers;
            }

            if (isShowLayers)
            {
                ChangeDetector changeDetect = new ChangeDetector();

                GUILayout.BeginVertical();

                GUIContent upButton = new GUIContent("Up", "Move up layer");
                GUIContent downButton = new GUIContent("Dn", "Move down layer");
                GUIContent delButton = new GUIContent("X", "Remove layer from terrain atlas");


                int leftSpace = 30;

                GUIStyle centerLabelStyle = new GUIStyle(GUI.skin.label);
                centerLabelStyle.alignment = TextAnchor.MiddleLeft;


                GUILayout.Space(5);
                GUILayout.BeginVertical(GUI.skin.box);
                isDisableAutoUpdate = !GUILayout.Toggle(!isDisableAutoUpdate, "Enable auto update parameters");
                GUILayout.EndVertical();
                GUILayout.Space(5);

                scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
                int moveUpIndex = -1;
                int moveDownIndex = -1;
                bool hasDeleted = false;
                GUILayout.BeginVertical();
                for (int i = 0; i < atlasBuilder.layersList.Count; i++)
                {
                    GUILayout.BeginVertical(GUI.skin.box);

                    GUILayout.BeginVertical(GUI.skin.textArea);
                    GUILayout.BeginHorizontal();

                    string header = "Layer: " + ((atlasBuilder.layersList[i].albedo != null) ? atlasBuilder.layersList[i].albedo.name : "empty (set textures)");
                    if (headerFieldStyle == null)
                    {
                        headerFieldStyle = new GUIStyle(GUI.skin.label);
                        headerFieldStyle.fontSize = 14;
                    }
                    GUILayout.Label(header, headerFieldStyle);

                    GUI.enabled = (i != 0);
                    if (GUILayout.Button(upButton, GUILayout.Width(30)))
                    {
                        moveUpIndex = i;
                        Repaint();
                    }
                    GUI.enabled = (i < atlasBuilder.layersList.Count - 1);
                    if (GUILayout.Button(downButton, GUILayout.Width(30)))
                    {
                        moveDownIndex = i;
                        Repaint();
                    }
                    GUI.enabled = true;
                    GUILayout.Space(30);
                    if (GUILayout.Button(delButton, GUILayout.Width(20)))
                    {
                        Undo.RecordObject(atlasBuilder, "Remove layer from terrain atlas");
                        atlasBuilder.layersList[i] = null;
                        hasDeleted = true;
                        Repaint();
                        continue;
                    }

                    GUILayout.EndHorizontal();

                    GUILayout.EndVertical();
                    TerrainAtlasBuilder.TerrainLayer layer = atlasBuilder.layersList[i];
                    GUILayout.Space(15);
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(leftSpace);
                    layer.albedo = TextureField("Albedo", layer.albedo, atlasBuilder);
                    GUILayout.Space(20);
                    layer.normals = TextureField("Normals", layer.normals, atlasBuilder);
                    GUILayout.Space(20);
                    layer.parameters = TextureField("Parameters", layer.parameters, atlasBuilder);
                    GUILayout.Space(20);
                    layer.emission = TextureField("Emission", layer.emission, atlasBuilder);
                    GUILayout.Space(20);
                    layer.macro = TextureField("Macro", layer.macro, atlasBuilder);
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                    //Albedo heights map scale
                    GUILayout.Space(25);
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(leftSpace);
                    GUILayout.Label("Albedo heights map scale: ");
                    float scaleMin = layer.albedoHeightMapOffset;
                    float scaleMax = changeDetect.Begin(layer.albedoHeightMapOffset + layer.albedoHeightMapScale);
                    EditorGUILayout.MinMaxSlider(ref scaleMin, ref scaleMax, 0.0f, 1.0f);
                    GUILayout.Space(15);
                    changeDetect.MarkDirty(layer.albedoHeightMapOffset != scaleMin);
                    layer.albedoHeightMapOffset = scaleMin;
                    layer.albedoHeightMapScale = changeDetect.End(scaleMax) - scaleMin;
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                    GUILayout.Space(5);
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(leftSpace);
                    GUILayout.Label("Layer relief factor: ");
                    GUILayout.Space(44);
                    layer.reliefFactor = changeDetect.End(EditorGUILayout.Slider(changeDetect.Begin(layer.reliefFactor), 0.0f, 1.0f));
                    GUILayout.Space(15);
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                    //Emission
                    if (layer.emission != null)
                    {
                        GUILayout.Space(25);
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(leftSpace);
                        GUILayout.Label("Emission tint: ", centerLabelStyle, GUILayout.Height(32));
                        GUILayout.Space(48);
                        Color newColor = EditorGUILayout.ColorField(new GUIContent(), layer.emissionTint, false, false, true, GUILayout.Width(50), GUILayout.Height(32));
                        changeDetect.MarkDirty(layer.emissionTint != newColor);
                        layer.emissionTint = newColor;
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                    }
                    //Tile size
                    GUILayout.Space(25);
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(leftSpace);
                    GUILayout.Label("Tile size in meters:");
                    GUILayout.Space(20);
                    layer.sizeInMeters = EditorGUILayout.FloatField(changeDetect.Begin(layer.sizeInMeters), GUILayout.Width(50));
                    if (layer.sizeInMeters < 1)
                    {
                        layer.sizeInMeters = 1;
                        Repaint();
                    }
                    if (layer.sizeInMeters > 1024)
                    {
                        layer.sizeInMeters = 1024;
                        Repaint();
                    }
                    changeDetect.End(layer.sizeInMeters);
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                    //Macro scale
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(leftSpace);
                    GUILayout.Label("Macro texture scale: ");
                    GUILayout.Space(8);
                    layer.macroScale = EditorGUILayout.FloatField(changeDetect.Begin(layer.macroScale), GUILayout.Width(50));
                    if (layer.macroScale < 0.001f)
                    {
                        layer.macroScale = 0.001f;
                        Repaint();
                    }
                    if (layer.macroScale > 1.0f)
                    {
                        layer.macroScale = 1.0f;
                        Repaint();
                    }
                    changeDetect.End(layer.macroScale);
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                    //Tile offset
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(leftSpace);
                    GUILayout.Label("Tile offset by X:");
                    GUILayout.Space(38);
                    layer.offsetX = changeDetect.End(EditorGUILayout.FloatField(changeDetect.Begin(layer.offsetX), GUILayout.Width(50)));
                    GUILayout.Space(10);
                    GUILayout.Label("by Z: ");
                    layer.offsetZ = changeDetect.End(EditorGUILayout.FloatField(changeDetect.Begin(layer.offsetZ), GUILayout.Width(50)));
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                    //Mip bias
                    GUILayout.Space(5);
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(leftSpace);
                    GUILayout.Label("Textures mip bias: ");
                    GUILayout.Space(18);
                    layer.mipBias = EditorGUILayout.FloatField(changeDetect.Begin(layer.mipBias), GUILayout.Width(50));
                    if (layer.mipBias < -8.0f)
                    {
                        layer.mipBias = -8.0f;
                        Repaint();
                    }
                    if (layer.mipBias > 8.0f)
                    {
                        layer.mipBias = 8.0f;
                        Repaint();
                    }
                    changeDetect.End(layer.mipBias);
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    //Layer extentions
                    GUILayout.Space(25);
                    //TerrainLayerFilling
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(leftSpace);
                    GUILayout.Label("Layer filling: ");
                    TerrainLayerFilling layerFilling = (TerrainLayerFilling)EditorGUILayout.ObjectField(layer.layerFilling, typeof(TerrainLayerFilling), false);
                    if (layer.layerFilling != layerFilling)
                    {
                        Undo.RecordObject(atlasBuilder, "Change layer filling in terrain atlas");
                        layer.layerFilling = layerFilling;
                    }
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                    //TerrainLayerFX
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(leftSpace);
                    GUILayout.Label("Layer FX: ");
                    GUILayout.Space(16);
                    JdbMetadata layerFXMeta = (JdbMetadata)EditorGUILayout.ObjectField(layer.layerFX.Metadata, typeof(JdbMetadata), allowSceneObjects: false);
                    if (layer.layerFX?.Metadata != layerFXMeta)
                    {
                        changeDetect.MarkDirty(true);
                        Undo.RecordObject(atlasBuilder, "Change layer FX in terrain atlas");
                        layer.layerFX = new FXStepMarkerDefRef() { Metadata = layerFXMeta };
                    }
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    GUILayout.Space(30);
                    GUILayout.EndVertical();
                }
                if (hasDeleted)
                {
                    atlasBuilder.layersList.RemoveAll(obj => obj == null);
                }
                else if (moveUpIndex >= 0)
                {
                    if (moveUpIndex > 0 && moveUpIndex < atlasBuilder.layersList.Count)
                    {
                        Undo.RecordObject(atlasBuilder, "Layer from terrain atlas move up");
                        Swap(atlasBuilder.layersList, moveUpIndex - 1, moveUpIndex);
                    }
                }
                else if (moveDownIndex >= 0)
                {
                    if (moveDownIndex >= 0 && moveDownIndex < atlasBuilder.layersList.Count - 1)
                    {
                        Undo.RecordObject(atlasBuilder, "Layer from terrain atlas move down");
                        Swap(atlasBuilder.layersList, moveDownIndex, moveDownIndex + 1);
                    }
                }
                if (GUILayout.Button(new GUIContent("+", "Add new layer to terrain atlas"), GUILayout.Height(40)))
                {
                    Undo.RecordObject(atlasBuilder, "Add new layer into terrain atlas");
                    atlasBuilder.layersList.Add(new TerrainAtlasBuilder.TerrainLayer());
                }
                GUILayout.Space(20);
                GUILayout.EndVertical();
                EditorGUILayout.EndScrollView();

                GUILayout.EndVertical();

                if (changeDetect.isChange)
                {
                    EditorUtility.SetDirty(atlasBuilder);
                    if (!isDisableAutoUpdate)
                    {
                        Build(atlasBuilder, true);
                        UpdateTerrains(atlasBuilder);
                    }
                }
            }

            GUILayout.EndVertical();
        }

        private void Swap(List<TerrainAtlasBuilder.TerrainLayer> list, int index1, int index2)
        {
            TerrainAtlasBuilder.TerrainLayer layer = list[index1];
            list[index1] = list[index2];
            list[index2] = layer;
        }

        private int InputPowTwo(string label, int value, TerrainAtlasBuilder atlasBuilder)
        {
            int newValue = EditorGUILayout.IntField(label, value);
            if (newValue != value)
            {
                if (newValue < 256 || newValue > 4096 || !Mathf.IsPowerOfTwo(newValue))
                {
                    Undo.RecordObject(atlasBuilder, "Change texture size in terrain atlas");
                    newValue = value;
                }
            }
            return newValue;
        }

        private Texture2D TextureField(string name, Texture2D texture, TerrainAtlasBuilder atlasBuilder)
        {
            GUIContent labelCnt = texture != null ? new GUIContent(name, texture.name) : new GUIContent(name);
            GUILayout.BeginVertical();
            if (textureFieldStyle == null)
            {
                textureFieldStyle = new GUIStyle(GUI.skin.label);
                textureFieldStyle.alignment = TextAnchor.UpperCenter;
                textureFieldStyle.fixedWidth = 70;
            }
            GUILayout.Label(labelCnt, textureFieldStyle);
            Texture2D result = (Texture2D)EditorGUILayout.ObjectField(texture, typeof(Texture2D), false, GUILayout.Width(70), GUILayout.Height(70));
            GUILayout.EndVertical();
            if (result != texture)
            {
                Undo.RecordObject(atlasBuilder, "Change texture in terrain atlas");
            }
            return result;
        }


        private float progressBarStep;
        private float progressBarValue;
        private void Build(TerrainAtlasBuilder atlasBuilder, bool skipCreateOrUpdateTextureArrays)
        {
            errorString = null;
            if (atlasBuilder.terrainAtlas == null)
            {
                errorString = "Terrain Atlas no set";
                return;
            }
            if (atlasBuilder.layersList == null || atlasBuilder.layersList.Count == 0)
            {
                errorString = "No textures for build";
                return;
            }

            if (!skipCreateOrUpdateTextureArrays)
            {
                EditorUtility.DisplayProgressBar("Build terrain atls", "Create texture arrays", 0.0f);
            }

            List<Texture2D> listAlbedo = CollectUniqueTextures(atlasBuilder.layersList, i => atlasBuilder.layersList[i].albedo);
            List<Texture2D> listNormals = CollectUniqueTextures(atlasBuilder.layersList, i => atlasBuilder.layersList[i].normals);
            List<Texture2D> listParameters = CollectUniqueTextures(atlasBuilder.layersList, i => atlasBuilder.layersList[i].parameters);
            List<Texture2D> listEmission = CollectUniqueTextures(atlasBuilder.layersList, i => atlasBuilder.layersList[i].emission);
            List<Texture2D> listMacro = CollectUniqueTextures(atlasBuilder.layersList, i => atlasBuilder.layersList[i].macro);

            if (listAlbedo == null || listNormals == null || listParameters == null)
            {
                errorString = "Build error: main list is empty";
                EditorUtility.ClearProgressBar();
                return;
            }

            if (!skipCreateOrUpdateTextureArrays)
            {
                int totalTextures = listAlbedo.Count + listNormals.Count + listParameters.Count;
                totalTextures += listEmission != null ? listEmission.Count : 0;
                totalTextures += listMacro != null ? listMacro.Count : 0;
                progressBarStep = totalTextures > 0 ? 0.5f / totalTextures : 0.5f;
                progressBarValue = 0.0f;

                Texture2DArray albedo = MakeTextureArray("Albebo", atlasBuilder.baseTextureSize, listAlbedo.Count, true, i => listAlbedo[i]);
                Texture2DArray normals = MakeTextureArray("Normals", atlasBuilder.baseTextureSize, listNormals.Count, false, i => listNormals[i]);
                Texture2DArray parameters = MakeTextureArray("Parameters", atlasBuilder.parametersSize, listParameters.Count, false, i => listParameters[i]);
                Texture2DArray emission = (listEmission != null) ? MakeTextureArray("Emission", atlasBuilder.baseTextureSize, listEmission.Count, true, i => listEmission[i]) : null;
                Texture2DArray macro = (listMacro != null) ? MakeTextureArray("Macro", atlasBuilder.macroSize, listMacro.Count, true, i => listMacro[i]) : null;

                if (albedo == null || normals == null || parameters == null)
                {
                    if (string.IsNullOrEmpty(errorString)) errorString += "\n";
                    if (albedo == null)
                    {                        
                        errorString += "Albedo texture array is not created\n";
                    }
                    if (normals == null)
                    {
                        errorString += "Normals texture array is not created\n";
                    }
                    if (parameters == null)
                    {
                        errorString += "Parameters texture array is not created\n";
                    }
                    EditorUtility.ClearProgressBar();
                    return;
                }

                string savePath;
                if (atlasBuilder.terrainAtlas != null)
                {
                    savePath = AssetDatabase.GetAssetPath(atlasBuilder.terrainAtlas.GetInstanceID());
                }
                else
                {
                    savePath = "Assets";
                }

                EditorUtility.DisplayProgressBar("Build terrain atls", "Update albedo texture array asset", 0.6f);

                savePath = Path.GetDirectoryName(savePath) + "/" + atlasBuilder.namePrefixForTextureArrays;
                string ext = ".asset";
                albedo = UpdateTexture2DArray(savePath + TerrainAtlasBuilder.albedoPostfix + ext, albedo);
                EditorUtility.DisplayProgressBar("Build terrain atls", "Update normals texture array asset", 0.7f);
                normals = UpdateTexture2DArray(savePath + TerrainAtlasBuilder.normalsPostfix + ext, normals);
                EditorUtility.DisplayProgressBar("Build terrain atls", "Update parameters texture array asset", 0.8f);
                parameters = UpdateTexture2DArray(savePath + TerrainAtlasBuilder.parametersPostfix + ext, parameters);
                if (emission != null)
                {
                    EditorUtility.DisplayProgressBar("Build terrain atls", "Update emission texture array asset", 0.85f);
                    emission = UpdateTexture2DArray(savePath + TerrainAtlasBuilder.emissionPostfix + ext, emission);
                }
                if (macro != null)
                {
                    EditorUtility.DisplayProgressBar("Build terrain atls", "Update macro texture array asset", 0.9f);
                    macro = UpdateTexture2DArray(savePath + TerrainAtlasBuilder.macroPostfix + ext, macro);
                }

                atlasBuilder.terrainAtlas.albedo = albedo;
                atlasBuilder.terrainAtlas.normals = normals;
                atlasBuilder.terrainAtlas.parameters = parameters;
                atlasBuilder.terrainAtlas.emission = emission;
                atlasBuilder.terrainAtlas.macro = macro;

                EditorUtility.DisplayProgressBar("Build terrain atls", "Update atlas data", 0.95f);
            }

            atlasBuilder.terrainAtlas.layers = new TerrainAtlas.Layer[atlasBuilder.layersList.Count];
            for (int i = 0; i < atlasBuilder.layersList.Count; i++)
            {
                TerrainAtlasBuilder.TerrainLayer src = atlasBuilder.layersList[i];
                TerrainAtlas.Layer dst = new TerrainAtlas.Layer();
                dst.albedoName = src.albedo.name;
                dst.sizeInMeters = Mathf.Max(src.sizeInMeters, 0.0f);
                dst.offsetX = src.offsetX;
                dst.offsetZ = src.offsetZ;
                dst.albedoIndex = i;
                dst.macroTilling = Mathf.Max(src.macroTilling, 0.0f);
                dst.macroScale = Mathf.Clamp(src.macroScale, 0.001f, 5.0f);
                dst.albedoIndex = listAlbedo.FindIndex(t => t == src.albedo);
                dst.normalsIndex = listNormals.FindIndex(t => t == src.normals);
                dst.parametersIndex = listParameters.FindIndex(t => t == src.parameters);
                dst.emissionIndex = (src.emission != null) ? listEmission.FindIndex(t => t == src.emission) : -1.0f;
                dst.macroIndex = (src.macro != null) ? listMacro.FindIndex(t => t == src.macro) : -1.0f;
                dst.albedoHeightMapScale = Mathf.Clamp01(src.albedoHeightMapScale);
                dst.albedoHeightMapOffset = Mathf.Clamp01(Mathf.Clamp01(src.albedoHeightMapScale + src.albedoHeightMapOffset) - src.albedoHeightMapScale);
                dst.reliefFactor = Mathf.Clamp01(src.reliefFactor);
                dst.emissionTint = src.emissionTint;
                dst.mipBias = src.mipBias;                
                dst.layerFX = src.layerFX;
                dst.layerFilling = src.layerFilling;
                atlasBuilder.terrainAtlas.layers[i] = dst;
            }
            EditorUtility.SetDirty(atlasBuilder.terrainAtlas);


            if (!skipCreateOrUpdateTextureArrays)
            {
                EditorUtility.DisplayProgressBar("Build terrain atls", "Finish!", 1.0f);
                EditorUtility.ClearProgressBar();
            }
        }

        private List<Texture2D> CollectUniqueTextures(List<TerrainAtlasBuilder.TerrainLayer> layersList, GetTextureForAtlas texGetter)
        {
            List<Texture2D> dict = null;
            for (int i = 0; i < layersList.Count; i++)
            {
                Texture2D tex = texGetter(i);
                if (tex != null)
                {
                    if (dict == null)
                    {
                        dict = new List<Texture2D>();
                    }
                    if (dict.FindIndex(t => t == tex) < 0)
                    {
                        dict.Add(tex);
                    }
                }
            }
            return dict;
        }

        private string AppendError(string current, string append)
        {
            if (current == null) return append;
            return current + "\n" + append;
        }

        private delegate Texture2D GetTextureForAtlas(int index);
        private Texture2DArray MakeTextureArray(string textureDesc, int textureSideSize, int layersCount, bool sRGB, GetTextureForAtlas texGetter)
        {
            Texture2D[] textures = new Texture2D[layersCount];
            for (int i = 0; i < layersCount; i++) textures[i] = texGetter(i);
            string errorStr = null;
            TextureFormat format = TextureFormat.ARGB32;
            for (int i = 0; i < textures.Length; i++)
            {
                if (textures[i] == null)
                {
                    errorStr = AppendError(errorStr, string.Format("{0} texture no set at index {1}.", textureDesc, i.ToString()));
                    continue;
                }
                if (i == 0) format = textures[0].format;
                if (textures[i].format != format)
                {
                    errorStr = AppendError(errorStr, string.Format("{0} texture {1} have different format.", textureDesc, textures[i].name));
                }
                if (textures[i].width != textureSideSize || textures[i].height != textureSideSize)
                {
                    errorStr = AppendError(errorStr, string.Format("{0} texture {1} have invalid size.", textureDesc, textures[i].name));
                    continue;
                }
                if ((1 << (textures[i].mipmapCount - 1)) != textureSideSize)
                {
                    errorStr = AppendError(errorStr, string.Format("{0} texture {1} have invalid mipmaps count.", textureDesc, textures[i].name));
                    continue;
                }
            }
            if (errorStr != null)
            {
                errorString = AppendError(errorString, errorStr);
                return null;
            }
            Texture2DArray atlas = new Texture2DArray(textureSideSize, textureSideSize, textures.Length, textures[0].format, true, !sRGB);
            atlas.filterMode = FilterMode.Trilinear;
            for (int i = 0; i < textures.Length; i++)
            {
                Graphics.CopyTexture(textures[i], 0, atlas, i);
                progressBarValue += progressBarStep;
                EditorUtility.DisplayProgressBar("Build terrain atls", "Create texture arrays", progressBarValue);
            }
            atlas.Apply(false);
            return atlas;
        }

        Texture2DArray UpdateTexture2DArray(string path, Texture2DArray texArray)
        {
            Texture2DArray savedArray = (Texture2DArray)AssetDatabase.LoadAssetAtPath(path, typeof(Texture2DArray));
            if (savedArray != null)
            {
                EditorUtility.CopySerialized(texArray, savedArray);
            }
            else
            {
                AssetDatabase.CreateAsset(texArray, path);
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            }
            return (Texture2DArray)AssetDatabase.LoadAssetAtPath(path, typeof(Texture2DArray));
        }

        private void UpdateTerrains(TerrainAtlasBuilder atlasBuilder)
        {
            atlasBuilder.terrainAtlas.Update();
            TerrainBaker[] terrainBakers = Profile.FindObjectsOfTypeAll<TerrainBaker>();
            foreach (TerrainBaker bk in terrainBakers)
            {
                bk.ForceUpdate(false);
                EditorSceneManager.MarkSceneDirty(bk.gameObject.scene);
            }
            EditorApplication.QueuePlayerLoopUpdate();
            SceneView.RepaintAll();
        }
    }
}
