using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Core.Environment.Logging.Extension;
using EnumerableExtensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;

namespace Assets.Src.ResourcesSystem.JdbUpdater
{
    public static class JdbRewriteTool
    {
        private static readonly NLog.Logger Logger = LogManager.GetLogger("JdbRewriteTool");


        //=== Public ==========================================================

        public static bool JdbUpdate(
            JsonSerializer serializer,
            string repositoryPath,
            string resourceRelPath,
            object updatedObject,
            bool withDebug,
            string updatedPropName)
        {
            return JdbUpdate(serializer, repositoryPath, resourceRelPath, updatedObject, withDebug, new[] {updatedPropName});
        }

        public static bool JdbUpdate(
            JsonSerializer serializer,
            string repositoryPath,
            string resourceRelPath,
            object updatedObject,
            bool withDebug,
            string[] updatedPropNames = null)
        {
            if (serializer.AssertIfNull(nameof(serializer)))
                return false;

            var fullPath = GetResourceFileFullPath(repositoryPath, resourceRelPath);
            if (!File.Exists(fullPath))
            {
                Logger.IfError()?.Message($"File not exists: '{fullPath}'").Write();
                return false;
            }

            string oldJdbText = null;
            try
            {
                oldJdbText = File.ReadAllText(fullPath);
            }
            catch (Exception e)
            {
                Logger.IfError()?.Message($"{nameof(File.ReadAllText)}() exception: {e.Message}\n{e.StackTrace}").Write();
                return false;
            }

            if (oldJdbText.AssertIfNull(nameof(oldJdbText)))
                return false;

            var newJdbText = UpdateJsonFromObject(serializer, oldJdbText, updatedObject, updatedPropNames, withDebug);
            try
            {
                File.WriteAllText(fullPath, newJdbText);
            }
            catch (Exception e)
            {
                Logger.IfError()?.Message($"{nameof(File.WriteAllText)}() exception: {e.Message}\n{e.StackTrace}").Write();
                return false;
            }

            return true;
        }

        public static string UpdateJsonFromObject(
            JsonSerializer serializer,
            string json,
            object updatedObject,
            string[] updatedPropNames,
            bool withDebug,
            bool omitDefaultValues = false)
        {
            if (withDebug)
            {
                var objStringWriter = new StringWriter();
                serializer.Serialize(objStringWriter, updatedObject);
                Logger.IfInfo()?.Message($"objStringWriter: <<<{objStringWriter}>>> {nameof(updatedPropNames)}: {updatedPropNames.ItemsToString()}").Write();
            }

            var propWatchers = new List<PropWatcher>();
            if (updatedPropNames != null)
                updatedPropNames.ForEach(n => propWatchers.Add(new PropWatcher(n)));

            JObject jObject = null;
            try
            {
                jObject = JObject.FromObject(updatedObject, serializer);
            }
            catch (Exception e)
            {
                Logger.IfError()?.Message($"updatedObject={updatedObject}, Exception: {e}").Write(); //2del
            }

            var stringWriter = new StringWriter();
            var jsonTextWriter = new JsonTextWriter(stringWriter);
            jsonTextWriter.Formatting = Formatting.Indented;
            jsonTextWriter.Indentation = 2;

            var reader = new JsonTextReader(new StringReader(json));

            string prevPropName = null;
            string ignoreThisPath = null;
            while (reader.Read())
            {
                if (ignoreThisPath != null)
                {
                    if (reader.Path.StartsWith(ignoreThisPath)) //читаем reader без записи, т.к. значение по Path записано из updatedJObject
                    {
                        if (reader.TokenType == JsonToken.Comment)
                        {
                            jsonTextWriter.WriteToken(reader.TokenType, reader.Value);
                        }

                        continue;
                    }

                    ignoreThisPath = null;
                }

                if (IsUpdatedPath(reader.Path, propWatchers))
                {
                    var updatedToken = jObject.SelectToken(reader.Path);
                    if (!IsDefaultValue(updatedToken))
                    {
                        jsonTextWriter.WriteToken(reader.TokenType, reader.Value); //имя свойства
                        WriteUpdatedToken(updatedToken, jsonTextWriter, withDebug);
                    }

                    ignoreThisPath = reader.Path;
                    continue;
                }

                if (prevPropName != null)
                {
                    if (withDebug)
                        Logger.IfInfo()
                            ?.Message(
                                $"[{reader.Path}] <{reader.TokenType}> Delayed write propertyName '{prevPropName}' " +
                                $"Value=<{reader.Value?.GetType()}> {reader.Value}...")
                            .Write();
                    if (omitDefaultValues && IsDefaultValue(reader)) //Дефолтные значения: пишем или нет
                    {
                        if (withDebug)
                            Logger.IfInfo()?.Message("Omited").Write();
                        prevPropName = null; //Omit default values
                        continue;
                    }

                    if (withDebug)
                        Logger.IfInfo()?.Message("... written").Write();
                    jsonTextWriter.WriteToken(JsonToken.PropertyName, prevPropName);
                    prevPropName = null;
                }

                if (reader.Value == null)
                {
                    if (withDebug)
                        Logger.IfInfo()?.Message($"[{reader.Path}] <{reader.TokenType}> Write token with null value").Write();
                    if (reader.TokenType == JsonToken.EndObject)
                    {
                        foreach (var propWatcher in propWatchers)
                        {
                            if (propWatcher.IsPassed || reader.Path != propWatcher.ParentHierPath)
                                continue;

                            //Вписываем недостающие в оригинальном json токены (если они есть в обновленном объекте)
                            propWatcher.IsPassed = true;
                            var updatedToken = jObject.SelectToken(propWatcher.HierPath);
                            if (!IsDefaultValue(updatedToken))
                            {
                                jsonTextWriter.WriteToken(JsonToken.PropertyName, propWatcher.PropertyName); //имя свойства
                                WriteUpdatedToken(updatedToken, jsonTextWriter, withDebug);
                            }
                        }
                    }

                    jsonTextWriter.WriteToken(reader.TokenType);
                    continue;
                }

                if (withDebug)
                    Logger.IfInfo()?.Message($"[{reader.Path}] <{reader.TokenType}> Write token with value={reader.Value}").Write();
                if (reader.TokenType == JsonToken.PropertyName)
                {
                    if (withDebug)
                        Logger.IfInfo()?.Message($"[{reader.Path}] <{reader.TokenType}> Save property name '{reader.Value}'").Write();
                    prevPropName = (string) reader.Value;
                }
                else
                {
                    jsonTextWriter.WriteToken(reader.TokenType, reader.Value);
                }
            }

            propWatchers.ForEach(
                propWatcher =>
                {
                    if (!propWatcher.IsPassed)
                        Logger.IfError()?.Message($"propWatcher isn't passed: {propWatcher}").Write();
                });

            return TransformBlockToLineComments(stringWriter.ToString());
        }


