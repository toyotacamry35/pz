using Assets.Src.Lib.ProfileTools;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;

namespace AwesomeTechnologies
{
    [Serializable]
    public enum TextureMaskBand
    {
        RChannel = 1,
        GChannel = 2,
        BChannel = 3,
        AChannel = 4
    }
    public static class GeometryUtilityAllocFree
    {
        public static Plane[] FrustrumPlanes = new Plane[6];

        private static readonly Action<Plane[], Matrix4x4> InternalExtractPlanes =
            (Action<Plane[], Matrix4x4>)Delegate.CreateDelegate(
                typeof(Action<Plane[], Matrix4x4>),
                typeof(GeometryUtility).GetMethod("Internal_ExtractPlanes",
                    BindingFlags.Static | BindingFlags.NonPublic));

        public static void CalculateFrustrumPlanes(Camera camera)
        {
            InternalExtractPlanes(FrustrumPlanes, camera.projectionMatrix * camera.worldToCameraMatrix);
        }
    }

    public partial class VegetationSystem
    {
        public int FrustumKernelHandle;
        public int DistanceKernelHandle;
        public ComputeShader FrusumMatrixShader;
        private int _cameraFrustumPlan0;
        private int _cameraFrustumPlan1;
        private int _cameraFrustumPlan2;
        private int _cameraFrustumPlan3;
        private int _cameraFrustumPlan4;
        private int _cameraFrustumPlan5;


        public int MergeBufferKernelHandle;
        public ComputeShader MergeBufferShader;
        //public ComputeBuffer MergeBuffer;
        private int _mergeBufferID = -1;
        private int _mergeSourceBuffer0ID = -1;
        private int _mergeSourceBuffer1ID = -1;
        private int _mergeSourceBuffer2ID = -1;
        private int _mergeSourceBuffer3ID = -1;
        private int _mergeSourceBuffer4ID = -1;
        private int _mergeSourceBuffer5ID = -1;
        private int _mergeSourceBuffer6ID = -1;
        private int _mergeSourceBuffer7ID = -1;
        private int _mergeSourceBuffer8ID = -1;
        private int _mergeSourceBuffer9ID = -1;
        private int _mergeSourceBuffer10ID = -1;
        private int _mergeSourceBuffer11ID = -1;
        private int _mergeSourceBuffer12ID = -1;
        private int _mergeSourceBuffer13ID = -1;
        private int _mergeSourceBuffer14ID = -1;
        private int _mergeSourceBuffer15ID = -1;

        private int _mergeInstanceCount0ID = -1;
        private int _mergeInstanceCount1ID = -1;
        private int _mergeInstanceCount2ID = -1;
        private int _mergeInstanceCount3ID = -1;
        private int _mergeInstanceCount4ID = -1;
        private int _mergeInstanceCount5ID = -1;
        private int _mergeInstanceCount6ID = -1;
        private int _mergeInstanceCount7ID = -1;
        private int _mergeInstanceCount8ID = -1;
        private int _mergeInstanceCount9ID = -1;
        private int _mergeInstanceCount10ID = -1;
        private int _mergeInstanceCount11ID = -1;
        private int _mergeInstanceCount12ID = -1;
        private int _mergeInstanceCount13ID = -1;
        private int _mergeInstanceCount14ID = -1;
        private int _mergeInstanceCount15ID = -1;

        private int _visibleBufferLod0ID = -1;
        //private int _visibleBufferLod1ID = -1;
        //private int _visibleBufferLod2ID = -1;
        private int _sourceBufferID = -1;
        private int _instanceCountID = -1;
        private int _boundingSphereRadiusID = -1;
        private int _useLodsID = -1;

        private int _lod1Distance = -1;
        private int _lod2Distance = -1;

        public Texture RealTimeMaskTexture;
        public Texture DummyMaskTexture;
        public TextureMaskBand RealTimeMaskBand = TextureMaskBand.RChannel;
        public bool RealTimeMaskEnabled;
        public bool RealTimeMaskInvert;
        public float RealTimeMaskCutoff = 0.5f;
        public bool ManualVegetationRefresh = false;

