using System;
using System.Linq.Expressions;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using SharedCode.Serializers;

namespace ReactivePropsNs.Touchables
{
    public static class DeltaObjectListPropertyToModelStream
    {
        /// <summary>
        /// Отсутствие просоединённой коллекции трактуется как отсутствие в коллекции значений. Если нужно различать эти случаи нужно сделать другой экстеншен.
        /// На каждый элемент DeltaObject.DictionaryProperty в Stream выкладывается порождённый им элемент, в данном варианте не в UnityThread, а в тредпуле.
        /// </summary>
        public static IHashSetStream<TModel> ToHashSetStreamThreadPool<TDelta, TValue, TModel>(
            this ITouchable<TDelta> source,
            DisposableComposite disposables,
            Expression<Func<TDelta, IDeltaList<TValue>>> getListExpression,
            Func<TValue, Func<ITouchable<TValue>>, TModel> viewModelFactory
        ) where TDelta : IDeltaObject where TValue : class, IDeltaObject, IBaseDeltaObjectWrapper where TModel : IDisposable
        {
            var output = new HashSetStream<TModel>();
            disposables.Add(output);
            var processor = new InternalDeltaObjectListPropertyToViewModelsStream<TDelta, TValue, TModel>(
                output,
                null,
                getListExpression,
                viewModelFactory,
                (taskFactory, repository) => AsyncUtils.RunAsyncTask(taskFactory, repository),
                output);
            disposables.Add(processor);
            disposables.Add(source.Subscribe(processor));
            return output;
        }
    }
}
