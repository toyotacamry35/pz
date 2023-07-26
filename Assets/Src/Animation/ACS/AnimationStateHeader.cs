using System;
using System.Collections.Generic;
using UnityEngine;

namespace Src.Animation.ACS
{
    [SharedBetweenAnimators]
    [DisallowMultipleComponent]
    [AddComponentMenu("")]
    public class AnimationStateHeader : StateMachineBehaviour
    {
        [HideInInspector][SerializeField] private AnimationStateComponent[] _components;

        public T GetComponent<T>() where T : AnimationStateComponent
        {
            if (_components != null)
                foreach (var c in _components)
                    if (c is T)
                        return (T) c;
            return null;
        }

        public AnimationStateComponent GetComponent(Type type)
        {
            if (_components != null)
                foreach (var c in _components)
                    if (type.IsInstanceOfType(c))
                        return c;
            return null;
        }
 
        #region Internal

#if UNITY_EDITOR
        public void AddComponent(AnimationStateComponent c)
        {
            if (c == null) throw new ArgumentNullException(nameof(c));
            if (_components == null || Array.IndexOf(_components, c) == -1)
            {
                var size = _components != null ? _components.Length : 0;
                Array.Resize(ref _components, size + 1);
                _components[size] = c;
            }
        }

        public void ReplaceComponent(AnimationStateComponent old, AnimationStateComponent @new)
        {
            if (old == null) throw new ArgumentNullException(nameof(old));
            if (@new == null) throw new ArgumentNullException(nameof(@new));
            var idx = _components != null ? Array.IndexOf(_components, old) : -1;
            if (idx == -1) throw new KeyNotFoundException($"{old.name} not found in {this}");
            _components[idx] = @new;
        }

        public bool IsComponentExists(AnimationStateComponent c)
        {
            if (c == null) throw new ArgumentNullException(nameof(c));
            return _components != null && Array.IndexOf(_components, c) != -1;
        }

#endif

        #endregion
    }
}