using System.Collections.Generic;
using Cinemachine;
using Cinemachine.Editor;
using UnityEditor;
using UnityEngine;

namespace Assets.Src.Camera.Editor
{
    [CustomEditor(typeof(PlayerCameraRig))]
    internal sealed class BaseCameraRigEditor 
        : CinemachineVirtualCameraBaseEditor<PlayerCameraRig>
    {
        //private EmbeddeAssetEditor<CinemachineBlenderSettings> _blendsEditor;
        private UnityEditorInternal.ReorderableList _camerasList;

        protected override List<string> GetExcludedPropertiesInInspector()
        {
            List<string> excluded = base.GetExcludedPropertiesInInspector();
            excluded.Add(FieldPath(x => x.CustomBlends));
            return excluded;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            /*_blendsEditor = new EmbeddeAssetEditor<CinemachineBlenderSettings>(
                    FieldPath(x => x.CustomBlends), this);
            _blendsEditor.OnChanged = b =>
                {
                    UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
                };
            _blendsEditor.OnCreateEditor = ed =>
                {
                    CinemachineBlenderSettingsEditor editor = ed as CinemachineBlenderSettingsEditor;
                    if (editor != null)
                        editor.GetAllVirtualCameras = () => { return Target.ChildCameras; };
                };*/
            _camerasList = null;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            //if (_blendsEditor != null)
              //  _blendsEditor.OnDisable();
        }

        public override void OnInspectorGUI()
        {
            BeginInspector();
            if (_camerasList == null)
                SetupChildList();

            // Ordinary properties
            DrawHeaderInInspector();
            DrawPropertyInInspector(FindProperty(x => x.m_Priority));
            DrawRemainingPropertiesInInspector();

            // vcam children
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.Separator();
            _camerasList.DoLayoutList();
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }

            // Blends
            /*_blendsEditor.DrawEditorCombo(
                "Create New Blender Asset",
                Target.gameObject.name + " Blends", "asset", string.Empty,
                "Custom Blends", false);*/
            
            // Extensions
            DrawExtensionsWidgetInInspector();
        }

        void SetupChildList()
        {
            float vSpace = 2;
            float hSpace = 3;
            float floatFieldWidth = EditorGUIUtility.singleLineHeight * 2.5f;
            float hBigSpace = EditorGUIUtility.singleLineHeight * 2 / 3;
            PlayerCameraRig.ChildCamera elementProto = null;

            _camerasList = new UnityEditorInternal.ReorderableList(serializedObject,
                    serializedObject.FindProperty(() => Target.Cameras),
                    true, true, true, true);

            _camerasList.elementHeight = EditorGUIUtility.singleLineHeight * 2 + vSpace * 3;

            _camerasList.drawHeaderCallback = (Rect rect) =>
                {
                    EditorGUI.LabelField(rect, "Tag / Camera");
                    GUIContent priorityText = new GUIContent("Priority");
                    var textDimensions = GUI.skin.label.CalcSize(priorityText);
                    rect.x += rect.width - textDimensions.x;
                    rect.width = textDimensions.x;
                    EditorGUI.LabelField(rect, priorityText);
                };
            _camerasList.drawElementCallback
                = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    rect.y += vSpace; 
                    rect.height = EditorGUIUtility.singleLineHeight;
                    rect.width -= floatFieldWidth + hBigSpace;
                    SerializedProperty elementProp = _camerasList.serializedProperty.GetArrayElementAtIndex(index);
                    SerializedProperty tagProp = elementProp.FindPropertyRelative(() => elementProto.Tag);
                    EditorGUI.PropertyField(new Rect(rect.x, rect.y + vSpace, rect.width, rect.height), tagProp, GUIContent.none);
                    SerializedProperty cameraProp = elementProp.FindPropertyRelative(() => elementProto.Camera);
                    EditorGUI.PropertyField(new Rect(rect.x, rect.y + rect.height + vSpace * 2, rect.width, rect.height), cameraProp, GUIContent.none);
                    var camera = cameraProp.objectReferenceValue; 
                    if (camera)
                    {
                        float oldWidth = EditorGUIUtility.labelWidth;
                        EditorGUIUtility.labelWidth = hBigSpace;
                        SerializedObject cameraObj = new SerializedObject(camera);
                        rect.x += rect.width + hSpace;
                        rect.width = floatFieldWidth + hBigSpace;
                        SerializedProperty priorityProp = cameraObj.FindProperty(() => Target.m_Priority);
                        EditorGUI.PropertyField(rect, priorityProp, new GUIContent(" ", priorityProp.tooltip));
                        EditorGUIUtility.labelWidth = oldWidth; 
                        cameraObj.ApplyModifiedProperties();
                    }
                };
            _camerasList.onChangedCallback = (UnityEditorInternal.ReorderableList l) =>
                {
                    if (l.index < 0 || l.index >= l.serializedProperty.arraySize)
                        return;
                    Object o = l.serializedProperty.GetArrayElementAtIndex(
                            l.index).FindPropertyRelative(() => elementProto.Camera).objectReferenceValue;
                    CinemachineVirtualCameraBase vcam = (o != null)
                        ? (o as CinemachineVirtualCameraBase) : null;
                    if (vcam != null)
                        vcam.transform.SetSiblingIndex(l.index);
                };
            _camerasList.onAddCallback = (UnityEditorInternal.ReorderableList l) =>
                {
                    var index = l.serializedProperty.arraySize;
                    /*var vcam = CinemachineMenu.CreateDefaultVirtualCamera();
                    Undo.SetTransformParent(vcam.transform, Target.transform, "");
                    vcam.transform.SetSiblingIndex(index);*/
                };
            _camerasList.onRemoveCallback = (UnityEditorInternal.ReorderableList l) =>
                {
                    Object o = l.serializedProperty.GetArrayElementAtIndex(
                            l.index).FindPropertyRelative(() => elementProto.Camera).objectReferenceValue;
                    CinemachineVirtualCameraBase vcam = (o != null)
                        ? (o as CinemachineVirtualCameraBase) : null;
                    if (vcam != null)
                        Undo.DestroyObjectImmediate(vcam.gameObject);
                };
        }
    }
}
