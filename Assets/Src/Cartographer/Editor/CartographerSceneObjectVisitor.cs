using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

namespace Assets.Src.Cartographer.Editor
{
    public class CartographerSceneObjectVisitor
    {
        private static void VisitGameObjects(List<GameObject> gameObjects, Func<GameObject, bool> visitor)
        {
            if ((gameObjects != null) && (gameObjects.Count > 0))
            {
                foreach (var gameObject in gameObjects)
                {
                    if (visitor(gameObject))
                    {
                        VisitGameObjects(CartographerCommon.GetChildren(gameObject), visitor);
                    }
                }
            }
        }

        public static void Visit(Scene scene, Func<GameObject, bool> visitor)
        {
            VisitGameObjects(CartographerCommon.GetRootGameObjects(scene), visitor);
        }
    }
};