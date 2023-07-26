using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Editor.Tools.SplineFlatteners
{
    public struct CubicBezierSection
    {
        public Vector3 start;
        public Vector3 controlPoint1;
        public Vector3 controlPoint2;
        public Vector3 end;

        public CubicBezierSection(Vector3 start, Vector3 end, Vector3 controlPointNearestToStart, Vector3 controlPointNearestToEnd)
        {
            this.start = start;
            this.end = end;
            this.controlPoint1 = controlPointNearestToStart;
            this.controlPoint2 = controlPointNearestToEnd;
        }

        public Vector3 Position(float t)
        {
            var tSquared = t * t;
            var tCubed = tSquared * t;
            var oneMinusT = 1 - t;
            var oneMinusTSquared = oneMinusT * oneMinusT;
            var oneMinusTCubed = oneMinusTSquared * oneMinusT;
            return start * oneMinusTCubed
                 + controlPoint1 * 3f * oneMinusTSquared * t
                 + controlPoint2 * 3f * oneMinusT * tSquared
                 + end * tCubed;
        }

        internal (CubicBezierSection left, CubicBezierSection right) Split(float t)
        {
            var t1 = Vector3.Lerp(start, controlPoint1, t);
            var t2 = Vector3.Lerp(controlPoint1, controlPoint2, t);
            var t3 = Vector3.Lerp(controlPoint2, end, t);
            var t12 = Vector3.Lerp(t1, t2, t);
            var t23 = Vector3.Lerp(t2, t3, t);
            var t123 = Vector3.Lerp(t12, t23, t);
            return (new CubicBezierSection(start, t123, t1, t12), new CubicBezierSection(t123, end, t23, t3));
        }
    }

    public class CubicBezierSegmentFlattener
    // this class implements parabolic approximation algorhytm (PAA) introduced in the paper "Precise Flattening of Cubic Bezier Segments" (Thomas F. Hain, Athar L. Ahmad, David D. Langan), 2005
    // note that algorhythm below is expanded on 3D but internally uses the same 2D math thus ignoring one dimension (all calculations is done on (P1, P2, P3) plane) that naturally leads to a higher error (that exceeds 'tolerance' parameter) when P1, P2, P3, P4 are not coplanar
    {
        // this function works only with Bezier curve sections without inflection points
        public static IEnumerable<(Vector3 lineStart, float startT, Vector3 lineEnd, float endT)> FlattenSectionWithoutInflections(CubicBezierSection bezierSection, float tolerance)
        {
            float t = 0; // t value along whole spline section
            var currentBezierSection = bezierSection;
            while (true)
            {
                var currentSectionT = FlattenSectionWithoutInflectionsStep(currentBezierSection, tolerance); // t value relative to current (remaining) sub-section of input section
                if (currentSectionT >= 1)
                    break;
                var wholeSegmentCurrentT = t + currentSectionT * (1 - t);
                (_, var nextBezierSection) = currentBezierSection.Split(currentSectionT);
                yield return (currentBezierSection.start, t, nextBezierSection.start, wholeSegmentCurrentT);
                t = wholeSegmentCurrentT;
                currentBezierSection = nextBezierSection;
            }
            yield return (currentBezierSection.start, t, bezierSection.end, 1);
        }

        // code from Rust crate 'lyon_geom' was used as reference
        protected static float FlattenSectionWithoutInflectionsStep(CubicBezierSection bezierSection, float tolerance)
        {
            var vector1 = bezierSection.controlPoint1 - bezierSection.start;
            var vector2 = bezierSection.controlPoint2 - bezierSection.start;
            var vector2Xvector1 = Vector3.Cross(vector2, vector1).sqrMagnitude;
            if (Mathf.Approximately(vector2Xvector1, 0f))
                return 1f;
            var inverseS = (float)Math.Sqrt(Vector3.Dot(vector1, vector1)) / vector2Xvector1;
            var t = 2f * Mathf.Sqrt(tolerance * Mathf.Abs(inverseS) / 3f);

            if (t >= 0.995f || Mathf.Approximately(t, 0f))
                return 1f;

            return t;
        }
    }
}
