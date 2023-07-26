using System;
using NLog;
using Src.Locomotion;

namespace Assets.Src.NetworkedMovement.MoveActions
{
    abstract class MoveAction : IMoveAction
    {
        protected static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public abstract bool Init();

        public abstract MoveActionResult Tick(Pawn.SimulationLevel simulationLevel);

        public abstract void GetLocomotionInput(InputState<MobInputs> inputs);
 
        public virtual void Terminate() { }

        public virtual void Dispose() {}

        protected MoveAction(Guid entityId)
        {
            EntityId = entityId;
        }

        protected Guid EntityId { get; private set; }
        
        protected MobMoveActionsConstantsDef Constants { get; private set; }

        public override string ToString() => GetType().Name;

        // --- Dbg: ---------------------------------
        protected bool DbgMode = false; ///true;
    }
}
