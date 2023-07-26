using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Reflection;
using Newtonsoft.Json;

namespace SharedCode.Utils.BsonSerialization
{
    public static class TypeHashCalculatorHelper
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
                return fullName ? type.FullName.Replace("+", ".") : type.Name;
        }
    }
}