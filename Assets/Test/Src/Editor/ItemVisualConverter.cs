using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Src.GameObjectAssembler;
using Assets.Src.Inventory;
using Assets.Src.ResourcesSystem.Base;
using Assets.Src.ResourceSystem;
using EnumerableExtensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog.Fluent;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Wizardry;
using UnityEditor;
using UnityEngine;

namespace Src.Test
{
    public static class ItemVisualConverter
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        //[MenuItem("Build/Internal/Check JdbMetadata integrity")]
        public static void Check()
        {
            Debug.Log("Start check...");
            var paths = AssetDatabase.FindAssets("t:jdbmetadata")
                .Select(AssetDatabase.GUIDToAssetPath)
                .ToArray();

            int okCount = 0;
            for (int i = 0, len = paths.Length; i < len; i++)
            {
                try
                {
                    var baseResource = ((JdbMetadata) AssetDatabase.LoadMainAssetAtPath(paths[i])).Get<BaseResource>();
                    if (baseResource != null)
                        okCount++;
                    else
                        Debug.LogError($"BaseResource is null: '{paths[i]}'");
                }
                catch (Exception e)
                {
                    Debug.LogError($"Exception of loading for '{paths[i]}': {e}");
                }
            }

            Debug.Log($"Items successfully loaded: {okCount} from {paths.Length}");
        }

        //[MenuItem("Build/Internal/Convert Items")]
        /// <summary>
        /// ����� ��������, ���������� ItemResourceVisual, 
        /// </summary>
        public static void Do()
        {
//            Debug.Log("Gathering items...");
//            var items = AssetDatabase.FindAssets("t: GameObject")
//                .Select(AssetDatabase.GUIDToAssetPath)
//                .Select(v => (GameObject) AssetDatabase.LoadMainAssetAtPath(v))
//                .Select(v => v.GetComponent<ItemResourceVisual>())
//                .Where(v => v != null)
//                .ToArray();
//
//            Debug.Log("Start converting...");
//            foreach (var item in items)
//            {
//                ConvertUIProps(item);
//                ConvertConsumableDef(item);
//            }
//
//            foreach (var item in items)
//            {
//                RemoveConstructionComponent(item);
//            }
//
//            Debug.Log("End converting");
        }

        //[MenuItem("Build/Internal/Convert Consumables")]
        public static void ConvertConsumables()
        {
            //ConvertConsumables_ConsumableComponents();
            //ConvertConsumables_ItemResources();
            ConvertConsumables_SingleSpellToGroups();
        }

        /// <summary>
        /// ����� jdbmetadata � ItemResource, ������� ������ ����� ������
        /// </summary>
        private static void ConvertConsumables_ItemResources()
        {
            Debug.Log("Gathering ItemResources with consumable data...");
            var resourceTokens = AssetDatabase.FindAssets("t:jdbmetadata")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(LoadAsToken<BaseItemResource>)
                .Where(token => token != null)
                .ToArray();

            Debug.Log($"resourceTokens count: {resourceTokens.Length}");

            int convertedCount = 0;
//            foreach (var token in resourceTokens)
//            {
//                if (token.Token[nameof(ItemResource.OldConsumableData)] != null)
//                {
//                    convertedCount++;
//                    token.Token[nameof(ItemResource.ConsumableData)] = token.Token[nameof(ItemResource.OldConsumableData)];
//                    token.Token[nameof(ItemResource.OldConsumableData)].Parent.Remove();
//                    SaveJdb(token.Path, token.Token);
//                }
//            }

            Debug.Log($"Converted ItemResources count: {convertedCount}");
        }

        /// <summary>
        /// ����� jdbmetadata �� ����� �����, ������ ������ �� ������� ������ (ConsumableDef), �������� �� ��� ������ (ConsumDef), ���������� �����
        /// </summary>
        private static void ConvertConsumables_ConsumableComponents()
        {
            Debug.Log("Gathering ConsumableComponents...");
            var jdbPaths = AssetDatabase.FindAssets("t:jdbmetadata")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(path => path.EndsWith("-Consumable.jdb"))
                //                .First();
                .ToArray();

            foreach (var assetPath in jdbPaths)
            {
                Debug.Log($"Item '{assetPath}'");
                ConvertConsumableDefToConsumDefInJdb(assetPath);
            }
        }

        private static void ConvertConsumables_SingleSpellToGroups()
        {
            Debug.Log("Gathering ConsumableComponents...");
            var jdbPaths = AssetDatabase.FindAssets("t:jdbmetadata")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(path => path.EndsWith("-Consumable.jdb"));
            //.First();
            //Debug.Log($"Item: {jdbPaths}");
            foreach (var jdbPath in jdbPaths)
                ConvertSingleSpellToGroups(jdbPath);
        }


