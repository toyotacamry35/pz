using Assets.Src.Instancenator;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Assets.Instancenator
{
    [ExecuteInEditMode]
    [AddComponentMenu("Instancenator/Instance Renderer")]
    public class InstanceCompositionRenderer : MonoBehaviour
    {
        protected const int unityArgsSize = 5;
        protected const int maxArgsPerBuffer = (2048 / 4) / unityArgsSize;
        public InstanceComposition composition;
        public bool enableCastShadows = true;
        public bool isEnableDrawInEditor = false;
        public bool enableComputeShader = true;
        public bool isEnableDrawBlocksBound = false;
        public bool isEnableDynamicUpdate = false;

        [Range(0f, 1f)]
        public float thinningCoeff = 0.3f;
        public ComputeShader computeShader; // set from InstanceCompositionScenePostProcessor and from OnValidate

        protected struct BlockRenderData
        {
            public int argOffset;
            public int argsBufferIndex;
            public MaterialPropertyBlock propertyBlock;
            public Bounds bounds;
            public float maxDistanceSqr;
        }

        protected const int blockComputeDataSize = 64;
        protected struct BlockComputeData
        {
            public Vector4UInt LodInstanceCount;
            public Vector4 LodDistance;
            public Vector3 center;
            public int instanceArgsIndex;
            public Vector3 extents;
            public int lodCount;
        }

        protected int computeKernelID;
        protected ComputeBuffer instances;
        protected List<ComputeBuffer> argsBuffers;
        protected List<ComputeBuffer> computeArgsBuffers;
        protected BlockRenderData[] blocksRenderData;
        protected bool isStatic;
        protected Bounds rootBounds;
        protected float rootMaxDistanceSqr = 0.0f;

        private static Plane[] cameraFrustumPlanes;
        private static Vector4[] cameraFrustumPlanesVector;
        private static Vector3[] cameraFrustumCornersTemp;
        private static Vector4[] allCameraFrustumCorners;
        private static Vector3 cameraPosition;
        private static bool positionValid = false;
        private static bool positionQueried = false;
        private static bool cameraPresent = false;

        private void Awake()
        {
            var headless = SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null;
            if (headless)
                Destroy(this);
        }

        private void OnEnable()
        {
            Init();
            InstanceCompositionDistanceManager.GrassDistanceChangedEvent += HandleDistanceBiasChange;
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                if (!isEnableDrawInEditor)
                {
                    return;
                }
            }
#endif

            if (!positionQueried)
            {
                cameraPresent = GetCamera(out Camera camera);
                positionValid = GetCameraPosition(out cameraPosition, in camera);
                if (cameraPresent && positionValid)
                {
                    GetCullingPlanesAndFrustumPoints(camera);
                }
                positionQueried = true;
            }
            if (positionValid)
                Render(cameraPosition, cameraPresent);
        }

        private void LateUpdate()
        {
            positionQueried = false;
        }

        private void OnDisable()
        {
            blocksRenderData = null;
            ReleaseInstancesBuffer();
            ReleaseArgsBuffers();
            InstanceCompositionDistanceManager.GrassDistanceChangedEvent -= HandleDistanceBiasChange;
        }

        private void HandleDistanceBiasChange(float bias)
        {
            if (computeShader != default)
                computeShader.SetFloat(InstanceComposition.computeLodBiasID, bias);
        }

        private void ReleaseInstancesBuffer()
        {
            if (instances != null)
            {
                instances.Release();
                instances = null;
            }
        }

        private void ReleaseArgsBuffers()
        {
            ReleaseBufferList(argsBuffers);
            ReleaseBufferList(computeArgsBuffers);
        }

        private void ReleaseBufferList(List<ComputeBuffer> bufferList)
        {
            if (bufferList != null)
            {
                for (int i = 0; i < bufferList.Count; i++)
                {
                    bufferList[i].Release();
                    bufferList[i] = null;
                }
                bufferList.Clear();
            }
        }

        public void OnChangeComposition()
        {
            Init();
        }

        private void InitCameraRelatedFields()
        {
            if (cameraFrustumPlanes == default)
                cameraFrustumPlanes = new Plane[6];
            if (cameraFrustumCornersTemp == default)
                cameraFrustumCornersTemp = new Vector3[4];
            if (allCameraFrustumCorners == default)
                allCameraFrustumCorners = new Vector4[8];
            if (cameraFrustumPlanesVector == default)
                cameraFrustumPlanesVector = new Vector4[6];
        }

        protected void Init()
        {
            if (composition == null || composition.blocks == null || composition.blocks.Length == 0 ||
                composition.instances == null || composition.instances.Length == 0)
            {
                return;
            }
            isStatic = !isEnableDynamicUpdate || gameObject.isStatic;
            if (transform.lossyScale != Vector3.one)
            {
                Debug.Log("Instancenator now not work correctly with scaled objects!");
            }
            InstanceComposition.InitShaderIDs();
            InitComputeShaderIDs();

            InitCameraRelatedFields();

            if (blocksRenderData == null || blocksRenderData.Length != composition.blocks.Length)
            {
                blocksRenderData = new BlockRenderData[composition.blocks.Length];
            }
            if (instances != null && instances.count != composition.instances.Length)
            {
                instances.Release();
                instances = null;
            }
            if (instances == null)
            {
                instances = new ComputeBuffer(composition.instances.Length, InstanceComposition.InstanceData.structSize);
            }
            instances.SetData(composition.instances);


            int allLodsCount = 0;
            for (int i = 0; i < composition.blocks.Length; i++)
            {
                allLodsCount += composition.blocks[i].lods.Length;
            }
            if (argsBuffers == null)
            {
                int argBuffersCountMax = (allLodsCount + maxArgsPerBuffer - 1) / maxArgsPerBuffer;
                argsBuffers = new List<ComputeBuffer>(argBuffersCountMax);
            }
            if (computeArgsBuffers == null)
            {
                int computeBuffersCountMax = (allLodsCount + maxArgsPerBuffer - 1) / maxArgsPerBuffer;
                computeArgsBuffers = new List<ComputeBuffer>(computeBuffersCountMax);
            }
            List<BlockComputeData> computeSourceArgBuffer = new List<BlockComputeData>(maxArgsPerBuffer);
            List<uint> sourceArgBuffer = new List<uint>(maxArgsPerBuffer * unityArgsSize);
            int argsBufferIndex = 0;

            rootMaxDistanceSqr = 0.0f;
            for (int i = 0; i < blocksRenderData.Length; i++)
            {
                //Bounds with transform
                bool isNotAxisAligned = Helpers.IsAxisAligned(transform);
                if (isNotAxisAligned || !isStatic)
                {
                    blocksRenderData[i].bounds = Helpers.CalcOrientedBoxBounds(transform.localToWorldMatrix, composition.blocks[i].bounds);
                }
                else
                {
                    blocksRenderData[i].bounds = composition.blocks[i].bounds;
                    blocksRenderData[i].bounds.center += transform.position;
                }
                //Max distance
                float maxDistance = composition.blocks[i].lods[composition.blocks[i].lods.Length - 1].maxDistance;
                blocksRenderData[i].maxDistanceSqr = maxDistance * maxDistance;
                rootMaxDistanceSqr = Mathf.Max(rootMaxDistanceSqr, blocksRenderData[i].maxDistanceSqr);
                //Args
                int lodsCount = composition.blocks[i].lods.Length;
                if (sourceArgBuffer.Count + lodsCount * unityArgsSize > maxArgsPerBuffer * unityArgsSize)
                {
                    MoveArgDataToBuffers(argsBufferIndex, sourceArgBuffer, computeSourceArgBuffer);
                    sourceArgBuffer.Clear();
                    computeSourceArgBuffer.Clear();
                    argsBufferIndex++;
                }

                blocksRenderData[i].argOffset = sourceArgBuffer.Count;
                blocksRenderData[i].argsBufferIndex = argsBufferIndex;

                var computeArg = new BlockComputeData();
                computeArg.center = blocksRenderData[i].bounds.center;
                computeArg.extents = blocksRenderData[i].bounds.extents;
                computeArg.instanceArgsIndex = sourceArgBuffer.Count;
                computeArg.lodCount = lodsCount;

                for (int j = 0; j < lodsCount; j++)
                {
                    Mesh instanceMesh = composition.blocks[i].lods[j].instanceMesh;
                    if (instanceMesh != null)
                    {
                        sourceArgBuffer.Add(instanceMesh.GetIndexCount(0));
                        sourceArgBuffer.Add(0);
                        sourceArgBuffer.Add(instanceMesh.GetIndexStart(0));
                        sourceArgBuffer.Add(instanceMesh.GetBaseVertex(0));
                        sourceArgBuffer.Add(0);
                        computeArg.LodInstanceCount[j] = (uint)composition.blocks[i].lods[j].instancesCount;
                        computeArg.LodDistance[j] = composition.blocks[i].lods[j].maxDistance;
                    }
                    else
                    {
                        sourceArgBuffer.Add(0);
                        sourceArgBuffer.Add(0);
                        sourceArgBuffer.Add(0);
                        sourceArgBuffer.Add(0);
                        sourceArgBuffer.Add(0);
                        computeArg.LodInstanceCount[j] = 0;
                    }
                }
                for (int j = lodsCount; j < 4; j++)
                {
                    computeArg.LodDistance[j] = float.MaxValue;
                }

                computeSourceArgBuffer.Add(computeArg);

                //Transition parameters
                Vector4 transitionMul = composition.blocks[i].transitionDistance;
                Vector4 transitionAdd = composition.blocks[i].transitionOffset;
                for (int k = 0; k < 4; k++)
                {
                    transitionMul[k] = transitionMul[k] > 1e-5f ? 1.0f / transitionMul[k] : 0.0f;
                    transitionAdd[k] *= -transitionMul[k];
                }
                //MaterialPropertyBlock
                MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
                propertyBlock.SetBuffer(InstanceComposition.shaderInstanceBufferName, instances);
                propertyBlock.SetInt(InstanceComposition.shaderInstancesOffset, composition.blocks[i].instancesOffset);
                propertyBlock.SetVector(InstanceComposition.shaderBoxSize, composition.blocks[i].bounds.size);
                propertyBlock.SetVector(InstanceComposition.shaderBoxMin, composition.blocks[i].bounds.min);
                propertyBlock.SetMatrix(InstanceComposition.shaderObjectToWorld, transform.localToWorldMatrix);
                propertyBlock.SetVector(InstanceComposition.shaderValueMul, composition.blocks[i].valueMax - composition.blocks[i].valueMin);
                propertyBlock.SetVector(InstanceComposition.shaderValueAdd, composition.blocks[i].valueMin);
                propertyBlock.SetVector(InstanceComposition.shaderTransitionMul, transitionMul);
                propertyBlock.SetVector(InstanceComposition.shaderTransitionAdd, transitionAdd);
                blocksRenderData[i].propertyBlock = propertyBlock;
            }

            MoveArgDataToBuffers(argsBufferIndex, sourceArgBuffer, computeSourceArgBuffer);
            sourceArgBuffer.Clear();

            if (isStatic && blocksRenderData.Length >= 1)
            {
                rootBounds = composition.bounds;
                rootBounds.center += transform.position;
            }
            if (computeShader != default)
            {
                computeShader.SetFloat(InstanceComposition.computeLodBiasID, InstanceCompositionDistanceManager.CurrentDistanceBias);
                computeShader.SetFloat(InstanceComposition.computeThinningCoefficientID, thinningCoeff);
            }
        }

        private void InitComputeShaderIDs()
        {
            if (computeShader != null)
            {
                if (computeShader.HasKernel("CS_Vegetation"))
                    computeKernelID = computeShader.FindKernel("CS_Vegetation");
                else
                    Debug.LogError("No ComputeShader kernel found for InstanceCompositionRenderer");
            }
            else
            {
                Debug.LogError($"No compute shader found");
            }
        }

        private void MoveArgDataToBuffers(int argsBufferIndex, List<uint> sourceArgBuffer, List<BlockComputeData> computeSourceArgsBuffer)
        {
            if (argsBufferIndex < argsBuffers.Count)
            {
                if (argsBuffers[argsBufferIndex] != null && argsBuffers[argsBufferIndex].count != sourceArgBuffer.Count)
                {
                    argsBuffers[argsBufferIndex].Release();
                    argsBuffers[argsBufferIndex] = null;
                }
            }
            else
            {
                argsBuffers.Add(null);
            }
            if (argsBuffers[argsBufferIndex] == null)
            {
                argsBuffers[argsBufferIndex] = new ComputeBuffer(sourceArgBuffer.Count, sizeof(uint), ComputeBufferType.IndirectArguments);
            }
            argsBuffers[argsBufferIndex].SetData(sourceArgBuffer);

            if (argsBufferIndex < computeArgsBuffers.Count)
            {
                if (computeArgsBuffers[argsBufferIndex] != null && computeArgsBuffers[argsBufferIndex].count != computeSourceArgsBuffer.Count)
                {
                    computeArgsBuffers[argsBufferIndex].Release();
                    computeArgsBuffers[argsBufferIndex] = null;
                }
            }
            else
            {
                computeArgsBuffers.Add(null);
            }
            if (computeArgsBuffers[argsBufferIndex] == null)
            {
                computeArgsBuffers[argsBufferIndex] = new ComputeBuffer(computeSourceArgsBuffer.Count, blockComputeDataSize, ComputeBufferType.Structured);
            }
            computeArgsBuffers[argsBufferIndex].SetData(computeSourceArgsBuffer);
        }

        protected void Render(Vector3 cameraPosition, bool cameraPresent)
        {
            if (composition == null || instances == null || blocksRenderData == null)
            {
#if UNITY_EDITOR
                if (composition != null || blocksRenderData == null || instances == null)
                {
                    Init();
                }
#endif
                return;
            }

            if (composition.blocks == null || composition.blocks.Length == 0)
            {
                return;
            }

            if (rootMaxDistanceSqr > 0.1f)
            {
                if (rootBounds.SqrDistance(cameraPosition) >= rootMaxDistanceSqr)
                {
                    return;
                }
            }

            if (enableComputeShader)
            {
                if (computeShader == default)
                {
                    return;
                }
                computeShader.SetVector(InstanceComposition.computeWorldSpaceCameraPositionID, cameraPosition);
                computeShader.SetVectorArray(InstanceComposition.computeFrustumPlanesID, cameraFrustumPlanesVector);
                computeShader.SetVectorArray(InstanceComposition.computeFrustumPointsID, allCameraFrustumCorners);

                for (int i = 0; i < argsBuffers.Count; i++)
                {
                    computeShader.SetBuffer(computeKernelID, InstanceComposition.computeIndirectArgsID, argsBuffers[i]);
                    computeShader.SetBuffer(computeKernelID, InstanceComposition.computeBlockInfoID, computeArgsBuffers[i]);
                    computeShader.Dispatch(computeKernelID, 1, 1, 1);
                }
            }

            var biasSqr = InstanceCompositionDistanceManager.CurrentDistanceBias * InstanceCompositionDistanceManager.CurrentDistanceBias;
            for (int i = 0; i < composition.blocks.Length; i++)
            {
                if (!isStatic)
                {
                    blocksRenderData[i].bounds = Helpers.CalcOrientedBoxBounds(transform.localToWorldMatrix, composition.blocks[i].bounds);
                }

                float distToCameraSqr = blocksRenderData[i].bounds.SqrDistance(cameraPosition);

                if (distToCameraSqr > blocksRenderData[i].maxDistanceSqr * biasSqr)
                {
                    continue;
                }
 
                if (!isStatic)
                {
                    blocksRenderData[i].propertyBlock.SetMatrix(InstanceComposition.shaderObjectToWorld, transform.localToWorldMatrix);
                }

                for (int lodIndex = 0; lodIndex < composition.blocks[i].lods.Length; lodIndex++)
                {
                    UnityEngine.Rendering.ShadowCastingMode castingMode = enableCastShadows && composition.blocks[i].lods[lodIndex].isCastShadows ?
                                                                                UnityEngine.Rendering.ShadowCastingMode.TwoSided : UnityEngine.Rendering.ShadowCastingMode.Off;
                    if (composition.blocks[i].lods[lodIndex].instanceMesh != null)
                    {
                        Graphics.DrawMeshInstancedIndirect(
                                            composition.blocks[i].lods[lodIndex].instanceMesh,
                                            0,
                                            composition.blocks[i].lods[lodIndex].instanceMaterial,
                                            blocksRenderData[i].bounds,
                                            argsBuffers[blocksRenderData[i].argsBufferIndex],
                                            (blocksRenderData[i].argOffset + lodIndex * unityArgsSize) * sizeof(uint),
                                            blocksRenderData[i].propertyBlock,
                                            castingMode);
                    }
                }
            }
        }

            private bool GetCamera(out Camera camera)
        {
            camera = default;
            if (MainCamera.Camera)
            {
                camera = MainCamera.Camera;
                return true;
            }

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                if (UnityEditor.SceneView.lastActiveSceneView != null)
                {
                    if (UnityEditor.SceneView.lastActiveSceneView.camera != null)
                    {
                        camera = UnityEditor.SceneView.lastActiveSceneView.camera;
                        return true;
                    }
                }
            }
#endif
            return false;
        }

        private bool GetCameraPosition(out Vector3 cameraPosition, in Camera camera)
        {
            cameraPosition = Vector3.zero;

            if (camera != default)
            {
                cameraPosition = camera.transform.position;
                return true;
            }
#if UNITY_EDITOR
            else if (!Application.isPlaying && UnityEditor.SceneView.lastActiveSceneView != null)
            {
                cameraPosition = UnityEditor.SceneView.lastActiveSceneView.pivot;
                return true;
            }
#endif
            else
            {
                return false;
            }
        }

        private void GetCullingPlanesAndFrustumPoints(in Camera camera)
        {
            InitCameraRelatedFields();

            // From 'https://docs.unity3d.com/ScriptReference/GeometryUtility.CalculateFrustumPlanes.html':
            // Ordering: [0] = Left, [1] = Right, [2] = Down, [3] = Up, [4] = Near, [5] = Far
            GeometryUtility.CalculateFrustumPlanes(camera, cameraFrustumPlanes);
            for (int i = 0; i < cameraFrustumPlanes.Length; i++)
            {
                var normal = cameraFrustumPlanes[i].normal;
                cameraFrustumPlanesVector[i] = new Vector4(normal.x, normal.y, normal.z, cameraFrustumPlanes[i].distance);
            }

            var viewRect = new Rect(0, 0, 1, 1);

            camera.CalculateFrustumCorners(viewRect, camera.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, cameraFrustumCornersTemp);
            for (int i = 0; i < 4; i++)
                allCameraFrustumCorners[i] = camera.transform.TransformPoint(cameraFrustumCornersTemp[i]);

            camera.CalculateFrustumCorners(viewRect, camera.nearClipPlane, Camera.MonoOrStereoscopicEye.Mono, cameraFrustumCornersTemp);
            for (int i = 0; i < 4; i++)
                allCameraFrustumCorners[i + 4] = camera.transform.TransformPoint(cameraFrustumCornersTemp[i]);
        }

