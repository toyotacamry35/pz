using Assets.Src.Lib.ProfileTools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Assets.TerrainBaker.Editor
{
    class TerrainBakerUtilites : EditorWindow
    {
        private static string atlasBuilderPath;
        private static List<Texture2D> sysTextures;
        private TerrainAtlasBuilder atlasBuilder;
        private Terrain[] allTerrains;
        private const int pageUtils = 0;
        private const int pageTextures = 1;
        private int currentPage = 0;


        [MenuItem("Level Design/Terrain baker utilites")]
        static void Init()
        {
            GetWindow(typeof(TerrainBakerUtilites), true, "TerrainBaker Utilites");
        }


        private void OnEnable()
        {
            minSize = new Vector2(800, 700);
            LoadAtlasByName();
        }

        private void OnDisable()
        {
            terrainsTextures = null;
        }


        private void FindAtlas()
        {
            if (atlasBuilder == null)
            {
                LoadAtlasByName();
                if (atlasBuilder != null)
                {
                    return;
                }
                string[] guids = AssetDatabase.FindAssets("TerrainAtlasBuilder");
                if (guids.Length == 1)
                {
                    atlasBuilderPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                    atlasBuilder = AssetDatabase.LoadAssetAtPath<TerrainAtlasBuilder>(atlasBuilderPath);
                    if (atlasBuilder != null)
                    {
                        return;
                    }
                }
                guids = AssetDatabase.FindAssets("t:TerrainAtlasBuilder");
                if (guids.Length == 1)
                {
                    atlasBuilderPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                    atlasBuilder = AssetDatabase.LoadAssetAtPath<TerrainAtlasBuilder>(atlasBuilderPath);
                }
            }
        }

        private void LoadAtlasByName()
        {
            if (!string.IsNullOrEmpty(atlasBuilderPath))
            {
                atlasBuilder = AssetDatabase.LoadAssetAtPath<TerrainAtlasBuilder>(atlasBuilderPath);
            }
        }

        private void Refresh()
        {
            FindAtlas();
            allTerrains = Profile.FindObjectsOfTypeAll<Terrain>();
            ReadTerrainsTextures(allTerrains);
            indexTerrainTexturesTexture = -1;
            indexTerrainTextures = -1;
            terrainBakersErrorString = null;
        }

        private void OnGUI()
        {
            GUILayout.BeginVertical();
            GUILayout.Space(20);

            TerrainAtlasBuilder prevAtlasBuilder = atlasBuilder;
            atlasBuilder = EditorGUILayout.ObjectField("Terrain Atlas Builder", atlasBuilder, typeof(TerrainAtlasBuilder), false, GUILayout.Width(450)) as TerrainAtlasBuilder;
            if (prevAtlasBuilder != atlasBuilder)
            {
                if (atlasBuilder != null)
                {
                    atlasBuilderPath = AssetDatabase.GetAssetPath(atlasBuilder);
                    if (allTerrains != null)
                    {
                        ReadTerrainsTextures(allTerrains);
                    }
                }
                else
                {
                    atlasBuilderPath = null;
                    terrainsTextures = null;
                }
            }
            string infoMessage = CheckAtlas();

            if (infoMessage != null)
            {
                GUILayout.Space(40);
                EditorGUILayout.HelpBox(infoMessage, MessageType.Warning);
            }
            GUILayout.Space(40);
            if (GUILayout.Button("Refresh data", GUILayout.Height(32)))
            {
                Refresh();
            }
            if (allTerrains != null)
            {
                if (GUILayout.Button("Force update terrain runtime data", GUILayout.Height(32)))
                {
                    ForceUpdateBakers();
                }
                GUILayout.Space(20);
                GUIContent[] toolbar = new GUIContent[] { new GUIContent("Utilites"), new GUIContent("Textures") };
                currentPage = GUILayout.Toolbar(currentPage, toolbar);
                GUILayout.Space(20);
                switch (currentPage)
                {
                    case pageUtils:
                        Utils();
                        break;
                    case pageTextures:
                        Textures();
                        break;
                }
            }
            GUILayout.Space(20);
            GUILayout.EndVertical();
        }

        private string CheckAtlas()
        {
            string infoMessage = null;

            if (atlasBuilder == null)
            {
                infoMessage = "Select TerrainAtlasBuilder before start work";
            }
            else if (atlasBuilder.terrainAtlas == null || atlasBuilder.terrainAtlas.albedo == null)
            {
                infoMessage = "Build atlas in TerrainAtlasBuilder before start work";
            }
            else if (atlasBuilder.layersList.Count != atlasBuilder.terrainAtlas.layers.Length)
            {
                infoMessage = "Rebuild atlas in TerrainAtlasBuilder before start work";
            }
            else
            {
                for (int i = 0; i < atlasBuilder.layersList.Count; i++)
                {
                    if (atlasBuilder.layersList[i].albedo == null)
                    {
                        infoMessage = "Fix and build atlas in TerrainAtlasBuilder before start work";
                    }
                }
            }
            return infoMessage;
        }


        private class TerrainTextureInfo
        {
            public int atlasIndex = -1;
            public Texture2D sysTexture;
        }

        private class TerrainTextures
        {
            public string name;
            public List<TerrainTextureInfo> textures;
            public Terrain terrain;
        };
        List<TerrainTextures> terrainsTextures = null;
        private int indexTerrainTextures = -1;
        private int indexTerrainTexturesTexture = -1;

        void ReadTerrainsTextures(Terrain[] terrains)
        {
            if (sysTextures == null)
            {
                sysTextures = new List<Texture2D>();
            }
            if (terrainsTextures == null)
            {
                terrainsTextures = new List<TerrainTextures>();
            }
            if (terrainsTextures.Count != terrains.Length)
            {
                terrainsTextures.Clear();
                terrainsTextures.AddRange(new TerrainTextures[terrains.Length]);
            }
            if (atlasBuilder == null)
            {
                indexTerrainTextures = -1;
                indexTerrainTexturesTexture = -1;
                return;
            }
            if (indexTerrainTextures > terrains.Length)
            {
                indexTerrainTextures = -1;
                indexTerrainTexturesTexture = -1;
            }
            for (int i = 0; i < terrains.Length; i++)
            {
                if (terrainsTextures[i] == null)
                {
                    terrainsTextures[i] = new TerrainTextures();
                }
                terrainsTextures[i].name = terrains[i].name;
                ReadTerrainTextures(terrainsTextures[i], terrains[i]);
            }
            if (indexTerrainTextures >= 0)
            {
                if (indexTerrainTexturesTexture >= terrainsTextures[indexTerrainTextures].textures.Count)
                {
                    indexTerrainTexturesTexture = -1;
                }
            }
        }

        private void ReadTerrainTextures(TerrainTextures terrainTextures, Terrain terrain)
        {
            TerrainLayer[] processedTerrainLayers = terrain.terrainData.terrainLayers;
            terrainTextures.terrain = terrain;
            if (terrainTextures.textures == null)
            {
                terrainTextures.textures = new List<TerrainTextureInfo>();
            }
            terrainTextures.textures.Clear();
            for (int j = 0; j < processedTerrainLayers.Length; j++)
            {
                TerrainTextureInfo info = new TerrainTextureInfo();
                TerrainLayer terrainLayer = processedTerrainLayers[j];
                if (terrainLayer.diffuseTexture != null)
                {
                    string name = terrainLayer.diffuseTexture.name;
                    if (name.StartsWith(TerrainBaker.sysLayerNamePrefix))
                    {
                        Texture2D sysTexture = sysTextures.Find(obj => obj.name == name);
                        if (sysTexture == null)
                        {
                            sysTexture = terrainLayer.diffuseTexture;
                            sysTextures.Add(sysTexture);
                        }
                        info.sysTexture = sysTexture;
                    }
                    else
                    {
                        info.atlasIndex = atlasBuilder.layersList.FindIndex(obj => obj.albedo != null && obj.albedo.name == name);
                    }
                }
                terrainTextures.textures.Add(info);
            }
        }

        const int imageSizeAtlas = 96;
        const int imageSizeTerrains = 64;
        const int borderSize = 4;
        const int terrainLabelWidth = 200;
        Vector2 scrollPos;
        void Textures()
        {
            GUILayout.BeginVertical();
            GUILayout.Space(10);
            int layersCount = atlasBuilder.layersList.Count;
            int columns = Mathf.Max((int)((position.width - imageSizeAtlas * 0.5f) / imageSizeAtlas), 4);

            EditorGUILayout.LabelField("Layers from " + atlasBuilder.terrainAtlas.name);
            GUILayout.Space(10);
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            if (sysTextures == null)
            {
                sysTextures = new List<Texture2D>();
            }
            int allTexturesCount = atlasBuilder.layersList.Count;// + sysTextures.Count;
            for (int i = 0, curCol = 0; i < allTexturesCount; i++, curCol++)
            {
                if (curCol >= columns)
                {
                    curCol = 0;
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                }

                Texture2D previewTexture;
                //if (i < atlasBuilder.layersList.Count)
                //{
                    previewTexture = atlasBuilder.layersList[i].albedo;
                //}
                //else
                //{
                //    previewTexture = sysTextures[i - atlasBuilder.layersList.Count];
                //}
                string layerAlbedoName = previewTexture.name;
                previewTexture = AssetPreview.GetAssetPreview(previewTexture) ?? previewTexture;

                GUILayout.BeginVertical();

                if (indexTerrainTextures >= 0 && indexTerrainTexturesTexture >= 0)
                {
                    if (GUILayout.Button(new GUIContent() { image = previewTexture, tooltip = layerAlbedoName }, GUILayout.Width(imageSizeAtlas), GUILayout.Height(imageSizeAtlas)))
                    {
                        terrainsTextures[indexTerrainTextures].textures[indexTerrainTexturesTexture].atlasIndex = i;
                    }
                }
                else
                {
                    GUILayout.Box(new GUIContent() { image = previewTexture, tooltip = layerAlbedoName }, GUILayout.Width(imageSizeAtlas), GUILayout.Height(imageSizeAtlas));
                }

                GUILayout.Label(layerAlbedoName, GUILayout.Width(imageSizeAtlas));
                GUILayout.EndVertical();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(20);

            GUILayout.Label("Terrains");
            GUILayout.Space(10);
            GUILayout.BeginVertical();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(position.width));
            GUILayout.BeginVertical();
            if (terrainsTextures != null && terrainsTextures.Count > 0)
            {
                GUIStyle labelStyle = new GUIStyle();
                labelStyle.alignment = TextAnchor.MiddleCenter;
                for (int i = 0; i < terrainsTextures.Count; i++)
                {
                    GUILayout.BeginHorizontal(EditorStyles.helpBox);
                    GUIContent label = new GUIContent();
                    label.text = terrainsTextures[i].name;
                    label.tooltip = terrainsTextures[i].name;
                    GUILayout.BeginVertical();
                    GUILayout.Space(20);
                    GUILayout.Label(label, labelStyle, GUILayout.Width(terrainLabelWidth));
                    GUILayout.Space(4);
                    if (terrainsTextures[i].textures.FindIndex(obj => obj.atlasIndex < 0 && obj.sysTexture == null) < 0)
                    {
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button("Set for this"))
                        {
                            ForceSetMaterialsToTerrain(terrainsTextures[i].terrain, terrainsTextures[i].textures);
                        }
                        if (GUILayout.Button("Set for all"))
                        {
                            if (EditorUtility.DisplayDialog("Replace all textures in selected terrains",
                                                        "Are you sure you want to replace textures from current terrain to other?",
                                                        "Replace", "Cancel"))
                            {
                                for (int j = 0; j < terrainsTextures.Count; j++)
                                {
                                    ForceSetMaterialsToTerrain(terrainsTextures[j].terrain, terrainsTextures[i].textures);
                                }
                                ReadTerrainsTextures(allTerrains);
                                Repaint();
                            }
                        }
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.FlexibleSpace();
                    GUILayout.EndVertical();
                    List<TerrainTextureInfo> textures = terrainsTextures[i].textures;
                    for (int j = 0; j < textures.Count; j++)
                    {
                        GUIContent content = new GUIContent();
                        if (textures[j].atlasIndex >= 0)
                        {
                            int layerIndex = textures[j].atlasIndex;
                            Texture2D previewTexture = atlasBuilder.layersList[layerIndex].albedo;
                            previewTexture = AssetPreview.GetAssetPreview(previewTexture) ?? previewTexture;
                            content.image = previewTexture;
                            content.tooltip = atlasBuilder.layersList[layerIndex].albedo.name;
                        }
                        else if (textures[j].sysTexture != null)
                        {
                            Texture2D previewTexture = textures[j].sysTexture;
                            previewTexture = AssetPreview.GetAssetPreview(previewTexture) ?? previewTexture;
                            content.image = previewTexture;
                            content.tooltip = textures[j].sysTexture.name;
                        }
                        else
                        {
                            content.image = Texture2D.blackTexture;
                            content.tooltip = "Empty";
                        }
                        if (indexTerrainTextures == i && indexTerrainTexturesTexture == j)
                        {
                            GUILayout.BeginVertical(EditorStyles.helpBox);
                            GUILayout.Box(content, GUI.skin.button, GUILayout.Width(imageSizeTerrains), GUILayout.Height(imageSizeTerrains));
                            GUILayout.EndVertical();
                        }
                        else
                        {
                            GUILayout.BeginVertical();
                            GUILayout.Space(10);
                            if (GUILayout.Toggle(false, content, GUI.skin.box, GUILayout.Width(imageSizeTerrains - 8), GUILayout.Height(imageSizeTerrains - 8)))
                            {
                                indexTerrainTextures = i;
                                indexTerrainTexturesTexture = j;
                            }
                            GUILayout.Space(10);
                            GUILayout.EndVertical();
                        }
                    }

                    GUILayout.BeginVertical();
                    GUILayout.Space(10);
                    if (GUILayout.Button("+"))
                    {
                        if (indexTerrainTextures == i)
                        {
                            if (indexTerrainTexturesTexture >= 0)
                            {
                                terrainsTextures[i].textures.Insert(indexTerrainTexturesTexture, new TerrainTextureInfo());
                            }
                            else
                            {
                                terrainsTextures[i].textures.Add(new TerrainTextureInfo());
                                indexTerrainTexturesTexture = terrainsTextures[i].textures.Count - 1;
                            }
                        }
                        else
                        {
                            terrainsTextures[i].textures.Add(new TerrainTextureInfo());
                        }
                    }
                    if (indexTerrainTextures == i && indexTerrainTexturesTexture >= 0)
                    {
                        if (GUILayout.Button("-"))
                        {
                            if (indexTerrainTexturesTexture >= 0)
                            {
                                if (indexTerrainTexturesTexture > terrainsTextures[i].textures.Count - 1)
                                {
                                    indexTerrainTexturesTexture = terrainsTextures[i].textures.Count - 1;
                                }
                                terrainsTextures[indexTerrainTextures].textures.RemoveAt(indexTerrainTexturesTexture);
                                if (indexTerrainTexturesTexture > terrainsTextures[i].textures.Count - 1)
                                {
                                    indexTerrainTexturesTexture = terrainsTextures[i].textures.Count - 1;
                                }
                            }
                        }
                    }
                    GUILayout.Space(10);
                    GUILayout.EndVertical();

                    if (indexTerrainTextures == i && indexTerrainTexturesTexture >= 0)
                    {
                        GUILayout.BeginVertical();
                        GUILayout.Space(20);
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button("<"))
                        {
                            if (indexTerrainTexturesTexture > 0)
                            {
                                TerrainTextureInfo tmp = terrainsTextures[i].textures[indexTerrainTexturesTexture - 1];
                                terrainsTextures[i].textures[indexTerrainTexturesTexture - 1] = terrainsTextures[i].textures[indexTerrainTexturesTexture];
                                terrainsTextures[i].textures[indexTerrainTexturesTexture] = tmp;
                                indexTerrainTexturesTexture--;
                                Repaint();
                            }
                        }
                        if (GUILayout.Button(">"))
                        {
                            if (indexTerrainTexturesTexture < terrainsTextures[i].textures.Count - 1)
                            {
                                TerrainTextureInfo tmp = terrainsTextures[i].textures[indexTerrainTexturesTexture + 1];
                                terrainsTextures[i].textures[indexTerrainTexturesTexture + 1] = terrainsTextures[i].textures[indexTerrainTexturesTexture];
                                terrainsTextures[i].textures[indexTerrainTexturesTexture] = tmp;
                                indexTerrainTexturesTexture++;
                                Repaint();
                            }
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.Space(15);
                        GUILayout.EndVertical();
                    }

                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUILayout.EndVertical();
        }

        private TerrainLayer FindTerrainLayerInAssets(Texture2D diffuseTexture, Texture2D normalMapTexture)
        {
            var terrainLayerAssetFiePaths = Directory.GetFiles(atlasBuilder.terrainLayersFolder, "*.terrainlayer", SearchOption.TopDirectoryOnly);
            foreach(var terrainLayerAssetFiePath in terrainLayerAssetFiePaths)
            {
                var terrainLayer = AssetDatabase.LoadAssetAtPath<TerrainLayer>(terrainLayerAssetFiePath);
                if (terrainLayer != null && (terrainLayer.diffuseTexture == diffuseTexture) && (terrainLayer.normalMapTexture == normalMapTexture))
                {
                    return terrainLayer;
                }
            }
            return null;
        }

        private string GetNewTerrainLayerPath(string name)
        {
            var tries = 0;
            var max_tries = 100;
            var result = $"{atlasBuilder.terrainLayersFolder}/layer_{name}_{tries:00}.terrainlayer";
            while (File.Exists(result) && tries < max_tries)
            {
                ++tries;
                result = $"{atlasBuilder.terrainLayersFolder}/layer_{name}_{tries:00}.terrainlayer";
            }
            return result;
        }
        
        private TerrainLayer UpdateOrCreateTerrainLayer(TerrainLayer[] terrainLayers, Texture2D diffuseTexture, Texture2D normalMapTexture, Vector2 tileOffset, Vector2 tileSize)
        {
            foreach(var terrainLayer in terrainLayers)
            {
                if ((terrainLayer.diffuseTexture == diffuseTexture) && (terrainLayer.normalMapTexture == normalMapTexture))
                {
                    terrainLayer.tileOffset = tileOffset;
                    if ((terrainLayer.tileOffset != tileOffset) || (terrainLayer.tileSize != tileSize))
                    {
                        terrainLayer.tileOffset = tileOffset;
                        terrainLayer.tileSize = tileSize;
                        EditorUtility.SetDirty(terrainLayer);
                    }
                    return terrainLayer;
                }
            }
            var newTerrainLayer = FindTerrainLayerInAssets(diffuseTexture, normalMapTexture);
            var createAsset = false;
            if (newTerrainLayer == null)
            {
                newTerrainLayer = new TerrainLayer();
                createAsset = true;
            }
            var newTerrainLayerFileName = diffuseTexture.name;
            newTerrainLayer.diffuseTexture = diffuseTexture;
            newTerrainLayer.normalMapTexture = normalMapTexture;
            newTerrainLayer.tileOffset = tileOffset;
            newTerrainLayer.tileSize = tileSize;
            if (createAsset)
            {
                var newTerrainLayerPath = GetNewTerrainLayerPath(newTerrainLayerFileName);
                if (!Directory.Exists(atlasBuilder.terrainLayersFolder))
                {
                    Directory.CreateDirectory(atlasBuilder.terrainLayersFolder);
                }
                AssetDatabase.CreateAsset(newTerrainLayer, newTerrainLayerPath);
                newTerrainLayer = AssetDatabase.LoadAssetAtPath<TerrainLayer>(newTerrainLayerPath);
            }
            else
            {
                EditorUtility.SetDirty(newTerrainLayer);
            }
            return newTerrainLayer;
        }

        private void ForceSetMaterialsToTerrain(Terrain terrain, List<TerrainTextureInfo> selLayers)
        {
            var processedTerrainLayers = new TerrainLayer[selLayers.Count];
            for (int j = 0; j < selLayers.Count; j++)
            {
                TerrainLayer terrainLayer = null;
                if (selLayers[j].atlasIndex >= 0)
                {
                    int layerIndex = selLayers[j].atlasIndex;
                    TerrainAtlasBuilder.TerrainLayer layer = atlasBuilder.layersList[layerIndex];
                    terrainLayer = UpdateOrCreateTerrainLayer(terrain.terrainData.terrainLayers, layer.albedo, layer.normals, new Vector2(layer.offsetX, layer.offsetZ), new Vector2(layer.sizeInMeters, layer.sizeInMeters));
                }
                else
                {
                    terrainLayer = UpdateOrCreateTerrainLayer(terrain.terrainData.terrainLayers, selLayers[j].sysTexture, null, Vector2.zero, new Vector2(10, 10));
                }
                processedTerrainLayers[j] = terrainLayer;
            }
            terrain.terrainData.terrainLayers = processedTerrainLayers;
            EditorUtility.SetDirty(terrain.terrainData);
            EditorUtility.SetDirty(terrain);
            EditorUtility.SetDirty(terrain.gameObject);
            EditorSceneManager.MarkSceneDirty(terrain.gameObject.scene);
        }

        private long terrainBakersAssigned = 0;
        private int terrainBakersAssignedCount = 0;
        private string terrainBakersErrorString = null;

        private void Utils()
        {
            if (allTerrains == null)
            {
                return;
            }
            GUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label("Convert terrains");
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            if (GUILayout.Button("Set or update all terrain bakers and links", GUILayout.Height(32)))
            {
                Refresh();
                EditorUtility.DisplayProgressBar("Set or update terrain bakers and links", "Create ", 0.0f);
                terrainBakersErrorString = null;
                Terrain[,] terrains = PrepareTerrains(allTerrains);
                if (terrains != null)
                {
                    float terrainsCount = terrains.GetLength(0) * terrains.GetLength(1);
                    terrainsCount = (terrainsCount > 0.0) ? 1.0f / terrainsCount : 0.0f;

                    for (int i = 0, count = 0; i < terrains.GetLength(0); i++)
                    {
                        for (int j = 0; j < terrains.GetLength(1); j++, count++)
                        {
                            if (terrains[i, j] == null)
                            {
                                continue;
                            }
                            EditorUtility.DisplayProgressBar("Set or update terrain bakers and links", "Create Assets", count * terrainsCount);
                            GameObject obj = terrains[i, j].gameObject;
                            TerrainBaker baker = obj.GetComponent<TerrainBaker>();
                            if (baker == null)
                            {
                                baker = obj.AddComponent<TerrainBaker>();
                                baker.atlas = atlasBuilder.terrainAtlas;
                            }
                            if (baker != null)
                            {
                                terrains[i, j].enabled = false;
                                baker.enableUpdatesFromFixedTerrainFormer = IsPresentClass(baker.gameObject, "TerrainFormer");
                                baker.ForceUpdate(true);
                                EditorUtility.SetDirty(terrains[i, j]);
                                EditorUtility.SetDirty(baker);
                                EditorUtility.SetDirty(obj);
                                EditorSceneManager.MarkSceneDirty(obj.scene);
                            }
                        }
                    }
                    int sizeI = terrains.GetLength(0);
                    int sizeJ = terrains.GetLength(1);
                    for (int i = 0; i < sizeI; i++)
                    {
                        for (int j = 0; j < sizeJ; j++)
                        {
                            if (j < sizeJ - 1)
                            {
                                CheckOrCreateLink(terrains[i, j], terrains[i, j + 1], Side.maxX);
                            }
                            if (i < sizeI - 1)
                            {
                                CheckOrCreateLink(terrains[i, j], terrains[i + 1, j], Side.maxZ);
                            }
                        }
                    }
                    terrainBakersAssignedCount = terrains.Length;
                    if (terrains.Length == 0) terrainBakersAssignedCount = -1;
                    terrainBakersAssigned = DateTime.Now.Ticks;
                }
                EditorUtility.DisplayProgressBar("Set or update terrain bakers and links", "Create Assets", 1.0f);
                AssetDatabase.Refresh();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                EditorUtility.ClearProgressBar();
            }
            GUILayout.Space(20);
            if (GUILayout.Button("Check and normalize terrain alphamaps", GUILayout.Height(32)))
            {
                terrainBakersErrorString = "";
                bool isChangeTerrains = false;
                for (int i = 0; i < allTerrains.Length; i++)
                {
                    if (allTerrains[i] != null && allTerrains[i].terrainData != null && allTerrains[i].terrainData.alphamapLayers > 0)
                    {
                        int layersCount = allTerrains[i].terrainData.alphamapLayers;
                        if (layersCount == 0)
                        {
                            continue;
                        }
                        int w = allTerrains[i].terrainData.alphamapWidth;
                        int h = allTerrains[i].terrainData.alphamapHeight;
                        bool[] sysLayers = new bool[layersCount];
                        TerrainLayer[] protos = allTerrains[i].terrainData.terrainLayers;
                        if (protos.Length == layersCount)
                        {
                            for (int j = 0; j < layersCount; j++)
                            {
                                sysLayers[j] = (protos[j].diffuseTexture != null && protos[j].diffuseTexture.name.StartsWith(TerrainBaker.sysLayerNamePrefix));
                            }
                        }
                        float[,,] alphamaps = allTerrains[i].terrainData.GetAlphamaps(0, 0, w, h);
                        bool isChange = false;
                        for (int y = 0; y < h; y++)
                        {
                            for (int x = 0; x < w; x++)
                            {
                                float sum = 0.0f;
                                for (int layer = 0; layer < layersCount; layer++)
                                {
                                    if (sysLayers[layer])
                                    {
                                        alphamaps[y, x, layer] = (alphamaps[y, x, layer] > 1e-5f) ? 0.01f : 0.0f;
                                    }
                                    sum += alphamaps[y, x, layer];
                                }
                                if (Mathf.Abs(sum - 1.0f) > 0.01f)
                                {
                                    isChange = true;
                                    if (sum > 0.01f)
                                    {
                                        for (int layer = 0; layer < layersCount; layer++)
                                        {
                                            alphamaps[y, x, layer] /= sum;
                                        }
                                    }
                                    else
                                    {
                                        alphamaps[y, x, 0] = 1.0f;
                                        for (int layer = 1; layer < layersCount; layer++)
                                        {
                                            alphamaps[y, x, layer] = 0.0f;
                                        }
                                    }
                                }
                            }
                        }
                        if (isChange)
                        {
                            isChangeTerrains = true;
                            allTerrains[i].terrainData.SetAlphamaps(0, 0, alphamaps);
                            if (!string.IsNullOrEmpty(terrainBakersErrorString))
                            {
                                terrainBakersErrorString += "\n";
                            }
                            terrainBakersErrorString += "Fixed terrain " + allTerrains[i].name;
                            if (allTerrains[i].name != allTerrains[i].terrainData.name)
                            {
                                terrainBakersErrorString += ", terrainData(" + allTerrains[i].terrainData.name + ")";
                            }
                        }
                    }
                }
                if (isChangeTerrains)
                {
                    ForceUpdateBakers();
                }
                if (!string.IsNullOrEmpty(terrainBakersErrorString))
                {
                    terrainBakersAssigned = DateTime.Now.Ticks;
                }
                else
                {
                    terrainBakersErrorString = null;
                }
            }
            GUILayout.Space(20);
            GUILayout.EndHorizontal();
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            if (GUILayout.Button("Remove all terrain bakers", GUILayout.Height(32)))
            {
                Refresh();
                if (EditorUtility.DisplayDialog("Remove components",
                            "Are you sure you want to delete all TerrainBakers?",
                            "Delete", "Cancel"))
                {
                    RemoveAllTerrainBakers(allTerrains);
                }
            }
            GUILayout.Space(20);
            if (GUILayout.Button("Remove all baker's resources", GUILayout.Height(32)))
            {                
                Refresh();
                if (EditorUtility.DisplayDialog("No UNDO!",
                            "Are you sure you want to delete all TerrainBaker's resources?",
                            "Delete", "Cancel"))
                {
                    RemoveAllTerrainBakerResources(allTerrains);
                }
            }
            GUILayout.Space(20);
            GUILayout.EndHorizontal();
            if (string.IsNullOrEmpty(terrainBakersErrorString))
            {
                if (terrainBakersAssigned != 0)
                {
                    if (DateTime.Now.Ticks - terrainBakersAssigned < 30000000)
                    {
                        GUILayout.Space(20);
                        if (terrainBakersAssignedCount > 0)
                        {
                            EditorGUILayout.HelpBox(string.Format("Terrain Baker on {0} objects", terrainBakersAssignedCount.ToString()), MessageType.Info);
                        }
                        else
                        {
                            EditorGUILayout.HelpBox("No child objects with terrain for set Terrain Baker", MessageType.Warning);
                        }
                    }
                    else
                    {
                        terrainBakersAssigned = 0;
                        terrainBakersAssignedCount = 0;
                    }
                }
            }
            else
            {
                EditorGUILayout.HelpBox(terrainBakersErrorString, MessageType.Error);
            }
            GUILayout.Space(20);
            GUILayout.EndVertical();

            GUILayout.Space(20);

            GUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label("System textures");
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            if (GUILayout.Button("Set enable draw", GUILayout.Height(32)))
            {
                Refresh();
                ForceUpdateBakers();
                ProcessBakers(allTerrains, obj => obj.enableDrawSystemTextures = true);
            }
            GUILayout.Space(10);
            if (GUILayout.Button("Set disable draw", GUILayout.Height(32)))
            {
                Refresh();
                ProcessBakers(allTerrains, obj => obj.enableDrawSystemTextures = false);
            }
            GUILayout.Space(40);
            if (GUILayout.Button("Set enable update holes", GUILayout.Height(32)))
            {
                Refresh();
                ProcessBakers(allTerrains, obj => obj.enableHolesUpdate = true);
            }
            GUILayout.Space(10);
            if (GUILayout.Button("Set disable update holes", GUILayout.Height(32)))
            {
                Refresh();
                ProcessBakers(allTerrains, obj => obj.enableHolesUpdate = false);
            }
            GUILayout.Space(20);
            GUILayout.EndHorizontal();
            GUILayout.Space(20);
            GUILayout.EndVertical();

            GUILayout.Space(20);

            GUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label("Switch draw");
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            if (GUILayout.Button("Render by TerrainBakers", GUILayout.Height(32)))
            {
                Refresh();
                ProcessBakers(allTerrains, obj =>
                {
                    MeshRenderer mr = obj.GetComponent<MeshRenderer>();
                    if (mr != null) mr.enabled = true;
                    obj.terrain.enabled = false;
                });
            }
            GUILayout.Space(20);
            if (GUILayout.Button("Render by Unity Terrains", GUILayout.Height(32)))
            {
                Refresh();
                ProcessBakers(allTerrains, obj =>
                {
                    MeshRenderer mr = obj.GetComponent<MeshRenderer>();
                    if (mr != null) mr.enabled = false;
                    obj.terrain.enabled = true;
                });
            }
            GUILayout.Space(20);
            GUILayout.EndHorizontal();
            GUILayout.Space(20);
            GUILayout.EndVertical();

            GUILayout.Space(20);

            GUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label("TerrainBakers update method");
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            if (GUILayout.Button("Update from modified TerrainFormer", GUILayout.Height(32)))
            {
                Refresh();
                ProcessBakers(allTerrains, obj => { obj.enableUpdatesFromFixedTerrainFormer = true; obj.enableUpdateEveryFrame = false; });
            }
            GUILayout.Space(20);
            if (GUILayout.Button("Heuristic update", GUILayout.Height(32)))
            {
                Refresh();
                ProcessBakers(allTerrains, obj => { obj.enableUpdatesFromFixedTerrainFormer = false; obj.enableUpdateEveryFrame = false; });
            }
            GUILayout.Space(20);
            if (GUILayout.Button("All time update", GUILayout.Height(32)))
            {
                Refresh();
                ProcessBakers(allTerrains, obj => { obj.enableUpdatesFromFixedTerrainFormer = false; obj.enableUpdateEveryFrame = true; });
            }
            GUILayout.Space(20);
            GUILayout.EndHorizontal();
            GUILayout.Space(20);
            GUILayout.EndVertical();
        }

        private bool IsPresentClass(GameObject gObj, string className)
        {
            //Transform[] transforms = gObj.GetComponentsInParent<Transform>();
            //foreach (Transform tf in transforms)
            {
                MonoBehaviour[] components = gObj.GetComponents<MonoBehaviour>();
                foreach (MonoBehaviour cmp in components)
                {
                    string fullName = cmp.GetType().FullName;
                    if (fullName.EndsWith(className))
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        private class TerrainPosition
        {
            public Vector2Int pos;
            public Terrain terrain;
        }

        private Terrain[,] PrepareTerrains(Terrain[] terrains)
        {
            terrainBakersErrorString = null;
            List<TerrainPosition> terrainPositions = new List<TerrainPosition>();
            int heightmapSize = terrains[0].terrainData.heightmapResolution;
            int alphamapSize = terrains[0].terrainData.alphamapWidth;
            float heightScale = terrains[0].terrainData.size.y;
            float heightPosition = terrains[0].transform.position.y;
            Vector2Int minPos = Vector2Int.zero;
            Vector2Int maxPos = Vector2Int.zero;
            for (int i = 0; i < terrains.Length; i++)
            {
                string terrainErrors = TerrainBaker.CheckTerrainData(terrains[i].terrainData);
                if (string.IsNullOrEmpty(terrainErrors))
                {
                    if (terrains[i].terrainData.heightmapResolution != heightmapSize || terrains[i].terrainData.alphamapWidth != alphamapSize)
                    {
                        terrainErrors = (terrainErrors == null) ? "" : terrainErrors + "\n";
                        terrainErrors += "Terrain have difference heightmap size";
                    }
                    if (terrains[i].terrainData.alphamapWidth != alphamapSize)
                    {
                        terrainErrors = (terrainErrors == null) ? "" : terrainErrors + "\n";
                        terrainErrors += "Terrain have difference alphamap size";
                    }
                    const float eps = 1e-20f;
                    if (Mathf.Abs(terrains[i].terrainData.size.y - heightScale) > eps)
                    {
                        terrainErrors = (terrainErrors == null) ? "" : terrainErrors + "\n";
                        terrainErrors += "Terrain have difference Height";
                    }
                    if (string.IsNullOrEmpty(terrainErrors))
                    {
                        Vector3 fPos = terrains[i].transform.position / (heightmapSize - 1.0f);
                        TerrainPosition tp = new TerrainPosition();
                        tp.pos = new Vector2Int((int)fPos.x, (int)fPos.z);
                        tp.terrain = terrains[i];
                        if (terrains.Length > 1)
                        {
                            if (Mathf.Abs(fPos.x - tp.pos.x) > eps || Mathf.Abs(fPos.z - tp.pos.y) > eps)
                            {
                                terrainErrors = (terrainErrors == null) ? "" : terrainErrors + "\n";
                                terrainErrors += "Terrain position not aling by size";
                            }
                        }
                        if (Mathf.Abs(terrains[i].transform.position.y - heightPosition) > eps)
                        {
                            terrainErrors = (terrainErrors == null) ? "" : terrainErrors + "\n";
                            terrainErrors += "Terrain y position not equal with other";
                        }
                        if (string.IsNullOrEmpty(terrainErrors))
                        {
                            int idx = terrainPositions.FindIndex(obj => obj.pos == tp.pos);
                            if (idx < 0)
                            {
                                terrainPositions.Add(tp);
                                if (i > 0)
                                {
                                    minPos = Vector2Int.Min(minPos, tp.pos);
                                    maxPos = Vector2Int.Max(maxPos, tp.pos);
                                }
                                else
                                {
                                    minPos = tp.pos;
                                    maxPos = tp.pos;
                                }
                            }
                            else
                            {
                                terrainErrors = (terrainErrors == null) ? "" : terrainErrors + "\n";
                                terrainErrors += "Terrain have position same " + terrainPositions[idx].terrain.name;
                            }
                        }
                    }
                }
                if (!string.IsNullOrEmpty(terrainErrors))
                {
                    if (!string.IsNullOrEmpty(terrainBakersErrorString)) terrainBakersErrorString += "\n";
                    terrainBakersErrorString += "Terrain " + terrains[i].name + " error\n" + terrainErrors;
                }
            }
            Vector2Int size = maxPos - minPos + Vector2Int.one;
            if (size.x > 256 || size.x > 256)
            {
                if (!string.IsNullOrEmpty(terrainBakersErrorString)) terrainBakersErrorString += "\n";
                terrainBakersErrorString += "Terrains place very large area, more than 256 elements per axis";
            }
            if (!string.IsNullOrEmpty(terrainBakersErrorString))
            {
                return null;
            }
            Terrain[,] terrain = new Terrain[size.y, size.x];
            for (int i = 0; i < terrainPositions.Count; i++)
            {
                Vector2Int pos = terrainPositions[i].pos - minPos;
                terrain[pos.y, pos.x] = terrainPositions[i].terrain;
            }
            return terrain;
        }

        enum Side
        {
            minX,
            minZ,
            maxX,
            maxZ
        }

        private static readonly char[] SceneNumbersDelimiters = new char[] { '_', '.' };
        private static Vector3Int TryToGetCoordinates(string sceneName)
        {
            var result = new Vector3Int(0, 0, 0);
            if (!string.IsNullOrEmpty(sceneName))
            {
                var segments = sceneName.Split(SceneNumbersDelimiters, StringSplitOptions.RemoveEmptyEntries);
                if (segments.Length > 1)
                {
                    var coordinatesAquired = 0;
                    for (var index = 1; index < segments.Length; ++index)
                    {
                        var coordinate = segments[index][0];
                        var value = segments[index].Substring(1);
                        if (coordinate == 'x' || coordinate == 'X')
                        {
                            int _value;
                            if (int.TryParse(value, out _value))
                            {
                                result.x = _value;
                                ++coordinatesAquired;
                            }
                        }
                        else if (coordinate == 'y' || coordinate == 'Y')
                        {
                            int _value;
                            if (int.TryParse(value, out _value))
                            {
                                result.y = _value;
                                ++coordinatesAquired;
                            }
                        }
                        else if (coordinate == 'z' || coordinate == 'Z')
                        {
                            int _value;
                            if (int.TryParse(value, out _value))
                            {
                                result.z = _value;
                                ++coordinatesAquired;
                            }
                        }
                    }
                }
            }
            return result;
        }

        private static int Vector3IntCompare(Vector3Int left, Vector3Int right)
        {
            var result = left.x.CompareTo(right.x);
            if (result == 0)
            {
                result = left.z.CompareTo(right.z);
            }
            return result;
        }

        private void CheckOrCreateLink(Terrain t1, Terrain t2, Side t1Link)
        {
            if (t1 == null || t2 == null || t1.terrainData == null || t2.terrainData == null)
            {
                return;
            }
            GameObject obj1 = t1.gameObject;
            TerrainBaker b1 = obj1.GetComponent<TerrainBaker>();
            GameObject obj2 = t2.gameObject;
            TerrainBaker b2 = obj2.GetComponent<TerrainBaker>();
            if (b1 == null || b2 == null || b1 == b2)
            {
                return;
            }
            switch (t1Link)
            {
                case Side.minX:
                    if (b1.linkMinX != null && b1.linkMinX == b2.linkMaxX)
                    {
                        return;
                    }
                    break;
                case Side.minZ:
                    if (b1.linkMinZ != null && b1.linkMinZ == b2.linkMaxZ)
                    {
                        return;
                    }
                    break;
                case Side.maxX:
                    if (b1.linkMaxX != null && b1.linkMaxX == b2.linkMinX)
                    {
                        return;
                    }
                    break;
                case Side.maxZ:
                    if (b1.linkMaxZ != null && b1.linkMaxZ == b2.linkMinZ)
                    {
                        return;
                    }
                    break;
            }
            string path1 = AssetDatabase.GetAssetOrScenePath(t1.terrainData).Replace('\\', '/');
            string path2 = AssetDatabase.GetAssetOrScenePath(t2.terrainData).Replace('\\', '/');
            path1 = path1.Substring(0, path1.LastIndexOf('/'));
            path2 = path2.Substring(0, path2.LastIndexOf('/'));
            string terrainDataPath1 = path1.Substring(0, path1.LastIndexOf('/'));
            string terrainDataPath2 = path2.Substring(0, path2.LastIndexOf('/'));
            if (terrainDataPath1.CompareTo(terrainDataPath2) != 0)
            {
                return;
            }
            string sceneName1 = path1.Substring(path1.LastIndexOf('/') + 1);
            string sceneName2 = path2.Substring(path2.LastIndexOf('/') + 1); 
            var scene1Coordinates = TryToGetCoordinates(sceneName1);
            var scene2Coordinates = TryToGetCoordinates(sceneName2);
            string fromSceneName;
            string toSceneName;
            if (Vector3IntCompare(scene1Coordinates, scene2Coordinates) <= 0)
            {
                fromSceneName = sceneName1;
                toSceneName = sceneName2;
            }
            else
            {
                fromSceneName = sceneName2;
                toSceneName = sceneName1;
            }
            var craterPath = terrainDataPath1.Substring(0, terrainDataPath1.LastIndexOf('/'));
            var linkName = $"From_{fromSceneName}_To_{toSceneName}";
            string path = $"{craterPath}/TerrainLinks/{linkName}_tb_link.asset";
            TerrainBakerLink link = AssetDatabase.LoadAssetAtPath<TerrainBakerLink>(path);
            if (link == null)
            {
                link = ScriptableObject.CreateInstance<TerrainBakerLink>();
                //link.name = linkName; //AUTOMATIC NAME
                AdaptiveMeshBuilder.InitLinkData(link, t1.terrainData.heightmapResolution);
                AssetDatabase.CreateAsset(link, path);
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                link = AssetDatabase.LoadAssetAtPath<TerrainBakerLink>(path);
            }
            if (link.heightmapSize != t1.terrainData.heightmapResolution)
            {
                AdaptiveMeshBuilder.InitLinkData(link, t1.terrainData.heightmapResolution);
                EditorUtility.SetDirty(link);
            }
            switch (t1Link)
            {
                case Side.minX:
                    b1.linkMinX = link;
                    b2.linkMaxX = link;
                    break;
                case Side.minZ:
                    b1.linkMinZ = link;
                    b2.linkMaxZ = link;
                    break;
                case Side.maxX:
                    b1.linkMaxX = link;
                    b2.linkMinX = link;
                    break;
                case Side.maxZ:
                    b1.linkMaxZ = link;
                    b2.linkMinZ = link;
                    break;
            }
        }

        private void ForceUpdateBakers()
        {
            TerrainBaker[] terrainBakers = Profile.FindObjectsOfTypeAll<TerrainBaker>();
            foreach (TerrainBaker bk in terrainBakers)
            {
                bk.ForceUpdate(false);
                EditorSceneManager.MarkSceneDirty(bk.gameObject.scene);
            }
            EditorApplication.QueuePlayerLoopUpdate();
            SceneView.RepaintAll();
        }

        private delegate void BakerProcessor(TerrainBaker baker);
        private void ProcessBakers(Terrain[] terrains, BakerProcessor func)
        {
            for (int i = 0; i < terrains.Length; i++)
            {
                if (terrains[i] != null)
                {
                    TerrainBaker baker = terrains[i].GetComponent<TerrainBaker>();
                    if (baker != null)
                    {
                        func(baker);
                        EditorUtility.SetDirty(baker);
                    }
                }
            }
            SceneView.RepaintAll();
        }

        private void RemoveAllTerrainBakers(Terrain[] terrains)
        {
            float kProgress = terrains.Length > 0 ? 1.0f / terrains.Length : 0;
            for (int i = 0; i < terrains.Length; i++)
            {
                if (terrains[i] == null)
                {
                    continue;
                }
                EditorUtility.DisplayProgressBar("Delete TerrainBaker components", "Delete from " + terrains[i].name, i * kProgress);
                GameObject gObj = terrains[i].gameObject;
                if (gObj == null)
                {
                    continue;
                }
                Undo.RecordObject(gObj, gObj.name + " before delete TerrainBaker");
                DestroyImmediate(gObj.GetComponent<TerrainBaker>());
                DestroyImmediate(gObj.GetComponent<TerrainBakerMaterialSupport>());
                DestroyImmediate(gObj.GetComponent<MeshRenderer>());
                DestroyImmediate(gObj.GetComponent<MeshFilter>());
                Terrain t = gObj.GetComponent<Terrain>();
                if (t != null)
                {
                    t.enabled = true;
                }
            }
            EditorUtility.ClearProgressBar();
        }

        private void RemoveAllTerrainBakerResources(Terrain[] terrains)
        {
            float kProgress = terrains.Length > 0 ? 1.0f / terrains.Length : 0;
            for (int i = 0; i < terrains.Length; i++)
            {
                float progress = i * kProgress;
                string[] guids = AssetDatabase.FindAssets(terrains[i].name + "*_tb_*");
                float kProgressInTerrain = guids.Length > 0 ? kProgress / guids.Length : 0;
                for (int j = 0; j < guids.Length; j++)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[j]);
                    EditorUtility.DisplayProgressBar("Delete TerrainBaker resources", "Delete " + path, progress + j * kProgressInTerrain);
                    AssetDatabase.DeleteAsset(path);
                }
            }
            EditorUtility.ClearProgressBar();
        }
    }
}
