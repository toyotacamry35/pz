using System;
using System.Linq;
using System.Text;
using System.Threading;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using SharedCode.EntitySystem.EntityPropertyResolvers;

namespace ReactivePropsNs.Touchables
{
    public static class TestTools
    {
        public static string AddTime(this string source)
        {
            return $"[{DateTime.Now:HH:mm:ss.ffff}] {source}";
        }

        public static string ToLogString(this IDeltaObject deltaObject)
        {
            if (deltaObject == null)
                return "<null>";
            IDeltaObject baseDeltaObject = deltaObject is IBaseDeltaObjectWrapper ? deltaObject = ((IBaseDeltaObjectWrapper)deltaObject).GetBaseDeltaObject() : deltaObject;
            if (((IDeltaObjectExt)baseDeltaObject).GetParentEntity() == null)
                return $"<{deltaObject.GetType().NiceName()}:{(baseDeltaObject as IDeltaObjectExt)?.LocalId} detached>";
            return $"<{deltaObject.GetType().NiceName()}[id:{(baseDeltaObject as IDeltaObjectExt)?.LocalId}]>";
            //return $"<{deltaObject.GetType().NiceName()}:{(baseDeltaObject as IDeltaObjectExt)?.LocalId} {EntityPropertyResolver.GetPropertyAddress(baseDeltaObject).ToString()}>";
        }
        
        public static string ToShortLog<TKey, TValue>(this IDeltaDictionary<TKey, TValue> target, int itemsCount = 4)
        {
            if (target == null)
                return "<null>";
            return $"{{{target.GetType().NiceName()}}}";
        }
        public static string ToShortLog<TValue>(this IDeltaList<TValue> target, int itemsCount = 4)
        {
            if (target == null)
                return "<null>";
            return $"{{{target.GetType().NiceName()}}}";
        }

        private static StringBuilder _sharedStringBuilder = null;
        /// <summary> Вызывать строго только под локом Entity  </summary>
        public static string ToKeysStringUnderLock<TKey, TValue>(this IDeltaDictionary<TKey, TValue> targetDictionary, int maxCount = 4)
        {
            var sb = Interlocked.Exchange(ref _sharedStringBuilder, null) ?? new StringBuilder();
            sb.Append('{').Append(targetDictionary.GetType().NiceName()).Append(" Keys:[");
            _sharedStringBuilder = null;
            int i = 0;
            var keys = targetDictionary.Keys.ToArray();
            for (; i < keys.Length; i++)
                if (i < maxCount)
                {
                    if (i != 0)
                        sb.Append(", ");
                    sb.Append(keys[i]);
                }
                else
                {
                    sb.Append(", ... ");
                }
            sb.Append("]:Count=");
            sb.Append(keys.Length);
            sb.Append("}");
            string result = sb.ToString();
            sb.Clear();
            Interlocked.Exchange(ref _sharedStringBuilder, sb);
            return result;
        }
    }
}
