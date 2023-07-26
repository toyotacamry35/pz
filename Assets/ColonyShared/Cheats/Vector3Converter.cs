using Core.Cheats;
using SharedCode.Utils;
using System;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;

namespace GeneratedCode.Cheats
{
    internal class Vector3Conv : ICheatArgConverter
    {
        private static readonly Regex Vec3Splitter = new Regex("(\\S+)\\s+(\\S+)\\s+(\\S+)");

        public bool CanConvert(Type targetType) => targetType == typeof(Vector3);

        public object Convert(string value, ParameterInfo parameter)
        {
            var match = Vec3Splitter.Match(value);
            if (match == null)
                throw new ArgumentException($"Cant convert string '{parameter}' to {parameter}", parameter.Name);

            var xStr = match.Groups[1].Value;
            var yStr = match.Groups[2].Value;
            var zStr = match.Groups[3].Value;

            var x = float.Parse(xStr, CultureInfo.InvariantCulture);
            var y = float.Parse(yStr, CultureInfo.InvariantCulture);
            var z = float.Parse(zStr, CultureInfo.InvariantCulture);

            return new Vector3(x, y, z);
        }
    }
}
