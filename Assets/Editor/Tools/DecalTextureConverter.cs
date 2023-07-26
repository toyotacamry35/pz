using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor.Tools
{
    public class DecalTextureConverter : EditorWindow
    {
        private const string tempPath = "C:/Magick";

        [MenuItem("Tools/Tech Art/Convert Decal Textures", false)]
        private static void MenuItem()
        {
            var materials = AssetDatabase.FindAssets("t: Material")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(w => !w.StartsWith("Assets/Plugins"))
                .Select(x => AssetDatabase.LoadAssetAtPath<Material>(x))
                .Where(z => z.shader.name.Contains("Decalicious"))
                .ToList();

            var dict = new Dictionary<string, List<string>>();

            int count = 0;
            foreach (var mat in materials)
            {
                var maskTex = mat.GetTexture("_MaskTex");
                if (maskTex == null)
                    continue;
                var mainTex = mat.GetTexture("_MainTex");
                if (mainTex == null)
                    continue;
                var maskPath = AssetDatabase.GetAssetPath(maskTex);
                var mainPath = AssetDatabase.GetAssetPath(mainTex);

                if (!dict.ContainsKey(mainPath))
                    dict[mainPath] = new List<string>() { maskPath };
                else
                    dict[mainPath].Add(maskPath);

                maskPath = Path.Combine("C:/Users/Bone/Documents/colony_proto_second/", maskPath);
                mainPath = Path.Combine("C:/Users/Bone/Documents/colony_proto_second/", mainPath);
                UnityEngine.Debug.LogError(mainPath);
                UnityEngine.Debug.LogError(maskPath);
                var magickPath = Path.ChangeExtension(Path.Combine(tempPath, Path.GetFileName(mainPath)), "png");

                Process cmd = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "cmd.exe";
                startInfo.WorkingDirectory = tempPath;
                startInfo.RedirectStandardInput = true;
                startInfo.RedirectStandardOutput = true;
                startInfo.UseShellExecute = false;
                cmd.StartInfo = startInfo;
                var tempFilePath = Path.Combine(tempPath, "temp.txt");
                var consoleLine0 = $"magick convert \"{mainPath}\" -format \"%wx%h\" info: > temp.txt";
                var consoleLine1 = "SET /p size=<temp.txt";
                var consoleLine2 = $"magick convert \"{maskPath}\" -colorspace LinearGray -resize \"%size%\" -gravity center \"{magickPath}\"";
                var consoleLine3 = $"magick convert \"{mainPath}\" \"{magickPath}\" -channel-fx \"| gray=>alpha\" \"{mainPath}\"";
                cmd.Start();
                cmd.StandardInput.WriteLine(consoleLine0);
                cmd.StandardInput.Flush();
                cmd.StandardInput.WriteLine(consoleLine1);
                cmd.StandardInput.Flush();
                cmd.StandardInput.WriteLine(consoleLine2);
                cmd.StandardInput.Flush();
                cmd.StandardInput.WriteLine(consoleLine3);
                cmd.StandardInput.Flush();
                cmd.StandardInput.Close();
                cmd.WaitForExit();
                var result = cmd.StandardOutput.ReadToEnd();
                UnityEngine.Debug.LogError(result, mainTex);
                count++;
            }
            UnityEngine.Debug.Log($"{count} objects processed successfully");
            UnityEngine.Debug.LogError($"Do not forget to delete temporary files from {tempPath}");

            File.Delete("C:/temp.txt");
            foreach (var x in dict)
            {
                string s = "";
                if (x.Value.Count > 1)
                {
                    foreach (var alphaPath in x.Value)
                        s += $"\n{alphaPath}";
                    UnityEngine.Debug.LogError($"One mask {x.Key} corresponding to several alphas:{s}");
                }
            }
        }
    }
}