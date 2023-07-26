namespace Src.Locomotion
{
    public sealed class CharacterInputs : CommonInputs
    {
        #region Registry        
        private new static readonly Registry Define = new Registry(nameof(CharacterInputs), CommonInputs.Define); 
        public override int AxisCount => Define.AxisCount;        
        public override int TriggerCount => Define.TriggerCount;
        public override InputInfo GetInputInfo(string name) => Define.GetInputInfo(name);
        #endregion
        
        /// Режим движения в блоке
        public static readonly InputTrigger Block = Define.Trigger(nameof(Block));  
        /// Режим движения при прицеливании
        public static readonly InputTrigger Aim = Define.Trigger(nameof(Aim));    
        /// Спринт
        public static readonly InputTrigger Sprint = Define.Trigger(nameof(Sprint));  
        /// Режим атаки с воздуха
        public static readonly InputTrigger AirborneAttack = Define.Trigger(nameof(AirborneAttack)); 
        /// 
        public static readonly InputTrigger CheatMode = Define.Trigger(nameof(CheatMode));
        /// 
        public static readonly InputAxis CheatSpeed = Define.Axis(nameof(CheatSpeed));        
        ///
        public static readonly InputTrigger Sticking = Define.Trigger(nameof(Sticking)); 
    }
}