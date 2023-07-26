using System;
using System.Collections.Generic;
using Assets.Editor.SceneProcessors;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Assets.TerrainBaker.Editor
{
    public class ScenePostProcess : IProcessSceneWithReport
    {
        Type[] checkForSingle = new Type[]
        {
            typeof(Terrain),
            typeof(TerrainCollider),
            typeof(TerrainBaker),
            typeof(MeshRenderer),
            typeof(MeshFilter),
            typeof(TerrainBakerMaterialSupport)
        };

        Type[] removeComponentsByTypeFromCurrent = new Type[]
        {
            typeof(Terrain),
            typeof(TerrainCollider),
            typeof(TerrainBaker)
        };

        string[] removeComponentsByNameFromThisAndParents = {"TerrainFormer", "TerrainSetNeighbours", "ReliefTerrain", "BigGameObjectMarkerBehaviour"};


        public int callbackOrder => 1;

        public void OnProcessScene(Scene scene, BuildReport report)
        {
            //var debugTimer = new System.Diagnostics.Stopwatch();
            //debugTimer.Start();

            bool isBakersPresent = false;
            List<Component> removeComponents = new List<Component>();
            foreach (GameObject obj in scene.CollectGameObjectsWithComponent<TerrainBaker>())
            {
                isBakersPresent = true;
                if (CheckSingles(obj, checkForSingle))
                {
                    continue;
                }

                CollectByType(obj, removeComponentsByTypeFromCurrent, removeComponents);
                foreach (Transform transform in obj.GetComponentsInParent<Transform>())
                {
                    CollectByName(obj, removeComponentsByNameFromThisAndParents, removeComponents);
                }

                RemoveComponents(removeComponents);
                removeComponents.Clear();
            }

            if (isBakersPresent)
            {
                foreach (GameObject obj in SceneHelpers.CollectGameObjectsWithComponent<Terrain>(scene))
                {
                    Debug.LogErrorFormat("In the scene still present GameObject {0} with a component Terran", obj.name);
                }
            }

            //var seconds = (debugTimer?.ElapsedMilliseconds ?? 0) / 1000.0f;
            //Debug.LogError($"TerrainBaker.ScenePostProcess, time: {seconds} sec, scene: {scene.name}");
        }

        private static bool CheckSingles(GameObject obj, Type[] checkForSingle)
        {
            bool haveErrors = false;
            foreach (Type type in checkForSingle)
            {
                Component[] comps = obj.GetComponents(type);
                if (comps.Length == 1)
                {
                    continue;
                }

                haveErrors = true;
                if (comps.Length == 0)
                {
                    Debug.LogErrorFormat("GameObject {0} must contain a component {1}", obj.name, type);
                }
                else
                {
                    Debug.LogErrorFormat("GameObject {0} contains more then one component {1}", obj.name, type);
                }
            }

            return haveErrors;
        }

        private static void CollectByType(GameObject obj, Type[] types, List<Component> components)
        {
            foreach (Type type in types)
            {
                foreach (Component cmp in obj.GetComponents(type))
                {
                    AddUniqComponent(components, cmp);
                }
            }
        }

        private static void CollectByName(GameObject obj, string[] names, List<Component> components)
        {
            foreach (string typeName in names)
            {
                MonoBehaviour[] comps = obj.GetComponents<MonoBehaviour>();
                foreach (MonoBehaviour cmp in comps)
                {
                    if (cmp == default(MonoBehaviour))
                    {
                        Debug.LogErrorFormat("Missing component on gameobject {0}", obj.name);
                        continue;
                    }

                    string fullName = cmp.GetType().FullName;
                    if (fullName.EndsWith(typeName))
                    {
                        AddUniqComponent(components, cmp);
                    }
                }
            }
        }

        private static void AddUniqComponent(List<Component> components, Component cmp)
        {
            if (components.FindIndex(c => c == cmp) < 0)
            {
                components.Add(cmp);
            }
        }

        private static void RemoveComponents(List<Component> removeComponents)
        {
            for (int i = removeComponents.Count - 1; i >= 0; i--)
            {
                if (removeComponents[i] != null)
                {
                    Object.DestroyImmediate(removeComponents[i]);
                }
            }
        }
    }
}