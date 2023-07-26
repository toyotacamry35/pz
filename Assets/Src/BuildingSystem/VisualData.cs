using System;

namespace Assets.Src.BuildingSystem
{
    public abstract class VisualData : PropertyData
    {
        private bool placeholder = false;
        private bool valid = false;
        private bool selected = false;
        private bool visible = false;
        private float delay = 0.0f;

        public bool Placeholder
        {
            get { return placeholder; }
            set
            {
                if (placeholder != value)
                {
                    bool oldPlaceholder = placeholder;
                    placeholder = value;
                    PlaceholderChanged?.Invoke(this, new PropertyArgs("Placeholder", oldPlaceholder, placeholder));
                }
            }
        }

        public bool Valid
        {
            get { return valid; }
            set
            {
                if (valid != value)
                {
                    bool oldValid = valid;
                    valid = value;
                    ValidChanged?.Invoke(this, new PropertyArgs("Valid", oldValid, valid));
                }
            }
        }

        public bool Selected
        {
            get { return selected; }
            set
            {
                if (selected != value)
                {
                    bool oldSelected = selected;
                    selected = value;
                    SelectedChanged?.Invoke(this, new PropertyArgs("Selected", oldSelected, selected));
                }
            }
        }

        public bool Visible
        {
            get { return visible; }
            set
            {
                if (visible != value)
                {
                    bool oldVisible = visible;
                    visible = value;
                    VisibleChanged?.Invoke(this, new PropertyArgs("Visible", oldVisible, visible));
                }
            }
        }

        public event EventHandler<PropertyArgs> PlaceholderChanged;
        public event EventHandler<PropertyArgs> ValidChanged;
        public event EventHandler<PropertyArgs> SelectedChanged;
        public event EventHandler<PropertyArgs> VisibleChanged;

        public VisualBehaviour.VisualState GetVisualState()
        {
            if (placeholder)
            {
                return valid ? VisualBehaviour.VisualState.Suitable : VisualBehaviour.VisualState.Unsuitable;
            }
            else return GetVisualStateEx();
        }
        public string GetVisualCommonName()
        {
            return GetVisualCommonNameEx();
        }
        public string GetVisualVersionName()
        {
            return GetVisualVersionNameEx();
        }
        public float GetFracturedChunkScale()
        {
            return GetFracturedChunkScaleEx();
        }

        protected abstract VisualBehaviour.VisualState GetVisualStateEx();
        protected abstract string GetVisualCommonNameEx();
        protected abstract string GetVisualVersionNameEx();
        protected abstract float GetFracturedChunkScaleEx();
    }
}
