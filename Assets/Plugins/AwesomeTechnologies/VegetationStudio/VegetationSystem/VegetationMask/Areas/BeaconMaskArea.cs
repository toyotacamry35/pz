using UnityEngine;

namespace AwesomeTechnologies
{
    public class BeaconMaskArea : BaseMaskArea
    {
        public float Radius;
        public Vector3 Position;
        public float[] FalloffCurveArray = new float[0];

        public void Init()
        {
            MaskBounds = GetMaskBounds();
        }

        public override bool ContainsMask(Vector3 point, VegetationType vegetationType, VegetationTypeIndex vegetationTypeIndex, ref float size, ref float density)
        {
            bool hasVegetationType = HasVegetationType(vegetationTypeIndex, ref size, ref density);

            if (!hasVegetationType) return false;

            //float distance = Vector3.Distance(point, Position);
            float distance = Vector2.Distance(new Vector2(point.x,point.z), new Vector2(Position.x,Position.z));
            if (distance < Radius)
            {
                float floatIndex = (distance / Radius);
                int index = Mathf.FloorToInt(floatIndex * FalloffCurveArray.Length);
                index = Mathf.Clamp(index, 0, FalloffCurveArray.Length - 1);

                float spawnChance = FalloffCurveArray[index];
                density *= spawnChance;
                return true;
            }
            return false;
        }

        private Bounds GetMaskBounds()
        {
            return new Bounds(Position, new Vector3(Radius * 2, Radius * 2, Radius * 2)); 
        }
    }
}
