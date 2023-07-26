using SharedCode.Aspects.Cartographer;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.Assertions;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using Assets.Src.Cartographer.Editor.TreeDataModel;

namespace Assets.Src.Cartographer.Editor
{
    public class GameObjectsTreeView : TreeViewWithTreeModel<GameObjectTreeElement>
    {
        const float DefaultRowHeight = 20.0f;

        enum Columns
        {
            Name,
            Count,
        }

        public enum SortOption
        {
            Name,
            Count,
        }

        SortOption[] ColumnSortOptions =
        {
            SortOption.Name,
            SortOption.Count
        };

        public event Action<IList<int>> InternalSelectionChanged;
        public event Action<int> InternalFocusChanged;

        public static void TreeToList(TreeViewItem root, IList<TreeViewItem> result)
        {
            if (root == null)
            {
                throw new NullReferenceException("The input 'T root' is null");
            }
            if (result == null)
            {
                throw new NullReferenceException("The input 'IList<T> result' list is null"); ;
            }

            result.Clear();

            if (root.children == null)
            {
                return;
            }

            Stack<TreeViewItem> stack = new Stack<TreeViewItem>();
            for (var i = root.children.Count - 1; i >= 0; i--)
            {
                stack.Push(root.children[i]);
            }

            while (stack.Count > 0)
            {
                TreeViewItem current = stack.Pop();
                result.Add(current);

                if (current.hasChildren && current.children[0] != null)
                {
                    for (int i = current.children.Count - 1; i >= 0; i--)
                    {
                        stack.Push(current.children[i]);
                    }
                }
            }
        }

        public GameObjectsTreeView(TreeViewState state, MultiColumnHeader multicolumnHeader, TreeModel<GameObjectTreeElement> model) : base(state, multicolumnHeader, model)
        {
            Assert.AreEqual(ColumnSortOptions.Length, Enum.GetValues(typeof(Columns)).Length, "Ensure number of sort options are in sync with number of MyColumns enum values");

            rowHeight = DefaultRowHeight;
            columnIndexForTreeFoldouts = 0;
            multicolumnHeader.sortingChanged += OnSortingChanged;

            Reload();
        }

        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            var rows = base.BuildRows(root);
            SortIfNeeded(root, rows);
            return rows;
        }

        void OnSortingChanged(MultiColumnHeader multiColumnHeader)
        {
            SortIfNeeded(rootItem, GetRows());
        }

        void SortIfNeeded(TreeViewItem root, IList<TreeViewItem> rows)
        {
            if (rows.Count <= 1)
                return;

            if (multiColumnHeader.sortedColumnIndex == -1)
            {
                return;
            }
            SortByMultipleColumns();
            TreeToList(root, rows);
            Repaint();
        }

        void SortByMultipleColumns()
        {
            var sortedColumns = multiColumnHeader.state.sortedColumns;

            if (sortedColumns.Length == 0)
                return;

            var myTypes = rootItem.children.Cast<TreeViewElement<GameObjectTreeElement>>();
            var orderedQuery = InitialOrder(myTypes, sortedColumns);
            for (int i = 1; i < sortedColumns.Length; i++)
            {
                SortOption sortOption = ColumnSortOptions[sortedColumns[i]];
                bool ascending = multiColumnHeader.IsSortedAscending(sortedColumns[i]);

                switch (sortOption)
                {
                    case SortOption.Name:
                        orderedQuery = orderedQuery.ThenBy(l => l.data.name, ascending);
                        break;
                    case SortOption.Count:
                        orderedQuery = orderedQuery.ThenBy(l => l.data.Count, ascending);
                        break;
                }
            }
            rootItem.children = orderedQuery.Cast<TreeViewItem>().ToList();
        }

        IOrderedEnumerable<TreeViewElement<GameObjectTreeElement>> InitialOrder(IEnumerable<TreeViewElement<GameObjectTreeElement>> myTypes, int[] history)
        {
            SortOption sortOption = ColumnSortOptions[history[0]];
            bool ascending = multiColumnHeader.IsSortedAscending(history[0]);
            switch (sortOption)
            {
                case SortOption.Name:
                    return myTypes.Order(l => l.data.name, ascending);
                case SortOption.Count:
                    return myTypes.Order(l => l.data.Count, ascending);
                default:
                    Assert.IsTrue(false, "Unhandled enum");
                    break;
            }

            // default
            return myTypes.Order(l => l.data.name, ascending);
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            var item = (TreeViewElement<GameObjectTreeElement>)args.item;

            for (int i = 0; i < args.GetNumVisibleColumns(); ++i)
            {
                CellGUI(args.GetCellRect(i), item, (Columns)args.GetColumn(i), ref args);
            }
        }

