using Src.Debugging;
using UnityEditor;

namespace Src.Editor.Tools
{
    [InitializeOnLoad]
    public static class InitLogSystemMenuItem
    {
        private const string MENU_ITEM = "Debug/Init Log System On Play";

        static InitLogSystemMenuItem()
        {
            EditorApplication.delayCall += () => { Menu.SetChecked(MENU_ITEM, DebugState.Instance.InitLogSystemOnStart); };
            DebugState.Instance.OnInitLogSystemOnStart += v => { Menu.SetChecked(MENU_ITEM, v); };
        }
        
        [MenuItem(MENU_ITEM)]
        public static void InitLogSystemOnStartMenuItem() => DebugState.Instance.InitLogSystemOnStart = !DebugState.Instance.InitLogSystemOnStart;
    }
}