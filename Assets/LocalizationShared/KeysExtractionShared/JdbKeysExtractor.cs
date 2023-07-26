using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Assets.Src.ResourcesSystem.Base;
using Assets.Src.ResourcesSystem.JdbUpdater;
using Core.Environment.Logging.Extension;
using ResourcesSystem.Loader;
using EnumerableExtensions;
using JetBrains.Annotations;
using NLog;

namespace L10n.KeysExtractionNs
{
    public class JdbKeysExtractor
    {
        public enum ObservableKind
        {
            None,
            LocalizedString,
            WithLocalizedAttribute,
            ResourceRef,
            Array,
            LocalizedStringArray,
            GenericList,
            GenericDictionary
        }

        public enum Operation
        {
            ScanOnly,
            KeysExtraction,
            DeleteUnusedKeys,
            RecordVersionsAutofix,
            UpdateExtractionComments,
            HashesRecalculation, //DEBUG
        }

        private static readonly NLog.Logger Logger = LogManager.GetLogger("Localization");

        public const string KeyPartsDivider = ".";
        public const int MaxRecursionLevel = 64;
        public readonly string[] DeprecatedKeySigns = {".", " ", ","};

        private GameResources _gameResources;

        private KeysExtractionService _keysExtractionService;

        private string[] _filenamesForDebug;


        //=== Ctor ============================================================

        public JdbKeysExtractor(GameResources gameResources, KeysExtractionService keysExtractionService, string[] filenamesForDebug)
        {
            if (gameResources.AssertIfNull(nameof(gameResources)) ||
                keysExtractionService.AssertIfNull(nameof(keysExtractionService)))
                return;

            _filenamesForDebug = filenamesForDebug != null ? filenamesForDebug.Where(s => !string.IsNullOrEmpty(s)).ToArray() : new string[0]; //not null
            _keysExtractionService = keysExtractionService;
            _gameResources = gameResources;
        }


        //=== Public ==========================================================

        public List<LocalizedStringMetadata> GetAllLocalizedStrings(string jdbRelPath, Operation operation, ref List<string> processedWithErrors)
        {
            var isDebug = _filenamesForDebug.Any(jdbRelPath.Contains);
            if (isDebug)
                Logger.IfDebug()?.Message($"{nameof(GetAllLocalizedStrings)}({jdbRelPath})").Write(); //2del
            var localizedStrings = new List<LocalizedStringMetadata>();
            IResource resource = null;
            try
            {
                resource = _gameResources.LoadResource<IResource>(jdbRelPath);
            }
            catch (Exception e)
            {
                Logger.IfError()?.Message($"Resource '{jdbRelPath}' loading exception: {e.Message}\n{e.StackTrace}").Write();
            }

            if (resource != null)
            {
                if (IsObservableType(resource.GetType(), out var observableKind))
                {
                    var jdbContext = new JdbContext(jdbRelPath, operation);

                    object resObj = resource;
                    if (IsNeedToUpdateObservableObject(observableKind, ref resObj, ref localizedStrings, jdbContext) && operation == Operation.KeysExtraction)
                    {
                        var newLocalizedStrings = localizedStrings.Where(e => e.IsNewKey).ToArray();
                        if (newLocalizedStrings.Any())
                            JdbRewriteTool.JdbUpdate(
                                _keysExtractionService.SkipRefsSerializer,
                                _keysExtractionService.RepositoryPath,
                                jdbRelPath,
                                resObj,
                                isDebug,
                                newLocalizedStrings.Select(e => e.HierPath).ToArray());
                    }
                }
            }
            else
            {
                processedWithErrors.Add(jdbRelPath);
                Logger.IfError()?.Message($"Resource from '{jdbRelPath}' is null").Write();
            }

            return localizedStrings;
        }


        //=== Private =========================================================

