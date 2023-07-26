using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects;
using NUnit.Framework;
using SharedCode.Utils.BsonSerialization;

namespace Assets.Tests
{
    [TestFixture]
    class DatabaseTests
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        [Test]
        public void TestTypeHashGeneration()
        {
            var type = typeof(RealmsCollectionEntity);
            string currentHash = TypeHashCalculator.GetHashByType(type);
            Logger.IfInfo()?.Message(currentHash).Write();
        }
    }
}
