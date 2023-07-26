using Assets.Src.ResourcesSystem.Base;

namespace Assets.ColonyShared.SharedCode.Aspects
{
    public class CameraDef : BaseResource
    {
        public float MaxPitchDown = 50;
        public float MaxPitchUp = 50;
        public float YawSensitivity = 100;
        public float PitchSensitivity = 2;
    }
}