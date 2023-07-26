using Assets.ColonyShared.SharedCode.Aspects;

namespace Assets.Src.Camera
{
    public interface ICameraRig
    {
        CameraDef ActiveCamera { get; }

        void ActivateCamera(CameraDef tag, object causer);

        void DeactivateCamera(CameraDef tag, object causer);
    }
}