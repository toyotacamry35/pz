using System;
using System.Collections.Generic;
using System.IO;
using Assets.Editor.Tools;
using JetBrains.Annotations;
using Src.Animation;
using Src.Animation.ACS;
using Src.Tools;
using UnityEditor;
using UnityEditor.Animations;

namespace Assets.Editor.Animation.ACS
{
    [UsedImplicitly]
    public class AnimationStateProfileUpdater : AnimationStateComponentBatchUpdater<AnimationStateProfile>
    {
        protected override void Update(AnimationStateComponentUpdaterContext ctx, (AnimationStateProfile, AnimatorState, AnimatorStateMachine, int)[] comps)
        {
            if (comps == null || comps.Length == 0)
                return;
            
            var storage = LoadOrCreateInfoStorage(ctx.AssetPath);

            var infos = new List<(SerializableGuid, AnimationStateInfo)>();
            foreach (var tuple in comps)
            {
                infos.Add((tuple.Item1.Guid, AnimatorControllerUtils.GetStateInfo(tuple.Item2, tuple.Item4, ctx.AnimationController, ctx.AnimatorOverrideController, storage.GetSerializedInfo(tuple.Item1.Guid))));
            }

            bool dirty = false;
            if (storage.UpdateInfo(infos))
            {
                storage.Dirty();
                dirty = true;
            }

            if (dirty)
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(storage));
            }
        }
        
        private static AnimationStateInfoStorage LoadOrCreateInfoStorage(string path)
        {
            path = Path.ChangeExtension(path, ".info.asset");
            var storage = AssetDatabase.LoadAssetAtPath(path, typeof(AnimationStateInfoStorage)) as AnimationStateInfoStorage;
            if (!storage)
            {
                storage = AnimationStateInfoStorage.CreateInstance<AnimationStateInfoStorage>();
                path = AssetDatabase.GenerateUniqueAssetPath(path);
                AssetDatabase.CreateAsset(storage, path);
                storage = (AnimationStateInfoStorage)AssetDatabase.LoadAssetAtPath(path, typeof(AnimationStateInfoStorage)) ?? throw new NullReferenceException($"{nameof(AnimationStateInfoStorage)} in {path} is null");
            }
            return storage;
        }
    }
}
