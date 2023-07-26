using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text;
using System;
using System.Linq;

namespace Assets.Src.Cartographer.Editor
{
    public class CartographerAssetHelper
    {
        public class AssetUsage
        {
            public string Name = string.Empty;

            public int Connected = 0;
            public int Disconnected = 0;
            public int Similar = 0;

            public int Total { get { return Connected + Disconnected + Similar; } }
            public void AddUsage(PrefabInstanceStatus status)
            {
                if (status == PrefabInstanceStatus.Connected)
                {
                    ++Connected;
                }
                else if (status == PrefabInstanceStatus.Disconnected)
                {
                    ++Disconnected;
                }
                else
                {
                    ++Similar;
                }
            }
        }

        public class AssetUsageEx : AssetUsage
        {
            public List<string> Assets = new List<string>();
        }

        public static void PrintList(List<string> lines, List<string> list, string label, bool sort, bool addEmptyLine)
        {
            if (addEmptyLine) { lines.Add($""); }
            lines.Add($"--- {label}:");
            if (sort) { list.Sort((x, y) => x.CompareTo(y)); }
            foreach (var element in list)
            {
                lines.Add($"{element}");
            }
        }

        public static void PrintAssetPaths(List<string> lines, List<AssetUsage> paths, string label, bool sort, bool addEmptyLine)
        {
            if (addEmptyLine) { lines.Add($""); }
            lines.Add($"--- {label}:");
            if (sort) { paths.Sort((x, y) => (x.Total == y.Total ? x.Name.CompareTo(y.Name) : y.Total.CompareTo(x.Total))); }
            foreach (var path in paths)
            {
                lines.Add($"{path.Total} {path.Connected}/{path.Disconnected}/{path.Similar}, {path.Name}");
            }
        }

        public static void PrintAssets(List<string> lines, Dictionary<string, AssetUsageEx> assets, bool checkLODs, string label, bool sort, bool addEmptyLine, ICartographerProgressCallback progressCallback)
        {
            if (addEmptyLine) { lines.Add($""); }
            lines.Add($"--- {label}:");
            lines.Add($"<Total Usage> <Connected/Disconnected/Similar>, <Prefab Name>, <Prefabs Found>, <Prefab Paths:LODGroups(<count>)/<lod count>/..../<lod count>>");
            var list = assets.ToList();
            //if (sort) { list.Sort((x, y) => ((x.Value.Assets.Count == y.Value.Assets.Count) ? (x.Value.Total == y.Value.Total ? x.Key.CompareTo(y.Key) : y.Value.Total.CompareTo(x.Value.Total)) : y.Value.Assets.Count.CompareTo(x.Value.Assets.Count))); }
            if (sort) { list.Sort((x, y) => ((x.Value.Total == y.Value.Total) ? (x.Value.Assets.Count == y.Value.Assets.Count ? x.Key.CompareTo(y.Key) : y.Value.Assets.Count.CompareTo(x.Value.Assets.Count)) : y.Value.Total.CompareTo(x.Value.Total))); }
            var stringBuilder = new StringBuilder();

            var count = 0;
            foreach (var element in list)
            {
                count += element.Value.Assets.Count;
            }
            var index = 0;
            foreach (var element in list)
            {
                stringBuilder.Clear();
                if (sort) { element.Value.Assets.Sort((x, y) => (x.CompareTo(y))); }
                foreach (var path in element.Value.Assets)
                {
                    CartographerCommon.AppendIfNotEmpty(stringBuilder, ", ");
                    stringBuilder.Append(path);
                    if (checkLODs)
                    {
                        ++index;
                        progressCallback?.OnProgress("Check Prefab LOD groups", $"Prefab: {index} / {count},  {element.Key}", index * 1.0f / (count + 1.0f));
                        if (element.Value.Total > 0)
                        {
                            GameObject prefab = null;
                            try
                            {
                                prefab = PrefabUtility.LoadPrefabContents(path);
                            }
                            catch (Exception e)
                            {
                            }
                            if (prefab != null)
                            {
                                var lodGroups = prefab.GetComponentsInChildren<LODGroup>(true);
                                var lodGroupsCount = lodGroups?.Length ?? 0;
                                stringBuilder.Append($":LODGroups({(lodGroupsCount == 0 ? "NONE" : lodGroupsCount.ToString())})");
                                if (lodGroups != null)
                                {
                                    foreach (var lodGroup in lodGroups)
                                    {
                                        stringBuilder.Append($"/{lodGroup.lodCount}");
                                    }
                                }
                                PrefabUtility.UnloadPrefabContents(prefab);
                            }
                            else
                            {
                                stringBuilder.Append($":NOTLOADED");
                            }
                        }
                    }
                }
                lines.Add($"{element.Value.Total} {element.Value.Connected}/{element.Value.Disconnected}/{element.Value.Similar}, {element.Key}, {element.Value.Assets.Count}, {stringBuilder}");
            }
        }
    }
};