        /// <summary>
        /// Возвращает true, если obj это структура и она изменена (извлечен новый ключ)
        /// </summary>
        private bool IsNeedToUpdateObservableObject(
            ObservableKind objObservableKind,
            ref object obj,
            ref List<LocalizedStringMetadata> localizedStringsList,
            JdbContext context,
            string propsHierPath = "",
            int recursionLevel = 0)
        {
            var debugInfoBegin = context.IsDebugAvailable ? $"'{context.JdbRelPath}' <{objObservableKind}> [{propsHierPath}] rl={recursionLevel}" : null;
            if (obj is IResource resource)
            {
                if (context.IsDebugAvailable)
                    Logger.IfDebug()?.Message($"******** {debugInfoBegin} res={resource.Address}").Write();

                //чистка ссылок на другие ресурсы и на себя во избежание ошибок сериализации обновленного объекта
                if (resource.Address.Root != context.JdbRelPath || //ссылка на другой файл
                    resource.Address.Line == 0 && resource.Address.Col == 0 && recursionLevel > 0) //ссылка на себя
                {
                    if (context.CanBeChanged)
                    {
                        obj = null;
                        if (context.IsDebugAvailable)
                            Logger.IfDebug()?.Message($"{debugInfoBegin} Link to self, obj=null").Write(); //2del
                    }

                    return context.CanBeChanged;
                }
            }

            if (recursionLevel > MaxRecursionLevel)
            {
                Logger.Error(
                    $"{debugInfoBegin} <{obj.GetType()}> obj={obj} " +
                    $"Break due recursion level exceeded acceptable value: {MaxRecursionLevel}");
                return false;
            }

            recursionLevel++;

            var changed = false;
            switch (objObservableKind)
            {
                case ObservableKind.LocalizedString:
                    var localizedString = (LocalizedString) obj;
                    //извлечение ключей из props объекта
                    if (!LocalizedString.IsKey(localizedString))
                    {
                        if (KeysExtractionService.IsOperationForExistingKeysOnly(context.Operation) || string.IsNullOrEmpty(localizedString.Key))
                            return false; //при операции удаления неиспользуемых новые ключи можно игнорировать, также игнорируем пустые ключи

                        if (context.Operation == Operation.KeysExtraction)
                        {
                            localizedString = _keysExtractionService.TakeEmbeddedTranslationDataAndReturnDataWithNewKey(
                                localizedString,
                                LocalizedStringMetadata.GetComment(context.JdbRelPath, propsHierPath));
                            obj = localizedString;
                        }

                        localizedStringsList.Add(new LocalizedStringMetadata(context.JdbRelPath, localizedString, propsHierPath, isNewKey: true));
                        if (context.IsDebugAvailable)
                            Logger.IfDebug()?.Message($"{debugInfoBegin} New key, {localizedString}").Write(); //2del
                        return context.CanBeChanged;
                    }
                    else
                    {
                        localizedStringsList.Add(new LocalizedStringMetadata(context.JdbRelPath, localizedString, propsHierPath, isNewKey: false));
                        return false;
                    }


                case ObservableKind.WithLocalizedAttribute:
                case ObservableKind.ResourceRef:
                    var observableProperties = GetObservableProperties(obj.GetType());
//                    Logger.Debug($"GatherKeys: '{propsHierPath}' found observable props: " +
//                                 observableProperties
//                                     .Select(kvp => $"({kvp.Value}) <{kvp.Key.PropertyType.NiceName()}> {kvp.Key.Name}")
//                                     .ItemsToString()); //2del

                    foreach (var kvp in observableProperties)
                    {
                        var propertyValue = kvp.Key.GetValue(obj);
                        if (propertyValue == null)
                            continue;

                        var hierPath = objObservableKind == ObservableKind.ResourceRef
                            ? propsHierPath //пропускаем ".Target"
                            : $"{GetHierPathWithDividerIfNeed(propsHierPath)}{kvp.Key.Name}";

                        if (IsNeedToUpdateObservableObject(kvp.Value, ref propertyValue, ref localizedStringsList, context, hierPath, recursionLevel))
                        {
                            changed = context.CanBeChanged;
                            if (context.IsDebugAvailable)
                                Logger.IfDebug()?.Message($"{debugInfoBegin} Localized property '{kvp.Key.Name}', changed{changed.AsSign()}").Write(); //2del

                            if (ObservableKind.ResourceRef == objObservableKind)
                            {
                                var propType = kvp.Key.PropertyType;
                                var type = typeof(ResourceRef<>).MakeGenericType(propType);
                                obj = type.GetConstructor(new[] {propType}).Invoke(new object[] {propertyValue});
                                if (context.IsDebugAvailable)
                                    Logger.IfDebug()?.Message($"{debugInfoBegin} Set new <{type}> object").Write(); //2del
                            }
                            else
                            {
                                if (kvp.Key.PropertyType.IsValueType)
                                {
                                    kvp.Key.SetValue(obj, propertyValue);
                                    if (context.IsDebugAvailable)
                                        Logger.IfDebug()?.Message($"{debugInfoBegin} Rewrite VT property {kvp.Key.Name}: {propertyValue}").Write(); //2del
                                }
                            }
                        }
                    }

                    return changed;

                case ObservableKind.Array:
                case ObservableKind.LocalizedStringArray:
                case ObservableKind.GenericList:
                    var arrayOrList = (IList) obj;
                    if (!arrayOrList.AssertIfNull(nameof(arrayOrList)))
                    {
                        var elementsKind = GetArrayOrListElementKind(arrayOrList);
                        if (context.IsDebugAvailable)
                            Logger.IfDebug()?.Message($"{debugInfoBegin} Found collection: count={arrayOrList.Count}").Write(); //2del

                        for (int i = 0; i < arrayOrList.Count; i++)
                        {
                            var elem = arrayOrList[i];
                            if (elem == null)
                                continue;

                            if (IsNeedToUpdateObservableObject(
                                elementsKind,
                                ref elem,
                                ref localizedStringsList,
                                context,
                                $"{propsHierPath}[{i}]",
                                recursionLevel))
                            {
                                changed = context.CanBeChanged;
                                if (elem.GetType().IsValueType)
                                {
                                    arrayOrList[i] = elem;
                                    if (context.IsDebugAvailable)
                                        Logger.IfDebug()?.Message($"'{debugInfoBegin} Rewrite VT element[{i}]: {elem}").Write(); //2del
                                }
                            }
                        }
                    }

                    return changed;

                case ObservableKind.GenericDictionary:
                    var dictionary = (IDictionary) obj;
                    if (!dictionary.AssertIfNull(nameof(dictionary)))
                    {
                        var elementsKind = GetDictionaryElementKind(dictionary);
//                        Logger.Debug($"GatherKeys: '{propsHierPath}' Explore list or array: count={dictionary.Count}, " +
//                                     $"elementsKind={elementsKind}"); //2del

                        var keys = new List<object>();
                        foreach (DictionaryEntry entry in dictionary)
                            keys.Add(entry.Key);

                        foreach (var k in keys)
                        {
                            if (k is string keyAsString && DeprecatedKeySigns.Any(s => keyAsString.Contains(s)))
                            {
                                throw new Exception(
                                    $"'{propsHierPath}' Need to remove from key '{keyAsString}' deprecated symbols " +
                                    $"{DeprecatedKeySigns.ItemsToString(isShowCount: false, isDelimitVertically: false, delimeter: "")}");
                            }

                            var dctValue = dictionary[k];
                            if (dctValue == null)
                                continue;

                            var isElemStruct = dctValue.GetType().IsValueType;
                            if (IsNeedToUpdateObservableObject(
                                elementsKind,
                                ref dctValue,
                                ref localizedStringsList,
                                context,
                                $"{GetHierPathWithDividerIfNeed(propsHierPath)}{k}",
                                recursionLevel))
                            {
                                changed = context.CanBeChanged;
                                if (isElemStruct)
                                {
                                    dictionary[k] = dctValue;
                                    if (context.IsDebugAvailable)
                                        Logger.IfDebug()?.Message($"{debugInfoBegin} Rewrite dict. element [{k}]: {dctValue}").Write(); //2del
                                }
                            }
                        }
                    }

                    return changed;
            }

            Logger.IfError()?.Message($"'{propsHierPath}' obj={obj} Unhandled ObservableKind: {objObservableKind}").Write();
            return false;
        }

