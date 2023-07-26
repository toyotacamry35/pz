using ReactivePropsNs;
using Uins;
using UnityEngine;
using UnityEngine.UI;

namespace WeldAdds
{
    public abstract class SomethingVisibilitySetter : HasDisposablesMonoBehaviour
    {
        /// <summary>
        /// Если указан, то выставляется ignoreLayout = !IsVisible
        /// </summary>
        public LayoutElement LayoutElement;

        [Range(0, 1)]
        public float OnAlpha = 1;

        [Range(0, 1)]
        public float OffAlpha;

        protected ReactiveProperty<bool> IsVisibleRp = new ReactiveProperty<bool>();


        //=== Props ===========================================================

        public bool IsVisible
        {
            get => IsVisibleRp.HasValue && IsVisibleRp.Value;
            set => IsVisibleRp.Value = value;
        }


        //=== Unity ===========================================================

        private void Awake()
        {
            if (!OnAwakeSelfCheck())
            {
                enabled = false;
                return;
            }

            if (!IsVisibleRp.HasValue)
                IsVisibleRp.Value = false;

            IsVisibleRp.Action(
                D,
                isVisible =>
                {
                    if (LayoutElement != null)
                        LayoutElement.ignoreLayout = !isVisible;
                });
            IsVisibleRp.Action(D, OnVisibilityChanged);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            IsVisibleRp.Dispose();
        }


        //=== Protected =======================================================

        protected abstract bool OnAwakeSelfCheck();

        protected abstract void OnVisibilityChanged(bool isVisible);
    }
}