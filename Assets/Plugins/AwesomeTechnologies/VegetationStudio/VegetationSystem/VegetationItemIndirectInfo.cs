using System.Collections.Generic;
using Assets.Src.Lib.ProfileTools;
using AwesomeTechnologies.Utility;
using UnityEngine;
using UnityEngine.Profiling;

namespace AwesomeTechnologies
{
    public struct IndirectShaderData
    {
        public Matrix4x4 PositionMatrix;
        public Matrix4x4 InversePositionMatrix;
        public Vector4 ControlData;
    }

    public class VegetationItemIndirectInfo
    {
        public bool InstancedIndirect;
        public VegetationItemModelInfo CurrentVegetationItemModelInfo;

        public CustomList<Matrix4x4> MatrixList;
        private bool _needProcess;

        public int InstanceCount;

        public ComputeBuffer PositionBuffer;
        public ComputeBuffer IndirectShaderDataBuffer;


        public List<ComputeBuffer> ArgsBufferlod0List = new List<ComputeBuffer>();
        public List<ComputeBuffer> ArgsBufferlod1List = new List<ComputeBuffer>();
        public List<ComputeBuffer> ArgsBufferlod2List = new List<ComputeBuffer>();

        public MaterialPropertyBlock MaterialPropertyBlockLOD0;
        //public MaterialPropertyBlock MaterialPropertyBlockLOD1;
        //public MaterialPropertyBlock MaterialPropertyBlockLOD2;
        public readonly uint[] Args = {0, 0, 0, 0, 0};
        private int _threadGroups;

        public VegetationItemIndirectInfo(bool instancedIndirect)
        {
            InstancedIndirect = instancedIndirect;
        }

        public void AddMatrixList(CustomList<Matrix4x4> matrixList)
        {
            MatrixList = matrixList;
            _needProcess = true;
        }

        public void SetDirty()
        {
            _needProcess = true;
        }

        void ReleaseArgsBuffers()
        {
            for (int i = 0; i <= ArgsBufferlod0List.Count - 1; i++)
            {
                if (ArgsBufferlod0List[i] != null) ArgsBufferlod0List[i].Release();
            }

            for (int i = 0; i <= ArgsBufferlod1List.Count - 1; i++)
            {
                if (ArgsBufferlod1List[i] != null) ArgsBufferlod1List[i].Release();
            }

            for (int i = 0; i <= ArgsBufferlod2List.Count - 1; i++)
            {
                if (ArgsBufferlod2List[i] != null) ArgsBufferlod2List[i].Release();
            }

            ArgsBufferlod0List.Clear();
            ArgsBufferlod1List.Clear();
            ArgsBufferlod2List.Clear();
        }

