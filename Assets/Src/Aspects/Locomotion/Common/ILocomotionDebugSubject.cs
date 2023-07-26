using Src.Locomotion.Unity;

namespace Src.Locomotion
{
    public interface ILocomotionDebugSubject
    {
         ILocomotionDebugInfoProvider LocomotionDebugInfo { get; }
        LocomotionDebugTrail LocomotionDebugTrail { get; }
    }
}