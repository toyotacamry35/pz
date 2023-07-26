using System.Reflection;
using System.Text;
using Assets.Src.ResourcesSystem.Base;
using Assets.Src.GameObjectAssembler.Res;
using Assets.Src.Aspects.Templates;
using NUnit.Framework;
using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates;
using Assets.Src.GameObjectAssembler.TestComponents;
using ResourcesSystem.Loader;
using UnityEngine;
using Assets.Src.RubiconAI.Editor;
using Assets.Src.ResourceSystem.Editor;
using ResourcesSystem;

namespace Assets.Src.GameObjectAssembler.Editor
{
    public class GameObjectAssemblerTests
    {
        /*[Test]
        public static void ReloadAllResources()
        {
            //var db = MetaFileDatabaseInitializer.FillJdbMetaDB();
            GameResourcesHolder.Instance = new GameResources(new FolderLoader(Application.dataPath));
            GameResourcesHolder.Instance.ConfigureForUnityEditor();
            foreach (var jdb in db.MetaFilesMap)
            {
                jdb.Value.Get();
            }
        }*/
        [Test]
        public static void TestUth()
        {
            var gameResourcers = new GameResources(new FolderLoader(Application.dataPath));
            gameResourcers.ConfigureForUnityEditor();
            gameResourcers.LoadResource<UnityGameObjectDef>("/AI/Defs/Mobs/Uth");
            var gameResourcers2 = new GameResources(new FolderLoader(Application.dataPath));
            gameResourcers2.ConfigureForUnityRuntime();
            gameResourcers2.LoadResource<UnityGameObjectDef>("/AI/Defs/Mobs/Uth");
        }
        [Test]
        public static void TestTrivial()
        {
            UnityGameObjectDef def = new UnityGameObjectDef();
            var rb = new RigidbodyDef();
            rb.Mass = 4;

            def.Components = new ResourceRef<ComponentDef>[] { rb };

            JsonToGO assembler = JsonToGO.Instance;
            var go = assembler.Assemble(def);
            Assert.NotNull(go);
            var rbComp = go.GetComponent<UnityEngine.Rigidbody>();
            Assert.NotNull(rbComp);
            Assert.AreEqual(rb.Mass, rbComp.mass);
        }

        [Test]
        public static void TestRef()
        {
            UnityGameObjectDef def = new UnityGameObjectDef();
            var tc1 = new TestComponent1Def();
            var tc2 = new TestComponent2Def();
            tc1.Comp2Ref = tc2;

            def.Components = new ResourceRef<ComponentDef>[] { tc1, tc2 };

            JsonToGO assembler = JsonToGO.Instance;
            var go = assembler.Assemble(def);
            Assert.NotNull(go);
            var tc1Comp = go.GetComponent<TestComponents.TestComponent1>();
            Assert.NotNull(tc1Comp);

            var tc2Comp = go.GetComponent<TestComponents.TestComponent2>();
            Assert.NotNull(tc1Comp);

            Assert.AreSame(tc2Comp, tc1Comp.m_Comp2Ref);
            Assert.Null(tc1Comp.Ref);
        }

        [Test]
        public static void TestEnum()
        {
            UnityGameObjectDef res = new UnityGameObjectDef();
            var cr = new TestEnumComponentDef
            {
                Field = Assets.Src.GameObjectAssembler.Editor.TestEnum.Value2
            };

            res.Components = new ResourceRef<ComponentDef>[] { cr };

            JsonToGO assembler = JsonToGO.Instance;
            var go = assembler.Assemble(res);
            Assert.NotNull(go);
            var comp = go.GetComponent<TestComponents.TestEnumComponent>();
            Assert.NotNull(comp);
            Assert.AreEqual(Assets.Src.GameObjectAssembler.Editor.TestEnum.Value2, comp.Field);
        }

