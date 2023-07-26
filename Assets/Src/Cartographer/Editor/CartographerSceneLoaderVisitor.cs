using UnityEngine;

namespace Assets.Src.Cartographer.Editor
{
    public class CartographerSceneLoaderVisitor
    {
        public delegate bool VisitorDelegate(GameObject gameObject, SceneLoaderGameObjectType gameObjectType);

        public static void Visit(GameObject sceneLoaderGameObject, VisitorDelegate visitor)
        {
            var sceneLoaderBehaviour = sceneLoaderGameObject.GetComponent<SceneLoaderBehaviour>();
            if (sceneLoaderBehaviour != null)
            {
                var continueVisiting = true;
                if (continueVisiting)
                {
                    foreach (var gameObject in sceneLoaderBehaviour.GameObjectsTerrain)
                    {
                        continueVisiting = visitor(gameObject, SceneLoaderGameObjectType.GameObjectTerrain);
                        if (!continueVisiting)
                        {
                            break;
                        }
                    }
                }
                if (continueVisiting)
                {
                    foreach (var gameObject in sceneLoaderBehaviour.GameObjectsStatic)
                    {
                        continueVisiting = visitor(gameObject, SceneLoaderGameObjectType.GameObjectStatic);
                        if (!continueVisiting)
                        {
                            break;
                        }
                    }
                }
                if (continueVisiting)
                {
                    foreach (var gameObject in sceneLoaderBehaviour.GameObjectsFX)
                    {
                        continueVisiting = visitor(gameObject, SceneLoaderGameObjectType.GameObjectFX);
                        if (!continueVisiting)
                        {
                            break;
                        }
                    }
                }
            }
        }
    }
};