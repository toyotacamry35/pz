using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Src.ResourcesSystem.Base;
using ResourcesSystem.Loader;
using Assets.Src.ResourceSystem;
using Core.Environment.Logging.Extension;
using EnumerableExtensions;
using NLog;
using UnityEditor;
using UnityEngine;

namespace L10n
{
    public class BaseResourcesBrowser : EditorWindow
    {
        private static readonly NLog.Logger Logger = LogManager.GetLogger("Default");
        private const float RepaintPeriod = 0.3f;
        private const float HeaderHgh = 50;
        private const float MinFoldoutHgh = 12;
        private const float ScrollsHeightRatio = .3f;
        private const int TypesColumnsCount = 5;

        private Type _defType;
        public string DefTypeName => _defType?.Name ?? "";
        private bool _isChangedType;
        private Type[] _baseResourceTypes;
        private string _typeFilter; //фильтр типов
        private int _shownTypesCount;

        private ILoader _loader;
        private GameResources _gameResources;
        private int _loadedCount;
        private int _loadedMaxCount = 1;
        private IEnumerator _loadingEnumerator;

        private bool _typesFoldout = true;
        private Vector2 _scrollPosHi;
        private bool _resourcesFoldout = true;
        private bool _unusedTyesFoldout = false;
        private Vector2 _scrollPosLo;

        private Dictionary<string, ResourceData> _allLoadedResources = new Dictionary<string, ResourceData>();
        private Dictionary<string, ResourceData> _allBrokenResources = new Dictionary<string, ResourceData>();
        private Dictionary<string, ResourceData> _shownResources = new Dictionary<string, ResourceData>();
        private List<string> _unusedTypes = new List<string>();

        private GUIStyle _titleNormalFoldoutStyle;
        private GUIStyle _titleRedFoldoutStyle;
        private bool _isBrokenResourcesShown;
        private float _lastRepaintTime;

        private string _resourcesFilter = "";
        private float _filteredCount;

        private static BaseResourcesBrowser _window;


        //=== Unity ===========================================================

        [MenuItem("Tools/BaseResources Browser", false, 9999)]
        private static void Init()
        {
            _window = (BaseResourcesBrowser) GetWindow(typeof(BaseResourcesBrowser));
            _window.Show();
            _window.titleContent = new GUIContent("BaseResourcesBrowser");
            _window.minSize = new Vector2(300, 200);
            _window.OnInit();
        }

