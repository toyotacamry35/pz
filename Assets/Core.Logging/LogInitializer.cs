using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using Core.Environment.Logging.Extension;
using Core.Reflection;
using NLog;
using NLog.Config;
using NLog.LayoutRenderers;
using NLog.Targets;

namespace Core.Environment.Logging
{
    public static class LogInitializer
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly Type[] Attributes =
        {
            typeof(TargetAttribute),
            typeof(LayoutRendererAttribute)
        };

        static LogInitializer()
        {
            var interestingTypes = AppDomain.CurrentDomain.GetAssembliesSafe().SelectMany(v => v.GetTypesSafe()).Where(IsInterestingType).ToArray();
            var targets = interestingTypes.Where(type => type.CustomAttributes.Any(attr => attr.AttributeType == typeof(TargetAttribute)));
            var layoutRenderers = interestingTypes.Where(type => type.CustomAttributes.Any(attr => attr.AttributeType == typeof(LayoutRendererAttribute)));

            foreach (var target in targets)
            {
                var attr = target.GetCustomAttribute<TargetAttribute>(false);
                Target.Register(attr.Name, target);
            }

            foreach (var layoutRenderer in layoutRenderers)
            {
                var attr = layoutRenderer.GetCustomAttribute<LayoutRendererAttribute>(false);
                LayoutRenderer.Register(attr.Name, layoutRenderer);
            }
        }

        private static bool IsInterestingType(Type type) => type.CustomAttributes.Select(v => v.AttributeType).Any(attr => Attributes.Any(attr2 => attr2 == attr));

        public static void InitLogger(FileInfo logConfigFile)
            => InitLogger(logConfigFile, new DirectoryInfo(Path.Combine(System.Environment.CurrentDirectory, "Logs")));

        public static void InitLogger(FileInfo logConfigFile, DirectoryInfo logDirectory)
        {
            if (!logConfigFile.Exists)
                throw new FileNotFoundException("Cant find log config file", logConfigFile.FullName);

            using(var stream = File.OpenRead(logConfigFile.FullName))
            using (var reader = new StreamReader(stream))
                InitLogger(reader, logConfigFile, logDirectory);
        }

        public static void InitLogger(TextReader configStreamReader, FileInfo logConfigFile, DirectoryInfo logDirectory)
        {
            using (var xmlReader = XmlReader.Create(configStreamReader))
                LogManager.Configuration = new XmlLoggingConfiguration(xmlReader, logConfigFile?.FullName);
            LogManager.Configuration.Variables["basedir"] = logDirectory.FullName;

            Logger.IfInfo()?.Message("Log system is initialized with config '{logConfigFile}'", logConfigFile?.FullName ?? "unknown").Write();
        }
    }
}
