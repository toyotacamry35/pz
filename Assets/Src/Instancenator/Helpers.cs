using UnityEngine;


namespace Assets.Instancenator
{
    public class Helpers
    {
        const float eps = 1e-5f;


        public static Vector3 ConvertVertexToBlock(Vector3 localPositionInBounds, InstanceComposition.Block block)
        {
            Vector3 pos = localPositionInBounds - block.bounds.min;
            pos.x = block.bounds.size.x > eps ? pos.x / block.bounds.size.x : 0.0f;
            pos.y = block.bounds.size.y > eps ? pos.y / block.bounds.size.y : 0.0f;
            pos.z = block.bounds.size.z > eps ? pos.z / block.bounds.size.z : 0.0f;
            return pos;
        }

        public static Vector4 ConvertValueToBlock(Vector4 value, InstanceComposition.Block block)
        {
            Vector4 valueDelta = block.valueMax - block.valueMin;
            Vector4 valueRel = new Vector4();
            valueRel.x = valueDelta.x > eps ? (valueDelta.x - block.valueMin.x) / valueDelta.x : 0.0f;
            valueRel.y = valueDelta.y > eps ? (valueDelta.y - block.valueMin.y) / valueDelta.y : 0.0f;
            valueRel.z = valueDelta.z > eps ? (valueDelta.z - block.valueMin.z) / valueDelta.z : 0.0f;
            valueRel.w = valueDelta.w > eps ? (valueDelta.w - block.valueMin.w) / valueDelta.w : 0.0f;
            return valueRel;
        }

        public static bool IsAxisAligned(Transform transform)
        {
            Vector3 testPoint = new Vector3(1, 2, 3);
            Vector3 transformedPoint = transform.TransformVector(testPoint);
            return transformedPoint != testPoint;
        }

        public static Bounds CalcOrientedBoxBounds(Matrix4x4 localToWorldMatrix, Bounds bounds)
        {
            Vector3 center = bounds.center;
            Vector3 extents = bounds.extents;

            Vector3[] points = new Vector3[8];
            points[0] = center + new Vector3(extents.x, extents.y, extents.z);
            points[1] = center + new Vector3(extents.x, extents.y, -extents.z);
            points[2] = center + new Vector3(extents.x, -extents.y, extents.z);
            points[3] = center + new Vector3(extents.x, -extents.y, -extents.z);
            points[4] = center + new Vector3(-extents.x, extents.y, extents.z);
            points[5] = center + new Vector3(-extents.x, extents.y, -extents.z);
            points[6] = center + new Vector3(-extents.x, -extents.y, extents.z);
            points[7] = center + new Vector3(-extents.x, -extents.y, -extents.z);

            for (int i = 0; i < 8; i++)
            {
                points[i] = localToWorldMatrix.MultiplyPoint(points[i]);
            }

            Bounds newBounds = new Bounds(points[0], Vector3.zero);
            for (int i = 1; i < 8; i++)
            {
                newBounds.Encapsulate(points[i]);
            }

            return newBounds;
        }

    }
}
