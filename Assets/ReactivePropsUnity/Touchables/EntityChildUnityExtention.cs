using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SharedCode.EntitySystem;
using SharedCode.Serializers;

namespace ReactivePropsNs.Touchables
{
    public static class EntityChildUnityExtention
    {
        public static ITouchable<TChild> Child<TParent, TChild>(this ITouchable<TParent> source,
            ICollection<IDisposable> disposables, Expression<Func<TParent, TChild>> childGetterExpr)
            where TParent : IDeltaObject where TChild : IDeltaObject
        {
            var child = new DeltaObjectChild<TParent, TChild>(childGetterExpr, (taskFactory, repository) => AsyncUtils.RunAsyncTask(taskFactory, repository));
            var agent = source.Subscribe(child);
            disposables.Add(child);
            disposables.Add(agent);
            return child;
        }
    }
}