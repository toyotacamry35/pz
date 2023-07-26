using System.Collections.Generic;
using Cinemachine.Editor;
using UnityEditor;

namespace Assets.Src.Camera.Editor
{
    [CustomEditor(typeof(PlayerFreeLookCamera))]
    internal class FreeLookCameraEditor : CinemachineVirtualCameraBaseEditor<PlayerFreeLookCamera>
    {
         protected override List<string> GetExcludedPropertiesInInspector()
        {
            List<string> excluded = base.GetExcludedPropertiesInInspector();
            return excluded;
        }

        public override void OnInspectorGUI()
        {
            // Ordinary properties
            BeginInspector();
            DrawHeaderInInspector();
            DrawPropertyInInspector(FindProperty(x => x.m_Priority));
            DrawRemainingPropertiesInInspector();
            DrawExtensionsWidgetInInspector();
        }
    }
}
