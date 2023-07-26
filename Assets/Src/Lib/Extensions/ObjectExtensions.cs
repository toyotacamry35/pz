using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static partial class Extensions
{
    /// <summary>
    /// After reaching that limit dumping of current collection elements is breaks and go to next element
    /// </summary>
    public const int CollectionElementsLimit = 200;


    //=== Public ==========================================================

    /// <summary>
    /// Returns string of object obj description. It dumps generics/not collections (IEnumerable, ICollection, IList, IDictionary), 
    /// and also multidimensional/jagged arrays
    /// </summary>
    /// <param name="title">Title before object</param>
    /// <param name="depth">Indent level</param>
    public static string VarDump(this object obj, string title, int depth = 0)
    {
        return obj.VarDumpWork(title, depth, false);
    }

    public static string VarDump(this object obj, int depth = 0)
    {
        return obj.VarDumpWork(null, depth, false);
    }

    /// <summary>
    /// Returns string of object obj description. It dumps generics/not collections (IEnumerable, ICollection, IList, IDictionary), 
    /// and also multidimensional/jagged arrays
    /// </summary>
    /// <param name="title">Title before object</param>
    /// <param name="depth">Indent level</param>
    public static string VarDumpVerbose(this object obj, string title, int depth = 0)
    {
        return obj.VarDumpWork(title, depth, true);
    }

    public static string VarDumpVerbose(this object obj, int depth = 0)
    {
        return obj.VarDumpWork(null, depth, true);
    }

    public static string ColonByCount(int count)
    {
        return count > 0 ? ":" : "";
    }


    //=== Private =========================================================

    private static string VarDumpWork(this object obj, string title, int depth, bool isVerbose)
    {
        var objectDumpLines = new List<DumpLine>();
        objectDumpLines.AddDumpLines(title, obj, depth, isVerbose);
        if (objectDumpLines.AssertIfNull(nameof(objectDumpLines)))
        {
            return "";
        }

        var sb = new StringBuilder();
        for (int i = 0; i < objectDumpLines.Count; i++)
        {
            var debugLine = objectDumpLines[i];
            if (debugLine.AssertIfNull("debugLine"))
                continue;

            sb.AppendLine(debugLine.ToDump(isVerbose));
        }

        return sb.ToString();
    }

    public static void AddDumpLines(this List<DumpLine> dumpLines, string key, object obj, int depth, bool isVerbose)
    {
        if (dumpLines == null)
            throw new NullReferenceException("AddDumpLines(): dumpLines is null!");

        IDumpable asDumpable = obj as IDumpable;
        if (asDumpable != null)
        {
            asDumpable.ToDumpLines(dumpLines, key, depth, isVerbose);
            return;
        }

        var goDeeper = false;
        var sbValue = new StringBuilder();
        Array asArray = null;
        int[] arrayDimensions = null;
        IEnumerable asEnumerable = null;
        ICollection asCollection = null;
        IDictionary asDictionary = null;
        IList asList = null;
        string asString = null;
        if (obj == null)
        {
            sbValue.Append("(null)");
        }
        else
        {
            if (isVerbose)
            {
                sbValue.AppendFormat("{0} ", obj.GetType().NiceName());
            }

            Transform asTransform = obj as Transform;
            if (asTransform != null)
            {
                sbValue.Append(asTransform.name);
            }
            else
            {
                asArray = obj as Array;
                if (asArray != null)
                {
                    goDeeper = true;
                    sbValue.AppendFormat("Array[{0}] ({1}){2}",
                        GetLengthsByRank(asArray, out arrayDimensions),
                        asArray.Length,
                        ColonByCount(asArray.Length));
                }
                else
                {
                    asEnumerable = obj as IEnumerable;
                    if (asEnumerable != null)
                    {
                        asString = obj as string;
                        if (asString != null)
                        {
                            sbValue.AppendFormat("\"{0}\"", asString);
                        }
                        else
                        {
                            goDeeper = true;
                            asCollection = obj as ICollection;
                            if (asCollection != null)
                            {
                                asList = obj as IList;
                                if (asList != null)
                                {
                                    sbValue.AppendFormat("IList ({0}){1}", asList.Count, ColonByCount(asList.Count));
                                }
                                else
                                {
                                    asDictionary = obj as IDictionary;
                                    if (asDictionary != null)
                                    {
                                        sbValue.AppendFormat("IDictionary ({0}){1}", asDictionary.Count,
                                            ColonByCount(asDictionary.Count));
                                    }
                                    else
                                    {
                                        sbValue.AppendFormat("ICollection ({0}){1}", asCollection.Count,
                                            ColonByCount(asCollection.Count));
                                    }
                                }
                            }
                            else
                            {
                                sbValue.Append("IEnumerable:");
                            }
                        }
                    }
                    else
                    {
                        sbValue.Append(obj); //Any other objects
                    }
                }
            }
        }

        dumpLines.Add(new DumpLine(depth, key, sbValue.ToString()));

        if (!goDeeper)
            return;

        int i = 0;
        //Something enumerable
        if (asArray != null)
        {
            if (arrayDimensions.Length == 1)
            {
                for (int len = asArray.Length; i < len;)
                {
                    if (!CollectionElementToDumpLines(dumpLines, null, ref i, asArray.GetValue(i), depth, isVerbose))
                        break;
                }
            }
            else
            {
                MultiDimentionArrayElementsToDumpLines(dumpLines, ref i, asArray, 0, null, arrayDimensions, depth,
                    isVerbose);
            }
        }
        else
        {
            if (asDictionary != null)
            {
                var dctKeys = asDictionary.Keys;
                foreach (var elemKey in dctKeys)
                {
                    if (!CollectionElementToDumpLines(dumpLines, elemKey, ref i, asDictionary[elemKey], depth, isVerbose))
                        break;
                }
            }
            else
            {
                if (asList != null && asList.Count > 0)
                {
                    for (int listCount = asList.Count; i < listCount;)
                    {
                        if (!CollectionElementToDumpLines(dumpLines, null, ref i, asList[i], depth, isVerbose))
                            break;
                    }
                }
                else
                {
                    foreach (var elem in asEnumerable)
                    {
                        if (!CollectionElementToDumpLines(dumpLines, null, ref i, elem, depth, isVerbose))
                            break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Adds to dumpLines description strings for element of collection (elem)
    /// </summary>
    /// <returns>False - enumeration break cause elements limit is reached</returns>
    private static bool CollectionElementToDumpLines(List<DumpLine> dumpLines, object key, ref int elemIndex, object elem,
        int depth, bool isVerbose)
    {
        if (elemIndex > CollectionElementsLimit)
        {
            dumpLines.Add(new DumpLine(depth + 1, null, "..."));
            return false;
        }

        string keyString = key == null
            ? "[" + elemIndex + "]"
            : (key is string
                ? "\"" + key + "\""
                : key.ToString());

        dumpLines.AddDumpLines(keyString, elem, depth + 1, isVerbose);

        elemIndex++;
        return true;
    }

    /// <summary>
    /// Adds to dumpLines description strings for multidimension array (Rank > 1) elements 
    /// </summary>
    private static bool MultiDimentionArrayElementsToDumpLines(List<DumpLine> dumpLines, ref int elemIndex, Array asArray,
        int currentDimension, int[] currentIndices, int[] dimensionLengts, int depth, bool isVerbose)
    {
        if (currentIndices == null)
        {
            currentIndices = (int[]) dimensionLengts.Clone();
            for (int i = 0, len = dimensionLengts.Length; i < len; i++)
                currentIndices[i] = 0;
        }

        string keyString;
        for (int i = 0, len = dimensionLengts[currentDimension]; i < len; i++)
        {
            currentIndices[currentDimension] = i;
            keyString = GetMultiDimentionArrayKey(currentDimension, currentIndices);
            if (currentDimension + 1 == dimensionLengts.Length)
            {
                //last level - time to add keys and values
                if (elemIndex > CollectionElementsLimit)
                {
                    dumpLines.Add(new DumpLine(depth + 1, null, "..."));
                    return false;
                }

                dumpLines.AddDumpLines(keyString, asArray.GetValue(currentIndices), depth + 1, isVerbose);
                elemIndex++;
            }
            else
            {
                //upper levels - add key (from indexes till current)
                currentIndices[currentDimension] = i;
                keyString = GetMultiDimentionArrayKey(currentDimension, currentIndices);
                dumpLines.Add(new DumpLine(depth, null, keyString));

                if (!MultiDimentionArrayElementsToDumpLines(dumpLines, ref elemIndex, asArray, currentDimension + 1,
                    currentIndices, dimensionLengts, depth + 1, isVerbose))
                    return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Returns string kind of '[x,y,z]' with array dimension lengths, also array of this lengts in 'dimensions'
    /// </summary>
    private static string GetLengthsByRank(Array array, out int[] dimensions)
    {
        dimensions = null;
        if (array == null)
            return "";

        var arrayRank = array.Rank;
        if (arrayRank == 1)
        {
            dimensions = new[] {array.Length};
            return "";
        }

        var lstDimensions = new List<int>();
        var sb = new StringBuilder();
        for (int i = 0; i < arrayRank; i++)
        {
            var len = array.GetLength(i);
            lstDimensions.Add(len);
            sb.Append(len);
            if (i + 1 < arrayRank)
                sb.Append(",");
        }
        dimensions = lstDimensions.ToArray();
        return sb.ToString();
    }

    /// <summary>
    /// Returns string kind of '[x,y,]' with currentIndices to currentDimension (then - commas only)
    /// </summary>
    private static string GetMultiDimentionArrayKey(int currentDimension, int[] currentIndices)
    {
        var sb = new StringBuilder("[");
        for (int i = 0, len = currentIndices.Length; i < len; i++)
        {
            if (i <= currentDimension)
                sb.Append(currentIndices[i]);

            if (i + 1 < len)
                sb.Append(",");
        }
        sb.Append("]");
        return sb.ToString();
    }


    //=== Debug with reflection ===============================================

    /// <summary>
    /// Отладочное. Выводит для объекта obj тип и коллекцию полей и свойств (showProps)
    /// </summary>
    public static string FieldsAndPropsToString(this object obj, string title = "", bool showProps = true,
        bool isDetaliedInfo = true, Type forTypeOnly = null)
    {
        if (obj == null)
            return "(null)";

        var type = obj.GetType();
        StringBuilder sb = new StringBuilder();
        if (isDetaliedInfo)
        {
            sb.Append($"<{type.NiceName()}>{(string.IsNullOrEmpty(title) ? "" : " " + title)} ");
        }
        else
        {
            if (!string.IsNullOrEmpty(title))
                sb.Append(title);
        }

        var allFields = type.GetFields();
        if (allFields.Length > 0)
        {
            if (isDetaliedInfo)
                sb.Append("\nFields:");
            sb.AppendLine();
            foreach (var fieldInfo in allFields)
            {
                if (!fieldInfo.IsPublic ||
                    (forTypeOnly != null && !forTypeOnly.IsAssignableFrom(fieldInfo.FieldType)))
                    continue;

                sb.Append(fieldInfo.GetValue(obj).VarDump(
                    isDetaliedInfo
                        ? $"{fieldInfo.FieldType.NiceName()} {fieldInfo.Name}"
                        : fieldInfo.Name,
                    1));
            }
        }
        if (showProps)
        {
            var allProps = type.GetProperties();
            if (allProps.Length > 0)
            {
                if (isDetaliedInfo)
                    sb.Append("\nProperties:");
                sb.AppendLine();
                foreach (var propertyInfo in allProps)
                {
                    if (forTypeOnly != null && !forTypeOnly.IsAssignableFrom(propertyInfo.PropertyType))
                        continue;

                    string varDump = null;
                    var getSetPrefix = (!isDetaliedInfo || propertyInfo.GetSetMethod() == null) ? "" : "set ";
                    bool isGetValueFail = false;
                    if (propertyInfo.GetGetMethod() != null)
                    {
                        if (isDetaliedInfo)
                            getSetPrefix += "get ";
                        try
                        {
                            varDump = propertyInfo.GetValue(obj, null).VarDump(
                                isDetaliedInfo
                                    ? $"{getSetPrefix}<{propertyInfo.PropertyType.NiceName()}> {propertyInfo.Name}"
                                    : propertyInfo.Name,
                                1);
                        }
                        catch (Exception)
                        {
                            isGetValueFail = true;
                        }
                    }

                    if (varDump != null)
                        sb.Append(varDump);
                    else
                    {
                        sb.Append("  ");
                        if (isDetaliedInfo)
                            sb.Append($"{getSetPrefix}<{propertyInfo.PropertyType.NiceName()}> ");

                        sb.Append($"{propertyInfo.Name}{(isGetValueFail ? "" : " ?")}");
                    }
                }
            }
        }

        return sb.ToString();
    }
}