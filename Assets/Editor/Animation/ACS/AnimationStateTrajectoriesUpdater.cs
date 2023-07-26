using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Assets.ColonyShared.SharedCode.Utils;
using Assets.Editor.Tools;
using Assets.Src.ResourcesSystem.Base;
using JetBrains.Annotations;
using ResourceSystem.Aspects.Misc;
using Src.Animation;
using Src.Animation.ACS;
using Src.Tools;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Editor.Animation.ACS
{
    [UsedImplicitly]
    public class AnimationStateTrajectoriesUpdater : AnimationStateComponentBatchUpdater<AnimationStateTrajectory>
    {
        protected override void Update(AnimationStateComponentUpdaterContext ctx, (AnimationStateTrajectory, AnimatorState, AnimatorStateMachine, int)[] comps)
        {
            GenerateTrajectories(ctx.AnimationController, ctx.AssetPath, comps);
        }
        
        private static void GenerateTrajectories(AnimatorController ac, string acPath, (AnimationStateTrajectory component, AnimatorState state, AnimatorStateMachine stateMachine, int layer)[] comps)
        {
            var inputs = comps.Select(x => (x.state, x.layer, boneMarker: x.component.BodyPart.EDITOR_Target, guid:x.component.Guid)).ToArray();
            if (inputs.Length == 0)
               return;

            var storage = CreateTrajectoriesStorage(acPath);
            var settings = storage.Settings ?? CreateTrajectoriesSettings(storage);
            var body = settings.Body;
            var bodyMarker = body.GetMarkerRef();
            
            if (!body)
            {
                Debug.LogError($"No body for trajectories defined at {acPath}", storage);
                return;
            }

            var trajectoryTuples = AnimationTrajectoryGenerator
                .GenerateTrajectories(body, bodyMarker.EDITOR_Target, ac, inputs.Select(x => (x.state, x.layer, GetAndCheckBoneMarker(x, ac))), settings.TimeStep, settings.OptimizationTolerance)
                .Select(x => (bodyMarker, x.boneMarker, x.state, x.trajectory, inputs.Single(y => y.state == x.state && y.boneMarker == x.boneMarker.EDITOR_Target).guid))
                .ToArray();

            var oldTrajectories = LoadTrajectories(storage);
            
            var trajectoriesPool = (!settings.ForceUpdate ? oldTrajectories.Concat(trajectoryTuples.Select(x => x.trajectory)) : trajectoryTuples.Select(x => x.trajectory)).ToArray(); // сначала старые потом новые, чтобы реюзать старые
            for (int i = 0; i < trajectoryTuples.Length; ++i)
            {
                var tuple = trajectoryTuples[i];
                var trajectory = trajectoriesPool.FirstOrDefault(x => tuple.trajectory.EqualsByContent(x, settings.EqualsTolerance));
                if (trajectory)
                    trajectoryTuples[i].trajectory = trajectory;
            }
            
            int newTrajectories = 0;
            int deletedTrajectories = 0;
            int trajectoryId = 0;
            var usedTrajectories = trajectoryTuples.Select(x => x.trajectory).Distinct().ToArray();
            foreach (var trajectory in usedTrajectories)
            {
                try
                {
                    if (!oldTrajectories.Any(x => ReferenceEquals(x, trajectory)))
                    {
                        trajectory.name = GenerateTrajectoryName(trajectoriesPool, ref trajectoryId); 
                        trajectory.Position.name = $"{trajectory.name} | position";
                        trajectory.Rotation.name = $"{trajectory.name} | rotation";
                        storage.AddTrajectoryAsset(trajectory);
                        storage.Dirty();
                        newTrajectories++;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogException(e, storage);
                }
            }
            
            foreach (var trajectory in oldTrajectories)
            {
                if (!usedTrajectories.Contains(trajectory))
                {
                    try
                    {
                        storage.RemoveTrajectoryAsset(trajectory);
                        storage.Dirty();
                        deletedTrajectories++;
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e, storage);
                    }
                }
            }

            var registryUpdated = storage.UpdateRegistry(trajectoryTuples.Select(x => (x.guid, $"{x.state.name} | {x.boneMarker.EDITOR_Target.____GetDebugShortName()}", x.trajectory)).ToArray());
            if (registryUpdated)
                storage.Dirty();

            if (newTrajectories > 0 || deletedTrajectories > 0 || registryUpdated)
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(storage));
            }

            Debug.Log($"{AssetDatabase.GetAssetPath(storage)}: {usedTrajectories.Length} trajectories; {newTrajectories} added; {deletedTrajectories} deleted");
        }

        private static EditorBaseResourceWrapper<GameObjectMarkerDef>[] GetAndCheckBoneMarker((AnimatorState state, int layer, EditorBaseResourceWrapper<GameObjectMarkerDef> boneMarker, SerializableGuid guid) tuple, AnimatorController ac)
        {
            if (tuple.boneMarker == null)
            {
                Debug.LogError($"No body part defined in {nameof(AnimationStateTrajectory)} for state {tuple.state.name} defined at {AssetDatabase.GetAssetPath(ac)}");
                return null;
            }
            return new [] {tuple.boneMarker};
        }

        private static AnimationTrajectoriesSettings CreateTrajectoriesSettings(AnimationTrajectoriesStorage storage)
        {
            var settings = AnimationTrajectoriesSettings.CreateInstance<AnimationTrajectoriesSettings>();
            settings.name = nameof(AnimationTrajectoriesSettings);
            AssetDatabase.AddObjectToAsset(settings, storage);
            AssetDatabase.SetMainObject(storage, AssetDatabase.GetAssetPath(storage));
            settings.Dirty();
            storage.Settings = settings;
            storage.Dirty();
            return settings;
        }
        
        private static AnimationTrajectoriesStorage CreateTrajectoriesStorage(string path)
        {
            path = Path.ChangeExtension(path, ".trajectories.asset");
            var storage = AssetDatabase.LoadAssetAtPath(path, typeof(AnimationTrajectoriesStorage)) as AnimationTrajectoriesStorage;
            if (!storage)
            {
                storage = AnimationTrajectoriesStorage.CreateInstance<AnimationTrajectoriesStorage>();
                path = AssetDatabase.GenerateUniqueAssetPath(path);
                AssetDatabase.CreateAsset(storage, path);
                storage = (AnimationTrajectoriesStorage)AssetDatabase.LoadAssetAtPath(path, typeof(AnimationTrajectoriesStorage)) ?? throw new NullReferenceException($"{nameof(AnimationTrajectoriesStorage)} in {path} is null");
            }
            return storage;
        }

        private static AnimationTrajectory[] LoadTrajectories(AnimationTrajectoriesStorage storage)
        {
            string assetPath = AssetDatabase.GetAssetPath(storage);
            return AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath).OfType<AnimationTrajectory>().ToArray();
        }

        private static string GenerateTrajectoryName(AnimationTrajectory[] trajectories, ref int counter)
        {
            string name;
            do
            {
                name = $"trajectory_{counter++}";
            }
            while (trajectories.Any(x => x.name == name));
            return name;
        }
    }
}