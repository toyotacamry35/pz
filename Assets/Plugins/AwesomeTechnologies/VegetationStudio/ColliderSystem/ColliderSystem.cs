using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using AwesomeTechnologies.Utility;
using AwesomeTechnologies.Vegetation;
using UnityEngine.AI;

namespace AwesomeTechnologies.Colliders
{
    [Serializable]
    public enum ColliderSystemType
    {
        Disabled,
        Visible
    }

    public enum ColliderRange
    {
        Normal = 0,
        Long = 1,
        VeryLong = 2
    }

    public class VegetationCellCollider
    {
        public VegetationCell VegetationCell;
        public List<GameObject> ColliderList = new List<GameObject>();
        public int ColliderCount;
    }

    [HelpURL("http://www.awesometech.no/index.php/home/vegetation-studio/components/collider-system")]
    [AwesomeTechnologiesScriptOrder(105)]
    [ExecuteInEditMode]
    public class ColliderSystem : MonoBehaviour
    {
        //TUDO add Area selection system. Possible support for multiple transforms

        public ColliderSystemType ColliderSystemType = ColliderSystemType.Disabled;
        public ColliderRange ColliderRange = ColliderRange.Normal;
        public VegetationSystem VegetationSystem;
        public bool ShowCellGrid;
        public bool HideColliders;

        public LayerMask TreeLayer = 0;
        public LayerMask ObjectLayer = 0;
        public LayerMask LargeObjectLayer = 0;

        [NonSerialized]
        private List<VegetationCellCollider> _vegetationCellColliderList = new List<VegetationCellCollider>();

        public GameObject ColliderParent;


        public delegate void MultiCreateColliderDelegate(Collider collider);
        public MultiCreateColliderDelegate OnCreateColliderDelegate;

        public delegate void MultiBeforeDestroyColliderDelegate(Collider collider);
        public MultiBeforeDestroyColliderDelegate OnBeforeDestroyColliderDelegate;

        public int CellCount
        {
            get
            {
                if (_vegetationCellColliderList != null)
                {
                    return _vegetationCellColliderList.Count;
                }
                return 0;
            }
        }

        public int ColliderCount
        {
            get
            {
                return GetColliderCount();
            }
        }

        private int GetColliderCount()
        {
            int count = 0;
            for (int i = 0; i <= _vegetationCellColliderList.Count - 1; i++)
            {
                count += _vegetationCellColliderList[i].ColliderCount;
            }

            return count;
        }
        public void RefreshColliders()
        {
            ClearVegetationCellColliderList();
            for (int i = 0; i <= VegetationSystem.VisibleVegetationCellList.Count - 1; i++)
            {
                if (VegetationSystem.VisibleVegetationCellList[i].DistanceBand <= (int)ColliderRange)
                {
                    AddVegetationCellCollider(VegetationSystem.VisibleVegetationCellList[i]);
                }
            }

            if (ColliderParent && HideColliders)
            {
                ColliderParent.hideFlags = HideFlags.HideInHierarchy;
            }
            else
            {
                ColliderParent.hideFlags = HideFlags.None;
            }
        }

        // ReSharper disable once UnusedMember.Local
        void OnEnable()
        {
            VegetationSystem = gameObject.GetComponent<VegetationSystem>();
            if (VegetationSystem)
            {
                VegetationSystem.OnVegetationCellVisibleDelegate += OnVegetationCellVisible;
                VegetationSystem.OnVegetationCellInvisibleDelegate += OnVegetationInvisible;
                VegetationSystem.OnResetVisibleCellListDelegate += OnResetVisibleCellList;
                VegetationSystem.OnVegetationCellChangeDistanceBandDelegate += OnVegetationCellChangeDistanceBand;
                VegetationSystem.OnSetVegetationPackageDelegate += OnSetVegetationPackage;
            }

            _vegetationCellColliderList = new List<VegetationCellCollider>();

            if (!ColliderParent)
            {
                CreateColliderParent();
            }

            //RefreshColliders();
        }