        private void CreateComputeBuffer(bool useComputeShaders)
        {
            if (IndirectShaderDataBuffer != null) IndirectShaderDataBuffer.Release();
            IndirectShaderDataBuffer = null;

            if (PositionBuffer != null) PositionBuffer.Release();
            PositionBuffer = null;

            ReleaseArgsBuffers();

            MaterialPropertyBlockLOD0 = new MaterialPropertyBlock();
            //CurrentVegetationItemModelInfo.VegetationRendererLOD0.GetPropertyBlock(MaterialPropertyBlockLOD0);
            //MaterialPropertyBlockLOD1 = new MaterialPropertyBlock();
            //CurrentVegetationItemModelInfo.VegetationRendererLOD1.GetPropertyBlock(MaterialPropertyBlockLOD1);
            //MaterialPropertyBlockLOD2 = new MaterialPropertyBlock();
            //CurrentVegetationItemModelInfo.VegetationRendererLOD2.GetPropertyBlock(MaterialPropertyBlockLOD2);

            int instanceCount = MatrixList.Count;
            InstanceCount = instanceCount;
            if (instanceCount > 0)
            {

                for (int i = 0; i <= CurrentVegetationItemModelInfo.VegetationItemInfo.sourceMesh.subMeshCount - 1; i++)
                {
                    ComputeBuffer argsBufferlod0 = new ComputeBuffer(1, Args.Length * sizeof(uint),
                        ComputeBufferType.IndirectArguments);
                    ArgsBufferlod0List.Add(argsBufferlod0);
                }
                /*
                for (int i = 0; i <= CurrentVegetationItemModelInfo.VegetationMeshLod1.subMeshCount - 1; i++)
                {
                    ComputeBuffer argsBufferlod1 = new ComputeBuffer(1, Args.Length * sizeof(uint),
                        ComputeBufferType.IndirectArguments);
                    ArgsBufferlod1List.Add(argsBufferlod1);
                }

                for (int i = 0; i <= CurrentVegetationItemModelInfo.VegetationMeshLod2.subMeshCount - 1; i++)
                {
                    ComputeBuffer argsBufferlod2 = new ComputeBuffer(1, Args.Length * sizeof(uint),
                        ComputeBufferType.IndirectArguments);
                    ArgsBufferlod2List.Add(argsBufferlod2);
                }
                */
                _threadGroups = Mathf.CeilToInt(instanceCount / 32f);
                int bufferCount = _threadGroups * 32;           

                PositionBuffer = new ComputeBuffer(bufferCount, 16 * 4);
                IndirectShaderDataBuffer = new ComputeBuffer(bufferCount, (16 * 4 * 2) + 16);

                Profiler.BeginSample("update buffers");
                PositionBuffer.SetData(MatrixList.Data);

               

                MaterialPropertyBlockLOD0.SetBuffer("IndirectShaderDataBuffer", IndirectShaderDataBuffer);
   
                if (SystemInfo.supportsComputeShaders && useComputeShaders)
                {
                    ComputeShader inverseMatrixShader = Profile.Load<ComputeShader>("CreateInverseMatrix");
                    int kernelHandle = inverseMatrixShader.FindKernel("CreateInverseMatrix");
                    inverseMatrixShader.SetBuffer(kernelHandle, "positionBuffer", PositionBuffer);
                    inverseMatrixShader.SetBuffer(kernelHandle, "IndirectShaderDataBuffer", IndirectShaderDataBuffer);
                    inverseMatrixShader.Dispatch(kernelHandle, _threadGroups, 1, 1);                  
                }
                else
                {
                    CustomList<IndirectShaderData> inverseList = new CustomList<IndirectShaderData>(MatrixList.Count);
                    for (int i = 0; i <= MatrixList.Count - 1; i++)
                    {
                        IndirectShaderData shaderData = new IndirectShaderData
                        {
                            PositionMatrix = MatrixList[i],
                            InversePositionMatrix = MatrixList[i].inverse,
                            ControlData = Vector4.zero
                        };
                        inverseList.Add(shaderData);
                    }
                    IndirectShaderDataBuffer.SetData(inverseList.Data);
                }

                SetArgBuffers();
                Profiler.EndSample();
            }
        }

        public void SetArgBuffers()
        {
            if (ArgsBufferlod0List.Count == 0 || ArgsBufferlod1List.Count == 0 || ArgsBufferlod2List.Count == 0) return;

            for (int i = 0; i <= CurrentVegetationItemModelInfo.VegetationItemInfo.sourceMesh.subMeshCount - 1; i++)
            {
                Args[0] = CurrentVegetationItemModelInfo.VegetationItemInfo.sourceMesh.GetIndexCount(i);
                Args[1] = (uint) InstanceCount;
                Args[2] = CurrentVegetationItemModelInfo.VegetationItemInfo.sourceMesh.GetIndexStart(i);
                ArgsBufferlod0List[i].SetData(Args);
            }
            /*
            for (int i = 0; i <= CurrentVegetationItemModelInfo.VegetationMeshLod1.subMeshCount - 1; i++)
            {
                Args[0] = CurrentVegetationItemModelInfo.VegetationMeshLod1.GetIndexCount(i);
                Args[1] = (uint) InstanceCount;
                Args[2] = CurrentVegetationItemModelInfo.VegetationMeshLod1.GetIndexStart(i);
                ArgsBufferlod1List[i].SetData(Args);
            }

            for (int i = 0; i <= CurrentVegetationItemModelInfo.VegetationMeshLod2.subMeshCount - 1; i++)
            {
                Args[0] = CurrentVegetationItemModelInfo.VegetationMeshLod2.GetIndexCount(i);
                Args[1] = (uint) InstanceCount;
                Args[2] = CurrentVegetationItemModelInfo.VegetationMeshLod2.GetIndexStart(i);
                ArgsBufferlod2List[i].SetData(Args);
            }
            */
        }

        public void UpdateComputeBuffer(bool useComputeShaders)
        {
            if (_needProcess)
            {
                Profiler.BeginSample("outside function");
                CreateComputeBuffer(useComputeShaders);
                _needProcess = false;
                Profiler.EndSample();
            }          
        }       

        public void OnDisable()
        {
            if (IndirectShaderDataBuffer != null) IndirectShaderDataBuffer.Release();
            IndirectShaderDataBuffer = null;

            if (PositionBuffer != null) PositionBuffer.Release();
            PositionBuffer = null;

            ReleaseArgsBuffers();

        }
    }
}
