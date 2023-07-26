//using System;
//using System.Collections;
//using System.Collections.Generic;
//using Assets.Src.ResourcesSystem.Base;
//using Assets.Src.ResourcesSystem.JdbUpdater;
//using ResourcesSystem.Loader;
//using System.Linq;
//using L10n.KeysExtractionNs;
//using NLog;
//using ObservableKind = L10n.KeysExtractionNs.JdbKeysExtractor.ObservableKind;
//
//namespace L10n.ConversionNs
//{
//    public class JdbPropsConverter
//    {
//        private static readonly NLog.Logger Logger = LogManager.GetLogger("Localization");
//        private GameResources _gameResources;
//        private ConversionService _conversionService;
//        private bool _doConversion;
//        private string _jdbRelPath;
//
//
//        //=== Ctor ============================================================
//
//        public JdbPropsConverter(GameResources gameResources, ConversionService conversionService)
//        {
//            if (gameResources.AssertIfNull(nameof(gameResources)) ||
//                conversionService.AssertIfNull(nameof(conversionService)))
//                return;
//
//            _conversionService = conversionService;
//            _gameResources = gameResources;
//        }
//
//
//        //=== Public ==========================================================
//
//        public List<ConversionMetadata> ScanOrConvertData(string jdbRelPath, bool doConversion)
//        {
////            Logger.IfDebug()?.Message($"ScanOrConvertData({jdbRelPath}, doConversion{doConversion.AsSign()})").Write(); //2del
//            _doConversion = doConversion;
//            _jdbRelPath = jdbRelPath;
//            var conversionList = new List<ConversionMetadata>();
//            IResource resource = null;
//            try
//            {
//                resource = _gameResources.LoadResource<IResource>(jdbRelPath);
//            }
//            catch (Exception e)
//            {
//                Logger.IfError()?.Message($"Resource '{jdbRelPath}' loading exception: {e.Message}\n{e.StackTrace}").Write();
//            }
//
//            if (resource == null)
//            {
//                Logger.IfError()?.Message($"ScanOrConvertData('{jdbRelPath}', doConversion{doConversion.AsSign()}): resource is null").Write(); //2del
//                return conversionList;
//            }
//
//            ObservableKind observableKind;
//            if (JdbKeysExtractor.IsObservableType(resource.GetType(), out observableKind))
//            {
//                object resObj = resource;
//                if (InObservableObject(ref resObj, observableKind, conversionList) && doConversion)
//                {
//                    var newConverted = conversionList.Where(e => e.IsConverted).ToArray();
//                    if (newConverted.Any())
//                    {
//                        Logger.IfDebug()?.Message($"Rewrite file '{jdbRelPath}'").Write();
//                        List<string> paths = newConverted.Select(e => e.HierPath).ToList();
//                        paths.AddRange(newConverted.Select(e => e.HierPathLs).ToList());
//                        JdbRewriteTool.JdbUpdate(
//                            _conversionService.SkipRefsSerializer,
//                            _conversionService.RepositoryPath,
//                            jdbRelPath,
//                            resObj,
//                            false,
//                            paths.ToArray());
//                    }
//                }
//            }
//
////            Logger.Debug($"ConversionMetadataList('{jdbRelPath}', doConversion{doConversion.AsSign()}): " +
////                         $"{conversionList.ItemsToStringByLines()}"); //2del
//
//            return conversionList;
//        }
//
//        public static string GetHierPathForStringProperty(string localizedStringHierPath)
//        {
//            if ((localizedStringHierPath?.Length ?? 0) < 2)
//            {
//                Logger.IfError()?.Message($"{nameof(GetHierPathForStringProperty)}({localizedStringHierPath}) Too short original path").Write();
//                return localizedStringHierPath;
//            }
//
//            return localizedStringHierPath.Substring(0, localizedStringHierPath.Length - 2);
//        }
//
//        //=== Private =========================================================
//
//        /// <summary>
//        /// Возвращает true, если obj это структура и она изменена (извлечен новый ключ)
//        /// </summary>
//        private bool InObservableObject(ref object obj, ObservableKind objObservableKind, List<ConversionMetadata> conversionList,
//            string propsHierPath = "", int recursionLevel = 0, object unconvertedData = null)
//        {
//            if (obj is IResource)
//            {
//                var resource = obj as IResource;
//                if (resource.Address.Root != _jdbRelPath) //не выходим из границ файла
//                    return false;
//
//                if (resource.Address.Line == 0 && resource.Address.Col == 0 && propsHierPath.Length > 0) //ссылка на самого себя
//                    return false;
//
////                Logger.IfDebug()?.Message($"'{_jdbRelPath}' [{propsHierPath}] Into '{(obj as IResource).Address}'").Write(); //2del
//            }
//
//            var isChanged = false;
////            Logger.IfDebug()?.Message($"InObservableObject.Start: '{propsHierPath}' kind={objObservableKind}, obj={obj}").Write(); //2del
//
//            if (recursionLevel > JdbKeysExtractor.MaxRecursionLevel)
//            {
//                Logger.Error($"InObservableObject: '{propsHierPath}' <{obj.GetType()}> obj={obj} " +
//                             $"Break due recursion level exceeded acceptable value: {JdbKeysExtractor.MaxRecursionLevel}");
//                return false;
//            }
//
//            recursionLevel++;
//
//            switch (objObservableKind)
//            {
//                case ObservableKind.LocalizedString:
//                    if (!(unconvertedData is string))
//                        return false;
//
//                    var unconvertedText = (string) unconvertedData;
//                    if (string.IsNullOrEmpty(unconvertedText))
//                        return false;
//
//                    var localizedString = (LocalizedString) obj;
//                    if (!LocalizedString.IsKey(localizedString))
//                    {
//                        if (_doConversion)
//                        {
//                            localizedString = new LocalizedString(unconvertedText, localizedString.TranslationData);
//                            obj = localizedString;
//                        }
//
//                        if (!propsHierPath.EndsWith("]")) //значения элементов массива пропускаем, потому что под запись сам массив
//                        {
//                            conversionList.Add(new ConversionMetadata(
//                                _jdbRelPath,
//                                propsHierPath,
//                                _doConversion ? null : new[] {unconvertedText},
//                                _doConversion));
//                        }
//
//                        return _doConversion;
//                    }
//
//                    //LS с ключом
//                    if (_doConversion)
//                        _conversionService.SetUnconvertedTextByKey(localizedString.Key, unconvertedText);
//
//                    if (!propsHierPath.EndsWith("]")) //значения элементов массива пропускаем, потому что под запись сам массив
//                    {
//                        conversionList.Add(
//                            new ConversionMetadata(
//                                _jdbRelPath,
//                                propsHierPath,
//                                _doConversion ? null : new[] {unconvertedText},
//                                _doConversion));
//                    }
//
//                    return _doConversion;
//
//                case ObservableKind.ResourceRef:
//                case ObservableKind.WithLocalizedAttribute:
//                    var observableProperties = JdbKeysExtractor.GetObservableProperties(obj.GetType());
////                    Logger.Debug($"InObservableObject: '{propsHierPath}' found observable props: " +
////                                 observableProperties
////                                     .Select(kvp => $"({kvp.Value}) <{kvp.Key.PropertyType.NiceName()}> {kvp.Key.Name}")
////                                     .ItemsToString()); //2del
//
//                    foreach (var kvp in observableProperties)
//                    {
//                        var propertyValue = kvp.Key.GetValue(obj);
//                        var propHierPath = objObservableKind == ObservableKind.ResourceRef 
//                            ? propsHierPath //пропускаем "Target"
//                            : $"{HierPathAndDivider(propsHierPath)}{kvp.Key.Name}";
//                        object foundUnconvertedData = null;
//
//                        switch (kvp.Value)
//                        {
//                            case ObservableKind.LocalizedString:
//                            {
//                                var unconvertedPropertyInfo = obj.GetType().GetProperty(GetHierPathForStringProperty(kvp.Key.Name));
//                                if (unconvertedPropertyInfo == null ||
//                                    !(unconvertedPropertyInfo.PropertyType == typeof(string) ||
//                                      unconvertedPropertyInfo.PropertyType == typeof(string[])))
//                                    continue;
//
//                                if (unconvertedPropertyInfo.PropertyType == typeof(string))
//                                {
//                                    var foundUnconvertedText = (string) unconvertedPropertyInfo.GetValue(obj);
//                                    if (string.IsNullOrEmpty(foundUnconvertedText))
//                                        continue;
//
//                                    foundUnconvertedData = foundUnconvertedText;
//                                }
//                                else
//                                {
//                                    //string[]
//                                    var foundUnconvertedTextArr = (string[]) unconvertedPropertyInfo.GetValue(obj);
//                                    if (foundUnconvertedTextArr == null ||
//                                        foundUnconvertedTextArr.Length == 0 ||
//                                        string.IsNullOrEmpty(foundUnconvertedTextArr[0]))
//                                        continue;
//
//                                    foundUnconvertedData = foundUnconvertedTextArr[0];
//                                }
//
//                                if (_doConversion)
//                                    unconvertedPropertyInfo.SetValue(obj, null);
//
//                                break;
//                            }
//
////                            case ObservableKind.LocalizedStringArray:
////                            {
////                                var elemType = kvp.Key.PropertyType.GetElementType();
////                                if (elemType == typeof(LocalizedString))
////                                {
////                                    //ищем рядом массив строк
////                                    var unconvertedPropertyInfo = obj.GetType().GetProperty(GetHierPathForStringProperty(kvp.Key.Name));
////                                    if (unconvertedPropertyInfo == null || unconvertedPropertyInfo.PropertyType != typeof(string[]))
////                                    {
////                                        Logger.IfError()?.Message($"Not found unconverted string[] for {kvp.Key.Name}").Write();
////                                        continue;
////                                    }
////
////                                    var unconvertedTextArray = (string[]) unconvertedPropertyInfo.GetValue(obj);
////                                    if (unconvertedTextArray == null)
////                                        continue;
////
////                                    if (unconvertedTextArray.All(string.IsNullOrEmpty))
////                                        continue;
////
////                                    conversionList.Add(new ConversionMetadata(
////                                        _jdbRelPath, 
////                                        propHierPath, 
////                                        _doConversion ? null : unconvertedTextArray.Where(s => !string.IsNullOrEmpty(s)).ToArray(),
////                                        _doConversion));
////
////                                    foundUnconvertedData = unconvertedTextArray;
////                                    if (_doConversion)
////                                        unconvertedPropertyInfo.SetValue(obj, null);
////                                }
////                                else
////                                {
////                                    if (propertyValue == null)
////                                        continue;
////                                }
////
////                                break;
////                            }
//
//                            default:
//                                if (propertyValue == null)
//                                    continue;
//                                break;
//                        }
//
//                        var isNullObjectWasObserved = propertyValue == null;
//                        if (InObservableObject(ref propertyValue, kvp.Value, conversionList, propHierPath, recursionLevel,
//                            foundUnconvertedData))
//                        {
//                            isChanged = true;
//                            if (kvp.Key.PropertyType.IsValueType || isNullObjectWasObserved)
//                            {
//                                kvp.Key.SetValue(obj, propertyValue);
////                                Logger.IfDebug()?.Message($"InObservableObject: '{propsHierPath}' Rewrite property {kvp.Key.Name}: {propertyValue}").Write(); //2del
//                            }
//                        }
//                    }
//
//                    return isChanged;
//
////                case ObservableKind.LocalizedStringArray:
////                    if (unconvertedData as string[] == null)
////                        return false;
////
////                    var unconvertedStrArray = (string[]) unconvertedData;
////                    var lsArray = (LocalizedString[]) obj;
////
////                    var maxPayloadIndex = 0;
////                    for (int i = 0; i < unconvertedStrArray.Length; i++)
////                        if (!string.IsNullOrEmpty(unconvertedStrArray[i]))
////                            maxPayloadIndex = i;
////
////                    if (lsArray == null)
////                    {
////                        lsArray = new LocalizedString[maxPayloadIndex + 1];
////                    }
////                    else
////                    {
////                        if (lsArray.Length < maxPayloadIndex + 1)
////                        {
////                            var newLsArray = new LocalizedString[maxPayloadIndex + 1];
////                            for (int i = 0; i < lsArray.Length; i++)
////                                newLsArray[i] = lsArray[i];
////                            lsArray = newLsArray;
////                        }
////                    }
////
////                    //с unconvertedStrArray ничего не делаем, т.к. уровнем выше его обнулили, если это требовалось
////                    for (int i = 0; i < unconvertedStrArray.Length; i++)
////                    {
////                        if (string.IsNullOrEmpty(unconvertedStrArray[i]))
////                            continue;
////
////                        var lsArrayElem = (object) lsArray[i];
////                        if (InObservableObject(ref lsArrayElem, ObservableKind.LocalizedString, conversionList,
////                            $"{propsHierPath}[{i}]", recursionLevel, unconvertedStrArray[i]))
////                        {
////                            isChanged = true;
////                            lsArray[i] = (LocalizedString) lsArrayElem;
////                        }
////                    }
////
////                    if (isChanged)
////                        obj = lsArray;
////
////                    return isChanged;
//
//                case ObservableKind.Array:
//                case ObservableKind.GenericList:
//                    var arrayOrList = (IList) obj;
//                    if (!arrayOrList.AssertIfNull(nameof(arrayOrList)))
//                    {
//                        var elementsKind = JdbKeysExtractor.GetArrayOrListElementKind(arrayOrList);
////                        Logger.Debug($"InObservableObject: '{propsHierPath}' Explore list or array: count={arrayOrList.Count}, " +
////                                     $"elementsKind={elementsKind}"); //2del
//
//                        for (int i = 0; i < arrayOrList.Count; i++)
//                        {
//                            var elem = arrayOrList[i];
//                            if (elem == null)
//                                continue;
//
//                            if (InObservableObject(ref elem, elementsKind, conversionList, $"{propsHierPath}[{i}]",
//                                recursionLevel))
//                            {
//                                isChanged = true;
//                                if (elem.GetType().IsValueType)
//                                {
//                                    arrayOrList[i] = elem;
////                                    Logger.IfDebug()?.Message($"InObservableObject: '{propsHierPath}[{i}]' Rewrite element: {elem}").Write(); //2del
//                                }
//                            }
//                        }
//                    }
//
//                    return isChanged;
//
//                case ObservableKind.GenericDictionary:
//                    var dictionary = (IDictionary) obj;
//                    if (!dictionary.AssertIfNull(nameof(dictionary)))
//                    {
//                        var elementsKind = JdbKeysExtractor.GetDictionaryElementKind(dictionary);
////                        Logger.Debug($"InObservableObject: '{propsHierPath}' Explore list or array: count={dictionary.Count}, " +
////                                     $"elementsKind={elementsKind}"); //2del
//
//                        var keys = new List<object>();
//                        foreach (DictionaryEntry entry in dictionary)
//                            keys.Add(entry.Key);
//
//                        foreach (var k in keys)
//                        {
//                            var dctValue = dictionary[k];
//                            if (dctValue == null)
//                                continue;
//
//                            var isElemStruct = dctValue.GetType().IsValueType;
//                            if (InObservableObject(ref dctValue, elementsKind, conversionList,
//                                $"{HierPathAndDivider(propsHierPath)}{k}", recursionLevel))
//                            {
//                                isChanged = true;
//                                if (isElemStruct)
//                                {
//                                    dictionary[k] = dctValue;
////                                    Logger.Debug($"InObservableObject: '{HierPathAndDivider(propsHierPath)}{k}' " +
////                                                 $"Rewrite value: {dctValue}"); //2del
//                                }
//                            }
//                        }
//                    }
//
//                    return isChanged;
//            }
//
//            Logger.IfError()?.Message($"InObservableObject: '{propsHierPath}' obj={obj} Unhandled ObservableKind: {objObservableKind}").Write();
//            return false;
//        }
//
//        private string HierPathAndDivider(string hierPath)
//        {
//            return JdbKeysExtractor.GetHierPathWithDividerIfNeed(hierPath);
//        }
//    }
//}