        //=== Private =========================================================

        private static void WriteUpdatedToken(JToken token, JsonTextWriter writer, bool withDebug)
        {
            if (token.AssertIfNull(nameof(token)) ||
                writer.AssertIfNull(nameof(writer)))
                return;

            var reader = token.CreateReader();
            string prevPropName = null;

            while (reader.Read())
            {
                if (prevPropName != null) //Дефолтные значения не пишем
                {
                    if (withDebug)
                        Logger.IfInfo()
                            ?.Message(
                                $"WUT[{reader.Path}] <{reader.TokenType}> Delayed write propertyName " +
                                $"'{prevPropName}' Value=<{reader.Value?.GetType()}> {reader.Value}...")
                            .Write();
                    if (IsDefaultValue(reader))
                    {
                        if (withDebug)
                            Logger.IfInfo()?.Message("Omited").Write();
                        prevPropName = null; //Omit default values
                        continue;
                    }

                    if (withDebug)
                        Logger.IfInfo()?.Message("... written").Write();
                    writer.WriteToken(JsonToken.PropertyName, prevPropName);
                    prevPropName = null;
                }

                if (reader.Value == null)
                {
                    if (withDebug)
                        Logger.IfInfo()?.Message($"WUT[{reader.Path}] <{reader.TokenType}> Write token with null value").Write();
                    writer.WriteToken(reader.TokenType);
                    continue;
                }

                if (withDebug)
                    Logger.IfInfo()?.Message($"WUT[{reader.Path}] <{reader.TokenType}> Write token with value={reader.Value}").Write();
                if (reader.TokenType == JsonToken.PropertyName)
                {
                    if (withDebug)
                        Logger.IfInfo()?.Message($"WUT[{reader.Path}] <{reader.TokenType}> Save property name '{reader.Value}'").Write();
                    prevPropName = (string) reader.Value;
                }
                else
                {
                    writer.WriteToken(reader.TokenType, reader.Value);
                }
            }
        }

        private static bool IsDefaultValue(JToken token)
        {
            return (token == null ||
                    (token.Type == JTokenType.Null || token.Value<object>() == null) ||
                    token.Type == JTokenType.Integer && Convert.ToInt64(token.Value<object>()) == 0) ||
                   (token.Type == JTokenType.Boolean && !token.Value<bool>()) ||
                   (token.Type == JTokenType.String && token.Value<string>() == null);
        }