        private void OnSetVegetationPackage(VegetationPackage vegetationPackage)
        {
            //RefreshColliders();
        }
        void CreateColliderParent()
        {
            List<GameObject> childList = new List<GameObject>();
            foreach (Transform child in transform)
            {
                if (child.transform.name == "Colliders")
                {
                    childList.Add(child.gameObject);
                }
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

            ColliderParent = new GameObject("Colliders");
            ColliderParent.transform.SetParent(transform, false);
        }

        private void DeleteOldColliders()
        {
            List<GameObject> childList = new List<GameObject>();
            foreach (Transform child in ColliderParent.transform)
            {
                childList.Add(child.gameObject);
            }

            for (int i = 0; i <= childList.Count - 1; i++)
            {
                DestroyImmediate(childList[i]);
            }
        }

        private void OnResetVisibleCellList()
        {
            ClearVegetationCellColliderList();
        }

        void OnVegetationCellVisible(VegetationCell vegetationCell, int distanceBand)
        {
            if (distanceBand <= (int)ColliderRange)
            {

                AddVegetationCellCollider(vegetationCell);
            }

#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }

        private void OnVegetationInvisible(VegetationCell vegetationCell, int distanceBand)
        {
            RemoveVegetationCellCollider(vegetationCell);
            //if (vegetationCell.IsVisible) Debug.Log("strange");

#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }

        private void OnVegetationCellChangeDistanceBand(VegetationCell vegetationCell, int distanceBand, int previousDistanceBand)
        {
            if (!vegetationCell.IsVisible)
            {
                RemoveVegetationCellCollider(vegetationCell);
                return;
            }

            if ((distanceBand <= (int)ColliderRange) && (previousDistanceBand > (int)ColliderRange))
            {
                AddVegetationCellCollider(vegetationCell);
            }
            else if (distanceBand > (int)ColliderRange)
            {
                RemoveVegetationCellCollider(vegetationCell);
            }
            else
            {

#if UNITY_EDITOR
                EditorUtility.SetDirty(this);
#endif
            }
        }
        private void RemoveVegetationCellCollider(VegetationCell vegetationCell)
        {
            for (int i = 0; i <= _vegetationCellColliderList.Count - 1; i++)
            {
                if (_vegetationCellColliderList[i].VegetationCell != vegetationCell) continue;

                for (int j = 0; j <= _vegetationCellColliderList[i].ColliderList.Count - 1; j++)
                {
                    if (OnBeforeDestroyColliderDelegate != null)
                    {
                        OnBeforeDestroyColliderDelegate(_vegetationCellColliderList[i].ColliderList[j].GetComponent<Collider>());
                    }

                    if (Application.isPlaying)
                    {
                        Destroy(_vegetationCellColliderList[i].ColliderList[j]);
                    }
                    else
                    {
                        DestroyImmediate(_vegetationCellColliderList[i].ColliderList[j]);
                    }

                }
                _vegetationCellColliderList.RemoveAt(i);
                break;
            }

            vegetationCell.OnSpawnVegetationDelegate -= OnSpawnVegetation;
        }

        public GameObject BakeNavmeshColliders(bool createMesh)
        {
            if (!VegetationSystem) return null;

            GameObject navMeshColliders = new GameObject { name = "Navmesh Colliders" };
            for (int i = 0; i <= VegetationSystem.VegetationCellList.Count - 1; i++)
            {
#if UNITY_EDITOR
                if (i % 100 == 0)
                {
                    float progress = (float)i / VegetationSystem.VegetationCellList.Count;
                    EditorUtility.DisplayProgressBar("Bake navmesh", "Spawn all colliders", progress);
                }
#endif
                AddVegetationCellCollider(VegetationSystem.VegetationCellList[i], navMeshColliders, true);
            }

            SetStatic(navMeshColliders);
            if (createMesh)
            {
#if UNITY_EDITOR
                EditorUtility.DisplayProgressBar("Bake navmesh", "Convert to meshes", 0.5f);
#endif
                CreateNavMeshColliderMeshes(navMeshColliders);
            }

#if UNITY_EDITOR
            EditorUtility.ClearProgressBar();
#endif

            return navMeshColliders;
        }


        private static void CreateNavMeshColliderMeshes(GameObject go)
        {
            Material colliderMaterial = new Material(Shader.Find("Standard"));
            colliderMaterial.SetColor("_Color", Color.gray);

            Collider[] colliders = go.GetComponentsInChildren<Collider>();
            for (int i = 0; i <= colliders.Length - 1; i++)
            {
                var collider1 = colliders[i] as CapsuleCollider;
                if (collider1 != null)
                {
                    CapsuleCollider capsuleCollider = collider1;
                    MeshFilter meshFilter = capsuleCollider.gameObject.AddComponent<MeshFilter>();
                    meshFilter.sharedMesh =
                        MeshUtils.CreateCapsuleMesh(capsuleCollider.radius, capsuleCollider.height);
                    MeshRenderer meshrenderer = capsuleCollider.gameObject.AddComponent<MeshRenderer>();
                    meshrenderer.sharedMaterial = colliderMaterial;
                    DestroyImmediate(capsuleCollider);
                }

                var meshCollider1 = colliders[i] as MeshCollider;
                if (meshCollider1 != null)
                {
                    MeshCollider meshCollider = meshCollider1;
                    MeshFilter meshFilter = meshCollider.gameObject.AddComponent<MeshFilter>();
                    meshFilter.sharedMesh = meshCollider.sharedMesh;
                    MeshRenderer meshrenderer = meshCollider.gameObject.AddComponent<MeshRenderer>();
                    meshrenderer.sharedMaterial = colliderMaterial;
                    DestroyImmediate(meshCollider);
                }

                var boxCollider1 = colliders[i] as BoxCollider;
                if (boxCollider1 != null)
                {
                    BoxCollider boxCollider = boxCollider1;
                    MeshFilter meshFilter = boxCollider.gameObject.AddComponent<MeshFilter>();
                    meshFilter.sharedMesh = MeshUtils.CreateBoxMesh(boxCollider.size.z, boxCollider.size.x, boxCollider.size.y);
                    MeshRenderer meshrenderer = boxCollider.gameObject.AddComponent<MeshRenderer>();
                    meshrenderer.sharedMaterial = colliderMaterial;
                    DestroyImmediate(boxCollider);
                }

                var sphereCollider1 = colliders[i] as SphereCollider;
                if (sphereCollider1 != null)
                {
                    SphereCollider sphereCollider = sphereCollider1;
                    MeshFilter meshFilter = sphereCollider.gameObject.AddComponent<MeshFilter>();
                    meshFilter.sharedMesh = MeshUtils.CreateSphereMesh(sphereCollider.radius);
                    MeshRenderer meshrenderer = sphereCollider.gameObject.AddComponent<MeshRenderer>();
                    meshrenderer.sharedMaterial = colliderMaterial;
                    DestroyImmediate(sphereCollider);
                }
            }
        }

        private static void SetStatic(GameObject go)
        {

#if UNITY_EDITOR
            Collider[] colliders = go.GetComponentsInChildren<Collider>();
            for (int i = 0; i <= colliders.Length - 1; i++)
            {
                colliders[i].gameObject.isStatic = true;
            }
#endif
        }

        private void AddVegetationCellCollider(VegetationCell vegetationCell, GameObject parentGameObject = null,
            bool bakeNavmesh = false)
        {
            vegetationCell.OnSpawnVegetationDelegate += OnSpawnVegetation;

            GameObject currentParent = ColliderParent;
            if (bakeNavmesh && parentGameObject != null)
            {
                currentParent = parentGameObject;
            }

            VegetationCellCollider vegetationCellCollider =
                new VegetationCellCollider { VegetationCell = vegetationCell };

            VegetationPackage vegetationPackage = VegetationSystem.currentVegetationPackage;

            int colliderCount = 0;
            if (vegetationCell.InitDone)
            {
                for (int i = 0; i <= vegetationPackage.VegetationInfoList.Count - 1; i++)
                {
                    VegetationItemInfo vegetationItemInfo = vegetationPackage.VegetationInfoList[i];
                    if (bakeNavmesh && !vegetationItemInfo.ColliderUseForBake) continue;

                    if (vegetationPackage.VegetationInfoList[i].ColliderType != ColliderType.Disabled)
                    {
                        if (vegetationPackage.VegetationInfoList[i].VegetationType == VegetationType.Tree ||
                        vegetationPackage.VegetationInfoList[i].VegetationType == VegetationType.Objects ||
                        vegetationPackage.VegetationInfoList[i].VegetationType == VegetationType.LargeObjects)
                        {
                            if (vegetationItemInfo.VegetationRenderType == VegetationRenderType.InstancedIndirect && Application.isPlaying)
                            {
                                CustomList<Matrix4x4> currentVegetationList;
                                if (bakeNavmesh)
                                {
                                    currentVegetationList = vegetationCell.DirectSpawnVegetationIndirect(vegetationItemInfo.VegetationItemID, true);
                                }
                                else
                                {
                                    currentVegetationList = vegetationCell.GetCurrentIndirectVegetationList(i);                                    
                                }

                                if (currentVegetationList != null)
                                {
                                    colliderCount += currentVegetationList.Count;

                                    for (int j = 0; j <= currentVegetationList.Count - 1; j++)
                                    {
                                        Matrix4x4 colliderMatrix = currentVegetationList[j];
                                        CreateCollider(colliderMatrix, vegetationPackage, i, j, bakeNavmesh, vegetationCell,
                                            currentParent, vegetationItemInfo, vegetationCellCollider);
                                    }
                                }
                            }
                            else
                            {
                                List<Matrix4x4> currentVegetationList;
                                if (bakeNavmesh)
                                {
                                    currentVegetationList = vegetationCell.DirectSpawnVegetation(vegetationItemInfo.VegetationItemID, true);
                                }
                                else
                                {
                                    currentVegetationList = vegetationCell.GetCurrentVegetationList(i);
                                }

                                if (currentVegetationList != null)
                                {
                                    colliderCount += currentVegetationList.Count;

                                    for (int j = 0; j <= currentVegetationList.Count - 1; j++)
                                    {
                                        Matrix4x4 colliderMatrix = currentVegetationList[j];
                                        CreateCollider(colliderMatrix, vegetationPackage, i, j, bakeNavmesh, vegetationCell,
                                            currentParent, vegetationItemInfo, vegetationCellCollider);
                                    }
                                }
                            }                           
                        }
                    }
                }
            }
            if (!bakeNavmesh)
            {
                vegetationCellCollider.ColliderCount = colliderCount;
                _vegetationCellColliderList.Add(vegetationCellCollider);
            }
        }

        void CreateCollider(Matrix4x4 colliderMatrix, VegetationPackage vegetationPackage, int i,int j, bool bakeNavmesh, VegetationCell vegetationCell, GameObject currentParent, VegetationItemInfo vegetationItemInfo, VegetationCellCollider vegetationCellCollider)
        {
            RuntimeObjectInfo runtimeObjectInfo;
            Vector3 vegetationItemScale;

            //TUDO add pool of colliders
            switch (vegetationPackage.VegetationInfoList[i].ColliderType)
            {
                case ColliderType.Capsule:
                    GameObject capsuleColliderObject = new GameObject(vegetationCell.CellIndex + " " + vegetationPackage.VegetationInfoList[i].VegetationType + " CapsuleCollider_" + j);

                    if (!bakeNavmesh) capsuleColliderObject.hideFlags = HideColliders ? HideFlags.HideAndDontSave : HideFlags.DontSave;
                    capsuleColliderObject.layer = GetColliderLayer(vegetationPackage.VegetationInfoList[i].VegetationType);

                    vegetationItemScale = MatrixTools.ExtractScaleFromMatrix(colliderMatrix);

                    CapsuleCollider capsuleCollider = capsuleColliderObject.AddComponent<CapsuleCollider>();
                    capsuleCollider.height = vegetationPackage.VegetationInfoList[i].ColliderHeight;
                    capsuleCollider.radius = vegetationPackage.VegetationInfoList[i].ColliderRadius;
                    capsuleCollider.isTrigger =
                        vegetationPackage.VegetationInfoList[i].ColliderTrigger;
                    capsuleColliderObject.transform.SetParent(currentParent.transform, false);
                    capsuleColliderObject.transform.position = MatrixTools.ExtractTranslationFromMatrix(colliderMatrix) + new Vector3(0, (capsuleCollider.height * vegetationItemScale.y) / 2f, 0);
                    capsuleColliderObject.transform.rotation = MatrixTools.ExtractRotationFromMatrix(colliderMatrix);
                    capsuleColliderObject.transform.localScale = vegetationItemScale;


                    Vector3 colliderOffset =
                        new Vector3(
                            capsuleColliderObject.transform.localScale.x *
                            vegetationPackage.VegetationInfoList[i].ColliderOffset.x,
                            capsuleColliderObject.transform.localScale.y *
                            vegetationPackage.VegetationInfoList[i].ColliderOffset.y,
                            capsuleColliderObject.transform.localScale.z *
                            vegetationPackage.VegetationInfoList[i].ColliderOffset.z);
                    capsuleColliderObject.transform.localPosition += MatrixTools.ExtractRotationFromMatrix(colliderMatrix) * colliderOffset;

                    if (!bakeNavmesh)
                    {
                        AddNavMesObstacle(capsuleColliderObject, vegetationItemInfo);
                        AddVegetationItemInstanceInfo(capsuleColliderObject, colliderMatrix,
                            vegetationItemInfo.VegetationType,
                            vegetationItemInfo.VegetationItemID);
                        vegetationCellCollider.ColliderList.Add(capsuleColliderObject);
                        runtimeObjectInfo =
                            capsuleColliderObject.AddComponent<RuntimeObjectInfo>();
                        runtimeObjectInfo.VegetationItemInfo =
                            vegetationPackage.VegetationInfoList[i];
                        if (OnCreateColliderDelegate != null)
                            OnCreateColliderDelegate(capsuleCollider);
                    }
                    break;
                case ColliderType.Sphere:
                    GameObject sphereColliderObject = new GameObject(vegetationCell.CellIndex.ToString() + " " + vegetationPackage.VegetationInfoList[i].VegetationType.ToString() + " SphereCollider_" + j.ToString());
                    if (!bakeNavmesh) sphereColliderObject.hideFlags = HideColliders ? HideFlags.HideAndDontSave : HideFlags.DontSave;

                    sphereColliderObject.layer = GetColliderLayer(vegetationPackage.VegetationInfoList[i].VegetationType);

                    vegetationItemScale = MatrixTools.ExtractScaleFromMatrix(colliderMatrix);

                    SphereCollider sphereCollider = sphereColliderObject.AddComponent<SphereCollider>();
                    sphereCollider.radius = vegetationPackage.VegetationInfoList[i].ColliderRadius;
                    sphereCollider.isTrigger =
                        vegetationPackage.VegetationInfoList[i].ColliderTrigger;
                    sphereColliderObject.transform.SetParent(currentParent.transform, false);
                    sphereColliderObject.transform.position =
                        MatrixTools.ExtractTranslationFromMatrix(colliderMatrix);// + vegetationPackage.VegetationInfoList[i].ColliderOffset;
                    sphereColliderObject.transform.rotation = MatrixTools.ExtractRotationFromMatrix(colliderMatrix);
                    sphereColliderObject.transform.localScale = vegetationItemScale;

                    Vector3 sphereColliderOffset =
                        new Vector3(
                            sphereColliderObject.transform.localScale.x *
                            vegetationPackage.VegetationInfoList[i].ColliderOffset.x,
                            sphereColliderObject.transform.localScale.y *
                            vegetationPackage.VegetationInfoList[i].ColliderOffset.y,
                            sphereColliderObject.transform.localScale.z *
                            vegetationPackage.VegetationInfoList[i].ColliderOffset.z);
                    sphereColliderObject.transform.localPosition += MatrixTools.ExtractRotationFromMatrix(colliderMatrix) * sphereColliderOffset;

                    if (!bakeNavmesh)
                    {
                        AddNavMesObstacle(sphereColliderObject, vegetationItemInfo);
                        AddVegetationItemInstanceInfo(sphereColliderObject, colliderMatrix,
                            vegetationItemInfo.VegetationType,
                            vegetationItemInfo.VegetationItemID);
                        vegetationCellCollider.ColliderList.Add(sphereColliderObject);
                        runtimeObjectInfo =
                            sphereColliderObject.AddComponent<RuntimeObjectInfo>();
                        runtimeObjectInfo.VegetationItemInfo =
                            vegetationPackage.VegetationInfoList[i];
                        if (OnCreateColliderDelegate != null)
                            OnCreateColliderDelegate(sphereCollider);
                    }

                    break;
                case ColliderType.CustomMesh:
                case ColliderType.Mesh:
                    GameObject meshColliderObject = new GameObject(vegetationCell.CellIndex.ToString() + " " + vegetationPackage.VegetationInfoList[i].VegetationType.ToString() + " MeshCollider_" + j.ToString());
                    if (!bakeNavmesh) meshColliderObject.hideFlags = HideColliders ? HideFlags.HideAndDontSave : HideFlags.DontSave;
                    meshColliderObject.layer = GetColliderLayer(vegetationPackage.VegetationInfoList[i].VegetationType);
                    MeshCollider meshCollider = meshColliderObject.AddComponent<MeshCollider>();
                    VegetationItemModelInfo vegetationItemModelInfo = VegetationSystem.GetVegetationModelInfo(i);
                    meshCollider.sharedMesh = vegetationItemInfo.ColliderType == ColliderType.CustomMesh ? vegetationItemInfo.ColliderMesh : vegetationItemModelInfo.VegetationItemInfo.sourceMesh;

                    meshColliderObject.transform.SetParent(currentParent.transform, false);
                    meshColliderObject.transform.position =
                    MatrixTools.ExtractTranslationFromMatrix(colliderMatrix);// + vegetationPackage.VegetationInfoList[i].ColliderOffset;
                    meshColliderObject.transform.rotation = MatrixTools.ExtractRotationFromMatrix(colliderMatrix);
                    meshColliderObject.transform.localScale = MatrixTools.ExtractScaleFromMatrix(colliderMatrix);

                    if (!bakeNavmesh)
                    {
                        AddNavMesObstacle(meshColliderObject, vegetationItemInfo);
                        AddVegetationItemInstanceInfo(meshColliderObject, colliderMatrix,
                            vegetationItemInfo.VegetationType,
                            vegetationItemInfo.VegetationItemID);
                        vegetationCellCollider.ColliderList.Add(meshColliderObject);
                        runtimeObjectInfo = meshColliderObject.AddComponent<RuntimeObjectInfo>();
                        runtimeObjectInfo.VegetationItemInfo = vegetationPackage.VegetationInfoList[i];
                        if (OnCreateColliderDelegate != null) OnCreateColliderDelegate(meshCollider);

                    }
                    break;
            }
        }

        void AddNavMesObstacle(GameObject go, VegetationItemInfo vegetationItemInfo)
        {
            NavMeshObstacle navMeshObstacle;

            switch (vegetationItemInfo.NavMeshObstacleType)
            {
                case NavMeshObstacleType.Box:
                     navMeshObstacle = go.AddComponent<NavMeshObstacle>();
                    navMeshObstacle.shape = NavMeshObstacleShape.Box;
                    navMeshObstacle.center = vegetationItemInfo.NavMeshObstacleCenter;
                    navMeshObstacle.size = vegetationItemInfo.NavMeshObstacleSize;
                    navMeshObstacle.carving = vegetationItemInfo.NavMeshObstacleCarve;
                    break;
                case NavMeshObstacleType.Capsule:
                     navMeshObstacle = go.AddComponent<NavMeshObstacle>();
                    navMeshObstacle.shape = NavMeshObstacleShape.Capsule;
                    navMeshObstacle.center = vegetationItemInfo.NavMeshObstacleCenter;
                    navMeshObstacle.radius = vegetationItemInfo.NavMeshObstacleRadius;
                    navMeshObstacle.height = vegetationItemInfo.NavMeshObstacleHeight;
                    navMeshObstacle.carving = vegetationItemInfo.NavMeshObstacleCarve;
                    break;
            }
        }

        void AddVegetationItemInstanceInfo(GameObject go, Matrix4x4 positionMatrix, VegetationType vegetationType, string vegetationItemID)
        {
           var vegetationItemInstanceInfo = go.AddComponent<VegetationItemInstanceInfo>();
           vegetationItemInstanceInfo.Position = MatrixTools.ExtractTranslationFromMatrix(positionMatrix);
           vegetationItemInstanceInfo.VegetationItemInstanceID = Mathf.RoundToInt(vegetationItemInstanceInfo.Position.x *100f).ToString() + "_" +
                                                                 Mathf.RoundToInt(vegetationItemInstanceInfo.Position.y * 100f).ToString() + "_" +
                                                                 Mathf.RoundToInt(vegetationItemInstanceInfo.Position.z * 100f).ToString();
           vegetationItemInstanceInfo.Rotation = MatrixTools.ExtractRotationFromMatrix(positionMatrix);
           vegetationItemInstanceInfo.Scale = MatrixTools.ExtractScaleFromMatrix(positionMatrix);
           vegetationItemInstanceInfo.VegetationType = vegetationType;
           vegetationItemInstanceInfo.VegetationItemID = vegetationItemID;
        }
        private LayerMask GetColliderLayer(VegetationType vegetationType)
        {
            switch (vegetationType)
            {
                case VegetationType.Tree:
                    return TreeLayer;
                case VegetationType.Objects:
                    return ObjectLayer;
                case VegetationType.LargeObjects:
                    return LargeObjectLayer;
                default:
                    return 0;
            }
        }

        private void OnSpawnVegetation(VegetationCell vegetationCell)
        {
            RemoveVegetationCellCollider(vegetationCell);
            AddVegetationCellCollider(vegetationCell);
        }

        private void ClearVegetationCellColliderList()
        {

            for (int i = 0; i <= _vegetationCellColliderList.Count - 1; i++)
            {
                for (int j = 0; j <= _vegetationCellColliderList[i].ColliderList.Count - 1; j++)
                {
                    if (OnBeforeDestroyColliderDelegate != null)
                    {
                        OnBeforeDestroyColliderDelegate(_vegetationCellColliderList[i].ColliderList[j].GetComponent<Collider>());
                    }

                    if (Application.isPlaying)
                    {
                        Destroy(_vegetationCellColliderList[i].ColliderList[j]);
                    }
                    else
                    {
                        DestroyImmediate(_vegetationCellColliderList[i].ColliderList[j]);
                    }
                }
            }
            DeleteOldColliders();
            _vegetationCellColliderList.Clear();
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

            ClearVegetationCellColliderList();
        }

        // ReSharper disable once UnusedMember.Local
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            if (_vegetationCellColliderList == null) return;
            for (int i = 0; i <= _vegetationCellColliderList.Count - 1; i++)
            {
                if (!_vegetationCellColliderList[i].VegetationCell.InitDone || !ShowCellGrid) continue;

                switch (_vegetationCellColliderList[i].VegetationCell.DistanceBand)
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
                Gizmos.DrawWireCube(_vegetationCellColliderList[i].VegetationCell.CellBounds.center, _vegetationCellColliderList[i].VegetationCell.CellBounds.size);
            }
        }
    }
}