        public static string GetHierPathWithDividerIfNeed(string hierPath)
        {
            return hierPath.Length > 0 ? hierPath + KeyPartsDivider : "";
        }

        public static List<KeyValuePair<PropertyInfo, ObservableKind>> GetObservableProperties(Type type)
        {
            var list = new List<KeyValuePair<PropertyInfo, ObservableKind>>();
            var props = type.GetProperties();
            foreach (var propertyInfo in props)
            {
                var isObservableType = IsObservableType(propertyInfo.PropertyType, out var observableKind);
                if (isObservableType)
                    list.Add(new KeyValuePair<PropertyInfo, ObservableKind>(propertyInfo, observableKind));
            }

            return list;
        }

        public static ObservableKind GetArrayOrListElementKind([NotNull] IList list)
        {
            var listType = list.GetType();
            var elemType = listType.IsArray ? listType.GetElementType() : listType.GetGenericArguments()[0];
            var isObservableType = IsObservableType(elemType, out var elemObservableKind);
            return elemObservableKind;
        }

        public static ObservableKind GetDictionaryElementKind([NotNull] IDictionary dictionary)
        {
            var elemType = dictionary.GetType().GetGenericArguments()[1];
            var isObservableType = IsObservableType(elemType, out var elemObservableKind);
            return elemObservableKind;
        }