#if UNITY_EDITOR

        private Vector3 lastCameraPosition = Vector3.zero;

        void OnDrawGizmos()
        {
            if (!enabled)
            {
                return;
            }

            if (!positionValid)
                return;

            if (isEnableDrawInEditor)
            {
                const float updateDistance = 1.0f;
                if ((cameraPosition - lastCameraPosition).sqrMagnitude > updateDistance * updateDistance)
                {
                    lastCameraPosition = cameraPosition;
                    UnityEditor.EditorUtility.SetDirty(this);
                }
            }

            if (!isEnableDrawBlocksBound)
            {
                return;
            }

            if (composition == null || blocksRenderData == null)
            {
                Init();
                return;
            }

            if (rootMaxDistanceSqr > 0.1f)
            {

                Gizmos.color = Color.green;

                if (rootBounds.SqrDistance(cameraPosition) >= rootMaxDistanceSqr)
                {
                    Gizmos.color = Color.magenta;
                }

                Gizmos.DrawWireCube(rootBounds.center, rootBounds.size * 1.02f);
            }

            for (int i = 0; i < composition.blocks.Length; i++)
            {
                Gizmos.color = Color.yellow;
                if (UnityEditor.SceneView.lastActiveSceneView != null)
                {
                    float distToCameraSqr = blocksRenderData[i].bounds.SqrDistance(cameraPosition);
                    if (distToCameraSqr > blocksRenderData[i].maxDistanceSqr)
                    {
                        Gizmos.color = Color.red;
                    }
                }

                Gizmos.DrawWireCube(blocksRenderData[i].bounds.center, blocksRenderData[i].bounds.size * 0.99f);
            }

        }

        private void OnValidate()
        {
            string path = "Assets/Src/Instancenator/Shaders/InstancenatorComputeShader.compute";
            if (computeShader == default)
                computeShader = AssetDatabase.LoadAssetAtPath<ComputeShader>(path);
            computeShader.SetFloat(InstanceComposition.computeThinningCoefficientID, thinningCoeff);
            Shader.SetGlobalFloat(InstanceComposition.shaderGlobalLodBiasReciprocal, 1);
        }
#endif
        public struct Vector4UInt
        {
            private uint x;
            private uint y;
            private uint z;
            private uint w;

            public uint this[int index]
            {
                get
                {
                    switch (index)
                    {
                        case 0:
                            return x;
                        case 1:
                            return y;
                        case 2:
                            return z;
                        case 3:
                            return w;
                        default:
                            throw new IndexOutOfRangeException($"Invalid Vector4Int index addressed: {index}!");
                    }
                }
                set
                {
                    switch (index)
                    {
                        case 0:
                            x = value;
                            break;
                        case 1:
                            y = value;
                            break;
                        case 2:
                            z = value;
                            break;
                        case 3:
                            w = value;
                            break;
                        default:
                            throw new IndexOutOfRangeException($"Invalid Vector4Int index addressed: {index}!");
                    }
                }
            }
        }
    }
}