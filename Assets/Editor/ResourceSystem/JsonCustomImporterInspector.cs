using System;
using System.IO;
using Assets.Editor.ResourceSystem;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Src.ResourceSystem.Editor
{
    [CustomEditor(typeof(JsonCustomImporter))]
    public class JsonCustomImporterInspector : ScriptedImporterEditor
    {
        private JdbReflectionDrawer _reflectionDrawer;
        private string _guid;
        private Mode _currentMode;

        private Mode CurrentMode
        {
            get { return _currentMode; }
            set
            {
                if (_currentMode != value)
                {
                    _currentMode = value;
                    EditorPrefs.SetString("JdbMetadataInspector.Mode", value.ToString());
                }
            }
        }

        public override bool showImportedObject => CurrentMode == Mode.Importer;

        protected override void OnHeaderGUI()
        {
            if (_reflectionDrawer == null)
            {
                base.OnHeaderGUI();
                return;
            }

            GUILayout.Space(3);
            using (new EditorGUILayout.HorizontalScope(GUI.skin.box))
            {
                var icon = ResourceSystemIcons.GetAssetIconByGuid(_guid);
                if (icon)
                    GUILayout.Box(icon, GUI.skin.label, GUILayout.Width(40), GUILayout.Height(40));

                using (new EditorGUILayout.VerticalScope())
                {
                    GUILayout.Space(3);
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        GUILayout.Label(Path.GetFileName(_reflectionDrawer.JdbAddress), new GUIStyle(GUI.skin.label) {fontStyle = FontStyle.Bold});
                        GUILayout.Label($" ({_reflectionDrawer.JdbTypeName})");
                        GUILayout.FlexibleSpace();
                    }

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Toggle(CurrentMode == Mode.Structure, "Structure", GUI.skin.GetStyle("minibuttonleft"), GUILayout.Width(70)))
                            CurrentMode = Mode.Structure;
                        if (GUILayout.Toggle(CurrentMode == Mode.Importer, "Importer", GUI.skin.GetStyle("minibuttonright"), GUILayout.Width(70)))
                            CurrentMode = Mode.Importer;
                    }
                }
            }
        }

        public override void OnInspectorGUI()
        {
            switch (CurrentMode)
            {
                case Mode.Importer:
                    base.OnInspectorGUI();
                    break;
                case Mode.Structure:
                    _reflectionDrawer?.Draw(EditorApplication.isPlaying);
                    GUILayout.FlexibleSpace();
                    ApplyRevertGUI();
                    break;
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();
            var importer = (JsonCustomImporter) target;
            var path = importer.assetPath;
            _guid = AssetDatabase.AssetPathToGUID(path);
            var jdbMetadata = AssetDatabase.LoadAssetAtPath<JdbMetadata>(path);
            _reflectionDrawer = new JdbReflectionDrawer(jdbMetadata, showFilenameInHeader: false, initialIndent: 0);
            Enum.TryParse(EditorPrefs.GetString("JdbMetadataInspector.Mode", nameof(Mode.Structure)), out _currentMode);
        }

        public override void OnDisable()
        {
            base.OnDisable();
            _reflectionDrawer = null;
        }

        enum Mode
        {
            Importer,
            Structure
        }
    }
}