using System.Text;
using ResourcesSystem.Loader;
using Assets.Src.ResourcesSystem.Base;
using NUnit.Framework;

namespace Assets.Src.RubiconAI.Editor
{
    class ResourcesTest
    {
        [Test]
        public static void TestTrivial()
        {
            var loader = new InMemoryLoader();
            var resources = new GameResources(loader);
            var address = "/1";
            var json = @"{
                              ""$type"": ""SomeClass"",
                              ""Data"": ""SomeData"",
                              ""OtherData"": ""123""
                         }";
            loader.KnownFilesAdd(address, new StringBuilder(json));
            var loaded = resources.LoadResource<BaseResource>(address);
            Assert.AreEqual(typeof(SomeClass), loaded.GetType());
            var casted = (SomeClass)loaded;
            Assert.AreEqual(123, casted.OtherData);
            Assert.AreEqual("SomeData", casted.Data);
        }

        [Test]
        public static void TestTerminalTypeRef()
        {
            var loader = new InMemoryLoader();
            var resources = new GameResources(loader);
            var address1 = "/1";
            var json1 = @"{
                              ""$type"": ""SomeClass"",
                              ""Data"": ""SomeData"",
                              ""OtherData"": ""123""
                         }";
            var address2 = "/2";
            var json2 = @"{
                              ""$type"": ""TestResource"",
                              ""InternalData"": ""/1""
                         }";

            loader.KnownFilesAdd(address1, new StringBuilder(json1));
            loader.KnownFilesAdd(address2, new StringBuilder(json2));

            var loaded2 = resources.LoadResource<BaseResource>(address2);
            Assert.AreEqual(typeof(TestResource), loaded2.GetType());

            var casted2 = (TestResource)loaded2;

            var loaded1 = casted2.InternalData.Target;
            Assert.AreEqual(typeof(SomeClass), loaded1.GetType());

            Assert.AreEqual(123, loaded1.OtherData);
            Assert.AreEqual("SomeData", loaded1.Data);
        }

        [Test]
        public static void TestDerivedTypeRef()
        {
            var loader = new InMemoryLoader();
            var resources = new GameResources(loader);
            var address1 = "/1";
            var json1 = @"{
                              ""$type"": ""SomeChildA"",
                              ""Data"": ""SomeData"",
                              ""OtherData"": ""123""
                         }";
            var address2 = "/2";
            var json2 = @"{
                              ""$type"": ""TestResource"",
                              ""InternalData"": ""/1""
                         }";

            loader.KnownFilesAdd(address1, new StringBuilder(json1));
            loader.KnownFilesAdd(address2, new StringBuilder(json2));

            var loaded2 = resources.LoadResource<BaseResource>(address2);
            Assert.AreEqual(typeof(TestResource), loaded2.GetType());

            var casted2 = (TestResource)loaded2;

            var loaded1 = casted2.InternalData.Target;
            Assert.AreEqual(typeof(SomeChildA), loaded1.GetType());

            Assert.AreEqual(123, loaded1.OtherData);
            Assert.AreEqual("SomeData", loaded1.Data);
        }

        [Test]
        public static void TestInlinedResource()
        {
            var loader = new InMemoryLoader();
            var resources = new GameResources(loader);
            var address = "/2";
            var json = @"{
                              ""$type"": ""TestResource"",
                              ""InternalData"": {
                                ""$type"": ""SomeChildA"",
                                ""Data"": ""SomeData"",
                                ""OtherData"": ""123""
                              }
                         }";

            loader.KnownFilesAdd(address, new StringBuilder(json));

            var loaded = resources.LoadResource<BaseResource>(address);
            Assert.AreEqual(typeof(TestResource), loaded.GetType());

            var casted = (TestResource)loaded;

            var loadedInlined = casted.InternalData.Target;
            Assert.AreEqual(typeof(SomeChildA), loadedInlined.GetType());

            Assert.AreEqual(123, loadedInlined.OtherData);
            Assert.AreEqual("SomeData", loadedInlined.Data);
        }

        [Test]
        public static void TestRefToInlinedResource()
        {
            var loader = new InMemoryLoader();
            var resources = new GameResources(loader);
            var address = "/2";
            var json = @"{
                              ""$type"": ""TestResource"",
                              ""InternalData"": {
                                ""$type"": ""SomeChildA"",
                                ""Data"": ""SomeData"",
                                ""OtherData"": ""123""
                              }
                         }";

            loader.KnownFilesAdd(address, new StringBuilder(json));

            var resRef = $"{address}:3:47";
            var loaded = resources.LoadResource<SomeClass>(resRef);
            Assert.AreEqual(typeof(SomeChildA), loaded.GetType());

            Assert.AreEqual(123, loaded.OtherData);
            Assert.AreEqual("SomeData", loaded.Data);
        }

        [Test]
        public static void TestInternalRef()
        {
            var loader = new InMemoryLoader();
            var resources = new GameResources(loader);
            var address = "/2";
            var json = @"{
                              ""$type"": ""TestResource"",
                              ""InternalData"": {
                                ""$type"": ""SomeChildA"",
                                ""$id"" : ""Resource1"",
                                ""Data"": ""SomeData"",
                                ""OtherData"": ""123""
                              },
                              ""InternalData2"": ""$Resource1""
                         }";

            loader.KnownFilesAdd(address, new StringBuilder(json));

            var loaded = resources.LoadResource<BaseResource>(address);
            Assert.AreEqual(typeof(TestResource), loaded.GetType());

            var casted = (TestResource)loaded;

            var loadedInlined = casted.InternalData.Target;
            Assert.AreEqual(typeof(SomeChildA), loadedInlined.GetType());

            Assert.AreEqual(123, loadedInlined.OtherData);
            Assert.AreEqual("SomeData", loadedInlined.Data);

            Assert.AreSame(casted.InternalData.Target, casted.InternalData2.Target);
        }
    }
}
