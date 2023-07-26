using UnityEngine.SceneManagement;
using SharedCode.Aspects.Cartographer;

namespace Assets.Src.Cartographer.Editor
{
    public class CSOCollectVisuals : ICartographerSceneOperation
    {
        public class MessagesClass : IProgressMessages
        {
            public string Title { get { return "Collect Visuals"; } }
            public string RunQuestion { get { return "Are you sure you want to collect visuals?"; } }
            public string WelcomeMessage { get { return "Collect visuals"; } }
            public string OnScenePrefix { get { return "Collect visuals"; } }
        }
        public static MessagesClass Messages = new MessagesClass();

        // constructor ------------------------------------------------------------------------
        public CSOCollectVisuals()
        {
        }

        // ICartographerSceneOperation interface ----------------------------------------------------------
        public IProgressMessages ProgressMessages { get { return Messages; } }

        public bool CanOperate(CartographerScene cartographerScene, CartographerParamsDef cartographerParams, SceneCollectionDef sceneCollection, bool openedScenesOnly)
        {
            if ((cartographerScene.TypeMask & CartographerSceneType.BackgroundClient) == CartographerSceneType.BackgroundClient)
            {
                return true;
            }
            return false;
        }

        public bool Start(CartographerParamsDef cartographerParams, SceneCollectionDef sceneCollection, bool openedScenesOnly, ICartographerProgressCallback progressCallback)
        {
            return true;
        }

        public bool Operate(Scene scene, CartographerSceneType cartographerSceneTypeMask, CartographerParamsDef cartographerParams, SceneCollectionDef sceneCollection, bool openedScenesOnly, ICartographerProgressCallback progressCallback)
        {
            return true;
        }

        public void Finish(CartographerParamsDef cartographerParams, SceneCollectionDef sceneCollection, bool openedScenesOnly, ICartographerProgressCallback progressCallback)
        {
        }
    }
};