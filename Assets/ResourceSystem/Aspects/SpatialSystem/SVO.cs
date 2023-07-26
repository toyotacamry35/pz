using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Assets.Src.SpatialSystem
{
    public class SVO : IIndexMap
    {
        private const string Header = "SVO_1.0";
        public int IndexesCount { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        private IReadOnlyList<int> Nodes { get; set; }

        public SVO()
        {
        }

        public SVO(int width, int height, IReadOnlyList<int> nodes, int indexesCount)
        {
            Width = width;
            Height = height;
            Nodes = nodes;
            IndexesCount = indexesCount;
        }

        public SVO(Stream stream)
        {
            ReadFromStream(stream);
        }

        public void ReadFromStream(Stream stream)
        {
            using (var br = new BinaryReader(stream))
            {
                var header = br.ReadString();
                if (Header != header)
                    throw new Exception("SVO Version Mismatch");
                IndexesCount = br.ReadInt32();

                Width = br.ReadInt32();
                Height = br.ReadInt32();

                var length = br.ReadInt32();
                var nodes = new int[length];
                for (var i = 0; i < length; i++)
                    nodes[i] = br.ReadInt32();
                Nodes = nodes;
            }
        }

        public void WriteToStream(Stream stream)
        {
            using (var bw = new BinaryWriter(stream, Encoding.Default, true))
            {
                bw.Write(Header);
                bw.Write(IndexesCount);
                bw.Write(Width);
                bw.Write(Height);
                bw.Write(Nodes.Count);
                foreach (var node in Nodes)
                    bw.Write(node);

                bw.Flush();
            }
        }

        public short GetAt(int x, int y)
        {
            if (x >= Width || x < 0)
                throw new ArgumentException($"X ({x}) is out of range (0-{Width})", nameof(x));
            if (y >= Height || y < 0)
                throw new ArgumentException($"Y ({y}) is out of range (0-{Height}) ", nameof(y));
            if (Nodes == null || Nodes.Count < 1)
                throw new ArgumentException("Empty Index Map", nameof(y));

            var curNodeIndex = 0;
            var width = Width;
            var height = Height;

            while (true)
            {
                var data = Nodes[curNodeIndex];
                if (data >= 0)
                    return (short) data;

                curNodeIndex += -1 * data;

                width /= 2;
                height /= 2;
                if (y >= height)
                {
                    y -= height;
                    curNodeIndex += 2;
                }

                if (x >= width)
                {
                    x -= width;
                    curNodeIndex += 1;
                }
            }
        }

        public IEnumerable<IndexBlock> GetIndexBlocks()
        {
            return GetIndexBlocks(0, 0, 0, Width, Height);
        }

        private IEnumerable<IndexBlock> GetIndexBlocks(int curNodeIndex, int x, int y, int width, int height)
        {
            var data = Nodes[curNodeIndex];
            if (data >= 0)
                yield return new IndexBlock {X = x, Y = y, Width = width, Height = height, Index = (short) data};
            else
            {
                width /= 2;
                height /= 2;
                curNodeIndex -= data;
                foreach (var indexBlock in GetIndexBlocks(curNodeIndex, x, y, width, height))
                    yield return indexBlock;
                foreach (var indexBlock in GetIndexBlocks(curNodeIndex + 1, x + width, y, width, height))
                    yield return indexBlock;
                foreach (var indexBlock in GetIndexBlocks(curNodeIndex + 2, x, y + height, width, height))
                    yield return indexBlock;
                foreach (var indexBlock in GetIndexBlocks(curNodeIndex + 3, x + width, y + height, width, height))
                    yield return indexBlock;
            }
        }


        public static SVO BuildSVO<T>(IPointsRect<T> map) where T : IConvertible
        {
            if (!IsPowOf2(map.Width))
                throw new ArgumentException("Width Not PowerOfTwo", nameof(map));
            if (!IsPowOf2(map.Height))
                throw new ArgumentException("Height Not PowerOfTwo", nameof(map));
            var indexes = GetRegionEnumerable(map, 0, 0, map.Width, map.Height)
                .Distinct()
                .OrderBy(index => index)
                .Select((index, i) => (index: Convert.ToInt32(index), i))
                .ToArray();
            if (indexes.Any(tuple => tuple.i != tuple.index))
                throw new Exception("Error Inconsistent Indexes Provided");

            var nodes = CreateNodes(map);
            return new SVO(map.Width, map.Height, nodes, indexes.Length);
        }

        private static List<int> CreateNodes<T>(IPointsRect<T> map) where T : IConvertible
        {
            var nodeList = new List<int> {0};
            FillNode(0, nodeList, map, (0, 0, map.Width, map.Height));
            nodeList.TrimExcess();
            return nodeList;
        }

        private static void FillNode<T>(
            int currentNodeIndex,
            List<int> nodeList,
            IPointsRect<T> points,
            (int x0, int y0, int x1, int y1) region) where T : IConvertible
        {
            var (x0, y0, x1, y1) = region;
            var uniquePixels = GetRegionEnumerable(points, x0, y0, x1, y1).Distinct().ToArray();
            if (uniquePixels.Skip(1).Any())
            {
                var nextJump = nodeList.Count - currentNodeIndex;
                WriteNodeNext(nextJump, nodeList, currentNodeIndex);
                nodeList.AddRange(Enumerable.Repeat(0, 4));

                var childWidth = (x1 - x0) / 2;
                var childHeight = (y1 - y0) / 2;
                for (var y = 0; y < 2; ++y)
                for (var x = 0; x < 2; ++x)
                {
                    var childNodeIndex = currentNodeIndex + nextJump + y * 2 + x;
                    FillNode(childNodeIndex, nodeList, points, (
                        x0 + x * childWidth,
                        y0 + y * childHeight,
                        x0 + (x + 1) * childWidth,
                        y0 + (y + 1) * childHeight
                    ));
                }
            }
            else
            {
                var value = Convert.ToInt32(uniquePixels.Single());
                WriteNodeValue(value, nodeList, currentNodeIndex);
            }
        }

        private static void WriteNodeNext(int nextIndex, IList<int> nodeList, int node)
        {
            nodeList[node] = -1 * nextIndex;
        }

        private static void WriteNodeValue(int value, IList<int> nodeList, int node)
        {
            nodeList[node] = value;
        }

        private static IEnumerable<T> GetRegionEnumerable<T>(IPointsRect<T> points, int x0, int y0, int x1, int y1)
            where T : IConvertible
        {
            for (var y = y0; y < y1; ++y)
            for (var x = x0; x < x1; ++x)
                yield return points.GetAt(x, y);
        }

        private static bool IsPowOf2(int val)
        {
            var pow = Math.Log(val, 2);
            var powInt = (int) Math.Round(pow);
            var valNew = (int) Math.Pow(2, powInt);
            return valNew == val;
        }
    }
}