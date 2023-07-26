//using System.Collections;
//using System.Linq;
//using Assets.Src.ResourcesSystem.Base;
//using ResourcesSystem.Loader;
//using Assets.Src.ResourceSystem;
//using Assets.Src.ResourceSystem.Editor;
//using NLog;
//using UnityEditor;
//using UnityEngine;
//
//namespace L10n.ConversionNs
//{
//    public class L10nConversionWindow : EditorWindow
//    {
//        private static readonly NLog.Logger Logger = LogManager.GetLogger("Localization");
//
//        private const string LocalizationConfigRelPath = "/locales/LocalizationsConfig";
//        public const string AllTypesName = "All";
//
//
//        private const float RepaintPeriod = 0.3f;
//        private const float HeaderHgh = 50;
//        private const float MinFoldoutHgh = 12;
//        private const float ScrollsHeightRatio = .3f;
//        private const int TypesColumnsCount = 3;
//        private const int ConvertButtonWdt = 130;
//
//        private string _typeStr;
//
//        private bool _isChangedType;
//
//        private IEnumerator _loadingEnumerator;
//
//        private bool _typesFoldout = true;
//        private Vector2 _scrollPosHi;
//        private bool _resourcesFoldout = true;
//        private Vector2 _scrollPosLo;
//
//        private GUIStyle _titleNormalFoldoutStyle;
//        private GUIStyle _titleRedFoldoutStyle;
//        private GUIStyle _titleLabelStyle;
//        private GUIStyle _updatedResourceTextStyle;
//
//        private float _lastRepaintTime;
//
//        private string _resourcesFilter = "";
//        private int _filteredCount;
//        private bool _isUnconvertedOnly;
//        private bool _isAllTypes;
//
//        private ConversionService _conversionService;
//        private string _loadingPathFilter = "";
//
//        private static L10nConversionWindow _window;
//
//
//        //=== Unity ===========================================================
//
//        [MenuItem("Localization/Old fields conversion window...", false, 31)]
//        private static void Init()
//        {
//            _window = (L10nConversionWindow) GetWindow(typeof(L10nConversionWindow));
//            _window.Show();
//            _window.titleContent = new GUIContent("OldFieldsConversionWindow");
//            _window.minSize = new Vector2(300, 200);
//            _window.OnInit();
//        }
//
//        private void OnGUI()
//        {
//            if (_loadingEnumerator != null)
//            {
//                EditorGUILayout.LabelField(
//                    $"Loading...{_conversionService.LoadedResourcesCount}/{_conversionService.LoadedResourcesMaxCount}");
//
//                EditorGUILayout.BeginHorizontal();
//
//                if (GUILayout.Button("Cancel", GUILayout.MaxWidth(100)))
//                    StopLoading();
//
//                if (_loadingPathFilter.Length > 0)
//                {
//                    EditorGUILayout.LabelField($"Partial loading: '{_loadingPathFilter}'");
//                    EditorGUILayout.Space();
//                }
//
//                EditorGUILayout.EndHorizontal();
//
//                return;
//            }
//
//            EditorGUILayout.BeginHorizontal();
//
//            if (GUILayout.Button("Reinit", GUILayout.MaxWidth(100)))
//                OnInit();
//
//            EditorGUILayout.LabelField("Partial loading path:", GUILayout.MaxWidth(ConvertButtonWdt));
//            _loadingPathFilter = EditorGUILayout.TextField(_loadingPathFilter, GUILayout.MaxWidth(ConvertButtonWdt * 2)).ToLower();
//
//            EditorGUILayout.EndHorizontal();
//
//            var oldTypeStr = _typeStr;
//            EditorGUILayout.BeginHorizontal();
//            EditorGUILayout.LabelField("Def name", GUILayout.MaxWidth(100));
//            _typeStr = EditorGUILayout.TextField(_typeStr);
//            if (oldTypeStr != _typeStr || _isChangedType)
//            {
//                _isChangedType = false;
//                _conversionService.SetCurrentDefType(_typeStr);
//                _conversionService.UpdateResourcesByType(_conversionService.CurrentDefType, _isAllTypes);
//            }
//
//            EditorGUILayout.LabelField(_conversionService.CurrentDefType?.ToString() ?? "none");
//            EditorGUILayout.EndHorizontal();
//
//            _typesFoldout = EditorGUILayout.Foldout(
//                _typesFoldout,
//                $"{nameof(BaseResource)} types: {_conversionService.UsedBaseResourceTypes?.Length ?? 0}," +
//                $"   resources: {_conversionService.ShownResources.Count}",
//                true,
//                _titleNormalFoldoutStyle);
//
//            var scrollHgh = _typesFoldout ? _window.position.height - HeaderHgh : MinFoldoutHgh;
//            if (_resourcesFoldout && _typesFoldout)
//                scrollHgh *= ScrollsHeightRatio;
//            _scrollPosHi = EditorGUILayout.BeginScrollView(_scrollPosHi, GUILayout.MaxHeight(scrollHgh));
//
//            //--- Types ---
//            if (_typesFoldout)
//            {
//                if (_conversionService.UsedBaseResourceTypes != null)
//                {
//                    //All types button
//                    bool isPressedAllTypesButton = AllTypesName == _typeStr
//                        ? EditorGuiAdds.ColoredButton(Color.gray, AllTypesName)
//                        : GUILayout.Button(AllTypesName);
//                    if (isPressedAllTypesButton)
//                    {
//                        _typeStr = _typeStr == AllTypesName ? "" : AllTypesName;
//                        _isAllTypes = _typeStr != "";
//                        _isChangedType = true;
//                    }
//
//                    EditorGUILayout.BeginHorizontal();
//                    int pos = 1;
//                    foreach (var type in _conversionService.UsedBaseResourceTypes)
//                    {
//                        bool isPressed = type.Name == _typeStr
//                            ? EditorGuiAdds.ColoredButton(Color.gray, type.Name)
//                            : GUILayout.Button(type.Name);
//                        if (isPressed)
//                        {
//                            _isAllTypes = false;
//                            _typeStr = _typeStr == type.Name ? "" : type.Name;
//                            _isChangedType = true;
//                        }
//
//                        if (pos % TypesColumnsCount == 0)
//                        {
//                            EditorGUILayout.EndHorizontal();
//                            EditorGUILayout.BeginHorizontal();
//                        }
//
//                        pos++;
//                    }
//
//                    EditorGUILayout.EndHorizontal();
//                }
//            }
//
//            EditorGUILayout.EndScrollView();
//
//            //--- Resources ---
//            EditorGUILayout.BeginHorizontal();
//            _resourcesFoldout = EditorGUILayout.Foldout(
//                _resourcesFoldout,
//                _conversionService.IsBrokenResourcesShown
//                    ? "Broken resources:"
//                    : $"Resources of type {_conversionService.CurrentDefType?.Name}:",
//                true,
//                _conversionService.IsBrokenResourcesShown && _conversionService.ShownResources.Count > 0
//                    ? _titleRedFoldoutStyle
//                    : _titleNormalFoldoutStyle);
//            _isUnconvertedOnly = EditorGUILayout.Toggle("Unconverted only", _isUnconvertedOnly);
//            _resourcesFilter = EditorGUILayout.TextField(
//                _conversionService.ShownResources.Count == _filteredCount || !_resourcesFoldout
//                    ? $"Filter ({_conversionService.ShownResources.Count})"
//                    : $"Filter ({_filteredCount}/{_conversionService.ShownResources.Count})",
//                _resourcesFilter);
//            EditorGUILayout.EndHorizontal();
//
//            var commonFieldWdt = (_window.position.width - ConvertButtonWdt - 20) / 2;
//            if (_resourcesFoldout && _conversionService.ShownResources.Count > 0)
//            {
//                //Заголовок
//                EditorGUILayout.BeginHorizontal();
//                EditorGUILayout.LabelField("Path", _titleLabelStyle, GUILayout.MaxWidth(commonFieldWdt));
//                EditorGUILayout.LabelField("Object", _titleLabelStyle, GUILayout.MaxWidth(commonFieldWdt));
//                if (GUILayout.Button("Convert all", GUILayout.MaxWidth(ConvertButtonWdt)))
//                {
//                    _conversionService.RunScanOrConversion(
//                        _conversionService.ShownResources
//                            .Where(kvp => kvp.Value.HasUnconvertedProps)
//                            .Select(kvp => kvp.Key)
//                            .ToArray(),
//                        true);
//                }
//
//                EditorGUILayout.EndHorizontal();
//            }
//
//            scrollHgh = _resourcesFoldout ? _window.position.height - HeaderHgh : MinFoldoutHgh;
//            if (_typesFoldout && _resourcesFoldout)
//                scrollHgh *= (1 - ScrollsHeightRatio);
//
//            _scrollPosLo = EditorGUILayout.BeginScrollView(_scrollPosLo, GUILayout.MaxHeight(scrollHgh));
//
//            if (_resourcesFoldout)
//            {
//                _filteredCount = 0;
//
//                //Записи
//                foreach (var kvp in _conversionService.ShownResources)
//                {
//                    if ((!_isUnconvertedOnly || kvp.Value.HasUnconvertedProps) &&
//                        (_resourcesFilter == "" || kvp.Key.Contains(_resourcesFilter)))
//                    {
//                        _filteredCount++;
//                        EditorGUILayout.BeginHorizontal();
//                        EditorGUILayout.TextField(kvp.Key,
//                            kvp.Value.IsConverted ? _updatedResourceTextStyle : EditorStyles.textField,
//                            GUILayout.MaxWidth(commonFieldWdt));
//                        EditorGUILayout.ObjectField(kvp.Value.JdbMetadata, typeof(JdbMetadata), false, GUILayout.MaxWidth(commonFieldWdt));
//                        if (kvp.Value.HasUnconvertedProps)
//                        {
//                            if (GUILayout.Button($"Convert ({kvp.Value.UnconvertedProps.Count})", GUILayout.MaxWidth(ConvertButtonWdt)))
//                            {
//                                _conversionService.RunScanOrConversion(new string[] {kvp.Key}, true);
//                            }
//                        }
//
//                        EditorGUILayout.EndHorizontal();
//                    }
//                }
//            }
//
//            EditorGUILayout.EndScrollView();
//        }
//
//        private void Update()
//        {
//            if (_loadingEnumerator != null)
//            {
//                if (!_loadingEnumerator.MoveNext())
//                    StopLoading();
//
//                if (Time.realtimeSinceStartup - _lastRepaintTime > RepaintPeriod)
//                {
//                    _lastRepaintTime = Time.realtimeSinceStartup;
//                    Repaint();
//                }
//            }
//        }
//
//
//        //=== Private =========================================================
//
//        private void OnInit()
//        {
//            _titleNormalFoldoutStyle = EditorGuiAdds.GetStyle(Color.black, true, EditorStyles.foldout);
//            _titleRedFoldoutStyle = EditorGuiAdds.GetStyle(Color.red, true, EditorStyles.foldout);
//            _titleLabelStyle = EditorGuiAdds.GetStyle(Color.black, true, EditorStyles.label);
//            _titleLabelStyle.alignment = TextAnchor.MiddleCenter;
//            _updatedResourceTextStyle = EditorGuiAdds.GetStyle(Color.black, true, EditorStyles.textField);
//
//            var loader = new FolderLoader(Application.dataPath);
//            var gameResources = new GameResources(loader);
//            gameResources.ConfigureForUnityEditor();
//            var localizationConfigDef = gameResources.LoadResource<LocalizationConfigDef>(LocalizationConfigRelPath);
//
//            _conversionService = new ConversionService(
//                Application.dataPath,
//                localizationConfigDef,
//                _loadingPathFilter.Length > 0
//                    ? loader.AllPossibleRoots.Where(e => e.ToLower().Contains(_loadingPathFilter)).ToList()
//                    : loader.AllPossibleRoots.ToList(),
//                gameResources);
//
//            _conversionService.SetCurrentDefType(_typeStr);
//
//            _loadingEnumerator = _conversionService.GetAllResourcesEnumerator();
//            _lastRepaintTime = Time.realtimeSinceStartup;
//        }
//
//        private void StopLoading()
//        {
//            _loadingEnumerator = null;
//            _conversionService.ExcludeUnusedTypes(_isAllTypes);
//            Repaint();
//        }
//    }
//}