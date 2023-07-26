using Assets.ResourceSystem.Aspects.ManualDefsForSpells;
using SharedCode.Entities.GameObjectEntities;
using System;
using SharedCode.Utils;
using Quaternion = SharedCode.Utils.Quaternion;
using Transform = SharedCode.Entities.Transform;
using Vector3 = SharedCode.Utils.Vector3;

namespace Assets.ColonyShared.GeneratedCode.Regions
{
	public static class GeometryHelpers
	{
		public static bool IsPointInsideBox(Vector3 pointCoords, Vector3 boxCenter, Vector3 boxExtents, Quaternion boxInverseRotation)
		{
			var delta = pointCoords - boxCenter;
			var deltaInBoxSpace = boxInverseRotation * delta;
			if (deltaInBoxSpace.x < -boxExtents.x || deltaInBoxSpace.x > boxExtents.x ||
			    deltaInBoxSpace.y < -boxExtents.y || deltaInBoxSpace.y > boxExtents.y ||
			    deltaInBoxSpace.z < -boxExtents.z || deltaInBoxSpace.z > boxExtents.z)
				return false;
			return true;
		}

		internal static bool IsPointInsideSphere(Vector3 pointCoords, Vector3 sphereCenter, float sphereRadius)
		{
			return (pointCoords - sphereCenter).SqrMagnitude < sphereRadius * sphereRadius;
		}

		public static BoundingBox GetBoundingBoxOfSphereWithTransform(Vector3 center, float radius, Transform transform)
		{
			var sphereDimension = 2 * radius;
			var sphereDimensions = new Vector3(sphereDimension, sphereDimension, sphereDimension);
			var sphereStartCoords = (transform.Position + center) - new Vector3(radius, radius, radius);
			return new BoundingBox {StartCoords = sphereStartCoords, Dimensions = sphereDimensions};
		}

		public static BoundingBox GetBoundingBoxOfBoxWithTransform(Vector3 size, Transform transform)
		{
			var extent = size * 0.5f;
			Vector3 lbb = transform.TransformPoint(-extent); // y ^lub
			Vector3 rbb = transform.TransformPoint(new Vector3(extent.x, -extent.y, -extent.z)); //   |  x 
			//   |  /lbf
			Vector3 rbf = transform.TransformPoint(new Vector3(extent.x, -extent.y, extent.z)); //   | /
			Vector3 lbf = transform.TransformPoint(new Vector3(-extent.x, -extent.y, extent.z)); //   |/________>  rbb
			//  lbb        z
			Vector3 lub = transform.TransformPoint(new Vector3(-extent.x, extent.y, -extent.z)); //
			Vector3 rub = transform.TransformPoint(new Vector3(extent.x, extent.y, -extent.z)); //    Negative / Positive axis direction
			// X    l__         r__
			Vector3 ruf = transform.TransformPoint(extent); // Y    _b_         _u_
			Vector3 luf = transform.TransformPoint(new Vector3(-extent.x, extent.y, extent.z)); // Z    __b         __f

			Vector3 min = new Vector3(
				Math.Min(Math.Min(Math.Min(Math.Min(Math.Min(Math.Min(Math.Min(ruf.x, luf.x), rub.x), lub.x), lbf.x), rbf.x), rbb.x),
					lbb.x),
				Math.Min(Math.Min(Math.Min(Math.Min(Math.Min(Math.Min(Math.Min(ruf.y, luf.y), rub.y), lub.y), lbf.y), rbf.y), rbb.y),
					lbb.y),
				Math.Min(Math.Min(Math.Min(Math.Min(Math.Min(Math.Min(Math.Min(ruf.z, luf.z), rub.z), lub.z), lbf.z), rbf.z), rbb.z),
					lbb.z));

			Vector3 max = new Vector3(
				Math.Max(Math.Max(Math.Max(Math.Max(Math.Max(Math.Max(Math.Max(ruf.x, luf.x), rub.x), lub.x), lbf.x), rbf.x), rbb.x),
					lbb.x),
				Math.Max(Math.Max(Math.Max(Math.Max(Math.Max(Math.Max(Math.Max(ruf.y, luf.y), rub.y), lub.y), lbf.y), rbf.y), rbb.y),
					lbb.y),
				Math.Max(Math.Max(Math.Max(Math.Max(Math.Max(Math.Max(Math.Max(ruf.z, luf.z), rub.z), lub.z), lbf.z), rbf.z), rbb.z),
					lbb.z));
			Vector3 boundsSize = max - min;

			return new BoundingBox {StartCoords = (Vector3) min, Dimensions = (Vector3) boundsSize};
		}

		public static bool PointToRectPos(
			Vector3 coords, Vector3 center, Vector3 extents, Quaternion inverseRotation,
			int rectWidth, int rectHeight, out int rectX, out int rectY)
		{
			var delta = coords - center;
			var deltaInBoxSpace = inverseRotation * delta;

			if (
				deltaInBoxSpace.x < -extents.x || deltaInBoxSpace.x > extents.x ||
				deltaInBoxSpace.y < -extents.y || deltaInBoxSpace.y > extents.y ||
				deltaInBoxSpace.z < -extents.z || deltaInBoxSpace.z > extents.z)
			{
				rectX = 0;
				rectY = 0;
				return false;
			}

			var normalizedX = deltaInBoxSpace.x / (2 * extents.x) + 0.5f;
			var normalizedY = deltaInBoxSpace.z / (2 * extents.z) + 0.5f;

			rectX = (int) Math.Floor(normalizedX * rectWidth);
			rectY = (int) Math.Floor(normalizedY * rectHeight);
			return true;
		}

