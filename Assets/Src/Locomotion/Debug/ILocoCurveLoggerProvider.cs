using SharedCode.Utils;
using TimeUnits = System.Int64;

namespace Assets.Src.Locomotion.Debug
{
    public interface ILocoCurveLoggerProvider : ICurveLoggerProvider, IFrameIdNormalizer
    {
    }

    public interface IFrameIdNormalizer
    {
        TimeUnits NormalizeFrameId(TimeUnits frameId);
    }
    // Just plug (don't assume, curve 'll be convenient(readable))
    public class DefaultFrameIdNormalizer : IFrameIdNormalizer
    {
        private const long DefaultBaseFrameId = 63700000000000L; //FrameId is Now in ticks. This val. is in the past when code is written.
        public static DefaultFrameIdNormalizer Instance = new DefaultFrameIdNormalizer();
        public TimeUnits NormalizeFrameId(TimeUnits frameId) => frameId - DefaultBaseFrameId;
    }
}
