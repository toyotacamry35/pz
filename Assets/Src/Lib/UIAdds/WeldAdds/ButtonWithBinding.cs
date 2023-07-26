using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine.UI;
using UnityWeld.Binding;

namespace WeldAdds
{
    [Binding]
    public class ButtonWithBinding : Button, INotifyPropertyChangedExt
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private SelectionState _state;


        //=== Props ===========================================================

        //Названо так кривовато, потому что IsHighlighted уже занято
        [Binding]
        public bool HasHighlight => _state == SelectionState.Highlighted;

        [Binding]
        public bool HasPress => _state == SelectionState.Pressed;

        [Binding]
        public bool HasDisable => _state == SelectionState.Disabled;

        [Binding]
        public bool HasNormal => _state == SelectionState.Normal;

        /// <summary>
        /// 0 Normal, 1 Highlighted, 2 Pressed, 3 Selected, 4 Disabled
        /// Стандартное использование: 3-4 номерных индекса (Normal-Highlighted-Pressed-Selected) и else (Disabled)
        /// </summary>
        [Binding]
        public int StateIndex => (int) _state;


        //=== Public ==========================================================

        public void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        //=== Overrided =======================================================

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            var oldHasHighlight = HasHighlight;
            var oldHasPress = HasPress;
            var oldHasDisable = HasDisable;
            var oldHasNormal = HasNormal;

            base.DoStateTransition(state, instant);
            _state = state;

            if (HasHighlight != oldHasHighlight)
                NotifyPropertyChanged(nameof(HasHighlight));

            if (HasPress != oldHasPress)
                NotifyPropertyChanged(nameof(HasPress));

            if (HasDisable != oldHasDisable)
                NotifyPropertyChanged(nameof(HasDisable));

            if (HasNormal != oldHasNormal)
                NotifyPropertyChanged(nameof(HasNormal));

            NotifyPropertyChanged(nameof(StateIndex));
        }
    }
}