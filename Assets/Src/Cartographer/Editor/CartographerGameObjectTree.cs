using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using SharedCode.Aspects.Cartographer;
using AwesomeTechnologies.VegetationStudio;
using System;

namespace Assets.Src.Cartographer.Editor
{
    public class CartographerGameObjectTree
    {
        [Flags]
        public enum TreeType
        {
            None = 0x0000, // 0
            BigGameObjects = 0x0001, //1
            Prefabs = 0x0002, //2
            DisconnectedPrefabs = 0x0004, //3
            NonPrefabs = 0x0008, //4
            Empty = 0x0010, //5
            Duplicates = 0x0020, //6
            NonUniformScales = 0x0040, //7
            NegativeScales = 0x0080, //8
            LODsWithNonMatchingTransforms = 0x0100, //9
            LODsWithBadRenderers = 0x0200, //10
            RenderersOutsideLODGroups = 0x0400, //11
            NullComponents = 0x0800, //12
            Renderers = 0x1000, //13
        }

        public static TreeType GetTreeTypeByIndex(int index)
        {
            if (index > 0)
            {
                return (TreeType)(1 << (index - 1));
            }
            return TreeType.None;
        }

        public static int MinTreeTypeIndex = 1;
        public static int MaxTreeTypeIndex = 13;

        public class Element
        {
            public int Key { get; set; } = 0;
            public string Name { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
        }

        public class Leaf : Element
        {
            public GameObject GameObject { get; set; } = null;
            public int Count { get; set; } = 1;
        }

        public class Node : Element
        {
            public List<Leaf> Leafs { get; } = new List<Leaf>();
        }

        public class ErrorElement
        {
            public TreeType TreeType { get; set; } = TreeType.None;
            public GameObject GameObject { get; set; } = null;
            public string Description { get; set; } = string.Empty;
        }

        public class RegisterType
        {
            private int currentFreeKey = 0;
            private Dictionary<int, Element> registeredElements = new Dictionary<int, Element>();
            private Dictionary<TreeType, Dictionary<string, Node>> registeredNodesByName = new Dictionary<TreeType, Dictionary<string, Node>>();
            private Dictionary<TreeType, Dictionary<GameObject, Node>> registeredNodesByGameObject = new Dictionary<TreeType, Dictionary<GameObject, Node>>();

            private int RegisterKey()
            {
                var result = currentFreeKey;
                ++currentFreeKey;
                return result;
            }

            private void UnregisterKey(int key)
            {
            }

            public void Clear()
            {
                registeredNodesByName.Clear();
                registeredNodesByGameObject.Clear();
                registeredElements.Clear();
                currentFreeKey = 0;
            }

            public void RegisterNode(TreeType treeType, string name, Node node)
            {
                Dictionary<string, Node> nodesByName;
                if (!registeredNodesByName.TryGetValue(treeType, out nodesByName))
                {
                    nodesByName = new Dictionary<string, Node>();
                    registeredNodesByName.Add(treeType, nodesByName);
                }
                nodesByName.Add(name, node);
            }

            public bool TryGetNode(TreeType treeType, string name, out Node node)
            {
                node = null;
                Dictionary<string, Node> nodesByName;
                if (registeredNodesByName.TryGetValue(treeType, out nodesByName))
                {
                    return nodesByName.TryGetValue(name, out node);
                }
                return false;
            }

            public void RegisterNode(TreeType treeType, GameObject gameObject, Node node)
            {
                Dictionary<GameObject, Node> nodesByGameObject;
                if (!registeredNodesByGameObject.TryGetValue(treeType, out nodesByGameObject))
                {
                    nodesByGameObject = new Dictionary<GameObject, Node>();
                    registeredNodesByGameObject.Add(treeType, nodesByGameObject);
                }
                nodesByGameObject.Add(gameObject, node);
            }