        private static bool IsDefaultValue(JsonReader reader)
        {
            return (reader.TokenType == JsonToken.Integer && Convert.ToInt64(reader.Value) == 0) ||
                   (reader.TokenType == JsonToken.Boolean && !((bool) reader.Value)) ||
                   (reader.TokenType == JsonToken.Null && reader.Value == null) ||
                   (reader.TokenType == JsonToken.String && reader.Value == null);
        }

        private static bool IsUpdatedPath(string path, List<PropWatcher> propWatchers)
        {
            if (propWatchers.Count == 0)
                return true;

            foreach (var propWatcher in propWatchers)
            {
                if (propWatcher.HierPath != path)
                    continue;

                propWatcher.IsPassed = true;
                return true;
            }

            return false;
        }

        /// <summary>
        /// JsonTextWriter не умеет писать строковые комментарии, заменяя их блоковыми, а также вставляет комментарии перед запятыми, что также
        /// Метод возвращает эти псевдо-блоковые комментарии обратно в строковые и также переставляет запятую до комментариев
        /// </summary>
        private static string TransformBlockToLineComments(string json)
        {
            var regexWithComma = new Regex(@"/\*(.+)\*/\s*\,\s*$"); //блоковые комменты одной строкой, после которых запятая
            var regexWithoutComma = new Regex(@"/\*(.+)\*/"); //блоковые комменты одной строкой
            var regexBlockCommentRemains = new Regex(@"\*/\s*/\*"); //остаток после замены: разделители между 2 блоковыми рядом
            var stringWriter = new StringWriter();
            using (var stringReader = new StringReader(json))
            {
                while (true)
                {
                    var str = stringReader.ReadLine();
                    if (str == null)
                        break;

                    if (regexWithComma.IsMatch(str))
                    {
                        str = regexWithComma.Replace(str, ", //$1");
                        if (regexBlockCommentRemains.IsMatch(str))
                            str = regexBlockCommentRemains.Replace(str, "\n//"); //TODOM добавлять начальные пробелы из предыдущей строки
                    }
                    else
                    {
                        if (regexWithoutComma.IsMatch(str))
                        {
                            str = regexWithoutComma.Replace(str, "//$1");

                            if (regexBlockCommentRemains.IsMatch(str))
                                str = regexBlockCommentRemains.Replace(str, "\n//");
                        }
                    }

                    stringWriter.WriteLine(str);
                }
            }

            return stringWriter.ToString();
        }

        private static string GetResourceFileFullPath(string repositoryPath, string assetRelPath, string extension = "jdb")
        {
            if (assetRelPath.StartsWith("Assets"))
                assetRelPath = assetRelPath.Remove(0, "Assets".Length);
            repositoryPath = repositoryPath.TrimEnd('/');
            assetRelPath = assetRelPath.TrimStart('/');
            var fullPath = Path.Combine(repositoryPath, assetRelPath);
            return Path.ChangeExtension(fullPath, extension);
        }


        //=== Class ===================================================================================================

        private class PropWatcher
        {
            public string HierPath { get; }

            public string ParentHierPath { get; }

            public bool IsPassed { get; set; }

            private string _propertyName;
            public string PropertyName => _propertyName;

            public PropWatcher(string hierPath)
            {
                HierPath = hierPath;
                ParentHierPath = GetParentHierPath(hierPath, out _propertyName);
            }

            /// <summary>
            /// Возвращает путь до свойства propertyName и само имя свойства. Разделители - точки
            /// </summary>
            private string GetParentHierPath(string hierPath, out string propertyName)
            {
                propertyName = "";
                if (string.IsNullOrEmpty(hierPath))
                {
                    Logger.IfError()?.Message($"{nameof(GetParentHierPath)}('{hierPath}') Empty {nameof(hierPath)}").Write();
                    return null;
                }

                var lastDotIndex = hierPath.LastIndexOf('.');

                if (lastDotIndex < 0)
                {
                    propertyName = HierPath;
                    return "";
                }

                propertyName = hierPath.Substring(lastDotIndex + 1);
                return hierPath.Remove(lastDotIndex);
            }

            public override string ToString()
            {
                return $"{nameof(PropWatcher)} {nameof(HierPath)}='{HierPath}', {nameof(ParentHierPath)}='{ParentHierPath}' " +
                       $"{nameof(IsPassed)}{IsPassed.AsSign()}";
            }
        }
    }
}