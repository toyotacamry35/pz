using System;
using Core.Environment.Logging.Extension;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.EntityPropertyResolvers;

namespace ReactivePropsNs.Touchables
{
    public class TestToucher
    {
        /// <summary> Отладочная информация, располагается в родительском классе чтобы нумерация не спутывалась. </summary>
        protected static int _idCounter;
    }
    public class TestToucher<T> : TestToucher, IToucher<T>, IDisposable where T : IDeltaObject
    {
        private Action<T> _onAddExtraAction;
        private Action<T> _onRemoveExtraAction;
        private Action _onCompletedExtraAction;


        //=== Props ===========================================================

        public int AddCount { get; private set; }

        public bool IsAdded => AddCount > 0;

        public DateTime LastAddDateTime { get; private set; }

        public int RemoveCount { get; private set; }
        public int RemoveNullCount { get; private set; }

        public bool IsRemoved => RemoveCount > 0;

        public DateTime LastRemoveDateTime;

        public int CompletedCount { get; private set; }

        public bool IsCompleted => CompletedCount > 0;

        public DateTime LastCompletedDateTime { get; private set; }

        public bool IsDisposed { get; private set; }

        public string Id { get; set; }
        public bool logEverything = false;

        //=== Ctor ============================================================

        public TestToucher(Action<T> onAddExtraAction = null, Action<T> onRemoveExtraAction = null, Action onCompletedExtraAction = null)
        {
            Id = (++_idCounter).ToString();
            SetExtraActions(onAddExtraAction, onRemoveExtraAction, onCompletedExtraAction);
        }


        //=== Public ==========================================================

        /// <summary> Все экстраэкшены вызываются строго один раз. </summary>
        public void SetExtraActions(Action<T> onAddExtraAction = null, Action<T> onRemoveExtraAction = null, Action onCompletedExtraAction = null)
        {
            _onAddExtraAction = onAddExtraAction;
            _onRemoveExtraAction = onRemoveExtraAction;
            _onCompletedExtraAction = onCompletedExtraAction;
        }
        public void SetAddExtraActions(Action<T> onAddExtraAction = null)
        {
            _onAddExtraAction = onAddExtraAction;
        }
        public void SetRemoveExtraActions(Action<T> onRemoveExtraAction = null)
        {
            _onRemoveExtraAction = onRemoveExtraAction;
        }
        public void SetCompleteExtraActions(Action onCompletedExtraAction = null)
        {
            _onCompletedExtraAction = onCompletedExtraAction;
        }

        public static void ResetIdCounter()
        {
            _idCounter = 0;
        }

        public T CachedDeltaObject;
        public void OnAdd(T deltaObject)
        {
            
            if (logEverything)
                Console.WriteLine($"{GetType().Name}[Id:{Id}].{nameof(OnAdd)}({deltaObject.ToLogString()})".AddTime());
            if (IsDisposed)
                return;

            AddCount++;
            LastAddDateTime = DateTime.Now;
            CachedDeltaObject = deltaObject;
            //ToLogs($"{nameof(OnAdd)}{EntityToString(entity)} -- {this}");
            var extraAction = _onAddExtraAction;
            _onAddExtraAction = null;
            extraAction?.Invoke(deltaObject);
        }

        public void OnRemove(T deltaObject)
        {
            if (logEverything)
                Console.WriteLine($"{GetType().Name}[Id:{Id}].{nameof(OnRemove)}({deltaObject.ToLogString()})".AddTime());
            if (IsDisposed)
                return;

            RemoveCount++;
            if (deltaObject == null)
                RemoveNullCount++;
            LastRemoveDateTime = DateTime.Now;
            CachedDeltaObject = default;
            //ToLogs($"{nameof(OnRemove)}{EntityToString(entity)} -- {this}");
            var extraAction = _onRemoveExtraAction;
            _onRemoveExtraAction = null;
            extraAction?.Invoke(deltaObject);
        }

        public void OnCompleted()
        {
            if (IsDisposed)
                return;

            CompletedCount++;
            LastCompletedDateTime = DateTime.Now;
            //ToLogs($"{nameof(OnCompleted)}() -- {this}");
            var extraAction = _onCompletedExtraAction;
            _onCompletedExtraAction = null;
            extraAction?.Invoke();

            Dispose();
        }

        public void Dispose()
        {
            IsDisposed = true;
        }

        public override string ToString()
        {
            return $"({GetType().NiceName()} [{Id}] Add={AddCount} ({LastAddDateTime:HH:mm:ss.ffff}), " +
                   $"Remove={RemoveCount} ({LastRemoveDateTime:HH:mm:ss.ffff}), Completed={CompletedCount} ({LastCompletedDateTime:HH:mm:ss.ffff})" +
                   $"{(IsDisposed ? " disposed" : "")})";
        }


        //=== Private =========================================================

        private void ToLogs(string msg)
        {
            msg = $"[{DateTime.Now:HH:mm:ss.ff}] {msg}";
            ReactiveLogs.Logger.IfInfo()?.Message(msg).Write();
            Console.WriteLine(msg);
        }

        private string EntityToString(T target, bool isDetailed = false)
        {
            var entityInfo = "null";
            if (target is IEntity entity)
            {
                var idToStr = entity.Id.ToString();
                entityInfo = isDetailed
                    ? $"{nameof(entity.Id)}={idToStr}, {nameof(entity.TypeId)}={entity.TypeId}, {nameof(entity.ParentEntityId)}={entity.ParentEntityId}, " +
                      $"repoId={entity.EntitiesRepository.Id}"
                    : idToStr.Substring(idToStr.Length - 6);
            }
            else if (target is IDeltaObject deltaObj)
            {
                var idToStr = deltaObj.ParentEntityId.ToString();
                entityInfo = isDetailed
                    ? $"{nameof(deltaObj.ParentEntityId)}={idToStr}, {nameof(deltaObj.TypeId)}={deltaObj.TypeId}, " +
                      $"repoId={deltaObj.EntitiesRepository.Id}"
                    : idToStr.Substring(idToStr.Length - 6);
            }

            return $"(Entity <{typeof(T).Name}> {entityInfo})";
        }
        private Func<string, string> _requestLogHandler;
        public void SetRequestLogHandler(Func<string, string> handler) => _requestLogHandler = handler;
        public string Log(string prefix)
        {
            return prefix + ToString() + (_requestLogHandler != null ? $"\n{_requestLogHandler(prefix + '\t')}" : "");
        }
    }
}