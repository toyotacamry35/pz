using System.Collections.Generic;
using UnityEngine;

namespace Assets.Editor.Tools.SplineFlatteners
{
    public class CatmullRomSplineFlattener
    {
        public static IEnumerable<ICollection<LineSectionWithWidth>> FlattenSpline(List<CatmullRomSection> catmullRomSpline, float tolerance)
        {
            var flattener = new CatmullRomSectionFlattener(tolerance);
            foreach (var section in catmullRomSpline)
            {
                var list = new List<LineSectionWithWidth>();
                var flattenedSection = flattener.FlattenSection(section);
                foreach (var line in flattenedSection)
                {
                    var width = Mathf.Abs((line.endWidth + line.startWidth) * 0.5f);
                    list.Add(new LineSectionWithWidth(line.startPosition, line.endPosition, width));
                }
                yield return list;
            }
        }
    }

    public class CatmullRomSectionFlattener
    {
        private float tolerance;
        public CatmullRomSectionFlattener(float tolerance) => this.tolerance = tolerance;
        public IEnumerable<(Vector3 startPosition, float startWidth, Vector3 endPosition, float endWidth)> FlattenSection(CatmullRomSection catmullRomSection)
        {
            // first we should convert Catmull-Rom to Bezier
            // CatmullRom to Bezier conversion can be represented as following:
            //  ⌈ P1 ⌉               ⌈ P2                      ⌉
            //  | P2 |               | P2 + (P3 - P1) / 6 * τ  |
            //  | P3 |       =>      | P3 - (P4 - P2) / 6 * τ  |
            //  ⌊ P4 ⌋               ⌊ P3                      ⌋
            // (Catmull-Rom)                 (Bezier)
            // where Pn, n = 1,4 is control points, τ is tension parameter
            // Catmull-Rom matrix representatinon:
            // [ 1      t       t^2     t^3 ] * 0.5 * ⌈ 0       2       0       0 ⌉ * ⌈ P1 ⌉
            //                                        | -τ      0       τ       0 |   | P2 |
            //                                        | 2τ      τ-6     -2(τ-3) -τ|   | P3 |
            //                                        ⌊ -τ      4-τ     τ-4     τ ⌋   ⌊ P4 ⌋
            // RAM River uses following expression to determine point position of point on spline (see function named 'GetCatmullRomPosition"):
            // position = 0.5 * (2 * P2 + t * (P3 - P1) + t^2 * (2 * P1 - 5 * P2 + 4 * P3 - P4) + t^3 * (-P1 + 3 * P2 - 3 * P3 + P4)) =>
            //  => Ram River uses baked τ = 1
            var bezierSection = new CubicBezierSection(catmullRomSection.point2,
                catmullRomSection.point3,
                catmullRomSection.point2 + (catmullRomSection.point3 - catmullRomSection.point1) / 6f,
                catmullRomSection.point3 - (catmullRomSection.point4 - catmullRomSection.point2) / 6f);
            var widthDifference = catmullRomSection.endWidth - catmullRomSection.startWidth;
            foreach (var item in CubicBezierSegmentFlattener.FlattenSectionWithoutInflections(bezierSection, tolerance))
            {
                yield return (item.lineStart, catmullRomSection.startWidth + widthDifference * item.startT, item.lineEnd, catmullRomSection.endWidth - widthDifference * item.endT);
            }
        }
    }

    public struct CatmullRomSection
    {
        public Vector3 point1;
        public Vector3 point2;
        public Vector3 point3;
        public Vector3 point4;
        public float startWidth;
        public float endWidth;

        public CatmullRomSection(Vector3 point1, Vector3 point2, Vector3 point3, Vector3 point4, float startWidth, float endWidth)
        {
            this.point1 = point1;
            this.point2 = point2;
            this.point3 = point3;
            this.point4 = point4;
            this.startWidth = startWidth;
            this.endWidth = endWidth;
        }
    }

    public struct LineSectionWithWidth
    {
        public Vector3 start;
        public Vector3 end;
        public float width;

        public LineSectionWithWidth(Vector3 start, Vector3 end, float width)
        {
            this.start = start;
            this.end = end;
            this.width = width;
        }
    }
}
