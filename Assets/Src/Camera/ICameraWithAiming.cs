using SharedCode.Entities.Engine;
using Src.Aspects.Doings;

namespace Src.Camera
{
    public interface ICameraWithAiming
    {
        void SetGuideProvider(IGuideProvider guideProvider);
    }
}
