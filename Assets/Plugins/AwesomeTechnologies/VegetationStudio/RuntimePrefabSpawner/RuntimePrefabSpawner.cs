using System.Collections.Generic;
using UnityEngine;
using AwesomeTechnologies.Utility;
using AwesomeTechnologies.Vegetation;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AwesomeTechnologies.RuntimePrefabSpawner
{
    public enum RuntimePrefabRange
    {
        Normal = 0,
        Long = 1,
        VeryLong = 2
    }

    public class VegetationCellRuntimePrefab
    {
        public VegetationCell VegetationCell;
        public List<GameObject> RuntimePrefabList = new List<GameObject>();
        public int RuntimePrefabCount;
    }

    [HelpURL("http://www.awesometech.no/index.php/runtime-prefab-spawner")]
    public class RuntimePrefabSpawner : MonoBehaviour
    {
        public VegetationSystem VegetationSystem;      
        public List<RuntimePrefabRule> RuntimePrefabRuleList = new List<RuntimePrefabRule>();
        public RuntimePrefabRange RuntimePrefabRange;
        public GameObject RuntimePrefabParent;
        public bool HideRuntimePrefabs;
        public bool ShowCellGrid;
        public string SelectedVegetationItemID = "";

        private List<VegetationCellRuntimePrefab> _vegetationCellRuntimePrefabList = new List<VegetationCellRuntimePrefab>();

        public delegate void MultiCreateRuntimePrefabDelegate(GameObject go);
        public MultiCreateRuntimePrefabDelegate OnCreateRuntimePrefabDelegate;
        public delegate void MultiBeforeDestroyRuntimePrefabDelegate(GameObject go);
        public MultiBeforeDestroyRuntimePrefabDelegate OnBeforeDestroyRuntimePrefabDelegate;

        public delegate void MultiEvaluateRuntimePrefabSpawnChanceDelegate(string vegetationItemInstanceID, ref float spawnFrequency,int seed);
        public MultiEvaluateRuntimePrefabSpawnChanceDelegate OnEvaluateRuntimePrefabSpawnChance;

        // ReSharper disable once UnusedMember.Local
        void OnEnable()
        {
            FindVegetationSystem();
            if (VegetationSystem)
            {
                VegetationSystem.OnVegetationCellVisibleDelegate += OnVegetationCellVisible;
                VegetationSystem.OnVegetationCellInvisibleDelegate += OnVegetationInvisible;
                VegetationSystem.OnResetVisibleCellListDelegate += OnResetVisibleCellList;
                VegetationSystem.OnVegetationCellChangeDistanceBandDelegate += OnVegetationCellChangeDistanceBand;
                VegetationSystem.OnSetVegetationPackageDelegate += OnSetVegetationPackage;
            }

            _vegetationCellRuntimePrefabList = new List<VegetationCellRuntimePrefab>();

            if (!RuntimePrefabParent)
            {
                CreateRuntimePrefabParent();
            }

            RefreshRuntimePrefabs();
        }

        public void FindVegetationSystem()
        {
            VegetationSystem = gameObject.GetComponentInChildren<VegetationSystem>();
        }
        void CreateRuntimePrefabParent()
        {
            List<GameObject> deleteList = new List<GameObject>();

            foreach (Transform child in transform)
            {
                if (child.transform.name == "RuntimePrefabs")
                {
                    deleteList.Add(child.gameObject);

                }
            }


            for (int i = 0; i <= deleteList.Count - 1; i++)
            {
                if (Application.isPlaying)
                {
                    Destroy(deleteList[i]);
                }
                else
                {
                    DestroyImmediate(deleteList[i]);
                }
            }

            RuntimePrefabParent = new GameObject("RuntimePrefabs");
            RuntimePrefabParent.transform.SetParent(transform, false);
        }


        void OnVegetationCellVisible(VegetationCell vegetationCell, int distanceBand)
        {
            if (distanceBand <= (int)RuntimePrefabRange)
            {

                AddVegetationCellPrefabs(vegetationCell);
            }

#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }

        void OnVegetationInvisible(VegetationCell vegetationCell, int distanceBand)
        {
            RemoveVegetationCellPrefabs(vegetationCell);

#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }

        void OnVegetationCellChangeDistanceBand(VegetationCell vegetationCell, int distanceBand, int previousDistanceBand)
        {
            if (!vegetationCell.IsVisible)
            {
                RemoveVegetationCellPrefabs(vegetationCell);
                return;
            }

            if ((distanceBand <= (int)RuntimePrefabRange) && (previousDistanceBand > (int)RuntimePrefabRange))
            {
                AddVegetationCellPrefabs(vegetationCell);
            }
            else if (distanceBand > (int)RuntimePrefabRange)
            {
                RemoveVegetationCellPrefabs(vegetationCell);
            }
            else
            {

#if UNITY_EDITOR
                EditorUtility.SetDirty(this);
#endif
            }
        }

        public List<RuntimePrefabRule> GetActiveRuntimePrefabRules(string vegetationItemID)
        {
            List<RuntimePrefabRule> activeRuntimePrefabRuleList =  new List<RuntimePrefabRule>();

            for (int i = 0; i <= RuntimePrefabRuleList.Count - 1; i++)
            {
                if (vegetationItemID == RuntimePrefabRuleList[i].VegetationItemID &&
                    RuntimePrefabRuleList[i].RuntimePrefab != null)
                {
                    activeRuntimePrefabRuleList.Add(RuntimePrefabRuleList[i]);
                }
            }

            return activeRuntimePrefabRuleList;
        }

        public void AddVegetationCellPrefabs(VegetationCell vegetationCell)
        {
            vegetationCell.OnSpawnVegetationDelegate += OnSpawnVegetation;

            VegetationCellRuntimePrefab vegetationCellRuntimePrefab =
                new VegetationCellRuntimePrefab {VegetationCell = vegetationCell};

            VegetationPackage vegetationPackage = VegetationSystem.currentVegetationPackage;
            VegetationPlacingData vegetationPlacingData = VegetationSystem.currentVegetationPlacingData;

            int runtimePrefabCount = 0;

            if (vegetationCell.InitDone)
            {
                for (int i = 0; i <= vegetationPackage.VegetationInfoList.Count - 1; i++)
                {
                    VegetationItemInfo vegetationItemInfo = vegetationPackage.VegetationInfoList[i];
                    List<RuntimePrefabRule> activeRuntimePrefabRuleList =
                        GetActiveRuntimePrefabRules(vegetationItemInfo.VegetationItemID);

                    if (activeRuntimePrefabRuleList.Count == 0) continue;

                    if (vegetationItemInfo.VegetationRenderType == VegetationRenderType.InstancedIndirect)
                    {
                        CustomList<Matrix4x4> currentVegetationList = vegetationCell.GetCurrentIndirectVegetationList(i);
                        if (currentVegetationList == null) continue;

                        for (int k = 0; k <= activeRuntimePrefabRuleList.Count - 1; k++)
                        {
                            RuntimePrefabRule runtimePrefabRule = activeRuntimePrefabRuleList[k];

                            for (int j = 0; j <= currentVegetationList.Count - 1; j++)
                            {
                                CreatePrefab(currentVegetationList[j], i, runtimePrefabRule, vegetationPackage, vegetationPlacingData,
                                    ref runtimePrefabCount, vegetationCellRuntimePrefab);
                            }
                        }
                    }
                    else
                    {
                        List<Matrix4x4> currentVegetationList = vegetationCell.GetCurrentVegetationList(i);
                        if (currentVegetationList == null) continue;

                        for (int k = 0; k <= activeRuntimePrefabRuleList.Count - 1; k++)
                        {
                            RuntimePrefabRule runtimePrefabRule = activeRuntimePrefabRuleList[k];

                            for (int j = 0; j <= currentVegetationList.Count - 1; j++)
                            {
                                CreatePrefab(currentVegetationList[j], i, runtimePrefabRule, vegetationPackage, vegetationPlacingData,
                                    ref runtimePrefabCount, vegetationCellRuntimePrefab);
                            }
                        }
                    }
                }
            }

            vegetationCellRuntimePrefab.RuntimePrefabCount = runtimePrefabCount;
            _vegetationCellRuntimePrefabList.Add(vegetationCellRuntimePrefab);
        }

        void CreatePrefab(Matrix4x4 currentVegetationMatrix, int index, RuntimePrefabRule runtimePrefabRule, VegetationPackage vegetationPackage, VegetationPlacingData currentPlacingData, ref int runtimePrefabCount, VegetationCellRuntimePrefab vegetationCellRuntimePrefab)
        {
            Matrix4x4 colliderMatrix = currentVegetationMatrix;//currentVegetationList[j];
            Vector3 itemPosition = MatrixTools.ExtractTranslationFromMatrix(colliderMatrix);
            Random.InitState(Mathf.CeilToInt(itemPosition.x + itemPosition.z) + index + runtimePrefabRule.Seed);

            float spawnFrequency = runtimePrefabRule.SpawnFrequency;
            string vegetationItemInstanceID = GetVegetationItemInstanceID(itemPosition);

            if (OnEvaluateRuntimePrefabSpawnChance != null) OnEvaluateRuntimePrefabSpawnChance(vegetationItemInstanceID, ref spawnFrequency, runtimePrefabRule.Seed);

            if (Random.Range(0f, 1f) <= spawnFrequency)
            {
                runtimePrefabCount++;
                GameObject runtimePrefabObject = Instantiate(runtimePrefabRule.RuntimePrefab);
                runtimePrefabObject.name = runtimePrefabRule.RuntimePrefab.name;
                runtimePrefabObject.hideFlags = HideRuntimePrefabs ? HideFlags.HideAndDontSave : HideFlags.DontSave;
                runtimePrefabObject.layer = runtimePrefabRule.PrefabLayer;

                if (runtimePrefabObject && RuntimePrefabParent)
                {
                    runtimePrefabObject.transform.SetParent(RuntimePrefabParent.transform, false);
                }

                runtimePrefabObject.transform.position = itemPosition;

                runtimePrefabObject.transform.rotation = MatrixTools.ExtractRotationFromMatrix(colliderMatrix);
                runtimePrefabObject.transform.Rotate(runtimePrefabRule.PrefabRotation);

                runtimePrefabObject.transform.localScale = runtimePrefabRule.UseVegetationItemScale ? new Vector3(MatrixTools.ExtractScaleFromMatrix(colliderMatrix).x * runtimePrefabRule.PrefabScale.x, MatrixTools.ExtractScaleFromMatrix(colliderMatrix).y * runtimePrefabRule.PrefabScale.y, MatrixTools.ExtractScaleFromMatrix(colliderMatrix).z * runtimePrefabRule.PrefabScale.z) : runtimePrefabRule.PrefabScale;

                Vector3 scaledPrefabOffset =
                    new Vector3(
                        runtimePrefabObject.transform.localScale.x *
                        runtimePrefabRule.PrefabOffset.x,
                        runtimePrefabObject.transform.localScale.y *
                        runtimePrefabRule.PrefabOffset.y,
                        runtimePrefabObject.transform.localScale.z *
                        runtimePrefabRule.PrefabOffset.z);
                runtimePrefabObject.transform.localPosition += MatrixTools.ExtractRotationFromMatrix(colliderMatrix) * scaledPrefabOffset;

                RuntimeObjectInfo runtimeObjectInfo = runtimePrefabObject.AddComponent<RuntimeObjectInfo>();
                runtimeObjectInfo.VegetationItemInfo = vegetationPackage.VegetationInfoList[index];

                VegetationItemInstanceInfo vegetationItemInstanceInfo = runtimePrefabObject
                    .AddComponent<VegetationItemInstanceInfo>();
                vegetationItemInstanceInfo.VegetationItemInstanceID = vegetationItemInstanceID;
                vegetationItemInstanceInfo.Position =
                    MatrixTools.ExtractTranslationFromMatrix(colliderMatrix);
                vegetationItemInstanceInfo.Scale =
                    MatrixTools.ExtractScaleFromMatrix(colliderMatrix);
                vegetationItemInstanceInfo.Rotation =
                    MatrixTools.ExtractRotationFromMatrix(colliderMatrix);
                vegetationItemInstanceInfo.VegetationItemID =
                    vegetationPackage.VegetationInfoList[index].VegetationItemID;
                vegetationItemInstanceInfo.VegetationType =
                    vegetationPackage.VegetationInfoList[index].VegetationType;

                vegetationCellRuntimePrefab.RuntimePrefabList.Add(runtimePrefabObject);

                if (OnCreateRuntimePrefabDelegate != null) OnCreateRuntimePrefabDelegate(runtimePrefabObject);
            }
        }

        string GetVegetationItemInstanceID(Vector3 position)
        {
            string vegetationItemInstanceID = Mathf.RoundToInt(position.x * 100f).ToString() + "_" +
                                                                  Mathf.RoundToInt(position.y * 100f).ToString() + "_" +
                                                                  Mathf.RoundToInt(position.z * 100f).ToString();
            return vegetationItemInstanceID;
        }

        public void RemoveVegetationCellPrefabs(VegetationCell vegetationCell)
        {
            for (int i = 0; i <= _vegetationCellRuntimePrefabList.Count - 1; i++)
            {
                if (_vegetationCellRuntimePrefabList[i].VegetationCell == vegetationCell)
                {
                    for (int j = 0; j <= _vegetationCellRuntimePrefabList[i].RuntimePrefabList.Count - 1; j++)
                    {
                        if (OnBeforeDestroyRuntimePrefabDelegate != null)
                        {
                            OnBeforeDestroyRuntimePrefabDelegate(_vegetationCellRuntimePrefabList[i].RuntimePrefabList[j]);
                        }

                        if (Application.isPlaying)
                        {
                            Destroy(_vegetationCellRuntimePrefabList[i].RuntimePrefabList[j]);
                        }
                        else
                        {
                            DestroyImmediate(_vegetationCellRuntimePrefabList[i].RuntimePrefabList[j]);
                        }

                    }
                    _vegetationCellRuntimePrefabList.RemoveAt(i);
                    break;
                }
            }
            vegetationCell.OnSpawnVegetationDelegate -= OnSpawnVegetation;
        }


        void OnSpawnVegetation(VegetationCell vegetationCell)
        {
            RemoveVegetationCellPrefabs(vegetationCell);
            AddVegetationCellPrefabs(vegetationCell);
        }

        void OnResetVisibleCellList()
        {
            ClearVegetationCellRuntimePrefabList();
        }

        void OnSetVegetationPackage(VegetationPackage vegetationPackage)
        {
            RefreshRuntimePrefabs();
        }

        public void RefreshRuntimePrefabs()
        {
            if (!Application.isPlaying) return;

            ClearVegetationCellRuntimePrefabList();
            for (int i = 0; i <= VegetationSystem.VisibleVegetationCellList.Count - 1; i++)
            {
                if (VegetationSystem.VisibleVegetationCellList[i].DistanceBand <= (int)RuntimePrefabRange)
                {
                    AddVegetationCellPrefabs(VegetationSystem.VisibleVegetationCellList[i]);
                }
            }

            if (RuntimePrefabParent && HideRuntimePrefabs)
            {
                RuntimePrefabParent.hideFlags = HideFlags.HideInHierarchy;
            }
            else
            {
                if (RuntimePrefabParent) RuntimePrefabParent.hideFlags = HideFlags.None;
            }
        }

        void ClearVegetationCellRuntimePrefabList()
        {
            for (int i = 0; i <= _vegetationCellRuntimePrefabList.Count - 1; i++)
            {
                for (int j = 0; j <= _vegetationCellRuntimePrefabList[i].RuntimePrefabList.Count - 1; j++)
                {
                    if (Application.isPlaying)
                    {
                        Destroy(_vegetationCellRuntimePrefabList[i].RuntimePrefabList[j]);
                    }
                    else
                    {
                        DestroyImmediate(_vegetationCellRuntimePrefabList[i].RuntimePrefabList[j]);
                    }

                }
            }
            DeleteOldRuntimePrefabs();
            _vegetationCellRuntimePrefabList.Clear();
        }

        void DeleteOldRuntimePrefabs()
        {
            if (!RuntimePrefabParent) return;

            List<GameObject> childList = new List<GameObject>();
            foreach (Transform child in RuntimePrefabParent.transform)
            {
                childList.Add(child.gameObject);
            }

            for (int i = 0; i <= childList.Count - 1; i++)
            {
                if (Application.isPlaying)
                {
                    Destroy(childList[i]);
                }
                else
                {
                    DestroyImmediate(childList[i]);
                }
            }
        }


        // ReSharper disable once UnusedMember.Local
        void OnDisable()
        {
            if (VegetationSystem)
            {
                VegetationSystem.OnVegetationCellVisibleDelegate -= OnVegetationCellVisible;
                VegetationSystem.OnVegetationCellInvisibleDelegate -= OnVegetationInvisible;
                VegetationSystem.OnResetVisibleCellListDelegate -= OnResetVisibleCellList;
                VegetationSystem.OnVegetationCellChangeDistanceBandDelegate -= OnVegetationCellChangeDistanceBand;
                VegetationSystem.OnSetVegetationPackageDelegate -= OnSetVegetationPackage;
            }

            ClearVegetationCellRuntimePrefabList();
        }

        // ReSharper disable once UnusedMember.Local
        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            if (_vegetationCellRuntimePrefabList != null)
            {
                for (int i = 0; i <= _vegetationCellRuntimePrefabList.Count - 1; i++)
                {
                    if (_vegetationCellRuntimePrefabList[i].VegetationCell.InitDone && ShowCellGrid)
                    {
                        switch (_vegetationCellRuntimePrefabList[i].VegetationCell.DistanceBand)
                        {
                            case 0:
                                Gizmos.color = new Color(0, 1f, 0, 1F);
                                break;
                            case 1:
                                Gizmos.color = new Color(1f, 1f, 0, 1f);
                                break;
                            case 2:
                                Gizmos.color = new Color(1f, 0.5f, 0, 1f);
                                break;
                            case 3:
                                Gizmos.color = new Color(1f, 1f, 1f, 1f);
                                break;
                            default:
                                Gizmos.color = new Color(0, 0, 1f, 1f);
                                break;
                        }
                        Gizmos.DrawWireCube(_vegetationCellRuntimePrefabList[i].VegetationCell.CellBounds.center, _vegetationCellRuntimePrefabList[i].VegetationCell.CellBounds.size);
                    }
                }
            }
        }
    }
}