        private readonly List<VegetationCell> _hasBufferList = new List<VegetationCell>();
        //MaterialPropertyBlock _mergedMaterialPropertyBlock;

        void SetupFrustumComputeShaders()
        {
            DummyMaskTexture = new Texture2D(2, 2);

            if (SystemInfo.supportsComputeShaders)
            {
                FrusumMatrixShader = Profile.Load<ComputeShader>("GPUFrustumCulling");
                FrustumKernelHandle = FrusumMatrixShader.FindKernel("GPUFrustumCulling");
                DistanceKernelHandle = FrusumMatrixShader.FindKernel("DistanceCulling");

                _cameraFrustumPlan0 = Shader.PropertyToID("_VS_CameraFrustumPlane0");
                _cameraFrustumPlan1 = Shader.PropertyToID("_VS_CameraFrustumPlane1");
                _cameraFrustumPlan2 = Shader.PropertyToID("_VS_CameraFrustumPlane2");
                _cameraFrustumPlan3 = Shader.PropertyToID("_VS_CameraFrustumPlane3");
                _cameraFrustumPlan4 = Shader.PropertyToID("_VS_CameraFrustumPlane4");
                _cameraFrustumPlan5 = Shader.PropertyToID("_VS_CameraFrustumPlane5");

                MergeBufferShader = Profile.Load<ComputeShader>("MergeInstancedIndirectBuffers");
                MergeBufferKernelHandle = MergeBufferShader.FindKernel("MergeInstancedIndirectBuffers");

                _mergeBufferID = Shader.PropertyToID("MergeBuffer");

                _mergeSourceBuffer0ID = Shader.PropertyToID("MergeSourceBuffer0");
                _mergeSourceBuffer1ID = Shader.PropertyToID("MergeSourceBuffer1");
                _mergeSourceBuffer2ID = Shader.PropertyToID("MergeSourceBuffer2");
                _mergeSourceBuffer3ID = Shader.PropertyToID("MergeSourceBuffer3");
                _mergeSourceBuffer4ID = Shader.PropertyToID("MergeSourceBuffer4");
                _mergeSourceBuffer5ID = Shader.PropertyToID("MergeSourceBuffer5");
                _mergeSourceBuffer6ID = Shader.PropertyToID("MergeSourceBuffer6");
                _mergeSourceBuffer7ID = Shader.PropertyToID("MergeSourceBuffer7");
                _mergeSourceBuffer8ID = Shader.PropertyToID("MergeSourceBuffer8");
                _mergeSourceBuffer9ID = Shader.PropertyToID("MergeSourceBuffer9");
                _mergeSourceBuffer10ID = Shader.PropertyToID("MergeSourceBuffer10");
                _mergeSourceBuffer11ID = Shader.PropertyToID("MergeSourceBuffer11");
                _mergeSourceBuffer12ID = Shader.PropertyToID("MergeSourceBuffer12");
                _mergeSourceBuffer13ID = Shader.PropertyToID("MergeSourceBuffer13");
                _mergeSourceBuffer14ID = Shader.PropertyToID("MergeSourceBuffer14");
                _mergeSourceBuffer15ID = Shader.PropertyToID("MergeSourceBuffer15");

                _mergeInstanceCount0ID = Shader.PropertyToID("MergeSourceBufferCount0");
                _mergeInstanceCount1ID = Shader.PropertyToID("MergeSourceBufferCount1");
                _mergeInstanceCount2ID = Shader.PropertyToID("MergeSourceBufferCount2");
                _mergeInstanceCount3ID = Shader.PropertyToID("MergeSourceBufferCount3");
                _mergeInstanceCount4ID = Shader.PropertyToID("MergeSourceBufferCount4");
                _mergeInstanceCount5ID = Shader.PropertyToID("MergeSourceBufferCount5");
                _mergeInstanceCount6ID = Shader.PropertyToID("MergeSourceBufferCount6");
                _mergeInstanceCount7ID = Shader.PropertyToID("MergeSourceBufferCount7");
                _mergeInstanceCount8ID = Shader.PropertyToID("MergeSourceBufferCount8");
                _mergeInstanceCount9ID = Shader.PropertyToID("MergeSourceBufferCount9");
                _mergeInstanceCount10ID = Shader.PropertyToID("MergeSourceBufferCount10");
                _mergeInstanceCount11ID = Shader.PropertyToID("MergeSourceBufferCount11");
                _mergeInstanceCount12ID = Shader.PropertyToID("MergeSourceBufferCount12");
                _mergeInstanceCount13ID = Shader.PropertyToID("MergeSourceBufferCount13");
                _mergeInstanceCount14ID = Shader.PropertyToID("MergeSourceBufferCount14");
                _mergeInstanceCount15ID = Shader.PropertyToID("MergeSourceBufferCount15");

                _instanceCountID = Shader.PropertyToID("_InstanceCount");
                _sourceBufferID = Shader.PropertyToID("SourceShaderDataBuffer");
                _visibleBufferLod0ID = Shader.PropertyToID("VisibleBufferLOD0");
                //_visibleBufferLod1ID = Shader.PropertyToID("VisibleBufferLOD1");
                //_visibleBufferLod2ID = Shader.PropertyToID("VisibleBufferLOD2");
                _useLodsID = Shader.PropertyToID("UseLODs");
                _boundingSphereRadiusID = Shader.PropertyToID("_BoundingSphereRadius");

                _lod1Distance = Shader.PropertyToID("_LOD1Distance");
                _lod2Distance = Shader.PropertyToID("_LOD2Distance");
                //_mergedMaterialPropertyBlock = new MaterialPropertyBlock();
            }
        }


