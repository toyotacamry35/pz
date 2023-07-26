using SharedCode.Utils;

namespace Src.Locomotion
{
    public interface ILocomotionBody
    {
        LocomotionVector Velocity { get; }
        
        LocomotionVector Position { get; }
        
        float Orientation { get; }
        
        Vector2 Forward { get; }
    }
}