        private static void ConvertSingleSpellToGroups(string jdbAssetPath)
        {
//            var fsPath = GetFsPathFromAssetPath(jdbAssetPath);
//            var token = LoadJdb(fsPath);
//            var spellsGroups = new SpellsGroup[1];
//            spellsGroups[0].ActionTitle = token.SelectToken("ActionTitle").Value<string>();
//            token.SelectToken("ActionTitle").Parent.Remove();
//            spellsGroups[0].Spells = new ResourceRef<SpellDef>[0];
//            List<string> spellsList = new List<string>();
//            var spell = token.SelectToken("Spell").Value<string>();
//            if (!string.IsNullOrEmpty(spell))
//            {
//                spellsList.Add(spell);
//                token.SelectToken("Spell").Parent.Remove();
//            }
//
//            spell = token.SelectToken("Spell2")?.Value<string>();
//            if (!string.IsNullOrEmpty(spell))
//            {
//                spellsList.Add(spell);
//                token.SelectToken("Spell2").Parent.Remove();
//            }
//
//            //Debug.Log($"============ {jdbAssetPath}: {spellsList.ItemsToString()}");
//
//            spellsGroups[0].Spells = new ResourceRef<SpellDef>[spellsList.Count];
//
//            var newToken = JToken.FromObject(spellsGroups);
//
//            var spellsToken = newToken.First[nameof(SpellsGroup.Spells)];
//            var spellsArray = (JArray) spellsToken;
//
//            List<JToken> toRemove = new List<JToken>();
//            for (int i = 0; i < spellsArray.Count; i++) //������� ����� � ���� �������� � �������� ���������� Base-�������
//                toRemove.Add(spellsArray[i]);
//            foreach (var jToken in toRemove)
//                jToken.Remove();
//
//            foreach (var spellPath in spellsList)
//                spellsArray.Add(JToken.FromObject(spellPath));
//
//            token[nameof(ConsumDef.SpellsGroups)] = newToken;
//            SaveJdb(fsPath, token);
        }

        private static string GetFsPathFromAssetPath(string assetPath, string extension = "jdb")
        {
            if (assetPath.StartsWith("Assets"))
                assetPath = assetPath.Remove(0, "Assets".Length);
            var fsPath = Application.dataPath + assetPath;
            fsPath = Path.ChangeExtension(fsPath, extension);
            return fsPath;
        }