            public bool TryGetNode(TreeType treeType, GameObject gameObject, out Node node)
            {
                node = null;
                Dictionary<GameObject, Node> nodesByGameObject;
                if (registeredNodesByGameObject.TryGetValue(treeType, out nodesByGameObject))
                {
                    return nodesByGameObject.TryGetValue(gameObject, out node);
                }
                return false;
            }

            public Element RegisterElement(Element element, string name, string description)
            {
                var key = RegisterKey();
                element.Key = key;
                element.Name = name;
                element.Description = description;
                registeredElements.Add(key, element);
                return element;
            }

            public bool TryGetElement(int key, out Element element)
            {
                return registeredElements.TryGetValue(key, out element);
            }
        }

        public class DuplicateChecker
        {
            public static readonly float DefaultDuplicatesThreshold = 0.0001f;
            public static readonly float DefaultNonUniformScalesThreshold = 1.05f;

            private class DuplicateCell
            {
                public Vector3 Key { get; set; } = Vector3.zero;
                public List<GameObject> GameObjects { get; } = new List<GameObject>();
            }

            private bool enable = true;
            private float threshold = DefaultDuplicatesThreshold;
            private double thresholdSqr = 1.0 * DefaultDuplicatesThreshold * DefaultDuplicatesThreshold;
            private double thresholdMultiplier = 1.0 / DefaultDuplicatesThreshold;
            private Dictionary<Vector3, DuplicateCell> Cells { get; } = new Dictionary<Vector3, DuplicateCell>();

            private static Vector3 CreateKey(GameObject gameObject, double _thresholdMultiplier)
            {
                double x = Math.Floor(gameObject.transform.position.x * _thresholdMultiplier) / _thresholdMultiplier;
                double y = Math.Floor(gameObject.transform.position.y * _thresholdMultiplier) / _thresholdMultiplier;
                double z = Math.Floor(gameObject.transform.position.z * _thresholdMultiplier) / _thresholdMultiplier;
                return new Vector3((float)x, (float)y, (float)z);
            }

            private void GetLookupGameObjects(Vector3 key, List<GameObject> lookupGameObjects)
            {
                for (var x = -1; x < 2; ++x)
                {
                    for (var y = -1; y < 2; ++y)
                    {
                        for (var z = -1; z < 2; ++z)
                        {
                            var lookupKey = new Vector3(key.x + (x * threshold), key.y + (y * threshold), key.z + (z * threshold));
                            DuplicateCell cell;
                            if (Cells.TryGetValue(lookupKey, out cell))
                            {
                                lookupGameObjects.AddRange(cell.GameObjects);
                            }
                        }
                    }
                }
            }

            private void AddGameObject(GameObject gameObject)
            {
                if (!enable || (gameObject == null))
                {
                    return;
                }
                var key = CreateKey(gameObject, thresholdMultiplier);
                DuplicateCell cell;
                if (!Cells.TryGetValue(key, out cell))
                {
                    cell = new DuplicateCell();
                    cell.Key = key;
                    Cells.Add(key, cell);
                }
                cell.GameObjects.Add(gameObject);
            }

