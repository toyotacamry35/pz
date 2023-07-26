using System;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.Instancenator.Editor
{
    public class InstanceCompositionBuilder
    {
        public struct InstanceData
        {
            public Vector3 position;
            public Quaternion rotate;
            public Vector4 value;
            public byte lodIndex2bit;
            public byte options2bit;
        }

        public struct LOD
        {
            public Mesh mesh;
            public Material material;
            public float distanceInPercents;
            public bool isCastShadows;
        }

        public class Block
        {
            public LOD[] lods;
            public float maxDistance = 150.0f;
            public float transitionInPersents = 10.0f;
            public bool isEnableTransition = false;
            public List<InstanceData> instances = new List<InstanceData>();
        }

        public List<Block> blocks = new List<Block>();

        public int minTrianglesForSplit = 30000;
        public float stopSplitBoundsSize = 32;
        public Vector4 getScaleFomValueMulX = new Vector4(0, 0, 0, 1);
        public Vector4 getScaleFomValueMulY = new Vector4(0, 0, 0, 1);
        public Vector4 getScaleFomValueMulZ = new Vector4(0, 0, 0, 1);
        public Vector3 getScaleFomValueAdd = Vector3.zero;


        public void Process(InstanceComposition compositoin)
        {
            buildBlocksBuffer = new List<InstanceComposition.Block>();
            instancesBuffer = new List<InstanceComposition.InstanceData>();
            for (int i = 0; i < blocks.Count; i++)
            {
                Block block = blocks[i];
                if (block.instances.Count == 0 || block.maxDistance < 1.0f ||
                    block.transitionInPersents < 0.0f || block.transitionInPersents > 100.0f ||
                    block.lods.Length == 0 || block.lods.Length > 4)
                {
                    Debug.LogError("InstanceCompositionBuilder: invalid block " + i + "!");
                    continue;
                }

                if (block.lods[0].mesh == null || block.lods[0].material == null ||
                    block.lods[0].distanceInPercents <= 0.0f || block.lods[0].distanceInPercents > 100.0f)
                {
                    Debug.LogError("InstanceCompositionBuilder: invalidate lod 0!");
                    continue;
                }

                if (Mathf.Abs(block.lods[block.lods.Length - 1].distanceInPercents - 100.0f) > 0.01f)
                {
                    Debug.LogError("InstanceCompositionBuilder: distanceInPercents for lod " + (block.lods.Length - 1) + " must be 100!");
                    continue;
                }

                Block processBlock = new Block();
                processBlock.lods = new LOD[block.lods.Length];
                processBlock.maxDistance = block.maxDistance;
                processBlock.isEnableTransition = block.isEnableTransition;
                processBlock.transitionInPersents = block.transitionInPersents;
                bool isHaveErrors = false;
                for (int j = 0; j < block.lods.Length; j++)
                {
                    processBlock.lods[j] = block.lods[j];
                    if (j == 0)
                    {
                        continue;
                    }
                    if (processBlock.lods[j].mesh == null)
                    {
                        processBlock.lods[j].mesh = processBlock.lods[j - 1].mesh;
                    }
                    if (processBlock.lods[j].material == null)
                    {
                        processBlock.lods[j].material = processBlock.lods[j - 1].material;
                    }
                    if (processBlock.lods[j].distanceInPercents <= processBlock.lods[j - 1].distanceInPercents)
                    {
                        Debug.LogError("InstanceCompositionBuilder: invalidate lod " + j + "!");
                        isHaveErrors = true;
                    }
                }
                if (isHaveErrors)
                {
                    continue;
                }


                bool presentInvalidLodIndex = false;
                bool presentInvalidOptions = false;
                Bounds splitBounds = new Bounds(block.instances[0].position, Vector3.zero);
                foreach (InstanceData data in block.instances)
                {
                    splitBounds.Encapsulate(data.position);
                    presentInvalidLodIndex |= data.lodIndex2bit < 0;
                    presentInvalidLodIndex |= data.lodIndex2bit >= block.lods.Length;
                    presentInvalidOptions |= (data.options2bit & ~3) != 0;
                }
                splitBounds.size = splitBounds.size + new Vector3(0.01f, 0.5f, 0.01f);
                if (presentInvalidLodIndex)
                {
                    Debug.LogError("InstanceCompositionBuilder: block " + i + " instances have invalidate lodIndex2bit!");
                    continue;
                }
                if (presentInvalidOptions)
                {
                    Debug.LogError("InstanceCompositionBuilder: block " + i + " instances have invalidate options2bit!");
                    continue;
                }

                SplitBlock(splitBounds, block.instances, processBlock);
            }

            if (buildBlocksBuffer.Count > 0)
            {
                compositoin.bounds = buildBlocksBuffer[0].bounds;
                for (int i = 1; i < buildBlocksBuffer.Count; i++)
                {
                    compositoin.bounds.Encapsulate(buildBlocksBuffer[i].bounds);
                }
                compositoin.blocks = buildBlocksBuffer.ToArray();
                compositoin.instances = instancesBuffer.ToArray();
            }
            buildBlocksBuffer = null;
            instancesBuffer = null;
        }

        private List<InstanceComposition.Block> buildBlocksBuffer;
        private List<InstanceComposition.InstanceData> instancesBuffer;


        private void SplitBlock(Bounds splitBounds, List<InstanceData> instances, Block block)
        {
            if (instances.Count == 0)
            {
                return;
            }
            if (splitBounds.size.x > stopSplitBoundsSize && splitBounds.size.z > stopSplitBoundsSize)
            {
                int meshTrianglesCount = (int)block.lods[0].mesh.GetIndexCount(0) / 3;
                if (instances.Count * meshTrianglesCount > minTrianglesForSplit)
                {
                    Vector3 center = splitBounds.center;
                    Vector3 halfSize = new Vector3(splitBounds.size.x * 0.5f, splitBounds.size.y, splitBounds.size.z * 0.5f);
                    float quatX = splitBounds.size.x * 0.25f;
                    float quatZ = splitBounds.size.z * 0.25f;

                    //zx
                    Bounds bounds00 = new Bounds(center + new Vector3(-quatX, 0.0f, -quatZ), halfSize);
                    List<InstanceData> instances00 = new List<InstanceData>();
                    Bounds bounds01 = new Bounds(center + new Vector3(quatX, 0.0f, -quatZ), halfSize);
                    List<InstanceData> instances01 = new List<InstanceData>();
                    Bounds bounds10 = new Bounds(center + new Vector3(-quatX, 0.0f, quatZ), halfSize);
                    List<InstanceData> instances10 = new List<InstanceData>();
                    Bounds bounds11 = new Bounds(center + new Vector3(quatX, 0.0f, quatZ), halfSize);
                    List<InstanceData> instances11 = new List<InstanceData>();
                    //Sort by position
                    foreach (InstanceData data in instances)
                    {
                        List<InstanceData> buffer = null;
                        if (data.position.z < center.z)
                        {
                            //0x
                            buffer = (data.position.x < center.x) ? instances00 : instances01;
                        }
                        else
                        {
                            //1x
                            buffer = (data.position.x < center.x) ? instances10 : instances11;
                        }
                        buffer.Add(data);
                    }
                    SplitBlock(bounds00, instances00, block);
                    SplitBlock(bounds01, instances01, block);
                    SplitBlock(bounds10, instances10, block);
                    SplitBlock(bounds11, instances11, block);
                    return;
                }
            }
            InstanceComposition.Block icBlock = new InstanceComposition.Block();
            icBlock.transitionOffset = Vector4.zero;
            icBlock.transitionDistance = Vector4.zero;
            icBlock.lods = new InstanceComposition.LOD[block.lods.Length];
            for (int i = 0; i < block.lods.Length; i++)
            {
                icBlock.lods[i].instanceMesh = block.lods[i].mesh;
                icBlock.lods[i].instanceMaterial = block.lods[i].material;
                icBlock.lods[i].instancesCount = 0;
                icBlock.lods[i].maxDistance = Mathf.Clamp01(block.lods[i].distanceInPercents * 0.01f) * block.maxDistance;
                icBlock.lods[i].isCastShadows = block.lods[i].isCastShadows;
                if (block.isEnableTransition)
                {
                    float transitionOffset = icBlock.lods[i].maxDistance * Mathf.Clamp01(block.transitionInPersents * 0.01f);
                    float transitionDistance = icBlock.lods[i].maxDistance - transitionOffset;
                    icBlock.transitionOffset[i] = transitionOffset;
                    icBlock.transitionDistance[i] = transitionDistance;
                }
            }
            Bounds meshBounds = block.lods[0].mesh.bounds;
            icBlock.bounds = new Bounds(instances[0].position, Vector3.zero);
            icBlock.valueMin = instances[0].value;
            icBlock.valueMax = instances[0].value;
            instances.Sort((left, right) => left.lodIndex2bit > right.lodIndex2bit ? -1 : 1);
            foreach (InstanceData data in instances)
            {
                icBlock.valueMin = Vector4.Min(icBlock.valueMin, data.value);
                icBlock.valueMax = Vector4.Max(icBlock.valueMax, data.value);

                Vector3 scale;
                scale.x = Vector4.Dot(getScaleFomValueMulX, data.value) + getScaleFomValueAdd.x;
                scale.y = Vector4.Dot(getScaleFomValueMulY, data.value) + getScaleFomValueAdd.y;
                scale.z = Vector4.Dot(getScaleFomValueMulZ, data.value) + getScaleFomValueAdd.z;

                Matrix4x4 localToWorld = Matrix4x4.TRS(data.position, data.rotate, scale);
                Bounds worldInstanceBounds = Helpers.CalcOrientedBoxBounds(localToWorld, meshBounds);
                icBlock.bounds.Encapsulate(worldInstanceBounds);

                icBlock.lods[data.lodIndex2bit & 3].instancesCount++;
            }

            for (int i = block.lods.Length - 2; i >= 0; i--)
            {
                icBlock.lods[i].instancesCount += icBlock.lods[i + 1].instancesCount;
            }

            icBlock.instancesOffset = instancesBuffer.Count;
            buildBlocksBuffer.Add(icBlock);

            for (int i = 0; i < instances.Count; i++)
            {
                Vector3 position = Helpers.ConvertVertexToBlock(instances[i].position, icBlock);
                Vector4 value = Helpers.ConvertValueToBlock(instances[i].value, icBlock);
                Quaternion rotate = Quaternion.Normalize(instances[i].rotate);

                InstanceComposition.InstanceData data = InstanceComposition.InstanceData.Create(position,
                                                                                                rotate,
                                                                                                value,
                                                                                                instances[i].lodIndex2bit,
                                                                                                instances[i].options2bit);
                instancesBuffer.Add(data);
            }
        }

    }
}
