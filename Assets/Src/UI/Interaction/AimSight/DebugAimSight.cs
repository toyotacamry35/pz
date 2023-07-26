using Assets.Src.Lib.Cheats;
using Core.Cheats;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class DebugAimSight : BindingViewModel
    {
        private const string ShowDebugAimSightKey = "ShowDebugAimSight";
        private static DebugAimSight _instance;


        //=== Props ===============================================================

        private bool _isVisible;

        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    IsVisibleFinally = GetIsVisibleFinally();
                }
            }
        }

        private bool _isVisibleFinally;

        [Binding]
        public bool IsVisibleFinally
        {
            get { return _isVisibleFinally; }
            set
            {
                if (_isVisibleFinally != value)
                {
                    _isVisibleFinally = value;
                    NotifyPropertyChanged();
                }
            }
        }


        public bool ShowDebugAimSight
        {
            get { return UniquePlayerPrefs.GetInt(ShowDebugAimSightKey) == 1; }
            set
            {
                UniquePlayerPrefs.SetInt(ShowDebugAimSightKey, value ? 1 : 0);
                IsVisibleFinally = GetIsVisibleFinally();
            }
        }


        //=== Unity ===============================================================

        private void Awake()
        {
            _instance = SingletonOps.TrySetInstance(this, _instance);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (_instance == this)
                _instance = null;
        }


        //=== Public ==============================================================

        private bool GetIsVisibleFinally()
        {
            return ShowDebugAimSight && IsVisible;
        }

        [Cheat]
        public static void toggle_debug_aim()
        {
            _instance?.Toggle();
        }

        private void Toggle()
        {
            ShowDebugAimSight = !ShowDebugAimSight;
        }
    }
}