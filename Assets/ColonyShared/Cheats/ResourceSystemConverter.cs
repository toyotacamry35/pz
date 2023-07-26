using Assets.Src.ResourcesSystem.Base;
using Core.Cheats;
using ResourcesSystem.Loader;
using System;
using System.Reflection;
using System.Text.RegularExpressions;

namespace GeneratedCode.Cheats
{
    internal class ResourceSystemConverter : ICheatArgConverter
    {
        private static readonly Regex DefSplitter = new Regex("^(.*/Assets|Assets|)(?<Jdb>[\\w/]+(:\\d+|)(:\\d+|)(:\\d+|))(\\.jdb|)$");
        public bool CanConvert(Type targetType)
        {
            return typeof(IResource).IsAssignableFrom(targetType);
        }

        public object Convert(string value, ParameterInfo parameter)
        {
            value = value.Replace('\\', '/');
            var match = DefSplitter.Match(value);
            if (match == null)
                throw new ArgumentException($"Cant convert string '{parameter}' to Jdb path {parameter}", parameter.Name);
            var path = match.Groups["Jdb"].Value;
            var resource = GameResourcesHolder.Instance.LoadResource(path, parameter.ParameterType);
            if (resource == null)
                throw new ArgumentException($"Cant load resource of type {parameter.ParameterType} by path '{path}'", parameter.Name);

            return resource;
        }
    }
}
