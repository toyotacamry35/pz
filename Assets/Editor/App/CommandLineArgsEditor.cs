using System.Linq;
using Assets.Src.Tools;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor.App
{
    public class CommandLineArgsEditor : EditorWindow
    {
        [MenuItem("Tools/Command Line Arguments...")]
        public static void OpenWindow()
        {
            EditorWindow.GetWindow<CommandLineArgsEditor>();
        }

        private void OnGUI()
        {
            var args = CmdArgumentHelper.AdditionalCommandLineArguments;
            var toRemove = -1;

            using (var change = new EditorGUI.ChangeCheckScope())
            {
                for (int i = 0; i < args.Length; ++i)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        args[i] = EditorGUILayout.TextField(args[i]);
                        if (GUILayout.Button("-", GUILayout.Width(14)))
                            toRemove = i;
                    }
                }

                bool updated = change.changed;
                
                if (toRemove != -1)
                {
                    args = args.Where((x,i) => i != toRemove).ToArray();
                    updated = true;
                }
                else
                if (GUILayout.Button("+"))
                {
                    args = args.Append(string.Empty).ToArray();
                    updated = true;
                }

                if (updated)
                {
                    CmdArgumentHelper.AdditionalCommandLineArguments = args;
                }
            }
        }
    }
}