using System.IO;
using ResourcesSystem.Loader;
using Assets.Src.ResourceSystem.Editor;
using L10n.KeysExtractionNs;
using NLog;
using Uins;
using UnityEditor;
using UnityEngine;

namespace L10n.XlsConverterNs
{
    public static class EditorXlsConversion
    {
        private const string MenuEiPath = EditorKeysExtraction.MenuLocalizationName + "/Экспорт, импорт";
        private const string Menu_DevRuOps_Path = MenuEiPath + "/Операции dev-RU";
        private const string Menu_RuEnXOps_Path = MenuEiPath + "/Операции RU-EN-X";

        private const string MenuCommandExportDiff = "/Экспорт (не переведенное)";
        private const string MenuCommandExportAll = "/Экспорт (всё)";
        private const string MenuCommandImport = "/Импорт";
        private const string MenuCommandExportSummary = "/Экспорт в сводную таблицу";

        private static readonly NLog.Logger Logger = LogManager.GetLogger("Localization");

        private const string XlsFolderRelPath = "/../!Build/xls";

        [MenuItem(MenuEiPath, false, 21)]
        [MenuItem(Menu_DevRuOps_Path + MenuCommandExportDiff, false, 1)]
        private static void DevRuOps_ExportDiff()
        {
            ExportToXlsx(true);
        }

        [MenuItem(MenuEiPath, false, 21)]
        [MenuItem(Menu_DevRuOps_Path + MenuCommandExportAll, false, 5)]
        private static void DevRuOps_ExportAll()
        {
            ExportToXlsx(true, true);
        }

        [MenuItem(MenuEiPath, false, 22)]
        [MenuItem(Menu_RuEnXOps_Path + MenuCommandExportDiff, false, 1)]
        private static void RuEnXOps_ExportDiff()
        {
            ExportToXlsx(false);
        }

        [MenuItem(MenuEiPath, false, 22)]
        [MenuItem(Menu_RuEnXOps_Path + MenuCommandExportAll, false, 5)]
        private static void RuEnXOps_ExportAll()
        {
            ExportToXlsx(false, true);
        }

        [MenuItem(MenuEiPath, false, 25)]
        [MenuItem(MenuEiPath + MenuCommandImport, false, 5)]
        private static void Any_Import()
        {
            ImportFromXlsx();
        }

//        [MenuItem(MenuEiPath, false, 25)]
//        [MenuItem(MenuEiPath + MenuCommandExportSummary, false, 10)]
//        private static void ExportSummary()
//        {
//            ExportToSummaryXlsx();
//        }

        private static void ExportToXlsx(bool isDevRu, bool isAll = false)
        {
            var loader = new FolderLoader(Application.dataPath);
            var gameResources = new GameResources(loader);
            gameResources.ConfigureForUnityEditor();
            var xlsExporter = new XlsExporter(Application.dataPath, XlsFolderRelPath, gameResources);
            xlsExporter.Export(isDevRu, isAll);
        }

        private static void ExportToSummaryXlsx()
        {
            UI.CallerLog("Started...");
        }

        private static void ImportFromXlsx()
        {
            var loader = new FolderLoader(Application.dataPath);
            var gameResources = new GameResources(loader);
            gameResources.ConfigureForUnityEditor();
            var importFilePath = EditorUtility.OpenFilePanel("Файл для импорта", "", "xlsx");
            var xlsImporter = new XlsImporter(Application.dataPath, gameResources);
            xlsImporter.Import(importFilePath);
        }
    }
}