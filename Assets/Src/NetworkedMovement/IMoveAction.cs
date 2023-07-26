using System;
using SharedCode.EntitySystem;
using Src.Locomotion;

namespace Assets.Src.NetworkedMovement.MoveActions
{
    internal interface IMoveAction : IDisposable
    {
        bool Init();

        MoveActionResult Tick(Pawn.SimulationLevel simulationLevel);
        
        void GetLocomotionInput(InputState<MobInputs> inputs);
        
        void Terminate();
    }
    
    public enum MoveActionResult
    {
        Running,
        Finished,
        Failed
    }
}