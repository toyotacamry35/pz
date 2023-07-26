using ColonyShared.SharedCode.Aspects.Locomotion;

namespace SharedCode.Entities.Engine
{
    public interface ILocomotionEngineAgent
    {
        LocomotionFlags Flags { get; }

        void SetReady(bool val);
    }
}