            private static void CheckForDistance(Dictionary<GameObject, List<GameObject>> duplicateGameObjcts, Dictionary<GameObject, List<GameObject>> checkGameObjects, GameObject gameObject, List<GameObject> lookupGameObjects, double _thresholdSqr)
            {
                var instanceId = gameObject.GetInstanceID();
                foreach (var lookupGameObject in lookupGameObjects)
                {
                    if (instanceId != lookupGameObject.GetInstanceID())
                    {
                        var x = (1.0 * gameObject.transform.position.x) - (1.0 * lookupGameObject.transform.position.x);
                        var y = (1.0 * gameObject.transform.position.y) - (1.0 * lookupGameObject.transform.position.y);
                        var z = (1.0 * gameObject.transform.position.z) - (1.0 * lookupGameObject.transform.position.z);
                        if (((x * x) + (y * y) + (z * z)) <= _thresholdSqr)
                        {
                            var gameObjectName = CartographerCommon.PrefabSearch.FixNameForPrefabSearch(gameObject.name);
                            var lookupGameObjectName = CartographerCommon.PrefabSearch.FixNameForPrefabSearch(lookupGameObject.name);
                            if (string.Equals(gameObjectName, lookupGameObjectName, StringComparison.OrdinalIgnoreCase))
                            {
                                List<GameObject> list;
                                if (checkGameObjects.TryGetValue(gameObject, out list))
                                {
                                    if (!checkGameObjects.ContainsKey(lookupGameObject))
                                    {
                                        list.Add(lookupGameObject);
                                        checkGameObjects.Add(lookupGameObject, list);
                                    }
                                }
                                else
                                {
                                    if (checkGameObjects.TryGetValue(lookupGameObject, out list))
                                    {
                                        list.Add(gameObject);
                                        checkGameObjects.Add(gameObject, list);
                                    }
                                    else
                                    {
                                        list = new List<GameObject>();
                                        list.Add(gameObject);
                                        list.Add(lookupGameObject);
                                        duplicateGameObjcts.Add(gameObject, list);
                                        checkGameObjects.Add(gameObject, list);
                                        checkGameObjects.Add(lookupGameObject, list);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            public void Clear()
            {
                Cells.Clear();
            }

            public void Start(bool _enable, float _threshhold)
            {
                Clear();
                enable = _enable;
                if (_threshhold < Mathf.Epsilon)
                {
                    _threshhold = Mathf.Epsilon;
                }
                threshold = _threshhold;
                thresholdSqr = 1.0 * _threshhold * _threshhold;
                thresholdMultiplier = 1.0 / _threshhold;
            }

            public void Collect(Scene scene, GameObject gameObject, CartographerSceneType cartographerSceneTypeMask)
            {
                AddGameObject(gameObject);
            }

            public Dictionary<GameObject, List<GameObject>> GetDuplicateGameObjcts()
            {
                var result = new Dictionary<GameObject, List<GameObject>>();
                var check = new Dictionary<GameObject, List<GameObject>>();
                var lookupGameObjects = new List<GameObject>();
                foreach (var keyCellPair in Cells)
                {
                    var cell = keyCellPair.Value;
                    lookupGameObjects.Clear();
                    GetLookupGameObjects(cell.Key, lookupGameObjects);
                    if (lookupGameObjects.Count > 0)
                    {
                        foreach (var gameObject in cell.GameObjects)
                        {
                            CheckForDistance(result, check, gameObject, lookupGameObjects, thresholdSqr);
                        }
                    }
                }
                return result;
            }

            public void Print(List<string> lines, CartographerCommon.FormatArguments formatArguments, bool addEmptyLine)
            {
                var duplicateGameObjcts = GetDuplicateGameObjcts();
                if (duplicateGameObjcts.Count > 0)
                {
                    if (addEmptyLine) { lines.Add($""); }
                    lines.Add($"--- Game objects with the same position");
                    var newLines = new List<string>();
                    newLines.Add($"Scene\tLookup name\tName\tPath\tCoordinates");
                    foreach (var duplicateGameObjctsPair in duplicateGameObjcts)
                    {
                        if (newLines.Count > 1)
                        {
                            newLines.Add($"\t\t\t");
                        }
                        foreach (var duplicateGameObject in duplicateGameObjctsPair.Value)
                        {
                            var sceneName = duplicateGameObject.scene.name;
                            var lookupName = CartographerCommon.PrefabSearch.FixNameForPrefabSearch(duplicateGameObject.name);
                            var name = duplicateGameObject.name;
                            var fullName = CartographerCommon.GetFullName(duplicateGameObject);
                            newLines.Add($"{sceneName}\t{lookupName}\t{name}\t{fullName}\t({duplicateGameObject.transform.position.x}, {duplicateGameObject.transform.position.y}, {duplicateGameObject.transform.position.z})");
                        }
                    }
                    lines.AddRange(CartographerCommon.FixLines(newLines, formatArguments));
                }
            }
        }

        public RegisterType Register { get; } = new RegisterType();

        private Dictionary<TreeType, Dictionary<int, Node>> trees = new Dictionary<TreeType, Dictionary<int, Node>>();
        private Dictionary<TreeType, List<ErrorElement>> errorLists = new Dictionary<TreeType, List<ErrorElement>>();

        private DuplicateChecker duplicateChecker = new DuplicateChecker();
        private float nonUniformScalesThreshold = DuplicateChecker.DefaultNonUniformScalesThreshold;
        private bool ignoreUnknownCartographerSceneTypes = true;

        public Dictionary<int, Node> GetTree(TreeType treeType)
        {
            Dictionary<int, Node> tree;
            if (!trees.TryGetValue(treeType, out tree))
            {
                tree = new Dictionary<int, Node>();
                trees.Add(treeType, tree);
            }
            return tree;
        }

        public List<ErrorElement> GetErrors(TreeType treeType)
        {
            List<ErrorElement> errorList;
            if (!errorLists.TryGetValue(treeType, out errorList))
            {
                errorList = new List<ErrorElement>();
                errorLists.Add(treeType, errorList);
            }
            return errorList;
        }

        // helpers --------------------------------------------------------------------------------
        private void AddToTreeByStringAndCount(TreeType treeType, GameObject leafGameObject, string nodeGameObjectName, int count, string description)
        {
            var tree = GetTree(treeType);
            Node node;
            if (!Register.TryGetNode(treeType, nodeGameObjectName, out node))
            {
                node = Register.RegisterElement(new Node(), nodeGameObjectName, string.Empty) as Node;
                Register.RegisterNode(treeType, nodeGameObjectName, node);
                tree.Add(node.Key, node);
            }
            var leaf = Register.RegisterElement(new Leaf(), leafGameObject.name, description) as Leaf;
            leaf.GameObject = leafGameObject;
            leaf.Count = count;
            node.Leafs.Add(leaf);
        }

        private void AddToTreeByName(TreeType treeType, GameObject leafGameObject, string description)
        {
            var nodeGameObjectName = CartographerCommon.PrefabSearch.FixNameForPrefabSearch(leafGameObject.name);
            AddToTreeByStringAndCount(treeType, leafGameObject, nodeGameObjectName, 1, description);
        }

        private void AddToTreeByNameAndCount(TreeType treeType, GameObject leafGameObject, int count, string description)
        {
            var nodeGameObjectName = CartographerCommon.PrefabSearch.FixNameForPrefabSearch(leafGameObject.name);
            AddToTreeByStringAndCount(treeType, leafGameObject, nodeGameObjectName, count, description);
        }

        private void AddToTreeByGameObject(TreeType treeType, GameObject nodeGameObject, GameObject leafGameObject, string description)
        {
            var tree = GetTree(treeType);
            Node node;
            if (!Register.TryGetNode(treeType, nodeGameObject, out node))
            {
                node = Register.RegisterElement(new Node(), nodeGameObject.name, string.Empty) as Node;
                Register.RegisterNode(treeType, nodeGameObject, node);
                tree.Add(node.Key, node);
            }
            var leaf = Register.RegisterElement(new Leaf(), leafGameObject.name, description) as Leaf;
            leaf.GameObject = leafGameObject;
            node.Leafs.Add(leaf);
        }

        private void CreateDuplicates(TreeType treeTypeMask)
        {
            var treeType = TreeType.Duplicates;
            if ((treeTypeMask & treeType) > 0)
            {
                var tree = GetTree(treeType);
                var duplicateGameObjcts = duplicateChecker.GetDuplicateGameObjcts();
                if (duplicateGameObjcts.Count > 0)
                {
                    foreach (var duplicateGameObjctsPair in duplicateGameObjcts)
                    {
                        var fixedName = CartographerCommon.PrefabSearch.FixNameForPrefabSearch(duplicateGameObjctsPair.Key.name);
                        Node node;
                        node = Register.RegisterElement(new Node(), fixedName, string.Empty) as Node;
                        Register.RegisterNode(treeType, duplicateGameObjctsPair.Key, node);
                        tree.Add(node.Key, node);
                        foreach (var duplicateGameObjct in duplicateGameObjctsPair.Value)
                        {
                            var leaf = Register.RegisterElement(new Leaf(), duplicateGameObjct.name, string.Empty) as Leaf;
                            leaf.GameObject = duplicateGameObjct;
                            node.Leafs.Add(leaf);
                        }
                    }
                }
                duplicateChecker.Clear();
            }
        }

        private void CollectEmptyGameObject(Scene scene, GameObject gameObject, CartographerSceneType cartographerSceneTypeMask, TreeType treeTypeMask)
        {
            var treeType = TreeType.Empty;
            if ((treeTypeMask & treeType) > 0)
            {
                AddToTreeByName(treeType, gameObject, string.Empty);
            }
        }

        private void CollectPrefabsUsage(Scene scene, GameObject gameObject, CartographerSceneType cartographerSceneTypeMask, TreeType treeTypeMask)
        {
            var status = PrefabUtility.GetPrefabInstanceStatus(gameObject);
            if (status == PrefabInstanceStatus.Connected)
            {
                var treeType = TreeType.Prefabs;
                if ((treeTypeMask & treeType) > 0)
                {
                    var prefabGameObject = PrefabUtility.GetCorrespondingObjectFromSource(gameObject);
                    if (prefabGameObject != null)
                    {
                        AddToTreeByGameObject(treeType, prefabGameObject, gameObject, string.Empty);
                    }
                    else
                    {
                        var errorList = GetErrors(treeType);
                        errorList.Add(new ErrorElement() { TreeType = treeType, GameObject = gameObject, Description = $"GetCorrespondingObjectFromSource() returned null" });
                    }
                }
            }
            else
            {
                var disconnected = (status == PrefabInstanceStatus.Disconnected);
                if ((((treeTypeMask & TreeType.DisconnectedPrefabs) > 0) && disconnected) || (((treeTypeMask & TreeType.NonPrefabs) > 0) && !disconnected))
                {
                    var treeType = disconnected ? TreeType.DisconnectedPrefabs : TreeType.NonPrefabs;
                    var tree = GetTree(treeType);
                    var fixedName = CartographerCommon.PrefabSearch.FixNameForPrefabSearch(gameObject.name);
                    Node node;
                    if (!Register.TryGetNode(treeType, fixedName, out node))
                    {
                        node = Register.RegisterElement(new Node(), fixedName, string.Empty) as Node;
                        Register.RegisterNode(treeType, fixedName, node);
                        tree.Add(node.Key, node);
                    }
                    var leaf = Register.RegisterElement(new Leaf(), gameObject.name, string.Empty) as Leaf;
                    leaf.GameObject = gameObject;
                    node.Leafs.Add(leaf);
                }
            }
        }

        private void CollectNonUniformScales(Scene scene, GameObject gameObject, CartographerSceneType cartographerSceneTypeMask, TreeType treeTypeMask)
        {
            var treeType = TreeType.NonUniformScales;
            if ((treeTypeMask & treeType) > 0)
            {
                var transforms = gameObject.GetComponentsInChildren<Transform>();
                foreach (var transform in transforms)
                {
                    var difference = CartographerCommon.GetVectorDifference(transform.localScale);
                    if (difference >= nonUniformScalesThreshold)
                    {
                        AddToTreeByName(treeType, transform.gameObject, string.Empty);
                    }
                }
            }
        }

        private void CollectNegativeScales(Scene scene, GameObject gameObject, CartographerSceneType cartographerSceneTypeMask, TreeType treeTypeMask)
        {
            var treeType = TreeType.NegativeScales;
            if ((treeTypeMask & treeType) > 0)
            {
                var transforms = gameObject.GetComponentsInChildren<Transform>();
                foreach (var transform in transforms)
                {
                    if ((transform.localScale.x <= 0.0f) || (transform.localScale.y <= 0.0f) || (transform.localScale.z <= 0.0f))
                    {
                        AddToTreeByName(treeType, transform.gameObject, string.Empty);
                    }
                }
            }
        }

        private void CollectLODsWithNonMatchingTransforms(Scene scene, GameObject gameObject, CartographerSceneType cartographerSceneTypeMask, TreeType treeTypeMask)
        {
            var treeType = TreeType.LODsWithNonMatchingTransforms;
            if ((treeTypeMask & treeType) > 0)
            {
                var lodGroups = gameObject.GetComponentsInChildren<LODGroup>(true);
                if ((lodGroups != null) && (lodGroups.Length > 0))
                {
                    foreach (var lodGroup in lodGroups)
                    {
                        if (lodGroup != null)
                        {
                            var lods = lodGroup.GetLODs();
                            if ((lods != null) && (lods.Length > 0))
                            {
                                var firstRenderers = new List<Renderer>();
                                var different = false;
                                for (int index = 0; index < lods.Length; ++index)
                                {
                                    var lod = lods[index];
                                    for (var rendererIndex = 0; rendererIndex < lods[index].renderers.Length; ++rendererIndex)
                                    {
                                        var renderer = lods[index].renderers[rendererIndex];
                                        if (renderer == null)
                                        {
                                            different = true;
                                            break;
                                        }
                                        else
                                        {
                                            if (index == 0)
                                            {
                                                firstRenderers.Add(renderer);
                                            }
                                            else
                                            {
                                                if (firstRenderers.Count <= rendererIndex)
                                                {
                                                    different = true;
                                                    break;
                                                }
                                                else
                                                {
                                                    var firstRenderer = firstRenderers[rendererIndex];
                                                    if (firstRenderer.GetType() != renderer.GetType())
                                                    {
                                                        different = true;
                                                        break;
                                                    }
                                                    else if (firstRenderer.transform.position != renderer.transform.position)
                                                    {
                                                        different = true;
                                                        break;
                                                    }
                                                    else if (firstRenderer.transform.rotation != renderer.transform.rotation)
                                                    {
                                                        different = true;
                                                        break;
                                                    }
                                                    else if (firstRenderer.transform.localScale != renderer.transform.localScale)
                                                    {
                                                        different = true;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    if (different)
                                    {
                                        break;
                                    }
                                }
                                if (different)
                                {
                                    AddToTreeByName(treeType, lodGroup.gameObject, string.Empty);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void CollectNullComponents(Scene scene, GameObject gameObject, CartographerSceneType cartographerSceneTypeMask, TreeType treeTypeMask)
        {
            var treeType = TreeType.NullComponents;
            if ((treeTypeMask & treeType) > 0)
            {
                var gameObjectComponents = gameObject.GetComponentsInChildren<Component>(true);
                var nullComponents = 0;
                foreach (var component in gameObjectComponents)
                {
                    if (component == null)
                    {
                        ++nullComponents;
                    }
                }
                if (nullComponents > 0)
                {
                    AddToTreeByNameAndCount(treeType, gameObject, nullComponents, $"{nullComponents}");
                }
            }
        }

        private void CollectRenderers(Scene scene, GameObject gameObject, CartographerSceneType cartographerSceneTypeMask, TreeType treeTypeMask)
        {
            if ((treeTypeMask & (TreeType.LODsWithBadRenderers | TreeType.RenderersOutsideLODGroups | TreeType.Renderers)) > 0)
            {
                var renderers = gameObject.GetComponentsInChildren<Renderer>(true);

                var badRenderers = new HashSet<Renderer>();
                if ((renderers != null) && (renderers.Length > 0))
                {
                    var rendererTypes = new Dictionary<Type, CartographerCommon.KeyCount<Type>>();
                    foreach (var renderer in renderers)
                    {
                        if (renderer != null)
                        {
                            badRenderers.Add(renderer);
                            var key = renderer.GetType();
                            CartographerCommon.AddKey(key, rendererTypes);
                        }
                    }
                    foreach (var rendererType in rendererTypes)
                    {
                        AddToTreeByStringAndCount(TreeType.Renderers, gameObject, rendererType.Key.Name, rendererType.Value.Count, $"{rendererType.Value.Count}");
                    }
                }

                var nullLODGroupsCount = 0;
                var zeroLODGroupsCount = 0;
                var nullRendererReferencesCount = 0;
                var duplicateRendererReferencesCount = 0;
                var lodGroups = gameObject.GetComponentsInChildren<LODGroup>(true);
                if ((lodGroups != null) && (lodGroups.Length > 0))
                {
                    foreach (var lodGroup in lodGroups)
                    {
                        if (lodGroup != null)
                        {
                            var lods = lodGroup.GetLODs();
                            if ((lods != null) && (lods.Length > 0))
                            {
                                for (int index = 0; index < lods.Length; ++index)
                                {
                                    var lod = lods[index];
                                    foreach (var renderer in lods[index].renderers)
                                    {
                                        if (renderer != null)
                                        {
                                            if (!badRenderers.Remove(renderer))
                                            {
                                                ++duplicateRendererReferencesCount;
                                            }
                                        }
                                        else
                                        {
                                            ++nullRendererReferencesCount;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                ++zeroLODGroupsCount;
                            }
                        }
                        else
                        {
                            ++nullLODGroupsCount;
                        }
                    }
                }
                if ((nullLODGroupsCount > 0) || (zeroLODGroupsCount > 0) || (nullRendererReferencesCount > 0) || (duplicateRendererReferencesCount > 0))
                {
                    AddToTreeByName(TreeType.LODsWithBadRenderers, gameObject, $"{nullLODGroupsCount}\t{zeroLODGroupsCount}\t{nullRendererReferencesCount}\t{duplicateRendererReferencesCount}");
                }
                if (badRenderers.Count > 0)
                {
                    AddToTreeByNameAndCount(TreeType.RenderersOutsideLODGroups, gameObject, badRenderers.Count, $"{badRenderers.Count}");
                }
            }
        }

        private void CollectBigGameObjects(Scene scene, GameObject gameObject, CartographerSceneType cartographerSceneTypeMask, TreeType treeTypeMask)
        {
            var treeType = TreeType.BigGameObjects;
            if ((treeTypeMask & treeType) > 0)
            {
                var gameObjectComponents = gameObject.GetComponentsInChildren<BigGameObjectMarkerBehaviour>(true);
                if ((gameObjectComponents != null) && (gameObjectComponents.Length > 0))
                {
                    AddToTreeByName(treeType, gameObject, string.Empty);
                }
            }
        }

        private bool VisitSceneGameObject(Scene scene, GameObject gameObject, CartographerSceneType cartographerSceneTypeMask, TreeType treeTypeMask)
        {
            if (CartographerCommon.IsGameObjectFolder(gameObject))
            {
                if (gameObject.transform.childCount == 0)
                {
                    CollectEmptyGameObject(scene, gameObject, cartographerSceneTypeMask, treeTypeMask);
                    return false;
                }
                return true;
            }
            if ((cartographerSceneTypeMask & CartographerSceneType.StreamCollection) == CartographerSceneType.StreamCollection)
            {
                var sceneLoaderBehaviour = gameObject.GetComponent<SceneLoaderBehaviour>();
                var vegetationStudioManager = gameObject.GetComponent<VegetationStudioManager>();
                if ((sceneLoaderBehaviour != null) || (vegetationStudioManager != null))
                {
                    return true;
                }
            }
            CollectPrefabsUsage(scene, gameObject, cartographerSceneTypeMask, treeTypeMask);
            CollectNonUniformScales(scene, gameObject, cartographerSceneTypeMask, treeTypeMask);
            CollectNegativeScales(scene, gameObject, cartographerSceneTypeMask, treeTypeMask);
            CollectLODsWithNonMatchingTransforms(scene, gameObject, cartographerSceneTypeMask, treeTypeMask);
            CollectNullComponents(scene, gameObject, cartographerSceneTypeMask, treeTypeMask);
            CollectRenderers(scene, gameObject, cartographerSceneTypeMask, treeTypeMask);
            CollectBigGameObjects(scene, gameObject, cartographerSceneTypeMask, treeTypeMask);
            if ((treeTypeMask & TreeType.Duplicates) > 0)
            {
                duplicateChecker.Collect(scene, gameObject, cartographerSceneTypeMask);
            }
            return false;
        }

        private bool CanCollectScene(CartographerScene cartographerScene, CartographerParamsDef cartographerParams, SceneCollectionDef sceneCollection)
        {
            return (!ignoreUnknownCartographerSceneTypes) ||
                   ((cartographerScene.TypeMask & (CartographerSceneType.StreamCollection |
                                                   CartographerSceneType.BackgroundClient |
                                                   CartographerSceneType.BackgroundServer |
                                                   CartographerSceneType.MapDefClient |
                                                   CartographerSceneType.MapDefServer)) > 0);
        }

        // separate calls -------------------------------------------------------------------------
        public void Start(TreeType treeTypeMask, float _duplicatesThreshold, float _nonUniformScalesThreshold, bool _ignoreUnknownCartographerSceneTypes)
        {
            Register.Clear();
            trees.Clear();
            duplicateChecker.Start((treeTypeMask & TreeType.Duplicates) > 0, _duplicatesThreshold);
            nonUniformScalesThreshold = _nonUniformScalesThreshold;
            ignoreUnknownCartographerSceneTypes = _ignoreUnknownCartographerSceneTypes;
        }

        public void Finish(TreeType treeTypeMask)
        {
            CreateDuplicates(treeTypeMask);
        }

        public bool CollectScene(Scene scene, CartographerSceneType cartographerSceneTypeMask, CartographerParamsDef cartographerParams, SceneCollectionDef sceneCollection, TreeType treeTypeMask)
        {
            CartographerSceneObjectVisitor.Visit(scene, gameObject => { return VisitSceneGameObject(scene, gameObject, cartographerSceneTypeMask, treeTypeMask); });
            return true;
        }

        // one call -------------------------------------------------------------------------------
        public void Collect(CartographerParamsDef cartographerParams, SceneCollectionDef sceneCollection, TreeType treeTypeMask, float _duplicatesThreshold, float _nonUniformScalesThreshold, bool _ignoreUnknownCartographerSceneTypes)
        {
            Start(treeTypeMask, _duplicatesThreshold, _nonUniformScalesThreshold, _ignoreUnknownCartographerSceneTypes);
            if ((cartographerParams == null) || (sceneCollection == null) || (treeTypeMask == 0))
            {
                return;
            }
            var sceneCount = SceneManager.sceneCount;
            for (var sceneIndex = 0; sceneIndex < sceneCount; ++sceneIndex)
            {
                var scene = SceneManager.GetSceneAt(sceneIndex);
                var newCartographerScene = new CartographerScene();
                newCartographerScene.Name = scene.name;
                newCartographerScene.Path = CartographerCommon.CombineAssetPath(scene.path, string.Empty, CartographerCommon.UnityExtension);
                newCartographerScene.TypeMask = CartographerSceneOperation.GetCartographerSceneTypeMask(newCartographerScene.Path, cartographerParams, sceneCollection);
                if (scene.isLoaded && CanCollectScene(newCartographerScene, cartographerParams, sceneCollection))
                {
                    if (!CollectScene(scene, newCartographerScene.TypeMask, cartographerParams, sceneCollection, treeTypeMask))
                    {
                        break;
                    }
                }
            }
            Finish(treeTypeMask);
        }
    }
};