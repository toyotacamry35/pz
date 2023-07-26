using System;

namespace Assets.Src.SpatialSystem
{
	public interface IPointsRect<out T> where T : IConvertible
	{
		int Width { get; }
		int Height { get; }

		T GetAt(int x, int y);
	}
}