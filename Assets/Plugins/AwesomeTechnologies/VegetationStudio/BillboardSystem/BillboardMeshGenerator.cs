using AwesomeTechnologies.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace AwesomeTechnologies.Billboards
{
    public class BillboardMeshGenerator
    {
        public float VegetationItemSize = 1;
        public int BillboardIndex;
        public LayerMask TreeLayerMask;
        public Transform BillboardParent;
        public float QuadSize = 1;
        public Bounds CelBounds;
        //private Mesh _currentMesh;
        private readonly List<Vector3> _vertices = new List<Vector3>();
        private readonly List<int> _indices = new List<int>();
        private readonly List<UnityEngine.Vector2> _uvs = new List<UnityEngine.Vector2>();
        private readonly List<UnityEngine.Vector2> _uv2S = new List<UnityEngine.Vector2>();
        private readonly List<UnityEngine.Vector2> _uv3S = new List<UnityEngine.Vector2>();
        private readonly List<Vector3> _normals = new List<Vector3>();
        public List<Mesh> MeshList = new List<Mesh>();
        //public List<GameObject> GameObjectList = new List<GameObject>();

        private readonly Vector3[] _srcVerts = new Vector3[4];
        private readonly UnityEngine.Vector2[] _srcUVs = new UnityEngine.Vector2[4];
        private readonly int[] _srcInds = new int[6];

        public int QuadCount;

        private int vertLimit = 60000;
        //private int maxQuadsCount = 60000 / 4;

        private int _vertIndex;
        //public Material BillboardMaterial;

        public BillboardMeshGenerator()
        {
            _srcVerts[0] = new Vector3(-0.5f, -0.5f, 0);
            _srcVerts[1] = new Vector3(0.5f, 0.5f, 0);
            _srcVerts[2] = new Vector3(0.5f, -0.5f, 0);
            _srcVerts[3] = new Vector3(-0.5f, 0.5f, 0);

            _srcUVs[0] = new UnityEngine.Vector2(0f, 0f);
            _srcUVs[1] = new UnityEngine.Vector2(1f, 1f);
            _srcUVs[2] = new UnityEngine.Vector2(1f, 0f);
            _srcUVs[3] = new UnityEngine.Vector2(0f, 1);

            _srcInds[0] = 0;
            _srcInds[1] = 1;
            _srcInds[2] = 2;
            _srcInds[3] = 1;
            _srcInds[4] = 0;
            _srcInds[5] = 3;

            //BillboardMaterial = new Material(Shader.Find("Custom/ATrees"));
        }
        public void AddQuad(Matrix4x4 matrix, Vector3 terrainPosition, float boundsYExtent)
        {
            QuadCount++;

            //Vector3 position = MatrixTools.ExtractTranslationFromMatrix(matrix);
            Vector3 scale = MatrixTools.ExtractScaleFromMatrix(matrix) / 2f;

            Vector3 position = MatrixTools.ExtractTranslationFromMatrix(matrix) + new Vector3(0,boundsYExtent * scale.y * 2 ,0);
            Quaternion rotation = MatrixTools.ExtractRotationFromMatrix(matrix);
            //Vector3 scale = MatrixTools.ExtractScaleFromMatrix(matrix);
           

            _vertices.Add(position);
            _vertices.Add(position);
            _vertices.Add(position);
            _vertices.Add(position);

            _normals.Add(_srcVerts[0]);
            _normals.Add(_srcVerts[1]);
            _normals.Add(_srcVerts[2]);
            _normals.Add(_srcVerts[3]);

            _uvs.Add(_srcUVs[0]);
            _uvs.Add(_srcUVs[1]);
            _uvs.Add(_srcUVs[2]);
            _uvs.Add(_srcUVs[3]);

            //Vector2 rotationTint;
            //rotationTint.x = rotation.eulerAngles.y / 360f; // rotation from 0.....1
            //rotationTint.y = 5f;// .7f + Random.value * .6f; // color.rgb * rotation.y
            var rotationVector = new UnityEngine.Vector2((360f - rotation.eulerAngles.y) / 360f, 1f);
            _uv2S.Add(rotationVector);
            _uv2S.Add(rotationVector);
            _uv2S.Add(rotationVector);
            _uv2S.Add(rotationVector);
            //_uv2S.Add(new Vector2(rotation.eulerAngles.y / 360f, 1f));
            //_uv2S.Add(new Vector2(rotation.eulerAngles.y / 360f, 1f));
            //_uv2S.Add(new Vector2(rotation.eulerAngles.y / 360f, 1f));
            //_uv2S.Add(new Vector2(rotation.eulerAngles.y / 360f, 1f));

            UnityEngine.Vector2 scaleAndVFix;

            scaleAndVFix.x = VegetationItemSize * scale.x * 2f;// Random.Range(10f, 15f); // quad scale
            scaleAndVFix.y = -(boundsYExtent * scale.y * 2);// scaleAndVFix.x * .3f; // vertical max shift on look from top

            _uv3S.Add(scaleAndVFix);
            _uv3S.Add(scaleAndVFix);
            _uv3S.Add(scaleAndVFix);
            _uv3S.Add(scaleAndVFix);

            _indices.Add(_srcInds[0] + _vertIndex);
            _indices.Add(_srcInds[1] + _vertIndex);
            _indices.Add(_srcInds[2] + _vertIndex);
            _indices.Add(_srcInds[3] + _vertIndex);
            _indices.Add(_srcInds[4] + _vertIndex);
            _indices.Add(_srcInds[5] + _vertIndex);

            _vertIndex += 4;

            if (_vertices.Count > vertLimit)
            {
                ProcessMesh(terrainPosition, true);
              
            }
        }

        public void ClearMeshes()
        {
            for (int i = 0; i <= MeshList.Count - 1; i++)
            {
                if (Application.isPlaying)
                {
                    Object.Destroy(MeshList[i]);
                }
                else
                {
                    Object.DestroyImmediate(MeshList[i]);
                }
            }
            MeshList.Clear();
            _vertIndex = 0;
            _vertices.Clear();
            _indices.Clear();
            _uvs.Clear();
            _uv2S.Clear();
            _uv3S.Clear();
            _normals.Clear();
        }

        //public void MoveMeshes(Vector3 offset, List<Vector3> moveVertexTempList)
        //{
        //    for (int i = 0; i <= MeshList.Count - 1; i++)
        //    {
        //        moveVertexTempList.Clear();
        //        MeshList[i].GetVertices(moveVertexTempList);
        //        //Vector3[] vertices = MeshList[i].vertices;
        //        for (int j = 0; j <= moveVertexTempList.Count - 1; j++)
        //        {
        //            moveVertexTempList[j] = moveVertexTempList[j] + offset;
        //        }
        //        MeshList[i].SetVertices(moveVertexTempList);
        //        //MeshList[i].vertices = vertices;

        //        MeshList[i].bounds = CelBounds;
        //    }
        //}

        public void ProcessMesh(Vector3 terrainPosition,bool fullMesh = false)
        {
            if (_vertices.Count == 0) return;

            var newMesh = new Mesh {hideFlags = HideFlags.DontSave};
            newMesh.SetVertices(_vertices);            
            newMesh.SetIndices(_indices.ToArray(), MeshTopology.Triangles, 0,false);
            newMesh.SetUVs(0, _uvs);
            newMesh.SetUVs(1, _uv2S);
            newMesh.SetUVs(2, _uv3S);
            newMesh.SetNormals(_normals);

           // Bounds meshBound = CelBounds;
           // meshBound.center -= terrainPosition;
           // newMesh.bounds = meshBound;
            newMesh.RecalculateBounds();

            MeshList.Add(newMesh);

            _vertIndex = 0;
            _vertices.Clear();
            _indices.Clear();
            _uvs.Clear();
            _uv2S.Clear();
            _uv3S.Clear();
            _normals.Clear();

            //GameObject newMeshObject = new GameObject();
            //newMeshObject.hideFlags = HideFlags.DontSave;// | HideFlags.HideInHierarchy;
            //newMeshObject.transform.SetParent(BillboardParent, false);
            //newMeshObject.transform.position = new Vector3();
            //newMeshObject.transform.rotation = Quaternion.identity;
            //newMeshObject.transform.localScale = new Vector3(1, 1, 1);
            //newMeshObject.name = "BillboardMesh_" + BillboardIndex.ToString() + "_" + MeshList.Count.ToString();
            //newMeshObject.layer = TreeLayerMask;


            //GameObjectList.Add(newMeshObject);

            //MeshFilter meshFilter = newMeshObject.AddComponent<MeshFilter>();
            //meshFilter.sharedMesh = newMesh;

            //MeshRenderer meshRenderer = newMeshObject.AddComponent<MeshRenderer>();
            //meshRenderer.sharedMaterial = BillboardMaterial;
            //meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
    }
}

