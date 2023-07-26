namespace Src.Locomotion
{
    public interface ILocomotionInputSource<TInputs> where TInputs : Inputs, new()
    {
        IInputState<TInputs> GetInput();
    }
}
