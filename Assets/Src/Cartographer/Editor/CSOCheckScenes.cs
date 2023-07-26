using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using SharedCode.Aspects.Cartographer;
using AwesomeTechnologies.VegetationStudio;
using System.Text;

namespace Assets.Src.Cartographer.Editor
{
    public class CSOCheckScenesArguments
    {
        public CartographerCommon.FormatArguments FormatArguments { get; } = new CartographerCommon.FormatArguments();

        public bool IgnoreUnknownCartographerSceneTypes { get; set; } = true;

        public bool DeleteObsoleteGameObjects { get; set; } = true;
        public bool FixNegativeScales { get; set; } = true;
        public bool SaveScenes { get; set; } = false;

        public bool CollectNegativeScaleStats { get; set; } = true;

        public string ResultsFilePath { get; set; } = "CheckResults.txt";
    }

    public class CSOCheckScenes : ICartographerSceneOperation
    {
        private enum NegativeScaleType
        {
            None = 0,
            XY = 1,
            YZ = 2,
            XZ = 3,
            X = 4,
            Y = 5,
            Z = 6,
            XYZ = 7,
        }

        private class NegativeScaleStats
        {
            public NegativeScaleType NegativeScaleType { get; set; } = NegativeScaleType.None;
            public string PrefabName { get; set; } = string.Empty;
            public string PrefabPath { get; set; } = string.Empty;
            public int Count { get; set; } = 0;
        }

        private class MergedNegativeScaleStats
        {
            public List<NegativeScaleType> NegativeScaleTypes { get; } = new List<NegativeScaleType>();
            public string PrefabName { get; set; } = string.Empty;
            public string PrefabPath { get; set; } = string.Empty;
            public int Count { get; set; } = 0;

            public string GetNegativeScalePrint()
            {
                var result = new StringBuilder();
                if (NegativeScaleTypes.Count > 0)
                {
                    NegativeScaleTypes.Sort();
                    foreach (var negativeScaleType in NegativeScaleTypes)
                    {
                        CartographerCommon.AppendIfNotEmpty(result, ", ");
                        result.Append(negativeScaleType);
                    }
                }
                return result.ToString();
            }
        }

        private Dictionary<NegativeScaleType, Dictionary<string, NegativeScaleStats>> negativeScaleStatsCollections = new Dictionary<NegativeScaleType, Dictionary<string, NegativeScaleStats>>();

        // messages -------------------------------------------------------------------------------
        public class MessagesClass : IProgressMessages
        {
            public string Title { get { return "Check Scenes"; } }
            public string RunQuestion { get { return "Are you sure you want to check scenes?"; } }
            public string WelcomeMessage { get { return "Check Scenes"; } }
            public string OnScenePrefix { get { return "Check Scenes"; } }
        }

        public static MessagesClass Messages = new MessagesClass();

        // data -----------------------------------------------------------------------------------
        private CSOCheckScenesArguments arguments = new CSOCheckScenesArguments();
        private CartographerGameObjectTree gameObjectTree = new CartographerGameObjectTree();
        private CartographerGameObjectTree.TreeType treeTypeMask = CartographerGameObjectTree.TreeType.DisconnectedPrefabs |
                                                                   CartographerGameObjectTree.TreeType.NonPrefabs |
                                                                   CartographerGameObjectTree.TreeType.Empty |
                                                                   CartographerGameObjectTree.TreeType.Duplicates |
                                                                   CartographerGameObjectTree.TreeType.NonUniformScales |
                                                                   CartographerGameObjectTree.TreeType.NegativeScales |
                                                                   CartographerGameObjectTree.TreeType.LODsWithNonMatchingTransforms |
                                                                   CartographerGameObjectTree.TreeType.LODsWithBadRenderers |
                                                                   CartographerGameObjectTree.TreeType.RenderersOutsideLODGroups |
                                                                   CartographerGameObjectTree.TreeType.NullComponents;
        private List<string> deletedGameObjectNames = new List<string>();
        private List<string> fixedNegativeScaleGameObjectNames = new List<string>();
        private List<string> folderWithNegativeTransformsList = new List<string>();
        private List<string> specialWithNegativeTransformsList = new List<string>();
        private List<string> multipleNegativeTransformsList = new List<string>();
        private List<string> cantGetAnyPrefabList = new List<string>();
        private List<string> cantGetCorrespondingPrefabList = new List<string>();

