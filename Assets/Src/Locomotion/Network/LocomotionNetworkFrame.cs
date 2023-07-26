//using ColonyShared.SharedCode.Utils;
//using Mathf = Assets.ColonyShared.SharedCode.Utils.SharedHelpers; 
//
//namespace Src.Locomotion
//{
//    public struct LocomotionNetworkFrame
//    {
//        public readonly bool IsValid;
//        public readonly long FrameId;
//        public readonly LocomotionVariables Variables;
//
//        public LocomotionNetworkFrame(long frameId, LocomotionVariables vars) 
//        {
//            IsValid = true;
//            FrameId = frameId;
//            Variables = vars;
//        }
//
//        public override string ToString()
//        {
//            var sb = StringBuildersPool.Acquire()
//                .Append(Variables.Flags)
//                .Append(" | ").Append(Variables.Position)
//                .Append(" | ").Append(Variables.Velocity)
//                .Append(" | ").Append((int) (Variables.Orientation * Mathf.Rad2Deg));
//            if (!Variables.AngularVelocity.ApproximatelyZero())
//                sb.Append(" | ").Append((int) (Variables.AngularVelocity * Mathf.Rad2Deg));
//            return sb.Release();
//        }
//    }
//}