        public static bool IsObservableType(Type type, out ObservableKind observableKind)
        {
            observableKind = ObservableKind.None;

            if (type == typeof(LocalizedString))
            {
                observableKind = ObservableKind.LocalizedString;
                return true;
            }

            if (type.GetCustomAttributes(typeof(LocalizedAttribute), false).Length > 0)
            {
                observableKind = ObservableKind.WithLocalizedAttribute;
                return true;
            }

            if (type.IsArray)
            {
                observableKind = ObservableKind.Array;
                var res = IsObservableType(type.GetElementType(), out var someObservableKind);
                if (someObservableKind == ObservableKind.LocalizedString)
                    observableKind = ObservableKind.LocalizedStringArray;
                return res;
            }

            //Из допустимых остались списки и словари
            if (!type.IsGenericType)
                return false;

            var genericArgs = type.GetGenericArguments();
            var genericTypeDefinition = type.GetGenericTypeDefinition();

            if (genericTypeDefinition == typeof(List<>))
            {
                observableKind = ObservableKind.GenericList;
                return IsObservableType(genericArgs[0], out var someObservableType);
            }

            if (genericTypeDefinition == typeof(Dictionary<,>))
            {
                observableKind = ObservableKind.GenericDictionary;
                return IsObservableType(genericArgs[1], out var someObservableType);
            }

            if (genericTypeDefinition == typeof(ResourceRef<>) &&
                genericArgs[0].GetCustomAttributes(typeof(LocalizedAttribute), false).Length > 0)
            {
                observableKind = ObservableKind.ResourceRef;
                return true;
            }

            return false;
        }


        //=== Subclass ================================================================================================

        /// <summary>
        /// Параметры обработки Jdb-ресурса
        /// </summary>
        private class JdbContext
        {
            /// <summary>
            /// Путь к ресурсу
            /// </summary>
            public string JdbRelPath;

            public Operation Operation;

            public List<string> JdbRelPathsForDebug = new List<string>();

            public JdbContext(string jdbRelPath, Operation operation)
            {
                JdbRelPath = jdbRelPath;
                Operation = operation;
            }

            public bool IsDebugAvailable => JdbRelPathsForDebug.Contains(JdbRelPath);

            public bool CanBeChanged => Operation == Operation.KeysExtraction;
        }
    }
}