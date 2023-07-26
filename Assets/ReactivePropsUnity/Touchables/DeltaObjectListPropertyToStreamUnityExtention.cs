using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using System;
using System.Linq.Expressions;

namespace ReactivePropsNs.Touchables
{
    public static class DeltaObjectListPropertyToStreamUnityExtention
    {
        /// <summary>
        /// Инструмент по умолчанию. Отсутствие просоединённой коллекции трактуется как отсутствие в коллекции значений. Если нужно различать эти случаи нужно сделать другой экстеншен.
        /// Перекладывает DeltaObject.DictionaryProperty в DictionaryStream Выводя его в UnityThread
        /// </summary>
        public static IHashSetStream<TValue> ToHashSetStream<TDelta, TValue>(this ITouchable<TDelta> source, DisposableComposite disposables,
            Expression<Func<TDelta, IDeltaList<TValue>>> getValueExpression) where TDelta : IDeltaObject
        {
            if (typeof(IDeltaObject).IsAssignableFrom(typeof(TValue)))
                throw new Exception($"{nameof(ToHashSetStream)} значения являются {typeof(TValue).NiceName()} а IDeltaObject нельзя перекладывать напрямую в стрим. Это расширение только для простых значений. Используйте вместо этого ToModelsDictionaryStream");
            var output = new UnityThreadHashsetStream<TValue>();
            disposables.Add(output);
            var processor = new DeltaObjectListPropertyToStream.Processor<TDelta, TValue, TValue>(output, null, getValueExpression, value => value, output);
            disposables.Add(processor);
            disposables.Add(source.Subscribe(processor));
            return output;
        }
    }
}