        // ReSharper disable once ParameterHidesMember
        private void SetFrustumCullingPlanes(Camera camera)
        {
            GeometryUtilityAllocFree.CalculateFrustrumPlanes(camera);

            Vector4 cameraFrustumPlane0 = new Vector4(GeometryUtilityAllocFree.FrustrumPlanes[0].normal.x, GeometryUtilityAllocFree.FrustrumPlanes[0].normal.y, GeometryUtilityAllocFree.FrustrumPlanes[0].normal.z,
                GeometryUtilityAllocFree.FrustrumPlanes[0].distance);
            Vector4 cameraFrustumPlane1 = new Vector4(GeometryUtilityAllocFree.FrustrumPlanes[1].normal.x, GeometryUtilityAllocFree.FrustrumPlanes[1].normal.y, GeometryUtilityAllocFree.FrustrumPlanes[1].normal.z,
                GeometryUtilityAllocFree.FrustrumPlanes[1].distance);
            Vector4 cameraFrustumPlane2 = new Vector4(GeometryUtilityAllocFree.FrustrumPlanes[2].normal.x, GeometryUtilityAllocFree.FrustrumPlanes[2].normal.y, GeometryUtilityAllocFree.FrustrumPlanes[2].normal.z,
                GeometryUtilityAllocFree.FrustrumPlanes[2].distance);
            Vector4 cameraFrustumPlane3 = new Vector4(GeometryUtilityAllocFree.FrustrumPlanes[3].normal.x, GeometryUtilityAllocFree.FrustrumPlanes[3].normal.y, GeometryUtilityAllocFree.FrustrumPlanes[3].normal.z,
                GeometryUtilityAllocFree.FrustrumPlanes[3].distance);
            Vector4 cameraFrustumPlane4 = new Vector4(GeometryUtilityAllocFree.FrustrumPlanes[4].normal.x, GeometryUtilityAllocFree.FrustrumPlanes[4].normal.y, GeometryUtilityAllocFree.FrustrumPlanes[4].normal.z,
                GeometryUtilityAllocFree.FrustrumPlanes[4].distance);
            Vector4 cameraFrustumPlane5 = new Vector4(GeometryUtilityAllocFree.FrustrumPlanes[5].normal.x, GeometryUtilityAllocFree.FrustrumPlanes[5].normal.y, GeometryUtilityAllocFree.FrustrumPlanes[5].normal.z,
                GeometryUtilityAllocFree.FrustrumPlanes[5].distance);

            FrusumMatrixShader.SetVector(_cameraFrustumPlan0, cameraFrustumPlane0);
            FrusumMatrixShader.SetVector(_cameraFrustumPlan1, cameraFrustumPlane1);
            FrusumMatrixShader.SetVector(_cameraFrustumPlan2, cameraFrustumPlane2);
            FrusumMatrixShader.SetVector(_cameraFrustumPlan3, cameraFrustumPlane3);
            FrusumMatrixShader.SetVector(_cameraFrustumPlan4, cameraFrustumPlane4);
            FrusumMatrixShader.SetVector(_cameraFrustumPlan5, cameraFrustumPlane5);

            FrusumMatrixShader.SetFloat("_CullFarStart", vegetationSettings.VegetationDistance);
            FrusumMatrixShader.SetFloat("_CullFarDistance", 20);

            Vector3 worldSpaceCameraPosition = camera.transform.position;
            Vector4 worldSpaceCameraPos = new Vector4(worldSpaceCameraPosition.x, worldSpaceCameraPosition.y, worldSpaceCameraPosition.z, 1);
            FrusumMatrixShader.SetVector("_WorldSpaceCameraPos", worldSpaceCameraPos);
        }

