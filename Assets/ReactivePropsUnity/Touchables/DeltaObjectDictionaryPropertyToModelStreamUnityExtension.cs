using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using SharedCode.Serializers;
using System;
using System.Linq.Expressions;

namespace ReactivePropsNs.Touchables
{
    public static class DeltaObjectDictionaryPropertyToModelStreamUnityExtension
    {
        /// <summary>
        /// Отсутствие просоединённой коллекции трактуется как отсутствие в коллекции значений. Если нужно различать эти случаи нужно сделать другой экстеншен.
        /// На каждый элемент DeltaObject.DictionaryProperty в Stream выкладывается порождённый им элемент, обычно ViewModel в UnityThread
        /// </summary>
        public static IDictionaryStream<TKey, TModel> ToDictionaryStream<TDelta, TKey, TValue, TModel>(
            this ITouchable<TDelta> source,
            DisposableComposite disposables,
            Expression<Func<TDelta, IDeltaDictionary<TKey, TValue>>> getDictionaryExpression,
            Func<TKey, TValue, Func<ITouchable<TValue>>, TModel> factoryInThreadPool) where TDelta : class, IDeltaObject
            where TValue : class, IDeltaObject, IBaseDeltaObjectWrapper
            where TModel : IDisposable
        {
            var output = new UnityThreadDictionaryStream<TKey, TModel>();
            disposables.Add(output);
            var processor = new InternalDeltaObjectDictionaryPropertyToViewModelsStream<TDelta, TKey, TValue, TModel>(
                output,
                null,
                getDictionaryExpression,
                factoryInThreadPool,
                (taskFactory, repository) => AsyncUtils.RunAsyncTask(taskFactory, repository),
                output);
            disposables.Add(processor);
            disposables.Add(source.Subscribe(processor));
            return output;
        }
    }
}