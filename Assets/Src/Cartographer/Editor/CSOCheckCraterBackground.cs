using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using SharedCode.Aspects.Cartographer;

namespace Assets.Src.Cartographer.Editor
{
    //old unused checker actually it is just a dude
    public class CSOCheckCraterBackground : ICartographerSceneOperation
    {
        public class MessagesClass : IProgressMessages
        {
            public string Title { get { return "Check Crater Background"; } }
            public string RunQuestion { get { return "Are you sure you want to check crater background?"; } }
            public string WelcomeMessage { get { return "Check crater background"; } }
            public string OnScenePrefix { get { return "Check crater background"; } }
        }
        public static MessagesClass messages { get; } = new MessagesClass();

        private Vector3Int checkedCoordinates = new Vector3Int();
        private Scene craterScene = new Scene(); // !!! unfilled !!!

        private void CheckScene(Scene craterScene, Scene scene, Vector3Int coordinates, CartographerParamsDef.BackgroundClientCreation craterCreationParams, ICartographerProgressCallback progressCallback)
        {
            var somethingChanged = false;
            var rootGameObjects = CartographerCommon.GetRootGameObjects(scene);
            foreach (var rootGameObject in rootGameObjects)
            {
                var sceneLoaderBehaviour = rootGameObject.GetComponent<SceneLoaderBehaviour>();
                if (sceneLoaderBehaviour != null)
                {
                    //TODO
                }
                else
                {
                    CartographerCommon.ReportError($"CreateCraterBackground, Unknown root object: {rootGameObject.name}, scene: {scene.name}");
                }
            }
            if (somethingChanged)
            {
                progressCallback?.OnProgress(messages.Title, $"Saving scene: {scene.name}", 1.0f);
                EditorSceneManager.SaveScene(scene);
            }
        }

        // ICartographerSceneOperation interface ----------------------------------------------------------
        public IProgressMessages ProgressMessages { get { return messages; } }

        public bool CanOperate(CartographerScene cartographerScene, CartographerParamsDef cartographerParams, SceneCollectionDef sceneCollection, bool openedScenesOnly)
        {
            if (((cartographerScene.TypeMask & CartographerSceneType.StreamCollection) == CartographerSceneType.StreamCollection) && CartographerCommon.IsSceneForStreaming(cartographerScene.Name, out checkedCoordinates))
            {
                if (CartographerCommon.InsideRect(checkedCoordinates, cartographerParams.BackgroundClientCreationParams.GenerateTerrainColliders) ||
                    CartographerCommon.InsideRect(checkedCoordinates, cartographerParams.BackgroundClientCreationParams.GenerateTerrainVisuals) ||
                    CartographerCommon.InsideRect(checkedCoordinates, cartographerParams.BackgroundClientCreationParams.GenerateStaticObjectVisuals))
                {
                    return true;
                }
            }
            return false;
        }

        public bool Start(CartographerParamsDef cartographerParams, SceneCollectionDef sceneCollection, bool openedScenesOnly, ICartographerProgressCallback progressCallback)
        {
            return true;
        }

        public bool Operate(Scene scene, CartographerSceneType cartographerSceneTypeMask, CartographerParamsDef cartographerParams, SceneCollectionDef sceneCollection, bool openedScenesOnly, ICartographerProgressCallback progressCallback)
        {
            CheckScene(craterScene, scene, checkedCoordinates, cartographerParams.BackgroundClientCreationParams, progressCallback);
            return true;
        }

        public void Finish(CartographerParamsDef cartographerParams, SceneCollectionDef sceneCollection, bool openedScenesOnly, ICartographerProgressCallback progressCallback)
        {
        }
    }
};