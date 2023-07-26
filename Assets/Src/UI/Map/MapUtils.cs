using System.Linq;
using Assets.Src.Shared;
using Uins;
using UnityEngine;

internal static class MapUtils
{
	public static void GetCornersFromTerrainBounds(out Vector2 cornerBottomLeft, out Vector2 cornerTopRight)
	{
		var bounds = new Bounds();
		var isFirstPass = true;
		var terrains = Terrain.activeTerrains;
		UI.CallerLog($">>>>>>> Terrains count: {terrains?.Length ?? 0}"); //DEBUG
		if (terrains != null && terrains.Length > 0)
		{
			foreach (var terrain in terrains)
			{
				var terrainTransform = terrain.transform;

				if (terrain.terrainData == null && terrainTransform.GetComponent<Collider>() == null)
					continue;
				var terrainData = terrain.terrainData;
				var terrainBounds = terrainData != null
					? terrainData.bounds
					: terrainTransform.GetComponent<Collider>().bounds;

				terrainBounds.center += terrainTransform.position;

				if (isFirstPass)
				{
					isFirstPass = false;
					bounds = terrainBounds;
				}
				else
				{
					bounds.Encapsulate(terrainBounds);
				}
			}
		}
		else
		{
			var terrainColliders = Object.FindObjectsOfType<Collider>().Where(cld => cld.gameObject.layer == PhysicsLayers.Terrain);
			foreach (var terrainCollider in terrainColliders)
			{
				var colliderBounds = terrainCollider.bounds;
				colliderBounds.center += terrainCollider.transform.position;

				if (isFirstPass)
				{
					isFirstPass = false;
					bounds = colliderBounds;
				}
				else
				{
					bounds.Encapsulate(colliderBounds);
				}
			}
		}

		cornerBottomLeft = new Vector2(bounds.min.x, bounds.min.z);
		cornerTopRight = new Vector2(bounds.max.x, bounds.max.z);
		UI.CallerLogInfo($"Corners from terrains: BL={cornerBottomLeft}, TR={cornerTopRight}, bounds={bounds}"); //DEBUG
	}
}