using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NLog;
using UnityEngine;

public static partial class Extensions
{
    private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Возвращает null == targetVar. При положительном результате сообщает подробности в Unity консоль
    /// </summary>
    public static bool AssertIfNull<T>(this T targetVar,
        string varName,
        GameObject context,
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0,
        [CallerMemberName] string callerMethodName = "")
        where T : class
    {
        bool res = targetVar == null;
        if (res)
        {
            Logger.Error((callerMethodName == ""
                             ? LogObjectExtensions.GetShortDescr(typeof(T), varName, "is null")
                             : LogObjectExtensions.GetFullDescr(typeof(T), varName, "is null", callerMethodName, callerFilePath,
                                 callerLineNumber))
                         + (context == null ? "" : $" '{context.transform.FullName()}'"),
                context);
        }

        return res;
    }

    /// <summary>
    /// Возвращает true, если collection == null, или пуста, или содержит нулевые элементы.
    /// И сообщает при первом положительном результате подробности в Unity консоль
    /// </summary>
    public static bool IsNullOrEmptyOrHasNullElements<T>(this ICollection<T> collection,
        string collectionName,
        GameObject context,
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0,
        [CallerMemberName] string callerMethodName = "")
    {
        bool res = collection == null;
        if (res)
        {
            Logger.Error(
                callerMethodName == ""
                    ? LogObjectExtensions.GetShortDescr(typeof(T), collectionName, "is null")
                    : LogObjectExtensions.GetFullDescr(typeof(T), collectionName, "is null", callerMethodName, callerFilePath,
                        callerLineNumber),
                context);
        }
        else
        {
            if (collection.Count == 0)
                res = true;
            if (res)
            {
                Logger.Error(
                    callerMethodName == ""
                        ? LogObjectExtensions.GetShortDescr(typeof(T), collectionName, "is empty")
                        : LogObjectExtensions.GetFullDescr(typeof(T), collectionName, "is empty", callerMethodName, callerFilePath,
                            callerLineNumber),
                    context);
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
                            ? LogObjectExtensions.GetShortDescr(typeof(T), collectionName, cause)
                            : LogObjectExtensions.GetFullDescr(typeof(T), collectionName, cause, callerMethodName, callerFilePath,
                                callerLineNumber),
                        context);
                }
            }
        }

        return res;
    }
}