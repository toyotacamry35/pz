using System;
using System.Collections.Generic;

namespace SharedCode.Serializers.Protobuf
{
    public static class TypeNameHelper
    {
        public static string GetFriendlyName(this Type type, bool fullName = false)
        {
            if (type.IsGenericType)
            {
                string friendlyName = fullName ? type.FullName : type.Name;
                int iBacktick = friendlyName.IndexOf('`');
                if (iBacktick > 0)
                {
                    friendlyName = friendlyName.Remove(iBacktick);
                }
                friendlyName += "<";
                Type[] typeParameters = type.GetGenericArguments();
                for (int i = 0; i < typeParameters.Length; ++i)
                {
                    string typeParamName = typeParameters[i].GetFriendlyName(fullName);
                    friendlyName += (i == 0 ? typeParamName : "," + typeParamName);
                }
                friendlyName += ">";
                return friendlyName.Replace("+", ".");
            }
            else
                return GetAlias(type, fullName);
        }

        private static readonly Dictionary<Type, string> Aliases =
            new Dictionary<Type, string>()
            {
                { typeof(byte), "byte" },
                { typeof(sbyte), "sbyte" },
                { typeof(short), "short" },
                { typeof(ushort), "ushort" },
                { typeof(int), "int" },
                { typeof(uint), "uint" },
                { typeof(long), "long" },
                { typeof(ulong), "ulong" },
                { typeof(float), "float" },
                { typeof(double), "double" },
                { typeof(decimal), "decimal" },
                { typeof(object), "object" },
                { typeof(bool), "bool" },
                { typeof(char), "char" },
                { typeof(string), "string" },
                { typeof(void), "void" }
            };

        public static string GetAlias(Type type, bool fullName)
        {
            string val = null;
            if (!Aliases.TryGetValue(type, out val))
                return fullName ? type.FullName.Replace("+", ".") : type.Name;
            return val;
        }
    }
}
