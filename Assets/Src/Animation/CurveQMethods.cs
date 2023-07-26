using Assets.ColonyShared.SharedCode.Utils;
using UnityEngine;

namespace Src.Animation
{
    public static class CurveQMethods
    {   
        public static Quaternion Evaluate(this CurveQ @this, float time)
        {
            return new Quaternion(@this.curveX.Evaluate(time), @this.curveY.Evaluate(time), @this.curveZ.Evaluate(time), @this.curveW.Evaluate(time));
        }

        public static void Clear(this CurveQ @this)
        {
            @this.curveX = new AnimationCurve();
            @this.curveY = new AnimationCurve();
            @this.curveZ = new AnimationCurve();
            @this.curveW = new AnimationCurve();
        }

        public static void AddKey(this CurveQ @this, float time, Quaternion qaut)
        {
            @this.curveX.AddKey(time, qaut.x);
            @this.curveY.AddKey(time, qaut.y);
            @this.curveZ.AddKey(time, qaut.z);
            @this.curveW.AddKey(time, qaut.w);
        }
        public static bool EqualsByContent(this CurveQ @this, CurveQ that, float tolerance)
        {
            return ReferenceEquals(@this, that) ||
                   AnimationCurveUtils.EqualsByContent(@this.curveX, that.curveX, tolerance) &&
                   AnimationCurveUtils.EqualsByContent(@this.curveY, that.curveY, tolerance) &&
                   AnimationCurveUtils.EqualsByContent(@this.curveZ, that.curveZ, tolerance) &&
                   AnimationCurveUtils.EqualsByContent(@this.curveW, that.curveW, tolerance);
        }
        
        public static bool Optimize(this CurveQ @this, float tolerance)
        {
            bool rv = false;
            rv |= ReplaceByOptimized(@this, ref @this.curveX, AnimationCurveUtils.SimpleOptimization(@this.curveX, tolerance));
            rv |= ReplaceByOptimized(@this, ref @this.curveY, AnimationCurveUtils.SimpleOptimization(@this.curveY, tolerance));
            rv |= ReplaceByOptimized(@this, ref @this.curveZ, AnimationCurveUtils.SimpleOptimization(@this.curveZ, tolerance));
            rv |= ReplaceByOptimized(@this, ref @this.curveW, AnimationCurveUtils.SimpleOptimization(@this.curveW, tolerance));
            return rv;
        }

        private static bool ReplaceByOptimized(CurveQ @this, ref AnimationCurve curve, AnimationCurve optimized)
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