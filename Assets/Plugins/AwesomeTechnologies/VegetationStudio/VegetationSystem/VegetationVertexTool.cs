using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AwesomeTechnologies.VegetationStudio
{
    public enum LeafSelection
    {
        None = 0,
        Selecting = 1,
        Selected = 2,
    }

    public enum DebugMode
    {
        RGB = 0,
        Red = 1,
        Green = 2,
        Blue = 3,
        Alpha = 4,
        UV = 5,
        UV2 = 6,
        UV3 = 7,
        UV4 = 8
    }

    public enum BendingModes
    {
        VertexColors_Legacy = 1,
        VertexColorsAndUV4 = 2,
        VertexColors = 3
    }

    public enum Set1stBendingModes
    {
        AlongY_Axis = 1,
        RelativeToPivot = 2
    }
    public enum Mask2ndBendingModes
    {
        AlongY_Axis = 1,
        ByVertexColorBlue = 2
    }

    public class VegetationVertexTool : MonoBehaviour
    {
        protected bool isDebug = false;
        public bool isPrecisionPaint = false;

        private string shaderName = "";
        public int selected = 0;
        public BendingModes BendingModeSelected = BendingModes.VertexColors;
        public Set1stBendingModes set1StBendingModes = Set1stBendingModes.AlongY_Axis;
        public Mask2ndBendingModes mask2NdBendingModes = Mask2ndBendingModes.AlongY_Axis;
        public LeafSelection leafSelectionState;

        public bool mergeSubMeshes = false;
        public AnimationCurve curvY = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
        public AnimationCurve curvX = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
        public AnimationCurve curvZ = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
        public AnimationCurve curvPart = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
        public float primaryBlending = 1.0f;
        public float secondaryBlending = 0;

        public float currentBlending = 0;
        public AnimationCurve currentCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
        public DebugMode debugMode = DebugMode.RGB;

        VegetationShaderType type;
        public float roundStrenght = 0.2f;
        public float roundOuterSize = 0.4f;
        public float roundInnerSize = 0.3f;
        public float roundStrenghtMin = 0;
        public float roundStrenghtMax = 1f;

        public Mesh ReSavedObjectMesh(Mesh currentMesh, string localName, string dirPath, string lod)
        {
            
            Vector3[] vertices = currentMesh.vertices;
            Color[] colors = currentMesh.colors;
            Vector2[] uv = currentMesh.uv;


            if (colors.Length == 0)
            {
                colors = new Color[vertices.Length];
                for (int i = 0; i < vertices.Length; i++)
                {
                    colors[i] = new Color(0.0f, 0.0f, 0.0f, 1.0f);
                }
                currentMesh.colors = colors;
            }
            ///// create a new mesh    
            Mesh newMesh = new Mesh();
            newMesh.vertices = currentMesh.vertices;
            newMesh.colors = currentMesh.colors;
            newMesh.uv = currentMesh.uv;

            if (currentMesh.uv2 != null)
                newMesh.uv2 = currentMesh.uv2;

            if (currentMesh.uv3 != null)
                newMesh.uv3 = currentMesh.uv3;

            if (currentMesh.uv4 != null)
                newMesh.uv4 = currentMesh.uv4;

            newMesh.normals = currentMesh.normals;
            newMesh.tangents = currentMesh.tangents;

            if (currentMesh.subMeshCount == 1)
            {
                newMesh.triangles = currentMesh.triangles;
            }
            else if (currentMesh.subMeshCount == 2 && mergeSubMeshes == false)
            {
                newMesh.subMeshCount = 2;
                int[] tri1 = currentMesh.GetTriangles(0);
                int[] tri2 = currentMesh.GetTriangles(1);
                newMesh.SetTriangles(tri1, 0);
                newMesh.SetTriangles(tri2, 1);
            }
            // Convert tree creator tree
            else if (currentMesh.subMeshCount == 2 && mergeSubMeshes == true)
            {
                newMesh.subMeshCount = 1;
                int[] tri1 = currentMesh.GetTriangles(0);
                int[] tri2 = currentMesh.GetTriangles(1);
                int[] triCombined = new int[tri1.Length + tri2.Length];
                int counter = 0;

                Color[] TempColors = newMesh.colors;

                for (int i = 0; i < tri1.Length; i++)
                {
                    triCombined[i] = tri1[i];
                    counter = i;
                }
                counter += 1;
                for (int j = 0; j < tri2.Length; j++)
                {
                    triCombined[counter + j] = tri2[j];
                }
                newMesh.SetTriangles(triCombined, 0);
                newMesh.colors = TempColors;
            }
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.CreateAsset(newMesh, dirPath + "/" + localName + "/" + localName + lod + ".asset");
#endif
            return newMesh;
        }

        public void ReSaveObject()
        {
#if UNITY_EDITOR
            string path = AssetDatabase.GetAssetPath(PrefabUtility.GetCorrespondingObjectFromSource(gameObject));
            string localName = name;
            string dirPath = path.Substring(0, path.LastIndexOf('/'));

            if (!AssetDatabase.IsValidFolder(dirPath + "/" + localName))
                AssetDatabase.CreateFolder(dirPath, localName);

            GameObject newGO = new GameObject();
            newGO.name = localName;

            Material oldMat = GetComponentInChildren<Renderer>().sharedMaterial;
            Shader oldShader = oldMat.shader;
            Material newMaterial = new Material(oldShader);
            newMaterial.CopyPropertiesFromMaterial(GetComponentInChildren<Renderer>().sharedMaterial);

            LODGroup lODGroup = GetComponent<LODGroup>();
            if (lODGroup != null)
            {
                LODGroup newLodGroup = newGO.AddComponent<LODGroup>();
                newLodGroup.SetLODs(lODGroup.GetLODs());

                //LOD[] lod = new LOD[lODGroup.GetLODs().Length]
                LOD[] lod = lODGroup.GetLODs();

                MeshFilter[] mfs = GetComponentsInChildren<MeshFilter>(true);
                for (int i = 0; i < mfs.Length; i++)
                {
                    string lodStr = "_LOD" + i.ToString();
                    Mesh currentMesh = mfs[i].GetComponent<MeshFilter>().sharedMesh;
                    Mesh newMesh = ReSavedObjectMesh(currentMesh, localName, dirPath, lodStr);
                    GameObject newLodGO = new GameObject();
                    newLodGO.transform.parent = newGO.transform;
                    newLodGO.transform.localPosition = Vector3.zero;
                    newLodGO.name = localName + lodStr;
                    newLodGO.AddComponent<MeshFilter>().sharedMesh = newMesh;

                    MeshRenderer MR = newLodGO.AddComponent<MeshRenderer>();
                    MR.sharedMaterial = newMaterial;
                    lod[i].renderers = new Renderer[1];
                    lod[i].renderers[0] = MR;

                    //ApplyVertex(newMesh);
                }

                newLodGroup.SetLODs(lod);
            }
            else
            {
                Mesh currentMesh = GetComponentInChildren<MeshFilter>().sharedMesh;
                Mesh newMesh = ReSavedObjectMesh(currentMesh, localName, dirPath, "");
                newGO.AddComponent<MeshFilter>().sharedMesh = newMesh;

                newGO.AddComponent<MeshRenderer>().sharedMaterial = newMaterial;

                //ApplyVertex(newMesh);
            }

            

            UnityEngine.Object newSourcePrefab = PrefabUtility.CreateEmptyPrefab(dirPath + "/" + localName + "/" + localName + ".prefab");
                                    

            AssetDatabase.CreateAsset(newMaterial, dirPath + "/" + localName + "/" + localName + ".mat");
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            AssetDatabase.SaveAssets();

            

            GameObject newSource = PrefabUtility.ReplacePrefab(newGO, newSourcePrefab, ReplacePrefabOptions.ConnectToPrefab);
            EditorUtility.SetDirty(newSourcePrefab);
            AssetDatabase.SetLabels(newSourcePrefab, new string[1] { "Grass" });
            AssetDatabase.Refresh(ImportAssetOptions.Default);
            AssetDatabase.SaveAssets();

            
#endif
        }

        private void Reset()
        {
            Setup();
        }

        private void OnEnable()
        {
            Setup();
        }

        void Setup()
        {
            Material sourceMaterial = GetComponentInChildren<Renderer>().sharedMaterial;
            //BendingModeSelected = (BendingModes)(sourceMaterial.GetFloat("_BendingControls") + 1.0f);
            shaderName = sourceMaterial.shader.name;
        }

        void MeshTranslate(Mesh mesh, Vector3 pivot)
        {
            Vector3[] vertices = mesh.vertices;
            int vertexCount = mesh.vertexCount;

            for (int i = 0; i < vertexCount; i++)
                vertices[i] += pivot;

            mesh.vertices = vertices;
            mesh.UploadMeshData(true);
        }

        public void ApplyVertex()
        {
#if UNITY_EDITOR

            MeshRenderer MR = GetComponent<MeshRenderer>();
            Material mat = MR.sharedMaterial;

            
            Mesh currentMesh = GetComponent<MeshFilter>().sharedMesh;
            Transform currentSelection = GetComponent<Transform>();
            Vector3[] vertices = currentMesh.vertices;
            Color[] colors = currentMesh.colors;
            Vector2[] myUVs = currentMesh.uv;
            Vector2[] myUVs2 = currentMesh.uv2;
            Vector2[] myUVs3 = currentMesh.uv3;
            Vector2[] myUVs4 = currentMesh.uv4;
           
            if (colors.Length == 0)
            {
                colors = new Color[vertices.Length];
                for (int i = 0; i < vertices.Length; i++)
                {
                    colors[i] = new Color(0.0f, 0.0f, 0.0f, 1.0f);
                }
            }


            currentMesh.colors = colors;
            currentMesh.uv = myUVs;
            currentMesh.uv2 = myUVs2;
            currentMesh.uv3 = myUVs3;
            currentMesh.uv4 = myUVs4;

#endif
        }

        /*
        public void CreateBendingSetup()
        {
            Mesh currentMesh = GetComponent<MeshFilter>().sharedMesh;
            Vector3[] vertices = currentMesh.vertices;
            Color[] colors = currentMesh.colors;
           // Vector2[] myUVs = currentMesh.uv;
           // Vector2[] myUVs2 = currentMesh.uv2;
           // Vector2[] myUVs3 = currentMesh.uv3;
            Vector2[] myUVs4 = currentMesh.uv4;

            if (colors.Length == 0)
            {
                colors = new Color[vertices.Length];
                for (int i = 0; i < vertices.Length; i++)
                    colors[i] = new Color(0.0f, 0.0f, 0.0f, 1.0f);
            }
            
            
            //		// Recreate vertex color blue array if there is none
            //		if (VertexBlueValsStored == false) {
            //			VertexBlueVals = new float[colors.Length];
            //		}
            //Debug.Log(vertices.Length);
            for (int i = 0; i < vertices.Length; i++)
            {
                
                Bounds bounds = currentMesh.bounds;

                //	Legacy Bending: Primary and secondary stored in vertex color blue
                if (BendingModeSelected == BendingModes.VertexColors_Legacy)
                {
                    GetComponent<Renderer>().sharedMaterial.SetFloat("_BendingControls", 0.0f);
                    if (vertices[i].y <= 0.0f)
                    {
                        colors[i].b = 0.0f;
                        //Debug.Log("S0 B1 " + i.ToString() + " " + colors[i].b);
                    }
                    else
                    {
                        colors[i].b = Mathf.Lerp(0.0f, primaryBlending, curvY.Evaluate(vertices[i].y / bounds.size.y)); // primary
                        Vector3 tempPos = new Vector3(vertices[i].x, 0.0f, vertices[i].z);
                        float Length = Vector3.Distance(tempPos, new Vector3(0.0f, 0.0f, 0.0f)) / ((bounds.size.x + bounds.size.z) * 0.5f);
                        Length = curvX.Evaluate(Length);
                        colors[i].b += Mathf.Lerp(0.0f, secondaryBlending, Length);
                        //colors[i].b = Mathf.Lerp (0.0f, maxBendingValueX, Length ); // secondary
                        //Debug.Log("S0 B2 " + i.ToString() + " " + colors[i].b);
                    }
                }

                //	New bending: Primary and secondary stored in UV4 or Veretx Colors only 
                //if(BendingModeSelected == BendingModes.VertexColorsAndUV4) {
                else
                {
                    if (BendingModeSelected == BendingModes.VertexColorsAndUV4)
                    {
                        GetComponent<Renderer>().sharedMaterial.SetFloat("_BendingControls", 1.0f);
                    }
                    else
                    {
                        GetComponent<Renderer>().sharedMaterial.SetFloat("_BendingControls", 2.0f);
                    }

                    // //////////////////////////////
                    // Primary Bending 

                    // Along y-axis
                    if (set1StBendingModes == Set1stBendingModes.AlongY_Axis)
                    {
                        //	UV4
                        if (BendingModeSelected == BendingModes.VertexColorsAndUV4)
                        {
                            myUVs4[i].x = Mathf.Lerp(0.0f, primaryBlending, curvY.Evaluate(vertices[i].y / bounds.size.y));
                            //Debug.Log("S1 B1 " + i.ToString() + " " + myUVs2[i].x);
                        }
                        //	VertexColors Only
                        else
                        {
                            colors[i].a = Mathf.Lerp(0.0f, secondaryBlending, curvY.Evaluate(vertices[i].y / bounds.size.y));
                            //Debug.Log("S1 B2 " + i.ToString() + " " + colors[i].a);
                        }
                    }
                    // Relative to Pivot
                    else
                    {
                        float Length = Vector3.Distance(vertices[i], new Vector3(0.0f, 0.0f, 0.0f)) / ((bounds.size.x + bounds.size.y + bounds.size.z) * 0.333f);
                        Length = curvY.Evaluate(Length);
                        //	UV4
                        if (BendingModeSelected == BendingModes.VertexColorsAndUV4)
                        {
                            myUVs4[i].x = Mathf.Lerp(0.0f, secondaryBlending, Length);
                            //Debug.Log("S2 B1 " + i.ToString() + " " + myUVs2[i].x);
                        }
                        //	VertexColors Only
                        else
                        {
                            colors[i].a = Mathf.Lerp(0.0f, secondaryBlending, Length);
                            //Debug.Log("S2 B2 " + i.ToString() + " " + colors[i].a);
                        }
                    }

                    // /////////////////////////////
                    // Secondary Bending

                    // Mask by vertex color blue
                    if (mask2NdBendingModes == Mask2ndBendingModes.ByVertexColorBlue)
                    {
                        // Store original vertex color blue as we have to reset it!
                        //	if (VertexBlueValsStored == false) {
                        //		VertexBlueVals[i] = colors[i].b;
                        //	}
                        if (currentMesh != null)
                        {
                            Color[] originalColors = currentMesh.colors;
                            if (originalColors.Length == colors.Length)
                            {
                                Vector3 tempPos = new Vector3(vertices[i].x, 0.0f, vertices[i].z);
                                float Length = Vector3.Distance(tempPos, new Vector3(0.0f, 0.0f, 0.0f)) / ((bounds.size.x + bounds.size.z) * 0.5f);
                                Length = curvX.Evaluate(Length);
                                //	UV4
                                if (BendingModeSelected == BendingModes.VertexColorsAndUV4)
                                {
                                    myUVs4[i].y = Mathf.Lerp(0.0f, secondaryBlending, Length) * originalColors[i].b; //2.0f ???????
                                    colors[i].b = 0.0f;
                                    //Debug.Log("S3 B1 " + i.ToString() + " " + myUVs2[i].x);
                                }
                                //	VertexColors Only
                                else
                                {
                                    colors[i].b = Mathf.Lerp(0.0f, secondaryBlending, Length) * originalColors[i].b;
                                    //Debug.Log("S3 B2 " + i.ToString() + " " + colors[i].b);
                                }
                            }
                            else
                            {
                                //Debug.Log("Mesh data of current working mesh does not fit to the assigned original mesh.");
                            }
                        }
                    }
                    // Mask along y-axis
                    else
                    {
                        // Store original vertex color blue as we have to reset it!
                        //	if (VertexBlueValsStored == false) {
                        //		VertexBlueVals[i] = colors[i].b; 
                        //	}
                        Vector3 tempPos = new Vector3(vertices[i].x, 0.0f, vertices[i].z);
                        float Length = Vector3.Distance(tempPos, new Vector3(0.0f, 0.0f, 0.0f)) / ((bounds.size.x + bounds.size.z) * 0.5f);
                        Length = curvX.Evaluate(Length);
                        //	UV4
                        if (BendingModeSelected == BendingModes.VertexColorsAndUV4)
                        {
                            myUVs4[i].y = Mathf.Lerp(0.0f, secondaryBlending, Length) * curvZ.Evaluate(vertices[i].y / bounds.size.y); // * 2.0f;
                            colors[i].b = 0.0f;
                            //Debug.Log("S4 B1 " + i.ToString() + " " + myUVs4[i].x);
                        }
                        //	VertexColors Only
                        else
                        {
                            colors[i].b = Mathf.Lerp(0.0f, secondaryBlending, Length) * curvZ.Evaluate(vertices[i].y / bounds.size.y);
                            //Debug.Log("S4 B2 " + i.ToString() + " " + colors[i].b);
                        }
                    }
                }
            }
            //
            //// Update mesh
            currentMesh.colors = colors;
            if (BendingModeSelected == BendingModes.VertexColorsAndUV4)
            {
                currentMesh.uv4 = myUVs4;
                
            }

            currentMesh.UploadMeshData(false);

        }
    */
        public void FillBlack()
        {
            Mesh currentMesh = GetComponent<MeshFilter>().sharedMesh;
            Vector3[] vertices = currentMesh.vertices;
            Color[] colors = currentMesh.colors;

            for (int i = 0; i < vertices.Length; i++)
                colors[i].r = 0;

            currentMesh.colors = colors;
            currentMesh.UploadMeshData(false);

        }

        void ApplyVertexPartMesh(ref Mesh currentMesh)
        {
            Vector3[] vertices = currentMesh.vertices;
            Color[] colors = currentMesh.colors;
            Vector2[] myUVs = currentMesh.uv;
            Vector2[] myUVs2 = currentMesh.uv2;
            Vector2[] myUVs3 = currentMesh.uv3;
            Vector2[] myUVs4 = currentMesh.uv4;

            if (colors.Length == 0)
            {
                colors = new Color[vertices.Length];
                for (int i = 0; i < vertices.Length; i++)
                    colors[i] = new Color(0.0f, 0.0f, 0.0f, 1.0f);
            }
        
            for (int i = 0; i < vertices.Length; i++)
            {
                Bounds bounds = currentMesh.bounds;

                switch (debugMode)
                {
                    case DebugMode.Red:
                        {
                            colors[i].r = Mathf.Clamp(curvPart.Evaluate(vertices[i].y / bounds.size.y), secondaryBlending, primaryBlending);
                        }
                        break;
                    case DebugMode.Blue:
                        {
                            colors[i].b = Mathf.Clamp(curvPart.Evaluate(vertices[i].y / bounds.size.y), secondaryBlending, primaryBlending);
                        }
                        break;
                    case DebugMode.Green:
                        {
                            colors[i].g = Mathf.Clamp(curvPart.Evaluate(vertices[i].y / bounds.size.y), secondaryBlending, primaryBlending);

                        }
                        break;
                    case DebugMode.Alpha:
                        {
                            colors[i].a = Mathf.Clamp(curvPart.Evaluate(vertices[i].y / bounds.size.y), secondaryBlending, primaryBlending);

                        }
                        break;
                    case DebugMode.UV3:
                        {

                        }
                        break;

                    case DebugMode.UV4:
                        {

                        }
                        break;
                }
            }

            currentMesh.colors = colors;
            currentMesh.uv = myUVs;
            currentMesh.uv2 = myUVs2;
            currentMesh.uv3 = myUVs3;
            currentMesh.uv4 = myUVs4;
            currentMesh.UploadMeshData(false);
        }

        public void ApplyVertexPart()
        {
#if UNITY_EDITOR

            MeshRenderer MR = GetComponentInChildren<MeshRenderer>();
            Material mat = MR.sharedMaterial;

            LODGroup lODGroup = GetComponent<LODGroup>();
            if (lODGroup != null)
            {
                MeshFilter[] mfs = GetComponentsInChildren<MeshFilter>(true);
                for (int i=0; i<mfs.Length; i++)
                {
                    Mesh mesh = mfs[i].sharedMesh;
                    ApplyVertexPartMesh(ref mesh);
                }
            }
            else
            {
                MeshFilter mf = GetComponent<MeshFilter>();
                Mesh mesh = mf.sharedMesh;
                ApplyVertexPartMesh(ref mesh);
            }
#endif
        }

        

        public void ApplyVertex(Mesh mesh)
        {
#if UNITY_EDITOR
            Vector3[] vertices = mesh.vertices;
            Color[] colors = mesh.colors;
            if (colors.Length == 0)
            {
                colors = new Color[vertices.Length];
                for (int i = 0; i < vertices.Length; i++)
                {
                    colors[i] = new Color(0.0f, 0.0f, 0.0f, 1.0f);
                }
            }

            for (int i = 0; i < vertices.Length; i++)
            {
                Bounds bounds = mesh.bounds;

                colors[i].a = Mathf.Lerp(0.0f, primaryBlending, curvY.Evaluate(vertices[i].y / bounds.size.y));

                Vector3 tempPos = new Vector3(vertices[i].x, 0.0f, vertices[i].z);
                float Length = Vector3.Distance(tempPos, new Vector3(0.0f, 0.0f, 0.0f)) / ((bounds.size.x + bounds.size.z) * 0.5f);
                Length = curvX.Evaluate(Length);

                colors[i].b = Mathf.Lerp(0.0f, secondaryBlending, Length) * curvZ.Evaluate(vertices[i].y / bounds.size.y);
            }

            mesh.colors = colors;
            
#endif
        }

        public void UpdateBounds()
        {
            MeshRenderer mr = GetComponentInChildren<MeshRenderer>();
            float size = Mathf.Max(mr.bounds.size.x, mr.bounds.size.z);
            mr.sharedMaterial.SetVector("_Bounds", new Vector4(-size, -size, 0, 0));
        }

        public void EnableDebugShader()
        {
            isDebug = true;
            GetComponentInChildren<Renderer>().sharedMaterial.shader = Shader.Find("Hidden/Foliage Shader Debug");
        }

        public void DeleteUVLayer()
        {
            LODGroup lODGroup = GetComponent<LODGroup>();
            if (lODGroup != null)
            {
                MeshFilter[] mfs = GetComponentsInChildren<MeshFilter>(true);
                for (int i = 0; i < mfs.Length; i++)
                {
                    Mesh mesh = mfs[i].sharedMesh;
                    DeleteUVLayerMesh(mesh);
                }
            }
            else
            {
                Mesh currentMesh = GetComponent<MeshFilter>().sharedMesh;
                DeleteUVLayerMesh(currentMesh);
            }

            
        }

        void DeleteUVLayerMesh(Mesh currentMesh)
        {
            

            switch (selected)
            {
                case 5:
                    currentMesh.uv = null;
                    break;
                case 6:
                    currentMesh.uv2 = null;
                    break;
                case 7:
                    currentMesh.uv3 = null;
                    break;
                case 8:
                    currentMesh.uv4 = null;
                    break;
            }

            currentMesh.UploadMeshData(false);
        }

        public void CopyUVLayer(bool isLeft)
        {
            LODGroup lODGroup = GetComponent<LODGroup>();
            if (lODGroup != null)
            {
                MeshFilter[] mfs = GetComponentsInChildren<MeshFilter>(true);
                for (int i = 0; i < mfs.Length; i++)
                {
                    Mesh mesh = mfs[i].sharedMesh;
                    CopyUVLayerMesh(mesh, isLeft);
                }
            }
            else
            {
                Mesh currentMesh = GetComponent<MeshFilter>().sharedMesh;
                CopyUVLayerMesh(currentMesh, isLeft);
            }



        }

        void CopyUVLayerMesh(Mesh currentMesh, bool isLeft)
        {
            switch (selected)
            {
                case 5:
                    if (isLeft)
                        currentMesh.uv4 = currentMesh.uv;
                    else
                        currentMesh.uv2 = currentMesh.uv;
                    break;
                case 6:
                    if (isLeft)
                        currentMesh.uv = currentMesh.uv2;
                    else
                        currentMesh.uv3 = currentMesh.uv2;
                    break;
                case 7:
                    if (isLeft)
                        currentMesh.uv2 = currentMesh.uv3;
                    else
                        currentMesh.uv4 = currentMesh.uv3;
                    break;
                case 8:
                    if (isLeft)
                        currentMesh.uv3 = currentMesh.uv4;
                    else
                        currentMesh.uv = currentMesh.uv4;
                    break;
            }
            currentMesh.UploadMeshData(false);
        }

        public void DisableDebugShader()
        {
            isPrecisionPaint = false;
            isDebug = false;
            if (shaderName!="")
                GetComponentInChildren<Renderer>().sharedMaterial.shader = Shader.Find(shaderName);
            else
                GetComponentInChildren<Renderer>().sharedMaterial.shader = Shader.Find("AwesomeTechnologies/AFoliageStandard");
        }

        public void SetDebugMode()
        {
            /*
            if (debugMode == DebugMode.UV4)
            {
                Vector2[] uvs = GetComponentInChildren<MeshFilter>().sharedMesh.uv4;
                
                for (int i = 0; i < uvs.Length; i++)
                {
                    Debug.Log(i + "|x: " + uvs[i].x + ", y: " + uvs[i].y);
                }
            }  
            */
            if (debugMode == 0 || debugMode == DebugMode.UV || debugMode == DebugMode.UV2 || debugMode == DebugMode.UV3 || debugMode == DebugMode.UV4)
                DisableDebugShader();
            else
            {
                if (isDebug == false)
                    EnableDebugShader();
                GetComponentInChildren<Renderer>().sharedMaterial.SetFloat("_DebugMode", (int)debugMode);
            }


        }
    }
}