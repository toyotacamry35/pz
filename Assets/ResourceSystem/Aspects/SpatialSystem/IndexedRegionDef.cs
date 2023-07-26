using System;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.Aspects.Regions;
using SharedCode.Utils;

namespace Assets.Src.SpatialSystem
{
	public class IndexedRegionDef : ARegionDef, ISaveableResource
	{
		public short Index { get; set; }
		public Guid Id { get; set; }
	}
}