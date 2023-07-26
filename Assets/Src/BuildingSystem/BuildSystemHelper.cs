using Assets.Src.Tools;
using SharedCode.Aspects.Building;
using SharedCode.DeltaObjects.Building;
using UnityEngine;
using SharedCode.Utils;
using System.Reflection;

namespace Assets.Src.BuildingSystem
{
    public static class BuildSystemHelper
    {
        private static System.Random random = new System.Random();

        private static bool visualRaycastEnableCheat = false;
        private static GameObject visualRaycastEnableCheatGameObject = null;

        public static void SetVisualRaycastCheat(bool enable)
        {
            visualRaycastEnableCheat = enable;
            if (visualRaycastEnableCheat && (visualRaycastEnableCheatGameObject == null))
            {
                var buildParamsDef = BuildUtils.BuildParamsDef;
                visualRaycastEnableCheatGameObject = GameObjectInstantiate.Invoke(buildParamsDef.RaycastHelperPrefab, buildParamsDef.RaycastHelperPrefabDef, UnityEngine.Vector3.zero, UnityEngine.Quaternion.identity, true);
            }
            else if(!visualRaycastEnableCheat && (visualRaycastEnableCheatGameObject != null))
            {
                GameObject.Destroy(visualRaycastEnableCheatGameObject);
                visualRaycastEnableCheatGameObject = null;
            }
            if (visualRaycastEnableCheatGameObject != null)
            {
                visualRaycastEnableCheatGameObject.SetActive(visualRaycastEnableCheat);
            }
            BuildUtils.Message?.Report($"enable: {visualRaycastEnableCheat}, gameObject: {((visualRaycastEnableCheatGameObject == null)? "NULL" : "Created")}", MethodBase.GetCurrentMethod().DeclaringType.Name);
        }

        public class ElementDataHitInfo
        {
            public BuildType BuildType { get; set; } = BuildType.None;
            public float Distance { get; set; } = 0.0f;
            public GameObject GameObject { get; set; } = null;
            public ElementData Data { get; set; } = null;
        }

        public static ElementDataHitInfo RayCast(Ray ray)
        {
            var buildParamsDef = BuildUtils.BuildParamsDef;
            ElementDataHitInfo elementDataHitInfo = new ElementDataHitInfo();
            ray.origin += ray.direction * buildParamsDef.RaycastNearDistance;
            if ((visualRaycastEnableCheatGameObject != null) && visualRaycastEnableCheatGameObject.activeSelf)
            {
                visualRaycastEnableCheatGameObject.transform.position = ray.origin;
                visualRaycastEnableCheatGameObject.transform.rotation = UnityEngine.Quaternion.LookRotation(ray.direction);
            }
            var hits = Physics.RaycastAll(ray, PhysicsChecker.CheckDistance(buildParamsDef.RaycastFarDistance, "BuildingSystemHelper"));
            if ((hits != null) && (hits.Length > 0))
            {
                float minDistance = hits[0].distance;
                foreach (var hit in hits)
                {
                    var buildingElementBehaviour = hit.transform.GetComponentInParent<BuildingElementBehaviour>();
                    if (buildingElementBehaviour != null)
                    {
                        var localData = buildingElementBehaviour.GetData();
                        if (localData != null)
                        {
                            if ((elementDataHitInfo.Data == null) || (hit.distance < minDistance))
                            {
                                elementDataHitInfo.BuildType = BuildType.BuildingElement;
                                elementDataHitInfo.Distance = hit.distance;
                                elementDataHitInfo.GameObject = hit.transform.gameObject;
                                elementDataHitInfo.Data = localData;
                                minDistance = hit.distance;
                            }
                        }
                    }
                    else
                    {
                        var fenceElementBehaviour = hit.transform.GetComponentInParent<FenceElementBehaviour>();
                        if (fenceElementBehaviour != null)
                        {
                            var localData = fenceElementBehaviour.GetData();
                            if (localData != null)
                            {
                                if ((elementDataHitInfo.Data == null) || (hit.distance < minDistance))
                                {
                                    elementDataHitInfo.BuildType = BuildType.FenceElement;
                                    elementDataHitInfo.Distance = hit.distance;
                                    elementDataHitInfo.GameObject = hit.transform.gameObject;
                                    elementDataHitInfo.Data = localData;
                                    minDistance = hit.distance;
                                }
                            }
                        }
                    }
                }
            }
            return elementDataHitInfo;
        }

        public static float GetInitialHealth(this BuildRecipeDef def)
        {
            if ((def != null) && (def.Stats != null))
            {
                return def.Stats.Health;
            }
            return 0.0f;
        }

        public static int GetInitialVisual(this BuildRecipeDef def)
        {
            if ((def != null) && (def.Visual != null) && def.Visual.VersionsEnable)
            {
                int totalWeigth = 0;
                foreach (var version in def.Visual.Versions)
                {
                    if (version.Weight > 0)
                    {
                        totalWeigth += version.Weight;
                    }
                }
                if (totalWeigth > 0)
                {
                    int weight = random.Next(totalWeigth);
                    for (int index = 0; index < def.Visual.Versions.Count; ++index)
                    {
                        if (def.Visual.Versions[index].Weight > 0)
                        {
                            weight -= def.Visual.Versions[index].Weight;
                            if (weight < 0)
                            {
                                return index;
                            }
                        }
                    }
                    return 0;
                }
            }
            return -1;
        }

        public static int GetInitialInteraction(this BuildRecipeDef def)
        {
            if ((def != null) && (def.Interaction != null) && def.Interaction.Enable)
            {
                return def.Interaction.DefaultInteraction;
            }
            return -1;
        }

        public static bool GetNextInteraction(this BuildRecipeDef def, int interaction, out int nextInteraction)
        {
            nextInteraction = interaction;
            if ((def != null) &&
                (def.Interaction != null) &&
                (def.Interaction.Enable) &&
                (def.Interaction.Interactions != null) &&
                (interaction >= 0) &&
                (interaction <= def.Interaction.Interactions.Count))
            {
                nextInteraction = def.Interaction.Interactions[interaction].NextInteraction;
                return true;
            }
            return false;
        }

        public static string GetVisualCommonName(this BuildRecipeDef def, int visual)
        {
            if ((def != null) && (def.Visual != null) && def.Visual.VersionsEnable)
            {
                return def.Visual.VersionsCommon;
            }
            return string.Empty;
        }

        public static string GetVisualVersionName(this BuildRecipeDef def, int visual)
        {
            if ((def != null) &&
                (def.Visual != null) &&
                (def.Visual.VersionsEnable) &&
                (def.Visual.Versions != null) &&
                (visual > 0) &&
                (visual < def.Visual.Versions.Count))
            {
                return def.Visual.Versions[visual].Name;
            }
            return string.Empty;
        }
    }
}