		public static bool RectPosToPoint(out Vector3 coords, Vector3 center, Vector3 extents, Quaternion inverseRotation, int rectWidth,
			int rectHeight, int rectX, int rectY)
		{
			if (rectX >= rectWidth || rectY >= rectHeight)
			{
				coords = default;
				return false;
			}

			var normalizedX = (rectX + 0.5f) / rectWidth;
			var normalizedY = (rectY + 0.5f) / rectHeight;
			var deltaInBoxSpace = Vector3.zero;
			deltaInBoxSpace.x = 2 * (normalizedX - 0.5f) * extents.x;
			deltaInBoxSpace.z = 2 * (normalizedY - 0.5f) * extents.z;

			if (
				deltaInBoxSpace.x < -extents.x || deltaInBoxSpace.x > extents.x ||
				deltaInBoxSpace.y < -extents.y || deltaInBoxSpace.y > extents.y ||
				deltaInBoxSpace.z < -extents.z || deltaInBoxSpace.z > extents.z)
			{
				coords = default;
				return false;
			}

			var delta = Quaternion.Inverse(inverseRotation) * deltaInBoxSpace;

			coords = center + delta;
			coords.y = center.y + extents.y;
			return true;
		}

        public static bool CheckIntersectOBBandAABB(Vector3 boxCenter, Vector3 boxExtents, Quaternion boxRotation, Vector3 box2Center, Vector3 box2Extents, Quaternion box2Rotation)
        {
            //TODO написать реализации, пока в качестве заглушки возвращается находится ли центр AABB в OBB
			var result = IsPointInsideBox(box2Center, boxCenter, boxExtents, boxRotation);

#if UNITY_EDITOR
			{
				//WEAPON
				BoxShapeDef weaponDef = new BoxShapeDef();
				weaponDef.Position = boxCenter;
				weaponDef.Extents = boxExtents;
				weaponDef.Rotation = boxRotation.eulerAngles;
				EntitytObjectsUnitySpawnService.SpawnService.DrawShape(weaponDef, Color.red, 0.1f);

                //TARGET
				BoxShapeDef targetDef = new BoxShapeDef();
				targetDef.Position = box2Center;
				targetDef.Extents = box2Extents;
				targetDef.Rotation = box2Rotation.eulerAngles;
				EntitytObjectsUnitySpawnService.SpawnService.DrawShape(targetDef, result ? Color.blue : Color.green, 0.1f);
			}
#endif


			return result;
        }

        //public static bool ShapesCollided(BoxShapeDef box, SphereShapeDef sphere)
		public static bool CheckIntersectOBBandSphere(Vector3 boxCenter, Vector3 boxExtents, Quaternion boxRotation, Vector3 sphereCenter, float sphereRadius)
        {
            var result = true;
			// transfer sphere to box CS
			var boxInverseTransform = Quaternion.Inverse(boxRotation);
			var delta = sphereCenter - boxCenter;
			var spherePositionInBoxCS = boxInverseTransform * delta;

			//and check collision with AABB box
			bool outside = false;
			// first check special case when center of sphere is inside the box and clip coords along axes
			if (spherePositionInBoxCS.x < -boxExtents.x)
			{
				outside = true;
				spherePositionInBoxCS.x = -boxExtents.x;
			}
			else if (spherePositionInBoxCS.x > boxExtents.x)
			{
				outside = true;
				spherePositionInBoxCS.x = boxExtents.x;
			}
			if (spherePositionInBoxCS.y < -boxExtents.y)
			{
				outside = true;
				spherePositionInBoxCS.y = -boxExtents.y;
			}
			else if (spherePositionInBoxCS.y > boxExtents.y)
			{
				outside = true;
				spherePositionInBoxCS.y = boxExtents.y;
			}
			if (spherePositionInBoxCS.z < -boxExtents.z)
			{
				outside = true;
				spherePositionInBoxCS.z = -boxExtents.z;
			}
			else if (spherePositionInBoxCS.z > boxExtents.z)
			{
				outside = true;
				spherePositionInBoxCS.z = boxExtents.z;
			}

			if (outside)
			// center of our sphere is outside of the box
			{
				// get squared distance from sphere center to the box surface and compare to the squared radius of the sphere
				var clippedToWorldSquared = (delta - (boxRotation * spherePositionInBoxCS)).SqrMagnitude;
				var sqrRadius = sphereRadius * sphereRadius;
				if (sqrRadius < clippedToWorldSquared) // '<' here since "collided" means that we should return 'true' if shapes touches each other
                    result = false;
			}

#if UNITY_EDITOR
            {
				////WEAPON
				BoxShapeDef weaponDef = new BoxShapeDef();
				weaponDef.Position = boxCenter;
				weaponDef.Extents = boxExtents;
				weaponDef.Rotation = boxRotation.eulerAngles;
				EntitytObjectsUnitySpawnService.SpawnService.DrawShape(weaponDef, result ? Color.yellow : Color.red, result ? 1.0f : 0.1f);

				//TARGET
				SphereShapeDef targetDef = new SphereShapeDef();
                targetDef.Position = sphereCenter;
                targetDef.Radius = sphereRadius;
                EntitytObjectsUnitySpawnService.SpawnService.DrawShape(targetDef, result ? Color.blue : Color.green, result ? 1.0f : 0.1f);
            }
#endif

            return result;
        }
	}
}