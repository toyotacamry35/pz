using System;
using System.Linq;
using Assets.Editor.Tools;
using Core.Reflection;
using Src.Animation.ACS;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Editor.Animation.ACS
{
    public class AnimationStateComponentsPostprocessor // : AssetPostprocessor
    {
        private static bool DebugEnabled = false;
        
        private static ValueTuple<Type, AnimationStateComponentUpdater>[] _updaters;
        private static ValueTuple<Type, AnimationStateComponentBatchUpdater>[] _batchUpdaters;
        
        private static ValueTuple<Type, AnimationStateComponentUpdater>[] Updaters => _updaters ?? (_updaters = GatherUpdaters());
        private static ValueTuple<Type, AnimationStateComponentBatchUpdater>[] BatchUpdaters => _batchUpdaters ?? (_batchUpdaters = GatherBatchUpdaters());

        [MenuItem("Build/Prepare Character Animator")]
        public static void UpdateAll()
        {
            try
            {
                var assets = AssetDatabase.FindAssets("t:AnimatorController").Concat(AssetDatabase.FindAssets("t:AnimatorOverrideController")).ToArray();
                for (int i = 0; i < assets.Length; ++i)
                {
                    var path = AssetDatabase.GUIDToAssetPath(assets[i]);
                    EditorUtility.DisplayProgressBar("Updating Animation trajectories", path, (float)i / assets.Length);
                    ProcessAnimatorAtPath(path);
                }
            } 
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }
        
//        private void OnPreprocessAsset()
//        {
//            if (typeof(AnimatorController).IsAssignableFrom(AssetDatabase.GetMainAssetTypeAtPath(assetPath)))
//            {
//                if (DebugEnabled) Debug.Log($"OnPreprocessAsset: {assetImporter} {assetImporter.assetPath} {assetPath} {assetImporter.assetTimeStamp} {assetImporter.importSettingsMissing} {assetImporter.userData}");
//                ProcessAnimatorAtPath(assetPath);
//            }
//        }

        private static void ProcessAnimatorAtPath(string path)
        {
            var type = AssetDatabase.GetMainAssetTypeAtPath(path);
            Object obj = null;
            var debugEnabled = AnimationStateComponentSupport.DebugEnabled;
            AnimationStateComponentSupport.DebugEnabled = DebugEnabled;
            try
            {
                if (type == typeof(AnimatorController))
                {
                    var ctx = new AnimationStateComponentUpdaterContext((AnimatorController)(obj = AssetDatabase.LoadMainAssetAtPath(path)), null, path);
                    ProcessAnimator(ctx);
                }
                else
                if (type == typeof(AnimatorOverrideController))
                {
                    var @override = (AnimatorOverrideController)(obj = AssetDatabase.LoadMainAssetAtPath(path));
                    var so = new SerializedObject(@override);
                    var prop = so.FindProperty("m_Controller");
                    var animator = (AnimatorController)prop.objectReferenceValue;
                    if (animator)
                    {
                        var ctx = new AnimationStateComponentUpdaterContext(animator, @override, path);
                        ProcessAnimator(ctx);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e, obj);
            }
            finally
            {
                AnimationStateComponentSupport.DebugEnabled = debugEnabled;
            }
        }

        private static void ProcessAnimator(AnimationStateComponentUpdaterContext ctx)
        {
            if (ctx.AnimationController == null) throw new ArgumentNullException(nameof(ctx.AnimationController));
            
            var tuples = AnimatorControllerUtils.FindBehaviours<AnimationStateComponent>(ctx.AnimationController);

            foreach (var tuple in tuples)
                try
                {
                    BuildUpComponentStructure(tuple.Item1, tuple.Item2);
                }
                catch (Exception e)
                {
                    Debug.LogError($"{ctx.AssetPath}: {tuple.Item2.name}: {e}", ctx.AnimationController);
                }

            // BuildUpComponentStructure могла нагенерить новых компонент
            tuples = AnimatorControllerUtils.FindBehaviours<AnimationStateComponent>(ctx.AnimationController);

            foreach (var updater in Updaters)
            {
                var batch = tuples.Where(x => updater.Item1.IsInstanceOfType(x.Item1)).ToArray();
                foreach (var tuple in batch)
                    try
                    {
                        if(DebugEnabled) Debug.Log($"Update Component:{tuple.Item1.GetType().Name} on State:{tuple.Item2.name} with Updater:{updater.Item2.GetType().Name}");
                        updater.Item2.Update(ctx, tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"{ctx.AssetPath}: {tuple.Item2.name}: {e}", ctx.AnimationController);
                    }
            }

            foreach (var updater in BatchUpdaters)
            {
                var batch = tuples.Where(x => updater.Item1.IsInstanceOfType(x.Item1)).ToArray();
                try
                {
                    if(DebugEnabled) Debug.Log($"Update Components:{batch.Length} with Updater:{updater.Item2.GetType().Name}");
                    updater.Item2.Update(ctx, batch);
                }
                catch (Exception e)
                {
                    Debug.LogError($"{ctx.AssetPath}: {e}", ctx.AnimationController);
                }
            }
        }        
        
        private static void BuildUpComponentStructure(AnimationStateComponent comp, AnimatorState state)
        {
            var header = state.GetOrCreateHeader();
            header.AddComponentIfNeed(comp);

            //if (comp.name != state.name)
            if (!string.IsNullOrEmpty(comp.name))
            {
                //comp.name = state.name;
                comp.name = string.Empty;
                comp.Dirty();
            }

            if (comp is AnimationStateComponentWithGuid acg && !acg.Guid.IsValid)
            {
                acg.GenerateGuid(false);
                comp.Dirty();
            }
            
            var compType = comp.GetType();
            foreach (var reqAttr in compType.GetCustomAttributesSafe<RequireComponent>(true))
            {
                CreateRequiredComponent(state, reqAttr.m_Type0);
                CreateRequiredComponent(state, reqAttr.m_Type1);
                CreateRequiredComponent(state, reqAttr.m_Type2);
            }
            
            foreach (var reqAttr in compType.GetCustomAttributesSafe<DisallowMultipleComponent>(true))
            {
                if(state.behaviours.Any(x => !ReferenceEquals(x, comp) && x.GetType() == compType))
                    throw new Exception($"Duplicate component {compType.Name} on state {state.name}");
            }
        }

        private static void CreateRequiredComponent(AnimatorState state, Type compType)
        {
            if (compType == null)
                return;
            state.GetOrCreateComponent(compType, true);
        }
        
        private static ValueTuple<Type, AnimationStateComponentUpdater>[] GatherUpdaters()
        {
            return AppDomain.CurrentDomain.GetAssembliesSafeNonStandard()
                .Where(v => !v.IsDynamic)
                .SelectMany(v => v.GetTypesSafe())
                .Where(type => typeof(AnimationStateComponentUpdater).IsAssignableFrom(type) && !type.IsAbstract)
                .Select(x => ValueTuple.Create(
                    GetAnimationStateComponentUpdaterType(x, typeof(AnimationStateComponentUpdater<>)),
                    (AnimationStateComponentUpdater) Activator.CreateInstance(x)))
                .ToArray();
        }

        private static ValueTuple<Type, AnimationStateComponentBatchUpdater>[] GatherBatchUpdaters()
        {
            return AppDomain.CurrentDomain.GetAssembliesSafeNonStandard()
                .Where(v => !v.IsDynamic)
                .SelectMany(v => v.GetTypesSafe())
                .Where(type => typeof(AnimationStateComponentBatchUpdater).IsAssignableFrom(type) && !type.IsAbstract)
                .Select(x => ValueTuple.Create(
                    GetAnimationStateComponentUpdaterType(x, typeof(AnimationStateComponentBatchUpdater<>)),
                    (AnimationStateComponentBatchUpdater) Activator.CreateInstance(x)))
                .ToArray();
        }

        private static Type GetAnimationStateComponentUpdaterType(Type t, Type match)
        {
            if (t.IsGenericType && t.GetGenericTypeDefinition() == match)
                return t.GenericTypeArguments[0];
            return GetAnimationStateComponentUpdaterType(t.BaseType, match);
        }
    }
}