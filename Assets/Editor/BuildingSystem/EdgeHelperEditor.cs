using UnityEditor;
using UnityEngine;

namespace Assets.Src.BuildingSystem.Editor
{
    [CustomEditor(typeof(EdgeHelperBehaviour))]
    public class EdgeHelperEditor : UnityEditor.Editor
    {
        private bool initialized = false;
        private bool failed = false;
        public bool needUpdatePrefab = false;

        public SerializedProperty BlockFace_Property = null;
        public SerializedProperty BlockEdge_Property = null;

        public GUIContent BlockFace_Label = new GUIContent("Faces");
        public GUIContent BlockEdge_Label = new GUIContent("Edges");

        public GUIContent BlockFaceInt_Label = new GUIContent("Faces (int)");
        public GUIContent BlockEdgeInt_Label = new GUIContent("Edges (int)");

        private bool Init()
        {
            if (initialized) { return !failed; }
            initialized = true;

            BlockFace_Property = serializedObject.FindProperty("BlockFace");
            BlockEdge_Property = serializedObject.FindProperty("BlockEdge");

            failed = (BlockFace_Property == null) ||
                     (BlockEdge_Property == null);

            return !failed;
        }

        public override void OnInspectorGUI()
        {
            EdgeHelperBehaviour edgeHelperBehaviour = target as EdgeHelperBehaviour;
            if (edgeHelperBehaviour == null)
            {
                EditorGUILayout.HelpBox("Target object is null or not a EdgeHelperBehaviour.", MessageType.Error, true);
                return;
            }
            else
            {
                if (!Init())
                {
                    EditorGUILayout.HelpBox("Error initializing Properties.", MessageType.Error, true);
                }
                else
                {
                    var facesChanged = false;
                    var edgesChanged = false;
                    serializedObject.Update();

                    EditorGUILayout.Separator();
                    EditorGUI.BeginChangeCheck();
                    BlockFace_Property.intValue = EditorGUILayout.MaskField(BlockFace_Label, BlockFace_Property.intValue, BlockFace_Property.enumNames);
                    facesChanged = EditorGUI.EndChangeCheck();

                    EditorGUILayout.Separator();
                    EditorGUI.BeginChangeCheck();
                    BlockEdge_Property.intValue = EditorGUILayout.MaskField(BlockEdge_Label, BlockEdge_Property.intValue, BlockEdge_Property.enumNames);
                    edgesChanged = EditorGUI.EndChangeCheck();

                    serializedObject.ApplyModifiedProperties();

                    EditorGUILayout.Separator();
                    EditorGUI.BeginChangeCheck();
                    var blockFaceValue = EditorGUILayout.IntField(BlockFaceInt_Label, (int)(edgeHelperBehaviour.BlockFace));
                    if (EditorGUI.EndChangeCheck())
                    {
                        edgeHelperBehaviour.BlockFace = (EdgeHelperBehaviour.Faces)(blockFaceValue);
                        facesChanged = true;
                    }
                    EditorGUILayout.HelpBox($"{edgeHelperBehaviour.BlockFace}", MessageType.Info, true);

                    EditorGUILayout.Separator();
                    EditorGUI.BeginChangeCheck();
                    var blockEdgeValue = EditorGUILayout.IntField(BlockEdgeInt_Label, (int)edgeHelperBehaviour.BlockEdge);
                    if (EditorGUI.EndChangeCheck())
                    {
                        edgeHelperBehaviour.BlockEdge = (EdgeHelperBehaviour.Edges)(blockEdgeValue);
                        edgesChanged = true;
                    }
                    EditorGUILayout.HelpBox($"{edgeHelperBehaviour.BlockEdge}", MessageType.Info, true);

                    if (facesChanged)
                    {
                        edgeHelperBehaviour.UpdateFaces();
                    }
                    if (edgesChanged)
                    {
                        edgeHelperBehaviour.UpdateEdges();
                    }
                }
            }
        }
    }
}
