using System;
using UnityEditor;

namespace Src.Debugging
{
    public class DebugState
    {
        public static readonly DebugState Instance = new DebugState(); 

#region IsVisibleFps         
        private const string IsVisibleFpsKey = "DebugIsVisibleFps";
        
        public int IsVisibleFps
        {
            get => UniquePlayerPrefs.GetInt(IsVisibleFpsKey);
            set
            {
                if (IsVisibleFps != value)
                {
                    UniquePlayerPrefs.SetInt(IsVisibleFpsKey, value);
                    OnIsVisibleFpsChanged?.Invoke(value);
                }
            }
        }

        public event Action<int> OnIsVisibleFpsChanged;
#endregion
    

#region InitLogSystemOnStart         
        private const string InitLogSystemOnStartKey = "InitLogSystemOnStart";

        
        public bool InitLogSystemOnStart
        {
            get => UniquePlayerPrefs.GetBool(InitLogSystemOnStartKey);
            set
            {
                if (InitLogSystemOnStart != value)
                {
                    UniquePlayerPrefs.SetBool(InitLogSystemOnStartKey, value);
                    OnInitLogSystemOnStart?.Invoke(value);
                }
            }
        }

        public event Action<bool> OnInitLogSystemOnStart;
#endregion
        

#region SkipIntro         
    private const string SkipIntroKey = "SkipIntro";

    public bool SkipIntro
    {
        get => UniquePlayerPrefs.GetBool(SkipIntroKey,
#if UNITY_EDITOR
            true
#else
            false
#endif
        );
        set
        {
            if (SkipIntro != value)
            {
                UniquePlayerPrefs.SetBool(SkipIntroKey, value);
                OnSkipIntro?.Invoke(value);
            }
        }
    }

    public event Action<bool> OnSkipIntro;
#endregion
    }
}