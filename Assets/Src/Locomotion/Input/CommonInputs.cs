namespace Src.Locomotion
{
    public abstract class CommonInputs : Inputs
    {
        #region Registry        
        protected static readonly Registry Define = new Registry(nameof(CommonInputs));
        #endregion
 
        /// X компонента вектора направления взгляда относительно которого рассчитывается направление движения
        public static readonly InputAxis  GuideX = Define.Axis(nameof(GuideX)); 
        /// Y компонента вектора направления взгляда относительно которого рассчитывается направление движения
        public static readonly InputAxis GuideY = Define.Axis(nameof(GuideY));
        /// Вектор направления взгляда относительно которого рассчитывается направление движения
        public static readonly InputAxes Guide = Define.Axes(GuideX, GuideY);
        /// Ось движения вперёд/назад [-1,1]
        public static readonly InputAxis MoveLng = Define.Axis(nameof(MoveLng));
        /// Ось движения влево/вправо [-1,1]
        public static readonly InputAxis MoveLat = Define.Axis(nameof(MoveLat));
        /// Оси движения по горизонтали 
        public static readonly InputAxes Move = Define.Axes(MoveLng, MoveLat);
        /// Ось движения вверх/вниз [-1,1]
        public static readonly InputAxis MoveVertical = Define.Axis(nameof(MoveVertical));
        /// Прыжок
        public static readonly InputTrigger Jump = Define.Trigger(nameof(Jump));
        /// Уворот
        public static readonly InputTrigger Dodge = Define.Trigger(nameof(Dodge));  
        
        /// Режим прямого указания текущей скорости перемещения
        public static readonly InputTrigger Direct = Define.Trigger(nameof(Direct)); 
        /// Скорость перемещения в мировой СК для режима Direct
        public static readonly InputAxis DirectVelocityX = Define.Axis(nameof(DirectVelocityX)); 
        /// Скорость перемещения в мировой СК для режима Direct
        public static readonly InputAxis DirectVelocityY = Define.Axis(nameof(DirectVelocityY));
        /// Скорость режим Direct 
        public static readonly InputAxes DirectVelocity = Define.Axes(DirectVelocityX, DirectVelocityY);
        /// Скорость перемещения в мировой СК для режима Direct
        public static readonly InputAxis DirectVelocityVertical = Define.Axis(nameof(DirectVelocityVertical));
        /// Прямое задание ориентации для режима Direct
        public static readonly InputAxis DirectOrientation = Define.Axis(nameof(DirectOrientation));
    }
}