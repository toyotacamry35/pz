using System.Collections.Generic;
using Assets.Src.SpatialSystem;
using GeneratedCode.Custom.Config;
using SharedCode.Utils;

namespace Assets.ColonyShared.GeneratedCode.Regions
{
	public interface IIndexRegion : IRegion
	{
		MapDef MapDef { get; }
		int IndexWidth { get; set; }
		int IndexHeight { get; set; }
		
		short GetIndexAt(Vector3 coords);
		short GetIndexFromRectAt(int x, int y);

		IEnumerable<IndexBlock> GetIndexBlocks();
	}
}