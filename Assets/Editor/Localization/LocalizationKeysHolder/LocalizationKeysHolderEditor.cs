using System.Linq;
using ResourcesSystem.Loader;
using UnityEditor;
using UnityEngine;

namespace L10n
{
    [CustomEditor(typeof(LocalizationKeysHolder))]
    class LocalizationKeysHolderEditor : Editor
    {
        private SerializedProperty _defRefProp;

        private SerializedProperty _jdbKey1Prop;
        private SerializedProperty _jdbKey2Prop;
        private SerializedProperty _jdbKey3Prop;
        private SerializedProperty _jdbKey4Prop;

        private SerializedProperty _useKey2Prop;
        private SerializedProperty _useKey3Prop;
        private SerializedProperty _useKey4Prop;

        private int _popupIndex1;
        private int _popupIndex2;
        private int _popupIndex3;
        private int _popupIndex4;

        private const string JdbKeyLabelBase = "JdbKey";
        private const string UseKeyLabelBase = "Use ";
        private readonly string _jdbKeyLabel1 = JdbKeyLabelBase;
        private readonly string _jdbKeyLabel2 = JdbKeyLabelBase + 2;
        private readonly string _jdbKeyLabel3 = JdbKeyLabelBase + 3;
        private readonly string _jdbKeyLabel4 = JdbKeyLabelBase + 4;

        private readonly string _useKeyLabel2 = UseKeyLabelBase + JdbKeyLabelBase + 2;
        private readonly string _useKeyLabel3 = UseKeyLabelBase + JdbKeyLabelBase + 3;
        private readonly string _useKeyLabel4 = UseKeyLabelBase + JdbKeyLabelBase + 4;

        private GUIContent _jdbGuiContent1;
        private GUIContent _jdbGuiContent2;
        private GUIContent _jdbGuiContent3;
        private GUIContent _jdbGuiContent4;

        private GUIContent _useGuiContent2;
        private GUIContent _useGuiContent3;
        private GUIContent _useGuiContent4;

        private string[] _popupKeys;

        private bool _isGameResourcesAvailable;
        private bool _isPopupsAvailable;
        private bool _defRefPropIsChanged;


        //=== Unity ===============================================================

