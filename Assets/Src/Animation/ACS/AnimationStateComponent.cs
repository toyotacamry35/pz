using NLog.Filters;
using UnityEngine;

namespace Src.Animation.ACS
{
    public abstract class AnimationStateComponent : StateMachineBehaviour
    {
        [HideInInspector][SerializeField] private AnimationStateHeader _header;

        public AnimationStateHeader Header => _header;

        public T GetComponent<T>() where T : AnimationStateComponent => Header.GetComponent<T>();

        #region Internal

#if UNITY_EDITOR
        public void SetHeader(AnimationStateHeader header)
        {
            _header = header;
        }
#endif

        #endregion
    }
}