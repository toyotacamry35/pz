using System;
using System.Linq.Expressions;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using SharedCode.Serializers;

// ReSharper disable CommentTypo
namespace ReactivePropsNs.Touchables
{
    public static class DeltaObjectDictionaryPropertyToModelStreamExtension
    {
        /// <summary>
        ///     Отсутствие просоединённой коллекции трактуется как отсутствие в коллекции значений. Если нужно различать эти случаи
        ///     нужно сделать другой экстеншен.
        ///     На каждый элемент DeltaObject.DictionaryProperty в Stream выкладывается порождённый им элемент, в данном варианте
        ///     не в UnityThread, а в тредпуле.
        /// </summary>
        public static IDictionaryStream<TKey, TModel> ToDictionaryStreamThreadPool<TDelta, TKey, TValue, TModel>(
            this ITouchable<TDelta> source,
            DisposableComposite disposables,
            Expression<Func<TDelta, IDeltaDictionary<TKey, TValue>>> getDictionaryExpression,
            Func<TKey, TValue, Func<ITouchable<TValue>>, TModel> viewModelFactory) where TDelta : class, IDeltaObject
            where TValue : class, IDeltaObject, IBaseDeltaObjectWrapper
            where TModel : IDisposable
        {
            var output = new DictionaryStream<TKey, TModel>();
            disposables.Add(output);
            var processor = new InternalDeltaObjectDictionaryPropertyToViewModelsStream<TDelta, TKey, TValue, TModel>(
                output,
                null,
                getDictionaryExpression,
                viewModelFactory,
                (taskFactory, repository) => AsyncUtils.RunAsyncTask(taskFactory, repository),
                output);
            disposables.Add(processor);
            disposables.Add(source.Subscribe(processor));
            return output;
        }
    }
}