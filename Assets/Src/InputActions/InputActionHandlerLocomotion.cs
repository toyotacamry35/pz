using Src.Locomotion;

namespace Src.InputActions
{
    public interface IInputActionHandlerLocomotion
    {
        void FetchInputValue(InputState<CharacterInputs> frame);
    }
}