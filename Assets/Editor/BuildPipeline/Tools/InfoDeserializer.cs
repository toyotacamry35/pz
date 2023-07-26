using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.Build.Pipeline.Utilities;

namespace Assets.Test.Src.Editor
{
    public static class InfoDeserializer
    {
        [MenuItem("Tools/Deserialize Infos")]
        public static void LoadFileInfo()
        {
            var folder = new DirectoryInfo("Infos");
            var files = folder.GetFiles("*.info");
            foreach (var info in files)
            {
                var bytes = File.ReadAllBytes(info.FullName);
                using (MemoryStream file = new MemoryStream(bytes))
                {
                    BinaryFormatter bf = new BinaryFormatter();

                    object obj = bf.Deserialize(file);

                    var objects = obj as CachedInfo;

                    var serialize = JsonConvert.SerializeObject(objects);
                    File.WriteAllText($"{folder.Name}/{info.Name}.json",serialize);
                }
            }
        }
    }
}