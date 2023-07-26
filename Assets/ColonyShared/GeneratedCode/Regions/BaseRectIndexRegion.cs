using System;
using System.Collections.Generic;
using Assets.Src.SpatialSystem;
using GeneratedCode.Custom.Config;
using SharedCode.Aspects.Regions;
using SharedCode.Entities;
using SharedCode.Utils;

namespace Assets.ColonyShared.GeneratedCode.Regions
{
	public abstract class BaseRectIndexRegion : BaseRegion, IIndexRegion
	{
		private Vector3 Center { get; set; }
		private Vector3 Extents { get; set; }
		private Quaternion InverseRotation { get; set; }
		public int IndexHeight { get; set; }
		public int IndexWidth { get; set; }
		public MapDef MapDef { get; private set; }

		private readonly List<IRegion> _notIndexedRegions = new List<IRegion>();
		private readonly Dictionary<short, IndexedRegion> _indexedRegions = new Dictionary<short, IndexedRegion>();

		public override bool IsInside(Vector3 pointCoords) =>
			GeometryHelpers.IsPointInsideBox(pointCoords, Center, Extents, InverseRotation);

		public override void InitRegionWithDef(ARegionDef def, Transform providedTransform = default)
		{
			base.InitRegionWithDef(def, providedTransform);

			if (!(def is RectIndexRegionDef indexRegionDef))
				throw new Exception($"Error {GetType()} No Region Def");

			Center = indexRegionDef.Center + providedTransform.Position;
			Extents = indexRegionDef.Extents;
			InverseRotation = indexRegionDef.InverseRotation;
			IndexWidth = indexRegionDef.IndexWidth;
			IndexHeight = indexRegionDef.IndexHeight;
			MapDef = indexRegionDef.MapDef;
		}

		public abstract short GetIndexFromRectAt(int x, int y);
		
		public abstract IEnumerable<IndexBlock> GetIndexBlocks();
		
		public virtual short GetIndexAt(Vector3 coords)
		{
			if (GeometryHelpers.PointToRectPos(
				coords,
				Center,
				Extents,
				InverseRotation,
				IndexWidth,
				IndexHeight,
				out var x, out var y))
				return GetIndexFromRectAt(x, y);

			return -1;
		}

		public override void GetAllContainingChildren(List<IRegion> regions, Vector3 pointCoords)
		{
			if (_indexedRegions != null &&
			    _indexedRegions.TryGetValue(GetIndexAt(pointCoords), out var indexedRegion)
			) {
				regions.Add(indexedRegion);
				indexedRegion.GetAllContainingChildren(regions, pointCoords);
			}

			if (_notIndexedRegions != null)
				foreach (var childRegion in _notIndexedRegions)
					childRegion.GetAllContainingRegionsNonAlloc(regions, pointCoords);
		}

		protected override void OnChildAdded(IRegion region)
		{
			if (region is IndexedRegion indexedRegion)
				_indexedRegions[indexedRegion.Index] = indexedRegion;
			else
				_notIndexedRegions.Add(region);
		}
	}
}