        void SetRealTimeMaskParameters()
        {
            Profiler.BeginSample("SetRealTimeMaskParameters");

            FrusumMatrixShader.SetBool("_RealTimeMaskEnabled", RealTimeMaskEnabled && RealTimeMaskTexture);
            FrusumMatrixShader.SetBool("_RealTimeMaskInvert", RealTimeMaskInvert);
            Vector4 terrainPosition = new Vector4(UnityTerrainData.terrainPosition.x,
                UnityTerrainData.terrainPosition.y, UnityTerrainData.terrainPosition.z, 1);
            Vector4 terrainSize = new Vector4(UnityTerrainData.size.x,
                UnityTerrainData.size.y, UnityTerrainData.size.z, 1);           
            FrusumMatrixShader.SetVector("_TerrainPosition", terrainPosition);
            FrusumMatrixShader.SetVector("_TerrainSize", terrainSize);
            FrusumMatrixShader.SetFloat("_RealTimeMaskCutoff", RealTimeMaskCutoff);
            FrusumMatrixShader.SetInt("_RealTimeMaskBand", (int) RealTimeMaskBand);

            if (UseGPUCulling)
            {
                if (RealTimeMaskTexture)
                {
                    FrusumMatrixShader.SetTexture(FrustumKernelHandle, "_RealTimeMaskTexture", RealTimeMaskTexture);
                }
                else
                {
                    FrusumMatrixShader.SetTexture(FrustumKernelHandle, "_RealTimeMaskTexture", DummyMaskTexture);

                }
            }
            else
            {
                if (RealTimeMaskTexture)
                {
                    FrusumMatrixShader.SetTexture(DistanceKernelHandle, "_RealTimeMaskTexture", RealTimeMaskTexture);
                }
                else
                {
                    FrusumMatrixShader.SetTexture(FrustumKernelHandle, "_RealTimeMaskTexture", DummyMaskTexture);

                }
            }

            if (RealTimeMaskTexture)
            {
                FrusumMatrixShader.SetInt("_RealTimeMaskWidth", RealTimeMaskTexture.width);
                FrusumMatrixShader.SetInt("_RealTimeMaskHeight", RealTimeMaskTexture.height);
            }
            else
            {
                FrusumMatrixShader.SetInt("_RealTimeMaskWidth", 1024);
                FrusumMatrixShader.SetInt("_RealTimeMaskHeight", 1024);
            }

            Profiler.EndSample();
        }
        void DrawCellsIndirectComputeShader()
        {
#if UNITY_5_6_OR_NEWER
            SetFrustumCullingPlanes(GetCurrentCamera());
            Profiler.BeginSample("Merge Compute Buffers");

            SetRealTimeMaskParameters();
                       
            for (int i = 0; i <= VegetationModelInfoList.Count - 1; i++)
            {
                VegetationItemInfo vegetationItemInfo = VegetationModelInfoList[i].VegetationItemInfo;

                int totalInstanceCount = 0;
                _hasBufferList.Clear();

                for (int j = 0; j <= VisibleVegetationCellList.Count - 1; j++)
                {
                    VegetationCell vegetationCell = VisibleVegetationCellList[j];
                    if (vegetationCell.DistanceBand > 2) continue;
                    if (vegetationCell.IndirectInfoList[i].IndirectShaderDataBuffer == null) continue;
                    if (vegetationCell.IndirectInfoList[i].InstanceCount == 0) continue;
                    _hasBufferList.Add(vegetationCell);
                }

                if (_hasBufferList.Count == 0) continue;

                int buffercount = 16;
                for (int j = 0; j <= _hasBufferList.Count - 1; j++)
                {
                    totalInstanceCount += _hasBufferList[j].IndirectInfoList[i].InstanceCount;
                }

                if (totalInstanceCount > VegetationModelInfoList[i].MergeBuffer.count)
                {
                    VegetationModelInfoList[i].UpdateComputeBufferSize(totalInstanceCount + 5000);
                }

                VegetationModelInfoList[i].MergeBuffer.SetCounterValue(0);
                MergeBufferShader.SetBuffer(MergeBufferKernelHandle, _mergeBufferID, VegetationModelInfoList[i].MergeBuffer);

                for (int j = 0; j <= _hasBufferList.Count - 1; j += buffercount)  
                {
                    int instanceCount0 = _hasBufferList[j].IndirectInfoList[i].InstanceCount;

                    for (int k = 1; k <= buffercount - 1; k++)
                    {
                        if (j + k < _hasBufferList.Count)
                        {
                            int tempInstanceCount = _hasBufferList[j + k].IndirectInfoList[i].InstanceCount;
                            if (tempInstanceCount > instanceCount0) instanceCount0 = tempInstanceCount;
                        }
                    }

                    int threadGroups = Mathf.CeilToInt((float)instanceCount0 / 32f);

                    SetComputeShaderBuffer(_mergeSourceBuffer0ID, _mergeInstanceCount0ID, j, i);
                    SetComputeShaderBuffer(_mergeSourceBuffer1ID, _mergeInstanceCount1ID, j + 1, i);
                    SetComputeShaderBuffer(_mergeSourceBuffer2ID, _mergeInstanceCount2ID, j + 2, i);
                    SetComputeShaderBuffer(_mergeSourceBuffer3ID, _mergeInstanceCount3ID, j + 3, i);
                    SetComputeShaderBuffer(_mergeSourceBuffer4ID, _mergeInstanceCount4ID, j + 4, i);
                    SetComputeShaderBuffer(_mergeSourceBuffer5ID, _mergeInstanceCount5ID, j + 5, i);
                    SetComputeShaderBuffer(_mergeSourceBuffer6ID, _mergeInstanceCount6ID, j + 6, i);
                    SetComputeShaderBuffer(_mergeSourceBuffer7ID, _mergeInstanceCount7ID, j + 7, i);
                    SetComputeShaderBuffer(_mergeSourceBuffer8ID, _mergeInstanceCount8ID, j + 8, i);
                    SetComputeShaderBuffer(_mergeSourceBuffer9ID, _mergeInstanceCount9ID, j + 9, i);
                    SetComputeShaderBuffer(_mergeSourceBuffer10ID, _mergeInstanceCount10ID, j + 10, i);
                    SetComputeShaderBuffer(_mergeSourceBuffer11ID, _mergeInstanceCount11ID, j + 11, i);
                    SetComputeShaderBuffer(_mergeSourceBuffer12ID, _mergeInstanceCount12ID, j + 12, i);
                    SetComputeShaderBuffer(_mergeSourceBuffer13ID, _mergeInstanceCount13ID, j + 13, i);
                    SetComputeShaderBuffer(_mergeSourceBuffer14ID, _mergeInstanceCount14ID, j + 14, i);
                    SetComputeShaderBuffer(_mergeSourceBuffer15ID, _mergeInstanceCount15ID, j + 15, i);

                    if (MergeBufferKernelHandle == 0)
                    { 
                        MergeBufferShader.Dispatch(MergeBufferKernelHandle, threadGroups, 1, 1);

                        for (int k = 0; k <= VegetationModelInfoList[i].VegetationItemInfo.sourceMesh.subMeshCount - 1; k++)
                        {
                            ComputeBuffer.CopyCount(VegetationModelInfoList[i].MergeBuffer,
                                VegetationModelInfoList[i].ArgsBufferMergedLOD0List[k], sizeof(uint) * 1);
                        }
                     }
                }

                Profiler.BeginSample("GPU culling");

                float lod1Distance = VegetationModelInfoList[i].LOD1Distance * QualitySettings.lodBias * VegetationModelInfoList[i].VegetationItemInfo.LODFactor;
                float lod2Distance = VegetationModelInfoList[i].LOD2Distance * QualitySettings.lodBias * VegetationModelInfoList[i].VegetationItemInfo.LODFactor;

                int threadGroupsFrustum = Mathf.CeilToInt(totalInstanceCount / 32f);
                VegetationModelInfoList[i].VisibleBufferLOD0.SetCounterValue(0);
               // VegetationModelInfoList[i].VisibleBufferLOD1.SetCounterValue(0);
               // VegetationModelInfoList[i].VisibleBufferLOD2.SetCounterValue(0);

                if (UseGPUCulling)
                {
                    FrusumMatrixShader.SetBuffer(FrustumKernelHandle, _sourceBufferID, VegetationModelInfoList[i].MergeBuffer);
                    FrusumMatrixShader.SetBuffer(FrustumKernelHandle, _visibleBufferLod0ID, VegetationModelInfoList[i].VisibleBufferLOD0);
                    //FrusumMatrixShader.SetBuffer(FrustumKernelHandle, _visibleBufferLod1ID, VegetationModelInfoList[i].VisibleBufferLOD1);
                    //FrusumMatrixShader.SetBuffer(FrustumKernelHandle, _visibleBufferLod2ID, VegetationModelInfoList[i].VisibleBufferLOD2);
                    FrusumMatrixShader.SetInt(_instanceCountID, totalInstanceCount);
                    FrusumMatrixShader.SetBool(_useLodsID, false);
                    FrusumMatrixShader.SetFloat(_boundingSphereRadiusID, VegetationModelInfoList[i].BoundingSphereRadius);
                    FrusumMatrixShader.SetFloat(_lod1Distance, lod1Distance);
                    FrusumMatrixShader.SetFloat(_lod2Distance, lod2Distance);
                    if (threadGroupsFrustum > 0)
                    {
                        FrusumMatrixShader.Dispatch(FrustumKernelHandle, threadGroupsFrustum, 1, 1);
                    }
                }
                else
                {
                    FrusumMatrixShader.SetBuffer(DistanceKernelHandle, _sourceBufferID, VegetationModelInfoList[i].MergeBuffer);
                    FrusumMatrixShader.SetBuffer(DistanceKernelHandle, _visibleBufferLod0ID, VegetationModelInfoList[i].VisibleBufferLOD0);
                    //FrusumMatrixShader.SetBuffer(DistanceKernelHandle, _visibleBufferLod1ID, VegetationModelInfoList[i].VisibleBufferLOD1);
                   // FrusumMatrixShader.SetBuffer(DistanceKernelHandle, _visibleBufferLod2ID, VegetationModelInfoList[i].VisibleBufferLOD2);
                    FrusumMatrixShader.SetInt(_instanceCountID, totalInstanceCount);
                    FrusumMatrixShader.SetBool(_useLodsID, false);
                    FrusumMatrixShader.SetFloat(_boundingSphereRadiusID, VegetationModelInfoList[i].BoundingSphereRadius);
                    FrusumMatrixShader.SetFloat(_lod1Distance, lod1Distance);
                    FrusumMatrixShader.SetFloat(_lod2Distance, lod2Distance);
                    if (threadGroupsFrustum > 0)
                    {
                        FrusumMatrixShader.Dispatch(DistanceKernelHandle, threadGroupsFrustum, 1, 1);
                    }
                }

                for (int k = 0; k <= VegetationModelInfoList[i].VegetationItemInfo.sourceMesh.subMeshCount - 1; k++)
                {
                    ComputeBuffer.CopyCount(VegetationModelInfoList[i].VisibleBufferLOD0,
                        VegetationModelInfoList[i].ArgsBufferMergedLOD0List[k], sizeof(uint) * 1);
                }

                /*
                for (int k = 0; k <= VegetationModelInfoList[i].VegetationMeshLod1.subMeshCount - 1; k++)
                {
                    ComputeBuffer.CopyCount(VegetationModelInfoList[i].VisibleBufferLOD1,
                        VegetationModelInfoList[i].ArgsBufferMergedLOD1List[k], sizeof(uint) * 1);
                }

                for (int k = 0; k <= VegetationModelInfoList[i].VegetationMeshLod2.subMeshCount - 1; k++)
                {
                    ComputeBuffer.CopyCount(VegetationModelInfoList[i].VisibleBufferLOD2,
                        VegetationModelInfoList[i].ArgsBufferMergedLOD2List[k], sizeof(uint) * 1);
                }
            */

                Profiler.EndSample();

                /*
                if (vegetationItemInfo.ShaderType == VegetationShaderType.Speedtree)
                {
                    if (VegetationModelInfoList[i].VegetationRendererLOD0)
                    {
                        //VegetationModelInfoList[i].VegetationRendererLOD0.GetPropertyBlock(_mergedMaterialPropertyBlock);
                        VegetationModelInfoList[i].VegetationRendererLOD0.GetPropertyBlock(VegetationModelInfoList[i].VegetationMaterialPropertyBlockLOD0);
                       // VegetationModelInfoList[i].VegetationRendererLOD0.GetPropertyBlock(VegetationModelInfoList[i].VegetationMaterialPropertyBlockLOD1);
                        //VegetationModelInfoList[i].VegetationRendererLOD0.GetPropertyBlock(VegetationModelInfoList[i].VegetationMaterialPropertyBlockLOD2);
                    }
                }
                
                if (UseIndirectLoDs)
                {
                    DrawMergedVegetationCellBuffer(i, VegetationModelInfoList[i].VisibleBufferLOD0, VegetationModelInfoList[i].ArgsBufferMergedLOD0List, VegetationModelInfoList[i].VegetationMeshLod0, VegetationModelInfoList[i].VegetationMaterialsLOD0, VegetationModelInfoList[i].VegetationMaterialPropertyBlockLOD0);
                    //DrawMergedVegetationCellBuffer(i, VegetationModelInfoList[i].VisibleBufferLOD1, VegetationModelInfoList[i].ArgsBufferMergedLOD1List, VegetationModelInfoList[i].VegetationMeshLod1, VegetationModelInfoList[i].VegetationMaterialsLOD1, VegetationModelInfoList[i].VegetationMaterialPropertyBlockLOD1);
                    //DrawMergedVegetationCellBuffer(i, VegetationModelInfoList[i].VisibleBufferLOD2, VegetationModelInfoList[i].ArgsBufferMergedLOD2List, VegetationModelInfoList[i].VegetationMeshLod2, VegetationModelInfoList[i].VegetationMaterialsLOD2, VegetationModelInfoList[i].VegetationMaterialPropertyBlockLOD2);
                }
                else
                {
                */
                     DrawMergedVegetationCellBuffer(i, VegetationModelInfoList[i].VisibleBufferLOD0, VegetationModelInfoList[i].ArgsBufferMergedLOD0List, VegetationModelInfoList[i].VegetationItemInfo.sourceMesh, VegetationModelInfoList[i].VegetationItemInfo.sourceMaterials, VegetationModelInfoList[i].VegetationMaterialPropertyBlockLOD0);
                //}

            }
            Profiler.EndSample();
#endif
        }

