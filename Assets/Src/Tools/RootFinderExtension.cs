using JetBrains.Annotations;
using Assets.Src.Lib.Unity;
using UnityEngine;
using Assets.Src.SpawnSystem;

namespace Assets.Src.Tools
{
    public static class RootFinderExtension
    {
        [Pure]
        public static GameObject GetRoot(this GameObject obj)
        {
            if (!obj)
                return null;
            var current = obj.transform;
            var root = current.root;
            while (root != current)
            {
                if (current.GetComponent<GameObjectRootMarker>() || current.GetComponent<EntityGameObject>()) 
                    break;
                if (!current.transform.parent)
                    break;
                current = current.transform.parent;
            }
            return current.gameObject;
        }
    }
}
