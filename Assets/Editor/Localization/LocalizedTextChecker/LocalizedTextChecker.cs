using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityWeld.Binding;

namespace L10n
{
    public class LocalizedTextChecker : EditorWindow
    {
        private const int UpdateButtonWdt = 100;
        private const int AddButtonWdt = 70;
        private const int NonLocalizedButtonWdt = 140;
        private const int FirstLabelWdt = 30;
        private const int ObjectFieldWdt = 250;
        private const int ShortToggleFieldWdt = 60;
        private const int MidToggleFieldWdt = 80;
        private const int LongToggleFieldWdt = 110;
        private const int FontSizeFieldWdt = 22;
        private const int SmallButtonWdt = 18;

        private List<TextMeshProInfo> _infos = new List<TextMeshProInfo>();
        private bool _needForReinit;
        private Vector2 _scrollPosition;
        private bool _showFontInfo;
        private bool _needSpaceForButtons = true;
        private TMP_FontAsset _selectedFontAsset;
        private bool _showOnlyComponentsWithSelectedFont;
        private bool _showNonLocalized = true;
        private bool _showAdditionalRow;
        private int _shownCount;
        private string _pathFilter = "";

        private GUIStyle _selectedFontLabelStyle;
        private GUIStyle _nonLocalizedLabelStyle;
        private GUIStyle _nonLocalizedTextStyle;
        private GUIStyle _setLocalizedButtonStyle;
        private GUIStyle _titleLabelStyle;


        //=== Unity ===============================================================

