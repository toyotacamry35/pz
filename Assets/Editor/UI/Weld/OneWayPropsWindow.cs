using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins.Editor.Weld
{
    class OneWayPropsWindow : EditorWindow
    {
        private const int UpdateButtonWdt = 100;
        private const int MidToggleFieldWdt = 80;
        private const int SmallButtonWdt = 18;
        private const int ObjectFieldWdt = 250;
        private const int NameFieldWdt = 210;
        private const int FirstLabelWdt = 30;
        private const int ShortToggleFieldWdt = 100;
        private const int FilterSecondLineSpace = 80;


        private string _hierarchyFilter = "";
        private string _vmFilter = "";
        private string _vmPropFilter = "";
        private string _targetFilter = "";
        private string _targetPropFilter = "";
        private List<OneWayPropsInfo> _infos;
        private bool _needForReinit;
        private Vector2 _scrollPosition;
        private int _shownCount;
        private bool _isFullProps;
        private GUIStyle _titleLabelStyle;
        private GUIStyle _titleLabelCenteredStyle;


        //=== Unity ===========================================================

        [MenuItem("Tools/UI/OneWayPropertyBinding browser")]
        private static void StaticInit()
        {
            var window = (OneWayPropsWindow) GetWindow(typeof(OneWayPropsWindow));
            window.titleContent = new GUIContent("OneWayProps");
            window.Init();
            window.Show();
            //window.minSize = new Vector2(300, 200);
        }

        private void OnGUI()
        {
            if (_infos == null || _needForReinit)
            {
                _needForReinit = false;
                Init();
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Update", GUILayout.MaxWidth(UpdateButtonWdt)))
            {
                _needForReinit = true;
                return;
            }

            _isFullProps = EditorGUILayout.ToggleLeft("Full names", _isFullProps, GUILayout.MaxWidth(ShortToggleFieldWdt));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal(); //Summary, filter
            var countInfo = _infos.Count == _shownCount ? _shownCount.ToString() : $"{_shownCount}/{_infos.Count}";
            EditorGUILayout.LabelField($"{nameof(OneWayPropertyBinding)} ({countInfo}):", _titleLabelStyle);
            EditorGUILayout.Space();
            ShowFilter("Vm", ref _vmFilter);
            ShowFilter("VmProp", ref _vmPropFilter);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            ShowFilter("Hier", ref _hierarchyFilter);
            ShowFilter("Trg", ref _targetFilter);
            ShowFilter("TrgProp", ref _targetPropFilter);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal(); //--- Заголовки
            EditorGUILayout.LabelField(" ", GUILayout.MaxWidth(FirstLabelWdt));
            EditorGUILayout.LabelField("Object", _titleLabelCenteredStyle, GUILayout.MinWidth(ObjectFieldWdt));
            EditorGUILayout.LabelField("ViewModel", _titleLabelCenteredStyle, GUILayout.MinWidth(NameFieldWdt));
            EditorGUILayout.LabelField("ViewModel property", _titleLabelCenteredStyle, GUILayout.MinWidth(NameFieldWdt));
            EditorGUILayout.LabelField("Target", _titleLabelCenteredStyle, GUILayout.MinWidth(NameFieldWdt));
            EditorGUILayout.LabelField("Target property", _titleLabelCenteredStyle, GUILayout.MinWidth(NameFieldWdt));
            EditorGUILayout.EndHorizontal();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            _shownCount = 0;
            for (int i = 0; i < _infos.Count; i++)
            {
                var info = _infos[i];
                var owpBinding = info.Component;
                if (owpBinding == null)
                {
                    _needForReinit = true;
                    continue;
                }

                if (IsFilteredOut(info.Path, _hierarchyFilter) ||
                    IsFilteredOut(_isFullProps ? info.FullVm : info.ShortVm, _vmFilter) ||
                    IsFilteredOut(_isFullProps ? owpBinding.viewModelPropertyName : info.ShortVmProp, _vmPropFilter) ||
                    IsFilteredOut(_isFullProps ? info.FullTarget : info.ShortTarget, _targetFilter) ||
                    IsFilteredOut(_isFullProps ? owpBinding.uiPropertyName : info.ShortTargetProp, _targetPropFilter))
                    continue;

                _shownCount++;

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField($"[{i}]", GUILayout.MaxWidth(FirstLabelWdt));
                EditorGUILayout.ObjectField(owpBinding, typeof(OneWayPropertyBinding), true, GUILayout.MinWidth(ObjectFieldWdt));
                EditorGUILayout.TextField(_isFullProps ? info.FullVm : info.ShortVm, GUILayout.MinWidth(NameFieldWdt));
                EditorGUILayout.TextField(_isFullProps ? owpBinding.viewModelPropertyName : info.ShortVmProp, GUILayout.MinWidth(NameFieldWdt));
                EditorGUILayout.TextField(_isFullProps ? info.FullTarget : info.ShortTarget, GUILayout.MinWidth(NameFieldWdt));
                EditorGUILayout.TextField(_isFullProps ? owpBinding.uiPropertyName : info.ShortTargetProp, GUILayout.MinWidth(NameFieldWdt));

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }


        //=== Private ==============================================================

        private void Init()
        {
            if (_hierarchyFilter == null)
                _hierarchyFilter = "";

            _titleLabelStyle = EditorGuiAdds.GetStyle(Color.black, true, EditorStyles.label);
            _titleLabelCenteredStyle = EditorGuiAdds.GetStyle(Color.black, true, EditorStyles.label);
            _titleLabelCenteredStyle.alignment = TextAnchor.MiddleCenter;
            _infos = SearchForComponents();
        }

        private List<OneWayPropsInfo> SearchForComponents()
        {
            var infos = new List<OneWayPropsInfo>();
            var components = GetAllComponentsOfScene<OneWayPropertyBinding>();
            foreach (var oneWayPropertyBinding in components)
            {
                infos.Add(new OneWayPropsInfo(oneWayPropertyBinding));
            }

            return infos;
        }

        private T[] GetAllComponentsOfScene<T>() where T : Component
        {
            var roots = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
            return roots.SelectMany(r => r.GetComponentsInChildren<T>(true)).ToArray();
        }

        private void ShowFilter(string filterTitle, ref string filterVar)
        {
            EditorGUILayout.LabelField($"{filterTitle} filter:", GUILayout.MaxWidth(MidToggleFieldWdt));
            filterVar = EditorGUILayout.TextField(filterVar, GUILayout.MaxWidth(NameFieldWdt)).ToLower();
            if (GUILayout.Button("X", EditorStyles.miniButton, GUILayout.MaxWidth(SmallButtonWdt)))
                filterVar = "";
        }

        private bool IsFilteredOut(string val, string filter)
        {
            return filter.Length > 0 && !val.ToLower().Contains(filter);
        }


        //=== Subclass ================================================================================================

        private class OneWayPropsInfo
        {
            public OneWayPropertyBinding Component;
            public string Path;
            public string ShortVm;
            public string FullVm;
            public string ShortVmProp;
            public string ShortTarget;
            public string FullTarget;
            public string ShortTargetProp;

            public OneWayPropsInfo(OneWayPropertyBinding component)
            {
                Component = component;
                Update();
            }

            public void Update()
            {
                Path = Component?.transform.FullName() ?? "";
                ShortVmProp = GetShortName(Component?.viewModelPropertyName);
                ShortTargetProp = GetShortName(Component?.uiPropertyName);
                (FullVm, ShortVm) = GetFullAndShortNames(Component?.viewModelPropertyName);
                (FullTarget, ShortTarget) = GetFullAndShortNames(Component?.uiPropertyName);
            }

            private (string, string) GetFullAndShortNames(string fullName)
            {
                if (string.IsNullOrEmpty(fullName))
                    return ("", "");

                var lastDotIndex = fullName.LastIndexOf(".", StringComparison.Ordinal);
                if (lastDotIndex < 0)
                    return (fullName, fullName);

                var fullVmName = fullName.Remove(lastDotIndex);
                return (fullVmName, GetShortName(fullVmName));
            }

            private string GetShortName(string fullName)
            {
                if (string.IsNullOrEmpty(fullName))
                    return "";

                var lastDotIndex = fullName.LastIndexOf(".", StringComparison.Ordinal);

                return lastDotIndex < 0
                    ? fullName
                    : fullName.Substring(lastDotIndex + 1);
            }
        }
    }
}