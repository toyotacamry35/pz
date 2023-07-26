using System.Threading.Tasks;
using SharedCode.Entities.Engine;

namespace GeneratedCode.DeltaObjects
{
    public partial class LocomotionOwner
    {
        public ValueTask<bool> IsValidImpl()
        {
            var result = Locomotion != null 
                && DirectMotionProducer != null 
                && GuideProvider != null;
            return new ValueTask<bool>(result);
        }
        public ValueTask SetLocomotionImpl(ILocomotionEngineAgent locomotion, IDirectMotionProducer motionProducer, IGuideProvider guideProvider)
        {
            Locomotion = locomotion;
            DirectMotionProducer = motionProducer;
            GuideProvider = guideProvider;
            return new ValueTask();
        }
    }
}