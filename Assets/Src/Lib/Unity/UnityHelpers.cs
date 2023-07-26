using Assets.Src.GameObjectAssembler;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace Assets.Src.Lib.Unity
{
    public static class UnityHelpers
    {
        private static CustomSampler _sampler;
        public static GameObject InstantiateWithPrototype(GameObject original, Transform parent, bool worldPositionStays)
        {
            if (_sampler == null)
                _sampler = CustomSampler.Create("UnityHelpers.InstantiateWithPrototype");

            _sampler.Begin();

            var goInstance = JsonToGO.Instance.InstantiateAndMerge(original, Vector3.zero, Quaternion.identity, false);
            goInstance.transform.SetParent(parent, worldPositionStays);
            goInstance.SetActive(true);

            _sampler.End();
            return goInstance;
        }

        public static Dictionary<string, int> GameObjectInstantiateStatistics = new Dictionary<string, int>();

        private static void AddinstantiateStatictic<T>(T obj) where T : Object
        {
            var str = obj.ToString();
            int value = 0;
            if (GameObjectInstantiateStatistics.TryGetValue(str, out value))
            {
                GameObjectInstantiateStatistics[str] = value + 1;
            }
            else
            {
                GameObjectInstantiateStatistics[str] = 1;
            }
        }

        public static Object Instantiate(Object original)
        {
            AddinstantiateStatictic(original);
            return GameObject.Instantiate(original);
        }

        public static T Instantiate<T>(T original, Transform parent) where T : Object
        {
            AddinstantiateStatictic(original);
            return GameObject.Instantiate(original, parent);
        }

        public static T Instantiate<T>(T original, Vector3 position, Quaternion rotation, Transform parent) where T : Object
        {
            AddinstantiateStatictic(original);
            return GameObject.Instantiate(original, position, rotation, parent);
        }

        public static T Instantiate<T>(T original, Vector3 position, Quaternion rotation) where T : Object
        {
            AddinstantiateStatictic(original);
            return GameObject.Instantiate(original, position, rotation);
        }

        public static T Instantiate<T>(T original) where T : Object
        {
            AddinstantiateStatictic(original);
            return GameObject.Instantiate(original);
        }

        public static Object Instantiate(Object original, Transform parent)
        {
            AddinstantiateStatictic(original);
            return GameObject.Instantiate(original, parent);
        }
    }
}
