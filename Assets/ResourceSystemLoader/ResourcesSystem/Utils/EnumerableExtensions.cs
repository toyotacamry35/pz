using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnumerableExtensions
{
    public static class ItemExtensions
    {
        private const string NextLineDelimiter = "\n";
        private const string TabDelimiter = "\t";

        /// <summary>
        /// Возвращает ToString() элементов перечисления enumerable, разделенные по строкам, обрамленные квадр. скобками,
        /// с указанием числа элементов
        /// </summary>
        /// <summary>
        /// Возвращает ToString() элементов перечисления enumerable, разделенные delimeter, 
        /// обрамленные beginsWith и endsWith. Пример: "[(3) 1, 2, 5]"
        /// </summary>
        /// <param name="enumerable">перечисляемое (не имеет count)</param>
        /// <param name="isShowCount">показывать ли число элементов</param>
        /// <param name="isDelimitVertically">добавлять ли переносы строк между элементами</param>
        /// <param name="delimeter">разделитель элементов (обычно запятая или запятая с пробелом)</param>
        /// <param name="beginsWith">открывающий знак</param>
        /// <param name="endsWith">закрывающий знак</param>
        /// <param name="tabDelimeters">символы табуляции (на 1 шт.)</param>
        /// <param name="ifNull">что отдавать, когда enumerable null</param>
        public static string ItemsToString(this IEnumerable enumerable, bool isShowCount = true, bool isDelimitVertically = false,
            string delimeter = ", ", string beginsWith = "[", string endsWith = "]", string tabDelimeters = TabDelimiter,
            string ifNull = "null")
        {
            if (enumerable == null)
                return ifNull;

            //string tabDelimeters = TabDelimiter;
            int count = 0;
            var stringBuilder = new StringBuilder(isShowCount ? "" : beginsWith);
            foreach (var item in enumerable)
            {
                if (count > 0)
                {
                    stringBuilder.Append(delimeter);
                    if (isDelimitVertically)
                        stringBuilder.AppendLine();
                }

                if (isDelimitVertically)
                    stringBuilder.Append(tabDelimeters);
                stringBuilder.Append(item);
                count++;
            }

            stringBuilder.Append(endsWith);

            return isShowCount
                ? $"{beginsWith}({count}) {(isDelimitVertically ? NextLineDelimiter : "")}{stringBuilder}"
                : stringBuilder.ToString();
        }

        /// <summary>
        /// Возвращает ToString() элементов перечисления enumerable, разделенные по строкам, обрамленные квадр. скобками,
        /// с указанием числа элементов
        /// </summary>
        public static string ItemsToStringByLines(this IEnumerable enumerable, bool contentOnly = false)
        {
            return contentOnly
                ? ItemsToString(enumerable, false, true, "", "", "", "")
                : ItemsToString(enumerable, true, true);
        }

        /// <summary>
        /// Возвращает ToString() элементов перечисления enumerable через запятую
        /// </summary>
        public static string ItemsToStringSimple(this IEnumerable enumerable)
        {
            return ItemsToString(enumerable, false, false, ",", "", "", "");
        }

        /// <summary>
        /// Расширение для быстрого вывода лога
        /// </summary>
        /// <param name="source">Исходная коллекция</param>
        /// <param name="methodLog">Метод для логирования</param>
        /// <typeparam name="T">Параметр коллекции</typeparam>
        public static IEnumerable<T> Log<T>(this IEnumerable<T> source, Action<string> methodLog)
        {
            var serialized = source.ItemsToString();
            methodLog.Invoke($"{nameof(source)}: {serialized}");
            return source;
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source.AssertIfNull(nameof(source)) ||
                action.AssertIfNull(nameof(action)))
                return;

            foreach (T element in source)
                action(element);
        }

        public static bool NullableSequenceEqual<T>(this IEnumerable<T> x, IEnumerable<T> y, IEqualityComparer<T> comparer = null)
        {
            if (ReferenceEquals(x, y))
                return true;

            if (x != null && y != null)
                return x.SequenceEqual(y, comparer);

            return false;
        }

        public static bool NullableEqualWithoutOrder<T>(this IEnumerable<T> x, IEnumerable<T> y, IEqualityComparer<T> comparer = null)
        {
            if (ReferenceEquals(x, y))
                return true;

            if (x != null && y != null)
                return x.OrderBy(t => t).SequenceEqual(y.OrderBy(t => t), comparer);

            return false;
        }
    }
}