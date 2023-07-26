using Assets.Src.Shared;
using Assets.TerrainBaker;
using System;
using System.Collections.Generic;
using System.IO;
using Core.Environment.Logging.Extension;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Src.Cartographer.Editor
{
    public class CaptureScreenshotEditorParams
    {
        public float PixelsPerMeter { get; set; } = 1.0f;
        public float Angle { get; set; } = 0.0f;
        public int Border { get; set; } = 0;
        public bool Heightmap { get; set; } = false;
        public bool Rainbow { get; set; } = true;
        public bool Red { get; set; } = true;
        public bool Green { get; set; } = true;
        public bool Blue { get; set; } = true;
        public bool TerrainOnly { get; set; } = false;
    }

    public class CaptureScreenshotEditorWindow : EditorWindow
    {
        private static bool makeScreenshotFromEditor = false;
        private static char[] delimiters = new char[] { ' ' };
        private static Color[] rainbowColors = new Color[]
        {
            new Color(1.0f, 0.0f, 0.0f), 
            new Color(1.0f, 0.5f, 0.0f),
            new Color(1.0f, 1.0f, 0.0f),
            new Color(0.5f, 1.0f, 0.0f),
            new Color(0.0f, 1.0f, 0.0f),
            new Color(0.0f, 1.0f, 0.5f),
            new Color(0.0f, 1.0f, 1.0f),
            new Color(0.0f, 0.5f, 1.0f),
            new Color(0.0f, 0.0f, 1.0f),
            new Color(0.5f, 0.0f, 1.0f),
            new Color(1.0f, 0.0f, 1.0f),
            new Color(1.0f, 0.0f, 0.5f),
        };

        private CaptureScreenshotEditorParams captureScreenshotEditorParams = new CaptureScreenshotEditorParams();

        // private methods ------------------------------------------------------------------------
        private static void MoveEditorCameraByScreenshot()
        {
            string filePath = EditorUtility.OpenFilePanel("Please choose Screenshot", CaptureScreenshotUtils.GetScreenshotFolder(), "jpg");
            string cameraString = string.Empty;
            if (File.Exists(filePath))
            {
                var file = File.Open(filePath, FileMode.Open);
                var reader = new BinaryReader(file);
                if (reader.BaseStream.Length > 2)
                {
                    var signature = reader.ReadUInt16();
                    if (signature == 0xD8FF)
                    {
                        while (reader.BaseStream.Position != reader.BaseStream.Length)
                        {
                            var sectionSignature = reader.ReadUInt16();
                            if (sectionSignature == 0xD9FF)
                            {
                                break;
                            }
                            int length = reader.ReadByte() << 8;
                            length += reader.ReadByte();
                            if (sectionSignature == 0xFEFF)
                            {
                                var chars = reader.ReadChars(length - 2);
                                cameraString = new string(chars);
                                break;
                            }
                            else
                            {
                                reader.ReadChars(length - 2);
                            }
                        }
                    }
                }
                reader.Close();
                file.Close();
            }
            var segments = cameraString.Split(delimiters);
            var values = new List<float>();
            for (int index = 0; index < segments.Length; ++index)
            {
                float value = 0.0f;
                if (float.TryParse(segments[index], out value))
                {
                    values.Add(value);
                }
                else
                {
                    break;
                }
            }
            if (values.Count > 5)
            {
                var position = new Vector3(values[0], values[1], values[2]);
                var rotation = Quaternion.Euler(values[3], values[4], values[5]);
                position += rotation * Vector3.forward * SceneView.lastActiveSceneView.cameraDistance;
                SceneView.lastActiveSceneView.pivot = position;
                SceneView.lastActiveSceneView.rotation = rotation;
                SceneView.lastActiveSceneView.Repaint();
                CaptureScreenshotUtils.Logger.IfInfo()?.Message($"Camera moved by:\t{filePath}").Write();
            }
        }

        private static bool MakeCameraScreenshot(UnityEngine.Camera renderCamera, int width, int height, string filePath, bool makeCameraInfo)
        {
            if (renderCamera == null) { return false; }
            var cameraInfo = makeCameraInfo ? CaptureScreenshotUtils.CreateCameraInfo(renderCamera) : string.Empty;

            RenderTexture renderTexture = new RenderTexture(width, height, 24);
            var oldMainCameraTargetTexture = renderCamera.targetTexture;
            renderCamera.targetTexture = renderTexture;
            renderCamera.Render();
            Texture2D texure = new Texture2D(width, height);
            var oldRenderTextureActive = RenderTexture.active;
            RenderTexture.active = renderTexture;
            texure.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            var bytes = makeCameraInfo ? texure.EncodeToJPG(100) : texure.EncodeToPNG();
            RenderTexture.active = oldRenderTextureActive;
            renderCamera.targetTexture = oldMainCameraTargetTexture;
            DestroyImmediate(texure);
            renderTexture.Release();

            return CaptureScreenshotUtils.SaveScreenshot(filePath, bytes, cameraInfo);
        }

        private static void MakeEditorScreenshot()
        {
            var renderCamera = SceneView.lastActiveSceneView.camera;
            var width = Screen.currentResolution.width;
            var height = Screen.currentResolution.height;
            var filePath = CaptureScreenshotUtils.GetUniqueScreenshotPath(true, ".jpg");
            if (MakeCameraScreenshot(renderCamera, width, height, filePath, true))
            {
                CaptureScreenshotUtils.Logger.IfInfo()?.Message($"Screenshot taken:\t{filePath}").Write();
            }
        }

        private static Dictionary<MeshRenderer, UnityEngine.Rendering.ShadowCastingMode> SwitchShadowsOff()
        {
            var previousValues = new Dictionary<MeshRenderer, UnityEngine.Rendering.ShadowCastingMode>();
            var terrainObjects = FindObjectsOfType<TerrainBakerMaterialSupport>();
            foreach (var terrainObject in terrainObjects)
            {
                var meshRenderrer = terrainObject.GetComponent<MeshRenderer>();
                if (meshRenderrer != null)
                {
                    previousValues.Add(meshRenderrer, meshRenderrer.shadowCastingMode);
                    meshRenderrer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                }
            }
            return previousValues;
        }

        private static void RestoreShadows(Dictionary<MeshRenderer, UnityEngine.Rendering.ShadowCastingMode> meshRenderrers)
        {
            foreach (var meshRenderrer in meshRenderrers)
            {
                if (meshRenderrer.Key != null)
                {
                    meshRenderrer.Key.shadowCastingMode = meshRenderrer.Value;
                }
            }
        }

        public static Bounds GetTerrainBounds()
        {
            var result = new Bounds();
            bool notInitialised = true;

            var terrains = Terrain.activeTerrains;
            foreach (var terrain in terrains)
            {
                if (terrain.terrainData != null)
                {
                    var terrainBounds = terrain.terrainData.bounds;
                    terrainBounds.center += terrain.transform.position;
                    if (notInitialised)
                    {
                        result = terrainBounds;
                        notInitialised = false;
                    }
                    else
                    {
                        result.Encapsulate(terrainBounds);
                    }
                }
            }

            var terrainObjects = FindObjectsOfType<TerrainBakerMaterialSupport>();
            foreach (var terrainObject in terrainObjects)
            {
                var meshRenderrer = terrainObject.GetComponent<MeshRenderer>();
                if (meshRenderrer != null)
                {
                    var terrainBounds = meshRenderrer.bounds;
                    if (notInitialised)
                    {
                        result = terrainBounds;
                        notInitialised = false;
                    }
                    else
                    {
                        result.Encapsulate(terrainBounds);
                    }
                }
            }

            if (notInitialised)
            {
                var sceneCount = SceneManager.sceneCount;
                for (var sceneIndex = 0; sceneIndex < sceneCount; ++sceneIndex)
                {
                    var scene = SceneManager.GetSceneAt(sceneIndex);
                    if (scene.isLoaded)
                    {
                        var rootObjects = CartographerCommon.GetRootGameObjects(scene);
                        if (rootObjects != null)
                        {
                            foreach (var rootObject in rootObjects)
                            {
                                var children = CartographerCommon.GetChildren(rootObject);
                                if (children != null)
                                {
                                    foreach (var child in children)
                                    {
                                        if (child.name.Equals("TerrainMesh"))
                                        {
                                            var meshRenderrer = child.GetComponent<MeshRenderer>();
                                            if (meshRenderrer != null)
                                            {
                                                var terrainBounds = meshRenderrer.bounds;
                                                if (notInitialised)
                                                {
                                                    result = terrainBounds;
                                                    notInitialised = false;
                                                }
                                                else
                                                {
                                                    result.Encapsulate(terrainBounds);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }

        public static Vector2Int GetMinimapScreenshotSize(CaptureScreenshotEditorParams captureScreenshotEditorParams)
        {
            var terrainBounds = GetTerrainBounds();
            var aspect = Mathf.Cos(captureScreenshotEditorParams.Angle * Mathf.Deg2Rad);
            return new Vector2Int((int)(captureScreenshotEditorParams.Border * 2.0f + terrainBounds.extents.x * 2.0f * captureScreenshotEditorParams.PixelsPerMeter),
                                  (int)(captureScreenshotEditorParams.Border * 2.0f + terrainBounds.extents.z * 2.0f * captureScreenshotEditorParams.PixelsPerMeter * aspect));
        }

        private static Color GetRainbowColor(float factor, float minFactor, float multiplier)
        {
            var reach = (factor - minFactor) * multiplier;
            var index = (int)reach;
            return Color.Lerp(rainbowColors[index], rainbowColors[index + 1], reach - index);
        }

        public static void MakeMinimapScreenshots(CaptureScreenshotEditorParams captureScreenshotEditorParams)
        {
            if (captureScreenshotEditorParams.Heightmap)
            {
                var terrainBounds = GetTerrainBounds();
                var width = (int)(terrainBounds.extents.x * 2.0f * captureScreenshotEditorParams.PixelsPerMeter);
                var height = (int)(terrainBounds.extents.z * 2.0f * captureScreenshotEditorParams.PixelsPerMeter);
                var mapHeights = new float[width * height];
                var minHeight = float.MaxValue;
                var maxHeight = -float.MaxValue;

                var start = new Vector2(terrainBounds.min.x, terrainBounds.min.z);
                var step = new Vector2(terrainBounds.extents.x / width, terrainBounds.extents.z / height);
                var traceHeight = terrainBounds.max.y + 1000.0f;
                var traceDistance = (terrainBounds.extents.y + 1000.0f) * 2.0f;
                var layer = captureScreenshotEditorParams.TerrainOnly ? PhysicsLayers.TerrainMask : Physics.DefaultRaycastLayers;

                for (var x = 0; x < width; ++x)
                {
                    for (var y = 0; y < height; ++y)
                    {
                        var mapHeight = 0.0f;
                        var point = new Vector3(step.x + start.x + x * terrainBounds.extents.x * 2.0f / width, traceHeight, step.y + start.y + y * terrainBounds.extents.z * 2.0f / height);
                        RaycastHit raycastHit;
                        var result = Physics.Raycast(point, Vector3.down, out raycastHit, traceDistance, layer);
                        if (result)
                        {
                            mapHeight = raycastHit.point.y;
                        }
                        minHeight = Mathf.Min(minHeight, mapHeight);
                        maxHeight = Mathf.Max(maxHeight, mapHeight);
                        mapHeights[x * height + y] = mapHeight;
                    }
                }
                var texure = new Texture2D(width, height);
                var multiplier = 1.0f / (maxHeight - minHeight + 1.0f);
                var rainbowMultilpier = (rainbowColors.Length - 1) * multiplier;

                for (var x = 0; x < width; ++x)
                {
                    for (var y = 0; y < height; ++y)
                    {
                        if (captureScreenshotEditorParams.Rainbow)
                        {
                            var color = GetRainbowColor(mapHeights[x * height + y], minHeight, rainbowMultilpier);
                            texure.SetPixel(x, y, color);
                        }
                        else
                        {
                            var color = (mapHeights[x * height + y] - minHeight) * multiplier;
                            texure.SetPixel(x, y, new Color(captureScreenshotEditorParams.Red ? color : 0.0f, captureScreenshotEditorParams.Green ? color : 0.0f, captureScreenshotEditorParams.Blue ? color : 0.0f));
                        }
                    }
                }
                var bytes = texure.EncodeToPNG();
                DestroyImmediate(texure);
                var filePath = CaptureScreenshotUtils.GetUniqueScreenshotPath(true, string.Empty);
                var filePath_x_y = filePath + ".png";
                CaptureScreenshotUtils.SaveScreenshot(filePath_x_y, bytes, string.Empty);
                CaptureScreenshotUtils.Logger.IfInfo()?.Message($"Heightmap made: min: ({terrainBounds.min.x}, {terrainBounds.min.y}, {terrainBounds.min.z}), max: ({terrainBounds.max.x}, {terrainBounds.max.y}, {terrainBounds.max.z}), pixels: {captureScreenshotEditorParams.PixelsPerMeter}, angle: {captureScreenshotEditorParams.Angle}, border:{captureScreenshotEditorParams.Border}, path: {filePath_x_y}").Write();
            }
            else
            {
                var oldLodBias = QualitySettings.lodBias;
                QualitySettings.lodBias = 100000.0f;
                var previousShadows = SwitchShadowsOff();

                var terrainBounds = GetTerrainBounds();

                //var step = new Vector2(defaultWidth * meterPerPixel, defaultHeight * meterPerPixel);
                //var count = new Vector2Int((int)Mathf.Ceil(terrainBounds.extents.x * 2 / step.x), (int)Mathf.Ceil(terrainBounds.extents.z * 2 / step.y));

                var aspect = Mathf.Cos(captureScreenshotEditorParams.Angle * Mathf.Deg2Rad);
                var width = (int)(captureScreenshotEditorParams.Border * 2.0f + terrainBounds.extents.x * 2.0f * captureScreenshotEditorParams.PixelsPerMeter);
                var height = (int)(captureScreenshotEditorParams.Border * 2.0f + terrainBounds.extents.z * 2.0f * captureScreenshotEditorParams.PixelsPerMeter * aspect);
                var clipPlane = Mathf.Max(terrainBounds.extents.x, terrainBounds.extents.z);
                var renderCamera = new GameObject().AddComponent<UnityEngine.Camera>();
                renderCamera.orthographic = true;
                renderCamera.aspect = terrainBounds.extents.x / (terrainBounds.extents.z * aspect);
                renderCamera.orthographicSize = terrainBounds.extents.z * aspect + captureScreenshotEditorParams.Border / captureScreenshotEditorParams.PixelsPerMeter;
                renderCamera.transform.rotation = Quaternion.Euler(90.0f - captureScreenshotEditorParams.Angle, 0.0f, 0.0f);
                renderCamera.farClipPlane = terrainBounds.extents.y + clipPlane;
                renderCamera.nearClipPlane = (terrainBounds.extents.y + clipPlane) * (-1.0f);

                var filePath = CaptureScreenshotUtils.GetUniqueScreenshotPath(true, string.Empty);

                var progress = 0;
                //for (var x = 0; x < count.x; ++x)
                //{
                //    for (var y = 0; y < count.y; ++y)
                //    {

                ++progress;
                var position = terrainBounds.center;
                //var position = new Vector3(terrainBounds.min.x + (x + 0.5f) * step.x, terrainBounds.max.y + ascension, terrainBounds.min.z + (y + 0.5f) * step.y);

                //SceneView.lastActiveSceneView.pivot = position;
                //SceneView.lastActiveSceneView.Repaint();

                renderCamera.transform.position = position;
                //EditorUtility.DisplayProgressBar("Minimap screenshot creation", $"creating: x: {x}, y: {y}...", progress * 1.0f / (count.x * count.y));

                //var renderTexture = RenderTexture.GetTemporary();
                RenderTexture renderTexture = null;
                try
                {
                    renderTexture = new RenderTexture(width, height, 24);
                }
                catch (Exception ex)
                {
                    CaptureScreenshotUtils.Logger.IfError()?.Message($"Can't create RenderTexture({width}, {height}), message: {ex.Message}").Write();
                }
                if (renderTexture != null)
                {
                    var oldMainCameraTargetTexture = renderCamera.targetTexture;
                    renderCamera.targetTexture = renderTexture;
                    renderCamera.Render();
                    var texure = new Texture2D(width, height);
                    var oldRenderTextureActive = RenderTexture.active;
                    RenderTexture.active = renderTexture;
                    texure.ReadPixels(new Rect(0, 0, width, height), 0, 0);
                    var bytes = texure.EncodeToPNG();
                    RenderTexture.active = oldRenderTextureActive;
                    renderCamera.targetTexture = oldMainCameraTargetTexture;
                    DestroyImmediate(texure);
                    renderTexture.Release();
                    //var cameraString_x_y = $"{renderCamera.transform.position.x} {renderCamera.transform.position.y} {renderCamera.transform.position.z} {renderCamera.transform.rotation.eulerAngles.x} {renderCamera.transform.rotation.eulerAngles.y} {renderCamera.transform.rotation.eulerAngles.z}";
                    //var filePath_x_y = $"{filePath}_x{x:00}_y{y:00}.png";
                    var filePath_x_y = filePath + ".png";

                    CaptureScreenshotUtils.SaveScreenshot(filePath_x_y, bytes, string.Empty);
                    DestroyImmediate(renderCamera.gameObject);
                    //EditorUtility.ClearProgressBar();

                    CaptureScreenshotUtils.Logger.IfInfo()?.Message($"Minimap made: min: ({terrainBounds.min.x}, {terrainBounds.min.y}, {terrainBounds.min.z}), max: ({terrainBounds.max.x}, {terrainBounds.max.y}, {terrainBounds.max.z}), pixels: {captureScreenshotEditorParams.PixelsPerMeter}, angle: {captureScreenshotEditorParams.Angle}, border:{captureScreenshotEditorParams.Border}, path: {filePath_x_y}").Write();
                }
                //    }
                //}
                RestoreShadows(previousShadows);
                QualitySettings.lodBias = oldLodBias;
            }
        }

        // Menu -----------------------------------------------------------------------------------
        [MenuItem("Level Design/Screenshots/Make Screenshot _F12")]
        public static void MakeScreenshotMenu()
        {
            if (CartographerCommon.IsEditor())
            {
                MakeEditorScreenshot();
            }
            else
            {
                CaptureScreenshotBehaviour.CreategameScreenshot = true;
            }
        }

        [MenuItem("Level Design/Screenshots/Move Camera by Screenshot #F12")]
        public static void MoveCameraByScreenshotMenu()
        {
            if (CartographerCommon.IsEditor())
            {
                MoveEditorCameraByScreenshot();
            }
        }

        [MenuItem("Level Design/Screenshots/Make Minimap _F11")]
        public static void ScreenshotEditorWindowMenu()
        {
            if (CartographerCommon.IsEditor())
            {
                var window = GetWindow<CaptureScreenshotEditorWindow>("Minimap");
                window.Show();
            }
        }

        // unity methods --------------------------------------------------------------------------
        void OnGUI()
        {
            GUILayout.Space(10);

            captureScreenshotEditorParams.PixelsPerMeter = EditorGUILayout.FloatField("Pixels per meter:", captureScreenshotEditorParams.PixelsPerMeter);
            captureScreenshotEditorParams.Angle = EditorGUILayout.FloatField("Angle:", captureScreenshotEditorParams.Angle);
            captureScreenshotEditorParams.Border = EditorGUILayout.IntField("Border:", captureScreenshotEditorParams.Border);
            captureScreenshotEditorParams.Heightmap = EditorGUILayout.Toggle("Heightmap:", captureScreenshotEditorParams.Heightmap);
            if (captureScreenshotEditorParams.Heightmap)
            {
                captureScreenshotEditorParams.Rainbow = EditorGUILayout.Toggle("  Rainbow:", captureScreenshotEditorParams.Rainbow);
                if (!captureScreenshotEditorParams.Rainbow)
                {
                    captureScreenshotEditorParams.Red = EditorGUILayout.Toggle("  Red:", captureScreenshotEditorParams.Red);
                    captureScreenshotEditorParams.Green = EditorGUILayout.Toggle("  Green:", captureScreenshotEditorParams.Green);
                    captureScreenshotEditorParams.Blue = EditorGUILayout.Toggle("  Blue:", captureScreenshotEditorParams.Blue);
                }
            }
            captureScreenshotEditorParams.TerrainOnly = EditorGUILayout.Toggle("Terrain Only:", captureScreenshotEditorParams.TerrainOnly);

            captureScreenshotEditorParams.PixelsPerMeter = Mathf.Max(1.0f / 16.0f, Mathf.Min(16.0f, captureScreenshotEditorParams.PixelsPerMeter));
            captureScreenshotEditorParams.Angle = Mathf.Max(0.0f, Mathf.Min(90.0f, captureScreenshotEditorParams.Angle));
            captureScreenshotEditorParams.Border = Mathf.Max(0, Mathf.Min(128, captureScreenshotEditorParams.Border));

            GUILayout.Space(10);

            var size = GetMinimapScreenshotSize(captureScreenshotEditorParams);
            EditorGUILayout.LabelField($"Minimap size: {size.x} x {size.y} pixels");

            GUILayout.Space(10);

            if (GUILayout.Button("Make Minimap Screenshot"))
            {
                MakeMinimapScreenshots(captureScreenshotEditorParams);
            }
        }
    }
}