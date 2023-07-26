using System;
using NUnit.Framework;
using Src.Locomotion;

public static class RingBufferTests
{

    [TestCase(0)]
    [TestCase(1)]
    [TestCase(61)]
    public static void InsertForwardTest(int capacity)
    {
        var rnd = new Random(123456);
        var buffer = new RingBuffer<int>(61);
        var list = new System.Collections.Generic.List<int>();
        for (int i = 0; i < 2000; ++i)
        {
            var idx = rnd.Next(buffer.Count);
            Assert.DoesNotThrow(() => buffer.InsertFront(idx, i));
            list.Insert(idx, i);
            for(int j=0; j<buffer.Count; ++j)
                Assert.AreEqual(buffer[j], list[j]);
        }
    }
}
