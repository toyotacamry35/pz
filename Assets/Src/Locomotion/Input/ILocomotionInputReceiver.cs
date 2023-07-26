using SharedCode.Utils;

namespace Src.Locomotion
{
    public interface ILocomotionInputReceiver 
    {
        void SetInput(InputAxis it, float value);
        void SetInput(InputAxes it, Vector2 value);
        void SetInput(InputTrigger it, bool value);
        void PushInput(object causer, string inputName, float value);
        void PopInput(object causer, string inputName);
    }
}