        void CellGUI(Rect cellRect, TreeViewElement<GameObjectTreeElement> item, Columns column, ref RowGUIArgs args)
        {
            // Center cell rect vertically (makes it easier to place controls, icons etc in the cells)
            CenterRectUsingSingleLineHeight(ref cellRect);
            switch (column)
            {
                case Columns.Name:
                {
                    args.rowRect = cellRect;
                    base.RowGUI(args);
                }
                break;

                case Columns.Count:
                    string value = item.data.Count.ToString();
                    DefaultGUI.LabelRightAligned(cellRect, value, args.selected, args.focused);
                    break;
            }
        }

        protected override bool CanMultiSelect(TreeViewItem item)
        {
            return true;
        }

        public static MultiColumnHeaderState CreateDefaultMultiColumnHeaderState(float treeViewWidth)
        {
            var columns = new[]
            {
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Name"),
                    headerTextAlignment = TextAlignment.Left,
                    sortedAscending = true,
                    sortingArrowAlignment = TextAlignment.Center,
                    width = 150,
                    minWidth = 60,
                    autoResize = true,
                    allowToggleVisibility = false
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Count", "How much times each prefab is used"),
                    headerTextAlignment = TextAlignment.Right,
                    sortedAscending = true,
                    sortingArrowAlignment = TextAlignment.Center,
                    width = 110,
                    minWidth = 60,
                    autoResize = true
                }
            };

            Assert.AreEqual(columns.Length, Enum.GetValues(typeof(Columns)).Length, "Number of columns should match number of enum values: You probably forgot to update one of them.");

