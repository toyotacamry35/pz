using ResourcesSystem.Loader;
using GeneratedCode.Custom.Config;
using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Assets.Src.ResourcesSystem.Base;
using Core.Environment.Logging.Extension;
using Newtonsoft.Json;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Aspects.Sessions;
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
#endif

namespace Assets.Src.Tools
{
    public static class CmdArgumentHelper
    {
        private static readonly NLog.Logger Logger = LogManager.GetLogger(nameof(CmdArgumentHelper));

        private const string AnyArgPrefix = "-";
        private const string StartPrefix = "-startup";
        private const string ConnectionSuffix = "-connection";
        private const string PlaySuffix = "-play";
        private const string AuthSuffix = "-auth";
        private const string PlatformPrefix = "-platform";

        public static string[] AdditionalCommandLineArguments
#if UNITY_EDITOR
        {
            get { return UniquePlayerPrefs.GetString("CommandLineArgs")?.Split('\u001b'); }
            set
            {
                UniquePlayerPrefs.SetString(
                    "CommandLineArgs",
                    value != null && value.Length > 0 ? string.Join("\u001b", value) : string.Empty);
            }
        }
#else
        => null;
#endif

        /// <summary>
        /// Формат передачи в командной строке для ConnectionParams, например: -startup-connection ConnectionPort=1234 ConnectionAddress=...
        /// </summary>
        /// <param name="startupParams"></param>
        public static void FillStartupParams(StartupParams startupParams)
        {
            Logger.IfInfo()?.Message($"Command Line Received {string.Join(" ", GetCommandLineArgs().Select(s => $"\"{s}\""))}").Write();
            var connectionParams = new ConnectionParams();
            var playParams = new PlayParams();
            var platformParams = new PlatformParams();
            try
            {
                FillMembers(ref startupParams, StartPrefix);
                FillMembers(ref connectionParams, StartPrefix + ConnectionSuffix);
                FillMembers(ref playParams, StartPrefix + PlaySuffix);
                FillMembers(ref connectionParams, StartPrefix + AuthSuffix);
                FillMembers(ref platformParams, PlatformPrefix);
            }
            catch (Exception e)
            {
                Logger.IfError()?.Message($"Error In Command Line Params {e}").Write();
            }

