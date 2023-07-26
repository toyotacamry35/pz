namespace Src.Locomotion
{
    public sealed class MobInputs : CommonInputs
    {
        #region Registry        
        private new static readonly Registry Define = new Registry(nameof(MobInputs), CommonInputs.Define); 
        public override int AxisCount => Define.AxisCount;        
        public override int TriggerCount => Define.TriggerCount;
        public override InputInfo GetInputInfo(string name) => Define.GetInputInfo(name);
        #endregion

        /// Бег (аналог спринта у персонажа)
        public static readonly InputTrigger Run = Define.Trigger(nameof(Run));
        
        //#no_need?: ///#PZ-6613:
        /**/ /// Ось поворота влево(>0)/вправо(<0) [-1,1]
        /**/ public static readonly InputAxis Rotation = Define.Axis(nameof(Rotation));

        /// Моб желает идти по пути
        public static readonly InputTrigger FollowPath = Define.Trigger(nameof(FollowPath));

        /// Jump to target point instruction. (`TargetPointX,Y,V` inputs are required)
        public static readonly InputTrigger JumpToTarget = Define.Trigger(nameof(JumpToTarget));

        /// Teleport to TargetPoint
        public static readonly InputTrigger Teleport = Define.Trigger(nameof(Teleport));
        
        /// Coordinates of target point (f.e. for jump-to-point and teleport)
        public static readonly InputAxis TargetPointX = Define.Axis(nameof(TargetPointX));
        public static readonly InputAxis TargetPointY = Define.Axis(nameof(TargetPointY));
        public static readonly InputAxis TargetPointV = Define.Axis(nameof(TargetPointV));
        public static readonly InputAxes TargetPointXY = Define.Axes(TargetPointX, TargetPointY);
        
        public static readonly InputAxis SpeedFactor = Define.Axis(nameof(SpeedFactor));
    }
}