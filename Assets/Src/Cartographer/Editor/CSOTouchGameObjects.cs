using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using SharedCode.Aspects.Cartographer;

namespace Assets.Src.Cartographer.Editor
{
    public class CSOTouchGameObjects : ICartographerSceneOperation
    {
        // messages -------------------------------------------------------------------------------
        public class MessagesClass : IProgressMessages
        {
            public string Title { get { return "Touch GameObjects"; } }
            public string RunQuestion { get { return "Are you sure you want to touch all gameObjects?"; } }
            public string WelcomeMessage { get { return "Touch GameObjects"; } }
            public string OnScenePrefix { get { return "Touch GameObjects"; } }
        }

        public static MessagesClass Messages = new MessagesClass();

        // data -----------------------------------------------------------------------------------
        private string newTag = string.Empty;
        private HashSet<string> prefabPaths = new HashSet<string>();

        //main procedures -------------------------------------------------------------------------
        private bool TouchGameObject(Scene scene, GameObject gameObject, CartographerSceneType cartographerSceneTypeMask, ref bool currentSceneChanged)
        {
            if (CartographerCommon.IsGameObjectFolder(gameObject))
            {
                if (gameObject.transform.childCount > 0)
                {
                    return true;
                }
            }

            var prefabType = PrefabUtility.GetPrefabInstanceStatus(gameObject);
            if (prefabType != PrefabInstanceStatus.Connected)
            {
                var prefabGameObject = PrefabUtility.GetCorrespondingObjectFromSource(gameObject);
                if (prefabGameObject != null)
                {
                    var prefabPath = CartographerCommon.GetAssetPath(prefabGameObject);
                    prefabPaths.Add(prefabPath);
                }
            }
            return false;
        }

        // constructor ----------------------------------------------------------------------------
        public CSOTouchGameObjects(string _newTag)
        {
            newTag = _newTag;
        }

        // ICartographerSceneOperation interface --------------------------------------------------
        public IProgressMessages ProgressMessages { get { return Messages; } }

        public bool CanOperate(CartographerScene cartographerScene, CartographerParamsDef cartographerParams, SceneCollectionDef sceneCollection, bool openedScenesOnly)
        {
            return true;
        }

        public bool Start(CartographerParamsDef cartographerParams, SceneCollectionDef sceneCollection, bool openedScenesOnly, ICartographerProgressCallback progressCallback)
        {
            prefabPaths.Clear();
            return true;
        }

        public bool Operate(Scene scene, CartographerSceneType cartographerSceneTypeMask, CartographerParamsDef cartographerParams, SceneCollectionDef sceneCollection, bool openedScenesOnly, ICartographerProgressCallback progressCallback)
        {
            var currentSceneChanged = false;
            CartographerSceneObjectVisitor.Visit(scene, gameObject => { return TouchGameObject(scene, gameObject, cartographerSceneTypeMask, ref currentSceneChanged); });
            if (currentSceneChanged)
            {
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
            }
            return true;
        }

        public void Finish(CartographerParamsDef cartographerParams, SceneCollectionDef sceneCollection, bool openedScenesOnly, ICartographerProgressCallback progressCallback)
        {
            if((prefabPaths != null) && (prefabPaths.Count > 0))
            {
                foreach(var prefabPath in prefabPaths)
                {
                    var changed = false;
                    var rootGameObject = PrefabUtility.LoadPrefabContents(prefabPath);
                    //TODO

                    //newTag

                    //TODO
                    if (changed)
                    {
                        PrefabUtility.SavePrefabAsset(rootGameObject);
                    }
                    PrefabUtility.UnloadPrefabContents(rootGameObject);

                    //for testing
                    break;
                }
            }
        }
    }
};