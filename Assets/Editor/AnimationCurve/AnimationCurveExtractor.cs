using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Src.Animation;
using UnityEditor;
using UnityEngine;

namespace Assets.Src.Editor
{
    public static class AnimationCurveExtractor
    {
        private enum CurveType
        {
            Common,
            Velocity
        }

        private static readonly string[] Axes = {"x", "y", "z", "w"};

        public static AnimationCurve ExtractCurve([NotNull] AnimationClip clip, [NotNull] string name, float velocityTimeStep = 0.01f)
        {
            if (clip == null) throw new ArgumentNullException(nameof(clip));
            if (name == null) throw new ArgumentNullException(nameof(name));
            var curveBindings = AnimationUtility.GetCurveBindings(clip);
            var binding = ResolveCurve(curveBindings, name);
            var curve = AnimationUtility.GetEditorCurve(clip, binding.Item2);
            switch (binding.Item1)
            {
                case CurveType.Common:
                    return AnimationCurveUtils.CreateCopy(curve);
                case CurveType.Velocity:
                    return AnimationCurveUtils.CreateDerivative(curve, velocityTimeStep);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        public static Tuple<AnimationCurve,AnimationCurve,AnimationCurve> ExtractCurve3([NotNull] AnimationClip clip, [NotNull] string name)
        {
            if (clip == null) throw new ArgumentNullException(nameof(clip));
            if (name == null) throw new ArgumentNullException(nameof(name));
            var curveBindings = AnimationUtility.GetCurveBindings(clip);
            var binding = ResolveCurve3(curveBindings, name);
            var curveX = AnimationUtility.GetEditorCurve(clip, binding.Item2);
            var curveY = AnimationUtility.GetEditorCurve(clip, binding.Item3);
            var curveZ = AnimationUtility.GetEditorCurve(clip, binding.Item4);
            switch (binding.Item1)
            {
                case CurveType.Common:
                    return Tuple.Create(
                        AnimationCurveUtils.CreateCopy(curveX),
                        AnimationCurveUtils.CreateCopy(curveY),
                        AnimationCurveUtils.CreateCopy(curveZ)
                        );
                case CurveType.Velocity:
                    return Tuple.Create(
                        AnimationCurveUtils.CreateDerivative(curveX),
                        AnimationCurveUtils.CreateDerivative(curveY),
                        AnimationCurveUtils.CreateDerivative(curveZ)
                    );
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static int GetCurvesCount([NotNull] AnimationClip clip)
        {
            if (clip == null) throw new ArgumentNullException(nameof(clip));
            return AnimationUtility.GetCurveBindings(clip).SelectMany(GitBindingAllNames).Count();
        }

        public static IEnumerable<string> GetCurvesNames([NotNull] AnimationClip clip)
        {
            if (clip == null) throw new ArgumentNullException(nameof(clip));
            return AnimationUtility.GetCurveBindings(clip)
                .SelectMany(GitBindingAllNames)
                .Select(y => y.Item2);
        }

        public static IEnumerable<string> GetCurves3Names([NotNull] AnimationClip clip)
        {
            if (clip == null) throw new ArgumentNullException(nameof(clip));
            return AnimationUtility.GetCurveBindings(clip)
                .SelectMany(GitBindingAllNames)
                .GroupBy(x => NameWithoutAxis(x.Item2).Item1)
                .Where(x => !string.IsNullOrEmpty(x.Key) && x.Count() == 3)
                .Select(x => x.Key);
        }

        private static ValueTuple<CurveType, EditorCurveBinding> ResolveCurve(EditorCurveBinding[] bindings, string name)
        {
            foreach (var binding in bindings)
            foreach (var tuple in GitBindingAllNames(binding))
                if (tuple.Item2 == name)
                    return ValueTuple.Create(tuple.Item1, binding);
            throw new Exception($"Can't resolve animation curve '{name}'");
        }
        
        private static ValueTuple<CurveType, EditorCurveBinding, EditorCurveBinding, EditorCurveBinding> ResolveCurve3(EditorCurveBinding[] bindings, string name)
        {
            string nameX = name + ".x", nameY = name + ".y", nameZ = name + ".z";
            Tuple<CurveType,EditorCurveBinding> x = null, y = null, z = null;
            foreach (var binding in bindings)
            foreach (var tuple in GitBindingAllNames(binding))
            {
                if (tuple.Item2 == nameX)
                    x = Tuple.Create(tuple.Item1, binding);
                else
                if (tuple.Item2 == nameY)
                    y = Tuple.Create(tuple.Item1, binding);
                else
                if (tuple.Item2 == nameZ)
                    z = Tuple.Create(tuple.Item1, binding);
            }
            if (x != null && y != null && z != null)
            {
                if(x.Item1 != y.Item1 || x.Item1 != z.Item1)
                    throw new Exception($"Animation curve type mixed for '{name}'");
                return ValueTuple.Create(x.Item1, x.Item2, y.Item2, z.Item2);
            }
            throw new Exception($"Can't resolve animation curve '{name}'");
        }
        
        private static IEnumerable<Tuple<CurveType, string>> GitBindingAllNames(EditorCurveBinding binding)
        {
            var fullName = GitBindingFullName(binding);
            yield return Tuple.Create(CurveType.Common, fullName);
            var (fullNameWithoutAxis, axis) = NameWithoutAxis(binding.propertyName);
            if (axis != null)
            {
                if (fullNameWithoutAxis == "MotionT")
                    yield return Tuple.Create(CurveType.Velocity, "Velocity." + axis);
                else if (!fullNameWithoutAxis.EndsWith("Q"))
                    yield return Tuple.Create(CurveType.Velocity, fullNameWithoutAxis + ".Velocity." + axis);
            }
        }

        private static string GitBindingFullName(EditorCurveBinding binding)
        {
            return string.IsNullOrEmpty(binding.path) ? binding.propertyName : $"{binding.path}/{binding.propertyName}";
        }

        private static (string, string) NameWithoutAxis(string name)
        {
            if (!string.IsNullOrEmpty(name))
                foreach (var axis in Axes)
                    if (name.EndsWith("." + axis, StringComparison.OrdinalIgnoreCase))
                        return (name.Substring(0, name.Length - 2), axis);
            return (name, null);
        }
    }
}