        // helpers --------------------------------------------------------------------------------
        private Dictionary<string, GameObject> invertedPrefabs = new Dictionary<string, GameObject>();
        private GameObject TryGetInvertedPrefab(GameObject gameObject)
        {
            GameObject invertedPrefabGameObject = null;
            var prefabGameObject = PrefabUtility.GetCorrespondingObjectFromSource(gameObject);
            if (prefabGameObject != null)
            {
                var prefabPath = CartographerCommon.GetAssetPath(prefabGameObject);
                if (!invertedPrefabs.TryGetValue(prefabPath, out invertedPrefabGameObject))
                {
                    var invertedPostfix = "_invert.prefab";
                    var normalPostfix = ".prefab";
                    var invertedPrefabPath = string.Empty;
                    if (prefabPath.EndsWith(invertedPostfix, System.StringComparison.OrdinalIgnoreCase))
                    {
                        invertedPrefabPath = prefabPath.Substring(0, prefabPath.Length - invertedPostfix.Length) + normalPostfix;
                    }
                    else if (prefabPath.EndsWith(normalPostfix, System.StringComparison.OrdinalIgnoreCase))
                    {
                        invertedPrefabPath = prefabPath.Substring(0, prefabPath.Length - normalPostfix.Length) + invertedPostfix;
                    }
                    if (!string.IsNullOrEmpty(invertedPrefabPath))
                    {
                        invertedPrefabGameObject = AssetDatabase.LoadAssetAtPath<GameObject>(invertedPrefabPath);
                    }
                    invertedPrefabs.Add(prefabPath, invertedPrefabGameObject);
                }
            }
            return invertedPrefabGameObject;
        }

        //main procedures -------------------------------------------------------------------------
        private NegativeScaleType GetNegativeScaleType(Transform transform)
        {
            var negativeScaleType = NegativeScaleType.None;
            if ((transform.localScale.x <= 0.0f) && (transform.localScale.y > 0.0f) && (transform.localScale.z > 0.0f))
            {
                negativeScaleType = NegativeScaleType.X;
            }
            else if ((transform.localScale.x > 0.0f) && (transform.localScale.y <= 0.0f) && (transform.localScale.z > 0.0f))
            {
                negativeScaleType = NegativeScaleType.Y;
            }
            else if ((transform.localScale.x > 0.0f) && (transform.localScale.y > 0.0f) && (transform.localScale.z <= 0.0f))
            {
                negativeScaleType = NegativeScaleType.Z;
            }
            else if ((transform.localScale.x <= 0.0f) && (transform.localScale.y <= 0.0f) && (transform.localScale.z > 0.0f))
            {
                negativeScaleType = NegativeScaleType.XY;
            }
            else if ((transform.localScale.x > 0.0f) && (transform.localScale.y <= 0.0f) && (transform.localScale.z <= 0.0f))
            {
                negativeScaleType = NegativeScaleType.YZ;
            }
            else if ((transform.localScale.x <= 0.0f) && (transform.localScale.y > 0.0f) && (transform.localScale.z <= 0.0f))
            {
                negativeScaleType = NegativeScaleType.XZ;
            }
            else if ((transform.localScale.x <= 0.0f) && (transform.localScale.y <= 0.0f) && (transform.localScale.z <= 0.0f))
            {
                negativeScaleType = NegativeScaleType.XYZ;
            }
            return negativeScaleType;
        }

        private bool DeleteObsoleteGameObject(Scene scene, GameObject gameObject, CartographerSceneType cartographerSceneTypeMask, ref bool currentSceneChanged)
        {
            if (gameObject.name.Contains("CREATORS") ||
                gameObject.name.Contains("Missing Prefab") ||
                gameObject.name.Contains("SmallRocks_02") ||
                gameObject.name.Contains("SmallRocks_03") ||
                gameObject.name.Contains("sav_tubule_long"))
            {
                currentSceneChanged = true;
                deletedGameObjectNames.Add($"{CartographerCommon.GetGameObjectPrint(gameObject)}");
                UnityEngine.Object.DestroyImmediate(gameObject);
                return false;
            }
            var prefabStatus = PrefabUtility.GetPrefabInstanceStatus(gameObject);
            if (prefabStatus == PrefabInstanceStatus.MissingAsset)
            {
                currentSceneChanged = true;
                deletedGameObjectNames.Add($"{CartographerCommon.GetGameObjectPrint(gameObject)}");
                UnityEngine.Object.DestroyImmediate(gameObject);
                return false;
            }
            return true;
        }

