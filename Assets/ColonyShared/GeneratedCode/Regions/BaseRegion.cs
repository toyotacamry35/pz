using System.Collections.Generic;
using SharedCode.Aspects.Regions;
using SharedCode.Entities;
using SharedCode.Utils;

namespace Assets.ColonyShared.GeneratedCode.Regions
{
	public abstract class BaseRegion : IRegion
	{
		private IRegion _parentRegion;
		protected readonly List<IRegion> ChildRegions = new List<IRegion>();

		public IRegion ParentRegion
		{
			get => _parentRegion;
			set
			{
				_parentRegion = value;
				OnParentUpdated();
			}
		}

		List<IRegion> IRegion.ChildRegions => ChildRegions;

		public void AddChild(IRegion buildRegion)
		{
			if (!ChildRegions.Contains(buildRegion))
			{
				ChildRegions.Add(buildRegion);
				OnChildAdded(buildRegion);
			}
		}

		public int Level { get; set; }
		public ARegionDef RegionDef { get; set; }

		public virtual void InitRegionWithDef(ARegionDef def, Transform providedTransform = default)
		{
			RegionDef = def;
		}

		public abstract bool IsInside(Vector3 coords);

		public virtual void GetAllContainingRegionsNonAlloc(List<IRegion> regions, Vector3 pointCoords)
		{
			if (IsInside(pointCoords))
			{
				regions.Add(this);

				GetAllContainingChildren(regions, pointCoords);
			}
		}

		public virtual void GetAllContainingChildren(List<IRegion> regions, Vector3 pointCoords)
		{
			if (ChildRegions != null)
				foreach (var childRegion in ChildRegions)
					childRegion.GetAllContainingRegionsNonAlloc(regions, pointCoords);
		}

		protected virtual void OnChildAdded(IRegion buildRegion)
		{
		}

		protected virtual void OnParentUpdated()
		{
		}
	}
}