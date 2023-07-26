using Src.Debugging;
using UnityEditor;

namespace Src.Editor.Tools
{
    [InitializeOnLoad]
    public static class SkipIntroMenuItem
    {
        private const string MENU_ITEM = "Debug/Skip Intro";

        static SkipIntroMenuItem()
        {
            EditorApplication.delayCall += () => { Menu.SetChecked(MENU_ITEM, DebugState.Instance.SkipIntro); };
            DebugState.Instance.OnSkipIntro += v => { Menu.SetChecked(MENU_ITEM, v); };
        }
    
        [MenuItem(MENU_ITEM)]
        public static void SkipIntro() => DebugState.Instance.SkipIntro = !DebugState.Instance.SkipIntro;
    }
}