        [MenuItem("Localization/UI-scenes. LocalizedText checker window...", false, 100)]
        private static void Do()
        {
            var window = (LocalizedTextChecker) GetWindow(typeof(LocalizedTextChecker));
            window.Init();
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal(); //First string, options
            if (GUILayout.Button("Update", GUILayout.MaxWidth(UpdateButtonWdt)))
            {
                _needForReinit = true;
            }

            EditorGUILayout.Space();
            _showAdditionalRow = EditorGUILayout.ToggleLeft("2 lines", _showAdditionalRow, GUILayout.MaxWidth(ShortToggleFieldWdt));
            _showNonLocalized = EditorGUILayout.ToggleLeft("NonLocalized", _showNonLocalized, GUILayout.MaxWidth(LongToggleFieldWdt));
            _showFontInfo = EditorGUILayout.ToggleLeft("Show font info", _showFontInfo, GUILayout.MaxWidth(LongToggleFieldWdt));
            if (_showFontInfo)
            {
                EditorGUILayout.LabelField("Select font", GUILayout.MaxWidth(MidToggleFieldWdt));
                _selectedFontAsset = (TMP_FontAsset) EditorGUILayout.ObjectField(_selectedFontAsset, typeof(TMP_FontAsset), false,
                    GUILayout.MaxWidth(ObjectFieldWdt));
                _showOnlyComponentsWithSelectedFont = EditorGUILayout.ToggleLeft("Only this font", _showOnlyComponentsWithSelectedFont);
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal(); //Summary, filter

            var countInfo = _infos.Count == _shownCount ? _shownCount.ToString() : $"{_shownCount}/{_infos.Count}";
            EditorGUILayout.LabelField($"<{nameof(LocalizedText)}> components ({countInfo}):", _titleLabelStyle);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Filter:", GUILayout.MaxWidth(MidToggleFieldWdt));
            _pathFilter = EditorGUILayout.TextField(_pathFilter, GUILayout.MaxWidth(ObjectFieldWdt));
            if (GUILayout.Button("X", EditorStyles.miniButton, GUILayout.MaxWidth(SmallButtonWdt)))
                _pathFilter = "";

            EditorGUILayout.EndHorizontal();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            EditorGUILayout.Space();

            _shownCount = 0;
            var needSpaceForButtons = false;
            for (int i = 0; i < _infos.Count; i++)
            {
                var tmpInfo = _infos[i];
                var tmpUgui = tmpInfo.TmpUgui;
                if (tmpUgui == null)
                    _needForReinit = true;

                if (_pathFilter.Length > 0 && !tmpInfo.Path.Contains(_pathFilter))
                    continue;

                if (_showOnlyComponentsWithSelectedFont && (tmpUgui == null || tmpUgui.font != _selectedFontAsset))
                    continue;

                if (!_showNonLocalized && tmpInfo.IsNonLocalized)
                    continue;

                _shownCount++;

                if (_showAdditionalRow)
                {
                    EditorGUILayout.Space();
                    EditorGuiAdds.HorizontalLine(Color.black);

                    EditorGUILayout.BeginHorizontal(); //--- Additional row

                    EditorGUILayout.LabelField(tmpInfo.Path, GetLabelStyle(tmpInfo, _selectedFontAsset));

                    if (GUILayout.Button(
                        tmpInfo.IsNonLocalized ? "Remove NonLocalized" : "Set NonLocalized",
                        tmpInfo.IsNonLocalized ? EditorStyles.miniButton : _setLocalizedButtonStyle,
                        GUILayout.MaxWidth(NonLocalizedButtonWdt)))
                    {
                        if (tmpInfo.IsNonLocalized)
                        {
                            DestroyImmediate(tmpInfo.NonLocalized);
                        }
                        else
                        {
                            if (tmpUgui != null)
                                tmpUgui.gameObject.AddComponent<NonLocalized>();
                        }

                        tmpInfo.Update();
                    }

                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.BeginHorizontal(); //--- Main row

                EditorGUILayout.LabelField($"[{i}]", GetLabelStyle(tmpInfo, _selectedFontAsset), GUILayout.MaxWidth(FirstLabelWdt));
                EditorGUILayout.ObjectField(tmpUgui, typeof(TextMeshProUGUI), true, GUILayout.MinWidth(ObjectFieldWdt));
                EditorGUILayout.TextField(tmpUgui != null ? tmpUgui.text : "",
                    tmpInfo.IsNonLocalized ? _nonLocalizedTextStyle : EditorStyles.textField);
                if (!tmpInfo.HasLocalizedText && !tmpInfo.IsNonLocalized)
                {
                    needSpaceForButtons = true;
                    if (GUILayout.Button("Add LTMP", GUILayout.MaxWidth(AddButtonWdt)))
                    {
                        if (tmpUgui != null)
                        {
                            var localizedTextMeshPro = tmpUgui.gameObject.AddComponent<LocalizedTextMeshPro>();
                            localizedTextMeshPro.Target = tmpUgui;
                            var oneWayPropertyBinding = tmpUgui.gameObject.AddComponent<OneWayPropertyBinding>();
                            oneWayPropertyBinding.uiPropertyName =
                                $"{typeof(LocalizedTextMeshPro).FullName}.{nameof(LocalizedTextMeshPro.LocalizedString)}";
                            tmpInfo.Update();
                        }
                    }
                }
                else
                {
                    if (_needSpaceForButtons)
                        EditorGUILayout.LabelField("", GUILayout.MaxWidth(AddButtonWdt));
                }

                if (_showFontInfo)
                {
                    var fontSize = EditorGUILayout.FloatField(
                        tmpUgui != null ? tmpUgui.fontSize : 0, GUILayout.MaxWidth(FontSizeFieldWdt));
                    if (tmpUgui != null && !Mathf.Approximately(tmpUgui.fontSize, fontSize))
                        tmpUgui.fontSize = fontSize;

                    var fontAsset = (TMP_FontAsset) EditorGUILayout.ObjectField(tmpUgui.font, typeof(TMP_FontAsset), false,
                        GUILayout.MinWidth(ObjectFieldWdt));

                    if (tmpUgui != null && fontAsset != tmpUgui.font)
                        tmpUgui.font = fontAsset;
                }

                EditorGUILayout.EndHorizontal();
            }

            _needSpaceForButtons = needSpaceForButtons;
            EditorGUILayout.EndScrollView();

            if (_needForReinit)
            {
                _needForReinit = false;
                Init();
            }
        }


        //=== Private =============================================================

        private void Init()
        {
            _selectedFontLabelStyle = EditorGuiAdds.GetStyle(Color.red, true, EditorStyles.label);
            _nonLocalizedLabelStyle = EditorGuiAdds.GetStyle(Color.gray, false, EditorStyles.label);
            _titleLabelStyle = EditorGuiAdds.GetStyle(Color.black, true, EditorStyles.label);
            _nonLocalizedTextStyle = EditorGuiAdds.GetStyle(Color.gray, false, EditorStyles.textField);
            _setLocalizedButtonStyle = EditorGuiAdds.GetStyle(Color.gray, false, EditorStyles.miniButton);
            var tmpComponents = GetAllComponentsOfScene<TextMeshProUGUI>();
            _infos.Clear();
            _needSpaceForButtons = false;
            if (tmpComponents != null)
            {
                foreach (var tmpUgui in tmpComponents)
                {
                    var tmpInfo = new TextMeshProInfo(tmpUgui);
                    if (!_needSpaceForButtons && tmpInfo.LocalizedText == null)
                        _needSpaceForButtons = true;
                    _infos.Add(tmpInfo);
                }
            }
        }

        private T[] GetAllComponentsOfScene<T>() where T : Component
        {
            var roots = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
            return roots.SelectMany(r => r.GetComponentsInChildren<T>(true)).ToArray();
        }

        private GUIStyle GetLabelStyle(TextMeshProInfo tmpInfo, TMP_FontAsset selectedFontAsset)
        {
            if (tmpInfo.TmpUgui != null && tmpInfo.TmpUgui.font == selectedFontAsset)
                return _selectedFontLabelStyle;

            return tmpInfo.IsNonLocalized ? _nonLocalizedLabelStyle : EditorStyles.label;
        }


        //=== Class =======================================================================================================

        private class TextMeshProInfo
        {
            //=== Props ===========================================================

            public TextMeshProUGUI TmpUgui { get; }

            public LocalizedText LocalizedText { get; private set; }

            public NonLocalized NonLocalized { get; private set; }

            public string Path { get; private set; }

            public bool IsNonLocalized => NonLocalized != null;

            public bool HasLocalizedText => LocalizedText != null;


            //=== Ctor ============================================================

            public TextMeshProInfo(TextMeshProUGUI tmpUgui)
            {
                TmpUgui = tmpUgui;
                Update();
            }


            //=== Public ==========================================================

            public void Update()
            {
                if (TmpUgui != null)
                {
                    Path = TmpUgui.transform.FullName();
                    LocalizedText = TmpUgui.GetComponent<LocalizedText>();
                    NonLocalized = TmpUgui.GetComponent<NonLocalized>();
                }
                else
                {
                    Path = "";
                    LocalizedText = null;
                    NonLocalized = null;
                }
            }
        }
    }
}