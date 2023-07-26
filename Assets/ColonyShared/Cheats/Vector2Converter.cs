using Core.Cheats;
using SharedCode.Utils;
using System;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;

namespace GeneratedCode.Cheats
{
    internal class Vector2Converter : ICheatArgConverter
    {
        private static readonly Regex Vec2Splitter = new Regex("(\\S+)\\s+(\\S+)");

        public bool CanConvert(Type targetType) => targetType == typeof(Vector2);

        public object Convert(string value, ParameterInfo parameter)
        {
            var match = Vec2Splitter.Match(value);
            if (match == null)
                throw new ArgumentException($"Cant convert string '{parameter}' to Vector2 {parameter}", parameter.Name);

            var xStr = match.Groups[1].Value;
            var yStr = match.Groups[2].Value;

            var x = float.Parse(xStr, CultureInfo.InvariantCulture);
            var y = float.Parse(yStr, CultureInfo.InvariantCulture);

            return new Vector2(x, y);
        }
    }
}
