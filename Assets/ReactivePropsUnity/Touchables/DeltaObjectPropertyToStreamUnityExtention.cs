using SharedCode.EntitySystem;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using static ReactivePropsNs.Touchables.DeltaObjectPropertyToStreamExtention;

namespace ReactivePropsNs.Touchables
{
    public static class DeltaObjectPropertyToStreamUnityExtention
    {
        /// <summary>
        /// Перекладывает DeltaObject.Property в Stream Выводя его в UnityThread
        /// </summary>
        public static IStream<TValue> ToStream<TDelta, TValue>(this ITouchable<TDelta> source, ICollection<IDisposable> disposables,
            Expression<Func<TDelta, TValue>> getValueExpression, string detaliedLogPrefix = null) where TDelta : class, IDeltaObject
        {
            Func<string, string> _createLog = null;
            var output = new UnityThreadStream<TValue>(prefix => _createLog(prefix));
            disposables.Add(output);
            var dopts = new DeltaObjectPropertyToStream<TDelta, TValue>(output, null, getValueExpression, detaliedLogPrefix);
            _createLog = dopts.Log;
            disposables.Add(dopts);
            disposables.Add(source.Subscribe(dopts));
            return output;
        }

        /// <summary>
        /// Перекладывает DeltaObject.Property в Stream Выводя его в UnityThread
        /// </summary>
        public static (IStream<TValue> value, IStream<bool> hasValue) ToStreamHasValue<TDelta, TValue>(this ITouchable<TDelta> source, ICollection<IDisposable> disposables,
            Expression<Func<TDelta, TValue>> getValueExpression, bool useDefaultWhenHasNoValue = false, string detaliedLogPrefix = null) where TDelta : class, IDeltaObject
        {
            Func<string, string> _createLog = null;
            var output = new UnityThreadStream<TValue>(prefix => _createLog(prefix));
            disposables.Add(output);
            var hasValue = new UnityThreadStream<bool>(prefix => _createLog(prefix));
            disposables.Add(hasValue);
            var dopts = new DeltaObjectPropertyToStream<TDelta, TValue>(output, hasValue, getValueExpression, detaliedLogPrefix) { useDefaultWhenHasNoValue = useDefaultWhenHasNoValue };
            _createLog = dopts.Log;
            disposables.Add(dopts);
            disposables.Add(source.Subscribe(dopts));
            return (output, hasValue);
        }
    }
}