using SharedCode.Aspects.Regions;
using UnityEngine;

namespace Assets.Src.Regions
{
	public class FogOfWarRegion : MonoBehaviour
	{
		public FogOfWarRegionDef GetRegionDef()
		{
			return new FogOfWarRegionDef();
		}
	}
}