        private bool FixNegativeScale(Scene scene, GameObject gameObject, CartographerSceneType cartographerSceneTypeMask, ref bool currentSceneChanged)
        {
            if (CartographerCommon.IsGameObjectFolder(gameObject))
            {
                if (gameObject.transform.childCount > 0)
                {
                    return true;
                }
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

            var negativeScaleType = GetNegativeScaleType(gameObject.transform);
            var status = PrefabUtility.GetPrefabInstanceStatus(gameObject);
            if (status == PrefabInstanceStatus.Connected)
            {

                if ((negativeScaleType == NegativeScaleType.XY) || (negativeScaleType == NegativeScaleType.YZ) || (negativeScaleType == NegativeScaleType.XZ))
                {
                    gameObject.transform.localScale = new Vector3(Mathf.Abs(gameObject.transform.localScale.x), Mathf.Abs(gameObject.transform.localScale.y), Mathf.Abs(gameObject.transform.localScale.z));
                    if (negativeScaleType == NegativeScaleType.XY)
                    {
                        var halfRotation = Quaternion.FromToRotation(Vector3.right, Vector3.up);
                        gameObject.transform.rotation = gameObject.transform.rotation * halfRotation * halfRotation;
                    }
                    else if (negativeScaleType == NegativeScaleType.YZ)
                    {
                        var halfRotation = Quaternion.FromToRotation(Vector3.up, Vector3.forward);
                        gameObject.transform.rotation = gameObject.transform.rotation * halfRotation * halfRotation;
                    }
                    else  //XZ 
                    {
                        var halfRotation = Quaternion.FromToRotation(Vector3.forward, Vector3.right);
                        gameObject.transform.rotation = gameObject.transform.rotation * halfRotation * halfRotation;
                    }
                    currentSceneChanged = true;
                    fixedNegativeScaleGameObjectNames.Add($"{CartographerCommon.GetGameObjectPrint(gameObject)}");
                }
                else if ((negativeScaleType == NegativeScaleType.X) || (negativeScaleType == NegativeScaleType.Y) || (negativeScaleType == NegativeScaleType.Z) || (negativeScaleType == NegativeScaleType.XYZ))
                {
                    var localScale = new Vector3(Mathf.Abs(gameObject.transform.localScale.x), Mathf.Abs(gameObject.transform.localScale.y), Mathf.Abs(gameObject.transform.localScale.z));
                    var prefabForReplace = TryGetInvertedPrefab(gameObject);
                    if (prefabForReplace != null)
                    {
                        var gameObjectForReplace = PrefabUtility.InstantiatePrefab(prefabForReplace) as GameObject;
                        if (gameObjectForReplace != null)
                        {
                            gameObjectForReplace.name = gameObject.name;
                            gameObjectForReplace.transform.parent = gameObject.transform.parent;
                            gameObjectForReplace.transform.localPosition = gameObject.transform.localPosition;
                            gameObjectForReplace.transform.localRotation = gameObject.transform.localRotation;
                            gameObjectForReplace.transform.localScale = localScale;
                            gameObjectForReplace.transform.SetSiblingIndex(gameObject.transform.GetSiblingIndex());
                            CartographerCommon.MakeAdditionalReplacement(gameObjectForReplace, gameObject, prefabForReplace);
                            UnityEngine.Object.DestroyImmediate(gameObject);
                            if (negativeScaleType == NegativeScaleType.X)
                            {
                                //nothing to do here
                            }
                            else if (negativeScaleType == NegativeScaleType.Y)
                            {
                                var halfRotation = Quaternion.FromToRotation(Vector3.right, Vector3.up);
                                gameObjectForReplace.transform.rotation = gameObjectForReplace.transform.rotation * halfRotation * halfRotation;
                            }
                            else if (negativeScaleType == NegativeScaleType.Z)
                            {
                                var halfRotation = Quaternion.FromToRotation(Vector3.forward, Vector3.right);
                                gameObjectForReplace.transform.rotation = gameObjectForReplace.transform.rotation * halfRotation * halfRotation;
                            }
                            else // XYZ
                            {
                                var halfRotation = Quaternion.FromToRotation(Vector3.up, Vector3.forward);
                                gameObjectForReplace.transform.rotation = gameObjectForReplace.transform.rotation * halfRotation * halfRotation;
                            }
                            currentSceneChanged = true;
                            fixedNegativeScaleGameObjectNames.Add($"{CartographerCommon.GetGameObjectPrint(gameObjectForReplace)}");
                        }
                    }
                }
            }
            return false;
        }

        private void CollectNegativeScale(Scene scene, GameObject gameObject, CartographerSceneType cartographerSceneTypeMask)
        {
            if (arguments.CollectNegativeScaleStats)
            {
                var transforms = gameObject.GetComponentsInChildren<Transform>();
                if (transforms != null)
                {
                    var negativeTransforms = 0;
                    var multipleGameObjectsLine = new StringBuilder();
                    foreach (var transform in transforms)
                    {
                        var negativeScaleType = GetNegativeScaleType(transform);
                        if (negativeScaleType != NegativeScaleType.None)
                        {
                            ++negativeTransforms;
                            CartographerCommon.AppendIfNotEmpty(multipleGameObjectsLine, ", ");
                            multipleGameObjectsLine.Append(CartographerCommon.GetFullName(transform.gameObject));
                            var status = PrefabUtility.GetPrefabInstanceStatus(transform.gameObject);
                            if (status == PrefabInstanceStatus.Connected)
                            {
                                var prefabGameObject = PrefabUtility.GetCorrespondingObjectFromSource(transform.gameObject);
                                if (prefabGameObject != null)
                                {
                                    var prefabPath = CartographerCommon.GetAssetPath(prefabGameObject);
                                    Dictionary<string, NegativeScaleStats> negativeScaleStatsCollection;
                                    if (!negativeScaleStatsCollections.TryGetValue(negativeScaleType, out negativeScaleStatsCollection))
                                    {
                                        negativeScaleStatsCollection = new Dictionary<string, NegativeScaleStats>();
                                        negativeScaleStatsCollections.Add(negativeScaleType, negativeScaleStatsCollection);
                                    }
                                    NegativeScaleStats negativeScaleStatsElement;
                                    if (!negativeScaleStatsCollection.TryGetValue(prefabPath, out negativeScaleStatsElement))
                                    {
                                        negativeScaleStatsElement = new NegativeScaleStats();
                                        negativeScaleStatsElement.NegativeScaleType = negativeScaleType;
                                        negativeScaleStatsElement.PrefabName = prefabGameObject.name;
                                        negativeScaleStatsElement.PrefabPath = prefabPath;
                                        negativeScaleStatsElement.Count = 0;
                                        negativeScaleStatsCollection.Add(prefabPath, negativeScaleStatsElement);
                                    }
                                    negativeScaleStatsElement.Count += 1;
                                }
                                else
                                {
                                    cantGetCorrespondingPrefabList.Add(CartographerCommon.GetGameObjectPrint(transform.gameObject));
                                }
                            }
                            else
                            {
                                cantGetAnyPrefabList.Add(CartographerCommon.GetGameObjectPrint(transform.gameObject));
                            }
                        }
                        if (negativeTransforms > 1)
                        {
                            multipleNegativeTransformsList.Add($"{CartographerCommon.GetGameObjectPrint(gameObject)}\t{multipleGameObjectsLine}");
                        }
                    }
                }
            }
        }

        private bool CheckGameObject(Scene scene, GameObject gameObject, CartographerSceneType cartographerSceneTypeMask)
        {
            if (CartographerCommon.IsGameObjectFolder(gameObject))
            {
                var negativeScaleType = GetNegativeScaleType(gameObject.transform);
                if (negativeScaleType != NegativeScaleType.None)
                {
                    folderWithNegativeTransformsList.Add($"{CartographerCommon.GetGameObjectPrint(gameObject)}");
                }
                if (gameObject.transform.childCount > 0)
                {
                    return true;
                }
            }
            if ((cartographerSceneTypeMask & CartographerSceneType.StreamCollection) == CartographerSceneType.StreamCollection)
            {
                var sceneLoaderBehaviour = gameObject.GetComponent<SceneLoaderBehaviour>();
                var vegetationStudioManager = gameObject.GetComponent<VegetationStudioManager>();
                if ((sceneLoaderBehaviour != null) || (vegetationStudioManager != null))
                {
                    var negativeScaleType = GetNegativeScaleType(gameObject.transform);
                    if (negativeScaleType != NegativeScaleType.None)
                    {
                        specialWithNegativeTransformsList.Add($"{CartographerCommon.GetGameObjectPrint(gameObject)}");
                    }
                    return true;
                }
            }
            CollectNegativeScale(scene, gameObject, cartographerSceneTypeMask);
            //
            // Custom code here
            //
            return false;
        }

        private int GetObjectTreeLeafsCount(Dictionary<int, CartographerGameObjectTree.Node> tree)
        {
            var result = 0;
            if (tree.Count > 0)
            {
                foreach (var node in tree)
                {
                    result += node.Value.Leafs.Count;
                }
            }
            return result;
        }

        private void PrintGameObjectTree(List<string> lines, CartographerGameObjectTree.TreeType treeType, string label, string descriptionLabel, CartographerCommon.FormatArguments formatArguments, bool addEmptyLine, bool addEmptyLineBetweenNodes)
        {
            var tree = gameObjectTree.GetTree(treeType);
            if (tree.Count > 0)
            {
                if (addEmptyLine)
                {
                    lines.Add($"");
                }
                if (!string.IsNullOrEmpty(label))
                {
                    lines.Add($"{label}: {tree.Count}/{GetObjectTreeLeafsCount(tree)}");
                }
                var newLines = new List<string>();
                newLines.Add($"Scene\tLookup name\tName\tPath\tPosition\tRotation\tScale{(string.IsNullOrEmpty(descriptionLabel) ? string.Empty : $"\t{descriptionLabel}")}");
                foreach (var node in tree)
                {
                    if (addEmptyLineBetweenNodes && (newLines.Count > 1))
                    {
                        newLines.Add(string.Empty);
                    }
                    var lookupName = node.Value.Name;
                    foreach (var leaf in node.Value.Leafs)
                    {
                        var sceneName = leaf.GameObject.scene.name;
                        var name = leaf.GameObject.name;
                        var fullName = CartographerCommon.GetFullName(leaf.GameObject);
                        newLines.Add($"{sceneName}\t{lookupName}\t{name}\t{fullName}\t" +
                                     $"({leaf.GameObject.transform.position.x}, {leaf.GameObject.transform.position.y}, {leaf.GameObject.transform.position.z})" +
                                     $"({leaf.GameObject.transform.rotation.x}, {leaf.GameObject.transform.rotation.y}, {leaf.GameObject.transform.rotation.z})" +
                                     $"({leaf.GameObject.transform.localScale.x}, {leaf.GameObject.transform.localScale.y}, {leaf.GameObject.transform.localScale.z})" +
                                     $"{(string.IsNullOrEmpty(leaf.Description) ? string.Empty : $"\t{leaf.Description}")}");
                    }
                }
                lines.AddRange(CartographerCommon.FixLines(newLines, formatArguments));
            }
        }

        private int CompareMergedNegativeScaleStats(MergedNegativeScaleStats left, MergedNegativeScaleStats right)
        {
            var resultCount = right.Count.CompareTo(left.Count);
            if (resultCount == 0)
            {
                return left.PrefabName.CompareTo(right.PrefabName);
            }
            return resultCount;
        }

        private void PrintNegativeTransforms(List<string> lines, string label, bool sort, CartographerCommon.FormatArguments formatArguments, bool addEmptyLine)
        {
            if (negativeScaleStatsCollections.Count > 0)
            {
                if (addEmptyLine)
                {
                    lines.Add($"");
                }

                var newLines = new List<string>();
                newLines.Add($"Count\tNegative Scale\tName\tPath");

                var oddMergedNegativeScaleStatsCollection = new Dictionary<string, MergedNegativeScaleStats>();
                var evenMergedNegativeScaleStatsCollection = new Dictionary<string, MergedNegativeScaleStats>();
                foreach (var negativeScaleStatsCollection in negativeScaleStatsCollections)
                {
                    bool odd = (negativeScaleStatsCollection.Key == NegativeScaleType.X) ||
                                (negativeScaleStatsCollection.Key == NegativeScaleType.Y) ||
                                (negativeScaleStatsCollection.Key == NegativeScaleType.Z) ||
                                (negativeScaleStatsCollection.Key == NegativeScaleType.XYZ);
                    foreach (var negativeScaleStatsElement in negativeScaleStatsCollection.Value)
                    {
                        MergedNegativeScaleStats mergedNegativeScaleStatsElement;
                        if( !oddMergedNegativeScaleStatsCollection.TryGetValue(negativeScaleStatsElement.Key, out mergedNegativeScaleStatsElement))
                        {
                            mergedNegativeScaleStatsElement = new MergedNegativeScaleStats();
                            mergedNegativeScaleStatsElement.PrefabName = negativeScaleStatsElement.Value.PrefabName;
                            mergedNegativeScaleStatsElement.PrefabPath = negativeScaleStatsElement.Value.PrefabPath;
                            if (odd)
                            {
                                oddMergedNegativeScaleStatsCollection.Add(negativeScaleStatsElement.Key, mergedNegativeScaleStatsElement);
                            }
                            else
                            {
                                evenMergedNegativeScaleStatsCollection.Add(negativeScaleStatsElement.Key, mergedNegativeScaleStatsElement);
                            }
                        }
                        mergedNegativeScaleStatsElement.NegativeScaleTypes.Add(negativeScaleStatsCollection.Key);
                        mergedNegativeScaleStatsElement.Count += negativeScaleStatsElement.Value.Count;
                    }
                }
                var oddNegativeScaleStatsList = new List<MergedNegativeScaleStats>();
                foreach(var oddMergedNegativeScaleStatsElement in oddMergedNegativeScaleStatsCollection)
                {
                    oddNegativeScaleStatsList.Add(oddMergedNegativeScaleStatsElement.Value);
                }
                var evenNegativeScaleStatsList = new List<MergedNegativeScaleStats>();
                foreach (var evenMergedNegativeScaleStatsElement in evenMergedNegativeScaleStatsCollection)
                {
                    evenNegativeScaleStatsList.Add(evenMergedNegativeScaleStatsElement.Value);
                }
                if (sort)
                {
                    oddNegativeScaleStatsList.Sort(CompareMergedNegativeScaleStats);
                    evenNegativeScaleStatsList.Sort(CompareMergedNegativeScaleStats);
                }
                int oddCount = 0;
                int oddUsageCount = 0;
                int evenCount = 0;
                int evenUsageCount = 0;
                foreach (var oddNegativeScaleStatsElement in oddNegativeScaleStatsList)
                {
                    ++oddCount;
                    oddUsageCount += oddNegativeScaleStatsElement.Count;
                }
                foreach (var evenNegativeScaleStatsElement in evenNegativeScaleStatsList)
                {
                    ++evenCount;
                    evenUsageCount += evenNegativeScaleStatsElement.Count;
                }
                if (!string.IsNullOrEmpty(label))
                {
                    lines.Add($"{label}: {oddCount + evenCount}/{oddUsageCount + evenUsageCount} (odd: {oddCount}/{oddUsageCount}, even: {evenCount}/{evenUsageCount})");
                }
                foreach (var oddNegativeScaleStatsElement in oddNegativeScaleStatsList)
                {
                    newLines.Add($"{oddNegativeScaleStatsElement.Count}\t{oddNegativeScaleStatsElement.GetNegativeScalePrint()}\t{oddNegativeScaleStatsElement.PrefabName}\t{oddNegativeScaleStatsElement.PrefabPath}");
                }
                foreach (var evenNegativeScaleStatsElement in evenNegativeScaleStatsList)
                {
                    newLines.Add($"{evenNegativeScaleStatsElement.Count}\t{evenNegativeScaleStatsElement.GetNegativeScalePrint()}\t{evenNegativeScaleStatsElement.PrefabName}\t{evenNegativeScaleStatsElement.PrefabPath}");
                }
                lines.AddRange(CartographerCommon.FixLines(newLines, formatArguments));
            }
        }

        private void PrintResults()
        {
            var lines = new List<string>();
            if (arguments.DeleteObsoleteGameObjects)
            {
                CartographerCommon.PrintList(lines, CartographerCommon.FixLines(deletedGameObjectNames, arguments.FormatArguments), $"--- Deleted GameObjects: {deletedGameObjectNames.Count}", false, arguments.FormatArguments, (lines.Count > 0));
            }
            if (arguments.FixNegativeScales)
            {
                CartographerCommon.PrintList(lines, CartographerCommon.FixLines(fixedNegativeScaleGameObjectNames, arguments.FormatArguments), $"--- Fixed negative scale on GameObjects: {fixedNegativeScaleGameObjectNames.Count}", false, arguments.FormatArguments, (lines.Count > 0));
            }
            CartographerCommon.PrintList(lines, CartographerCommon.FixLines(folderWithNegativeTransformsList, arguments.FormatArguments), $"--- folders with negative transforms: {folderWithNegativeTransformsList.Count}", false, arguments.FormatArguments, (lines.Count > 0));
            CartographerCommon.PrintList(lines, CartographerCommon.FixLines(specialWithNegativeTransformsList, arguments.FormatArguments), $"--- special objects with negative transforms: {specialWithNegativeTransformsList.Count}", false, arguments.FormatArguments, (lines.Count > 0));
            if (arguments.CollectNegativeScaleStats)
            {
                CartographerCommon.PrintList(lines, CartographerCommon.FixLines(multipleNegativeTransformsList, arguments.FormatArguments), $"--- Multiple negative transforms: {multipleNegativeTransformsList.Count}", false, arguments.FormatArguments, (lines.Count > 0));
                CartographerCommon.PrintList(lines, CartographerCommon.FixLines(cantGetAnyPrefabList, arguments.FormatArguments), $"--- Can't get any prefab: {cantGetAnyPrefabList.Count}", false, arguments.FormatArguments, (lines.Count > 0));
                CartographerCommon.PrintList(lines, CartographerCommon.FixLines(cantGetCorrespondingPrefabList, arguments.FormatArguments), $"--- Can't get corresponding prefab: {cantGetCorrespondingPrefabList.Count}", false, arguments.FormatArguments, (lines.Count > 0));
                PrintNegativeTransforms(lines, $"--- Negative transforms statisitic", true, arguments.FormatArguments, (lines.Count > 0));
            }
            PrintGameObjectTree(lines,
                                CartographerGameObjectTree.TreeType.DisconnectedPrefabs,
                                $"--- Disconnected prefabs",
                                string.Empty,
                                arguments.FormatArguments,
                                (lines.Count > 0),
                                false);
            PrintGameObjectTree(lines,
                                CartographerGameObjectTree.TreeType.NonPrefabs,
                                $"--- Non prefabs",
                                string.Empty,
                                arguments.FormatArguments,
                                (lines.Count > 0),
                                false);
            PrintGameObjectTree(lines,
                                CartographerGameObjectTree.TreeType.Empty,
                                $"--- Empty game objects",
                                string.Empty,
                                arguments.FormatArguments,
                                (lines.Count > 0),
                                false);
            PrintGameObjectTree(lines,
                                CartographerGameObjectTree.TreeType.Duplicates,
                                $"--- Duplicates",
                                string.Empty,
                                arguments.FormatArguments,
                                (lines.Count > 0),
                                false);
            PrintGameObjectTree(lines,
                                CartographerGameObjectTree.TreeType.NonUniformScales,
                                $"--- Non uniform scale",
                                string.Empty,
                                arguments.FormatArguments,
                                (lines.Count > 0),
                                false);
            PrintGameObjectTree(lines,
                                CartographerGameObjectTree.TreeType.NegativeScales,
                                $"--- Negative scale",
                                string.Empty,
                                arguments.FormatArguments,
                                (lines.Count > 0),
                                false);
            PrintGameObjectTree(lines,
                                CartographerGameObjectTree.TreeType.LODsWithNonMatchingTransforms,
                                $"--- LODs with non matching transforms",
                                string.Empty,
                                arguments.FormatArguments,
                                (lines.Count > 0),
                                false);
            PrintGameObjectTree(lines,
                                CartographerGameObjectTree.TreeType.LODsWithBadRenderers,
                                $"--- LODs with bad renderers",
                                $"Null LODGroups\tzero LODGroups\tnull renderers in LODs\tduplicate renderers in LODs",
                                arguments.FormatArguments,
                                (lines.Count > 0),
                                false);
            PrintGameObjectTree(lines,
                                CartographerGameObjectTree.TreeType.RenderersOutsideLODGroups,
                                $"--- Renderers outside LODGroups",
                                $"Count",
                                arguments.FormatArguments,
                                (lines.Count > 0),
                                false);
            PrintGameObjectTree(lines,
                                CartographerGameObjectTree.TreeType.NullComponents,
                                $"--- GameObjects with null components",
                                $"Count",
                                arguments.FormatArguments,
                                (lines.Count > 0),
                                false);
            PrintGameObjectTree(lines,
                                CartographerGameObjectTree.TreeType.Renderers,
                                $"--- Renderers",
                                $"Count",
                                arguments.FormatArguments,
                                (lines.Count > 0),
                                false);
            for (int index = CartographerGameObjectTree.MinTreeTypeIndex; index <= CartographerGameObjectTree.MaxTreeTypeIndex; ++index)
            {
                var treeType = CartographerGameObjectTree.GetTreeTypeByIndex(index);
                var errorsList = gameObjectTree.GetErrors(treeType);
                if (errorsList.Count > 0)
                {
                    var errors = new List<string>();
                    foreach (var error in errorsList)
                    {
                        errors.Add($"{error.TreeType}\t{error.Description}\t{CartographerCommon.GetGameObjectPrintAsPrefab(error.GameObject)}");
                    }
                    CartographerCommon.PrintList(lines, errors, $"--- Errors in: {treeType} list", false, arguments.FormatArguments, (lines.Count > 0));
                }
            }

            var filePath = arguments.ResultsFilePath;
            File.WriteAllLines(filePath, lines);
            CartographerCommon.ReportError($"File created: {filePath}");
        }

        // constructor ----------------------------------------------------------------------------
        public CSOCheckScenes(CSOCheckScenesArguments newArguments)
        {
            if (newArguments != null)
            {
                arguments = newArguments;
            }
        }

        // ICartographerSceneOperation interface --------------------------------------------------
        public IProgressMessages ProgressMessages { get { return Messages; } }

        public bool CanOperate(CartographerScene cartographerScene, CartographerParamsDef cartographerParams, SceneCollectionDef sceneCollection, bool openedScenesOnly)
        {
            return (!arguments.IgnoreUnknownCartographerSceneTypes) ||
                   ((cartographerScene.TypeMask & (CartographerSceneType.StreamCollection |
                                                   CartographerSceneType.BackgroundClient |
                                                   CartographerSceneType.BackgroundServer |
                                                   CartographerSceneType.MapDefClient |
                                                   CartographerSceneType.MapDefServer)) > 0);
        }

        public bool Start(CartographerParamsDef cartographerParams, SceneCollectionDef sceneCollection, bool openedScenesOnly, ICartographerProgressCallback progressCallback)
        {
            gameObjectTree.Start(treeTypeMask,
                                 CartographerGameObjectTree.DuplicateChecker.DefaultDuplicatesThreshold,
                                 CartographerGameObjectTree.DuplicateChecker.DefaultNonUniformScalesThreshold,
                                 arguments.IgnoreUnknownCartographerSceneTypes);
            return true;
        }

        public bool Operate(Scene scene, CartographerSceneType cartographerSceneTypeMask, CartographerParamsDef cartographerParams, SceneCollectionDef sceneCollection, bool openedScenesOnly, ICartographerProgressCallback progressCallback)
        {
            var currentSceneChanged = false;
            if (arguments.DeleteObsoleteGameObjects)
            {
                CartographerSceneObjectVisitor.Visit(scene, gameObject => { return DeleteObsoleteGameObject(scene, gameObject, cartographerSceneTypeMask, ref currentSceneChanged); });
            }
            if (arguments.FixNegativeScales)
            {
                CartographerSceneObjectVisitor.Visit(scene, gameObject => { return FixNegativeScale(scene, gameObject, cartographerSceneTypeMask, ref currentSceneChanged); });
            }
            if (currentSceneChanged)
            {
                EditorSceneManager.MarkSceneDirty(scene);
                if (arguments.SaveScenes)
                {
                    EditorSceneManager.SaveScene(scene);
                }
            }
            CartographerSceneObjectVisitor.Visit(scene, gameObject => { return CheckGameObject(scene, gameObject, cartographerSceneTypeMask); });
            gameObjectTree.CollectScene(scene, cartographerSceneTypeMask, cartographerParams, sceneCollection, treeTypeMask);
            return true;
        }

        public void Finish(CartographerParamsDef cartographerParams, SceneCollectionDef sceneCollection, bool openedScenesOnly, ICartographerProgressCallback progressCallback)
        {
            gameObjectTree.Finish(treeTypeMask);
            PrintResults();
        }
    }
};