            startupParams.ConnectionParams = connectionParams;
            startupParams.PlayParams = playParams;
            startupParams.PlatformParams = platformParams;
        }

        private static void FillMembers<T>(ref T target, string startPrefix)
        {
            var type = typeof(T);
            object boxed = target;
            const BindingFlags bindingFlags =
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            var capture = false;
            foreach (var s in GetCommandLineArgs().Skip(1))
            {
                if (s.StartsWith(AnyArgPrefix))
                {
                    capture = s == startPrefix;
                    continue;
                }

                if (!capture) continue;

                var pair = s.Split('=');
                var key = pair[0];
                var value = pair.Length > 1 ? pair[1] : null;

                var field = type.GetField(key, bindingFlags);
                if (field != null)
                {
                    field.SetValue(boxed, ChangeType(value, field.FieldType));
                    Logger.IfInfo()?.Message($"Received Command Line {type} Field Arg: \"{field.Name}={field.GetValue(boxed)}\"").Write();
                }
                else
                {
                    var prop = type.GetProperty(key, bindingFlags);
                    if (prop != null)
                    {
                        prop.SetValue(boxed, ChangeType(value, prop.PropertyType));
                        Logger.IfInfo()?.Message($"Received Command Line {type} Property Arg: \"{prop.Name}={prop.GetValue(boxed)}\"").Write();
                    }
                    else
                        throw new Exception($"Unknown Class {type} Member {key} = \"{value}\"");
                }
            }

            target = (T) boxed;
        }

        public static (string IP, int port, RealmRulesQueryDef realmRulesQuery, bool player) GetConnectParams()
        {
            var args = GetCommandLineArgs().FirstOrDefault(x => x.StartsWith("-connect"));
            if (args == null)
                throw new InvalidOperationException("Cant get connection params");

            var connectArg = args.Split(':');
            try
            {
                var IP = connectArg[1];
                var port = int.Parse(connectArg[2]);
                var realmRulesQueryPath = connectArg[3];
                var realmRulesQuery = GameResourcesHolder.Instance.LoadResource<RealmRulesQueryDef>(realmRulesQueryPath);
                if (realmRulesQuery == null)
                     Logger.IfError()?.Message("Connect bot realm rules query {0} not found",  realmRulesQueryPath).Write();
                var player = (connectArg[4] == "true") ? true : false;
                return (IP, port, realmRulesQuery, player);
            }
            catch (Exception e)
            {
                Logger.IfError()?.Message(e, "Exception occured during parsing of -connect command line args", string.Join(",", connectArg)).Write();
                throw;
            }
        }

        public static BotConnectionParams[] GetBotParams()
        {
            var botArgs = GetCommandLineArgs().Where(x => x.StartsWith("-bot"));
            if (botArgs.Count() <= 0)
                throw new InvalidOperationException();

            try
            {
                var bcparams = new List<BotConnectionParams>();

                foreach (var botArg in botArgs)
                {
                    var botArgSplitted = botArg.Split(':');
                    bcparams.Add(
                        new BotConnectionParams
                        {
                            _count = int.Parse(botArgSplitted[1]),
                            _botBrainDefPath = botArgSplitted[2],
                            _spawnPoint = botArgSplitted[3]
                        });
                }

                return bcparams.ToArray();
            }
            catch (Exception e)
            {
                Logger.IfError()?.Message(e, "Exception occured during parsing of -bot command line args").Write();
                throw;
            }
        }

        public static ResourceRef<WorldConstantsResource> GetConstantsParams(ResourceRef<WorldConstantsResource> settings)
        {
            var args = GetCommandLineArgs().Where(x => x.StartsWith("-constants")).SelectMany(x => x.Split(':').Skip(1)).ToArray();
            Logger.IfDebug()?.Message($"Cmd line:{string.Join(" ", args)}").Write();
            try
            {
                var regex = new Regex(@"\s*(\w+)\s*=\s*(.*)\s*");
                var opts = args.Select(x => x.Trim()).Where(x => regex.IsMatch(x)).ToArray();
                var cfgName = args.Except(opts).FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(cfgName))
                {
                    if (!cfgName.Contains('/') && !cfgName.Contains('\\'))
                        cfgName = "/Constants/" + cfgName;
                    Logger.IfDebug()?.Message("ConstantsFile:{0}", cfgName).Write();
                    settings = new ResourceRef<WorldConstantsResource>(cfgName);
                }

                var settingsType = settings.GetType().GetGenericArguments()[0];
                foreach (var opt in opts)
                {
                    try
                    {
                        var match = regex.Match(opt);
                        if (!match.Success)
                            throw new Exception($"Bad constant definition {opt}");
                        var propName = match.Groups[1].Value;
                        var propValue = match.Groups[2].Value;
                        Logger.IfDebug()?.Message("propName:{0} propValue:{1}", propName, propValue).Write();
                        var field = settingsType.GetField(
                            propName,
                            BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                        if (field != null)
                            field.SetValue(settings.Target, ChangeType(propValue, field.FieldType));
                        else
                        {
                            var prop = settingsType.GetProperty(
                                propName,
                                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                            if (prop != null)
                                prop.SetValue(settings.Target, ChangeType(propValue, prop.PropertyType));
                            else
                                throw new Exception($"Unknown constant {opt}");
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.IfError()?.Message(e.Message).Write();
                    }
                }

                if (Logger.IsDebugEnabled)
                    Logger.IfDebug()
                        ?.Message(
                            "Constants:\n" +
                            string.Join(
                                "\n",
                                settings.Target.GetType().GetProperties().Select(x => $"{x.Name}={x.GetValue(settings.Target)}")))
                        .Write();

                return settings;
            }
            catch (Exception e)
            {
                Logger.IfError()?.Message($"Exception during arg parsin{e.ToString()}").Write();
                Logger.Log(
                    LogLevel.Error,
                    $"Exception occured during parsing of -constants command line args [{string.Join(" ", args)}]",
                    e);
                return settings;
            }
        }


        private static object ChangeType(string value, Type type)
        {
            if (value == null) return Activator.CreateInstance(type);
            return type.IsEnum
                ? Enum.Parse(type, (string) Convert.ChangeType(value, typeof(string)), true)
                : Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
        }

        private static IEnumerable<string> GetCommandLineArgs()
        {
            return AdditionalCommandLineArguments?.Length > 0
                ? Environment.GetCommandLineArgs().Concat(AdditionalCommandLineArguments.Select(x => x.Trim()))
                : Environment.GetCommandLineArgs();
        }
    }

    public class BotConnectionParams
    {
        public int _count;
        public string _botBrainDefPath;
        public string _spawnPoint;
    }
}
