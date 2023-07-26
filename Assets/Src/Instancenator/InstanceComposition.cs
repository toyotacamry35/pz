using System;
using UnityEngine;


namespace Assets.Instancenator
{

    public class InstanceComposition : ScriptableObject
    {
        public static int shaderInstanceBufferName;
        public static int shaderInstancesOffset;
        public static int shaderBoxSize;
        public static int shaderBoxMin;
        public static int shaderValueMul;
        public static int shaderValueAdd;
        public static int shaderTransitionMul;
        public static int shaderTransitionAdd;
        public static int shaderObjectToWorld;
        public static int shaderGlobalLodBiasReciprocal;
        public static int computeBlockInfoID;
        public static int computeIndirectArgsID;
        public static int computeThinningCoefficientID;
        public static int computeWorldSpaceCameraPositionID;
        public static int computeFrustumPlanesID;
        public static int computeFrustumPointsID;
        public static int computeLodBiasID;


        private static bool isInitShaderIDs = false;

        public static void InitShaderIDs()
        {
            if (!isInitShaderIDs)
            {
                shaderInstanceBufferName = Shader.PropertyToID("InstanceBuffer");
                shaderInstancesOffset = Shader.PropertyToID("InstancesOffset");
                shaderBoxSize = Shader.PropertyToID("BoxSize");
                shaderBoxMin = Shader.PropertyToID("BoxMin");
                shaderValueMul = Shader.PropertyToID("ValueMul");
                shaderValueAdd = Shader.PropertyToID("ValueAdd");
                shaderTransitionMul = Shader.PropertyToID("TransitionMul");
                shaderTransitionAdd = Shader.PropertyToID("TransitionAdd");
                shaderObjectToWorld = Shader.PropertyToID("ObjectToWorldMtx");
                shaderGlobalLodBiasReciprocal = Shader.PropertyToID("LodBiasReciprocal");
                computeBlockInfoID = Shader.PropertyToID("_BlockInfo");
                computeIndirectArgsID = Shader.PropertyToID("_IndirectArgs");
                computeThinningCoefficientID = Shader.PropertyToID("_ThinningCoefficient");
                computeWorldSpaceCameraPositionID = Shader.PropertyToID("_WorldSpaceCameraPosition");
                computeFrustumPlanesID = Shader.PropertyToID("_FrustumPlanes");
                computeFrustumPointsID = Shader.PropertyToID("_FrustumPoints");
                computeLodBiasID = Shader.PropertyToID("_LodBias");
                isInitShaderIDs = true;
            }
        }


        [Serializable]
        public struct InstanceData
        {
            public const int structSize = 16;

            [SerializeField]
            private uint p0;//posz16_posx16;
            [SerializeField]
            private uint p1;//quatw16_posy16;
            [SerializeField]
            private uint p2;//opts3_tr2_quatys1_quatz13_quatx13;
            [SerializeField]
            private uint p3;//value8888;

            public static InstanceData Create(Vector3 pos, Quaternion rotate, Vector4 value, int transitionIndex, int options)
            {
                InstanceData data = new InstanceData();

                uint x = (uint)(Mathf.Clamp01(pos.x) * 65535.0f);
                uint y = (uint)(Mathf.Clamp01(pos.y) * 65535.0f);
                uint z = (uint)(Mathf.Clamp01(pos.z) * 65535.0f);

                uint qx = (uint)(Mathf.Clamp01(rotate.x * 0.5f + 0.5f) * 0x1fff);
                uint qz = (uint)(Mathf.Clamp01(rotate.z * 0.5f + 0.5f) * 0x1fff);
                uint qw = (uint)(Mathf.Clamp01(rotate.w * 0.5f + 0.5f) * 0xffff);
                uint qys = rotate.y >= 0.0f ? 0 : 1u;


                uint vx = (uint)(Mathf.Clamp01(value.x) * 255.0f);
                uint vy = (uint)(Mathf.Clamp01(value.y) * 255.0f);
                uint vz = (uint)(Mathf.Clamp01(value.z) * 255.0f);
                uint vw = (uint)(Mathf.Clamp01(value.w) * 255.0f);

                uint trIndex = (uint)Mathf.Clamp(transitionIndex, 0, 3);
                uint opts = (uint)(options & 3);

                data.p0 = (z << 16) | x;
                data.p1 = (qw << 16) | y;
                data.p2 = (opts << 29) | (trIndex << 27) | (qys << 26) | (qz << 13) | qx;
                data.p3 = (vw << 24) | (vz << 16) | (vy << 8) | (vx);

                return data;
            }

            public static uint SuppressCS0414(InstanceData id)
            {
                return id.p0 + id.p1 + id.p2 + id.p3;
            }
        }


        [Serializable]
        public struct LOD
        {
            public Mesh instanceMesh;
            public Material instanceMaterial;
            public int instancesCount;
            public float maxDistance;
            public bool isCastShadows;
        }

        [Serializable]
        public struct Block
        {
            public Bounds bounds;
            public Vector4 valueMin;
            public Vector4 valueMax;
            public Vector4 transitionOffset;
            public Vector4 transitionDistance;
            public int instancesOffset;
            public LOD[] lods;
        }

        public Bounds bounds;
        public Block[] blocks;
        public InstanceData[] instances;    //block0[[[[lod3]lod2]lod1]lod0], block1[[[[lod3]lod2]lod1]lod0], block2[[[[lod3]lod2]lod1]lod0], ...
    }

}
