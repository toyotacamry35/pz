using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NLog;

public static class LogObjectExtensions
{
    private static readonly NLog.Logger Logger = LogManager.GetLogger("Default");

    public static string AsSignedString(this int input, string format = "")
    {
        return input.ToString(format == "" ? "+0;-0;0" : $"+{format};-{format};{format}");
    }

    public static string AsSignedString(this double input, string format = "")
    {
        return input.ToString(format == "" ? "+0.0;-0.0;0" : $"+{format};-{format};{format}");
    }

    public static string AsSignedString(this float input, string format = "")
    {
        return ((double) input).AsSignedString(format);
    }

    public static string AsSign(this bool b)
    {
        return b ? "+" : "-";
    }

    /// <summary>
    /// Возвращает null == targetVar. При положительном результате сообщает подробности в Unity консоль
    /// </summary>
    public static bool AssertIfNull<T>(
        this T targetVar,
        string varName,
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0,
        [CallerMemberName] string callerMethodName = "")
        where T : class
    {
        bool res = targetVar == null;
        if (res)
        {
            Logger.Error(
                callerMethodName == ""
                    ? GetShortDescr(typeof(T), varName, "is null")
                    : GetFullDescr(typeof(T), varName, "is null", callerMethodName, callerFilePath, callerLineNumber));
        }

        return res;
    }

    public static bool AssertIfNull<T>(
        this ref T? targetVar,
        string varName,
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0,
        [CallerMemberName] string callerMethodName = "")
        where T : struct
    {
        bool res = targetVar == null;
        if (res)
        {
            Logger.Error(
                callerMethodName == ""
                    ? GetShortDescr(typeof(T), varName, "is null")
                    : GetFullDescr(typeof(T), varName, "is null", callerMethodName, callerFilePath, callerLineNumber));
        }

        return res;
    }

    /// <summary>
    /// Возвращает true, если collection == null, или пуста, или содержит нулевые элементы.
    /// И сообщает при первом положительном результате подробности в Unity консоль
    /// </summary>
    public static bool IsNullOrEmptyOrHasNullElements<T>(
        this ICollection<T> collection,
        string collectionName,
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0,
        [CallerMemberName] string callerMethodName = "")
    {
        bool res = collection == null;
        if (res)
        {
            Logger.Error(
                callerMethodName == ""
                    ? GetShortDescr(typeof(T), collectionName, "is null")
                    : GetFullDescr(typeof(T), collectionName, "is null", callerMethodName, callerFilePath, callerLineNumber));
        }
        else
        {
            if (collection.Count == 0)
                res = true;
            if (res)
            {
                Logger.Error(
                    callerMethodName == ""
                        ? GetShortDescr(typeof(T), collectionName, "is empty")
                        : GetFullDescr(typeof(T), collectionName, "is empty", callerMethodName, callerFilePath, callerLineNumber));
            }
            else
            {
                int i = 0;
                foreach (var elem in collection)
                {
                    if (elem == null)
                    {
                        res = true;
                        break;
                    }

                    i++;
                }

                if (res)
                {
                    var cause = $"element [{i}] is null";
                    Logger.Error(
                        callerMethodName == ""
                            ? GetShortDescr(typeof(T), collectionName, cause)
                            : GetFullDescr(typeof(T), collectionName, cause, callerMethodName, callerFilePath, callerLineNumber));
                }
            }
        }

        return res;
    }

    //=== not Private ==============================================================

    public static string GetShortDescr(Type type, string name, string cause)
    {
        return $"<{type.NiceName()}> '{name}' {cause}!";
    }

    public static string GetFullDescr(Type type, string name, string cause, string methodName, string filePath, int lineNumber)
    {
        return $"{methodName}(): <{type.NiceName()}> '{name}' {cause}!  {filePath}, {lineNumber}";
    }
}