using UnityEngine;

namespace AwesomeTechnologies
{
    public class CircleMaskArea : BaseMaskArea
    {
        public float Radius = 0.1f;
        public Vector3 Position;
        public VegetationType VegetationType;

        public void Init()
        {
            MaskBounds = GetMaskBounds();
        }

        public override bool Contains(Vector3 point, VegetationType vegetationType, bool useAdditionalDistance, bool useExcludeFilter)
        {
            if (VegetationType != vegetationType) return false;
            float distance = Vector2.Distance(new Vector2(point.x, point.z), new Vector2(Position.x,Position.z));
            return distance < Radius;
        }

        private Bounds GetMaskBounds()
        {
            return new Bounds(Position, new Vector3(Radius * 2, Radius * 2, Radius * 2));
        }
    }
}