        void SetComputeShaderBuffer(int bufferID, int bufferCountID, int cellIndex, int vegetationItemIndex)
        {
            if (cellIndex < _hasBufferList.Count)
            {
                VegetationCell vegetationCell = _hasBufferList[cellIndex];
                int instanceCount = _hasBufferList[cellIndex].IndirectInfoList[vegetationItemIndex].InstanceCount;
                MergeBufferShader.SetBuffer(MergeBufferKernelHandle, bufferID, vegetationCell.IndirectInfoList[vegetationItemIndex].IndirectShaderDataBuffer);
                MergeBufferShader.SetInt(bufferCountID, instanceCount);
            }
            else
            {
                MergeBufferShader.SetInt(bufferCountID, 0);
            }
        }

        void DrawMergedVegetationCellBuffer(int i, ComputeBuffer visibleBuffer, List<ComputeBuffer> argBufferList, Mesh mesh, Material[] materials, MaterialPropertyBlock mergedMaterialPropertyBlock)
        {
            Camera targetCamera = null;
            if (Application.isPlaying)
                targetCamera = SelectedCamera;
            VegetationItemInfo vegetationItemInfo = VegetationModelInfoList[i].VegetationItemInfo;

            LayerMask vegetationLayer = GetVegetationLayer(vegetationItemInfo.VegetationType);
            if (vegetationItemInfo.VegetationRenderType != VegetationRenderType.InstancedIndirect) return;

            ShadowCastingMode shadowMode = ShadowCastingMode.Off;
            if (Application.isPlaying) shadowMode = GetVegetationShadowMode(vegetationItemInfo.VegetationType);
            if (vegetationItemInfo.DisableShadows) shadowMode = ShadowCastingMode.Off;

            mergedMaterialPropertyBlock.SetBuffer("VisibleShaderDataBuffer", visibleBuffer);
            mergedMaterialPropertyBlock.SetBuffer("IndirectShaderDataBuffer", visibleBuffer);
            
            float boundsDistance = vegetationSettings.VegetationDistance * 2 + 100;
            Bounds cellBounds = new Bounds(GetCameraPosition(),
                new Vector3(boundsDistance, boundsDistance, boundsDistance));

            for (int j = 0; j <= mesh.subMeshCount - 1; j++)
            {
                Graphics.DrawMeshInstancedIndirect(mesh, j, materials[j], cellBounds,
                    argBufferList[j], 0, mergedMaterialPropertyBlock, shadowMode, RecieveShadows, vegetationLayer,
                    targetCamera);
            }

            //for (int j = 0; j <= mesh.subMeshCount - 1; j++)
            //{
            //    //Mesh subMesh = mesh.GetSubmesh(j);
            //    materials[j].SetFloat("_Cutoff", 0.3f);

            //    Graphics.DrawMeshInstancedIndirect(mesh, j, materials[j],
            //        new Bounds(GetCameraPosition(), new Vector3(boundsDistance, boundsDistance, boundsDistance)),
            //        argBuffer, 0, mergedMaterialPropertyBlock, shadowMode, RecieveShadows, vegetationLayer,
            //        targetCamera);
            //}
        }
    }
}
