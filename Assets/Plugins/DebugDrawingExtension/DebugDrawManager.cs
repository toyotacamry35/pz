using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using Newtonsoft.Json;
using NLog;
using UnityEngine;

namespace Plugins.DebugDrawingExtension
{
    public partial class DebugDraw
    {
        public static class Manager
        {
            private static string ConfigPath => Path.Combine(Application.dataPath, @"DebugDraw.json");
            private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

            private static readonly Dictionary<string, DebugDraw> Drawers = new Dictionary<string, DebugDraw>();
            private static Dictionary<Regex, DebugDrawConfig.Rule> Rules = new Dictionary<Regex, DebugDrawConfig.Rule>();

            public static DebugDraw GetDrawer(string facility)
            {
                if (!Drawers.TryGetValue(facility, out var drawer))
                    Drawers.Add(facility, drawer = new DebugDraw(facility, Rules));
                return drawer;
            }

            public static void Initialize()
            {
                Rules = LoadRules(ConfigPath); 
                ConfigureDrawers(Drawers, Rules);
                var configWatcher = CreateConfigWatcher(ConfigPath, () =>
                    UnityQueueHelper.RunInUnityThreadNoWait(() =>
                    {
                        Rules = LoadRules(ConfigPath);
                        ConfigureDrawers(Drawers, Rules);
                    }));
                Application.quitting += () => configWatcher?.Dispose();
            }

            private static Dictionary<Regex, DebugDrawConfig.Rule> LoadRules(string path)
            {
                if (File.Exists(path))
                {
                    try
                    {
                        var data = File.ReadAllText(path);
                        var config = JsonConvert.DeserializeObject<DebugDrawConfig>(data);
                        var rules = config.Rules.ToDictionary(
                            x => new Regex("^" + Regex.Escape(x.Key).Replace(@"\*", ".*").Replace(@"\?", ".") + "$", RegexOptions.IgnoreCase),
                            y => y.Value);
                        Logger.IfInfo()?.Message($"Config {path} loaded").Write();
                        return rules;
                    }
                    catch (Exception e)
                    {
                        Logger.IfWarn()?.Exception(e).Write();
                    }
                }
                return new Dictionary<Regex, DebugDrawConfig.Rule>();
            }

            private static void ConfigureDrawers(Dictionary<string, DebugDraw> drawers, Dictionary<Regex, DebugDrawConfig.Rule> rules)
            {
                foreach (var drawer in drawers)
                    drawer.Value.ApplyRules(rules);
            }

            private static FileSystemWatcher CreateConfigWatcher(string path, Action updateAction)
            {
                FileSystemWatcher configWatcher = null;
                try
                {
                    configWatcher = new FileSystemWatcher
                    {
                        Path = Path.GetDirectoryName(path),
                        Filter = "*.json",
                        NotifyFilter = NotifyFilters.LastWrite
                    };
                    configWatcher.Changed += (o, e) => updateAction();
                    configWatcher.Created += (o, e) => updateAction();
                    configWatcher.Deleted += (o, e) => updateAction();
                    configWatcher.EnableRaisingEvents = true;
                }
                catch (Exception e)
                {
                    Logger.IfWarn()?.Exception(e).Write();
                }

                return configWatcher;
            }
        }
    }

    [UsedImplicitly]
    internal class DebugDrawConfig
    {
        [UsedImplicitly] public Dictionary<string, Rule> Rules; // string may be a wildcard, e.g. "*SomeFacility*", "?omeFacility"

        [UsedImplicitly]
        public class Rule
        {
            [UsedImplicitly] public DebugDrawLevel Level = DebugDrawLevel.None;
            [UsedImplicitly] public float Duration = 1;
            [UsedImplicitly] public bool DepthTest = false;
        }
    }
}