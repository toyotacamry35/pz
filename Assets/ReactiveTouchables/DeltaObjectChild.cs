using System;
using SharedCode.EntitySystem;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Threading.Tasks;
using SharedCode.Serializers;

namespace ReactivePropsNs.Touchables
{
    public class DeltaObjectChild<TParentDelta, TChildDelta> : IToucher<TParentDelta>, ITouchable<TChildDelta>
        where TParentDelta : IDeltaObject where TChildDelta : IDeltaObject
    {
        private ThreadSafeDisposeWrapper _disposeWrapper;
        private DeltaObjectChildProxy<TChildDelta> _childProxy;

        private object _lock = new object(); // Это только чтобы подписка и рассылка не пересекались друг с другом. Возможно оно не нужно
        private TChildDelta _cachedChild = default; // Возможно оно не нужно
        private Func<TParentDelta, TChildDelta> _childGetter;
        private string _propertyName;

        private Func<string, string> _reciveStreamLog;

        public DeltaObjectChild(Expression<Func<TParentDelta, TChildDelta>> childGetterExpr, Action<Func<Task>, IEntitiesRepository> asyncTaskRunner)
        {
            _disposeWrapper = new ThreadSafeDisposeWrapper(DisposeInternal, this);
            _childProxy = new DeltaObjectChildProxy<TChildDelta>(asyncTaskRunner, DeepLog);
            _propertyName = childGetterExpr.GetMemberName();
            _childGetter = childGetterExpr.Compile();
        }

        ~DeltaObjectChild()
        {
            if (IsDisposed)
                return;
            //ReactiveLogs.Logger.IfWarn()?.Message($"Non disposed {GetType().NiceName()} was finalized").Write();
        }

        public void OnAdd(TParentDelta deltaObject)
        {
            if (!_disposeWrapper.StartWorker())
                return;

            //Console.WriteLine($"{GetType().Name}.{nameof(OnAdd)}({deltaObject.ToLogString()})".AddTime());
            lock (_lock) // Этот лок не лишний, потому что мы гарантированы EGOProxy никогда не даст сделать два Add одновременно но никтон егарантирует, что не придёт OnChangeProperty
            {
                // Если мы сюда попали, значит хозяйская entity уже на локе на read.
                deltaObject.SubscribePropertyChanged(_propertyName, OnChangeProperty);
                _childProxy.OnAdd(_cachedChild = _childGetter(deltaObject));
            }
            _disposeWrapper.FinishWorker();
        }

        public void OnRemove(TParentDelta deltaObject)
        {
            if (!_disposeWrapper.StartWorker())
                return;

            //Console.WriteLine($"{GetType().Name}.{nameof(OnRemove)}({deltaObject.ToLogString()})".AddTime());
            lock (_lock) // Этот лок не лишний, потому что мы гарантированы EGOProxy никогда не даст сделать два Remove одновременно но никтон егарантирует, что не придёт OnChangeProperty
            {
                if (deltaObject != null)
                {
                    var child = _cachedChild;
                    deltaObject.UnsubscribePropertyChanged(_propertyName, OnChangeProperty);
                    _cachedChild = default;
                    _childProxy.OnRemove(child);
                }
                else // Если пришёл пустой deltaObject значит Entity сдохла, и отписываться от неё вообще не нужнго, только почиститься.
                {
                    _cachedChild = default;
                    _childProxy.OnRemove(default);
                }
            }

            _disposeWrapper.FinishWorker();
        }

        private Task OnChangeProperty(EntityEventArgs args)
        {
            if (!_disposeWrapper.StartWorker())
                return Task.CompletedTask;

            lock (_lock)
            {
                //Console.WriteLine($"[{DateTime.Now:HH:mm:ss.ffff}] {GetType().Name}.{nameof(OnChangeProperty)}() // {nameof(_parentEntityLockedIntoChildThread)}={_parentEntityLockedIntoChildThread})");
                if (_cachedChild != null)
                {
                    var previouseChild = _cachedChild;
                    _cachedChild = default;
                    _childProxy.OnRemove(previouseChild);
                }

                _cachedChild = ((IDeltaObject) args.NewValue).To<TChildDelta>();

                if (_cachedChild != null)
                {
                    _childProxy.OnAdd(_cachedChild);
                }
            }

            _disposeWrapper.FinishWorker();
            return Task.CompletedTask;
        }

        public void OnCompleted()
        {
            Dispose();
        }

        public IDisposable Subscribe(IToucher<TChildDelta> toucher)
        {
            //Console.WriteLine($"[{DateTime.Now:HH:mm:ss.ffff}] {GetType().Name}.{nameof(Subscribe)}({toucher}) // {nameof(_parentEntityLockedIntoChildThread)}={_parentEntityLockedIntoChildThread})");
            return _childProxy.Subscribe(toucher);
        }

        public void SetRequestLogHandler(Func<string, string> handler)
        {
            _reciveStreamLog = handler;
        }

        public string DeepLog(string prefix)
        {
            string header = $"{prefix}ITouchable<{typeof(TParentDelta).Name}>.Child({_propertyName})";
            if (_disposeWrapper.IsDisposed)
                return header + " DISPOSED";
            lock (_lock)
                if (_cachedChild == null)
                    return $"{header} DISCONNECTED{(_reciveStreamLog == null ? "" : $"\n{_reciveStreamLog(prefix + '\t')}")}";
                else
                    return $"{header} CHILD:{_cachedChild}{(_reciveStreamLog == null ? "" : $"\n{_reciveStreamLog(prefix + '\t')}")}";
        }

        public bool IsDisposed => _disposeWrapper.IsDisposed;

        public void Dispose()
        {
            _disposeWrapper.RequestDispose();
        }

        private void DisposeInternal()
        {
            _childProxy.Dispose();
            GC.SuppressFinalize(this);
        }
    }

    public static class EntityChildTreadPoolExtention
    {
        public static ITouchable<TChild> ChildTreadPool<TParent, TChild>(this ITouchable<TParent> source,
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