        private void OnEnable()
        {
            //Наступает, при новом показе инспектора
            _jdbGuiContent1 = new GUIContent(_jdbKeyLabel1);
            _jdbGuiContent2 = new GUIContent(_jdbKeyLabel2);
            _jdbGuiContent3 = new GUIContent(_jdbKeyLabel3);
            _jdbGuiContent4 = new GUIContent(_jdbKeyLabel4);

            _useGuiContent2 = new GUIContent(_useKeyLabel2);
            _useGuiContent3 = new GUIContent(_useKeyLabel3);
            _useGuiContent4 = new GUIContent(_useKeyLabel4);

            _defRefProp = serializedObject.FindProperty(nameof(LocalizationKeysHolder.JdbRef));

            _jdbKey1Prop = serializedObject.FindProperty(nameof(LocalizationKeysHolder.JdbKey1));
            _jdbKey2Prop = serializedObject.FindProperty(nameof(LocalizationKeysHolder.JdbKey2));
            _jdbKey3Prop = serializedObject.FindProperty(nameof(LocalizationKeysHolder.JdbKey3));
            _jdbKey4Prop = serializedObject.FindProperty(nameof(LocalizationKeysHolder.JdbKey4));

            _useKey2Prop = serializedObject.FindProperty(nameof(LocalizationKeysHolder.UseKey2));
            _useKey3Prop = serializedObject.FindProperty(nameof(LocalizationKeysHolder.UseKey3));
            _useKey4Prop = serializedObject.FindProperty(nameof(LocalizationKeysHolder.UseKey4));

            _isGameResourcesAvailable = GameResourcesHolder.Instance != null;

            if (_isGameResourcesAvailable)
            {
                _isPopupsAvailable = PopupDataInit();
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();

            if (!_isGameResourcesAvailable)
            {
                _isGameResourcesAvailable = GameResourcesHolder.Instance != null;
                if (_isGameResourcesAvailable)
                {
                    _isPopupsAvailable = PopupDataInit();
                }
            }

            if (_isGameResourcesAvailable && _defRefPropIsChanged)
            {
                _defRefPropIsChanged = false;
                _isPopupsAvailable = PopupDataInit();
            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_defRefProp, new GUIContent("DefRef"));
            if (EditorGUI.EndChangeCheck())
            {
                _defRefPropIsChanged = true;
            }

            if (_isGameResourcesAvailable && _isPopupsAvailable)
                ShowJdbKeyAsPopup(_jdbKeyLabel1, _jdbKey1Prop, ref _popupIndex1);
            else
                EditorGUILayout.PropertyField(_jdbKey1Prop, _jdbGuiContent1);

            EditorGUILayout.PropertyField(_useKey2Prop, _useGuiContent2);

            if (_useKey2Prop.boolValue)
            {
                if (_isGameResourcesAvailable && _isPopupsAvailable)
                    ShowJdbKeyAsPopup(_jdbKeyLabel2, _jdbKey2Prop, ref _popupIndex2);
                else
                    EditorGUILayout.PropertyField(_jdbKey2Prop, _jdbGuiContent2);

                EditorGUILayout.PropertyField(_useKey3Prop, _useGuiContent3);

                if (_useKey3Prop.boolValue)
                {
                    if (_isGameResourcesAvailable && _isPopupsAvailable)
                        ShowJdbKeyAsPopup(_jdbKeyLabel3, _jdbKey3Prop, ref _popupIndex3);
                    else
                        EditorGUILayout.PropertyField(_jdbKey3Prop, _jdbGuiContent3);

                    EditorGUILayout.PropertyField(_useKey4Prop, _useGuiContent4);

                    if (_useKey4Prop.boolValue)
                    {
                        if (_isGameResourcesAvailable && _isPopupsAvailable)
                            ShowJdbKeyAsPopup(_jdbKeyLabel4, _jdbKey4Prop, ref _popupIndex4);
                        else
                            EditorGUILayout.PropertyField(_jdbKey4Prop, _jdbGuiContent4);
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }


        //=== Private ==========================================================

        private void ShowJdbKeyAsPopup(string fieldName, SerializedProperty jdbKeyProp, ref int popupIndex)
        {
            var oldPopupIndex = popupIndex;
            popupIndex = EditorGUILayout.Popup("JdbKey", popupIndex, _popupKeys);
            if (oldPopupIndex != popupIndex)
            {
                jdbKeyProp.stringValue = _popupKeys[popupIndex];
            }
        }

        private bool PopupDataInit()
        {
            var localizationKeysHolder = (LocalizationKeysHolder) _defRefProp.serializedObject.targetObject;
            var defWrapper = localizationKeysHolder.JdbRef?.AlwaysFreshTarget;

            var localizedStrings = defWrapper?.Resource?.LocalizedStrings;
            if (localizedStrings == null)
                return false;

            //_txt2 =$"count={localizedStrings.Count} {DateTime.Now:HH:mm:ss}"; //2del
            var keys = localizedStrings.Keys.Where(k => !string.IsNullOrEmpty(k)).ToList();
            if (keys.Count == 0)
                return false;

            keys.Insert(0, " ");
            _popupKeys = keys.ToArray();

            SetPopupIndex(_jdbKey1Prop, out _popupIndex1);
            SetPopupIndex(_jdbKey2Prop, out _popupIndex2);
            SetPopupIndex(_jdbKey3Prop, out _popupIndex3);
            SetPopupIndex(_jdbKey4Prop, out _popupIndex4);

            return true;
        }

        private void SetPopupIndex(SerializedProperty jdbKeyProp, out int popupIndex)
        {
            popupIndex = 0;
            var isFoundKey = false;
            var currentJdbKey = jdbKeyProp.stringValue;
            for (int i = 0; i < _popupKeys.Length; i++)
            {
                if (_popupKeys[i] == currentJdbKey)
                {
                    isFoundKey = true;
                    popupIndex = i;
                    break;
                }
            }

            if (!isFoundKey)
                jdbKeyProp.stringValue = _popupKeys[popupIndex];
        }
    }
}