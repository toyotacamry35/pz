using SharedCode.Aspects.Cartographer;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Src.Cartographer.Editor
{
    [CustomEditor(typeof(CartographerBehaviour))]
    public class CartographerEditor : UnityEditor.Editor
    {
        // Consts ---------------------------------------------------------------------------------
        private static readonly bool defaultShowSpecialButtons = false;
        private static readonly bool defaultShowAreasButtons = false;
        private static readonly string defaultAreaName = string.Empty;
        //
        private static string creationError = "You should call Create()";
        private static float gridlineThickness = 1.0f;
        private static float segmentThickness = 2.0f;
        private static Color mouseColor = new Color(0f, 0f, 1f, 0.6f);
        private static Color addMouseColor = new Color(0f, 1f, 0f, 0.6f);
        private static Color removeMouseColor = new Color(1f, 0f, 0f, 0.6f);
        private static Color gridlineColor = new Color(1f, 1f, 1f, 0.8f);
        private static Color selectionColor = new Color(1f, 1f, 1f, 0.6f);
        private static Color loadColor = new Color(0f, 1f, 1f, 0.4f);
        private static Color cameraFrameColorShadow = new Color(0f, 0f, 0f, 0.4f);
        private static Color cameraFrameColor = new Color(1f, 1f, 0.5f, 1.0f);

        // Public static properties ---------------------------------------------------------------
        public static bool ShowSpecialButtons { get; set; } = defaultShowSpecialButtons;
        public static bool ShowAreasButtons { get; set; } = defaultShowAreasButtons;
        public static string AreaName { get; set; } = defaultAreaName;

        // Private --------------------------------------------------------------------------------
        private bool created = false;
        //
        private Vector2 initialMousePoint = Vector2.zero;
        private Vector2 mousePoint = Vector2.zero;
        private Vector2 cameraPoint = Vector2.zero;
        private Rect minimapRect = Rect.zero;
        private bool mousePressed = false;
        private bool mouseReverted = false;
        private bool mousePressedAdd = false;
        private bool mousePressedRemove = false;
        private bool cameraMoved = false;
        private Rect mouseRect = Rect.zero;
        private HashSet<int> initialSelection = new HashSet<int>();
        private HashSet<int> instantSelection = new HashSet<int>();
        private HashSet<int> selection = new HashSet<int>();
        private Vector2Int mouseSceneIndex = Vector2Int.zero;

        private void Create()
        {
            if (!created)
            {
                var sceneCollection = GetSceneCollection();
                if (sceneCollection != null)
                {
                    CartographerSaveLoad.StreamSceneSize = (Vector3)(sceneCollection.SceneSize);
                }
                var cartographerParams = GetCartographerParams();
                if (cartographerParams != null)
                {
                    CartographerSaveLoad.MinimalOcculderBounds = (Vector3)(cartographerParams.MinimalOcculderBounds);
                }
                created = true;
            }
        }

        private void Destroy()
        {
            if (created)
            {
                created = false;
            }
            creationError = "Destroy() called";
        }

        private CartographerParamsDef GetCartographerParams()
        {
            return GetCartographer()?.CartographerParams?.Get<CartographerParamsDef>() ?? null;
        }

        private SceneCollectionDef GetSceneCollection()
        {
            return GetCartographer()?.SceneCollection?.Get<SceneCollectionDef>() ?? null;
        }

        private Texture GetMinimap()
        {
            return GetCartographer()?.Minimap ?? null;
        }

        private GameObject GetCartographerGameObject()
        {
            return GetCartographer()?.gameObject ?? null;
        }

        private CartographerBehaviour GetCartographer()
        {
            return target as CartographerBehaviour;
        }

        private static Rect GetDrawRect(Rect minimapRect, int x, int y, Vector2 segment)
        {
            return new Rect(minimapRect.xMin + segment.x * x + segmentThickness, minimapRect.yMin + segment.y * y + segmentThickness, segment.x - segmentThickness - gridlineThickness, segment.y - segmentThickness - gridlineThickness);
        }

        private static Vector2Int GetMouseSceneIndex(SceneCollectionDef sceneCollection, Rect minimapRect, Vector2 mousePoint, Vector2 segment)
        {
            var x = Mathf.FloorToInt((mousePoint.x - minimapRect.xMin) / segment.x);
            var y = Mathf.FloorToInt((mousePoint.y - minimapRect.yMin) / segment.y);
            return new Vector2Int(sceneCollection.SceneStart.x + x, sceneCollection.SceneStart.z + sceneCollection.SceneCount.z - y - 1);
        }

        private static void GetCollection(SceneCollectionDef mainSceneCollection, SceneCollectionDef sceneCollection, HashSet<int> collection)
        {
            collection.Clear();
            foreach (var sceneName in sceneCollection.SceneNames)
            {
                Vector3Int coordinates;
                if (CartographerCommon.IsSceneForStreaming(sceneName, out coordinates))
                {
                    collection.Add(CartographerCommon.GetStreamScenePackedIndex(coordinates, mainSceneCollection));
                }
            }
        }

        private static void GetCollection(SceneCollectionDef mainSceneCollection, RectInt selectionRect, HashSet<int> collection)
        {
            collection.Clear();
            for (var x = selectionRect.xMin; x < selectionRect.xMax; ++x)
            {
                for (var y = selectionRect.yMin; y < selectionRect.yMax; ++y)
                {
                    var coordinates = new Vector3Int(x, 0, y);
                    collection.Add(CartographerCommon.GetStreamScenePackedIndex(coordinates, mainSceneCollection));
                }
            }
        }

        private static void CombineCollection(HashSet<int> initialSelection, HashSet<int> instantSelection, bool mousePressedAdd, bool mousePressedRemove, HashSet<int> collection)
        {
            collection.Clear();
            foreach (var item in initialSelection)
            {
                collection.Add(item);
            }
            if (mousePressedAdd)
            {
                foreach (var item in instantSelection)
                {
                    collection.Add(item);
                }
            }
            else if (mousePressedRemove)
            {
                foreach (var item in instantSelection)
                {
                    collection.Remove(item);
                }
            }
        }

        private static RectInt GetCollectionRect(RectInt rect, SceneCollectionDef sceneCollection)
        {
            return new RectInt(sceneCollection.SceneStart.x + rect.xMin, sceneCollection.SceneStart.z + sceneCollection.SceneCount.z - rect.yMin - rect.height, rect.width, rect.height);
        }

        private static Vector2 GetSelectionPoint(SceneCollectionDef sceneCollection, Rect minimapRect, Vector2 cameraPoint)
        {
            var cameraStart = new Vector2(sceneCollection.SceneStart.x * sceneCollection.SceneSize.x, sceneCollection.SceneStart.z * sceneCollection.SceneSize.z);
            var cameraDimensions = new Vector2(sceneCollection.SceneCount.x * sceneCollection.SceneSize.x, sceneCollection.SceneCount.z * sceneCollection.SceneSize.z);
            var x = (cameraPoint.x - minimapRect.xMin);
            var y = (minimapRect.yMax - cameraPoint.y);
            return new Vector2(cameraStart.x + x * cameraDimensions.x / minimapRect.width, cameraStart.y + y * cameraDimensions.y / minimapRect.height);
        }

        private static RectInt GetSelectionRect(SceneCollectionDef sceneCollection, Rect minimapRect, Rect rect, Vector2 segment)
        {
            var x = Mathf.FloorToInt((rect.xMin - minimapRect.xMin) / segment.x);
            var y = Mathf.FloorToInt((rect.yMin - minimapRect.yMin) / segment.y);
            var selectionRect = new RectInt(x, y, Mathf.CeilToInt((rect.xMax - minimapRect.xMin) / segment.x) - x, Mathf.CeilToInt((rect.yMax - minimapRect.yMin) / segment.y) - y);
            return new RectInt(sceneCollection.SceneStart.x + selectionRect.xMin, sceneCollection.SceneStart.z + sceneCollection.SceneCount.z - selectionRect.yMin - selectionRect.height, selectionRect.width, selectionRect.height);
        }

        private static void GetSceneStartAndCount(SceneCollectionDef sceneCollection, HashSet<int> selection, out SharedCode.Utils.Vector3Int start, out SharedCode.Utils.Vector3Int count)
        {
            var minx = int.MaxValue;
            var miny = int.MaxValue;
            var maxx = int.MinValue;
            var maxy = int.MinValue;
            foreach (var item in selection)
            {
                var coordinates = CartographerCommon.GetStreamSceneCoordinates(item, sceneCollection);
                minx = Mathf.Min(coordinates.x, minx);
                miny = Mathf.Min(coordinates.z, miny);
                maxx = Mathf.Max(coordinates.x, maxx);
                maxy = Mathf.Max(coordinates.z, maxy);
            }
            start = new SharedCode.Utils.Vector3Int(minx, 0, miny);
            count = new SharedCode.Utils.Vector3Int(maxx - minx + 1, 0, maxy - miny + 1);
        }

        private void MoveCamera(Vector2 realCameraPoint)
        {
            SceneView.lastActiveSceneView.pivot = new Vector3(realCameraPoint.x, SceneView.lastActiveSceneView.pivot.y, realCameraPoint.y);
        }

        private Rect GetCameraRect(SceneCollectionDef sceneCollection, Rect minimapRect, bool shadow)
        {
            if (SceneView.lastActiveSceneView != null)
            {
                var frameRadius = 4;
                var radius = 5.0f;
                var cameraPos = SceneView.lastActiveSceneView.pivot;
                var cameraDimensions = new Vector2(sceneCollection.SceneCount.x * sceneCollection.SceneSize.x, sceneCollection.SceneCount.z * sceneCollection.SceneSize.z);
                var x = minimapRect.xMin + ((cameraPos.x - sceneCollection.SceneStart.x * sceneCollection.SceneSize.x) * minimapRect.width / cameraDimensions.x);
                var y = minimapRect.yMax - ((cameraPos.z - sceneCollection.SceneStart.z * sceneCollection.SceneSize.z) * minimapRect.height / cameraDimensions.y) - 1;
                var center = new Vector2Int((int)Mathf.Clamp(x, minimapRect.xMin + radius, minimapRect.xMax - radius),
                                            (int)Mathf.Clamp(y, minimapRect.yMin + radius, minimapRect.yMax - radius));
                return new Rect(center.x - frameRadius + (shadow ? 1 : 0), center.y - frameRadius + (shadow ? 1 : 0), frameRadius * 2 + 1, frameRadius * 2 + 1);
            }
            return new Rect(0, 0, 1, 1);
        }

        private void CheckAndRemoveScenes(string message, List<string> scenePaths, CartographerParamsDef cartorgapherParams, SceneCollectionDef sceneCollection)
        {
            if (EditorUtility.DisplayDialog("Remove Scenes", message, CartographerCommon.Messages.YesButton, CartographerCommon.Messages.NoButton))
            {
                var result = 0;
                var unsavedScenePaths = CartographerSaveLoad.CheckOpenedStreamScenesForChanges(scenePaths);
                if ((unsavedScenePaths != null) && (unsavedScenePaths.Count > 0))
                {
                    result = EditorUtility.DisplayDialogComplex("Scene(s) Have Been Modified", $"Do you want to save the changes you made in the scenes:\r\n{CartographerCommon.GetSceneNamesLabel(unsavedScenePaths, 16)}\r\n\r\nYour changes will be lost if you don't save them.", "Save", "Cancel", "Don't Save");
                    if (result == 0)
                    {
                        var cartographerGameObject = GetCartographerGameObject();
                        if (cartographerGameObject != null)
                        {
                            CartographerSaveLoad.SaveAllOpenedStreamScenes(cartographerGameObject.scene, cartorgapherParams, sceneCollection, true, true, true, false, scenePaths, CartographerProgressCallback.Default);
                        }
                    }
                }
                if (result != 1)
                {
                    CartographerSaveLoad.RemoveOpenedStreamScenes(scenePaths, CartographerProgressCallback.Default);
                }
            }
        }

        public void Awake()
        {
            Create();
        }

        public void OnDestroy()
        {
            Destroy();
        }

        public override bool RequiresConstantRepaint()
        {
            return GetCartographerParams()?.ConstantRepaint ?? false;
        }

        public override void OnInspectorGUI()
        {
            // various messages if any error occurs
            if (!created)
            {
                EditorGUILayout.HelpBox($"Can't initialize editor, error: {creationError}.", MessageType.Info);
            }
            else
            {
                var resultsFilePath = string.Empty;
                var eventCurrent = Event.current;
                if (((eventCurrent.type == EventType.MouseDown) || (eventCurrent.type == EventType.MouseDrag)) && (eventCurrent.button == 2))
                {
                    mousePoint = eventCurrent.mousePosition;
                    selection.Clear();
                    Repaint();
                }
                else if (((eventCurrent.type == EventType.MouseDown) || (eventCurrent.type == EventType.MouseDrag)) && (eventCurrent.button == 0))
                {
                    mousePoint = eventCurrent.mousePosition;
                    if (!mouseReverted)
                    {
                        var mouseOver = minimapRect.Contains(mousePoint);
                        if (mouseOver)
                        {
                            if (mousePressed == false)
                            {
                                initialMousePoint = mousePoint;
                                mousePressedAdd = eventCurrent.shift;
                                mousePressedRemove = eventCurrent.control;
                                initialSelection.Clear();
                                foreach (var item in selection)
                                {
                                    initialSelection.Add(item);
                                }
                            }
                            mousePressed = true;
                            Repaint();
                            return; // DO NOT REMOVE
                        }
                        else
                        {
                            mousePressed = false;
                            Repaint();
                        }
                    }
                }
                else if (((eventCurrent.type == EventType.MouseUp) && (eventCurrent.button == 0)) || (eventCurrent.type == EventType.MouseLeaveWindow))
                {
                    mousePoint = eventCurrent.mousePosition;
                    mouseReverted = false;
                    mousePressed = false;
                    Repaint();
                }
                else if (((eventCurrent.type == EventType.MouseDown) || (eventCurrent.type == EventType.MouseDrag)) && (eventCurrent.button == 1))
                {
                    mousePoint = eventCurrent.mousePosition;
                    if (mousePressed)
                    {
                        selection.Clear();
                        foreach (var item in initialSelection)
                        {
                            selection.Add(item);
                        }
                        mousePressed = false;
                        mouseReverted = true;
                        Repaint();
                    }
                    else if (!mouseReverted)
                    {
                        cameraPoint = eventCurrent.mousePosition;
                        var mouseOver = minimapRect.Contains(cameraPoint);
                        if (mouseOver)
                        {
                            cameraMoved = true;
                            Repaint();
                            return; // DO NOT REMOVE
                        }
                        else
                        {
                            cameraMoved = false;
                            Repaint();
                        }
                    }
                }
                else if (((eventCurrent.type == EventType.MouseUp) && (eventCurrent.button == 1)) || (eventCurrent.type == EventType.MouseLeaveWindow))
                {
                    cameraMoved = false;
                    Repaint();
                }

                var cartorgapherParams = GetCartographerParams();
                var sceneCollection = GetSceneCollection();
                var minimap = GetMinimap();

                ShowSpecialButtons = EditorGUILayout.Toggle("Show Internals", ShowSpecialButtons);
                ShowAreasButtons = EditorGUILayout.Toggle("Show Areas", ShowAreasButtons);

                if (ShowSpecialButtons)
                {
                    EditorGUILayout.LabelField("Internals", EditorStyles.boldLabel);
                    EditorGUILayout.HelpBox($"initialMousePoint: {initialMousePoint.ToString()}\nmousePoint: {mousePoint.ToString()}\nminimapRect: {minimapRect.ToString()}\nmousePressed: {mousePressed}\nmouseRect: {mouseRect.ToString()}\nselection.Count: {selection.Count}", MessageType.Info);
                    var serializedObject = new SerializedObject(target);
                    var cartographerParamsProperty = serializedObject.FindProperty("CartographerParams");
                    var sceneCollectionProperty = serializedObject.FindProperty("SceneCollection");
                    var minimapProperty = serializedObject.FindProperty("Minimap");
                    // main selection
                    EditorGUILayout.ObjectField(cartographerParamsProperty);
                    EditorGUILayout.ObjectField(sceneCollectionProperty);
                    EditorGUILayout.ObjectField(minimapProperty);
                    serializedObject.ApplyModifiedProperties();
                    serializedObject.Dispose();
                    EditorGUILayout.LabelField("Switches", EditorStyles.boldLabel);
                    CartographerSaveLoad.DisableSceneCallbacks = EditorGUILayout.Toggle("Toggle Debug Mode", CartographerSaveLoad.DisableSceneCallbacks);
                    cartorgapherParams.ShowGridlines = EditorGUILayout.Toggle("Show Gridlines", cartorgapherParams.ShowGridlines);
                    cartorgapherParams.ShowLoadedScenes = EditorGUILayout.Toggle("Show Loaded Scenes", cartorgapherParams.ShowLoadedScenes);
                    cartorgapherParams.ConstantRepaint = EditorGUILayout.Toggle("Constant Repaint", cartorgapherParams.ConstantRepaint);
                }

                if ((cartorgapherParams != null) && (sceneCollection != null) && (minimap != null))
                {
                    var cartographerGameObject = GetCartographerGameObject();
                    if (cartographerGameObject != null)
                    {

                        EditorGUILayout.LabelField("Scene management", EditorStyles.boldLabel);
                        if (ShowSpecialButtons)
                        {
                            var myTag = string.Empty;
                            GUILayout.Space(10);
                            myTag = EditorGUILayout.TextField("New Tag:", myTag);
                            GUILayout.Space(10);
                            GUILayout.BeginHorizontal();
                            if (GUILayout.Button("My Button"))
                            {
                                if (EditorUtility.DisplayDialog("My Button", "Если вы не Юрия Сальникеов не нажимайте эту кнопку!", CartographerCommon.Messages.YesButton, CartographerCommon.Messages.NoButton))
                                {
                                    var operation = new CSOTouchGameObjects(myTag);
                                    CartographerSceneOperation.Operate(operation,
                                                                       cartorgapherParams,
                                                                       sceneCollection,
                                                                       true,
                                                                       CartographerSceneType.StreamCollection |
                                                                       CartographerSceneType.BackgroundClient |
                                                                       CartographerSceneType.BackgroundServer |
                                                                       CartographerSceneType.MapDefClient |
                                                                       CartographerSceneType.MapDefServer,
                                                                       CartographerProgressCallback.Default);
                                }
                            }
                            if (GUILayout.Button("Force Save"))
                            {
                                if (EditorUtility.DisplayDialog("Save All Scenes", "Are you sure you wanna save all scenes?", CartographerCommon.Messages.YesButton, CartographerCommon.Messages.NoButton))
                                {
                                    CartographerSaveLoad.SaveAllOpenedStreamScenes(cartographerGameObject.scene, cartorgapherParams, sceneCollection, true, true, true, true, null, CartographerProgressCallback.Default);
                                }
                            }
                            if (GUILayout.Button("All Client"))
                            {
                                if (EditorUtility.DisplayDialog(CSOGenerateBackgroundClient.Messages.Title, CSOGenerateBackgroundClient.Messages.RunQuestion, CartographerCommon.Messages.YesButton, CartographerCommon.Messages.NoButton))
                                {
                                    var operation = new CSOGenerateBackgroundClient();
                                    CartographerSceneOperation.Operate(operation, cartorgapherParams, sceneCollection, false, CartographerSceneType.StreamCollection, CartographerProgressCallback.Default);
                                }
                            }
                            if (GUILayout.Button("All Server"))
                            {
                                if (EditorUtility.DisplayDialog(CSOGenerateBackgroundServer.Messages.Title, CSOGenerateBackgroundServer.Messages.RunQuestion, CartographerCommon.Messages.YesButton, CartographerCommon.Messages.NoButton))
                                {
                                    var operation = new CSOGenerateBackgroundServer();
                                    CartographerSceneOperation.Operate(operation, cartorgapherParams, sceneCollection, false, CartographerSceneType.StreamCollection, CartographerProgressCallback.Default);
                                }
                            }
                            //if (GUILayout.Button("Save All Prefabs"))
                            //{

                            //    if (EditorUtility.DisplayDialog("Force Save All Prefabs", "Are you sure you wanna force save all Prefabs?", CartographerCommon.Messages.YesButton, CartographerCommon.Messages.NoButton))
                            //    {
                            //        CartographerUtils.ForceSaveAllPrefabs(CartographerProgressCallback.Default);
                            //    }
                            //}
                            //if (GUILayout.Button("Get Opened Visuals"))
                            //{
                            //    if (EditorUtility.DisplayDialog(CSOCollectVisuals.Messages.Title, CSOCollectVisuals.Messages.RunQuestion, CartographerCommon.Messages.YesButton, CartographerCommon.Messages.NoButton))
                            //    {
                            //        var operation = new CSOCollectVisuals();
                            //        CartographerSceneOperation.Operate(operation,
                            //                                           cartorgapherParams,
                            //                                           sceneCollection,
                            //                                           true,
                            //                                           CartographerScenePathType.StreamCollection |
                            //                                           CartographerScenePathType.BackgroundClient |
                            //                                           CartographerScenePathType.BackgroundServer |
                            //                                           CartographerScenePathType.MapDefClient |
                            //                                           CartographerScenePathType.MapDefServer,
                            //                                           CartographerProgressCallback.Default);
                            //    }
                            //}
                            //if (GUILayout.Button("Get All Visuals"))
                            //{
                            //    if (EditorUtility.DisplayDialog(CSOCollectVisuals.Messages.Title, CSOCollectVisuals.Messages.RunQuestion, CartographerCommon.Messages.YesButton, CartographerCommon.Messages.NoButton))
                            //    {
                            //        var operation = new CSOCollectVisuals();
                            //        CartographerSceneOperation.Operate(operation,
                            //                                           cartorgapherParams,
                            //                                           sceneCollection,
                            //                                           false,
                            //                                           CartographerScenePathType.StreamCollection |
                            //                                           CartographerScenePathType.BackgroundClient |
                            //                                           CartographerScenePathType.BackgroundServer |
                            //                                           CartographerScenePathType.MapDefClient |
                            //                                           CartographerScenePathType.MapDefServer,
                            //                                           CartographerProgressCallback.Default);
                            //    }
                            //}
                            //if (GUILayout.Button("Force Save"))
                            //{

                            //    if (EditorUtility.DisplayDialog("Save All Scenes", "Are you sure you wanna save all scenes?", CartographerCommon.Messages.YesButton, CartographerCommon.Messages.NoButton))
                            //    {
                            //        var cartographerGameObject = GetCartographerGameObject();
                            //        if (cartographerGameObject != null)
                            //        {
                            //            CartographerUtils.SaveScenes(cartographerGameObject.scene, cartorgapherParams, sceneCollection, true, true, true, true, null, CartographerProgressCallback.Default);
                            //        }
                            //    }
                            //}
                            //if (GUILayout.Button("TTM"))
                            //{
                            //    if (EditorUtility.DisplayDialog("Terrain To Mesh", "Are you sure you wanna create terrain mesh?", CartographerCommon.Messages.YesButton, CartographerCommon.Messages.NoButton))
                            //    {
                            //        var cartographerGameObject = GetCartographerGameObject();
                            //        if (cartographerGameObject != null)
                            //        {
                            //            CartographerUtils.CreateTerrainMeshes(cartographerGameObject.scene);
                            //        }
                            //    }
                            //}
                            //if (GUILayout.Button("Check Client"))
                            //{
                            //    if (EditorUtility.DisplayDialog("Check Crater Background", "Are you sure you wanna check crater background?", CartographerCommon.Messages.YesButton, CartographerCommon.Messages.NoButton))
                            //    {
                            //        var operation = new CartographerUtils.CheckCraterBackgroundOperation();
                            //        CartographerUtils.OperateSceneCollection(operation, cartorgapherParams, sceneCollection, false, CartographerProgressCallback.Default);
                            //    }
                            //}

                            //if (GUILayout.Button("ResaveAllTerrainData"))
                            //{
                            //    if (EditorUtility.DisplayDialog("Resave Terrain", "Are you sure you wanna resave all terrains?", CartographerCommon.Messages.YesButton, CartographerCommon.Messages.NoButton))
                            //    {
                            //        var operation = new CartographerUtils.ResaveAllTerrain();
                            //        CartographerUtils.OperateSceneCollection(operation, cartorgapherParams, sceneCollection, false, CartographerProgressCallback.Default);
                            //    }
                            //}

                            //if (GUILayout.Button("Check"))
                            //{
                            //    if (EditorUtility.DisplayDialog("Check Scenes", "Are you sure you wanna check all scenes?", CartographerCommon.Messages.YesButton, CartographerCommon.Messages.NoButton))
                            //    {
                            //        CartographerUtils.CheckSceneCollection(sceneCollection, CartographerProgressCallback.Default);
                            //    }
                            //}
                            //if (GUILayout.Button("Resave"))
                            //{
                            //    if (EditorUtility.DisplayDialog("Resave Scenes", "Are you sure you wanna resave all scenes?", CartographerCommon.Messages.YesButton, CartographerCommon.Messages.NoButton))
                            //    {
                            //        CartographerUtils.ResaveSceneCollection(sceneCollection, CartographerProgressCallback.Default);
                            //    }
                            //}
                            //if (GUILayout.Button("Clean"))
                            //{
                            //    if (EditorUtility.DisplayDialog("Clean Scenes", "Are you sure you wanna remove all vegetation from scenes?", CartographerCommon.Messages.YesButton, CartographerCommon.Messages.NoButton))
                            //    {
                            //        CartographerUtils.CleanSceneCollection(sceneCollection, CartographerProgressCallback.Default);
                            //    }
                            //}
                            //if (GUILayout.Button("TXT"))
                            //{
                            //    if (EditorUtility.DisplayDialog("Create Text Minimap", "Are you sure you wanna create text minimap?", CartographerCommon.Messages.YesButton, CartographerCommon.Messages.NoButton))
                            //    {
                            //        CartographerUtils.CreateTextMinimap(cartorgapherParams, sceneCollection);
                            //    }
                            //}
                            //if (GUILayout.Button("JDB"))
                            //{
                            //    if (EditorUtility.DisplayDialog("Create Jdb Scene Collection", "Are you sure you wanna create jdb scene collection?", CartographerCommon.Messages.YesButton, CartographerCommon.Messages.NoButton))
                            //    {
                            //        CartographerUtils.CreateSceneCollectionFile(cartorgapherParams);
                            //    }
                            //}
                            //if (GUILayout.Button("FIX"))
                            //{
                            //    if (EditorUtility.DisplayDialog("Fix Jdb Scene Collection file", "Are you sure you wanna fix jdb scene collection?", CartographerCommon.Messages.YesButton, CartographerCommon.Messages.NoButton))
                            //    {
                            //        CartographerUtils.FixSceneCollectionFile(sceneCollection);
                            //    }
                            //}
                            //if (GUILayout.Button("Create"))
                            //{
                            //    if (EditorUtility.DisplayDialog("Create Scenes", "Are you sure you wanna create scene collection?", CartographerCommon.Messages.YesButton, CartographerCommon.Messages.NoButton))
                            //    {
                            //        CartographerUtils.CreateSceneCollection(cartorgapherParams, CartographerProgressCallback.Default);
                            //    }
                            //}
                            GUILayout.EndHorizontal();
                        }
                        GUILayout.BeginHorizontal();
                        var checkPressed = GUILayout.Button("Check");
                        var savePressed = GUILayout.Button("Save");
                        if (checkPressed || savePressed)
                        {
                            var proseed = true;
                            if (savePressed)
                            {
                                proseed = EditorUtility.DisplayDialog("Save All Scenes", "Are you sure you wanna save all scenes?", CartographerCommon.Messages.YesButton, CartographerCommon.Messages.NoButton);
                            }
                            if (proseed)
                            {
                                CartographerSaveLoad.TouchAllOpenedStreamScenes();
                                var settings = CartographerSceneStatsEditorWindow.GetSettings();
                                var arguments = new CSOCheckScenesArguments();
                                if (settings != null)
                                {
                                    arguments.IgnoreUnknownCartographerSceneTypes = settings.CollectSceneStatsArguments.IgnoreUnknownCartographerSceneTypes;
                                    arguments.FormatArguments.CopyFrom(settings.CollectSceneStatsArguments.FormatArguments);
                                }
                                var operation = new CSOCheckScenes(arguments);
                                CartographerSceneOperation.Operate(operation, cartorgapherParams, sceneCollection, true, CartographerSceneType.None, CartographerProgressCallback.Default);
                                if (savePressed)
                                {
                                    CartographerSaveLoad.SaveAllOpenedStreamScenes(cartographerGameObject.scene, cartorgapherParams, sceneCollection, true, true, true, false, null, CartographerProgressCallback.Default);
                                }
                                resultsFilePath = arguments.ResultsFilePath;
                            }
                        }
                        if (GUILayout.Button("Stats"))
                        {
                            CartographerSceneStatsEditorWindow.ToggleWindow(true);
                        }
                        if (GUILayout.Button("Client"))
                        {
                            if (EditorUtility.DisplayDialog(CSOGenerateBackgroundClient.Messages.Title, CSOGenerateBackgroundClient.Messages.RunQuestion, CartographerCommon.Messages.YesButton, CartographerCommon.Messages.NoButton))
                            {
                                var operation = new CSOGenerateBackgroundClient();
                                CartographerSceneOperation.Operate(operation, cartorgapherParams, sceneCollection, true, CartographerSceneType.None, CartographerProgressCallback.Default);
                            }
                        }
                        if (GUILayout.Button("Server"))
                        {
                            if (EditorUtility.DisplayDialog(CSOGenerateBackgroundServer.Messages.Title, CSOGenerateBackgroundServer.Messages.RunQuestion, CartographerCommon.Messages.YesButton, CartographerCommon.Messages.NoButton))
                            {
                                var operation = new CSOGenerateBackgroundServer();
                                CartographerSceneOperation.Operate(operation, cartorgapherParams, sceneCollection, true, CartographerSceneType.None, CartographerProgressCallback.Default);
                            }
                        }
                        GUILayout.EndHorizontal();
                    }
                    else
                    {
                        EditorGUILayout.HelpBox($"Can't find cartographer.", MessageType.Info);
                    }
                    if (ShowAreasButtons)
                    {
                        EditorGUILayout.LabelField("Areas", EditorStyles.boldLabel);
                        if (cartorgapherParams.SceneLoadAreas != null)
                        {
                            for (int sceneLoadAreaIndex = 0; sceneLoadAreaIndex < cartorgapherParams.SceneLoadAreas.Count; ++sceneLoadAreaIndex)
                            {
                                var sceneLoadArea = cartorgapherParams.SceneLoadAreas[sceneLoadAreaIndex];
                                GUILayout.BeginHorizontal();
                                GUILayout.Label(sceneLoadArea.Name, GUILayout.Width(100));
                                if (GUILayout.Button("Load"))
                                {
                                    if (EditorUtility.DisplayDialog("Load Scenes", $"Are you sure you wanna load scenes from {sceneLoadArea.Name}?", CartographerCommon.Messages.YesButton, CartographerCommon.Messages.NoButton))
                                    {
                                        var scenePaths = CartographerSaveLoad.GetStreamScenePaths(sceneLoadArea, false);
                                        CartographerSaveLoad.LoadStreamScenes(scenePaths, CartographerProgressCallback.Default);
                                    }
                                }
                                if (GUILayout.Button("Remove"))
                                {
                                    var message = $"Are you sure you wanna remove scenes from {sceneLoadArea.Name}?";
                                    var scenePaths = CartographerSaveLoad.GetStreamScenePaths(sceneLoadArea, false);
                                    CheckAndRemoveScenes(message, scenePaths, cartorgapherParams, sceneCollection);
                                }
                                if (GUILayout.Button("Select"))
                                {
                                    GetCollection(sceneCollection, cartorgapherParams.SceneLoadAreas[sceneLoadAreaIndex].Collection, selection);
                                }
                                if (GUILayout.Button("X"))
                                {
                                    if (EditorUtility.DisplayDialog("Remove Area", $"Are you sure you wanna remove this area {sceneLoadArea.Name}?", CartographerCommon.Messages.YesButton, CartographerCommon.Messages.NoButton))
                                    {
                                        cartorgapherParams.SceneLoadAreas.RemoveAt(sceneLoadAreaIndex);
                                        GameResourcesLikeFileSaver.SaveFile(cartorgapherParams);
                                        Repaint();
                                        return;
                                    }
                                }
                                GUILayout.EndHorizontal();
                            }
                        }
                    }
                    EditorGUILayout.LabelField("Minimap", EditorStyles.boldLabel);
                    var size = EditorGUIUtility.currentViewWidth - 18;
                    var segment = new Vector2(size / sceneCollection.SceneCount.x, size / sceneCollection.SceneCount.z);

                    GUILayout.Label(minimap, GUILayout.Width(size), GUILayout.Height(size));
                    if (eventCurrent.type == EventType.Repaint)
                    {
                        minimapRect = GUILayoutUtility.GetLastRect();
                    }

                    mouseSceneIndex = GetMouseSceneIndex(sceneCollection, minimapRect, Event.current.mousePosition, segment);

                    if (mousePressed)
                    {
                        mouseRect = new Rect(Mathf.Min(initialMousePoint.x, mousePoint.x), Mathf.Min(initialMousePoint.y, mousePoint.y), Mathf.Abs(initialMousePoint.x - mousePoint.x), Mathf.Abs(initialMousePoint.y - mousePoint.y));
                        if (mousePressedAdd || mousePressedRemove)
                        {
                            GetCollection(sceneCollection, GetSelectionRect(sceneCollection, minimapRect, mouseRect, segment), instantSelection);
                            CombineCollection(initialSelection, instantSelection, mousePressedAdd, mousePressedRemove, selection);
                        }
                        else
                        {
                            GetCollection(sceneCollection, GetSelectionRect(sceneCollection, minimapRect, mouseRect, segment), selection);
                        }
                    }
                    else
                    {
                        mouseRect = Rect.zero;
                    }

                    if (cameraMoved)
                    {
                        MoveCamera(GetSelectionPoint(sceneCollection, minimapRect, cameraPoint));
                    }

                    EditorGUI.DrawRect(GetCameraRect(sceneCollection, minimapRect, true), cameraFrameColorShadow);
                    EditorGUI.DrawRect(GetCameraRect(sceneCollection, minimapRect, false), cameraFrameColor);

                    var mouseRectValid = (mouseRect.width != 0) && (mouseRect.height != 0);

                    if (selection.Count > 0)
                    {
                        foreach (var item in selection)
                        {
                            var coordinates = CartographerCommon.GetStreamSceneCoordinates(item, sceneCollection);
                            var x = coordinates.x - sceneCollection.SceneStart.x;
                            var y = sceneCollection.SceneStart.z + sceneCollection.SceneCount.z - coordinates.z - 1;
                            var drawRect = GetDrawRect(minimapRect, x, y, segment);
                            EditorGUI.DrawRect(drawRect, selectionColor);
                        }
                    }

                    if (cartorgapherParams.ShowGridlines)
                    {
                        for (var index = 1; index < sceneCollection.SceneCount.x; ++index)
                        {
                            var drawRect = new Rect(minimapRect.xMin + segment.x * index, minimapRect.yMin, gridlineThickness, minimapRect.height);
                            EditorGUI.DrawRect(drawRect, Color.white);
                        }
                        for (var index = 1; index < sceneCollection.SceneCount.z; ++index)
                        {
                            var drawRect = new Rect(minimapRect.xMin, minimapRect.yMin + segment.y * index, minimapRect.width, gridlineThickness);
                            EditorGUI.DrawRect(drawRect, gridlineColor);
                        }
                    }

                    if (cartorgapherParams.ShowLoadedScenes)
                    {
                        var loadedSceneCoordinates = new List<Vector3Int>();
                        CartographerSaveLoad.GetLoadedStreamSceneCoordinates(loadedSceneCoordinates);
                        foreach (var coordinates in loadedSceneCoordinates)
                        {
                            var x = coordinates.x - sceneCollection.SceneStart.x;
                            var y = sceneCollection.SceneStart.z + sceneCollection.SceneCount.z - coordinates.z - 1;
                            var drawRect = GetDrawRect(minimapRect, x, y, segment);
                            EditorGUI.DrawRect(drawRect, loadColor);
                        }
                    }

                    if (mouseRectValid)
                    {
                        EditorGUI.DrawRect(mouseRect, mousePressedRemove ? removeMouseColor : (mousePressedAdd ? addMouseColor : mouseColor));
                    }

                    EditorGUILayout.LabelField("Selection", EditorStyles.boldLabel);

                    if (selection.Count > 0)
                    {
                        EditorGUILayout.HelpBox($"Mouse: {mouseSceneIndex.x} {mouseSceneIndex.y},  Selection: {selection.Count}", MessageType.Info);
                    }
                    else
                    {
                        EditorGUILayout.HelpBox($"Mouse: {mouseSceneIndex.x} {mouseSceneIndex.y}, Nothing selected!", MessageType.Info);
                    }
                    if (selection.Count > 0)
                    {
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button("Load"))
                        {
                            if (EditorUtility.DisplayDialog("Load Scenes", $"Are you sure you wanna load these scenes?", CartographerCommon.Messages.YesButton, CartographerCommon.Messages.NoButton))
                            {
                                var scenePaths = CartographerSaveLoad.GetStreamScenePaths(selection, sceneCollection);
                                CartographerSaveLoad.LoadStreamScenes(scenePaths, CartographerProgressCallback.Default);
                            }
                        }
                        if (GUILayout.Button("Remove"))
                        {
                            var message = "Are you sure you wanna remove these scenes?";
                            var scenePaths = CartographerSaveLoad.GetStreamScenePaths(selection, sceneCollection);
                            CheckAndRemoveScenes(message, scenePaths, cartorgapherParams, sceneCollection);
                        }
                        if (GUILayout.Button("Add"))
                        {
                            if (!string.IsNullOrEmpty(AreaName))
                            {
                                if (EditorUtility.DisplayDialog("Add Area", $"Are you sure you wanna create new Area with name {AreaName}? ", CartographerCommon.Messages.YesButton, CartographerCommon.Messages.NoButton))
                                {
                                    var sceneLoadArea = new CartographerParamsDef.SceneLoadArea();
                                    sceneLoadArea.Name = AreaName;
                                    sceneLoadArea.Collection = new SceneCollectionDef();

                                    sceneLoadArea.Collection.CollectByX = sceneCollection.CollectByX;
                                    sceneLoadArea.Collection.CollectByY = sceneCollection.CollectByY;
                                    sceneLoadArea.Collection.CollectByZ = sceneCollection.CollectByZ;

                                    sceneLoadArea.Collection.ScenePrefix = sceneCollection.ScenePrefix;
                                    sceneLoadArea.Collection.SceneFolder = sceneCollection.SceneFolder;

                                    sceneLoadArea.Collection.SceneNames = new List<string>();
                                    foreach (var item in selection)
                                    {
                                        var coordinates = CartographerCommon.GetStreamSceneCoordinates(item, sceneCollection);
                                        sceneLoadArea.Collection.SceneNames.Add(CartographerCommon.GetStreamSceneAssetName(coordinates, sceneCollection));
                                    }
                                    sceneLoadArea.Collection.SceneNames.Sort(CartographerNameComparer.Comparer);

                                    sceneLoadArea.Collection.SceneSize = sceneCollection.SceneSize;

                                    SharedCode.Utils.Vector3Int start;
                                    SharedCode.Utils.Vector3Int count;
                                    GetSceneStartAndCount(sceneCollection, selection, out start, out count);
                                    sceneLoadArea.Collection.SceneStart = start;
                                    sceneLoadArea.Collection.SceneCount = count;
                                    cartorgapherParams.SceneLoadAreas.Add(sceneLoadArea);
                                    GameResourcesLikeFileSaver.SaveFile(cartorgapherParams);
                                    Repaint();
                                    return;
                                }
                            }
                            else
                            {
                                EditorUtility.DisplayDialog("Add Area", "Area name is empty!", "OK");
                            }
                        }
                        AreaName = GUILayout.TextField(AreaName, GUILayout.Width(100), GUILayout.Height(20));
                        GUILayout.EndHorizontal();
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox($"{((cartorgapherParams == null) ? "CartorgapherParams is null\n" : "")}{((sceneCollection == null) ? "SceneCollection is null\n" : "")}{((minimap == null) ? "Minimap is null\n" : "")}", MessageType.Error);
                    //if (ShowSpecialButtons && (cartorgapherParams != null))
                    //{
                    //    EditorGUILayout.LabelField("Scene management", EditorStyles.boldLabel);
                    //    if (GUILayout.Button("Create"))
                    //    {
                    //        if (EditorUtility.DisplayDialog("Create Scenes", "Are you sure you wanna create scene collection?", CartographerCommon.Messages.YesButton, CartographerCommon.Messages.NoButton))
                    //        {
                    //            CartographerObsolete.CreateSceneCollection(cartorgapherParams, CartographerProgressCallback.Default);
                    //        }
                    //    }
                    //}
                }
                if (!string.IsNullOrEmpty(resultsFilePath))
                {
                    CartographerCommon.ShowTextFile(resultsFilePath, true, false);
                }
            }
        }
    }
}