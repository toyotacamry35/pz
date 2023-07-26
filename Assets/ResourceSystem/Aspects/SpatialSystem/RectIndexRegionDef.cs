using Assets.Src.ResourcesSystem.Base;
using GeneratedCode.Custom.Config;
using SharedCode.Aspects.Regions;
using SharedCode.Utils;

namespace Assets.Src.SpatialSystem
{
	public class RectIndexRegionDef : ARegionDef
	{
		public ResourceRef<MapDef> MapDef { get; set; }
		public Vector3 Center { get; set; }
		public Vector3 Extents { get; set; }
		public Quaternion InverseRotation { get; set; }
		public int IndexHeight { get; set; }
		public int IndexWidth { get; set; }
	}
}