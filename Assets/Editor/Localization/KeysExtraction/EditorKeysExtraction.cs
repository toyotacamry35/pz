using System.Linq;
using ResourcesSystem.Loader;
using Assets.Src.ResourceSystem.Editor;
using JetBrains.Annotations;
using NLog;
using UnityEditor;
using UnityEngine;

namespace L10n.KeysExtractionNs
{
    public static class EditorKeysExtraction
    {
        public const string MenuLocalizationName = "Localization";
        private const string MenuExtractionPath = MenuLocalizationName + "/Работа с ключами";

        private static readonly NLog.Logger Logger = LogManager.GetLogger("UI");

        [MenuItem(MenuExtractionPath, false, 11)]
        [MenuItem(MenuExtractionPath + "/Сканировать", false, 1)]
        [UsedImplicitly]
        private static void ScanOnly()
        {
            Do(JdbKeysExtractor.Operation.ScanOnly);
        }

        [MenuItem(MenuExtractionPath, false, 11)]
        [MenuItem(MenuExtractionPath + "/Извлечь ключи", false, 2)]
        [UsedImplicitly]
        private static void ExtractKeys()
        {
            Do(JdbKeysExtractor.Operation.KeysExtraction);
        }

        [MenuItem(MenuExtractionPath, false, 11)]
        [MenuItem(MenuExtractionPath + "/Удалить неиспользуемые ключи", false, 3)]
        [UsedImplicitly]
        private static void DeleteUnusedKeys()
        {
            Do(JdbKeysExtractor.Operation.DeleteUnusedKeys);
        }

        [MenuItem(MenuExtractionPath, false, 11)]
        [MenuItem(MenuExtractionPath + "/Обновить Comments dev-словаря (о применении ключей)", false, 4)]
        [UsedImplicitly]
        private static void UpdateExtractionComments()
        {
            Do(JdbKeysExtractor.Operation.UpdateExtractionComments);
        }

        /// <summary>
        /// Если обнаруживаем переводы с версией выше чем у develop, опускаем до develop
        /// </summary>
        [MenuItem(MenuExtractionPath, false, 11)]
        [MenuItem(MenuExtractionPath + "/Исправить версии переводов", false, 6)]
        [UsedImplicitly]
        private static void RecordVersionsAutofix()
        {
            Do(JdbKeysExtractor.Operation.RecordVersionsAutofix);
        }

        [MenuItem(MenuExtractionPath, false, 11)]
        [MenuItem(MenuExtractionPath + "/Пересчитать хешкоды", false, 7)]
        [UsedImplicitly]
        private static void HashesRecalculation()
        {
            Do(JdbKeysExtractor.Operation.HashesRecalculation);
        }

        private static void Do(JdbKeysExtractor.Operation operation)
        {
            var loader = new FolderLoader(Application.dataPath);
            var gameResources = new GameResources(loader);
            gameResources.ConfigureForUnityEditor();

            var extractionService = new KeysExtractionService(Application.dataPath, loader.AllPossibleRoots.ToList(), gameResources);
            var results = extractionService.RunOperation(operation);
            ReportWindow.ShowWindow(operation.ToString(), results);
        }
    }
}