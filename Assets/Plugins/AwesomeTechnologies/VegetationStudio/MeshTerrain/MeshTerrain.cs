using Assets.Src.Lib.ProfileTools;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace AwesomeTechnologies.MeshTerrains
{
    [System.Serializable]
    public enum MeshTerrainSourceType
    {
        MeshTerrainSource1 = 0,
        MeshTerrainSource2 = 1,
        MeshTerrainSource3 = 2,
        MeshTerrainSource4 = 3,
        MeshTerrainSource5 = 4,
        MeshTerrainSource6 = 5,
        MeshTerrainSource7 = 6,
        MeshTerrainSource8 = 7
    }

    [System.Serializable]
    public struct MeshTerrainMeshSource
    {
        public MeshRenderer MeshRenderer;
        public MeshTerrainSourceType MeshTerrainSourceType;
        public MaterialPropertyBlock MaterialPropertyBlock;
    }

    [System.Serializable]
    public struct MeshTerrainTerrainSource
    {
        public Terrain Terrain;
        public MeshTerrainSourceType MeshTerrainSourceType;
        public MaterialPropertyBlock MaterialPropertyBlock;
    }

    [ExecuteInEditMode]
    public class MeshTerrain : MonoBehaviour
    {

        Color GetMeshTerrainSourceTypeColor(MeshTerrainSourceType meshTerrainSourceType)
        {
            switch (meshTerrainSourceType)
            {
                case MeshTerrainSourceType.MeshTerrainSource1:
                    return Color.green;
                case MeshTerrainSourceType.MeshTerrainSource2:
                    return Color.red;
                case MeshTerrainSourceType.MeshTerrainSource3:
                    return Color.blue;
                case MeshTerrainSourceType.MeshTerrainSource4:
                    return Color.yellow;
                case MeshTerrainSourceType.MeshTerrainSource5:
                    return Color.grey;
                case MeshTerrainSourceType.MeshTerrainSource6:
                    return Color.magenta;
                case MeshTerrainSourceType.MeshTerrainSource7:
                    return Color.cyan;
                case MeshTerrainSourceType.MeshTerrainSource8:
                    return Color.white;
            }

            return Color.green;
        }

        public int CurrentTabIndex;
        public MeshTerrainData MeshTerrainData;
        public List<MeshTerrainTerrainSource> MeshTerrainTerrainSourceList = new List<MeshTerrainTerrainSource>();
        public List<MeshTerrainMeshSource> MeshTerrainMeshSourceList = new List<MeshTerrainMeshSource>();
        public bool ShowDebugInfo = false;
        public bool NeedGeneration = false;

        private Material _debugMaterial;

        public void AddMeshRenderer(GameObject go, MeshTerrainSourceType meshTerrainSourceType)
        {
            MeshRenderer[] meshRenderers = go.GetComponentsInChildren<MeshRenderer>();

            for (int i = 0; i <= meshRenderers.Length - 1; i++)
            {
                var newMeshTerrainTerrainSource = new MeshTerrainMeshSource()
                {
                    MeshRenderer = meshRenderers[i],
                    MeshTerrainSourceType = meshTerrainSourceType,
                    MaterialPropertyBlock = new MaterialPropertyBlock()

                };
                MeshTerrainMeshSourceList.Add(newMeshTerrainTerrainSource);
            }
            NeedGeneration = true;
        }


        public void AddTerrain(GameObject go, MeshTerrainSourceType meshTerrainSourceType)
        {

            Terrain[] terrains = go.GetComponentsInChildren<Terrain>();

            for (int i = 0; i <= terrains.Length - 1; i++)
            {
                var newMeshTerrainTerrainSource = new MeshTerrainTerrainSource
                {
                    Terrain = terrains[i],
                    MeshTerrainSourceType = meshTerrainSourceType,
                    MaterialPropertyBlock = new MaterialPropertyBlock()
                };

                MeshTerrainTerrainSourceList.Add(newMeshTerrainTerrainSource);
                NeedGeneration = true;
            }         
        }

        void OnEnable()
        {
           _debugMaterial = Profile.Load<Material>("MeshTerrainDebugMaterial");

           

        }


        bool HasMeshRenderer(MeshRenderer meshRenderer)
        {
            for (int i = 0; i <= MeshTerrainMeshSourceList.Count - 1; i++)
            {
                if (MeshTerrainMeshSourceList[i].MeshRenderer == meshRenderer) return true;
            }
            return false;
        }

        bool HasTerrain(Terrain terrain)
        {
            for (int i = 0; i <= MeshTerrainTerrainSourceList.Count - 1; i++)
            {
                if (MeshTerrainTerrainSourceList[i].Terrain == terrain) return true;
            }
            return false;
        }

      

        void OnRenderObject()
        {
            DrawDebuginfo();
        }

        void Update()
        {
            DrawDebuginfo();
        }

        void DrawDebuginfo()
        {
            if (ShowDebugInfo)
            {
                for (int i = 0; i <= MeshTerrainMeshSourceList.Count - 1; i++)
                {
                    if (MeshTerrainMeshSourceList[i].MaterialPropertyBlock == null)
                    {
                        MeshTerrainMeshSource meshTerrainMeshSource = MeshTerrainMeshSourceList[i];
                        meshTerrainMeshSource.MaterialPropertyBlock = new MaterialPropertyBlock();
                        MeshTerrainMeshSourceList[i] = meshTerrainMeshSource;
                    }
                    DrawMeshRenderer(MeshTerrainMeshSourceList[i].MeshRenderer, MeshTerrainMeshSourceList[i].MaterialPropertyBlock, MeshTerrainMeshSourceList[i].MeshTerrainSourceType);
                }
            }
        }

        void DrawMeshRenderer(MeshRenderer meshRenderer, MaterialPropertyBlock materialPropertyBlock, MeshTerrainSourceType meshTerrainSourceType)
        {
            if (!meshRenderer) return;
            MeshFilter meshFilter = meshRenderer.gameObject.GetComponent<MeshFilter>();
            if (!meshFilter) return;
            if (!meshFilter.sharedMesh) return;
            Matrix4x4 matrix = Matrix4x4.TRS(meshRenderer.transform.position, meshRenderer.transform.rotation,meshRenderer.transform.lossyScale);

            materialPropertyBlock.SetColor("_Color", GetMeshTerrainSourceTypeColor(meshTerrainSourceType));

            for (int i = 0; i <= meshFilter.sharedMesh.subMeshCount - 1; i++)
            {
                Graphics.DrawMesh(meshFilter.sharedMesh, matrix, _debugMaterial, 0, null, i, materialPropertyBlock);
            }
           
        }
    }
}
