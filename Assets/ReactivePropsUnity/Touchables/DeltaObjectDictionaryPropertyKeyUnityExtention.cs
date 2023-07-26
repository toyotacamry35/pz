using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using SharedCode.Serializers;
using static ReactivePropsNs.Touchables.DeltaObjectDictionaryPropertyKeyExtention;

namespace ReactivePropsNs.Touchables
{
    public static class DeltaObjectDictionaryPropertyKeyUnityExtention
    {
        public static ITouchable<TValue> Key<TDelta, TKey, TValue>(this ITouchable<TDelta> source, ICollection<IDisposable> disposables, Expression<Func<TDelta, IDeltaDictionary<TKey, TValue>>> getDictionaryExpression, TKey key) where TDelta : class, IDeltaObject where TValue : class, IDeltaObject
        {
            var processor = new DeltaObjectDictionaryPropertyKeyProcessor<TDelta, TKey, TValue>(getDictionaryExpression, key, (taskFactory, repository) => AsyncUtils.RunAsyncTask(taskFactory, repository));
            disposables.Add(processor);
            disposables.Add(source.Subscribe(processor));
            return processor;
        }
    }
}
