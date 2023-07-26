using System;
using SharedCode.EntitySystem;
using System.Collections.Generic;
using SharedCode.Serializers;

namespace ReactivePropsNs.Touchables {
    public static class EntityOfTypeExtention {
        public static ITouchable<U> OfType<T, U>(this ITouchable<T> source, ICollection<IDisposable> disposables) where T : IDeltaObject where U : class, IDeltaObject {
            var innerDisposable = new DisposableComposite();
            disposables.Add(innerDisposable);
            DeltaObjectChildProxy<U> childProxy = new DeltaObjectChildProxy<U>((taskFactory, repository) => AsyncUtils.RunAsyncTask(taskFactory, repository), prefix => $"{prefix}ITouchable<{typeof(T).NiceName()}>.Cast<{typeof(U).NiceName()}>()\n{source.DeepLog(prefix + '\t')}");
            innerDisposable.Add(childProxy);
            bool connected = false;
            var toucherProxy = new Toucher.Proxy<T>(
                item => {
                    if (item is U uItem) {
                        connected = true;
                        childProxy.OnAdd(uItem);
                    }
                },
                item => {
                    if (connected && item is U uItem) {
                        connected = false;
                        childProxy.OnRemove(uItem);
                    }
                },
                () => {
                    if (connected) {
                        connected = false;
                        childProxy.OnRemove(null);
                    }
                },
                innerDisposable.Dispose
            );
            innerDisposable.Add(toucherProxy);
            innerDisposable.Add(source.Subscribe(toucherProxy));
            return childProxy;
        }
    }
}
