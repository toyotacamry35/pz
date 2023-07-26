using Assets.Src.ResourcesSystem.Base;
using ResourcesSystem.Loader;
using NLog;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Core.Environment.Logging.Extension;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ResourceSystemTools
{
    public class ResourceIndexator
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private static JObject Load(ILoader loader, string relativePath)
        {
            using (var file = loader.OpenRead(relativePath))
            using (var streamReader = new StreamReader(file))
            using (var jsonTextReader = new JsonTextReader(streamReader))
            {
                var ls = new JsonLoadSettings();
                ls.CommentHandling = CommentHandling.Load;
                ls.DuplicatePropertyNameHandling = DuplicatePropertyNameHandling.Ignore;
                return JObject.Load(jsonTextReader, ls);
            }
        }

        private static void Save(ILoader loader, string relativePath, JToken obj)
        {
            using (var file = loader.OpenWrite(relativePath))
            using (var streamWriter = new StreamWriter(file))
            using (var jsonTextWriter = new JsonTextWriter(streamWriter))
            {
                jsonTextWriter.Formatting = Formatting.Indented;
                obj.WriteTo(jsonTextWriter);
            }
        }

        private static string RunGitProcess(DirectoryInfo gitRepoPath, string arguments)
        {
            var psi = new System.Diagnostics.ProcessStartInfo()
            {
                FileName = "git.exe",
                WorkingDirectory = gitRepoPath.FullName,
                Arguments = arguments,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var proc = System.Diagnostics.Process.Start(psi))
            {
                var ms = new MemoryStream();
                var res = proc.StandardOutput.ReadToEnd();

                proc.WaitForExit();
                if (proc.ExitCode != 0)
                    throw new Exception($"Proecess {psi} has exited with code {proc.ExitCode}");

                return res;
            }
        }

        private static readonly Regex FindCommiterTime = new Regex("^committer-time (\\d+)$", RegexOptions.Multiline);

        private static DateTime GetLineModificationDate(DirectoryInfo resourceSystemRoot, string filePath, JToken token)
        {
            var line = ((IJsonLineInfo)token).LineNumber;
            var res = RunGitProcess(new DirectoryInfo(resourceSystemRoot.FullName), $"blame --line-porcelain -t -L {line},{line} .{filePath}.jdb");
            var timeStr = FindCommiterTime.Match(res).Groups[1].Value;
            var time = DateTimeOffset.FromUnixTimeSeconds(int.Parse(timeStr)).DateTime;
            return time;
        }

        private static (ConcurrentBag<(string root, JObject[] items)> withoutId, ConcurrentDictionary<Guid, ConcurrentBag<(string root, JToken location)>> guidToRoot) LoadResourceSystemIds(ILoader loader)
        {
            var knownSaveableTypes = KnownTypesCollector.KnownTypes.Where(v => typeof(ISaveableResource).IsAssignableFrom(v.Value)).ToDictionary(v => v.Key, v => v.Value);

            var allResources = loader.AllPossibleRoots;

            var guidToRoot = new ConcurrentDictionary<Guid, ConcurrentBag<(string root, JToken location)>>();

            var withoutIdsAll = new ConcurrentBag<(string root, JObject[] items)>();

            foreach (var relativePath in allResources)
            {
                var obj = Load(loader, relativePath);

                var withoutIds = (from token in obj.DescendantsAndSelf()
                                  let childObj = token as JObject
                                  where childObj != null
                                  where childObj.ContainsKey("$type")
                                  where knownSaveableTypes.ContainsKey(childObj["$type"].Value<string>())
                                  where !childObj.ContainsKey("Id")
                                  select childObj).ToArray();

                if(withoutIds.Any())
                    withoutIdsAll.Add((relativePath, withoutIds));

                var withIds = from token in obj.DescendantsAndSelf()
                              let childObj = token as JObject
                              where childObj != null
                              where childObj.ContainsKey("$type")
                              where knownSaveableTypes.ContainsKey(childObj["$type"].Value<string>())
                              where childObj.ContainsKey("Id")
                              let idVal = childObj["Id"]
                              select (token: idVal, guid: Guid.Parse(idVal.Value<string>()));

                foreach (var id in withIds)
                    guidToRoot.GetOrAdd(id.guid, _ => new ConcurrentBag<(string root, JToken location)>()).Add((relativePath, id.token));
            }

            return (withoutIdsAll, guidToRoot);

        }

        public static IEnumerable<(Guid id, string root)> GetIdList(ILoader loader)
        {
            var obj = Load(loader, "/SaveableIds");
            return (obj["Resources"] as JArray).Select(v => (Guid.Parse(v["Id"].Value<string>()), v["Root"].Value<string>()));
        }


#if UNITY_EDITOR
        [UnityEditor.MenuItem("Build/FIx JDB IDs %#v")]
        private static void FixAllResourcesUnity() => FixAllResources(new DirectoryInfo(UnityEngine.Application.dataPath));
#endif
        public static void FixAllResources(DirectoryInfo resourceSystemRoot)
        {
            var loader = new FolderLoader(resourceSystemRoot.FullName);

            var (withoutIds, guidToRoot) = LoadResourceSystemIds(loader);

            var duplicates = guidToRoot.Where(v => v.Value.Count > 1).Select(v => (v.Key, v.Value)).ToArray();

            if(!duplicates.Any() && !withoutIds.Any())
            {
                Logger.Info("No errors found");
                GenerateSaveableResourceIndex(loader, guidToRoot);
                return;
            }

            AddMissingIds(loader, withoutIds);
            FixDuplicateIds(resourceSystemRoot, loader, duplicates);
            Logger.IfInfo()?.Message("Resources fixed").Write();

            var (_, guidToRootAfterFix) = LoadResourceSystemIds(loader);
            GenerateSaveableResourceIndex(loader, guidToRootAfterFix);

        }

        private static void FixDuplicateIds(DirectoryInfo resourceSystemRoot, FolderLoader loader, (Guid Key, ConcurrentBag<(string root, JToken location)> Value)[] duplicates)
        {
            var idsToReplace = new ConcurrentDictionary<string, ConcurrentBag<JToken>>();

            var allInvovedRoots = duplicates.SelectMany(v => v.Value).Select(v => (Loc: v, Date: GetLineModificationDate(resourceSystemRoot, v.root, v.location))).ToDictionary(v => (v.Loc.root, ((IJsonLineInfo)v.Loc.location).LineNumber), v => v.Date);

            foreach (var (id, items) in duplicates)
            {
                {
                    var (root, location) = items.OrderBy(v => allInvovedRoots[(v.root, ((IJsonLineInfo)v.location).LineNumber)]).First();

                    var line = ((IJsonLineInfo)location).LineNumber;
                    Logger.IfInfo()?.Message($"Duplicate id found. Original: guid: {id} path: {root}:{line} modified on {allInvovedRoots[(root, line)]}").Write();
                }

                {
                    var toReplace = items.OrderBy(v => allInvovedRoots[(v.root, ((IJsonLineInfo)v.location).LineNumber)]).Skip(1);
                    foreach (var (root, location) in toReplace)
                    {
                        var line = ((IJsonLineInfo)location).LineNumber;
                        Logger.IfInfo()?.Message($"Duplicate id found. Will be replaced: guid: {id} path: {root}:{line} modified on {allInvovedRoots[(root, line)]}").Write();
                        idsToReplace.GetOrAdd(root, _ => new ConcurrentBag<JToken>()).Add(location);
                    }
                }
            }

            foreach (var pathToReplace in idsToReplace)
            {
                Logger.IfInfo()?.Message($"Replacing ids in file: {pathToReplace.Key} guids: {string.Join(", ", pathToReplace.Value)}").Write();

                var root = pathToReplace.Value.First().Root;

                foreach (var token in pathToReplace.Value)
                    token.Replace(Guid.NewGuid().ToString());

                Save(loader, pathToReplace.Key, root);
            }

        }

        private static void AddMissingIds(ILoader loader, ConcurrentBag<(string root, JObject[] items)> withoutIds)
        {
            foreach (var withoutId in withoutIds)
            {
                var root = withoutId.items.First().Root;

                foreach (var obj in withoutId.items)
                {
                    obj.Add(new JProperty("Id", Guid.NewGuid().ToString()));
                    var line = ((IJsonLineInfo)obj).LineNumber;
                    Logger.IfInfo()?.Message($"Missing id found: {withoutId.root}:{line}").Write();
                }
                Save(loader, withoutId.root, root);
            }
        }

        public static void GenerateSaveableResourceIndex(ILoader loader, ConcurrentDictionary<Guid, ConcurrentBag<(string root, JToken location)>> guidToRoot)
        {
            var rootObj = new JObject();
            rootObj["$type"] = "SaveableResourceIndex";
            var idArray = guidToRoot.Select(v => (v.Key, v.Value.Single().root)).OrderBy(v => v.root).ThenBy(v => v.Key).Select(v => new JObject() { new JProperty("Id", v.Key.ToString()), new JProperty("Root", v.root) });
            rootObj["Resources"] = new JArray(idArray);
            Save(loader, "/SaveableIds", rootObj);
            Logger.IfInfo()?.Message("Updated saveable id index").Write();
        }
    }
}
