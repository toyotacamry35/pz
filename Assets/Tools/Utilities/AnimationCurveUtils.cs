using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Src.Animation
{
    public static class AnimationCurveUtils
    {
        public static void Copy(AnimationCurve inSource, AnimationCurve inDest)
        {
            inDest.keys = inSource.keys;
            inDest.preWrapMode = inSource.preWrapMode;
            inDest.postWrapMode = inSource.postWrapMode;
        }

        public static AnimationCurve CreateCopy(AnimationCurve inSource)
        {
            var newCurve = new AnimationCurve();
            Copy(inSource, newCurve);
            return newCurve;
        }
        
        public static AnimationCurve CreateDerivative(AnimationCurve motion, float timeStep = 0.01f)
        {
            timeStep = Mathf.Max(timeStep, 0.001f);
           // AnimationCurve curve = new AnimationCurve();
            var minTime = motion[0].time;
            var maxTime = motion[motion.length - 1].time;
            var count = Mathf.CeilToInt((maxTime - minTime) / timeStep);
            var time = minTime;
            var frames = new Keyframe[count];
            float prevValue = 0;
            for (int i = 0; i <= count; i++)
            {
                time = Mathf.Clamp(time + timeStep, minTime, maxTime);
                var value = motion.Evaluate(time);
                if (i > 0)
                {
                    //curve.AddKey(time - timeStep, (value - prevValue.Value) / timeStep);
                    frames[i - 1] = new Keyframe(time - timeStep, (value - prevValue) / timeStep);
                }
                prevValue = value;
            }
            var curve = new AnimationCurve(frames);
            return curve;
        }
        
        public static float Differentiate(this AnimationCurve curve, float x, float delta = 0.00001f)
        {
            var x1 = x - delta;
            var x2 = x + delta;
            var y1 = curve.Evaluate(x1);
            var y2 = curve.Evaluate(x2);
            return (y2 - y1) / (x2 - x1);
        }
        
        public static bool EqualsByContent(AnimationCurve a, AnimationCurve b, float tolerance)
        {
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;
            if (ReferenceEquals(a, b))
                return true;
            if (a.length != b.length)
                return false;
            for (int i = 0; i < a.length; ++i)
                if (Mathf.Abs(a[i].time - b[i].time) > tolerance || Mathf.Abs(a[i].value - b[i].value) > tolerance)
                    return false;
            return true;
        }
        
        public static AnimationCurve SimpleOptimization(AnimationCurve curve, float tolerance)
        {
            if (curve.length < 2)
                return curve;
            var selectedKeys = new List<int>(curve.length);
            selectedKeys.Add(0);
            int skip = 0;
            for (int  prevIdx = 0, idx = 1, nextIdx = 2; nextIdx < curve.length; )
            {
                var prev = curve[prevIdx];
                var next = curve[nextIdx];
                if (CheckEval(curve, prev, next, 5 + skip * 2, tolerance))
                {
                    ++skip;
                    ++idx;
                    nextIdx = idx + 1;
                }
                else
                {
                    selectedKeys.Add(idx);
                    prevIdx = idx;
                    idx = prevIdx + 1;
                    nextIdx = idx + 1;
                    skip = 0;
                }
            }
            selectedKeys.Add(curve.length - 1);
            if (selectedKeys.Count != curve.length)
                return new AnimationCurve(selectedKeys.Select(x => curve[x]).ToArray());
            return curve;
        }

        public static AnimationCurve Scale(AnimationCurve curve, float scale)
        {
            var frames = curve.keys.ToArray();
            for (int i = 0; i < frames.Length; i++)
            {
                frames[i].value *= scale;
                frames[i].inTangent *= scale;
                frames[i].outTangent *= scale;
            }
            return new AnimationCurve(frames);
        }

        private static bool CheckEval(AnimationCurve curve, Keyframe prev, Keyframe next, int steps, float tolerance)
        {
            var tmp = new AnimationCurve(prev, next);
            for (int s = 1; s < steps - 1; ++s)
            {
                var t = Mathf.Lerp(prev.time, next.time, s / (float)steps);
                if (Mathf.Abs(tmp.Evaluate(t) - curve.Evaluate(t)) > tolerance)
                    return false;
            }
            return true;
        }
        
        public static AnimationCurve Normalize(AnimationCurve curve, float minRange = 0.001f)
        {
            var frames = curve.keys.ToArray();
            float min = 0, max = 0;
            for (int i = 0; i < frames.Length; i++)
            {
                if (frames[i].value < min || i == 0)
                    min = frames[i].value;
                if (frames[i].value > max || i == 0)
                    max = frames[i].value;
            }

            if (max - min < minRange)
            {
                min = (min + max - minRange) * 0.5f;
                max = (min + max + minRange) * 0.5f;
            }

            var scale = 1f / (max - min); 
            for (int i = 0; i < frames.Length; i++)
            {
                frames[i].value = (frames[i].value - min) * scale;
                frames[i].inTangent *= scale;
                frames[i].outTangent *= scale;
            }
            return new AnimationCurve(frames);
        }
    }
}