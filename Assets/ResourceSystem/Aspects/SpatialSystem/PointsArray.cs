using System;

namespace Assets.Src.SpatialSystem
{
	public class PointsArray<T> : IPointsRect<T> where T : IConvertible
	{
		private readonly T[] _arr;

		public int IndexesCount { get; }
		public int Width { get; }
		public int Height { get; }

		public PointsArray(T[] arr, int rowWidth, int rowHeight)
		{
			_arr = arr;
			Width = rowWidth;
			Height = rowHeight;
		}

		public T GetAt(int x, int y)
		{
			return _arr[y * Width + x];
		}
	}
}