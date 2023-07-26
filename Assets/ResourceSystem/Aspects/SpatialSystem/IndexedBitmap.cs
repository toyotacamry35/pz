using System;
using System.IO;
using System.Text;

namespace Assets.Src.SpatialSystem
{
	// ReSharper disable once InconsistentNaming
	public class IndexedBitmap : IIndexMap
	{
		public int IndexesCount { get; private set; }
		public int Width { get; private set; }
		public int Height { get; private set; }

		private byte[] _bytes;

		public short GetAt(int x, int y)
		{
			var i = (y * Width + x) * sizeof(short);
			if (i < 0 || i > _bytes.Length - sizeof(short))
				throw new Exception($"Error Requested Pixel x:{x}, y:{y} Exceed Size w:{Width}, h:{Height}");

			return BitConverter.ToInt16(_bytes, i);
		}

		public IndexedBitmap(int width, int height, int indexesCount)
		{
			Width = width;
			Height = height;

			_bytes = new byte[width * height * sizeof(short)];
		}

		public IndexedBitmap(Stream stream)
		{
			ReadFromStream(stream);
		}

		private IndexedBitmap(int width, int height, byte[] bytes)
		{
			Width = width;
			Height = height;

			_bytes = bytes;
		}

		public void SetIndexAt(int x, int y, short index)
		{
			var i = (y * Width + x) * sizeof(short);
			var bytes = BitConverter.GetBytes(index);
			Buffer.BlockCopy(bytes, 0, _bytes, i, sizeof(short));
		}

		public void WriteToStream(Stream stream)
		{
			using (var bw = new BinaryWriter(stream, Encoding.Default, true))
			{
				bw.Write(IndexesCount);
				bw.Write(Width);
				bw.Write(Height);
				bw.Write(_bytes);
				bw.Flush();
			}
		}

		public void ReadFromStream(Stream stream)
		{
			var br = new BinaryReader(stream);
			IndexesCount = br.ReadInt32();
			Width = br.ReadInt32();
			Height = br.ReadInt32();
			_bytes = br.ReadBytes(Width * Height * sizeof(short));
		}
	}
}