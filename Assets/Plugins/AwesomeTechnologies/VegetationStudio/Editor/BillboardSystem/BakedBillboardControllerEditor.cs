using UnityEditor;

namespace AwesomeTechnologies.Billboards
{
    [CustomEditor(typeof(BakedBillboardController))]
    public class BakedBillboardControllerEditor : VegetationStudioBaseEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.HelpBox("This components controlls the culling distance and camera position for the billboard culling. You should set the far clipping on the tree layer to just a bit more than billboard distance + total vegetation and tree distance.", MessageType.Info);
        }
    }
}
