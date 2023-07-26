using Assets.Tools;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using SharedCode.Serializers;
using System;
using System.Linq.Expressions;

namespace ReactivePropsNs.Touchables
{
    public static class DeltaObjectListPropertyToModelStreamUnityExtention
    {
        /// <summary>
        /// Отсутствие просоединённой коллекции трактуется как отсутствие в коллекции значений. Если нужно различать эти случаи нужно сделать другой экстеншен.
        /// На каждый элемент DeltaObject.DictionaryProperty в Stream выкладывается порождённый им элемент, обычно ViewModel в UnityThread
        /// </summary>
        public static IHashSetStream<TModel> ToHashSetStream<TDelta, TValue, TModel>(
            this ITouchable<TDelta> source,
            DisposableComposite disposables,
            Expression<Func<TDelta, IDeltaList<TValue>>> getListExpression,
            Func<TValue, Func<ITouchable<TValue>>, TModel> viewModelFactory
        ) where TDelta : IDeltaObject where TValue : class, IDeltaObject, IBaseDeltaObjectWrapper where TModel : IDisposable
        {
            var output = new UnityThreadHashsetStream<TModel>();
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