        [Test]
        public static void TestMerge()
        {
            UnityGameObjectDef res = new UnityGameObjectDef();
            var cr = new TPCMStubDef
            {
                ReuseExisting = true
            };
            cr.CanMoveAndRotate = new PredicateFalseDef();

            res.Components = new ResourceRef<ComponentDef>[] { cr };

            var go = new GameObject();
            var refHolder = go.AddComponent<JsonRefHolder>();
            refHolder.Definition = res;

            go.AddComponent<TPCMStub>();

            JsonToGO assembler = JsonToGO.Instance;
            assembler.Merge(go);
            Assert.NotNull(go);

            var allComponents = go.GetComponents<TPCMStub>();
            Assert.AreEqual(1, allComponents.Length);

            var tpmNew = allComponents[0];

            var targetField =
                typeof(TPCMStub).GetField("_canMoveAndRotate", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.NotNull(targetField);
            var targetPred = (PredicateDef)targetField.GetValue(tpmNew);

            Assert.AreSame(targetPred, cr.CanMoveAndRotate.Target);

        }
        [Test]
        public static void TestMergeComponentsAndObjectsOnTheirRespectivePlaces()
        {
            UnityGameObjectDef res = new UnityGameObjectDef();
            var cr = new TPCMStubDef
            {
                ReuseExisting = true
            };
            cr.CanMoveAndRotate = new PredicateFalseDef();
            var tc1 = new TestComponent2Def();
            var tc2 = new TestComponent2Def();
            var tc3 = new TestComponent2Def();
            res.Components = new ResourceRef<ComponentDef>[] { cr, tc1 };
            res.Children = new ResourceRef<UnityGameObjectDef>[] { new UnityGameObjectDef() { Name = "TestName", Components = new ResourceRef<ComponentDef>[] { tc2, tc3 } } };
            var go = new GameObject();
            var refHolder = go.AddComponent<JsonRefHolder>();
            refHolder.Definition = res;

            go.AddComponent<TPCMStub>();

            JsonToGO assembler = JsonToGO.Instance;
            assembler.Merge(go);
            Assert.NotNull(go);
            Assert.AreEqual(go.transform.childCount, 1);
            Assert.AreEqual(go.GetComponents<TestComponent2>().Length, 1);
            Assert.AreEqual(go.transform.GetChild(0).name, "TestName");
            Assert.NotNull(go.GetComponent<TestComponent2>());
            Assert.AreEqual(go.transform.GetChild(0).GetComponents<TestComponent2>().Length, 2);

        }
        [Test]
        public static void TestJsonLoadingBasic()
        {
            var json = @"{
            ""$type"": ""UnityGameObjectDef"",
            ""Name"": ""PlayerPawn"",
            ""Components"": [
            {
                ""$type"": ""TPCMStubDef"",
                ""ReuseExisting"": true,
            
                ""CanMoveAndRotate"": {
                    ""$type"": ""PredicateFalse""
                }
            },
            {
                ""$type"":""TestComponent2Def""
            } 
            ],
            ""Children"":
            [
                ""/file2""
            ]
        }";

            var json2 = @"
                {
                    ""$type"":""UnityGameObjectDef"",
                    ""Name"":""TestName"",
                    ""Components"":
                    [
                        {
                            ""$type"":""TestComponent2Def""
                        },
                        {
                            ""$type"":""TestComponent2Def""
                        } 
                    ]
                }";
            var loader = new InMemoryLoader();
            loader.KnownFilesAdd("/file", new StringBuilder(json));
            loader.KnownFilesAdd("/file2", new StringBuilder(json2));
            var gameResourcers = new GameResources(loader);
            UnityGameObjectDef res = gameResourcers.LoadResource<UnityGameObjectDef>("/file");
            var go = new GameObject();
            var refHolder = go.AddComponent<JsonRefHolder>();
            refHolder.Definition = res;

            go.AddComponent<TPCMStub>();

            JsonToGO assembler = JsonToGO.Instance;
            assembler.Merge(go);
            Assert.NotNull(go);
            Assert.AreEqual(go.transform.childCount, 1);
            Assert.AreEqual(go.GetComponents<TestComponent2>().Length, 1);
            Assert.AreEqual(go.transform.GetChild(0).name, "TestName");
            Assert.NotNull(go.GetComponent<TestComponent2>());
            Assert.AreEqual(go.transform.GetChild(0).GetComponents<TestComponent2>().Length, 2);

        }
        [Test]
        public static void PrototypesSimple()
        {
            var json = @"{
                ""$type"":""TestComponentPrototypeDef"",
                ""$proto"":""/file2"",
                ""Components"":[ ]
            }";

            var json2 = @"
                {
                    ""$type"":""TestComponentPrototypeDef"",
                    ""Value1"":10,
                    ""Value2"":20,
                    ""Components"":[ ]
                }";
            var loader = new InMemoryLoader();
            loader.KnownFilesAdd("/file", new StringBuilder(json));
            loader.KnownFilesAdd("/file2", new StringBuilder(json2));
            var gameResourcers = new GameResources(loader);
            TestComponentPrototypeDef res = gameResourcers.LoadResource<TestComponentPrototypeDef>("/file");
            TestComponentPrototypeDef res2 = gameResourcers.LoadResource<TestComponentPrototypeDef>("/file2");
            Assert.AreEqual(10f, res.Value1);
            Assert.AreEqual(20f, res.Value2);
            //not the same instance of a list
            Assert.AreNotSame(res.Components, res2.Components);
        }
        [Test]
        public static void PototypesWithVariables()
        {
            var json = @"{
                
                
                    ""$type"":""TestComponentPrototypeDef"",
                    ""$vars"":{
                    ""ValueVar1"":{ ""Type"":""float"", ""Value"":2 },
                    ""ValueVar2"":{ ""Type"":""float"", ""Value"":3 }    
                },
                    ""Value1"":""@ValueVar1"",
                    ""Value2"":""@ValueVar2"",
                    ""Components"":[ ]
            }";

            var json2 = @"
                {
                    ""$type"":""TestComponentPrototypeDef"",
                    ""$overrideVars"" : { ""ValueVar1"" : 10, ""ValueVar2"" : 20},
                    ""$proto"":""/file"",
                    ""Components"":[ ]
                }";
            var loader = new InMemoryLoader();
            loader.KnownFilesAdd("/file", new StringBuilder(json));
            loader.KnownFilesAdd("/file2", new StringBuilder(json2));
            var gameResourcers = new GameResources(loader);
            TestComponentPrototypeDef res = gameResourcers.LoadResource<TestComponentPrototypeDef>("/file");
            TestComponentPrototypeDef res2 = gameResourcers.LoadResource<TestComponentPrototypeDef>("/file2");
            Assert.AreEqual(res.Value1, 2);
            Assert.AreEqual(res.Value2, 3);
            Assert.AreEqual(res2.Value1, 10);
            Assert.AreEqual(res2.Value2, 20);
            //not the same instance of a list
            Assert.AreNotSame(res.Components, res2.Components);

        }

       

    }

    public class TPCMStub : MonoBehaviour
    {
        private PredicateDef _canMoveAndRotate;
    }
}