        private static void ConvertConsumableDefToConsumDefInJdb(string jdbAssetPath)
        {
            var fsPath = GetFsPathFromAssetPath(jdbAssetPath);
            var oldToken = LoadJdb(fsPath);
            var oldObject = oldToken as JObject;
            if (oldObject.AssertIfNull(nameof(oldObject)))
                return;

            var newToken = JToken.FromObject(new object());
            newToken["$type"] = nameof(ConsumDef);
            newToken["ActionTitle"] = oldObject.SelectToken("ActionTitle").Value<string>();
            newToken["ChargesMax"] = oldObject.SelectToken("ChargesMax")?.Value<int>() ?? 0;

            //�������� ���� � ������� �� ������:
            //  ConsumableDef/InteractiveDef "Interactive"/ContextualActionsDef "ContextualActions"/
            //  Dictionary<string, Dictionary<int, ContextualActionDef>> "Spell"/SpellDef "Spell"
            var spellDictToken = oldObject.SelectToken("Interactive")?.SelectToken("ContextualActions")
                ?.SelectToken("SpellsByAction")?.First?.First;

            if (!spellDictToken.AssertIfNull(nameof(spellDictToken)))
            {
                for (int i = 0; i < 2; i++)
                {
                    var spellObject = spellDictToken[i.ToString()] as JObject;
                    if (spellObject != null)
                    {
                        var spellPath = spellObject.GetValue("Spell")?.ToString();
                        if (!string.IsNullOrEmpty(spellPath))
                        {
                            Debug.Log($"spellPaths[{i}]='{spellPath}'");
                            if (i == 0)
                                newToken["Spell"] = spellPath;
                            else
                                newToken["Spell2"] = spellPath;
                        }
                    }
                }
            }

            SaveJdb(fsPath, newToken);
        }

//        /// <summary>
//        /// ������� ������ � �������� �� ItemResourceVisual � jdb � ItemResource ����� JToken
//        /// </summary>
//        private static void ConvertUIProps(ItemResourceVisual vis)
//        {
//            var fullPath = GetFsPathFromAssetPath(AssetDatabase.GetAssetPath(vis));
//            JToken jobj = LoadJdb(fullPath);
//
//            /*var iconPath = AssetDatabase.GetAssetPath(vis.Icon);
//            if(!string.IsNullOrWhiteSpace(iconPath))
//                jobj["Icon"] = iconPath;
//
//            var bigIconPath = AssetDatabase.GetAssetPath(vis.BigIcon);
//            if (!string.IsNullOrWhiteSpace(bigIconPath))
//                jobj["BigIcon"] = bigIconPath;
//
//            string titleIcon = AssetDatabase.GetAssetPath(vis.TitleIcon);
//            if (!string.IsNullOrWhiteSpace(titleIcon))
//                jobj["TitleIcon"] = titleIcon;
//
//            string animLayer = vis.CharacterAnimationLayer;
//            if (!string.IsNullOrWhiteSpace(animLayer))
//                jobj["CharacterAnimationLayer"] = animLayer;
//
//            int attackType = vis.CharacterAttackType;
//            if (attackType != 0)
//                jobj["CharacterAttackType"] = attackType;*/
//
//            SaveJdb(fullPath, jobj);
//        }

//        /// <summary>
//        /// ��������� jdb ����������� (Components) �� ������ � ItemResourceVisual, ��������� Construction � ��������� jdb 
//        /// � ���������� ������ �� ���� � jdb ItemResource.ConstructionData
//        /// </summary>
//        private static void ConvertConsumableDef(ItemResourceVisual vis)
//        {
//            var jsonHolder = vis.GetComponent<JsonRefHolder>();
//            if (jsonHolder == null)
//                return;
//
//            var fullPathSrcDef = Application.dataPath + jsonHolder.Ref + ".jdb";
//            if (!File.Exists(fullPathSrcDef))
//            {
//                Logger.Error().Message($"JsonRefHolder on '{AssetDatabase.GetAssetPath(vis)}' has broken ref: '{jsonHolder.Ref}'")
//                    .UnityObj(vis).Write();
//                return;
//            }
//
//            var srcObj = LoadJdb(fullPathSrcDef);
//
//            var constrObj = GetAndRemoveComponent<ConsumDef>(srcObj); //���� <ConsumableDef>
//            if (constrObj == null)
//                return;
//
//            var fullPathConstrDef = Application.dataPath + jsonHolder.Ref + "-Construction.jdb";
//            SaveJdb(fullPathConstrDef, constrObj);
//
//            var path = AssetDatabase.GetAssetPath(vis).Remove(0, "Assets".Length);
//            path = path.Remove(path.Length - ".prefab".Length);
//
//            var fullPathItemDef = Application.dataPath + path + ".jdb";
//            var itemDef = LoadJdb(fullPathItemDef);
//            itemDef["ConstructionData"] = jsonHolder.Ref + "-Construction";
//            SaveJdb(fullPathItemDef, itemDef);
//        }

//        private static void RemoveConstructionComponent(ItemResourceVisual vis)
//        {
//            var jsonHolder = vis.GetComponent<JsonRefHolder>();
//            if (jsonHolder == null)
//                return;
//
//            var fullPathSrcDef = Application.dataPath + jsonHolder.Ref + ".jdb";
//            if (!File.Exists(fullPathSrcDef))
//            {
//                Logger.Error().Message($"JsonRefHolder on '{AssetDatabase.GetAssetPath(vis)}' has broken ref: {jsonHolder.Ref}")
//                    .UnityObj(vis).Write();
//                return;
//            }
//
//            var srcObj = LoadJdb(fullPathSrcDef);
//
////            var constrObj = GetAndRemoveComponent<ConstructionDef>(srcObj);
////            if (constrObj == null)
////                return;
//
//            SaveJdb(fullPathSrcDef, srcObj);
//        }

        private static JToken GetComponent<T>(JToken def)
        {
            var arr = def["Components"] as JArray;

            for (int i = 0; i < arr.Count; ++i)
                if (arr[i]["$type"].ToString() == typeof(T).Name)
                    return arr[i];
            return null;
        }

        private static JToken GetAndRemoveComponent<T>(JToken def)
        {
            var arr = def["Components"] as JArray;

            if (arr == null)
                return null;

            for (int i = 0; i < arr.Count; ++i)
                if (arr[i]["$type"].ToString() == typeof(T).Name)
                {
                    var res = arr[i].DeepClone();
                    arr.RemoveAt(i);
                    return res;
                }

            return null;
        }

        public static JToken LoadJdb(string fullPath)
        {
            using (var textReader = File.OpenText(fullPath))
            using (var reader = new JsonTextReader(textReader))
            {
                return JToken.Load(reader);
            }
        }

        public static void SaveJdb(string fullPath, JToken obj)
        {
            using (var textWriter = File.CreateText(fullPath))
            using (var writer = new JsonTextWriter(textWriter))
            {
                writer.Formatting = Formatting.Indented;
                obj.WriteTo(writer);
            }
        }

        private static TokenWithPath LoadAsToken<T>(string assetPath) where T : BaseResource
        {
            var fsPath = GetFsPathFromAssetPath(assetPath);
            var token = LoadJdb(fsPath);
            if (token.IsNullOrEmpty() ||
                !token.HasValues ||
                token["$type"].ToString() != typeof(T).Name)
                return null;

            return new TokenWithPath() {Path = fsPath, Token = token};
        }

        private class TokenWithPath
        {
            public string Path;
            public JToken Token;
        }
    }
}