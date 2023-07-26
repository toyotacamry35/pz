using System.Collections.Generic;
using System.Linq;
using EnumerableExtensions;
using UnityEditor;
using UnityEngine;
using Assets.Src.ResourceSystem;

namespace L10n.KeysExtractionNs
{
    public class ReportWindow : EditorWindow
    {
        private Vector2 _scrollPosition;
        private GUIStyle _titleNormalFoldoutStyle;
        private GUIStyle _greenTextStyle;

        private bool _reportFoldout = true;
        private string _text;

        private bool _keysFoldout;

        private KeysExtractionService.Results _results;
        private Dictionary<LocalizedString, KeyUsages> _filteredOutKeys = new Dictionary<LocalizedString, KeyUsages>();
        private string _keyFilter = "";

        public static void ShowWindow(string title, KeysExtractionService.Results results)
        {
            var window = (ReportWindow) GetWindow(typeof(ReportWindow));
            window.Show();
            window.titleContent = new GUIContent(title);
            window.minSize = new Vector2(100, 100);
            window._text = results.AllMessages.ItemsToStringByLines(true);
            window.OnInit(results);
        }


        //=== Unity ===========================================================

        private void OnGUI()
        {
            if (string.IsNullOrEmpty(_text))
                return;

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            _keysFoldout = EditorGUILayout.Foldout(_keysFoldout, "Использование ключей:", true, _titleNormalFoldoutStyle);
            if (_keysFoldout)
            {
                EditorGUILayout.BeginHorizontal();

                _keyFilter = EditorGUILayout.TextField(
                    $"Фильтр ({_filteredOutKeys.Count}/{_results.DevTranslations?.Count ?? 0}):",
                    _keyFilter,
                    GUILayout.MaxWidth(400));
                if (GUILayout.Button("Найти", GUILayout.MaxWidth(200)))
                {
                    FindLinks(_keyFilter, _results, _filteredOutKeys);
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();

                foreach (var kvp in _filteredOutKeys)
                {
                    //-- Описание ключа
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(kvp.Key.Key, GUILayout.MaxWidth(100));
                    EditorGUILayout.TextField(kvp.Key.TranslationData?.Text ?? "", _greenTextStyle);
                    EditorGUILayout.ToggleLeft("IsPlural", kvp.Key.TranslationData?.IsPlural ?? false, GUILayout.MaxWidth(60));
                    EditorGUILayout.ToggleLeft("IsProtected", kvp.Key.TranslationData?.IsProtected ?? false, GUILayout.MaxWidth(80));
                    EditorGUILayout.EndHorizontal();

                    var keyUsage = kvp.Value;
                    EditorGUILayout.BeginVertical();

                    //-- Ссылки на ресурсы
                    foreach (var resourceData in keyUsage.LinksFromResources)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("", GUILayout.MaxWidth(100));
                        EditorGUILayout.TextField(resourceData.HierPath, GUILayout.MaxWidth(500));
                        EditorGUILayout.ObjectField(resourceData.JdbMetadata, typeof(JdbMetadata), false);
                        EditorGUILayout.EndHorizontal();
                    }

                    if (keyUsage.LinksFromKeys.Count > 0)
                        EditorGUILayout.LabelField("      В тексте других ключей:");

                    //-- Ссылки на ключи
                    foreach (var tdeKvp in keyUsage.LinksFromKeys)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("", GUILayout.MaxWidth(100));
                        EditorGUILayout.LabelField(tdeKvp.Key, GUILayout.MaxWidth(100));
                        EditorGUILayout.TextField(tdeKvp.Value.Text);
                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space();
                }
            }

            _reportFoldout = EditorGUILayout.Foldout(_reportFoldout, "Отчет:", true, _titleNormalFoldoutStyle);
            if (_reportFoldout)
            {
                EditorGUILayout.TextArea(_text);
            }

            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("Close"))
                Close();
        }


        //=== Private ==============================================================

        private void OnInit(KeysExtractionService.Results results)
        {
            _titleNormalFoldoutStyle = EditorGuiAdds.GetStyle(Color.black, true, EditorStyles.foldout);
            _greenTextStyle = EditorGuiAdds.GetStyle(Color.grey, false, EditorStyles.textField);
            _results = results;
        }

        private void FindLinks(string keyFilter, KeysExtractionService.Results results, Dictionary<LocalizedString, KeyUsages> filteredOutKeys)
        {
            filteredOutKeys.Clear();
            if (string.IsNullOrEmpty(_keyFilter))
                return;

            foreach (var key in results.DevTranslations.Keys)
            {
                if (!key.Contains(keyFilter))
                    continue;

                var keyUsages = new KeyUsages();
                if (!results.DevTranslations.TryGetValue(key, out var translationDataExt))
                    continue;

                filteredOutKeys.Add(new LocalizedString(key, translationDataExt), keyUsages);
                //Заполнение LinksFromKeys
                foreach (var translationKvp in results.DevTranslations)
                {
                    if (translationKvp.Key != key && translationKvp.Value.Text.Contains(key))
                        keyUsages.LinksFromKeys.Add(translationKvp.Key, translationKvp.Value);
                }

                foreach (var lsMeta in results.NewLocalizedStrings)
                {
                    if (LocalizedString.IsKey(lsMeta.LocalizedString))
                    {
                        if (lsMeta.LocalizedString.Key == key) //ссылка на ключ
                            keyUsages.LinksFromResources.Add(new ResourceData(lsMeta.JdbRelPath, results.GameResources, lsMeta.HierPath));
                    }
                    else
                    {
                        if (lsMeta.LocalizedString.Key.Contains(key)) //если текст еще не извлечен, то имеет смысл искать и в нем ссылки внутри текста
                            keyUsages.LinksFromResources.Add(new ResourceData(lsMeta.JdbRelPath, results.GameResources, lsMeta.HierPath));
                    }
                }

                foreach (var lsMeta in results.UsingLocalizedStrings) //это исключительно ссылки на имена ключей
                {
                    if (lsMeta.LocalizedString.Key == key) //ссылка на ключ
                        keyUsages.LinksFromResources.Add(new ResourceData(lsMeta.JdbRelPath, results.GameResources, lsMeta.HierPath));
                }
            }
        }


        //=== Classs ==========================================================

        private class KeyUsages
        {
            public List<ResourceData> LinksFromResources { get; } = new List<ResourceData>();
            public Dictionary<string, TranslationDataExt> LinksFromKeys { get; } = new Dictionary<string, TranslationDataExt>();
        }
    }
}