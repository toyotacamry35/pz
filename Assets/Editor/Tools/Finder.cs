using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Src.Tools.Editor
{
    public class Finder : EditorWindow
    {
        private const string DefaultPath = @".*\.prefab";
        private const float RepaintPeriod = 0.2f;

        private Filter _currentFilter;
        private IEnumerator _searchRoutine;
        private List<Object> _searchResult;
        private float _progressValue;
        private string _progressInfo;
        private float _lastRepaintTime;
        private Context _context = new Context();
        private GUIStyle _wrongTextFieldStyle;


        [MenuItem("Tools/Finder", false, 5)]
        static void MenuItem()
        {
            ShowWindow();
        }
        /*
        [MenuItem("Tools/StatAddNames")]
        static void StatAddNames()
        {
            string[] allfiles = Directory.GetFiles("Assets/UtilPrefabs/Stats", "*.jdb", SearchOption.AllDirectories);
            int c = 0;
            foreach (var fileStr in allfiles)
            {
                var lines = File.ReadAllText(fileStr);
                if (!lines.Contains("StatName") && lines.Contains("StatResource"))
                {
                    var name = Path.GetFileNameWithoutExtension(fileStr);
                    var i = lines.LastIndexOf("StatResource");
                    lines = lines.Substring(0, i) + "StatResource\", \n\t\"StatName\": \"" + name + "\"" + lines.Substring(i + 13);

                    //if (c == 0)
                    {
                        File.Delete(fileStr);
                        using (StreamWriter sw = File.CreateText(fileStr))
                        {
                            sw.Write(lines);
                        }

                        Debug.Log(fileStr);
                    }
                    c++;
                }
            }
        }*/

        static void ShowWindow()
        {
            var window = GetWindow<Finder>(true, "Finder");
            window.position = new Rect(100, 100, 500, 300);
            window.Init();
        }

        private bool SearchInProgress => _searchRoutine != null;

        public void Init()
        {
            _wrongTextFieldStyle = EditorGuiAdds.GetStyle(Color.red, false, EditorStyles.textField);
            _context = new Context();
        }

        private void OnGUI()
        {
            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    _currentFilter.Path = EditorGUILayout.TextField(new GUIContent("Path", "Поиск по имени файла (регулярное выражение)"),
                        !string.IsNullOrEmpty(_currentFilter.Path) ? _currentFilter.Path : DefaultPath);
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    _currentFilter.Name =
                        EditorGUILayout.TextField(new GUIContent("Name", "Поиск по имени объекта (регулярное выражение)"), _currentFilter.Name);
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    _currentFilter.UseLayer = EditorGUILayout.Toggle(new GUIContent("", "Учитывать слой"), _currentFilter.UseLayer, GUILayout.Width(14));
                    using (var changed = new EditorGUI.ChangeCheckScope())
                    {
                        _currentFilter.Layer = EditorGUILayout.LayerField(new GUIContent("Layer", "Учитывать слой"), _currentFilter.Layer);
                        if (changed.changed)
                            _currentFilter.UseLayer = true;
                    }
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    _currentFilter.UseTag = EditorGUILayout.Toggle(new GUIContent("", "Учитывать тэг"), _currentFilter.UseTag, GUILayout.Width(14));
                    using (var changed = new EditorGUI.ChangeCheckScope())
                    {
                        _currentFilter.Tag = EditorGUILayout.TagField(new GUIContent("Tag", "Учитывать тэг"), _currentFilter.Tag);
                        if (changed.changed)
                            _currentFilter.UseTag = true;
                    }
                }
                
                using (new EditorGUILayout.HorizontalScope())
                {
                    _currentFilter.UseComponent = EditorGUILayout.Toggle(
                        new GUIContent("", "Учитывать Компонент"), _currentFilter.UseComponent, GUILayout.Width(14));
                    using (var changed = new EditorGUI.ChangeCheckScope())
                    {
                        _currentFilter.Component = EditorGUILayout.TextField(new GUIContent("Component", "Учитывать Компонент"), _currentFilter.Component);
                    }
                }

                if (_currentFilter.UseComponent)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField("", GUILayout.Width(60));
                        using (var changed = new EditorGUI.ChangeCheckScope())
                        {
                            var style = _context?.IsWrongFieldName ?? false ? _wrongTextFieldStyle : EditorStyles.textField;
                            _currentFilter.ComponentFieldName = EditorGUILayout.TextField(
                                new GUIContent("Field/Prop. Name", "Имя поля/свойство Component"),
                                _currentFilter.ComponentFieldName,
                                style);
                            if (changed.changed)
                                _context?.Reset();
                        }

                        using (var changed = new EditorGUI.ChangeCheckScope())
                        {
                            _currentFilter.ComponentFieldNameMustBeNonEmpty = EditorGUILayout.ToggleLeft(
                                new GUIContent("Must be non-empty", "Значение должно быть не пустым"),
                                _currentFilter.ComponentFieldNameMustBeNonEmpty,
                                GUILayout.Width(130));
                        }
                    }
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    _currentFilter.UseReferencedObject = EditorGUILayout.Toggle(new GUIContent("", "Должен иметь ссылку на объект"),
                        _currentFilter.UseReferencedObject, GUILayout.Width(14));
                    using (var changed = new EditorGUI.ChangeCheckScope())
                    {
                        _currentFilter.ReferencedObject = EditorGUILayout.ObjectField(
                            new GUIContent("References object", "Должен иметь ссылку на объект"),
                            _currentFilter.ReferencedObject, typeof(Object), allowSceneObjects: false);
                    }
                }

                GUILayout.FlexibleSpace();
            }

            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
            {
                _currentFilter.DeepSearch =
                    EditorGUILayout.ToggleLeft(new GUIContent("Deep search", "Поиск по под-объектам префабов"), _currentFilter.DeepSearch);
                using (new EditorGUI.DisabledGroupScope(SearchInProgress))
                    if (GUILayout.Button("Find"))
                        StartSearch();
            }

            if (_searchResult != null)
                using (new EditorGUILayout.HorizontalScope(GUI.skin.box))
                {
                    EditorGUILayout.LabelField($"{_searchResult.Count} objects found");
                    using (new EditorGUI.DisabledGroupScope(SearchInProgress))
                    {
                        if (GUILayout.Button("Print", GUILayout.Width(70)))
                            LogResult(_searchResult);
                        if (GUILayout.Button("Select", GUILayout.Width(70)))
                            Selection.objects = _searchResult.ToArray();
                    }
                }

            if (SearchInProgress)
                if (EditorUtility.DisplayCancelableProgressBar("Searching", _progressInfo, _progressValue))
                    StopSearch();
        }

        private void Update()
        {
            if (SearchInProgress)
            {
                if (!_searchRoutine.MoveNext())
                    StopSearch();

                if (Time.realtimeSinceStartup - _lastRepaintTime > RepaintPeriod)
                {
                    _lastRepaintTime = Time.realtimeSinceStartup;
                    Repaint();
                }
            }
        }

        private void StartSearch()
        {
            _searchResult = new List<Object>();
            _progressValue = 0;
            _currentFilter.PathRegex = _currentFilter.Path != null ? new Regex(_currentFilter.Path) : null;
            _currentFilter.NameRegex = _currentFilter.Name != null ? new Regex(_currentFilter.Name) : null;
            _searchRoutine = Find(AssetDatabase.GetAllAssetPaths(), _currentFilter, _context, _searchResult, (p, i) =>
            {
                _progressValue = p;
                _progressInfo = i;
            });
        }

        private void StopSearch()
        {
            EditorUtility.ClearProgressBar();
            _searchRoutine = null;
            Repaint();
        }

        private static void LogResult(List<Object> result)
        {
            foreach (var @object in result)
                Debug.Log($"{AssetDatabase.GetAssetOrScenePath(@object)} '{@object?.name}'", @object);
        }

        private static IEnumerator Find(string[] pathes, Filter filter, Context context, List<Object> result, Action<float, string> progress)
        {
            int i = 0;
            foreach (var path in pathes)
            {
                ++i;
                if (filter.PathRegex != null && !filter.PathRegex.IsMatch(path))
                    continue;
                var @object = AssetDatabase.LoadAssetAtPath<Object>(path);
                Find(@object, filter, context, result);
                progress(i / (float) pathes.Length, path);
                yield return null;
            }
        }

        private static void Find(Object root, Filter filter, Context context, List<Object> result)
        {
            if (!root)
                return;

            if (Check(root, filter, context))
                result.Add(root);

            if (filter.DeepSearch)
            {
                var go = root as GameObject;
                var xform = go ? go.transform : root as Transform;
                if (xform)
                    foreach (Transform child in xform)
                        Find(child.gameObject, filter, context, result);
            }
        }

        public static bool Check(Object obj, Filter filter, Context context)
        {
            if (!obj)
                return false;
            if (filter.NameRegex != null && !filter.NameRegex.IsMatch(obj.name))
                return false;
            if (filter.UseLayer && !(obj is GameObject && (obj as GameObject).layer == filter.Layer))
                return false;
            if (filter.UseTag && !(obj is GameObject && (obj as GameObject).CompareTag(filter.Tag)))
                return false;
            if (filter.UseComponent)
            {
                var go = obj as GameObject;
                if (go == null)
                    return false;

                var comp = go.GetComponent(filter.Component);
                if (comp == null)
                    return false;

                if (!string.IsNullOrEmpty(filter.ComponentFieldName))
                {
                    var memberInfo = context.GetMemberInfo(comp, filter.ComponentFieldName);
                    if (memberInfo == null)
                        return false;

                    return GetMemberInfo(comp, memberInfo) != null == filter.ComponentFieldNameMustBeNonEmpty;
                }
            }

            if (filter.UseReferencedObject)
            {
                bool contains = false;
                var go = obj as GameObject;
                if (go != null && GameObjectReferences(go, filter))
                    contains = true;

                else if (AssetReferences(obj, filter.ReferencedObject))
                    contains = true;

                if (contains == false)
                    return false;
            }

            return true;
        }

        private static object GetMemberInfo(object obj, MemberInfo memberInfo)
        {
            if (obj == null || memberInfo == null)
                return null;

            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo) memberInfo).GetValue(obj);
                case MemberTypes.Property:
                    return ((PropertyInfo) memberInfo).GetValue(obj);
                default:
                    return null;
            }
        }

        private static bool GameObjectReferences(GameObject go, Filter filter)
        {
            var components = go.GetComponentsInChildren<Component>(includeInactive: true);
            foreach (var component in components)
                if (component != null && ComponentReferences(component, filter.ReferencedObject))
                    return true;
            return false;
        }

        private static bool ComponentReferences(Component component, Object referencedObject)
        {
            SerializedObject serializedObj = new SerializedObject(component);
            SerializedProperty prop = serializedObj.GetIterator();
            while (prop.NextVisible(true))
                if (prop.propertyType == SerializedPropertyType.ObjectReference &&
                    prop.objectReferenceValue != null &&
                    prop.objectReferenceValue == referencedObject)
                    return true;

            return false;
        }

        private static bool AssetReferences(Object obj, Object referencedObject)
        {
            SerializedObject serializedObj = new SerializedObject(obj);
            SerializedProperty prop = serializedObj.GetIterator();
            while (prop.NextVisible(true))
            {
                if (prop.propertyType == SerializedPropertyType.ObjectReference &&
                    prop.objectReferenceValue != null &&
                    prop.objectReferenceValue == referencedObject)
                    return true;
            }

            return false;
        }

        public class Context
        {
            private bool _isRequestedFieldInfo;
            private MemberInfo _memberInfo;

            public bool IsWrongFieldName => _isRequestedFieldInfo && _memberInfo == null;

            public MemberInfo GetMemberInfo(object obj, string fieldName)
            {
                if (obj == null)
                    return null;

                if (_isRequestedFieldInfo)
                    return _memberInfo;

                _isRequestedFieldInfo = true;
                var members = obj.GetType().GetMember(
                    fieldName,
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.GetField | BindingFlags.GetProperty | 
                    BindingFlags.FlattenHierarchy | BindingFlags.Instance);
                if (members.Length > 0)
                    _memberInfo = members[0];
                return _memberInfo;
            }

            public void Reset()
            {
                _isRequestedFieldInfo = false;
                _memberInfo = null;
            }
        }

        public struct Filter
        {
            public string Path;
            public Regex PathRegex;
            public string Name;
            public Regex NameRegex;
            public bool UseLayer;
            public LayerMask Layer;
            public bool UseTag;
            public string Tag;
            public bool UseComponent;
            public string Component;
            public string ComponentFieldName;
            public bool ComponentFieldNameMustBeNonEmpty;
            public bool UseReferencedObject;
            public Object ReferencedObject;
            public bool DeepSearch;
        }
    }
}