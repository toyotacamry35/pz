using ColonyShared.SharedCode.Aspects.Locomotion;
using static Src.Locomotion.MobInputs;

namespace Src.Locomotion.States
{
    internal class MobStateTeleport : StateBase<MobStateMachineContext>
    {
        //#Note: Сейчас этот стейт не исп-ся, т.к. переход в него только по "c.Input[Teleport]" (см. `MobStateMachine`), а этот инпут
        //  выставлялся только в `TeleportHelper.SetupInputState`, который теперь не вызывается.
        //  (раньше вызывался из `MoveActionFollowPath.GetLocomotionInput` и &KeepDist).
        public override void Execute(MobStateMachineContext ctx, VariablesPipeline pipeline)
        {
            pipeline.ApplyFlags(LocomotionFlags.Teleport)
                .ApplyTeleport(new LocomotionVector(ctx.Input[TargetPointXY], ctx.Input[TargetPointV]));
        }
    }
}