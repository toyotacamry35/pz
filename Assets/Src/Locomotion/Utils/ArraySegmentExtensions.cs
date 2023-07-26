using System;

namespace Src.Locomotion
{
    internal static class ArraySegmentExtensions
    {
        internal static T Get<T>(this ArraySegment<T> self, int index)
        {
            if (self.Array == null)
                throw new InvalidOperationException("Null Array Segment");
            if (index < 0 || index >= self.Count)
                throw new ArgumentOutOfRangeException($"Index {index} out of range {self.Count}", nameof (index));
            return self.Array[self.Offset + index];
        }

        internal static void Set<T>(this ArraySegment<T> self, int index, T value)
        {
            if (self.Array == null)
                throw new InvalidOperationException("Null Array Segment");
            if (index < 0 || index >= self.Count)
                throw new ArgumentOutOfRangeException($"Index {index} out of range {self.Count}", nameof (index));
            self.Array[self.Offset + index] = value;
        }
        
        internal static void Clear<T>(this ArraySegment<T> self)
        {
            if (self.Array == null)
                throw new InvalidOperationException("Null Array Segment");
            Array.Clear(self.Array, self.Offset, self.Count);
        }
        
        internal static void Copy<T>(this ArraySegment<T> src, ArraySegment<T> dst)
        {
            if (src.Array == null) throw new InvalidOperationException($"Null Array Segment {src.Array}");
            if (dst.Array == null) throw new InvalidOperationException($"Null Array Segment {dst.Array}");
            Array.Copy(src.Array, src.Offset, dst.Array, dst.Offset, src.Count);
        }

    }
}