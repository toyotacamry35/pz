using System.IO;
using NUnit.Framework;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.Src.SpatialSystem.Editor
{
	public class SVOTests
	{
		private const string TestTexturePath = "Assets/Content/RegionsData/FogOfWar/Savannah_FogOfWar.png";
		private const int W = 16;
		private const int H = 16;

		private Color32[] SingleColor { get; set; }
		private PointsArray<byte> SampleIndexes { get; set; }


		[SetUp]
		public void Setup()
		{
			SingleColor = Enumerable.Range(0, W * H)
				.Select(v => (Color32) Color.red).ToArray();

			// Y in Array Declaration From Top To Bottom
			SampleIndexes = new PointsArray<byte>(new byte[]
				{
					1, 1, 1, 0,
					1, 2, 2, 2,
					3, 3, 2, 2,
					3, 3, 4, 0
				}, 4, 4
			);
		}

		[Test]
		public void TestBuildSampleIndexes()
		{
			var svo = SVO.BuildSVO(SampleIndexes);
			for (var y = 0; y < SampleIndexes.Height; ++y)
			for (var x = 0; x < SampleIndexes.Width; ++x)
				Assert.AreEqual(svo.GetAt(x, y), SampleIndexes.GetAt(x, y));
		}

		[Test]
		public void TestBuildSingleColor()
		{
			var indexedColors = SVOExporter.CreateIndexes<Color32, int>(SingleColor);
			var svo = SVOExporter.CreateSVO(SingleColor, W, H, indexedColors);
			Assert.AreEqual(svo.GetAt(0, 0), indexedColors.Single().Value);
			Assert.AreEqual(svo.GetAt(W - 1, H - 1), indexedColors.Single().Value);
		}

		[Test]
		public void TestRealData()
		{
			var tex2d = AssetDatabase.LoadAssetAtPath<Texture2D>(TestTexturePath);
			Assert.NotNull(tex2d, "Asset is missing - change to any real weather texture");

			var pixels = tex2d.GetPixels32();
			var w = tex2d.width;
			var h = tex2d.height;

			var colors = SVOExporter.CreateIndexes<Color32, int>(tex2d.GetPixels32());
			var svo = SVOExporter.CreateSVO(pixels, w, h, colors);

			var rng = new System.Random(0);
			for (var _ = 0; _ < 1000; ++_)
			{
				var x = rng.Next(w);
				var y = rng.Next(h);

				var svoVal = svo.GetAt(x, y);
				var texColor = pixels[x + y * w];

				var texVal = colors[texColor];

				Assert.AreEqual(texVal, svoVal, "Difference at pixel ({0}, {1}) at try {2}", x, y, _);
			}
		}

		[Test]
		public void TestRandomData()
		{
			var rng = new System.Random(0);
			var bytes = new byte[3];
			const int width = 256;
			const int height = 256;
			var pixels = Enumerable.Range(0, width * height).Select(i =>
			{
				rng.NextBytes(bytes);
				return new Color32(bytes[0], bytes[1], bytes[2], 0xFF);
			}).ToArray();

			var colors = SVOExporter.CreateIndexes<Color32, int>(pixels);
			var svo = SVOExporter.CreateSVO(pixels, width, height, colors);

			for (var _ = 0; _ < 1000; ++_)
			{
				var x = rng.Next(width);
				var y = rng.Next(height);

				var svoVal = svo.GetAt(x, y);
				var texColor = pixels[x + y * width];

				var texVal = colors[texColor];

				Assert.AreEqual(texVal, svoVal, "Difference at pixel ({0}, {1}) at try {2}", x, y, _);
			}
		}

		[Test]
		public void TestSerialization()
		{
			var svo = SVO.BuildSVO(SampleIndexes);

			byte[] data;
			using (var ms = new MemoryStream())
			{
				svo.WriteToStream(ms);
				data = ms.ToArray();
			}

			SVO deserializedSVO;
			using (var ms = new MemoryStream(data))
			{
				deserializedSVO = new SVO();
				deserializedSVO.ReadFromStream(ms);
			}

			for (var y = 0; y < SampleIndexes.Height; ++y)
			for (var x = 0; x < SampleIndexes.Width; ++x)
				Assert.AreEqual(svo.GetAt(x, y), deserializedSVO.GetAt(x, y));
		}
	}
}