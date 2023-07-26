using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using System;
using System.Linq.Expressions;
using static ReactivePropsNs.Touchables.DeltaObjectDictionaryPropertyToStream;

namespace ReactivePropsNs.Touchables
{
    public static class DeltaObjectDictionaryPropertyToStreamUnity
    {
        /// <summary>
        /// Инструмент по умолчанию. Отсутствие просоединённой коллекции трактуется как отсутствие в коллекции значений. Если нужно различать эти случаи нужно сделать другой экстеншен.
        /// Перекладывает DeltaObject.DictionaryProperty в DictionaryStream Выводя его в UnityThread
        /// </summary>
        public static IDictionaryStream<TKey, TValue> ToDictionaryStream<TDelta, TKey, TValue>(this ITouchable<TDelta> source, DisposableComposite disposables,
            Expression<Func<TDelta, IDeltaDictionary<TKey, TValue>>> getValueExpression) where TDelta : class, IDeltaObject
        {
            if (typeof(IDeltaObject).IsAssignableFrom(typeof(TValue)))
                throw new Exception($"{nameof(ToDictionaryStream)} значения являются {typeof(TValue).GetType().Name} а IDeltaObject нельзя перекладывать напрямую в стрим. Это расширение только для простых значений. Используйте вместо этого ToModelsDictionaryStream");
            var output = new UnityThreadDictionaryStream<TKey, TValue>();
            disposables.Add(output);
            var processor = new InternalDeltaObjectDictionaryPropertyToStream<TDelta, TKey, TValue, TValue>(output, null, getValueExpression, (key, value) => value, output);
            disposables.Add(processor);
            disposables.Add(source.Subscribe(processor));
            return output;
        }
    }
}
