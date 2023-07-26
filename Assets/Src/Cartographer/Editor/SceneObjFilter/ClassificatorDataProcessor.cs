using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Src.Cartographer.Editor
{
    class ClassificatorDataProcessor
    {
        public List<GameObject> DisconnectedPrefabInstances { get; private set; }
        public Dictionary<GameObject, List<PropertyModification>> ModifiedPrefabInstance { get; private set; }
        private Dictionary<Object, GameObjectWithModificationInfo> _freqTable;
        private ClassificatorPredicateManager<GameObject> _predicates;
        private static ClassificatorDataProcessor _proc;
        public static ClassificatorDataProcessor Instance { get { if (_proc == null) _proc = new ClassificatorDataProcessor(); return _proc; } }

        private ClassificatorDataProcessor()
        {
            Reload();
            EditorApplication.hierarchyChanged += Reload;
        }

        ~ClassificatorDataProcessor()
        {
            EditorApplication.hierarchyChanged -= Reload;
        }

        public event System.Action OnDataChange;

        public void Reload()
        {
            DisconnectedPrefabInstances = new List<GameObject>();
            ModifiedPrefabInstance = new Dictionary<GameObject, List<PropertyModification>>();
            _freqTable = new Dictionary<Object, GameObjectWithModificationInfo>();
            _predicates = new ClassificatorPredicateManager<GameObject>();
            VerifyPrefabs();
            OnDataChange?.Invoke();
        }

        private void AddToFreqTable(GameObject needToCount, bool modified)
        {
            var correspondingPrefab = PrefabUtility.GetCorrespondingObjectFromSource(needToCount);
            if (_freqTable.ContainsKey(correspondingPrefab))
            {
                _freqTable[correspondingPrefab].Add(needToCount, modified);
            }
            else
            {
                var go = new GameObjectWithModificationInfo();
                go.Add(needToCount, modified);
                _freqTable.Add(correspondingPrefab, go);
            }
        }

        private void AddToFreqTable(List<GameObject> needToCountList, bool modified)
        {
            foreach (var needToCount in needToCountList)
            {
                AddToFreqTable(needToCount, modified);
            }
        }

        public void VerifyPrefabs()
        {
            List<GameObject> gameObjects = new List<GameObject>();
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (scene.isLoaded)
                {
                    var roots = scene.GetRootGameObjects();
                    var gameObjectsInScene = roots.SelectMany(x => x.GetComponentsInChildren<Transform>(true)).Select(x => x.gameObject);
                    gameObjects.AddRange(gameObjectsInScene);
                }
            }

            List<GameObject> prefabs = new List<GameObject>();
            foreach (GameObject gameObject in gameObjects)
            {
                PrefabType type = PrefabUtility.GetPrefabType(gameObject);
                switch (type)
                {
                    case PrefabType.PrefabInstance:
                        var prefabRootInstance = PrefabUtility.FindValidUploadPrefabInstanceRoot(gameObject);
                        if (prefabRootInstance == gameObject)
                        {
                            prefabs.Add(gameObject);
                        }
                        break;

                    case PrefabType.MissingPrefabInstance:
                    case PrefabType.DisconnectedPrefabInstance:
                    case PrefabType.DisconnectedModelPrefabInstance:
                        DisconnectedPrefabInstances.Add(gameObject);
                        break;

                    default:
                        break;
                }
            }

            var modprefabs = _predicates.DivideToModifiedAndNonmodified(prefabs);
            ObjectsWithPredicateList<GameObject> modifiedObjs = null, nonmodifiedObjs = null;
            foreach (var prefs in modprefabs)
            {
                if (prefs._className == "Modified")
                    modifiedObjs = prefs;
                if (prefs._className == null)
                    nonmodifiedObjs = prefs;
            }
            if (modifiedObjs != null)
            {
                AddToFreqTable(modifiedObjs._objects, true);
                foreach (var obj in modifiedObjs._objects)
                {
                    List<PropertyModification> valuableModifiedProperties = modifiedObjs.GetValuableModifiedPropertiesList(obj);
                    ModifiedPrefabInstance.Add(obj, valuableModifiedProperties);
                }
            }
            if (nonmodifiedObjs != null)
            {
                AddToFreqTable(nonmodifiedObjs._objects, false);
            }
            // uncomment when filling predicate list from JSON will be made
            //var mod = _predicates.DoMatch(modifiedObjs.ToArray());
        }

        private static bool ModifiedValuable(PropertyModification[] propsToCount, string[] propNamesToExclude)
        {
            bool modified = false;
            foreach (var prop in propsToCount)
            {
                bool containUnignored = false;
                bool isEnabled = false;
                foreach (var name in propNamesToExclude)
                {
                    if (!prop.propertyPath.StartsWith(name))
                    {
                        containUnignored = true;
                        if (prop.propertyPath == "m_Enabled")
                        {
                            isEnabled = true;
                        }
                    }
                }

                if (containUnignored == true && isEnabled == true)
                {
                    modified = true;
                }

            }
            return modified;
        }

        private List<PropertyModification> ExcludeProperties(PropertyModification[] excludeFrom, string[] propNamesToExclude)
        {
            List<PropertyModification> result = new List<PropertyModification>();
            foreach (var prop in excludeFrom)
            {
                bool add = true;
                foreach (var name in propNamesToExclude)
                {
                    if (prop.propertyPath.StartsWith(name))
                    {
                        add = false;
                        break;
                    }
                }

                if (add)
                    result.Add(prop);
            }
            return result;
        }

        public Dictionary<Object, GameObjectWithModificationInfo> GetFreqTable() => _freqTable;

        public Dictionary<GameObject, List<PropertyModification>> GetModifiedTable() => ModifiedPrefabInstance;

        public List<GameObject> GetListOfDiconnectedPrefabs() => DisconnectedPrefabInstances;

        public void SelectObjectsByID(IList<int> IDs)
        {
            List<GameObject> toSelect = new List<GameObject>();
            foreach (var id in IDs)
            {
                var objectWithID = EditorUtility.InstanceIDToObject(id);
                if (objectWithID != null)
                {
                    switch (PrefabUtility.GetPrefabType(objectWithID))
                    {
                        case PrefabType.Prefab:
                            toSelect.AddRange(_freqTable[objectWithID]._modifiedGameObjects);
                            toSelect.AddRange(_freqTable[objectWithID]._nonmodifiedGameObjects);
                            break;
                        case PrefabType.PrefabInstance:
                        case PrefabType.MissingPrefabInstance:
                        case PrefabType.DisconnectedPrefabInstance:
                        case PrefabType.DisconnectedModelPrefabInstance:
                            try
                            {
                                toSelect.Add((GameObject)objectWithID);
                            }
                            catch
                            {
                                throw new UnityException("Unsuccesful cast to GameObject");
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            GameObject[] currentSelection = Selection.gameObjects;
            foreach (var select in toSelect)
            {
                ArrayUtility.Add(ref currentSelection, select);
            }
            Selection.objects = currentSelection;
        }

        public void ClearCurrentSelection()
        {
            //GameObject[] currentSelection = Selection.gameObjects;
            //foreach (var gameObject in currentSelection)
            //{
            //    ArrayUtility.Remove(ref currentSelection, gameObject);
            //}
            //Selection.objects = currentSelection;
            Selection.objects = null;
        }

        public void DeleteGameObjectsInCurrentSelection()
        {
            var scenes = new HashSet<Scene>();
            List<GameObject> currentSelection = new List<GameObject>(Selection.gameObjects);
            Selection.objects = null;
            foreach (var gameObject in currentSelection)
            {
                scenes.Add(gameObject.scene);
                GameObject.DestroyImmediate(gameObject);
            }
            foreach (var scene in scenes)
            {
                EditorSceneManager.MarkSceneDirty(scene);
            }
        }
    }

    class GameObjectWithModificationInfo
    {
        internal List<GameObject> _modifiedGameObjects = new List<GameObject>();
        internal List<GameObject> _nonmodifiedGameObjects = new List<GameObject>();
        public int Count { get; private set; } = 0;
        private int _modifiedCount = 0, _nonmodifiedCount = 0;

        internal void Add(GameObject go, bool isModified)
        {
            if (isModified)
            {
                _modifiedGameObjects.Add(go);
                _modifiedCount = _modifiedGameObjects.Count;
            }
            else
            {
                _nonmodifiedGameObjects.Add(go);
                _nonmodifiedCount = _nonmodifiedGameObjects.Count;
            }
            Count = _modifiedCount + _nonmodifiedCount;
        }

        public GameObject this[int i]
        {
            get
            {
                if (i > Count)
                    throw new System.IndexOutOfRangeException("Index was outsude of the bounds");
                if (i < _modifiedCount)
                    return _modifiedGameObjects[i];
                return _nonmodifiedGameObjects[i - _modifiedCount];
            }
        }
    }
}
