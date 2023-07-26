using Assets.ColonyShared.SharedCode.Utils;
using UnityEngine;

namespace Src.Animation
{
    public static class Curve3Methods
    {
        public static Vector3 Evaluate(this Curve3 @this, float time)
        {
            return new Vector3(@this.curveX.Evaluate(time), @this.curveY.Evaluate(time), @this.curveZ.Evaluate(time));
        }

        public static Vector3 Tangent(this Curve3 @this, float time)
        {
            return new Vector3(@this.curveX.Differentiate(time), @this.curveY.Differentiate(time), @this.curveZ.Differentiate(time));
        }
        
        public static float Distance(this Curve3 @this, float fromTime, float tillTime)
        {
            if (fromTime > tillTime)
            {
                var tmp = tillTime;
                tillTime = fromTime;
                fromTime = tmp;
            }
            return @this._lengthByTime.Evaluate(tillTime) - @this._lengthByTime.Evaluate(fromTime);
        }

        public static float Shift(this Curve3 @this, float time, float distance)
        {
            if (Mathf.Approximately(distance, 0))
                return time;
            var l1 = @this._lengthByTime.Evaluate(time);
            var l2 = l1 + distance;
            return @this._timeByLength.Evaluate(l2);
        }

        public static float DeltaTimeByDistance(this Curve3 @this, float time, float distance)
        {
            if (Mathf.Approximately(distance, 0))
                return 0;
            var l1 = @this._lengthByTime.Evaluate(time);
            var l2 = l1 + distance;
            return @this._timeByLength.Evaluate(l2) - time;
        }
 
        public static void Clear(this Curve3 @this)
        {
            @this.curveX = new AnimationCurve();
            @this.curveY = new AnimationCurve();
            @this.curveZ = new AnimationCurve();
            @this._lengthByTime = new AnimationCurve();
            @this._timeByLength = new AnimationCurve();
            @this._minTime = @this._maxTime = 0;
        }

        public static void AddKey(this Curve3 @this, float time, Vector3 point)
        {
            @this.curveX.AddKey(time, point.x);
            @this.curveY.AddKey(time, point.y);
            @this.curveZ.AddKey(time, point.z);
        }

        public static void Recalculate(this Curve3 @this, float timeStep)
        {
            @this._lengthByTime = new AnimationCurve();
            @this._timeByLength = new AnimationCurve();
            
            @this._minTime = Mathf.Min(@this.curveX[0].time, @this.curveY[0].time, @this.curveZ[0].time);
            @this._maxTime = Mathf.Min(@this.curveX[@this.curveX.length - 1].time, @this.curveY[@this.curveY.length - 1].time, @this.curveZ[@this.curveZ.length - 1].time);

            float length = 0;
            float time = @this._minTime;
            var prevPoint = @this.Evaluate(time);
            @this._lengthByTime.AddKey(time, length);
            @this._timeByLength.AddKey(length, time);

            while (time < @this._maxTime)
            {
                time += timeStep;
                var point = @this.Evaluate(Mathf.Min(time, @this._maxTime));
                var delta = (point - prevPoint).magnitude;
                length += delta;
                @this._lengthByTime.AddKey(time, length);
                @this._timeByLength.AddKey(length, time);
                prevPoint = point;
            }

            for (int i = 0; i < @this._lengthByTime.length; i++)
                @this._lengthByTime.SmoothTangents(i, 1);
            for (int i = 0; i < @this._timeByLength.length; i++)
                @this._timeByLength.SmoothTangents(i, 1);
        }
        
        public static bool EqualsByContent(this Curve3 @this, Curve3 that, float tolerance)
        {
            return ReferenceEquals(@this, that) ||
                   AnimationCurveUtils.EqualsByContent(@this.curveX, that.curveX, tolerance) &&
                   AnimationCurveUtils.EqualsByContent(@this.curveY, that.curveY, tolerance) &&
                   AnimationCurveUtils.EqualsByContent(@this.curveZ, that.curveZ, tolerance);
        }

        public static bool Optimize(this Curve3 @this, float tolerance)
        {
            bool rv = false;
            rv |= ReplaceByOptimized(@this, ref @this.curveX, AnimationCurveUtils.SimpleOptimization(@this.curveX, tolerance));
            rv |= ReplaceByOptimized(@this, ref @this.curveY, AnimationCurveUtils.SimpleOptimization(@this.curveY, tolerance));
            rv |= ReplaceByOptimized(@this, ref @this.curveZ, AnimationCurveUtils.SimpleOptimization(@this.curveZ, tolerance));
            if (@this._timeByLength != null)
                rv |= ReplaceByOptimized(@this, ref @this._timeByLength, AnimationCurveUtils.SimpleOptimization(@this._timeByLength, tolerance));
            if(@this._lengthByTime != null)
                rv |= ReplaceByOptimized(@this, ref @this._lengthByTime, AnimationCurveUtils.SimpleOptimization(@this._lengthByTime, tolerance));
            return rv;
        }

        private static bool ReplaceByOptimized(Curve3 @this, ref AnimationCurve curve, AnimationCurve optimized)
        {
            if (!ReferenceEquals(optimized, curve))
            {
//                Debug.Log($"{@this.name}: curve optimization: {curve.length - optimized.length} keys");
                curve = optimized;
                return true;
            }
            return false;
        }
    }
}