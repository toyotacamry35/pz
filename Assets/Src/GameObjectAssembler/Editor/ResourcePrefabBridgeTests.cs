using Assets.Src.ResourcesSystem.Base;
using ResourcesSystem.Loader;
using NUnit.Framework;
using System.Text;
using UnityEditor;

namespace Assets.Src.RubiconAI.Editor
{
    public class ResourcePrefabBridgeTests
    {
        [Test]
        public static void TestTrivial()
        {
            var loader = new InMemoryLoader();
            var resources = new GameResources(loader);
            var address = "/1";
            var json = @"{
                              ""$type"": ""ClassWithPrefabRef"",
                              ""Data"": ""Assets/UtilPrefabs/User.prefab""
                         }";
            loader.KnownFilesAdd(address, new StringBuilder(json));
            var loaded = resources.LoadResource<BaseResource>(address);
            Assert.AreEqual(typeof(ClassWithPrefabRef), loaded.GetType());
            var casted = (ClassWithPrefabRef)loaded;
            var tgtObj = AssetDatabase.LoadMainAssetAtPath("Assets/UtilPrefabs/User.prefab");
            Assert.AreEqual(tgtObj, casted.Data.Target);
        }
    }


}