            var state = new MultiColumnHeaderState(columns);
            return state;
        }

        protected override void SelectionChanged(IList<int> selectedIds)
        {
            InternalSelectionChanged?.Invoke(selectedIds);
        }

        protected override void DoubleClickedItem(int focusedId)
        {
            InternalFocusChanged?.Invoke(focusedId);
        }
    }

    public class SelectionHelper
    {
        public static IList<GameObjectTreeElement> GetGameObjectTreeElements(CartographerGameObjectTree gameObjectTree, CartographerGameObjectTree.TreeType type, bool useDescription)
        {
            var tree = gameObjectTree.GetTree(type);

            var treeElements = new List<GameObjectTreeElement>();
            var root = new GameObjectTreeElement("Root", -1, 0);
            treeElements.Add(root);
            if (tree != null)
            {
                foreach (var keyValuePair in tree)
                {
                    var node = keyValuePair.Value;
                    var nodeName = node.Name;
                    var leafs = node.Leafs.Count;
                    var nodeId = node.Key;
                    var newTreeNode = new GameObjectTreeElement(nodeName, 0, nodeId);
                    treeElements.Add(newTreeNode);
                    var nodeCount = 0;
                    node.Leafs.Sort((left, right) =>
                    {
                        var result = right.Count.CompareTo(left.Count);
                        if (result == 0)
                        {
                            return left.Name.CompareTo(right.Name);
                        }
                        return result;
                    });
                    for (var index = 0; index < leafs; ++index)
                    {
                        var leaf = node.Leafs[index];
                        var leafName = (useDescription && !string.IsNullOrEmpty(leaf.Description)) ? $"{leaf.Name}, <{leaf.Description}>" : leaf.Name;
                        var leafCount = leaf.Count;
                        var leafId = leaf.Key;
                        treeElements.Add(new GameObjectTreeElement(leafName, 1, leafId) { Count = leafCount });
                        nodeCount += leafCount;
                    }
                    newTreeNode.Count = nodeCount;
                }
            }
            return treeElements;
        }

        public static void ClearCurrentSelection()
        {
            Selection.objects = null;
        }

        public static void FocusObject(int objectID)
        {
            SceneView.FrameLastActiveSceneView();
        }

        public static void SelectObjects(CartographerGameObjectTree gameObjectTree, IList<int> objectIds)
        {
            List<GameObject> newSelection = new List<GameObject>();
            foreach (var objectId in objectIds)
            {
                CartographerGameObjectTree.Element element;
                if (gameObjectTree.Register.TryGetElement(objectId, out element))
                {
                    var node = element as CartographerGameObjectTree.Node;
                    if (node != null)
                    {
                        foreach (var leaf in node.Leafs)
                        {
                            if (leaf.GameObject != null)
                            {
                                newSelection.Add(leaf.GameObject);
                            }
                        }
                    }
                    else
                    {
                        var leaf = element as CartographerGameObjectTree.Leaf;
                        if ((leaf != null) && (leaf.GameObject != null))
                        {
                            newSelection.Add(leaf.GameObject);
                        }
                    }
                }
            }
            Selection.objects = newSelection.ToArray();
        }

        public static GameObject FindPrefabInCurrentSelection()
        {
            GameObject result = null;
            var selection = Selection.gameObjects;
            foreach (var gameObject in selection)
            {
                var prefabStatus = PrefabUtility.GetPrefabInstanceStatus(gameObject);
                if (prefabStatus == PrefabInstanceStatus.Connected)
                {
                    result = PrefabUtility.GetCorrespondingObjectFromSource(gameObject);
                }
                else
                {
                    var prefabSearch = new CartographerCommon.PrefabSearch();
                    var fixedName = CartographerCommon.PrefabSearch.FixNameForPrefabSearch(gameObject.name);
                    {
                        var prefabsFound = prefabSearch.FindAssets($"{fixedName} t:Prefab");
                        if ((prefabsFound != null) && (prefabsFound.Length > 0))
                        {
                            var id = prefabsFound[0];
                            var path = AssetDatabase.GUIDToAssetPath(id);
                            result = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
                            break;
                        }
                    }
                    {
                        var modelsFound = prefabSearch.FindAssets($"{fixedName} t:Model");
                        if ((modelsFound != null) && (modelsFound.Length > 0))
                        {
                            var id = modelsFound[0];
                            var path = AssetDatabase.GUIDToAssetPath(id);
                            result = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
                            break;
                        }
                    }
                }
                break;
            }
            if (result != null)
            {
                EditorGUIUtility.PingObject(result);
            }
            return result;
        }

        public static void ReplaceGameObjectsInCurrentSelection(GameObject replacePrefab)
        {
            var selection = Selection.gameObjects;
            for (var index = (selection.Length - 1); index >= 0; --index)
            {
                var selected = selection[index];
                var newObject = PrefabUtility.InstantiatePrefab(replacePrefab) as GameObject;
                if (newObject != null)
                {
                    Undo.RegisterCreatedObjectUndo(newObject, "Replace With Prefabs");
                    newObject.name = selected.name;
                    newObject.transform.parent = selected.transform.parent;
                    newObject.transform.localPosition = selected.transform.localPosition;
                    newObject.transform.localRotation = selected.transform.localRotation;
                    newObject.transform.localScale = selected.transform.localScale;
                    newObject.transform.SetSiblingIndex(selected.transform.GetSiblingIndex());
                    CartographerCommon.MakeAdditionalReplacement(newObject, selected, replacePrefab);
                    Undo.DestroyObjectImmediate(selected);
                }
            }
        }

        public static void DeleteGameObjectsInCurrentSelection()
        {
            var scenes = new HashSet<Scene>();
            List<GameObject> currentSelection = new List<GameObject>(Selection.gameObjects);
            Selection.objects = null;
            foreach (var gameObject in currentSelection)
            {
                scenes.Add(gameObject.scene);
                Undo.DestroyObjectImmediate(gameObject);
            }
            foreach (var scene in scenes)
            {
                EditorSceneManager.MarkSceneDirty(scene);
            }
        }
    }

    [Serializable]
    public class GameObjectTreeElement : TreeElement
    {
        public int Count;
        public GameObjectTreeElement(string name, int depth, int id) : base(name, depth, id) { }
    }

    public class MeshRestrictions
    {
        public float MinLOD0 { get; set; } = 0.9f;
        public float MinCutoff { get; set; } = 0.018f;
        public float MaxCutoff { get; set; } = 0.022f;
        public int MinLODCount { get; set; } = 3;
        public int MaxRenderersCount { get; set; } = 10;
        public int MaxLODGroupsCount { get; set; } = 1;
        public int MaxLastLODPolycount { get; set; } = 300;
        public bool AllowNonLODRenderers { get; set; } = false;
        public bool AllowNonMeshRenderers { get; set; } = false;

        public void Validate()
        {
            if (MinLOD0 > 1.0f) { MinLOD0 = 1.0f; }
            else if (MinLOD0 < 0.0f) { MinLOD0 = 0.0f; }
            if (MinCutoff > 1.0f) { MinCutoff = 1.0f; }
            else if (MinCutoff < 0.0f) { MinCutoff = 0.0f; }
            if (MaxCutoff > 1.0f) { MaxCutoff = 1.0f; }
            else if (MaxCutoff < 0.0f) { MaxCutoff = 0.0f; }
            if (MaxCutoff < MinCutoff) { MaxCutoff = MinCutoff; }
            if (MinLODCount < 0) { MinLODCount = 0; }
            if (MaxRenderersCount < 0) { MaxRenderersCount = 0; }
            if (MaxLODGroupsCount < 0) { MaxLODGroupsCount = 0; }
            if (MaxLastLODPolycount < 0) { MaxLastLODPolycount = 0; }
        }
    }

    public class CSOCollectSceneStatsArguments
    {
        public CartographerCommon.FormatArguments FormatArguments { get; } = new CartographerCommon.FormatArguments();
        public MeshRestrictions MeshRestrictions { get; } = new MeshRestrictions();

        public bool IgnoreUnknownCartographerSceneTypes { get; set; } = true;

        public bool CollectPrefabsUsage { get; set; } = true;
        public bool CollectComponentsUsage { get; set; } = true;
        public bool CollectGameObjects { get; set; } = true;
        public float CheckDuplicatesThreshold { get; set; } = CartographerGameObjectTree.DuplicateChecker.DefaultDuplicatesThreshold;
        public float CheckNonUniformScalesThreshold { get; set; } = CartographerGameObjectTree.DuplicateChecker.DefaultNonUniformScalesThreshold;

        public bool KeepGameObjectReferences { get; set; } = false;

        public string ResultsFilePath { get; set; } = "Components.txt";
    }

    public class CartographerSceneStatsEditorWindow : EditorWindow
    {
        private static int DefaultTabsCountInColumn = 3;

        private struct TabDataElement
        {
            public string Name { get; set; }
            public CartographerGameObjectTree.TreeType TreeType { get; set; }
            public bool UseDescription { get; set; }
            public TabDataElement(string name, CartographerGameObjectTree.TreeType treeType, bool useDescription)
            {
                Name = name;
                TreeType = treeType;
                UseDescription = useDescription;
            }
        }

        private static TabDataElement[] DefaultTabsData =
        {
            new TabDataElement("Big Game Objects", CartographerGameObjectTree.TreeType.BigGameObjects, false),
            new TabDataElement("Duplicates", CartographerGameObjectTree.TreeType.Duplicates, false),
            new TabDataElement("Renderers", CartographerGameObjectTree.TreeType.Renderers, false),
            //
            new TabDataElement("Disconnected", CartographerGameObjectTree.TreeType.DisconnectedPrefabs, false),
            new TabDataElement("Non Uniform Scale", CartographerGameObjectTree.TreeType.NonUniformScales, false),
            new TabDataElement("Null Components", CartographerGameObjectTree.TreeType.NullComponents, false),
            //
            new TabDataElement("Non Prefabs", CartographerGameObjectTree.TreeType.NonPrefabs, false),
            new TabDataElement("Negative Scale", CartographerGameObjectTree.TreeType.NegativeScales, false),
            new TabDataElement("Non LOD Renderers", CartographerGameObjectTree.TreeType.RenderersOutsideLODGroups, false),
            //
            new TabDataElement("Empty", CartographerGameObjectTree.TreeType.Empty, false),
            new TabDataElement("Bad LOD Transforms", CartographerGameObjectTree.TreeType.LODsWithNonMatchingTransforms, false),
            new TabDataElement("Bad LOD Renderers", CartographerGameObjectTree.TreeType.LODsWithBadRenderers, true),
        };

        public class Settings
        {
            public CartographerGameObjectTree.TreeType ActiveTreeType { get; set; } = CartographerGameObjectTree.TreeType.None;
            public bool ActiveUseDescription { get; set; } = false;
            public CSOCollectSceneStatsArguments CollectSceneStatsArguments { get; set; } = new CSOCollectSceneStatsArguments();
        }

        private CartographerBehaviour cartographerBehaviour = null;

        private static Settings settings = new Settings();

        private CartographerGameObjectTree gameObjectTree = new CartographerGameObjectTree();

        [SerializeField] private GameObject replacePrefab = null;
        [SerializeField] TreeViewState prefabsTreeViewState = new TreeViewState();
        [SerializeField] MultiColumnHeaderState prefabsMultiColumnHeaderState = null;
        public GameObjectsTreeView prefabTreeView { get; private set; } = null;
        private SearchField prefabSearchField = null;

        public static void ToggleWindow(bool show)
        {
            if (CartographerCommon.IsEditor())
            {
                var window = GetWindow<CartographerSceneStatsEditorWindow>("Scene stats");
                window.Show();
            }
        }

        public static Settings GetSettings()
        {
            if (CartographerCommon.IsEditor())
            {
                return settings;
            }
            return null;
        }

        [MenuItem("Level Design/Scene Stats")]
        public static void CartographerSceneStatsEditorWindowMenu()
        {
            ToggleWindow(true);
        }

        private CartographerBehaviour GetCartograper()
        {
            if (cartographerBehaviour == null)
            {
                cartographerBehaviour = FindObjectOfType<CartographerBehaviour>();
            }
            return cartographerBehaviour;
        }

        private static CartographerParamsDef GetCartographerParams(CartographerBehaviour cartograper)
        {
            return cartograper?.CartographerParams?.Get<CartographerParamsDef>() ?? null;
        }

        private static SceneCollectionDef GetSceneCollection(CartographerBehaviour cartograper)
        {
            return cartograper?.SceneCollection?.Get<SceneCollectionDef>() ?? null;
        }

        private string ShowSceneStats(CartographerBehaviour cartograper)
        {
            var resultsFilePath = string.Empty;
            try
            {
                if (GUILayout.Button("Reset to default values"))
                {
                    settings.CollectSceneStatsArguments = new CSOCollectSceneStatsArguments();
                }
                GUILayout.Space(10);
                settings.CollectSceneStatsArguments.FormatArguments.Verboose = EditorGUILayout.Toggle("Verboose:", settings.CollectSceneStatsArguments.FormatArguments.Verboose);
                settings.CollectSceneStatsArguments.FormatArguments.InsertSpaces = EditorGUILayout.Toggle("Use spaces as delimiter:", settings.CollectSceneStatsArguments.FormatArguments.InsertSpaces);
                if (settings.CollectSceneStatsArguments.FormatArguments.InsertSpaces)
                {
                    settings.CollectSceneStatsArguments.FormatArguments.Spaces = EditorGUILayout.IntField("Spaces in delimiter:", settings.CollectSceneStatsArguments.FormatArguments.Spaces);
                }
                settings.CollectSceneStatsArguments.FormatArguments.Delimiter = EditorGUILayout.TextField("Secondary delimiter:", settings.CollectSceneStatsArguments.FormatArguments.Delimiter);
                GUILayout.Space(10);
                settings.CollectSceneStatsArguments.IgnoreUnknownCartographerSceneTypes = EditorGUILayout.Toggle("Ignore unknown scenes:", settings.CollectSceneStatsArguments.IgnoreUnknownCartographerSceneTypes);
                GUILayout.Space(10);
                settings.CollectSceneStatsArguments.CollectPrefabsUsage = EditorGUILayout.Toggle("Collect prefabs:", settings.CollectSceneStatsArguments.CollectPrefabsUsage);
                settings.CollectSceneStatsArguments.CollectComponentsUsage = EditorGUILayout.Toggle("Collect components:", settings.CollectSceneStatsArguments.CollectComponentsUsage);
                settings.CollectSceneStatsArguments.CollectGameObjects = EditorGUILayout.Toggle("Collect objects:", settings.CollectSceneStatsArguments.CollectGameObjects);
                settings.CollectSceneStatsArguments.CheckDuplicatesThreshold = EditorGUILayout.FloatField("Coordinates threshold:", settings.CollectSceneStatsArguments.CheckDuplicatesThreshold);
                settings.CollectSceneStatsArguments.CheckNonUniformScalesThreshold = EditorGUILayout.FloatField("Scales threshold:", settings.CollectSceneStatsArguments.CheckNonUniformScalesThreshold);
                GUILayout.Space(10);
                settings.CollectSceneStatsArguments.MeshRestrictions.MinLOD0 = EditorGUILayout.FloatField("Min LOD0:", settings.CollectSceneStatsArguments.MeshRestrictions.MinLOD0);
                settings.CollectSceneStatsArguments.MeshRestrictions.MinCutoff = EditorGUILayout.FloatField("Min Cutoff:", settings.CollectSceneStatsArguments.MeshRestrictions.MinCutoff);
                settings.CollectSceneStatsArguments.MeshRestrictions.MaxCutoff = EditorGUILayout.FloatField("Max Cutoff:", settings.CollectSceneStatsArguments.MeshRestrictions.MaxCutoff);
                settings.CollectSceneStatsArguments.MeshRestrictions.MinLODCount = EditorGUILayout.IntField("Min LODs Count:", settings.CollectSceneStatsArguments.MeshRestrictions.MinLODCount);
                settings.CollectSceneStatsArguments.MeshRestrictions.MaxRenderersCount = EditorGUILayout.IntField("Max Renders Count:", settings.CollectSceneStatsArguments.MeshRestrictions.MaxRenderersCount);
                settings.CollectSceneStatsArguments.MeshRestrictions.MaxLODGroupsCount = EditorGUILayout.IntField("Max LODGroups Count:", settings.CollectSceneStatsArguments.MeshRestrictions.MaxLODGroupsCount);
                settings.CollectSceneStatsArguments.MeshRestrictions.MaxLastLODPolycount = EditorGUILayout.IntField("Max Last LOD Polycount:", settings.CollectSceneStatsArguments.MeshRestrictions.MaxLastLODPolycount);
                settings.CollectSceneStatsArguments.MeshRestrictions.AllowNonLODRenderers = EditorGUILayout.Toggle("Allow Non LOD Renders:", settings.CollectSceneStatsArguments.MeshRestrictions.AllowNonLODRenderers);
                settings.CollectSceneStatsArguments.MeshRestrictions.AllowNonMeshRenderers = EditorGUILayout.Toggle("Allow Non Mesh Renders:", settings.CollectSceneStatsArguments.MeshRestrictions.AllowNonMeshRenderers);
                settings.CollectSceneStatsArguments.MeshRestrictions.Validate();
                GUILayout.Space(10);
                settings.CollectSceneStatsArguments.ResultsFilePath = EditorGUILayout.TextField("Result:", settings.CollectSceneStatsArguments.ResultsFilePath);
                GUILayout.Space(10);
                var checkPressed = GUILayout.Button("Check Opened Scenes");
                var savePressed = GUILayout.Button("Save Opened Scenes");
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
                        var arguments = new CSOCheckScenesArguments();
                        arguments.IgnoreUnknownCartographerSceneTypes = settings.CollectSceneStatsArguments.IgnoreUnknownCartographerSceneTypes;
                        arguments.FormatArguments.CopyFrom(settings.CollectSceneStatsArguments.FormatArguments);
                        var operation = new CSOCheckScenes(arguments);
                        CartographerSceneOperation.Operate(operation, GetCartographerParams(cartograper), GetSceneCollection(cartograper), true, CartographerSceneType.None, CartographerProgressCallback.Default);
                        if (savePressed)
                        {
                            CartographerSaveLoad.SaveAllOpenedStreamScenes(cartograper.gameObject.scene, GetCartographerParams(cartograper), GetSceneCollection(cartograper), true, true, true, false, null, CartographerProgressCallback.Default);
                        }
                        resultsFilePath = arguments.ResultsFilePath;
                    }
                }
                var collectStatsOnOpenedScenesPressed = GUILayout.Button("Collect Stats on Opened Scenes");
                var collectStatsOnAllScenesPressed = GUILayout.Button("Collect Stats on All Scenes");
                if (collectStatsOnOpenedScenesPressed || collectStatsOnAllScenesPressed)
                {
                    var proseed = true;
                    if (collectStatsOnOpenedScenesPressed)
                    {
                        proseed = EditorUtility.DisplayDialog("Collect Stats on Opened Scenes", "Are you sure you wanna collect stats on opened scenes?", CartographerCommon.Messages.YesButton, CartographerCommon.Messages.NoButton);
                    }
                    else
                    {
                        proseed = EditorUtility.DisplayDialog("Collect Stats on All Scenes", "Are you sure you wanna collect stats on all scenes?", CartographerCommon.Messages.YesButton, CartographerCommon.Messages.NoButton);
                    }
                    if (proseed)
                    {
                        var operation = new CSOCollectSceneStats(settings.CollectSceneStatsArguments);
                        CartographerSceneOperation.Operate(operation,
                                                            GetCartographerParams(cartograper),
                                                            GetSceneCollection(cartograper),
                                                            collectStatsOnOpenedScenesPressed,
                                                            CartographerSceneType.StreamCollection |
                                                            CartographerSceneType.BackgroundClient |
                                                            CartographerSceneType.BackgroundServer |
                                                            CartographerSceneType.MapDefClient |
                                                            CartographerSceneType.MapDefServer,
                                                            CartographerProgressCallback.Default);
                        resultsFilePath = settings.CollectSceneStatsArguments.ResultsFilePath;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"{e.Message}\t{e.StackTrace}");
                if (e.InnerException != null)
                {
                    Debug.LogError($"\t{ e.InnerException.Message}\t{ e.InnerException.StackTrace}");
                }
            }
            return resultsFilePath;
        }

        private void ShowReplacePrefab()
        {
            replacePrefab = (GameObject)EditorGUILayout.ObjectField("Replace with prefab", replacePrefab, typeof(GameObject), false);
            var canReplace = (replacePrefab != null);
            var replacePrefabType = canReplace ? PrefabUtility.GetPrefabAssetType(replacePrefab) : PrefabAssetType.MissingAsset;
            canReplace = (replacePrefabType == PrefabAssetType.Regular) && (Selection.gameObjects != null);
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Selection count: " + (Selection.gameObjects?.Length ?? 0), GUILayout.Width(146));
            if (GUILayout.Button(canReplace ? "Replace" : "Can't Replace") && canReplace)
            {
                if (EditorUtility.DisplayDialog("Replace Selected Game Objects", "Are you sure you wanna replace selected game objects?", CartographerCommon.Messages.YesButton, CartographerCommon.Messages.NoButton))
                {
                    SelectionHelper.ReplaceGameObjectsInCurrentSelection(replacePrefab);
                    CreatePrefabTreeView();
                }
            }
            if (GUILayout.Button("Find", GUILayout.Width(69)))
            {
                replacePrefab = SelectionHelper.FindPrefabInCurrentSelection();
            }
            GUILayout.EndHorizontal();
        }

        private void ShowPrefabsTree(Rect rect)
        {
            if (prefabTreeView != null)
            {
                prefabTreeView.OnGUI(rect);
            }
        }

        private void ShowPrefabsSearchBar(Rect rect)
        {
            if (prefabTreeView != null)
            {
                prefabTreeView.searchString = prefabSearchField.OnGUI(rect, prefabTreeView.searchString);
            }
        }

        private void ShowPrefabsButtons(Rect rect)
        {
            var width = rect.width / 2 - 1;
            var buttonRect0 = new Rect(rect.x, rect.y, width - 2, rect.height);
            var buttonRect1 = new Rect(rect.x + width + 2, rect.y, width, rect.height);

            if (GUI.Button(buttonRect0, "Reload"))
            {
                CreatePrefabTreeView();
            }
            if (GUI.Button(buttonRect1, "Delete"))
            {
                if (EditorUtility.DisplayDialog("Delete Selected Game Objects", "Are you sure you wanna delete selected game objects?", CartographerCommon.Messages.YesButton, CartographerCommon.Messages.NoButton))
                {
                    SelectionHelper.DeleteGameObjectsInCurrentSelection();
                    CreatePrefabTreeView();
                }
            }
        }

        private void DestroyPrefabTreeView()
        {
            if (prefabTreeView != null)
            {
                prefabTreeView.InternalSelectionChanged -= OnInternalSelectionChanged;
                prefabTreeView.InternalFocusChanged -= OnInternalFocusChanged;
                if (prefabSearchField != null)
                {
                    prefabSearchField.downOrUpArrowKeyPressed -= prefabTreeView.SetFocusAndEnsureSelectedItem;
                    prefabSearchField = null;
                }
                prefabTreeView = null;
            }
        }

        private void CreatePrefabTreeView()
        {
            DestroyPrefabTreeView();
            var treeType = settings.ActiveTreeType;
            var useDescription = settings.ActiveUseDescription;
            if (treeType != CartographerGameObjectTree.TreeType.None)
            {
                var cartographer = GetCartograper();
                gameObjectTree.Collect(GetCartographerParams(cartographer),
                                       GetSceneCollection(cartographer),
                                       treeType,
                                       settings.CollectSceneStatsArguments.CheckDuplicatesThreshold,
                                       settings.CollectSceneStatsArguments.CheckNonUniformScalesThreshold,
                                       settings.CollectSceneStatsArguments.IgnoreUnknownCartographerSceneTypes);
                var multiColumnHeader = new MultiColumnHeader(prefabsMultiColumnHeaderState);
                multiColumnHeader.ResizeToFit();
                var treeModel = new TreeModel<GameObjectTreeElement>(SelectionHelper.GetGameObjectTreeElements(gameObjectTree, treeType, useDescription));

                prefabTreeView = new GameObjectsTreeView(prefabsTreeViewState, multiColumnHeader, treeModel);
                prefabTreeView.InternalSelectionChanged += OnInternalSelectionChanged;
                prefabTreeView.InternalFocusChanged += OnInternalFocusChanged;

                prefabSearchField = new SearchField();
                prefabSearchField.downOrUpArrowKeyPressed += prefabTreeView.SetFocusAndEnsureSelectedItem;
            }
        }

        private void OnInternalSelectionChanged(IList<int> selectedIDs)
        {
            SelectionHelper.SelectObjects(gameObjectTree, selectedIDs);
        }

        private void OnInternalFocusChanged(int focusedId)
        {
            SelectionHelper.FocusObject(focusedId);
        }

        // unity methods --------------------------------------------------------------------------
        private void OnEnable()
        {
            GetCartograper();
            var headerState = GameObjectsTreeView.CreateDefaultMultiColumnHeaderState(position.width - 40);
            if (MultiColumnHeaderState.CanOverwriteSerializedFields(prefabsMultiColumnHeaderState, headerState))
            {
                MultiColumnHeaderState.OverwriteSerializedFields(prefabsMultiColumnHeaderState, headerState);
            }
            prefabsMultiColumnHeaderState = headerState;
            CreatePrefabTreeView();
        }

        private void OnDestroy()
        {
            DestroyPrefabTreeView();
            cartographerBehaviour = null;
        }

        void OnGUI()
        {
            GUILayout.Space(10);
            var cartograper = GetCartograper();
            if (cartograper == null)
            {
                EditorGUILayout.HelpBox($"Can't find Cartographer. Please, load CraterEditor scene.", MessageType.Error);
            }
            else
            {
                var horisontalSpace = 4;
                var verticalSpace = 3;
                var buttonHeight = 18;
                var buttonsWidth = 140;
                //
                var resultsFilePath = string.Empty;
                var previousTab = settings.ActiveTreeType;
                {
                    GUILayout.BeginVertical();
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(horisontalSpace);
                    if (GUILayout.Toggle(settings.ActiveTreeType == CartographerGameObjectTree.TreeType.None, "Scene Check, Save and Stats", EditorStyles.toolbarButton))
                    {
                        settings.ActiveTreeType = CartographerGameObjectTree.TreeType.None;
                        settings.ActiveUseDescription = false;
                    }
                    GUILayout.Space(horisontalSpace);
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                }
                {
                    GUILayout.BeginVertical();
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(horisontalSpace);
                    if (GUILayout.Toggle(settings.ActiveTreeType == CartographerGameObjectTree.TreeType.Prefabs, "Connected Prefabs", EditorStyles.toolbarButton))
                    {
                        settings.ActiveTreeType = CartographerGameObjectTree.TreeType.Prefabs;
                        settings.ActiveUseDescription = false;
                    }
                    GUILayout.Space(horisontalSpace);
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                }
                {
                    GUILayout.BeginVertical();
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(horisontalSpace);
                    {
                        GUILayout.BeginVertical();
                        for (int index = 0; index < DefaultTabsData.Length; ++index)
                        {
                            if ((index > 0) && ((index % DefaultTabsCountInColumn) == 0))
                            {
                                GUILayout.EndVertical();
                                GUILayout.BeginVertical();
                            }
                            var treeType = DefaultTabsData[index].TreeType;
                            if (GUILayout.Toggle(settings.ActiveTreeType == treeType, DefaultTabsData[index].Name, EditorStyles.toolbarButton))
                            {
                                settings.ActiveTreeType = treeType;
                                settings.ActiveUseDescription = DefaultTabsData[index].UseDescription;
                            }
                        }
                        GUILayout.EndVertical();
                    }
                    GUILayout.Space(horisontalSpace);
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                }
                {
                    GUILayout.BeginVertical();
                    if (settings.ActiveTreeType == CartographerGameObjectTree.TreeType.None)
                    {
                        resultsFilePath = ShowSceneStats(cartograper);
                    }
                    else
                    {
                        ShowReplacePrefab();
                        if (settings.ActiveTreeType != previousTab)
                        {
                            CreatePrefabTreeView();
                        }
                        var lastRect = GUILayoutUtility.GetLastRect();
                        var shift = lastRect.y + lastRect.height + verticalSpace;
                        var multiColumnTreeViewRect = new Rect(horisontalSpace, buttonHeight + 1 + shift, position.width - (horisontalSpace * 2), position.height - buttonHeight - 2 - shift - verticalSpace);
                        var toolbarRect = new Rect(horisontalSpace, shift, position.width - buttonsWidth - 4 - (horisontalSpace * 2), buttonHeight);
                        var buttonRect = new Rect(position.width - buttonsWidth - horisontalSpace, shift, buttonsWidth, buttonHeight);
                        ShowPrefabsTree(multiColumnTreeViewRect);
                        ShowPrefabsSearchBar(toolbarRect);
                        ShowPrefabsButtons(buttonRect);
                    }
                    GUILayout.EndVertical();
                }
                if (!string.IsNullOrEmpty(resultsFilePath))
                {
                    CartographerCommon.ShowTextFile(resultsFilePath, false, false);
                }
            }
        }
    }
}
