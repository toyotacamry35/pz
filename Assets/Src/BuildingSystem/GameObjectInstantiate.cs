using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.Src.GameObjectAssembler;
using Assets.Src.GameObjectAssembler.Res;
using Assets.Src.ResourcesSystem.Base;
using UnityEngine;

namespace Assets.Src.BuildingSystem
{
    public static class GameObjectInstantiate
    {
        public static GameObject Invoke(UnityRef<GameObject> prefab, ResourceRef<UnityGameObjectDef> prefabDef, Vector3 position, Quaternion rotation, bool setActive)
        {
            GameObject result = null;
            if (prefabDef != null)
            {
                result = JsonToGO.Instance.InstantiateAndMergeWith(prefab?.Target, prefabDef, position, rotation, setActive);
            }
            else if ((prefab != null) && (prefab.Target != null))
            {
                bool prefabActive = prefab.Target.activeSelf;
                if (prefabActive) { prefab.Target?.SetActive(false); }
                result = GameObject.Instantiate(prefab.Target, position, rotation);
                if (prefabActive) { prefab.Target?.SetActive(true); }
                if (setActive) { result.SetActive(true); }
            }
            else
            {
                result = new GameObject("BuildingElement without prefab");
                result.transform.SetPositionAndRotation(position, rotation);
                result.SetActive(setActive);
            }
            UnityEngine.Object.DontDestroyOnLoad(result);
            return result;
        }
    }
}