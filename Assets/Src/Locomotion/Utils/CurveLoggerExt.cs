using SharedCode.Utils;
using Src.Locomotion;

using TimeUnits = System.Int64;

namespace Assets.Src.Locomotion.Utils
{
    public static class CurveLoggerExt
    {
        public static void AddData(this CurveLogger logger, string id, TimeUnits timeStapm, LocomotionVector v)
        {
            logger.AddData(id, timeStapm, v.ToWorld());
        }

        public static Vector3 VeloAsDltPos(LocomotionVector pos, LocomotionVector velo) => CurveLogger.VeloAsDltPos(pos.ToWorld(), velo.ToWorld());
        public static Vector3 VeloAsDltPos(ref LocomotionVariables inVars) => VeloAsDltPos(inVars.Position, inVars.Velocity);
    }
}
