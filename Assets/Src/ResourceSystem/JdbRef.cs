using Assets.Src.ResourcesSystem.Base;
using System;

namespace Assets.Src.ResourceSystem
{
    [Serializable]
    public class JdbRef<T> : JdbRefBase where T : class, IResource
    {
        public T Target => _metadata?.Get<T>();
        
#if UNITY_EDITOR
        public EditorBaseResourceWrapper<T> EDITOR_Target => _EDITOR_Target ?? (_EDITOR_Target = new EditorBaseResourceWrapper<T>(_metadata?.GetFullTreeCopy<T>()));

        private EditorBaseResourceWrapper<T> _EDITOR_Target;
#endif
    }
}