        private void OnGUI()
        {
            if (_loadingEnumerator != null)
            {
                EditorGUILayout.LabelField($"Загрузка... {_loadedCount}/{_loadedMaxCount}");

                if (GUILayout.Button("Отмена", GUILayout.MaxWidth(150)))
                    StopLoading(true);

                return;
            }

            if (GUILayout.Button("Перечитать всё", GUILayout.MaxWidth(200)))
                OnInit();

            if (_isChangedType)
            {
                _isChangedType = false;
                UpdateResourcesByType(_defType);
            }

            EditorGUILayout.BeginHorizontal();
            _typesFoldout = EditorGUILayout.Foldout(_typesFoldout, "Типы:", true, _titleNormalFoldoutStyle);
            if (_typesFoldout)
            {
                EditorGUILayout.LabelField($"Фильтр ({_shownTypesCount}/{_baseResourceTypes?.Length ?? 0})", GUILayout.MaxWidth(100));
                _typeFilter = EditorGUILayout.TextField(_typeFilter);
            }

            EditorGUILayout.EndHorizontal();

            var scrollHgh = _typesFoldout ? _window.position.height - HeaderHgh : MinFoldoutHgh;
            if (_resourcesFoldout && _typesFoldout)
                scrollHgh *= ScrollsHeightRatio;
            _scrollPosHi = EditorGUILayout.BeginScrollView(_scrollPosHi, GUILayout.MaxHeight(scrollHgh));

            if (_typesFoldout)
            {
                EditorGUILayout.LabelField($"Выбран: {DefTypeName}");
                if (_baseResourceTypes != null)
                {
                    EditorGUILayout.BeginHorizontal();
                    _shownTypesCount = 0;
                    foreach (var type in _baseResourceTypes)
                    {
                        if (!type.Name.Contains(_typeFilter))
                            continue;

                        _shownTypesCount++;
                        bool isPressed = type.Name == DefTypeName
                            ? EditorGuiAdds.ColoredButton(Color.green, type.Name)
                            : GUILayout.Button(type.Name);
                        if (isPressed)
                        {
                            _defType = DefTypeName == type.Name ? null : GetDefType(type.Name);
                            _isChangedType = true;
                        }

                        if (_shownTypesCount % TypesColumnsCount == 0)
                        {
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                        }
                    }

                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.BeginHorizontal();
            _resourcesFoldout = EditorGUILayout.Foldout(
                _resourcesFoldout,
                _isBrokenResourcesShown ? "Ресурсы с ошибками:" : $"Ресурсы типа {DefTypeName}:",
                true,
                _isBrokenResourcesShown && _shownResources.Count > 0 ? _titleRedFoldoutStyle : _titleNormalFoldoutStyle);
            if (_resourcesFoldout)
            {
                _resourcesFilter = EditorGUILayout.TextField(
                    _resourcesFilter == "" || !_resourcesFoldout
                        ? $"Фильтр ({_shownResources.Count})"
                        : $"Фильтр ({_filteredCount}/{_shownResources.Count})",
                    _resourcesFilter);
            }

            EditorGUILayout.EndHorizontal();

            scrollHgh = (_resourcesFoldout || _unusedTyesFoldout) ? _window.position.height - HeaderHgh : MinFoldoutHgh * 2;
            if (_typesFoldout && (_resourcesFoldout || _unusedTyesFoldout))
                scrollHgh *= (1 - ScrollsHeightRatio);
            //TODOM Завести 1) 3-й скролл, 2) собрать скроллы и высоты в массиве, 3) делать пересчет высот скроллов при переключении foldout'ов

            _scrollPosLo = EditorGUILayout.BeginScrollView(_scrollPosLo, GUILayout.MaxHeight(scrollHgh));

            if (_resourcesFoldout)
            {
                _filteredCount = 0;
                foreach (var kvp in _shownResources)
                {
                    if (_resourcesFilter == "" || kvp.Key.Contains(_resourcesFilter))
                    {
                        _filteredCount++;
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.TextField(kvp.Key);
                        EditorGUILayout.ObjectField(kvp.Value.JdbMetadata, typeof(JdbMetadata), false);
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }

            if (_unusedTypes.Count > 0)
            {
                _unusedTyesFoldout = EditorGUILayout.Foldout(
                    _unusedTyesFoldout,
                    $"Типы без ресурсов ({_unusedTypes.Count}):",
                    true,
                    _titleNormalFoldoutStyle);

                if (_unusedTyesFoldout)
                {
                    foreach (var unusedType in _unusedTypes)
                        EditorGUILayout.LabelField(unusedType);
                }
            }

            EditorGUILayout.EndScrollView();
        }

        private void Update()
        {
            if (_loadingEnumerator != null)
            {
                if (!_loadingEnumerator.MoveNext())
                    StopLoading();

                if (Time.realtimeSinceStartup - _lastRepaintTime > RepaintPeriod)
                {
                    _lastRepaintTime = Time.realtimeSinceStartup;
                    Repaint();
                }
            }
        }


        //=== Private =========================================================

        private void OnInit()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            List<Type> baseResourceTypesList = new List<Type>();
            foreach (var assembly in assemblies)
            {
                Type[] assemblyTypes = null;
                try
                {
                    assemblyTypes = assembly.GetTypes().Where(t => typeof(BaseResource).IsAssignableFrom(t)).ToArray(); //одна из сборок падает на этом :(
                }
                catch (Exception e)
                {
                    Logger.IfWarn()?.Message($"Assembly '{assembly.FullName}' {e.Message}").Write();
                }

                if (assemblyTypes == null || assemblyTypes.Length == 0)
                    continue;

                baseResourceTypesList.AddRange(assemblyTypes);
                Logger.IfInfo()?.Message($"Assembly '{assembly.FullName}': {assemblyTypes.ItemsToStringByLines()}").Write();
            }

            _baseResourceTypes = baseResourceTypesList.ToArray();
            _defType = null;
            _typeFilter = "";
            _titleNormalFoldoutStyle = EditorGuiAdds.GetStyle(Color.black, true, EditorStyles.foldout);
            _titleRedFoldoutStyle = EditorGuiAdds.GetStyle(Color.red, true, EditorStyles.foldout);

            _loader = new FolderLoader(Application.dataPath);
            _gameResources = GameResourcesHolder.Instance;
            if (_gameResources.AssertIfNull(nameof(_gameResources)))
                return;

            _allLoadedResources.Clear();
            _unusedTypes.Clear();
            _loadingEnumerator = GetAllResourcesEnumerator();
            _lastRepaintTime = Time.realtimeSinceStartup;
        }

        private void StopLoading(bool earlyStop = false)
        {
            _loadingEnumerator = null;
            ExcludeUnusedTypes(earlyStop);
            Repaint();
        }

        private void ExcludeUnusedTypes(bool skipUnusedTypesFill)
        {
            List<Type> usedTypes = new List<Type>();
            foreach (var type in _baseResourceTypes) //исключаем типы, ресурсов которых нет в проекте
            {
                UpdateResourcesByType(type, true);
                if (_shownResources.Count > 0)
                    usedTypes.Add(type);
                else
                {
                    if (!skipUnusedTypesFill && !type.IsAbstract)
                        _unusedTypes.Add(type.NiceName());
                }
            }

            usedTypes.Sort((t1, t2) => t1.Name.CompareTo(t2.Name));
            _baseResourceTypes = usedTypes.ToArray();
            UpdateResourcesByType(_defType);
        }

        private IEnumerator GetAllResourcesEnumerator()
        {
            _allLoadedResources.Clear();
            var paths = _loader?.AllPossibleRoots?.ToList();
            if (paths != null)
            {
                _loadedMaxCount = paths.Count();
                _loadedMaxCount = _loader.AllPossibleRoots.Count();
                _loadedCount = 0;
                foreach (var path in _loader.AllPossibleRoots)
                {
                    var res = _gameResources.LoadResource<IResource>(path);
                    _loadedCount++;
                    if (res != null)
                        _allLoadedResources.Add(path, new ResourceData(path, _gameResources));
                    else
                        _allBrokenResources.Add(path, new ResourceData(path, _gameResources));

                    yield return null;
                }
            }
        }

        private void UpdateResourcesByType(Type type, bool speedMode = false)
        {
            _shownResources.Clear();
            _isBrokenResourcesShown = type == null;
            if (type != null)
            {
                foreach (var kvp in _allLoadedResources)
                {
                    var resourceData = kvp.Value;
                    if (resourceData?.BaseResource != null && resourceData.BaseResource.GetType() == type)
                    {
                        _shownResources.Add(kvp.Key, kvp.Value);
                        if (speedMode)
                            break;
                    }
                }
            }
            else
            {
                _shownResources = _allBrokenResources.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }
        }

        private Type GetDefType(string typeStr)
        {
            return _baseResourceTypes.FirstOrDefault(t => t.Name == typeStr);
        }
    }
}