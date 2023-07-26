using System;
using System.Linq;
using System.Reflection;
using Assets.Src.ResourcesSystem.Base;
using Assets.Src.ResourceSystem;
using Assets.Src.ResourceSystem.L10n;
using ResourcesSystem.Loader;
using UnityEditor;
using UnityEngine;

namespace L10n
{
    [CustomPropertyDrawer(typeof(LocalizationKeyProp))]
    public class LocalizationKeyPropDrawer : PropertyDrawer
    {
        private const string JdbRefProp = "JdbRef";
        private const string JdbKeyProp = "JdbKey";

        private static bool IsGameResourcesAvailable => GameResourcesHolder.Instance != null && EditorGameResourcesForMonoBehaviours.NewGR != null;

        private GUIContent _jdbGuiContent1;
        private string[] _popupKeys;
        private int _popupIndex;
        private bool _isPopupsAvailable;
        private bool _jdbRefPropIsChanged;
        private bool _isGameResourcesReady;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight +
                   EditorGUIUtility.standardVerticalSpacing +
                   EditorGUI.GetPropertyHeight(property.FindPropertyRelative(JdbRefProp)) +
                   EditorGUI.GetPropertyHeight(property.FindPropertyRelative(JdbKeyProp));
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var jdbRefProp = property.FindPropertyRelative(JdbRefProp);
            var jdbKeyProp = property.FindPropertyRelative(JdbKeyProp);

            if (_jdbRefPropIsChanged || !_isGameResourcesReady)
            {
                _jdbRefPropIsChanged = false;
                _isGameResourcesReady = IsGameResourcesAvailable;

                _isPopupsAvailable = PopupDataInit(jdbRefProp, jdbKeyProp);
            }

            var propRect = new Rect(
                position.x,
                position.y,
                position.width,
                EditorGUIUtility.singleLineHeight
            );

            EditorGUI.LabelField(propRect, label);
            propRect.y += propRect.height;
            propRect.height = EditorGUI.GetPropertyHeight(jdbRefProp);
            propRect.width -= propRect.height;

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel += 1;

            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(propRect, jdbRefProp);
            _jdbRefPropIsChanged = EditorGUI.EndChangeCheck();

            propRect.x += propRect.width;
            propRect.width = propRect.height;
            var popupStyle = GUI.skin.FindStyle("IconButton");
            var popupIcon = EditorGUIUtility.IconContent("d_Refresh");
            if (GUI.Button(propRect, popupIcon, popupStyle))
                _jdbRefPropIsChanged = true;

            propRect.y += propRect.height + EditorGUIUtility.standardVerticalSpacing;
            propRect.x = position.x;
            propRect.width = position.width;
            propRect.height = EditorGUI.GetPropertyHeight(jdbKeyProp);
            if (_isPopupsAvailable)
            {
                var oldPopupIndex = _popupIndex;
                _popupIndex = EditorGUI.Popup(propRect, jdbKeyProp.displayName, _popupIndex, _popupKeys);
                if (oldPopupIndex != _popupIndex)
                    jdbKeyProp.stringValue = _popupKeys[_popupIndex];
            }
            else
                EditorGUI.PropertyField(propRect, jdbKeyProp);

            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }

        private bool PopupDataInit(SerializedProperty jdbRefProp, SerializedProperty jdbKeyProp)
        {
            if (!IsGameResourcesAvailable)
                return false;

            var defRef = (LocalizationKeysDefRef) GetTargetObjectOfProperty(jdbRefProp);
            if (defRef == null)
                return false;

            var res = EditorGameResourcesForMonoBehaviours.GetNew().LoadResource<LocalizationKeysDef>(((IResource) defRef.Target).Address);
            var localizedStrings = res?.LocalizedStrings;
            if (localizedStrings == null)
                return false;

            var keys = localizedStrings.Keys.Where(s => !string.IsNullOrEmpty(s)).ToList();
            if (keys.Count == 0)
                return false;

            keys.Insert(0, " ");
            _popupKeys = keys.ToArray();

            _popupIndex = 0;
            var isFoundKey = false;
            var currentJdbKey = jdbKeyProp.stringValue;
            for (var i = 0; i < _popupKeys.Length; i++)
                if (_popupKeys[i] == currentJdbKey)
                {
                    isFoundKey = true;
                    _popupIndex = i;
                    break;
                }

            if (!isFoundKey)
                jdbKeyProp.stringValue = _popupKeys[_popupIndex];

            return true;
        }

        private static object GetTargetObjectOfProperty(SerializedProperty prop)
        {
            if (prop == null) return null;

            var path = prop.propertyPath.Replace(".Array.data[", "[");
            object obj = prop.serializedObject.targetObject;
            var elements = path.Split('.');
            foreach (var element in elements)
                if (element.Contains("["))
                {
                    var elementName = element.Substring(0, element.IndexOf("[", StringComparison.Ordinal));
                    var index = Convert.ToInt32(
                        element.Substring(element.IndexOf("[", StringComparison.Ordinal)).Replace("[", "").Replace("]", ""));
                    obj = GetValue_Imp(obj, elementName, index);
                }
                else
                {
                    obj = GetValue_Imp(obj, element);
                }

            return obj;
        }

        private static object GetValue_Imp(object source, string name)
        {
            if (source == null)
                return null;
            var type = source.GetType();

            while (type != null)
            {
                var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (f != null)
                    return f.GetValue(source);

                var p = type.GetProperty(
                    name,
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (p != null)
                    return p.GetValue(source, null);

                type = type.BaseType;
            }

            return null;
        }

        private static object GetValue_Imp(object source, string name, int index)
        {
            var enumerable = GetValue_Imp(source, name) as System.Collections.IEnumerable;
            if (enumerable == null) return null;
            var enm = enumerable.GetEnumerator();

            for (var i = 0; i <= index; i++)
                if (!enm.MoveNext())
                    return null;
            return enm.Current;
        }
    }
}