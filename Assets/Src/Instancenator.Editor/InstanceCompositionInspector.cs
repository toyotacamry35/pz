using UnityEditor;
using UnityEngine;

namespace Assets.Instancenator.Editor
{

    [CustomEditor(typeof(InstanceComposition))]
    public class InstanceCompositionInspector : UnityEditor.Editor
    {
        private Vector2 scrollPos = Vector2.zero;

        public void OnEnable()
        {

        }

        public void OnDisable()
        {

        }

        public override void OnInspectorGUI()
        {
            InstanceComposition ic = target as InstanceComposition;

            EditorGUILayout.BeginVertical();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            GUILayout.Space(20);
            EditorGUILayout.LabelField("Object box center: " + ic.bounds.center);
            EditorGUILayout.LabelField("Object box size: " + ic.bounds.size);
            EditorGUILayout.LabelField("Object instances: " + ic.instances.Length);
            GUILayout.Space(20);
            if (ic.blocks != null && ic.blocks.Length != 0)
            {
                for (int i = 0; i < ic.blocks.Length; i++)
                {
                    EditorGUILayout.LabelField("Block " + i);

                    int allInstances = 0;
                    for (int j = 0; j < ic.blocks[i].lods.Length; j++)
                    {
                        allInstances = Mathf.Max(allInstances, ic.blocks[i].lods[j].instancesCount);
                    }
                    EditorGUILayout.LabelField("     Instances: " + allInstances);
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("     Box center: " + ic.blocks[i].bounds.center);
                    EditorGUILayout.LabelField("     Box size: " + ic.blocks[i].bounds.size);
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("     Value min: " + ic.blocks[i].valueMin);
                    EditorGUILayout.LabelField("     Value max: " + ic.blocks[i].valueMax);
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("     Transition");
                    for (int k = 0; k < 4; k++)
                    {
                        float from = ic.blocks[i].transitionOffset[k];
                        float dist = ic.blocks[i].transitionDistance[k];                        
                        string label = "no transition";
                        if (from > 0.0f || dist > 0.0f)
                        {
                            label = string.Format("from {0,0:F1} to {1,0:F1}", from, from + dist);
                        }
                        EditorGUILayout.LabelField("         [" + k + "] " + label);
                    }
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("     Total instances buffer size: " + allInstances * InstanceComposition.InstanceData.structSize);
                    float beginRadius = 0.0f;
                    for (int j = 0; j < ic.blocks[i].lods.Length; j++)
                    {
                        EditorGUILayout.LabelField(" ");
                        EditorGUILayout.LabelField("     LOD " + j);
                        Mesh mesh = ic.blocks[i].lods[j].instanceMesh;
                        Material material = ic.blocks[i].lods[j].instanceMaterial;
                        int trianglesInMesh = 0;
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.BeginVertical();
                        EditorGUILayout.LabelField(" ", GUILayout.Width(32));
                        EditorGUILayout.EndVertical();
                        if (mesh != null)
                        {
                            trianglesInMesh = mesh.triangles != null ? mesh.triangles.Length / 3 : 0;
                            EditorGUILayout.BeginVertical();
                            EditorGUILayout.LabelField("Mesh:" + mesh.name, GUILayout.Width(128));
                            EditorGUILayout.LabelField("Triangles:" + trianglesInMesh, GUILayout.Width(128));
                            Texture2D meshPreview = AssetPreview.GetAssetPreview(mesh);
                            if (meshPreview != null)
                            {                                
                                EditorGUILayout.LabelField(new GUIContent(meshPreview), GUILayout.Width(64), GUILayout.Height(64));
                            }
                            EditorGUILayout.EndVertical();
                        }
                        else
                        {
                            EditorGUILayout.LabelField("Mesh: <no mesh set>");
                        }
                        if (material != null)
                        {
                            EditorGUILayout.BeginVertical();
                            EditorGUILayout.LabelField("Material: " + material.name);
                            EditorGUILayout.LabelField(" ");
                            Texture2D materialPreview = AssetPreview.GetAssetPreview(material);
                            if (materialPreview != null)
                            {
                                EditorGUILayout.LabelField(new GUIContent(materialPreview), GUILayout.Width(64), GUILayout.Height(64));
                            }
                            EditorGUILayout.EndVertical();
                        }
                        else
                        {
                            EditorGUILayout.LabelField("Material: <no material set>");
                        }
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.LabelField("         Use instances in lod:" + ic.blocks[i].lods[j].instancesCount);
                        EditorGUILayout.LabelField(string.Format("         Range from {0,0:F1} to {1,0:F1}", beginRadius, ic.blocks[i].lods[j].maxDistance));
                        EditorGUILayout.LabelField("         Total triangles:" + ic.blocks[i].lods[j].instancesCount * trianglesInMesh);
                        EditorGUILayout.LabelField("         Instances buffer size:" + ic.blocks[i].lods[j].instancesCount * InstanceComposition.InstanceData.structSize);
                        beginRadius = ic.blocks[i].lods[j].maxDistance;
                    }
                }
            }
            else
            {
                EditorGUILayout.LabelField("InstanceComposition now is Empty");
            }

            GUILayout.Space(20);
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();

        }
    }
}