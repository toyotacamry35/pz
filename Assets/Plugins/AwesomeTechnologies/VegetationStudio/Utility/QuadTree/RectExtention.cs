using UnityEngine;

namespace AwesomeTechnologies.Utility.Quadtree
{
    public static class RectExtension
    {
        public static bool Contains(this Rect self, Rect rect)
        {
            return self.Contains(new UnityEngine.Vector2(rect.xMin, rect.yMin)) && self.Contains(new UnityEngine.Vector2(rect.xMax, rect.yMax));
        }

        public static void FromBounds(this Rect self, Bounds bounds)
        {
            self.xMin = bounds.center.x - bounds.extents.x;
            self.yMin = bounds.center.z - bounds.extents.z;
            self.width = bounds.size.x;
            self.height = bounds.size.z;
        }

        public static Rect CreateRectFromBounds(Bounds bounds)
        {
            return new Rect(bounds.center.x - bounds.extents.x, bounds.center.z - bounds.extents.z, bounds.size.x, bounds.size.z);
        }
    }
}
