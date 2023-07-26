using System;
using System.Collections.Generic;
using System.Linq;
using EnumerableExtensions;
using UnityEngine;

namespace Assets.Src.SpatialSystem.Editor
{
	public static class SVOExporter
	{
		public static SVO CreateSVO<TParam, T>(IEnumerable<TParam> pixels, int width, int height, IReadOnlyDictionary<TParam, T> indexes)
			where T : IConvertible
		{
			var map = pixels.Select(color32 => indexes[color32]).ToArray();
			var pointsArray = new PointsArray<T>(map, width, height);
			return SVO.BuildSVO(pointsArray);
		}

		public static Dictionary<TParam, T> CreateIndexes<TParam, T>(IEnumerable<TParam> pixels)
		{
			var hash = new HashSet<TParam>();
			pixels.ForEach(c => hash.Add(c));
			return hash
				.Select((c, i) => (c, i))
				.ToDictionary(v => v.c, v => (T) Convert.ChangeType(v.i, typeof(T)));
		}

		public static IndexedBitmap CreateIndexedBitmap<TParam, T>(IEnumerable<TParam> pixels, int width, int height,
			IReadOnlyDictionary<TParam, T> indexedColors)
			where T : IConvertible
		{
			var indexMap = new IndexedBitmap(width, height, indexedColors.Values.Count());
			var i = 0;
			foreach (var pixel in pixels)
			{
				var quotient = Math.DivRem(i, width, out var remainder);
				if (indexedColors.TryGetValue(pixel, out var indexedColor))
					indexMap.SetIndexAt(remainder, quotient, Convert.ToInt16(indexedColor));
				else
					throw new Exception($"Error Color {pixel} Not In Index");
				++i;
			}

			return indexMap;
		}

		public static Texture2D ExportIndexTextureR16<TParam, T>(
			IEnumerable<TParam> pixels, int width, int height, IReadOnlyDictionary<TParam, T> indexes
		) where T : IConvertible
		{
			const short maxValue = short.MaxValue;

			var count = indexes.Values.Count();
			if (count > maxValue)
				throw new ArgumentException($"Error To Many Indexes Passed: {count} Max: {maxValue}");

			var indexed = pixels.Select(param => indexes[param]).ToArray();

			var indexPixels = indexed
				.Select(index => new Color(Convert.ToSingle(index) / maxValue, 0, 0))
				.ToArray();
			var texture = new Texture2D(width, height, TextureFormat.R16, false, true);
			texture.SetPixels(indexPixels);
			texture.Apply();

			//Test Read Generated
			indexPixels = texture.GetPixels();
			for (var i = 0; i < indexPixels.Length; i++)
				if ((short) Mathf.RoundToInt(indexPixels[i].r * maxValue) != Convert.ToInt16(indexed[i]))
					throw new Exception($"Error Set Pixel At {i} Float: {indexPixels[i].r} * {maxValue} Original: {indexed[i]}");

			return texture;
		}
	}
}