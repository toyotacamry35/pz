using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

public static partial class Extensions
{
    private static readonly IReadOnlyDictionary<string, string> systemTypeNameToNiceName = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>()
        {
            {"Boolean", "bool"},
            {"Int32", "int"},
            {"Single", "float"},
            {"Int64", "long"},
            {"Int16", "short"},
            {"UInt32", "uint"},
            {"UInt64", "ulong"},
            {"UInt16", "ushort"},
            {"String", "string"}
        });

    public static string NiceName(this Type type)
    {
        return NiceNameImpl(type, false);
    }
    
    public static string FullNiceName(this Type type)
    {
        return NiceNameImpl(type, true);
    }

    private static string NiceNameImpl(Type type, bool fullName)
    {
        if (type == null)
        {
            return "";
        }

        if (type.IsArray)
        {
            var element = NiceNameImpl(type.GetElementType(), fullName);
            return element + "[]";
        }

        if (!type.IsGenericType)
        {
            if (systemTypeNameToNiceName.TryGetValue(type.Name, out var val))
                return val; // No ns

            return fullName ? type.FullName : type.Name;
        }

        var typeName = (fullName ? type.FullName : type.Name) ?? type.Name;
        var apostropheFirstIndex = typeName.IndexOf('`');
        var sb = new StringBuilder(typeName.Substring(0, apostropheFirstIndex));

        sb.Append("<");
        var genericArgs = type.GetGenericArguments();
        for (int i = 0; i < genericArgs.Length; i++)
        {
            if (i > 0)
            {
                sb.Append(", ");
            }
            sb.Append(NiceNameImpl(genericArgs[i], fullName));
        }

        sb.Append(">");
        return sb.ToString();
    }

}
