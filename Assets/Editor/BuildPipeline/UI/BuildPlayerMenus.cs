using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Environment.Logging.Extension;
using NLog;
using UnityEditor;
using UnityEngine;

namespace Assets.Test.Src.Editor
{
    internal static class BuildPlayerMenus
    {
        private static NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        
        [MenuItem("Build/Modular/Release/Full Release Build", priority = 0)]
        private static int BuildFullRelease()
        {
            BuildPlayerCommon.CleanBuild();

            BuildReleasePlayer();
            BuildPlayerCommon.BuildShared(ignoreSettings: true);
            BuildPlayerCommon.BuildSystem(ignoreSettings: true);
            BuildPlayerCommon.BuildMap("Dropzone", ignoreSettings: true);
            BuildPlayerCommon.BuildMap("Savannah", ignoreSettings: true);
            return 0;
        }

        [MenuItem("Build/Modular/Release/Minimal Release Build", priority = 1)]
        private static int BuildMinimalRelease()
        {
            BuildPlayerCommon.CleanBuild(false);

            BuildReleasePlayer();
            BuildPlayerCommon.BuildShared(ignoreSettings: true);
            BuildPlayerCommon.BuildSystem(ignoreSettings: true);
            return 0;
        }
        
        [MenuItem("Build/Modular/Release/Shared and System Build", priority = 2)]
        private static int BuildSharedSystemRelease()
        {
            BuildPlayerCommon.CleanBuild(false);

            BuildPlayerCommon.BuildShared(ignoreSettings: true);
            BuildPlayerCommon.BuildSystem(ignoreSettings: true);
            return 0;
        }
        
        [MenuItem("Build/Modular/Debug/Full Debug Build", priority = 0)]
        private static int BuildFullDebug()
        {
            BuildPlayerCommon.CleanBuild();

            BuildDebugPlayer();
            BuildPlayerCommon.BuildShared(ignoreSettings: true);
            BuildPlayerCommon.BuildSystem(ignoreSettings: true);
            BuildPlayerCommon.BuildMap("Dropzone", ignoreSettings: true);
            BuildPlayerCommon.BuildMap("Savannah", ignoreSettings: true);
            return 0;
        }
        
        [MenuItem("Build/Modular/Debug/Minimal Debug Build", priority = 1)]
        private static int BuildMinimalDebug()
        {
            BuildPlayerCommon.CleanBuild(false);

            BuildDebugPlayer();
            BuildPlayerCommon.BuildShared(ignoreSettings: true);
            BuildPlayerCommon.BuildSystem(ignoreSettings: true);
            return 0;
        }
        
        [MenuItem("Build/Modular/Debug/Shared and System Build", priority = 2)]
        private static int BuildSharedSystemDebug()
        {
            BuildPlayerCommon.CleanBuild(false);

            BuildPlayerCommon.BuildShared(ignoreSettings: true);
            BuildPlayerCommon.BuildSystem(ignoreSettings: true);
            return 0;
        }

        [MenuItem("Build/Pipeline Debug/Cached Full Build", priority = 3)]
        private static int BuildCachedFull()
        {
            BuildDebugPlayer();
            BuildPlayerCommon.BuildShared(ignoreSettings: true);
            BuildPlayerCommon.BuildSystem(ignoreSettings: true);
            BuildPlayerCommon.BuildMap("Dropzone", ignoreSettings: true);
            BuildPlayerCommon.BuildMap("Savannah", ignoreSettings: true);
            return 0;
        }

        [MenuItem("Build/Pipeline Debug/Cached Build Bundles", priority = 5)]
        private static int BuildCachedBundles()
        {
            BuildPlayerCommon.BuildShared(ignoreSettings: true);
            BuildPlayerCommon.BuildSystem(ignoreSettings: true);
            BuildPlayerCommon.BuildMap("Dropzone", ignoreSettings: true);
            BuildPlayerCommon.BuildMap("Savannah", ignoreSettings: true);
            return 0;
        }

        [MenuItem("Build/Pipeline Debug/Cached Minimal Build", priority = 7)]
        private static int BuildCachedMinimalRelease()
        {
            BuildDebugPlayer();
            BuildPlayerCommon.BuildShared(ignoreSettings: true);
            BuildPlayerCommon.BuildSystem(ignoreSettings: true);
            return 0;
        }

        [MenuItem("Build/Pipeline Debug/Cached Shared Bundles", priority = 8)]
        private static int BuildCachedSharedBundles()
        {
            return BuildPlayerCommon.BuildShared(ignoreSettings: true);
        }

        [MenuItem("Build/Utility/Invalidate Local Dependency Cache", priority = 9)]
        public static void InvalidateCacheAll()
        {
            BuildPlayerCommon.InvalidateCacheAll();
        }


        [MenuItem("Build/Modular/Maps/Dropzone", priority = 11)]
        private static int BuildDropzone()
        {
            return BuildPlayerCommon.BuildMap("Dropzone");
        }

