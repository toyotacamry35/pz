using System;
using Assets.Src.Locomotion.Utils;
using ColonyShared.SharedCode.Aspects.Locomotion;
using ColonyShared.SharedCode.Utils;
using JetBrains.Annotations;
using SharedCode.Utils;

namespace Src.Locomotion
{
    public class LocomotionSimpleMovementWithGroundNode : ILocomotionPipelinePassNode
    {
        private readonly ILocomotionGroundSensor _groundSensor;
        private readonly ICurveLoggerProvider _curveLogProv;

        public LocomotionSimpleMovementWithGroundNode([NotNull] ILocomotionGroundSensor groundSensor, Func<bool> shouldSaveLocoVars, Action<LocomotionVariables, Type> saveLocoVarsCallback, [CanBeNull] ICurveLoggerProvider curveLogProv = null)
        {
            if (groundSensor == null) throw new ArgumentNullException(nameof(groundSensor));
            _groundSensor = groundSensor;
            _curveLogProv = curveLogProv;
            ShouldSaveLocoVars = shouldSaveLocoVars;
            SaveLocoVarsCallback = saveLocoVarsCallback;
        }

        bool ILocomotionPipelinePassNode.IsReady => true;

        ///#PZ-13568: #Dbg:
        protected Func<bool> ShouldSaveLocoVars;
        protected Action<LocomotionVariables, Type> SaveLocoVarsCallback;

        LocomotionVariables ILocomotionPipelinePassNode.Pass(LocomotionVariables vars, float dt)
        {
            var velocity = vars.Velocity;
            var position = vars.Position;

            if (!vars.Flags.Any(LocomotionFlags.Teleport))
            {
                var deltaPosition = (velocity + vars.ExtraVelocity) * dt + vars.ExtraPosition;
                position += deltaPosition;

                // handle +deltaPosition piercing the ground:
                if (_groundSensor.DistanceToGround + deltaPosition.Vertical <= 0)
                {
                    position = new LocomotionVector(position.Horizontal, vars.Position.Vertical - _groundSensor.DistanceToGround);
                    velocity = new LocomotionVector(velocity.Horizontal, Math.Max(velocity.Vertical, 0));
                }
            }

            ///#PZ-13568: #Dbg: #Tmp replaced by next lines:
            // return new LocomotionVariables(vars) { Position = position, Velocity = velocity, ExtraVelocity = LocomotionVector.Zero, ExtraPosition = LocomotionVector.Zero };
            var result = new LocomotionVariables(vars) { Position = position, Velocity = velocity, ExtraVelocity = LocomotionVector.Zero, ExtraPosition = LocomotionVector.Zero };

            _curveLogProv?.CurveLogger?.IfActive?.AddData("0.3) SmplPhys.Velo", SyncTime.Now, result.Velocity);
            _curveLogProv?.CurveLogger?.IfActive?.AddData("0.3) SmplPhys.Pos", SyncTime.Now, result.Position);
            _curveLogProv?.CurveLogger?.IfActive?.AddData("0.3) SmplPhys.DistanceToGround", SyncTime.Now, _groundSensor.DistanceToGround);
            //#Dbg:
            if (ShouldSaveLocoVars?.Invoke() ?? false)
                SaveLocoVarsCallback(result, this.GetType());

            return result;

            //Dbg:
            // var resultVars = new LocomotionVariables(vars) { Position = position, Velocity = velocity, ExtraVelocity = LocomotionVector.Zero, ExtraPosition = LocomotionVector.Zero};
            // _curveLogProv?.CurveLogger?.IfActive?.AddData("0.3) SmplPhys.Velo", SyncTime.Now, resultVars.Velocity);
            // return resultVars;
        }
    }
}