using Assets.Src.ResourcesSystem.Base;

namespace Assets.Src.SpatialSystem
{
	public interface IIndexMap : IPointsRect<short>, IBinarySerializable
	{
		int IndexesCount { get; }
	}
}