        [MenuItem("Build/Modular/Maps/SavannahGameplayTest", priority = 11)]
        private static int BuildSGT()
        {
            return BuildPlayerCommon.BuildMap("SavannahGameplayTest");
        }

        [MenuItem("Build/Modular/Maps/CraterSmall", priority = 11)]
        private static int BuildCraterSmall()
        {
            return BuildPlayerCommon.BuildMap("CraterSmall");
        }

        [MenuItem("Build/Modular/Maps/Jungle", priority = 11)]
        private static int BuildJungle()
        {
            return BuildPlayerCommon.BuildMap("Jungle");
        }

        [MenuItem("Build/Modular/Maps/Savannah", priority = 11)]
        private static int BuildSavannah()
        {
            return BuildPlayerCommon.BuildMap("Savannah");
        }

        [MenuItem("Build/Modular/Maps/Test_Obj", priority = 11)]
        private static int BuildTestObj()
        {
            return BuildPlayerCommon.BuildMap("test_obj");
        }

        [MenuItem("Build/Modular/Maps/QA", priority = 11)]
        private static int BuildQA()
        {
            return BuildPlayerCommon.BuildMap("QA");
        }

        [MenuItem("Build/Modular/Maps/Mobzone", priority = 11)]
        private static int BuildMobzone()
        {
            return BuildPlayerCommon.BuildMap("Mobzone");
        }

        [MenuItem("Build/Modular/Maps/Test_eco", priority = 11)]
        private static int BuildTestEco()
        {
            return BuildPlayerCommon.BuildMap("test_eco");
        }

        [MenuItem("Build/Modular/Maps/Test_carnage", priority = 11)]
        private static int BuildTestCarnage()
        {
            return BuildPlayerCommon.BuildMap("test_carnage");
        }

        [MenuItem("Build/Modular/Maps/Test_locomotion", priority = 11)]
        private static int BuildTestLocomotion()
        {
            return BuildPlayerCommon.BuildMap("test_locomotion");
        }

        [MenuItem("Build/Modular/Maps/Pool_Day", priority = 11)]
        private static int BuildPoolDay()
        {
            return BuildPlayerCommon.BuildMap("pool_day");
        }

        [MenuItem("Build/Modular/Player/Build Shared Bundles", priority = 10)]
        private static int BuildSharedBundles()
        {
            return BuildPlayerCommon.BuildShared();
        }

        [MenuItem("Build/Modular/Player/Build System Bundles", priority = 10)]
        private static int BuildSystemBundles()
        {
            return BuildPlayerCommon.BuildSystem();
        }

        [MenuItem("Build/Modular/Player/Build Debug Player", priority = 10)]
        private static int BuildDebugPlayer()
        {
            return BuildPlayerCommon.BuildPlayer(BuildResourcesInitializer.CommonConfig);
        }

        [MenuItem("Build/Modular/Player/Build Release Player", priority = 10)]
        private static int BuildReleasePlayer()
        {
            return BuildPlayerCommon.BuildPlayer(BuildResourcesInitializer.ReleaseConfig);
        }

        [MenuItem("Build/Modular/Maps/BuildArena", priority = 11)]
        private static int BuildMap()
        {
            return BuildPlayerCommon.BuildMap("test_arena");
        }

        [MenuItem("Build/Modular/Maps/test_arena_interactive", priority = 11)]
        private static int BuildTestArenaInteractive()
        {
            return BuildPlayerCommon.BuildMap("test_arena_interactive");
        }

        [MenuItem("Build/Modular/Maps/Test_Mobs_Battle", priority = 11)]
        private static int BuildTest_Mobs_Battle()
        {
            return BuildPlayerCommon.BuildMap("Test_Mobs_Battle");
        }

        [MenuItem("Build/Replace Shader In Selected Materials")]
        private static void ReplaceShaders()
        {
            EditorUtility.DisplayCancelableProgressBar("Parse shader", "", 0);
            
            var materials = Selection.objects.Where(o => o as Material != null).Cast<Material>().ToArray();
            if (materials.Length == 0) return;
            var preloadedShaders = ShaderHelper.PreloadBuiltinShaders();
            var shaderNameToShaderPath = ShaderHelper.ParseShaderFolder();
            var changedMaterials = new List<Material>();
            var buildingShaders = new List<GUID>();
            
            EditorUtility.DisplayCancelableProgressBar("Replace material", "", 0.25f);
            foreach (var material in materials)
            {
                var path = AssetDatabase.GetAssetPath(material);

                var result = ShaderHelper.CheckAndReplaceShader(path, shaderNameToShaderPath, preloadedShaders, buildingShaders, changedMaterials);
                if (result <= 0) continue;
                 Logger.IfError()?.Message("Could not replace shader").Write();;
                return;
            }
            AssetDatabase.SaveAssets();
            EditorUtility.ClearProgressBar();
        }
    }
}