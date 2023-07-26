using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Src.ResourcesSystem.Base;
using Assets.Src.ResourceSystem;
using ColonyShared.SharedCode.Aspects.Misc;
using Core.Environment.Logging.Extension;
using GeneratedCode.Aspects.Combat;
using NLog;
using ResourcesSystem.Loader;
using ResourceSystem.Aspects.Misc;
using Src.Animation;
using Src.Animation.ACS;
using Src.Tools;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Assets.Editor.Tools
{
    public static class AnimationTrajectoryGenerator
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public static (AnimatorState state, GameObjectMarkerRef boneMarker, AnimationTrajectory trajectory)[] GenerateTrajectories(
            GameObject bodyPrefab,
            EditorBaseResourceWrapper<GameObjectMarkerDef> bodyMarker,
            AnimatorController ac,
            IEnumerable<(AnimatorState state, int layer, EditorBaseResourceWrapper<GameObjectMarkerDef>[] boneMarkers)> states,
            float timeStep,
            float optimizationTolerance)
        {
            if (bodyPrefab == null) throw new ArgumentNullException(nameof(bodyPrefab));
            if (ac == null) throw new ArgumentNullException(nameof(ac));
            
            var prefabPath = AssetDatabase.GetAssetPath(bodyPrefab);
            var bodyInstance = (GameObject)PrefabUtility.InstantiatePrefab(bodyPrefab);
            bodyInstance.transform.position = Vector3.zero;
            bodyInstance.transform.localScale = Vector3.one;
            bodyInstance.transform.rotation = Quaternion.identity;
            var bodyPartDefs = states.SelectMany(x => x.boneMarkers).Distinct().ToArray(); 

            try
            {
                var animator = bodyInstance.GetComponentsInChildren<Animator>().FirstOrDefault(x => x.runtimeAnimatorController == ac || (x .runtimeAnimatorController is AnimatorOverrideController over) && over.runtimeAnimatorController == ac);
                if (!animator)
                    throw new Exception($"Animator {ac} not found in {prefabPath}");

                var bodyParts = bodyInstance.GetComponentsInChildren<GameObjectMarker>()
                    .Where(x => x.EDITOR_MarkerDef != null && bodyPartDefs.Any(y => y == x.EDITOR_MarkerDef))
                    .ToDictionary(x => x.EDITOR_MarkerDef, x => x.transform);
                
                if (bodyPartDefs.Any(x => !bodyParts.ContainsKey(x)))
                    throw new Exception($"Body parts [{string.Join(", ", bodyPartDefs.Where(x => !bodyParts.ContainsKey(x)).Select(x => x.____GetDebugAddress()))}] not found in {prefabPath}");

                return states
                    .SelectMany(state =>
                    {
                        var result = state.boneMarkers.Select(marker => (transform: bodyParts[marker], trajectory: AnimationTrajectory.CreateInstance(), boneMarker: marker)).ToArray();
                        GenerateTrajectories(bodyInstance, bodyMarker, animator, state.state, state.layer, timeStep, optimizationTolerance, result);
                        return result.Select((x, i) => (state.state, x.transform.GetMarkerRef(), x.trajectory));
                    })
                    .ToArray();
            }
            finally
            {
                GameObject.DestroyImmediate(bodyInstance);
            }
        }


        private static void GenerateTrajectories(
            GameObject body, 
            EditorBaseResourceWrapper<GameObjectMarkerDef> bodyMarker,
            Animator animator, 
            AnimatorState state, 
            int layerIdx, 
            float timeStep, 
            float optimizationTolerance,
            (Transform transform, AnimationTrajectory trajectory, EditorBaseResourceWrapper<GameObjectMarkerDef> boneMarker)[] result)
        {
            float duration = GetMotionLength(state.motion, animator);

            foreach (var tuple in result)
            {
                tuple.trajectory.Clear();
                if (tuple.transform.root != body.transform) throw new Exception($"BodyPart:{tuple.transform.FullName()} Root:{tuple.transform.root.name} ({tuple.transform.root.GetInstanceID()}) Body:{body.name} ({body.GetInstanceID()})");
            }

            var timeParameterActive = state.timeParameterActive;
            state.timeParameterActive = false;

            var speedParameterActive = state.speedParameterActive;
            state.speedParameterActive = false;

            var speed = state.speed;
            state.speed = 1;

            int steps = Mathf.CeilToInt(duration / timeStep);
            try
            {
                var invBodyRot = Quaternion.Inverse(body.transform.rotation);
                if (steps > 0)
                    for (int i = 0; i <= steps; ++i)
                    {
                        float time = Mathf.Min(i * timeStep, duration);
                        animator.PlayInFixedTime(state.name, layerIdx, time);
                        animator.Update(timeStep);
                        foreach (var tuple in result)
                        {
                            var pos = body.transform.InverseTransformPoint(tuple.transform.position);
                            var rot = invBodyRot * tuple.transform.rotation;
                            tuple.trajectory.AddKey(time, pos, rot);
                        }
                    }
            }
            finally
            {
                state.timeParameterActive = timeParameterActive;
                state.speedParameterActive = speedParameterActive;
                state.speed = speed;
                PrefabUtility.RevertObjectOverride(body, InteractionMode.AutomatedAction);
            }

            var stateDescriptor = state.behaviours.OfType<AnimationStateDescriptor>().First();

            string trajectoriesDir = AttackDoerServerAnimationTrajectory.GetTrajectoriesDir(bodyMarker.Resource);
            string exportName = AttackDoerServerAnimationTrajectory.GetTrajectoryName(stateDescriptor.StateDef);
            string exportPath = AttackDoerServerAnimationTrajectory.GetTrajectoryPath(bodyMarker.Resource, stateDescriptor.StateDef);
            TrajectoryAnimationSetDef trajectoryAnimationSetDef = new TrajectoryAnimationSetDef();
            TrajectoryAnimationSetDef oldTrajectoryAnimationSetDef = null;
            var gr = EditorGameResourcesForMonoBehaviours.GetNew();
            try { oldTrajectoryAnimationSetDef = gr.TryLoadResource<TrajectoryAnimationSetDef>(exportPath); } catch (Exception) {}
            if (oldTrajectoryAnimationSetDef != null)
            {
                foreach (var pair in oldTrajectoryAnimationSetDef.Trajectories)
                {
                    foreach (var tuple in result)
                        if (new EditorBaseResourceWrapper<GameObjectMarkerDef>(pair.Key) == tuple.boneMarker)
                            goto skip;
                    var tr = pair.Value.Target;
                    ((IResource)tr).Address = new ResourceIDFull(); // это необходимо, чтобы в дальнейшем траектория нормально сохранилась в виде объекта, а не в виде ссылки на себя саму. 
                    trajectoryAnimationSetDef.Trajectories.Add(pair.Key, tr);
                    skip: ;
                }
            }

            foreach (var tuple in result)
            {
                tuple.trajectory.Recalculate(timeStep * 0.5f);
                var tragectoryDef = tuple.trajectory.GenerateTrajectoryDef(TrajectoryDefTimeStep, TrajectoryDefLengthStep);
                trajectoryAnimationSetDef.Trajectories.Add(tuple.boneMarker.Resource, tragectoryDef);
                tuple.trajectory.Optimize(optimizationTolerance);
            }

            GameResources.SimpleSave($"{GameResourcesHolder.Instance.Deserializer.Loader.GetRoot()}{trajectoriesDir}", exportName, trajectoryAnimationSetDef, out string resultPath);
            Logger.IfDebug()?.Message($"Saved trajectorySet to {resultPath}").Write();
        }

        private static float GetMotionLength(Motion motion, Animator animator)
        {
            if (motion is AnimationClip clip)
                return GetRealClip(clip, animator)?.length ?? 0;
            if (motion is BlendTree blend)
                return blend.children.Select(x => GetMotionLength(x.motion, animator)).Average();
            return 0;
        }

        private static AnimationClip GetRealClip(AnimationClip clip, Animator animator)
        {
            if (animator.runtimeAnimatorController is AnimatorOverrideController over)
                return over[clip.name] ?? clip;
            return clip;
        }
        
        private const float TrajectoryDefTimeStep = 0.1f;
        
        private const float TrajectoryDefLengthStep = 0.1f;

        /*
        public static Tuple<string, JdbMetadata, AnimationTrajectory>[] GenerateAttackTrajectories(GameObject bodyPrefab, AnimatorController ac, float timeStep)
        {
            if (bodyPrefab == null) throw new ArgumentNullException(nameof(bodyPrefab));
            if (ac == null) throw new ArgumentNullException(nameof(ac));

            var bodyInstance = GameObject.Instantiate(bodyPrefab, Vector3.zero, Quaternion.identity);

            try
            {
                var animator = bodyInstance.GetComponentsInChildren<Animator>().FirstOrDefault(x => x.runtimeAnimatorController == ac);
                if (!animator)
                    throw new Exception($"Animator {ac} not found in {bodyPrefab}");

                var bones = bodyInstance.GetComponentsInChildren<AttackEventSubscriptionHandler>().Select(x => x.transform).ToArray();
                if (bones.Length == 0)
                    throw new Exception($"Bones with {nameof(AttackEventSubscriptionHandler)} not found in {bodyPrefab}");

                var states = FindStatesWithMark(ac).ToArray();
                return states
                    .Where(x => x.Ref.Attributes?.Get<AttackAttributes>() != null)
                    .SelectMany(state =>
                    {
                        var result = bones.Select(bone =>
                        {
                            AnimationTrajectory trajectory = AnimationTrajectory.CreateInstance<AnimationTrajectory>();
                            trajectory.Position = Curve3.CreateInstance<Curve3>();
                            trajectory.Rotation = CurveQ.CreateInstance<CurveQ>();
                            return Tuple.Create(bone, trajectory);
                        }).ToArray();
                        GenerateTrajectories(bodyPrefab, animator, state.State, state.Layer, timeStep, result);
                        return result.Select(x => Tuple.Create(x.Item1.ReversedPathTo(bodyInstance.transform), state.Ref.Attributes, x.Item2));
                    })
                    .ToArray();
            }
            finally
            {
                GameObject.DestroyImmediate(bodyInstance);
            }
        }

        public static void GenerateTrajectory(GameObject body, AnimatorController ac, string stateName, string boneName, float timeStep, AnimationTrajectory trajectory)
        {
            var bodyInstance = GameObject.Instantiate(body, Vector3.zero, Quaternion.identity);

            try
            {
                var animator = bodyInstance.GetComponentsInChildren<Animator>().FirstOrDefault(x => x.runtimeAnimatorController == ac);
                if (!animator)
                    throw new Exception($"Animator {ac} not found in {body}");

                var boneObject = bodyInstance.transform.FindChildRecursive(boneName);
                if (!boneObject)
                    throw new Exception($"Bone with name {boneName} not found in {body}");

                var @as = FindState(ac, stateName).FirstOrDefault();
                if (!@as.State)
                    throw new Exception($"Animation state {stateName} not found in {ac}");

                var result = new[] {Tuple.Create(boneObject, trajectory)};
                GenerateTrajectories(body, animator, @as.State, @as.Layer, timeStep, result);
            }
            finally
            {
                GameObject.DestroyImmediate(bodyInstance);
            }
        }


        private static IEnumerable<StateWithMarkTuple> FindStatesWithMark(AnimatorController ac)
        {
            return ac.layers.SelectMany((x, i) => FindStatesWithMark(i, x.stateMachine)).ToArray();
        }

        private static IEnumerable<StateWithMarkTuple> FindStatesWithMark(int layer, AnimatorStateMachine asm)
        {
            return asm.states
                .Select(x => x.state).Select(x => new StateWithMarkTuple(x.behaviours.OfType<AnimationTrajectoryRef>().FirstOrDefault(), x, layer)).Where(x => x.Ref)
                .Concat(asm.stateMachines.Select(x => x.stateMachine).SelectMany(x => FindStatesWithMark(layer, x)));
        }

        private static IEnumerable<StateTuple> FindState(AnimatorController ac, string state)
        {
            if (state == null)
                return null;

            var path = state.Split('.');
            if (path.Length > 1)
            {
                var layerName = path[0];
                path = path.Skip(1).ToArray();
                return ac.layers.Select((x, i) => new {x, i}).Where(x => x.x.name == layerName).SelectMany(x => FindState(x.i, x.x.stateMachine, path)).ToArray();
            }

            return ac.layers.SelectMany((x, i) => FindState(i, x.stateMachine, state));
        }

        private static IEnumerable<StateTuple> FindState(int layer, AnimatorStateMachine asm, string[] path)
        {
            if (path.Length == 0)
                throw new ArgumentException("path is empty", nameof(path));

            var name = path[0];
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            path = path.Skip(1).ToArray();

            if (path.Length == 0)
                return asm.states.Select(x => x.state).Where(x => x && x.name == name).Select(x => new StateTuple(x, layer));

            return asm.stateMachines.Select(x => x.stateMachine).Where(x => x && x.name == name).SelectMany(x => FindState(layer, x, path));
        }

        private static IEnumerable<StateTuple> FindState(int layer, AnimatorStateMachine asm, string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            return asm.states
                .Select(x => x.state).Where(x => x && x.name == name).Select(x => new StateTuple(x, layer))
                .Concat(asm.stateMachines.Select(x => x.stateMachine).SelectMany(x => FindState(layer, x, name)));
        }

        struct StateTuple
        {
            public readonly AnimatorState State;
            public readonly int Layer;

            public StateTuple(AnimatorState state, int layer)
            {
                State = state;
                Layer = layer;
            }
        }
        
        struct StateWithMarkTuple
        {
            public readonly AnimationTrajectoryRef Ref;
            public readonly AnimatorState State;
            public readonly int Layer;

            public StateWithMarkTuple(AnimationTrajectoryRef @ref, AnimatorState state, int layer)
            {
                Ref = @ref;
                State = state;
                Layer = layer;
            }
        }
    */
    }
}